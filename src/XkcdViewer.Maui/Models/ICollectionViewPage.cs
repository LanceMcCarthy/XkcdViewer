namespace XkcdViewer.Maui.Models;

public interface ICollectionViewPage
{
    public void ScrollIntoView(object? item, bool isAnimated);
}