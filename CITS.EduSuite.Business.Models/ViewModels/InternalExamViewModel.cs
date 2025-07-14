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
    public class InternalExamViewModel : BaseModel
    {
        public InternalExamViewModel()
        {
            CourseTypes = new List<SelectListModel>();
            Courses = new List<SelectListModel>();
            Universities = new List<SelectListModel>();
            AcademicTerms = new List<SelectListModel>();
            CourseYears = new List<SelectListModel>();
            Batches = new List<SelectListModel>();
            Branches = new List<SelectListModel>();
            InternalExamTerm = new List<SelectListModel>();
            InternalExamDetails = new List<InternalExamDetailsModel>();
            ClassDetails = new List<SelectListModel>();
            ClassDetailsKeys = new List<long?>();
            StudetMailDetails = new List<StudetMailDetails>();

        }

        public List<SelectListModel> CourseTypes { get; set; }
        public List<SelectListModel> Courses { get; set; }
        public List<SelectListModel> Universities { get; set; }
        public List<SelectListModel> AcademicTerms { get; set; }
        public List<SelectListModel> CourseYears { get; set; }
        public List<SelectListModel> Batches { get; set; }
        public List<SelectListModel> Branches { get; set; }
        public List<SelectListModel> InternalExamTerm { get; set; }

        public List<SelectListModel> ClassDetails { get; set; }

        [Required(ErrorMessageResourceType = typeof(ModelResources), ErrorMessageResourceName = "ClassDetailsRequired")]
        public List<long?> ClassDetailsKeys { get; set; }


        //[Required(ErrorMessageResourceType = typeof(ModelResources), ErrorMessageResourceName = "ClassDetailsRequired")]
        public long ClassDetailsKey { get; set; }
        public string ClassDetailsName { get; set; }

        public List<InternalExamDetailsModel> InternalExamDetails { get; set; }

        [Required(ErrorMessageResourceType = typeof(ModelResources), ErrorMessageResourceName = "AttendanceYearRequired")]
        public short CourseYear { get; set; }
        public string CourseYearName { get; set; }


        [Required(ErrorMessageResourceType = typeof(ModelResources), ErrorMessageResourceName = "BranchRequired")]
        public short? BranchKey { get; set; }
        public string BranchName { get; set; }

        [Required(ErrorMessageResourceType = typeof(ModelResources), ErrorMessageResourceName = "CourseTypeRequired")]
        public short CourseTypeKey { get; set; }


        [Required(ErrorMessageResourceType = typeof(ModelResources), ErrorMessageResourceName = "UniversityKeyRequired")]
        public short UniversityMasterKey { get; set; }
        public string UniversityName { get; set; }



        [Required(ErrorMessageResourceType = typeof(ModelResources), ErrorMessageResourceName = "CourseKeyRequired")]
        public long CourseKey { get; set; }
        public string CourseName { get; set; }


        [Required(ErrorMessageResourceType = typeof(ModelResources), ErrorMessageResourceName = "SyllabusRequired")]
        public short AcademicTermKey { get; set; }
        public string AcademicTermName { get; set; }
        public bool IsTeacher { get; set; }

        [Required(ErrorMessageResourceType = typeof(ModelResources), ErrorMessageResourceName = "BatchRequired")]
        public short BatchKey { get; set; }
        public string BatchName { get; set; }
        public int NoOfSubjects { get; set; }
        public long RowKey { get; set; }
        public short InternalExamTermKey { get; set; }
        public string InternalExamTermName { get; set; }
        public long? InternalExamKey { get; set; }

        //public DateTime ScheduledDate { get; set; }
        public int ExamYear { get; set; }
        public bool? IfEdit { get; set; }
        public decimal? MaximumMarkAll { get; set; }
        public decimal? MinimumMarkAll { get; set; }
        public string SearchText { get; set; }
        public List<StudetMailDetails> StudetMailDetails { get; set; }
        public int? CourseDuration { get; set; }

        public List<string>ClassDetailsNames { get; set; }
    }

    public class InternalExamDetailsModel
    {
        public long RowKey { get; set; }

        public long InternalExamKey { get; set; }
        public long SubjectKey { get; set; }
        public string SubjectName { get; set; }

        [RequiredIfTrue("ExamStatus", ErrorMessageResourceType = typeof(ModelResources), ErrorMessageResourceName = "ExamDateRequired")]
        public DateTime? ExamDate { get; set; }
        public bool ExamStatus { get; set; }

        [RegularExpression(@"^(100(\.[0]{1,2})?|[0-9]{1,2}(\.[0-9]{1,2})?)$", ErrorMessageResourceType = typeof(ModelResources), ErrorMessageResourceName = "RegularExpressionMarkMessage")]
        [RequiredIfTrue("ExamStatus", ErrorMessageResourceType = typeof(ModelResources), ErrorMessageResourceName = "MaximumMarkRequired")]
        //[GreaterThan("MinimumMark", PassOnNull = true, ErrorMessageResourceType = typeof(ModelResources), ErrorMessageResourceName = "MaximumMarkLessThanErrorMessage")]
        [Display(Name = "MaximumMark", ResourceType = typeof(EduSuiteUIResources))]
        public decimal? MaximumMark { get; set; }

        [RegularExpression(@"^(0*(\d{1,2}(\.\d+)?)|\.\d+|100(\.0+$)?)$", ErrorMessageResourceType = typeof(ModelResources), ErrorMessageResourceName = "RegularExpressionMarkMessage")]
        [RequiredIfTrue("ExamStatus", ErrorMessageResourceType = typeof(ModelResources), ErrorMessageResourceName = "MinimumMarkRequired")]
        [LessThan("MaximumMark", PassOnNull = true, ErrorMessageResourceType = typeof(ModelResources), ErrorMessageResourceName = "MinimumMarkGreaterThanErrorMessage")]
        [Display(Name = "MinimumMark", ResourceType = typeof(EduSuiteUIResources))]
        public decimal? MinimumMark { get; set; }
        public TimeSpan? ExamStartTime { get; set; }

        [GreaterThan("ExamStartTime", PassOnNull = true, ErrorMessageResourceType = typeof(ModelResources), ErrorMessageResourceName = "ExamStartTimeLessThanErrorMessage")]
        public TimeSpan? ExamEndTime { get; set; }
        public long InternalExamDetailsKey { get; set; }


    }

    public class StudetMailDetails
    {
        public string StudentName { get; set; }
        public string StudentEmail { get; set; }
        public string StudentPhone { get; set; }
    }
}
