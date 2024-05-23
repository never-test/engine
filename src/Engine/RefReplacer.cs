using System.Text.RegularExpressions;

namespace NeverTest;

internal sealed class RefReplacer
{
    public void Replace(JToken act, Func<JToken> output, IScenarioContext context)
    {
        var outputFactory = new Lazy<JToken>(output); 
        if(act is JValue v) DoReplace(v, outputFactory);

        var valueNodes = act
            .SelectTokens("$..*")
            .OfType<JValue>()
            .Where(t => t.Value is string s && s.Contains("${{"))
            .ToList();

        var candidates = valueNodes.Count;
        var actualCount = 0;

        foreach (var node in valueNodes)
        {
            if (DoReplace(node, outputFactory))
            {
                actualCount++;
            }
        }

        // context.Log.LogTrace(
        //     "Replacement: Candidates = {CandidateCount} Replaced = {ReplacedCount}",
        //     candidates,
        //     actualCount);
    }

    private static readonly Regex s_matchReplacementToken = new(@"{\$(?'path'[^\}]+)\}");

    private static bool DoReplace(JValue node, Lazy<JToken> outputFactory)
    {

        if (node.Value is not string input)
        {
            return false;
        }

        var replaced =  s_matchReplacementToken.Replace(
            input,
            match =>
            {
                var target = $"${match.Groups["path"].Value}";
                var output = outputFactory.Value;
                var result = output 
                                 .SelectToken(target)?
                                 .Value<string>() ??
                             throw new InvalidOperationException($"'{node.Path}' contains step expression '{target}' that could not be resolved ");

                // context.Log.LogTrace("Replace: {Target} ⇒ {Value}", target, result);
                return result;
            });

        node.Value = replaced;
        return replaced != input;
    }
}