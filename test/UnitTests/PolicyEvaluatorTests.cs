namespace Jeevan.PolicyEvaluation.UnitTests;

public abstract class PolicyEvaluatorTests<TState>
    where TState : class
{
    private readonly ITestOutputHelper _output;
    private readonly PolicyEvaluatorFixture<TState> _fixture;
    private readonly PolicyEvaluator<TState> _evaluator;

    protected PolicyEvaluatorTests(ITestOutputHelper output, PolicyEvaluatorFixture<TState> fixture)
    {
        _output = output;
        _fixture = fixture;
        _evaluator = fixture.CreateEvaluator(output);
    }

    [Fact]
    public void NullExpressionTest()
    {
        Should.Throw<ArgumentNullException>(() => _evaluator.EvaluateExpression(null!, _fixture.CustomData));
    }

    [Theory]
    [InlineData("True1", true)]
    [InlineData("False1", false)]
    [InlineData("(True1)", true)]
    [InlineData("(False1)", false)]
    [InlineData("NA1", true)]
    [InlineData("(NA1)", true)]
    public void SimpleExpressionTests(string expression, bool expectedResult)
    {
        _output.WriteLine(expression);
        IExpressionEvaluationOutcome outcome = _evaluator.EvaluateExpression(expression, _fixture.CustomData);
        outcome.ShouldBe(expectedResult);
    }

    [Theory]
    [InlineData(1, "True1 OR False2 AND True3", true)]
    [InlineData(2, "False1 AND True2 OR True3", true)]
    [InlineData(3, "False1 OR True2 OR False3 AND True4", true)]
    [InlineData(4, "True1 AND False2 OR False3 AND True4", false)]
    [InlineData(5, "False1 OR True2 AND False3 OR True4", true)]
    [InlineData(6, "True1 AND False2 AND True3 OR False4", false)]
    [InlineData(7, "(True1 OR False2) AND (True3 OR False4)", true)]
    [InlineData(8, "(False1 AND True2) OR (False3 AND True4)", false)]
    [InlineData(9, "True1 AND False2 OR False3 AND True4 OR False5", false)]
    [InlineData(10, "False1 OR True2 AND False3 OR True4 AND True5", true)]
    [InlineData(11, "(False1 OR True2) AND False3 OR False4 AND (True5 OR False6)", false)]
    [InlineData(12, "(True1 AND False2) OR (True3 AND False4) AND (False5 OR True6)", false)]
    [InlineData(13, "False1 AND (True2 OR False3) AND (True4 OR False5 OR False6)", false)]
    [InlineData(14, "True1 OR (False2 AND False3 OR True4) AND False5", true)]
    [InlineData(15, "(False1 OR True2) AND (False3 OR True4) AND (True5 OR False6)", true)]
    [InlineData(16, "(True1 AND False2) OR (False3 OR True4) OR (True5 AND False6)", true)]
    [InlineData(17, "False1 AND True2 OR False3 OR (True4 AND True5) OR False6", true)]
    [InlineData(18, "True1 OR False2 AND True3 AND (False4 OR True5) AND False6", true)]
    [InlineData(19, "(True1 AND False2 OR True3) AND (False4 OR False5 AND True6)", false)]
    [InlineData(20, "(False1 OR True2 AND False3) OR (True4 OR False5 AND False6)", true)]
    [InlineData(21, "True1 AND True2 AND False3 OR False4 AND True5 AND False6", false)]
    [InlineData(22, "False1 OR False2 OR True3 AND (False4 OR True5 OR True6)", true)]
    [InlineData(23, "(True1 AND False2 OR False3) AND (True4 OR False5 OR True6)", false)]
    [InlineData(24, "(False1 OR True2 AND False3) OR (True4 AND False5 AND True6)", false)]
    [InlineData(25, "(False1 AND True2 OR True3 OR False4) AND False5 AND True6", false)]
    [InlineData(26, "(True1 OR False2 AND True3 AND False4) OR True5 OR False6", true)]
    [InlineData(27, "(False1 AND True2 OR False3 OR True4) AND False5 OR True6", true)]
    [InlineData(28, "(True1 OR False2 AND True3 AND False4) AND True5 AND False6", false)]
    [InlineData(29, "True1 OR True1 AND False2", true)]
    [InlineData(30, "False1 AND True2 OR False1", false)]
    [InlineData(31, "False1 OR False2 OR True3 AND True3", true)]
    [InlineData(32, "True1 AND False2 OR False2 AND True1", false)]
    [InlineData(33, "True1 OR True2 AND True2 OR True3", true)]
    [InlineData(34, "True1 OR (True1 AND False2)", true)]
    [InlineData(35, "(False1 AND True2) OR False1", false)]
    [InlineData(36, "False1 OR (False2 OR True3) AND True3", true)]
    [InlineData(37, "True1 AND (False2 OR False2) AND True1", false)]
    [InlineData(38, "True1 OR (True2 AND True2) OR True3", true)]
    [InlineData(39, "NA1 AND NA2 AND NA3", true)]
    [InlineData(40, "NA1 OR NA2 OR NA3", true)]
    [InlineData(41, "NA1 AND NA2 OR NA3", true)]
    [InlineData(42, "NA1 OR NA2 AND NA3", true)]
    public void ComplexExpressionTests(int index, string expression, bool expectedResult)
    {
        _output.WriteLine($"{index}: {expression}");
        IExpressionEvaluationOutcome outcome = _evaluator.EvaluateExpression(expression, _fixture.CustomData);
        outcome.ShouldBe(expectedResult);
    }

    [Theory]
    [InlineData(1, "True1 AND& False2 OR True3", 6)]
    [InlineData(2, "False1 OR True2 && False3", 16)]
    [InlineData(3, "True1 OR False2 (AND True3)", 16)]
    [InlineData(4, "False1 OR True2 OR False3 AND", 29)]
    [InlineData(5, "True1 OR OR False2 AND True3", 9)]
    [InlineData(6, "False1 OR True2 ) AND False3", 16)]
    [InlineData(7, "True1 AND (False2 AND True3", 27)]
    [InlineData(8, "False1 OR True2 OR AND False3", 19)]
    [InlineData(9, "(True1 OR False2 AND True3) AND", 31)]
    [InlineData(10, "False1 OR OR True2 AND False3", 10)]
    [InlineData(11, "True1 AND False2 OR True3)", 25)]
    [InlineData(12, "AND True1 OR False2", 0)]
    [InlineData(13, "AND", 0)]
    [InlineData(14, "OR", 0)]
    [InlineData(15, "True1 and False1", 6)]
    [InlineData(16, "True1 AND (False2 OR (True3 AND False4))", 21)]
    [InlineData(17, "(True1 AND)", 10)]
    [InlineData(18, "True1 AND False2 True3", 17)]
    [InlineData(19, "True1 AND Blah", 10)]
    [InlineData(20, "True1 OR Blah", 9)]
    [InlineData(21, "Blah AND False1", 0)]
    [InlineData(22, "Blah OR False1", 0)]
    [InlineData(23, "Blah1 OR True1 AND Blah2", 19)]
    [InlineData(24, "InvalidPolicyName", 0)]
    [InlineData(25, "True1 AND False_But_Invalid_Name", 10)]
    [InlineData(26, "True", 0)]
    [InlineData(27, "", 0)]
    [InlineData(28, "    ", 0)]
    [InlineData(29, "True1 AND False34", 10)]
    [InlineData(30, "False2 OR True35", 10)]
    public void ExpressionSyntaxErrorExceptionTests(int index, string expression, int expectedPosition)
    {
        _output.WriteLine($"{index}: {expression}");

        ExpressionSyntaxErrorException exception = Should.Throw<ExpressionSyntaxErrorException>(
            () => _evaluator.EvaluateExpression(expression, _fixture.CustomData));

        exception.Position.ShouldBeGreaterThanOrEqualTo(0);

        _output.WriteLine(expression);
        _output.WriteLine(new string(' ', exception.Position) + '^');
        _output.WriteLine(exception.Message);

        exception.Position.ShouldBe(expectedPosition);
    }

    [Theory]
    [InlineData("InvalidPolicyName1")]
    public void PolicyEvaluatorExceptionTests(string expression)
    {
        _output.WriteLine(expression);
        PolicyEvaluatorException exception = Should.Throw<PolicyEvaluatorException>(
            () => _evaluator.EvaluateExpression(expression, _fixture.CustomData));
        exception.ShouldBeOfType<PolicyEvaluatorException>();

        _output.WriteLine(exception.Message);
    }
}

internal static class PolicyEvaluatorStaticHelper
{
    internal static IPolicyOutcome EvaluatePolicy(string policyName)
    {
        if (policyName.EndsWith("34"))
            return PolicyOutcome.Error("Weird policy name! Why does it end with '34'?");
        if (policyName.EndsWith("35"))
            throw new InvalidOperationException("Policy names should not end with '35'!");
        if (policyName.StartsWith("True", StringComparison.OrdinalIgnoreCase))
            return PolicyOutcome.Pass;
        if (policyName.StartsWith("False", StringComparison.OrdinalIgnoreCase))
            return PolicyOutcome.Fail("Failed");
        if (policyName.StartsWith("NA", StringComparison.OrdinalIgnoreCase))
            return PolicyOutcome.NotApplicable;
        return PolicyOutcome.InvalidPolicyName(policyName);
    }

    internal static bool IsValidPolicyName(string policyName) => char.IsNumber(policyName[^1]);
}
