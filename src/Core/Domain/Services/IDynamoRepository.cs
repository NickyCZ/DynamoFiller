using Domain.Records;

namespace Domain.Services;

public interface IDynamoRepository
{
    Task<bool> InsertMultiplePrices(IEnumerable<MultiplePrices> multiplePrices);
}
