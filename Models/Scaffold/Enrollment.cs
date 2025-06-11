using System;
using System.Collections.Generic;

namespace WebApplication1.Models.Scaffold;

public partial class Enrollment
{
    public int Id { get; set; }

    public int StudentId { get; set; }

    public int CourseId { get; set; }

    public int? Grade { get; set; }

    public DateTime EnrollmentDate { get; set; }

    public virtual Course Course { get; set; } = null!;

    public virtual Student Student { get; set; } = null!;
}
