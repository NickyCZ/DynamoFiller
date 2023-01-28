using System.ComponentModel.DataAnnotations;

namespace PublicApi.MultiplePrices.Models;

public class MultiplePricesModel
{
    [Required]
    public List<string> Instruments { get; set; } = new List<string>();
}