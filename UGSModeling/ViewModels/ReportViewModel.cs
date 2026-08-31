using CommunityToolkit.Maui.Storage;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using DocumentFormat.OpenXml;
using DocumentFormat.OpenXml.Bibliography;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Wordprocessing;
using Syncfusion.Maui.Charts;
using Syncfusion.Maui.Core;
using System.Collections.Immutable;
using System.Collections.ObjectModel;
using System.Windows.Input;
using UGSModeling.Data;
using UGSModeling.Models;

using A = DocumentFormat.OpenXml.Drawing;
using DW = DocumentFormat.OpenXml.Drawing.Wordprocessing;
using PIC = DocumentFormat.OpenXml.Drawing.Pictures;
using SD = System.Drawing;



namespace UGSModeling.ViewModels
{
    internal partial class ReportViewModel :ObservableObject
    {
        private readonly UGSDataBase _database;

        [ObservableProperty]
        private ObservableCollection<UGSParameter> uGSParameters; //список параметров, для отчета не нужен

        [ObservableProperty]
        private ObservableCollection<Graph> _graphs;

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

        public static List<string> uniqNames;
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
        private string graphName;

        [ObservableProperty]
        private string xAxesTitle;

        [ObservableProperty]
        private string yAxesTitle;

        [ObservableProperty]
        private string lbText;

       
        private string? folderPath;
        MemoryStream memorystream;

        [ObservableProperty]
        private ObservableCollection<GraphUI> _graphs2 = new();

        [ObservableProperty]
        private ObservableCollection<UGSReport> _reports = new();

        public ICommand OpenFileCommand { get; }

        public ICommand ReportDeleteCommand{ get; }


        [ObservableProperty]
        private string _reportName;

        public ReportViewModel()
        {
            _database = new UGSDataBase();
            LoadItems();
            GraphData = new ObservableCollection<GraphModel>();
            memorystream = CreateWordDocument();
            OpenFileCommand = new Command<string>(OnOpenFile);
            ReportDeleteCommand = new Command<UGSReport>(OnDelete);


        }

        [RelayCommand]
        private async void LoadItems() //загрузка данных
        {
            //загрузка параметров, для отчета не нужна
            var items = await _database.ParamGetItems();
            items.Reverse();
            UGSParameters = new ObservableCollection<UGSParameter>(items);

            var itemsGraph = await _database.GraphGetItems();
            itemsGraph.Reverse();
            Graphs = new ObservableCollection<Graph>(itemsGraph);

            uniqNames = UGSParameters.Select(x => x.Name).Distinct().ToList();
            uniqDates = UGSParameters.Select(x => x.Date).Distinct().ToList();

            YSelectIndex = 0; YSelectIndex = 1; YSelectIndex = 0;
            XSelectIndex = 0; XSelectIndex = 1; XSelectIndex = 0;
            YSelectItems = uniqNames;
            Graphs2.Clear();
            foreach (var graph in Graphs)
            {
                Graphs2.Add(new GraphUI(graph));
            }

            var itemsRep = await _database.ReportGetItem();
            itemsRep.Reverse();
            Reports = new ObservableCollection<UGSReport>(itemsRep);




        }
       
        [RelayCommand]
        private async void LoadGraph(Graph item) //функция загрузки
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

        [RelayCommand]
        private async void BuildGraph() //функция построения
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



       

        //private async void SelectFolderButtonClicked(object sender, EventArgs e)
        //{
        //    var folder = await FolderPicker.PickAsync(default);
        //    folderPath = folder.Folder.Path;
        //    FilePathLabel.Text = folderPath;
        //    await Task.Run(() => CreateWordDocument());

        //    memorystream = CreateWordDocument();

        //}
        [RelayCommand]
        private async void PickGraph(Graph item) //Vibori
        {
            await _database.GraphDeleteItem(item);
            LoadItems();
        }






        //---------------------------------Дальше бога нет------------------------------------------

      

        private Dictionary<Graph, bool> _processingGraphs = new Dictionary<Graph, bool>();
        private Dictionary<Graph, string> _buttonTexts = new Dictionary<Graph, string>();


        [ObservableProperty]
        private Graph _choosedGraph;

        [ObservableProperty]
        private string _buttonLabel = "Добавить в отчет";


        [ObservableProperty]
        private bool _isEnable = true;

        [RelayCommand]
    
        //Старая хуйня для кнопки, оставил по приколу
        private async Task ChooseGraph(Graph graphItem)
        {
            if (graphItem != null)
            {
                ChoosedGraph = graphItem;
                InsertImageIntoWordDocument(memorystream, ChoosedGraph.Path);
                string[,] Params = await GetArrAsync(graphItem);
                InsertTableToDocument(memorystream, Params);

                ButtonLabel = "График добавлен";
            }


        }

        [RelayCommand]
        private async Task AddGraph(GraphUI graphItem)
        {
            if (graphItem != null)
            {
                InsertImageIntoWordDocument(memorystream, graphItem.Path);
                string[,] Params = await GetArrAsync(graphItem);
                InsertTableToDocument(memorystream, Params);

            }


        }
        public async void OnDelete(UGSReport report)
        {
            await _database.ReportDelete(report);
            LoadItems();

        }

        public async void OnOpenFile(string filePath)
        {
            if (string.IsNullOrEmpty(filePath))
            {
                await Application.Current.MainPage.DisplayAlert("Ошибка", "Путь к файлу не указан", "OK");
                return;
            }

            try
            {
                // Проверяем, существует ли файл
                if (!File.Exists(filePath))
                {
                    await Application.Current.MainPage.DisplayAlert("Ошибка", $"Файл не найден: {filePath}", "OK");
                    return;
                }

                // Открываем файл с помощью Launcher
                var success = await Launcher.Default.OpenAsync(new OpenFileRequest
                {
                    File = new ReadOnlyFile(filePath)
                });

                if (!success)
                {
                    await Application.Current.MainPage.DisplayAlert("Ошибка", "Не удалось открыть файл", "OK");
                }
            }
            catch (Exception ex)
            {
                await Application.Current.MainPage.DisplayAlert("Ошибка", $"Не удалось открыть файл: {ex.Message}", "OK");
            }
        }


        [ObservableProperty]
        private string _selectedFolderPath;



        //[RelayCommand]
        //private async Task SelectFolder()
        //{
        //    try
        //    {
        //        var folder = await FolderPicker.PickAsync(default);
        //        if (folder?.Folder != null)
        //        {
        //            SelectedFolderPath = folder.Folder.Path;


        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        // Обработка ошибки
        //        await Application.Current.MainPage.DisplayAlert("Ошибка", ex.Message, "OK");
        //    }
        //}
        
        
        
        
        
        //---------------------МАТЕРИК ВОРДА!!----------------------------------------



        //Тестовая хуйня для кнопки
        //БЛЯТЬ Всмысле теестовая? Я эту хуйню реально использую вообще-то
        [RelayCommand]
        private async void SaveWordDocumentButton()
        {
            //string fileName = $"Отчет_{DateTime.Now:ddMMyyyy}.docx";
            string fileName = ReportName;
            List<GraphUI> selectedGraphs = GetSelectedGraphs();
            foreach (GraphUI graph in selectedGraphs)
            {
               await AddGraph(graph);
            }
            string fullPath = FileSystem.AppDataDirectory + String.Format("\\{0}.docx", fileName);

            SaveDocumentToFile(memorystream, fullPath);

            var report = new UGSReport
            {
                Name = fileName,
                Date = DateTime.Now.ToString(),
                Path = fullPath,
                UserId = 0
            };
            await _database.ReportAddItem(report);
            await Application.Current.MainPage.DisplayAlert("Готово", "Отчет создан", "ОК");
            LoadItems();
            memorystream = CreateWordDocument();
        }

        private List<GraphUI> GetSelectedGraphs()
        {
            return Graphs2
                .Where(r => r.IsSelected)
                .ToList();
        }

        //Сохраняет документ по указанному пути
        private void SaveDocumentToFile(MemoryStream documentStream, string filePath)
        {
            using (FileStream fileStream = new FileStream(filePath, FileMode.Create, FileAccess.Write))
            {
                documentStream.WriteTo(fileStream);
            }
        }
        
        //Создает документ и пихает в него заголовок или типо того
        private MemoryStream CreateWordDocument()
        {
            var memoryStream = new MemoryStream();

            using (WordprocessingDocument wordDocument = WordprocessingDocument.Create(memoryStream, WordprocessingDocumentType.Document))
            {
                MainDocumentPart mainPart = wordDocument.AddMainDocumentPart();
                mainPart.Document = new Document();

                Body body = new Body();

                Paragraph titleParagraph = new Paragraph();
                Run titleRun = new Run();
                RunProperties titleProperties = new RunProperties();
                titleProperties.Bold = new Bold();
                titleProperties.FontSize = new DocumentFormat.OpenXml.Wordprocessing.FontSize() { Val = "32" };
                titleRun.PrependChild(titleProperties);
                titleRun.AppendChild(new Text("Отчет"));
                titleParagraph.AppendChild(titleRun);
                body.AppendChild(titleParagraph);

                body.AppendChild(new Paragraph());

                // Дата
                Paragraph dateParagraph = new Paragraph();
                Run dateRun = new Run();
                dateRun.AppendChild(new Text($"Дата создания: {DateTime.Now:dd.MM.yyyy}"));
                dateParagraph.AppendChild(dateRun);
                body.AppendChild(dateParagraph);

                body.AppendChild(new Paragraph());



                mainPart.Document.AppendChild(body);
                mainPart.Document.Save();
            }
            memoryStream.Position = 0; // Сбрасываем позицию в начало
            return memoryStream;
            //InsertImageIntoWordDocument(filePath, "C:\\Users\\Mold\\AppData\\Local\\User Name\\com.companyname.ugsmodeling\\Data\\123.png");

        }



        //Вставляет изобрадение
        public static void InsertImageIntoWordDocument(string documentPath, string imagePath)
        {

            using (WordprocessingDocument wordDocument = WordprocessingDocument.Open(documentPath, true))
            {
                if (wordDocument.MainDocumentPart is null)
                {
                    throw new ArgumentNullException(nameof(wordDocument.MainDocumentPart), "А где?");
                }


                MainDocumentPart mainPart = wordDocument.MainDocumentPart;


                ImagePart imagePart = mainPart.AddImagePart(ImagePartType.Jpeg);
                using (FileStream stream = new FileStream(imagePath, FileMode.Open))
                {
                    imagePart.FeedData(stream);
                }

                string relationshipId = mainPart.GetIdOfPart(imagePart);


                SD.Image image = SD.Image.FromFile(imagePath);

                var drawingElement = CreateImageDrawingElement(relationshipId, image);

                wordDocument.MainDocumentPart.Document.Body.AppendChild(new Paragraph(new Run(drawingElement)));
            }
        }
        //Перегрузка с мемористримом (НАХУЯ ПЕРЕГРУЖАТЬ??? ЗАЧЕМ МНЕ СТАРАЯ ВЕРСИЯ???)
        public static void InsertImageIntoWordDocument(Stream documentStream, string imagePath)
        {

            using (WordprocessingDocument wordDocument = WordprocessingDocument.Open(documentStream, true))
            {
                if (wordDocument.MainDocumentPart is null)
                {
                    throw new ArgumentNullException(nameof(wordDocument.MainDocumentPart), "А где?");
                }


                MainDocumentPart mainPart = wordDocument.MainDocumentPart;


                ImagePart imagePart = mainPart.AddImagePart(ImagePartType.Jpeg);
                using (FileStream stream = new FileStream(imagePath, FileMode.Open))
                {
                    imagePart.FeedData(stream);
                }

                string relationshipId = mainPart.GetIdOfPart(imagePart);


                SD.Image image = SD.Image.FromFile(imagePath);

                var drawingElement = CreateImageDrawingElement(relationshipId, image);

                wordDocument.MainDocumentPart.Document.Body.AppendChild(new Paragraph(new Run(drawingElement)));
            }
        }
        //Господи блять
        private static Drawing CreateImageDrawingElement(string relationshipId, System.Drawing.Image image)
        {
            const long emusPerInch = 914400;
            const long targetWidthEmus = 6 * emusPerInch;
            double aspectRatio = (double)image.Height / image.Width;
            long targetHeightEmus = (long)(targetWidthEmus * aspectRatio);





            return new Drawing(
                new DW.Inline(
                    new DW.Extent() { Cx = targetWidthEmus, Cy = targetHeightEmus },
                    new DW.EffectExtent()
                    {
                        LeftEdge = 0L,
                        TopEdge = 0L,
                        RightEdge = 0L,
                        BottomEdge = 0L
                    },
                    new DW.DocProperties()
                    {
                        Id = (UInt32Value)1U,
                        Name = "Picture 1"
                    },
                    new DW.NonVisualGraphicFrameDrawingProperties(
                        new A.GraphicFrameLocks() { NoChangeAspect = true }),
                    new A.Graphic(
                        new A.GraphicData(
                            new PIC.Picture(
                                new PIC.NonVisualPictureProperties(
                                    new PIC.NonVisualDrawingProperties()
                                    {
                                        Id = (UInt32Value)0U,
                                        Name = "My Image.jpg"
                                    },
                                    new PIC.NonVisualPictureDrawingProperties()),
                                new PIC.BlipFill(
                                    new A.Blip(
                                        new A.BlipExtensionList(
                                            new A.BlipExtension()
                                            {
                                                Uri = "{28A0092B-C50C-407E-A947-70E740481C1C}"
                                            })
                                    )
                                    {
                                        Embed = relationshipId,
                                        CompressionState = A.BlipCompressionValues.Print
                                    },
                                    new A.Stretch(
                                        new A.FillRectangle())),
                                new PIC.ShapeProperties(
                                    new A.Transform2D(
                                        new A.Offset() { X = 0L, Y = 0L },
                                        new A.Extents() { Cx = targetWidthEmus, Cy = targetHeightEmus }),
                                    new A.PresetGeometry(
                                        new A.AdjustValueList()
                                    )
                                    { Preset = A.ShapeTypeValues.Rectangle }))
                        )
                        { Uri = "http://schemas.openxmlformats.org/drawingml/2006/picture" })
                )
                {
                    DistanceFromTop = (UInt32Value)0U,
                    DistanceFromBottom = (UInt32Value)0U,
                    DistanceFromLeft = (UInt32Value)0U,
                    DistanceFromRight = (UInt32Value)0U,
                    EditId = "50D07946"
                });
        }


        //---------------------------ОСТРОВА ТАБЛИЦ---------------------------------------------

     
        //Я это пишу пока еще нихуя не начал писать, но по задумке эта функция должна извлекать ИЗВЛЕКАть получается данные, на которых строится график и сохранять их как двумерный ДВУМЕРНЫЙ ПОЛУЧАЕТСЯ массив получается НАДЕЮСЬ Я НЕ ПОСТРАДАЮ!!
        private async Task<string[,]> GetArrAsync(Graph graph)
        {
            var items = await _database.ParamGetItems();
            items.Reverse();
            UGSParameters = new ObservableCollection<UGSParameter>(items);

            var Xparam = items.OrderBy(x => x.Date).Where(x => x.Name == graph.XParam).ToList();

            var Yparam = items.OrderBy(x => x.Date).Where(x => x.Name == graph.YParam).ToList();
            
            string[,] Arr = new string[Xparam.Count +1, Yparam.Count+1];
            Arr[0, 0] = graph.XParam;
            Arr[0,1] = graph.YParam;
            int c = 1;
            foreach(UGSParameter x in Xparam)
            {
                Arr[c,0] = x.Value;
                c++;
            }
            c = 1;
            foreach (UGSParameter x in Yparam)
            {
                Arr[c, 1] = x.Value;
                c++;
            }
            return Arr;

        }

        //Перегрузка под уродливого брата-близнеца
        private async Task<string[,]> GetArrAsync(GraphUI graph)
        {
            var items = await _database.ParamGetItems();
            items.Reverse();
            UGSParameters = new ObservableCollection<UGSParameter>(items);

            var Xparam = items.OrderBy(x => x.Date).Where(x => x.Name == graph.XParam).ToList();

            var Yparam = items.OrderBy(x => x.Date).Where(x => x.Name == graph.YParam).ToList();

            string[,] Arr = new string[Xparam.Count + 1, Yparam.Count + 1];
            Arr[0, 0] = graph.XParam;
            Arr[0, 1] = graph.YParam;
            int c = 1;
            foreach (UGSParameter x in Xparam)
            {
                Arr[c, 0] = x.Value;
                c++;
            }
            c = 1;
            foreach (UGSParameter x in Yparam)
            {
                Arr[c, 1] = x.Value;
                c++;
            }
            return Arr;

        }


        // Создает таблицу из двумерного массива данных и вставляет в документ
        public void InsertTableToDocument(Stream documentStream, string[,] data)
        {
            using (WordprocessingDocument wordDoc = WordprocessingDocument.Open(documentStream, true))
            {
                if (wordDoc.MainDocumentPart?.Document.Body == null)
                    throw new Exception("Не удалось получить тело документа");

                Table table = new Table();

                TableProperties tableProperties = CreateTableProperties();
                table.AppendChild<TableProperties>(tableProperties);

                for (int row = 0; row <= data.GetUpperBound(0); row++)
                {
                    TableRow tableRow = new TableRow();

                    for (int col = 0; col <= 1; col++)
                    {
                        TableCell cell = CreateTableCell(data[row, col]);
                        tableRow.Append(cell);
                    }

                    table.Append(tableRow);
                }

                wordDoc.MainDocumentPart.Document.Body.Append(table);
            }
        }

        // Создает и настраивает ячейку таблицы
        private TableCell CreateTableCell(string text)
        {
            TableCell cell = new TableCell();

            Paragraph paragraph = new Paragraph();
            Run run = new Run();
            run.AppendChild(new Text(text));
            paragraph.AppendChild(run);
            cell.AppendChild(paragraph);

            cell.AppendChild(new TableCellProperties(
                new TableCellWidth { Type = TableWidthUnitValues.Auto }));

            return cell;
        }

        // Создает оформление таблицы (рамки)
        private TableProperties CreateTableProperties()
        {
            TableProperties properties = new TableProperties(
                new TableBorders(
                    new TopBorder
                    {
                        Val = new EnumValue<BorderValues>(BorderValues.Single),
                        Size = 12
                    },
                    new BottomBorder
                    {
                        Val = new EnumValue<BorderValues>(BorderValues.Single),
                        Size = 12
                    },
                    new LeftBorder
                    {
                        Val = new EnumValue<BorderValues>(BorderValues.Single),
                        Size = 12
                    },
                    new RightBorder
                    {
                        Val = new EnumValue<BorderValues>(BorderValues.Single),
                        Size = 12
                    },
                    new InsideHorizontalBorder
                    {
                        Val = new EnumValue<BorderValues>(BorderValues.Single),
                        Size = 12
                    },
                    new InsideVerticalBorder
                    {
                        Val = new EnumValue<BorderValues>(BorderValues.Single),
                        Size = 12
                    }
                )
            );

            return properties;
        }
    }


    public partial class GraphUI : ObservableObject
    {
        private readonly Graph _graph;

        public GraphUI(Graph graph)
        {
            _graph = graph;
        }

        public int Id => _graph.Id;
        public string XParam => _graph.XParam;
        public string YParam => _graph.YParam;
        public string GraphDesc => _graph.GraphDesc;
        public string Type => _graph.Type;
        public string Path => _graph.Path;




        [ObservableProperty]
        private bool _isSelected;
    }



}


