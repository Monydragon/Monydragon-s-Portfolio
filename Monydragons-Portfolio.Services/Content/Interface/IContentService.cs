using Microsoft.AspNetCore.Components;

namespace Monydragons_Portfolio.Services.Content.Interface;

public interface IContentService
{
    Task GenerateBlogContentJsonAsync(string targetDirectory);
    DateTime ExtractDateFromFolderName(string folderName);
    MarkupString ProcessContent(string content);
    bool IsVideoUrl(string url);
    string EmbedVideo(string url);
    MarkupString Linkify(string text);
    string GetVideoIdFromUrl(string url);
    string GetTitleWithoutDate(string title);
}