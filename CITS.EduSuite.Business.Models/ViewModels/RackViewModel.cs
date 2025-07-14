using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CITS.EduSuite.Business.Models.Resources;
using System.ComponentModel.DataAnnotations;

namespace CITS.EduSuite.Business.Models.ViewModels
{
    public class RackViewModel : BaseModel
    {
        public RackViewModel()
        {
            Branches = new List<SelectListModel>();
            SubRackDetailsModel = new List<SubRackDetailsModel>();
        }

        public int MasterRowKey { get; set; }

        [Required(ErrorMessageResourceType = typeof(ModelResources), ErrorMessageResourceName = "RackNameRequired")]
        [StringLength(100, ErrorMessageResourceType = typeof(ModelResources), ErrorMessageResourceName = "RackNameLengthErrorMessage")]
        [RegularExpression(@"^[0-9 a-zA-Z ,()&-/]+$", ErrorMessageResourceType = typeof(ModelResources), ErrorMessageResourceName = "RackNameRegularExpressionErrorMessage")]
        public string RackName { get; set; }

        [Required(ErrorMessageResourceType = typeof(ModelResources), ErrorMessageResourceName = "RackCodeRequired")]
        [StringLength(5, ErrorMessageResourceType = typeof(ModelResources), ErrorMessageResourceName = "RackCodeLengthErrorMessage")]
        [RegularExpression(@"^[0-9 a-zA-Z]+$", ErrorMessageResourceType = typeof(ModelResources), ErrorMessageResourceName = "RackCodeRegularExpressionErrorMessage")]
        [System.Web.Mvc.Remote("CheckRackCodeExists", "Rack", AdditionalFields = "MasterRowKey", ErrorMessageResourceType = typeof(ModelResources), ErrorMessageResourceName = "RackCodeExists")]
        public string RackCode { get; set; }

        public bool IsActive { get; set; }
        public string IsActiveText { get { return IsActive ? EduSuiteUIResources.Yes : EduSuiteUIResources.No; } }


        [StringLength(100, ErrorMessageResourceType = typeof(ModelResources), ErrorMessageResourceName = "RackRemarksLengthErrorMessage")]
        public string Remarks { get; set; }

        [Required(ErrorMessageResourceType = typeof(ModelResources), ErrorMessageResourceName = "BranchRequired")]
        public short BranchKey { get; set; }
        public string BranchName { get; set; }
        
        
        public int? SubRackCount { get; set; }

        public List<SelectListModel> Branches { get; set; }


        public List<SubRackDetailsModel> SubRackDetailsModel { get; set; }
    }
    public class SubRackDetailsModel
    {
        public SubRackDetailsModel()
        {

        }
        public long RowKey { get; set; }
        public int RackKey { get; set; }

        [Required(ErrorMessageResourceType = typeof(ModelResources), ErrorMessageResourceName = "SubRackNameRequired")]
        [StringLength(50, ErrorMessageResourceType = typeof(ModelResources), ErrorMessageResourceName = "SubRackNameLengthErrorMessage")]
        [RegularExpression(@"^[0-9 a-zA-Z ,()&-/]+$", ErrorMessageResourceType = typeof(ModelResources), ErrorMessageResourceName = "SubRackNameRegularExpressionErrorMessage")]
        //[System.Web.Mvc.Remote("CheckCertificateTypeExists", "UniversityCertificate", AdditionalFields = "RowKey", ErrorMessageResourceType = typeof(ModelResources), ErrorMessageResourceName = "RackCodeExists")]
        public string SubRackName { get; set; }

        [Required(ErrorMessageResourceType = typeof(ModelResources), ErrorMessageResourceName = "SubRackCodeRequired")]
        [StringLength(5, ErrorMessageResourceType = typeof(ModelResources), ErrorMessageResourceName = "SubRackCodeLengthErrorMessage")]
        [RegularExpression(@"^[0-9 a-zA-Z]+$", ErrorMessageResourceType = typeof(ModelResources), ErrorMessageResourceName = "SubRackCodeRegularExpressionErrorMessage")]
        [System.Web.Mvc.Remote("CheckSubRackCodeExists", "Rack", AdditionalFields = "RowKey", ErrorMessageResourceType = typeof(ModelResources), ErrorMessageResourceName = "SubRackCodeExists")]
        public string SubRackCode { get; set; }



    }
}
