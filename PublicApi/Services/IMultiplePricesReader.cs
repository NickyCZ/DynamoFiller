using PublicApi.Records;

namespace PublicApi.Services;

public interface IMultiplePricesReader
{
    Task<IEnumerable<MultiplePrices>> GetMultiplePricesAsync(string filePath, CancellationToken stoppingToken = default);
}
