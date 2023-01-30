using Amazon.DynamoDBv2.Model;
using Amazon.DynamoDBv2;
using PublicApi.MultiplePrices.Items;
using PublicApi.Repository;

namespace PublicApi.MultiplePrices.TableServices;

public class MultiplePricesTableService : IMultiplePricesTableService
{
    private readonly IDynamoDBRepository<MultiplePricesItem> dynamoDBRepository;
    private readonly ILogger<MultiplePricesTableService> logger;
    public MultiplePricesTableService(IDynamoDBRepository<MultiplePricesItem> dynamoDBRepository, ILogger<MultiplePricesTableService> logger)
    {
        this.dynamoDBRepository = dynamoDBRepository ?? throw new ArgumentNullException(nameof(dynamoDBRepository));
        this.logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }
    public async Task CreateTableAsync()
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
                ReadCapacityUnits = 10,
                WriteCapacityUnits = 10
            }
        };
        await context.CreateTableAsync(request);
    }

}
