namespace NeverTest.Http;

using FluentAssertions.Json;
using Microsoft.Extensions.DependencyInjection;

public static  class BuilderExtensions
{
    public static ScenarioBuilder<TState> UseHttp<TState>(this ScenarioBuilder<TState> builder)
        where TState : class, IState
    {
        builder.Services.AddHttpClient();
        builder.Services.AddSingleton<JsonResponseDeserializer>();

        AddHttpCodeAssert("httpOk", 200);

        return builder
            .Acts
                .Register<Get>()
                .Register<GetJson>()
                .Register<Post>()
                .Register<PostJson>()
            .Builder;

        void AddHttpCodeAssert(string name, int statusCode)
        {
            var like = JObject.FromObject(new {StatusCode = statusCode});
            builder
                .Asserts
                .Register<BodyJson>()
                .Add((actual, _, ctx) => actual
                    .Should()
                    .ContainSubtree(like), o => o.Name = name);
        }
    }
}
