using Amazon.DynamoDBv2.Model;
using Amazon.DynamoDBv2;
using PublicApi.MultiplePrices.Items;

namespace PublicApi.Repository.TableSeed;

public class TableSeedService
{
    public static async Task SeedAsync(IDynamoDBRepository<MultiplePricesItem> dynamoRepository, ILogger logger, int retry = 0)
    {
        var retryForAvailability = retry;
        try
        {
            var context = dynamoRepository.GetDynamo();
            if (await TableExistsAsync(context, "MultiplePrices"))
            {
                return;
            }
            await CreateMultiplePricesTableAsync(dynamoRepository, logger);
        }
        catch (Exception ex)
        {
            if (retryForAvailability >= 10) throw;

            retryForAvailability++;

            logger.LogError(ex.Message);
            await SeedAsync(dynamoRepository, logger, retryForAvailability);
            throw;
        }
    }

    private static async Task<bool> TableExistsAsync(IAmazonDynamoDB context, string tableName)
    {
        var response = await context.ListTablesAsync();
        return response.TableNames.Contains(tableName);
    }

    private static async Task CreateMultiplePricesTableAsync(IDynamoDBRepository<MultiplePricesItem> dynamoDBRepository, ILogger logger)
    {
        logger.LogInformation("Creation MultiplePricesTable");
        var context = dynamoDBRepository.GetDynamo();
        var request = new CreateTableRequest
        {
            TableName = "MultiplePrices",
            KeySchema = new List<KeySchemaElement>
    {
        new KeySchemaElement
        {
            AttributeName = "Instrument",
            KeyType = KeyType.HASH
        },
        new KeySchemaElement
        {
            AttributeName = "UnixDateTime",
            KeyType = KeyType.RANGE
        }
    },
            AttributeDefinitions = new List<AttributeDefinition>
    {
        new AttributeDefinition
        {
            AttributeName = "Instrument",
            AttributeType = ScalarAttributeType.S
        },
        new AttributeDefinition
        {
            AttributeName = "UnixDateTime",
            AttributeType = ScalarAttributeType.N
        }
    },
            ProvisionedThroughput = new ProvisionedThroughput
            {
                ReadCapacityUnits = 10000,
                WriteCapacityUnits = 10000
            }
        };
        await context.CreateTableAsync(request);
    }

}