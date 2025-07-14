using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using CITS.EduSuite.Business.Models.Resources;

namespace CITS.EduSuite.Business.Models.ViewModels
{
    public class EnquiryCallStatusViewModel : BaseModel
    {
        public EnquiryCallStatusViewModel()
        {
            IsActive = true;
            IsDuration = false;
            EnquiryStatusList = new List<SelectListModel>();
            ShowInMenuKeys = new List<int>();
            MenuList = new List<SelectListModel>();
        }

        public int RowKey { get; set; }


        [Required(ErrorMessageResourceType = typeof(ModelResources), ErrorMessageResourceName = "EnquiryCallStatusNameRequired")]
        [StringLength(50, ErrorMessageResourceType = typeof(ModelResources), ErrorMessageResourceName = "EnquiryCallStatusNameLengthErrorMessage")]
        [RegularExpression(@"^[0-9 a-zA-Z *()&-/]+$", ErrorMessageResourceType = typeof(ModelResources), ErrorMessageResourceName = "EnquiryCallStatusNameRegularExpressionErrorMessage")]
        public string EnquiryCallStatusName { get; set; }

        [Required(ErrorMessageResourceType = typeof(ModelResources), ErrorMessageResourceName = "EnquiryStatusRequired")]
        public short EnquiryStatusKey { get; set; }
        public string EnquiryStatusName { get; set; }
        public short DisplayOrder { get; set; }
        public bool IsActive { get; set; }

        public bool IsDuration { get; set; }

        public string ShowInMenuKeysList { get; set; }
        public List<SelectListModel> EnquiryStatusList { get; set; }
        public List<SelectListModel> MenuList { get; set; }
        public List<Int32> ShowInMenuKeys { get; set; }
    }
}
