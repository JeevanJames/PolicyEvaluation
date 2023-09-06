using System.Diagnostics.CodeAnalysis;

namespace Jeevan.PolicyEvaluation;

/// <summary>
///     The outcome of the evaluation of a logical expression of policies.
/// </summary>
public interface IExpressionEvaluationOutcome
{
    bool IsSatisfied { get; }

    bool IsNotSatisfied([NotNullWhen(true)] out string? policyName, [NotNullWhen(true)] out string? message);
}
