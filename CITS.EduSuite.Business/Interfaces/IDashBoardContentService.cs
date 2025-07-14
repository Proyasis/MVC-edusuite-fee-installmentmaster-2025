using CITS.EduSuite.Business.Models.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CITS.EduSuite.Business.Interfaces
{
   public interface IDashBoardContentService
    {
        DashBoardContentViewModel GetDashBoardContentById(int? id);
        DashBoardContentViewModel CreateDashBoardContent(DashBoardContentViewModel model);
        DashBoardContentViewModel UpdateDashBoardContent(DashBoardContentViewModel model);
        DashBoardContentViewModel DeleteDashBoardContent(DashBoardContentViewModel model);
        List<DashBoardContentViewModel> GetDashBoardContent(string searchText);
    }
}
