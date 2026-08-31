using UGSModeling.ViewModels;

namespace UGSModeling
{
    public partial class MainPage : ContentPage
    {
        public MainPage()
        {
            InitializeComponent();
        }

        private async void GoToUploadData(object sender, EventArgs e)
        {
            await Shell.Current.GoToAsync("//UploadData");
        }

        private async void GoToCalculate(object sender, EventArgs e)
        {
            await Shell.Current.GoToAsync("//Calculate");
        }

        private async void GoToBuildGraph(object sender, EventArgs e)
        {
            await Shell.Current.GoToAsync("//BuildGraph");
        }

        private async void GoToEmail(object sender, EventArgs e)
        {
            await Shell.Current.GoToAsync("//EmailPage");
        }

        private async void GoToReport(object sender, EventArgs e)
        {
            await Shell.Current.GoToAsync("//ReportPage");
        }

        private async void GoToPHG(object sender, EventArgs e)
        {
            await Shell.Current.GoToAsync("//Gister");
        }
    }
}
