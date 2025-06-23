using System.ComponentModel.DataAnnotations;

namespace GarageMate.Api.Dtos.Vehicles;

public record class VehicleDetailsUpdateDto
{
    [Required]
    [MaxLength(20)]
    public string PlateNumber { get; init; } = string.Empty;

    [Required]
    [MaxLength(50)]
    public string Make { get; init; } = string.Empty;

    [Required]
    [MaxLength(50)]
    public string Model { get; init; } = string.Empty;

    public int Year { get; init; }

    public string? Color { get; init; }

    [Required]
    [MaxLength(50)]
    public string Vin { get; init; } = string.Empty;

    public string? Notes { get; init; }
}
