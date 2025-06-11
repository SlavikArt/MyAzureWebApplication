using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using WebApplication1.Data;
using WebApplication1.Models;
using Microsoft.Extensions.Logging;

namespace WebApplication1.Controllers
{
    public class EnrollmentsController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<EnrollmentsController> _logger;

        public EnrollmentsController(ApplicationDbContext context, ILogger<EnrollmentsController> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<IActionResult> Index()
        {
            var enrollments = await _context.Enrollments
                .Include(e => e.Student)
                .Include(e => e.Course)
                .ToListAsync();
            return View(enrollments);
        }

        public IActionResult Create()
        {
            ViewData["StudentId"] = new SelectList(_context.Students, "Id", "FullName");
            ViewData["CourseId"] = new SelectList(_context.Courses, "Id", "Title");
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("StudentId,CourseId,Grade")] Enrollment enrollment)
        {
            if (ModelState.IsValid)
            {
                enrollment.EnrollmentDate = DateTime.Now;
                _context.Add(enrollment);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["StudentId"] = new SelectList(_context.Students, "Id", "FullName", enrollment.StudentId);
            ViewData["CourseId"] = new SelectList(_context.Courses, "Id", "Title", enrollment.CourseId);
            return View(enrollment);
        }

        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var enrollment = await _context.Enrollments
                .Include(e => e.Student)
                .Include(e => e.Course)
                .FirstOrDefaultAsync(e => e.Id == id);

            if (enrollment == null)
            {
                return NotFound();
            }

            if (enrollment.Student == null)
            {
                enrollment.Student = await _context.Students.FindAsync(enrollment.StudentId);
            }
            if (enrollment.Course == null)
            {
                enrollment.Course = await _context.Courses.FindAsync(enrollment.CourseId);
            }

            return View(enrollment);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,StudentId,CourseId,Grade,EnrollmentDate")] Enrollment enrollment)
        {
            _logger.LogInformation($"Edit POST called for enrollment {id} with grade {enrollment.Grade}");

            if (id != enrollment.Id)
            {
                _logger.LogWarning($"ID mismatch: {id} != {enrollment.Id}");
                return NotFound();
            }

            ModelState.Remove("Student");
            ModelState.Remove("Course");
            ModelState.Remove("Grade");

            if (ModelState.IsValid)
            {
                try
                {
                    var existingEnrollment = await _context.Enrollments.FindAsync(id);
                    if (existingEnrollment == null)
                    {
                        _logger.LogWarning($"Enrollment {id} not found");
                        return NotFound();
                    }

                    _logger.LogInformation($"Updating grade from {existingEnrollment.Grade} to {enrollment.Grade}");
                    
                    existingEnrollment.Grade = enrollment.Grade;
                    await _context.SaveChangesAsync();
                    _logger.LogInformation("Grade updated successfully");
                    
                    return RedirectToAction(nameof(Index));
                }
                catch (DbUpdateConcurrencyException ex)
                {
                    _logger.LogError(ex, "Concurrency error while updating enrollment");
                    if (!EnrollmentExists(enrollment.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error updating enrollment");
                    throw;
                }
            }
            else
            {
                _logger.LogWarning("ModelState is invalid: {errors}", 
                    string.Join(", ", ModelState.Values
                        .SelectMany(v => v.Errors)
                        .Select(e => e.ErrorMessage)));
            }

            var enrollmentWithData = await _context.Enrollments
                .Include(e => e.Student)
                .Include(e => e.Course)
                .FirstOrDefaultAsync(e => e.Id == id);
            
            if (enrollmentWithData == null)
            {
                return NotFound();
            }

            return View(enrollmentWithData);
        }

        private bool EnrollmentExists(int id)
        {
            return _context.Enrollments.Any(e => e.Id == id);
        }
    }
} 