using CITS.EduSuite.Business.Models.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CITS.EduSuite.Business.Interfaces
{
    public interface IBankReconciliationService
    {
        BankReconciliationViewModel GetBankReconciliationById(BankReconciliationViewModel model);
        BankReconciliationViewModel CreateBankReconciliation(BankReconciliationViewModel model);
        BankReconciliationViewModel GetBranches(BankReconciliationViewModel model);
        BankReconciliationViewModel FillBankAccounts(BankReconciliationViewModel model);
        BankReconciliationViewModel ViewBankReconciliation(BankReconciliationViewModel model);
    }
}
