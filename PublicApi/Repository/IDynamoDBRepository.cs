using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.DataModel;

namespace PublicApi.Repository;

public interface IDynamoDBRepository<T> where T : class
{
    AmazonDynamoDBClient GetDynamo();
    DynamoDBContext GetDynamoDBContext();
    Task WriteMany(string instrumentName, IEnumerable<T> items);

}

