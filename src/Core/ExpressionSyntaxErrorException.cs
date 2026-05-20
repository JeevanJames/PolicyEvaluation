namespace Jeevan.PolicyEvaluation;

/// <summary>
///     Exception thrown when an expression is invalid due to syntax errors.
/// </summary>
public sealed class ExpressionSyntaxErrorException : PolicyEvaluatorException
{
    public ExpressionSyntaxErrorException(int position, string message)
        : base(message)
    {
        Position = position;
    }

    public ExpressionSyntaxErrorException(int position, string message, Exception innerException)
        : base(message, innerException)
    {
        Position = position;
    }

    /// <summary>
    ///     Gets the index of the syntax error in the expression.
    /// </summary>
    public int Position { get; }
}
