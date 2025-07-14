using CITS.EduSuite.Business.Models.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CITS.EduSuite.Business.Interfaces
{
    public interface IAssetTypeService
    {
        List<AssetTypeViewModel> GetAssetType(string SearchText);
        AssetTypeViewModel GetAssetTypeById(int? Id);
        AssetTypeViewModel CreateAssetType(AssetTypeViewModel model);
        AssetTypeViewModel UpdateAssetType(AssetTypeViewModel model);
        AssetTypeViewModel DeleteAssetType(AssetTypeViewModel model);
        AssetTypeViewModel CheckHSNCodeExists(string HSNCode, int RowKey);
        HSNCodeMasterViewModel GetHSNCodeDetailsById(HSNCodeMasterViewModel model);
    }
}
