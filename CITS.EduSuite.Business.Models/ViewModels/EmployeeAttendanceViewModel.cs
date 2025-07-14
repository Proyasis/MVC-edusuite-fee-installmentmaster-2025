using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CITS.EduSuite.Business.Models.Resources;
using System.ComponentModel.DataAnnotations;
using CITS.Validations;

namespace CITS.EduSuite.Business.Models.ViewModels
{
    public class EmployeeAttendanceViewModel : BaseModel
    {

        public EmployeeAttendanceViewModel()
        {
            Departments = new List<SelectListModel>();
            Employees = new List<GroupSelectListModel>();
            AttendanceStatuses = new List<GroupSelectListModel>();
            LeaveTypes = new List<SelectListModel>();
            //AttendanceDate = DateTimeUTC.Now;
            Branches = new List<SelectListModel>();
            //AttendanceMonthKey = Convert.ToByte(DateTimeUTC.Now.Month);
            //AttendanceYearKey = Convert.ToInt16(DateTimeUTC.Now.Year);
            EmployeeAttendanceStatus = new List<SelectListModel>();
        }

        public long RowKey { get; set; }

        // [Required(ErrorMessageResourceType = typeof(ModelResources), ErrorMessageResourceName = "BranchRequired")]
        public short BranchKey { get; set; }
        public string BranchName { get; set; }

        //[Required(ErrorMessageResourceType = typeof(ModelResources), ErrorMessageResourceName = "EmployeeRequired")]
        public long EmployeeKey { get; set; }
        public string EmployeeName { get; set; }

        public string EmployeeCode { get; set; }
        public string BiomatricId { get; set; }

        [Required(ErrorMessageResourceType = typeof(ModelResources), ErrorMessageResourceName = "AttendanceStatusRequired")]
        //[RequiredIf("AttendanceConfigType", DbConstants.AttendanceConfigType.MarkPresent, ErrorMessageResourceType = typeof(ModelResources), ErrorMessageResourceName = "AttendanceStatusRequired")]
        public short AttendanceStatusKey { get; set; }
        public string AttendanceStatusName { get; set; }
        public string AttendanceStatusCode { get; set; }
        public string AttendanceStatusColor { get; set; }

        //[RequiredIf("AttendanceConfigType", DbConstants.AttendanceConfigType.MarkPresent, ErrorMessageResourceType = typeof(ModelResources), ErrorMessageResourceName = "LeaveTypeRequired")]
        [RequiredIf("AttendanceStatusKey", DbConstants.AttendanceStatus.Leave, ErrorMessageResourceType = typeof(ModelResources), ErrorMessageResourceName = "LeaveTypeRequired")]
        public short? LeaveTypeKey { get; set; }
        public string LeaveTypeName { get; set; }

        public long LeaveCarryForwardKey { get; set; }

        public short ApprovalStatusKey { get; set; }
        public string ApprovalStatusName { get; set; }




        //[Required(ErrorMessageResourceType = typeof(ModelResources), ErrorMessageResourceName = "Department_Required")]
        //public short DepartmentKey { get; set; }

        [Required(ErrorMessageResourceType = typeof(ModelResources), ErrorMessageResourceName = "EmployeeAttendanceDate_Required")]
        public DateTime? AttendanceDate { get; set; }

        public byte AttendanceMonthKey { get; set; }
        public short AttendanceYearKey { get; set; }


        //[RequiredIfNot("AttendanceConfigType", DbConstants.AttendanceConfigType.MarkPresent, ErrorMessageResourceType = typeof(ModelResources), ErrorMessageResourceName = "EmployeeAttendanceInTime_Required")]
        [RequiredIf("AttendanceStatusKey", DbConstants.AttendanceStatus.Present, ErrorMessageResourceType = typeof(ModelResources), ErrorMessageResourceName = "EmployeeAttendanceInTime_Required")]
        [DisplayFormat(DataFormatString = "{0:hh\\:mm tt}", ConvertEmptyStringToNull = true)]
        public TimeSpan? InTime { get; set; }

        public DateTime? InDateTime
        {
            get
            {
                return (InTime != null ? Convert.ToDateTime(AttendanceDate) + (InTime ?? TimeSpan.Zero) : (DateTime?)null);
            }
            set
            {
                InTime = value != null && InTime == null ? value.Value.TimeOfDay : InTime;
            }
        }

        // [Required(ErrorMessageResourceType = typeof(ModelResources), ErrorMessageResourceName = "EmployeeAttendanceOutTime_Required")]

        //[RequiredIf("AttendanceStatusKey",DbConstants.AttendanceStatus.Present, ErrorMessageResourceType = typeof(ModelResources), ErrorMessageResourceName = "EmployeeAttendanceOutTime_Required")]
        [GreaterThanOrEqualTo("InTime", PassOnNull = true, ErrorMessageResourceType = typeof(ModelResources), ErrorMessageResourceName = "OutTimeCompareErrorMessage")]
        [DisplayFormat(DataFormatString = "{0:hh\\:mm tt}", ConvertEmptyStringToNull = true)]
        public TimeSpan? OutTime { get; set; }

        public DateTime? OutDateTime
        {
            get
            {
                return (OutTime != null ? Convert.ToDateTime(AttendanceDate) + (OutTime ?? TimeSpan.Zero) : (DateTime?)null);
            }
            set
            {
                OutTime = value != null && OutTime == null ? value.Value.TimeOfDay : OutTime;
            }
        }

        //public DateTime? OutDateTime { get; set; }

        //[RequiredIf("AttendanceStatusKey", DbConstants.AttendanceStatus.Present, ErrorMessageResourceType = typeof(ModelResources), ErrorMessageResourceName = "EmployeeAttendanceInTime_Required")]
        //public DateTime? InDateTime { get; set; }


        public DateTime? ShiftInTime { get; set; }

        public DateTime? ShiftOutTime { get; set; }

        public short TotalOvertime { get; set; }


        [StringLength(200, ErrorMessageResourceType = typeof(ModelResources), ErrorMessageResourceName = "EmployeeAttendanceRemarksLengthErrorMessage")]
        public string Remarks { get; set; }
        public string DepartmentName { get; set; }
        public bool? ClockInStatus { get; set; }

        public byte AttendanceConfigType { get; set; }
        public int? Duration { get; set; }
        public int? LateBy { get; set; }
        public int? EarlyBy { get; set; }
        public int? ShiftKey { get; set; }
        public int? OverTime { get; set; }
        public int? OverTimeE { get; set; }
        public bool IsHalfDay { get; set; }
        public bool? MissedInPunch { get; set; }
        public bool? MissedOutPunch { get; set; }
        public byte AttendancePresentStatusKey { get; set; }
        public string AttendancePresentStatusName { get; set; }
        public string AttendanceStatusRemarks { get; set; }

        public List<SelectListModel> Branches { get; set; }
        public List<SelectListModel> Departments { get; set; }
        public List<GroupSelectListModel> AttendanceStatuses { get; set; }
        public List<SelectListModel> LeaveTypes { get; set; }
        public List<GroupSelectListModel> Employees { get; set; }
        public DateTime? SearchFromDate { get; set; }
        [GreaterThanOrEqualTo("SearchFromDate", ErrorMessageResourceType = typeof(ModelResources), ErrorMessageResourceName = "GreaterThanFromDateErrorMessage")]
        public DateTime? SearchToDate { get; set; }
        public long TotalRecords { get; set; }
        public int PageIndex { get; set; }
        public int PageSize { get; set; }
        public string SortBy { get; set; }
        public string SortOrder { get; set; }
        public List<SelectListModel> EmployeeAttendanceStatus { get; set; }


    }
}
