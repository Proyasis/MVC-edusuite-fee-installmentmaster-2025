using CITS.EduSuite.Business.Models.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CITS.EduSuite.Business.Interfaces
{
    public interface IAssetPurchaseService
    {
        AssetPurchaseMasterViewModel GetAssetPurchaseMasterById(AssetPurchaseMasterViewModel model);
        AssetPurchaseMasterViewModel CreateAssetPurchaseMaster(AssetPurchaseMasterViewModel model);
        AssetPurchaseMasterViewModel UpdateAssetPurchaseMaster(AssetPurchaseMasterViewModel model);
        AssetPurchaseMasterViewModel DeleteAssetPurchaseMaster(AssetPurchaseMasterViewModel model);
        List<AssetPurchaseMasterViewModel> GetAssetPurchaseMaster(AssetPurchaseMasterViewModel model, string searchText);
        AssetPurchaseMasterViewModel DeleteAssetPurchaseItem(AssetPurchaseDetailsViewModel objViewModel);
        //List<SelectPassListModel> FillMaterialSizeByMaterialType(AssetPurchaseMasterViewModel model);
        void GetMaterialGSTByMaterialType(AssetPurchaseDetailsViewModel model);
        AssetPurchaseMasterViewModel GetPartyDetailsByPartyKey(long? PartyKey);
        //Payement       
        PaymentWindowViewModel GetAssetPurchasePaymentById(Int64 Id);
        PaymentWindowViewModel CallAssetPurchasePayment(PaymentWindowViewModel model);

        AssetPurchaseMasterViewModel GetPartyByPartyType(AssetPurchaseMasterViewModel model);


        AssetPurchaseMasterViewModel ViewAssetPurchaseMasterById(int? id);
        decimal CheckShortBalance(short PaymentModeKey, long Rowkey, long BankAccountKey);

        AssetPurchaseDetailsViewModel GetRawMaterialDetailsById(int? id, short branchKey);
        void FillRawMaterialsById(AssetPurchaseDetailsViewModel model);
        AssetPurchaseMasterViewModel FillBranches(AssetPurchaseMasterViewModel model);

        AssetPurchaseMasterViewModel CheckBillNumberExists(string BillNumber, long? RowKey);
        AssetPurchaseDetailsViewModel FillRateTypes(AssetPurchaseDetailsViewModel model);
    }
}
