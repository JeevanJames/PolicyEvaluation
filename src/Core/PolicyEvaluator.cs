namespace Jeevan.PolicyEvaluation;

public sealed class PolicyEvaluator
{
    public PolicyEvaluator(Func<string, bool> evaluator)
    {
        Evaluator = evaluator ?? throw new ArgumentNullException(nameof(evaluator));
    }

    public Func<string, bool> Evaluator { get; set; }

    public bool EvaluateExpression(string expression)
    {
        PositionalToken[] tokens = Tokenize(expression).ToArray();
        Expression expr = CreateExpression(tokens);
        return expr.Evaluate(Evaluator);
    }

    private static Expression CreateExpression(PositionalToken[] tokens)
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
                    outerTokens.Add(new Expression(nestedTokens.ToList()));
                    nestedTokens.Clear();
                    continue;
            }

            if (nested)
                nestedTokens.Add(token);
            else
                outerTokens.Add(token);
        }

        return new Expression(outerTokens);
    }

    private static IEnumerable<PositionalToken> Tokenize(string expression)
    {
        // Tracking variables
        PositionalToken token = StartToken.Default;
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
                                throw new ExpressionSyntaxErrorException(position, "Invalid place for a policy name.");
                            PolicyNameToken pnToken = new(position, identifier);
                            yield return pnToken;
                            token = pnToken;
                            break;
                    }

                    position += identifier.Length;
                    break;
            }
        }

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

public enum PolicyOutcome
{
    Pass,
    Fail,
    Indeterminate,
    InvalidPolicyName,
}
