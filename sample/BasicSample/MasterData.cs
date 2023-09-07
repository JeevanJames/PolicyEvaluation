namespace Jeevan.PolicyEvaluation.Sample.BasicSample;

internal sealed class MasterData
{
    internal Dictionary<string, string> Claims { get; } = new(StringComparer.OrdinalIgnoreCase)
    {
        ["name"] = "The Flash",
        ["userid"] = "flash",
        ["email"] = "flash@justice-league.com",
        ["age"] = "30",
    };

    internal Dictionary<string, PolicyExecutorFunc> Policies { get; } = new(StringComparer.OrdinalIgnoreCase)
    {
        ["JusticeLeagueMember"] = c => new(
            c["email"].EndsWith("@justice-league.com", StringComparison.OrdinalIgnoreCase),
            "The user must have a valid Justice League email ID."),
        ["BusinessHours"] = _ => new(
            DateTime.Now is { Hour: >= 8 and <= 17 },
            "Must be during business hours."),
        ["YoungEnoughToWorkMore"] = c => new(
            int.Parse(c["age"]) <= 30,
            "User must be young enough to work outside office hours (30 years or less)."),
    };
}

internal sealed record PolicyResult(bool Success, string ErrorMessage);

internal delegate PolicyResult PolicyExecutorFunc(Dictionary<string, string> claims);
