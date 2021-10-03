using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace ChapmanUniversity1._0.Models
{
    public class StudentEnrollmentViewModel
    {

        [Display(Name = "Course Number")]
        public int CourseNumber { get; set; }

        [Display(Name = "Course Name")]
        public string CourseName { get; set; }

        [Display(Name = "Season")]
        public Season SemesterSeason { get; set; }

        public int CourseId { get; set; }

        public int SemesterId { get; set; }

        public int EnrollmentId { get; set; }

    

        public Semester Semester { get; set; }
    }
}
