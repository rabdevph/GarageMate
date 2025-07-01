namespace GarageMate.Shared.Dtos.Customers;

public record class CustomerIndividualDto
{
    public string FirstName { get; init; } = string.Empty;

    public string LastName { get; init; } = string.Empty;
}
