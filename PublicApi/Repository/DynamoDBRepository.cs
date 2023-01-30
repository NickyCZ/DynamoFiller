using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.DataModel;
using Amazon.Runtime;
using Microsoft.Extensions.Options;
using PublicApi.Settings;

namespace PublicApi.Repository;

public class DynamoDBRepository<T> : IDynamoDBRepository<T> where T : class
{
    private readonly DynamoDBContext context;
    private readonly AmazonDynamoDBClient client;
    public DynamoDBRepository(IOptions<DatabaseSettings> databaseSettings)
    {
        var credentials = new BasicAWSCredentials(databaseSettings.Value.AccessKeyId, databaseSettings.Value.SecretAccessKey);
        var config = new AmazonDynamoDBConfig
        {
            ServiceURL = databaseSettings.Value.ServiceURL,
            AuthenticationRegion = databaseSettings.Value.Region
        };
        this.client = new AmazonDynamoDBClient(credentials, config);        
        this.context = new DynamoDBContext(this.client);
    }
    public AmazonDynamoDBClient GetDynamo()
    {
        return this.client;
    }
    public async Task<T> GetAsync(string id)
    {
        try
        {
            return await context.LoadAsync<T>(id);
        }
        catch (Exception ex)
        {
            throw new Exception($"Amazon error in Get operation! Error: {ex}");
        }
    }
    public async Task WriteAsync(T item)
    {
        try
        {
            await context.SaveAsync(item);
        }
        catch (Exception ex)
        {
            throw new Exception($"Amazon error in Write operation! Error: {ex}");
        }
    }
    public async Task WriteManyAsync(IEnumerable<T> items)
    {
        try
        {
            var batchWrite = context.CreateBatchWrite<T>();
            batchWrite.AddPutItems(items);
            await batchWrite.ExecuteAsync();
        }
        catch (Exception ex)
        {
            throw new Exception($"Amazon error in AddMany operation! Error: {ex}");
        }
    }

    public async Task DeleteAsync(T item)
    {
        try
        {
            await context.DeleteAsync(item);
        }
        catch (Exception ex)
        {
            throw new Exception($"Amazon error in Delete operation! Error: {ex}");
        }
    }
}