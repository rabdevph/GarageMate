using GarageMate.Api.Dtos.Vehicles;
using GarageMate.Api.Enums;
using GarageMate.Api.Models;

namespace GarageMate.Api.Mapping;

public static class VehicleMapping
{
    public static Vehicle ToEntity(this VehicleCreateDto dto)
    {
        var vehicle = new Vehicle
        {
            PlateNumber = dto.PlateNumber,
            Make = dto.Make,
            Model = dto.Model,
            Year = dto.Year,
            Color = dto.Color,
            Vin = dto.Vin,
            Notes = dto.Notes
        };

        return vehicle;
    }

    public static VehicleDetailsDto ToVehicleDetailsDto(this Vehicle vehicle)
    {
        var currentOwnership = vehicle.VehicleOwnerships.FirstOrDefault(vo => vo.IsCurrentOwner);

        VehicleOwnerDto? ownerDto = null;

        if (currentOwnership?.Customer is not null)
        {
            var customer = currentOwnership.Customer;
            var name = customer.Type == CustomerType.Individual
                ? $"{customer.IndividualCustomer?.FirstName} {customer.IndividualCustomer?.LastName}"
                : customer.CompanyCustomer?.CompanyName ?? "Unknown";

            ownerDto = new VehicleOwnerDto
            {
                CustomerId = customer.Id,
                Name = name
            };
        }

        return new VehicleDetailsDto
        {
            Id = vehicle.Id,
            PlateNumber = vehicle.PlateNumber,
            Make = vehicle.Make,
            Model = vehicle.Model,
            Year = vehicle.Year,
            Color = vehicle.Color,
            Vin = vehicle.Vin,
            Notes = vehicle.Notes,
            CurrentOwner = ownerDto
        };
    }
}
