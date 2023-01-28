using Amazon;
using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.Model;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using PublicApi.Extension;
using PublicApi.Settings;

namespace PublicApi.Repository;

public class DynamoRepository : IDynamoRepository
{
    private readonly IAmazonDynamoDB _dynamoDb;
    private readonly IOptions<DatabaseSettings> _databaseSettings;

    public DynamoRepository(IOptions<DatabaseSettings> databaseSettings)
    {
        _databaseSettings = databaseSettings;
        var config = new AmazonDynamoDBConfig
        {
            RegionEndpoint = RegionEndpoint.GetBySystemName(_databaseSettings.Value.Region),
        };
        _dynamoDb = new AmazonDynamoDBClient(config);
    }


    public async Task<IEnumerable<T>> GetItemsAsync<T>(string tableName)
    {
        var request = new ScanRequest
        {
            TableName = tableName
        };

        var items = new List<T>();

        do
        {
            var response = await _dynamoDb.ScanAsync(request);
            request.ExclusiveStartKey = response.LastEvaluatedKey;

            // Deserialize the items in the response
            var deserializedItems = response.Items.Select(i =>
            {
                string json = JsonConvert.SerializeObject(i);
                var item = JsonConvert.DeserializeObject<T>(json);
                return item;
            }).Where(x => x != null);

        } while (request.ExclusiveStartKey != null && request.ExclusiveStartKey.Count != 0);

        // Return the items
        return items;
    }

    public async Task InsertItemAsync<T>(string tableName, T item)
    {
        // Convert the item to a Dictionary<string, AttributeValue>
        var attributeValues = item.ToAttributeValues();

        // Create the request
        var request = new PutItemRequest
        {
            TableName = tableName,
            Item = attributeValues
        };

        // Insert the item
        await _dynamoDb.PutItemAsync(request);
    }

}