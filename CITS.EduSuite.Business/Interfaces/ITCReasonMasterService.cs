using CITS.EduSuite.Business.Models.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CITS.EduSuite.Business.Interfaces
{
    public interface ITCReasonMasterService
    {
        TCReasonMasterViewModel GetTCReasonMasterById(int? id);
        TCReasonMasterViewModel CreateTCReasonMaster(TCReasonMasterViewModel model);
        TCReasonMasterViewModel UpdateTCReasonMaster(TCReasonMasterViewModel model);
        TCReasonMasterViewModel DeleteTCReasonMaster(TCReasonMasterViewModel model);
        List<TCReasonMasterViewModel> GetTCReasonMaster(string searchText);
    }
}
