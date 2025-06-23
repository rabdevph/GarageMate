using GarageMate.Api.Dtos.Vehicles;
using GarageMate.Api.Models;

namespace GarageMate.Api.Extensions;

public static class VehicleExtension
{
    public static void UpdateVehicleDetails(this Vehicle vehicle, VehicleDetailsUpdateDto dto)
    {
        vehicle.PlateNumber = dto.PlateNumber;
        vehicle.Make = dto.Make;
        vehicle.Model = dto.Model;
        vehicle.Year = dto.Year;
        vehicle.Color = dto.Color;
        vehicle.Vin = dto.Vin;
        vehicle.Notes = dto.Notes;
    }
}
