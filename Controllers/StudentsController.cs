using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebApplication1.Data;
using WebApplication1.Models;
using WebApplication1.Services;
using Microsoft.Extensions.Logging;
using System.Text.Json;
using System.Text;

namespace WebApplication1.Controllers
{
    public class StudentsController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IAzureStorageService _storageService;
        private readonly ILogger<StudentsController> _logger;

        public StudentsController(
            ApplicationDbContext context,
            IAzureStorageService storageService,
            ILogger<StudentsController> logger)
        {
            _context = context;
            _storageService = storageService;
            _logger = logger;
        }

        public async Task<IActionResult> Index()
        {
            var students = await _context.Students.ToListAsync();
            
            await LogActivity(ActivityType.ListStudents, "Retrieved list of all students");
            
            return View(students);
        }

        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var student = await _context.Students
                .FirstOrDefaultAsync(m => m.Id == id);
            if (student == null)
            {
                return NotFound();
            }

            await LogActivity(ActivityType.ViewStudent, $"Viewed details for student {student.FirstName} {student.LastName}");

            return View(student);
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,FirstName,LastName,EnrollmentDate,ImageFile")] Student student)
        {
            if (ModelState.IsValid)
            {
                if (student.ImageFile != null)
                {
                    var imageUrl = await _storageService.UploadImageAsync(student.ImageFile);
                    student.ImageUrl = imageUrl;
                }

                _context.Add(student);
                await _context.SaveChangesAsync();

                await LogActivity(ActivityType.CreateStudent, 
                    $"Created new student: {student.FirstName} {student.LastName}",
                    JsonSerializer.Serialize(new { StudentId = student.Id }));

                return RedirectToAction(nameof(Index));
            }
            return View(student);
        }

        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var student = await _context.Students.FindAsync(id);
            if (student == null)
            {
                return NotFound();
            }

            await LogActivity(ActivityType.ViewStudent, $"Accessed edit page for student {student.FirstName} {student.LastName}");

            return View(student);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,FirstName,LastName,Email,EnrollmentDate,ImageFile")] Student student)
        {
            if (id != student.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    var existingStudent = await _context.Students.FindAsync(id);
                    if (existingStudent == null)
                    {
                        return NotFound();
                    }

                    if (student.ImageFile != null)
                    {
                        var imageUrl = await _storageService.UploadImageAsync(student.ImageFile);
                        student.ImageUrl = imageUrl;
                    }
                    else
                    {
                        student.ImageUrl = existingStudent.ImageUrl;
                    }

                    _context.Entry(existingStudent).CurrentValues.SetValues(student);
                    await _context.SaveChangesAsync();

                    await LogActivity(ActivityType.EditStudent, 
                        $"Updated student: {student.FirstName} {student.LastName}",
                        JsonSerializer.Serialize(new { StudentId = student.Id }));
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!StudentExists(student.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(student);
        }

        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var student = await _context.Students
                .FirstOrDefaultAsync(m => m.Id == id);
            if (student == null)
            {
                return NotFound();
            }

            await LogActivity(ActivityType.ViewStudent, $"Accessed delete page for student {student.FirstName} {student.LastName}");

            return View(student);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var student = await _context.Students.FindAsync(id);
            if (student != null)
            {
                _context.Students.Remove(student);
                await _context.SaveChangesAsync();

                await LogActivity(ActivityType.DeleteStudent, 
                    $"Deleted student: {student.FirstName} {student.LastName}",
                    JsonSerializer.Serialize(new { StudentId = student.Id }));
            }
            
            return RedirectToAction(nameof(Index));
        }

        private bool StudentExists(int id)
        {
            return _context.Students.Any(e => e.Id == id);
        }

        public async Task<IActionResult> HighAchievers()
        {
            var highAchievers = await _context.Students
                .Include(s => s.Enrollments)
                    .ThenInclude(e => e.Course)
                .Where(s => s.Enrollments.Any(e => e.Grade > 90))
                .ToListAsync();

            return View(highAchievers);
        }

        [HttpGet]
        public async Task<IActionResult> FixStudentImageUrls()
        {
            var students = await _context.Students.ToListAsync();
            _logger.LogInformation("Found {Count} students in the database", students.Count);
            
            var result = new StringBuilder();
            result.AppendLine($"Found {students.Count} students in the database:");
            result.AppendLine();
            
            int updated = 0;
            int failed = 0;
            
            foreach (var student in students)
            {
                try
                {
                    result.AppendLine($"Processing student {student.Id}: {student.FirstName} {student.LastName}");
                    result.AppendLine($"Old ImageUrl: {(string.IsNullOrEmpty(student.ImageUrl) ? "No image" : student.ImageUrl)}");
                    
                    if (string.IsNullOrEmpty(student.ImageUrl))
                    {
                        result.AppendLine("No image URL to update");
                        result.AppendLine();
                        continue;
                    }

                    string blobName;
                    if (student.ImageUrl.Contains("/user-images/"))
                    {
                        blobName = student.ImageUrl.Split("/user-images/")[1].Split("?")[0];
                    }
                    else
                    {
                        blobName = student.ImageUrl;
                    }

                    result.AppendLine($"Extracted blob name: {blobName}");
                    
                    var fullUrl = await _storageService.GetImageUrlAsync(blobName);
                    if (!string.IsNullOrEmpty(fullUrl))
                    {
                        student.ImageUrl = fullUrl;
                        updated++;
                        result.AppendLine($"Updated to: {fullUrl}");
                    }
                    else
                    {
                        failed++;
                        result.AppendLine("Failed to get new URL");
                    }
                    result.AppendLine();
                }
                catch (Exception ex)
                {
                    failed++;
                    result.AppendLine($"Error: {ex.Message}");
                    result.AppendLine();
                }
            }
            
            await _context.SaveChangesAsync();
            result.AppendLine($"Summary: Updated {updated} URLs, Failed: {failed}");
            return Content(result.ToString());
        }

        private async Task LogActivity(ActivityType activityType, string description, string additionalData = "")
        {
            try
            {
                var message = JsonSerializer.Serialize(new
                {
                    ActivityType = activityType.ToString(),
                    Description = description,
                    AdditionalData = additionalData
                });

                await _storageService.AddMessageToQueueAsync(message);
                _logger.LogInformation($"Activity logged: {activityType} - {description}");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error logging activity");
            }
        }
    }
} 