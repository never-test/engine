namespace NeverTest.Http;

internal class JsonResponseDeserializer
{
#pragma warning disable CA1822
    public async Task<JToken?> Deserialize(HttpResponseMessage message)
#pragma warning restore CA1822
    {
        var bodyAsString = await message.Content.ReadAsStringAsync();
        return JToken.Parse(bodyAsString);
    }
}
