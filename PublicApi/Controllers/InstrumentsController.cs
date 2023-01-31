using Mediator;
using Microsoft.AspNetCore.Mvc;
using PublicApi.Commands;
using PublicApi.MultiplePrices.Models;
using System.Net;

namespace PublicApi.Controllers;

[ApiController]
[Route("[controller]")]
public class InstrumentsController : ControllerBase
{
    private readonly IMediator mediator;
    private readonly ILogger<InstrumentsController> logger;

    public InstrumentsController(IMediator mediator, ILogger<InstrumentsController> logger)
    {
        this.mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
        this.logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    [HttpPost("add-new-instruments")]
    [ProducesResponseType((int)HttpStatusCode.Forbidden)]
    [ProducesResponseType((int)HttpStatusCode.OK)]
    [ProducesResponseType((int)HttpStatusCode.BadRequest)]
    public async Task<IActionResult> AddInstruments([FromBody] MultiplePricesModel model)
    {
        if (ModelState.IsValid)
        {
            logger.LogInformation("Add instruments");
            var result = await mediator.Send(new AddInstrumentCommand() { Model = model });
            return Ok(result);
        }
        return BadRequest(ModelState);
    }
}