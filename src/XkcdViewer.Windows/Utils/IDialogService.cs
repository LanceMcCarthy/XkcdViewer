using Microsoft.UI.Xaml.Controls;
using System.Threading.Tasks;

namespace XkcdViewer.Windows.Utils;

public interface IDialogService
{
    Task<ContentDialogResult> ShowDialogAsync(ContentDialog dialog);
}