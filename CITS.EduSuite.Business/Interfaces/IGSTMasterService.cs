using CITS.EduSuite.Business.Models.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CITS.EduSuite.Business.Interfaces
{
    public interface IGSTMasterService
    {
        GSTMasterViewModel GetGSTMasterById(int? id);
        GSTMasterViewModel CreateGSTMaster(GSTMasterViewModel model);
        GSTMasterViewModel UpdateGSTMaster(GSTMasterViewModel model);
        GSTMasterViewModel DeleteGSTMaster(GSTMasterViewModel model);
        List<GSTMasterViewModel> GetGSTMaster(string searchText);
    }
}
