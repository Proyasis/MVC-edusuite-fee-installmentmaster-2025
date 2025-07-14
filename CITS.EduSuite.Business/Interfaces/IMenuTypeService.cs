using CITS.EduSuite.Business.Models.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CITS.EduSuite.Business.Interfaces
{
   public interface IMenuTypeService
    {
        MenuTypeViewModel GetMenuTypeById(int? id);
        MenuTypeViewModel CreateMenuType(MenuTypeViewModel model);
        MenuTypeViewModel UpdateMenuType(MenuTypeViewModel model);
        MenuTypeViewModel DeleteMenuType(MenuTypeViewModel model);
        List<MenuTypeViewModel> GetMenuType(string searchText);
    }
}
