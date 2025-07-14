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
    public class ExamResultViewModel : BaseModel
    {
        public ExamResultViewModel()
        {
            Branches = new List<SelectListModel>();
            Employees = new List<SelectListModel>();
            ExamStatus = new List<SelectListModel>();
            Batches = new List<SelectListModel>();
        }
        public List<SelectListModel> Branches { get; set; }

        public List<SelectListModel> Employees { get; set; }
        public List<SelectListModel> Batches { get; set; }
        public short? SearchBranchKey { get; set; }

        public long? SearchEmployeeKey { get; set; }
        public long? UserKey { get; set; }
        public string CourseYearName { get; set; }

        public short? BranchKey { get; set; }
        public string BranchName { get; set; }

        public long? EmployeeKey { get; set; }

        public short? CourseTypeKey { get; set; }

        public short? UniversityMasterKey { get; set; }
        public string UniversityName { get; set; }

        public long? CourseKey { get; set; }
        public string CourseName { get; set; }
        
        public short? AcademicTermKey { get; set; }
        public string AcademicTermName { get; set; }
        
        public short? BatchKey { get; set; }
        public string BatchName { get; set; }
        public int? NoOfSubject { get; set; }
        
        public bool CheckStatus { get; set; }
        public string ApplicationKeys { get; set; }

        public int? ExamYear { get; set; }

        public string ExamYearText { get; set; }
        
        public short? ExamTermKey { get; set; }
        public string ExamTermName { get; set; }
        public long? SubjectKey { get; set; }
        
        public long? ExamScheduleKey { get; set; }

        public int PageIndex { get; set; }
        public int PageSize { get; set; }

        public string SortBy { get; set; }
        public string SortOrder { get; set; }
        public string SearchText { get; set; }
        public long? ApplicationKey { get; set; }
        public int? CourseDuration { get; set; }
        public short? CurrentYear { get; set; }
        public List<SelectListModel> ExamStatus { get; set; }

        public List<ExamResultDetail> ExamResultDetail { get; set; }
        public List<ExamResultSubjectDetail> ExamResultSubjectDetail { get; set; }
    }
    public class ExamResultDetail
    {
        public long RowKey { get; set; }

        public string AdmissionNo { get; set; }
        public string StudentName { get; set; }

        public long ApplicationKey { get; set; }
        public long SubjectKey { get; set; }
        public string SubjectName { get; set; }

        public string ResultStatus { get; set; }

        //[RequiredIfFalse("AbsentStatus", ErrorMessageResourceType = typeof(ModelResources), ErrorMessageResourceName = "MaximumMarkRequired")]
        public decimal? Mark { get; set; }
        public decimal? MaximumMark { get; set; }
        public decimal? MinimumMark { get; set; }
        public string Remarks { get; set; }

        public bool AbsentStatus { get; set; }
        public long ExamScheduleKey { get; set; }

        //[Required(ErrorMessageResourceType = typeof(ModelResources), ErrorMessageResourceName = "ExamStatusRequired")]
        public byte? ExamStatus { get; set; }
        public int? AppearenceCount { get; set; }
        public short? AcademicTermKey { get; set; }
        public int? CourseDuration { get; set; }
        public short? SubjectYear { get; set; }
        public string SubjectYearName { get; set; }
        public short? ExamTermKey { get; set; }
        public string ExamTermName { get; set; }
        public short? ExamCenterKey { get; set; }
        public string ExamCenterName { get; set; }
        public DateTime? ExamDate { get; set; }
    }

    public class ExamResultSubjectDetail
    {
        public long? SubjectKey { get; set; }
        public string SubjectName { get; set; }
        public int ExamYear { get; set; }
        public long? ExamScheduleKey { get; set; }
        public int? NoOfResult { get; set; }
        public int? Passed { get; set; }
        public int? Failed { get; set; }
        public int? Absent { get; set; }

        public short? BranchKey { get; set; }
        public short? UniversityMasterKey { get; set; }
        public long? CourseKey { get; set; }
        public short? AcademicTermKey { get; set; }
        public short? BatchKey { get; set; }
        public short? ExamTermKey { get; set; }

    }
}
