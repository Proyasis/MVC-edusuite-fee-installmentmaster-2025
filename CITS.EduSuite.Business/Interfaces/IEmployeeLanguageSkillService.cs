using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CITS.EduSuite.Business.Models.ViewModels;

namespace CITS.EduSuite.Business.Interfaces
{
   public interface IEmployeeLanguageSkillService
    {

       EmployeeLanguageSkillViewModel GetEmployeeLanguageSkillsById(Int64 EmployeeId);

       EmployeeLanguageSkillViewModel UpdateEmployeeLanguageSkills(EmployeeLanguageSkillViewModel model);

       EmployeeLanguageSkillViewModel CheckIdentityTypeExists(Int16 LanguageKey,Int64 EmployeeKey,Int64 RowKey);

       EmployeeLanguageSkillViewModel DeleteEmployeeLanguageSkills(languageSkillsViewModel model);



    }
}
