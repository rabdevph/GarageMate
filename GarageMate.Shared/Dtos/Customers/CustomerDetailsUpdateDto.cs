using System.ComponentModel.DataAnnotations;
using GarageMate.Shared.Enums;

namespace GarageMate.Shared.Dtos.Customers;

public record class CustomerDetailsUpdateDto
{
    [Required]
    public CustomerType Type { get; init; }

    [Required]
    [EmailAddress]
    [MaxLength(256)]
    public string Email { get; init; } = string.Empty;

    [Required]
    [MaxLength(20)]
    public string PhoneNumber { get; init; } = string.Empty;

    [MaxLength(512)]
    public string? Address { get; init; }

    [MaxLength(1024)]
    public string? Notes { get; init; }

    public CustomerIndividualDto? Individual { get; init; }
    public CustomerCompanyDto? Company { get; init; }
}
