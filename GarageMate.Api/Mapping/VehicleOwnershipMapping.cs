using GarageMate.Api.Dtos.VehicleOwnerships;
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
}
