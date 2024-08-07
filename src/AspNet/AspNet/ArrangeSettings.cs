using NeverTest.Arranges;

namespace NeverTest.AspNet;

[Arrange("app_settings")]
internal class ArrangeSettings<TEntryPoint>: IArrangeStep<ApplicationSettings, AppState<TEntryPoint>> where TEntryPoint : class
{
    internal static readonly ArrangeSettings<TEntryPoint> s_instance = new();
    public Task Arrange(ApplicationSettings options, IArrangeScenarioContext<AppState<TEntryPoint>> context)
    {
            context.State().Configure(builder =>
            {
                foreach (var (key, value) in options)
                {
                    builder.UseSetting(key, value);
                }
            });

            return Task.CompletedTask;
    }
}

public class ApplicationSettings : Dictionary<string, string>;
