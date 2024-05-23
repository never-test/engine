namespace NeverTest;

using FluentAssertions.Json;
using Newtonsoft.Json;

/// <summary>
/// Contains context for a single scenario in a set.
/// In particular, contains scoped service provider
/// where scope is created for a single scenario
/// </summary>

public record ScenarioContext<T> : IScenarioContext<T> where T: IState
{
    private int _level = 0;
    public T State() => StateInstance;
    public required ILogger Log { get; init; }

    private ScenarioFrame _root = null!;
    public string Indent => new(' ', Math.Max(Level - 1, 0) * 2);
    public int Level => _level;
    public async Task<ScenarioFrame> ExecuteActToken(
        JToken input, 
        string path,
        string output)
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
                Path = input.Path
            };
            // store root frame for refs is needed
            _root = _currentFrame;

        }
        else
        {
            _currentFrame = _currentFrame.CreateOutputFrame(output, path, input);
        }
        
        var queue = new Queue<ScenarioFrame>();
        
        this.Trace("start #{Level}", Level);
        this.Trace("str enq {Frame} #{Level}",_currentFrame.ToString(), Level);
            
        queue.Enqueue(_currentFrame);
  
        while (queue.Count != 0)
        {
            var next = queue.Dequeue();
            this.Trace("deq {Frame}",next.ToString());
            
            foreach (var frame in next.GetFrames(this))
            {
                queue.Enqueue(frame);
                this.Trace("enq {Frame}",frame.ToString());
            }
            
            if (next.FrameType == FrameType.Execution)
            {
                this.Trace("exc {Frame}",next.ToString());
                await ExecuteAct(next);
            }
        }

        this.Trace("end #{Level}", Level);
        var rr = _currentFrame;
        
        if(prev is not null)
        {
            _currentFrame = prev;
        }
        _level--;

        return rr!;
    }

    public required IServiceProvider Provider { get; init; }
    
    public required ScenarioEngine ScenarioEngine { get; init; }
    public required Scenario Scenario { get; init; }
    public required T StateInstance { get; init; }

    public async Task ExecuteAssertToken(JToken? token)
    {
        this.Info("-------------------------");

        var root = _currentFrame?? throw new InvalidOperationException("Root frame is null");
        var output = root.BuildOutput(this);

        Log.LogInformation("{Output}", output.ToString());

        output.Should().BeEquivalentTo(token);
        await Task.CompletedTask;
    }

    private ScenarioFrame? _currentFrame;

    public ScenarioFrame Frame => _currentFrame ?? throw new InvalidOperationException("Current scenario frame is not set."); 
    private  async Task ExecuteAct(ScenarioFrame frame)
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
            
            ProcessRefs(frame.Input);
            result = await frame.Step.Invocation(frame.Input, this);
            
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
        
        var actResult =  new ActResult
        {
            Status = status,
            Exception = exception,
            Value = result
        };
        
        frame.SetResult(actResult, this);
    }

    private void ProcessRefs(JToken? input)
    {
        if (input is null) return;
        if (!Scenario.Options.Refs) return;
        
        // todo: put this somewhere? Provider?
        var replacer = new RefReplacer();
        replacer.Replace(input, () => _root.BuildOutput(this),this) ;
    }
}


public record ActResult
{
    public static readonly ActResult Pending = new()
    {
        Value = null,
        Exception = null,
        Status = ExecutionStatus.Pending
    };

    public  required ExecutionStatus Status { get; init; }
    public required object? Value { get; init; }
    public required  Exception? Exception { get; set; }
}

public enum ExecutionStatus
{
    Executed = 1,
    Faulted = 2,
    EngineError = 3,
    Ignored = 4,
    Pending = 5 
}

 public enum FrameType
 {
     Output = 1,
     Execution = 2
 }

/// <summary>
/// Non-generic version of scenario context to be used in non-generic-context steps
/// </summary>
public record ScenarioContext : ScenarioContext<IState>
{
    public ScenarioContext(JToken when) : base()
    {
    }
}

public enum Form
{
    Unknown = 1,
    Value, 
    Object,
    Array
}


public static class ScenarioContextLoggingExtensions
{
    public static void Info(this IScenarioContext context, string message, params object?[] args) => context.Write(LogLevel.Information, message, args);
    public static void Trace(this IScenarioContext context, string message, params object?[] args) => context.Write(LogLevel.Trace, message, args);
    public static void Debug(this IScenarioContext context, string message, params object?[] args) => context.Write(LogLevel.Debug, message, args);
    public static void Warn(this IScenarioContext context, string message, params object?[] args) => context.Write(LogLevel.Warning, message, args);
    private static void Write(this IScenarioContext context,
        LogLevel level,
        string message, 
        params object?[] args)
    {
        if(!context.Log.IsEnabled(level)) return;
        
        var array = new object[args.Length + 1];
        array[0] = context.Indent;
        Array.Copy(args,0, array,1, args.Length);
        var symbol = level switch
        {
            LogLevel.Trace => '#',
            LogLevel.Debug => '*',
            LogLevel.Warning => '?',
            LogLevel.Error => '!',
            LogLevel.Critical => '@',
            _ => ' '
        };
        var msg = $"{symbol}{{Indent}} {message}";
        
#pragma warning disable CA2254
        // ReSharper disable once TemplateIsNotCompileTimeConstantProblem
        context.Log.Log(level, msg, array);
#pragma warning restore CA2254
    }
}