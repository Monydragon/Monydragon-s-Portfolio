using Monydragons_Portfolio.Services;

namespace Monydragons_Portfolio.BuildConsole;

public class Program
{
    public static async Task Main()
    {
        // Navigate up three levels from the current working directory to get to the project root directory
        var projectRoot = Directory.GetParent(Directory.GetCurrentDirectory()).Parent.Parent.FullName;
        var blogDirectory = Path.Combine(projectRoot, "blog");

        // Ensure the blog directory exists
        if (!Directory.Exists(blogDirectory))
        {
            Directory.CreateDirectory(blogDirectory);
        }

        // Instantiate the BlogContentService
        var service = new BlogContentService();

        // Generate the blog content JSON file
        await service.GenerateBlogContentJsonAsync(blogDirectory);

        Console.WriteLine($"Blog content JSON generated in {blogDirectory}.");
    }
}