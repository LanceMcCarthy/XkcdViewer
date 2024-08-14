using XkcdViewer.Maui.ViewModels;

namespace XkcdViewer.Maui.Views;

public class BasePage : ContentPage, IQueryAttributable
{
    public void ApplyQueryAttributes(IDictionary<string, object> query)
    {
        //if (query.TryGetValue("NameOfParam", out var paramValue))
        //{
        //    this.PublicBasePageProperty = paramValue;
        //    OnPropertyChanged(nameof(PublicBasePageProperty));
        //}
    }

    #region Navigation and Lifecycle Methods that view models can hook into safely

    protected override void OnNavigatingFrom(NavigatingFromEventArgs args)
    {
        base.OnNavigatingFrom(args);

        if (BindingContext is PageViewModelBase viewModel)
        {
            viewModel.OnNavigatingFrom(args);
        }
    }

    protected override void OnNavigatedFrom(NavigatedFromEventArgs args)
    {
        base.OnNavigatedFrom(args);

        if (BindingContext is PageViewModelBase viewModel)
        {
            viewModel.OnNavigatedFrom(args);
        }
    }

    protected override void OnNavigatedTo(NavigatedToEventArgs args)
    {
        base.OnNavigatedTo(args);

        if(BindingContext is PageViewModelBase viewModel)
        {
            //viewModel.OnNavigatedTo(args, Favorites);

            viewModel.OnNavigatedTo(args);
        }


    }

    protected override void OnAppearing()
    {
        base.OnAppearing();

        if(BindingContext is PageViewModelBase viewModel)
        {
            viewModel.OnAppearing();
        }
    }

    protected override bool OnBackButtonPressed()
    {
        if (BindingContext is PageViewModelBase viewModel)
        {
            return viewModel.OnBackButtonRequested();
        }

        return base.OnBackButtonPressed();
    }

    #endregion
}