using Amazon.DynamoDBv2;
using Microsoft.Extensions.Options;
using PublicApi.Records;
using PublicApi.Services;
using PublicApi.Settings;

namespace PublicApi.Repository;

public class DynamoRepository : IDynamoRepository
{
    private readonly IAmazonDynamoDB _dynamoDb;
    private readonly IOptions<DatabaseSettings> _databaseSettings;

    public DynamoRepository(IAmazonDynamoDB dynamoDb,
        IOptions<DatabaseSettings> databaseSettings)
    {
        _dynamoDb = dynamoDb;
        _databaseSettings = databaseSettings;
    }

    public Task<bool> InsertMultiplePrices(IEnumerable<MultiplePrices> multiplePrices)
    {
        throw new NotImplementedException();
    }
}
