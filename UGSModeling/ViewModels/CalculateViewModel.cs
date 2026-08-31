using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using NCalc;
using System.Collections.ObjectModel;
using System.Data;
using System.Drawing;
using UGSModeling.Data;
using UGSModeling.Models;

namespace UGSModeling.ViewModels
{
    internal partial class CalculateViewModel: ObservableObject
    {
        private readonly UGSDataBase _database;

        [ObservableProperty]
        private ObservableCollection<UGSParameter> uGSParameters;

        [ObservableProperty]
        private ObservableCollection<Formula> formulas;

        [ObservableProperty]
        private ObservableCollection<UGSParameter> uGSSearch;

        [ObservableProperty]
        private ObservableCollection<Formula> searchFormulas;

        [ObservableProperty]
        private string calc = "";

        [ObservableProperty]
        private string form = "";

        [ObservableProperty]
        private int paramCount = 0;

        [ObservableProperty]
        private string calcBind = "";

        [ObservableProperty]
        private string calcBindParams = "";

        [ObservableProperty]
        private string calcName = "";

        [ObservableProperty]
        private string calcUnit = "";

        [ObservableProperty]
        private string result = "";

        [ObservableProperty]
        private string resultCalc = "";

        [ObservableProperty]
        public static List<string> uniqNames;

        [ObservableProperty]
        private DateTime selectedDate = DateTime.Now;

        [ObservableProperty]
        private DateTime selectedStartDate = DateTime.Now;

        [ObservableProperty]
        private DateTime selectedEndDate = DateTime.Now;

        [ObservableProperty]
        private bool entryErr = false;

        [ObservableProperty]
        private bool twoDateCalcToggled = false;

        [ObservableProperty]
        private bool entryDateErr = false;

        [ObservableProperty]
        private bool entryTwoDateErr = false;

        [ObservableProperty]
        private string searchFormText = "";

        [ObservableProperty]
        private string searchParamText = "";

        [ObservableProperty]
        private string formName = "";

        public CalculateViewModel()
        {
            _database = new UGSDataBase();
            LoadItems();
        }

        [RelayCommand]
        public async void LoadFormulas()
        {
            if (SearchFormText != "")
            {
                SearchFormulas = new ObservableCollection<Formula>(Formulas.Where(x => x.FormDesc.ToLower().Contains(SearchFormText.ToLower()) ||
                                                                                            x.Params.ToLower().Contains(SearchFormText.ToLower())));
            }
            else
            {
                SearchFormulas = Formulas;
            }
        }

        [RelayCommand]
        public async void LoadParams()
        {
            if (SearchParamText != "")
            {
                UGSSearch = new ObservableCollection<UGSParameter>(UGSParameters.Where(x => x.Name.ToLower().Contains(SearchParamText.ToLower()) ||
                                                                                            x.Date.ToLower().Contains(SearchParamText.ToLower()) ||
                                                                                            x.Unit.ToLower().Contains(SearchParamText.ToLower())).ToList());
            }
            else
            {
                UGSSearch = UGSParameters;
            }
        }

        [RelayCommand]
        public async void LoadItems()
        {
            var items = await _database.ParamGetItems();
            items.Reverse();

            UGSParameters = new ObservableCollection<UGSParameter>(items);
            UniqNames = UGSParameters.Select(x => x.Name).Distinct().ToList();

            var itemsFormula = await _database.FormulaGetItems();
            itemsFormula.Reverse();
            Formulas = new ObservableCollection<Formula>(itemsFormula);

            LoadFormulas();
;           LoadParams();
        }

        [RelayCommand]
        private async void Clear ()
        {
            paramCount = 0;
            Calc = "";
            Form = "";
            CalcBind = "";
            CalcBindParams = "";
            CalcName = "";
            Result = "";
        }

        [RelayCommand]
        private async void SaveParam()
        {
            EntryErr = false;
            CalculateResult();
            if (CalcBind == "" || CalcName == "" || CalcUnit == "")
            {
                EntryErr = true;
            }
            else
            {
                var newItem = new UGSParameter
                {
                    Name = CalcName.ToString(),
                    Date = selectedDate.ToString("dd.MM.yyyy"),
                    Unit = CalcUnit.ToString(),
                    Value = ResultCalc.ToString()
                };
                await _database.ParamAddItem(newItem);
            }
            LoadItems();
        }

        [RelayCommand]
        private async void CalculateResult()
        {
            EntryErr = false;
            EntryDateErr = false;

            if (CalcBind == "" || CalcName == "")
            {
                EntryErr = true;
            }
            else
            {
                var str = CalcBind.Replace("[", "{").Replace("]", "}");
                List<decimal> paramValues = new List<decimal>();
                List<string> ParamEnters = new List<string>();
                var e = new Expression(CalcBind);
                if (CalcBindParams != "")
                {
                    try
                    {
                        if (CalcBindParams.Contains("/"))
                        {
                            ParamEnters = CalcBindParams.Split("/").ToList();
                        }
                        else
                        {
                            ParamEnters.Add(CalcBindParams);
                        }

                        for (int j = 0; j < ParamEnters.Count(); j++)
                        {
                            if (UGSParameters.Where(x => x.Name == ParamEnters[j]).Where(x => Convert.ToDateTime(x.Date) == SelectedDate).FirstOrDefault() == null)
                            {
                                EntryErr = true;
                                EntryDateErr = true;
                            }
                        }

                        if (EntryDateErr == false)
                        {
                            for (int j = 0; j < ParamEnters.Count(); j++)
                            {
                                paramValues.Add(Convert.ToDecimal(UGSParameters.Where(x => x.Name == ParamEnters[j]).Where(x => Convert.ToDateTime(x.Date) == SelectedDate).FirstOrDefault().Value));
                            }

                            for (int j = 0; j < ParamEnters.Count(); j++)
                            {
                                e.Parameters[j.ToString()] = Convert.ToDecimal(paramValues[j]);
                            }
                            decimal result = Convert.ToDecimal(e.Evaluate());
                            List<string> list = paramValues.Select(d => d.ToString()).ToList();
                            Calc = String.Format(str, list.ToArray());

                            ResultCalc = result.ToString();

                            Result = "Результат: " + CalcName.ToString() + " = " + result;
                        }
                    }
                    catch
                    {
                        EntryErr = true;
                    }
                }
                else
                {
                    try
                    {
                        decimal result = Convert.ToDecimal(e.Evaluate());

                        ResultCalc = result.ToString();

                        Result = "Результат: " + CalcName.ToString() + " = " + result;
                    }
                    catch
                    {
                        EntryErr = true;
                    }
                }
            }
        }

        [RelayCommand]
        public async void AddParamToCalc(string b)
        {
            Form += b.ToString();
            CalcBind += ("[" + ParamCount + "]");
            if(ParamCount == 0)
            {
                CalcBindParams += b.ToString();
            }
            else
            {
                CalcBindParams += ("/" + b.ToString());
            }
            ParamCount++;
        }

        [RelayCommand]
        public async void NewCalcToCalc(string b)
        {
            Form += b.ToString();
            CalcBind += b.ToString();
        }

        [RelayCommand]
        private async void SaveFormula()
        {
            EntryErr = false;
            if (CalcBind == "" || FormName == "" || CalcBindParams == "" || Form == "")
            {
                EntryErr = true;
            }
            else
            {
                var newFormula = new Formula
                {
                    Bind = CalcBind,
                    Params = CalcBindParams,
                    RecordForm = Form,
                    FormDesc = FormName,
                };
                await _database.FormulaAddItem(newFormula);
                LoadItems();
            }
        }

        [RelayCommand]
        private async void LoadFormula(Formula item)
        {
            CalcBind = item.Bind;
            CalcBindParams = item.Params;
            Form = item.RecordForm;
            CalcName = item.FormDesc;
        }

        [RelayCommand]
        private async void DeleteFormula(Formula item)
        {
            await _database.FormulaDeleteItem(item);
            LoadItems();
        }

        [RelayCommand]
        public async void DateChangedFunc(DateTime selectedDate)
        {
            LoadItems();
        }
    }
}
