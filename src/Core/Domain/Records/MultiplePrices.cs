namespace Domain.Records;

public record MultiplePrices
{
    public string Instrument { get; set; } = string.Empty;
    public int UnixDateTime { get; set; }
    public string Carry { get; set; } = string.Empty;
    public int CarryContract { get; set; }
    public string Price { get; set; } = string.Empty;
    public int PriceContract { get; set; }
    public string Forward { get; set; } = string.Empty;
    public int ForwardContract { get; set; }
}
