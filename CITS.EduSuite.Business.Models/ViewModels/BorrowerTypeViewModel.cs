using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CITS.EduSuite.Business.Models.Resources;

namespace CITS.EduSuite.Business.Models.ViewModels
{
    public class BorrowerTypeViewModel : BaseModel
    {
        public BorrowerTypeViewModel()
        {
            IsActive = true;
            NoOfBookIssueAtATime = 1;
        }
        public byte RowKey { get; set; }

        [Required(ErrorMessageResourceType = typeof(ModelResources), ErrorMessageResourceName = "RequiredErrorMessage")]
        [StringLength(100, ErrorMessageResourceType = typeof(ModelResources), ErrorMessageResourceName = "LengthErrorMessage")]
        [RegularExpression(@"^[0-9 a-zA-Z ]+$", ErrorMessageResourceType = typeof(ModelResources), ErrorMessageResourceName = "RegularExpressionErrorMessage")]
        [Display(Name = "BorrowerType", ResourceType = typeof(EduSuiteUIResources))]
        public string BorrowerTypeName { get; set; }

        [Required(ErrorMessageResourceType = typeof(ModelResources), ErrorMessageResourceName = "RequiredErrorMessage")]
        [System.Web.Mvc.Remote("CheckBorrowerTypeCodeExist", "BorrowerType", AdditionalFields = "RowKey", ErrorMessageResourceType = typeof(ModelResources), ErrorMessageResourceName = "ExistsErrorMessage")]
        [Display(Name = "Code", ResourceType = typeof(EduSuiteUIResources))]

        public string BorrowerTypeCode { get; set; }


        [Required(ErrorMessageResourceType = typeof(ModelResources), ErrorMessageResourceName = "RequiredErrorMessage")]
        [Range(1, Byte.MaxValue, ErrorMessageResourceType = typeof(ModelResources), ErrorMessageResourceName = "NoOfBookIssueAtATimeRangeErrorMessage")]
        [RegularExpression(@"^[0-9]+$", ErrorMessageResourceType = typeof(ModelResources), ErrorMessageResourceName = "RegularExpressionDigitErrorMessage")]
        [Display(Name = "NoOfBookIssueAtATime", ResourceType = typeof(EduSuiteUIResources))]
        public byte NoOfBookIssueAtATime { get; set; }


        public bool IsActive { get; set; }

        public string IsActiveText { get { return IsActive ? EduSuiteUIResources.Yes : EduSuiteUIResources.No; } }
    }
}
