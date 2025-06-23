namespace GarageMate.Api.Dtos.Vehicles;

public record class VehicleDetailsDto
{
    public int Id { get; init; }

    public string PlateNumber { get; init; } = string.Empty;

    public string Make { get; init; } = string.Empty;

    public string Model { get; init; } = string.Empty;

    public int Year { get; init; }

    public string? Color { get; init; }

    public string Vin { get; init; } = string.Empty;

    public string? Notes { get; init; }

    public VehicleOwnerDto? CurrentOwner { get; init; }
}
