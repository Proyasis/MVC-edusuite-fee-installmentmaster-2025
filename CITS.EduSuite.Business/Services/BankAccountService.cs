using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CITS.EduSuite.Business.Common;
using CITS.EduSuite.Business.Interfaces;
using CITS.EduSuite.Business.Models.ViewModels;
using CITS.EduSuite.Data;
using System.Data.Entity.Validation;
using System.Data.Entity.Infrastructure;
using CITS.EduSuite.Business.Models.Resources;
using System.Linq.Expressions;

namespace CITS.EduSuite.Business.Services
{
    public class BankAccountService : IBankAccountService
    {
        private EduSuiteDatabase dbContext;

        public BankAccountService(EduSuiteDatabase objDB)
        {
            this.dbContext = objDB;
        }

        public List<BankAccountViewModel> GetBankAccounts(BankAccountViewModel model, out long TotalRecords)
        {
            try
            {
                var Take = model.PageSize;
                var Skip = (model.PageIndex - 1) * model.PageSize;
                IQueryable<BankAccountViewModel> bankAccountList = (from B in dbContext.BankAccounts
                                                                    orderby B.RowKey descending
                                                                    where (B.AccountNumber.Contains(model.searchText)) || (B.NameInAccount.Contains(model.searchText))
                                                                    select new BankAccountViewModel
                                                                    {
                                                                        RowKey = B.RowKey,
                                                                        AccountNumber = B.AccountNumber,
                                                                        IFSCCode = B.IFSCCode,
                                                                        MICRCode = B.MICRCode,
                                                                        BankName = B.Bank.BankName,
                                                                        BranchLocation = B.BranchLocation,
                                                                        NameInAccount = B.NameInAccount,
                                                                        AccountTypeName = B.BankAccountType.AccountTypeName,
                                                                        CurrentAccountBalance = B.CurrentAccountBalance,
                                                                        OpeningAccountBalance = B.OpeningAccountBalance,
                                                                        BranchName = B.Branch.BranchName,
                                                                        BranchKey = B.BranchKey
                                                                    });
                if (model.BranchKey != 0)
                {
                    bankAccountList = bankAccountList.Where(row => row.BranchKey == model.BranchKey);
                }

                if (model.SortBy != "")
                {
                    bankAccountList = SortApplications(bankAccountList, model.SortBy, model.SortOrder);
                }
                TotalRecords = bankAccountList.Count();
                return bankAccountList.Skip(Skip).Take(Take).ToList<BankAccountViewModel>();
            }
            catch (Exception ex)
            {
                TotalRecords = 0;
                ActivityLog.CreateActivityLog(MenuConstants.BankAccount, ActionConstants.View, DbConstants.LogType.Error, null, ex.GetBaseException().Message);
                return new List<BankAccountViewModel>();
            }
        }
        private IQueryable<BankAccountViewModel> SortApplications(IQueryable<BankAccountViewModel> Query, string SortName, string SortOrder)
        {
            string command = SortOrder == "desc" ? "OrderByDescending" : "OrderBy";
            var type = typeof(BankAccountViewModel);
            var property = type.GetProperty(SortName);
            var parameter = Expression.Parameter(type, "p");
            var propertyAccess = Expression.MakeMemberAccess(parameter, property);
            var orderByExpression = Expression.Lambda(propertyAccess, parameter);
            var resultExpression = Expression.Call(typeof(Queryable), command, new Type[] { type, property.PropertyType },
                                          Query.Expression, Expression.Quote(orderByExpression));
            return Query.Provider.CreateQuery<BankAccountViewModel>(resultExpression);
        }


        public BankAccountViewModel GetBankAccountById(long id)
        {
            try
            {
                BankAccountViewModel model = new BankAccountViewModel();

                model = dbContext.BankAccounts.Select(row => new BankAccountViewModel
                {
                    RowKey = row.RowKey,
                    BranchKey = row.BranchKey,
                    AccountNumber = row.AccountNumber,
                    IFSCCode = row.IFSCCode,
                    MICRCode = row.MICRCode,
                    BankKey = row.BankKey,
                    BranchLocation = row.BranchLocation,
                    NameInAccount = row.NameInAccount,
                    AccountTypeKey = row.AccountTypeKey,
                    OpeningAccountBalance = row.OpeningAccountBalance,
                    CurrentAccountBalance = row.CurrentAccountBalance,
                    IsActive = row.IsActive,
                    BranchKeys = dbContext.BranchAccounts.Where(y => y.BankAccountKey == row.RowKey).Select(x => x.BranchKey).ToList()
                }).Where(x => x.RowKey == id).FirstOrDefault();
                if (model == null)
                {
                    model = new BankAccountViewModel();
                }
                FillDropdownLists(model);
                return model;
            }
            catch (Exception ex)
            {
                ActivityLog.CreateActivityLog(MenuConstants.BankAccount, ActionConstants.View, DbConstants.LogType.Error, null, ex.GetBaseException().Message);
                return new BankAccountViewModel();


            }
        }

        public BankAccountViewModel CreateBankAccount(BankAccountViewModel model)
        {
            FillDropdownLists(model);
            var AccountCheck = dbContext.BankAccounts.Where(row => row.AccountNumber.ToLower() == model.AccountNumber.ToLower()).ToList();
            BankAccount bankAccountModel = new BankAccount();

            if (AccountCheck.Count != 0)
            {
                model.Message = String.Format(EduSuiteUIResources.ExistsErrorMessage, EduSuiteUIResources.BankAccount);
                model.IsSuccessful = false;
                return model;
            }
            Bank bank = dbContext.Banks.Where(x => x.RowKey == model.BankKey).FirstOrDefault();

            using (var transaction = dbContext.Database.BeginTransaction())
            {
                try
                {
                    AccountHeadService accountHeadService = new AccountHeadService(dbContext);
                    AccountHeadViewModel accountHeadViewModel = new AccountHeadViewModel();
                    accountHeadViewModel.AccountHeadName = model.NameInAccount + EduSuiteUIResources.BlankSpace + model.AccountNumber + (bank != null ? EduSuiteUIResources.OpenBracketWithSpace + bank.BankName + EduSuiteUIResources.ClosingBracketWithSpace : "");

                    accountHeadViewModel.AccountHeadTypeKey = DbConstants.AccountHeadType.CurrentAssets;
                    accountHeadViewModel.IsActive = true;
                    accountHeadViewModel.IsSystemAccount = true;
                    accountHeadViewModel = accountHeadService.createAccountChart(accountHeadViewModel);

                    Int64 maxKey = dbContext.BankAccounts.Select(p => p.RowKey).DefaultIfEmpty().Max();
                    bankAccountModel.RowKey = Convert.ToInt64(maxKey + 1);
                    bankAccountModel.BranchKey = model.BranchKey;
                    bankAccountModel.AccountNumber = model.AccountNumber;
                    bankAccountModel.IFSCCode = model.IFSCCode;
                    bankAccountModel.MICRCode = model.MICRCode;
                    bankAccountModel.BankKey = model.BankKey;
                    bankAccountModel.BranchLocation = model.BranchLocation;
                    bankAccountModel.NameInAccount = model.NameInAccount;
                    bankAccountModel.AccountTypeKey = model.AccountTypeKey;
                    bankAccountModel.OpeningAccountBalance = model.OpeningAccountBalance;
                    bankAccountModel.CurrentAccountBalance = model.OpeningAccountBalance;
                    bankAccountModel.AccountHeadKey = accountHeadViewModel.RowKey;
                    bankAccountModel.IsActive = model.IsActive;

                    bankAccountModel.Bank = dbContext.Banks.SingleOrDefault(x => x.RowKey == model.BankKey);

                    dbContext.BankAccounts.Add(bankAccountModel);
                    model.RowKey = bankAccountModel.RowKey;
                    UpdateBankBranches(model);
                    //List<AccountFlowViewModel> accountFlowModelList = new List<AccountFlowViewModel>();
                    //DebitAmountList(accountFlowModelList, false, bankAccountModel);
                    //CreateAccountFlow(accountFlowModelList, false);

                    dbContext.SaveChanges();
                    transaction.Commit();
                    model.Message = EduSuiteUIResources.Success;
                    model.IsSuccessful = true;
                    ActivityLog.CreateActivityLog(MenuConstants.BankAccount, ActionConstants.Add, DbConstants.LogType.Info, model.RowKey, model.Message);

                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    model.Message = String.Format(EduSuiteUIResources.FailedToSaveErrorMessage, EduSuiteUIResources.BankAccount);
                    model.IsSuccessful = false;
                    ActivityLog.CreateActivityLog(MenuConstants.BankAccount, ActionConstants.Add, DbConstants.LogType.Error, model.RowKey, ex.GetBaseException().Message);

                }
            }
            return model;
        }

        public BankAccountViewModel UpdateBankAccount(BankAccountViewModel model)
        {
            FillDropdownLists(model);
            var AccountCheck = dbContext.BankAccounts.Where(row => row.RowKey != model.RowKey && row.AccountNumber.ToLower() == model.AccountNumber.ToLower()).ToList();
            BankAccount bankAccountModel = new BankAccount();

            if (AccountCheck.Count != 0)
            {
                model.Message = String.Format(EduSuiteUIResources.ExistsErrorMessage, EduSuiteUIResources.BankAccount);
                model.IsSuccessful = false;
                return model;
            }

            Bank bank = dbContext.Banks.Where(x => x.RowKey == model.BankKey).FirstOrDefault();

            using (var transaction = dbContext.Database.BeginTransaction())
            {
                try
                {


                    AccountHeadService accountHeadService = new AccountHeadService(dbContext);
                    AccountHeadViewModel accountHeadViewModel = new AccountHeadViewModel();
                    accountHeadViewModel.AccountHeadName = model.NameInAccount + EduSuiteUIResources.BlankSpace + model.AccountNumber + (bank != null ? EduSuiteUIResources.OpenBracketWithSpace + bank.BankName + EduSuiteUIResources.ClosingBracketWithSpace : "");
                    accountHeadViewModel.AccountHeadTypeKey = DbConstants.AccountHeadType.CurrentAssets;
                    accountHeadViewModel.IsActive = true;
                    accountHeadViewModel.IsSystemAccount = true;
                    bankAccountModel = dbContext.BankAccounts.SingleOrDefault(row => row.RowKey == model.RowKey);
                    accountHeadViewModel.RowKey = bankAccountModel.AccountHeadKey;
                    accountHeadViewModel = accountHeadService.updateAccountChart(accountHeadViewModel);

                    bankAccountModel.BranchKey = model.BranchKey;
                    bankAccountModel.AccountNumber = model.AccountNumber;
                    bankAccountModel.IFSCCode = model.IFSCCode;
                    bankAccountModel.MICRCode = model.MICRCode;
                    bankAccountModel.BankKey = model.BankKey;
                    bankAccountModel.BranchLocation = model.BranchLocation;
                    bankAccountModel.NameInAccount = model.NameInAccount;
                    bankAccountModel.AccountTypeKey = model.AccountTypeKey;
                    bankAccountModel.OpeningAccountBalance = model.OpeningAccountBalance;
                    bankAccountModel.AccountHeadKey = accountHeadViewModel.RowKey;
                    if (!dbContext.CashFlows.Any(row => row.BankAccountKey == model.RowKey))
                    {
                        bankAccountModel.CurrentAccountBalance = model.OpeningAccountBalance;
                    }
                    bankAccountModel.IsActive = model.IsActive;
                    bankAccountModel.Bank = dbContext.Banks.SingleOrDefault(x => x.RowKey == model.BankKey);

                    UpdateBankBranches(model);
                    //List<AccountFlowViewModel> accountFlowModelList = new List<AccountFlowViewModel>();
                    //DebitAmountList(accountFlowModelList, true, bankAccountModel);
                    //CreateAccountFlow(accountFlowModelList, true);

                    dbContext.SaveChanges();
                    transaction.Commit();
                    model.Message = EduSuiteUIResources.Success;
                    model.IsSuccessful = true;
                    ActivityLog.CreateActivityLog(MenuConstants.BankAccount, ActionConstants.Edit, DbConstants.LogType.Info, model.RowKey, model.Message);

                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    model.Message = String.Format(EduSuiteUIResources.FailedToSaveErrorMessage, EduSuiteUIResources.BankAccount);
                    model.IsSuccessful = false;
                    ActivityLog.CreateActivityLog(MenuConstants.BankAccount, ActionConstants.Edit, DbConstants.LogType.Error, model.RowKey, ex.GetBaseException().Message);

                }
            }
            return model;
        }

        public BankAccountViewModel DeleteBankAccount(BankAccountViewModel model)
        {
            using (var transaction = dbContext.Database.BeginTransaction())
            {
                try
                {
                    BankAccount BankAccountModel = dbContext.BankAccounts.SingleOrDefault(row => row.RowKey == model.RowKey);
                    long AccountHeadKey = BankAccountModel.AccountHeadKey;
                    List<BranchAccount> BankBranches = dbContext.BranchAccounts.Where(x => x.BankAccountKey == model.RowKey).ToList();
                    if (BankBranches.Count > 0)
                    {
                        dbContext.BranchAccounts.RemoveRange(BankBranches);
                    }
                    dbContext.BankAccounts.Remove(BankAccountModel);


                    AccountFlowViewModel accountFlowModel = new AccountFlowViewModel();
                    accountFlowModel.TransactionTypeKey = DbConstants.TransactionType.BankAccount;
                    accountFlowModel.TransactionKey = model.RowKey;
                    accountFlowModel.IsDelete = false;
                    AccountFlowService accountFlowService = new AccountFlowService(dbContext);
                    //accountFlowService.DeleteAccountFlow(accountFlowModel);
                    accountFlowService.DeleteAccountHead(AccountHeadKey);
                    dbContext.SaveChanges();
                    transaction.Commit();
                    model.Message = EduSuiteUIResources.Success;
                    model.IsSuccessful = true;
                    ActivityLog.CreateActivityLog(MenuConstants.BankAccount, ActionConstants.Delete, DbConstants.LogType.Info, model.RowKey, model.Message);


                }
                catch (DbUpdateException ex)
                {
                    transaction.Rollback();
                    if (ex.GetBaseException().Message.ToUpper().Contains(EduSuiteUIResources.ForeignKeyError.ToUpper()))
                    {
                        model.Message = String.Format(EduSuiteUIResources.CantDeleteErrorMessage, EduSuiteUIResources.BankAccount);
                        model.IsSuccessful = false;
                        ActivityLog.CreateActivityLog(MenuConstants.BankAccount, ActionConstants.Delete, DbConstants.LogType.Error, model.RowKey, ex.GetBaseException().Message);

                    }
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    model.Message = String.Format(EduSuiteUIResources.FailedToDeleteErrorMessage, EduSuiteUIResources.BankAccount);
                    model.IsSuccessful = false;
                    ActivityLog.CreateActivityLog(MenuConstants.BankAccount, ActionConstants.Delete, DbConstants.LogType.Error, model.RowKey, ex.GetBaseException().Message);

                }
            }

            return model;
        }

        public decimal GetAccountBalanceByAccount(Int64 Id)
        {
            return (dbContext.BankAccounts.Where(row => row.RowKey == Id).Select(row => row.CurrentAccountBalance ?? 0).SingleOrDefault());
        }

        private void FillAccountTypes(BankAccountViewModel model)
        {
            model.AccountTypes = dbContext.VwBankAccountTypeSelectActiveOnlies.Select(row => new SelectListModel
            {
                RowKey = row.RowKey,
                Text = row.AccountTypeName

            }).ToList();
        }
        private void FillBanks(BankAccountViewModel model)
        {
            model.Banks = dbContext.VwBankSelectActiveOnlies.Select(row => new SelectListModel
            {

                RowKey = row.RowKey,
                Text = row.BankName
            }).ToList();

        }
        private void FillDropdownLists(BankAccountViewModel model)
        {
            FillAccountTypes(model);
            FillBanks(model);
            GetBranches(model);
            GetBankBranches(model);
        }

        public BankAccountViewModel GetBranches(BankAccountViewModel model)
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
            return model;
        }
        #region Account
        private void CreateAccountFlow(List<AccountFlowViewModel> modelList, bool IsUpdate)
        {
            AccountFlowService accounFlowService = new AccountFlowService(dbContext);
            List<AccountFlowViewModel> accountFlowModelList = new List<AccountFlowViewModel>();
            if (IsUpdate != true)
            {
                accounFlowService.CreateAccountFlow(modelList);
            }
            else
            {
                accounFlowService.UpdateAccountFlow(modelList);
            }
        }
        private void DebitAmountList(List<AccountFlowViewModel> accountFlowModelList, bool IsUpdate, BankAccount BanckAccountModel)
        {
            long ExtraUpdateKey = 0;
            accountFlowModelList.Add(new AccountFlowViewModel
            {
                CashFlowTypeKey = DbConstants.CashFlowType.In,
                AccountHeadKey = BanckAccountModel.AccountHeadKey,
                Amount = BanckAccountModel.OpeningAccountBalance ?? 0,
                TransactionTypeKey = DbConstants.TransactionType.BankAccount,
                TransactionDate = DateTimeUTC.Now,
                TransactionKey = BanckAccountModel.RowKey,
                ExtraUpdateKey = ExtraUpdateKey,
                IsUpdate = IsUpdate,
                VoucherTypeKey = DbConstants.VoucherType.OpeningBalance,
                BranchKey = BanckAccountModel.BranchKey,
                Purpose = (BanckAccountModel.NameInAccount ?? BanckAccountModel.AccountNumber) + EduSuiteUIResources.Hyphen + BanckAccountModel.Bank.BankName + EduSuiteUIResources.BlankSpace + EduSuiteUIResources.OpeningBalance,

            });

            accountFlowModelList.Add(new AccountFlowViewModel
            {
                CashFlowTypeKey = DbConstants.CashFlowType.Out,
                AccountHeadKey = DbConstants.AccountHead.OpeningBalance,
                Amount = BanckAccountModel.OpeningAccountBalance ?? 0,
                TransactionTypeKey = DbConstants.TransactionType.BankAccount,
                TransactionDate = DateTimeUTC.Now,
                TransactionKey = BanckAccountModel.RowKey,
                ExtraUpdateKey = ExtraUpdateKey,
                IsUpdate = IsUpdate,
                VoucherTypeKey = DbConstants.VoucherType.OpeningBalance,
                BranchKey = BanckAccountModel.BranchKey,
                Purpose = (BanckAccountModel.NameInAccount ?? BanckAccountModel.AccountNumber) + EduSuiteUIResources.Hyphen + BanckAccountModel.Bank.BankName + EduSuiteUIResources.BlankSpace + EduSuiteUIResources.OpeningBalance,

            });

        }

        #endregion
        public void UpdateCurrentAccountBalance(BankAccountViewModel model, bool Edit, bool DeleteorWithdraw, decimal? OldAmount)
        {
            BankAccount bankAccount = dbContext.BankAccounts.SingleOrDefault(x => x.RowKey == model.RowKey);
            if (bankAccount != null)
            {
                if (Edit == false && DeleteorWithdraw == false)
                {
                    bankAccount.CurrentAccountBalance = (bankAccount.CurrentAccountBalance ?? 0) + (model.Amount ?? 0);
                }
                else if (Edit == false && DeleteorWithdraw == true)
                {
                    bankAccount.CurrentAccountBalance = (bankAccount.CurrentAccountBalance ?? 0) - (model.Amount ?? 0);
                }
                else if (Edit == true && DeleteorWithdraw == false)
                {
                    decimal? CurrentAccountBalance = bankAccount.CurrentAccountBalance;
                    CurrentAccountBalance = CurrentAccountBalance - OldAmount;
                    bankAccount.CurrentAccountBalance = (CurrentAccountBalance ?? 0) + (model.Amount ?? 0);
                }
                else if (Edit == true && DeleteorWithdraw == true)
                {
                    decimal? CurrentAccountBalance = bankAccount.CurrentAccountBalance;
                    CurrentAccountBalance = CurrentAccountBalance - OldAmount;
                    bankAccount.CurrentAccountBalance = (CurrentAccountBalance ?? 0);
                }

                dbContext.SaveChanges();
            }
        }
        private void UpdateBankBranches(BankAccountViewModel model)
        {
            Int32 MaxKey = dbContext.BranchAccounts.Select(x => x.RowKey).DefaultIfEmpty().Max();
            List<BranchAccount> BankBranches = dbContext.BranchAccounts.Where(x => x.BankAccountKey == model.RowKey).ToList();

            if (BankBranches.Count > 0)
            {
                dbContext.BranchAccounts.RemoveRange(BankBranches);
            }
            model.BranchKeys.Add(model.BranchKey ?? 0);
            if (model.BranchKeys != null && model.BranchKeys.Count > 0)
            {
                foreach (short BranchKey in model.BranchKeys)
                {
                    BranchAccount objBranchAccountmodel = new BranchAccount();

                    objBranchAccountmodel.RowKey = MaxKey + 1;
                    objBranchAccountmodel.BankAccountKey = model.RowKey;
                    objBranchAccountmodel.BranchKey = BranchKey;

                    dbContext.BranchAccounts.Add(objBranchAccountmodel);
                    dbContext.SaveChanges();
                    MaxKey++;
                }
            }
        }

        public BankAccountViewModel GetBankBranches(BankAccountViewModel model)
        {
            IQueryable<SelectListModel> BranchQuery = dbContext.vwBranchSelectActiveOnlies.OrderBy(row => row.DisplayOrder).Select(row => new SelectListModel
            {
                RowKey = row.RowKey,
                Text = row.BranchName
            });


            if (model.BranchKey != null && model.BranchKey != 0)
            {
                model.BankBranches = BranchQuery.Where(row => row.RowKey != model.BranchKey).ToList();
            }
            else
            {
                model.BankBranches = BranchQuery.ToList();
            }

            return model;
        }
    }
}
