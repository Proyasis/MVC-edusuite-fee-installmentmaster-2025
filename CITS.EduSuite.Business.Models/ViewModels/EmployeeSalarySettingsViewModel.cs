using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CITS.EduSuite.Business.Models.Resources;


namespace CITS.EduSuite.Business.Models.ViewModels
{
    public class EmployeeSalarySettingsViewModel : BaseModel
    {

        public EmployeeSalarySettingsViewModel()
        {

            EmployeeSalaryDetails = new List<EmployeeSalaryDetailViewModel>();
        }

        public long EmployeeKey { get; set; }

        public long ProvidentFundKey { get; set; }

        [Required(ErrorMessageResourceType = typeof(ModelResources), ErrorMessageResourceName = "ProvidentFundType_Required")]
        public string ProvidentFundType { get; set; }

        [Required(ErrorMessageResourceType = typeof(ModelResources), ErrorMessageResourceName = "EmployeeShare_Required")]
        [RegularExpression(@"^\d{0,10}(\.\d{1,2})?$", ErrorMessageResourceType = typeof(ModelResources), ErrorMessageResourceName = "AmountExpressionErrorMessage")]
        public decimal? EmployeeShare { get; set; }

        [Required(ErrorMessageResourceType = typeof(ModelResources), ErrorMessageResourceName = "EmployerShare_Required")]
        [RegularExpression(@"^\d{0,10}(\.\d{1,2})?$", ErrorMessageResourceType = typeof(ModelResources), ErrorMessageResourceName = "AmountExpressionErrorMessage")]
        public decimal? EmployerShare { get; set; }
        public List<EmployeeSalaryDetailViewModel> EmployeeSalaryDetails { get; set; }

    }

}
