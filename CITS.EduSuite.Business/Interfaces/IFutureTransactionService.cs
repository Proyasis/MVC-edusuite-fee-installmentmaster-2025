using CITS.EduSuite.Business.Models.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CITS.EduSuite.Business.Interfaces
{
    public interface IFutureTransactionService
    {
        FutureTransactionViewModel GetFutureTransactionById(FutureTransactionViewModel model);
        FutureTransactionViewModel CreateFutureTransaction(FutureTransactionViewModel model);
        FutureTransactionViewModel UpdateFutureTransaction(FutureTransactionViewModel model);
        FutureTransactionViewModel DeleteFutureTransaction(FutureTransactionViewModel model);
        List<FutureTransactionViewModel> GetFutureTransaction(FutureTransactionViewModel model, string searchText);
        FutureTransactionViewModel DeleteFutureTransactionOtherAmountTypeItem(FutureTransactionOtherAmountTypeViewModel objViewModel);
        FutureTransactionPaymentViewModel GetFutureTransactionPaymentById(Int64 Id);
        FutureTransactionPaymentViewModel CallFutureTransactionPayment(FutureTransactionPaymentViewModel model);
        FutureTransactionViewModel FillBranches(FutureTransactionViewModel model);
        FutureTransactionViewModel FillBankAccounts(FutureTransactionViewModel model);
        decimal CheckShortBalance(short PaymentModeKey, long Rowkey, long BankAccountKey, short branchKey, byte CashFlowTypeKey);
        FutureTransactionViewModel CheckBillNumberExists(string BillNumber, long? RowKey);
        byte GetAccountGroup(long accountHeadKey);
        short GetComapanyStateKey(short branchKey);
        FutureTransactionPaymentViewModel FutureTransactionCalculation(FutureTransactionPaymentViewModel model);
        FutureTransactionViewModel ViewFutureTransactionById(int? id);
        FutureTransactionViewModel DeleteFutureTransactionPayment(long RowKey);
        FutureTransactionViewModel FillAccountHeads(FutureTransactionViewModel model);
        FutureTransactionViewModel GetHSNCodeDetailsById(FutureTransactionViewModel model);
    }
}
