using System.ComponentModel.DataAnnotations;

namespace Domain.DTOs;

public class MultiplePricesModel
{
    [Required]
    public string Instrument { get; set; } = string.Empty;
}
