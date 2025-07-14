using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CITS.EduSuite.Business.Models.ViewModels;

namespace CITS.EduSuite.Business.Interfaces
{
   public interface IBankAccountService
    {
        List<BankAccountViewModel> GetBankAccounts(BankAccountViewModel model, out long TotalRecords);

        BankAccountViewModel GetBankAccountById(Int64 id);

        BankAccountViewModel CreateBankAccount(BankAccountViewModel model);


        BankAccountViewModel UpdateBankAccount(BankAccountViewModel model);


        BankAccountViewModel DeleteBankAccount(BankAccountViewModel model);

        decimal GetAccountBalanceByAccount(Int64 Id);

        BankAccountViewModel GetBranches(BankAccountViewModel model);

        BankAccountViewModel GetBankBranches(BankAccountViewModel model);
    }
}
