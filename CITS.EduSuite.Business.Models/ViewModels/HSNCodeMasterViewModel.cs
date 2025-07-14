using CITS.EduSuite.Business.Models.Resources;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CITS.EduSuite.Business.Models.ViewModels
{
    public class HSNCodeMasterViewModel : BaseModel
    {
        public HSNCodeMasterViewModel()
        {
            //Statuses = new List<SelectListModel>();
            //StatusKey = DbConstants.StatusKey.Active;
            IsActive = true;
        }
        public long RowKey { get; set; }

        [Required(ErrorMessageResourceType = typeof(ModelResources), ErrorMessageResourceName = "HSNCodeProductNameRequired")]
        [StringLength(100, ErrorMessageResourceType = typeof(ModelResources), ErrorMessageResourceName = "HSNCodeProductNameLength")]
        public string ProductName { get; set; }

        //[Required(ErrorMessageResourceType = typeof(ModelResources), ErrorMessageResourceName = "HSNCodeProductDescRequired")]
        //[StringLength(800, ErrorMessageResourceType = typeof(ModelResources), ErrorMessageResourceName = "HSNCodeProductDescRequired")]
        public string ProductDescription { get; set; }

        [Required(ErrorMessageResourceType = typeof(ModelResources), ErrorMessageResourceName = "HSNCodeRequired")]
        [StringLength(8, ErrorMessageResourceType = typeof(ModelResources), ErrorMessageResourceName = "HSNCodeLength")]
        [RegularExpression(@"^-*[0-9,\.]+$", ErrorMessageResourceType = typeof(ModelResources), ErrorMessageResourceName = "NumericExpressionErrorMessage")]
        public string HSNSACCode { get; set; }

        [Required(ErrorMessageResourceType = typeof(ModelResources), ErrorMessageResourceName = "TaxPercentageRequired")]
        [RegularExpression(@"^\d{0,3}(\.\d{1,2})?$", ErrorMessageResourceType = typeof(ModelResources), ErrorMessageResourceName = "Decimal3RegularExpressionErrorMessage")]       
        public decimal? HSNSGSTPer { get; set; }
        
        [RegularExpression(@"^\d{0,3}(\.\d{1,2})?$", ErrorMessageResourceType = typeof(ModelResources), ErrorMessageResourceName = "Decimal3RegularExpressionErrorMessage")]
        [Required(ErrorMessageResourceType = typeof(ModelResources), ErrorMessageResourceName = "TaxPercentageRequired")]
        public decimal? HSNCGSTPer { get; set; }

        [RegularExpression(@"^\d{0,3}(\.\d{1,2})?$", ErrorMessageResourceType = typeof(ModelResources), ErrorMessageResourceName = "Decimal3RegularExpressionErrorMessage")]
        public decimal? HSNIGSTPer { get; set; }

        //[Required(ErrorMessageResourceType = typeof(ModelResources), ErrorMessageResourceName = "StatusRequired")]
        //public byte StatusKey { get; set; }

        public string StatusName { get; set; }
        //public List<SelectListModel> Statuses { get; set; }
        public bool IsActive { get; set; }

    }
}
