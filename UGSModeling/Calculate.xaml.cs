using Microsoft.VisualBasic;
using UGSModeling.ViewModels;

namespace UGSModeling;

public partial class Calculate : ContentPage
{
	public Calculate()
	{
		InitializeComponent();
        BindingContext = new CalculateViewModel();
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();

        var viewModel = (CalculateViewModel)BindingContext;
        viewModel.LoadItems();
    }

    private async void GoToMainPage(object sender, EventArgs e)
    {
        await Shell.Current.GoToAsync("//MainPage");
    }

    private void DateChanged(object sender, DateChangedEventArgs e)
    {
        var viewModel = (CalculateViewModel)BindingContext;
        viewModel.DateChangedFunc(Convert.ToDateTime(e.NewDate));
    }

    private async void AddParam(object sender, EventArgs e)
    {
        Button clickedButton = (Button)sender;
        string b = clickedButton.Text;
        var viewModel = (CalculateViewModel)BindingContext;
        viewModel.AddParamToCalc(b);
    }

    private async void NewCalc(object sender, EventArgs e)
    {
        Button clickedButton = (Button)sender;
        string b = clickedButton.Text;
        var viewModel = (CalculateViewModel)BindingContext;
        viewModel.NewCalcToCalc(b);
    }

    private void SearchComplete(object sender, EventArgs e)
    {
        var viewModel = (CalculateViewModel)BindingContext;
        viewModel.LoadItems();
    }

    private void UGSCheck(object sender, ToggledEventArgs e)
    {
        if (e.Value == true)
        {
            UGSCheckTable.IsVisible = true;
        }
        else
        {
            UGSCheckTable.IsVisible = false;
        }
    }
}