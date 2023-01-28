namespace PublicApi.MultiplePrices.Services;

public interface IMultiplePricesService
{
    Task AddMultiplePrices(List<string> instrument);
}
