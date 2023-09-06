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
    /// <param name="policyName">The name of the policy that failed.</param>
    /// <param name="failureMessage">The optional failure message.</param>
    /// <returns>An <see cref="IPolicyOutcome"/> instance.</returns>
    public static IPolicyOutcome Fail(string policyName, string? failureMessage = null) =>
        failureMessage is null ? new FailOutcome(policyName) : new FailOutcome(policyName, failureMessage);

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

    /// <summary>
    ///     Returns an outcome of a policy evaluation that indicates an unexpected error.
    /// </summary>
    /// <param name="errorMessage">The message describing the error.</param>
    /// <returns>An <see cref="PolicyOutcome"/> instance that indicates an unexpected error.</returns>
    public static IPolicyOutcome Error(string errorMessage) => new ErrorOutcome(errorMessage);
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

    public bool IsNotSatisfied([NotNullWhen(true)] out string? policyName, [NotNullWhen(true)] out string? message)
    {
        policyName = message = null;
        return false;
    }
}

public readonly record struct FailOutcome(string PolicyName, string FailureMessage) : IExpressionEvaluationOutcome, IPolicyOutcome
{
    public FailOutcome(string policyName)
        : this(policyName, "The policy failed, but no details are specified.")
    {
    }

    public bool IsSatisfied => false;

    public bool IsNotSatisfied([NotNullWhen(true)] out string? policyName, [NotNullWhen(true)] out string? message)
    {
        policyName = PolicyName;
        message = FailureMessage;
        return true;
    }
}

internal readonly record struct NotApplicableOutcome : IPolicyOutcome;

internal readonly record struct InvalidPolicyNameOutcome(string PolicyName) : IPolicyOutcome;

internal readonly record struct ErrorOutcome(string ErrorMessage) : IPolicyOutcome
{
    internal ErrorOutcome(Exception exception)
        : this(exception.Message)
    {
        ErrorDetails = exception.ToString();
    }

    internal string? ErrorDetails { get; }
}
