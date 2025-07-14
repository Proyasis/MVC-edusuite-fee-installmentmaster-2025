using CITS.EduSuite.Business.Models.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CITS.EduSuite.Business.Interfaces
{
    public interface IUniversityMasterService
    {
        UniversityMasterViewModel GetUniversityMasterById(int? id);
        UniversityMasterViewModel CreateUniversityMaster(UniversityMasterViewModel model);
        UniversityMasterViewModel UpdateUniversityMaster(UniversityMasterViewModel model);
        UniversityMasterViewModel DeleteUniversityMaster(UniversityMasterViewModel model);
        List<UniversityMasterViewModel> GetUniversityMaster(string searchText);
        UniversityMasterViewModel CheckUniversityMasterCodeExists(UniversityMasterViewModel model);
        UniversityMasterViewModel CheckUniversityMasterNameExists(UniversityMasterViewModel model);
        UniversityMasterViewModel FillAccountHeadType(UniversityMasterViewModel model);
    }
}
