using CITS.EduSuite.Business.Models.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CITS.EduSuite.Business.Interfaces
{
    public interface IAssetService
    {
        AssetViewModel GetAssetById(long? id);
        AssetViewModel CreateAsset(AssetViewModel model);
        AssetViewModel UpdateAsset(AssetViewModel model);
        AssetViewModel DeleteAsset(AssetViewModel model);
        List<AssetViewModel> GetAsset(string searchText);
        AssetViewModel DeleteAssetDetails(int Id);
        AssetViewModel CheckAssetDetailCode(AssetDetailsViewModel model);
        AssetViewModel FillAccountHead(AssetViewModel model);
        void CreateAccountFlow(DateTime TransactionDate);
    }
}
