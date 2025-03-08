using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;

namespace XkcdViewer.Windows;

public sealed partial class MainWindow : Window
{
    public MainWindow()
    {
        InitializeComponent();
        RootGrid.Loaded += (s, e) => ((MainViewModel)((Grid)s).DataContext)?.InitialLoadAsync();
    }
}