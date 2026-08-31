using Syncfusion.Maui.Charts;
using System.Drawing;
using UGSModeling.Models;
using UGSModeling.ViewModels;

namespace UGSModeling;

public partial class BuildGraph : ContentPage
{
	public BuildGraph()
	{
		InitializeComponent();
        BindingContext = new BuildGraphViewModel();
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();

        var viewModel = (BuildGraphViewModel)BindingContext;
        viewModel.LoadItems();
    }

    private async void GoToMainPage(object sender, EventArgs e)
    {
        await Shell.Current.GoToAsync("//MainPage");
    }

    private void XTypeChange(object sender, EventArgs e)
    {
        var picker = (Picker)sender;
        string selectedItem = (string)picker.SelectedItem;
        var viewModel = (BuildGraphViewModel)BindingContext;
        viewModel.XChangeOnSelectedIndexChanged(selectedItem);
    }

    private void StartDateTextChanged(object sender, EventArgs e)
    {
        try
        {
            var viewModel = (BuildGraphViewModel)BindingContext;
            viewModel.EntryStartDateChanged(Convert.ToDateTime(((Entry)sender).Text));
            ((Entry)sender).Text = "";
        }
        catch { }
    }

    private void EndDateTextChanged(object sender, EventArgs e)
    {
        try
        {
            var viewModel = (BuildGraphViewModel)BindingContext;
            viewModel.EntryEndDateChanged(Convert.ToDateTime(((Entry)sender).Text));
            ((Entry)sender).Text = "";
        }
        catch { }
    }

    private void StartDateChanged(object sender, DateChangedEventArgs e)
    {
        var viewModel = (BuildGraphViewModel)BindingContext;
        viewModel.StartDateChangedFunc(Convert.ToDateTime(e.NewDate));
    }

    private void EndDateChanged(object sender, DateChangedEventArgs e)
    {
        var viewModel = (BuildGraphViewModel)BindingContext;
        viewModel.EndDateChangedFunc(Convert.ToDateTime(e.NewDate));
    }

    private async void SaveGraphAsImage(object sender, EventArgs e)
    {
        string name = GraphSaveName.Text.ToString();
        var result = await graph.CaptureAsync();
        using MemoryStream memoryStream = new();
        await result.CopyToAsync(memoryStream);
        File.WriteAllBytes(FileSystem.AppDataDirectory + String.Format("{0}.png", name), memoryStream.ToArray());
    }

    private void SearchComplete(object sender, EventArgs e)
    {
        var viewModel = (BuildGraphViewModel)BindingContext;
        viewModel.LoadItems();
    }

    private void UGSCheck(object sender, ToggledEventArgs e)
    {
        if(e.Value == true)
        {
            UGSCheckTable.IsVisible = true;
        }
        else
        {
            UGSCheckTable.IsVisible = false;
        }
    }
}