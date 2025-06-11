using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using WebApplication1.Services;
using Azure.Data.Tables;
using System.Text.Json;

namespace WebApplication1.Controllers
{
    public class ActivityController : Controller
    {
        private readonly TableServiceClient _tableServiceClient;
        private readonly ILogger<ActivityController> _logger;
        private const string UserActivityTable = "UserActivity";

        public ActivityController(
            TableServiceClient tableServiceClient,
            ILogger<ActivityController> logger)
        {
            _tableServiceClient = tableServiceClient;
            _logger = logger;
        }

        public async Task<IActionResult> Index()
        {
            try
            {
                var tableClient = _tableServiceClient.GetTableClient(UserActivityTable);
                var activities = new List<dynamic>();

                var query = tableClient.QueryAsync<TableEntity>();
                await foreach (var entity in query)
                {
                    activities.Add(new
                    {
                        UserId = entity.PartitionKey,
                        Timestamp = entity.GetDateTime("Timestamp"),
                        ActivityType = entity.GetString("ActivityType"),
                        Description = entity.GetString("Description"),
                        AdditionalData = entity.GetString("AdditionalData")
                    });
                }

                activities = activities.OrderByDescending(a => a.Timestamp).ToList();

                return View(activities);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving activity logs");
                return StatusCode(500, "An error occurred while retrieving activity logs.");
            }
        }
    }
} 