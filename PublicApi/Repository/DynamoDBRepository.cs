using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.DataModel;
using Amazon.Runtime;
using Microsoft.Extensions.Options;
using PublicApi.Settings;
using System.Threading.Tasks;

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
            AuthenticationRegion = databaseSettings.Value.Region,
            Timeout = TimeSpan.FromSeconds(60)
        };
        this.client = new AmazonDynamoDBClient(credentials, config);
        this.context = new DynamoDBContext(this.client);
    }
    public AmazonDynamoDBClient GetDynamo()
    {
        return this.client;
    }
    public async Task WriteMany(string instrumentName, IEnumerable<T> items)
    {
        int retryAttempts = 3;
        int batchSize = 100;
        int totalBatches = (int)Math.Ceiling((double)items.Count() / batchSize);

        logger.LogInformation("Writing " + instrumentName + " with " + items.Count() + " items in " + totalBatches + " batches.");

        for (int attempt = 1; attempt <= retryAttempts; attempt++)
        {
            try
            {
                var tasks = new List<Task>();
                for (int i = 0; i < totalBatches; i++)
                {
                    var task = Task.Run(async () =>
                    {
                        var batch = context.CreateBatchWrite<T>();
                        var currentBatch = items.Skip(i * batchSize).Take(batchSize);
                        batch.AddPutItems(currentBatch);
                        await batch.ExecuteAsync();
                    });

                    tasks.Add(task);
                }

                await Task.WhenAll(tasks);
                break;
            }
            catch (TimeoutException ex)
            {
                if (attempt == retryAttempts)
                {
                    logger.LogError("Operation failed after " + retryAttempts + " attempts. Exception details: " + ex.Message);
                    throw;
                }
                else
                {
                    int backoffTime = GetExponentialBackoffTime(attempt);
                    logger.LogWarning("Operation timed out, retrying in " + backoffTime + "ms. Attempt " + attempt + " of " + retryAttempts);
                    Task.Delay(backoffTime).Wait();
                }
            }
            catch (Exception ex)
            {
                logger.LogError("An error occurred while writing items. Exception details: " + ex.Message);
                throw;
            }
        }

        logger.LogInformation(instrumentName + " writing is complete.");
    }

    private int GetExponentialBackoffTime(int attempt)
    {
        return (int)Math.Pow(2, attempt) * 1000;
    }
}