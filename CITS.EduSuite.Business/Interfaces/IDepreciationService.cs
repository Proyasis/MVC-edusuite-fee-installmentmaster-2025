using CITS.EduSuite.Business.Models.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CITS.EduSuite.Business.Interfaces
{
    public interface IDepreciationService
    {
        DepreciationViewModel GetDepreciationById(long? id, long? AssetDetailKey, int? Period);
        DepreciationViewModel CreateDepreciation(DepreciationViewModel model);
        DepreciationViewModel UpdateDepreciation(DepreciationViewModel model);
        DepreciationViewModel DeleteDepreciation(DepreciationViewModel model);
        List<DepreciationViewModel> GetDepreciation(string searchText, DepreciationViewModel model);
        DepreciationViewModel ViewDepreciation(long? id);
    }
}
