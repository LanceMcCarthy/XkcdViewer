namespace XkcdViewer.Forms.UWP
{
    public sealed partial class MainPage
    {
        public MainPage()
        {
            this.InitializeComponent();
            LoadApplication(new XkcdViewer.Forms.NetStandard.App());
        }
    }
}
