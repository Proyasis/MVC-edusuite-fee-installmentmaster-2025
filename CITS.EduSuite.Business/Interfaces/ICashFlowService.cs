using CITS.EduSuite.Business.Models.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CITS.EduSuite.Business.Interfaces
{
    public interface ICashFlowService
    {
        List<CashFlowViewModel> GetCashFlows(CashFlowViewModel model);
        CashFlowViewModel GetCashFlowById(CashFlowViewModel model);
        CashFlowViewModel CreateCashFlow(CashFlowViewModel model);
        CashFlowViewModel UpdateCashFlow(CashFlowViewModel model);
        CashFlowViewModel DeleteCashFlow(CashFlowViewModel model);
        CashFlowViewModel FillAcountHeadType(CashFlowViewModel model);
        CashFlowViewModel FillAcountHead(CashFlowViewModel model);
        CashFlowViewModel GetBranches(CashFlowViewModel model);
        CashFlowViewModel PrintCashFlowById(CashFlowViewModel model);
        CashFlowViewModel FillDropdownListsForList(CashFlowViewModel model);
        CashFlowViewModel FillBankAccounts(CashFlowViewModel model);
        decimal CheckShortBalance(short PaymentModeKey, long Rowkey, long BankAccountKey, short branchKey, byte CashFlowTypeKey);
        CashFlowViewModel FillSearchAcountHead(CashFlowViewModel model);
    }
}
