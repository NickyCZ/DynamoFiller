namespace Domain.Services;

public interface IRecordsReader<T>
{
    public Task<IEnumerable<T>> CreateRecordsAsync(string rawInstrumentPath, CancellationToken cancellationToken = default);
}
