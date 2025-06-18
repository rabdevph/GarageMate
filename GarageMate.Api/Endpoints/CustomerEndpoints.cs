using GarageMate.Api.Data;
using GarageMate.Api.Dtos.Customers;
using GarageMate.Api.Enums;
using GarageMate.Api.Extensions;
using GarageMate.Api.Helpers;
using GarageMate.Api.Mapping;
using GarageMate.Api.Models;
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

        group.MapPost("/", async (CustomerCreateDto newCustomer, GarageMateContext dbContext, HttpContext http) =>
        {
            if (newCustomer.Type == CustomerType.Individual && newCustomer.Individual is null)
            {
                return ApiProblemHelper.CreateProblem(
                    "urn:problem-type:missing-customer-subtype-details",
                    "Missing Required Customer Subtype Details",
                    StatusCodes.Status400BadRequest,
                    "Individual customer details are required.",
                    http.Request.Path
                );
            }
            if (newCustomer.Type == CustomerType.Company && newCustomer.Company is null)
            {
                return ApiProblemHelper.CreateProblem(
                    "urn:problem-type:missing-customer-subtype-details",
                    "Missing Required Customer Subtype Details",
                    StatusCodes.Status400BadRequest,
                    "Company customer details are required.",
                    http.Request.Path
                );
            }

            var contactInfoExists = await dbContext.Customers
                .AnyAsync(c => c.Email == newCustomer.Email || c.PhoneNumber == newCustomer.PhoneNumber);
            var validationResult = ValidationHelper.ValidateDuplicateRecord(contactInfoExists, "Contact Detail", http.Request);
            if (validationResult is not null) return validationResult;

            validationResult = ValidationHelper.ValidateDto(newCustomer, http.Request);
            if (validationResult is not null) return validationResult;

            var customer = newCustomer.ToEntity();
            dbContext.Customers.Add(customer);

            return await DbContextHelper.TrySaveChangesAsync(
                dbContext,
                http,
                () =>
                {
                    return Task.FromResult(Results.CreatedAtRoute(
                        GetCustomerEndpointName,
                        new { id = customer.Id },
                        customer.ToCustomerDetailsDto()
                    ));
                }
            );
        });

        group.MapPut("/{id}", async (int id, CustomerDetailsUpdateDto updatedCustomer, GarageMateContext dbContext) =>
        {
            var customer = await dbContext.Customers
                .Include(c => c.IndividualCustomer)
                .Include(c => c.CompanyCustomer)
                .FirstOrDefaultAsync(c => c.Id == id);

            if (customer is null) return Results.NotFound();

            customer.UpdateCustomerDetails(updatedCustomer);
            await dbContext.SaveChangesAsync();

            return Results.Ok(customer.ToCustomerDetailsDto());
        });

        group.MapPatch("/{id}/status", async (int id, CustomerStatusUpdateDto updatedStatus, GarageMateContext dbContext) =>
        {
            var customer = await dbContext.Customers
                .Include(c => c.IndividualCustomer)
                .Include(c => c.CompanyCustomer)
                .FirstOrDefaultAsync(c => c.Id == id);

            if (customer is null) return Results.NotFound();

            customer.UpdateCustomerStatus(updatedStatus);
            await dbContext.SaveChangesAsync();

            return Results.Ok(customer.ToCustomerDetailsDto());
        });

        return group;
    }
}
