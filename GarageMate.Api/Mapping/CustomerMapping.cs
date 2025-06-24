using GarageMate.Api.Dtos.Customers;
using GarageMate.Api.Dtos.Vehicles;
using GarageMate.Api.Enums;
using GarageMate.Api.Models;

namespace GarageMate.Api.Mapping;

public static class CustomerMapping
{
    public static Customer ToEntity(this CustomerCreateDto dto)
    {
        var customer = new Customer
        {
            Type = dto.Type,
            Email = dto.Email,
            PhoneNumber = dto.PhoneNumber,
            Address = dto.Address,
            Notes = dto.Notes,
            IndividualCustomer = dto.Type == CustomerType.Individual && dto.Individual is not null
                ? new IndividualCustomer
                {
                    FirstName = dto.Individual.FirstName,
                    LastName = dto.Individual.LastName
                }
                :
                null,
            CompanyCustomer = dto.Type == CustomerType.Company && dto.Company is not null
                ? new CompanyCustomer
                {
                    CompanyName = dto.Company.CompanyName,
                    ContactPerson = dto.Company.ContactPerson,
                    Position = dto.Company.Position
                }
                :
                null,
        };

        return customer;
    }

    public static CustomerDetailsDto ToCustomerDetailsDto(this Customer customer)
    {
        return new CustomerDetailsDto
        {
            Id = customer.Id,
            Type = customer.Type,
            Email = customer.Email,
            PhoneNumber = customer.PhoneNumber,
            Address = customer.Address,
            Notes = customer.Notes,
            Status = customer.Status,
            CreatedAt = customer.CreatedAt,
            UpdatedAt = customer.UpdatedAt,
            Individual = customer.IndividualCustomer is not null
                ? new CustomerIndividualDto
                {
                    FirstName = customer.IndividualCustomer.FirstName,
                    LastName = customer.IndividualCustomer.LastName
                }
                :
                null,
            Company = customer.CompanyCustomer is not null
                ? new CustomerCompanyDto
                {
                    CompanyName = customer.CompanyCustomer.CompanyName,
                    ContactPerson = customer.CompanyCustomer.ContactPerson,
                    Position = customer.CompanyCustomer.Position
                }
                :
                null,
            OwnedVehicles = []
        };
    }

    public static CustomerDetailsDto ToCustomerDetailsDto(this Customer customer, bool onlyCurrent = false)
    {
        var ownerships = onlyCurrent
            ? customer.VehicleOwnerships.Where(vo => vo.IsCurrentOwner)
            : customer.VehicleOwnerships;

        return new CustomerDetailsDto
        {
            Id = customer.Id,
            Type = customer.Type,
            Email = customer.Email,
            PhoneNumber = customer.PhoneNumber,
            Address = customer.Address,
            Notes = customer.Notes,
            Status = customer.Status,
            CreatedAt = customer.CreatedAt,
            UpdatedAt = customer.UpdatedAt,
            Individual = customer.IndividualCustomer is not null
                ? new CustomerIndividualDto
                {
                    FirstName = customer.IndividualCustomer.FirstName,
                    LastName = customer.IndividualCustomer.LastName
                }
                :
                null,
            Company = customer.CompanyCustomer is not null
                ? new CustomerCompanyDto
                {
                    CompanyName = customer.CompanyCustomer.CompanyName,
                    ContactPerson = customer.CompanyCustomer.ContactPerson,
                    Position = customer.CompanyCustomer.Position
                }
                :
                null,
            OwnedVehicles = [.. ownerships
                .Select(vo => new VehicleSummaryDto
                {
                    Id = vo.Vehicle.Id,
                    PlateNumber = vo.Vehicle.PlateNumber,
                    Make = vo.Vehicle.Make,
                    Model = vo.Vehicle.Model,
                    Year = vo.Vehicle.Year,
                    Color = vo.Vehicle.Color,
                    IsCurrentOwner = vo.IsCurrentOwner
                })]
        };
    }

    public static void UpdateCustomerDetails(this Customer customer, CustomerDetailsUpdateDto dto)
    {
        customer.Type = dto.Type;
        customer.Email = dto.Email;
        customer.PhoneNumber = dto.PhoneNumber;
        customer.Address = dto.Address;
        customer.Notes = dto.Notes;

        if (dto.Type == CustomerType.Individual && dto.Individual is not null)
        {
            customer.IndividualCustomer ??= new IndividualCustomer();
            customer.IndividualCustomer.FirstName = dto.Individual.FirstName;
            customer.IndividualCustomer.LastName = dto.Individual.LastName;
            customer.CompanyCustomer = null;
        }

        if (dto.Type == CustomerType.Company && dto.Company is not null)
        {
            customer.CompanyCustomer ??= new CompanyCustomer();
            customer.CompanyCustomer.CompanyName = dto.Company.CompanyName;
            customer.CompanyCustomer.ContactPerson = dto.Company.ContactPerson;
            customer.CompanyCustomer.Position = dto.Company.Position;
            customer.IndividualCustomer = null;
        }

        customer.UpdatedAt = DateTime.UtcNow;
    }

    public static void UpdateCustomerStatus(this Customer customer, CustomerStatusUpdateDto dto)
    {
        customer.Status = dto.Status;
        customer.UpdatedAt = DateTime.UtcNow;
    }
}
