using UGSModeling.ViewModels;

namespace UGSModeling;

public partial class EmailPage : ContentPage
{
	
	
	
	public EmailPage()
	{
		InitializeComponent();
        BindingContext = new EmailPageViewModel();
    }
    private async void GoToMainPage(object sender, EventArgs e)
    {
        await Shell.Current.GoToAsync("//MainPage");
    }
}