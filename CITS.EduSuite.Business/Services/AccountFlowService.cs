using CITS.EduSuite.Business.Common;
using CITS.EduSuite.Business.Interfaces;
using CITS.EduSuite.Business.Models.ViewModels;
using CITS.EduSuite.Data;
using System;
using System.Collections.Generic;
using System.Data.Entity.Infrastructure;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CITS.EduSuite.Business.Models.Resources;
using System.Data.Common;
using CITS.EduSuite.Business.Extensions;

namespace CITS.EduSuite.Business.Services
{
    public class AccountFlowService : IAccountFlowService
    {
        private EduSuiteDatabase dbContext;
        public AccountFlowService(EduSuiteDatabase objdb)
        {
            this.dbContext = objdb;
        }
        public AccountFlowViewModel GetLedgerById(AccountFlowViewModel model, long id, string fromDate, string toDate, bool PeriodOnly, bool isCashFlow)
        {
            List<long> AccountHeadKeys = new List<long>();
            long AccountHeadKey = 0;
            if (isCashFlow)
            {
                AccountHeadKey = dbContext.AccountHeads.Where(x => x.RowKey == DbConstants.AccountHead.CashAccount).Select(x => x.RowKey).FirstOrDefault();
                AccountHeadKeys.Add(AccountHeadKey);
                AccountHeadKeys.AddRange(dbContext.BankAccounts.Select(x => x.AccountHeadKey).ToList());
            }
            else if (id != 0)
            {
                //var AccountHeadList = dbContext.AccountHeads.SingleOrDefault(x => x.RowKey == id);
                model.AccountHeadKey = id;
                AccountHeadKeys.Add(model.AccountHeadKey);
                //model.AccountHeadName = AccountHeadList.AccountHeadName;
            }
            else if (AccountHeadKey != 0)
            {
                model.AccountHeadKey = AccountHeadKey;
                AccountHeadKeys.Add(AccountHeadKey);
            }
            try
            {
                DateTime ToDate = new DateTime();
                DateTime FromDate = new DateTime();
                ToDate = toDate != "" ? Convert.ToDateTime(toDate) : DateTimeUTC.Now;
                if (fromDate != "")
                {
                    model.NextDate = ToDate.AddDays(+1).Date;
                    FromDate = Convert.ToDateTime(fromDate);
                    decimal oldTotalCreditAmount = 0;
                    decimal oldTotalDebitAmount = 0;
                    if (!PeriodOnly)
                    {
                        if (model.BranchKey != 0)
                        {
                            oldTotalCreditAmount = dbContext.AccountFlows.Where(row => System.Data.Entity.DbFunctions.TruncateTime(row.TransactionDate) < System.Data.Entity.DbFunctions.TruncateTime(FromDate) && row.CashFlowTypeKey == DbConstants.CashFlowType.Out && AccountHeadKeys.Contains(row.AccountHeadKey) && row.BranchKey == model.BranchKey && row.AddedBy == (model.AppUserKey ?? row.AddedBy)).Select(row => row.Amount).DefaultIfEmpty().Sum();
                            oldTotalDebitAmount = dbContext.AccountFlows.Where(row => System.Data.Entity.DbFunctions.TruncateTime(row.TransactionDate) < System.Data.Entity.DbFunctions.TruncateTime(FromDate) && row.CashFlowTypeKey == DbConstants.CashFlowType.In && AccountHeadKeys.Contains(row.AccountHeadKey) && row.BranchKey == model.BranchKey && row.AddedBy == (model.AppUserKey ?? row.AddedBy)).Select(row => row.Amount).DefaultIfEmpty().Sum();
                        }
                        else
                        {
                            oldTotalCreditAmount = dbContext.AccountFlows.Where(row => System.Data.Entity.DbFunctions.TruncateTime(row.TransactionDate) < System.Data.Entity.DbFunctions.TruncateTime(FromDate) && row.CashFlowTypeKey == DbConstants.CashFlowType.Out && AccountHeadKeys.Contains(row.AccountHeadKey) && row.AddedBy == (model.AppUserKey ?? row.AddedBy)).Select(row => row.Amount).DefaultIfEmpty().Sum();
                            oldTotalDebitAmount = dbContext.AccountFlows.Where(row => System.Data.Entity.DbFunctions.TruncateTime(row.TransactionDate) < System.Data.Entity.DbFunctions.TruncateTime(FromDate) && row.CashFlowTypeKey == DbConstants.CashFlowType.In && AccountHeadKeys.Contains(row.AccountHeadKey) && row.AddedBy == (model.AppUserKey ?? row.AddedBy)).Select(row => row.Amount).DefaultIfEmpty().Sum();
                        }
                    }

                    if (oldTotalCreditAmount <= oldTotalDebitAmount)
                    {
                        model.OpeningBalance = oldTotalDebitAmount - oldTotalCreditAmount;
                        model.OpeningBalanceDate = FromDate;
                        model.OpeningBalanceTypeKey = DbConstants.CashFlowType.In;
                    }
                    else if (oldTotalCreditAmount > oldTotalDebitAmount)
                    {
                        model.OpeningBalance = oldTotalCreditAmount - oldTotalDebitAmount;
                        model.OpeningBalanceDate = FromDate;
                        model.OpeningBalanceTypeKey = DbConstants.CashFlowType.Out;
                    }
                }
                if (fromDate != "")
                {
                    model.CreditAccountFlow.AddRange(dbContext.AccountFlows.Where(row =>
                        (model.BranchKey != 0) ? row.BranchKey == model.BranchKey &&
                         (AccountHeadKeys.Contains(row.AccountHeadKey) && row.Amount != 0 &&
                        System.Data.Entity.DbFunctions.TruncateTime(row.TransactionDate) >= System.Data.Entity.DbFunctions.TruncateTime(FromDate) &&
                        System.Data.Entity.DbFunctions.TruncateTime(row.TransactionDate) <= System.Data.Entity.DbFunctions.TruncateTime(ToDate) && row.AddedBy == (model.AppUserKey ?? row.AddedBy)) :
                        (row.AccountHeadKey == model.AccountHeadKey &&
                        System.Data.Entity.DbFunctions.TruncateTime(row.TransactionDate) >= System.Data.Entity.DbFunctions.TruncateTime(FromDate) &&
                        System.Data.Entity.DbFunctions.TruncateTime(row.TransactionDate) <= System.Data.Entity.DbFunctions.TruncateTime(ToDate) && row.AddedBy == (model.AppUserKey ?? row.AddedBy))
                        ).Select(row => new AccountFlowViewModel
                        {
                            RowKey = row.RowKey,
                            AccountHeadKey = row.AccountHeadKey,
                            CashFlowTypeKey = row.CashFlowTypeKey,
                            TransactionDate = row.TransactionDate,
                            VoucherTypeName = row.VoucherType.VoucherTypeName,
                            Purpose = isCashFlow && row.AccountHeadKey == AccountHeadKey ?
                                  row.Purpose + EduSuiteUIResources.BlankSpace + EduSuiteUIResources.By + EduSuiteUIResources.BlankSpace + EduSuiteUIResources.Cash
                                  : isCashFlow ? row.Purpose + EduSuiteUIResources.BlankSpace + EduSuiteUIResources.By + EduSuiteUIResources.BlankSpace + EduSuiteUIResources.Bank + EduSuiteUIResources.OpenBracket +
                                  dbContext.BankAccounts.Where(x => x.AccountHeadKey == row.AccountHeadKey).Select(x => (x.NameInAccount ?? x.AccountNumber) + EduSuiteUIResources.Hyphen + x.Bank.BankName).FirstOrDefault() + EduSuiteUIResources.CloseBracket
                                  : row.Purpose,
                            VoucherTypeKey = row.VoucherTypeKey,
                            DateAdded = row.DateAdded,
                            Amount = row.Amount
                        }).ToList());
                }
                else
                {
                    model.CreditAccountFlow.AddRange(dbContext.AccountFlows.Where(row =>
                        (model.BranchKey != 0) ?
                        row.BranchKey == model.BranchKey && row.Amount != 0 &&
                        (AccountHeadKeys.Contains(row.AccountHeadKey)) && row.AddedBy == (model.AppUserKey ?? row.AddedBy) :
                        (AccountHeadKeys.Contains(row.AccountHeadKey)) && row.AddedBy == (model.AppUserKey ?? row.AddedBy)
                        ).Select(row => new AccountFlowViewModel
                        {
                            RowKey = row.RowKey,
                            AccountHeadKey = row.AccountHeadKey,
                            CashFlowTypeKey = row.CashFlowTypeKey,
                            DateAdded = row.DateAdded,
                            TransactionDate = row.TransactionDate,
                            VoucherTypeName = row.VoucherType.VoucherTypeName,
                            Purpose = row.Purpose,
                            VoucherTypeKey = row.VoucherTypeKey,
                            DisplayOrder = row.VoucherTypeKey == DbConstants.VoucherType.OpeningBalance ? 1 : 2,
                            Amount = row.Amount
                        }).ToList());
                }
                //model.CreditAccountFlow = model.CreditAccountFlow.OrderBy(x => x.TransactionDate).ThenBy(c => c.DisplayOrder).ToList();
                model.CreditAccountFlow = model.CreditAccountFlow.OrderBy(x => x.TransactionDate.Date).ThenBy(c => c.DisplayOrder).ThenBy(x => x.RowKey).ToList();
                model.TotalCreditAmount = model.CreditAccountFlow.Where(x => x.CashFlowTypeKey == DbConstants.CashFlowType.Out).Select(row => row.Amount).DefaultIfEmpty().Sum();
                model.TotalDebitAmount = model.CreditAccountFlow.Where(x => x.CashFlowTypeKey == DbConstants.CashFlowType.In).Select(row => row.Amount).DefaultIfEmpty().Sum();
                if (model.OpeningBalanceTypeKey == DbConstants.CashFlowType.In)
                {
                    model.TotalDebitAmount = (model.TotalDebitAmount ?? 0) + model.OpeningBalance;
                }
                else if (model.OpeningBalanceTypeKey == DbConstants.CashFlowType.Out)
                {
                    model.TotalCreditAmount = (model.TotalCreditAmount ?? 0) + model.OpeningBalance;
                }
                if ((model.TotalCreditAmount ?? 0) <= (model.TotalDebitAmount ?? 0))
                {
                    model.ClosingBalanceDate = ToDate;
                    model.ClosingBalanceKey = DbConstants.CashFlowType.Out;
                    model.ClosingBalance = (model.TotalDebitAmount ?? 0) - (model.TotalCreditAmount ?? 0);
                    model.TotalCreditAmount = model.TotalDebitAmount;
                }
                else if ((model.TotalCreditAmount ?? 0) > (model.TotalDebitAmount ?? 0))
                {
                    model.ClosingBalanceDate = ToDate;
                    model.ClosingBalanceKey = DbConstants.CashFlowType.In;
                    model.ClosingBalance = (model.TotalCreditAmount ?? 0) - (model.TotalDebitAmount ?? 0);
                    model.TotalDebitAmount = model.TotalCreditAmount;
                }
                FillAccountHead(model);
                if (model == null)
                {
                    model = new AccountFlowViewModel();
                }
                return model;
            }
            catch (Exception ex)
            {
                ActivityLog.CreateActivityLog(MenuConstants.Ledger, ActionConstants.View, DbConstants.LogType.Error, DbConstants.User.UserKey, model.Message);
                model = new AccountFlowViewModel();
                model.Message = ex.GetBaseException().Message;

                return model;
            }
        }
        public AccountFlowViewModel GetDayBook(AccountFlowViewModel model, string fromDate, string toDate, bool PeriodOnly)
        {
            DateTime ToDate = new DateTime();
            DateTime FromDate = new DateTime();
            ToDate = toDate != "" ? Convert.ToDateTime(toDate) : DateTimeUTC.Now;
            FromDate = fromDate != "" ? Convert.ToDateTime(fromDate) : FromDate;
            try
            {
                model.CreditAccountFlow = dbContext.DayBook(FromDate, ToDate, model.BranchKey, PeriodOnly, DbConstants.User.UserKey).Select(row => new AccountFlowViewModel
                {
                    AccountHeadKey = row.AccountHeadKey,
                    CashFlowTypeKey = row.CashFlowTypeKey ?? 0,
                    VoucherTypeName = row.VoucherTypeName,
                    VoucherTypeKey = row.VoucherTypeKey ?? 0,
                    TransactionDate = row.TransactionDate,
                    TransactionTypeKey = row.TransactionTypeKey ?? 0,
                    Purpose = row.Purpose,
                    TotalCreditAmount = row.TotalCreditAmount,
                    TotalDebitAmount = row.TotalDebitAmount,
                    Amount = row.Amount ?? 0,
                    IsDummy = row.IsDummy
                }).ToList();

                FillAccountHead(model);
                if (model == null)
                {
                    model = new AccountFlowViewModel();
                }
                return model;
            }
            catch (Exception ex)
            {
                ActivityLog.CreateActivityLog(MenuConstants.DayBook, ActionConstants.View, DbConstants.LogType.Error, DbConstants.User.UserKey, model.Message);
                model = new AccountFlowViewModel();
                model.Message = ex.GetBaseException().Message;

                return model;
            }
        }
        
        public List<dynamic> GetDayBookSeprate(AccountFlowViewModel model, string fromDate, string toDate)
        {
            try
            {

                DateTime ToDate = new DateTime();
                DateTime FromDate = new DateTime();
                ToDate = toDate != "" ? Convert.ToDateTime(toDate) : DateTimeUTC.Now;
                FromDate = fromDate != "" ? Convert.ToDateTime(fromDate) : FromDate;



                List<dynamic> DayBookList = new List<dynamic>();

                dbContext.LoadStoredProc("dbo.DayBook_Seprate")
                    .WithSqlParam("@BranchKey", model.BranchKey)
                    .WithSqlParam("@FromDate", FromDate)
                    .WithSqlParam("@ToDate", ToDate)
                    .WithSqlParam("@AppUserKey", DbConstants.User.UserKey)
                    .ExecuteStoredProc((handler) =>
                        {
                            DayBookList = handler.ReadToDynamicList<dynamic>() as List<dynamic>;

                        });

                return DayBookList;
            }
            catch (Exception ex)
            {

                ActivityLog.CreateActivityLog(MenuConstants.DayBook, ActionConstants.View, DbConstants.LogType.Error, null, ex.GetBaseException().Message);
                return new List<dynamic>();


            }
        }

        public AccountFlowViewModel GetTrialBalance(AccountFlowViewModel model, string fromDate, string toDate)
        {
            try
            {
                DateTime ToDate = new DateTime();
                DateTime FromDate = new DateTime();
                ToDate = toDate != "" ? Convert.ToDateTime(toDate) : DateTimeUTC.Now;
                FromDate = fromDate != "" ? Convert.ToDateTime(fromDate) : FromDate;
                IEnumerable<AccountFlowViewModel> DebitAccountFlow = from tb in dbContext.GetTrialBalance(FromDate, ToDate)

                                                                     select new AccountFlowViewModel
                                                                     {
                                                                         AccountHeadKey = tb.AccountHeadKey,
                                                                         TotalCreditAmount = tb.Credit,
                                                                         TotalDebitAmount = tb.Debit,
                                                                         AccountHeadName = tb.AccountHeadName,
                                                                         BranchKey = tb.BranchKey,
                                                                     };
                if (model.BranchKey == 0)
                {
                    model.DebitAccountFlow = DebitAccountFlow.Where(row => (row.TotalCreditAmount != null || row.TotalDebitAmount != null)).ToList();
                }
                else
                {
                    model.DebitAccountFlow = DebitAccountFlow.Where(row => (row.TotalCreditAmount != null || row.TotalDebitAmount != null) && row.BranchKey == model.BranchKey).ToList();
                }
            }
            catch (Exception ex)
            {
                ActivityLog.CreateActivityLog(MenuConstants.TrialBalance, ActionConstants.View, DbConstants.LogType.Error, DbConstants.User.UserKey, model.Message);
                model = new AccountFlowViewModel();
                model.Message = ex.GetBaseException().Message;

                return model;
            }
            return model;
        }
        public AccountFlowViewModel GetBalanceSheet(AccountFlowViewModel model, string fromDate, string toDate)
        {
            try
            {
                DateTime ToDate = new DateTime();
                DateTime FromDate = new DateTime();
                ToDate = toDate != "" ? Convert.ToDateTime(toDate) : DateTimeUTC.Now;
                FromDate = fromDate != "" ? Convert.ToDateTime(fromDate) : FromDate;
                IEnumerable<AccountFlowViewModel> DebitAccountFlow = from tb in dbContext.BalanceSheet(FromDate, ToDate).Where(row =>
                    row.Debit != 0 &&row.AccountGroupKey==DbConstants.AccountGroup.Asset )
                                                                     select new AccountFlowViewModel
                                                                     {
                                                                         AccountHeadKey = tb.AccountHeadKey ?? 0,
                                                                         TotalCreditAmount = tb.Credit,
                                                                         //TotalDebitAmount = tb.Debit > 0 ? tb.Debit : tb.Credit < 0 ? -(tb.Credit) : tb.Debit,  // Changed on 09 Apr 2021
                                                                         TotalDebitAmount = tb.Debit,
                                                                         AccountHeadName = tb.AccountHeadName,
                                                                         AccountHeadTypeName = tb.AccountHeadTypeName,
                                                                         AccountHeadTypeKey = tb.AccountHeadTypeKey,
                                                                         //BranchKey = tb.BranchKey

                                                                     };


                IEnumerable<AccountFlowViewModel> CreditAccountFlow = from tb in dbContext.BalanceSheet(FromDate, ToDate).Where(row =>
                      row.Credit != 0 && row.AccountGroupKey == DbConstants.AccountGroup.Liability)

                                                                      select new AccountFlowViewModel
                                                                      {
                                                                          AccountHeadKey = tb.AccountHeadKey ?? 0,
                                                                          TotalCreditAmount = tb.Credit,
                                                                          //TotalDebitAmount = tb.Credit > 0 ? tb.Credit : tb.Debit < 0 ? -(tb.Debit) : tb.Credit,// Changed on 09 Apr 2021
                                                                          TotalDebitAmount = tb.Debit,
                                                                          AccountHeadName = tb.AccountHeadName,
                                                                          AccountHeadTypeName = tb.AccountHeadTypeName,
                                                                          AccountHeadTypeKey = tb.AccountHeadTypeKey,
                                                                          //BranchKey = tb.BranchKey
                                                                      };
                if (model.BranchKey == 0)
                {
                    DebitAccountFlow = DebitAccountFlow.Where(row => (row.TotalCreditAmount != null || row.TotalDebitAmount != null));
                    CreditAccountFlow = CreditAccountFlow.Where(row => (row.TotalCreditAmount != null || row.TotalDebitAmount != null));
                }
                //else
                //{
                //    DebitAccountFlow = DebitAccountFlow.Where(row => (row.TotalCreditAmount != null || row.TotalDebitAmount != null) && row.BranchKey == model.BranchKey);
                //    CreditAccountFlow = CreditAccountFlow.Where(row => (row.TotalCreditAmount != null || row.TotalDebitAmount != null) && row.BranchKey == model.BranchKey);
                //}
                model.DebitAccountFlow = DebitAccountFlow.OrderBy(x => x.AccountHeadTypeKey).ToList();
                model.CreditAccountFlow = CreditAccountFlow.OrderBy(x => x.AccountHeadTypeKey).ToList();

            }
            catch (Exception ex)
            {
                ActivityLog.CreateActivityLog(MenuConstants.BalanceSheet, ActionConstants.View, DbConstants.LogType.Error, DbConstants.User.UserKey, model.Message);
                model = new AccountFlowViewModel();
                model.Message = ex.GetBaseException().Message;
                return model;



            }
            return model;
        }
        public AccountFlowViewModel GetIncomeStatement(AccountFlowViewModel model, string fromDate, string toDate)
        {
            try
            {
                DateTime ToDate = new DateTime();
                DateTime FromDate = new DateTime();
                ToDate = toDate != "" ? Convert.ToDateTime(toDate) : DateTimeUTC.Now;
                FromDate = fromDate != "" ? Convert.ToDateTime(fromDate) : FromDate;
                var CreditAccountFlow = (from ah in dbContext.AccountHeads.Where(x => x.AccountHeadType.AccountGroup.RowKey == DbConstants.AccountGroup.Income)
                                         join af in dbContext.AccountFlows on ah.RowKey equals af.AccountHeadKey into afj
                                         from af in afj.DefaultIfEmpty()
                                         where
                                         af.RowKey != null &&
                                          (System.Data.Entity.DbFunctions.TruncateTime(af.TransactionDate) >= System.Data.Entity.DbFunctions.TruncateTime(FromDate) &&
                                         System.Data.Entity.DbFunctions.TruncateTime(af.TransactionDate) <= System.Data.Entity.DbFunctions.TruncateTime(ToDate))
                                         select new
                                         {
                                             af.BranchKey,
                                             af.CashFlowTypeKey,
                                             ah.AccountHeadName,
                                             ah.RowKey,
                                             af.Amount
                                         });


                var DebitAccountFlow = (from ah in dbContext.AccountHeads.Where(x => x.AccountHeadType.AccountGroup.RowKey == DbConstants.AccountGroup.Expenses)
                                        join af in dbContext.AccountFlows on ah.RowKey equals af.AccountHeadKey into afj
                                        from af in afj.DefaultIfEmpty()
                                        where af.RowKey != null &&
                                        (System.Data.Entity.DbFunctions.TruncateTime(af.TransactionDate) >= System.Data.Entity.DbFunctions.TruncateTime(FromDate) &&
                                        System.Data.Entity.DbFunctions.TruncateTime(af.TransactionDate) <= System.Data.Entity.DbFunctions.TruncateTime(ToDate))
                                        select new
                                        {
                                            af.BranchKey,
                                            af.CashFlowTypeKey,
                                            ah.AccountHeadName,
                                            ah.RowKey,
                                            af.Amount
                                        });
                if (model.BranchKey != 0)
                {
                    CreditAccountFlow = CreditAccountFlow.Where(x => x.BranchKey == model.BranchKey);
                    DebitAccountFlow = DebitAccountFlow.Where(x => x.BranchKey == model.BranchKey);
                }
                model.CreditAccountFlow = CreditAccountFlow.GroupBy(x => new { x.RowKey, x.AccountHeadName }).Select(x => new AccountFlowViewModel
                {
                    AccountHeadName = x.Key.AccountHeadName,
                    AccountHeadKey = x.Key.RowKey,
                    Amount = x.Where(y => y.CashFlowTypeKey == DbConstants.CashFlowType.Out).Select(y => y.Amount).DefaultIfEmpty().Sum() - x.Where(y => y.CashFlowTypeKey == DbConstants.CashFlowType.In).Select(y => y.Amount).DefaultIfEmpty().Sum()
                }).ToList();
                model.DebitAccountFlow = DebitAccountFlow.GroupBy(x => new { x.RowKey, x.AccountHeadName }).Select(x => new AccountFlowViewModel
                {
                    AccountHeadName = x.Key.AccountHeadName,
                    AccountHeadKey = x.Key.RowKey,
                    Amount = x.Where(y => y.CashFlowTypeKey == DbConstants.CashFlowType.In).Select(y => y.Amount).DefaultIfEmpty().Sum() - x.Where(y => y.CashFlowTypeKey == DbConstants.CashFlowType.Out).Select(y => y.Amount).DefaultIfEmpty().Sum()
                }).ToList();
            }
            catch (Exception ex)
            {
                ActivityLog.CreateActivityLog(MenuConstants.IncomeStatment, ActionConstants.View, DbConstants.LogType.Error, DbConstants.User.UserKey, model.Message);
                model = new AccountFlowViewModel();
                model.Message = ex.GetBaseException().Message;
                return model;

            }
            return model;
        }

        public ProfitAndLossAccountViewModel GetProfitAndLoss(ProfitAndLossAccountViewModel model, string fromDate, string toDate)
        {
            try
            {
                DateTime ToDate = new DateTime();
                DateTime FromDate = new DateTime();
                ToDate = toDate != "" ? Convert.ToDateTime(toDate) : DateTimeUTC.Now;
                FromDate = fromDate != "" ? Convert.ToDateTime(fromDate) : FromDate;
                model.IndirectExpenses = (from inc in dbContext.spProfitAndLossAccount(FromDate, ToDate, model.BranchKey).Where(x => x.AccountHeadTypeKey == DbConstants.AccountHeadType.IndirectExpense)
                                          orderby inc.DisplayOrder
                                          select new ProfitAndLossAccountDetailsViewModel
                                          {
                                              AccountGroupKey = inc.AccountGroupKey,
                                              AccountHeadTypeKey = inc.AccountHeadTypeKey,
                                              AccountHeadName = inc.AccountHeadName,
                                              Amount = inc.Amount,
                                          }).ToList();


                model.DirectExpenses = (from inc in dbContext.spProfitAndLossAccount(FromDate, ToDate, model.BranchKey).Where(x => x.AccountHeadTypeKey == DbConstants.AccountHeadType.DirectExpense)
                                        orderby inc.DisplayOrder
                                        select new ProfitAndLossAccountDetailsViewModel
                                        {
                                            AccountGroupKey = inc.AccountGroupKey,
                                            AccountHeadTypeKey = inc.AccountHeadTypeKey,
                                            AccountHeadName = inc.AccountHeadName,
                                            Amount = inc.Amount,
                                        }).ToList();

                model.IndirectIncomes = (from inc in dbContext.spProfitAndLossAccount(FromDate, ToDate, model.BranchKey).Where(x => x.AccountHeadTypeKey == DbConstants.AccountHeadType.IndirectIncome)
                                         orderby inc.DisplayOrder
                                         select new ProfitAndLossAccountDetailsViewModel
                                         {
                                             AccountGroupKey = inc.AccountGroupKey,
                                             AccountHeadTypeKey = inc.AccountHeadTypeKey,
                                             AccountHeadName = inc.AccountHeadName,
                                             Amount = inc.Amount,
                                         }).ToList();

                model.DirectIncomes = (from inc in dbContext.spProfitAndLossAccount(FromDate, ToDate, model.BranchKey).Where(x => x.AccountHeadTypeKey == DbConstants.AccountHeadType.DirectIncome)
                                       orderby inc.DisplayOrder
                                       select new ProfitAndLossAccountDetailsViewModel
                                       {
                                           AccountGroupKey = inc.AccountGroupKey,
                                           AccountHeadTypeKey = inc.AccountHeadTypeKey,
                                           AccountHeadName = inc.AccountHeadName,
                                           Amount = inc.Amount,
                                       }).ToList();

                model.Gross = model.DirectIncomes.Select(x => x.Amount).DefaultIfEmpty().Sum() - model.DirectExpenses.Select(x => x.Amount).DefaultIfEmpty().Sum();
                model.Net = (model.IndirectIncomes.Select(x => x.Amount).DefaultIfEmpty().Sum() - model.IndirectExpenses.Select(x => x.Amount).DefaultIfEmpty().Sum()) + model.Gross;
            }
            catch (Exception ex)
            {
                model = new ProfitAndLossAccountViewModel();
                model.Message = ex.GetBaseException().Message;
                return model;
            }
            return model;
        }
        public string GetGSTEFilingReport(AccountFlowViewModel model, byte month, short year, byte gstFlow)
        {
            var result = "";
            try
            {
                IEnumerable<string> results = dbContext.Database.SqlQuery<string>("exec spGSTEFilingReport @Month,@Year,@BranchKey,@GstFlow",
                                                                                  new SqlParameter("Month", month),
                                                                                  new SqlParameter("Year", year),
                                                                                   new SqlParameter("BranchKey", model.BranchKey),
                                                                                   new SqlParameter("GstFlow", gstFlow)
                                                                              );


                result = String.Join("", results);
            }
            catch (Exception ex)
            {
                model = new AccountFlowViewModel();
                model.Message = ex.GetBaseException().Message;
            }
            return result;
        }
        public GSTEFilingTotalViewModel GetGSTEFilingTotalReport(GSTEFilingTotalViewModel model, byte month, short year)
        {
            try
            {
                if (model.BranchKey != 0)
                {
                    //model = dbContext.GSTEFilingReportSum(month, year).Where(x => x.BranchKey == model.BranchKey).Select(row => new GSTEFilingTotalViewModel
                    //{
                    //    OutputCGSTAmt = row.OutputCGST,
                    //    OutputSGSTAmt = row.OutputSGST,
                    //    OutputIGSTAmt = row.OutputIGST,
                    //    InputCGSTAmt = row.InputCGST,
                    //    InputSGSTAmt = row.InputSGST,
                    //    InputIGSTAmt = row.InputIGST,
                    //}).FirstOrDefault();
                    //   changed on 13 Feb 2020 
                    model = dbContext.GSTEFilingReportSum(month, year).GroupBy(x => x.Groupkey).Select(row => new GSTEFilingTotalViewModel
                    {
                        OutputCGSTAmt = row.Sum(y => y.OutputCGST),
                        OutputSGSTAmt = row.Sum(y => y.OutputSGST),
                        OutputIGSTAmt = row.Sum(y => y.OutputIGST),
                        InputCGSTAmt = row.Sum(y => y.InputCGST),
                        InputSGSTAmt = row.Sum(y => y.InputSGST),
                        InputIGSTAmt = row.Sum(y => y.InputIGST),
                    }).FirstOrDefault();
                }
                else
                {
                    model = dbContext.GSTEFilingReportSum(month, year).Select(row => new GSTEFilingTotalViewModel
                    {
                        OutputCGSTAmt = row.OutputCGST,
                        OutputSGSTAmt = row.OutputSGST,
                        OutputIGSTAmt = row.OutputIGST,
                        InputCGSTAmt = row.InputCGST,
                        InputSGSTAmt = row.InputSGST,
                        InputIGSTAmt = row.InputIGST,
                    }).FirstOrDefault();
                }
                if (model != null)
                {
                    model.TotalInputCGSTAmt = model.InputCGSTAmt;
                    model.TotalInputSGSTAmt = model.InputSGSTAmt;
                    model.TotalInputIGSTAmt = model.InputIGSTAmt;
                    model.BalancePaidCGSTAmt = (model.InputCGSTAmt ?? 0) - (model.OutputCGSTAmt ?? 0);
                    model.BalancePaidSGSTAmt = (model.InputSGSTAmt ?? 0) - (model.OutputSGSTAmt ?? 0);
                    if ((model.BalancePaidCGSTAmt ?? 0) > 0)
                    {
                        model.InputCGSTAmt = model.OutputCGSTAmt;
                        model.BalancePaidCGSTAmt = 0;
                    }
                    else
                    {
                        model.BalancePaidCGSTAmt = -(model.BalancePaidCGSTAmt);
                        model.InputCGSTAmt = model.InputCGSTAmt;
                    }
                    if ((model.BalancePaidSGSTAmt ?? 0) > 0)
                    {
                        model.InputSGSTAmt = model.OutputSGSTAmt;
                        model.BalancePaidSGSTAmt = 0;
                    }
                    else
                    {
                        model.BalancePaidSGSTAmt = -(model.BalancePaidSGSTAmt);
                        model.InputSGSTAmt = model.InputSGSTAmt;
                    }
                    model.BalancePaidIGSTAmt = (model.InputIGSTAmt ?? 0) - (model.OutputIGSTAmt ?? 0);
                    if ((model.BalancePaidIGSTAmt ?? 0) > 0)
                    {
                        model.BalanceIIGSTAmt = model.OutputIGSTAmt;
                        if ((model.BalancePaidIGSTAmt ?? 0) > 0 && (model.BalancePaidCGSTAmt ?? 0) > 0)
                        {
                            model.BalanceICGSTAmt = model.BalancePaidCGSTAmt;
                            model.BalancePaidCGSTAmt = (model.BalancePaidIGSTAmt ?? 0) - (model.BalancePaidCGSTAmt ?? 0);
                            if ((model.BalancePaidCGSTAmt ?? 0) > 0 && (model.BalancePaidSGSTAmt ?? 0) > 0)
                            {
                                model.BalanceISGSTAmt = model.BalancePaidSGSTAmt;
                                model.BalancePaidSGSTAmt = (model.BalancePaidCGSTAmt ?? 0) - (model.BalancePaidSGSTAmt ?? 0);
                                if ((model.BalancePaidSGSTAmt ?? 0) < 0)
                                {
                                    model.BalanceISGSTAmt = model.BalancePaidCGSTAmt;
                                    model.BalancePaidSGSTAmt = -(model.BalancePaidSGSTAmt ?? 0);
                                }
                                else
                                {

                                    model.BalancePaidSGSTAmt = 0;
                                }
                                model.BalancePaidCGSTAmt = 0;
                            }
                            else
                            {
                                model.BalanceICGSTAmt = model.BalancePaidIGSTAmt;
                                model.BalancePaidCGSTAmt = -(model.BalancePaidCGSTAmt ?? 0);
                            }
                        }
                        model.BalancePaidIGSTAmt = 0;
                    }
                    else
                    {
                        model.BalanceIIGSTAmt = model.InputIGSTAmt;
                        model.BalancePaidIGSTAmt = -(model.BalancePaidIGSTAmt ?? 0);
                    }
                    model.RecievableCGSTAmt = (model.TotalInputCGSTAmt ?? 0) - (model.InputCGSTAmt ?? 0) - (model.BalanceICGSTAmt ?? 0);
                    model.RecievableSGSTAmt = (model.TotalInputSGSTAmt ?? 0) - (model.InputSGSTAmt ?? 0) - (model.BalanceISGSTAmt ?? 0);
                    model.RecievableIGSTAmt = (model.TotalInputIGSTAmt ?? 0) - (model.BalanceIIGSTAmt ?? 0) - (model.BalanceICGSTAmt ?? 0) - (model.BalanceISGSTAmt ?? 0);
                    if ((model.RecievableCGSTAmt ?? 0) < 0)
                    {
                        model.RecievableCGSTAmt = 0;
                    }
                    else
                    {
                        model.RecievableCGSTAmt = model.RecievableCGSTAmt;
                    }
                    if ((model.RecievableSGSTAmt ?? 0) < 0)
                    {
                        model.RecievableSGSTAmt = 0;
                    }
                    else
                    {
                        model.RecievableSGSTAmt = model.RecievableSGSTAmt;
                    }
                    if ((model.RecievableIGSTAmt ?? 0) < 0)
                    {
                        model.RecievableIGSTAmt = 0;
                    }
                    else
                    {
                        model.RecievableIGSTAmt = model.RecievableIGSTAmt;
                    }
                }
            }
            catch (Exception ex)
            {
                model = new GSTEFilingTotalViewModel();
                model.Message = ex.GetBaseException().Message;
                return model;
            }
            return model;
        }
        public List<AccountFlowViewModel> CreateAccountFlow(List<AccountFlowViewModel> modelList)
        {
            long MaxKey = dbContext.AccountFlows.Select(p => p.RowKey).DefaultIfEmpty().Max();

            foreach (AccountFlowViewModel AccountFlowList in modelList.Where(row => row.Amount != 0))
            {
                AccountFlow AccountFlowModel = new AccountFlow();

                AccountFlowModel.RowKey = Convert.ToInt64(++MaxKey);
                AccountFlowModel.Purpose = AccountFlowList.Purpose;
                AccountFlowModel.CashFlowTypeKey = AccountFlowList.CashFlowTypeKey;
                AccountFlowModel.AccountHeadKey = AccountFlowList.AccountHeadKey;
                AccountFlowModel.Amount = AccountFlowList.Amount;
                AccountFlowModel.TransactionTypeKey = AccountFlowList.TransactionTypeKey;
                AccountFlowModel.VoucherTypeKey = AccountFlowList.VoucherTypeKey;
                AccountFlowModel.TransactionKey = AccountFlowList.TransactionKey;
                AccountFlowModel.TransactionDate = AccountFlowList.TransactionDate;
                AccountFlowModel.BranchKey = AccountFlowList.BranchKey ?? 0;
                dbContext.AccountFlows.Add(AccountFlowModel);
                AccountHeadViewModel model = new AccountHeadViewModel();
                model.RowKey = AccountFlowList.AccountHeadKey;
                //if (AccountFlowList.CashFlowTypeKey == DbConstants.CashFlowType.In)
                //{
                //    model.TotalDebitAmount = AccountFlowList.Amount;
                //    updateDebitAmount(model);
                //}
                //else
                //{
                //    model.TotalCreditAmount = AccountFlowList.Amount;
                //    updateCreditAmount(model);
                //}
            }
            //dbContext.SaveChanges();
            return modelList;
        }
        public List<AccountFlowViewModel> UpdateAccountFlow(List<AccountFlowViewModel> modelList)
        {
            List<AccountFlowViewModel> CreateAccountModelList = new List<AccountFlowViewModel>();
            foreach (AccountFlowViewModel AccountFlowList in modelList)
            {
                AccountFlow AccountFlowModel = new AccountFlow();
                AccountHeadViewModel model = new AccountHeadViewModel();
                // AccountFlowModel = dbContext.AccountFlows.SingleOrDefault(row => row.TransactionTypeKey == AccountFlowList.TransactionTypeKey && row.TransactionKey == AccountFlowList.TransactionKey && row.CashFlowTypeKey == AccountFlowList.CashFlowTypeKey);

                //AccountFlowModel = dbContext.AccountFlows.SingleOrDefault(row => row.TransactionTypeKey == AccountFlowList.TransactionTypeKey && row.TransactionKey == AccountFlowList.TransactionKey && row.CashFlowTypeKey == (AccountFlowList.oldCashFlowTypeKey != 0 ? AccountFlowList.oldCashFlowTypeKey : AccountFlowList.CashFlowTypeKey) && row.AccountHeadCode == (AccountFlowList.OldAccountHeadCode ?? AccountFlowList.AccountHeadCode));
                if (AccountFlowList.TransactionTypeKey == DbConstants.TransactionType.CashTransaction)
                {
                    AccountFlowModel = dbContext.AccountFlows.SingleOrDefault(row => row.TransactionTypeKey == AccountFlowList.TransactionTypeKey && row.TransactionKey == AccountFlowList.TransactionKey && row.AccountHeadKey == ((AccountFlowList.OldAccountHeadKey == null ? null : AccountFlowList.OldAccountHeadKey) ?? AccountFlowList.AccountHeadKey) && row.CashFlowTypeKey == (AccountFlowList.oldCashFlowTypeKey != 0 ? AccountFlowList.oldCashFlowTypeKey : AccountFlowList.CashFlowTypeKey));

                }
                else
                {
                    AccountFlowModel = dbContext.AccountFlows.SingleOrDefault(row => row.TransactionTypeKey == AccountFlowList.TransactionTypeKey && row.TransactionKey == AccountFlowList.TransactionKey && row.AccountHeadKey == ((AccountFlowList.OldAccountHeadKey == null ? null : AccountFlowList.OldAccountHeadKey) ?? AccountFlowList.AccountHeadKey));
                }
                if (AccountFlowModel == null)
                {
                    if (AccountFlowList.Amount != 0)
                        CreateAccountModelList.Add(AccountFlowList);
                }
                else
                {
                    AccountFlowModel.Purpose = AccountFlowList.Purpose;
                    AccountFlowModel.CashFlowTypeKey = AccountFlowList.CashFlowTypeKey;
                    AccountFlowModel.AccountHeadKey = AccountFlowList.AccountHeadKey;
                    model.OldAmount = AccountFlowModel.Amount;
                    AccountFlowModel.Amount = AccountFlowList.Amount;
                    AccountFlowModel.TransactionDate = AccountFlowList.TransactionDate;
                    AccountFlowModel.VoucherTypeKey = AccountFlowList.VoucherTypeKey;
                    model.RowKey = AccountFlowList.AccountHeadKey;
                    AccountFlowModel.BranchKey = AccountFlowList.BranchKey ?? 0;
                    //if (AccountFlowList.CashFlowTypeKey == DbConstants.CashFlowType.In)
                    //{
                    //    model.TotalDebitAmount = AccountFlowList.Amount;
                    //    model.IsUpdate = AccountFlowList.IsUpdate;
                    //    model.ExtraUpdateKey = AccountFlowList.ExtraUpdateKey;
                    //    updateDebitAmount(model);
                    //}
                    //else
                    //{
                    //    model.TotalCreditAmount = AccountFlowList.Amount;
                    //    model.IsUpdate = AccountFlowList.IsUpdate;
                    //    model.ExtraUpdateKey = AccountFlowList.ExtraUpdateKey;
                    //    updateCreditAmount(model);
                    //}
                }
            }
            if (CreateAccountModelList.Count > 0)
            {
                CreateAccountFlow(CreateAccountModelList);
            }
            //dbContext.SaveChanges();
            return modelList;
        }
        public AccountFlowViewModel DeleteAccountFlow(AccountFlowViewModel model)
        {
            try
            {
                List<AccountFlow> AccountFlowList = dbContext.AccountFlows.Where(row => row.TransactionTypeKey == model.TransactionTypeKey && row.TransactionKey == model.TransactionKey).ToList();
                if (AccountFlowList != null)
                {
                    foreach (AccountFlow AccountFlow in AccountFlowList)
                    {
                        long AccountHeadKey = AccountFlow.AccountHeadKey;
                        byte CashFlowTypeKey = AccountFlow.CashFlowTypeKey;
                        decimal Amount = AccountFlow.Amount;
                        dbContext.AccountFlows.Remove(AccountFlow);
                        var AccountHead = dbContext.AccountHeads.Any(row => row.RowKey == AccountHeadKey && row.IsSystemAccount == true);
                        if (model.IsDelete == true && AccountHead == false)
                        {
                            DeleteAccountHead(AccountHeadKey);
                        }
                        else
                        {
                            UpdateAccountHead(AccountHeadKey, CashFlowTypeKey, Amount);
                        }
                    }
                }

                dbContext.SaveChanges();

            }
            catch (DbUpdateException ex)
            {
                model = new AccountFlowViewModel();
                model.Message = ex.GetBaseException().Message;
                return model;
            }
            catch (Exception ex)
            {
                model = new AccountFlowViewModel();
                model.Message = ex.GetBaseException().Message;
                return model;
            }
            return model;
        }
        public AccountFlowViewModel UpdateAccountFlowAmount(AccountFlowViewModel model)
        {
            try
            {
                List<AccountFlow> AccountFlowList = dbContext.AccountFlows.Where(row => row.TransactionTypeKey == model.TransactionTypeKey && row.TransactionKey == model.TransactionKey).ToList();
                if (AccountFlowList != null)
                {
                    foreach (AccountFlow AccountFlow in AccountFlowList)
                    {
                        long AccountHeadKey = AccountFlow.AccountHeadKey;
                        byte CashFlowTypeKey = AccountFlow.CashFlowTypeKey;
                        decimal Amount = model.Amount;
                        AccountFlow.Amount = AccountFlow.Amount - model.Amount;
                        if (AccountFlow.Amount == 0)
                        {
                            dbContext.AccountFlows.Remove(AccountFlow);
                        }
                        if (model.IsDelete == true)
                        {
                            DeleteAccountHead(AccountHeadKey);
                        }
                        else
                        {
                            UpdateAccountHead(AccountHeadKey, CashFlowTypeKey, Amount);
                        }
                    }
                }

                dbContext.SaveChanges();

            }
            catch (DbUpdateException ex)
            {
                model = new AccountFlowViewModel();
                model.Message = ex.GetBaseException().Message;
            }
            catch (Exception ex)
            {
                model = new AccountFlowViewModel();
                model.Message = ex.GetBaseException().Message;
            }
            return model;
        }
        private void updateCreditAmount(AccountHeadViewModel model)
        {
            AccountHead AccountHeadModel = new AccountHead();
            AccountHeadModel = dbContext.AccountHeads.SingleOrDefault(row => row.AccountHeadCode == model.AccountHeadCode);
            if (model.IsUpdate == true)
            {
                AccountHeadModel.TotalCreditAmount = (AccountHeadModel.TotalCreditAmount == null ? 0 : AccountHeadModel.TotalCreditAmount) - (model.OldAmount ?? 0) + model.TotalCreditAmount;
            }
            else
            {
                AccountHeadModel.TotalCreditAmount = (AccountHeadModel.TotalCreditAmount == null ? 0 : AccountHeadModel.TotalCreditAmount) + (model.TotalCreditAmount);
            }
            if (model.ExtraUpdateKey != null && model.ExtraUpdateKey != 0)
            {
                AccountHead accountHeadModel = new AccountHead();
                accountHeadModel = dbContext.AccountHeads.SingleOrDefault(row => row.RowKey == model.ExtraUpdateKey);
                accountHeadModel.TotalCreditAmount = (accountHeadModel.TotalCreditAmount == null ? 0 : accountHeadModel.TotalCreditAmount) - (model.OldAmount ?? 0);
            }
            dbContext.SaveChanges();
        }
        private void updateDebitAmount(AccountHeadViewModel model)
        {
            AccountHead AccountHeadModel = new AccountHead();
            AccountHeadModel = dbContext.AccountHeads.SingleOrDefault(row => row.AccountHeadCode == model.AccountHeadCode);
            if (model.IsUpdate == true)
            {
                AccountHeadModel.TotalDebitAmount = (AccountHeadModel.TotalDebitAmount == null ? 0 : AccountHeadModel.TotalDebitAmount) - (model.OldAmount ?? 0) + model.TotalDebitAmount;
            }
            else
            {
                AccountHeadModel.TotalDebitAmount = (AccountHeadModel.TotalDebitAmount == null ? 0 : AccountHeadModel.TotalDebitAmount) + (model.TotalDebitAmount);
            }
            if (model.ExtraUpdateKey != null && model.ExtraUpdateKey != 0)
            {
                AccountHead accountHeadModel = new AccountHead();
                accountHeadModel = dbContext.AccountHeads.SingleOrDefault(row => row.RowKey == model.ExtraUpdateKey);
                accountHeadModel.TotalDebitAmount = (accountHeadModel.TotalDebitAmount == null ? 0 : accountHeadModel.TotalDebitAmount) - (model.OldAmount ?? 0);
            }
            dbContext.SaveChanges();
        }
        public void DeleteAccountHead(long AccountHeadKey)
        {
            try
            {
                AccountHead AccountHead = dbContext.AccountHeads.SingleOrDefault(row => row.RowKey == AccountHeadKey);
                dbContext.AccountHeads.Remove(AccountHead);
                dbContext.SaveChanges();
            }
            catch (DbUpdateException ex)
            {
                throw ex;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        private void UpdateAccountHead(long AccountHeadKey, byte CashFlowTypeKey, decimal Amount)
        {
            try
            {
                AccountHead AccountHead = dbContext.AccountHeads.SingleOrDefault(row => row.RowKey == AccountHeadKey);
                if (CashFlowTypeKey == DbConstants.CashFlowType.In)
                {
                    AccountHead.TotalDebitAmount = (AccountHead.TotalDebitAmount ?? 0) - Amount;
                }
                else if (CashFlowTypeKey == DbConstants.CashFlowType.Out)
                {
                    AccountHead.TotalCreditAmount = (AccountHead.TotalDebitAmount ?? 0) - Amount;
                }
                dbContext.SaveChanges();
            }
            catch (DbUpdateException ex)
            {
                throw ex;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public void FillAccountHead(AccountFlowViewModel model)
        {
            model.AccountHead = dbContext.VwAccountHeadSelectActiveOnlies.Select(x => new SelectListModel
            {
                RowKey = x.RowKey,
                Text = x.AccountHeadName
            }).ToList();
            //return model;
        }
        public void FillBranches(AccountFlowViewModel model)
        {
            IQueryable<SelectListModel> BranchQuery = dbContext.vwBranchSelectActiveOnlies.OrderBy(row => row.DisplayOrder).Select(row => new SelectListModel
            {
                RowKey = row.RowKey,
                Text = row.BranchName
            });

            Employee Employee = dbContext.Employees.Where(x => x.AppUserKey == DbConstants.User.UserKey).SingleOrDefault();
            if (Employee != null)
            {
                if (Employee.BranchAccess != null)
                {
                    List<long> Branches = Employee.BranchAccess.Split(',').Select(Int64.Parse).ToList();
                    model.Branches = BranchQuery.Where(row => Branches.Contains(row.RowKey)).ToList();
                    //model.BranchKey = Employee.BranchKey;
                }
                else
                {
                    model.Branches = BranchQuery.Where(x => x.RowKey == Employee.BranchKey).ToList();
                    //model.BranchKey = Employee.BranchKey;
                }
            }
            else
            {
                model.Branches = BranchQuery.ToList();
            }

            if (model.Branches.Count == 1)
            {
                long? branchkey = model.Branches.Select(x => x.RowKey).FirstOrDefault();
                model.BranchKey = Convert.ToInt16(branchkey);
            }
        }

        public AccountFlowViewModel FillBankAccount(AccountFlowViewModel model)
        {
            model.BankAccounts = dbContext.BranchAccounts.Where(x => (model.BranchKey != 0) ? x.BranchKey == model.BranchKey : x.RowKey > 0).Select(row => new SelectListModel
            {
                RowKey = row.BankAccount.AccountHeadKey,
                Text = row.BankAccount.Bank.BankName + EduSuiteUIResources.OpenBracketWithSpace + (row.BankAccount.NameInAccount ?? row.BankAccount.AccountNumber) + EduSuiteUIResources.ClosingBracketWithSpace
            }).Distinct().ToList();
            return model;
        }

    }
}
