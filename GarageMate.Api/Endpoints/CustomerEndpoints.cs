using GarageMate.Api.Data;
using GarageMate.Api.Dtos.Customers;
using GarageMate.Api.Enums;
using GarageMate.Api.Mapping;
using Microsoft.EntityFrameworkCore;

namespace GarageMate.Api.Endpoints;

public static class CustomerEndpoints
{
    public static RouteGroupBuilder MapCustomerEndpoints(this WebApplication app)
    {
        var group = app.MapGroup("api/customers");

        group.MapGet("/", async (CustomerType? type, GarageMateContext dbContext) =>
        {
            var query = dbContext.Customers.AsQueryable();

            if (type.HasValue)
            {
                query = query.Where(c => c.Type == type.Value);
            }

            var customers = await query
                .Include(c => c.IndividualCustomer)
                .Include(c => c.CompanyCustomer)
                .ToListAsync();

            var result = customers.Select(c => c.ToCustomerDetailsDto());
            return Results.Ok(result);
        });

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
