using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using Azure.Storage.Queues;
using Azure.Data.Tables;
using Azure.Storage.Sas;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using System.Text;
using Microsoft.Extensions.Logging;

namespace WebApplication1.Services
{
    public class AzureStorageService : IAzureStorageService
    {
        private readonly BlobServiceClient _blobServiceClient;
        private readonly QueueServiceClient _queueServiceClient;
        private readonly TableServiceClient _tableServiceClient;
        private readonly string _containerName;
        private readonly string _queueName;
        private readonly string _tableName;
        private readonly ILogger<AzureStorageService> _logger;

        public AzureStorageService(
            IConfiguration configuration,
            ILogger<AzureStorageService> logger)
        {
            var connectionString = configuration.GetConnectionString("AzureStorage");
            _blobServiceClient = new BlobServiceClient(connectionString);
            _queueServiceClient = new QueueServiceClient(connectionString);
            _tableServiceClient = new TableServiceClient(connectionString);
            _containerName = "user-images";
            _queueName = "user-activity";
            _tableName = "UserActivity";
            _logger = logger;

            var containerClient = _blobServiceClient.GetBlobContainerClient(_containerName);
            containerClient.CreateIfNotExists();

            var queueClient = _queueServiceClient.GetQueueClient(_queueName);
            queueClient.CreateIfNotExists();

            var tableClient = _tableServiceClient.GetTableClient(_tableName);
            tableClient.CreateIfNotExists();
        }

        public async Task<string> UploadImageAsync(IFormFile file)
        {
            try
            {
                var containerClient = _blobServiceClient.GetBlobContainerClient(_containerName);
                var blobName = $"{Guid.NewGuid()}{Path.GetExtension(file.FileName)}";
                var blobClient = containerClient.GetBlobClient(blobName);

                using var stream = file.OpenReadStream();
                await blobClient.UploadAsync(stream, true);

                var sasUri = await GetImageUrlAsync(blobName);
                return sasUri;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error uploading image to blob storage");
                throw;
            }
        }

        public async Task<string> GetImageUrlAsync(string blobName)
        {
            try
            {
                var containerClient = _blobServiceClient.GetBlobContainerClient(_containerName);
                var blobClient = containerClient.GetBlobClient(blobName);

                if (!await blobClient.ExistsAsync())
                {
                    _logger.LogWarning("Blob {BlobName} not found in container {ContainerName}", blobName, _containerName);
                    return string.Empty;
                }

                var sasBuilder = new BlobSasBuilder
                {
                    BlobContainerName = _containerName,
                    BlobName = blobName,
                    Resource = "b",
                    StartsOn = DateTimeOffset.UtcNow.AddMinutes(-5),
                    ExpiresOn = DateTimeOffset.UtcNow.AddDays(365)
                };

                sasBuilder.SetPermissions(BlobSasPermissions.Read);

                var sasUri = blobClient.GenerateSasUri(sasBuilder);

                _logger.LogInformation("Generated SAS token for blob {BlobName} with expiration {Expiration}", 
                    blobName, sasBuilder.ExpiresOn);
                return sasUri.ToString();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error generating SAS token for blob {BlobName}", blobName);
                return string.Empty;
            }
        }

        public async Task SendMessageToQueueAsync(string message)
        {
            var queueClient = _queueServiceClient.GetQueueClient(_queueName);
            await queueClient.SendMessageAsync(Convert.ToBase64String(System.Text.Encoding.UTF8.GetBytes(message)));
        }

        public async Task SaveToTableStorageAsync(string partitionKey, string rowKey, Dictionary<string, object> properties)
        {
            try
            {
                var tableClient = _tableServiceClient.GetTableClient(_tableName);
                var entity = new TableEntity(partitionKey, rowKey);

                foreach (var prop in properties)
                {
                    entity[prop.Key] = prop.Value;
                }

                await tableClient.UpsertEntityAsync(entity);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error saving to table storage");
                throw;
            }
        }

        public async Task AddMessageToQueueAsync(string message)
        {
            try
            {
                var queueClient = _queueServiceClient.GetQueueClient(_queueName);
                var messageBytes = Encoding.UTF8.GetBytes(message);
                var base64Message = Convert.ToBase64String(messageBytes);
                await queueClient.SendMessageAsync(base64Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error adding message to queue");
                throw;
            }
        }
    }
} 