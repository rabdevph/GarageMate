using GarageMate.Api.Data;
using GarageMate.Api.Dtos.Customers;
using GarageMate.Api.Enums;
using GarageMate.Api.Mapping;
using Microsoft.EntityFrameworkCore;

namespace GarageMate.Api.Endpoints;

public static class CustomerEndpoints
{
    const string GetCustomerEndpointName = "GetCustomer";
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

        group.MapGet("/{id}", async (int id, GarageMateContext dbContext) =>
        {
            var customer = await dbContext.Customers
                .Include(c => c.IndividualCustomer)
                .Include(c => c.CompanyCustomer)
                .FirstOrDefaultAsync(c => c.Id == id);

            if (customer is null) return Results.NotFound();

            return Results.Ok(customer!.ToCustomerDetailsDto());
        })
        .WithName(GetCustomerEndpointName);

        group.MapPost("/", async (CustomerCreateDto newCustomer, GarageMateContext dbContext) =>
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
            return Results.CreatedAtRoute(GetCustomerEndpointName, new { id = customer.Id }, dto);
        });

        return group;
    }
}
