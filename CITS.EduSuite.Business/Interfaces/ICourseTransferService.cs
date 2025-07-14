using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CITS.EduSuite.Business.Models.ViewModels;

namespace CITS.EduSuite.Business.Interfaces
{
    public interface ICourseTransferService
    {
        List<dynamic> GetApplications(ApplicationViewModel model, out long TotalRecords);
        CourseTransferViewModel GetCourseTransferById(CourseTransferViewModel model);
        CourseTransferViewModel CreateCourseTransfer(CourseTransferViewModel model);
        CourseTransferViewModel UpdateCourseTransfer(CourseTransferViewModel model);
        CourseTransferViewModel DeleteCourseTransfer(long RowKey);
        void GetCourseType(CourseTransferViewModel model);
        void GetCourseByCourseType(CourseTransferViewModel model);
        void GetUniversity(CourseTransferViewModel model);
        List<CourseTransferViewModel> BindAllCourseTansferDetailsById(CourseTransferViewModel model);
    }
}
