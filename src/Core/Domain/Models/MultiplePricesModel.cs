namespace Domain.Models;

public record MultiplePricesModel
{
    public DateTime DATETIME { get; set; }
    public string CARRY { get; set; } = string.Empty;
    public int CARRY_CONTRACT { get; set; }
    public string PRICE { get; set; } = string.Empty;
    public int PRICE_CONTRACT { get; set; }
    public string FORWARD { get; set; } = string.Empty;
    public int FORWARD_CONTRACT { get; set; }
}
