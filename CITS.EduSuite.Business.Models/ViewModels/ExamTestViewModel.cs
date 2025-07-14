using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;


namespace CITS.EduSuite.Business.Models.ViewModels
{
    public class ExamTestViewModel : BaseModel
    {
        public ExamTestViewModel()
        {
            TestSections = new List<TestSectionViewModel>();
            TestModules = new List<TestPaperViewModel>();
            ExamAnswers = new List<ExamTestAnswerViewModel>();

            #region DropdownFilters
            TestPaperTypes = new List<SelectListModel>();

            ExamTestList = new List<ExamTestListViewModel>();
            #endregion
        }
        #region DropdownFilters
        public List<SelectListModel> TestPaperTypes { get; set; }
        #endregion


        public long RowKey { get; set; }
        public int ExamStatusKey { get; set; }
        public long ExamTestSectionKey { get; set; }
        public long TestPaperKey { get; set; }
        public string TestPaperName { get; set; }
        public long ApplicationKey { get; set; }
        public long ApplicationUserKey { get; set; }
        public string ApplicantName { get; set; }
        public string InstructionFileName { get; set; }
        public string SupportedFileName { get; set; }
        public string StudentProfilePhoto { get; set; }
        public string SubjectName { get; set; }
        public string TestPaperTypeName { get; set; }
        public byte ModuleKey { get; set; }
        public DateTime? ExamStart { get; set; }
        public DateTime? ExamEnd { get; set; }
        public bool IsFinished { get; set; }
        public bool AnswerKeyStatus { get; set; }
        public int ExamDuration { get; set; }
        public int QuestionStatusKey { get; set; }
        public decimal MaximumScore { get; set; }
        public decimal? TotalScore { get; set; }
        public bool IsShowAllQuestions { get; set; }
        public int? TestPaperTypeKey { get; set; }

        public List<TestSectionViewModel> TestSections { get; set; }
        public List<TestPaperViewModel> TestModules { get; set; }
        public List<ExamTestAnswerViewModel> ExamAnswers { get; set; }
        public List<ExamTestListViewModel> ExamTestList { get; set; }

        public int? TotalQuestions { get; set; }
        public int? TotalAttept { get; set; }
        public int? TotalCorrectAnswers { get; set; }
        public int? TotalInCorrectAnswers { get; set; }
        public decimal? TotalNegativeMarks { get; set; }

        public string EndTimeMilliSeconds { get; set; }
        public bool IsExamExists { get; set; }
        public string ExamKey { get; set; }




        public DateTime? SearchDateFrom { get; set; }
        public DateTime? SearchDateTo { get; set; }
        public string SearchText { get; set; }
        public int PageIndex { get; set; }
        public int PageSize { get; set; }
        public long TotalRecords { get; set; }
        public string SortBy { get; set; }
        public string SortOrder { get; set; }
        public short? RoleKey { get; set; }

    }
    public class ExamTestAnswerViewModel
    {
        public int QuestionStatusKey { get; set; }
        public int QuestionNumber { get; set; }
        public string AnswerText { get; set; }
        public long? OptionRowKey { get; set; }
        public bool IsCorrect { get; set; }
        public decimal TotalScore { get; set; }
        public HttpPostedFileBase AnswerFile { get; set; }
    }


    public class ExamTestListViewModel
    {
        public long? TestPaperTypeKey { get; set; }
        public string TestPaperName { get; set; }
        public long? RowKey { get; set; }
        public long? TestPaperKey { get; set; }
        public int? ExamStatusKey { get; set; }
        public string TestPaperTypeName { get; set; }
        public string SubjectName { get; set; }
        public string SubjectDivisionName { get; set; }
        public string SubjectSubDivisionName { get; set; }
        public string CourseName { get; set; }
        public DateTime? ExamDate { get; set; }
    }


}
