using Microsoft.VisualStudio.TestPlatform.TestHost;
using Newtonsoft.Json.Linq;

namespace NeverTest.AspNet.Tests;

public class CustomSet : AspNetScenarioSet<Program>
{
    public CustomSet(string set, LogLevel defaultLogLevel = LogLevel.Information) : base(set, defaultLogLevel)
    {
        Builder
            .Arranges
            .Add((token, context) =>
                {
                    context
                        .State()
                        .Configure(b =>
                        {
                            if (token is JObject obj)
                            {
                                foreach (var (key, value) in obj)
                                {
                                    b.UseSetting(key, value?.Value<string>());
                                }
                            }
                        });
                },
                o => o.Name = "my_cfg");
    }
}
