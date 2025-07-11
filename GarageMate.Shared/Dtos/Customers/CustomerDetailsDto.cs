using GarageMate.Shared.Dtos.Vehicles;
using GarageMate.Shared.Enums;

namespace GarageMate.Shared.Dtos.Customers;

public record class CustomerDetailsDto
{
    public int Id { get; init; }
    public CustomerType Type { get; init; }

    public string Email { get; init; } = string.Empty;

    public string PhoneNumber { get; init; } = string.Empty;

    public string? Address { get; init; }

    public string? Notes { get; init; }

    public CustomerStatus Status { get; init; }

    public DateTime CreatedAt { get; init; }

    public DateTime? UpdatedAt { get; init; }

    public CustomerIndividualDto? Individual { get; init; }
    public CustomerCompanyDto? Company { get; init; }

    public List<VehicleSummaryDto> OwnedVehicles { get; init; } = [];
}
