using Azure.Messaging;
using Azure.Storage.Blobs;
using Microsoft.Azure.EventGrid.Models;
using System.Text.RegularExpressions;

namespace ChatBotApi.Creaking;

public class BlobFileCreatedEventHandler
{
    private readonly ILogger<BlobFileCreatedEventHandler> _logger;
    private readonly IConfiguration _configuration;
    private readonly IPDFCreaking _pdfSplitter;
    private const string _upstreamContainerName = "data";
    private const string _downstreamContainerName = "data-creaked";

    public BlobFileCreatedEventHandler(ILogger<BlobFileCreatedEventHandler> logger, IConfiguration configuration, IPDFCreaking pdfSplitter)
    {
        _logger = logger;
        _configuration = configuration;
        _pdfSplitter = pdfSplitter;
    }

    public async Task Handle(CloudEvent cloudEvent)
    {
        switch (cloudEvent.Type)
        {
            case "Microsoft.Storage.BlobCreated":
                await ProcessBlobCreated(cloudEvent);
                break;
            default:
                _logger.LogWarning("CloudEvent type is not valid.");
                break;
        }
    }

    private async Task ProcessBlobCreated(CloudEvent cloudEvent)
    {
        // By default, ToObjectFromJson<T> uses System.Text.Json to deserialize the payload
        var blobEventData = cloudEvent.Data.ToObjectFromJson<StorageBlobCreatedEventData>();

        // Download the file from Azure Storage
        string fileName = UrlHelper.ExtractFileName(blobEventData.Url);
        string destinationPath = Path.Combine(Path.GetTempPath(), fileName);
        string upstreamBlobConnectionString = _configuration["UPSTREAM_AZURE_STORAGE_CONNECTION_STRING"];
        await DownloadFileFromAzureStorage(upstreamBlobConnectionString, blobEventData.Url, _upstreamContainerName, destinationPath);

        // Important: Some PDF creates only one page for the whole document. Here is your chance to print that out using Microsoft PDF driver or wharever you want.
        // I manually printed out large documents to make it work. ;)
        // create a temporary directory to store the file
        string chunckDirectory = Path.Combine(Path.GetTempPath(), Path.GetRandomFileName());
        Directory.CreateDirectory(chunckDirectory);

        // chunk the PDF file
        _pdfSplitter.SplitPdf(destinationPath, chunckDirectory);

        // Upload diretory to storage account container
        string downstreamBlobConnectionString = _configuration["DOWNSTREAM_AZURE_STORAGE_CONNECTION_STRING"];
        string destinationFolderName = CleanString(fileName);
        await UploadDirectoryToAzureStorage(downstreamBlobConnectionString, chunckDirectory, _downstreamContainerName, destinationFolderName, fileName);

        // clean up the temporary directory
        Directory.Delete(chunckDirectory, true);

        // Delete the original file from the upstream container
        System.IO.File.Delete(destinationPath);

        _logger.LogInformation("Creaking completed.");
    }

    private async Task UploadDirectoryToAzureStorage(string? downstreamBlobConnectionString, string sourceDirectory, string destinationContainerName, string destinationFolderName, string documentName)
    {
        // Create a BlobServiceClient object
        var blobServiceClient = new BlobServiceClient(downstreamBlobConnectionString);
        var blobContainerClient = blobServiceClient.GetBlobContainerClient(destinationContainerName);

        // Ensure the container exists
        await blobContainerClient.CreateIfNotExistsAsync();

        // Get all files in the source directory
        var files = Directory.GetFiles(sourceDirectory);

        foreach (var filePath in files)
        {
            var fileName = Path.GetFileName(filePath);
            var blobClient = blobContainerClient.GetBlobClient($"{destinationFolderName}/{fileName}");

            // Upload the file
            await blobClient.UploadAsync(filePath, overwrite: true);

            // Add metadata
            var metadata = new Dictionary<string, string>
                {
                    { "DocName", documentName }
                };
            await blobClient.SetMetadataAsync(metadata);
        }
    }

    private async Task DownloadFileFromAzureStorage(string connectionString, string fileUrl, string containerName, string downloadPath)
    {
        // Create a BlobServiceClient object 
        var blobServiceClient = new BlobServiceClient(connectionString);
        var blobContainerClient = blobServiceClient.GetBlobContainerClient(containerName);
        var blobClient = blobContainerClient.GetBlobClient(Path.GetFileName(fileUrl));

        await blobClient.DownloadToAsync(downloadPath);
    }

    private static string CleanString(string input)
    {
        // Remove spaces and special characters
        string cleanedString = Regex.Replace(input, @"[^a-zA-Z0-9]", "");
        // Convert to lowercase
        return cleanedString.ToLower().Replace(".pdf", string.Empty);
    }
}