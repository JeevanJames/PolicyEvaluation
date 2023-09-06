namespace Jeevan.PolicyEvaluation.UnitTests;

public sealed class WithCustomDataTests : PolicyEvaluatorTests,
    IClassFixture<WithCustomDataTests.WithCustomDataFixture>
{
    public WithCustomDataTests(ITestOutputHelper output, WithCustomDataFixture fixture)
        : base(output, fixture)
    {
    }

    public sealed class WithCustomDataFixture : PolicyEvaluatorFixture
    {
        private readonly PolicyEvaluatorHelper _helper = new();

        public override PolicyEvaluator CreateEvaluator(ITestOutputHelper logger)
        {
            return new PolicyEvaluator(_helper.EvaluatePolicy,
                new PolicyEvaluatorOptions
                {
                    PolicyNameChecker =
                        static (name, state) => state is PolicyEvaluatorHelper h && h.IsValidPolicyName(name),
                    Logger = logger.WriteLine,
                });
        }

        public override object CustomData => _helper;
    }
}

internal sealed class PolicyEvaluatorHelper
{
    internal IPolicyOutcome EvaluatePolicy(string policyName)
    {
        if (policyName.StartsWith("True", StringComparison.OrdinalIgnoreCase))
            return PolicyOutcome.Pass;
        if (policyName.StartsWith("False", StringComparison.OrdinalIgnoreCase))
            return PolicyOutcome.Fail("Failed");
        if (policyName.StartsWith("NA", StringComparison.OrdinalIgnoreCase))
            return PolicyOutcome.NotApplicable;
        return PolicyOutcome.InvalidPolicyName(policyName);
    }

    internal bool IsValidPolicyName(string policyName) => char.IsNumber(policyName[^1]);
}
