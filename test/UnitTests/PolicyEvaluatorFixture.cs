namespace Jeevan.PolicyEvaluation.UnitTests;

public abstract class PolicyEvaluatorFixture<TState>
    where TState : class
{
    public abstract PolicyEvaluator<TState> CreateEvaluator(ITestOutputHelper logger);

    public virtual TState? CustomData => null;
}
