using System.ComponentModel.DataAnnotations;
using GarageMate.Shared.Enums;

namespace GarageMate.Shared.Dtos.Customers;

public record class CustomerStatusUpdateDto
{
    [Required]
    public CustomerStatus Status { set; get; }
}
