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
            var client = dynamoRepository.GetDynamo();
            if (await TableExistsAsync(client, "MultiplePrices"))
            {
                return;
            }
            await CreateMultiplePricesTableAsync(client, logger);
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

    private static async Task<bool> TableExistsAsync(IAmazonDynamoDB client, string tableName)
    {
        try
        {
            var response = await client.ListTablesAsync();
            return response.TableNames.Contains(tableName);
        }
        catch (Exception ex)
        {
            Console.WriteLine("Error occured while checking if table exists: " + ex.Message);
            return false;
        }
    }

    private static async Task CreateMultiplePricesTableAsync(AmazonDynamoDBClient client, ILogger logger)
    {
        logger.LogInformation("Creation MultiplePricesTable");
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
                ReadCapacityUnits = 1000,
                WriteCapacityUnits = 1000
            }
        };
        await client.CreateTableAsync(request);
    }

}