using Amazon.DynamoDBv2.Model;
using Newtonsoft.Json;

namespace PublicApi.Extension;

public static class ItemExtensions
{
    public static Dictionary<string, AttributeValue> ToAttributeValues<T>(this T item)
    {
        var json = JsonConvert.SerializeObject(item);
        return JsonConvert.DeserializeObject<Dictionary<string, AttributeValue>>(json);
    }
}
