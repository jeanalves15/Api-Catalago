namespace Catalogo_Api.Pagination;

public abstract class QueryStringParameters
{
    const int PagMaxSize = 50;
    public int PageNumber { get; set; } = 1;

    private int _pageSize = PagMaxSize;
    public int PageSize
    {
        get { return _pageSize; }
        set { _pageSize = (value > PagMaxSize) ? PagMaxSize : value; }
    }



}
