using System;
using System.Collections.Generic;
using System.Linq;
using Cimbalino.Toolkit.Handlers;
using Cimbalino.Toolkit.Services;
using Xamarin.Forms;

namespace XkcdViewer.Forms.NetStandard.Common
{
    public class NavigationService : INavigationService
    {
        private NavigationPage _navigation;

        private NavigationPage Navigation
        {
            get
            {
                if (_navigation == null)
                {
                    throw new ArgumentNullException(nameof(_navigation), "No NavigationPage has been set. This must be done");
                }

                return _navigation;
            }
        }

        public virtual void SetNavigationPage(NavigationPage navigationPage)
        {
            SetNavigationPageInternal(navigationPage);
            (navigationPage as ExtendedNavigationPage)?.SetNavigationService(this);
        }

        public virtual bool Navigate(string source)
        {
            throw new NotSupportedException("Not supported in xamarin.forms");
        }

        public virtual bool Navigate(Uri source)
        {
            throw new NotSupportedException("Not supported in xamarin.forms");
        }

        public virtual bool Navigate<T>()
        {
            return Navigate<T>(null);
        }

        public virtual bool Navigate<T>(object parameter)
        {
            return Navigate(typeof(T), parameter);
        }

        public virtual bool Navigate(Type type)
        {
            return Navigate(type, null);
        }

        public virtual bool Navigate(Type type, object parameter)
        {
            var page = Activator.CreateInstance(type);
            (page as PageBase)?.SetNavigationParameter(parameter);
            Navigation.PushAsync((Page)page);
            return true;
        }

        public virtual void GoBack()
        {
            Navigation?.SendBackButtonPressed();
        }

        public virtual void GoForward()
        {
            throw new NotSupportedException("Not supported in xamarin.forms");
        }

        public virtual bool RemoveBackEntry()
        {
            var fullStack = Navigation.Navigation.NavigationStack;
            var stack = fullStack.Take(fullStack.Count - 1).ToList();
            if (stack.Count > 0)
            {
                var lastPage = stack.LastOrDefault();
                Navigation.Navigation.RemovePage(lastPage);
                return true;
            }

            return false;
        }

        public virtual void ClearBackstack()
        {
            var fullStack = Navigation.Navigation.NavigationStack;
            var stack = fullStack.Take(fullStack.Count - 1).ToList();
            if (stack.Count > 0)
            {
                foreach (var page in stack)
                {
                    Navigation.Navigation.RemovePage(page);
                }
            }
        }

        public virtual void RegisterFrame(object frame)
        {
            throw new NotSupportedException("Not supported in xamarin.forms");
        }

        public virtual Uri CurrentSource { get; }

        public virtual IEnumerable<KeyValuePair<string, string>> QueryString { get; } = null;

        public virtual object CurrentParameter { get; private set; }
        public virtual bool CanGoBack => NavigationStackCount > 1;
        public virtual bool CanGoForward { get; } = false;

        public event EventHandler<NavigationServiceNavigationEventArgs> Navigated;
        public event EventHandler<NavigationServiceBackKeyPressedEventArgs> BackKeyPressed;

        private int? NavigationStackCount => Navigation?.Navigation.NavigationStack.Count;

        internal void SetNavigationPageInternal(NavigationPage navigationPage)
        {
            if (navigationPage == null)
            {
                throw new ArgumentNullException(nameof(navigationPage), "You must not set a null navigation page");
            }

            if (_navigation != null)
            {
                _navigation.Popped -= NavigationOnPopped;
                _navigation.Pushed -= NavigationOnPushed;
            }

            _navigation = navigationPage;

            Navigation.Popped += NavigationOnPopped;
            Navigation.Pushed += NavigationOnPushed;
        }

        internal bool BackButtonPressed()
        {
            var args = new NavigationServiceBackKeyPressedEventArgs();
            BackKeyPressed?.Invoke(this, args);
            var shouldGoBack = true;

            switch (args.Behavior)
            {
                case NavigationServiceBackKeyPressedBehavior.DoNothing:
                    shouldGoBack = false;
                    break;
                case NavigationServiceBackKeyPressedBehavior.HideApp:
                case NavigationServiceBackKeyPressedBehavior.ExitApp:
                case NavigationServiceBackKeyPressedBehavior.GoBack:
                    break;
            }

            return shouldGoBack;
        }

        private void NavigationOnPushed(object sender, NavigationEventArgs e)
        {
            if (CanGoBack)
            {
                var page = Navigation.Navigation.NavigationStack[NavigationStackCount.Value - 2];
                var fromPage = page?.BindingContext as IHandleNavigatedFrom;
                fromPage?.OnNavigatedFromAsync(new NavigationServiceNavigationEventArgs(NavigationServiceNavigationMode.Forward, fromPage.GetType(), (page as PageBase)?.NavigationParameter, null));
            }

            var toPage = e.Page.BindingContext as IHandleNavigatedTo;
            var args = new NavigationServiceNavigationEventArgs(NavigationServiceNavigationMode.Forward, e.Page.GetType(), (e.Page as PageBase)?.NavigationParameter, null);
            CurrentParameter = args.Parameter;
            toPage?.OnNavigatedToAsync(args);
            Navigated?.Invoke(this, args);
        }

        private void NavigationOnPopped(object sender, NavigationEventArgs e)
        {
            var fromPage = e.Page.BindingContext as IHandleNavigatedFrom;
            var args = new NavigationServiceNavigationEventArgs(NavigationServiceNavigationMode.Back, e.Page.GetType(), (e.Page as PageBase)?.NavigationParameter, null);
            CurrentParameter = args.Parameter;
            fromPage?.OnNavigatedFromAsync(args);
            Navigated?.Invoke(this, args);

            var page = Navigation?.Navigation.NavigationStack?.LastOrDefault();
            var toPage = page?.BindingContext as IHandleNavigatedTo;
            toPage?.OnNavigatedToAsync(new NavigationServiceNavigationEventArgs(NavigationServiceNavigationMode.Back, toPage.GetType(), (page as PageBase)?.NavigationParameter, null));
        }
    }
}
