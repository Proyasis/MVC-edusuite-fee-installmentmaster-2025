using CITS.EduSuite.Business.Models.Resources;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CITS.EduSuite.Business.Models.ViewModels
{
    public class TimeTableTempMasterViewModel : BaseModel
    {

        public TimeTableTempMasterViewModel()
        {
            TimeTableTempDetailsModel = new List<TimeTableTempDetailsModel>();
            Employees = new List<SelectListModel>();
            ClassDetails = new List<SelectListModel>();
            WeeklyPeriods = new List<SelectListModel>();
            WeekDays = new List<SelectListModel>();
            Subjects = new List<SelectListModel>();
        }
        public long RowKey { get; set; }

        [Required(ErrorMessageResourceType = typeof(ModelResources), ErrorMessageResourceName = "RequiredErrorMessage")]
        [Display(Name = "FromDate", ResourceType = typeof(EduSuiteUIResources))]
        public DateTime? FromDate { get; set; }

        [Required(ErrorMessageResourceType = typeof(ModelResources), ErrorMessageResourceName = "RequiredErrorMessage")]
        [Display(Name = "ToDate", ResourceType = typeof(EduSuiteUIResources))]
        public DateTime? ToDate { get; set; }

        [Required(ErrorMessageResourceType = typeof(ModelResources), ErrorMessageResourceName = "RequiredErrorMessage")]
        [Display(Name = "Employee", ResourceType = typeof(EduSuiteUIResources))]
        public long? EmployeeKey { get; set; }
        public string EmployeeName { get; set; }
        public byte? Day { get; set; }
        public List<SelectListModel> Employees { get; set; }
        public List<TimeTableTempDetailsModel> TimeTableTempDetailsModel { get; set; }
        public List<SelectListModel> ClassDetails { get; set; }
        public List<SelectListModel> WeeklyPeriods { get; set; }
        public List<SelectListModel> WeekDays { get; set; }
        public List<SelectListModel> Subjects { get; set; }

        public short? BranchKey { get; set; }
        public string BranchName { get; set; }

    }

    public class TimeTableTempDetailsModel
    {
        public TimeTableTempDetailsModel()
        {
           
           
        }
        public long RowKey { get; set; }
        public long TempMasterKey { get; set; }
        public long? ClassDetailsKey { get; set; }
        public string ClassDetailsName { get; set; }
        public long? SubjectKey { get; set; }
        public string SubjectName { get; set; }
        public long? ToEmployeeKey { get; set; }
        public string EmployeeName { get; set; }
        public byte Day { get; set; }
        public byte PeriodKey { get; set; }
        public bool IsActive { get; set; }
        public DateTime? FromDate { get; set; }
        public DateTime? ToDate { get; set; }
      
    }
}
