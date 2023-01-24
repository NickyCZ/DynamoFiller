using Domain.Records;

namespace Domain.Services;

public interface IMultiplePricesReader
{
    Task<IEnumerable<MultiplePrices>> GetMultiplePricesAsync(string filePath, CancellationToken stoppingToken = default);
}
