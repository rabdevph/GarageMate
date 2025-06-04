namespace GarageMate.Api.Models;

public class CompanyCustomer
{
    public int CustomerId { get; set; }

    public string CompanyName { get; set; } = string.Empty;

    public string? ContactPerson { get; set; }

    public string? Position { get; set; }
}
