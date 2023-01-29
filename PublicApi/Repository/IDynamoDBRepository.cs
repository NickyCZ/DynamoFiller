namespace PublicApi.Repository;

public interface IDynamoDBRepository<T> where T : class
{
    Task<T> GetAsync(string id);
    Task WriteAsync(T item);
    Task DeleteAsync(T item);
}

