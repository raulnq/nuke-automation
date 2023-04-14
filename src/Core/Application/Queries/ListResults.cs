namespace Core.Application;
public class ListResults<T>
{
    public int Page { get; set; }
    public int PageSize { get; set; }
    public int TotalCount { get; set; }
    public int TotalPages { get; set; }
    public IEnumerable<T> Items { get; set; }
    public bool HasPreviousPage
    {
        get
        {
            return Page > 1;
        }
    }

    public bool HasNextPage
    {
        get
        {
            return Page < TotalPages;
        }
    }

    public ListResults()
    {

    }

    public ListResults(ListQuery<T> query, int totalCount, IEnumerable<T> source)
    {
        Page = query.Page;
        TotalPages = (int)Math.Ceiling(totalCount / (double)query.PageSize);
        PageSize = query.PageSize;
        TotalCount = totalCount;
        Items = source;
    }
}
