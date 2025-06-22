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

        group.MapGet("/", async (CustomerType? type, GarageMateContext dbContext, int page = 1, int pageSize = 5) =>
        {
            if (page < 1) page = 1;
            if (pageSize < 1 || pageSize > 100) pageSize = 10;

            var totalCount = await dbContext.Customers.CountAsync();

            var query = dbContext.Customers.AsQueryable();

            if (type.HasValue)
            {
                query = query.Where(c => c.Type == type.Value);
            }

            var customers = await query
                .OrderBy(c => c.Id)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .Include(c => c.IndividualCustomer)
                .Include(c => c.CompanyCustomer)
                .Select(c => c.ToCustomerDetailsDto())
                .ToListAsync();

            var result = new CustomerPaginatedResultDto<CustomerDetailsDto>
            {
                Page = page,
                PageSize = pageSize,
                TotalCount = totalCount,
                Items = customers
            };

            return Results.Ok(result);
        });

        group.MapGet("/{id}", async (int id, GarageMateContext dbContext, HttpContext http) =>
        {
            var customer = await dbContext.Customers
                .Include(c => c.IndividualCustomer)
                .Include(c => c.CompanyCustomer)
                .FirstOrDefaultAsync(c => c.Id == id);

            var validationResult = ValidationHelper.ValidateNotFound(customer, "Customer", http.Request);
            if (validationResult is not null) return validationResult;

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

        group.MapPut("/{id}", async (
            int id,
            CustomerDetailsUpdateDto updatedCustomer,
            GarageMateContext dbContext,
            HttpContext http) =>
        {
            var customer = await dbContext.Customers
                .Include(c => c.IndividualCustomer)
                .Include(c => c.CompanyCustomer)
                .FirstOrDefaultAsync(c => c.Id == id);

            var validationResult = ValidationHelper.ValidateNotFound(customer, "Customer", http.Request);
            if (validationResult is not null) return validationResult;

            validationResult = ValidationHelper.ValidateDto(updatedCustomer, http.Request);
            if (validationResult is not null) return validationResult;

            customer!.UpdateCustomerDetails(updatedCustomer);
            await dbContext.SaveChangesAsync();

            return Results.Ok(customer!.ToCustomerDetailsDto());
        });

        group.MapPatch("/{id}/status", async (
            int id,
            CustomerStatusUpdateDto updatedStatus,
            GarageMateContext dbContext,
            HttpContext http) =>
        {
            var customer = await dbContext.Customers
                .Include(c => c.IndividualCustomer)
                .Include(c => c.CompanyCustomer)
                .FirstOrDefaultAsync(c => c.Id == id);

            var validationResult = ValidationHelper.ValidateNotFound(customer, "Customer", http.Request);
            if (validationResult is not null) return validationResult;

            validationResult = ValidationHelper.ValidateDto(updatedStatus, http.Request);
            if (validationResult is not null) return validationResult;

            customer!.UpdateCustomerStatus(updatedStatus);
            await dbContext.SaveChangesAsync();

            return Results.Ok(customer!.ToCustomerDetailsDto());
        });

        return group;
    }
}
