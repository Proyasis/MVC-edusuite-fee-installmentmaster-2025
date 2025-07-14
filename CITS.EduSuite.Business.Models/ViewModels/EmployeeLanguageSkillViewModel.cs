using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CITS.EduSuite.Business.Models.Resources;

namespace CITS.EduSuite.Business.Models.ViewModels
{
  public  class EmployeeLanguageSkillViewModel:BaseModel
    {
      public EmployeeLanguageSkillViewModel()
        {
            EmployeeLanguageSkills = new List<languageSkillsViewModel>();
         
     
        }
        public long EmployeeKey { get; set; }
        public List<languageSkillsViewModel> EmployeeLanguageSkills { get; set; }
  
    }
    public class languageSkillsViewModel
    {
        public languageSkillsViewModel()
        {
            Languages = new List<SelectListModel>();
            SkillLevels = new List<SelectListModel>();
        }
        public long RowKey { get; set; }
        [Required(ErrorMessageResourceType = typeof(ModelResources), ErrorMessageResourceName = "EmployeeLanguageRequired")]
        [System.Web.Mvc.Remote("CheckLanguageSkillExists", "EmployeeLanguageSkill", AdditionalFields = "EmployeeKey,RowKey", ErrorMessageResourceType = typeof(ModelResources), ErrorMessageResourceName = "EmployeeLanguageSkillExists")]
        public short LanguageKey { get; set; }
        public string LanguageName { get; set; }

        public bool IsRead { get; set; }
        public bool IsWrite { get; set; }
        public bool IsSpeak { get; set; }

        [Required(ErrorMessageResourceType = typeof(ModelResources), ErrorMessageResourceName = "EmployeeLanguageSkillLevelRequired")]
        public byte SkillLevelKey { get; set; }
  
        public List<SelectListModel> Languages { get; set; }
        public List<SelectListModel> SkillLevels { get; set; }

    }
}
