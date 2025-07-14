using CITS.EduSuite.Business.Models.Resources;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CITS.EduSuite.Business.Models.ViewModels
{
    public class AttendanceTypeMasterViewModel : BaseModel
    {
        public AttendanceTypeMasterViewModel()
        {
            ÇlassDetails = new List<SelectListModel>();
            AttendanceTypes = new List<SelectListModel>();
            AttendanceTypeDetailsModel = new List<AttendanceTypeDetailsModel>();
        }
        public short RowKey { get; set; }

        [Required(ErrorMessageResourceType = typeof(ModelResources), ErrorMessageResourceName = "RequiredErrorMessage")]
        [Display(Name = "FromDate", ResourceType = typeof(EduSuiteUIResources))]
        public DateTime? FromDate { get; set; }

        [Required(ErrorMessageResourceType = typeof(ModelResources), ErrorMessageResourceName = "RequiredErrorMessage")]
        [Display(Name = "ToDate", ResourceType = typeof(EduSuiteUIResources))]
        public DateTime? ToDate { get; set; }

        [Required(ErrorMessageResourceType = typeof(ModelResources), ErrorMessageResourceName = "RequiredErrorMessage")]
        [Display(Name = "ClassCode", ResourceType = typeof(EduSuiteUIResources))]
        public List<long> ClassKeys { get; set; }
        public string Classess { get; set; }
        public List<SelectListModel> ÇlassDetails { get; set; }
        public List<SelectListModel> AttendanceTypes { get; set; }
        public List<AttendanceTypeDetailsModel> AttendanceTypeDetailsModel { get; set; }
    }

    public class AttendanceTypeDetailsModel
    {
        public AttendanceTypeDetailsModel()
        {

        }
        public short RowKey { get; set; }

        [System.Web.Mvc.Remote("CheckAttendanceTypeExists", "AttendanceTypeMaster", ErrorMessageResourceType = typeof(ModelResources), ErrorMessageResourceName = "ExistsErrorMessage")]
        [Display(Name = "AttendanceType", ResourceType = typeof(EduSuiteUIResources))]
        public short AttendanceTypeKey { get; set; }
        public short AttendanceTypeMasterKey { get; set; }
        public TimeSpan? StartTime { get; set; }
        public TimeSpan? EndTime { get; set; }
        public int? GraceTime { get; set; }



    }
}
