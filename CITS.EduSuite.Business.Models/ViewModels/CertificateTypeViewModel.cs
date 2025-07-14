using CITS.EduSuite.Business.Models.Resources;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CITS.EduSuite.Business.Models.ViewModels
{
    public class CertificateTypeViewModel : BaseModel
    {
        public CertificateTypeViewModel()
        {
            IsActive = true;          
        }
        public short RowKey { get; set; }
        
        [System.Web.Mvc.Remote("CheckCertificateTypeNameExists", "CertificateType", AdditionalFields = "RowKey", ErrorMessageResourceType = typeof(ModelResources), ErrorMessageResourceName = "ExistsErrorMessage")]
        [StringLength(50, ErrorMessageResourceType = typeof(ModelResources), ErrorMessageResourceName = "LengthErrorMessage")]
        [RegularExpression(@"^[0-9 a-zA-Z *()&-/]+$", ErrorMessageResourceType = typeof(ModelResources), ErrorMessageResourceName = "RegularExpressionErrorMessage")]
        [Required(ErrorMessageResourceType = typeof(ModelResources), ErrorMessageResourceName = "RequiredErrorMessage")]
        [Display(Name = "CertificateType", ResourceType = typeof(EduSuiteUIResources))]
        public string CertificateTypeName { get; set; }
        public bool IsActive { get; set; }
        public string IsActiveText { get { return IsActive ? EduSuiteUIResources.Yes : EduSuiteUIResources.No; } }
    }
}

