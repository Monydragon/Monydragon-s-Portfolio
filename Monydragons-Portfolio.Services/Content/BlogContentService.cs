using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;
using Microsoft.AspNetCore.Components;
using Monydragon_Portfolios.Models;
using Monydragons_Portfolio.Services.Content.Interface;
using Newtonsoft.Json;

namespace Monydragons_Portfolio.Services
{
    public class BlogContentService : IContentService
    {
        public static readonly HashSet<string> DocumentExtensions =
        [
            ".txt", ".pdf", ".doc", ".docx", ".odt" // ... other document extensions
        ];
        public static readonly HashSet<string> ImageExtensions =
        [
            ".png", ".jpg", ".jpeg", ".gif", ".bmp", ".tiff", ".svg" // ... other image extensions
        ];
        
        public static readonly HashSet<string> videoExtensions = 
        [
            ".mp4", ".webm", ".ogg", ".mov", ".avi", ".wmv", ".flv", ".mkv"
        ];
        
        public static readonly Dictionary<string, string> videoDomains = new()
        {
            { "youtube.com", "https://www.youtube.com/embed/" },
            { "youtu.be", "https://www.youtube.com/embed/" },
            { "vimeo.com", "https://player.vimeo.com/video/" },
            { "tiktok.com", "https://www.tiktok.com/embed/" },
            { "facebook.com", "https://www.facebook.com/plugins/video.php?href=" },
            { "instagram.com", "https://www.instagram.com/p/" }
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


        public DateTime ExtractDateFromFolderName(string folderName)
        {
            var regex = new Regex(@"(\d{4}-\d{2}-\d{2})");
            var match = regex.Match(folderName);
            if (match.Success)
            {
                return DateTime.ParseExact(match.Value, "yyyy-MM-dd", CultureInfo.InvariantCulture);
            }
            return DateTime.MinValue; // Or a sensible default
        }

        public MarkupString ProcessContent(string content)
        {
            // Define a regex pattern to match URLs
            var urlPattern = @"(http[s]?:\/\/[^\s]+)";
        
            // Use a StringBuilder to build the new content
            var contentBuilder = new StringBuilder();

            // Use Regex to split the content by URLs, keeping the URLs in the result array
            var parts = Regex.Split(content, urlPattern);

            foreach (var part in parts)
            {
                if (Uri.IsWellFormedUriString(part, UriKind.Absolute))
                {
                    if (IsVideoUrl(part))
                    {
                        // If it's a video URL, embed the video
                        contentBuilder.Append(EmbedVideo(part));
                    }
                    else
                    {
                        // If it's a regular URL, hyperlink it
                        contentBuilder.AppendFormat("<a href=\"{0}\" target=\"_blank\">{0}</a>", part);
                    }
                }
                else
                {
                    // If it's not a URL, add the text as is
                    contentBuilder.Append(part);
                }
            }

            return new MarkupString(contentBuilder.ToString());
            
        }

        public bool IsVideoUrl(string url)
        {
            // Check if the URL has a video file extension
            if (videoExtensions.Any(ext => url.EndsWith(ext, StringComparison.OrdinalIgnoreCase)))
            {
                return true;
            }

            // Check if the URL is from a known video platform
            foreach (var domain in videoDomains.Keys)
            {
                if (url.Contains(domain))
                {
                    return true;
                }
            }

            // Default to false if no video patterns are matched
            return false;
        }

        public string EmbedVideo(string url)
        {
            // Assuming IsVideoUrl has already confirmed it's a video URL
            string embedUrl = string.Empty;
            string iframeTemplate = "<iframe width=\"560\" height=\"315\" src=\"{0}\" frameborder=\"0\" allow=\"accelerometer; autoplay; clipboard-write; encrypted-media; gyroscope; picture-in-picture\" allowfullscreen></iframe>";

            // Direct video file links (e.g., .mp4)
            if (videoExtensions.Any(ext => url.EndsWith(ext, StringComparison.OrdinalIgnoreCase)))
            {
                embedUrl = url; // Direct link, you might need a custom player or handle differently
                return string.Format(iframeTemplate, embedUrl);
            }

            string videoId = GetVideoIdFromUrl(url);

            if (url.Contains("youtube.com") || url.Contains("youtu.be"))
            {
                embedUrl = $"https://www.youtube.com/embed/{videoId}";
            }
            else if (url.Contains("vimeo.com"))
            {
                embedUrl = $"https://player.vimeo.com/video/{videoId}";
            }
            // Construct the iframe HTML string
            if (!string.IsNullOrEmpty(embedUrl))
            {
                return string.Format(iframeTemplate, embedUrl);
            }

            // If no embeddable URL is identified, return an empty string or possibly a link to the URL.
            return $"<a href=\"{url}\" target=\"_blank\" rel=\"noopener noreferrer\">{url}</a>";
            
        }

        public MarkupString Linkify(string text)
        {
            // Define a regex pattern to match URLs
            var urlPattern = @"((http|https|ftp|mailto):\/\/[\w\-_]+(\.[\w\-_]+)+([\w\-\.,@?^=%&amp;:/~\+#]*[\w\-\@?^=%&amp;/~\+#])?)";
    
            var linkedText = Regex.Replace(text, urlPattern, match =>
            {
                var url = match.Value;

                return $"<a href=\"{url}\" target=\"_blank\">{url}</a>";
            });
    
            return new MarkupString(linkedText);
            
        }

        public string GetVideoIdFromUrl(string url)
        {
            // First, ensure that the URL is a valid, well-formed URI.
            if (!Uri.IsWellFormedUriString(url, UriKind.Absolute))
            {
                Console.WriteLine($"Invalid URL format: {url}");
                return string.Empty;
            }

            Uri uri;
            try
            {
                uri = new Uri(url);
            }
            catch (UriFormatException ex)
            {
                Console.WriteLine($"Exception when parsing URL: {ex.Message}");
                return string.Empty;
            }

            string videoId = string.Empty;

            if (uri.Host.Contains("youtube.com") || uri.Host.Contains("youtu.be"))
            {
                // For YouTube regular URLs, the video ID is in the query string parameter "v".
                var query = HttpUtility.ParseQueryString(uri.Query);
                videoId = query["v"];

                if (string.IsNullOrEmpty(videoId))
                {
                    // For YouTube short URLs like "youtu.be", the video ID is in the path.
                    videoId = uri.Segments.LastOrDefault()?.Trim('/');
                }
            }
            else if (uri.Host.Contains("vimeo.com"))
            {
                // For Vimeo, the video ID is usually the last segment in the path, which is a number.
                videoId = uri.Segments.LastOrDefault(s => s != "/")?.Trim('/');
            }
            else if (uri.Host.Contains("tiktok.com"))
            {
                // For TikTok, the video ID is in the path after "/video/".
                var match = Regex.Match(uri.PathAndQuery, @"\/video\/(\d+)");
                if (match.Success)
                {
                    videoId = match.Groups[1].Value;
                }
            }
            else if (uri.Host.Contains("facebook.com"))
            {
                // Facebook video IDs are complex, might need API access to get reliable results.
                // This is a naive approach and might not work for all Facebook video URLs.
                videoId = uri.Segments.LastOrDefault()?.Trim('/');
            }
            else if (uri.Host.Contains("instagram.com"))
            {
                // Instagram video IDs are in the path after "/p/".
                var match = Regex.Match(uri.PathAndQuery, @"\/p\/([^\/]+)");
                if (match.Success)
                {
                    videoId = match.Groups[1].Value;
                }
            }

            return videoId;
        }
        
        public string GetTitleWithoutDate(string title)
        {
            // Assuming the date is always in the format "YYYY-MM-DD - "
            // and you want to remove the first 11 characters (10 from date and 1 from the dash and space)
            if (string.IsNullOrWhiteSpace(title))
            {
                return title;
            }

            int index = title.IndexOf(" - ");
            if (index != -1)
            {
                // Return the substring after the " - "
                return title.Substring(index + 3); // "+ 3" to also remove the " - "
            }

            // If the title does not contain " - ", return it unchanged
            return title;
        }

    }
}
