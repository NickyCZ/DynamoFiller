using Domain.Records;

namespace Domain.Services;

public interface IMultiplePricesReader
{
    Task<IEnumerable<MultiplePrices>> GetDataAsync(string filePath, CancellationToken stoppingToken = default);
}
