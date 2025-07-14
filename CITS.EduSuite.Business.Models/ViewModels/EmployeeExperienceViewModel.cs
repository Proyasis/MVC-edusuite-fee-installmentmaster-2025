using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using CITS.EduSuite.Business.Models.Resources;
using CITS.Validations;

namespace CITS.EduSuite.Business.Models.ViewModels
{
    public class EmployeeExperienceViewModel:BaseModel
    {

      public EmployeeExperienceViewModel()
        {
            EmployeeExperiences = new List<ExperienceViewModel>();
        }
        public long EmployeeKey { get; set; }
        public string EmployeeCode { get; set; }

        public List<ExperienceViewModel> EmployeeExperiences { get; set; }
    }
    public class ExperienceViewModel
    {
        public long RowKey { get; set; }

        [Required(ErrorMessageResourceType = typeof(ModelResources), ErrorMessageResourceName = "CompanyNameRequired")]
        [StringLength(100, ErrorMessageResourceType = typeof(ModelResources), ErrorMessageResourceName = "CompanyNameLengthErrorMessage")]
        [RegularExpression(@"^[0-9 a-zA-Z ,()&-/\s]+$", ErrorMessageResourceType = typeof(ModelResources), ErrorMessageResourceName = "CompanyNameExpressionErrorMessage")]
        public string CompanyName { get; set; }

        [StringLength(200, ErrorMessageResourceType = typeof(ModelResources), ErrorMessageResourceName = "CompanyAddressLengthErrorMessage")]
        [RegularExpression(@"^[0-9 a-zA-Z ,()&-/\s]+$", ErrorMessageResourceType = typeof(ModelResources), ErrorMessageResourceName = "CompanyAddressExpressionErrorMessage")]
        public string CompanyAddress { get; set; }

        [StringLength(20, ErrorMessageResourceType = typeof(ModelResources), ErrorMessageResourceName = "PhoneNumberLengthErrorMessage")]
        [RegularExpression(@"^[0-9]+$", ErrorMessageResourceType = typeof(ModelResources), ErrorMessageResourceName = "PhoneNumberExpressionErrorMessage")]
        public string CompanyPhoneNumber { get; set; }

        [StringLength(100, ErrorMessageResourceType = typeof(ModelResources), ErrorMessageResourceName = "JobFieldLengthErrorMessage")]
        [RegularExpression(@"^[0-9 a-zA-Z ,()&-/\s]+$", ErrorMessageResourceType = typeof(ModelResources), ErrorMessageResourceName = "JobFieldExpressionErrorMessage")]
        public string JobField { get; set; }

        [StringLength(100, ErrorMessageResourceType = typeof(ModelResources), ErrorMessageResourceName = "JobPostionLengthErrorMessage")]
        [RegularExpression(@"^[0-9 a-zA-Z ,()&-/\s]+$", ErrorMessageResourceType = typeof(ModelResources), ErrorMessageResourceName = "JobPostionExpressionErrorMessage")]
        public string JobPostion { get; set; }

        //[Required(ErrorMessageResourceType = typeof(ModelResources), ErrorMessageResourceName = "ExperienceStartDateRequired")]
        public DateTime? ExperienceStartDate { get; set; }

        //[Required(ErrorMessageResourceType = typeof(ModelResources), ErrorMessageResourceName = "ExperianceEndDateRequired")]
        [GreaterThanOrEqualTo("ExperienceStartDate", PassOnNull = true, ErrorMessageResourceType = typeof(ModelResources), ErrorMessageResourceName = "ExperianceEndDateCompareErrorMessage")]
        public DateTime? ExperianceEndDate { get; set; }

        [RegularExpression(@"^\d{0,10}(\.\d{1,2})?$", ErrorMessageResourceType = typeof(ModelResources), ErrorMessageResourceName = "StartingSalaryExpressionErrorMessage")]
        public decimal? StartingSalary { get; set; }

        [RegularExpression(@"^\d{0,10}(\.\d{1,2})?$", ErrorMessageResourceType = typeof(ModelResources), ErrorMessageResourceName = "EndingSalaryExpressionErrorMessage")]
        public decimal? EndingSalary { get; set; }

        [StringLength(100, ErrorMessageResourceType = typeof(ModelResources), ErrorMessageResourceName = "ContactPersonNameLengthErrorMessage")]
        [RegularExpression(@"^[0-9 a-zA-Z ,()&-/\s]+$", ErrorMessageResourceType = typeof(ModelResources), ErrorMessageResourceName = "ContactPersonNameExpressionErrorMessage")]
        public string ContactPersonName { get; set; }

        [StringLength(20, ErrorMessageResourceType = typeof(ModelResources), ErrorMessageResourceName = "PhoneNumberLengthErrorMessage")]
        [RegularExpression(@"^[0-9]+$", ErrorMessageResourceType = typeof(ModelResources), ErrorMessageResourceName = "PhoneNumberExpressionErrorMessage")]
        public string ContactPersonNumber { get; set; }

        [StringLength(100, ErrorMessageResourceType = typeof(ModelResources), ErrorMessageResourceName = "ContactPersonDesignationLengthErrorMessage")]
        [RegularExpression(@"^[0-9 a-zA-Z ,()&-/\s]+$", ErrorMessageResourceType = typeof(ModelResources), ErrorMessageResourceName = "ContactPersonDesignationExpressionErrorMessage")]
        public string ContactPersonDesignation { get; set; }

        [StringLength(100, ErrorMessageResourceType = typeof(ModelResources), ErrorMessageResourceName = "EmployeeExperienceRemarksLengthErrorMessage")]
        public string Remarks { get; set; }
        public HttpPostedFileBase AttanchedFile { get; set; }
        public string AttanchedFileName { get; set; }
        public string AttanchedFileNamePath { get; set; }
    }
}
