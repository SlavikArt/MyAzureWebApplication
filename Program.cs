using Microsoft.EntityFrameworkCore;
using WebApplication1.Data;
using WebApplication1.Services;
using Azure.Storage.Blobs;
using Azure.Storage.Queues;
using Azure.Data.Tables;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllersWithViews();

builder.Services.AddSingleton(x => new BlobServiceClient(builder.Configuration.GetConnectionString("AzureStorage")));
builder.Services.AddSingleton(x => new QueueServiceClient(builder.Configuration.GetConnectionString("AzureStorage")));
builder.Services.AddSingleton(x => new TableServiceClient(builder.Configuration.GetConnectionString("AzureStorage")));
builder.Services.AddSingleton<IAzureStorageService, AzureStorageService>();

builder.Services.AddHostedService<QueueProcessorService>();
builder.Services.AddScoped<ITranslationService, TranslationService>();

builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"),
        sqlServerOptionsAction: sqlOptions =>
        {
            sqlOptions.EnableRetryOnFailure(
                maxRetryCount: 5,
                maxRetryDelay: TimeSpan.FromSeconds(30),
                errorNumbersToAdd: null);
        }));

builder.Logging.AddApplicationInsights(
    configureTelemetryConfiguration: (config) => config.ConnectionString = builder.Configuration.GetConnectionString("APPLICATIONINSIGHTS_CONNECTION_STRING"),
    configureApplicationInsightsLoggerOptions: (options) => { }
);

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
