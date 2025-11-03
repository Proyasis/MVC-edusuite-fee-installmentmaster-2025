using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CITS.EduSuite.Business.Models.ViewModels;
using static CITS.EduSuite.Business.Services.ApplicationPersonalService;

namespace CITS.EduSuite.Business.Interfaces
{
    public interface IApplicationPersonalService
    {
        ApplicationPersonalViewModel GetApplicationPersonalById(ApplicationPersonalViewModel model);
        ApplicationPersonalViewModel CreateApplicationPersonal(ApplicationPersonalViewModel model);
        ApplicationPersonalViewModel FillBatches(ApplicationPersonalViewModel model);
        ApplicationPersonalViewModel FillAcademicTerm(ApplicationPersonalViewModel model);
        CourseFeeDurationViewModel GetCourseFeeAndDuration(int academicTermKey, long courseKey, long universityKey);



        bool SaveInstallmentEntry(InsatallmentViewModel model);

        ApplicationPersonalViewModel GetCourseType1(ApplicationPersonalViewModel model);
        ApplicationPersonalViewModel GetCourseByCourseType1(ApplicationPersonalViewModel model);
        ApplicationPersonalViewModel GetUniversity1(ApplicationPersonalViewModel model);
        

        ApplicationPersonalViewModel UpdateApplicationPersonal(ApplicationPersonalViewModel model);
        void GetCourseType(ApplicationPersonalViewModel model);
        void GetCourseByCourseType(ApplicationPersonalViewModel model);
        void GetUniversity(ApplicationPersonalViewModel model);
        ApplicationPersonalViewModel CheckPhoneExists(string MobileNumber, Int64 RowKey);
        ApplicationPersonalViewModel CheckEmailExists(string EmailAddress, Int64 RowKey);
        ApplicationPersonalViewModel GetAdmissionFeesByCourse(ApplicationPersonalViewModel model);
        void GetYearByMode(ApplicationPersonalViewModel model);
        ApplicationPersonalViewModel GetOfferDetails(ApplicationPersonalViewModel model);
        ApplicationPersonalViewModel FetchApplicationFromEnquiry(ApplicationPersonalViewModel model);
        ApplicationPersonalViewModel FillClassDetails(ApplicationPersonalViewModel model);
        ApplicationPersonalViewModel FillCaste(ApplicationPersonalViewModel model);
        ApplicationPersonalViewModel GetCurrentYearByYear(ApplicationPersonalViewModel model);
        ApplicationPersonalViewModel GetEmployeesByBranchId(ApplicationPersonalViewModel model);
        bool CheckSecondLanguage(short? CourseTypekey);
       
    }
}
