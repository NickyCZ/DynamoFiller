using Amazon.DynamoDBv2.DataModel;

namespace PublicApi.MultiplePrices.Entities;

[DynamoDBTable("MultiplePrices")]
public record MultiplePricesItem
{
    [DynamoDBHashKey]
    public string Instrument { get; set; } = string.Empty;
    [DynamoDBRangeKey]
    public int UnixDateTime { get; set; }
    [DynamoDBProperty]
    public decimal? Carry { get; set; }
    [DynamoDBProperty]
    public int CarryContract { get; set; }
    [DynamoDBProperty]
    public decimal? Price { get; set; }
    [DynamoDBProperty]
    public int PriceContract { get; set; }
    [DynamoDBProperty]
    public decimal? Forward { get; set; }
    [DynamoDBProperty]
    public int ForwardContract { get; set; }
}
