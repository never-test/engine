namespace NeverTest.Http;

internal class JsonResponseDeserializer
{
    public async Task<JToken?> Deserialize(HttpResponseMessage message)
    {
        // todo: nicer
        var bodyAsString = await message.Content.ReadAsStringAsync();
        return JToken.Parse(bodyAsString);
    }
}
