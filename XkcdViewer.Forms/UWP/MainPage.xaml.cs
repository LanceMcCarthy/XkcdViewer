using FFImageLoading.Forms.WinUWP;

namespace XkcdViewer.Forms.UWP
{
    public sealed partial class MainPage
    {
        public MainPage()
        {
            this.InitializeComponent();
            CachedImageRenderer.Init();
            LoadApplication(new XkcdViewer.Forms.NetStandard.App());
        }
    }
}
