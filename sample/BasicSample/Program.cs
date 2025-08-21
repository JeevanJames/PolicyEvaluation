using Jeevan.PolicyEvaluation;
using Jeevan.PolicyEvaluation.Sample.BasicSample;

using static System.Console;

// Create an instance of MasterData.
// This consists of the user claims and policies.
MasterData masterData = new();

// Create an instance of PolicyEvaluator, specifying the delegate to evaluate the policies.
PolicyEvaluator<MasterData> pe = new(EvaluatePolicy, builder => builder
    .CheckPolicyNameWith<MasterData>((name, md) => md.Policies.ContainsKey(name)));

// The initial expression to evaluate.
string? expression = "(Business.Hours OR YoungEnough_To_WorkLate) AND Justice-LeagueMember";

string policies = string.Join(", ", masterData.Policies.Keys);

// Repeat asking the user for expressions to evaluate.
while (!string.IsNullOrEmpty(expression))
{
    WriteLine($"Evaluating expression... {expression}");

    try
    {
        IExpressionEvaluationOutcome outcome = pe.EvaluateExpression(expression, masterData);

        if (outcome.IsNotSatisfied(out string? policyName, out string? failureMessage))
        {
            ForegroundColor = ConsoleColor.Red;
            WriteLine($"Policy {policyName} failed with message... {failureMessage}");
        }
        else
        {
            ForegroundColor = ConsoleColor.Green;
            WriteLine("Passed");
        }

        ResetColor();
    }
    catch (ExpressionSyntaxErrorException syntaxError)
    {
        ForegroundColor = ConsoleColor.Red;
        WriteLine($"Syntax error: {syntaxError.Message}");
        WriteLine(expression);
        WriteLine(new string(' ', syntaxError.Position) + '^');
        ResetColor();
    }

    WriteLine();
    WriteLine("Available policies:");
    WriteLine(policies);
    WriteLine("Enter a new expression with these policies or ENTER to exit: ");
    expression = ReadLine();
}

static IPolicyOutcome EvaluatePolicy(string policyName, MasterData? md)
{
    ArgumentNullException.ThrowIfNull(md);

    if (!md.Policies.TryGetValue(policyName, out PolicyExecutorFunc? policy))
        return PolicyOutcome.InvalidPolicyName(policyName);

    (bool result, string message) = policy(md.IronMan);
    return result ? PolicyOutcome.Pass : PolicyOutcome.Fail(policyName, message);
}
