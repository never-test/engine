namespace NeverTest;

/// <summary>
/// Represents context of currently running scenario. 
/// </summary>
public interface IScenarioContext
{
    /// <summary>
    /// Returns instance of the configured engine for current scenario.
    /// </summary>
    ScenarioEngine ScenarioEngine { get; init; }

    /// <summary>
    /// Gets scenario instance.  
    /// </summary>
    Scenario Scenario { get; init; }

    /// <summary>
    /// Gets scenario logger. 
    /// </summary>
    public ILogger Log { get; }

    /// <summary>
    /// Gets string that represents current indentation. 
    /// </summary>
    public string Indent { get; }
}
