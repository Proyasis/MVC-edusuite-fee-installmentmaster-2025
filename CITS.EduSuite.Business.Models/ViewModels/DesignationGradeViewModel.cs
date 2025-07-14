using CITS.EduSuite.Business.Models.Resources;
using CITS.Validations;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CITS.EduSuite.Business.Models.ViewModels
{
    public class DesignationGradeViewModel : BaseModel
    {
        public DesignationGradeViewModel()
        {
            Designations = new List<SelectListModel>();
            DesignationGradeDetails = new List<DesignationGradeDetailViewModel>();
        }

        public int RowKey { get; set; }

        [Required(ErrorMessageResourceType = typeof(ModelResources), ErrorMessageResourceName = "RequiredErrorMessage")]
        [Display(Name = "Designation", ResourceType = typeof(EduSuiteUIResources))]
        public short DesignationKey { get; set; }
        public string DesignationName { get; set; }

        [Required(ErrorMessageResourceType = typeof(ModelResources), ErrorMessageResourceName = "RequiredErrorMessage")]
        [StringLength(200, ErrorMessageResourceType = typeof(ModelResources), ErrorMessageResourceName = "LengthErrorMessage")]
        [Display(Name = "Grade", ResourceType = typeof(EduSuiteUIResources))]

        public string DesignationGradeName { get; set; }

        [Required(ErrorMessageResourceType = typeof(ModelResources), ErrorMessageResourceName = "RequiredErrorMessage")]
        [RegularExpression(@"^\d{0,10}(\.\d{1,2})?$", ErrorMessageResourceType = typeof(ModelResources), ErrorMessageResourceName = "RegularExpressionDigitErrorMessage")]
        [EqualTo("TotalIncludedAmount", ErrorMessageResourceType = typeof(ModelResources), ErrorMessageResourceName = "MonthlySalaryCompareErrorMessage")]
        [Display(Name = "TotalSalary", ResourceType = typeof(EduSuiteUIResources))]

        public decimal? MonthlySalary { get; set; }

        public string ColumnLetter { get; set; }


        public decimal? TotalIncludedAmount { get; set; }

        public List<DesignationGradeDetailViewModel> DesignationGradeDetails { get; set; }
        public List<SelectListModel> Designations { get; set; }
    }
    public class DesignationGradeDetailViewModel
    {
        public long RowKey { get; set; }
        public int DesignationGradeKey { get; set; }
        public string DesignationGradeName { get; set; }
        public int SalaryHeadKey { get; set; }
        public string SalaryHeadCode { get; set; }
        public string SalaryHeadName { get; set; }

        public int SalaryHeadTypeKey { get; set; }
        public string SalaryHeadTypeName { get; set; }
        public decimal AmountUnit { get; set; }
        public string Formula { get; set; }
        public string Applicable { get; set; }
        public bool IsInclude { get; set; }
        public bool IsFixed { get; set; }

    }
}
