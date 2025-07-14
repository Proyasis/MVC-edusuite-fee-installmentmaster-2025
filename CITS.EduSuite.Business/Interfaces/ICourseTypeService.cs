using CITS.EduSuite.Business.Models.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CITS.EduSuite.Business.Interfaces
{
    public interface ICourseTypeService
    {
        CourseTypeViewModel GetCourseTypeById(int? id);
        CourseTypeViewModel CreateCourseType(CourseTypeViewModel model);
        CourseTypeViewModel UpdateCourseType(CourseTypeViewModel model);
        CourseTypeViewModel DeleteCourseType(CourseTypeViewModel model);
        List<CourseTypeViewModel> GetCourseType(string searchText);
    }
}
