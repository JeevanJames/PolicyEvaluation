namespace Jeevan.PolicyEvaluation;

/// <summary>
///     A special token that contains a sequence of other tokens, forming a full expression.
///     <br/>
///     The reason why this class derives from <see cref="Token"/> and is not outside the hierarchy
///     is because one of the tokens it can contain is a nested expression.
/// </summary>
internal sealed record Expression : Token
{
    internal Expression(IList<Token> tokens)
    {
        Tokens = tokens ?? throw new ArgumentNullException(nameof(tokens));
    }

    internal IList<Token> Tokens { get; }

    internal bool Evaluate(Func<string, bool> evaluator)
    {
        // Special case: If there is only a single token, evaluate that token directly.
#pragma warning disable SA1011 // Closing square brackets should be spaced correctly
        switch (Tokens)
        {
            case [PolicyNameToken pnToken]:
                return evaluator(pnToken.Name);
            case [Expression expr]:
                return expr.Evaluate(evaluator);
        }
#pragma warning restore SA1011 // Closing square brackets should be spaced correctly

    restart:

        // Iterate tokens and resolve AND conditions first.
        // Start at index 1, because the tokenization will always ensure that the first token is not
        // an operator token.
        int index = 0;
        while (++index < Tokens.Count)
        {
            if (Tokens[index] is not AndToken)
                continue;

            bool firstTokenResult = EvaluateToken(index - 1, evaluator);
            bool secondTokenResult = EvaluateToken(index + 1, evaluator);
            bool result = firstTokenResult && secondTokenResult;

            Tokens[index - 1] = new ResultToken(result);
            Tokens.RemoveAt(index);
            Tokens.RemoveAt(index);
            index--;

            Console.WriteLine(this);
        }

        // Iterate tokens and resolve OR conditions
        index = 0;
        while (++index < Tokens.Count)
        {
            if (Tokens[index] is not OrToken)
                continue;

            bool firstTokenResult = EvaluateToken(index - 1, evaluator);
            bool secondTokenResult = EvaluateToken(index + 1, evaluator);
            bool result = firstTokenResult || secondTokenResult;

            // Short circuit: If any OR condition is true, the whole expression is true
            if (result)
                return true;

            Tokens[index - 1] = new ResultToken(result);
            Tokens.RemoveAt(index);
            Tokens.RemoveAt(index);

            Console.WriteLine(this);
        }

        if (Tokens is [ResultToken rt])
            return rt.Result;

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
    /// <returns>The result of the expression evaluation.</returns>
    /// <exception cref="PolicyEvaluatorException">Thrown if an unexpected token is encountered.</exception>
    private bool EvaluateToken(int index, Func<string, bool> evaluator)
    {
        switch (Tokens[index])
        {
            case Expression e:
                return e.Evaluate(evaluator);

            case ResultToken r:
                return r.Result;

            case PolicyNameToken p:
                bool result = evaluator(p.Name);
                AssignResultForPolicyName(p.Name, result, index);
                return result;

            default:
                throw new PolicyEvaluatorException($"Invalid token type {Tokens[index].GetType()}");
        }
    }

    /// <summary>
    ///     For all <see cref="PolicyNameToken"/> tokens in this expression, replace with a <see cref="ResultToken"/>
    ///     containing the specified <paramref name="result"/>.
    /// </summary>
    private void AssignResultForPolicyName(string policyName, bool result, int avoidAtIndex = -1)
    {
        for (int i = 0; i < Tokens.Count; i++)
        {
            if (i == avoidAtIndex)
                continue;

            switch (Tokens[i])
            {
                case PolicyNameToken pnToken when pnToken.Name.Equals(policyName, StringComparison.OrdinalIgnoreCase):
                    Tokens[i] = new ResultToken(result);
                    break;
                case Expression expr:
                    expr.AssignResultForPolicyName(policyName, result);
                    break;
            }
        }
    }

    public override string ToString() =>
        string.Join(' ', Tokens.Select(t => t is Expression ? $"({t})" : t.ToString()));
}
