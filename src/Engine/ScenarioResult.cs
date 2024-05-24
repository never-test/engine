using FluentAssertions;

namespace NeverTest;

public class ScenarioResult
{
    public static ScenarioResult CreateInconclusive(string reason, Scenario scenario) => new()
    {
        Inconclusive = reason,
        Exception = null,
        Duration = TimeSpan.Zero,
        Logs = [],
        Scenario = scenario
    };

    public required Exception? Exception { get; init; }
    public required TimeSpan Duration { get; init; }
    public required IEnumerable<string> Logs { get; init; }
    public string? Inconclusive { get; init; }

    public required Scenario Scenario { get; init; }

    public string GetHeader() =>
        $"""
         Scenario: {Scenario.Name}
         Duration: {Duration}
         """;

    public void Assert()
    {
        Exception.Should().BeNull("scenario {0} was expected not to result in exception");
    }
}
