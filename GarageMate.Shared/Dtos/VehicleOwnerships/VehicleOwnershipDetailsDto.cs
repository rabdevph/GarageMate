namespace GarageMate.Shared.Dtos.VehicleOwnerships;

public record class VehicleOwnershipDetailsDto
{
    public int Id { get; init; }
    public int VehicleId { get; init; }
    public int CustomerId { get; init; }
    public bool IsCurrentOwner { get; init; }
    public string? Notes { get; init; }
}
