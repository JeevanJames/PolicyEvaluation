using System.Diagnostics;

namespace Jeevan.PolicyEvaluation;

/// <summary>
///     A special token that contains a sequence of other tokens, forming a full expression.
///     <br/>
///     The reason why this class derives from <see cref="Token"/> and is not outside the hierarchy
///     is because one of the tokens it can contain is a nested expression.
/// </summary>
internal sealed record Expression : Token
{
    private readonly PolicyEvaluatorOptions _options;

    internal Expression(IList<Token> tokens, PolicyEvaluatorOptions options)
    {
        Tokens = tokens ?? throw new ArgumentNullException(nameof(tokens));
        _options = options;
    }

    internal IList<Token> Tokens { get; }

    internal IPolicyOutcome Evaluate(PolicyEvaluatorFunc evaluator, object? state)
    {
#pragma warning disable SA1011 // Closing square brackets should be spaced correctly
        // Special case: If there is only a single token, evaluate that token directly.
        switch (Tokens)
        {
            case [PolicyNameToken pnToken]:
                return evaluator(pnToken.Name, state);

            case [Expression expr]:
                return expr.Evaluate(evaluator, state);
        }
#pragma warning restore SA1011 // Closing square brackets should be spaced correctly

    restart: //TODO: Remove goto

        // Iterate tokens and resolve AND conditions first.
        // Start at index 1, because the tokenization will always ensure that the first token is not
        // an operator token.
        int index = 0;
        while (++index < Tokens.Count)
        {
            if (Tokens[index] is not AndToken)
                continue;

            IPolicyOutcome firstTokenOutcome = EvaluateToken(index - 1, evaluator, state);
            IPolicyOutcome secondTokenOutcome = EvaluateToken(index + 1, evaluator, state);
            IPolicyOutcome outcome = EvaluateLogicalCondition(firstTokenOutcome, secondTokenOutcome,
                isAndCondition: true);

            Tokens[index - 1] = new ResultToken(outcome);
            Tokens.RemoveAt(index);
            Tokens.RemoveAt(index);
            index--;

            _options.Logger?.Invoke(ToString());
        }

        // Iterate tokens and resolve OR conditions
        index = 0;
        while (++index < Tokens.Count)
        {
            if (Tokens[index] is not OrToken)
                continue;

            IPolicyOutcome firstTokenResult = EvaluateToken(index - 1, evaluator, state);
            IPolicyOutcome secondTokenResult = EvaluateToken(index + 1, evaluator, state);
            IPolicyOutcome result = EvaluateLogicalCondition(firstTokenResult, secondTokenResult,
                isAndCondition: false);

            // Short circuit: If any OR condition is true, the whole expression is true
            if (result == PolicyOutcome.Pass)
                return PolicyOutcome.Pass;

            Tokens[index - 1] = new ResultToken(result);
            Tokens.RemoveAt(index);
            Tokens.RemoveAt(index);

            _options.Logger?.Invoke(ToString());
        }

        if (Tokens is [ResultToken rt])
            return rt.Outcome;

#pragma warning disable S907 // "goto" statement should not be used
        if (Tokens.Count > 1)
            goto restart;
#pragma warning restore S907 // "goto" statement should not be used

        throw new PolicyEvaluatorException("Could not evaluate the expression.");
    }

    /// <summary>
    ///     Resolves the value of a policy, given its corresponding token. Only the following token
    ///     types are supported:
    ///     <br/>o <see cref="PolicyNameToken"/>: A policy that has not yet been evaluated.
    ///     <br/>o <see cref="Expression"/>: A nested expression.
    ///     <br/>o <see cref="ResultToken"/>: A policy whose result has already been evaluated.
    ///     <para/>
    ///     In case of a <see cref="PolicyNameToken" />, the method will look for other policy name
    ///     tokens with the same name in the expression and nested expressions and replace them with
    ///     the result of this policy evaluation. This way, the specific policy is only evaluated once.
    /// </summary>
    /// <param name="index">The index of the token in the <see cref="Tokens"/> collection.</param>
    /// <param name="evaluator">The policy evaluation function.</param>
    /// <param name="state">Optional custom state that can be passed into the evalator delegate.</param>
    /// <returns>The result of the expression evaluation.</returns>
    /// <exception cref="PolicyEvaluatorException">Thrown if an unexpected token is encountered.</exception>
    private IPolicyOutcome EvaluateToken(int index, PolicyEvaluatorFunc evaluator, object? state)
    {
        switch (Tokens[index])
        {
            case Expression e:
                return e.Evaluate(evaluator, state);

            case ResultToken r:
                return r.Outcome;

            case PolicyNameToken p:
                // This is the place where we actually evaluate a PolicyNameToken.
                // We want to handle any exceptions and invalid outcomes such as InvalidPolicyNameOutcome
                // and ErrorOutcome and ensure that we only return PassOutcome, FailOutcome or NotApplicableOutcome.
                try
                {
                    IPolicyOutcome outcome = evaluator(p.Name, state);
                    switch (outcome)
                    {
                        case InvalidPolicyNameOutcome:
                            throw new ExpressionSyntaxErrorException(p.Position,
                                $"A policy named {p.Name} does not exist.");
                        case ErrorOutcome eo:
                            throw new ExpressionSyntaxErrorException(p.Position, $"""
                                An unexpected error occurred when evaluating policy {p.Name}. Details below:
                                {eo.ErrorMessage}
                                """);
                        default:
                            Debug.Assert(outcome is PassOutcome or FailOutcome or NotApplicableOutcome);
                            AssignOutcomeForPolicy(p.Name, outcome, index);
                            return outcome;
                    }
                }
                catch (PolicyEvaluatorException)
                {
                    throw;
                }
                catch (Exception ex)
                {
                    throw new ExpressionSyntaxErrorException(p.Position, $"""
                        Unhandled exception when evaluating policy {p.Name}.
                        See inner exception for details.
                        """, ex);
                }

            default:
                throw new PolicyEvaluatorException($"Invalid token type {Tokens[index].GetType()}");
        }
    }

    private static IPolicyOutcome EvaluateLogicalCondition(IPolicyOutcome outcome1, IPolicyOutcome outcome2,
        bool isAndCondition)
    {
        Debug.Assert(outcome1 is not InvalidPolicyNameOutcome && outcome2 is not InvalidPolicyNameOutcome);

        if (outcome1 is NotApplicableOutcome && outcome2 is NotApplicableOutcome)
            return PolicyOutcome.NotApplicable;
        if (outcome1 is NotApplicableOutcome)
            return outcome2;
        if (outcome2 is NotApplicableOutcome)
            return outcome1;

        if (isAndCondition)
        {
            if (outcome1 is PassOutcome && outcome2 is PassOutcome)
                return PolicyOutcome.Pass;
            if (outcome1 is FailOutcome fo1)
                return fo1;
            if (outcome2 is FailOutcome fo2)
                return fo2;
            throw new PolicyEvaluatorException("Invalid condition reached when evaluating AND logical condition.");
        }

        if (outcome1 is PassOutcome || outcome2 is PassOutcome)
            return PolicyOutcome.Pass;
        if (outcome1 is FailOutcome fo && outcome2 is FailOutcome)
            return fo; // We return the first fail outcome only
        throw new PolicyEvaluatorException("Invalid condition reached when evaluating OR logical condition.");
    }

    /// <summary>
    ///     For all <see cref="PolicyNameToken"/> tokens in this expression, replace with a <see cref="ResultToken"/>
    ///     containing the specified <paramref name="outcome"/>.
    /// </summary>
    private void AssignOutcomeForPolicy(string policyName, IPolicyOutcome outcome, int avoidAtIndex = -1)
    {
        for (int i = 0; i < Tokens.Count; i++)
        {
            if (i == avoidAtIndex)
                continue;

            switch (Tokens[i])
            {
                case PolicyNameToken pnToken when pnToken.Name.Equals(policyName, StringComparison.OrdinalIgnoreCase):
                    Tokens[i] = new ResultToken(outcome);
                    break;
                case Expression expr:
                    expr.AssignOutcomeForPolicy(policyName, outcome);
                    break;
            }
        }
    }

    public override string ToString() =>
        string.Join(' ', Tokens.Select(t => t is Expression ? $"({t})" : t.ToString()));
}
