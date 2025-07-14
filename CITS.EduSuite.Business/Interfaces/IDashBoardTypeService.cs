using CITS.EduSuite.Business.Models.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CITS.EduSuite.Business.Interfaces
{
   public interface IDashBoardTypeService
    {
        DashBoardTypeViewModel GetDashBoardTypeById(int? id);
        DashBoardTypeViewModel CreateDashBoardType(DashBoardTypeViewModel model);
        DashBoardTypeViewModel UpdateDashBoardType(DashBoardTypeViewModel model);
        DashBoardTypeViewModel DeleteDashBoardType(DashBoardTypeViewModel model);
        List<DashBoardTypeViewModel> GetDashBoardType(string searchText);
    }
}
