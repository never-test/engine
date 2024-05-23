using System.Diagnostics;

namespace NeverTest;

[DebuggerDisplay(DebugString)]
public sealed record ScenarioFrame
{
    private const string DebugString = "{FrameType} ({Form}) {Path}"; 
    private const string ValueFrameKey = "__value"; 
    public required ScenarioFrame? Parent { get; init; }
    public ActResult Result { get; private set; } = ActResult.Pending;
    public required FrameType FrameType { get; init; }
    public Form Form { get; private set; }
    public string? TokenName { get; private set; }

    public required JToken? Input
    {
        get => _input;
        init
        {
            Form = value switch
            {
                JObject => Form.Object,
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
    public StepInstance Step { get; init; } = StepInstance.NotApplicable;
    
    private readonly Dictionary<string, ScenarioFrame> _frames = new();
    private readonly JToken? _input;

    public IEnumerable<ScenarioFrame> GetFrames(IScenarioContext context)
    {
        if (FrameType == FrameType.Execution) yield break;

        if (Input is JValue value)
        {
            var act = value.Value<string>()!;
            var frame = new ScenarioFrame
            {
                Parent = this,
                FrameType = FrameType.Execution,
                // TODO: handle edge cases e.g. nulls or other types 
                TokenName = act,
                Input = null,
                OutputName = ValueFrameKey,
                Path = value.Path,
                Step = GetStep(act),
            };
            
            _frames.Add(ValueFrameKey, frame);
            
            yield return frame;
        }

        if (Input is JObject objectForm)
        {
            foreach (var property in objectForm.Properties())
            {
                if (property.Name.StartsWith('$'))
                {
                    var frame = CreateOutputFrame(
                        property.Name, 
                        property.Path, 
                        property.Value);
                    
                    yield return frame;
                }
                else
                {
                    var frame = new ScenarioFrame
                    {
                        Parent = this,
                        FrameType = FrameType.Execution,
                        TokenName = property.Name,
                        Input = property.Value,
                        OutputName = property.Name,
                        Path = property.Path,
                        Step = GetStep(property.Name)
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
                    TokenName = index.ToString(),
                    Input = item,
                    OutputName = index.ToString(),
                    Path = item.Path,
                    Step = StepInstance.NotApplicable
                };
                index++;
                _frames.Add(frame.OutputName, frame);
                yield return frame;
            }
        }

        yield break;

        StepInstance GetStep(string name)
        {
            if (!context.ScenarioEngine.Acts.TryGetValue(new ActKey(name), out var step))
            {
                // TODO: rewrite without throwing to support validation phase..
                throw new InvalidOperationException($"Step '{step}' not found");
            }

            return step;
        }
    }

    public ScenarioFrame CreateOutputFrame(
        string outputName, 
        string path,
        JToken input)
    {
        var frame = new ScenarioFrame
        {
            Parent = this,
            FrameType = FrameType.Output,
            Input = input,
            OutputName = outputName,
            Path = path,
            Step = StepInstance.NotApplicable
        };

        _frames.Add(frame.OutputName, frame);

        return frame;
    }
    public void SetResult(ActResult result, IScenarioContext context)
    {
        if (Result != ActResult.Pending) throw new InvalidOperationException("Result already set");
        Result = result;
        
    }
    public JToken BuildOutput(IScenarioContext context)
    {
        
        // TODO: simplify, dont check for,
        if (FrameType is FrameType.Output && Form == Form.Value)
        {
            if (!_frames.TryGetValue(ValueFrameKey, out var frame))
            {
                return JToken.FromObject(new { Error = "Value frame is not found."});
            }
            var output = frame.Result.Value;
            if (output is not null)
            {
                return JToken.FromObject(output);
            }
        }
        if (FrameType == FrameType.Execution)
        {
            if (Result.Status == ExecutionStatus.Executed)
            {
                if(Result.Value is null) return JValue.CreateNull();
                return JToken.FromObject(Result.Value);
            }

            return JValue.CreateNull();
        }
        if (Form == Form.Object)
        {
            if (_frames.Count == 1 &&
                context.Scenario.Options.Folding &&
                !_frames.Single().Key.StartsWith('$'))
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
        
        return JToken.FromObject(new{ Error = "Output error"});
    }

    public override string ToString() => $"{FrameType} ({Form}) {Path} => {OutputName}";
}