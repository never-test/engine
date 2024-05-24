using FluentAssertions.Execution;

namespace NeverTest;

public class SetResult
{
    public required IReadOnlyCollection<ScenarioResult> ScenarioResults { get; init; }
    public required TimeSpan Duration { get; set; }

    public void Assert()
    {
        using (new AssertionScope())
        {
            foreach (var result in ScenarioResults)
            {
                result.Assert();
            }
        }
    }
}
