using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CITS.EduSuite.Business.Models.ViewModels;

namespace CITS.EduSuite.Business.Interfaces
{
    public interface IApplicationWebFormService
    {
        ApplicationWebFormViewModel GetApplicationWebFormById(ApplicationWebFormViewModel model);
        ApplicationWebFormViewModel CreateApplicationWebForm(ApplicationWebFormViewModel model);
        ApplicationWebFormViewModel UpdateApplicationWebForm(ApplicationWebFormViewModel model);
        void GetCourseType(ApplicationWebFormViewModel model);
        void GetCourseByCourseType(ApplicationWebFormViewModel model);
        void GetUniversity(ApplicationWebFormViewModel model);
        ApplicationWebFormViewModel FillCaste(ApplicationWebFormViewModel model);
        ApplicationWebFormViewModel GetEmployeesByBranchId(ApplicationWebFormViewModel model);
        ApplicationWebFormViewModel CheckPhoneExists(string MobileNumber, Int64 RowKey);
        List<ApplicationWebFormViewModel> GetApplicationWebForm(ApplicationWebFormViewModel model, out long TotalRecords);
        void FillBranches(ApplicationWebFormViewModel model);
        ApplicationWebFormViewModel DeleteApplicationWebForm(long Id);
        void FillWebFormStatus(ApplicationWebFormViewModel model);
        void FillWebEnquiryStatus(ApplicationWebFormViewModel model);
    }
}
