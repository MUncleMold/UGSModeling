using UGSModeling.ViewModels;

namespace UGSModeling;

public partial class UploadData : ContentPage
{
    public UploadData()
	{
        InitializeComponent();
        BindingContext = new UploadViewModel();
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();

        var viewModel = (UploadViewModel)BindingContext;
        viewModel.LoadItems("");
    }

    private async void GoToMainPage(object sender, EventArgs e)
    {
        await Shell.Current.GoToAsync("//MainPage");
    }

    private void SearchComplete(object sender, EventArgs e)
    {
        var viewModel = (UploadViewModel)BindingContext;
        viewModel.LoadItems(((Entry)sender).Text);
    }

    private async void DateNowSelect(object sender, ToggledEventArgs e)
    {
        if(e.Value == true)
        {
            DateEnter.Text = DateTime.Now.ToString("dd.MM.yyyy");
            DateEnter.IsEnabled = false;
        }
        else
        {
            DateEnter.Text = "";
            DateEnter.IsEnabled = true;
        }
    }
}