using System.Runtime.Serialization;

namespace Jeevan.PolicyEvaluation;

[Serializable]
public class PolicyEvaluatorException : Exception
{
    /// <summary>
    ///     Initializes a new instance of the <see cref="PolicyEvaluatorException" /> class with a
    ///     specified error message.
    /// </summary>
    /// <param name="message">The message that describes the error.</param>
    public PolicyEvaluatorException(string message)
        : base(message)
    {
    }

    /// <summary>
    ///     Initializes a new instance of the <see cref="PolicyEvaluatorException" /> class with serialized
    ///     data.
    /// </summary>
    /// <param name="info">
    ///     The SerializationInfo that holds the serialized object data about the exception being thrown.
    /// </param>
    /// <param name="context">
    ///     The StreamingContext that contains contextual information about the source or destination.
    /// </param>
    protected PolicyEvaluatorException(SerializationInfo info, StreamingContext context)
        : base(info, context)
    {
    }
}
