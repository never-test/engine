namespace NeverTest;
using NeverTest.Acts;

public static class ScenarioBuilderExtensions
{
    public static ScenarioBuilder<IState> UseDefaultEngine(this ScenarioBuilder<IState> builder) 
    {
        builder.AddAct<Ping>("ping")
            .AddAct<Echo>("echo")
            .AddAct<Repeat, JObject>("repeat");
        
        return builder;
    }
}