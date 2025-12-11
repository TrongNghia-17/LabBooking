namespace LabBooking.Application.Common.Wrappers;

public class PagedResult<T>
{
    public PagedResult()
    {
    }

    public PagedResult(IEnumerable<T> items, int totalCount, int pageSize, int pageNumber)
    {
        Items = items;
        TotalItemsCount = totalCount;
        TotalPages = (int)Math.Ceiling(totalCount / (double)pageSize);
        ItemsFrom = pageSize * (pageNumber - 1) + 1;
        ItemsTo = ItemsFrom + items.Count() - 1;
    }

    public IEnumerable<T> Items { get; init; } = [];
    public int TotalPages { get; init; }
    public int TotalItemsCount { get; init; }
    public int ItemsFrom { get; init; }
    public int ItemsTo { get; init; }
}
