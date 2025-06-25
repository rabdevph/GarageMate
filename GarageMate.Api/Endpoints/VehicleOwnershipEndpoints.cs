using GarageMate.Api.Data;
using GarageMate.Api.Dtos.VehicleOwnerships;
using GarageMate.Api.Helpers;
using GarageMate.Api.Mapping;
using Microsoft.EntityFrameworkCore;

namespace GarageMate.Api.Endpoints;

public static class VehicleOwnershipEndpoints
{
    const string GetVehicleOwnershipEndpointName = "GetVehicleOwnership";
    public static RouteGroupBuilder MapVehicleOwnershipEndpoints(this WebApplication app)
    {
        var group = app.MapGroup("api/vehicle-ownerships");

        /// <summary>
        /// GET: api/vehicle-ownerships/{id}
        /// Retrieves the details of a specific vehicle ownership record by its ID.
        /// </summary>
        group.MapGet("/{id}", async (int id, GarageMateContext dbContext, HttpContext http) =>
        {
            var ownership = await dbContext.VehicleOwnerships.FindAsync(id);
            if (ownership is null)
                return ValidationHelper.ValidateNotFound(ownership, "Vehicle Ownership", http.Request);

            return Results.Ok(ownership!.ToVehicleOwnershipDetailsDto());
        })
        .WithName(GetVehicleOwnershipEndpointName);

        /// <summary>
        /// POST: api/vehicle-ownerships
        /// Creates a new vehicle ownership entry.
        /// </summary>
        group.MapPost("/", async (
            VehicleOwnershipCreateDto newOwnership,
            GarageMateContext dbContext,
            HttpContext http) =>
        {
            var customer = await dbContext.Customers.FindAsync(newOwnership.CustomerId);
            if (customer is null)
                return ValidationHelper.ValidateNotFound(customer, "Customer", http.Request);

            var vehicle = await dbContext.Vehicles.FindAsync(newOwnership.VehicleId);
            if (vehicle is null)
                return ValidationHelper.ValidateNotFound(vehicle, "Vehicle", http.Request);

            var ownershipExists = await dbContext.VehicleOwnerships
                .AnyAsync(vo => vo.CustomerId == newOwnership.CustomerId && vo.VehicleId == newOwnership.VehicleId);
            if (ownershipExists)
                return ValidationHelper.ValidateDuplicateRecord(ownershipExists, "Ownership", http.Request);

            var vehicleOwnership = newOwnership.ToEntity();
            dbContext.VehicleOwnerships.Add(vehicleOwnership);

            return await DbContextHelper.TrySaveChangesAsync(
                dbContext,
                http,
                () => Task.FromResult(Results.CreatedAtRoute(
                        GetVehicleOwnershipEndpointName,
                        new { id = vehicleOwnership.Id },
                        vehicleOwnership.ToVehicleOwnershipDetailsDto()
                ))
            );
        });

        /// <summary>
        /// POST: api/vehicle-ownerships/transfer
        /// Transfers ownership of a vehicle to a new customer.
        /// Marks the current owner as no longer the owner and creates a new ownership entry.
        /// </summary>
        group.MapPost("/transfer", async (
            VehicleOwnershipTransferDto transferDto,
            GarageMateContext dbContext,
            HttpContext http) =>
        {
            var vehicle = await dbContext.Vehicles
                .Include(v => v.VehicleOwnerships)
                .FirstOrDefaultAsync(v => v.Id == transferDto.VehicleId);
            if (vehicle is null)
                return ValidationHelper.ValidateNotFound(vehicle, "Vehicle", http.Request);

            var newOwner = await dbContext.Customers.FindAsync(transferDto.NewCustomerId);
            if (newOwner is null)
                return ValidationHelper.ValidateNotFound(newOwner, "Customer", http.Request);

            var currentOwnership = vehicle.VehicleOwnerships.FirstOrDefault(vo => vo.IsCurrentOwner);
            if (currentOwnership is null)
                return ValidationHelper.ValidateNotFound(currentOwnership, "Ownership", http.Request);
            currentOwnership.UpdateVehicleOwnershipStatus(new VehicleOwnershipStatusUpdateDto { IsCurrentOwner = false });

            var newVehicleOwnership = transferDto.ToEntity();
            dbContext.VehicleOwnerships.Add(newVehicleOwnership);

            return await DbContextHelper.TrySaveChangesAsync(
                dbContext,
                http,
                () => Task.FromResult(Results.CreatedAtRoute(
                        GetVehicleOwnershipEndpointName,
                        new { id = newVehicleOwnership.Id },
                        newVehicleOwnership.ToVehicleOwnershipDetailsDto()
                ))
            );
        });

        return group;
    }
}
