namespace Jeevan.PolicyEvaluation;

public sealed class PolicyEvaluator : PolicyEvaluator<object>
{
    public PolicyEvaluator(Func<string, IPolicyOutcome> evaluator,
        Action<PolicyEvaluatorOptionsBuilder>? optionsBuilder = null)
        : base((expr, _) => evaluator(expr), optionsBuilder)
    {
    }

    /// <summary>
    ///     Evaluates a logic policy expression and returns whether it succeeds or fails.
    ///     <br/>
    ///     If the final policy outcome of the expression is <see cref="PolicyOutcome.NotApplicable"/>,
    ///     then the expression will be considered to have passed.
    /// </summary>
    /// <param name="expression">The logical policy expression to evaluate.</param>
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
    public IExpressionEvaluationOutcome EvaluateExpression(string expression)
    {
        return base.EvaluateExpression(expression);
    }
}
