using CITS.EduSuite.Business.Models.Resources;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CITS.EduSuite.Business.Models.ViewModels
{
    public class ApplicationFeeFollowupViewModel : BaseModel
    {
        public ApplicationFeeFollowupViewModel()
        {
            Branches = new List<SelectListModel>();
            ProcessStatuses = new List<SelectListModel>();
            StudentStatuses = new List<SelectListModel>();
            Universities = new List<SelectListModel>();
            Courses = new List<SelectListModel>();
            Batches = new List<SelectListModel>();
        }

        public long RowKey { get; set; }
        public long? ApplicationKey { get; set; }
        public DateTime FollowupDate { get; set; }
        public string Remarks { get; set; }
        public short ProcessStatusKey { get; set; }
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
        public string ProcessStatusName { get; set; }
        public string FeeTypes { get; set; }
        public List<SelectListModel> Branches { get; set; }
        public List<SelectListModel> ProcessStatuses { get; set; }
        public List<SelectListModel> StudentStatuses { get; set; }
        public List<SelectListModel> Universities { get; set; }
        public List<SelectListModel> Courses { get; set; }
        public List<SelectListModel> Batches { get; set; }
        public List<ApplicationFeeFollowupViewModel> LeadsModelList { get; set; }
    }

    public class ApplicationFeeFollowupDetailsViewModel : BaseModel
    {
        public ApplicationFeeFollowupDetailsViewModel()
        {
            Students = new List<SelectListModel>();
            ProcessStatuses = new List<SelectListModel>();
            FeeTypes = new List<SelectListModel>();
        }
        public long RowKey { get; set; }

        [Required(ErrorMessageResourceType = typeof(ModelResources), ErrorMessageResourceName = "RequiredErrorMessage")]
        [Display(Name = "Students", ResourceType = typeof(EduSuiteUIResources))]
        public long? ApplicationKey { get; set; }

        [Required(ErrorMessageResourceType = typeof(ModelResources), ErrorMessageResourceName = "RequiredErrorMessage")]
        [Display(Name = "Date", ResourceType = typeof(EduSuiteUIResources))]
        public DateTime? FollowupDate { get; set; }

        [Required(ErrorMessageResourceType = typeof(ModelResources), ErrorMessageResourceName = "RequiredErrorMessage")]
        [Display(Name = "FeeType", ResourceType = typeof(EduSuiteUIResources))]
        public List<short> FeeTypeKeys { get; set; }
        public string Remarks { get; set; }
        [Required(ErrorMessageResourceType = typeof(ModelResources), ErrorMessageResourceName = "RequiredErrorMessage")]
        [Display(Name = "Status", ResourceType = typeof(EduSuiteUIResources))]
        public short? ProcessStatusKey { get; set; }
        public string ProcessStatusName { get; set; }
        public bool IfExtendDate { get; set; }
        public DateTime? NextFollowUpDate { get; set; }
        public decimal? Amount { get; set; }
        public List<SelectListModel> Students { get; set; }
        public List<SelectListModel> FeeTypes { get; set; }
        public List<SelectListModel> ProcessStatuses { get; set; }
        public List<ApplicationFeeFollowupFeeTypes> ApplicationFeeFollowupFeeTypes { get; set; }
        public List<ApplicationFeeFollowupExtendDates> ApplicationFeeFollowupExtendDates { get; set; }
    }

    public class ApplicationFeeFollowupFeeTypes
    {
        public string FeeTypeName { get; set; }
    }
    public class ApplicationFeeFollowupExtendDates
    {
        public DateTime? OldFollowUpDate { get; set; }
        public DateTime? NextFollowUpDate { get; set; }
        public DateTime? DateAdded { get; set; }
        public string AddedBy { get; set; }
    }
}
