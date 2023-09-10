namespace Jeevan.PolicyEvaluation.UnitTests;

public sealed class WithoutCustomDataTests : PolicyEvaluatorTests<object>,
    IClassFixture<WithoutCustomDataTests.WithoutCustomDataFixture>
{
    public WithoutCustomDataTests(ITestOutputHelper output, WithoutCustomDataFixture fixture)
        : base(output, fixture)
    {
    }

    public sealed class WithoutCustomDataFixture : PolicyEvaluatorFixture<object>
    {
        public override PolicyEvaluator CreateEvaluator(ITestOutputHelper logger)
        {
            return new PolicyEvaluator(TestPolicyEvaluator, builder => builder
                .LogWith(logger.WriteLine)
                .CheckPolicyNameWith(PolicyEvaluatorStaticHelper.IsValidPolicyName));
        }

        private static IPolicyOutcome TestPolicyEvaluator(string policyName) =>
            PolicyEvaluatorStaticHelper.EvaluatePolicy(policyName);
    }
}
