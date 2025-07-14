using CITS.EduSuite.Business.Models.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CITS.EduSuite.Business.Interfaces
{
    public interface IBankStatementService
    {
        BankStatementMasterViewModel GetBankStatementById(short? id);
        List<BankStatementMasterViewModel> GetBankStatements(BankStatementMasterViewModel model, string searchText);
        BankStatementMasterViewModel CreateBankStatementMaster(BankStatementMasterViewModel model);
        BankStatementMasterViewModel UpdateBankStatementMaster(BankStatementMasterViewModel model);
        BankStatementMasterViewModel FillBankAccounts(BankStatementMasterViewModel model);
        BankStatementMasterViewModel DeleteBankStatement(BankStatementMasterViewModel model);
        BankStatementMasterViewModel GetBranches(BankStatementMasterViewModel model);
        BankStatementMasterViewModel GetBankStatementDetails(BankStatementMasterViewModel model);
        BankStatementDetailsViewModel DeleteBankStatementItem(BankStatementDetailsViewModel model);
    }
}
