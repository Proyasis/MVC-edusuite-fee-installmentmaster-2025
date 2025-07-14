using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using CITS.EduSuite.Business.Models.Resources;

namespace CITS.EduSuite.Business.Models.ViewModels
{
    public class ScholarshipTypeViewModel : BaseModel
    {
        public ScholarshipTypeViewModel()
        {
            IsActive = true;
        }
        public short RowKey { get; set; }

        [Required(ErrorMessageResourceType = typeof(ModelResources), ErrorMessageResourceName = "ScholarShipTypeNameRequired")]
        [StringLength(80, ErrorMessageResourceType = typeof(ModelResources), ErrorMessageResourceName = "ScholarShipTypeNameLengthErrorMessage")]
        [RegularExpression(@"^[0-9 a-zA-Z *()&-/]+$", ErrorMessageResourceType = typeof(ModelResources), ErrorMessageResourceName = "ScholarShipTypeNameExpressionErrorMessage")]
        [System.Web.Mvc.Remote("CheckScholarShipTypeNameExists", "ScholarshipType", AdditionalFields = "RowKey", ErrorMessageResourceType = typeof(ModelResources), ErrorMessageResourceName = "ScholarShipTypeNameExistsErrorMessage")]

        public string ScholarShipTypeName { get; set; }


        [Required(ErrorMessageResourceType = typeof(ModelResources), ErrorMessageResourceName = "ScholarShipTypeCodeRequired")]
        [StringLength(10, ErrorMessageResourceType = typeof(ModelResources), ErrorMessageResourceName = "ScholarShipTypeCodeLengthErrorMessage")]
        [RegularExpression(@"^[0-9 a-zA-Z *()&-/]+$", ErrorMessageResourceType = typeof(ModelResources), ErrorMessageResourceName = "ScholarShipTyperCodeExpressionErrorMessage")]
        [System.Web.Mvc.Remote("CheckScholarShipTypeCodeExists", "ScholarshipType", AdditionalFields = "RowKey", ErrorMessageResourceType = typeof(ModelResources), ErrorMessageResourceName = "ScholarShipTypeCodeExistsErrorMessage")]

        public string ScholarShipTypeCode { get; set; }
        public bool IsActive { get; set; }
        public string IsActiveText { get; set; }

        

    }
}
