using Telerik.Maui.Controls;

namespace XkcdViewer.Maui
{
    public partial class App : Application
    {
        public App()
        {
            InitializeComponent();
        }

        protected override Window CreateWindow(IActivationState? activationState)
        {
            return new Window(new AppShell());
        }

        private void MyImage_OnHandlerChanged(object? sender, EventArgs e)
        {
            var x = (sender as Image)?.Handler?.PlatformView;

//            if ((sender as Image)?.Handler?.PlatformView is Image nativeEntry)
//            {
//#if WINDOWS10_0_22000_0_OR_GREATER

//#endif
//            }
        }
    }
}
