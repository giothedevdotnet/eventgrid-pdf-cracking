using ChatBotFunc.ChunkFile;
using iText.Kernel.Pdf;

public class IPDFCreakingByPage : IPDFCreaking
{
    public void SplitPdf(string inputFilePath, string outputPath)
    {
        string fileName = UrlHelper.ExtractFileName(inputFilePath);

        using (PdfReader reader = new PdfReader(inputFilePath))
        {
            PdfDocument inputDocument = new PdfDocument(reader);
            int chunkIndex = 0;
            PdfDocument outputDocument = new PdfDocument(new PdfWriter(Path.Combine(outputPath, $"{chunkIndex}_{fileName}")));

            for (int pageIndex = 1; pageIndex <= inputDocument.GetNumberOfPages(); pageIndex++)
            {
                PdfPage page = inputDocument.GetPage(pageIndex);
                outputDocument.AddPage(page.CopyTo(outputDocument));
                outputDocument.Close();
                chunkIndex++;
                outputDocument = new PdfDocument(new PdfWriter(Path.Combine(outputPath, $"{chunkIndex}_{fileName}")));
            }
        }
    }
}
