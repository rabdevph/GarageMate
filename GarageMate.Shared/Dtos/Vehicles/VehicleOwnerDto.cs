namespace GarageMate.Shared.Dtos.Vehicles;

public record class VehicleOwnerDto
{
    public int CustomerId { get; init; }
    public string Name { get; init; } = string.Empty;
}
