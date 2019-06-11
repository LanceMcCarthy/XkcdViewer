using FFImageLoading.Forms.Platform;

namespace XkcdViewer.Forms.UWP
{
    public sealed partial class MainPage
    {
        public MainPage()
        {
            this.InitializeComponent();
            FFImageLoading.Forms.Platform.CachedImageRenderer.Init();
            LoadApplication(new XkcdViewer.Forms.NetStandard.App());
        }
    }
}
