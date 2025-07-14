using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CITS.EduSuite.Business.Models.Resources;

namespace CITS.EduSuite.Business.Models.ViewModels
{
    public class BookIssueTypeViewModel:BaseModel
    {
        public BookIssueTypeViewModel()
        {
            IsActive = true;
        }
        public byte RowKey { get; set; }

        [Required(ErrorMessageResourceType = typeof(ModelResources), ErrorMessageResourceName = "BookIssueTypeNameRequired")]
        [StringLength(100, ErrorMessageResourceType = typeof(ModelResources), ErrorMessageResourceName = "BookIssueTypeNameLengthErrorMessage")]
        [RegularExpression(@"^[0-9 a-zA-Z ]+$", ErrorMessageResourceType = typeof(ModelResources), ErrorMessageResourceName = "BookIssueTypeNameExpressionErrorMessage")]
        public string BookIssueTypeName { get; set; }

     

        public bool IsActive { get; set; }

        public string IsActiveText { get { return IsActive ? EduSuiteUIResources.Yes : EduSuiteUIResources.No; } }



    }
}
