using Amazon.DynamoDBv2;

namespace PublicApi.Repository;

public interface IDynamoDBRepository<T> where T : class
{
    AmazonDynamoDBClient GetDynamo();
    Task<T> GetAsync(string id);
    Task WriteAsync(T item);
    Task WriteManyAsync(IEnumerable<T> items);
    Task DeleteAsync(T item);
}

