using GarageMate.Shared.Dtos.VehicleOwnerships;
using GarageMate.Api.Models;

namespace GarageMate.Api.Mapping;

public static class VehicleOwnershipMapping
{
    public static VehicleOwnership ToEntity(this VehicleOwnershipCreateDto dto)
    {
        var vehicleOwnership = new VehicleOwnership
        {
            VehicleId = dto.VehicleId,
            CustomerId = dto.CustomerId,
            IsCurrentOwner = dto.IsCurrentOwner,
            Notes = dto.Notes
        };

        return vehicleOwnership;
    }

    public static VehicleOwnership ToEntity(this VehicleOwnershipTransferDto dto)
    {
        var vehicleOwnership = new VehicleOwnership
        {
            VehicleId = dto.VehicleId,
            CustomerId = dto.NewCustomerId,
            IsCurrentOwner = dto.IsCurrentOwner,
            Notes = dto.Notes
        };

        return vehicleOwnership;
    }

    public static VehicleOwnershipDetailsDto ToVehicleOwnershipDetailsDto(this VehicleOwnership vehicleOwnership)
    {
        return new VehicleOwnershipDetailsDto
        {
            Id = vehicleOwnership.Id,
            VehicleId = vehicleOwnership.VehicleId,
            CustomerId = vehicleOwnership.CustomerId,
            IsCurrentOwner = vehicleOwnership.IsCurrentOwner,
            Notes = vehicleOwnership.Notes
        };
    }

    public static void UpdateVehicleOwnershipStatus(this VehicleOwnership vehicleOwnership, VehicleOwnershipStatusUpdateDto dto)
    {
        vehicleOwnership.IsCurrentOwner = dto.IsCurrentOwner;
    }
}
