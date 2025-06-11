# My Azure Web Application

A modern ASP.NET Core web application for managing students, courses, and enrollments with Azure integration.

## 🔗 Quick Links
- [📸 View Screenshots](#-screenshots)
- [📁 Project Structure](#-project-structure)
- [⚙️ Configuration](#️-configuration)
- [🚀 Getting Started](#-getting-started)

## 🚀 Features

- Student management with image upload support
- Course management and enrollment system
- Real-time activity logging
- Azure Storage integration for file management
- Translation service with multiple language support
- Queue-based message processing
- User activity tracking and monitoring

## 📁 Project Structure

- [Controllers/](./Controllers/)
  - [ActivityController.cs](./Controllers/ActivityController.cs)
  - [CoursesController.cs](./Controllers/CoursesController.cs)
  - [EnrollmentsController.cs](./Controllers/EnrollmentsController.cs)
  - [StudentsController.cs](./Controllers/StudentsController.cs)
  - [StorageController.cs](./Controllers/StorageController.cs)
- [Models/](./Models/)
  - [Course.cs](./Models/Course.cs)
  - [Enrollment.cs](./Models/Enrollment.cs)
  - [ErrorViewModel.cs](./Models/ErrorViewModel.cs)
  - [Product.cs](./Models/Product.cs)
  - [Student.cs](./Models/Student.cs)
  - [TranslationHistory.cs](./Models/TranslationHistory.cs)
  - [UserActivity.cs](./Models/UserActivity.cs)
- [Services/](./Services/)
  - [AzureStorageService.cs](./Services/AzureStorageService.cs)
  - [QueueProcessorService.cs](./Services/QueueProcessorService.cs)
  - [TranslationService.cs](./Services/TranslationService.cs)
- [Data/](./Data/)
  - [ApplicationDbContext.cs](./Data/ApplicationDbContext.cs)
- [Views/](./Views/)
- [wwwroot/](./wwwroot/)
- [Migrations/](./Migrations/)
- [Properties/](./Properties/)
- [Program.cs](./Program.cs)
- [appsettings.json](./appsettings.json)

## 🛠️ Prerequisites

- .NET 8.0 SDK
- SQL Server
- Azure Account (for storage services)
- Visual Studio 2022 or VS Code

## ⚙️ Configuration

1. Clone the repository and navigate to the project directory
```bash
git clone https://github.com/yourusername/WebApplication1.git
cd WebApplication1
```

2. Create your configuration files:
   - Copy `appsettings.Example.json` to `appsettings.Development.json`
   - Update the connection strings and keys in `appsettings.Development.json` with your values:
```json
{
  "ConnectionStrings": {
    "DefaultConnection": "your_sql_connection_string",
    "AzureStorage": "your_azure_storage_connection_string",
    "APPLICATIONINSIGHTS_CONNECTION_STRING": "your_app_insights_connection_string"
  },
  "AzureTranslator": {
    "Key": "your_translator_key",
    "Endpoint": "https://api.cognitive.microsofttranslator.com",
    "Location": "westeurope"
  }
}
```

3. Required Azure Services:
   - Azure Storage Account (for blob storage and queues)
   - Azure Translator Service
   - Application Insights (optional, for monitoring)

## 🚀 Getting Started

1. Restore dependencies
```bash
dotnet restore
```

2. Apply database migrations
```bash
dotnet ef database update
```

3. Run the application
```bash
dotnet run
```

## 📸 Screenshots

### Azure Portal & Setup
![Azure Portal Home](./screenshots/01_PortalAzureHome.png)
*Azure Portal Home Page*

![My Test Web App Azure](./screenshots/02_MyTestWebApp_Azure.png)
*Web App Configuration in Azure*

![My Test Web App Home](./screenshots/03_MyTestWebApp_Home.png)
*Web App Home Page*

![My Test Web App Products](./screenshots/04_MyTestWebApp_Products.png)
*Products Page*

![Visual Studio Publish Succeeded](./screenshots/05_VisualStudio_Publish_Succeeded.png)
*Successful Deployment*

![My Test Web App Localhost](./screenshots/06_MyTestWebApp_Localhost_Working.png)
*Local Development Environment*

### Azure Services
![My Test Server App Azure](./screenshots/07_MyTestServerApp_Azure.png)
*Server App Configuration*

![Test DB Azure](./screenshots/08_TestDB_Azure.png)
*Database Configuration*

![Test DB Products Table](./screenshots/09_TestDB_Products_Table.png)
*Products Table Structure*

![Test DB Products Working](./screenshots/10_TestDB_Products_Working.png)
*Products Table Data*

### Monitoring & Logging
![Diagnostic Settings](./screenshots/11_DiagnosticSettings.png)
*Diagnostic Settings Configuration*

![My Test Log Analytics Logs](./screenshots/12_MyTestLogAnalytics_Logs.png)
*Log Analytics Dashboard*

![SSMS Connect](./screenshots/13_SSMS_Connect.png)
*SQL Server Management Studio Connection*

![SSMS Connected](./screenshots/14_SSMS_Connected.png)
*Connected to Database*

![My Test Log Analytics App Traces](./screenshots/15_MyTestLogAnalytics_AppTraces_Ececuted_DB_Command.png)
*Application Traces*

### Application UI
![App UI Update 1](./screenshots/16_1_App_UI_Update.png)
*Updated Application UI*

![App UI Update 2](./screenshots/16_2_App_UI_Update.png)
*UI Components*

![Products](./screenshots/16_3_Products.png)
*Products Management*

![Students](./screenshots/16_3_Students.png)
*Students List*

![Student Create](./screenshots/16_4_Student_Create.png)
*Create New Student*

![Student Details](./screenshots/16_5_Student_Details.png)
*Student Information*

![Students High Achievers](./screenshots/16_6_Students_High_Achievers.png)
*High Achieving Students*

![Courses](./screenshots/16_7_Courses.png)
*Courses List*

![Course Create](./screenshots/16_8_Course_Create.png)
*Create New Course*

![Courses Statistics](./screenshots/16_9_Courses_Statistics.png)
*Course Statistics*

![Course Details](./screenshots/16_10_Course_Details.png)
*Course Information*

![Enrollments](./screenshots/16_11_Enrollments.png)
*Enrollments List*

![Enrollment Create](./screenshots/16_12_Enrollment_Create.png)
*Create New Enrollment*

![Enrollment Edit](./screenshots/16_13_Enrollment_Edit.png)
*Edit Enrollment*

### Azure Storage
![Queues Azure Storage](./screenshots/17_Queues_Azure_Storage.png)
*Queue Storage Configuration*

![Tables Azure Storage](./screenshots/18_Tables_Azure_Storage.png)
*Table Storage Configuration*

![Blob Azure Storage](./screenshots/19_Blob_Azure_Storage.png)
*Blob Storage Configuration*

### Student Images
![Student Images](./screenshots/20_Student_Images.png)
*Student Image Gallery*

![Student Image Upload](./screenshots/21_Student_ImageUpload.png)
*Image Upload Interface*

![Student Image Upload Working](./screenshots/22_Student_ImageUpload_Working.png)
*Successful Image Upload*

### Activity & Translation
![Activity Logs Table](./screenshots/23_Activity_Logs_Table.png)
*Activity Logging Table*

![Translation](./screenshots/24_Translation.png)
*Translation Service*

![Translation Text](./screenshots/25_Translation_Text.png)
*Text Translation Interface*

![Translation History](./screenshots/26_Translation_History.png)
*Translation History Log*
