using GarageMate.Api.Enums;

namespace GarageMate.Api.Dtos.Customers;

public record class CreateCustomerDto
{
    public CustomerType Type { get; init; }

    public string Email { get; init; } = string.Empty;

    public string PhoneNumber { get; init; } = string.Empty;

    public string? Address { get; init; }

    public string? Notes { get; init; }

    public IndividualCustomerDto? Individual { get; init; }
    public CompanyCustomerDto? Company { get; init; }
}
