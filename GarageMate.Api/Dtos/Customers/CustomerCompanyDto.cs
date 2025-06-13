namespace GarageMate.Api.Dtos.Customers;

public record class CustomerCompanyDto
{
    public string CompanyName { get; init; } = string.Empty;

    public string? ContactPerson { get; init; }

    public string? Position { get; init; }
}
