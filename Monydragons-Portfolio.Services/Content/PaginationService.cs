using Monydragon_Portfolios.Models;
using Monydragons_Portfolio.Services.Content.Interface;

namespace Monydragons_Portfolio.Services;

public class PaginationService : IPaginationService
{
    public PaginationResult<T> Paginate<T>(IEnumerable<T> items, int currentPage, int pageSize)
    {
        var pagedItems = items.Skip((currentPage - 1) * pageSize).Take(pageSize);
        int totalItems = items.Count();
        int totalPages = (int)Math.Ceiling(totalItems / (double)pageSize);

        return new PaginationResult<T>
        {
            Items = pagedItems,
            TotalPages = totalPages,
            CurrentPage = currentPage,
            PageSize = pageSize
        };
    }

    // public PaginationResult<BlogPost> Paginate(IEnumerable<BlogPost> allItems, int currentPage, int pageSize)
    // {
    //     var totalItems = allItems.Count();
    //     var totalPages = (int)Math.Ceiling(totalItems / (double)pageSize);
    //     var itemsOnPage = allItems.Skip((currentPage - 1) * pageSize).Take(pageSize).ToList();
    //
    //     return new PaginationResult<BlogPost>
    //     {
    //         Items = itemsOnPage,
    //         CurrentPage = currentPage,
    //         PageSize = pageSize,
    //         TotalPages = totalPages,
    //         TotalItems = totalItems
    //     };
    // }
    
    public async Task<IEnumerable<T>> GetPage<T>(IEnumerable<T> allItems, int currentPage, int pageSize)
    {
        return allItems.Skip((currentPage - 1) * pageSize).Take(pageSize);
    }

    public int GetTotalPages(int totalItems, int pageSize)
    {
        return (int)Math.Ceiling((double)totalItems / pageSize);
    }
}