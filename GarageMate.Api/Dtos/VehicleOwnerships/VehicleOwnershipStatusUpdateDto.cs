namespace GarageMate.Api.Dtos.VehicleOwnerships;

public record class VehicleOwnershipStatusUpdateDto
{
    public bool IsCurrentOwner { get; init; }
}
