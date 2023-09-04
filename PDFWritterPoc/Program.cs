using iText.Kernel.Geom;
using iText.Kernel.Pdf;
using iText.Kernel.Pdf.Canvas;
using iText.Kernel.Pdf.Xobject;

namespace PdfWritterPoc
{
    public class CopyTo
    {
        public static readonly string output = "pdfOutput.pdf";
        public static long fileSize = 0;
        public static float xPosition;
        public static float yPosition;
        public static float a4Width;
        public static float a4Height;
        public static int areaEmpty; //total empty spaces in the matrix
        public static long maxFileSize = 8000000; //8Mb limit
        private static string[] srcFiles = new string[7];
        public static void Main(string[] args)
        {
            iText.Kernel.Pdf.PdfDocument pdfDocument = new(new PdfWriter(output));
            PageSize pdfPageFile = new PageSize(2380, 1700);
            pdfDocument.AddNewPage(pdfPageFile);

            for (var j = 0; j < 7; j++)
            {
                srcFiles[j] = $"output{j + 1}.pdf";
            }

            a4Width = PageSize.A4.GetWidth();
            a4Height = PageSize.A4.GetHeight();

            float totalWidth = pdfPageFile.GetWidth();
            float totalHeight = pdfPageFile.GetHeight();

            int numOfCols = (int)(totalWidth / a4Width);
            int numOfRows = (int)(totalHeight / a4Height);

            areaEmpty = numOfCols * numOfRows;

            //interate troghout the pdfs sources
            foreach (var srcFile in srcFiles)
            {
                (xPosition, yPosition, fileSize, areaEmpty) = CopyToFile(
                    xPosition,
                    yPosition,
                    a4Width,
                    totalWidth,
                    totalHeight,
                    a4Height,
                    srcFile,
                    pdfDocument,
                    areaEmpty,
                    fileSize
                );
            }
            pdfDocument.Close();
        }

        private static (
            float xPosition,
            float yPosition,
            long fileSize,
            int areaEmpty)
        CopyToFile(
            float xPosition,
            float yPosition,
            float a4Width,
            float totalWidth,
            float totalHeight,
            float a4Height,
            string pdfReader,
            PdfDocument pdfDocument,
            int areaEmpty,
            long fileSize)
        {
            try
            {
                //PdfReader reader = new(pdfReader);
                using (PdfReader reader = new PdfReader(pdfReader))
                {
                    using (PdfDocument readerDocument = new PdfDocument(reader))
                    {
                        //PdfDocument readerDocument = new(reader);
                        int numberOfPages = readerDocument.GetNumberOfPages();
                        int _emptySpace = areaEmpty;

                        fileSize += reader.GetFileLength();
                        areaEmpty = TryOperate(areaEmpty, numberOfPages);

                        if (_emptySpace != areaEmpty)
                        {
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
                        }
                        return (xPosition, yPosition, fileSize, areaEmpty);
                    }
                }
            }
            catch (Exception)
            {
                return (xPosition, yPosition, 0, areaEmpty);
            }
        }
        private static int TryOperate(int emptySpaces, int pages)
        {
            if (emptySpaces - pages < 0)
                return emptySpaces;
            else
                return emptySpaces - pages;
        }
    }
}
