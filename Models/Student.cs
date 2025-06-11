using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Http;

namespace WebApplication1.Models
{
    public class Student
    {
        public Student()
        {
            Enrollments = new List<Enrollment>();
        }

        public int Id { get; set; }

        [Required]
        [StringLength(50)]
        [Display(Name = "Last Name")]
        public string LastName { get; set; } = string.Empty;

        [Required]
        [StringLength(50)]
        [Display(Name = "First Name")]
        public string FirstName { get; set; } = string.Empty;

        [Required]
        [EmailAddress]
        public string Email { get; set; }

        [Display(Name = "Enrollment Date")]
        [DataType(DataType.Date)]
        public DateTime EnrollmentDate { get; set; }

        [Display(Name = "Profile Image")]
        public string? ImageUrl { get; set; }

        [NotMapped]
        [Display(Name = "Profile Image")]
        public IFormFile? ImageFile { get; set; }

        public ICollection<Enrollment> Enrollments { get; set; } = new List<Enrollment>();

        public string FullName => $"{FirstName} {LastName}";
    }
} 