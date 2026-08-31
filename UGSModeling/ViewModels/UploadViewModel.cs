using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using ExcelDataReader;
using System.Collections.ObjectModel;
using System.Data;
using UGSModeling.Data;
using UGSModeling.Models;

namespace UGSModeling.ViewModels
{
    internal partial class UploadViewModel: ObservableObject
    {
        private readonly UGSDataBase _database;

        [ObservableProperty]
        private ObservableCollection<UGSParameter> uGSParameters;

        [ObservableProperty]
        private string newTaskName = "";

        [ObservableProperty]
        private string newTaskDate = "";

        [ObservableProperty]
        private bool newTaskDateToggle = false;

        [ObservableProperty]
        private string newTaskUnit = "";

        [ObservableProperty]
        private double newTaskValue = 0;

        [ObservableProperty]
        private string searchText = "";

        [ObservableProperty]
        private string namePHColor = "gray";

        [ObservableProperty]
        private string datePHColor = "gray";

        [ObservableProperty]
        private string unitPHColor = "gray";

        [ObservableProperty]
        private string entryErrText = "";

        [ObservableProperty]
        private string dateTextColor = "white";

        [ObservableProperty]
        private bool loadErr = false;

        [ObservableProperty]
        private string lbText = "";

        public UploadViewModel()
        {
            _database = new UGSDataBase();
            LoadItems(SearchText);
        }

        [RelayCommand]
        public async void LoadItems(string searchStr)
        {
            var items = await _database.ParamGetItems();
            items.Reverse();
            UGSParameters = new ObservableCollection<UGSParameter>(items);
            if(searchStr != "")
            {
                UGSParameters = new ObservableCollection<UGSParameter>(UGSParameters.Where(x => x.Name.ToLower().Contains(searchStr.ToLower()) ||
                                                                                            x.Date.ToLower().Contains(searchStr.ToLower()) ||
                                                                                            x.Unit.ToLower().Contains(searchStr.ToLower())).ToList());
            }
        }

        [RelayCommand]
        private async void AddItem()
        {
            EntryErrText = "";
            NamePHColor = "gray";
            DatePHColor = "gray";
            DateTextColor = "white";
            UnitPHColor = "gray";
            string errEx = "0";
            string err = "!!!ОШИБКА!!! ";
            if(NewTaskName == "")
            {
                NamePHColor = "red";
                errEx += "1";
            }
            if (NewTaskDate == "" && NewTaskDateToggle == false)
            {
                DatePHColor = "red";
                errEx += "1";
            }
            if (NewTaskUnit == "")
            {
                UnitPHColor = "red";
                errEx += "1";
            }

            if(NewTaskDateToggle == false && newTaskDate != "")
            {
                try
                {
                    Convert.ToDateTime(newTaskDate).ToString("dd.MM.yyyy");
                }
                catch
                {
                    DateTextColor = "red";
                    errEx += "2";
                }
            }
            
            if (errEx == "0")
            {
                if (NewTaskDateToggle == false)
                {
                    var newItem = new UGSParameter
                    {
                        Name = NewTaskName.ToString(),
                        Date = NewTaskDate.ToString(),
                        Unit = NewTaskUnit.ToString(),
                        Value = NewTaskValue.ToString()
                    };
                    await _database.ParamAddItem(newItem);
                }
                else
                {
                    var newItem = new UGSParameter
                    {
                        Name = NewTaskName.ToString(),
                        Date = DateTime.Today.ToString("dd.MM.yyyy"),
                        Unit = NewTaskUnit.ToString(),
                        Value = NewTaskValue.ToString()
                    };
                    await _database.ParamAddItem(newItem);
                }
                LoadItems(SearchText);
            }
            else
            {
                if(errEx.Contains("1"))
                {
                    err += "Поля ввода не могут быть пустыми";
                }
                if (errEx.Contains("2"))
                {
                    err += "Неверный формат даты";
                }
                EntryErrText = err;
            }
        }

        [RelayCommand]
        private async void AddItemFromFile()
        {
            LoadErr = false;

            try
            {
                var allRows = new List<string[]>();

                var path = await FilePicker.PickAsync(new PickOptions
                {
                    PickerTitle = "Выберите Excel файл"
                });

                if(path != null)
                {
                    using (var stream = File.Open(path.FullPath, FileMode.Open, FileAccess.Read))
                    {
                        using (var reader = ExcelReaderFactory.CreateReader(stream))
                        {
                            var file = reader.AsDataSet();

                            for (int rowIndex = 1; rowIndex < file.Tables[0].Rows.Count; rowIndex++)
                            {
                                var rowData = new string[4];
                                for (int colIndex = 0; colIndex < file.Tables[0].Columns.Count; colIndex++)
                                {
                                    rowData[colIndex] = file.Tables[0].Rows[rowIndex][colIndex].ToString();
                                }

                                allRows.Add(rowData);
                            }

                            for (int i = 0; i < allRows.Count; i++)
                            {
                                var newItem = new UGSParameter
                                {
                                    Name = allRows[i][0].ToString(),
                                    Date = Convert.ToDateTime(allRows[i][1].ToString()).ToString("dd.MM.yyyy"),
                                    Unit = allRows[i][2].ToString(),
                                    Value = allRows[i][3].ToString()
                                };
                                await _database.ParamAddItem(newItem);
                                LoadItems(SearchText);
                            }
                        }
                    }
                }
                else
                {
                    LoadErr = true;
                }
            }
            catch
            {
                LoadErr = true;
            }
        }

        [RelayCommand]
        private async void DeleteItem(UGSParameter item)
        {
            await _database.ParamDeleteItem(item);
            LoadItems(SearchText);
        }
    }
}
