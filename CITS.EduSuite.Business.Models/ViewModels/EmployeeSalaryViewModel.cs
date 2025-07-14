using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using CITS.EduSuite.Business.Models.Resources;
using CITS.Validations;

namespace CITS.EduSuite.Business.Models.ViewModels
{
    public class EmployeeSalaryMasterViewModel : BaseModel
    {
        public EmployeeSalaryMasterViewModel()
        {
            EmployeeSalaryEarnings = new List<EmployeeSalaryDetailViewModel>();
            EmployeeSalaryDeductions = new List<EmployeeSalaryDetailViewModel>();
            EmployeeSalaryAdvances = new List<SalaryAdvanceViewModel>();
            EmployeeSalaryOtherEarnings = new List<EmployeeSalaryOtherAmountViewModel>();
            EmployeeSalaryOtherDeductions = new List<EmployeeSalaryOtherAmountViewModel>();
            Employees = new List<SelectListModel>();
            
            Branches = new List<SelectListModel>();
            SalaryMonthKey = Convert.ToByte(DateTimeUTC.Now.AddMonths(-1).Month);
            SalaryYearKey = Convert.ToInt16(DateTimeUTC.Now.AddMonths(-1).Year);
            SalaryMasterKeyList = new List<long>();
            // BranchKey = GlobalVariables.Defaults.DefaultBranch;
            LeaveTypes = new List<LeaveTypeViewModel>();
            //HolidayLists = new List<HolidayViewModel>();

        }

        public long SalaryMasterKey { get; set; }
        public List<long> SalaryMasterKeyList { get; set; }

        [Required(ErrorMessageResourceType = typeof(ModelResources), ErrorMessageResourceName = "BranchRequired")]
        public short BranchKey { get; set; }
        public string BranchName { get; set; }

        [Required(ErrorMessageResourceType = typeof(ModelResources), ErrorMessageResourceName = "EmployeeRequired")]
        public long EmployeeKey { get; set; }
        public string EmployeeName { get; set; }
        public string EmployeeEmailAddress { get; set; }
        public DateTime DateCreated { get; set; }
        public string EmployeeCode { get; set; }

        [Required(ErrorMessageResourceType = typeof(ModelResources), ErrorMessageResourceName = "SalaryMonthRequired")]
        //[GreaterThan("LeaveFrom", PassOnNull = true, ErrorMessageResourceType = typeof(ModelResources), ErrorMessageResourceName = "LeaveToCompareErrorMessage")]

        public DateTime? SalaryMonth
        {
            get
            {
                return new DateTime(SalaryYearKey, SalaryMonthKey, 01);
            }
            set
            {

            }
        }

        public byte SalaryMonthKey { get; set; }
        public short SalaryYearKey { get; set; }
        public string SalaryMonthName { get; set; }
        public DateTime DateTimeNow
        {
            get
            {
                return new DateTime(DateTimeUTC.Now.Year, DateTimeUTC.Now.Month, 01);
            }

        }
        public decimal MonthlySalary { get; set; }
        public decimal TotalSalary { get; set; }
        public decimal PaidAmount { get; set; }
        public string LoanPaymentKeys { get; set; }
        public decimal? LoanAmount { get; set; }

        [StringLength(50, ErrorMessageResourceType = typeof(ModelResources), ErrorMessageResourceName = "ReferenceNumberLengthErrorMessage")]
        public string ReferenceNumber { get; set; }
        public bool OtherAmountType { get; set; }

        [RegularExpression(@"^\d{0,10}(\.\d{1,2})?$", ErrorMessageResourceType = typeof(ModelResources), ErrorMessageResourceName = "AmountExpressionErrorMessage")]
        public decimal? OtherAmount { get; set; }

        [Required(ErrorMessageResourceType = typeof(ModelResources), ErrorMessageResourceName = "NoOfDaysWorkedRequired")]
        [RegularExpression(@"^\d{0,3}(\.\d{1,2})?$", ErrorMessageResourceType = typeof(ModelResources), ErrorMessageResourceName = "Decimal3RegularExpressionErrorMessage")]
        public decimal? NoOfDaysWorked { get; set; }

        [RequiredIf("SalaryTypeKey", DbConstants.SalaryType.Monthly, ErrorMessageResourceType = typeof(ModelResources), ErrorMessageResourceName = "BaseWorkingDaysRequired")]
        public decimal BaseWorkingDays { get; set; }
        public decimal BaseWorkingHours { get; set; }
        public decimal? TotalWorkingDays { get; set; }
        public decimal OvertimePerAHour { get; set; }

        //[RegularExpression(@"^\d{0,10}(\.\d{1,2})?$", ErrorMessageResourceType = typeof(ModelResources), ErrorMessageResourceName = "AmountExpressionErrorMessage")]
        //public decimal? OverTimeHours { get { return (int)(TotalOverTime ?? 0) / 60; } set { } }
        public decimal? OverTimeTotalAmount { get; set; }
        public string Remarks { get; set; }
        public decimal DaysInMonth { get; set; }
        public short SalaryStatusKey { get; set; }
        public string SalaryStatusName { get; set; }
        public long? SalaryPaymentKey { get; set; }
        public HttpPostedFileBase PaySlipFile { get; set; }
        public string PaySlipFileName { get; set; }
        public string PaySlipPassword { get; set; }
        public List<SelectListModel> Employees { get; set; }
        public List<EmployeeSalaryDetailViewModel> EmployeeSalaryEarnings { get; set; }
        public List<EmployeeSalaryDetailViewModel> EmployeeSalaryDeductions { get; set; }
        public List<EmployeeSalaryDetailViewModel> EmployeeSalaryDetails { get { return EmployeeSalaryEarnings.Concat(EmployeeSalaryDeductions).ToList(); } set { } }
        public List<SalaryAdvanceViewModel> EmployeeSalaryAdvances { get; set; }
        public List<EmployeeSalaryOtherAmountViewModel> EmployeeSalaryOtherEarnings { get; set; }
        public List<EmployeeSalaryOtherAmountViewModel> EmployeeSalaryOtherDeductions { get; set; }
        public List<EmployeeSalaryOtherAmountViewModel> EmployeeSalaryOtherAmounts { get { return EmployeeSalaryOtherEarnings.Concat(EmployeeSalaryOtherDeductions).ToList(); } set { } }
        public List<SelectListModel> Branches { get; set; }
        public List<SelectListModel>Salarytypelist { get; set; }
        //public decimal? TotalWorkingDuration { get; set; }
        //public decimal? TotalWorkingDurationHour { get { return (int)(TotalWorkingDuration ?? 0) / 60; } set { } }
        //public decimal? TotalWorkingDurationMinutes { get { return (int)(TotalWorkingDuration ?? 0) % 60; } set { } }
        //public decimal? OverTime { get; set; }
        //public decimal? OverTimeMinutes { get { return (int)(OverTime ?? 0) % 60; } set { } }
        //public decimal? TotalWeekOffDayOverTime { get; set; }
        //public decimal? TotalWeekOffDayOverTimeHours { get { return (int)(TotalWeekOffDayOverTime ?? 0) / 60; } set { } }
        //public decimal? TotalWeekOffDayOverTimeMinutes { get { return (int)(TotalWeekOffDayOverTime ?? 0) % 60; } set { } }
        //public decimal? TotalOverTime { get; set; }
        [RegularExpression(@"^[0-9]+$", ErrorMessageResourceType = typeof(ModelResources), ErrorMessageResourceName = "RegularExpressionDigitErrorMessage")]
        [Display(Name = "Overtime", ResourceType = typeof(EduSuiteUIResources))]
        public decimal? OverTimeHours { get; set; }

        [RegularExpression(@"^[0-9]+$", ErrorMessageResourceType = typeof(ModelResources), ErrorMessageResourceName = "RegularExpressionDigitErrorMessage")]
        [Display(Name = "Overtime", ResourceType = typeof(EduSuiteUIResources))]
        public decimal? OverTimeMinutes { get; set; }
        public decimal? HolidayCount { get; set; }
        public decimal? OffdayCount { get; set; }
        public decimal? WeekOffCount { get; set; }
        public decimal AdditionalDayAmount { get; set; }

        [RegularExpression(@"^[0-9]+$", ErrorMessageResourceType = typeof(ModelResources), ErrorMessageResourceName = "RegularExpressionDigitErrorMessage")]
        [Display(Name = "Overtime", ResourceType = typeof(EduSuiteUIResources))]
        public decimal? AdditionalDayCount { get; set; }
        public decimal? AdditionalDayTotalAmount { get; set; }
        public decimal? LeaveCount { get; set; }
        public decimal? TotalAbsentCount { get; set; }
        public bool IsAttendance { get; set; }
        public byte? SalaryTypeKey { get; set; }
        public string SalaryTypeName { get; set; }
        public List<LeaveTypeViewModel> LeaveTypes { get; set; }
        //public List<HolidayViewModel> HolidayLists { get; set; }
        public decimal? LOP { get; set; }
        public bool? IsFixed { get; set; }
        public decimal? AbsentDays { get; set; }
        public string VoucherNumber { get; set; }
        public int PageIndex { get; set; }
        public int PageSize { get; set; }
        public string SortBy { get; set; }
        public string SortOrder { get; set; }

    }

    public class EmployeeSalaryDetailViewModel
    {
        public long RowKey { get; set; }
        public long EmployeeSalaryMasterKey { get; set; }
        public string SalaryHeadCode { get; set; }
        public short SalaryHeadTypeKey { get; set; }
        public int SalaryHeadKey { get; set; }
        public string SalaryHeadName { get; set; }
        public decimal? Amount { get; set; }
        public string Formula { get; set; }
        public string Applicable { get; set; }
        public string SalaryHeadTypeName { get; set; }

        public bool IsFixed { get; set; }
        public bool IsInclude { get; set; }
    }
    public class EmployeeSalaryOtherAmountViewModel
    {
        public EmployeeSalaryOtherAmountViewModel()
        {
            SalaryOtherAmountTypes = new List<SelectListModel>();
        }
        public long RowKey { get; set; }
        public int? OtherAmountTypeKey { get; set; }
        public string OtherAmountTypeName { get; set; }
        public bool IsAddition { get; set; }
        public long EmployeeSalaryMasterKey { get; set; }

        //[RequiredIfNot("RowKey", 0, ErrorMessageResourceType = typeof(ModelResources), ErrorMessageResourceName = "AmountRequired")]
        [RegularExpression(@"^\d{0,10}(\.\d{1,2})?$", ErrorMessageResourceType = typeof(ModelResources), ErrorMessageResourceName = "AmountExpressionErrorMessage")]
        public decimal? Amount { get; set; }
        public List<SelectListModel> SalaryOtherAmountTypes { get; set; }

    }
    public class SalaryAdvanceViewModel
    {
        public long RowKey { get; set; }
        public long PaymentKey { get; set; }
        public decimal? PaidAmount { get; set; }
        public decimal? BeforeTakenAdvance { get; set; }
        public DateTime PaymentDate { get; set; }
        public string Purpose { get; set; }
        public bool IsDeduct { get; set; }
        public decimal AdvanceBalance
        {
            get
            {
                return (BeforeTakenAdvance ?? 0) - (PaidAmount ?? 0);
            }
            set { }
        }
    }
    public class SalaryPaymentSlipViewModel
    {
        public SalaryPaymentSlipViewModel()
        {
            SalaryPayments = new List<EmployeeSalaryDetailViewModel>();
            SalaryDeductions = new List<EmployeeSalaryDetailViewModel>();
            SalaryComponents = new List<SalaryComponentViewModel>();
            SalaryAdvances = new List<EmployeeSalaryDetailViewModel>();
        }
        public long EmployeeKey { get; set; }
        public string EmployeeEmailAddress { get; set; }
        public long SalaryMasterKey { get; set; }
        public byte SalaryMonthKey { get; set; }
        public short SalaryYearKey { get; set; }
        public string SalaryMonthName { get; set; }
        public string EmployeeCode { get; set; }
        public string EmployeeName { get; set; }
        public DateTime EmployeeDateOfBirth { get; set; }
        public string DepartmentName { get; set; }
        public decimal? BaseWorkingDays { get; set; }
        public decimal OverTimeTotalAmount { get; set; }
        public string DesignationName { get; set; }
        public DateTime PaymentDate { get; set; }
        public DateTime DateOfJoining { get; set; }
        public string AccountNumber { get; set; }
        public decimal PaidAmount { get; set; }
        public decimal AdvanceBalance { get; set; }
        public decimal? BalanceAmount { get; set; }
        public decimal? DaysInMonth { get; set; }
        public decimal NoOfDaysWorked { get; set; }
        public string CompanyName { get; set; }
        public string CompanySubName { get; set; }
        public string CompanyAddress { get; set; }
        public string CompanyLogo { get; set; }
        public string CompanyLogoPath { get; set; }
        public List<EmployeeSalaryDetailViewModel> SalaryPayments { get; set; }
        public List<EmployeeSalaryDetailViewModel> SalaryDeductions { get; set; }
        public List<EmployeeSalaryDetailViewModel> SalaryAdvances { get; set; }
        public List<SalaryComponentViewModel> SalaryComponents { get; set; }
        public decimal? LOP { get; set; }
        public string BankName { get; set; }
        public string IFSCCode { get; set; }
        public string AdharNumber { get; set; }
        public string UANNumber { get; set; }
        public string MICRCode { get; set; }
        public string AccountName { get; set; }
        public string Remarks { get; set; }
        public string EmployeeMobileNumber { get; set; }
        public decimal? TotalWorkingDays { get; set; }
        public decimal? AbsentDays { get; set; }
        public decimal? OvertimePerAHour { get; set; }
        public decimal? OverTimeHours { get; set; }
        public decimal? OverTimeMinutes { get; set; }
        public decimal? AdditionalDayAmount { get; set; }
        public decimal? AdditionalDayCount { get; set; }
        public decimal? WeekOffCount { get; set; }
        public decimal? OffdayCount { get; set; }
        public decimal? HolidayCount { get; set; }
        public string VoucherNumber { get; set; }
        public decimal? TotalSalary { get; set; }
        public decimal? MonthlySalary { get; set; }


    }
    public class LeaveTypeViewModel
    {
        public int? LeaveTypeCount { get; set; }
        public string LeaveTypeName { get; set; }
        public int? LeaveAvailableCount { get; set; }
        public Int16? LeaveTypeKey { get; set; }
        public bool? SalaryDeductionForAdditional { get; set; }
    }
}
