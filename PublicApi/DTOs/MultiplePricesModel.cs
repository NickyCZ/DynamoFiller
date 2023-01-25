using System.ComponentModel.DataAnnotations;

namespace PublicApi.DTOs;

public class MultiplePricesModel
{
    [Required]
    public string? Instrument { get; set; } = default;
}
