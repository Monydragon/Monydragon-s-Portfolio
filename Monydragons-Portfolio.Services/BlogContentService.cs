using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using Monydragon_Portfolios.Models;
using Newtonsoft.Json;

namespace Monydragons_Portfolio.Services
{
    public class BlogContentService
    {
        private static readonly List<string> DocumentExtensions = new List<string> 
        { 
            ".txt", ".pdf", ".doc", ".docx", ".odt", // ... other document extensions
        };
        private static readonly List<string> ImageExtensions = new List<string> 
        { 
            ".png", ".jpg", ".jpeg", ".gif", ".bmp", ".tiff", ".svg", // ... other image extensions
        };

        public async Task GenerateBlogContentJsonAsync(string targetDirectory)
        {
            // Assuming targetDirectory is the full path to the 'blog' folder inside wwwroot
            var wwwrootDirectory = Directory.GetParent(targetDirectory).FullName;

            // Ensure the target blog directory exists
            if (!Directory.Exists(targetDirectory))
            {
                Directory.CreateDirectory(targetDirectory);
            }

            var blogPosts = new List<BlogPost>();
            // Get all year directories
            var yearDirectories = Directory.GetDirectories(targetDirectory);

            foreach (var yearDirectory in yearDirectories)
            {
                // Get all month directories within the year directory
                var monthDirectories = Directory.GetDirectories(yearDirectory);

                foreach (var monthDirectory in monthDirectories)
                {
                    // Get all post directories within the month directory
                    var postDirectories = Directory.GetDirectories(monthDirectory);

                    foreach (var postDirectory in postDirectories)
                    {
                        var postInfo = new DirectoryInfo(postDirectory);
                        var post = new BlogPost
                        {
                            Title = postInfo.Name,
                            ContentFiles = new List<string>(),
                            ImageFiles = new List<string>(),
                            Date = ExtractDateFromFolderName(postInfo.Name)
                        };

                        var files = Directory.GetFiles(postDirectory).OrderBy(name => name).ToList();
                        foreach (var file in files)
                        {
                            var fileName = Path.GetFileName(file);
                            var extension = Path.GetExtension(file).ToLowerInvariant();
                            // Get the relative path with respect to the 'wwwroot' directory
                            var relativePath = Path.GetRelativePath(wwwrootDirectory, file).Replace("\\", "/"); // Convert to web-friendly path

                            if (DocumentExtensions.Contains(extension))
                            {
                                post.ContentFiles.Add(relativePath); // Prepend 'blog/' to the relative path
                            }
                            else if (ImageExtensions.Contains(extension))
                            {
                                post.ImageFiles.Add(relativePath); // Prepend 'blog/' to the relative path
                            }
                        }

                        blogPosts.Add(post);
                    }
                }
            }

            var jsonContent = JsonConvert.SerializeObject(blogPosts, Formatting.Indented);
            var outputFile = Path.Combine(targetDirectory, "_MonydragonBlogContent.json");
            await File.WriteAllTextAsync(outputFile, jsonContent);
        }


        private DateTime ExtractDateFromFolderName(string folderName)
        {
            var regex = new Regex(@"(\d{4}-\d{2}-\d{2})");
            var match = regex.Match(folderName);
            if (match.Success)
            {
                return DateTime.ParseExact(match.Value, "yyyy-MM-dd", CultureInfo.InvariantCulture);
            }
            return DateTime.MinValue; // Or a sensible default
        }
    }
}
