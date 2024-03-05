using Monydragon_Portfolios.Models;

namespace Monydragons_Portfolio.Services.Content.Interface;

public interface IFilteringSortingService
{
    Task<IEnumerable<BlogPost>> ApplyFiltersAsync(IEnumerable<BlogPost> blogPosts, string searchTerm);
    Task<IEnumerable<BlogPost>> SortPostsAsync(IEnumerable<BlogPost> blogPosts, bool descendingOrder);
}