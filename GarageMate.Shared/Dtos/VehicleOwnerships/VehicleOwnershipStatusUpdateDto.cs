namespace GarageMate.Shared.Dtos.VehicleOwnerships;

public record class VehicleOwnershipStatusUpdateDto
{
    public bool IsCurrentOwner { get; init; }
}
