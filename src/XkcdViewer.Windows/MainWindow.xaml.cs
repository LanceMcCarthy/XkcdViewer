using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using System;
using System.Threading.Tasks;
using XkcdViewer.Windows.Utils;

namespace XkcdViewer.Windows;

public sealed partial class MainWindow : Window, IDialogService
{
    // R# analyzer doesn't detect we're using x:Bind in the XAML
    // ReSharper disable once PrivateFieldCanBeConvertedToLocalVariable
    private readonly MainViewModel ViewModel;

    public MainWindow()
    {
        InitializeComponent();
        RootGrid.DataContext = ViewModel = new MainViewModel{DialogService = this};
        RootGrid.Loaded += (s, e) => ((MainViewModel)((Grid)s).DataContext)?.InitialLoadAsync();
    }

    public async Task<ContentDialogResult> ShowDialogAsync(ContentDialog dialog)
    {
        dialog.XamlRoot = RootGrid.XamlRoot;
        return await dialog.ShowAsync();
    }
}