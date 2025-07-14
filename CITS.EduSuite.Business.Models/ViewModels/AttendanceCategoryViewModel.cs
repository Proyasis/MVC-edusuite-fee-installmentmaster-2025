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
    public class AttendanceCategoryViewModel : BaseModel
    {
        public AttendanceCategoryViewModel()
        {
            OverTimeFormulaType = new List<SelectListModel>();
            AttendanceCategoryWeekOffs = new List<AttendanceCategoryWeekOffModel>();
            AbsenseDayTypes = new List<SelectListModel>();
            WeekDays = new List<SelectListModel>();
        }
        public short MasterRowKey { get; set; }

        [RegularExpression(@"^[0-9 a-zA-Z *()&-/]+$", ErrorMessageResourceType = typeof(ModelResources), ErrorMessageResourceName = "RegularExpressionErrorMessage")]
        [Required(ErrorMessageResourceType = typeof(ModelResources), ErrorMessageResourceName = "RequiredErrorMessage")]
        [StringLength(5, ErrorMessageResourceType = typeof(ModelResources), ErrorMessageResourceName = "LengthErrorMessage")]
        [System.Web.Mvc.Remote("CheckAttendanceCategoryCodeExists", "AttendanceCategory", AdditionalFields = "MasterRowKey", ErrorMessageResourceType = typeof(ModelResources), ErrorMessageResourceName = "ExistsErrorMessage")]
        [Display(Name = "Code", ResourceType = typeof(EduSuiteUIResources))]
        public string AttendanceCategoryCode { get; set; }

        [RegularExpression(@"^[0-9 a-zA-Z *()&-/]+$", ErrorMessageResourceType = typeof(ModelResources), ErrorMessageResourceName = "RegularExpressionErrorMessage")]
        [Required(ErrorMessageResourceType = typeof(ModelResources), ErrorMessageResourceName = "RequiredErrorMessage")]
        [StringLength(150, ErrorMessageResourceType = typeof(ModelResources), ErrorMessageResourceName = "LengthErrorMessage")]
        [Display(Name = "Name", ResourceType = typeof(EduSuiteUIResources))]
        public string AttendanceCategoryName { get; set; }

        [Required(ErrorMessageResourceType = typeof(ModelResources), ErrorMessageResourceName = "RequiredErrorMessage")]
        [Display(Name = "OverTimeFormula", ResourceType = typeof(EduSuiteUIResources))]
        public byte? OverTimeFormulaKey { get; set; }

        [RegularExpression(@"^([1-9][0-9]{0,2}|1000)$", ErrorMessageResourceType = typeof(ModelResources), ErrorMessageResourceName = "PleaseEnterValidTime")]
        [RequiredIfNot("OverTimeFormulaKey", DbConstants.OverTimeFormulaType.OverTimeNotApplicable, ErrorMessageResourceType = typeof(ModelResources), ErrorMessageResourceName = "RequiredErrorMessage")]
        [Display(Name = "MinOvertime", ResourceType = typeof(EduSuiteUIResources))]
        public int? MinOvertime { get; set; }

        [RegularExpression(@"^([1-9][0-9]{0,2}|1000)$", ErrorMessageResourceType = typeof(ModelResources), ErrorMessageResourceName = "PleaseEnterValidTime")]
        
        public int? MaxOvertime { get; set; }

        public bool ConsiderFirstLastPunch { get; set; }

        [RegularExpression(@"^([1-9][0-9]{0,2}|1000)$", ErrorMessageResourceType = typeof(ModelResources), ErrorMessageResourceName = "PleaseEnterValidTime")]
        
        public int? LateComingGraceTime { get; set; }

        [RegularExpression(@"^([1-9][0-9]{0,2}|1000)$", ErrorMessageResourceType = typeof(ModelResources), ErrorMessageResourceName = "PleaseEnterValidTime")]
       
        public int? EarlyGoingGraceTime { get; set; }
        public bool ConsiderEarlyPunch { get; set; }
        public bool ConsiderLatePunch { get; set; }
        public bool DeductBrakeHours { get; set; }
        public bool CalcuteHalfDay { get; set; }

        [RegularExpression(@"^([1-9][0-9]{0,2}|1000)$", ErrorMessageResourceType = typeof(ModelResources), ErrorMessageResourceName = "PleaseEnterValidTime")]
        [RequiredIf("CalcuteHalfDay", true, ErrorMessageResourceType = typeof(ModelResources), ErrorMessageResourceName = "RequiredErrorMessage")]
        [Display(Name = "HalfDayMins", ResourceType = typeof(EduSuiteUIResources))]
        public int? HalfDayMins { get; set; }
        public bool CalculateAbsent { get; set; }

        [RegularExpression(@"^([1-9][0-9]{0,2}|1000)$", ErrorMessageResourceType = typeof(ModelResources), ErrorMessageResourceName = "PleaseEnterValidTime")]
        [RequiredIf("CalculateAbsent", true, ErrorMessageResourceType = typeof(ModelResources), ErrorMessageResourceName = "RequiredErrorMessage")]
        [Display(Name = "AbsentMins", ResourceType = typeof(EduSuiteUIResources))]
        public int? AbsentMins { get; set; }
        public bool PartCalculateHalfDay { get; set; }

        [RegularExpression(@"^([1-9][0-9]{0,2}|1000)$", ErrorMessageResourceType = typeof(ModelResources), ErrorMessageResourceName = "PleaseEnterValidTime")]
        [RequiredIf("PartCalculateHalfDay", true, ErrorMessageResourceType = typeof(ModelResources), ErrorMessageResourceName = "RequiredErrorMessage")]
        [Display(Name = "PartHalfDayMins", ResourceType = typeof(EduSuiteUIResources))]
        public int? PartHalfDayMins { get; set; }
        public bool PartCalculateAbsent { get; set; }

        [RegularExpression(@"^([1-9][0-9]{0,2}|1000)$", ErrorMessageResourceType = typeof(ModelResources), ErrorMessageResourceName = "PleaseEnterValidTime")]
        [RequiredIf("PartCalculateAbsent", true, ErrorMessageResourceType = typeof(ModelResources), ErrorMessageResourceName = "RequiredErrorMessage")]
        [Display(Name = "PartAbsentMins", ResourceType = typeof(EduSuiteUIResources))]
        public int? PartAbsentMins { get; set; }
        public bool WOandHAsPreDayAbsent { get; set; }
        public bool WOandHAsSufDayAbsent { get; set; }
        public bool WOandHAsBothDayAbsent { get; set; }
        public bool MarkAsAbsentForLate { get; set; }

        [RegularExpression(@"^[1-9]$|^10$", ErrorMessageResourceType = typeof(ModelResources), ErrorMessageResourceName = "PleaseEnterValidDayCount")]
        [RequiredIf("MarkAsAbsentForLate", true, ErrorMessageResourceType = typeof(ModelResources), ErrorMessageResourceName = "RequiredErrorMessage")]
        [Display(Name = "ContinuousLateDay", ResourceType = typeof(EduSuiteUIResources))]
        public int? ContiousLateDay { get; set; }

        [RequiredIf("MarkAsAbsentForLate", true, ErrorMessageResourceType = typeof(ModelResources), ErrorMessageResourceName = "RequiredErrorMessage")]
        [Display(Name = "AbsentDayType", ResourceType = typeof(EduSuiteUIResources))]
        public byte? AbsentDayType { get; set; }
        public bool MarkHalfdayForLaterGoing { get; set; }

        [RegularExpression(@"^([1-9][0-9]{0,2}|1000)$", ErrorMessageResourceType = typeof(ModelResources), ErrorMessageResourceName = "PleaseEnterValidTime")]
        [RequiredIf("MarkHalfdayForLaterGoing", true, ErrorMessageResourceType = typeof(ModelResources), ErrorMessageResourceName = "RequiredErrorMessage")]
        [Display(Name = "HalfDayLateByMins", ResourceType = typeof(EduSuiteUIResources))]
        public int? HalfDayLateByMins { get; set; }
        public bool MarkHalfdayForEarlyGoing { get; set; }

        [RegularExpression(@"^([1-9][0-9]{0,2}|1000)$", ErrorMessageResourceType = typeof(ModelResources), ErrorMessageResourceName = "PleaseEnterValidTime")]
        [RequiredIf("MarkHalfdayForEarlyGoing", true, ErrorMessageResourceType = typeof(ModelResources), ErrorMessageResourceName = "RequiredErrorMessage")]
        [Display(Name = "HalfDayEarlyByMins", ResourceType = typeof(EduSuiteUIResources))]
        public int? HalfDayEarlyByMins { get; set; }
        //public byte? WeekOffDay1Key { get; set; }
        //public byte? WeekOffDay2Key { get; set; }
        //public byte? WeekOffDayWeekKeys { get; set; }

        public int? RefKey { get; set; }
        public bool IsActive { get; set; }
        public string IsActiveText { get { return IsActive ? EduSuiteUIResources.Yes : EduSuiteUIResources.No; } }
        public List<SelectListModel> OverTimeFormulaType { get; set; }
        public List<AttendanceCategoryWeekOffModel> AttendanceCategoryWeekOffs { get; set; }
        public List<SelectListModel> WeekDays { get; set; }       
        public List<SelectListModel> AbsenseDayTypes { get; set; }


    }

    public class AttendanceCategoryWeekOffModel : BaseModel
    {
        public AttendanceCategoryWeekOffModel()
        {
            Weeks = new List<SelectListModel>();
        }
        public int RowKey { get; set; }
        [Required(ErrorMessageResourceType = typeof(ModelResources), ErrorMessageResourceName = "RequiredErrorMessage")]
        [System.Web.Mvc.Remote("CheckWeekOffDayExists", "AttendanceCategory", AdditionalFields = "RowKey", ErrorMessageResourceType = typeof(ModelResources), ErrorMessageResourceName = "ExistsErrorMessage")]
        [Display(Name = "WeekOffDay", ResourceType = typeof(EduSuiteUIResources))]
        public byte? WeekOffDayKey { get; set; }
        public string WeekOffDayWeekKeysText { get; set; }
        public List<byte> WeekOffDayWeekKeys { get; set; }
        public List<SelectListModel> Weeks { get; set; }

    }
}