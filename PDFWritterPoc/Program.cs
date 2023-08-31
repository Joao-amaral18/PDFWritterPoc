using iText.Kernel.Geom;
using iText.Kernel.Pdf;
using iText.Kernel.Pdf.Canvas;
using iText.Kernel.Pdf.Xobject;

namespace PdfWritterPoc
{
    public class CopyTo
    {
        public static readonly string DEST = "out.pdf";
        public static readonly int numColumns = 4;
        public static readonly int numRows = 4;
        public static float xPosition = 0;
        public static float yPosition = 0;
        public static float a4Width;
        public static float a4Height;
        private static string srcFile = "output.pdf";

        public static void Main(string[] args)
        {
            PdfDocument pdfDocument = new PdfDocument(new PdfWriter(DEST));
            PageSize a0PageSize = PageSize.A0;
            pdfDocument.AddNewPage(a0PageSize);
            pdfDocument.Close();


            a4Width = PageSize.A4.GetWidth();
            a4Height = PageSize.A4.GetHeight();


            float totalWidth = a4Width * numColumns;
            float totalHeight = a4Height * numRows;
            float columnWidth = PageSize.A0.GetWidth();
            float columnHeight = PageSize.A0.GetHeight();

            for (int i = 0; i <= numColumns; i++)
            {
                int rowIndex = i % numColumns;
                yPosition = rowIndex * columnHeight;

                for (int j = 1; j <= (numRows + 1); i++)
                {
                    int columnIndex = j % numColumns;
                    xPosition = columnIndex * columnWidth;
                    CopyToFile(j, xPosition, yPosition, a4Width, srcFile, pdfDocument);
                }
                yPosition += a4Height;
            }

        }
        // private static void CopyToFile(int index, float xPosition, float yPosition, float a4Width, string srcFile, PdfDocument pdfDocument)
        // {

        //     PdfDocument pdfReader = new PdfDocument(new PdfReader(srcFile));
        //     int pages = pdfReader.GetNumberOfPages();
        //     PdfPage page = pdfReader.GetPage(index);
        //     PdfCanvas canvas = new PdfCanvas(page);

        //     for (int i = 1; i <= pages || i <= numColumns; i++)
        //     {
        //         PdfFormXObject pageXObject = page.CopyAsFormXObject(pdfDocument);
        //         canvas.AddXObjectAt(pageXObject, xPosition, yPosition);
        //         xPosition += a4Width;
        //     }
        //     pdfReader.Close();
        // }
        private static void CopyToFile(int index, float xPosition, float yPosition, float a4Width, PdfDocument pdfReader, PdfDocument pdfDocument)
        {
            int pages = pdfReader.GetNumberOfPages();
            PdfPage page = pdfReader.GetPage(index);

            using (PdfCanvas canvas = new PdfCanvas(pdfDocument.GetPage(1)))
            {
                for (int i = 1; i <= pages && i <= numColumns; i++)
                {
                    PdfFormXObject pageXObject = page.CopyAsFormXObject(pdfDocument);
                    canvas.AddXObject(pageXObject, xPosition, yPosition);
                    xPosition += a4Width;
                }
            }
        }
    }
}
