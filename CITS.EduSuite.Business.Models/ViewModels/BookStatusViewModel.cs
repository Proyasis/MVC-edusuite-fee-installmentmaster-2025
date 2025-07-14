using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CITS.EduSuite.Business.Models.Resources;
using System.ComponentModel.DataAnnotations;

namespace CITS.EduSuite.Business.Models.ViewModels
{
    public class BookStatusViewModel : BaseModel
    {
        public BookStatusViewModel()
        {
            CanIssue = true;
            IsActive = true;
            Statuses = new List<SelectListModel>();
        }

        public byte RowKey { get; set; }

        [Required(ErrorMessageResourceType = typeof(ModelResources), ErrorMessageResourceName = "BookStatusNameRequired")]
        [StringLength(100, ErrorMessageResourceType = typeof(ModelResources), ErrorMessageResourceName = "BookStatusNameLengthErrorMessage")]
        [RegularExpression(@"^[0-9 a-zA-Z -()*&/]+$", ErrorMessageResourceType = typeof(ModelResources), ErrorMessageResourceName = "BookStatusNameRegularExpressionErrorMessage")]
        public string BookStatusName { get; set; }


        public short StatusKey { get; set; }

        public string StatusName { get; set; }

        public bool IsActive { get; set; }
        public string IsActiveText { get { return IsActive ? EduSuiteUIResources.Yes : EduSuiteUIResources.No; } }

        public bool CanIssue { get; set; }

        public List<SelectListModel> Statuses { get; set; }
    }
}
