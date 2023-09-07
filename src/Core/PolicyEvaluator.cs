namespace Jeevan.PolicyEvaluation;

public sealed class PolicyEvaluator
{
    private readonly PolicyEvaluatorFunc _evaluator;
    private readonly PolicyEvaluatorOptions _options;

    public PolicyEvaluator(PolicyEvaluatorFunc evaluator, Action<PolicyEvaluatorOptionsBuilder>? optionsBuilder = null)
    {
        _evaluator = evaluator ?? throw new ArgumentNullException(nameof(evaluator));
        if (optionsBuilder is null)
            _options = PolicyEvaluatorOptions.Default;
        else
        {
            PolicyEvaluatorOptionsBuilder builder = new();
            optionsBuilder(builder);
            _options = builder.Build();
        }
    }

    public PolicyEvaluator(Func<string, IPolicyOutcome> evaluator,
        Action<PolicyEvaluatorOptionsBuilder>? optionsBuilder = null)
        : this((expr, _) => evaluator(expr), optionsBuilder)
    {
    }

    /// <summary>
    ///     Evaluates a logic policy expression and returns whether it succeeds or fails.
    ///     <br/>
    ///     If the final policy outcome of the expression is <see cref="PolicyOutcome.NotApplicable"/>,
    ///     then the expression will be considered to have passed.
    /// </summary>
    /// <param name="expression">The logical policy expression to evaluate.</param>
    /// <param name="state">
    ///     Optional custom data that is passed to the policy evaluation delegate and can be used to
    ///     evaluate the policies.
    /// </param>
    /// <returns>
    ///     An instance of <see cref="PassOutcome"/> if the expression evaluation is satisfied, otherwise
    ///     an <see cref="FailOutcome"/> instance.
    ///     <br/>
    ///     Use the <see cref="IExpressionEvaluationOutcome.IsSatisfied"/> property to check of the
    ///     result indicates a satisfied outcome.
    ///     <br/>
    ///     Alternatively, you can use the <see cref="IExpressionEvaluationOutcome.IsNotSatisfied"/>
    ///     method to check if the expression was not satisfied and get the message related to the
    ///     failure.
    /// </returns>
    /// <exception cref="PolicyEvaluatorException">
    ///     Thrown on any error during evaluation except for syntax errors.
    /// </exception>
    /// <exception cref="ExpressionSyntaxErrorException">
    ///     Thrown if the expression parsing results in a syntax error.
    /// </exception>
    public IExpressionEvaluationOutcome EvaluateExpression(string expression, object? state = null)
    {
        ArgumentNullException.ThrowIfNull(expression);

        if (expression.Length == 0)
            throw new ExpressionSyntaxErrorException(0, "Expression is empty.");

        // Parse the expression into a collection of positional tokens.
        // This method throws syntax exceptions for any issue during parsing.
        PositionalToken[] tokens = Tokenize(expression, state).ToArray();

        // Create an expression from the tokens, which is a collection of tokens at the same level.
        // All nested tokens are grouped together as expressions.
        // An expression is also created for the overall set of tokens and returned.
        Expression expr = CreateExpression(tokens);

        // Evaluate the outer-most expression and return the result.
        // As part this call, inner expressions and policy tokens are also evaluated.
        IPolicyOutcome finalOutcome = expr.Evaluate(_evaluator, state);

        return finalOutcome switch
        {
            PassOutcome or NotApplicableOutcome => default(PassOutcome),
            FailOutcome fo => fo,
            _ => throw new PolicyEvaluatorException($"Invalid policy outcome type - {finalOutcome.GetType()}."),
        };
    }

    private Expression CreateExpression(PositionalToken[] tokens)
    {
        List<Token> outerTokens = new(tokens.Length);
        List<Token> nestedTokens = new();
        bool nested = false;
        foreach (PositionalToken token in tokens)
        {
            switch (token)
            {
                case OpenParenthesisToken:
                    nested = true;
                    continue;
                case CloseParenthesisToken:
                    nested = false;
                    outerTokens.Add(new Expression(nestedTokens.ToList(), _options));
                    nestedTokens.Clear();
                    continue;
            }

            if (nested)
                nestedTokens.Add(token);
            else
                outerTokens.Add(token);
        }

        return new Expression(outerTokens, _options);
    }

    // One-pass parser to tokenize the string expression.
    private IEnumerable<PositionalToken> Tokenize(string expression, object? state)
    {
        // Tracking variables
        PositionalToken token = StartToken.Default; // Current token
        bool nested = false;

        int position = 0;
        while (position < expression.Length)
        {
            switch (expression[position])
            {
                case ' ':
                    position++;
                    break;
                case '(' when token is not StartToken and not OperatorToken:
                    throw new ExpressionSyntaxErrorException(position, "Invalid place for an opening parenthesis.");
                case '(':
                    if (nested)
                        throw new ExpressionSyntaxErrorException(position, "Cannot nest more than one level.");
                    OpenParenthesisToken opToken = new(position);
                    yield return opToken;
                    token = opToken;
                    nested = true;
                    position++;
                    break;
                case ')' when token is not PolicyNameToken:
                    throw new ExpressionSyntaxErrorException(position, "Invalid place for a closing parenthesis.");
                case ')':
                    if (!nested)
                        throw new ExpressionSyntaxErrorException(position, "Cannot close a parenthesis that was not opened.");
                    CloseParenthesisToken cpToken = new(position);
                    yield return cpToken;
                    token = cpToken;
                    nested = false;
                    position++;
                    break;
                default:
                    int endIndex = position;
                    while (endIndex < expression.Length)
                    {
                        if (expression[endIndex] is ' ' or '(' or ')')
                            break;
                        endIndex++;
                    }

                    string identifier = expression[position..endIndex];

                    switch (identifier)
                    {
                        case "AND" when token is not PolicyNameToken and not CloseParenthesisToken:
                            throw new ExpressionSyntaxErrorException(position, "Invalid place for an AND operator.");
                        case "AND":
                            AndToken andToken = new(position);
                            yield return andToken;
                            token = andToken;
                            break;
                        case "OR" when token is not PolicyNameToken and not CloseParenthesisToken:
                            throw new ExpressionSyntaxErrorException(position, "Invalid place for an OR operator.");
                        case "OR":
                            OrToken orToken = new(position);
                            yield return orToken;
                            token = orToken;
                            break;
                        default:
                            if (!IsValidPolicyName(identifier))
                                throw new ExpressionSyntaxErrorException(position, $"Invalid policy name or unrecognized token in expression: {identifier}.");
                            if (token is not StartToken and not OperatorToken and not OpenParenthesisToken and not StartToken)
                                throw new ExpressionSyntaxErrorException(position, $"Invalid place for a policy name - {identifier}.");
                            if (_options.PolicyNameChecker is not null && !_options.PolicyNameChecker(identifier, state))
                                throw new ExpressionSyntaxErrorException(position, $"A policy named {identifier} does not exist.");
                            PolicyNameToken pnToken = new(position, identifier);
                            yield return pnToken;
                            token = pnToken;
                            break;
                    }

                    position += identifier.Length;
                    break;
            }
        }

        // The last token cannot be the start token. This would indicate that the expression is white-space.
        if (token is StartToken)
            throw new ExpressionSyntaxErrorException(0, "Expression is blank.");

        // The last token should be a policy name or a closing parenthesis.
        if (token is not PolicyNameToken and not CloseParenthesisToken)
            throw new ExpressionSyntaxErrorException(position, "Expression should end with a policy name or a closing parenthesis.");

        // If we're still nested, then the expression is not closed.
        if (nested)
            throw new ExpressionSyntaxErrorException(position, "Nested expression is not closed.");
    }

    private static bool IsValidPolicyName(string policyName)
    {
        ArgumentNullException.ThrowIfNull(policyName);

        if (policyName.Length == 0)
            return false;

        // Check if the first character is a valid start of an identifier
        if (!char.IsLetter(policyName[0]))
            return false;

        // Check the rest of the characters
        for (int i = 1; i < policyName.Length; i++)
        {
            if (!char.IsLetterOrDigit(policyName[i]) && policyName[i] != '_')
                return false;
        }

        return true;
    }
}
