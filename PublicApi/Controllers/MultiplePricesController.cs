using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using PublicApi.DTOs;
using PublicApi.Services;
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
    private readonly IDynamoRepository dynamoRepository;
    private string FolderPath => options.Value.FolderPath;
    public MultiplePricesController(ILogger<MultiplePricesController> logger, 
                                    IMultiplePricesReader multiplePricesReader, 
                                    IOptions<MultiplePricesSettings> options,
                                    IDynamoRepository dynamoRepository)
    {
        this.logger = logger ?? throw new ArgumentNullException(nameof(logger));
        this.multiplePricesReader = multiplePricesReader ?? throw new ArgumentNullException(nameof(multiplePricesReader));
        this.options = options ?? throw new ArgumentNullException(nameof(options));
        this.dynamoRepository = dynamoRepository ?? throw new ArgumentNullException(nameof(options));
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
            var multiplePrices = await multiplePricesReader.GetMultiplePricesAsync(filePath);
            var result = await dynamoRepository.InsertMultiplePrices(multiplePrices);
            return Ok(result);
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
            
            foreach (var file in filesPaths)
            {
                var multiplePrices = await multiplePricesReader.GetMultiplePricesAsync(file);
                var result = await dynamoRepository.InsertMultiplePrices(multiplePrices);
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