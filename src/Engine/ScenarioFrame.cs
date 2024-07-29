using System.Diagnostics;
using System.Globalization;
using NeverTest.Acts;

namespace NeverTest;

[DebuggerDisplay(DebugString)]
public sealed class ScenarioFrame
{
    private const string DebugString = "{FrameType} ({Form}) {Path}";
    private const string ValueFrameKey = "__value";
    private const char VariableSymbol = '$';
    public required ScenarioFrame? Parent { get; init; }
    public ActResult Result { get; private set; } = ActResult.Pending;
    public required FrameType FrameType { get; init; }
    public Form Form { get; private set; }

    public required JToken? Input
    {
        get => _input;
        init
        {
            Form = value switch
            {
                JObject => Form.Complex,
                JArray => Form.Array,
                JValue => Form.Value,
                null => Form.Value,
                _ => throw new InvalidOperationException($"{value} / {value} is not supported")
            };
            _input = value;
        }
    }
    public required string OutputName { get; init; }
    public required string Path { get; init; }
    internal ActInstance Step { get; private init; } = ActInstance.NotApplicable;

    private readonly Dictionary<string, ScenarioFrame> _frames = new();
    private readonly JToken? _input;

    public IEnumerable<ScenarioFrame> GetFrames(IScenarioContext context)
    {
        if (FrameType == FrameType.Execution) yield break;

        if (Input is JValue value)
        {
            var act = ActKey.FromString(value.Value<string>()!);
            var frame = new ScenarioFrame
            {
                Parent = this,
                FrameType = FrameType.Execution,
                Input = null,
                OutputName = ValueFrameKey,
                Path = value.Path,
                Step = GetStep(act, Input.Path),
            };

            _frames.Add(ValueFrameKey, frame);

            yield return frame;
        }

        if (Input is JObject objectForm)
        {
            foreach (var property in objectForm.Properties())
            {
                if (property.Name.StartsWith(VariableSymbol))
                {
                    var frame = CreateOutputFrame(
                        property.Name,
                        property.Value);

                    yield return frame;
                }
                else
                {
                    var frame = new ScenarioFrame
                    {
                        Parent = this,
                        FrameType = FrameType.Execution,
                        Input = property.Value,
                        OutputName = property.Name,
                        Path = property.Path,
                        Step = GetStep(ActKey.FromString(property.Name), property.Path)
                    };

                    _frames.Add(frame.OutputName, frame);
                    yield return frame;
                }
            }
        }

        if (Input is JArray arrayForm)
        {
            var index = 0;

            foreach (var item in arrayForm)
            {
                var frame = new ScenarioFrame
                {
                    Parent = this,
                    FrameType = FrameType.Output,
                    Input = item,
                    OutputName = index.ToString(CultureInfo.InvariantCulture),
                    Path = item.Path,
                    Step = ActInstance.NotApplicable
                };

                index++;
                _frames.Add(frame.OutputName, frame);

                yield return frame;
            }
        }

        yield break;

        ActInstance GetStep(ActKey key, string path)
        {
            if (!context.Engine.Acts.TryGetValue(key, out var step))
            {
                // TODO: rewrite without throwing to support validation phase..
                throw new InvalidOperationException($"Act '{key}' is not recognized at {path}. Please make sure it is registered with the engine.");
            }

            return step;
        }
    }

    internal ScenarioFrame CreateOutputFrame(
        string outputName,
        JToken input)
    {

        var frame = new ScenarioFrame
        {
            Parent = this,
            FrameType = FrameType.Output,
            Input = input,
            OutputName = outputName,
            Path = input.Path,
            Step = ActInstance.NotApplicable
        };

        _frames.Add(frame.OutputName, frame);

        return frame;
    }

    internal void SetResult(ActResult result)
    {
        if (Result != ActResult.Pending) throw new InvalidOperationException("Result already set");
        Result = result;
    }


    public JToken BuildOutput(IScenarioContext context)
    {
        if (FrameType is FrameType.Output && Form == Form.Value)
        {
            if (!_frames.TryGetValue(ValueFrameKey, out var frame))
            {
                return JToken.FromObject(new { Error = "Value frame is not found." });
            }

            var output = frame.Result.Value;
            if (output is not null)
            {
                var actual = JToken.FromObject(output);
                context.TrackOutput(actual, output);
                return actual;
            }
        }

        if (FrameType == FrameType.Execution)
        {
            if (Result.Status == ExecutionStatus.Executed)
            {
                var actual = Result.Value is null ? JValue.CreateNull() : JToken.FromObject(Result.Value);
                context.TrackOutput(actual, Result.Value);
                return actual;
            }

            return JValue.CreateNull();
        }

        if (Form == Form.Complex)
        {
            if (_frames.Count == 1 &&
                (context.Scenario.Options.Folding
                ?? context.Scenario.SetOptions.Folding) &&
                !_frames.Single().Key.StartsWith(VariableSymbol))
            {
                return _frames.First().Value.BuildOutput(context);
            }

            var result = new JObject();

            foreach (var (name, frame) in _frames)
            {

                result.Add(name, frame.BuildOutput(context));
            }

            return result;
        }

        if (Form == Form.Array)
        {
            var result = new JArray();
            foreach (var (name, frame) in _frames)
            {
                result.Add(frame.BuildOutput(context));
            }

            return result;
        }

        throw new InvalidOperationException("Output error. Most likely a bug.");
    }

    public override string ToString() => $"{FrameType} ({Form}) {Path} => {OutputName}";
}
