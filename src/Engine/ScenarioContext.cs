using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace NeverTest;

using FluentAssertions.Json;
using Newtonsoft.Json;

using Asserts;
using Logging;

/// <summary>
/// Contains context for a single scenario in a set.
/// In particular, contains scoped service provider
/// where scope is created for a single scenario
/// </summary>

public record ScenarioContext<T> : IScenarioContext<T> where T : IState
{

    private ScenarioFrame _root = null!;
    private int _level = 0;
    private readonly Lazy<JsonSerializerSettings> _settingFactory;

    public T State() => StateInstance;
    public required ILogger Log { get; init; }
    public required IServiceProvider Provider { get; init; }
    public required ScenarioEngine Engine { get; init; }
    public required Scenario Scenario { get; init; }
    public required T StateInstance { get; init; }

    public string Indent => new(' ', Math.Max(Level - 1, 0) * 2);
    public int Level => _level;

    public JsonSerializerSettings JsonSerializerSettings => _settingFactory.Value;

    public ScenarioContext()
    {
        _settingFactory = new Lazy<JsonSerializerSettings>(() => Engine!.Provider.GetRequiredService<IOptions<JsonSerializerSettings>>().Value);
    }

    public async Task<ScenarioFrame> ExecuteActToken(JToken input, string output)
    {
        _level++;
        var prev = _currentFrame;
        if (_currentFrame is null)
        {
            _currentFrame = new ScenarioFrame
            {
                Parent = null!,
                FrameType = FrameType.Output,
                Input = input,
                OutputName = output,
                Path = input.Path,
            };
            _currentFrame.SetResult(new() {Status = ExecutionStatus.Executed, Value = null, Exception = null});
            // store root frame for refs is needed
            _root = _currentFrame;

        }
        else
        {
            _currentFrame = _currentFrame.CreateOutputFrame(output, input);
        }

        var queue = new Queue<ScenarioFrame>();

        this.Trace("start #{Level}", Level);
        this.Trace("str enq {Frame} #{Level}", _currentFrame.ToString(), Level);

        queue.Enqueue(_currentFrame);

        while (queue.Count != 0)
        {
            var next = queue.Dequeue();
            this.Trace("deq {Frame}", next.ToString());

            foreach (var frame in next.GetFrames(this))
            {
                queue.Enqueue(frame);
                this.Trace("enq {Frame}", frame.ToString());
            }

            if (next.FrameType == FrameType.Execution)
            {
                this.Trace("exc {Frame}", next.ToString());
                await ExecuteAct(next);
            }
        }

        this.Trace("end #{Level}", Level);
        var rr = _currentFrame;

        if (prev is not null)
        {
            _currentFrame = prev;
        }
        _level--;

        return rr!;
    }

    internal Task ProcessActs(JToken token) =>  ExecuteActToken(token, "when");

    internal async ValueTask ProcessAsserts(JToken token)
    {
        var output = _root.BuildOutput(this);

        if (Scenario.Options.Refs ?? Scenario.SetOptions.Refs)
        {
            var replacer = new RefReplacer();
            token = replacer.Replace(token, new Lazy<JToken>(() => output), this)!;
        }

        if (token is JObject jo)
        {
            foreach (var prop in jo.Properties())
            {
                if (prop.Name.StartsWith(MagicStrings.VariableMarker))
                {
                    await ExecuteAssertToken(prop.Value, GetVarOutput(prop.Name));
                }
            }
        }

        return;

        JToken GetVarOutput(string var)
        {
            if (output is JObject jObject && jObject.TryGetValue(var, out var varOutput))
            {
                return varOutput;
            }

            // not supporting vars in array or nested. is there use?

            throw new InvalidOperationException($"Could not find variable '{var}' in the output.");
        }
    }

    internal async Task ProcessOutputExpectations(JToken token)
    {
        var root = _currentFrame ?? throw new InvalidOperationException("Root frame is null");
        var output = root.BuildOutput(this);

        output.Should().BeEquivalentTo(token);

        await Task.CompletedTask;
    }

    private ScenarioFrame? _currentFrame;

    private async Task ExecuteAct(ScenarioFrame frame)
    {
        ExecutionStatus status;
        Exception? exception = null;
        object? result = null;

        this.Info("{Path}", frame.Path);
        this.Debug("\u21e8 in: {Input}", frame.Input?.ToString(Formatting.None));

        try
        {
            var prev = _currentFrame;
            _currentFrame = frame;
            var input  = ProcessRefs(frame.Input);
            result = await frame.Step.Invocation(input, this);

            _currentFrame = prev;

            if (Log.IsEnabled(LogLevel.Trace))
            {
                var asStr = result is null ? "(null)" : JToken.FromObject(result).ToString(Formatting.None);
                this.Debug("\u21e6 out: {Result}", asStr);
            }

            status = ExecutionStatus.Executed;
        }
        catch (Exception ex)
        {
            exception = ex;
            status = ExecutionStatus.Faulted;
            this.Warn("\u26a0 err: act '{Act}' threw {Message}", frame.ToString(), exception.Message);
        }

        var actResult = new ActResult
        {
            Status = status,
            Exception = exception,
            Value = result
        };

        frame.SetResult(actResult);
    }

    private JToken? ProcessRefs(JToken? input)
    {
        if (input is null) return input;
        if (!(Scenario.Options.Refs ?? Scenario.SetOptions.Refs)) return input;

        // todo: put this somewhere? Provider?
        var replacer = new RefReplacer();
        var output = new Lazy<JToken>(()=>_root.BuildOutput(this));
        return replacer.Replace(input, output, this);
    }


    public async ValueTask ExecuteAssertToken(JToken token, JToken? actual)
    {
        switch (token)
        {
            case JValue jv:
                {
                    var assert = Get(jv.Value<string>()!, jv.Path);
                    await assert.Invocation(actual, null, this);
                    break;
                }
            case JObject jo:
                {
                    foreach (var prop in jo.Properties())
                    {
                        var assert = Get(prop.Name, prop.Path);
                        await assert.Invocation(actual, prop.Value, this);
                    }
                    break;
                }
            case JArray ja:
                {
                    foreach (var val in ja)
                    {
                        await ExecuteAssertToken(val, actual);
                    }
                    break;
                }

            default: throw new InvalidOperationException($"Token {token.Type} at {token.Path}  is not supported.");
        }

        AssertInstance Get(string name, string path)
        {
            var key = AssertKey.FromString(name);
            if(!Engine.Asserts.TryGetValue(key, out var instance))
            {
                throw new InvalidOperationException($"Assert {name} was not found at {path}.");
            }

            return instance;
        }
    }
    public ValueTask ProcessExceptionExpectation(JToken expected, Exception actual)
    {
        this.Info("exception");
        return ExecuteAssertToken(expected, actual.ToJson(JsonSerializerSettings));
    }
}
