namespace Jeevan.PolicyEvaluation;

/// <summary>
///     Delegate to evaluate policies.
/// </summary>
/// <param name="policyName">The name of the policy to evaluate.</param>
/// <param name="state">
///     Optional custom data that can be used to evaluate the policy.
///     <br/>
///     By passing this data into the delegate instead of expecting it to be accessed from outside
///     the delegate's scope, we can prevent unnecessary closures.
/// </param>
/// <returns>The outcome of the policy evaluation.</returns>
public delegate PolicyOutcome PolicyEvaluatorFunc(string policyName, object? state);
