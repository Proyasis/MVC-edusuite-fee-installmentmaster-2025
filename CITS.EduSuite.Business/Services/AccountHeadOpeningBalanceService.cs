using CITS.EduSuite.Business.Common;
using CITS.EduSuite.Business.Interfaces;
using CITS.EduSuite.Business.Models.Resources;
using CITS.EduSuite.Business.Models.ViewModels;
using CITS.EduSuite.Data;
using System;
using System.Collections.Generic;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;


namespace CITS.EduSuite.Business.Services
{
    public class AccountHeadOpeningBalanceService : IAccountHeadOpeningBalanceService
    {
        private EduSuiteDatabase dbContext;
        public AccountHeadOpeningBalanceService(EduSuiteDatabase objDB)
        {
            this.dbContext = objDB;
        }
        public AccountHeadOpeningBalanceViewModel GetAccountHeadOpeningBalanceById(short? id)
        {
            try
            {
                AccountHeadOpeningBalanceViewModel model = new AccountHeadOpeningBalanceViewModel();
                model = dbContext.AccountHeadOpeningBalances.Where(x => x.BranchKey == id).Select(row => new AccountHeadOpeningBalanceViewModel
                {
                    BranchKey = row.BranchKey,
                    OpeningDate = row.OpeningDate,
                }).FirstOrDefault();
                if (model == null)
                {
                    model = new AccountHeadOpeningBalanceViewModel();
                    model.BranchKey = id ?? 0;
                }
                //model.AccountReceivable = dbContext.Parties.Where(x => (x.PartyTypeKey == DbConstants.PartyType.Customer || x.PartyTypeKey == DbConstants.PartyType.Agent) && (x.CashFlowTypeKey == DbConstants.CashFlowType.In)).Select(x => x.CreditBalance ?? 0).DefaultIfEmpty().Sum();
                //model.AdvancePayable = dbContext.Parties.Where(x => (x.PartyTypeKey == DbConstants.PartyType.Customer || x.PartyTypeKey == DbConstants.PartyType.Agent) && (x.CashFlowTypeKey == DbConstants.CashFlowType.Out)).Select(x => x.CreditBalance ?? 0).DefaultIfEmpty().Sum();
                //model.AccountPayable = dbContext.Parties.Where(x => (x.PartyTypeKey == DbConstants.PartyType.Supplier || x.PartyTypeKey == DbConstants.PartyType.OutSource) && (x.CashFlowTypeKey == DbConstants.CashFlowType.Out)).Select(x => x.CreditBalance ?? 0).DefaultIfEmpty().Sum();
                //model.AdvanceReceivable = dbContext.Parties.Where(x => (x.PartyTypeKey == DbConstants.PartyType.Supplier || x.PartyTypeKey == DbConstants.PartyType.OutSource) && (x.CashFlowTypeKey == DbConstants.CashFlowType.In)).Select(x => x.CreditBalance ?? 0).DefaultIfEmpty().Sum();
                //model.Inventory = dbContext.RawMaterialStocks.Where(x => x.PurchaseOrderDetailKey == null).Select(x => x.StockValue ?? 0).DefaultIfEmpty().Sum();
                openingBalanceDetails(model);
                FillBranches(model);
                return model;
            }
            catch (Exception ex)
            {
                AccountHeadOpeningBalanceViewModel model = new AccountHeadOpeningBalanceViewModel();
                model.Message = ex.GetBaseException().Message;
                return model;
            }
        }
        private void openingBalanceDetails(AccountHeadOpeningBalanceViewModel model)
        {
            model.OpeningBalanceDetails = (from ah in dbContext.AccountHeads

                                           join ahop in dbContext.AccountHeadOpeningBalances.Where(x => x.BranchKey == model.BranchKey) on ah.RowKey equals ahop.AccountHeadKey into ahopj
                                           from ahop in ahopj.DefaultIfEmpty()
                                           where ah.RowKey != DbConstants.AccountHead.CashSale
                                           select new OpeningBalanceDetailViewModel
                                           {
                                               RowKey = ahop.RowKey != null ? ahop.RowKey : 0,
                                               AccountHeadKey = ah.RowKey,
                                               AccountHeadName = ah.AccountHeadName,
                                               DebitAmount = ahop.DebitAmount,
                                               CreditAmount = ahop.CreditAmount,
                                               AccountGroupKey = ah.AccountHeadType.AccountGroupKey
                                           }).ToList();
        }
        public AccountHeadOpeningBalanceViewModel CreateAccountHeadOpeningBalance(AccountHeadOpeningBalanceViewModel model)
        {
            FillBranches(model);
            using (var transaction = dbContext.Database.BeginTransaction())
            {
                try
                {
                    int MaxKey = dbContext.AccountHeadOpeningBalances.Select(p => p.RowKey).DefaultIfEmpty().Max();
                    List<AccountFlowViewModel> accountFlowModelList = new List<AccountFlowViewModel>();
                    foreach (OpeningBalanceDetailViewModel modelItem in model.OpeningBalanceDetails)
                    {
                        if (modelItem.RowKey == 0)
                        {
                            if ((modelItem.CreditAmount ?? 0) != 0 || (modelItem.DebitAmount ?? 0) != 0)
                            {
                                AccountHeadOpeningBalance AccountHeadOpeningBalanceModel = new AccountHeadOpeningBalance();
                                AccountHeadOpeningBalanceModel.RowKey = Convert.ToInt16(MaxKey + 1);
                                modelItem.RowKey = AccountHeadOpeningBalanceModel.RowKey;
                                AccountHeadOpeningBalanceModel.BranchKey = model.BranchKey;
                                AccountHeadOpeningBalanceModel.OpeningDate = model.OpeningDate;
                                AccountHeadOpeningBalanceModel.AccountHeadKey = modelItem.AccountHeadKey;
                                AccountHeadOpeningBalanceModel.DebitAmount = modelItem.DebitAmount;
                                AccountHeadOpeningBalanceModel.CreditAmount = modelItem.CreditAmount;
                                AccountHeadOpeningBalanceModel.AccountHead = dbContext.AccountHeads.SingleOrDefault(x => x.RowKey == modelItem.AccountHeadKey);
                                dbContext.AccountHeadOpeningBalances.Add(AccountHeadOpeningBalanceModel);
                                accountFlowModelList = DebitAmountList(AccountHeadOpeningBalanceModel, accountFlowModelList, false);
                                if (modelItem.AccountHeadKey == DbConstants.AccountHead.CashAccount)
                                {
                                    Branch Branch = dbContext.Branches.SingleOrDefault(x => x.RowKey == model.BranchKey);
                                    Branch.OpeningCashBalance = (modelItem.DebitAmount ?? 0) != 0 ? -(modelItem.DebitAmount ?? 0) : (modelItem.CreditAmount ?? 0);
                                }
                                if (dbContext.BankAccounts.Any(x => x.AccountHeadKey == AccountHeadOpeningBalanceModel.AccountHead.RowKey))
                                {
                                    BankAccount BankAccount = dbContext.BankAccounts.SingleOrDefault(x => x.AccountHeadKey == AccountHeadOpeningBalanceModel.AccountHead.RowKey);
                                    BankAccount.OpeningAccountBalance = (modelItem.DebitAmount ?? 0) != 0 ? -(modelItem.DebitAmount ?? 0) : (modelItem.CreditAmount ?? 0);
                                    BankAccount.CurrentAccountBalance = BankAccount.OpeningAccountBalance;
                                }
                                MaxKey++;
                            }
                        }
                        else
                        {
                            AccountHeadOpeningBalance AccountHeadOpeningBalanceModel = new AccountHeadOpeningBalance();
                            AccountHeadOpeningBalanceModel = dbContext.AccountHeadOpeningBalances.SingleOrDefault(x => x.RowKey == modelItem.RowKey);
                            AccountHeadOpeningBalanceModel.BranchKey = model.BranchKey;
                            AccountHeadOpeningBalanceModel.OpeningDate = model.OpeningDate;
                            AccountHeadOpeningBalanceModel.AccountHeadKey = modelItem.AccountHeadKey;
                            AccountHeadOpeningBalanceModel.DebitAmount = modelItem.DebitAmount;
                            AccountHeadOpeningBalanceModel.CreditAmount = modelItem.CreditAmount;
                            accountFlowModelList = DebitAmountList(AccountHeadOpeningBalanceModel, accountFlowModelList, true);
                            if (modelItem.AccountHeadKey == DbConstants.AccountHead.CashAccount)
                            {
                                Branch Branch = dbContext.Branches.SingleOrDefault(x => x.RowKey == model.BranchKey);
                                Branch.OpeningCashBalance = (modelItem.DebitAmount ?? 0) != 0 ? -(modelItem.DebitAmount ?? 0) : (modelItem.CreditAmount ?? 0);
                            }
                            if (dbContext.BankAccounts.Any(x => x.AccountHeadKey == AccountHeadOpeningBalanceModel.AccountHead.RowKey))
                            {
                                BankAccount BankAccount = dbContext.BankAccounts.SingleOrDefault(x => x.AccountHeadKey == AccountHeadOpeningBalanceModel.AccountHead.RowKey);
                                BankAccount.CurrentAccountBalance = BankAccount.CurrentAccountBalance - BankAccount.OpeningAccountBalance;
                                BankAccount.OpeningAccountBalance = (modelItem.DebitAmount ?? 0) != 0 ? -(modelItem.DebitAmount ?? 0) : (modelItem.CreditAmount ?? 0);
                                BankAccount.CurrentAccountBalance = BankAccount.CurrentAccountBalance + BankAccount.OpeningAccountBalance;
                            }

                        }
                    }
                    CreateAccountFlow(accountFlowModelList, true);
                    dbContext.SaveChanges();
                    transaction.Commit();
                    model.Message = EduSuiteUIResources.Success;
                    model.IsSuccessful = true;

                }
                catch (Exception)
                {
                    transaction.Rollback();
                   
                    model.Message = String.Format(EduSuiteUIResources.FailedToSaveErrorMessage, EduSuiteUIResources.AccountHeadOpeningBalance);
                    model.IsSuccessful = false;
                }
            }

            return model;
        }
        public AccountHeadOpeningBalanceViewModel DeleteAccountHeadOpeningBalance(AccountHeadOpeningBalanceViewModel model)
        {
            using (var transaction = dbContext.Database.BeginTransaction())
            {
                try
                {
                    List<AccountHeadOpeningBalance> AccountHeadOpeningBalance = dbContext.AccountHeadOpeningBalances.Where(row => row.BranchKey == model.BranchKey).ToList();
                    List<long> Keys = AccountHeadOpeningBalance.Select(x => Convert.ToInt64(x.RowKey)).ToList();
                    List<AccountFlow> AccountList = dbContext.AccountFlows.Where(x => x.TransactionTypeKey == DbConstants.TransactionType.CompanyBranchTransaction && Keys.Contains(x.TransactionKey)).ToList();
                    dbContext.AccountFlows.RemoveRange(AccountList);
                    dbContext.AccountHeadOpeningBalances.RemoveRange(AccountHeadOpeningBalance);
                    dbContext.SaveChanges();
                    transaction.Commit();
                    model.Message = EduSuiteUIResources.Success;
                    model.IsSuccessful = true;
                }
                catch (DbUpdateException ex)
                {
                    transaction.Rollback();
                    if (ex.GetBaseException().Message.ToUpper().Contains(EduSuiteUIResources.ForeignKeyError.ToUpper()))
                    {
                        
                        model.Message = String.Format(EduSuiteUIResources.CantDeleteErrorMessage, EduSuiteUIResources.AccountHeadOpeningBalance);
                        model.IsSuccessful = false;
                    }
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                   
                    model.Message = String.Format(EduSuiteUIResources.FailedToDeleteErrorMessage, EduSuiteUIResources.AccountHeadOpeningBalance);
                    model.IsSuccessful = false;
                }
            }

            return model;
        }
        public List<AccountHeadOpeningBalanceViewModel> GetAccountHeadOpeningBalance(short BranchKey)
        {
            try
            {
                var AccountHeadOpeningBalanceList = (from c in dbContext.Branches
                                                     join p in dbContext.AccountHeadOpeningBalances on c.RowKey equals p.BranchKey into pj
                                                     from p in pj.DefaultIfEmpty()
                                                     orderby p.BranchKey
                                                     select new AccountHeadOpeningBalanceViewModel
                                                   {
                                                       BranchKey = c.RowKey,
                                                       BranchName = c.BranchName,
                                                       OpeningDate = p.OpeningDate != null ? p.OpeningDate : DateTimeUTC.Now,
                                                       TotalCredit = c.AccountHeadOpeningBalances.Select(x => x.CreditAmount).DefaultIfEmpty().Sum(),
                                                       TotalDebit = c.AccountHeadOpeningBalances.Select(x => x.DebitAmount).DefaultIfEmpty().Sum(),

                                                   }).ToList();
                if (BranchKey != 0)
                {
                    AccountHeadOpeningBalanceList = AccountHeadOpeningBalanceList.Where(x => x.BranchKey == BranchKey).ToList();
                }
                return AccountHeadOpeningBalanceList.GroupBy(x => x.BranchKey).Select(y => y.First()).ToList<AccountHeadOpeningBalanceViewModel>();
            }
            catch (Exception ex)
            {
                return new List<AccountHeadOpeningBalanceViewModel> { new AccountHeadOpeningBalanceViewModel { Message = ex.GetBaseException().Message } };
            }
        }
        private void CreateAccountFlow(List<AccountFlowViewModel> accountFlowModelList, bool IsUpdate)
        {
            AccountFlowService accounFlowService = new AccountFlowService(dbContext);
            if (IsUpdate == false)
            {
                accounFlowService.CreateAccountFlow(accountFlowModelList);
            }
            else
            {
                accounFlowService.UpdateAccountFlow(accountFlowModelList);
            }
        }
        private List<AccountFlowViewModel> DebitAmountList(AccountHeadOpeningBalance model, List<AccountFlowViewModel> accountFlowModelList, bool IsUpdate)
        {
            long AccountHeadKey;
            long ExtraUpdateKey = 0;
            AccountHeadKey = model.AccountHeadKey;
            accountFlowModelList.Add(new AccountFlowViewModel
            {
                CashFlowTypeKey = (model.DebitAmount ?? 0) != 0 ? DbConstants.CashFlowType.Out : DbConstants.CashFlowType.In,
                AccountHeadKey = AccountHeadKey,
                Amount = (model.DebitAmount ?? 0) != 0 ? (model.DebitAmount ?? 0) : (model.CreditAmount ?? 0),
                TransactionTypeKey = DbConstants.TransactionType.CompanyBranchTransaction,
                VoucherTypeKey = DbConstants.VoucherType.OpeningBalance,
                TransactionDate = model.OpeningDate,
                ExtraUpdateKey = ExtraUpdateKey,
                IsUpdate = IsUpdate,
                Purpose = model.AccountHead.AccountHeadName + EduSuiteUIResources.BlankSpace + EduSuiteUIResources.OpeningBalance,
                BranchKey = model.BranchKey,
                TransactionKey = model.RowKey,
            });
            return accountFlowModelList;
        }
        public AccountHeadOpeningBalanceViewModel FillBranches(AccountHeadOpeningBalanceViewModel model)
        {

            IQueryable<SelectListModel> BranchQuery = dbContext.vwBranchSelectActiveOnlies.OrderBy(row => row.DisplayOrder).Select(row => new SelectListModel
            {
                RowKey = row.RowKey,
                Text = row.BranchName
            });


            if (!DbConstants.Role.AdminUserTypes.Contains(DbConstants.User.RoleKey))
            {
                Employee Employee = dbContext.Employees.Where(x => x.AppUserKey == DbConstants.User.UserKey).SingleOrDefault();

                if (Employee != null)
                {
                    List<long> branches = Employee.BranchAccess.ToString().Split(',').Select(Int64.Parse).ToList();
                    model.Branches = BranchQuery.Where(x => branches.Contains(x.RowKey)).ToList();
                    model.BranchKey = Employee.BranchKey;
                    //model.SearchBranchKey = model.BranchKey = Employee.BranchKey;
                }
                else
                {
                    model.Branches = BranchQuery.ToList();
                }

            }
            else
            {
                if (DbConstants.User.RoleKey == DbConstants.AdminKey)
                {
                    model.Branches = BranchQuery.ToList();
                }
                else
                {
                    Employee Employee = dbContext.Employees.Where(x => x.AppUserKey == DbConstants.User.UserKey).SingleOrDefault();
                    List<long> branches = Employee.BranchAccess.ToString().Split(',').Select(Int64.Parse).ToList();
                    model.Branches = BranchQuery.Where(x => branches.Contains(x.RowKey)).ToList();
                }




            }

            return model;
        }

    }
}
