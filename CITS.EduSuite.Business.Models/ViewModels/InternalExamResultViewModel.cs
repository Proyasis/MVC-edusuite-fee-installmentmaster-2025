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
    public class InternalExamResultViewModel : BaseModel
    {
        public InternalExamResultViewModel()
        {
            Branches = new List<SelectListModel>();
            Batches = new List<SelectListModel>();
            ClassDetails = new List<SelectListModel>();

            Employees = new List<SelectListModel>();
        }

        public List<SelectListModel> Branches { get; set; }
        public List<SelectListModel> Batches { get; set; }
        public List<SelectListModel> ClassDetails { get; set; }



        public List<SelectListModel> Employees { get; set; }

        public short? SearchBranchKey { get; set; }

        public long? SearchEmployeeKey { get; set; }
        
        
        public short? CourseYear { get; set; }
        public string CourseYearName { get; set; }

        public short? BranchKey { get; set; }
        public string BranchName { get; set; }

        public long? ClassDetailsKey { get; set; }
        public string ClassDetailsName { get; set; }

        public long? EmployeeKey { get; set; }

        public short CourseTypeKey { get; set; }

        public short UniversityMasterKey { get; set; }
        public string UniversityName { get; set; }

        public long CourseKey { get; set; }
        public string CourseName { get; set; }


        public short AcademicTermKey { get; set; }
        public string AcademicTermName { get; set; }

        public bool IsTeacher { get; set; }

        public short BatchKey { get; set; }
        public string BatchName { get; set; }
        public int? NoOfSubject { get; set; }


        public bool CheckStatus { get; set; }
        public string ApplicationKeys { get; set; }

        public int ExamYear { get; set; }

        public string ExamYearText { get; set; }
        public short? InternalExamTermKey { get; set; }
        public string InternalExamTermName { get; set; }
        public long? SubjectKey { get; set; }


        public long? InternalExamKey { get; set; }

        public int PageIndex { get; set; }
        public int PageSize { get; set; }

        public string SortBy { get; set; }
        public string SortOrder { get; set; }

        public List<InternalExamResultDetail> InternalExamResultDetails { get; set; }
        public List<InternaleExamResultSubjectDetail> InternaleExamResultSubjectDetails { get; set; }
    }
    public class InternalExamResultDetail
    {
        public long RowKey { get; set; }

        public string AdmissionNo { get; set; }
        public string StudentName { get; set; }
        public long ApplicationKey { get; set; }
        public long ClassDetailsKey { get; set; }
        public long InternalExamKey { get; set; }
        public long SubjectKey { get; set; }
        public string ResultStatus { get; set; }

        [RequiredIf("ResultStatus", "P", ErrorMessageResourceType = typeof(ModelResources), ErrorMessageResourceName = "MaximumMarkRequired")]
        public decimal? Mark { get; set; }
        public decimal? MaximumMark { get; set; }
        public decimal? MinimumMark { get; set; }
        public string Remarks { get; set; }
        public bool AbsentStatus { get; set; }
        public long InternalExamDetailsKey { get; set; }


        // For Send Mail  start
        public string StudentEmail { get; set; }
        public string MobileNumber { get; set; }
        public string GuardianMobileNumber { get; set; }
        public string CourseName { get; set; }
        public string UniversityName { get; set; }
        public string BatchName { get; set; }
        public string SubjctName { get; set; }
        public string ExamYearText { get; set; }
        public string ResultStatusText { get; set; }

        public int? SubjectYear { get; set; }
        public int? CourseDuration { get; set; }
        public short? AcademicTermKey { get; set; }
        public string InternalExamTermName { get; set; }
        // For Send Mail  End
    }

    public class InternaleExamResultSubjectDetail
    {
        public long? SubjectKey { get; set; }
        public string SubjectName { get; set; }
        public int ExamYear { get; set; }
        public long? ClassDetailsKey { get; set; }
        public long? InternalExamKey { get; set; }

        public int? NoOfResult { get; set; }
        public int? Passed { get; set; }
        public int? Failed { get; set; }
        public int? Absent { get; set; }
        public int? AddedResults { get; set; }
        public long? InternalExamDetailsKey { get; set; }



    }

}
