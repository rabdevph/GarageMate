using GarageMate.Api.Enums;

namespace GarageMate.Api.Dtos.Customers;

public record class CustomerStatusUpdateDto
{
    public CustomerStatus Status { set; get; }
}
