using CITS.EduSuite.Business.Models.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CITS.EduSuite.Business.Interfaces
{
    public interface IMenuDetailViewService
    {
        MenuDetailViewModel GetMenuDetailViewById(int? id);
        MenuDetailViewModel CreateMenuDetailView(MenuDetailViewModel model);
        MenuDetailViewModel UpdateMenuDetailView(MenuDetailViewModel model);
        MenuDetailViewModel DeleteMenuDetailView(MenuDetailViewModel model);
        MenuDetailViewModel DeleteMenuAction(MenuDetailViewModel model);
        List<MenuDetailViewModel> GetMenuDetailView(MenuDetailViewModel model);
    }
}
