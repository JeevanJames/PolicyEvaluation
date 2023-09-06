using System.Diagnostics.CodeAnalysis;

namespace Jeevan.PolicyEvaluation;

public static class PolicyOutcome
{
    /// <summary>
    ///     Represents a successful policy evaluation outcome.
    /// </summary>
    public static readonly IPolicyOutcome Pass = default(PassOutcome);

    /// <summary>
    ///     Returns a failed policy evaluation outcome with an optional failure message.
    /// </summary>
    /// <param name="failureMessage">The optional failure message.</param>
    /// <returns>An <see cref="IPolicyOutcome"/> instance.</returns>
    public static IPolicyOutcome Fail(string? failureMessage = null) =>
        failureMessage is null ? new FailOutcome() : new FailOutcome(failureMessage);

    /// <summary>
    ///     Represents an outcome of a policy evaluation where the policy could not be executed.
    /// </summary>
    public static readonly IPolicyOutcome NotApplicable = default(NotApplicableOutcome);

    /// <summary>
    ///     Returns an outcome of a policy evaluation that indicates the policy name was not valid
    ///     and hence could not be evaluated.
    /// </summary>
    /// <param name="policyName">The invalid policy name.</param>
    /// <returns>An <see cref="IPolicyOutcome"/> instance that indicates an invalid policy name.</returns>
    public static IPolicyOutcome InvalidPolicyName(string policyName) => new InvalidPolicyNameOutcome(policyName);
}

/// <summary>
///     The outcome of the evaluation of a logical expression of policies.
/// </summary>
public interface IExpressionEvaluationOutcome
{
    bool IsSatisfied { get; }

    bool IsNotSatisfied([NotNullWhen(true)] out string? message);
}

/// <summary>
///     The outcome of the evaluation of a single policy.
/// </summary>
public interface IPolicyOutcome
{
}

public readonly record struct PassOutcome : IExpressionEvaluationOutcome, IPolicyOutcome
{
    public bool IsSatisfied => true;

    public bool IsNotSatisfied([NotNullWhen(true)] out string? message)
    {
        message = null;
        return false;
    }
}

public readonly record struct FailOutcome(string FailureMessage) : IExpressionEvaluationOutcome, IPolicyOutcome
{
    public FailOutcome()
        : this("The policy failed, but no details are specified.")
    {
    }

    public bool IsSatisfied => false;

    public bool IsNotSatisfied([NotNullWhen(true)] out string? message)
    {
        message = FailureMessage;
        return true;
    }
}

internal readonly record struct NotApplicableOutcome : IPolicyOutcome;

internal readonly record struct InvalidPolicyNameOutcome(string PolicyName) : IPolicyOutcome;
