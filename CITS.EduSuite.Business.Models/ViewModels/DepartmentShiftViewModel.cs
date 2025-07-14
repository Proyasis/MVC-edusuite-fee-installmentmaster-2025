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
    public class DepartmentShiftViewModel : BaseModel
    {
        public DepartmentShiftViewModel()
        {
            Departments = new List<SelectListModel>();
            Shifts = new List<SelectListModel>();
            FromDate = DateTimeUTC.Now;
            ToDate = DateTimeUTC.Now;

        }
        public long RowKey { get; set; }


        [Required(ErrorMessageResourceType = typeof(ModelResources), ErrorMessageResourceName = "RequiredErrorMessage")]
        [Display(Name = "Department", ResourceType = typeof(EduSuiteUIResources))]
        public short DepartmentKey { get; set; }
        public string DepartmentName { get; set; }


        [Required(ErrorMessageResourceType = typeof(ModelResources), ErrorMessageResourceName = "RequiredErrorMessage")]
        [Display(Name = "Shift", ResourceType = typeof(EduSuiteUIResources))]
        public int ShiftKey { get; set; }
        public string ShiftName { get; set; }

        [Required(ErrorMessageResourceType = typeof(ModelResources), ErrorMessageResourceName = "RequiredErrorMessage")]
        [Display(Name = "FromDate", ResourceType = typeof(EduSuiteUIResources))]
        public DateTime FromDate { get; set; }
        [Required(ErrorMessageResourceType = typeof(ModelResources), ErrorMessageResourceName = "RequiredErrorMessage")]
        [Display(Name = "ToDate", ResourceType = typeof(EduSuiteUIResources))]
        public DateTime ToDate { get; set; }

        public long? RefKey { get; set; }

        public List<SelectListModel> Departments { get; set; }
        public List<SelectListModel> Shifts { get; set; }


    }
}
