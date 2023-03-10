using Mediator;
using Microsoft.AspNetCore.Mvc;
using PublicApi.Commands;
using PublicApi.MultiplePrices.Models;
using PublicApi.Queries;
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

    [HttpGet("tables")]
    [ProducesResponseType((int)HttpStatusCode.Forbidden)]
    [ProducesResponseType((int)HttpStatusCode.OK)]
    [ProducesResponseType((int)HttpStatusCode.BadRequest)]
    public async Task<IActionResult> GetTables()
    {
        var tables = await mediator.Send(new GetTablesQuery());
        return Ok(tables);
    }

    [HttpGet("cout-of-items")]
    [ProducesResponseType((int)HttpStatusCode.Forbidden)]
    [ProducesResponseType((int)HttpStatusCode.OK)]
    [ProducesResponseType((int)HttpStatusCode.BadRequest)]
    public async Task<IActionResult> CountOfItems(string nameOfTable)
    {
        var tables = await mediator.Send(new GetCountOfItemsQuery { TableName = nameOfTable });
        return Ok(tables);
    }

}