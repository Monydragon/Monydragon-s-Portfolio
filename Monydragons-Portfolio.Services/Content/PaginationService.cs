using Monydragon_Portfolios.Models;
using Monydragons_Portfolio.Services.Content.Interface;

namespace Monydragons_Portfolio.Services;

public class PaginationService : IPaginationService
{
    public PaginationResult<T> Paginate<T>(IEnumerable<T> items, int currentPage, int pageSize)
    {
        var itemsList = items.ToList();
        var pagedItems = itemsList.Skip((currentPage - 1) * pageSize).Take(pageSize);
        int totalItems = itemsList.Count;
        int totalPages = (int)Math.Ceiling(totalItems / (double)pageSize);

        return new PaginationResult<T>
        {
            Items = pagedItems,
            TotalItems = totalItems,
            TotalPages = totalPages,
            CurrentPage = currentPage,
            PageSize = pageSize
        };
    }
    
    public async Task<IEnumerable<T>> GetPage<T>(IEnumerable<T> allItems, int currentPage, int pageSize)
    {
        return allItems.Skip((currentPage - 1) * pageSize).Take(pageSize);
    }

    public int GetTotalPages(int totalItems, int pageSize)
    {
        return (int)Math.Ceiling((double)totalItems / pageSize);
    }
}