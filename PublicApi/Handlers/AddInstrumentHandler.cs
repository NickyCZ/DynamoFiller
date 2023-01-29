using Mediator;
using PublicApi.AdjustedPrices.Services;
using PublicApi.Commands;
using PublicApi.FxPrices.Services;
using PublicApi.MultiplePrices.Services;
using PublicApi.RollCalendars.Services;
namespace PublicApi.Handlers;

public class AddInstrumentHandler : IRequestHandler<AddInstrumentCommand, string>
{
    private readonly ILogger<AddInstrumentHandler> logger;
    private readonly IFxPricesServices fxPricesServices;
    private readonly IAdjustedPricesService adjustedPricesService;
    private readonly IMultiplePricesService multiplePricesService;
    private readonly IRollCalendarServices rollCalendarServices;
    public AddInstrumentHandler(ILogger<AddInstrumentHandler> logger, IFxPricesServices fxPricesServices, 
                                IAdjustedPricesService adjustedPricesService, IMultiplePricesService multiplePricesService,
                                IRollCalendarServices rollCalendarServices)
    {
        this.logger = logger ?? throw new ArgumentNullException(nameof(logger));
        this.fxPricesServices = fxPricesServices ?? throw new ArgumentNullException(nameof(fxPricesServices));
        this.adjustedPricesService = adjustedPricesService ?? throw new ArgumentNullException(nameof(adjustedPricesService));
        this.multiplePricesService = multiplePricesService ?? throw new ArgumentNullException(nameof(multiplePricesService));
        this.rollCalendarServices = rollCalendarServices ?? throw new ArgumentNullException(nameof(rollCalendarServices));
    }
    public async ValueTask<string> Handle(AddInstrumentCommand command, CancellationToken cancellationToken)
    {
        logger.LogInformation("Adding Instruments");
        var instruments = command.Model.Instruments;
        fxPricesServices.AddFxPrices(instruments);
        adjustedPricesService.AddAdjustedPrices(instruments);
        await multiplePricesService.AddMultiplePrices(instruments);
        rollCalendarServices.AddRollCalendar(instruments);
        return "Instruments added successfully";
    }
}