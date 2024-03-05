using Monydragon_Portfolios.Models;

namespace Monydragons_Portfolio.Services.Content.Interface;

public interface IBlogService
{
    Task<IEnumerable<BlogPost>> FetchPostsAsync(int pageNumber, int pageSize);
    Task<List<BlogPost>> GetBlogPostsAsync();
    Task<string> GetContentFileAsync(string contentFile);
}