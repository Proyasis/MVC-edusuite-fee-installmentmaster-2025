using CITS.EduSuite.Business.Models.Resources;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CITS.EduSuite.Business.Models.ViewModels
{
    public class PublisherViewModel : BaseModel
    {
        public PublisherViewModel()
        {
            IsActive = true;
        }

        public Int32 RowKey { get; set; }

        [Required(ErrorMessageResourceType = typeof(ModelResources), ErrorMessageResourceName = "PublisherNameRequierd")]
        [StringLength(200, ErrorMessageResourceType = typeof(ModelResources), ErrorMessageResourceName = "PublisherNameLengthErrorMessage")]
        [RegularExpression(@"^[0-9 a-zA-Z -()&/]+$", ErrorMessageResourceType = typeof(ModelResources), ErrorMessageResourceName = "PublisherNameRegularExpressionErrorMessage")]
        public string PublisherName { get; set; }

        [StringLength(20, ErrorMessageResourceType = typeof(ModelResources), ErrorMessageResourceName = "PublisherPhoneLengthErrorMessage")]
        [RegularExpression(@"^[0-9 +()&/]+$", ErrorMessageResourceType = typeof(ModelResources), ErrorMessageResourceName = "PublisherPhoneRegularExpressionErrorMessage")]
        public string PublisherPhone { get; set; }

        [StringLength(250, ErrorMessageResourceType = typeof(ModelResources), ErrorMessageResourceName = "PublisherNameLengthErrorMessage")]
        [RegularExpression(@"^[0-9 a-zA-Z -()&/\s]+$", ErrorMessageResourceType = typeof(ModelResources), ErrorMessageResourceName = "PublisherNameRegularExpressionErrorMessage")]
        public string PublisherAddress { get; set; }

        public bool IsActive { get; set; }

        public string IsActiveText { get { return IsActive ? EduSuiteUIResources.Yes : EduSuiteUIResources.No; } }
        public string SearchText { get; set; }

        public int PageIndex { get; set; }
        public int PageSize { get; set; }
        public string SortBy { get; set; }
        public string SortOrder { get; set; }
    }
}
