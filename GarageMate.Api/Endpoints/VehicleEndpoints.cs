using GarageMate.Api.Data;
using GarageMate.Api.Dtos.Vehicles;
using GarageMate.Api.Helpers;
using GarageMate.Api.Mapping;
using Microsoft.EntityFrameworkCore;

namespace GarageMate.Api.Endpoints;

public static class VehicleEndpoints
{
    const string GetVehicleEndpointName = "GetVehicle";
    public static RouteGroupBuilder MapVehicleEndpoints(this WebApplication app)
    {
        var group = app.MapGroup("api/vehicles");

        /// <summary>
        /// GET: api/vehicles
        /// Retrieves a paginated list of all vehicles with current ownership info.
        /// </summary>
        group.MapGet("/", async (
            GarageMateContext dbContext,
            int page = 1,
            int pageSize = 10) =>
        {
            if (page < 1) page = 1;
            if (pageSize < 1 || pageSize > 100) pageSize = 10;

            var totalCount = await dbContext.Vehicles.CountAsync();

            var vehicles = await dbContext.Vehicles
                .OrderBy(v => v.Id)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .Include(v => v.VehicleOwnerships)
                    .ThenInclude(vo => vo.Customer)
                        .ThenInclude(c => c.IndividualCustomer)
                .Include(v => v.VehicleOwnerships)
                    .ThenInclude(vo => vo.Customer)
                        .ThenInclude(c => c.CompanyCustomer)
                .Select(v => v.ToVehicleDetailsDto())
                .ToListAsync();

            var result = new VehiclePaginatedResultDto<VehicleDetailsDto>
            {
                Page = page,
                PageSize = pageSize,
                TotalCount = totalCount,
                Items = vehicles
            };

            return Results.Ok(result);
        });

        /// <summary>
        /// GET: api/vehicles/{id}
        /// Retrieves detailed information about a specific vehicle, including its current owner.
        /// </summary>
        group.MapGet("/{id}", async (
            int id,
            GarageMateContext dbContext,
            HttpContext http) =>
        {
            var vehicle = await dbContext.Vehicles
                .Include(v => v.VehicleOwnerships)
                    .ThenInclude(vo => vo.Customer)
                        .ThenInclude(c => c.IndividualCustomer)
                .Include(v => v.VehicleOwnerships)
                    .ThenInclude(vo => vo.Customer)
                        .ThenInclude(c => c.CompanyCustomer)
                .FirstOrDefaultAsync(v => v.Id == id);
            if (vehicle is null)
                return ValidationHelper.ValidateNotFound(vehicle, "Vehicle", http.Request);

            return Results.Ok(vehicle!.ToVehicleDetailsDto());
        })
        .WithName(GetVehicleEndpointName);

        /// <summary>
        /// POST: api/vehicles
        /// Creates a new vehicle record. VIN and PlateNumber must be unique.
        /// </summary>
        group.MapPost("/", async (
            VehicleCreateDto newVehicle,
            GarageMateContext dbContext,
            HttpContext http) =>
        {
            var vinExists = await dbContext.Vehicles
                .AnyAsync(v => v.Vin == newVehicle.Vin || v.PlateNumber == newVehicle.PlateNumber);
            if (vinExists)
                return ValidationHelper.ValidateDuplicateRecord(vinExists, "Vehicle Detail", http.Request);

            var validationResult = ValidationHelper.ValidateDto(newVehicle, http.Request);
            if (validationResult is not null)
                return validationResult;

            var vehicle = newVehicle.ToEntity();
            dbContext.Vehicles.Add(vehicle);

            return await DbContextHelper.TrySaveChangesAsync(
                dbContext,
                http,
                () => Task.FromResult(Results.CreatedAtRoute(
                        GetVehicleEndpointName,
                        new { id = vehicle.Id },
                        vehicle.ToVehicleDetailsDto()
                ))
            );
        });

        /// <summary>
        /// PUT: api/vehicles/{id}
        /// Updates details of an existing vehicle.
        /// </summary>
        group.MapPut("/{id}", async (
            int id,
            VehicleDetailsUpdateDto updatedVehicle,
            GarageMateContext dbContext,
            HttpContext http
        ) =>
        {
            var vehicle = await dbContext.Vehicles.FindAsync(id);
            if (vehicle is null)
                return ValidationHelper.ValidateNotFound(vehicle, "Vehicle", http.Request);

            var validationResult = ValidationHelper.ValidateDto(updatedVehicle, http.Request);
            if (validationResult is not null)
                return validationResult;

            vehicle!.UpdateVehicleDetails(updatedVehicle);

            return await DbContextHelper.TrySaveChangesAsync(
                dbContext,
                http,
                () => Task.FromResult(Results.Ok(vehicle!.ToVehicleDetailsDto()))
            );
        });


        return group;
    }
}
