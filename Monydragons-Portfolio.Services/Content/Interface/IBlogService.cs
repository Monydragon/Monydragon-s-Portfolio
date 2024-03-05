using Monydragon_Portfolios.Models;

namespace Monydragons_Portfolio.Services.Content.Interface;

public interface IBlogService
{
    Task<List<BlogPost>> GetBlogPostsAsync();
    Task<string> GetContentFileAsync(string contentFile);
}