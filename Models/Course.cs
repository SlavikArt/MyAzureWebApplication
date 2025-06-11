using System.ComponentModel.DataAnnotations;

namespace WebApplication1.Models
{
    public class Course
    {
        public Course()
        {
            Enrollments = new List<Enrollment>();
        }

        public int Id { get; set; }

        [Required]
        [StringLength(100)]
        public string Title { get; set; }

        [Required]
        [StringLength(10)]
        public string CourseCode { get; set; }

        public int Credits { get; set; }

        public ICollection<Enrollment> Enrollments { get; set; }
    }
} 