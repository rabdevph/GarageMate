using GarageMate.Api.Data;
using GarageMate.Shared.Dtos.Customers;
using GarageMate.Shared.Enums;
using GarageMate.Api.Helpers;
using GarageMate.Api.Mapping;
using Microsoft.EntityFrameworkCore;

namespace GarageMate.Api.Endpoints;

public static class CustomerEndpoints
{
    const string GetCustomerEndpointName = "GetCustomer";
    public static RouteGroupBuilder MapCustomerEndpoints(this WebApplication app)
    {
        var group = app.MapGroup("api/customers");

        /// <summary>
        /// GET: api/customers
        /// Retrieves a paginated list of all customers, optionally filtered by customer type (Individual or Company).
        /// </summary>
        group.MapGet("/", async (
            CustomerType? type,
            GarageMateContext dbContext,
            int page = 1,
            int pageSize = 10) =>
        {
            // For debuggin only.
            Console.WriteLine($"🚀 API HIT - Page: {page}, PageSize: {pageSize}, Type: {type?.ToString() ?? "All"}");

            if (page < 1) page = 1;
            if (pageSize < 1 || pageSize > 100) pageSize = 10;

            var query = dbContext.Customers.AsQueryable();

            if (type.HasValue)
            {
                query = query.Where(c => c.Type == type.Value);
            }

            var totalCount = await query.CountAsync();
            var totalPages = (int)Math.Ceiling((double)totalCount / pageSize);

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
                TotalPages = totalPages,
                Items = customers
            };

            return Results.Ok(result);
        });

        /// <summary>
        /// GET: api/customers/{id}
        /// Retrieves full details of specific customer by ID.
        /// Includes individual/company info and owned vehicles.
        /// </summary>
        group.MapGet("/{id}", async (
            int id,
            bool onlyCurrent,
            GarageMateContext dbContext,
            HttpContext http) =>
        {
            var customer = await dbContext.Customers
                .Include(c => c.IndividualCustomer)
                .Include(c => c.CompanyCustomer)
                .Include(c => c.VehicleOwnerships)
                    .ThenInclude(vo => vo.Vehicle)
                .FirstOrDefaultAsync(c => c.Id == id);
            if (customer is null)
                return ValidationHelper.ValidateNotFound(customer, "Customer", http.Request);

            return Results.Ok(customer!.ToCustomerDetailsDto(onlyCurrent));
        })
        .WithName(GetCustomerEndpointName);

        /// <summary>
        /// POST: api/customers
        /// Creates a new customer (individual or company) with required contact and subtype info.
        /// </summary>
        group.MapPost("/", async (
            CustomerCreateDto newCustomer,
            GarageMateContext dbContext,
            HttpContext http) =>
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
            if (contactInfoExists)
                return ValidationHelper.ValidateDuplicateRecord(contactInfoExists, "Contact Detail", http.Request);

            var validationResult = ValidationHelper.ValidateDto(newCustomer, http.Request);
            if (validationResult is not null)
                return validationResult;

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

        /// <summary>
        /// PUT: api/customers/{id}
        /// Updates the contact and subtype details of an existing customer.
        /// </summary>
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
            if (customer is null)
                return ValidationHelper.ValidateNotFound(customer, "Customer", http.Request);

            var validationResult = ValidationHelper.ValidateDto(updatedCustomer, http.Request);
            if (validationResult is not null)
                return validationResult;

            customer!.UpdateCustomerDetails(updatedCustomer);
            await dbContext.SaveChangesAsync();

            return Results.Ok(customer!.ToCustomerDetailsDto());
        });

        /// <summary>
        /// PATCH: api/customers/{id}/status
        /// Updates the active/inactive status of a customer.
        /// </summary>
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
            if (customer is null)
                return ValidationHelper.ValidateNotFound(customer, "Customer", http.Request);

            var validationResult = ValidationHelper.ValidateDto(updatedStatus, http.Request);
            if (validationResult is not null)
                return validationResult;

            customer!.UpdateCustomerStatus(updatedStatus);
            await dbContext.SaveChangesAsync();

            return Results.Ok(customer!.ToCustomerDetailsDto());
        });

        return group;
    }
}
