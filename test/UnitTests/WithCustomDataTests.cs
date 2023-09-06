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
    internal IPolicyOutcome EvaluatePolicy(string policyName) =>
        PolicyEvaluatorStaticHelper.EvaluatePolicy(policyName);

    internal bool IsValidPolicyName(string policyName) =>
        PolicyEvaluatorStaticHelper.IsValidPolicyName(policyName);
}
