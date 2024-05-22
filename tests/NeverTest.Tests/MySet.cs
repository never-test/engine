using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace NeverTest.Tests;

using NeverTest.Acts;
using Newtonsoft.Json.Linq;

public class MySet : ScenarioSetAttribute<MyScenarioState>
{
    public MySet(string set): base((set))
    {
        Builder

            .UseYaml()
            .AddAct<Ping>("ping")
            .AddAct<Echo>("echo")
            .AddAct<Repeat, JObject>("repeat")
            .Services
            .AddLogging(x => x.SetMinimumLevel(LogLevel.Trace));
    }
}