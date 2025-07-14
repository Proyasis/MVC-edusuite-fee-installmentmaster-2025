using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CITS.EduSuite.Business.Models.ViewModels;

namespace CITS.EduSuite.Business.Interfaces
{
    public interface IApplicationService
    {
        List<ApplicationPersonalViewModel> GetApplications(ApplicationPersonalViewModel model, out long TotalRecords);
        ApplicationPersonalViewModel DeleteApplication(long Id);
        ApplicationPersonalViewModel GetSearchDropdownList(ApplicationPersonalViewModel model);
        ApplicationPersonalViewModel UpdateApplicantPhoto(ApplicationPersonalViewModel model);
        ApplicationPersonalViewModel GetApplicantPhotoById(long Id);
        ApplicationPersonalViewModel DeleteApplicantPhoto(long Id);
        ApplicationViewModel GetApplicationDetailsById(long Id);
        IQueryable<EnquiryViewModel> GetInterestedEnquiry(EnquiryViewModel model);
        ApplicationEnRollmentNoViewModel GetEnrollmentNo(ApplicationEnRollmentNoViewModel model, List<long> ApplicationKeys);
        ApplicationEnRollmentNoViewModel GetStudentDetailsByStudentKey(long Applicationkey);
        ApplicationEnRollmentNoViewModel UpdateEnrollmentNo(ApplicationEnRollmentNoViewModel model);
        ApplicationPersonalViewModel FillSearchBatch(ApplicationPersonalViewModel model);
        List<List<dynamic>> AttendanceAcademicPerfomance(long Applicationkey);
        ApplicationPersonalViewModel GetApplicationPhoneNo(long Id);
        ApplicationPersonalViewModel UpdateApplicantionPhoneNo(ApplicationPersonalViewModel model);
        ApplicationViewModel ViewApplicationById(long Id);
        ApplicationPersonalViewModel FillSearchCourse(ApplicationPersonalViewModel model);
        ApplicationPersonalViewModel FillSearchUniversity(ApplicationPersonalViewModel model);
    }
}
