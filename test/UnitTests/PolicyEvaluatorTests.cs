namespace Jeevan.PolicyEvaluation.UnitTests;

public abstract class PolicyEvaluatorTests
{
    private readonly ITestOutputHelper _output;
    private readonly Fixture _fixture;
    private readonly PolicyEvaluator _evaluator;

    protected PolicyEvaluatorTests(ITestOutputHelper output, Fixture fixture)
    {
        _output = output;
        _fixture = fixture;
        _evaluator = fixture.CreateEvaluator(output);
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
        bool result = _evaluator.EvaluateExpression(expression, _fixture.CustomData);
        result.ShouldBe(expectedResult);
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
        _output.WriteLine($"{index} {expression}");
        bool result = _evaluator.EvaluateExpression(expression, _fixture.CustomData);
        result.ShouldBe(expectedResult);
    }

    [Theory]
    [InlineData("True1 AND& False2 OR True3")]
    [InlineData("False1 OR True2 && False3")]
    [InlineData("True1 OR False2 (AND True3)")]
    [InlineData("False1 OR True2 OR False3 AND")]
    [InlineData("True1 OR OR False2 AND True3")]
    [InlineData("False1 OR True2 ) AND False3")]
    [InlineData("True1 AND (False2 AND True3")]
    [InlineData("False1 OR True2 OR AND False3")]
    [InlineData("(True1 OR False2 AND True3) AND")]
    [InlineData("False1 OR OR True2 AND False3")]
    [InlineData("True1 AND False2 OR True3)")]
    [InlineData("AND True1 OR False2")]
    [InlineData("AND")]
    [InlineData("OR")]
    [InlineData("True1 and False1")]
    [InlineData("True1 AND (False2 OR (True3 AND False4))")]
    [InlineData("(True1 AND)")]
    [InlineData("True1 AND False2 True3")]
    [InlineData("True1 AND Blah")]
    [InlineData("True1 OR Blah")]
    [InlineData("Blah AND False1")]
    [InlineData("Blah OR False1")]
    [InlineData("Blah1 OR True1 AND Blah2")]
    [InlineData("InvalidPolicyName")]
    [InlineData("True1 AND False_But_Invalid_Name")]
    [InlineData("True")]
    public void ExpressionSyntaxErrorExceptionTests(string expression)
    {
        _output.WriteLine(expression);
        ExpressionSyntaxErrorException exception = Should.Throw<ExpressionSyntaxErrorException>(
            () => _evaluator.EvaluateExpression(expression, _fixture.CustomData));

        _output.WriteLine(new string(' ', exception.Position) + '^');
        _output.WriteLine(exception.Message);
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

public sealed class WithoutCustomData : PolicyEvaluatorTests, IClassFixture<WithoutCustomData.WithoutCustomDataFixture>
{
    public WithoutCustomData(ITestOutputHelper output, WithoutCustomDataFixture fixture)
        : base(output, fixture)
    {
    }

    public sealed class WithoutCustomDataFixture : Fixture
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

        private static PolicyOutcome TestPolicyEvaluator(string policyName, object? state)
        {
            if (policyName.StartsWith("True", StringComparison.OrdinalIgnoreCase))
                return PolicyOutcome.Pass;
            if (policyName.StartsWith("False", StringComparison.OrdinalIgnoreCase))
                return PolicyOutcome.Fail;
            if (policyName.StartsWith("NA", StringComparison.OrdinalIgnoreCase))
                return PolicyOutcome.NotApplicable;
            return PolicyOutcome.InvalidPolicyName;
        }
    }
}

public sealed class WithCustomData : PolicyEvaluatorTests, IClassFixture<WithCustomData.WithCustomDataFixture>
{
    public WithCustomData(ITestOutputHelper output, WithCustomDataFixture fixture)
        : base(output, fixture)
    {
    }

    public sealed class WithCustomDataFixture : Fixture
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

public sealed class PolicyEvaluatorHelper
{
    public PolicyOutcome EvaluatePolicy(string policyName)
    {
        if (policyName.StartsWith("True", StringComparison.OrdinalIgnoreCase))
            return PolicyOutcome.Pass;
        if (policyName.StartsWith("False", StringComparison.OrdinalIgnoreCase))
            return PolicyOutcome.Fail;
        if (policyName.StartsWith("NA", StringComparison.OrdinalIgnoreCase))
            return PolicyOutcome.NotApplicable;
        return PolicyOutcome.InvalidPolicyName;
    }

    public bool IsValidPolicyName(string policyName) => char.IsNumber(policyName[^1]);
}

public abstract class Fixture
{
    public abstract PolicyEvaluator CreateEvaluator(ITestOutputHelper logger);

    public virtual object? CustomData => null;
}
