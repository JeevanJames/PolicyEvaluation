using System.Runtime.Serialization;

namespace Jeevan.PolicyEvaluation;

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

    public PolicyEvaluatorException(string message, Exception innerException)
        : base(message, innerException)
    {
    }
}
