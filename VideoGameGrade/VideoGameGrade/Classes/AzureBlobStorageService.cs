using Azure.Storage.Blobs;
using Microsoft.Extensions.Configuration;
using System.IO;
using System.Threading.Tasks;

namespace VideoGameGrade.Services
{
    // service class for interacting with Azure Blob Storage
    public class AzureBlobStorageService : IAzureBlobStorageService
    {
        // BlobServiceClient manages interactions with our Azure Blob Storage account
        private readonly BlobServiceClient _blobServiceClient;

        // Constructor that initializes the BlobServiceClient with a connection string
        public AzureBlobStorageService(IConfiguration configuration)
        {
            // Retrieve the connection string from app settings
            var connectionString = configuration.GetConnectionString("AzureBlobStorage");
            // Initialize the BlobServiceClient with the retrieved connection string
            _blobServiceClient = new BlobServiceClient(connectionString);
        }

        // Method to upload a file to the Azure Blob Storage
        public async Task<string> UploadFileAsync(Stream fileStream, string fileName)
        {
            // The name of the container hosted in the Azure Blob Storage
            var containerName = "images";

            // Get a reference to the blob container client using the  container name
            var containerClient = _blobServiceClient.GetBlobContainerClient(containerName);

            // Ensure the container exists, and create it if it does not
            await containerClient.CreateIfNotExistsAsync();

            // Get a reference to a blob client object to manage uploading the file
            var blobClient = containerClient.GetBlobClient(fileName);

            // Upload the file stream to Azure Blob Storage, overwrite if the file already exists
            await blobClient.UploadAsync(fileStream, overwrite: true);

            // Return the absolute URI of the uploaded blob file, which is used to access the file directly.  Does rename the image file
            return blobClient.Uri.AbsoluteUri;
        }
    }
}