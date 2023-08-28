namespace Jeevan.PolicyEvaluation;

public sealed class PolicyEvaluatorOptions
{
    /// <summary>
    ///     Gets or sets a delegate to log diagnostic details of policy evaluation.
    /// </summary>
    public Action<string>? Logger { get; set; }

    public static PolicyEvaluatorOptions Default { get; } = new();
}
