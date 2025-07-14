using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CITS.EduSuite.Business.Common;
using CITS.EduSuite.Business.Interfaces;
using CITS.EduSuite.Business.Models.ViewModels;
using CITS.EduSuite.Data;
using System.Data.Entity.Infrastructure;
using CITS.EduSuite.Business.Models.Resources;
using System.Linq.Expressions;

namespace CITS.EduSuite.Business.Services
{
    public class BankTransactionService : IBankTransactionService
    {
        private EduSuiteDatabase dbContext;

        public BankTransactionService(EduSuiteDatabase objDB)
        {
            this.dbContext = objDB;
        }

        public List<BankTransactionViewModel> GetBankTransactionsByType(BankTransactionViewModel model, out long TotalRecords)
        {
            try
            {
                var Take = model.PageSize;
                var Skip = (model.PageIndex - 1) * model.PageSize;

                IQueryable<BankTransactionViewModel> BankTransactionList = (from BT in dbContext.BankTransactions
                                                                            orderby BT.TransactionDate descending
                                                                            where (BT.ReceiptNumber.Contains(model.SearchText) || BT.Purpose.Contains(model.SearchText) || BT.Remarks.Contains(model.SearchText))
                                                                            select new BankTransactionViewModel
                                                                            {
                                                                                RowKey = BT.RowKey,
                                                                                BranchKey = BT.BranchKey,
                                                                                BranchName = BT.Branch.BranchName,
                                                                                TransactionDate = BT.TransactionDate,
                                                                                FromBankAccountName = (BT.BankAccount.NameInAccount ?? BT.BankAccount.AccountNumber) + "-" + BT.BankAccount.Bank.BankName,
                                                                                ToBankAccountName = (BT.BankAccount1.NameInAccount ?? BT.BankAccount1.AccountNumber) + "-" + BT.BankAccount1.Bank.BankName,
                                                                                BankTransactionTypeKey = BT.BankTransactionTypeKey,
                                                                                BankTransactionTypeName = BT.BankTransactionType.BankTransactionTypeName,
                                                                                Amount = BT.Amount,
                                                                                BankCharge = BT.BankCharge,
                                                                                Purpose = BT.Purpose,
                                                                                PaidBy = BT.PaidBy,
                                                                                AuthorizedBy = BT.AuthorizedBy,
                                                                                ReceivedBy = BT.ReceivedBy,
                                                                                OnBehalfOf = BT.OnBehalfOf,
                                                                                Remarks = BT.Remarks,
                                                                                ReceiptNumber = BT.ReceiptNumber,
                                                                                FromBankAccountKey = BT.FromBankAccountKey,
                                                                                ToBankAccountKey = BT.ToBankAccountKey
                                                                            });
                Employee Employee = dbContext.Employees.Where(x => x.AppUserKey == DbConstants.User.UserKey).SingleOrDefault();
                if (Employee != null)
                {
                    if (Employee.BranchAccess != null)
                    {
                        var Branches = Employee.BranchAccess.Split(',').Select(Int16.Parse).ToList();
                        BankTransactionList = BankTransactionList.Where(row => Branches.Contains(row.BranchKey));
                    }
                }

                if (model.BranchKey != 0)
                {
                    BankTransactionList = BankTransactionList.Where(row => row.BranchKey == model.BranchKey);
                }
                if (model.BankTransactionTypeKey != 0)
                {
                    BankTransactionList = BankTransactionList.Where(row => row.BankTransactionTypeKey == model.BankTransactionTypeKey);
                }
                if (model.SearchBankAccountKey != 0)
                {
                    BankTransactionList = BankTransactionList.Where(row => row.FromBankAccountKey == model.SearchBankAccountKey || row.ToBankAccountKey == model.SearchBankAccountKey);
                }
                if (model.SearchDate != null)
                {
                    BankTransactionList = BankTransactionList.Where(x => System.Data.Entity.DbFunctions.TruncateTime(x.TransactionDate) == System.Data.Entity.DbFunctions.TruncateTime(model.SearchDate));
                }
                BankTransactionList = BankTransactionList.GroupBy(x => x.RowKey).Select(y => y.FirstOrDefault());
                if (model.SortBy != "")
                {
                    BankTransactionList = SortApplications(BankTransactionList, model.SortBy, model.SortOrder);
                }
                TotalRecords = BankTransactionList.Count();
                return BankTransactionList.Skip(Skip).Take(Take).ToList<BankTransactionViewModel>();
            }
            catch (Exception ex)
            {
                TotalRecords = 0;
                ActivityLog.CreateActivityLog(MenuConstants.BankTransaction, ActionConstants.View, DbConstants.LogType.Error, null, ex.GetBaseException().Message);
                return new List<BankTransactionViewModel>();

            }
        }

        private IQueryable<BankTransactionViewModel> SortApplications(IQueryable<BankTransactionViewModel> Query, string SortName, string SortOrder)
        {

            string command = SortOrder == "desc" ? "OrderByDescending" : "OrderBy";
            var type = typeof(BankTransactionViewModel);
            var property = type.GetProperty(SortName);
            var parameter = Expression.Parameter(type, "p");
            var propertyAccess = Expression.MakeMemberAccess(parameter, property);
            var orderByExpression = Expression.Lambda(propertyAccess, parameter);
            var resultExpression = Expression.Call(typeof(Queryable), command, new Type[] { type, property.PropertyType },
                                          Query.Expression, Expression.Quote(orderByExpression));
            return Query.Provider.CreateQuery<BankTransactionViewModel>(resultExpression);

        }

        public BankTransactionViewModel GetBankTransactionById(BankTransactionViewModel model)
        {
            BankTransactionViewModel bankTransactionViewModel = new BankTransactionViewModel();
            try
            {

                bankTransactionViewModel = dbContext.BankTransactions.Where(x => x.RowKey == model.RowKey).Select(row => new BankTransactionViewModel
                {
                    RowKey = row.RowKey,
                    BranchKey = row.BranchKey,
                    TransactionDate = row.TransactionDate,
                    BankTransactionTypeKey = row.BankTransactionTypeKey,
                    BankTransactionTypeName = row.BankTransactionType.BankTransactionTypeName,
                    FromBankAccountKey = row.FromBankAccountKey ?? 0,
                    FromBankAccountBalance = row.BankAccount.CurrentAccountBalance,
                    ToBankAccountKey = row.ToBankAccountKey ?? 0,
                    ToBankAccountBalance = row.BankAccount1.CurrentAccountBalance,
                    Amount = row.Amount,
                    BankCharge = row.BankCharge,
                    Purpose = row.Purpose,
                    PaidBy = row.PaidBy,
                    AuthorizedBy = row.AuthorizedBy,
                    ReceivedBy = row.ReceivedBy,
                    OnBehalfOf = row.OnBehalfOf,
                    Remarks = row.Remarks

                }).FirstOrDefault();
                if (bankTransactionViewModel == null)
                {
                    bankTransactionViewModel = new BankTransactionViewModel();
                    bankTransactionViewModel.BranchKey = model.BranchKey;
                    bankTransactionViewModel.BankTransactionTypeKey = model.BankTransactionTypeKey;
                    bankTransactionViewModel.BankTransactionTypeName = dbContext.BankTransactionTypes.Where(row => row.RowKey == model.BankTransactionTypeKey).Select(row => row.BankTransactionTypeName).SingleOrDefault();
                }
                FillDropdownLists(bankTransactionViewModel);
                return bankTransactionViewModel;
            }
            catch (Exception ex)
            {
                ActivityLog.CreateActivityLog(MenuConstants.BankTransaction, ActionConstants.View, DbConstants.LogType.Error, null, ex.GetBaseException().Message);
                return new BankTransactionViewModel();

            }
        }

        public BankTransactionViewModel CreateBankTransaction(BankTransactionViewModel model)
        {

            BankTransaction bankTransactionModel = new BankTransaction();
            FillDropdownLists(model);
            using (var transaction = dbContext.Database.BeginTransaction())
            {

                try
                {
                    ConfigurationViewModel ConfigModel = new ConfigurationViewModel();
                    ConfigModel.BranchKey = model.BranchKey;
                    ConfigModel.ConfigType = model.BankTransactionTypeKey == DbConstants.BankTransactionType.Withdrawal ? DbConstants.PaymentReceiptConfigType.ReceiptVoucher : DbConstants.PaymentReceiptConfigType.Payment;
                    Configurations.GenerateReceipt(dbContext, ConfigModel);
                    model.ReceiptNumber = ConfigModel.ReceiptNumber;
                    Int64 maxKey = dbContext.BankTransactions.Select(p => p.RowKey).DefaultIfEmpty().Max();
                    bankTransactionModel.RowKey = maxKey + 1;
                    bankTransactionModel.BranchKey = model.BranchKey;
                    bankTransactionModel.TransactionDate = Convert.ToDateTime(model.TransactionDate);

                    if (model.BankTransactionTypeKey == DbConstants.BankTransactionType.Withdrawal)
                    {
                        model.ToBankAccountKey = null;
                    }
                    else if (model.BankTransactionTypeKey == DbConstants.BankTransactionType.Deposit)
                    {
                        model.FromBankAccountKey = null;
                    }
                    bankTransactionModel.BankTransactionTypeKey = model.BankTransactionTypeKey;
                    bankTransactionModel.FromBankAccountKey = model.FromBankAccountKey;
                    bankTransactionModel.ToBankAccountKey = model.ToBankAccountKey;
                    bankTransactionModel.Amount = Convert.ToDecimal(model.Amount);
                    bankTransactionModel.BankCharge = model.BankCharge;

                    bankTransactionModel.PaidBy = model.PaidBy;
                    bankTransactionModel.AuthorizedBy = model.AuthorizedBy;
                    bankTransactionModel.ReceivedBy = model.ReceivedBy;
                    bankTransactionModel.OnBehalfOf = model.OnBehalfOf;
                    bankTransactionModel.Remarks = model.Remarks;
                    bankTransactionModel.SerialNumber = ConfigModel.SerialNumber;
                    bankTransactionModel.ReceiptNumber = ConfigModel.ReceiptNumber;
                    dbContext.BankTransactions.Add(bankTransactionModel);

                    model.RowKey = bankTransactionModel.RowKey;
                    purposeGeneration(model);
                    bankTransactionModel.Purpose = model.Purpose;
                    List<AccountFlowViewModel> accountFlowModelList = new List<AccountFlowViewModel>();

                    RecievableAmountList(bankTransactionModel.Amount, accountFlowModelList, false, model);
                    CreateAccountFlow(accountFlowModelList, false);

                    //if (model.BankTransactionTypeKey == DbConstants.BankTransactionType.Deposit)
                    //{
                    //    BankAccountService bankAccountService = new BankAccountService(dbContext);
                    //    BankAccountViewModel bankAccountModel = new BankAccountViewModel();
                    //    bankAccountModel.RowKey = model.ToBankAccountKey ?? 0;
                    //    bankAccountModel.Amount = model.Amount;
                    //    bankAccountService.UpdateCurrentAccountBalance(bankAccountModel, false, false, null);
                    //}
                    //else if (model.BankTransactionTypeKey == DbConstants.BankTransactionType.Withdrawal)
                    //{
                    //    BankAccountService bankAccountService = new BankAccountService(dbContext);
                    //    BankAccountViewModel bankAccountModel = new BankAccountViewModel();
                    //    bankAccountModel.RowKey = model.FromBankAccountKey ?? 0;
                    //    bankAccountModel.Amount = model.Amount;
                    //    bankAccountService.UpdateCurrentAccountBalance(bankAccountModel, false, true, null);
                    //}
                    //else
                    //{
                    //    BankAccountService bankAccountService = new BankAccountService(dbContext);
                    //    BankAccountViewModel bankAccountModel = new BankAccountViewModel();
                    //    bankAccountModel.RowKey = model.FromBankAccountKey ?? 0;
                    //    bankAccountModel.Amount = model.Amount;
                    //    bankAccountService.UpdateCurrentAccountBalance(bankAccountModel, true, true, model.Amount);



                    //    BankAccountService bankAccountServices = new BankAccountService(dbContext);
                    //    BankAccountViewModel bankAccountModels = new BankAccountViewModel();
                    //    bankAccountModels.RowKey = model.ToBankAccountKey ?? 0;
                    //    bankAccountModels.Amount = model.Amount;
                    //    bankAccountServices.UpdateCurrentAccountBalance(bankAccountModel, false, false, null);                        
                    //}

                    dbContext.SaveChanges();
                    transaction.Commit();
                    model.Message = EduSuiteUIResources.Success;
                    model.IsSuccessful = true;
                    ActivityLog.CreateActivityLog(MenuConstants.BankTransaction, ActionConstants.Add, DbConstants.LogType.Info, model.RowKey, model.Message);

                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    model.Message = String.Format(EduSuiteUIResources.FailedToSaveErrorMessage, EduSuiteUIResources.BankTransaction);
                    model.IsSuccessful = false;
                    ActivityLog.CreateActivityLog(MenuConstants.BankTransaction, ActionConstants.Add, DbConstants.LogType.Error, model.RowKey, ex.GetBaseException().Message);

                }
            }
            return model;
        }

        public BankTransactionViewModel UpdateBankTransaction(BankTransactionViewModel model)
        {
            BankTransaction bankTransactionModel = new BankTransaction();
            FillDropdownLists(model);
            using (var transaction = dbContext.Database.BeginTransaction())
            {

                try
                {
                    bankTransactionModel = dbContext.BankTransactions.SingleOrDefault(p => p.RowKey == model.RowKey);
                    model.OldFromBankAccountKey = bankTransactionModel.FromBankAccountKey;
                    model.OldToBankAccountKey = bankTransactionModel.ToBankAccountKey;
                    decimal? OldAmount = bankTransactionModel.Amount;
                    if (model.BankTransactionTypeKey == DbConstants.BankTransactionType.Withdrawal)
                    {
                        model.ToBankAccountKey = null;
                    }
                    else if (model.BankTransactionTypeKey == DbConstants.BankTransactionType.Deposit)
                    {
                        model.FromBankAccountKey = null;
                    }

                    bankTransactionModel.BranchKey = model.BranchKey;
                    bankTransactionModel.TransactionDate = Convert.ToDateTime(model.TransactionDate);
                    bankTransactionModel.BankTransactionTypeKey = model.BankTransactionTypeKey;
                    bankTransactionModel.FromBankAccountKey = model.FromBankAccountKey;
                    bankTransactionModel.ToBankAccountKey = model.ToBankAccountKey;
                    bankTransactionModel.Amount = Convert.ToDecimal(model.Amount);
                    bankTransactionModel.BankCharge = model.BankCharge;

                    bankTransactionModel.PaidBy = model.PaidBy;
                    bankTransactionModel.AuthorizedBy = model.AuthorizedBy;
                    bankTransactionModel.ReceivedBy = model.ReceivedBy;
                    bankTransactionModel.OnBehalfOf = model.OnBehalfOf;
                    bankTransactionModel.Remarks = model.Remarks;

                    List<AccountFlowViewModel> accountFlowModelList = new List<AccountFlowViewModel>();
                    purposeGeneration(model);
                    bankTransactionModel.Purpose = model.Purpose;
                    RecievableAmountList(bankTransactionModel.Amount, accountFlowModelList, true, model);
                    CreateAccountFlow(accountFlowModelList, true);

                    //if (model.BankTransactionTypeKey == DbConstants.BankTransactionType.Deposit)
                    //{
                    //    if (model.OldToBankAccountKey == (model.ToBankAccountKey ?? 0))
                    //    {
                    //        BankAccountService bankAccountService = new BankAccountService(dbContext);
                    //        BankAccountViewModel bankAccountModel = new BankAccountViewModel();
                    //        bankAccountModel.RowKey = model.ToBankAccountKey ?? 0;
                    //        bankAccountModel.Amount = model.Amount;
                    //        bankAccountService.UpdateCurrentAccountBalance(bankAccountModel, true, false, OldAmount);
                    //    }
                    //    else if (model.OldToBankAccountKey != (model.ToBankAccountKey ?? 0))
                    //    {
                    //        BankAccountService bankAccountService = new BankAccountService(dbContext);
                    //        BankAccountViewModel bankAccountModel = new BankAccountViewModel();
                    //        bankAccountModel.RowKey = model.OldToBankAccountKey??0;
                    //        bankAccountModel.Amount = -(OldAmount);
                    //        bankAccountService.UpdateCurrentAccountBalance(bankAccountModel, true, true, OldAmount);

                    //        BankAccountService bankAccountServices = new BankAccountService(dbContext);
                    //        BankAccountViewModel bankAccountModels = new BankAccountViewModel();
                    //        bankAccountModels.RowKey = model.ToBankAccountKey ?? 0;
                    //        bankAccountModels.Amount = model.Amount;
                    //        bankAccountService.UpdateCurrentAccountBalance(bankAccountModel, false, false, null);
                    //    }                         
                    //}
                    //else if (model.BankTransactionTypeKey == DbConstants.BankTransactionType.Withdrawal)
                    //{
                    //    if (model.OldFromBankAccountKey == (model.FromBankAccountKey ?? 0))
                    //    {
                    //        BankAccountService bankAccountService = new BankAccountService(dbContext);
                    //        BankAccountViewModel bankAccountModel = new BankAccountViewModel();
                    //        bankAccountModel.RowKey = model.FromBankAccountKey ?? 0;
                    //        bankAccountModel.Amount = model.Amount;
                    //        bankAccountService.UpdateCurrentAccountBalance(bankAccountModel, true, true, OldAmount);
                    //    }
                    //    else if (model.OldFromBankAccountKey != (model.ToBankAccountKey ?? 0))
                    //    {
                    //        BankAccountService bankAccountService = new BankAccountService(dbContext);
                    //        BankAccountViewModel bankAccountModel = new BankAccountViewModel();
                    //        bankAccountModel.RowKey = model.OldFromBankAccountKey ?? 0;
                    //        bankAccountModel.Amount = -(OldAmount);
                    //        bankAccountService.UpdateCurrentAccountBalance(bankAccountModel, true, true, OldAmount);

                    //        BankAccountService bankAccountServices = new BankAccountService(dbContext);
                    //        BankAccountViewModel bankAccountModels = new BankAccountViewModel();
                    //        bankAccountModels.RowKey = model.FromBankAccountKey ?? 0;
                    //        bankAccountModels.Amount = model.Amount;
                    //        bankAccountService.UpdateCurrentAccountBalance(bankAccountModel, false, false, null);
                    //    }
                    //}
                    //else
                    //{
                    //    BankAccountService bankAccountService = new BankAccountService(dbContext);
                    //    BankAccountViewModel bankAccountModel = new BankAccountViewModel();
                    //    bankAccountModel.RowKey = model.FromBankAccountKey ?? 0;
                    //    bankAccountModel.Amount = model.Amount;
                    //    bankAccountService.UpdateCurrentAccountBalance(bankAccountModel, true, true, model.Amount);



                    //    BankAccountService bankAccountServices = new BankAccountService(dbContext);
                    //    BankAccountViewModel bankAccountModels = new BankAccountViewModel();
                    //    bankAccountModels.RowKey = model.ToBankAccountKey ?? 0;
                    //    bankAccountModels.Amount = model.Amount;
                    //    bankAccountServices.UpdateCurrentAccountBalance(bankAccountModel, false, false, null);
                    //}



                    dbContext.SaveChanges();
                    transaction.Commit();
                    model.Message = EduSuiteUIResources.Success;
                    model.IsSuccessful = true;
                    ActivityLog.CreateActivityLog(MenuConstants.BankTransaction, ActionConstants.Edit, DbConstants.LogType.Info, model.RowKey, model.Message);

                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    model.Message = String.Format(EduSuiteUIResources.FailedToSaveErrorMessage, EduSuiteUIResources.BankTransaction);
                    model.IsSuccessful = false;
                    ActivityLog.CreateActivityLog(MenuConstants.BankTransaction, ActionConstants.Edit, DbConstants.LogType.Error, model.RowKey, ex.GetBaseException().Message);

                }
            }
            return model;
        }

        public BankTransactionViewModel DeleteBankTransaction(BankTransactionViewModel model)
        {
            using (var transaction = dbContext.Database.BeginTransaction())
            {
                try
                {
                    BankTransaction bankTransaction = dbContext.BankTransactions.SingleOrDefault(row => row.RowKey == model.RowKey);
                    ConfigurationViewModel ConfigModel = new ConfigurationViewModel();
                    ConfigModel.BranchKey = bankTransaction.BranchKey;
                    ConfigModel.SerialNumber = bankTransaction.SerialNumber ?? 0;
                    ConfigModel.IsDelete = true;
                    ConfigModel.ConfigType = bankTransaction.BankTransactionTypeKey == DbConstants.BankTransactionType.Withdrawal ? DbConstants.PaymentReceiptConfigType.ReceiptVoucher : DbConstants.PaymentReceiptConfigType.Payment;
                    Configurations.GenerateReceipt(dbContext, ConfigModel);

                    dbContext.BankTransactions.Remove(bankTransaction);

                    AccountFlowViewModel accountFlowModel = new AccountFlowViewModel();
                    accountFlowModel.TransactionTypeKey = DbConstants.TransactionType.OtherBankTransaction;
                    accountFlowModel.TransactionKey = model.RowKey;

                    AccountFlowService accountFlowService = new AccountFlowService(dbContext);
                    accountFlowService.DeleteAccountFlow(accountFlowModel);

                    dbContext.SaveChanges();
                    transaction.Commit();
                    model.Message = EduSuiteUIResources.Success;
                    model.IsSuccessful = true;
                    ActivityLog.CreateActivityLog(MenuConstants.BankTransaction, ActionConstants.Delete, DbConstants.LogType.Info, model.RowKey, model.Message);


                }
                catch (DbUpdateException ex)
                {
                    transaction.Rollback();
                    if (ex.GetBaseException().Message.ToUpper().Contains(EduSuiteUIResources.ForeignKeyError.ToUpper()))
                    {
                        model.Message = String.Format(EduSuiteUIResources.CantDeleteErrorMessage, EduSuiteUIResources.BankTransaction);
                        model.IsSuccessful = false;
                        ActivityLog.CreateActivityLog(MenuConstants.BankTransaction, ActionConstants.Delete, DbConstants.LogType.Error, model.RowKey, ex.GetBaseException().Message);

                    }
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    model.Message = String.Format(EduSuiteUIResources.FailedToDeleteErrorMessage, EduSuiteUIResources.BankTransaction);
                    model.IsSuccessful = false;
                    ActivityLog.CreateActivityLog(MenuConstants.BankTransaction, ActionConstants.Delete, DbConstants.LogType.Error, model.RowKey, ex.GetBaseException().Message);

                }
            }
            return model;
        }

        public BankTransactionViewModel GetBankTransactionTypes(BankTransactionViewModel model)
        {
            FillBankTransactionTypes(model);
            GetBranches(model);
            return model;
        }

        public BankTransactionViewModel GetBankAccountById(BankTransactionViewModel model)
        {

            IQueryable<SelectListModel> BankAccountsQuery = dbContext.BranchAccounts.Where(x => x.BankAccount.IsActive && x.BranchKey == model.BranchKey).OrderBy(row => row.RowKey).Select(row => new SelectListModel
            {
                RowKey = row.BankAccount.RowKey,
                Text = (row.BankAccount.NameInAccount ?? row.BankAccount.AccountNumber) + EduSuiteUIResources.Hyphen + row.BankAccount.Bank.BankName
            });


            if (model.FromBankAccountKey != null && model.FromBankAccountKey != 0)
            {
                model.ToBankAccounts = BankAccountsQuery.Where(row => row.RowKey != model.FromBankAccountKey).Distinct().ToList();
            }
            else
            {
                model.ToBankAccounts = BankAccountsQuery.Distinct().ToList();
            }

            return model;
        }

        private void FillDropdownLists(BankTransactionViewModel model)
        {
            FillBankAccounts(model);
            GetBankAccountById(model);
            GetBranches(model);
        }

        public BankTransactionViewModel FillBankAccounts(BankTransactionViewModel model)
        {

            model.FromBankAccounts = dbContext.BranchAccounts.Where(x => x.BranchKey == model.BranchKey && x.BankAccount.IsActive).Select(row => new SelectListModel
            {
                RowKey = row.BankAccount.RowKey,
                Text = (row.BankAccount.NameInAccount ?? row.BankAccount.AccountNumber) + EduSuiteUIResources.Hyphen + row.BankAccount.Bank.BankName
            }).Distinct().ToList();

            return model;
        }

        private void FillBankTransactionTypes(BankTransactionViewModel model)
        {
            model.BankTransactionTypes = dbContext.BankTransactionTypes.OrderBy(row => row.DisplayOrder).Select(row => new SelectListModel
            {
                RowKey = row.RowKey,
                Text = row.BankTransactionTypeName
            }).ToList();
        }

        public BankTransactionViewModel GetBranches(BankTransactionViewModel model)
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
                    // model.BranchKey = Employee.BranchKey;
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

        private void purposeGeneration(BankTransactionViewModel model)
        {
            model.FromBankAccountName = dbContext.BankAccounts.Where(x => x.RowKey == model.FromBankAccountKey).Select(row => (row.NameInAccount ?? row.AccountNumber) + EduSuiteUIResources.Hyphen + row.Bank.BankName).FirstOrDefault();
            model.ToBankAccountName = dbContext.BankAccounts.Where(x => x.RowKey == model.ToBankAccountKey).Select(row => (row.NameInAccount ?? row.AccountNumber) + EduSuiteUIResources.Hyphen + row.Bank.BankName).FirstOrDefault();



            if (model.Purpose == null || model.Purpose == "")
            {
                if (model.BankTransactionTypeKey == DbConstants.BankTransactionType.AccountTransfer)
                {
                    model.Purpose = EduSuiteUIResources.Cash + EduSuiteUIResources.BlankSpace + EduSuiteUIResources.Transferfrom + EduSuiteUIResources.BlankSpace + model.FromBankAccountName + EduSuiteUIResources.BlankSpace + EduSuiteUIResources.To + EduSuiteUIResources.BlankSpace + model.ToBankAccountName;
                }
                else if (model.BankTransactionTypeKey == DbConstants.BankTransactionType.Deposit)
                {
                    model.Purpose = EduSuiteUIResources.Cash + EduSuiteUIResources.BlankSpace + EduSuiteUIResources.Deposited + EduSuiteUIResources.BlankSpace + EduSuiteUIResources.To + EduSuiteUIResources.BlankSpace + model.ToBankAccountName;

                }
                else
                {
                    model.Purpose = EduSuiteUIResources.Cash + EduSuiteUIResources.BlankSpace + EduSuiteUIResources.withdraw + EduSuiteUIResources.BlankSpace + EduSuiteUIResources.From + EduSuiteUIResources.BlankSpace + model.FromBankAccountName;

                }
            }
            // string PaidBy = model.BankTransactionTypeKey == DbConstants.BankTransactionType.Deposit(model.PaidBy != null && model.PaidBy != "" ? model.PaidBy : model.AccountHeadName) + EduSuiteUIResources.BlankSpace) : (model.PaidBy != null && model.PaidBy != "" ? EduSuiteUIResources.Paid + EduSuiteUIResources.BlankSpace + EduSuiteUIResources.By + EduSuiteUIResources.BlankSpace + model.PaidBy + EduSuiteUIResources.BlankSpace : "");
            //string ReceivedBy = model.IsOrderPayment == true && model.CashFlowTypeKey == DbConstants.CashFlowType.Out ? (EduSuiteUIResources.PaidTo + EduSuiteUIResources.BlankSpace + (model.ReceivedBy != null && model.ReceivedBy != "" ? model.ReceivedBy : model.AccountHeadName) + EduSuiteUIResources.BlankSpace) : (model.ReceivedBy != null && model.ReceivedBy != "" ? EduSuiteUIResources.Recieved + EduSuiteUIResources.BlankSpace + EduSuiteUIResources.By + EduSuiteUIResources.BlankSpace + model.ReceivedBy + EduSuiteUIResources.BlankSpace : "");
            //string OnBehalfOf = model.OnBehalfOf != null && model.OnBehalfOf != "" ? EduSuiteUIResources.OnBehalfOf + EduSuiteUIResources.BlankSpace + model.OnBehalfOf + EduSuiteUIResources.BlankSpace : "";
            //string AuthorizedBy = model.AuthorizedBy != null && model.AuthorizedBy != "" ? EduSuiteUIResources.AuthorizedBy + EduSuiteUIResources.BlankSpace + model.AuthorizedBy + EduSuiteUIResources.BlankSpace : "";
            //model.Remarks = model.Remarks != null && model.Remarks != "" ? EduSuiteUIResources.OpenBracket + model.Remarks + EduSuiteUIResources.CloseBracket : "";
            //model.Purpose = model.Purpose + (model.CashFlowTypeKey == DbConstants.CashFlowType.In ? PaidBy + ReceivedBy : ReceivedBy + PaidBy) + OnBehalfOf + AuthorizedBy;
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

        private void RecievableAmountList(decimal Amount, List<AccountFlowViewModel> accountFlowModelList, bool IsUpdate, BankTransactionViewModel BankTransactionViewModel)
        {


            long ExtraUpdateKey = 0;
            byte CashFlowTypeKey = DbConstants.CashFlowType.Out;
            long accountHeadKey;
            long? OldaccountHeadKey = null;

            if (BankTransactionViewModel.BankTransactionTypeKey == DbConstants.BankTransactionType.AccountTransfer)
            {
                accountHeadKey = dbContext.BankAccounts.Where(x => x.RowKey == BankTransactionViewModel.ToBankAccountKey).Select(x => x.AccountHeadKey).FirstOrDefault();
                OldaccountHeadKey = dbContext.BankAccounts.Where(x => x.RowKey == BankTransactionViewModel.OldToBankAccountKey).Select(x => x.AccountHeadKey).FirstOrDefault();
                CashFlowTypeKey = DbConstants.CashFlowType.In;
            }
            else if (BankTransactionViewModel.BankTransactionTypeKey == DbConstants.BankTransactionType.Deposit)
            {
                accountHeadKey = dbContext.AccountHeads.Where(x => x.RowKey == DbConstants.AccountHead.CashAccount).Select(x => x.RowKey).FirstOrDefault();
                //OldaccountHeadKey = dbContext.BankAccounts.Where(x => x.RowKey == BankTransactionViewModel.OldToBankAccountKey).Select(x => x.RowKey).FirstOrDefault();
                CashFlowTypeKey = DbConstants.CashFlowType.Out;
            }
            else
            {
                accountHeadKey = dbContext.AccountHeads.Where(x => x.RowKey == DbConstants.AccountHead.CashAccount).Select(x => x.RowKey).FirstOrDefault();
                //OldaccountHeadKey = accountHeadKey;
                CashFlowTypeKey = DbConstants.CashFlowType.In;
            }

            accountFlowModelList.Add(new AccountFlowViewModel
            {
                CashFlowTypeKey = CashFlowTypeKey,
                AccountHeadKey = accountHeadKey,
                OldAccountHeadKey = OldaccountHeadKey,
                Amount = Amount,
                TransactionTypeKey = DbConstants.TransactionType.OtherBankTransaction,
                TransactionDate = Convert.ToDateTime(BankTransactionViewModel.TransactionDate),
                TransactionKey = BankTransactionViewModel.RowKey,
                ExtraUpdateKey = ExtraUpdateKey,
                IsUpdate = IsUpdate,
                VoucherTypeKey = DbConstants.VoucherType.OtherBankTransaction,
                BranchKey = BankTransactionViewModel.BranchKey,
                Purpose = BankTransactionViewModel.Purpose + EduSuiteUIResources.BlankSpace + BankTransactionViewModel.Remarks,
            });



            if (BankTransactionViewModel.BankTransactionTypeKey == DbConstants.BankTransactionType.Deposit)
            {
                accountHeadKey = dbContext.BankAccounts.Where(x => x.RowKey == BankTransactionViewModel.ToBankAccountKey).Select(x => x.AccountHeadKey).FirstOrDefault();
                OldaccountHeadKey = dbContext.BankAccounts.Where(x => x.RowKey == BankTransactionViewModel.OldToBankAccountKey).Select(x => x.AccountHeadKey).FirstOrDefault();
                CashFlowTypeKey = DbConstants.CashFlowType.In;
            }
            else
            {
                OldaccountHeadKey = dbContext.BankAccounts.Where(x => x.RowKey == BankTransactionViewModel.OldFromBankAccountKey).Select(x => x.AccountHeadKey).FirstOrDefault();
                accountHeadKey = dbContext.BankAccounts.Where(x => x.RowKey == BankTransactionViewModel.FromBankAccountKey).Select(x => x.AccountHeadKey).FirstOrDefault();
                CashFlowTypeKey = DbConstants.CashFlowType.Out;
            }

            accountFlowModelList.Add(new AccountFlowViewModel
            {
                CashFlowTypeKey = CashFlowTypeKey,
                AccountHeadKey = accountHeadKey,
                OldAccountHeadKey = OldaccountHeadKey,
                Amount = Amount,
                TransactionTypeKey = DbConstants.TransactionType.OtherBankTransaction,
                TransactionDate = Convert.ToDateTime(BankTransactionViewModel.TransactionDate),
                TransactionKey = BankTransactionViewModel.RowKey,
                ExtraUpdateKey = ExtraUpdateKey,
                IsUpdate = IsUpdate,
                VoucherTypeKey = DbConstants.VoucherType.OtherBankTransaction,
                BranchKey = BankTransactionViewModel.BranchKey,
                Purpose = BankTransactionViewModel.Purpose + EduSuiteUIResources.BlankSpace + BankTransactionViewModel.Remarks,
            });

        }

        #endregion

        public decimal CheckShortBalance(long Rowkey, long BankAccountKey, short branchKey, byte BankTransactionTypeKey)
        {
            decimal Balance = 0;
            if (BankTransactionTypeKey == DbConstants.BankTransactionType.Deposit)
            {
                long accountHeadKey = dbContext.AccountHeads.Where(x => x.RowKey == DbConstants.AccountHead.CashAccount).Select(x => x.RowKey).FirstOrDefault();
                decimal totalDebit = dbContext.AccountFlows.Where(x => x.AccountHeadKey == accountHeadKey && x.CashFlowTypeKey == DbConstants.CashFlowType.In && x.BranchKey == branchKey).Select(x => x.Amount).DefaultIfEmpty().Sum();
                decimal totalCredit = dbContext.AccountFlows.Where(x => x.AccountHeadKey == accountHeadKey && x.CashFlowTypeKey == DbConstants.CashFlowType.Out && x.BranchKey == branchKey).Select(x => x.Amount).DefaultIfEmpty().Sum();
                Balance = totalDebit - totalCredit;
                if (Rowkey != 0)
                {
                    var purchaseList = dbContext.BankTransactions.SingleOrDefault(x => x.RowKey == Rowkey);


                    Balance = Balance + purchaseList.Amount;

                }
            }
            else if (BankTransactionTypeKey == DbConstants.BankTransactionType.Withdrawal || BankTransactionTypeKey == DbConstants.BankTransactionType.AccountTransfer)
            {
                if (BankAccountKey != 0 && BankAccountKey != null)
                {
                    long accountHeadKey = dbContext.BankAccounts.Where(x => x.RowKey == BankAccountKey).Select(x => x.AccountHeadKey).FirstOrDefault();
                    decimal totalDebit = dbContext.AccountFlows.Where(x => x.AccountHeadKey == accountHeadKey && x.CashFlowTypeKey == DbConstants.CashFlowType.In).Select(x => x.Amount).DefaultIfEmpty().Sum();
                    decimal totalCredit = dbContext.AccountFlows.Where(x => x.AccountHeadKey == accountHeadKey && x.CashFlowTypeKey == DbConstants.CashFlowType.Out).Select(x => x.Amount).DefaultIfEmpty().Sum();
                    Balance = totalDebit - totalCredit;
                    if (Rowkey != 0)
                    {
                        var purchaseList = dbContext.BankTransactions.SingleOrDefault(x => x.RowKey == Rowkey);
                        if (BankAccountKey == purchaseList.FromBankAccountKey)
                        {
                            //Balance = Balance + purchaseList.Amount;
                            Balance = Balance + purchaseList.Amount;
                        }
                    }
                }
            }
            return Balance;
        }

    }
}
