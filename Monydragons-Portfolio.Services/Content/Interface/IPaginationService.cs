using Monydragon_Portfolios.Models;

namespace Monydragons_Portfolio.Services.Content.Interface;

public interface IPaginationService
{
    PaginationResult<T> Paginate<T>(IEnumerable<T> items, int currentPage, int pageSize);
    Task<IEnumerable<T>> GetPage<T>(IEnumerable<T> items, int currentPage, int pageSize);
    int GetTotalPages(int totalItems, int pageSize);
}