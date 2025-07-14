using CITS.EduSuite.Business.Models.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CITS.EduSuite.Business.Interfaces
{
    public interface IReligionService
    {
        ReligionViewModel GetReligionById(int? id);
        ReligionViewModel CreateReligion(ReligionViewModel model);
        ReligionViewModel UpdateReligion(ReligionViewModel model);
        ReligionViewModel DeleteReligion(ReligionViewModel model);
        List<ReligionViewModel> GetReligion(string searchText);
        ReligionViewModel CheckReligionNameExists(ReligionViewModel model);
    }
}
