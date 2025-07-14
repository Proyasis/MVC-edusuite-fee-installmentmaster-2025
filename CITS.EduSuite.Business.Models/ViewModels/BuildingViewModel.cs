using CITS.EduSuite.Business.Models.Resources;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CITS.EduSuite.Business.Models.ViewModels
{
    public class BuildingViewModel : BaseModel
    {
        public BuildingViewModel()
        {
            BuildingDetails = new List<BuildingDetailsModel>();
            IsActive = true;
        }
        public int RowKey { get; set; }

        [Required(ErrorMessageResourceType = typeof(ModelResources), ErrorMessageResourceName = "BuildingNameRequired")]
        [StringLength(100, ErrorMessageResourceType = typeof(ModelResources), ErrorMessageResourceName = "BuildingNameLengthErrorMessage")]
        [RegularExpression(@"^[0-9 a-zA-Z *()&-/]+$", ErrorMessageResourceType = typeof(ModelResources), ErrorMessageResourceName = "BuildingNameExpressionErrorMessage")]
        public string BuildingMasterName { get; set; }
        public bool IsActive { get; set; }
        public string IsActiveText { get { return IsActive ? EduSuiteUIResources.Yes : EduSuiteUIResources.No; } }

        [Required(ErrorMessageResourceType = typeof(ModelResources), ErrorMessageResourceName = "RoomCountRequired")]       
        [RegularExpression(@"^[0-9]+$", ErrorMessageResourceType = typeof(ModelResources), ErrorMessageResourceName = "RoomCountExpressionErrorMessage")]
        public int? RoomCount { get; set; }
        public List<BuildingDetailsModel> BuildingDetails { get; set; }
    }
    public class BuildingDetailsModel
    {
        public BuildingDetailsModel()
        {
            IsActive = true;
        }
        public long RowKey { get; set; }
        public int BuildingMasterKey { get; set; }

        [Required(ErrorMessageResourceType = typeof(ModelResources), ErrorMessageResourceName = "RequiredErrorMessage")]
        [StringLength(100, ErrorMessageResourceType = typeof(ModelResources), ErrorMessageResourceName = "LengthErrorMessage")]
        [RegularExpression(@"^[0-9 a-zA-Z *()&-/]+$", ErrorMessageResourceType = typeof(ModelResources), ErrorMessageResourceName = "RegularExpressionErrorMessage")]
        [System.Web.Mvc.Remote("CheckRoomName", "Building", ErrorMessageResourceType = typeof(ModelResources), ErrorMessageResourceName = "ExistsErrorMessage")]
        [Display(Name = "RoomName", ResourceType = typeof(EduSuiteUIResources))]
        public string BuildingDetailsName { get; set; }
        public bool IsActive { get; set; }

    }

}
