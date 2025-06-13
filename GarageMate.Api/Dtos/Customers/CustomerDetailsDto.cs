using GarageMate.Api.Enums;

namespace GarageMate.Api.Dtos.Customers;

public record class CustomerDetailsDto
{
    public int Id { get; init; }
    public CustomerType Type { get; init; }

    public string Email { get; init; } = string.Empty;

    public string PhoneNumber { get; init; } = string.Empty;

    public string? Address { get; init; }

    public string? Notes { get; init; }

    public CustomerIndividualDto? Individual { get; init; }
    public CustomerCompanyDto? Company { get; init; }
}
