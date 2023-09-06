namespace Jeevan.PolicyEvaluation;

public sealed class PolicyEvaluatorOptions
{
    /// <summary>
    ///     Gets or sets a delegate that can be run during the tokenization phase to check whether
    ///     a given policy name exists.
    /// </summary>
    public Func<string, object?, bool>? PolicyNameChecker { get; set; }

    /// <summary>
    ///     Gets or sets a delegate to log diagnostic details of policy evaluation.
    /// </summary>
    public Action<string>? Logger { get; set; }

    public static PolicyEvaluatorOptions Default { get; } = new();
}
