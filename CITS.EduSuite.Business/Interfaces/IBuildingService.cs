using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CITS.EduSuite.Business.Models.ViewModels;

namespace CITS.EduSuite.Business.Interfaces
{
    public interface IBuildingService
    {
        List<BuildingViewModel> GetBuildingDetails(string SearchText);
        BuildingViewModel GetBuildingDetailsById(int? id);

        BuildingViewModel CreateBuildingMaster(BuildingViewModel model);

        BuildingViewModel UpdateBuildingMaster(BuildingViewModel model);

        BuildingViewModel DeleteBuildingDetailsAll(int id);
        BuildingViewModel DeleteBuildingDetails(long Id, int BuildingMasterKey);

    }
}
