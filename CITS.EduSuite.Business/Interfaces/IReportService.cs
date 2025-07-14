using CITS.EduSuite.Business.Models.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CITS.EduSuite.Business.Interfaces
{
    public interface IReportService
    {
        
        List<dynamic> GetStudentsSummaryReport(ReportViewModel model, out long TotalRecords);

        #region FeePaymentSummary
        void FillDropDownLists(ReportViewModel model);
        //List<ReportViewModel> GetStudentsFeePaymentSummaryReport(ReportViewModel model);
        List<dynamic> GetStudentsFeePaymentSummaryReport(ReportViewModel model, out long TotalRecords);
        List<ApplicationFeePaymentDetailViewModel> BindTotalFeeDetails(ApplicationFeePaymentViewModel model);

        #endregion

        #region Study Mterial Issue Report
        //List<ReportViewModel> GetBookIssueSummaryReport(ReportViewModel model);
        List<dynamic> GetBookIssueSummaryReport(ReportViewModel model, out long TotalRecords);
        List<StudyMaterialDetailsModel> GetStudyMaterialById(StudyMaterialViewModel model);
        #endregion Study Mterial Issue Report

        #region Student Id Card Issue Report
        //List<ReportViewModel> GetStudentIdCardIssueSummaryReport(ReportViewModel model);
        List<dynamic> GetStudentIdCardIssueSummaryReport(ReportViewModel model, out long TotalRecords);
        #endregion Student Id Card Issue Report

        #region Internal Exam ResultSummary
        //List<ReportViewModel> GetInternalExamResultSummaryReport(ReportViewModel model);
        List<dynamic> GetInternalExamResultSummaryReport(ReportViewModel model, out long TotalRecords);
        List<InternalExamResultDetail> BindStudentMarkDetails(ReportViewModel model);
        #endregion

        #region University Fee PaymentSummary
        //List<ReportViewModel> GetUniversityFeePaymentSummaryReport(ReportViewModel model);
        List<dynamic> GetUniversityFeePaymentSummaryReport(ReportViewModel model, out long TotalRecords);
        List<UniversityPaymentDetailsmodel> BindUniversityTotalFeeDetails(UniversityPaymentViewmodel model);
        #endregion

        #region Attendance Summary
        // List<dynamic> GetAttendanceSummaryReport(ReportViewModel model);
        string GetAttendanceSummaryReport(ReportViewModel model);
        ReportViewModel GetApplicationById(ReportViewModel model);
        List<dynamic> GetAttendanceSummaryConvert(ReportViewModel model, out long TotalRecords);

        #endregion Attendance Summary

        #region Student Certicate Summary
        //List<ReportViewModel> GetStudentCerticateSummaryReport(ReportViewModel model);
        List<dynamic> GetStudentCerticateSummaryReport(ReportViewModel model, out long TotalRecords);
        List<StudentsCertificateReturnDetail> GetCertificateDetailsByApplication(long ApplicationKey);

        #endregion Student Certicate Summary

        #region University Certificate Summary
        List<dynamic> GetUniversityCerticateSummaryReport(ReportViewModel model, out long TotalRecords);
        List<UniversityCertificateDetails> GetUniversityCertificateDetailsByApplication(long ApplicationKey);

        #endregion University Certificate Summary

        #region Enquiry Report
        void FillDropdownLists(EnquiryReportViewModel model);

        List<CallReportCountData> GetCallReports(EnquiryReportViewModel model, out long TotalRecords);
        List<EnquiryReportViewModel> GetCallReportsDetails(EnquiryReportViewModel model, out long TotalRecords);

        #endregion Enquiry Report

        #region ActivityLog Report

        void FillAppUsers(ReportViewModel model);
        void FillMenus(ReportViewModel model);
        List<dynamic> GetActivityLogReport(ReportViewModel model, out long TotalRecords);

        #endregion ActivityLog Report

        #region UnitTest Exam Result
        ReportViewModel FillSubjectModules(ReportViewModel model);
        //ReportViewModel FillModuleTopics(ReportViewModel model);
        // List<ReportViewModel> GetUnitTestExamResultSummary(ReportViewModel model);
        List<dynamic> GetUnitTestExamResultSummary(ReportViewModel model, out long TotalRecords);
        List<UnitTestResultViewModel> BindUnitTestStudentMarkDetails(ReportViewModel model);
        #endregion UnitTest Exam Result

        #region Student ExamSchedule Result
        // List<ReportViewModel> GetStudentExamScheduleSummary(ReportViewModel model);
        List<dynamic> GetStudentExamScheduleSummary(ReportViewModel model, out long TotalRecords);
        List<ExamScheduleSummary> GetExamSchdeuleDetailsByApplication(long ApplicationKey);

        #endregion Student ExamSchedule Result

        #region Teacher Work Schedule
        //List<ReportViewModel> GetTeacherWorkScheduleSummary(ReportViewModel model);
        List<dynamic> GetTeacherWorkScheduleSummary(ReportViewModel model, out long TotalRecords);
        List<WorkscheduleSubjectmodel> GetHistoryWorkSchedule(ReportViewModel model);

        #endregion Teacher Work Schedule

        #region Student Late Summary
        List<dynamic> GetStudentLateSummary(ReportViewModel model, out long TotalRecords);

        #endregion Student Late Summary

        #region Student Leave Summary
        List<dynamic> GetStudentLeaveSummary(ReportViewModel model, out long TotalRecords);

        #endregion Student Leave Summary

        #region Student Absconders Summary
        List<dynamic> GetStudentAbscondersSummary(ReportViewModel model, out long TotalRecords);

        #endregion Student Absconders Summary

        #region Student EarlyDeparture Summary
        List<dynamic> GetStudentEarlyDepartureSummary(ReportViewModel model, out long TotalRecords);

        #endregion Student EarlyDeparture Summary

        void FillAttendanceTypes(ReportViewModel model);

        #region Day To Day Fee Payment Summary
        void FillDropDownForDayToDayFee(ReportViewModel model);
        List<ReportViewModel> GetStudentsDayToDayFeePaymentSummary(ReportViewModel model);

        #endregion Day To Day Fee Payment Summary

        #region Day To Day University Fee Payment Summary
        List<ReportViewModel> GetStudentsDayToDayUniversityPaymentSummary(ReportViewModel model);

        #endregion Day To Day Fee Payment Summary

        #region Fee Collection  Summary

        void FillCommonFeeTypes(ReportViewModel model);
        List<dynamic> GeTotalFeeCollectionSummary(ReportViewModel model);
        #endregion Fee Collection Summary

        #region Fee Refund Report
        List<dynamic> FeeRefundSummary(ReportViewModel model, out long TotalRecords);


        List<FeeRefundDetailViewModel> FillFeeRefundDetails(FeeRefundViewModel model);
        #endregion Fee Refund Report

        #region Enquiry Lead Count Summary
        List<dynamic> GetEnquiryLeadSummary(EnquiryReportViewModel model, out long TotalRecords);
        #endregion

        #region Enquiry Count Summary
        List<dynamic> GetEnquirySummary(EnquiryReportViewModel model, out long TotalRecords);
        #endregion

        #region Enquiry And Lead Details
        List<EnquiryReportViewModel> GetEnquiryandLeadDetails(EnquiryReportViewModel model, out long TotalRecords);

        #endregion Enquiry And Lead Details

        #region CashFlow Summary
        List<dynamic> GetCashFlowSummary(ReportViewModel model, out long TotalRecords);
        #endregion

        #region Employee Enquiry Target Summary
        List<dynamic> GetEmployeeEnquiryTargetSummary(ReportViewModel model);
        #endregion

        #region Fee Installment Summary
        List<ReportViewModel> GetFeeInstallmentSummary(ReportViewModel model);

        List<ApplicationFeePaymentViewModel> BindInstallmentFeeDetails(ApplicationFeePaymentViewModel model);
        #endregion

        #region Students Fee Paid and un Paid Summary
        string GetStudentFeeSummaryByDate(ReportViewModel model);
        #endregion

        #region Employee Salary Summary
        List<dynamic> GetEmployeeSalarySummary(ReportViewModel model, out long TotalRecords);
        #endregion Employee Salary Summary

        #region AccountHead Income And Expense Summary
        List<dynamic> GetAHIncomeExpenseSummary(ReportViewModel model, out long TotalRecords);
        void FillAccountHeadByBank(ReportViewModel model, bool IsAccountHeadKeys);
        #endregion

        #region Student LeaveLateAbscondersED Summary
        List<dynamic> GetStudentSummary_For_LeaveLateAbscondersED(ReportViewModel model, out long TotalRecords);
        #endregion Student LeaveLateAbscondersED Summary

        #region Employee Attendance Summary
        string GetEmpoyeeAttendanceSummaryReport(ReportViewModel model);
        #endregion
    }
}
