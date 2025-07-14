using CITS.EduSuite.Business.Models.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CITS.EduSuite.Business.Interfaces
{
    public interface ICourseService
    {
        CourseViewModel GetCourseById(int? id);
        CourseViewModel CreateCourse(CourseViewModel model);
        CourseViewModel UpdateCourse(CourseViewModel model);
        CourseViewModel DeleteCourse(CourseViewModel model);
        List<CourseViewModel> GetCourse(CourseViewModel model, out long TotalRecords);
        CourseViewModel CheckCourseCodeExists(CourseViewModel model);
        CourseViewModel CheckCourseNameExists(CourseViewModel model);
    }
}
