namespace GarageMate.Api.Dtos.Customers;

public record class CustomerPaginatedResultDto<T>
{
    public int Page { get; init; }
    public int PageSize { get; init; }
    public int TotalCount { get; init; }
    public List<T> Items { get; init; } = [];
}
