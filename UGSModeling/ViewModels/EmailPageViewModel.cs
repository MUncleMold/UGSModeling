using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using DocumentFormat.OpenXml.Bibliography;
using DocumentFormat.OpenXml.Wordprocessing;
using Microsoft.Maui.ApplicationModel.Communication;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;
using System.Windows.Input;
using UGSModeling.Data;
using UGSModeling.Models;

namespace UGSModeling.ViewModels
{

    

    internal partial class EmailPageViewModel : ObservableObject
    {


        private readonly UGSDataBase _database;

        [ObservableProperty]
        private ObservableCollection<UGSReport> reports;

        [ObservableProperty]
        private string _recipientEmail;

        [ObservableProperty]
        private string _emailSubject;

        [ObservableProperty]
        private string _emailBody;

        [ObservableProperty]
        private ObservableCollection<ReportUI> reports2 = new();

        public ICommand SendEmail { get; }


        public EmailPageViewModel()
        {
            _database = new UGSDataBase();
            LoadItems();
            SendEmail = new Command(async () => await SendEmailAsync());

        }

        [RelayCommand]
        private async void LoadItems()
        {
            var itemsRep = await _database.ReportGetItem();
            itemsRep.Reverse();
            Reports = new ObservableCollection<UGSReport>(itemsRep);

            Reports2.Clear();
            foreach (var report in Reports)
            {
                Reports2.Add(new ReportUI(report));
            }

        }
        private async Task SendFilesWithShareApiAsync(List<string> files)
        {
            try
            {
                string subject = EmailSubject;
                string body = EmailBody;
                string recipient = RecipientEmail;

                var emailContent = $""" Отправлено: {string.Join(", ", recipient)} Тема: {subject} {body}  """;
                var shareFiles = files.Select(f => new ShareFile(f)).ToList();

                var request = new ShareMultipleFilesRequest
                {
                    Title = emailContent,
                    Files = shareFiles  
                    
                    //Text = emailContent,
                    //Title = "Share this email", // Заголовок окна шаринга
                    //Subject = subject // Некоторые платформы используют это поле как тему
                    //Files = shareFiles

                };



                await Share.Default.RequestAsync(request);
            }
            catch (Exception ex)
            {
                // Обработка ошибок (например, если шаринг не поддерживается)
                await Application.Current.MainPage.DisplayAlert("Error",$"Cannot share: {ex.Message}","OK");
            }



        }
        private async Task SendEmailAsync()
        {
            // 1. Валидация email
            if (string.IsNullOrWhiteSpace(RecipientEmail))
            {
                await Application.Current.MainPage.DisplayAlert("Ошибка", "Введите email получателя", "OK");
                return;
            }

            // 2. Получаем выбранные файлы
            var selectedFiles = GetSelectedFilePaths();

            if (!selectedFiles.Any())
            {
                await Application.Current.MainPage.DisplayAlert("Предупреждение", "Выберите хотя бы один файл для отправки", "OK");
                return;
            }

            // 3. Проверяем существование файлов
            var invalidFiles = selectedFiles.Where(f => !File.Exists(f)).ToList();
            if (invalidFiles.Any())
            {
                return;
            }

            try
            {

                // 4. Отправляем файлы через Share API
                await SendFilesWithShareApiAsync(selectedFiles);

                // 5. Опционально: очищаем выбор после отправки
                // ClearAllFiles();
            }
            catch (Exception ex)
            {
                await Application.Current.MainPage.DisplayAlert("Ошибка", $"Не удалось отправить: {ex.Message}", "OK");
            }
            finally
            {
            }
        }
        private List<string> GetSelectedFilePaths()
        {
            return Reports2
                .Where(r => r.IsSelected)
                .Select(r => r.Path)
                .Where(File.Exists)
                .ToList();
        }
    }

    //Прикольно да?
    public partial class ReportUI : ObservableObject
    {
        private readonly UGSReport _report;

        public ReportUI(UGSReport report)
        {
            _report = report;
        }

        public int Id => _report.Id;
        public string Name => _report.Name;
        public string Path => _report.Path;
        public int UserId => _report.UserId;
        public string Date => _report.Date;



        [ObservableProperty]
        private bool _isSelected;
    }
}