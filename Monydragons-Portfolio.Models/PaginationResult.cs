namespace Monydragon_Portfolios.Models;

public class PaginationResult<T>
{
    public IEnumerable<T> Items { get; set; }
    public int TotalItems { get; set; }
    public int PageSize { get; set; }
    public int TotalPages { get; set; }
    public int CurrentPage { get; set; }
}