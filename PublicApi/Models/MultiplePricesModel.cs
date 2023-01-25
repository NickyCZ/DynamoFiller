namespace PublicApi.Models;

public record MultiplePricesModel
{
    public DateTime DATETIME { get; set; }
    public string? CARRY { get; set; } = default;
    public int CARRY_CONTRACT { get; set; }
    public string? PRICE { get; set; } = default;
    public int PRICE_CONTRACT { get; set; }
    public string? FORWARD { get; set; } = default;
    public int FORWARD_CONTRACT { get; set; }
}
