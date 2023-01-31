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
    private readonly ILogger<DynamoDBRepository<T>> logger;
    public DynamoDBRepository(IOptions<DatabaseSettings> databaseSettings, ILogger<DynamoDBRepository<T>> logger)
    {
        this.logger = logger ?? throw new ArgumentNullException(nameof(logger));
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
    public IDynamoDBContext GetDynamoContext()
    {
        return this.context;
    }
    public async Task<T> GetAsync(string id)
    {
        try
        {
            return await context.LoadAsync<T>(id);
        }
        catch (Exception ex)
        {
            throw new Exception($"Amazon error in GetAvailableInstrument operation! Error: {ex}");
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
    public void WriteManyAsync(IEnumerable<T> items)
    {
        logger.LogInformation("Writing " + items.Count() + " items");
        int batchSize = 25;
        var itemsList = items.ToList();
        int totalBatches = (int)Math.Ceiling((double)itemsList.Count / batchSize);

        Parallel.For(0, totalBatches, async i =>
        {
            var batch = context.CreateBatchWrite<T>();
            var currentBatch = itemsList.Skip(i * batchSize).Take(batchSize);
            batch.AddPutItems(currentBatch);
            await batch.ExecuteAsync();
        });

        logger.LogInformation("Done");
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