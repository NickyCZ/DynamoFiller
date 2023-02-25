using Mediator;

namespace PublicApi.Queries;

public class GetCountOfItemsQuery : IRequest<int>
{
    public string TableName { get; set; }
}