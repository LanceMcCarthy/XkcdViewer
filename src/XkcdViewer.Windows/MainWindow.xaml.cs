using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using System.Collections.Generic;

namespace XkcdViewer.Windows;

public sealed partial class MainWindow : Window
{
    private MainViewModel vm;
    public MainWindow()
    {
        this.InitializeComponent();
        RootGrid.DataContext = vm =new MainViewModel();
        RootGrid.Loaded += (s,e)=>((MainViewModel)((Grid)s).DataContext)?.InitialLoadAsync();


        SegmentedControl.ItemsSource = new List<string> { "Comics", "PDFs" };
        SegmentedControl.SelectedIndex = 0;
    }

    private void Segements_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        if (SegmentedControl.SelectedItem == "Comics")
        {
            Presenter.ContentTemplate = RootGrid.Resources["ComicTemplate"] as DataTemplate;
        }
        else
        {
            Presenter.ContentTemplate = RootGrid.Resources["PdfTemplate"] as DataTemplate;
        }
    }
}