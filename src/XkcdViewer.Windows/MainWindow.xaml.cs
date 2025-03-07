using Microsoft.Graphics.Imaging;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.Windows.AI.ContentModeration;
using Microsoft.Windows.AI.Generative;
using Microsoft.Windows.Management.Deployment;
using System;
using System.Diagnostics;
using System.IO;
using System.Net.Http;
using Windows.Graphics.Imaging;
using Windows.Media.Core;
using Windows.Media.SpeechSynthesis;
using Windows.Storage;
using Windows.Storage.Streams;

namespace XkcdViewer.Windows;

public sealed partial class MainWindow : Window
{
    public MainWindow()
    {
        InitializeComponent();
        RootGrid.Loaded += (s, e) => ((MainViewModel)((Grid)s).DataContext)?.InitialLoadAsync();
    }
}