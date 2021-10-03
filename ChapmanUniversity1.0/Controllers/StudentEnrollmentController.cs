using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata.Ecma335;
using System.Threading.Tasks;
using ChapmanUniversity1._0.Data;
using ChapmanUniversity1._0.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace ChapmanUniversity1._0.Controllers
{
    public class StudentEnrollmentController : Controller
    {
        private readonly SchoolContext _context;

        public StudentEnrollmentController(SchoolContext context)
        {
            _context = context;
        }

        // GET: StudentCourseEnrollments
        public ActionResult Index()
        {
            TempData.Remove("CourseEnrollmentSuccessAlert");
            TempData.Remove("CourseAlreadyRegisteredAlert");
            TempData.Remove("CourseRemovedSuccessfullyAlert");

            if (TempData["StudentId"] == null)
            {
                return NotFound();
            }

            var id = (int) TempData["StudentId"];
            TempData.Keep("StudentId");

            var student = _context.Students.Find(id);

            
            var studentEnrollments = _context.StudentCourseEnrollments.Include(d => d.Student).Include(d => d.Semester).Include(d => d.Course).ToList();
            List<StudentEnrollmentViewModel> enrollmentList = new List<StudentEnrollmentViewModel>();

            if(studentEnrollments.Count == 0)
            {
                student.IsStudentActive = "N";
                _context.Students.Update(student);
                _context.SaveChanges();
            }

            foreach (var t in studentEnrollments)
            {
                if (id == t.StudentId)
                {
                    StudentEnrollmentViewModel newStudentEnrollmentView = new StudentEnrollmentViewModel()
                    {
                        CourseName = t.Course.CourseName,
                        CourseNumber = t.Course.CourseNumber,
                        SemesterSeason = t.Semester.Season,
                        EnrollmentId = t.Id
                        
                    };
                    enrollmentList.Add(newStudentEnrollmentView);
                }
            }
            return View(enrollmentList);
        }

        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var enrollment = await _context.StudentCourseEnrollments
                .Include(e => e.Course).Include(e => e.Semester)
                .FirstOrDefaultAsync(m => m.Id == id);

            StudentEnrollmentViewModel newStudentEnrollmentView = new StudentEnrollmentViewModel()

            {
                CourseName = enrollment.Course.CourseName,
                CourseNumber = enrollment.Course.CourseNumber,
                SemesterSeason = enrollment.Semester.Season,
                EnrollmentId = enrollment.Id
            };

            return View(newStudentEnrollmentView);
        }

        public ActionResult Create()
        {
            ViewData["CourseId"] = new SelectList(_context.Courses, "Id", "CourseName");
            ViewData["SemesterId"] = new SelectList(_context.Semesters, "Id", "Season");

            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(StudentEnrollment studentEnrollment)
        {
            if (TempData["StudentId"] == null)
            {
                return NotFound();
            }

            var id = (int) TempData["StudentId"];
            TempData.Keep("StudentId");
            StudentEnrollment newStudentEnrollment = new StudentEnrollment
            {
                StudentId = id,
                CourseId = studentEnrollment.CourseId,
                SemesterId = studentEnrollment.SemesterId
            };

            var enrollmentExists =
                EnrollmentExists(newStudentEnrollment.CourseId, newStudentEnrollment.StudentId, newStudentEnrollment.SemesterId);

            var studentEnrollments = _context.StudentCourseEnrollments.Include(d => d.Student).Include(d => d.Course).ToList();

            var student = _context.Students.Find(id);
            
            if (studentEnrollments.Count == 0 || !enrollmentExists)
            {
                _context.Add(newStudentEnrollment);
                student.IsStudentActive = "Y";
                _context.SaveChanges();
                TempData.Add("CourseEnrollmentSuccessAlert", null);
                return RedirectToAction("Create", "StudentEnrollment");
            }



                return RedirectToAction("Create", "StudentEnrollment");
        }

        private bool EnrollmentExists(int courseId, int studentId,int semesterId)
        {
            var enrollmentList = _context.StudentCourseEnrollments.ToList();
            foreach (var enrollment in enrollmentList)
            {
                TempData.Remove("CourseAlreadyRegisteredAlert");
                if (enrollment.CourseId == courseId && enrollment.StudentId == studentId &&
                    enrollment.SemesterId == semesterId)
                {
                    TempData.Remove("CourseEnrollmentSuccessAlert");
                    TempData.Add("CourseAlreadyRegisteredAlert", null);
                    return true;
                }
            }
            TempData.Remove("CourseEnrollmentSuccessAlert");
            TempData.Remove("CourseAlreadyRegisteredAlert");
            return false;
        }

        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var enrollment = await _context.StudentCourseEnrollments
                .Include(e => e.Course)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (enrollment == null)
            {
                return NotFound();
            }
            return View(enrollment);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var enrollment = await _context.StudentCourseEnrollments.FindAsync(id);
            _context.StudentCourseEnrollments.Remove(enrollment);
            await _context.SaveChangesAsync();
            TempData.Add("CourseRemovedSuccessfullyAlert",null);
            return RedirectToAction(nameof(Index));
        }
    }
}
