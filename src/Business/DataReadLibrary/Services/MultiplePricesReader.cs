using Domain.Models;
using Domain.Records;
using Domain.Services;
using Microsoft.Extensions.Logging;

namespace DataReadLibrary.Services;

public class MultiplePricesReader : IMultiplePricesReader
{
    private readonly ILogger<MultiplePricesReader> logger;
    private readonly IRecordsReader<MultiplePricesModel> recordsReader;
    public MultiplePricesReader(ILogger<MultiplePricesReader> logger, IRecordsReader<MultiplePricesModel> recordsReader)
    {
        this.logger = logger ?? throw new ArgumentNullException(nameof(logger));
        this.recordsReader = recordsReader ?? throw new ArgumentNullException(nameof(recordsReader));
    }

    public async Task<IEnumerable<MultiplePrices>> GetDataAsync(string filePath, CancellationToken stoppingToken = default)
    {
        var fileNameWithoutExt = Path.GetFileNameWithoutExtension(filePath);
        logger.LogInformation("Process multiple prices data of " + fileNameWithoutExt + " instrument");
        var rawSeries = await recordsReader.CreateRecordsAsync(filePath);
        
        var multiplePricesSeries = rawSeries.Select(x => new MultiplePrices
        {
            Instrument = fileNameWithoutExt,
            UnixDateTime = (int)x.DATETIME.Subtract(DateTime.UnixEpoch).TotalSeconds,
            Carry = x.CARRY,
            CarryContract = x.CARRY_CONTRACT,
            Price = x.PRICE,
            PriceContract = x.PRICE_CONTRACT,
            Forward = x.FORWARD,
            ForwardContract = x.FORWARD_CONTRACT
        });
        return multiplePricesSeries;
    }
}