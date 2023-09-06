namespace Jeevan.PolicyEvaluation.UnitTests;

public sealed class WithoutCustomDataTests : PolicyEvaluatorTests,
    IClassFixture<WithoutCustomDataTests.WithoutCustomDataFixture>
{
    public WithoutCustomDataTests(ITestOutputHelper output, WithoutCustomDataFixture fixture)
        : base(output, fixture)
    {
    }

    public sealed class WithoutCustomDataFixture : PolicyEvaluatorFixture
    {
        public override PolicyEvaluator CreateEvaluator(ITestOutputHelper logger)
        {
            return new PolicyEvaluator(TestPolicyEvaluator,
                new PolicyEvaluatorOptions
                {
                    PolicyNameChecker = (name, _) => char.IsNumber(name[^1]),
                    Logger = logger.WriteLine,
                });
        }

        private static IPolicyOutcome TestPolicyEvaluator(string policyName, object? state)
        {
            if (policyName.StartsWith("True", StringComparison.OrdinalIgnoreCase))
                return PolicyOutcome.Pass;
            if (policyName.StartsWith("False", StringComparison.OrdinalIgnoreCase))
                return PolicyOutcome.Fail("Failed");
            if (policyName.StartsWith("NA", StringComparison.OrdinalIgnoreCase))
                return PolicyOutcome.NotApplicable;
            return PolicyOutcome.InvalidPolicyName(policyName);
        }
    }
}
