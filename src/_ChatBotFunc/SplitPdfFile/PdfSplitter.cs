using PdfSharp.Pdf;
using PdfSharp.Pdf.IO;
using System;
using System.IO;

public class PdfSplitter
{
    public static void SplitPdf(string inputFilePath, string outputPath)
    {
        const int maxChunkSize = 5 * 1024 * 1024; // 5MB in bytes

        using (PdfDocument inputDocument = PdfReader.Open(inputFilePath, PdfDocumentOpenMode.Import))
        {
            int chunkIndex = 0;
            PdfDocument outputDocument = new PdfDocument();
            long currentChunkSize = 0;

            for (int pageIndex = 0; pageIndex < inputDocument.PageCount; pageIndex++)
            {
                PdfPage page = inputDocument.Pages[pageIndex];
                outputDocument.AddPage(page);
                currentChunkSize += GetPageSize(page);

                if (currentChunkSize >= maxChunkSize || pageIndex == inputDocument.PageCount - 1)
                {
                    string outputFilePath = Path.Combine(outputPath, $"output_{chunkIndex}.pdf");
                    outputDocument.Save(outputFilePath);
                    outputDocument = new PdfDocument();
                    currentChunkSize = 0;
                    chunkIndex++;
                }
            }
        }
    }

    private static long GetPageSize(PdfPage page)
    {
        using (MemoryStream stream = new MemoryStream())
        {
            page.Owner.Save(stream, false);
            return stream.Length;
        }
    }
}
