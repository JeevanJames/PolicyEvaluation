namespace Jeevan.PolicyEvaluation.UnitTests;

internal static class ShouldlyExtensions
{
    internal static void ShouldBe(this IExpressionEvaluationOutcome outcome, bool expectedResult)
    {
        if (expectedResult)
            outcome.ShouldBeOfType<PassOutcome>();
        else
            outcome.ShouldBeOfType<FailOutcome>();
    }
}
