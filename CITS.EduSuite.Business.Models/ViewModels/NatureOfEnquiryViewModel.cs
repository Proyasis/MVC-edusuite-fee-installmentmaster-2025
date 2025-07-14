 using CITS.EduSuite.Business.Models.Resources;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CITS.EduSuite.Business.Models.ViewModels
{
    public class NatureOfEnquiryViewModel : BaseModel
    {
        public NatureOfEnquiryViewModel()
        {
            IsActive = true;
          
        }

        public short RowKey { get; set; }

        [Required(ErrorMessageResourceType = typeof(ModelResources), ErrorMessageResourceName = "NatureOfEnquiryNameRequired")]
        [StringLength(50, ErrorMessageResourceType = typeof(ModelResources), ErrorMessageResourceName = "NatureOfEnquiryNameLengthErrorMessage")]
        [RegularExpression(@"^[0-9 a-zA-Z *()&-/]+$", ErrorMessageResourceType = typeof(ModelResources), ErrorMessageResourceName = "NatureOfEnquiryNameExpressionErrorMessage")]
        [System.Web.Mvc.Remote("CheckNatureOfEnquiryNameExists", "NatureOfEnquiry", AdditionalFields = "RowKey", ErrorMessageResourceType = typeof(ModelResources), ErrorMessageResourceName = "NatureOfEnquiryNameExistsErrorMessage")]
        public string NatureOfEnquiryName { get; set; }
        public bool IsActive { get; set; }
        public string IsActiveText { get { return IsActive ? EduSuiteUIResources.Yes : EduSuiteUIResources.No; } }
    }
}


