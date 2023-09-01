using iText.Kernel.Geom;
using iText.Kernel.Pdf;
using iText.Kernel.Pdf.Canvas;
using iText.Kernel.Pdf.Xobject;

namespace PdfWritterPoc
{
    public class CopyTo
    {
        public static readonly string DEST = "pdfOutput.pdf";
        public static readonly int numColumns = 4;
        public static readonly int numRows = 4;
        public static long fileSize = 0;
        public static float xPosition;
        public static float yPosition;
        public static float a4Width;
        public static float a4Height;
        public static long maxFileSize = 8000000; //8Mb limit
        //private static string[] srcFiles = { "output.pdf", "output2.pdf", "output3.pdf" };
        private static string[] srcFiles = new string[7];
        public static void Main(string[] args)
        {
            iText.Kernel.Pdf.PdfDocument pdfDocument = new(new PdfWriter(DEST));
            PageSize pdfPageFile = new PageSize(2380, 1700);
            pdfDocument.AddNewPage(pdfPageFile);

            for (var a = 0; a < 7; a++)
            {
                srcFiles[a] = $"output{a + 1}.pdf";
            }

            a4Width = PageSize.A4.GetWidth();
            a4Height = PageSize.A4.GetHeight();
            xPosition = 0;
            yPosition = 0;

            float totalWidth = pdfPageFile.GetWidth();
            float totalHeight = pdfPageFile.GetHeight();

            int i = 1;

            //interate troghout the pdfs sources
            foreach (var srcFile in srcFiles)
            {
                (xPosition, yPosition, fileSize) = CopyToFile(i, xPosition, yPosition, a4Width, totalWidth, totalHeight, a4Height, srcFile, pdfDocument, fileSize);
                i++;
            }
            pdfDocument.Close();
        }
        private static (
            float xPosition,
            float yPosition,
            long fileSize)
        CopyToFile(
            int index,
            float xPosition,
            float yPosition,
            float a4Width,
            float totalWidth,
            float totalHeight,
            float a4Height,
            string pdfReader,
            PdfDocument pdfDocument,
            long fileSize)
        {
            try
            {
                PdfReader reader = new(pdfReader);
                PdfDocument readerDocument = new(reader);
                int numberOfPages = readerDocument.GetNumberOfPages();

                fileSize += reader.GetFileLength();

                //The final file
                PdfCanvas canvas = new(pdfDocument.GetPage(1));

                //Loop for the pages interation
                if (fileSize <= maxFileSize)
                {
                    for (int i = 1; i <= numberOfPages; i++)
                    {
                        if ((totalHeight - yPosition) > a4Height)
                        {
                            PdfPage page = readerDocument.GetPage(i);

                            PdfFormXObject pageXObject = page.CopyAsFormXObject(pdfDocument);
                            canvas.AddXObjectAt(pageXObject, xPosition, yPosition);
                            xPosition += a4Width;
                            if ((totalWidth - xPosition) < a4Width)
                            {
                                //it jumps to the line above and restarts the at the x 0 position
                                yPosition += a4Height;
                                xPosition = 0;
                            }
                        }
                    }
                }
                return (xPosition, yPosition, fileSize);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return (xPosition, yPosition, 0);
            }
        }
    }
}
