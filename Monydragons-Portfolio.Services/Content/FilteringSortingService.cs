using Monydragon_Portfolios.Models;
using Monydragons_Portfolio.Services.Content.Interface;

namespace Monydragons_Portfolio.Services;

public class FilteringSortingService : IFilteringSortingService
{
    public async Task<IEnumerable<BlogPost>> ApplyFiltersAsync(IEnumerable<BlogPost> blogPosts, string searchTerm)
    {
        if (string.IsNullOrWhiteSpace(searchTerm))
        {
            return blogPosts;
        }

        var filteredPosts = blogPosts.Where(post =>
            post.Title.Contains(searchTerm, StringComparison.OrdinalIgnoreCase) ||
            post.ContentFiles.Any(content => content.Contains(searchTerm, StringComparison.OrdinalIgnoreCase)) ||
            post.ImageFiles.Any(image => image.Contains(searchTerm, StringComparison.OrdinalIgnoreCase)));

        // Assume you might have asynchronous operations to apply to each post
        var tasks = filteredPosts.Select(async post =>
        {
            // Example async transformation (e.g., loading content from a file or API)
            post.ContentFiles = await Task.FromResult(post.ContentFiles); // Placeholder for actual async operation
            return post;
        });

        return await Task.WhenAll(tasks);
    }

    public Task<IEnumerable<BlogPost>> SortPostsAsync(IEnumerable<BlogPost> blogPosts, bool descendingOrder)
    {
        var sortedPosts = descendingOrder
            ? blogPosts.OrderByDescending(post => post.Date)
            : blogPosts.OrderBy(post => post.Date);

        return Task.FromResult(sortedPosts.AsEnumerable());
    }

}
