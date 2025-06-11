using Microsoft.AspNetCore.Http;

namespace WebApplication1.Services
{
    public interface IAzureStorageService
    {
        Task<string> UploadImageAsync(IFormFile file);
        Task<string> GetImageUrlAsync(string blobName);
        Task SaveToTableStorageAsync(string partitionKey, string rowKey, Dictionary<string, object> properties);
        Task AddMessageToQueueAsync(string message);
    }
} 