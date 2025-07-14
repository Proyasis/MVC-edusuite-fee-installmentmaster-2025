using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CITS.EduSuite.Business.Models.Resources;
using CITS.Validations;

namespace CITS.EduSuite.Business.Models.ViewModels
{
    public class AttendanceConfigurationViewModel : BaseModel
    {
        public AttendanceConfigurationViewModel()
        {
            Companies = new List<SelectListModel>();
            Branches = new List<SelectListModel>();
            AttendanceConfigTypes = new List<SelectListModel>();
            //Shifts = new List<SelectListModel>();
            UnitTypes = new List<SelectListModel>();
            AttendanceConfigTypeKey = DbConstants.AttendanceConfigType.InOut;
            AutoApproval = false;
            //ShiftApplocationTypes = new List<SelectListModel>();
        }
        public int RowKey { get; set; }
        public short? CompanyKey { get; set; }
        public string CompanyName { get; set; }

        [Required(ErrorMessageResourceType = typeof(ModelResources), ErrorMessageResourceName = "RequiredErrorMessage")]
        [Display(Name = "Branch", ResourceType = typeof(EduSuiteUIResources))]
        public short? BranchKey { get; set; }
        public string BranchName { get; set; }

        [Required(ErrorMessageResourceType = typeof(ModelResources), ErrorMessageResourceName = "RequiredErrorMessage")]
        [Display(Name = "ConfigurationType", ResourceType = typeof(EduSuiteUIResources))]
        public byte AttendanceConfigTypeKey { get; set; }
        public string AttendanceConfigTypeName { get; set; }

        //[Required(ErrorMessageResourceType = typeof(ModelResources), ErrorMessageResourceName = "ShiftRequired")]
        //public int ShiftKey { get; set; }
        //public string ShiftName { get; set; }

        [Required(ErrorMessageResourceType = typeof(ModelResources), ErrorMessageResourceName = "RequiredErrorMessage")]
        [RegularExpression(@"^\d{0,10}(\.\d{1,2})?$", ErrorMessageResourceType = typeof(ModelResources), ErrorMessageResourceName = "Decimal10RegularExpressionErrorMessage")]
        [Range(1, 24, ErrorMessageResourceType = typeof(ModelResources), ErrorMessageResourceName = "TotalWorkingHoursRangeErrorMessage")]
        [Display(Name = "TotalWorkingHours", ResourceType = typeof(EduSuiteUIResources))]
        public decimal? TotalWorkingHours { get; set; }

        [RegularExpression(@"^\d{0,10}(\.\d{1,2})?$", ErrorMessageResourceType = typeof(ModelResources), ErrorMessageResourceName = "AmountExpressionErrorMessage")]
        [Display(Name = "OvertimeAdditionAmount", ResourceType = typeof(EduSuiteUIResources))]
        public decimal? OvertimeAdditionAmount { get; set; }

        public byte? UnitTypeKey { get; set; }
        public string UnitTypeName { get; set; }
        public bool AutoApproval { get; set; }
        public int? MinimmDifferencePunch { get; set; }

        [Required(ErrorMessageResourceType = typeof(ModelResources), ErrorMessageResourceName = "RequiredErrorMessage")]
        [RegularExpression(@"^\d{0,10}(\.\d{1,2})?$", ErrorMessageResourceType = typeof(ModelResources), ErrorMessageResourceName = "Decimal10RegularExpressionErrorMessage")]
        [Range(1, 31, ErrorMessageResourceType = typeof(ModelResources), ErrorMessageResourceName = "TotalWorkingHoursRangeErrorMessage")]
        [Display(Name = "TotalWorkingDays", ResourceType = typeof(EduSuiteUIResources))]
        public decimal? BaseDaysPerMonth { get; set; }
        public List<SelectListModel> Companies { get; set; }
        public List<SelectListModel> Branches { get; set; }
        public List<SelectListModel> AttendanceConfigTypes { get; set; }
        //public List<SelectListModel> Shifts { get; set; }
        //public List<SelectListModel> ShiftApplocationTypes { get; set; }
        public List<SelectListModel> UnitTypes { get; set; }
    }
}
