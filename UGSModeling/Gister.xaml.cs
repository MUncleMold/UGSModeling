using DocumentFormat.OpenXml.Drawing;
using UGSModeling.ViewModels;

namespace UGSModeling;

public partial class Gister : ContentPage
{
    public Gister()
	{
		InitializeComponent();
        BindingContext = new GisterViewModel();
    }

    private void StepChanged(object sender, ValueChangedEventArgs e)
    {
        St2.Text = e.NewValue.ToString();
    }

    private void PeriodChanged(object sender, ValueChangedEventArgs e)
    {
        St1.Text = e.NewValue.ToString();
        var viewModel = (GisterViewModel)BindingContext;
        viewModel.LoadPeriod(Convert.ToInt32(e.NewValue));
    }

    private void Draw(object sender, EventArgs e)
    {
        decimal k = Convert.ToDecimal(L1.Text) / Convert.ToDecimal(1e12);
        decimal h = Convert.ToDecimal(L2.Text);
        decimal L = Convert.ToDecimal(L3.Text);
        decimal m = Convert.ToDecimal(L4.Text);
        decimal R = Convert.ToDecimal(L5.Text);
        decimal z = Convert.ToDecimal(L6.Text);
        decimal T = Convert.ToDecimal(L7.Text);
        decimal deg = Convert.ToDecimal(L8.Text) * Convert.ToDecimal(Math.PI) / 180;
        decimal pk = Convert.ToDecimal(L9.Text) * Convert.ToDecimal(1e6);
        decimal ρw = Convert.ToDecimal(L10.Text);
        decimal μw = Convert.ToDecimal(L11.Text) / Convert.ToDecimal(1e3);
        decimal pg = Convert.ToDecimal(L12.Text) * Convert.ToDecimal(1e6);
        decimal A0 = Convert.ToDecimal(L13.Text);
        decimal st2 = Convert.ToDecimal(St2.Text) * 86400;
        decimal st1 = Convert.ToDecimal(St1.Text);
        decimal ΔZ = Convert.ToDecimal(L14.Text);
        decimal Rk = Convert.ToDecimal(L15.Text);

        var viewModel = (GisterViewModel)BindingContext;
        viewModel.DrawGraphs(k, h, L, m, R, z, T, deg, pk, ρw, μw, pg, A0, st1, st2, ΔZ, Rk);
    }

    private void CostChange(object sender, EventArgs e)
    {
        var viewModel = (GisterViewModel)BindingContext;
        viewModel.PerTable.Clear();
        viewModel.LoadPeriod(10);
    }

    private async void GoToMainPage(object sender, EventArgs e)
    {
        await Shell.Current.GoToAsync("//MainPage");
    }

    private void PlastSwitchCom(object sender, ToggledEventArgs e)
    {
        var viewModel = (GisterViewModel)BindingContext;
        viewModel.PerTable.Clear();
        viewModel.LoadPeriod(10);

        if (e.Value == true)
        {
            L14.IsVisible = true;
            L14l.IsVisible = true;

            L15.IsVisible = true;
            L15l.IsVisible = true;

            L3.IsVisible = false;
            L3l.IsVisible = false;

            L8.IsVisible = false;
            L8l.IsVisible = false;

            GorText.TextColor = Colors.Gray;
            RadText.TextColor = Colors.White;
        }
        else
        {
            L14.IsVisible = false;
            L14l.IsVisible = false;

            L15.IsVisible = false;
            L15l.IsVisible = false;

            L3.IsVisible = true;
            L3l.IsVisible = true;

            L8.IsVisible = true;
            L8l.IsVisible = true;

            GorText.TextColor = Colors.White;
            RadText.TextColor = Colors.Gray;
        }
    }
}