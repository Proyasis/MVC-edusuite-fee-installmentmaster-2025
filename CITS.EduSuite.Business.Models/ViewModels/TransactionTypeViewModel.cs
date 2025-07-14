using CITS.EduSuite.Business.Models.Resources;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CITS.EduSuite.Business.Models.ViewModels
{
    public class TransactionTypeViewModel : BaseModel
    {
        public TransactionTypeViewModel()
        {
            Statuses = new List<SelectListModel>();
        }

        public byte RowKey { get; set; }

        [Required(ErrorMessageResourceType = typeof(ModelResources), ErrorMessageResourceName = "TransactionTypeNameRequired")]
        [StringLength(50, ErrorMessageResourceType = typeof(ModelResources), ErrorMessageResourceName = "TransactionTypeNameLengthErrorMessage")]
        [RegularExpression(@"^[0-9 a-zA-Z -()&/*]+$", ErrorMessageResourceType = typeof(ModelResources), ErrorMessageResourceName = "TransactionTypeNameRegularExpressionErrorMessage")]
        public string TransactionTypeName { get; set; }

        [Required(ErrorMessageResourceType = typeof(ModelResources), ErrorMessageResourceName = "TransactionTypeNameLocalRequired")]
        [StringLength(50, ErrorMessageResourceType = typeof(ModelResources), ErrorMessageResourceName = "TransactionTypeNameLocalLengthErrorMessage")]
        [RegularExpression(@"^[0-9 a-zA-Z -()&/*]+$", ErrorMessageResourceType = typeof(ModelResources), ErrorMessageResourceName = "TransactionTypeNameLocalRegularExpressionErrorMessage")]
        public string TransactionTypeNameLocal { get; set; }

        [Required(ErrorMessageResourceType = typeof(ModelResources), ErrorMessageResourceName = "StatusRequired")]
        public short StatusKey { get; set; }

        public string StatusName { get; set; }

        public List<SelectListModel> Statuses { get; set; }

    }

}

