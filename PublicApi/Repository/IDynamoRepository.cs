
namespace PublicApi.Repository;

public interface IDynamoRepository
{
    public Task<IEnumerable<T>> GetItemsAsync<T>(string tableName);
    public Task InsertItemAsync<T>(string tableName, T item);
}
