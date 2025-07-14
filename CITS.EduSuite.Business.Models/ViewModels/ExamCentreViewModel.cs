using CITS.EduSuite.Business.Models.Resources;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CITS.EduSuite.Business.Models.ViewModels
{
    public class ExamCentreViewModel : BaseModel
    {
        public ExamCentreViewModel()
        {
            IsActive = true;
          
        }

        public short RowKey { get; set; }

        [Required(ErrorMessageResourceType = typeof(ModelResources), ErrorMessageResourceName = "ExamCentreNameRequired")]
        [StringLength(50, ErrorMessageResourceType = typeof(ModelResources), ErrorMessageResourceName = "ExamCentreNameLengthErrorMessage")]
        [RegularExpression(@"^[0-9 a-zA-Z *()&-/]+$", ErrorMessageResourceType = typeof(ModelResources), ErrorMessageResourceName = "ExamCentreNameExpressionErrorMessage")]
        [System.Web.Mvc.Remote("CheckExamCentreNameExists", "ExamCentre", AdditionalFields = "RowKey", ErrorMessageResourceType = typeof(ModelResources), ErrorMessageResourceName = "ExamCentreNameExistsErrorMessage")]
        public string ExamCentreName { get; set; }

        [Required(ErrorMessageResourceType = typeof(ModelResources), ErrorMessageResourceName = "ExamCentreCodeRequired")]
        [StringLength(10, ErrorMessageResourceType = typeof(ModelResources), ErrorMessageResourceName = "ExamCentreCodeLengthErrorMessage")]
        [RegularExpression(@"^[0-9 a-zA-Z *()&-/]+$", ErrorMessageResourceType = typeof(ModelResources), ErrorMessageResourceName = "ExamCentreCodeExpressionErrorMessage")]
        [System.Web.Mvc.Remote("CheckExamCentreCodeExists", "ExamCentre", AdditionalFields = "RowKey", ErrorMessageResourceType = typeof(ModelResources), ErrorMessageResourceName = "ExamCentreCodeExistsErrorMessage")]
        public string ExamCentreCode { get; set; }
        public bool IsActive { get; set; }
        public string IsActiveText { get { return IsActive ? EduSuiteUIResources.Yes : EduSuiteUIResources.No; } }
    }
}
