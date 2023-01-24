using Domain.DTOs;
using Domain.Records;
using Domain.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using PublicApi.Settings;
using System.Net;

namespace PublicApi.Controllers;

[ApiController]
[Route("[controller]")]
public class MultiplePricesController : ControllerBase
{
    private readonly ILogger<MultiplePricesController> logger;
    private readonly IMultiplePricesReader multiplePricesReader;
    private readonly IOptions<MultiplePricesSettings> options;
    private string FolderPath => options.Value.FolderPath;
    public MultiplePricesController(ILogger<MultiplePricesController> logger, IMultiplePricesReader multiplePricesReader, IOptions<MultiplePricesSettings> options)
    {
        this.logger = logger ?? throw new ArgumentNullException(nameof(logger));
        this.multiplePricesReader = multiplePricesReader ?? throw new ArgumentNullException(nameof(multiplePricesReader));
        this.options = options ?? throw new ArgumentNullException(nameof(options));
    }

    [HttpPost("single")]
    [ProducesResponseType((int)HttpStatusCode.Forbidden)]
    [ProducesResponseType((int)HttpStatusCode.OK)]
    [ProducesResponseType((int)HttpStatusCode.BadRequest)]
    public async Task<IActionResult> PostSigleInstrument([FromBody] MultiplePricesModel model)
    {
        if (ModelState.IsValid)
        {
            logger.LogInformation("Process single instruments");
            var filesPaths = Directory.GetFiles(FolderPath, "*.csv");
            var filePath = filesPaths.FirstOrDefault(x => x.Contains(model.Instrument));
            if (filePath == null)
            {
                return BadRequest(ModelState);
            }
            var data = await multiplePricesReader.GetDataAsync(filePath);

            return Ok(ModelState);
        }
        return BadRequest(ModelState);
    }
    [HttpPost("all")]
    [ProducesResponseType((int)HttpStatusCode.Forbidden)]
    [ProducesResponseType((int)HttpStatusCode.OK)]
    [ProducesResponseType((int)HttpStatusCode.BadRequest)]
    public async Task<IActionResult> PostAllInstruments()
    {
        if (ModelState.IsValid)
        {
            logger.LogInformation("Process all instruments");
            var filesPaths = Directory.GetFiles(FolderPath, "*.csv");
            var multiplePrices = new List<MultiplePrices>();
            foreach (var file in filesPaths)
            {
                var data = await multiplePricesReader.GetDataAsync(file);
                multiplePrices.AddRange(data);
            }
            
            return Ok(ModelState);
        }
        return BadRequest(ModelState);
    }

    [HttpGet]
    [ProducesResponseType((int)HttpStatusCode.Forbidden)]
    [ProducesResponseType((int)HttpStatusCode.OK)]
    [ProducesResponseType((int)HttpStatusCode.BadRequest)]
    public ActionResult Get()
    {
        if (ModelState.IsValid)
        {
            logger.LogInformation("Start of the process");
            var filesPaths = Directory.GetFiles(FolderPath, searchPattern: "*.csv");
            var filesNames = filesPaths.Select(x=> Path.GetFileNameWithoutExtension(x));
            return Ok(filesNames);
        }
        return BadRequest(ModelState);
    }

}