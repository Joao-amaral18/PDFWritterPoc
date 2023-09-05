﻿using iText.Kernel.Geom;
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
        private static string[] srcFiles = { "autorizacao.pdf", "carimbotempo.pdf", "ConjuntoEvid.pdf", "docIdent.png" };

        public static void Main(string[] args)
        {
            PdfDocument pdfDocument = new(new PdfWriter(output));
            PageSize pdfPageFile = new PageSize(1800, 1700);
            pdfDocument.AddNewPage(pdfPageFile);

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
                CopyPdfToFile(
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

        private static void
        CopyPdfToFile(
            float a4Width,
            float totalWidth,
            float totalHeight,
            float a4Height,
            string pdfReader,
            PdfDocument pdfDocument,
            int occupyingArea,
            long fileSize)
        {
            try
            {
                using (PdfReader reader = new PdfReader(pdfReader))
                {
                    using (PdfDocument readerDocument = new PdfDocument(reader))
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

                        InsertIntoCanvas(numberOfPages, readerDocument, canvas, totalWidth, pdfDocument);

                        // Update the file size and empty area
                        fileSize += reader.GetFileLength();

                        // Add the pages to the PDF document
                        return;
                    }
                }
            }
            catch (Exception)
            {
                return;
            }
        }
        private static void InsertIntoCanvas(int numberOfPages, PdfDocument readerDocument, PdfCanvas canvas, float totalWidth, PdfDocument pdfDocument)
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
                    yPosition += a4Height;
                    xPosition = 0;
                }
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
