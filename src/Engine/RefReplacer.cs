using System.Text.RegularExpressions;
using FluentAssertions.Equivalency;

namespace NeverTest;

internal sealed class RefReplacer
{
    private const string Marker = "_{";

    public JToken? Replace(JToken actInput, JToken output, IScenarioContext context)
    {
        if (actInput is JValue v)
        {
            return DoReplace(v, output, context);
        }

        if (actInput is JArray ja)
        {
            var array = new JArray();
            foreach (var token in ja.Children())
            {
                array.Add(Replace(token, output, context) ?? JValue.CreateNull());
            }

            return array;
        }

        if (actInput is JObject jo)
        {
            var obj = new JObject();

            foreach (var (prop, token) in jo)
            {
                var replaced = token is not null ? Replace(token, output, context) : token;
                obj.Add(prop, replaced);
            }

            return obj;
        }

        return actInput;
    }

    private static readonly Regex s_matchAnyRef = new(Marker + @"\$(?'path'[^\}]+)\}");
    private static readonly Regex s_matchSingleRef = new("^"+ Marker + @"\$(?'path'[^\}]+)\}$");

    private JToken? DoReplace(JValue node, JToken output, IScenarioContext context)
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
            return output.SelectToken(targetPath)?.DeepClone();;
        }

        var replaced = s_matchAnyRef.Replace(
            input,
            match =>
            {
                var targetPath = $"${match.Groups["path"].Value}";
                return output
                           .SelectToken(targetPath)?
                           .Value<string>() ??
                       throw new InvalidOperationException($"'{node.Path}' contains step expression '{targetPath}' that could not be resolved ");
            });

             node.Value = replaced;
             return node;
    }
}
