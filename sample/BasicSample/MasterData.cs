namespace Jeevan.PolicyEvaluation.Sample.BasicSample;

internal sealed class MasterData
{
    internal Dictionary<string, string> TheFlash { get; } = new(StringComparer.OrdinalIgnoreCase)
    {
        ["name"] = "The Flash",
        ["userid"] = "flash",
        ["email"] = "flash@justice-league.com",
        ["age"] = "30",
        ["actual_name"] = "Barry Allen",
    };

    internal Dictionary<string, string> IronMan { get; } = new(StringComparer.OrdinalIgnoreCase)
    {
        ["name"] = "IronMan",
        ["userid"] = "ironman",
        ["email"] = "ironman@avengers.com",
        ["age"] = "40",
        ["actual_name"] = "Tony Stark",
    };

    internal Dictionary<string, PolicyExecutorFunc> Policies { get; } = new(StringComparer.OrdinalIgnoreCase)
    {
        ["JusticeLeagueMember"] = c => (
            c["email"].EndsWith("@justice-league.com", StringComparison.OrdinalIgnoreCase),
            "The user must have a valid Justice League email ID."),
        ["AvengersMember"] = c => (
            c["Email"].EndsWith("@avengers.com", StringComparison.OrdinalIgnoreCase),
            "The user must have a valid Avengers email ID."),
        ["BusinessHours"] = _ => (
            DateTime.Now is { Hour: >= 8 and <= 18 },
            "Must be during business hours."),
        ["YoungEnoughToWorkLate"] = c => (
            int.Parse(c["age"]) <= 30,
            "User must be young enough to work outside office hours (30 years or less)."),
    };
}

internal delegate (bool Success, string ErrorMessage) PolicyExecutorFunc(Dictionary<string, string> claims);
