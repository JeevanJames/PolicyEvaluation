using System.Runtime.Serialization;

namespace Jeevan.PolicyEvaluation;

/// <summary>
///     Exception thrown when an expression is invalid due to syntax errors.
/// </summary>
[Serializable]
public sealed class ExpressionSyntaxErrorException : PolicyEvaluatorException
{
    public ExpressionSyntaxErrorException(int position, string message)
        : base(message)
    {
        Position = position;
    }

    private ExpressionSyntaxErrorException(SerializationInfo info, StreamingContext context)
        : base(info, context)
    {
        Position = info.GetInt32(nameof(Position));
    }

    /// <summary>
    ///     Gets the index of the syntax error in the expression.
    /// </summary>
    public int Position { get; }

    /// <inheritdoc />
    public override void GetObjectData(SerializationInfo info, StreamingContext context)
    {
        info.AddValue(nameof(Position), Position);
        base.GetObjectData(info, context);
    }
}
