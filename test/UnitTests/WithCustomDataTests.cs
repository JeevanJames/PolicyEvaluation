namespace Jeevan.PolicyEvaluation.UnitTests;

public sealed class WithCustomDataTests : PolicyEvaluatorTests<PolicyEvaluatorHelper>,
    IClassFixture<WithCustomDataTests.WithCustomDataFixture>
{
    public WithCustomDataTests(ITestOutputHelper output, WithCustomDataFixture fixture)
        : base(output, fixture)
    {
    }

    public sealed class WithCustomDataFixture : PolicyEvaluatorFixture<PolicyEvaluatorHelper>
    {
        private readonly PolicyEvaluatorHelper _helper = new();

        public override PolicyEvaluator<PolicyEvaluatorHelper> CreateEvaluator(ITestOutputHelper logger)
        {
            return new PolicyEvaluator<PolicyEvaluatorHelper>((name, state) => state!.EvaluatePolicy(name),
                builder => builder
                    .LogWith(logger.WriteLine)
                    .CheckPolicyNameWith<PolicyEvaluatorHelper>(static (name, state) => state.IsValidPolicyName(name)));
        }

        public override PolicyEvaluatorHelper? CustomData => _helper;
    }
}

public sealed class PolicyEvaluatorHelper
{
    public IPolicyOutcome EvaluatePolicy(string policyName) =>
        PolicyEvaluatorStaticHelper.EvaluatePolicy(policyName);

    public bool IsValidPolicyName(string policyName) =>
        PolicyEvaluatorStaticHelper.IsValidPolicyName(policyName);
}
