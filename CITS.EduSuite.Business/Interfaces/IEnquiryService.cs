using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CITS.EduSuite.Business.Models.ViewModels;

namespace CITS.EduSuite.Business.Interfaces
{
    public interface IEnquiryService
    {
        List<EnquiryViewModel> GetEnquiries(EnquiryViewModel model, out int TotalRecords);
        List<EnquiryLeadViewModel> GetEnquiryLead(EnquiryLeadViewModel model);
        EnquiryViewModel GetEnquiryById(EnquiryViewModel model);
        List<EnquiryFeedbackViewModel> GetEnquiryFeedbackByEnquiryId(Int64 id);
        EnquiryFeedbackViewModel GetEnquiryFeedbackById(EnquiryFeedbackViewModel model);
        EnquiryViewModel CreateEnquiry(EnquiryViewModel model);
        EnquiryViewModel UpdateEnquiry(EnquiryViewModel model);
        EnquiryFeedbackViewModel CreateEnquiryFeedback(EnquiryFeedbackViewModel model);
        EnquiryFeedbackViewModel UpdateEnquiryFeedback(EnquiryFeedbackViewModel model);
        EnquiryViewModel DeleteEnquiry(EnquiryViewModel model);
        EnquiryFeedbackViewModel DeleteEnquiryFeeback(EnquiryFeedbackViewModel model);
        EnquiryViewModel GetSearchDropDownLists(EnquiryViewModel model);
        EnquiryViewModel GetDepartmentByBranchId(EnquiryViewModel model);
        EnquiryViewModel GetCourseTypeByAcademicTerm(EnquiryViewModel model);
        EnquiryViewModel GetCountryByAcademicTerm(EnquiryViewModel model);
        EnquiryViewModel GetCourseByCourseType(EnquiryViewModel model);
        EnquiryViewModel GetUniversityByCourse(EnquiryViewModel model);
        // EnquiryViewModel GetUniversityByCountry(EnquiryViewModel model);
        void GetCallStatusByEnquiryStatus(EnquiryViewModel model);
        EnquiryViewModel CheckMobileNumberExists(string MobileNumber, Int64 RowKey, Int64? EnquiryLeadKey);
        // EnquiryViewModel CheckMobileNumberExists2(string MobileNumber, short? TelephoneCodeKey, long RowKey, long? EnquiryLeadKey, byte AcademicTermKey);
        EnquiryViewModel CheckEmailAddressExists(string EmailAddress, Int64 RowKey, Int64? EnquiryLeadKey);
        EnquiryViewModel CheckCounsellingTimeExists(EnquiryViewModel model);
        EnquiryViewModel GetEmployeesByBranchId(EnquiryViewModel model);
        EnquiryFeedbackViewModel CheckCallStatusDuration(EnquiryFeedbackViewModel model);
        List<EnquiryViewModel> GetEnquiry(EnquiryViewModel model);
        EnquiryViewModel FillCourseDuration(EnquiryViewModel model);
        EnquiryViewModel GetPhoneNumberLength(EnquiryViewModel model);
    }
}
