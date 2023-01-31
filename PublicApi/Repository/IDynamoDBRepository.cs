using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.DataModel;

namespace PublicApi.Repository;

public interface IDynamoDBRepository<T> where T : class
{
    AmazonDynamoDBClient GetDynamo();
    IDynamoDBContext GetDynamoContext();
    Task<T> GetAsync(string id);
    Task WriteAsync(T item);
    Task WriteManyAsync(IEnumerable<T> items);
    Task DeleteAsync(T item);
}

