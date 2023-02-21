using Mediator;
using PublicApi.MultiplePrices.Items;
using PublicApi.Queries;
using PublicApi.Repository;

namespace PublicApi.Handlers;

public class GetTablesQueryHandler : IRequestHandler<GetTablesQuery, List<string>>
{
    private readonly ILogger<AddInstrumentHandler> logger;
    private readonly IDynamoDBRepository<MultiplePricesItem> dynamoDBRepository;

    public GetTablesQueryHandler(ILogger<AddInstrumentHandler> logger,
                                 IDynamoDBRepository<MultiplePricesItem> dynamoDBRepository)
    {
        this.logger = logger ?? throw new ArgumentNullException(nameof(logger));
        this.dynamoDBRepository = dynamoDBRepository ?? throw new ArgumentNullException(nameof(dynamoDBRepository));
    }

    public async ValueTask<List<string>> Handle(GetTablesQuery request, CancellationToken cancellationToken)
    {
        logger.LogInformation("get list of tables");
        var dynamoDbClient = dynamoDBRepository.GetDynamo();
        var response = await dynamoDbClient.ListTablesAsync(cancellationToken);
        return response.TableNames;
    }
}

