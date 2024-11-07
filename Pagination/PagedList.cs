namespace Catalogo_Api.Pagination;

public class PagedList<T>:List<T>  where T: class
{
    public int CurrentPage { get;private set; }
    public int TotalPage { get;private set; } 
    public int PageSize { get;private set; }
    public int TotalCount { get; private set;}
    public bool HasPrevious => CurrentPage > 1;
    public bool HasNext => CurrentPage < TotalCount;

    public PagedList(List<T> items,int count,int pageNumber,int pageSize)
    {
        TotalCount = count;
        PageSize = pageSize;
        CurrentPage = pageNumber;
        TotalPage=(int)Math.Ceiling(count/(double)pageSize);

        AddRange(items);

    }

    public static PagedList<T> ToPagedList(IQueryable<T> source,int pageNumber,int pageSize)
    {
        var count=source.Count();
        var itens=source.Skip((pageNumber -1)* pageSize).Take(pageSize).ToList();

        return new PagedList<T>(itens, count,pageNumber, pageSize);


    }
}
