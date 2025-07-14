using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CITS.EduSuite.Business.Models.ViewModels;


namespace CITS.EduSuite.Business.Interfaces
{
    public interface IEnquiryScheduleService
    {
        List<EnquiryScheduleViewModel> GetEnquiryLeadSchedule(EnquiryScheduleViewModel model);
        EnquiryScheduleViewModel GetSearchDropDownLists(EnquiryScheduleViewModel model);
        //EnquiryScheduleViewModel FillFeedbackDrodownLists(EnquiryScheduleViewModel model);
        EnquiryLeadFeedbackViewModel CreateEnquiryLeadFeedback(EnquiryLeadFeedbackViewModel model);
        EnquiryLeadFeedbackViewModel UpdateEnquiryLeadFeedback(EnquiryLeadFeedbackViewModel model);
        EnquiryScheduleViewModel GetScheduleSummary(EnquiryScheduleViewModel model);
        List<EnquiryScheduleViewModel> GetHistoryByMobileNumber(EnquiryScheduleViewModel model);
        void GetCallStatusByEnquiryStatus(EnquiryScheduleViewModel model);
        void FillTelephoneCodes(EnquiryScheduleViewModel model);
        EnquiryScheduleViewModel GetAllHistoryByMobileNumber(EnquiryScheduleViewModel model);
        EnquiryScheduleViewModel GetEmployeesByBranchId(EnquiryScheduleViewModel model);
        EnquiryScheduleViewModel ProductiveCallsHistory(EnquiryScheduleViewModel model);
        EnquiryScheduleViewModel AllocateMultipleStaff(EnquiryScheduleViewModel model);
        EnquiryScheduleViewModel GetFHDropDownLists(EnquiryScheduleViewModel model);
        EnquiryScheduleViewModel FillFHCallStatuses(EnquiryScheduleViewModel model);
    }
}
