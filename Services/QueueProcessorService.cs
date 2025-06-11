using Azure.Storage.Queues;
using Azure.Storage.Queues.Models;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using WebApplication1.Models;
using System.Text.Json;

namespace WebApplication1.Services
{
    public class QueueProcessorService : BackgroundService
    {
        private readonly QueueServiceClient _queueServiceClient;
        private readonly IAzureStorageService _storageService;
        private readonly ILogger<QueueProcessorService> _logger;
        private const string UserActivityQueue = "user-activity";

        public QueueProcessorService(
            QueueServiceClient queueServiceClient,
            IAzureStorageService storageService,
            ILogger<QueueProcessorService> logger)
        {
            _queueServiceClient = queueServiceClient;
            _storageService = storageService;
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            var queueClient = _queueServiceClient.GetQueueClient(UserActivityQueue);

            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    _logger.LogInformation("Checking for new messages in queue...");
                    
                    var messages = await queueClient.ReceiveMessagesAsync(maxMessages: 10);
                    
                    if (messages.Value.Count() > 0)
                    {
                        _logger.LogInformation($"Found {messages.Value.Count()} messages in queue");
                    }
                    
                    foreach (var message in messages.Value)
                    {
                        try
                        {
                            _logger.LogInformation($"Processing message: {message.MessageId}");
                            
                            var messageText = System.Text.Encoding.UTF8.GetString(
                                Convert.FromBase64String(message.MessageText));

                            _logger.LogInformation($"Message content: {messageText}");

                            var activityData = JsonSerializer.Deserialize<ActivityData>(messageText);
                            if (activityData == null)
                            {
                                _logger.LogWarning("Failed to deserialize activity data");
                                continue;
                            }

                            var activity = new UserActivity
                            {
                                PartitionKey = "user123",
                                RowKey = DateTime.UtcNow.Ticks.ToString(),
                                ActivityType = Enum.Parse<ActivityType>(activityData.ActivityType),
                                Description = activityData.Description,
                                Timestamp = DateTime.UtcNow,
                                AdditionalData = activityData.AdditionalData
                            };

                            _logger.LogInformation("Simulating processing time (30 seconds)...");
                            await Task.Delay(TimeSpan.FromSeconds(30), stoppingToken);

                            _logger.LogInformation("Saving activity to table storage...");
                            await _storageService.SaveToTableStorageAsync(
                                partitionKey: activity.PartitionKey,
                                rowKey: activity.RowKey,
                                properties: new Dictionary<string, object>
                                {
                                    { "ActivityType", activity.ActivityType.ToString() },
                                    { "Description", activity.Description },
                                    { "Timestamp", activity.Timestamp },
                                    { "AdditionalData", activity.AdditionalData ?? string.Empty }
                                }
                            );

                            _logger.LogInformation(
                                $"Successfully processed activity: {activity.ActivityType} for user {activity.PartitionKey}");

                            _logger.LogInformation("Deleting message from queue...");
                            await queueClient.DeleteMessageAsync(message.MessageId, message.PopReceipt);
                            _logger.LogInformation("Message deleted successfully");
                        }
                        catch (Exception ex)
                        {
                            _logger.LogError(ex, $"Error processing message: {message.MessageText}");
                        }
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error receiving messages from queue");
                }

                _logger.LogInformation("Waiting 10 seconds before next check...");
                await Task.Delay(TimeSpan.FromSeconds(10), stoppingToken);
            }
        }

        private class ActivityData
        {
            public string ActivityType { get; set; } = string.Empty;
            public string Description { get; set; } = string.Empty;
            public string AdditionalData { get; set; } = string.Empty;
        }
    }
} 