namespace NeverTest.Tests;

using NeverTest.Acts;
using Newtonsoft.Json.Linq;

public class MySet : ScenarioSetAttribute<MyCtx>
{
    public MySet(string set): base((set))
    {
        Builder
            .UsingYaml()
            .AddAct<Ping>("ping")
            .AddAct<Echo>("echo")
            .AddAct<Repeat, JObject>("repeat");
    }
}