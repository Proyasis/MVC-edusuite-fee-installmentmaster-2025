using CITS.EduSuite.Business.Models.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CITS.EduSuite.Business.Interfaces
{
    public interface IAccountFlowService
    {
        List<AccountFlowViewModel> CreateAccountFlow(List<AccountFlowViewModel> model);
        List<AccountFlowViewModel> UpdateAccountFlow(List<AccountFlowViewModel> model);
        AccountFlowViewModel GetLedgerById(AccountFlowViewModel model, long id, string fromDate, string toDate, bool PeriodOnly, bool isCashFlow);
        AccountFlowViewModel GetBalanceSheet(AccountFlowViewModel model, string fromDate, string toDate);
        AccountFlowViewModel GetDayBook(AccountFlowViewModel model, string fromDate, string toDate, bool PeriodOnly);
        AccountFlowViewModel GetTrialBalance(AccountFlowViewModel model, string fromDate, string toDate);
        AccountFlowViewModel GetIncomeStatement(AccountFlowViewModel model, string fromDate, string toDate);
        string GetGSTEFilingReport(AccountFlowViewModel model, byte month, short year, byte gstFlow);
        GSTEFilingTotalViewModel GetGSTEFilingTotalReport(GSTEFilingTotalViewModel model, byte month, short year);
        void FillBranches(AccountFlowViewModel model);
        void FillAccountHead(AccountFlowViewModel model);
        AccountFlowViewModel FillBankAccount(AccountFlowViewModel model);
        ProfitAndLossAccountViewModel GetProfitAndLoss(ProfitAndLossAccountViewModel model, string fromDate, string toDate);

        List<dynamic> GetDayBookSeprate(AccountFlowViewModel model, string fromDate, string toDate);
    }
}
