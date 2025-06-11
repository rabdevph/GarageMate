using GarageMate.Api.Data;
using GarageMate.Api.Dtos.Customers;
using GarageMate.Api.Enums;
using GarageMate.Api.Mapping;

namespace GarageMate.Api.Endpoints;

public static class CustomerEndpoints
{
    public static RouteGroupBuilder MapCustomerEndpoints(this WebApplication app)
    {
        var group = app.MapGroup("api/customers");

        group.MapPost("/", async (CreateCustomerDto newCustomer, GarageMateContext dbContext) =>
        {
            if (newCustomer.Type == CustomerType.Individual && newCustomer.Individual is null)
            {
                return Results.BadRequest("Individual customer details are required.");
            }
            if (newCustomer.Type == CustomerType.Company && newCustomer.Company is null)
            {
                return Results.BadRequest("Company customer details are required.");
            }

            var customer = newCustomer.ToEntity();
            dbContext.Customers.Add(customer);
            await dbContext.SaveChangesAsync();

            var dto = customer.ToCustomerDetailsDto();
            return Results.Created($"/api/customers/{customer.Id}", dto);
        });

        return group;
    }
}
