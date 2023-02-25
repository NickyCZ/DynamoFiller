using Amazon.DynamoDBv2.Model;
using Mediator;
using PublicApi.MultiplePrices.Items;
using PublicApi.Queries;
using PublicApi.Repository;

namespace PublicApi.Handlers;

public class GetCountOfItemsHandler : IRequestHandler<GetCountOfItemsQuery, int>
{
    private readonly ILogger<AddInstrumentHandler> logger;
    private readonly IDynamoDBRepository<MultiplePricesItem> dynamoDBRepository;

    public GetCountOfItemsHandler(ILogger<AddInstrumentHandler> logger,
                                 IDynamoDBRepository<MultiplePricesItem> dynamoDBRepository)
    {
        this.logger = logger ?? throw new ArgumentNullException(nameof(logger));
        this.dynamoDBRepository = dynamoDBRepository ?? throw new ArgumentNullException(nameof(dynamoDBRepository));
    }

    public async ValueTask<int> Handle(GetCountOfItemsQuery query, CancellationToken cancellationToken)
    {
        logger.LogInformation("get list of tables");
        var dynamoDbClient = dynamoDBRepository.GetDynamo();
        var request = new DescribeTableRequest
        {
            TableName = query.TableName
        };

        var response = await dynamoDbClient.DescribeTableAsync(request);
        var count = (int)response.Table.ItemCount;
        return count;
    }
}
