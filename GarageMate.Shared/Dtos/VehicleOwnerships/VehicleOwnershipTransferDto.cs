namespace GarageMate.Shared.Dtos.VehicleOwnerships;

public record class VehicleOwnershipTransferDto
{
    public int VehicleId { get; init; }
    public int NewCustomerId { get; init; }
    public bool IsCurrentOwner { get; init; }
    public string? Notes { get; init; }
}
