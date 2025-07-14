using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using CITS.EduSuite.Business.Models.Resources;
//using CITS.Validations;

namespace CITS.EduSuite.Business.Models.ViewModels
{
    public class ApplicationViewModel : BaseModel
    {
        public ApplicationViewModel()
        {
            PersonalDetails = new ApplicationPersonalViewModel();
            EducationalQualificationDetails = new List<EducationDetailViewModel>();
            DocumentDetails = new List<DocumentViewModel>();
            FeePyamentDetails = new List<ApplicationFeePaymentViewModel>();
            Branches = new List<SelectListModel>();
            Batches = new List<SelectListModel>();
            StudyMaterials = new List<StudyMaterialDetailsModel>();
            UniversityPyamentDetails = new List<UniversityPaymentViewmodel>();
            StudentIDCardDetails = new List<StudentIDCardList>();
            UniversityCertificateDetails = new List<UniversityCertificateDetails>();
            StudentsCertificateReturnDetail = new List<StudentsCertificateReturnDetail>();
            ExamScheduleSummary = new List<ExamScheduleSummary>();
            CourseYears = new List<SelectListModel>();
            StudentAbscondersDetails = new List<StudentAbscondersViewModel>();
            StudentDiaryDetails = new List<StudentDiaryViewModel>();
            StudentEarlyDepartureDetails = new List<StudentEarlyDepartureViewModel>();
            StudentLateDetails = new List<StudentLateViewModel>();
            StudentLeaveDetail = new List<StudentLeaveViewModel>();
            UnitTestResultDetails = new List<UnitTestResultViewModel>();
            FeeFollowupDetails = new List<ApplicationFeeFollowupDetailsViewModel>();
            Courses = new List<SelectListModel>();
            Universities = new List<SelectListModel>();

        }
        public decimal? TotalFee { get; set; }
        public decimal? TotalPaid { get; set; }
        public decimal? BalanceFee { get; set; }
        public ApplicationPersonalViewModel PersonalDetails { get; set; }
        public List<EducationDetailViewModel> EducationalQualificationDetails { get; set; }
        public List<DocumentViewModel> DocumentDetails { get; set; }
        public List<ApplicationFeePaymentViewModel> FeePyamentDetails { get; set; }
        public List<EnquiryScheduleViewModel> CallHistory { get; set; }
        public List<StudyMaterialDetailsModel> StudyMaterials { get; set; }
        public List<UniversityPaymentViewmodel> UniversityPyamentDetails { get; set; }
        public List<StudentIDCardList> StudentIDCardDetails { get; set; }
        public List<UniversityCertificateDetails> UniversityCertificateDetails { get; set; }
        public List<StudentsCertificateReturnDetail> StudentsCertificateReturnDetail { get; set; }
        public List<ExamScheduleSummary> ExamScheduleSummary { get; set; }
        public List<SelectListModel> CourseYears { get; set; }
        public List<StudentAbscondersViewModel> StudentAbscondersDetails { get; set; }
        public List<StudentDiaryViewModel> StudentDiaryDetails { get; set; }
        public List<StudentEarlyDepartureViewModel> StudentEarlyDepartureDetails { get; set; }
        public List<StudentLateViewModel> StudentLateDetails { get; set; }
        public List<StudentLeaveViewModel> StudentLeaveDetail { get; set; }
        public List<UnitTestResultViewModel> UnitTestResultDetails { get; set; }
        public List<ApplicationFeeFollowupDetailsViewModel> FeeFollowupDetails { get; set; }
        public List<SelectListModel> Branches { get; set; }
        public List<SelectListModel> Batches { get; set; }
        public List<SelectListModel> Courses { get; set; }
        public List<SelectListModel> Universities { get; set; }
        // new for Select Application Details
        public long RowKey { get; set; }
        public string AdmissionNo { get; set; }
        public string AcademicTermName { get; set; }
        public string CourseName { get; set; }
        public string UniversityName { get; set; }
        public string ApplicantName { get; set; }
        public string MobileNumber { get; set; }
        public string BatchName { get; set; }
        public string ApplicationStatusName { get; set; }
        public string CurrentYearText { get; set; }
        public short? BranchKey { get; set; }
        public int PageIndex { get; set; }
        public int PageSize { get; set; }
        public string SortBy { get; set; }
        public string SortOrder { get; set; }
        public int? AvailableBooks { get; set; }
        public int? IssuedBooks { get; set; }
        public int? TotalBooks { get; set; }

        // Students Certificate Return
        public int? NoOfCertificate { get; set; }
        public int? NoOfVerified { get; set; }
        public int? NoOfReturned { get; set; }

        // University Certificate Return
        public int? NoOfIssued { get; set; }
        public int? NoOfRecieved { get; set; }
        public int? NoOfTempReturned { get; set; }
        public short? BatchKey { get; set; }
        public string BranchName { get; set; }       
        public int? AvailableCertificates { get; set; }
        public int? RecievePending { get; set; }
        public string IsTaxText { get; set; }
        public string IsInstallmentText { get; set; }
        public string IsConsessionText { get; set; }
        public int? CourseDuration { get; set; }
        public short? AcademicTermKey { get; set; }
        public short? CurrentYear { get; set; }
        public bool? IsTax { get; set; }
        public decimal? OldPaid { get; set; }
        public string NextApplicationAdmissionNo { get; set; }
        public long? NextApplicationKey { get; set; }
        public string NextApplicationRollNoCode { get; set; }
        public string NextApplicationName { get; set; }
        public string PrevApplicationAdmissionNo { get; set; }
        public long? PrevApplicationKey { get; set; }
        public string PrevApplicationRollNoCode { get; set; }
        public string PrevApplicationName { get; set; }

        // Student Tc 
        public bool? IsGenerate { get; set; }
        public bool IsIssue { get; set; }
        public string IssuedBy { get; set; }
        public string GenerateBy { get; set; }
        public DateTime? GenerateDate { get; set; }
        public DateTime? IssuedDate { get; set; }
        public string ReasonMasterName { get; set; }
        public long? CourseKey { get; set; }
        public short? UniversityKey { get; set; }

    }
    //public class ApplicationScheduleViewModel : BaseModel
    //{
    //    public ApplicationScheduleViewModel()
    //    {
    //        EnquiryCallStatuses = new List<SelectListModel>();
    //        EnquiryStatuses = new List<SelectListModel>();
    //        CallTypes = new List<SelectListModel>();
    //    }

    //    public long RowKey { get; set; }


    //    [StringLength(200, ErrorMessageResourceType = typeof(ModelResources), ErrorMessageResourceName = "LeadFeedbackLengthErrorMessage")]
    //    [Required(ErrorMessageResourceType = typeof(ModelResources), ErrorMessageResourceName = "EnquiryLeadFeedbackRequired")]
    //    public string Feedback { get; set; }
    //    [Required(ErrorMessageResourceType = typeof(ModelResources), ErrorMessageResourceName = "CallDurationRequired")]
    //    public TimeSpan? CallDuration { get; set; }

    //   // [RequiredIfNot("EnquiryLeadCallStatusKey", DbConstants.EnquiryLeadCallStatus.NotInterested, ErrorMessageResourceType = typeof(ModelResources), ErrorMessageResourceName = "NextCallScheduleRequired")]
    //    public DateTime? NextCallSchedule { get; set; }

    //    public long ApplicationKey { get; set; }


    //    [Required(ErrorMessageResourceType = typeof(ModelResources), ErrorMessageResourceName = "CallTypeRequired")]
    //    public byte? CallTypeKey { get; set; }
    //    public string CallTypeName { get; set; }

    //    [Required(ErrorMessageResourceType = typeof(ModelResources), ErrorMessageResourceName = "EnquiryLeadCallStatusRequired")]
    //    public int? EnquiryCallStatusKey { get; set; }
    //    public string EnquiryCallStatusName { get; set; }

    //    //[Required(ErrorMessageResourceType = typeof(ModelResources), ErrorMessageResourceName = "EnquiryStatusRequired")]
    //    public short EnquiryStatusKey { get; set; }

    //    public List<SelectListModel> CallTypes { get; set; }
    //    public List<SelectListModel> EnquiryCallStatuses { get; set; }
    //    public List<SelectListModel> EnquiryStatuses { get; set; }
    //}
}
