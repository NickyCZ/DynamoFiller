using Microsoft.Extensions.Options;
using PublicApi.Handlers;
using PublicApi.MultiplePrices.Entities;
using PublicApi.MultiplePrices.Models;
using PublicApi.MultiplePrices.Records;
using PublicApi.RecordReaders;
using PublicApi.Repository;
using PublicApi.Settings;
using System.Collections.Concurrent;

namespace PublicApi.MultiplePrices.Services;

public class MultiplePricesService : IMultiplePricesService
{
    private readonly ILogger<AddInstrumentHandler> logger;
    private readonly IOptions<LocalFoldersSettings> localFoldersSettings;
    private readonly IRecordsReader<MultiplePricesRecord> recordsReader;
    private readonly IDynamoDBRepository<MultiplePricesItem> dynamoDBRepository;
    public MultiplePricesService(ILogger<AddInstrumentHandler> logger, IOptions<LocalFoldersSettings> localFoldersSettings,
                                 IRecordsReader<MultiplePricesRecord> recordsReader, IDynamoDBRepository<MultiplePricesItem> dynamoDBRepository)
    {
        this.logger = logger ?? throw new ArgumentNullException(nameof(logger));
        this.localFoldersSettings = localFoldersSettings ?? throw new ArgumentNullException(nameof(localFoldersSettings));
        this.recordsReader = recordsReader ?? throw new ArgumentNullException(nameof(recordsReader));
        this.dynamoDBRepository = dynamoDBRepository ?? throw new ArgumentNullException(nameof(dynamoDBRepository));
    }
    public async Task AddMultiplePrices(List<string> instruments)
    {
        var localFolder = localFoldersSettings.Value.MultiplePricesFolder;
        var instrumentFiles = this.GetFilesByInstrument(instruments, localFolder);
        var multiplePrices = await GetMultiplePrices(instrumentFiles);
        var item = multiplePrices.First();
        await dynamoDBRepository.WriteAsync(item);
        Console.WriteLine();
    }

    private List<string> GetFilesByInstrument(List<string> instruments, string localFolder)
    {
        if (instruments.Any())
        {
            return instruments.Select(x => Path.Combine(localFolder, $"{x}.csv"))
                             .Where(File.Exists)
                             .ToList();
        }
        else
        {
            return Directory.GetFiles(localFolder, "*.csv").ToList(); 
        }
    }
    private async Task<IEnumerable<MultiplePricesItem>> GetMultiplePrices(List<string> instrumentFiles)
    {
        var multiplePricesitems = new ConcurrentBag<MultiplePricesItem>();
        var tasks = instrumentFiles.Select(async instrumentFile =>
        {
            var instrumentName = Path.GetFileNameWithoutExtension(instrumentFile);
            this.logger.LogInformation("Loading CSV of " + instrumentName);
            var records = await recordsReader.CreateRecordsAsync(instrumentFile);
            var multiplePricesSeries = records.Select(x => new MultiplePricesItem
            {
                Instrument = instrumentName,
                UnixDateTime = (int)x.DATETIME.Subtract(DateTime.UnixEpoch).TotalSeconds,
                Carry = decimal.TryParse(x.CARRY, out decimal carry) ? carry : null,
                CarryContract = x.CARRY_CONTRACT,
                Price = decimal.TryParse(x.PRICE, out decimal price) ? price : null,
                PriceContract = x.PRICE_CONTRACT,
                Forward = decimal.TryParse(x.FORWARD, out decimal forward) ? forward : null,
                ForwardContract = x.FORWARD_CONTRACT
            });
            foreach (var multiplePrice in multiplePricesSeries)
            {
                multiplePricesitems.Add(multiplePrice);
            }
        });
        await Task.WhenAll(tasks);
        return multiplePricesitems;
    }

}
