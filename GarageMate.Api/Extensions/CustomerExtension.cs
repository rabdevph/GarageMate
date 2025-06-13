using GarageMate.Api.Dtos.Customers;
using GarageMate.Api.Enums;
using GarageMate.Api.Models;

namespace GarageMate.Api.Extensions;

public static class CustomerExtension
{
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
    }
}
