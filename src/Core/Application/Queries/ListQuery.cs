namespace Core.Application;

public class ListQuery<TResult> : BaseQuery<ListResults<TResult>>
{
    public int Page { get; set; } = 1;

    public int PageSize { get; set; } = 10;

    public string[]? OrderBy { get; set; }

    public bool Ascending { get; set; } = true;
}
