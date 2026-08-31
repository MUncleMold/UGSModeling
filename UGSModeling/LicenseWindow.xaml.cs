using UGSModeling.Stuff;

namespace UGSModeling;

public partial class LicenseWindow : ContentPage
{
    private readonly LicenseValidator _licenseValidator;

    public LicenseWindow()
	{
		InitializeComponent();
        _licenseValidator = new LicenseValidator();

    }
    private async void OnActivateClicked(object sender, EventArgs e)
    {
        if (string.IsNullOrWhiteSpace(LicenseEntry.Text))
        {
            await DisplayAlert("Ошибка", "Введите лицензионный ключ", "OK");
            return;
        }

        await ActivateLicense(LicenseEntry.Text);
    }

    private async void OnLoadFileClicked(object sender, EventArgs e)
    {
        try
        {

            var result = await FilePicker.PickAsync(new PickOptions
            {
                PickerTitle = "Выберите файл лицензии"
               
            });
            
            if (result != null)
            {
                using var stream = await result.OpenReadAsync();
                using var reader = new StreamReader(stream);
                var content = await reader.ReadToEndAsync();
                LicenseEntry.Text = content;
                
                await ActivateLicense(content);
            }
        }
        catch (Exception ex)
        {
            await DisplayAlert("Ошибка", $"Не удалось загрузить файл: {ex.Message}", "OK");
        }
    }
    
    private async Task ActivateLicense(string licenseContent)
    {

        StatusLabel.IsVisible = false;

        var (isValid, errors) = await _licenseValidator.ActivateAndSaveLicenseAsync(licenseContent);



        if (isValid)
        {
            await DisplayAlert("Успех", "Лицензия успешно активирована!", "OK");

            // Переход на главный экран
            Application.Current.MainPage = new AppShell();
        }
        else
        {
            StatusLabel.Text = string.Join("\n", errors);
            StatusLabel.IsVisible = true;
        }
    }

}