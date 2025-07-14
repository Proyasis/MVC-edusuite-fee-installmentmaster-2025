using CITS.EduSuite.Business.Models.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CITS.EduSuite.Business.Interfaces
{
    public interface IBranchService
    {
        List<BranchViewModel> GetBranches(string searchText);

        BranchViewModel GetBranchById(short? id);

        BranchViewModel CreateBranch(BranchViewModel model);

        BranchViewModel UpdateBranch(BranchViewModel model);

        BranchViewModel DeleteBranch(BranchViewModel model);

        BranchViewModel GetProvinceAndCodeByCountry(short CountryKey);

        BranchViewModel GetDistrictByProvince(int ProvinceKey);
        BranchViewModel CheckBranchCodeExist(BranchViewModel model);
        // BranchViewModel CheckBranchLocationExist(BranchViewModel model);
        BranchViewModel DeleteBranchLogo(short Id);
    }
}
