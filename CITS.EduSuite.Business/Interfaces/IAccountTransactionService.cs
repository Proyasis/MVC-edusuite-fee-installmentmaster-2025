using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CITS.EduSuite.Business.Models.ViewModels;

namespace CITS.EduSuite.Business.Interfaces
{
    public interface IAccountTransactionService
    {
        List<AccountTransactionViewModel> GetAccountTransactions(AccountTransactionViewModel model);
        AccountTransactionViewModel GetAccountTransactionById(AccountTransactionViewModel model);
        AccountTransactionViewModel CreateAccountTransaction(AccountTransactionViewModel model);
        AccountTransactionViewModel UpdateAccountTransaction(AccountTransactionViewModel model);
        AccountTransactionViewModel DeleteAccountTransaction(AccountTransactionViewModel model);
        List<SelectListModel> GetPartyByPartyType(AccountTransactionViewModel model);

        AccountTransactionViewModel GetBranches(AccountTransactionViewModel model);
        byte GetTransactionTypeByLedger(int LedgerKey);


        PaymentWindowViewModel GetAccountTransactionPaymentById(Int64 Id);

        PaymentWindowViewModel CreateTransactionPayment(PaymentWindowViewModel model);

        PaymentWindowViewModel UpdateTransactionPayment(PaymentWindowViewModel model);

    }
}
