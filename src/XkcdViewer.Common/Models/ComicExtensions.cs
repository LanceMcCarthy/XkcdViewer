using CommonHelpers.Services.DataModels;

namespace XkcdViewer.Common.Models;

public static class ComicExtensions
{
    public static Comic ToComic(this XkcdComic com)
    {
        return new Comic
        {
            Month = com.Month,
            Num = com.Num,
            Link = com.Link,
            Year = com.Year,
            News = com.News,
            SafeTitle = com.SafeTitle,
            Transcript = com.Transcript,
            Day = com.Day,
            Alt = com.Alt,
            Img = com.Img,
            Title = com.Title
        };
    }
}