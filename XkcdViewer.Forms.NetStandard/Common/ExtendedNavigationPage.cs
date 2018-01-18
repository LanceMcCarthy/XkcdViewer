using Cimbalino.Toolkit.Services;
using Xamarin.Forms;

namespace XkcdViewer.Forms.NetStandard.Common
{
    public class ExtendedNavigationPage : NavigationPage
    {
        private INavigationService _navigationService;

        public ExtendedNavigationPage(Page page)
            : base(page)
        {
        }

        public ExtendedNavigationPage(INavigationService navigationService, Page page)
            : base(page)
        {
            SetNavigationService(navigationService);
        }

        public void SetNavigationService(INavigationService navigationService)
        {
            _navigationService = navigationService;
            (_navigationService as NavigationService)?.SetNavigationPageInternal(this);
        }

        protected override bool OnBackButtonPressed()
        {
            var shouldGoBack = (_navigationService as NavigationService)?.BackButtonPressed();
            if (shouldGoBack ?? true)
            {
                return base.OnBackButtonPressed();
            }

            return false;
        }
    }
}
