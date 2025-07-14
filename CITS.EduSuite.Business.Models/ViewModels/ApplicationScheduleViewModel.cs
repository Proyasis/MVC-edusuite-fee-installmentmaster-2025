using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using CITS.EduSuite.Business.Models.Resources;
using CITS.Validations;
using System.Web.Mvc;

namespace CITS.EduSuite.Business.Models.ViewModels
{
    public class ApplicationScheduleViewModel : ApplicationScheduleDetailsViewModel
    {
        public ApplicationScheduleViewModel()
        {
            Branches = new List<SelectListModel>();
            ProcessStatuses = new List<SelectListModel>();
            StudentStatuses = new List<SelectListModel>();
            Universities = new List<SelectListModel>();
            Courses = new List<SelectListModel>();
            Batches = new List<SelectListModel>();
        }

        //public long RowKey { get; set; }
        //public long? ApplicationKey { get; set; }
        //public DateTime ReminderDate { get; set; }
        public string Remarks { get; set; }
        //public short ProcessStatusKey { get; set; }
        public string SearchFrom { get; set; }
        public string SearchTo { get; set; }
        public int SearchTabKey { get; set; }
        public int StartIndex { get; set; }
        public int PageSize { get; set; }
        public long TotalRecords { get; set; }
        public short? SearchBranchKey { get; set; }
        public short? SearchStudentStatusKey { get; set; }
        public short? SearchProcessStatusKey { get; set; }
        public short? SearchUniversityKey { get; set; }
        public short? SearchCourseKey { get; set; }
        public short? SearchBatchKey { get; set; }
        public string SearchAnyText { get; set; }
        public DateTime? SearchDateFrom { get; set; }
        public DateTime? SearchDateTo { get; set; }
        public int FetchKey { get; set; }
        public string Name { get; set; }
        public string AdmissionNo { get; set; }
        public string Mobile { get; set; }
        public string Course { get; set; }
        public string Affiliations { get; set; }
        public string Batch { get; set; }
        //public string ProcessStatusName { get; set; }
        public string ScheduleTypeName { get; set; }
        public string ApplicationCallStatusName { get; set; }
        public string CallTypeName { get; set; }
       // public TimeSpan? Duration { get; set; }
        public short? SearchCallTypeKey { get; set; }
        public short? SearchApplicationScheduleTypeKey { get; set; }
        public short? SearchApplicationCallStatusKey { get; set; }
        public List<SelectListModel> Branches { get; set; }
       // public List<SelectListModel> ProcessStatuses { get; set; }
        public List<SelectListModel> StudentStatuses { get; set; }
        public List<SelectListModel> Universities { get; set; }
        public List<SelectListModel> Courses { get; set; }
        public List<SelectListModel> Batches { get; set; }
        public List<ApplicationScheduleViewModel> LeadsModelList { get; set; }

    }
    public class ApplicationScheduleDetailsViewModel : BaseModel
    {
        public ApplicationScheduleDetailsViewModel()
        {
            Students = new List<SelectListModel>();
            ProcessStatuses = new List<SelectListModel>();
            CallTypes = new List<SelectListModel>();
            ApplicationScheduleTypes = new List<SelectListModel>();
            ApplicationCallStatus = new List<SelectListModel>();
        }
        public long RowKey { get; set; }

        [Required(ErrorMessageResourceType = typeof(ModelResources), ErrorMessageResourceName = "RequiredErrorMessage")]
        [Display(Name = "Students", ResourceType = typeof(EduSuiteUIResources))]
        public long? ApplicationKey { get; set; }

        [Required(ErrorMessageResourceType = typeof(ModelResources), ErrorMessageResourceName = "RequiredErrorMessage")]
        [Display(Name = "Date", ResourceType = typeof(EduSuiteUIResources))]
        public DateTime? ReminderDate { get; set; }     
        public TimeSpan? Duration { get; set; }
        public bool IsDuration { get; set; }
        public string Feedback { get; set; }

        [Required(ErrorMessageResourceType = typeof(ModelResources), ErrorMessageResourceName = "RequiredErrorMessage")]
        [Display(Name = "Status", ResourceType = typeof(EduSuiteUIResources))]
        public short? ProcessStatusKey { get; set; }
        public string ProcessStatusName { get; set; }

        [Required(ErrorMessageResourceType = typeof(ModelResources), ErrorMessageResourceName = "RequiredErrorMessage")]
        [Display(Name = "CallType", ResourceType = typeof(EduSuiteUIResources))]
        public byte? CallTypeKey { get; set; }

        [Required(ErrorMessageResourceType = typeof(ModelResources), ErrorMessageResourceName = "RequiredErrorMessage")]
        [Display(Name = "ScheduleType", ResourceType = typeof(EduSuiteUIResources))]
        public short? ApplicationScheduleTypeKey { get; set; }

        [Required(ErrorMessageResourceType = typeof(ModelResources), ErrorMessageResourceName = "RequiredErrorMessage")]
        [Display(Name = "CallStatus", ResourceType = typeof(EduSuiteUIResources))]
        public short? ApplicationCallStatusKey { get; set; }
        public List<SelectListModel> Students { get; set; }
        public List<SelectListModel> ProcessStatuses { get; set; }       
        public List<SelectListModel> CallTypes { get; set; }       
        public List<SelectListModel> ApplicationScheduleTypes { get; set; }
        public List<SelectListModel> ApplicationCallStatus { get; set; }
    }

}
