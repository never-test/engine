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
        AddHttpCodeAssert("httpCreated", 201);
        AddHttpCodeAssert("httpAccepted", 202);
        AddHttpCodeAssert("httpNoContent", 204);
        AddHttpCodeAssert("httpBadRequest", 400);
        AddHttpCodeAssert("httpUnauthorized", 401);
        AddHttpCodeAssert("httpForbidden", 403);
        AddHttpCodeAssert("httpNotFound", 404);
        AddHttpCodeAssert("httpMethodNotAllowed", 405);
        AddHttpCodeAssert("httpNotAcceptable", 406);
        AddHttpCodeAssert("httpConflict", 409);
        AddHttpCodeAssert("httpTooManyRequests", 429);
        AddHttpCodeAssert("httpInternalServerError", 500);

        return builder.
            Asserts
                .Register<BodyJson>()
                .Register<BodyString>()
            .Builder
            .Acts
                .Register<Get>()
                .Register<GetJson>()
                .Register<Post>()
                .Register<PostJson>()
                .Register<Delete>()
                .Register<Put>()
                .Register<PutJson>()
                .Register<Patch>()
                .Register<PatchJson>()

            .Builder;

        void AddHttpCodeAssert(string name, int statusCode)
        {
            var like = JObject.FromObject(new {StatusCode = statusCode});
            builder
                .Asserts
                .Add((actual, _, ctx) => actual
                    .Should()
                    .ContainSubtree(like), o => o.Name = name);
        }
    }
}
