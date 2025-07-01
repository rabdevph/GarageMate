namespace GarageMate.Shared.Dtos.VehicleOwnerships;

public record class VehicleOwnershipCreateDto
{
    public int VehicleId { get; init; }
    public int CustomerId { get; init; }
    public bool IsCurrentOwner { get; init; }
    public string? Notes { get; init; }
}
