using CITS.EduSuite.Business.Models.Resources;
using CITS.EduSuite.Business.Models.ViewModels;
using CITS.Validations;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CITS.EduSuite.Business.Models.ViewModels
{
    public class MemberPlanDetailsViewModel : BaseModel
    {
        public MemberPlanDetailsViewModel()
        {
            MemberType = new List<SelectListModel>();
            BorrowerType = new List<SelectListModel>();
            Courses = new List<SelectListModel>();
            Batches = new List<SelectListModel>();
            Universities = new List<SelectListModel>();
            ApplicationTypes = new List<SelectListModel>();
            AcademicTerms = new List<SelectListModel>();

            Branches = new List<SelectListModel>();
            CheckStatus = false;

        }
        public long RowKey { get; set; }

        public bool CheckStatus { get; set; }

        [RequiredIf("CheckStatus", true, ErrorMessageResourceType = typeof(ModelResources), ErrorMessageResourceName = "RequiredErrorMessage")]
        [Display(Name = "MemberType", ResourceType = typeof(EduSuiteUIResources))]
        public byte? MemberTypeKey { get; set; }

        [RequiredIfTrue("CheckStatus", ErrorMessageResourceType = typeof(ModelResources), ErrorMessageResourceName = "RequiredErrorMessage")]
        [Display(Name = "BorrowerType", ResourceType = typeof(EduSuiteUIResources))]
        public byte? BorrowerTypeKey { get; set; }
        public byte ApplicationTypeKey { get; set; }
        public long ApplicationKey { get; set; }
        public bool IsBlockMember { get; set; }
        public string CardId { get; set; }
        public string BorrowerTypeName { get; set; }
        public string MemberTypeName { get; set; }
        public string MemberFullName { get; set; }
        public string ApplicationTypeName { get; set; }
        public decimal RegistrationFee { get; set; }
        public decimal MemberShipFee { get; set; }


        [Required(ErrorMessageResourceType = typeof(ModelResources), ErrorMessageResourceName = "BranchRequired")]
        public short BranchKey { get; set; }
        public string BranchName { get; set; }


        public List<SelectListModel> MemberType { get; set; }
        public List<SelectListModel> BorrowerType { get; set; }
        public List<SelectListModel> Courses { get; set; }
        public List<SelectListModel> Batches { get; set; }
        public List<SelectListModel> Universities { get; set; }
        public List<SelectListModel> Applications { get; set; }
        public List<SelectListModel> ApplicationTypes { get; set; }
        public List<SelectListModel> AcademicTerms { get; set; }
        public List<SelectListModel> Branches { get; set; }
        public List<SelectListModel> Designation { get; set; }


        public long? CourseKey { get; set; }
        public short? UniversityMasterKey { get; set; }
        public short? BatchKey { get; set; }
        public short? AcademicTermKey { get; set; }
       
        public long? CardSerialNo { get; set; }
        public string SearchText { get; set; }

        public string MobilleNo { get; set; }
        public string CurrentYearText { get; set; }
        public string UniversityCourse { get; set; }
        public string BatchName { get; set; }
        public string CourseName { get; set; }
        public string UniversityName { get; set; }
        public short? CurrentYear { get; set; }
        public int? CourseDuration { get; set; }

        public string AdmissionNo { get; set; }
        public string DesignationName { get; set; }
        public short? DesignationKey { get; set; }

        public int PageIndex { get; set; }
        public int PageSize { get; set; }

        public string SortBy { get; set; }
        public string SortOrder { get; set; }
        public string MemberPhoto { get; set; }
    }
}
