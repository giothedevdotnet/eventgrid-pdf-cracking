using ChatBotFunc.ChunkFile;
using iText.Kernel.Pdf;
using iText.Layout;

public class IPDFCreakingBySize : IPDFCreaking
{
    public void SplitPdf(string inputFilePath, string outputPath)
    {
        string fileName = UrlHelper.ExtractFileName(inputFilePath);
        const int maxChunkSize = 5 * 1024 * 1024; // 5MB in bytes

        using (PdfReader reader = new PdfReader(inputFilePath))
        {
            PdfDocument inputDocument = new PdfDocument(reader);
            int chunkIndex = 0;
            long currentChunkSize = 0;
            PdfDocument outputDocument = new PdfDocument(new PdfWriter(Path.Combine(outputPath, $"{chunkIndex}_{fileName}")));

            for (int pageIndex = 1; pageIndex <= inputDocument.GetNumberOfPages(); pageIndex++)
            {
                PdfPage page = inputDocument.GetPage(pageIndex);
                outputDocument.AddPage(page.CopyTo(outputDocument));
                currentChunkSize += GetPageSize(page);

                if (currentChunkSize >= maxChunkSize || pageIndex == inputDocument.GetNumberOfPages())
                {
                    outputDocument.Close();
                    chunkIndex++;
                    currentChunkSize = 0;
                    if (pageIndex < inputDocument.GetNumberOfPages())
                    {
                        outputDocument = new PdfDocument(new PdfWriter(Path.Combine(outputPath, $"{chunkIndex}_{fileName}")));
                    }
                }
            }
        }
    }

    private long GetPageSize(PdfPage page)
    {
        using (MemoryStream stream = new MemoryStream())
        {
            PdfDocument tempDocument = new PdfDocument(new PdfWriter(stream));
            Document myDocument = new Document(tempDocument);
            tempDocument.AddPage(page.CopyTo(tempDocument));
            myDocument.Close();
            byte[] content = stream.ToArray();
            return content.Length;
        }
    }
}
