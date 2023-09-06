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
                    PolicyNameChecker = (name, _) => PolicyEvaluatorStaticHelper.IsValidPolicyName(name),
                    Logger = logger.WriteLine,
                });
        }

        private static IPolicyOutcome TestPolicyEvaluator(string policyName, object? state) =>
            PolicyEvaluatorStaticHelper.EvaluatePolicy(policyName);
    }
}
