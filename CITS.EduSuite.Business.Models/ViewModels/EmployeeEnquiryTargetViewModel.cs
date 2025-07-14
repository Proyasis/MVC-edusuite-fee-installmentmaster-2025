using CITS.EduSuite.Business.Models.Resources;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CITS.EduSuite.Business.Models.ViewModels
{
    public class EmployeeEnquiryTargetViewModel : BaseModel
    {
        public EmployeeEnquiryTargetViewModel()
        {
            IsActive = true;
            Branches = new List<SelectListModel>();
            EmployeeEnquiryTargetDetailsViewModel = new List<EmployeeEnquiryTargetDetailsViewModel>();
            TargetMonth = new List<SelectListModel>();
        }

        public long RowKey { get; set; }
        public long EmployeeKey { get; set; }
        public string EmployeeName { get; set; }

        //[StringLength(5, ErrorMessageResourceType = typeof(ModelResources), ErrorMessageResourceName = "LengthErrorMessage")]
        [RegularExpression(@"^[0-9]+$", ErrorMessageResourceType = typeof(ModelResources), ErrorMessageResourceName = "RegularExpressionDigitErrorMessage")]
        [Required(ErrorMessageResourceType = typeof(ModelResources), ErrorMessageResourceName = "RequiredErrorMessage")]
        [Display(Name = "CommonTarget", ResourceType = typeof(EduSuiteUIResources))]
        public int? CommonTarget { get; set; }
        public bool AllowMonthlyTarget { get; set; }
        public bool IsActive { get; set; }
        public string EmployeeCode { get; set; }
        public string MobileNumber { get; set; }
        public string DepartmentName { get; set; }
        public string DesignationName { get; set; }
        public string EmployeeStatusName { get; set; }
        public short? EmployeeStatusKey { get; set; }
        public short? BranchKey { get; set; }
        public string BranchName { get; set; }

        public int PageIndex { get; set; }
        public int PageSize { get; set; }
        public string SortBy { get; set; }
        public string SortOrder { get; set; }

        public List<SelectListModel> Branches { get; set; }
        public List<EmployeeEnquiryTargetDetailsViewModel> EmployeeEnquiryTargetDetailsViewModel { get; set; }
        public List<SelectListModel> TargetMonth { get; set; }
    }
    public class EmployeeEnquiryTargetDetailsViewModel : BaseModel
    {
        public long RowKey { get; set; }
        public long EnquiryTargetKey { get; set; }
        public int TargetMonth { get; set; }
        public int TargetYear { get; set; }

        [Required(ErrorMessageResourceType = typeof(ModelResources), ErrorMessageResourceName = "InstallmentMonthRequired")]
        [Display(Name = "MonthlyTarget", ResourceType = typeof(EduSuiteUIResources))]
        public string TargetYearMonth { get { return (TargetYear != 0 ? new DateTime(TargetYear, TargetMonth, 1).ToString("yyyy-MM") : null); } }


        //[StringLength(5, ErrorMessageResourceType = typeof(ModelResources), ErrorMessageResourceName = "LengthErrorMessage")]
        [RegularExpression(@"^[0-9]+$", ErrorMessageResourceType = typeof(ModelResources), ErrorMessageResourceName = "RegularExpressionDigitErrorMessage")]
        [Required(ErrorMessageResourceType = typeof(ModelResources), ErrorMessageResourceName = "RequiredErrorMessage")]
        [Display(Name = "MonthlyTarget", ResourceType = typeof(EduSuiteUIResources))]
        public int? MonthlyTarget { get; set; }
        public string Remarks { get; set; }
    }
}
