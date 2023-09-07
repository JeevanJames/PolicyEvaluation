using Jeevan.PolicyEvaluation;
using Jeevan.PolicyEvaluation.Sample.BasicSample;

const string expression = "(BusinessHours OR YoungEnoughToWorkMore) AND JusticeLeagueMember";

MasterData masterData = new();

PolicyEvaluator pe = new(EvaluatePolicy, builder => builder
    .CheckPolicyNameWith<MasterData>((name, md) => md.Policies.ContainsKey(name)));

IExpressionEvaluationOutcome outcome = pe.EvaluateExpression(expression, masterData);
if (outcome.IsNotSatisfied(out string? policyName, out string? failureMessage))
    Console.WriteLine($"Policy {policyName} failed with message... {failureMessage}");
else
    Console.WriteLine("Passed");

static IPolicyOutcome EvaluatePolicy(string policyName, object? state)
{
    if (state is not MasterData md)
        return PolicyOutcome.Error("Invalid state");

    if (!md.Policies.TryGetValue(policyName, out PolicyExecutorFunc? policy))
        return PolicyOutcome.InvalidPolicyName(policyName);

    (bool result, string message) = policy(md.Claims);
    return result ? PolicyOutcome.Pass : PolicyOutcome.Fail(policyName, message);
}
