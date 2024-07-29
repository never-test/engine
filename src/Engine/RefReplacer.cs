namespace NeverTest;

using System.Text.RegularExpressions;
using Logging;

internal sealed class RefReplacer
{
    public static JToken? Replace(JToken input, Lazy<JToken> output, IScenarioContext context)
    {
        if (input is JValue v)
        {
            return DoReplace(v, output, context);
        }

        if (input is JArray ja)
        {
            var array = new JArray();
            foreach (var token in ja.Children())
            {
                array.Add(Replace(token, output, context) ?? JValue.CreateNull());
            }

            return array;
        }

        if (input is JObject jo)
        {
            var obj = new JObject();

            foreach (var (prop, token) in jo)
            {
                var replaced = token is not null ? Replace(token, output, context) : token;
                obj.Add(prop, replaced);
            }

            return obj;
        }

        return input;
    }

    private static readonly Regex s_matchAnyRef = new(MagicStrings.ReplacementMarker + @"\$(?'path'[^\}]+)\}");
    private static readonly Regex s_matchSingleRef = new("^"+ MagicStrings.ReplacementMarker + @"\$(?'path'[^\}]+)\}$");

    private static JToken? DoReplace(JValue node, Lazy<JToken> output, IScenarioContext context)
    {
        context.Trace("ref: {path}", node.Path);
        if (node.Value is not string input)
        {
            return node;
        }

        var singleTokenMatch = s_matchSingleRef.Match(input);
        if (singleTokenMatch.Success)
        {
            var targetPath = $"${singleTokenMatch.Groups["path"].Value}";
            return output.Value.SelectToken(targetPath)?.DeepClone();;
        }

        var replaced = s_matchAnyRef.Replace(
            input,
            match =>
            {
                var targetPath = $"${match.Groups["path"].Value}";
                return output.Value
                           .SelectToken(targetPath)?
                           .Value<string>() ??
                       throw new InvalidOperationException($"'{node.Path}' contains step expression '{targetPath}' that could not be resolved ");
            });

             node.Value = replaced;
             return node;
    }
}
