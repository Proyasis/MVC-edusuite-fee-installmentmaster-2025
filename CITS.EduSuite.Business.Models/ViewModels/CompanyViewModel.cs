using CITS.EduSuite.Business.Models.Resources;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace CITS.EduSuite.Business.Models.ViewModels
{
    public class CompanyViewModel : BaseModel
    {
        public short RowKey { get; set; }

        [Required(ErrorMessageResourceType = typeof(ModelResources), ErrorMessageResourceName = "RequiredErrorMessage")]
        [StringLength(100, ErrorMessageResourceType = typeof(ModelResources), ErrorMessageResourceName = "LengthErrorMessage")]
        [RegularExpression(@"^[0-9 a-zA-Z *()&-/]+$", ErrorMessageResourceType = typeof(ModelResources), ErrorMessageResourceName = "RegularExpressionErrorMessage")]
        [Display(Name = "CompanyName", ResourceType = typeof(EduSuiteUIResources))]
        public string CompanyName { get; set; }
       
        [StringLength(100, ErrorMessageResourceType = typeof(ModelResources), ErrorMessageResourceName = "LengthErrorMessage")]
        [RegularExpression(@"^[0-9 a-zA-Z *()&-/]+$", ErrorMessageResourceType = typeof(ModelResources), ErrorMessageResourceName = "RegularExpressionErrorMessage")]
        [Display(Name = "CompanySubName", ResourceType = typeof(EduSuiteUIResources))]
        public string CompanySubName { get; set; }
        
        [StringLength(100, ErrorMessageResourceType = typeof(ModelResources), ErrorMessageResourceName = "LengthErrorMessage")]
        [RegularExpression(@"^[0-9 a-zA-Z *()&-/]+$", ErrorMessageResourceType = typeof(ModelResources), ErrorMessageResourceName = "RegularExpressionErrorMessage")]
        [Display(Name = "WebSite", ResourceType = typeof(EduSuiteUIResources))]
        public string Website { get; set; }
        public bool HasMultipleBranches { get; set; }
        public string CompanyLogo { get; set; }
        public string GSTINNumber { get; set; }
        public HttpPostedFileBase PhotoFile { get; set; }
        public string CompanyLogoPath { get; set; }
    }
}
