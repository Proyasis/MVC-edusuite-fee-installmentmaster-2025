using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CITS.EduSuite.Business.Models.Resources;

namespace CITS.EduSuite.Business.Models.ViewModels
{
   public class AdditionalSalaryComponentViewModel:BaseModel
    {
       public AdditionalSalaryComponentViewModel()
        {
            AdditionalComponentTypes = new List<SelectListModel>();
        }

       public long RowKey { get; set; }

        public long EmployeeSalaryMasterKey { get; set; }


        [Required(ErrorMessageResourceType = typeof(ModelResources), ErrorMessageResourceName = "AdditionalComponentTypeRequired")]
        [System.Web.Mvc.Remote("CheckEmployeeSalarySettingsTypeExists", "EmployeeSalarySettings", AdditionalFields = "EmployeeKey,RowKey", ErrorMessageResourceType = typeof(ModelResources), ErrorMessageResourceName = "EmployeeAdditionalComponentExists")]
      
        public short AdditionalComponentTypeKey { get; set; }
        public string AdditionalComponentTypeName { get; set; }

        [Required(ErrorMessageResourceType = typeof(ModelResources), ErrorMessageResourceName = "AdditionalComponentAmountRequired")]
        [RegularExpression(@"^\d{0,10}(\.\d{1,2})?$", ErrorMessageResourceType = typeof(ModelResources), ErrorMessageResourceName = "AmountExpressionErrorMessage")]
        public decimal? Amount { get; set; }
        public string OperationType { get; set; }

       [Required(ErrorMessageResourceType = typeof(ModelResources), ErrorMessageResourceName = "SalaryUnityType_Required")]
        public string UnitType { get; set; }
        public List<SelectListModel> AdditionalComponentTypes { get; set; }

    }
    }
