using CommunityToolkit.Maui.Storage;
using DocumentFormat.OpenXml;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Wordprocessing;
using UGSModeling.ViewModels;
using A = DocumentFormat.OpenXml.Drawing;
using DW = DocumentFormat.OpenXml.Drawing.Wordprocessing;
using PIC = DocumentFormat.OpenXml.Drawing.Pictures;
using SD = System.Drawing;

namespace UGSModeling;

public partial class ReportPage : ContentPage
{
	public ReportPage()
	{
        
        InitializeComponent();
        BindingContext = new ReportViewModel();
    }
    private async void GoToMainPage(object sender, EventArgs e)
    {
        await Shell.Current.GoToAsync("//MainPage");
    }
    private string? folderPath;
    MemoryStream memorystream;
    private async void SelectFolderButtonClicked(object sender, EventArgs e)
    {
        var folder = await FolderPicker.PickAsync(default);
        folderPath = folder.Folder.Path;
        FilePathLabel.Text = folderPath;
        await Task.Run(() => CreateWordDocument());

        memorystream = CreateWordDocument();


    }
    private async void SaveWordDocumentButton(object sender, EventArgs e)
	{
        string fileName = $"Отчет_{DateTime.Now:ddMMyyyy}.docx";
        string fullPath = System.IO.Path.Combine(folderPath, fileName);
        SaveDocumentToFile(memorystream, fullPath);

    }

    private void SaveDocumentToFile(MemoryStream documentStream, string filePath)
    {
        using (FileStream fileStream = new FileStream(filePath, FileMode.Create, FileAccess.Write))
        {
            documentStream.WriteTo(fileStream);
        }
    }

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
            titleProperties.FontSize = new  DocumentFormat.OpenXml.Wordprocessing.FontSize() { Val = "32" };
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



}