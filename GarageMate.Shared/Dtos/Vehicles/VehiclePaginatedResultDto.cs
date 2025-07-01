namespace GarageMate.Shared.Dtos.Vehicles;

public record class VehiclePaginatedResultDto<T>
{
    public int Page { get; init; }
    public int PageSize { get; init; }
    public int TotalCount { get; init; }
    public List<T> Items { get; init; } = [];
}
