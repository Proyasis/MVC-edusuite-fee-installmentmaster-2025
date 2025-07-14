using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CITS.EduSuite.Business.Models.Resources;
using CITS.Validations;
using CITS.EduSuite.Business.Models.Security;
using System.Threading;
using System.ComponentModel.DataAnnotations;

namespace CITS.EduSuite.Business.Models.ViewModels
{
    public class ScholarshipViewModel : BaseModel
    {
        public ScholarshipViewModel()
        {
            Branches = new List<SelectListModel>();
            ScholarshipTypes = new List<SelectListModel>();
            Districts = new List<SelectListModel>();
            AgeLimitLessThan = 18;
            AgeLimitGreaterThan = 35;


        }

        public long RowKey { get; set; }

        [Required(ErrorMessageResourceType = typeof(ModelResources), ErrorMessageResourceName = "EnquiryNameRequired")]
        [StringLength(100, ErrorMessageResourceType = typeof(ModelResources), ErrorMessageResourceName = "EnquiryNameLengthErrorMessage")]
        [RegularExpression(@"^[0-9 a-zA-Z *()&-/]+$", ErrorMessageResourceType = typeof(ModelResources), ErrorMessageResourceName = "EnquiryNameRegularExpressionErrorMessage")]
        public string ScholarShipName { get; set; }

        [Required(ErrorMessageResourceType = typeof(ModelResources), ErrorMessageResourceName = "EmployeeLastName_Required")]
        [StringLength(100, ErrorMessageResourceType = typeof(ModelResources), ErrorMessageResourceName = "EmployeeLastNameLengthErrorMessage")]
        [RegularExpression(@"^[0-9 a-zA-Z *()&-/]+$", ErrorMessageResourceType = typeof(ModelResources), ErrorMessageResourceName = "EmployeeLastNameRegularExpressionErrorMessage")]
        public string LastName { get; set; }

        [StringLength(100, ErrorMessageResourceType = typeof(ModelResources), ErrorMessageResourceName = "EmployeeMiddleNameLengthErrorMessage")]
        [RegularExpression(@"^[0-9 a-zA-Z *()&-/]+$", ErrorMessageResourceType = typeof(ModelResources), ErrorMessageResourceName = "EmployeeMiddleNameRegularExpressionErrorMessage")]
        public string MiddleName { get; set; }

        [StringLength(100, ErrorMessageResourceType = typeof(ModelResources), ErrorMessageResourceName = "EnquiryNameLengthErrorMessage")]
        public string ScholarShipAddress { get; set; }

        [StringLength(100, ErrorMessageResourceType = typeof(ModelResources), ErrorMessageResourceName = "EmailAddressLengthErrorMessage")]
        [RegularExpression(@"^\w+([-+.']\w+)*@\w+([-.]\w+)*\.\w+([-.]\w+)*$", ErrorMessageResourceType = typeof(ModelResources), ErrorMessageResourceName = "EmailAddressExpressionErrorMessage")]
        public string EmailAddress { get; set; }

        [Required(ErrorMessageResourceType = typeof(ModelResources), ErrorMessageResourceName = "EnquiryPhoneRequired")]
        [RegularExpression(@"^[0-9]{10}$", ErrorMessageResourceType = typeof(ModelResources), ErrorMessageResourceName = "MobileNumberExpressionErrorMessage")]
        [StringLength(10, ErrorMessageResourceType = typeof(ModelResources), ErrorMessageResourceName = "MobileNumberExpressionErrorMessage")]
        public string MobileNumber { get; set; }
        public string ScholarShipEducationQualification { get; set; }

        [Required(ErrorMessageResourceType = typeof(ModelResources), ErrorMessageResourceName = "BranchRequired")]
        public short BranchKey { get; set; }
        public string BranchName { get; set; }

        [Required(ErrorMessageResourceType = typeof(ModelResources), ErrorMessageResourceName = "ScholarshipTypeKeyRequired")]
        public short ScholarshipTypeKey { get; set; }
        public string ScholarshipTypeName { get; set; }


        [Required(ErrorMessageResourceType = typeof(ModelResources), ErrorMessageResourceName = "DateOfBirthRequired")]
        public DateTime DateOfBirth { get; set; }

        //[LessThan("AgeLimitLessThan", PassOnNull = true, ErrorMessageResourceType = typeof(ModelResources), ErrorMessageResourceName = "AgeLimitToGreaterThanErrorMessage")]
        //[GreaterThan("AgeLimitGreaterThan", PassOnNull = true, ErrorMessageResourceType = typeof(ModelResources), ErrorMessageResourceName = "AgeLimitToGreaterThanErrorMessage")]
        [Range(18, 35, ErrorMessageResourceType = typeof(ModelResources), ErrorMessageResourceName = "AgeLimitToGreaterThanErrorMessage")]
        public int? Age { get; set; }
        public int? AgeLimitLessThan { get; set; }
        public int? AgeLimitGreaterThan { get; set; }

        public byte Gender { get; set; }
        public string LocationName { get; set; }
        public string Remarks { get; set; }

        [RegularExpression(@"^[0-9]*$", ErrorMessageResourceType = typeof(ModelResources), ErrorMessageResourceName = "MobileNumberExpressionErrorMessage")]
        [StringLength(10, ErrorMessageResourceType = typeof(ModelResources), ErrorMessageResourceName = "MobileNumberExpressionErrorMessage")]

        public string MobileNumberOptional { get; set; }

        [Required(ErrorMessageResourceType = typeof(ModelResources), ErrorMessageResourceName = "DistrictRequired")]
        public int DistrictKey { get; set; }
        public string DistrictName { get; set; }


        public List<SelectListModel> Branches { get; set; }
        public List<SelectListModel> Districts { get; set; }
        public List<SelectListModel> ScholarshipTypes { get; set; }

        public DateTime? ScholarshipDate { get; set; }

        
        

        public string SearchName { get; set; }
        public string SearchPhone { get; set; }
        public DateTime? SearchFromDate { get; set; }
        public DateTime? SearchToDate { get; set; }
        public short? SearchBranchKey { get; set; }
        public int? SearchDistrictKey { get; set; }
        public short? SearchScholarshipTypeKey { get; set; }

        public string sidx { get; set; }
        public string sord { get; set; }
        public int page { get; set; }
        public int rows { get; set; }

        public long? EnquiryKey { get; set; }



    }


}
