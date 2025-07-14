using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using CITS.EduSuite.Business.Models.Resources;


namespace CITS.EduSuite.Business.Models.ViewModels
{
    public class BatchViewModel : BaseModel
    {
        public BatchViewModel()
        {
            IsActive = true;
        }

        public short Rowkey { get; set; }

        [Required(ErrorMessageResourceType = typeof(ModelResources), ErrorMessageResourceName = "RequiredErrorMessage")]
        [StringLength(50, ErrorMessageResourceType = typeof(ModelResources), ErrorMessageResourceName = "LengthErrorMessage")]
        [RegularExpression(@"^[0-9 a-zA-Z *()&-/]+$", ErrorMessageResourceType = typeof(ModelResources), ErrorMessageResourceName = "RegularExpressionErrorMessage")]
        [Display(Name = "Name", ResourceType = typeof(EduSuiteUIResources))]
        public string BatchName { get; set; }

        [Required(ErrorMessageResourceType = typeof(ModelResources), ErrorMessageResourceName = "RequiredErrorMessage")]
        [StringLength(10, ErrorMessageResourceType = typeof(ModelResources), ErrorMessageResourceName = "LengthErrorMessage")]
        [System.Web.Mvc.Remote("CheckBatchCodeExists", "Batch", AdditionalFields = "Rowkey", ErrorMessageResourceType = typeof(ModelResources), ErrorMessageResourceName = "BatchCodeExists")]
        [Display(Name = "Code", ResourceType = typeof(EduSuiteUIResources))]
        public string BatchCode { get; set; }
        public bool IsActive { get; set; }
        public string IsActiveText { get { return IsActive ? EduSuiteUIResources.Yes : EduSuiteUIResources.No; } }

        //[StringLength(4, ErrorMessageResourceType = typeof(ModelResources), ErrorMessageResourceName = "LengthErrorMessage")]
        [RegularExpression(@"^\d{4}$", ErrorMessageResourceType = typeof(ModelResources), ErrorMessageResourceName = "RegularExpressionDigitErrorMessage")]
        [Display(Name = "DurationFromYear", ResourceType = typeof(EduSuiteUIResources))]
        public short? DurationFromYear { get; set; }

        //[StringLength(4, ErrorMessageResourceType = typeof(ModelResources), ErrorMessageResourceName = "LengthErrorMessage")]
       [RegularExpression(@"^\d{4}$", ErrorMessageResourceType = typeof(ModelResources), ErrorMessageResourceName = "RegularExpressionDigitErrorMessage")]
        [Display(Name = "DurationToYear", ResourceType = typeof(EduSuiteUIResources))]
        public short? DurationToYear { get; set; }

        public DateTime? DurationFromDate { get; set; }
        public DateTime? DurationToDate { get; set; }


    }
}
