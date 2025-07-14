using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using CITS.EduSuite.Business.Models.Resources;
using CITS.Validations;

namespace CITS.EduSuite.Business.Models.ViewModels
{
    public class ApplicationEnRollmentNoViewModel : BaseModel
    {
        public ApplicationEnRollmentNoViewModel()
        {
            StudentList = new List<SelectListModel>();
            EnRollmentNoDetailsViewModel = new List<EnRollmentNoDetailsViewModel>();
            ApplicationStatus = new List<SelectListModel>();
            IsClass = false;
        }

        public bool IsClass { get; set; }
        public List<SelectListModel> StudentList { get; set; }
        public List<StudentDetailsModel> StudentDetailsModel { get; set; }
        public List<EnRollmentNoDetailsViewModel> EnRollmentNoDetailsViewModel { get; set; }

        public List<SelectListModel> ApplicationStatus { get; set; }
    }
    public class EnRollmentNoDetailsViewModel
    {
        public EnRollmentNoDetailsViewModel()
        {
            CurrentYears = new List<SelectListModel>();
            ClassDetails = new List<SelectListModel>();
            IsClass = false;
        }
        [Required(ErrorMessageResourceType = typeof(ModelResources), ErrorMessageResourceName = "ApplicationKeyRequired")]
        [System.Web.Mvc.Remote("CheckApplicationKeyExists", "Application", ErrorMessageResourceType = typeof(ModelResources), ErrorMessageResourceName = "ApplicationKeyExisted")]
        public long? ApplicationKey { get; set; }
        public string ApplicantName { get; set; }
        public string AdmissionNo { get; set; }
        public string StudentEnrollmentNo { get; set; }
        public string ExamRegisterNo { get; set; }
        public short? ApplicationStatusKey { get; set; }
        public short? CurrentYear { get; set; }
        public long? ClassDetailsKey { get; set; }
        public string ClassDetailsName { get; set; }
        public int? RollNumber { get; set; }
        public string RollNoCode { get; set; }
        public bool IsClass { get; set; }

        //[RequiredIfTrue("IsClass", ErrorMessageResourceType = typeof(ModelResources), ErrorMessageResourceName = "RequiredErrorMessage")]
        [Display(Name = "Remarks", ResourceType = typeof(EduSuiteUIResources))]
        public string ClassRemarks { get; set; }

        public List<SelectListModel> CurrentYears { get; set; }
        public List<SelectListModel> ClassDetails { get; set; }

    }
    public class StudentDetailsModel
    {
        public long? ApplicationKey { get; set; }
        public string ApplicantName { get; set; }
        public string AdmissionNo { get; set; }
        public string StudentEnrollmentNo { get; set; }
        public string ExamRegisterNo { get; set; }

    }
}
