namespace Jeevan.PolicyEvaluation.UnitTests;

public abstract class PolicyEvaluatorFixture
{
    public abstract PolicyEvaluator CreateEvaluator(ITestOutputHelper logger);

    public virtual object? CustomData => null;
}
