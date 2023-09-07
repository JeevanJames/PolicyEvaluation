namespace Jeevan.PolicyEvaluation;

internal sealed class PolicyEvaluatorOptions
{
    /// <summary>
    ///     Gets or sets a delegate that can be run during the tokenization phase to check whether
    ///     a given policy name exists.
    /// </summary>
    internal Func<string, object?, bool>? PolicyNameChecker { get; set; }

    /// <summary>
    ///     Gets or sets a delegate to log diagnostic details of policy evaluation.
    /// </summary>
    internal Action<string>? Logger { get; set; }

    internal static PolicyEvaluatorOptions Default { get; } = new();
}

public sealed class PolicyEvaluatorOptionsBuilder
{
    private readonly PolicyEvaluatorOptions _options = new();

    public PolicyEvaluatorOptionsBuilder LogWith(Action<string> logger)
    {
        _options.Logger = logger;
        return this;
    }

    public PolicyEvaluatorOptionsBuilder CheckPolicyNameWith(Func<string, bool> checker)
    {
        _options.PolicyNameChecker = (policyName, _) => checker(policyName);
        return this;
    }

    public PolicyEvaluatorOptionsBuilder CheckPolicyNameWith<TState>(Func<string, TState, bool> checker)
    {
        _options.PolicyNameChecker = (policyName, state) => state is TState s
            ? checker(policyName, s)
            : throw new PolicyEvaluatorException($"Cannot convert state to type {typeof(TState)}.");
        return this;
    }

    internal PolicyEvaluatorOptions Build() => _options;
}
