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
#pragma warning disable S2325 // Methods and properties that don't access instance data should be static
    public IPolicyOutcome EvaluatePolicy(string policyName) =>
        PolicyEvaluatorStaticHelper.EvaluatePolicy(policyName);

    public bool IsValidPolicyName(string policyName) =>
        PolicyEvaluatorStaticHelper.IsValidPolicyName(policyName);
#pragma warning restore S2325 // Methods and properties that don't access instance data should be static
}
