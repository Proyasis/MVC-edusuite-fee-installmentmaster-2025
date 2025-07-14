using CITS.EduSuite.Business.Models.Resources;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CITS.Validations;

namespace CITS.EduSuite.Business.Models.ViewModels
{
    public class ShiftViewModel : BaseModel
    {

        public ShiftViewModel()
        {
            WeekDays = new List<SelectListModel>();
            ShiftBreaks = new List<ShiftBreakModel>();

        }
        public int MasterRowKey { get; set; }


        [RegularExpression(@"^[0-9 a-zA-Z *()&-/]+$", ErrorMessageResourceType = typeof(ModelResources), ErrorMessageResourceName = "RegularExpressionErrorMessage")]
        [Required(ErrorMessageResourceType = typeof(ModelResources), ErrorMessageResourceName = "RequiredErrorMessage")]
        [StringLength(5, ErrorMessageResourceType = typeof(ModelResources), ErrorMessageResourceName = "LengthErrorMessage")]
        [System.Web.Mvc.Remote("CheckShiftCodeExists", "Shift", AdditionalFields = "MasterRowKey", ErrorMessageResourceType = typeof(ModelResources), ErrorMessageResourceName = "ExistsErrorMessage")]
        [Display(Name = "Code", ResourceType = typeof(EduSuiteUIResources))]
        public string ShiftCode { get; set; }

        [RegularExpression(@"^[0-9 a-zA-Z *()&-/]+$", ErrorMessageResourceType = typeof(ModelResources), ErrorMessageResourceName = "RegularExpressionErrorMessage")]
        [Required(ErrorMessageResourceType = typeof(ModelResources), ErrorMessageResourceName = "RequiredErrorMessage")]
        [StringLength(150, ErrorMessageResourceType = typeof(ModelResources), ErrorMessageResourceName = "LengthErrorMessage")]
        [Display(Name = "Name", ResourceType = typeof(EduSuiteUIResources))]

        public string ShiftName { get; set; }

        [Required(ErrorMessageResourceType = typeof(ModelResources), ErrorMessageResourceName = "RequiredErrorMessage")]
        [Display(Name = "BeginTime", ResourceType = typeof(EduSuiteUIResources))]
        public TimeSpan? BeginTime { get; set; }

        [Required(ErrorMessageResourceType = typeof(ModelResources), ErrorMessageResourceName = "RequiredErrorMessage")]
        [Display(Name = "EndTime", ResourceType = typeof(EduSuiteUIResources))]
        //[GreaterThan("BeginTime", PassOnNull = true, ErrorMessageResourceType = typeof(ModelResources), ErrorMessageResourceName = "EndTimeLessThanErrorMessage")]
        public TimeSpan? EndTime { get; set; }

        //public bool IfBreak1 { get; set; }

        //[RequiredIf("IfBreak1", true, ErrorMessageResourceType = typeof(ModelResources), ErrorMessageResourceName = "BeginTimeRequired")]
        //public TimeSpan? Break1BeginTime { get; set; }

        //[RequiredIf("IfBreak1", true, ErrorMessageResourceType = typeof(ModelResources), ErrorMessageResourceName = "EndTimeRequired")]
        ////[GreaterThan("Break1BeginTime", PassOnNull = true, ErrorMessageResourceType = typeof(ModelResources), ErrorMessageResourceName = "EndTimeLessThanErrorMessage")]
        //public TimeSpan? Break1EndTime { get; set; }

        //public bool IfBreak2 { get; set; }

        //[RequiredIf("IfBreak2", true, ErrorMessageResourceType = typeof(ModelResources), ErrorMessageResourceName = "BeginTimeRequired")]
        //public TimeSpan? Break2BeginTime { get; set; }

        //[RequiredIf("IfBreak2", true, ErrorMessageResourceType = typeof(ModelResources), ErrorMessageResourceName = "EndTimeRequired")]
        ////[GreaterThan("Break2BeginTime", PassOnNull = true, ErrorMessageResourceType = typeof(ModelResources), ErrorMessageResourceName = "EndTimeLessThanErrorMessage")]
        //public TimeSpan? Break2EndTime { get; set; }
        public int? PunchBeginBefore { get; set; }
        public int? PunchEndAfter { get; set; }
        public int? GraceTime { get; set; }
        public bool PartialDay { get; set; }

        [RequiredIf("PartialDay", true, ErrorMessageResourceType = typeof(ModelResources), ErrorMessageResourceName = "RequiredErrorMessage")]
        [Display(Name = "PartialDay", ResourceType = typeof(EduSuiteUIResources))]
        public byte? PartialDayKey { get; set; }

        [RequiredIf("PartialDay", true, ErrorMessageResourceType = typeof(ModelResources), ErrorMessageResourceName = "RequiredErrorMessage")]
        [Display(Name = "PartialDayBeginTime", ResourceType = typeof(EduSuiteUIResources))]
        public TimeSpan? PartialDayBeginTime { get; set; }

        [RequiredIf("PartialDay", true, ErrorMessageResourceType = typeof(ModelResources), ErrorMessageResourceName = "RequiredErrorMessage")]
        [Display(Name = "PartialDayEndTime", ResourceType = typeof(EduSuiteUIResources))]
        //[GreaterThan("PartialDayBeginTime", PassOnNull = true, ErrorMessageResourceType = typeof(ModelResources), ErrorMessageResourceName = "EndTimeLessThanErrorMessage")]
        public TimeSpan? PartialDayEndTime { get; set; }
        public short? RefKey { get; set; }

        public bool IsActive { get; set; }

        public List<SelectListModel> WeekDays { get; set; }
        public List<ShiftBreakModel> ShiftBreaks { get; set; }

    }
    public class ShiftBreakModel : BaseModel
    {
        public long RowKey { get; set; }
        [System.Web.Mvc.Remote("CheckShiftBreakExists", "Shift", AdditionalFields = "RowKey", ErrorMessageResourceType = typeof(ModelResources), ErrorMessageResourceName = "ExistsErrorMessage")]
        [Display(Name = "BreakName", ResourceType = typeof(EduSuiteUIResources))]
        public string BreakName { get; set; }
        [RequiredIfNot("BreakName", null, ErrorMessageResourceType = typeof(ModelResources), ErrorMessageResourceName = "RequiredErrorMessage")]
        [Display(Name = "BreakBeginTime", ResourceType = typeof(EduSuiteUIResources))]
        public TimeSpan? BreakBeginTime { get; set; }

        [RequiredIfNot("BreakName", null, ErrorMessageResourceType = typeof(ModelResources), ErrorMessageResourceName = "RequiredErrorMessage")]
        [Display(Name = "BreakEndTime", ResourceType = typeof(EduSuiteUIResources))]
        public TimeSpan? BreakEndTime { get; set; }
    }
}
