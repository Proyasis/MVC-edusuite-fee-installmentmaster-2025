using CITS.EduSuite.Business.Models.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CITS.EduSuite.Business.Interfaces
{
   public interface ICasteService
    {
        CasteViewModel GetCasteById(int? id);
        CasteViewModel CreateCaste(CasteViewModel model);
        CasteViewModel UpdateCaste(CasteViewModel model);
        CasteViewModel DeleteCaste(CasteViewModel model);
        List<CasteViewModel> GetCaste(string searchText);
    }
}
