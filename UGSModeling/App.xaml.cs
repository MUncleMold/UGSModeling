using Microsoft.Extensions.DependencyInjection;
using Standard.Licensing.Validation;
using UGSModeling.Stuff;

namespace UGSModeling
{
    public partial class App : Application
    {
        private readonly LicenseValidator _licenseValidator;

        public App()
        {
            InitializeComponent();
            _licenseValidator = new LicenseValidator();

        }
        protected override void OnStart()
        {
            CheckLicense();
            Application.Current.UserAppTheme = AppTheme.Dark;
        }
        private async Task CheckLicense()
        {


            var (isValid, errors) = _licenseValidator.ValidateLicenseFromFile();

            if (isValid)
            {
                MainPage = new AppShell();
            }
            else
            {
                // Показываем окно ввода лицензии
                MainPage = new NavigationPage(new LicenseWindow());
            }
        }
        protected override Window CreateWindow(IActivationState? activationState)
        {
            return new Window(new AppShell());
        }
    }
}