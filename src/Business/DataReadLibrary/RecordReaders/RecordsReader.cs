using CsvHelper;
using CsvHelper.Configuration;
using Domain.Services;
using System.Globalization;

namespace DataReadLibrary.RecordReaders;

public class RecordsReader<T>: IRecordsReader<T>
{
    public async Task<IEnumerable<T>> CreateRecordsAsync(string rawInstrumentPath, CancellationToken cancellationToken = default)
    {
        CsvConfiguration config = new(CultureInfo.InvariantCulture)
        {
            PrepareHeaderForMatch = args => args.Header.ToLower(),
        };
        using var stream = new BufferedStream(new FileStream(rawInstrumentPath, FileMode.Open, FileAccess.Read, FileShare.Read, bufferSize: 4096, useAsync: true));
        using var csv = new CsvReader(new StreamReader(stream), config);
        var records = csv.GetRecordsAsync<T>(cancellationToken);

        var syncSequence = await records.ToListAsync(cancellationToken);
        return syncSequence;
    }
}
