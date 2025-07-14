using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CITS.EduSuite.Business.Models.ViewModels;

namespace CITS.EduSuite.Business.Interfaces
{
    public interface IRackService
    {
        List<RackViewModel> GetRack(string searchText, short? BranchKey);

        RackViewModel GetRackById(int? id);

        RackViewModel CreateRack(RackViewModel model);

        RackViewModel UpdateRack(RackViewModel model);

        RackViewModel DeleteRack(RackViewModel model);
        RackViewModel DeleteSubRack(long id);
        RackViewModel CheckRackCodeExists(string RackCode, int RowKey);
        RackViewModel CheckSubRackCodeExists(string SubRackCode, long RowKey);
        void FillBranches(RackViewModel model);
    }
}
