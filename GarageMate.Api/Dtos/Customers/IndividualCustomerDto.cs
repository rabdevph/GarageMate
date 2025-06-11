namespace GarageMate.Api.Dtos.Customers;

public record class IndividualCustomerDto
{
    public string FirstName { get; init; } = string.Empty;

    public string LastName { get; init; } = string.Empty;
}
