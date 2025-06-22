using System.ComponentModel.DataAnnotations;
using GarageMate.Api.Enums;

namespace GarageMate.Api.Dtos.Customers;

public record class CustomerStatusUpdateDto
{
    [Required]
    public CustomerStatus Status { set; get; }
}
