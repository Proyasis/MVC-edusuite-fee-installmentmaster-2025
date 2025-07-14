using CITS.EduSuite.Business.Models.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
namespace CITS.EduSuite.Business.Interfaces
{
    public interface ILanguageService
    {
        List<LanguageViewModel> GetLanguages(string searchText);

        LanguageViewModel GetLanguagesById(Int16? id);

        LanguageViewModel CreateLanguage(LanguageViewModel model);

        LanguageViewModel UpdateLanguage(LanguageViewModel model);

        LanguageViewModel DeleteLanguage(LanguageViewModel model);

    }
}
