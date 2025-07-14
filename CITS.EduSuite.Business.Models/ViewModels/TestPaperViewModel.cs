using CITS.EduSuite.Business.Models.Resources;
using CITS.Validations;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;


namespace CITS.EduSuite.Business.Models.ViewModels
{
    public class TestPaperViewModel : BaseModel
    {
        public TestPaperViewModel()
        {
            QuestionTypes = new List<SelectListModel>();
            QuestionModules = new List<SelectListModel>();
            QuestionSections = new List<TestSectionViewModel>();
            TestQuestions = new List<TestQuestionViewModel>();
            Plans = new List<SelectListModel>();
            ExamTypes = new List<SelectListModel>();


            #region Student Filter
            Subjects = new List<SelectListModel>();
            AcademicTerms = new List<SelectListModel>();
            CourseTypes = new List<SelectListModel>();
            Courses = new List<SelectListModel>();
            UniversityMasters = new List<SelectListModel>();
            Mediums = new List<SelectListModel>();
            SecondLanguages = new List<SelectListModel>();
            Batches = new List<SelectListModel>();
            Modes = new List<SelectListModel>();
            CourseYears = new List<SelectListModel>();
            ClassModes = new List<SelectListModel>();      
            ClassCodes = new List<SelectListModel>();
            Batches = new List<SelectListModel>();
            Branches = new List<SelectListModel>();

            SubjectKeys = new List<long>();
            AcademicTermsKeys = new List<long>();
            CourseTypeKeys = new List<long>();
            CourseKeys = new List<long>();
            UniversityMasterKeys = new List<long>();
            MeadiumKeys = new List<long>();
            SecondLanguageKeys = new List<long>();
            BatchKeys = new List<long>();
            ModeKeys = new List<long>();
            CourseYearsKeys = new List<long>();
            ClassModeKeys = new List<long>();
            ClassCodeKeys = new List<long>();
            BranchKeys = new List<long>();

            #endregion
        }

        public List<SelectListModel> ExamTypes { get; set; }
        #region Student Filter
        public List<SelectListModel> Subjects { get; set; }
        public List<SelectListModel> AcademicTerms { get; set; }
        public List<SelectListModel> CourseTypes { get; set; }
        public List<SelectListModel> Courses { get; set; }
        public List<SelectListModel> UniversityMasters { get; set; }
        public List<SelectListModel> Mediums { get; set; }
        public List<SelectListModel> SecondLanguages { get; set; }
        public List<SelectListModel> Batches { get; set; }
        public List<SelectListModel> Modes { get; set; }
        public List<SelectListModel> CourseYears { get; set; }
        public List<SelectListModel> ClassModes { get; set; }
        public List<SelectListModel> ClassCodes { get; set; }
        public List<SelectListModel> Branches { get; set; }

        [RequiredIfNot("ExamTypeKey",DbConstants.ExamTypes.OtherExam,ErrorMessageResourceType = typeof(ModelResources), ErrorMessageResourceName = "SubjectRequired")]
        public List<long> SubjectKeys { get; set; }
        public List<long> AcademicTermsKeys { get; set; }
        public List<long> CourseTypeKeys { get; set; }
        public List<long> CourseKeys { get; set; }
        public List<long> UniversityMasterKeys { get; set; }
        public List<long> MeadiumKeys { get; set; }
        public List<long> SecondLanguageKeys { get; set; }
        public List<long> BatchKeys { get; set; }
        public List<long> ModeKeys { get; set; }
        public List<long> CourseYearsKeys { get; set; }
        public List<long> ClassModeKeys { get; set; }
        public List<long> ClassCodeKeys { get; set; }
        public List<long> BranchKeys { get; set; }


        public string SubjectKeys_ { get; set; }
        public string AcademicTermsKeys_ { get; set; }
        public string CourseTypeKeys_ { get; set; }
        public string CourseKeys_ { get; set; }
        public string UniversityMasterKeys_ { get; set; }
        public string MeadiumKeys_ { get; set; }
        public string SecondLanguageKeys_ { get; set; }
        public string BatchKeys_ { get; set; }
        public string ModeKeys_ { get; set; }
        public string CourseYearsKeys_ { get; set; }
        public string ClassModeKeys_ { get; set; }
        public string ClassCodeKeys_ { get; set; }
        public string BranchKeys_ { get; set; }
        #endregion

        public long RowKey { get; set; }
        public byte ModuleKey { get; set; }
        public decimal? TotalMark { get; set; }
        public string ModuleName { get; set; }
        public string SubjectName { get; set; }
        public string ExamDateTime { get; set; }
        public int SectionCount { get; set; }
        public int QuestionCount { get; set; }
        public int QuestionsCount { get; set; }
        public int LastQuestionIndex { get; set; }
        public bool IsActive { get; set; }

        [Required(ErrorMessageResourceType = typeof(ModelResources), ErrorMessageResourceName = "RequiredErrorMessage")]
        //[StringLength(50, ErrorMessageResourceType = typeof(ModelResources), ErrorMessageResourceName = "LengthErrorMessage")]
        //[Remote("CheckTestNameExists", "TestPaper", AdditionalFields = "RowKey", ErrorMessageResourceType = typeof(ModelResources), ErrorMessageResourceName = "AlreadyExistsErrorMessage")]
        [Display(Name = "TestPaper", ResourceType = typeof(EduSuiteUIResources))]

       
        public string TestPaperName { get; set; }
        public long? TestModuleKey { get; set; }

        public long? TestSectionKey { get; set; }

       
        [Required(ErrorMessageResourceType = typeof(ModelResources), ErrorMessageResourceName = "RequiredErrorMessage")]
        [Display(Name = "ExamType", ResourceType = typeof(EduSuiteUIResources))]
        public short? ExamTypeKey { get; set; }

        [Required(ErrorMessageResourceType = typeof(ModelResources), ErrorMessageResourceName = "RequiredErrorMessage")]
        [Display(Name = "ExamDuration", ResourceType = typeof(EduSuiteUIResources))]
        public TimeSpan?  ExamDuration { get; set; }
        public string TestSectionName { get; set; }
        public string SectionNumber { get; set; }

        [Display(Name = "SupportFile", ResourceType = typeof(EduSuiteUIResources))]
        public HttpPostedFileBase SupportedFilePath { get; set; }


        public string SupportedFileName { get; set; }

        [AllowHtml]
        //[Required(ErrorMessageResourceType = typeof(ModelResources), ErrorMessageResourceName = "RequiredErrorMessage")]
        [Display(Name = "QuestionPaper", ResourceType = typeof(EduSuiteUIResources))]
        public string QuestionPaper { get; set; }
        public string QuestionPaperFileName { get; set; }
        public int LastQuestionNumber { get; set; }

        public decimal? Mark { get; set; }
        public decimal? NegativeMark { get; set; }
        [AllowHtml]
        //[Required(ErrorMessageResourceType = typeof(ModelResources), ErrorMessageResourceName = "RequiredErrorMessage")]
        [Display(Name = "Instructions", ResourceType = typeof(EduSuiteUIResources))]
        public string TestInstruction { get; set; }
        public string TestInstructionFileName { get; set; }

        //public DateTime? FromDate { get; set; }
        //public DateTime? ToDate { get; set; }

        //[RequiredIfNotEmpty("ExamTime", ErrorMessageResourceType = typeof(ModelResources), ErrorMessageResourceName = "ExamDateRequired")]
        [RequiredIfNotEmpty("ExamTime", ErrorMessageResourceType = typeof(ModelResources), ErrorMessageResourceName = "RequiredErrorMessage")]
        [Display(Name = "ExamDate", ResourceType = typeof(EduSuiteUIResources))]
        public DateTime? ExamDate { get; set; }

        //[RequiredIfNotEmpty("ExamDate", ErrorMessageResourceType = typeof(ModelResources), ErrorMessageResourceName = "ExamTimeRequired")]
        [RequiredIfNotEmpty("ExamDate", ErrorMessageResourceType = typeof(ModelResources), ErrorMessageResourceName = "RequiredErrorMessage")]
        [Display(Name = "ExamTime", ResourceType = typeof(EduSuiteUIResources))]
        public TimeSpan? ExamTime { get; set; }

        //[Required(ErrorMessageResourceType = typeof(ModelResources), ErrorMessageResourceName = "SubjectRequired")]
        //public long SubjectKey { get; set; }

        //[RegularExpression(@"^[0-9]{0,10}$", ErrorMessageResourceType = typeof(ModelResources), ErrorMessageResourceName = "DurationRegularExperssion")][Display(Name = "ExamDate", ResourceType = typeof(EduSuiteUIResources))]
        public int? TestDuration { get; set; }

        //[Required(ErrorMessageResourceType = typeof(ModelResources), ErrorMessageResourceName = "RequiredErrorMessage")]
        [Display(Name = "Plan", ResourceType = typeof(EduSuiteUIResources))]
        public List<byte> PlanKeys { get; set; }
        public string PlanKeyText { get; set; }
        //[Required(ErrorMessageResourceType = typeof(ModelResources), ErrorMessageResourceName = "RequiredErrorMessage")]
        public int? MarkGroupKey { get; set; }
        public List<SelectListModel> QuestionTypes { get; set; }
        public List<SelectListModel> QuestionModules { get; set; }
        public List<TestSectionViewModel> QuestionSections { get; set; }
        public List<TestQuestionViewModel> TestQuestions { get; set; }
        public List<SelectListModel> Plans { get; set; }

        public List<MarkGroupViewModel> MarkGroups { get; set; }

        public List<TestPaperQuestionsViewModel> TestPaperQuestions { get; set; }


    }




    public class TestPaperQuestionsViewModel
    {
        public long RowKey { get; set; }
        [AllowHtml]
        [Required(ErrorMessageResourceType = typeof(ModelResources), ErrorMessageResourceName = "RequiredErrorMessage")]
        [Display(Name = "QuestionPaper", ResourceType = typeof(EduSuiteUIResources))]
        [MaxLength]
        public string QuestionPaper { get; set; }
        public int? MarkGroupKey { get; set; }
        public byte ModuleKey { get; set; }

    }

    public class TestQuestionViewModel : BaseModel
    {
        public TestQuestionViewModel()
        {
            QuestionOptions = new List<TestQuestionOptionsViewModel>();
           
        }
        public long RowKey { get; set; }
        public string TestSectionName { get; set; }
        public byte QuestionTypeKey { get; set; }
        public int QuestionNumber { get; set; }
        public int? QuestionStatusKey { get; set; }
        public bool IsCorrect { get; set; }
        public bool AnswerKeyStatus { get; set; }

        [Required(ErrorMessageResourceType = typeof(ModelResources), ErrorMessageResourceName = "MarkRequired")]
        public decimal? Mark { get; set; }

        //[Required(ErrorMessageResourceType = typeof(ModelResources), ErrorMessageResourceName = "RequiredErrorMessage")]
        public decimal? NegativeMark { get; set; }
        public int? QuestionDuration { get; set; }

        public decimal? MaximumMark { get; set; }

        //[Required(ErrorMessageResourceType = typeof(ModelResources), ErrorMessageResourceName = "RequiredErrorMessage")]
        public int? MarkGroupKey { get; set; }
        public string AnswerText { get; set; }
        public string AnswerKey { get; set; }
        public long? TestSectionKey { get; set; }

        public string QuestionPaperFileName { get; set; }

        [AllowHtml]
        [Required(ErrorMessageResourceType = typeof(ModelResources), ErrorMessageResourceName = "RequiredErrorMessage")]
        [Display(Name = "QuestionPaper", ResourceType = typeof(EduSuiteUIResources))]
        public string QuestionPaper { get; set; }
        public List<TestQuestionOptionsViewModel> QuestionOptions { get; set; }

    }

    public class TestQuestionOptionsViewModel
    {
        //[Required(ErrorMessageResourceType = typeof(ModelResources), ErrorMessageResourceName = "RequiredErrorMessage")]
        public string OptionText { get; set; }

        [Required(ErrorMessageResourceType = typeof(ModelResources), ErrorMessageResourceName = "RequiredErrorMessage")]
        public string OptionValue { get; set; }
        public long RowKey { get; set; }
        public long TestSectionKey { get; set; }
        public bool IsAnswerKey { get; set; }
    }



        public class TestSectionViewModel
    {
        public TestSectionViewModel()
        {
            QuestionDetails = new List<TestQuestionViewModel>();
        }
        public long RowKey { get; set; }
        public string TestSectionName { get; set; }
        public string TestSectionFileName { get; set; }
        public string SupportedFileName { get; set; }
        public string QuestionPaperFileName { get; set; }
        
        public List<TestQuestionViewModel> QuestionDetails { get; set; }

    }


    public class TestPaperFilter
    {
        public long RowKey { get; set; }
        public string Text { get; set; }
        public long SubjectKey { get; set; }
        public int AcademicTermKey { get; set; }
        public long CourseTypeKey { get; set; }
        public long CourseKey { get; set; }
        public long UniversityMasterKey { get; set; }
        public short StudentYear { get; set; }
    }




}
