using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CITS.EduSuite.Business.Models.ViewModels;

namespace CITS.EduSuite.Business.Interfaces
{
   public interface IBankTransactionService
    {
        List<BankTransactionViewModel> GetBankTransactionsByType(BankTransactionViewModel model, out long TotalRecords);
        BankTransactionViewModel GetBankTransactionById(BankTransactionViewModel model);

        BankTransactionViewModel CreateBankTransaction(BankTransactionViewModel model);

        BankTransactionViewModel UpdateBankTransaction(BankTransactionViewModel model);

        BankTransactionViewModel DeleteBankTransaction(BankTransactionViewModel model);

        BankTransactionViewModel GetBankTransactionTypes(BankTransactionViewModel model);

        BankTransactionViewModel GetBankAccountById(BankTransactionViewModel model);

        BankTransactionViewModel GetBranches(BankTransactionViewModel model);

        BankTransactionViewModel FillBankAccounts(BankTransactionViewModel model);

        decimal CheckShortBalance(long Rowkey, long BankAccountKey, short branchKey, byte BankTransactionTypeKey);
    }
}
