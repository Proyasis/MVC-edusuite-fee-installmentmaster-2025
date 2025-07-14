using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using CITS.EduSuite.Business.Models.Resources;


namespace CITS.EduSuite.Business.Models.ViewModels
{
 public class DistrictViewModel:BaseModel
    {
     public DistrictViewModel()
        {
            Provinces = new List<SelectListModel>();
            Countries = new List<SelectListModel>();
            IsActive = true;
        }


     public int RowKey { get; set; }
     [Required(ErrorMessageResourceType = typeof(ModelResources), ErrorMessageResourceName = "DistrictNameRequired")]
     [StringLength(100, ErrorMessageResourceType = typeof(ModelResources), ErrorMessageResourceName = "DistrictNameLengthErrorMessage")]
     [RegularExpression(@"^[0-9 a-zA-Z *()&-/]+$", ErrorMessageResourceType = typeof(ModelResources), ErrorMessageResourceName = "DistrictNameRegularExpressionErrorMessage")]
     public string Districtname { get; set; }
 
     [StringLength(100, ErrorMessageResourceType = typeof(ModelResources), ErrorMessageResourceName = "DistrictnameLocalLengthErrorMessage")]
     [RegularExpression(@"^[0-9 a-zA-Z *()&-/]+$", ErrorMessageResourceType = typeof(ModelResources), ErrorMessageResourceName = "DistrictnameLocalRegularExpressionErrorMessage")]
     public string DistrictnameLocal { get; set; }

     [Required(ErrorMessageResourceType = typeof(ModelResources), ErrorMessageResourceName = "CountryRequired")]
     public int CountryKey { get; set; }
     public string CountryName { get; set; }

     [Required(ErrorMessageResourceType = typeof(ModelResources), ErrorMessageResourceName = "ProvinceRequired")]
     public int ProvinceKey { get; set; }
     public string ProvinceName { get; set; }

     public int DisplayOrder { get; set; }
     public bool IsActive { get; set; }
     public string IsActiveText { get; set; }
     public List<SelectListModel> Countries { get; set; }
     public List<SelectListModel> Provinces { get; set; }
    }
}
