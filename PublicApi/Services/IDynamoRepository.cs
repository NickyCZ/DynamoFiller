using PublicApi.Records;

namespace PublicApi.Services;

public interface IDynamoRepository
{
    Task<bool> InsertMultiplePrices(IEnumerable<MultiplePrices> multiplePrices);
}
