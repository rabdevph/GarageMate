using GarageMate.Api.Dtos.Customers;
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
            Notes = dto.Notes
        };

        if (dto.Type == CustomerType.Individual && dto.Individual is not null)
        {
            customer.IndividualCustomer = new IndividualCustomer
            {
                FirstName = dto.Individual.FirstName,
                LastName = dto.Individual.LastName
            };
        }
        else if (dto.Type == CustomerType.Company && dto.Company is not null)
        {
            customer.CompanyCustomer = new CompanyCustomer
            {
                CompanyName = dto.Company.CompanyName,
                ContactPerson = dto.Company.ContactPerson,
                Position = dto.Company.Position
            };
        }

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
                null
        };
    }
}
