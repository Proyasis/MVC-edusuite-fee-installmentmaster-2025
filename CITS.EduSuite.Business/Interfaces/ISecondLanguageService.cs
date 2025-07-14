using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CITS.EduSuite.Business.Models.ViewModels;


namespace CITS.EduSuite.Business.Interfaces
{
   public interface ISecondLanguageService
    {
        SecondLanguageViewModel GetSecondLanguageById(int? id);
        SecondLanguageViewModel CreateSecondLanguage(SecondLanguageViewModel model);
        SecondLanguageViewModel UpdateSecondLanguage(SecondLanguageViewModel model);
        SecondLanguageViewModel DeleteSecondLanguage(SecondLanguageViewModel model);
        List<SecondLanguageViewModel> GetSecondLanguage(string searchText);
    }
}
