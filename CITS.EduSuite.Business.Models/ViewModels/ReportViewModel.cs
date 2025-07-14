using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CITS.EduSuite.Business.Models.ViewModels
{
    public class ReportViewModel : BaseSearchStudentsViewModel
    {

        public ReportViewModel()
        {
            FeeTypes = new List<SelectListModel>();
            PaymentStatuses = new List<SelectListModel>();
            PaymentModes = new List<SelectListModel>();
            Subjects = new List<SelectListModel>();
            InternalExamTerm = new List<SelectListModel>();
            InternalExamTermKeys = new List<long>();
            SubjectKeys = new List<long>();
            FeeTypeKeys = new List<long>();
            SubjectModules = new List<SelectListModel>();
            ModuleTopics = new List<SelectListModel>();
            SubjectModuleKeys = new List<long>();
            PaymentModeKeys = new List<short>();
            UserKeys = new List<long>();
            MenuKeys = new List<long>();
            AppUsers = new List<SelectListModel>();
            Menus = new List<SelectListModel>();
            AttendanceTypes = new List<SelectListModel>();
            ProcessStatuses = new List<SelectListModel>();
            AttendanceTypeKeys = new List<long>();
            AccountHeadKeys = new List<long>();
            BankAccountKeys = new List<long>();
            CashFlowTypeKeys = new List<long>();
            EmployeeKeys = new List<long>();
            DesignationKeys = new List<long>();
            DepartmentKeys = new List<long>();
            EmployeeStatusKeys = new List<long>();
            Designations = new List<SelectListModel>();
            Departments = new List<SelectListModel>();
            EmployeeStatus = new List<SelectListModel>();
            SalaryYears = new List<SelectListModel>();
            SalaryMonths = new List<SelectListModel>();
            SalaryYearKeys = new List<long>();
            SalaryMonthKeys = new List<long>();
        }
        public long RowKey { get; set; }
        public string ApplicationNo { get; set; }
        public string CourseType { get; set; }
        public string Name { get; set; }
        public string GuardianName { get; set; }
        public string MotherName { get; set; }
        public string PermanentAddress { get; set; }
        public string PresentAddress { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }
        public string Mobile { get; set; }
        public string Gender { get; set; }
        public string ClassRequired { get; set; }
        public DateTime? DOB { get; set; }
        //public byte Gender { get; set; }
        public int? StartYear { get; set; }
        public decimal? TotalFee { get; set; }
        public string ClassRequiredDesc { get; set; }
        public int? PresentJob_CourseOfStudyId { get; set; }
        public DateTime? DateOfAdmission { get; set; }
        public int? StudyMaterialIssueStatus { get; set; }
        public string EnrollmentNo { get; set; }
        public string PhotoPath { get; set; }
        public string ExamRegisterNo { get; set; }
        public string Remarks { get; set; }
        public string IsFromEnquiry { get; set; }
        public string AdmissionNo { get; set; }
        public long? SerialNumber { get; set; }
        public short? CurrentYear { get; set; }
        public int? RollNumber { get; set; }
        public string RollNoCode { get; set; }
        public string Course { get; set; }
        public string Affiliations { get; set; }
        public string Batch { get; set; }
        public string Mode { get; set; }
        public string ClassMode { get; set; }
        public string Religion { get; set; }
        public string AcademicTerm { get; set; }
        public string SecondLanguage { get; set; }
        public string Medium { get; set; }
        public string Income { get; set; }
        public string NatureOfEnquiry { get; set; }
        public string Agent { get; set; }
        public string StudentStatus { get; set; }
        public string Class { get; set; }
        public decimal? TotalPaid { get; set; }
        public decimal? BalanceFee { get; set; }
        public decimal? TotalUniversityPaid { get; set; }
        public string CurrentYearText { get; set; }
        public string IsConsessionText { get; set; }
        public string IsInstallmentText { get; set; }
        public string IsTaxText { get; set; }
        public string BranchName { get; set; }
        public string BloodGroupName { get; set; }
        public string ReportType { get; set; }

        #region Fee Payment
        public List<SelectListModel> FeeTypes { get; set; }
        public List<SelectListModel> PaymentStatuses { get; set; }
        public List<long> FeeTypeKeys { get; set; }
        public byte? PaymentStatus { get; set; }
        public decimal? PercentageFrom { get; set; }
        public decimal? PercentageTo { get; set; }
        public decimal? TotalPaidSum { get; set; }
        public decimal? BalanceFeeSum { get; set; }
        public decimal? TotalFeeSum { get; set; }
        public decimal? TotalUniversityPaidSum { get; set; }

        // For DayToDay Fee Receipt
        public DateTime? FeeDate { get; set; }
        public string FeeTypeName { get; set; }
        public string PaymentModeName { get; set; }
        public string ReceiptNo { get; set; }
        public decimal? FeeAmount { get; set; }
        public decimal? CGSTAmount { get; set; }
        public decimal? SGSTAmount { get; set; }
        public decimal? IGSTAmount { get; set; }
        public decimal? TaxableAmount { get; set; }
        public decimal? CessAmount { get; set; }
        public decimal? TotalAmount { get; set; }
        public decimal? CGSTRate { get; set; }
        public decimal? SGSTRate { get; set; }
        public decimal? CessRate { get; set; }
        public string FeeDescreption { get; set; }
        public string ChequeOrDDNumber { get; set; }
        public string CardNumber { get; set; }
        public string PaymentModeSubName { get; set; }
        public DateTime? ChequeClearanceDate { get; set; }
        public bool? IsRefund { get; set; }
        public decimal? RefundAmount { get; set; }
        public List<short> PaymentModeKeys { get; set; }
        public List<SelectListModel> PaymentModes { get; set; }
        public List<long> AccountHeadKeys { get; set; }
        public List<SelectListModel> AccountHeads { get; set; }
        public List<long> BankAccountKeys { get; set; }
        public List<SelectListModel> BankAccounts { get; set; }
        public List<long> CashFlowTypeKeys { get; set; }
        public List<SelectListModel> CashFlowTypes { get; set; }
        public bool? IsCancel { get; set; }
        public bool? IfServiceCharge { get; set; }
        public decimal? ServiceFee { get; set; }
        public decimal? TotalDeductionFee { get; set; }
        public DateTime? CancelDate { get; set; }
        public string CancelRemarks { get; set; }
        public string AccountHeadName { get; set; }
        public long? UniversityPaymentDetailsKey { get; set; }
        public string CatagoryName { get; set; }

        #endregion

        #region Book Issue
        public int? AvailableBookCount { get; set; }
        public int? IssuedBookCount { get; set; }
        public int? TotalBooks { get; set; }
        public decimal? ChalanAmount { get; set; }
        public string BankName { get; set; }
        public string ChalanNo { get; set; }
        public DateTime? ChalanDate { get; set; }


        #endregion Book Issue

        #region Id Card Issue
        public int? ReceivedStatusKey { get; set; }
        public int? IssuedStatusKey { get; set; }
        public int? IssuedStatus { get; set; }
        public int? ReceivedStatus { get; set; }
        public string ReceivedBy { get; set; }
        public string IssuedBy { get; set; }
        public DateTime? ReceivedDate { get; set; }
        public DateTime? IssuedDate { get; set; }

        #endregion Id Card Issue

        #region Internal Exam Report
        public long? InternalExamKey { get; set; }
        public long? InternalExamDetailsKey { get; set; }

        public long? SubjectKey { get; set; }
        public string SubjectName { get; set; }
        public long? ClassDetailsKey { get; set; }
        public DateTime? ExamDate { get; set; }
        public TimeSpan? ExamStartTime { get; set; }
        public TimeSpan? ExamEndTime { get; set; }
        public decimal? MaximumMark { get; set; }
        public decimal? MinimumMark { get; set; }
        public long? InternalExamTermKey { get; set; }
        public string InternalExamTermName { get; set; }
        public string BatchName { get; set; }
        public int? TotalStudents { get; set; }
        public int? Passed { get; set; }
        public int? Failed { get; set; }
        public int? Absent { get; set; }

        public List<SelectListModel> Subjects { get; set; }
        public List<SelectListModel> InternalExamTerm { get; set; }

        public List<long> SubjectKeys { get; set; }
        public List<long> InternalExamTermKeys { get; set; }
        public long? EmployeeKey { get; set; }
        public long? ApplicationKey { get; set; }
        public decimal? UniversityDays { get; set; }
        #endregion

        #region Student Certificate Summary

        public int? CertificateStatus { get; set; }

        public int? NoOfRecieved { get; set; }
        public int? NoOfVerified { get; set; }
        public int? TotalCertificates { get; set; }
        public int? NoOfTempReturned { get; set; }
        public int? NoOfIssued { get; set; }
        public int? NoOfReturned { get; set; }

        #endregion Student Certificate Summary

        #region UnitTest Exam Result
        public List<SelectListModel> SubjectModules { get; set; }
        public List<SelectListModel> ModuleTopics { get; set; }
        public List<long> SubjectModuleKeys { get; set; }
        public string ModuleName { get; set; }
        public int? TotalTopics { get; set; }
        public long? UnitTestScheduledKey { get; set; }
        public long? SubjectModuleKey { get; set; }

        #endregion UnitTest Exam Result

        #region Exam Status
        public int? AppliedSubject { get; set; }
        public int? ExamStatus { get; set; }
        public int? ExamResultStatus { get; set; }
        #endregion Exam Status

        #region TeacherWorkSchedule
        public int? Duration { get; set; }
        public int? ProgressStatus { get; set; }

        #endregion TeacherWorkSchedule

        #region Students Late

        public string AttachmentPath { get; set; }
        public string LateRemarks { get; set; }
        public int? LateMinutes { get; set; }
        public DateTime? LateDate { get; set; }
        public List<SelectListModel> AttendanceTypes { get; set; }
        public List<long> AttendanceTypeKeys { get; set; }

        #endregion Students Late

        #region Students Leave
        public DateTime? LeaveDateFrom { get; set; }
        public DateTime? LeaveDateTo { get; set; }
        public bool? IsApprove { get; set; }
        public string ApproveRemarks { get; set; }
        public string LeaveRemarks { get; set; }

        #endregion Students Leave

        #region Students Absconders
        public DateTime? AbscondersDate { get; set; }
        public bool? IsAbsconders { get; set; }
        public string AbscondersRemarks { get; set; }
        public string StudentsAbscondersRemarks { get; set; }

        #endregion Students Absconders

        #region Students Early Departure

        public DateTime? EarlyDepartureDate { get; set; }
        public TimeSpan? EarlyDepartureTime { get; set; }
        public string EarlyDepartureRemarks { get; set; }


        #endregion Students Early Departure

        #region Activity Log

        public List<long> UserKeys { get; set; }
        public List<long> MenuKeys { get; set; }
        public List<SelectListModel> AppUsers { get; set; }
        public List<SelectListModel> Menus { get; set; }

        public DateTime ActivityDate { get; set; }
        public string Status { get; set; }
        public string HostName { get; set; }
        public string UserID { get; set; }
        public string MenuName { get; set; }
        public string MenuAction { get; set; }
        public string ActionDone { get; set; }
        

        #endregion Activity Log

        #region Fee Refund
        public List<SelectListModel> ProcessStatuses { get; set; }
        public int? ProcessStatusKey { get; set; }
        public int? FeeRefundTabKey { get; set; }
        #endregion

        #region Fee Installment Summary

        public long? CourseKey { get; set; }
        public long? UniversityMasterKey { get; set; }
        public short? CourseTypeKey { get; set; }
        public short? ReligionKey { get; set; }
        public short? StudentStatusKey { get; set; }
        public short? ModeKey { get; set; }
        public int? AgentKey { get; set; }
        public short? CasteKey { get; set; }
        public string CasteName { get; set; }
        public Int32? InstallmentNo { get; set; }
        public string InstallmentMonth { get; set; }
        public int? InstallmentYear { get; set; }
        public int? InstallmentMonthKey { get; set; }
        public int? FeeYear { get; set; }
        public decimal? InstallmentAmount { get; set; }
        public decimal? InstallmentPaid { get; set; }
        public decimal? BalanceDue { get; set; }
        public decimal? DueFineAmount { get; set; }
        public decimal? SuperFineAmount { get; set; }
        public DateTime? DueDate { get; set; }

        #endregion

        #region Employee Salary
        public List<long> EmployeeKeys { get; set; }
        public List<long> DesignationKeys { get; set; }
        public List<long> DepartmentKeys { get; set; }
        public List<long> EmployeeStatusKeys { get; set; }
        public List<long> SalaryYearKeys { get; set; }
        public List<long> SalaryMonthKeys { get; set; }

        public decimal? TotalSalary { get; set; }
        public decimal? BalanceAmount { get; set; }

        public List<SelectListModel> Designations { get; set; }
        public List<SelectListModel> Departments { get; set; }
        public List<SelectListModel> EmployeeStatus { get; set; }
        public List<SelectListModel> SalaryYears { get; set; }
        public List<SelectListModel> SalaryMonths { get; set; }

        #endregion

        #region AccountHead Income And Expense Summary
        public decimal? TotalIncome { get; set; }
        public decimal? TotalExpense { get; set; }
        public decimal? TotalBalance { get; set; }
        public decimal? AHIncomes { get; set; }
        public decimal? Expense { get; set; }
        public decimal? Balance { get; set; }
        #endregion
    }
}