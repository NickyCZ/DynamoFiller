namespace PublicApi.FxPrices.Services;

public interface IFxPricesServices
{
    Task AddFxPrices(List<string> instrument);
}
