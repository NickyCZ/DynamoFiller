namespace PublicApi.Records;

public record MultiplePrices
{
    public string Instrument { get; set; } = string.Empty;
    public int UnixDateTime { get; set; }
    public decimal? Carry { get; set; }
    public int CarryContract { get; set; }
    public decimal? Price { get; set; }
    public int PriceContract { get; set; }
    public decimal? Forward { get; set; }
    public int ForwardContract { get; set; }
}
