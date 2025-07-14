using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CITS.EduSuite.Business.Models.Resources;
using CITS.Validations;
using CITS.EduSuite.Business.Models.Security;
using System.Threading;

namespace CITS.EduSuite.Business.Models.ViewModels
{
    public class ExamScheduleViewModel : BaseModel
    {

        public ExamScheduleViewModel()
        {
            Batches = new List<SelectListModel>();
            CourseTypes = new List<SelectListModel>();
            Courses = new List<SelectListModel>();
            ExamScheduleDetailsModel = new List<ExamScheduleDetailsModel>();
            Universities = new List<SelectListModel>();
            CourseYears = new List<SelectListModel>();
            AcademicTerms = new List<SelectListModel>();
            Subjects = new List<SelectListModel>();
            ExamCenter = new List<SelectListModel>();
            ExamTerm = new List<SelectListModel>();
            Branches = new List<SelectListModel>();
            ExamDate = DateTimeUTC.Now;

        }
        public long? ApplicationKey { get; set; }

        [RequiredIfTrue("IsMultipleExamSchedule", ErrorMessageResourceType = typeof(ModelResources), ErrorMessageResourceName = "SelectSubject")]
        public long? SubjectKey { get; set; }
        public string SubjectName { get; set; }

        [Required(ErrorMessageResourceType = typeof(ModelResources), ErrorMessageResourceName = "ScheduleDateRequired")]
        public DateTime ExamDate { get; set; }

        [RequiredIfTrue("IsMultipleExamSchedule", ErrorMessageResourceType = typeof(ModelResources), ErrorMessageResourceName = "ExamTermRequired")]
        public short ExamTermKey { get; set; }
        public string ExamTermName { get; set; }

        [RegularExpression(@"^(100(\.[0]{1,2})?|[0-9]{1,2}(\.[0-9]{1,2})?)$", ErrorMessageResourceType = typeof(ModelResources), ErrorMessageResourceName = "RegularExpressionMarkMessage")]
        [RequiredIfTrue("IsMultipleExamSchedule", ErrorMessageResourceType = typeof(ModelResources), ErrorMessageResourceName = "MaximumMarkRequired")]
        //[GreaterThan("MinimumMark", PassOnNull = true, ErrorMessageResourceType = typeof(ModelResources), ErrorMessageResourceName = "MaximumMarkLessThanErrorMessage")]
        [Display(Name = "MaximumMark", ResourceType = typeof(EduSuiteUIResources))]
        public decimal? MaximumMark { get; set; }

        [RegularExpression(@"^(100(\.[0]{1,2})?|[0-9]{1,2}(\.[0-9]{1,2})?)$", ErrorMessageResourceType = typeof(ModelResources), ErrorMessageResourceName = "RegularExpressionMarkMessage")]
        [RequiredIfTrue("IsMultipleExamSchedule", ErrorMessageResourceType = typeof(ModelResources), ErrorMessageResourceName = "MinimumMarkRequired")]
        [Display(Name = "MinimumMark", ResourceType = typeof(EduSuiteUIResources))]
        public decimal? MinimumMark { get; set; }

        public TimeSpan? ExamStartTime { get; set; }
        public TimeSpan? ExamEndTime { get; set; }

        [Required(ErrorMessageResourceType = typeof(ModelResources), ErrorMessageResourceName = "AttendanceYearRequired")]
        public short? CourseYear { get; set; }
      
        public string CourseYearName { get; set; }
        public short? CurrentYear { get; set; }

        [Required(ErrorMessageResourceType = typeof(ModelResources), ErrorMessageResourceName = "BranchRequired")]
        public short? BranchKey { get; set; }
        public string BranchName { get; set; }

        [Required(ErrorMessageResourceType = typeof(ModelResources), ErrorMessageResourceName = "CourseTypeRequired")]
        public short? CourseTypeKey { get; set; }


        [Required(ErrorMessageResourceType = typeof(ModelResources), ErrorMessageResourceName = "UniversityKeyRequired")]
        public short? UniversityMasterKey { get; set; }
        public string UniversityName { get; set; }

        [Required(ErrorMessageResourceType = typeof(ModelResources), ErrorMessageResourceName = "CourseKeyRequired")]
        public long? CourseKey { get; set; }
        public string CourseName { get; set; }

        [Required(ErrorMessageResourceType = typeof(ModelResources), ErrorMessageResourceName = "SyllabusRequired")]
        public short? AcademicTermKey { get; set; }
        public string AcademicTermName { get; set; }

        [Required(ErrorMessageResourceType = typeof(ModelResources), ErrorMessageResourceName = "BatchRequired")]
        public short? BatchKey { get; set; }
        public string BatchName { get; set; }
        
        public bool IsApplied { get; set; }
        public int PageIndex { get; set; }
        public int PageSize { get; set; }
        public long TotalRecords { get; set; }




        public int? CourseDuration { get; set; }
        public int? NoOfStudents { get; set; }
        public short? ExamCenterKeyAll { get; set; }
        public short? ExamTermKeyAll { get; set; }
        public string ExamRegisterNumber { get; set; }
        public string StudentName { get; set; }
        public string StudentEmail { get; set; }
        public string StudentPhone { get; set; }
        public bool IsMultipleExamSchedule { get; set; }
        public string SearchText { get; set; }
        public string SortBy { get; set; }
        public string SortOrder { get; set; }


        public List<ExamScheduleDetailsModel> ExamScheduleDetailsModel { get; set; }
        public List<SelectListModel> CourseTypes { get; set; }
        public List<SelectListModel> Courses { get; set; }
        public List<SelectListModel> Universities { get; set; }
        public List<SelectListModel> Batches { get; set; }
        public List<SelectListModel> CourseYears { get; set; }
        public List<SelectListModel> AcademicTerms { get; set; }
        public List<SelectListModel> Subjects { get; set; }
        public List<SelectListModel> ExamCenter { get; set; }
        public List<SelectListModel> ExamTerm { get; set; }
        public List<SelectListModel> Branches { get; set; }


       
    }
    public class ExamScheduleDetailsModel
    {

        public ExamScheduleDetailsModel()
        {
            ExamCenter = new List<SelectListModel>();
            ExamTerm = new List<SelectListModel>();
            ExamDate = DateTimeUTC.Now;

        }

        public long RowKey { get; set; }
        public long ApplicationKey { get; set; }
        //public long ExamScheduleMasterKey { get; set; }
        public long? SubjectKey { get; set; }
        public string SubjectName { get; set; }

        public string ExamRegisterNumber { get; set; }

        [RequiredIfTrue("IsActive", ErrorMessageResourceType = typeof(ModelResources), ErrorMessageResourceName = "ExamCentreRequired")]
        public short? ExamCenterKey { get; set; }
        public string ExamCenterName { get; set; }

        public int? AppearenceCount { get; set; }
        public bool IsActive { get; set; }
        public int DataPageIndex { get; set; }
        public string AdmissionNo { get; set; }
        public string StudentEnrollmentNo { get; set; }
        public string StudentName { get; set; }
        public string CourseName { get; set; }
        public string UniversityName { get; set; }
        public string BatchName { get; set; }
        public string Remarks { get; set; }

        [Required(ErrorMessageResourceType = typeof(ModelResources), ErrorMessageResourceName = "ScheduleDateRequired")]
        public DateTime ExamDate { get; set; }

        [Required(ErrorMessageResourceType = typeof(ModelResources), ErrorMessageResourceName = "ExamTermRequired")]
        public short ExamTermKey { get; set; }
        public string ExamTermName { get; set; }

        [RegularExpression(@"^(100(\.[0]{1,2})?|[0-9]{1,2}(\.[0-9]{1,2})?)$", ErrorMessageResourceType = typeof(ModelResources), ErrorMessageResourceName = "RegularExpressionMarkMessage")]
        [RequiredIfTrue("IsActive", ErrorMessageResourceType = typeof(ModelResources), ErrorMessageResourceName = "MaximumMarkRequired")]
        //[GreaterThan("MinimumMark", PassOnNull = true, ErrorMessageResourceType = typeof(ModelResources), ErrorMessageResourceName = "MaximumMarkLessThanErrorMessage")]
        [Display(Name = "MaximumMark", ResourceType = typeof(EduSuiteUIResources))]
        public decimal? MaximumMark { get; set; }

        [RegularExpression(@"^(100(\.[0]{1,2})?|[0-9]{1,2}(\.[0-9]{1,2})?)$", ErrorMessageResourceType = typeof(ModelResources), ErrorMessageResourceName = "RegularExpressionMarkMessage")]
        [RequiredIfTrue("IsActive", ErrorMessageResourceType = typeof(ModelResources), ErrorMessageResourceName = "MinimumMarkRequired")]
        [Display(Name = "MinimumMark", ResourceType = typeof(EduSuiteUIResources))]
        public decimal? MinimumMark { get; set; }

        public TimeSpan? ExamStartTime { get; set; }
        public TimeSpan? ExamEndTime { get; set; }


        public List<SelectListModel> ExamCenter { get; set; }

        public List<SelectListModel> ExamTerm { get; set; }
        public string ExamAttempName { get; set; }
        public string AppearenceCountText { get; set; }
        public byte? ExamStatus { get; set; }
        public short? SubjectYear { get; set; }
        public string SubjectYearName { get; set; }
        public short? AcademicTermKey { get; set; }
        public int? CourseDuration { get; set; }
        public string CurrentYearText { get; set; }
        public short? CurrentYear { get; set; }

    }

    public class ExamScheduleSummary
    {
        public string SubjectName { get; set; }
        public short? SubjectYear { get; set; }
        public string SubjectYearName { get; set; }
        public short? AcademicTermKey { get; set; }
        public int? CourseDuration { get; set; }
        public string ExamAttempName { get; set; }
        public decimal? MaximumMark { get; set; }

        public decimal? MinimumMark { get; set; }
        public decimal? Mark { get; set; }
        public string ExamTermName { get; set; }
        public string ExamRegisterNumber { get; set; }
        
        public DateTime? ExamDate { get; set; }

        public TimeSpan? ExamStartTime { get; set; }
        public TimeSpan? ExamEndTime { get; set; }

        public string ExamStatus { get; set; }
        public string AppearenceCountText { get; set; }
        public string StudentEnrollmentNo { get; set; }
        public int? AppearenceCount { get; set; }
        public string ResultStatus { get; set; }
        public string AppliedStatus { get; set; }


    }
}
