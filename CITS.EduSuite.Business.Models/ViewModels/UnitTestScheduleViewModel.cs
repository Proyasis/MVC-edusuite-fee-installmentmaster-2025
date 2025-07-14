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
    public class UnitTestScheduleViewModel : BaseModel
    {
        public UnitTestScheduleViewModel()
        {
            CourseTypes = new List<SelectListModel>();        
            Batches = new List<SelectListModel>();
            Branches = new List<SelectListModel>();
            Subjects = new List<SelectListModel>();
            SubjectModules = new List<SelectListModel>();
            ModuleTopics = new List<SelectListModel>();
            ClassDetails = new List<SelectListModel>();
            ModuleTopicKeys = new List<long?>();
            ExamDate = DateTimeUTC.Now;
            Teachers = new List<SelectListModel>();
        }

        public List<SelectListModel> CourseTypes { get; set; }
        public List<SelectListModel> Subjects { get; set; }
        public List<SelectListModel> SubjectModules { get; set; }
        public List<SelectListModel> ModuleTopics { get; set; }
        public List<SelectListModel> Batches { get; set; }
        public List<SelectListModel> Branches { get; set; }
        public List<SelectListModel> ClassDetails { get; set; }
        public List<SelectListModel> Teachers { get; set; }

        [Required(ErrorMessageResourceType = typeof(ModelResources), ErrorMessageResourceName = "ModuleTopicRequired")]
        public List<long?> ModuleTopicKeys { get; set; }
        public long ModuleTopicKey { get; set; }

        [Required(ErrorMessageResourceType = typeof(ModelResources), ErrorMessageResourceName = "ClassDetailsRequired")]
        public long ClassDetailsKey { get; set; }
        public string ClassDetailsName { get; set; }

        public List<InternalExamDetailsModel> InternalExamDetails { get; set; }

        [Required(ErrorMessageResourceType = typeof(ModelResources), ErrorMessageResourceName = "BranchRequired")]
        public short? BranchKey { get; set; }
        public string BranchName { get; set; }

        [Required(ErrorMessageResourceType = typeof(ModelResources), ErrorMessageResourceName = "SubjectRequired")]
        public long SubjectKey { get; set; }
        public string SubjectName { get; set; }

        [Required(ErrorMessageResourceType = typeof(ModelResources), ErrorMessageResourceName = "SubjectModuleRequired")]
        public long SubjectModuleKey { get; set; }
        public string SubjectModuleName { get; set; }
        
        
        [Required(ErrorMessageResourceType = typeof(ModelResources), ErrorMessageResourceName = "CourseTypeRequired")]
        public short CourseTypeKey { get; set; }

        [Required(ErrorMessageResourceType = typeof(ModelResources), ErrorMessageResourceName = "BatchRequired")]
        public short BatchKey { get; set; }
        public string BatchName { get; set; }
        public long RowKey { get; set; }

        public bool? IfEdit { get; set; }
        public bool IsUpdate { get; set; }


        [Required(ErrorMessageResourceType = typeof(ModelResources), ErrorMessageResourceName = "MinimumMarkRequired")]
        public decimal? MinimumMark { get; set; }

        [Required(ErrorMessageResourceType = typeof(ModelResources), ErrorMessageResourceName = "MaximumMarkRequired")]
        [GreaterThan("MinimumMark", PassOnNull = true, ErrorMessageResourceType = typeof(ModelResources), ErrorMessageResourceName = "MaximumMarkLessThanErrorMessage")]
        public decimal? MaximumMark { get; set; }
        public DateTime ExamDate { get; set; }
        public int? ModuleTopicsCount { get; set; }
        public List<UnitTestResultViewModel> UnitTestResultViewModel { get; set; }
        public string searchText { get; set; }
        public long? EmployeeKey { get; set; }
        public DateTime? SearchDate { get; set; }
        public int PageIndex { get; set; }
        public int PageSize { get; set; }
        public string SortBy { get; set; }
        public string SortOrder { get; set; }
    }

    public class UnitTestResultViewModel
    {
        public long RowKey { get; set; }

        public string AdmissionNo { get; set; }
        public string RollNoCode { get; set; }
        public string StudentName { get; set; }
        public string ApplicantPhoto { get; set; }
        public long ApplicationKey { get; set; }
        public long UnitTestScheduleKey { get; set; }
        public string ResultStatus { get; set; }

        [RequiredIf("AbsentStatus", false, ErrorMessageResourceType = typeof(ModelResources), ErrorMessageResourceName = "MaximumMarkRequired")]
        public decimal? Mark { get; set; }
        public string Remarks { get; set; }
        public bool AbsentStatus { get; set; }
        public decimal? MinimumMarks { get; set; }
        public decimal? MaximumMarks { get; set; }
        public string TopicNames { get; set; }

        public decimal? MinimumMark { get; set; }
        public decimal? MaximumMark { get; set; }
        public DateTime? ExamDate { get; set; }
        public string SubjectName { get; set; }
    }

}
