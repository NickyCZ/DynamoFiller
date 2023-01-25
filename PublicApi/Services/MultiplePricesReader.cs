using PublicApi.Models;
using PublicApi.Records;
using System.Globalization;

namespace PublicApi.Services;

public class MultiplePricesReader : IMultiplePricesReader
{
    private readonly ILogger<MultiplePricesReader> logger;
    private readonly IRecordsReader<MultiplePricesModel> recordsReader;
    public MultiplePricesReader(ILogger<MultiplePricesReader> logger, IRecordsReader<MultiplePricesModel> recordsReader)
    {
        this.logger = logger ?? throw new ArgumentNullException(nameof(logger));
        this.recordsReader = recordsReader ?? throw new ArgumentNullException(nameof(recordsReader));
    }

    public async Task<IEnumerable<MultiplePrices>> GetMultiplePricesAsync(string filePath, CancellationToken stoppingToken = default)
    {
        var fileNameWithoutExt = Path.GetFileNameWithoutExtension(filePath);
        logger.LogInformation("Process multiple prices data of " + fileNameWithoutExt + " instrument");
        var rawSeries = await recordsReader.CreateRecordsAsync(filePath, stoppingToken);
        var culture = new CultureInfo("en-US");
        var multiplePricesSeries = rawSeries.Select(x => new MultiplePrices
        {
            Instrument = fileNameWithoutExt,
            UnixDateTime = (int)x.DATETIME.Subtract(DateTime.UnixEpoch).TotalSeconds,
            Carry = Convert.ToDecimal(x.CARRY, culture),
            CarryContract = x.CARRY_CONTRACT,
            Price = Convert.ToDecimal(x.PRICE, culture),
            PriceContract = x.PRICE_CONTRACT,
            Forward = Convert.ToDecimal(x.FORWARD, culture),
            ForwardContract = x.FORWARD_CONTRACT
        });
        return multiplePricesSeries;
    }
}