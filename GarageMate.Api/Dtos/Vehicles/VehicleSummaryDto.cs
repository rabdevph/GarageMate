namespace GarageMate.Api.Dtos.Vehicles;

public record class VehicleSummaryDto
{
    public int Id { get; set; }
    public string PlateNumber { get; set; } = string.Empty;
    public string Make { get; set; } = string.Empty;
    public string Model { get; set; } = string.Empty;
    public int Year { get; set; }
    public string? Color { get; set; }
    public bool IsCurrentOwner { get; set; }
}
