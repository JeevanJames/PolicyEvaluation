namespace Jeevan.PolicyEvaluation;

#pragma warning disable S2094 // Classes should not be empty
internal abstract record Token;
#pragma warning restore S2094 // Classes should not be empty

/// <summary>
///     Any token that is created from parsing the initial expression string. Such tokens will have
///     information about their position in the expression string, which will be used when throwing
///     exceptions from the parsing, making it useful for debugging.
/// </summary>
/// <param name="Position">The zero-based index at which this token appears in the expression string.</param>
internal abstract record PositionalToken(int Position) : Token;

internal sealed record StartToken() : PositionalToken(0)
{
    public static readonly StartToken Default = new();
}

internal abstract record OperatorToken(int Position) : PositionalToken(Position);

internal sealed record AndToken(int Position) : OperatorToken(Position)
{
    public override string ToString() => "AND";
}

internal sealed record OrToken(int Position) : OperatorToken(Position)
{
    public override string ToString() => "OR";
}

internal abstract record ParenthesisToken(int Position) : PositionalToken(Position);

internal sealed record OpenParenthesisToken(int Position) : ParenthesisToken(Position)
{
    public override string ToString() => "(";
}

internal sealed record CloseParenthesisToken(int Position) : ParenthesisToken(Position)
{
    public override string ToString() => ")";
}

internal sealed record PolicyNameToken(int Position, string Name) : PositionalToken(Position)
{
    public override string ToString() => Name;
}

/// <summary>
///     A special token that holds the result of evaluating a policy from a <see cref="PolicyNameToken"/>
///     token.
/// </summary>
/// <param name="Outcome">The result of the policy evaluation.</param>
internal sealed record ResultToken(PolicyOutcome Outcome) : Token
{
    public override string ToString() => Outcome.ToString();
}
