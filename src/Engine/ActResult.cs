namespace NeverTest;

public record ActResult
{
    public static readonly ActResult Pending = new()
    {
        Value = null,
        Exception = null,
        Status = ExecutionStatus.Pending
    };

    public required ExecutionStatus Status { get; init; }
    public required object? Value { get; init; }
    public required Exception? Exception { get; set; }
}
