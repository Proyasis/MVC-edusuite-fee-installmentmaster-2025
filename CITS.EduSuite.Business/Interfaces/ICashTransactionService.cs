using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CITS.EduSuite.Business.Models.ViewModels;

namespace CITS.EduSuite.Business.Interfaces
{
   public interface ICashTransactionService
    {
        List<CashTransactionViewModel> GetCashTransactions(CashTransactionViewModel model, out long TotalRecords);

        CashTransactionViewModel GetCashTransactionsById(CashTransactionViewModel model);

        CashTransactionViewModel CreateCashTransactions(CashTransactionViewModel model);

        CashTransactionViewModel UpdateCashTransactions(CashTransactionViewModel model);

        CashTransactionViewModel DeleteCashTransactions(CashTransactionViewModel model);       
       
        CashTransactionViewModel GetToBranchById(CashTransactionViewModel model);

        decimal CheckShortBalance(long Rowkey,short branchKey);
    }
}
