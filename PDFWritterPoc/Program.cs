using iText.Kernel.Geom;
using iText.Kernel.Pdf;
using iText.Layout;
using iText.Html2pdf;

//PdfDocument pdfDocument = new PdfDocument(new PdfWriter(
//new FileStream("/Users/CODE-DEV-43/Documents/documentFinal.pdf", FileMode.Create, FileAccess.Write)));
//Document documentFinal = new Document(pdfDocument, PageSize.A1);

namespace PdfWritterDoc;
class Program
{

    public static void Main(string[] args)
    {
        string htmlFilePath = "/Users/CODE-DEV-43/Documents/html(1).html";
        Stream htmlStream = new FileStream(htmlFilePath, FileMode.Open);

        //path to the final PDF
        PdfDocument finalOutFile = new PdfDocument(new PdfWriter("OutputFilePath.pdf"));
        Document finalDocument = new Document(finalOutFile, PageSize.A0);

        //creating in the memory
        PdfDocument[] sourcePdfs = new PdfDocument[6];

        //path to the generated test pdf
        PdfDocument pdfDocument = new PdfDocument(new PdfWriter("output.pdf"));


        if (htmlStream.Length > 0)
            ConvertHtmlToPdf(htmlStream, pdfDocument);
        pdfDocument.Close();


        var outputPdf = File.ReadAllBytes("output.pdf");
        int fileTotalPages = PageCount(outputPdf);

        if (fileTotalPages > 1)
        {
            for (int i = 0; i < fileTotalPages; i++)
            {
                sourcePdfs[i] = new PdfDocument(new PdfReader("output.pdf"));
            }
        }

        static int PageCount(byte[] pdf)
        {
            using (var stream = new MemoryStream(pdf))
            {
                using (var reader = new PdfReader(stream))
                {
                    using (var document = new PdfDocument(reader))
                    {
                        return document.GetNumberOfPages();
                    }
                }
            }
        }
        static void ConvertHtmlToPdf(Stream htmlFile, PdfDocument pdfDocument)
        {
            HtmlConverter.ConvertToPdf(htmlFile, pdfDocument, new ConverterProperties());
        }
    }
}