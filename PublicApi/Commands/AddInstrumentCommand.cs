using Mediator;
using PublicApi.MultiplePrices.Models;

namespace PublicApi.Commands;

public class AddInstrumentCommand : IRequest<string>
{
    public MultiplePricesModel Model { get; set; } = new();
}
