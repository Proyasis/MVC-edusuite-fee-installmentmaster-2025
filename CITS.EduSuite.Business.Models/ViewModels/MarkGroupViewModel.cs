using CITS.Validations;
using CITS.EduSuite.Business.Models.Resources;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;


namespace CITS.EduSuite.Business.Models.ViewModels
{
    public   class MarkGroupViewModel:BaseModel
    {

        public MarkGroupViewModel()
        {

           

         }

         public long RowKey { get; set; }

      
        [Required(ErrorMessageResourceType = typeof(ModelResources), ErrorMessageResourceName = "RequiredErrorMessage")]
        [StringLength(20, ErrorMessageResourceType = typeof(ModelResources), ErrorMessageResourceName = "LengthErrorMessage")]
        [Display(Name = "Name", ResourceType = typeof(EduSuiteUIResources))]
        public string MarkGroupName { get; set; }

        [Required(ErrorMessageResourceType = typeof(ModelResources), ErrorMessageResourceName = "RequiredErrorMessage")]
        [Display(Name = "Mark", ResourceType = typeof(EduSuiteUIResources))]
        public decimal? Mark { get; set; }
        public decimal? NegativeMark { get; set; }
        public bool IsActive { get; set; }

       

    
      
    }
}
