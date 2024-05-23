namespace NeverTest;

internal static class Extensions
{
    /// <summary>
    /// Return completed value task allowing writing expression bodied fluent assertions 
    /// </summary>
    /// <param name="_"></param>
    /// <returns></returns>
    public static ValueTask End(this object? _)=> ValueTask.CompletedTask;
}