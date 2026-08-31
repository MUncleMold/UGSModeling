using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Syncfusion.Maui.Charts;
using Syncfusion.Maui.Core;
using System.Collections.Immutable;
using System.Collections.ObjectModel;
using UGSModeling.Data;
using UGSModeling.Models;

namespace UGSModeling.ViewModels
{
    internal partial class BuildGraphViewModel : ObservableObject
    {
        private readonly UGSDataBase _database;

        [ObservableProperty]
        private ObservableCollection<UGSParameter> uGSParameters;

        [ObservableProperty]
        private ObservableCollection<Graph> graphs;

        [ObservableProperty]
        private ObservableCollection<UGSParameter> uGSSearch;

        [ObservableProperty]
        private ObservableCollection<Graph> graphsSearch;

        [ObservableProperty]
        private ObservableCollection<GraphModel> graphData;

        [ObservableProperty]
        private List<string> xSelectTypes;

        [ObservableProperty]
        private List<string> ySelectTypes;

        [ObservableProperty]
        private string xSelectType;

        [ObservableProperty]
        private int xSelectIndex;

        [ObservableProperty]
        private int ySelectIndex;

        [ObservableProperty]
        private List<string> xSelectItems;

        [ObservableProperty]
        private List<string> ySelectItems;

        [ObservableProperty]
        private string xSelectItem;

        [ObservableProperty]
        private string ySelectItem;

        [ObservableProperty]
        public static List<string> uniqNames;

        [ObservableProperty]
        public static List<string> uniqDates;

        [ObservableProperty]
        private bool dataSelectVisible;

        [ObservableProperty]
        private bool paramSelectVisible;

        [ObservableProperty]
        private DateTime minStartDate = new DateTime(1900, 1, 1);

        [ObservableProperty]
        private DateTime maxStartDate = DateTime.Now;

        [ObservableProperty]
        private DateTime selectedStartDate = DateTime.Now;

        [ObservableProperty]
        private DateTime minEndDate = new DateTime(1900, 1, 1);

        [ObservableProperty]
        private DateTime maxEndDate = DateTime.Now;

        [ObservableProperty]
        private DateTime selectedEndDate = DateTime.Now;

        [ObservableProperty]
        private string graphName = "";

        [ObservableProperty]
        private string xAxesTitle;

        [ObservableProperty]
        private string yAxesTitle;

        [ObservableProperty]
        private string buildGraphErr;

        [ObservableProperty]
        private string lbText;

        [ObservableProperty]
        private string searchGraphText = "";

        [ObservableProperty]
        private string searchParamText = "";

        public BuildGraphViewModel()
        {
            _database = new UGSDataBase();
            LoadItems();
            GraphData = new ObservableCollection<GraphModel>();
            XSelectTypes = new List<string> { "Параметр", "Дата" };
            YSelectTypes = new List<string> { "Параметр" };
        }

        [RelayCommand]
        public async void LoadGraps()
        {
            if (SearchGraphText != "")
            {
                GraphsSearch = new ObservableCollection<Graph>(Graphs.Where(x => x.XParam.ToLower().Contains(SearchGraphText.ToLower()) ||
                                                                                            x.YParam.ToLower().Contains(SearchGraphText.ToLower()) ||
                                                                                            x.GraphDesc.ToLower().Contains(SearchGraphText.ToLower())).ToList());
            }
            else
            {
                GraphsSearch = Graphs;
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

            var itemsGraph = await _database.GraphGetItems();
            itemsGraph.Reverse();
            Graphs = new ObservableCollection<Graph>(itemsGraph);

            uniqNames = UGSParameters.Select(x => x.Name).Distinct().ToList();
            uniqDates = UGSParameters.Select(x => x.Date).Distinct().ToList();

            YSelectIndex = 0; YSelectIndex = 1; YSelectIndex = 0;
            YSelectItems = uniqNames;

            LoadParams();
            LoadGraps();
        }

        [RelayCommand]
        private async void BuildGraph()
        {
            BuildGraphErr = "";
            if (XSelectIndex == -1 || xSelectItem == null || ySelectItem == null)
            {
                BuildGraphErr = "!!!ОШИБКА!!! Указанны неверные данные";
            }
            else
            {
                if (XSelectType == "Параметр")
                {
                    GraphData.Clear();

                    XAxesTitle = XSelectItem.ToString();
                    YAxesTitle = YSelectItem.ToString();

                    var Xselect = UGSParameters.OrderBy(x => x.Date).Where(x => x.Name == XSelectItem).ToList();
                    var Yselect = UGSParameters.OrderBy(y => y.Date).Where(y => y.Name == YSelectItem).ToList();

                    if (Xselect.Count() >= Yselect.Count())
                    {
                        for (int i = 0; i < Yselect.Count(); i++)
                        {
                            GraphData.Add(new GraphModel(Xselect[i].Value.ToString(), Yselect[i].Value.ToString()));
                        }
                    }
                    else
                    {
                        for (int i = 0; i < Xselect.Count(); i++)
                        {
                            GraphData.Add(new GraphModel(Xselect[i].Value.ToString(), Yselect[i].Value.ToString()));
                        }
                    }
                }
                else
                {
                    GraphData.Clear();

                    XAxesTitle = "Дата";
                    YAxesTitle = YSelectItem.ToString();

                    var Yselect = UGSParameters.OrderBy(y => y.Date).Where(y => y.Name == YSelectItem).Where(y => selectedStartDate <= Convert.ToDateTime(y.Date) && Convert.ToDateTime(y.Date) <= selectedEndDate).ToList();

                    for (int i = 0; i < Yselect.Count(); i++)
                    {
                        GraphData.Add(new GraphModel(Yselect[i].Date.ToString(), Yselect[i].Value.ToString()));
                    }
                }
            }
        }

        [RelayCommand]
        public async void XChangeOnSelectedIndexChanged(string selectedItem)
        {
            if (selectedItem == "Параметр")
            {
                XSelectItems = uniqNames;
                DataSelectVisible = false;
                ParamSelectVisible = true;
            }
            else
            {
                XSelectItems = uniqDates;
                DataSelectVisible = true;
                ParamSelectVisible = false;
                XSelectItem = "";
            }
        }

        [RelayCommand]
        public async void EntryStartDateChanged(DateTime date)
        {
            SelectedStartDate = date;
        }

        [RelayCommand]
        public async void EntryEndDateChanged(DateTime date)
        {
            SelectedEndDate = date;
        }

        [RelayCommand]
        public async void StartDateChangedFunc(DateTime selectedDate)
        {
            if(SelectedEndDate < SelectedStartDate)
            {
                SelectedEndDate = selectedDate;
            }
        }

        [RelayCommand]
        public async void EndDateChangedFunc(DateTime selectedDate)
        {
            if (SelectedEndDate < SelectedStartDate)
            {
                SelectedStartDate = selectedDate;
            }
        }

        [RelayCommand]
        public async void SaveGraph()
        {
            BuildGraphErr = "";
            if (XSelectIndex == -1 || xSelectItem == null || ySelectItem == null)
            {
                BuildGraphErr = "!!!ОШИБКА!!! Указанны неверные данные";
            }
            else
            {
                if (XSelectType == "Параметр")
                {
                    var newGraph = new Graph
                    {
                        XParam = XSelectItem.ToString(),
                        YParam = YSelectItem.ToString(),
                        GraphDesc = GraphName.ToString(),
                        Type = "PtoP",
                        Path = FileSystem.AppDataDirectory + String.Format("{0}.png", GraphName)
                    };
                    await _database.GraphAddItem(newGraph);
                    LoadItems();
                }
                else
                {
                    var newGraph = new Graph
                    {
                        XParam = SelectedStartDate.ToString("dd.MM.yyyy") + "/" + SelectedEndDate.ToString("dd.MM.yyyy"),
                        YParam = YSelectItem.ToString(),
                        GraphDesc = GraphName.ToString(),
                        Type = "PtoD",
                        Path = FileSystem.AppDataDirectory + String.Format("{0}.png", GraphName)
                    };
                    await _database.GraphAddItem(newGraph);
                    LoadItems();
                }
            }
        }

        [RelayCommand]
        private async void DeleteGraph(Graph item)
        {
            await _database.GraphDeleteItem(item);
            LoadItems();
        }

        [RelayCommand]
        private async void LoadGraph(Graph item)
        {
            if (item.XParam.ToString().Contains("/"))
            {
                XSelectIndex = 1;
                var data = item.XParam.ToString().Split("/");
                SelectedStartDate = Convert.ToDateTime(data[0].ToString());
                SelectedEndDate = Convert.ToDateTime(data[1].ToString());
                YSelectItem = item.YParam.ToString();
                GraphName = item.GraphDesc.ToString();
                BuildGraph();
            }
            else
            {
                XSelectIndex = 0;
                XSelectItem = item.XParam.ToString();
                YSelectItem = item.YParam.ToString();
                GraphName = item.GraphDesc.ToString();
                BuildGraph();
            }
        }
    }
}

public class GraphModel
{
    public string X { get; set; }
    public string Y { get; set; }

    public GraphModel(string x, string y)
    {
        X = x;
        Y = y;
    }
}
