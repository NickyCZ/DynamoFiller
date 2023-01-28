namespace PublicApi.AdjustedPrices.Services;

public interface IAdjustedPricesService
{
    Task AddAdjustedPrices(List<string> instrument);
}
