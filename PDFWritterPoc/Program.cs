using ImageMagick;
using iText.IO.Image;
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
        public static long maxImageSize = 3000000;
        private static string[] srcFiles = { "autorizacao.pdf", "carimbotempo.pdf", "8mb.png", "9mb.png", "ConjuntoEvid.pdf" };

        public static void Main()
        {
            var pdfDocument = new PdfDocument(new PdfWriter(output));
            var pdfPageFile = new PageSize(1800, 1700);
            pdfDocument.AddNewPage(pdfPageFile);

            a4Width = PageSize.A4.GetWidth() + 2;//+2 pixels of padding
            a4Height = PageSize.A4.GetHeight();

            float totalWidth = pdfPageFile.GetWidth();
            float totalHeight = pdfPageFile.GetHeight();
            yPosition = totalHeight - a4Height;

            int numOfCols = (int)(totalWidth / a4Width);
            int numOfRows = (int)(totalHeight / a4Height);
            areaEmpty = numOfCols * numOfRows;

            //interate troghout the pdfs sources
            foreach (var srcFile in srcFiles)
                CopyPdfToFile(totalWidth, srcFile, pdfDocument, areaEmpty, fileSize);

            pdfDocument.Close();
        }

        private static void CopyPdfToFile(float totalWidth, string srcFile, PdfDocument pdfDocument, int occupyingArea, long fileSize)
        {
            try
            {
                var fileExtension = System.IO.Path.GetExtension(srcFile);
                if (fileExtension.Equals(".pdf", StringComparison.OrdinalIgnoreCase))
                {
                    using (var reader = new PdfReader(srcFile))
                    {
                        using (var readerDocument = new PdfDocument(reader))
                        {
                            int numberOfPages = readerDocument.GetNumberOfPages();
                            int initialEmptyArea = occupyingArea;

                            occupyingArea = TryOperate(occupyingArea, numberOfPages);
                            fileSize += reader.GetFileLength();

                            // If the empity area is the same it was befor adding pages, break;
                            if (initialEmptyArea == occupyingArea)
                                return;

                            // If the file is too large, do not merge it
                            if (fileSize + reader.GetFileLength() > maxFileSize)
                                return;

                            //The final file
                            PdfCanvas canvas = new(pdfDocument.GetPage(1));
                            InsertPdfIntoCanvas(numberOfPages, readerDocument, canvas, totalWidth, pdfDocument);

                            // Update the file size
                            fileSize += reader.GetFileLength();
                            return;
                        }
                    }
                }
                else if (fileExtension.Equals(".png", StringComparison.OrdinalIgnoreCase) || fileExtension.Equals(".jpg", StringComparison.OrdinalIgnoreCase))
                    InsertPngIntoCanvas(totalWidth, pdfDocument, srcFile);
            }
            catch (Exception)
            {
                return;
            }
        }
        private static void InsertPngIntoCanvas(float totalWidth, PdfDocument pdfDocument, string srcFile)
        {
            {
                var canvas = new PdfCanvas(pdfDocument.GetPage(1));

                var memoryStream = ManipulateImage(srcFile, maxImageSize, a4Width, a4Height);
                byte[] bytes = memoryStream.ToArray();

                // Create an ImageData object.
                var image = ImageDataFactory.Create(bytes);

                canvas.AddImageAt(image, xPosition, yPosition, true);
                xPosition += a4Width;
                if ((totalWidth - xPosition) < a4Width)
                {
                    //it jumps to the line bellow and restarts the at the x 0 position
                    yPosition -= a4Height;
                    xPosition = 0;
                }
            }
        }
        private static void InsertPdfIntoCanvas(int numberOfPages, PdfDocument readerDocument, PdfCanvas canvas, float totalWidth, PdfDocument pdfDocument)
        {

            for (int i = 1; i <= numberOfPages; i++)
            {
                PdfPage page = readerDocument.GetPage(i);
                PdfFormXObject pageXObject = page.CopyAsFormXObject(pdfDocument);
                canvas.AddXObjectAt(pageXObject, xPosition, yPosition);
                xPosition += a4Width;
                if ((totalWidth - xPosition) < a4Width)
                {
                    //it jumps to the line bellow and restarts the at the x 0 position
                    yPosition -= a4Height;
                    xPosition = 0;
                }
            }
        }
        public static MemoryStream ManipulateImage(string sourcePath, long maxSize, float a4Width, float a4Height)
        {
            MemoryStream stream = new MemoryStream();

            using (MagickImage image = new MagickImage(sourcePath))
            {
                //image.Scale(new Percentage(80));
                if (image.Width > a4Width && image.Width > image.Height)
                {
                    image.Rotate(90);
                    image.Resize((int)a4Width, (int)a4Height);
                }
                else if (image.Width > a4Width)
                {
                    image.Resize((int)a4Width, (int)a4Height);
                }
                image.Write(stream);
                //image.Sample((int)a4Width, (int)a4Height);
                return stream;

            }
        }
        private static int TryOperate(int limter, int pages)
        {
            if (limter - pages < 0)
                return limter;
            else
                return limter - pages;
        }
    }


}
