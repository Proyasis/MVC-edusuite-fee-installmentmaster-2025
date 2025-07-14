using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CITS.EduSuite.Business.Models.ViewModels;

namespace CITS.EduSuite.Business.Interfaces
{
    public interface IEnquiryLeadService
    {
        List<EnquiryLeadViewModel> GetEnquiryLeads(EnquiryLeadViewModel model, out int TotalRecords);
        EnquiryLeadViewModel GetEnquiryLeadById(EnquiryLeadViewModel model);
        List<EnquiryLeadFeedbackViewModel> GetEnquiryLeadFeedbackByLeadId(Int64 id);
        EnquiryLeadFeedbackViewModel GetEnquiryLeadFeedbackById(EnquiryLeadFeedbackViewModel model);
        EnquiryLeadViewModel UpdateEnquiryLeads(List<EnquiryLeadViewModel> model);
        EnquiryLeadFeedbackViewModel CreateEnquiryLeadFeedback(EnquiryLeadFeedbackViewModel model);
        EnquiryLeadFeedbackViewModel UpdateEnquiryLeadFeedback(EnquiryLeadFeedbackViewModel model);
        EnquiryLeadViewModel DeleteEnquiryLead(EnquiryLeadViewModel model);
        EnquiryLeadFeedbackViewModel DeleteEnquiryLeadFeeback(EnquiryLeadFeedbackViewModel model);
        EnquiryLeadViewModel GetSearchDropDownLists(EnquiryLeadViewModel model);
        EnquiryLeadViewModel GetEmployeesByBranchId(EnquiryLeadViewModel model);
        List<EnquiryLeadViewModel> GetPendingLeadByBranch(EnquiryLeadViewModel model, out int TotalRecords);
        EnquiryLeadViewModel GetBranches(EnquiryLeadViewModel model);
        EnquiryLeadViewModel GetEmployeesWithCodeByBranchId(EnquiryLeadViewModel model);
        void GetCallStatusByEnquiryStatus(EnquiryLeadFeedbackViewModel model);
        EnquiryLeadViewModel CheckMobileNumberExists(string MobileNumber, Int16 TelephoneCodeKey, Int64 RowKey);
        //EnquiryLeadViewModel CheckMobileNumberExists2(string MobileNumber, short? TelephoneCodeKey, long RowKey, byte AcademicTermKey);
        List<EnquiryLeadViewModel> CheckBulkMobileNumberExists(List<EnquiryLeadViewModel> modelList);
        List<EnquiryLeadViewModel> CheckBulkEmailAddressExists(List<EnquiryLeadViewModel> modelList);
        EnquiryLeadFeedbackViewModel CheckCallStatusDuration(EnquiryLeadFeedbackViewModel model);
        List<EnquiryLeadViewModel> GetEnquiryLead(EnquiryLeadViewModel model);
        EnquiryLeadViewModel CheckEmailAddressExists(string EmailAddress, long RowKey);
        EnquiryLeadViewModel GetLeadsAllocation(EnquiryLeadViewModel model);
        EnquiryLeadViewModel AllocateNewLeads(EnquiryLeadViewModel model);
        EnquiryLeadViewModel GetLeadValues(EnquiryLeadViewModel model);
        EnquiryLeadViewModel GetPhoneNumberLength(EnquiryLeadViewModel model);
    }
}
