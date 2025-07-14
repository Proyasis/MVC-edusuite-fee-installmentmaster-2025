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
    public class CashTransactionService : ICashTransactionService
    {
        private EduSuiteDatabase dbContext;

        public CashTransactionService(EduSuiteDatabase objDB)
        {
            this.dbContext = objDB;
        }

        public List<CashTransactionViewModel> GetCashTransactions(CashTransactionViewModel model, out long TotalRecords)
        {
            try
            {
                var Take = model.PageSize;
                var Skip = (model.PageIndex - 1) * model.PageSize;

                IQueryable<CashTransactionViewModel> CashTransactionsList = (from BT in dbContext.CashTransactions
                                                                             orderby BT.TransactionDate descending
                                                                             where (BT.Purpose.Contains(model.SearchText) ||
                                                                              BT.Remarks.Contains(model.SearchText))
                                                                             select new CashTransactionViewModel
                                                                             {
                                                                                 RowKey = BT.RowKey,
                                                                                 FromBranchKey = BT.FromBranchKey,
                                                                                 FromBranchName = BT.Branch.BranchName,
                                                                                 ToBranchKey = BT.ToBranchKey,
                                                                                 ToBranchName = BT.Branch1.BranchName,
                                                                                 TransactionDate = BT.TransactionDate,
                                                                                 Amount = BT.Amount,
                                                                                 Purpose = BT.Purpose,
                                                                                 PaidBy = BT.PaidBy,
                                                                                 AuthorizedBy = BT.AuthorizedBy,
                                                                                 ReceivedBy = BT.ReceivedBy,
                                                                                 OnBehalfOf = BT.OnBehalfOf,
                                                                                 Remarks = BT.Remarks
                                                                             });
                Employee Employee = dbContext.Employees.Where(x => x.AppUserKey == DbConstants.User.UserKey).SingleOrDefault();
                if (Employee != null)
                {
                    if (Employee.BranchAccess != null)
                    {
                        var Branches = Employee.BranchAccess.Split(',').Select(Int16.Parse).ToList();
                        CashTransactionsList = CashTransactionsList.Where(row => Branches.Contains(row.FromBranchKey ?? 0));
                    }
                }

                if (model.FromBranchKey != 0)
                {
                    CashTransactionsList = CashTransactionsList.Where(row => row.FromBranchKey == model.FromBranchKey);
                }
                if (model.SearchDate != null)
                {
                    CashTransactionsList = CashTransactionsList.Where(x => System.Data.Entity.DbFunctions.TruncateTime(x.TransactionDate) == System.Data.Entity.DbFunctions.TruncateTime(model.SearchDate));
                }

                CashTransactionsList = CashTransactionsList.GroupBy(x => x.RowKey).Select(y => y.FirstOrDefault());
                if (model.SortBy != "")
                {
                    CashTransactionsList = SortApplications(CashTransactionsList, model.SortBy, model.SortOrder);
                }
                TotalRecords = CashTransactionsList.Count();
                return CashTransactionsList.Skip(Skip).Take(Take).ToList<CashTransactionViewModel>();
            }
            catch (Exception ex)
            {
                TotalRecords = 0;
                ActivityLog.CreateActivityLog(MenuConstants.CashTransaction, ActionConstants.View, DbConstants.LogType.Error, null, ex.GetBaseException().Message);
                return new List<CashTransactionViewModel>();

            }
        }
        private IQueryable<CashTransactionViewModel> SortApplications(IQueryable<CashTransactionViewModel> Query, string SortName, string SortOrder)
        {

            string command = SortOrder == "desc" ? "OrderByDescending" : "OrderBy";
            var type = typeof(CashTransactionViewModel);
            var property = type.GetProperty(SortName);
            var parameter = Expression.Parameter(type, "p");
            var propertyAccess = Expression.MakeMemberAccess(parameter, property);
            var orderByExpression = Expression.Lambda(propertyAccess, parameter);
            var resultExpression = Expression.Call(typeof(Queryable), command, new Type[] { type, property.PropertyType },
                                          Query.Expression, Expression.Quote(orderByExpression));
            return Query.Provider.CreateQuery<CashTransactionViewModel>(resultExpression);

        }
        public CashTransactionViewModel GetCashTransactionsById(CashTransactionViewModel model)
        {
            CashTransactionViewModel CashTransactionViewModel = new CashTransactionViewModel();
            try
            {

                CashTransactionViewModel = dbContext.CashTransactions.Where(x => x.RowKey == model.RowKey).Select(row => new CashTransactionViewModel
                {
                    RowKey = row.RowKey,
                    FromBranchKey = row.FromBranchKey,
                    ToBranchKey = row.ToBranchKey,
                    TransactionDate = row.TransactionDate,

                    Amount = row.Amount,
                    Purpose = row.Purpose,
                    PaidBy = row.PaidBy,
                    AuthorizedBy = row.AuthorizedBy,
                    ReceivedBy = row.ReceivedBy,
                    OnBehalfOf = row.OnBehalfOf,
                    Remarks = row.Remarks

                }).FirstOrDefault();
                if (CashTransactionViewModel == null)
                {
                    CashTransactionViewModel = new CashTransactionViewModel();
                    CashTransactionViewModel.FromBranchKey = model.FromBranchKey;
                }
                FillDropdownLists(CashTransactionViewModel);
                return CashTransactionViewModel;
            }
            catch (Exception ex)
            {
                ActivityLog.CreateActivityLog(MenuConstants.CashTransaction, ActionConstants.View, DbConstants.LogType.Error, null, ex.GetBaseException().Message);
                return new CashTransactionViewModel();

            }
        }
        public CashTransactionViewModel CreateCashTransactions(CashTransactionViewModel model)
        {

            CashTransaction cashTransactionModel = new CashTransaction();
            FillDropdownLists(model);
            using (var transaction = dbContext.Database.BeginTransaction())
            {

                try
                {
                    Int64 maxKey = dbContext.CashTransactions.Select(p => p.RowKey).DefaultIfEmpty().Max();
                    cashTransactionModel.RowKey = maxKey + 1;
                    cashTransactionModel.FromBranchKey = model.FromBranchKey;
                    cashTransactionModel.ToBranchKey = model.ToBranchKey;
                    cashTransactionModel.TransactionDate = Convert.ToDateTime(model.TransactionDate);

                    cashTransactionModel.Amount = Convert.ToDecimal(model.Amount);
                    cashTransactionModel.PaidBy = model.PaidBy;
                    cashTransactionModel.AuthorizedBy = model.AuthorizedBy;
                    cashTransactionModel.ReceivedBy = model.ReceivedBy;
                    cashTransactionModel.OnBehalfOf = model.OnBehalfOf;
                    cashTransactionModel.Remarks = model.Remarks;
                    dbContext.CashTransactions.Add(cashTransactionModel);

                    model.RowKey = cashTransactionModel.RowKey;
                    purposeGeneration(model);
                    cashTransactionModel.Purpose = model.Purpose;
                    List<AccountFlowViewModel> accountFlowModelList = new List<AccountFlowViewModel>();

                    RecievableAmountList(cashTransactionModel.Amount, accountFlowModelList, false, model);
                    CreateAccountFlow(accountFlowModelList, false);



                    dbContext.SaveChanges();
                    transaction.Commit();
                    model.Message = EduSuiteUIResources.Success;
                    model.IsSuccessful = true;
                    ActivityLog.CreateActivityLog(MenuConstants.CashTransaction, ActionConstants.Add, DbConstants.LogType.Info, model.RowKey, model.Message);

                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    model.Message = String.Format(EduSuiteUIResources.FailedToSaveErrorMessage, EduSuiteUIResources.CashTransactions);
                    model.IsSuccessful = false;
                    ActivityLog.CreateActivityLog(MenuConstants.CashTransaction, ActionConstants.Add, DbConstants.LogType.Error, model.RowKey, ex.GetBaseException().Message);

                }
            }
            return model;
        }
        public CashTransactionViewModel UpdateCashTransactions(CashTransactionViewModel model)
        {
            CashTransaction cashTransactionModel = new CashTransaction();
            FillDropdownLists(model);
            using (var transaction = dbContext.Database.BeginTransaction())
            {

                try
                {
                    cashTransactionModel = dbContext.CashTransactions.SingleOrDefault(p => p.RowKey == model.RowKey);
                    model.OldFromBranchKey = cashTransactionModel.FromBranchKey;
                    model.OldToBranchKey = cashTransactionModel.ToBranchKey;
                    decimal? OldAmount = cashTransactionModel.Amount;


                    cashTransactionModel.FromBranchKey = model.FromBranchKey;
                    cashTransactionModel.TransactionDate = Convert.ToDateTime(model.TransactionDate);
                    cashTransactionModel.ToBranchKey = model.ToBranchKey;
                    cashTransactionModel.Amount = Convert.ToDecimal(model.Amount);
                    cashTransactionModel.PaidBy = model.PaidBy;
                    cashTransactionModel.AuthorizedBy = model.AuthorizedBy;
                    cashTransactionModel.ReceivedBy = model.ReceivedBy;
                    cashTransactionModel.OnBehalfOf = model.OnBehalfOf;
                    cashTransactionModel.Remarks = model.Remarks;

                    List<AccountFlowViewModel> accountFlowModelList = new List<AccountFlowViewModel>();
                    purposeGeneration(model);
                    cashTransactionModel.Purpose = model.Purpose;
                    RecievableAmountList(cashTransactionModel.Amount, accountFlowModelList, true, model);
                    CreateAccountFlow(accountFlowModelList, true);


                    dbContext.SaveChanges();
                    transaction.Commit();
                    model.Message = EduSuiteUIResources.Success;
                    model.IsSuccessful = true;
                    ActivityLog.CreateActivityLog(MenuConstants.CashTransaction, ActionConstants.Edit, DbConstants.LogType.Info, model.RowKey, model.Message);

                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    model.Message = String.Format(EduSuiteUIResources.FailedToSaveErrorMessage, EduSuiteUIResources.CashTransactions);
                    model.IsSuccessful = false;
                    ActivityLog.CreateActivityLog(MenuConstants.CashTransaction, ActionConstants.Edit, DbConstants.LogType.Error, model.RowKey, ex.GetBaseException().Message);

                }
            }
            return model;
        }
        public CashTransactionViewModel DeleteCashTransactions(CashTransactionViewModel model)
        {
            using (var transaction = dbContext.Database.BeginTransaction())
            {
                try
                {
                    CashTransaction cashTransaction = dbContext.CashTransactions.SingleOrDefault(row => row.RowKey == model.RowKey);


                    dbContext.CashTransactions.Remove(cashTransaction);

                    AccountFlowViewModel accountFlowModel = new AccountFlowViewModel();
                    accountFlowModel.TransactionTypeKey = DbConstants.TransactionType.CashTransaction;
                    accountFlowModel.TransactionKey = model.RowKey;

                    AccountFlowService accountFlowService = new AccountFlowService(dbContext);
                    accountFlowService.DeleteAccountFlow(accountFlowModel);

                    dbContext.SaveChanges();
                    transaction.Commit();
                    model.Message = EduSuiteUIResources.Success;
                    model.IsSuccessful = true;
                    ActivityLog.CreateActivityLog(MenuConstants.CashTransaction, ActionConstants.Delete, DbConstants.LogType.Info, model.RowKey, model.Message);
                }
                catch (DbUpdateException ex)
                {
                    transaction.Rollback();
                    if (ex.GetBaseException().Message.ToUpper().Contains(EduSuiteUIResources.ForeignKeyError.ToUpper()))
                    {
                        model.Message = String.Format(EduSuiteUIResources.CantDeleteErrorMessage, EduSuiteUIResources.CashTransactions);
                        model.IsSuccessful = false;
                        ActivityLog.CreateActivityLog(MenuConstants.CashTransaction, ActionConstants.Delete, DbConstants.LogType.Error, model.RowKey, ex.GetBaseException().Message);

                    }
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    model.Message = String.Format(EduSuiteUIResources.FailedToDeleteErrorMessage, EduSuiteUIResources.CashTransactions);
                    model.IsSuccessful = false;
                    ActivityLog.CreateActivityLog(MenuConstants.CashTransaction, ActionConstants.Delete, DbConstants.LogType.Error, model.RowKey, ex.GetBaseException().Message);

                }
            }
            return model;
        }
        public CashTransactionViewModel GetToBranchById(CashTransactionViewModel model)
        {
            IQueryable<SelectListModel> BranchQuery = dbContext.vwBranchSelectActiveOnlies.OrderBy(row => row.DisplayOrder).Select(row => new SelectListModel
            {
                RowKey = row.RowKey,
                Text = row.BranchName
            });


            if (model.FromBranchKey != null && model.FromBranchKey != 0)
            {
                model.ToBranch = BranchQuery.Where(row => row.RowKey != model.FromBranchKey).Distinct().ToList();
            }
            else
            {
                model.ToBranch = BranchQuery.Distinct().ToList();
            }

            return model;
        }
        private void FillDropdownLists(CashTransactionViewModel model)
        {
            GetToBranchById(model);
            GetBranches(model);
        }
        public void GetBranches(CashTransactionViewModel model)
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
                    model.FromBranch = BranchQuery.Where(row => Branches.Contains(row.RowKey)).ToList();
                    model.FromBranchKey = Employee.BranchKey;
                }
                else
                {
                    model.FromBranch = BranchQuery.Where(x => x.RowKey == Employee.BranchKey).ToList();
                    model.FromBranchKey = Employee.BranchKey;
                }
            }
            else
            {
                model.FromBranch = BranchQuery.ToList();
            }

            if (model.FromBranch.Count == 1)
            {
                long? branchkey = model.FromBranch.Select(x => x.RowKey).FirstOrDefault();
                model.FromBranchKey = Convert.ToInt16(branchkey);
            }

        }
        private void purposeGeneration(CashTransactionViewModel model)
        {
            model.FromBranchName = dbContext.Branches.Where(x => x.RowKey == model.FromBranchKey).Select(row => row.BranchName).FirstOrDefault();
            model.ToBranchName = dbContext.Branches.Where(x => x.RowKey == model.ToBranchKey).Select(row => row.BranchName).FirstOrDefault();
            if (model.Purpose == null || model.Purpose == "")
            {
                model.Purpose = EduSuiteUIResources.Cash + EduSuiteUIResources.BlankSpace + EduSuiteUIResources.Transferfrom + EduSuiteUIResources.BlankSpace + model.FromBranchName + EduSuiteUIResources.BlankSpace + EduSuiteUIResources.To + EduSuiteUIResources.BlankSpace + model.ToBranchName;
            }
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
        private void RecievableAmountList(decimal Amount, List<AccountFlowViewModel> accountFlowModelList, bool IsUpdate, CashTransactionViewModel CashTransactionViewModel)
        {

            long ExtraUpdateKey = 0;
            byte CashFlowTypeKey = DbConstants.CashFlowType.Out;
            long accountHeadKey;
            long? OldaccountHeadKey = null;


            accountHeadKey = dbContext.AccountHeads.Where(x => x.RowKey == DbConstants.AccountHead.CashAccount).Select(x => x.RowKey).FirstOrDefault();
            OldaccountHeadKey = dbContext.AccountHeads.Where(x => x.RowKey == DbConstants.AccountHead.CashAccount).Select(x => x.RowKey).FirstOrDefault();
            CashFlowTypeKey = DbConstants.CashFlowType.In;



            accountFlowModelList.Add(new AccountFlowViewModel
            {
                CashFlowTypeKey = CashFlowTypeKey,
                AccountHeadKey = accountHeadKey,
                OldAccountHeadKey = OldaccountHeadKey,
                Amount = Amount,
                TransactionTypeKey = DbConstants.TransactionType.CashTransaction,
                TransactionDate = Convert.ToDateTime(CashTransactionViewModel.TransactionDate),
                TransactionKey = CashTransactionViewModel.RowKey,
                ExtraUpdateKey = ExtraUpdateKey,
                IsUpdate = IsUpdate,
                VoucherTypeKey = DbConstants.VoucherType.CashTransaction,
                BranchKey = CashTransactionViewModel.ToBranchKey,
                Purpose = CashTransactionViewModel.Purpose + EduSuiteUIResources.BlankSpace + CashTransactionViewModel.Remarks,
            });



            OldaccountHeadKey = dbContext.AccountHeads.Where(x => x.RowKey == DbConstants.AccountHead.CashAccount).Select(x => x.RowKey).FirstOrDefault();
            accountHeadKey = dbContext.AccountHeads.Where(x => x.RowKey == DbConstants.AccountHead.CashAccount).Select(x => x.RowKey).FirstOrDefault();
            CashFlowTypeKey = DbConstants.CashFlowType.Out;


            accountFlowModelList.Add(new AccountFlowViewModel
            {
                CashFlowTypeKey = CashFlowTypeKey,
                AccountHeadKey = accountHeadKey,
                OldAccountHeadKey = OldaccountHeadKey,
                Amount = Amount,
                TransactionTypeKey = DbConstants.TransactionType.CashTransaction,
                TransactionDate = Convert.ToDateTime(CashTransactionViewModel.TransactionDate),
                TransactionKey = CashTransactionViewModel.RowKey,
                ExtraUpdateKey = ExtraUpdateKey,
                IsUpdate = IsUpdate,
                VoucherTypeKey = DbConstants.VoucherType.CashTransaction,
                BranchKey = CashTransactionViewModel.FromBranchKey,
                Purpose = CashTransactionViewModel.Purpose + EduSuiteUIResources.BlankSpace + CashTransactionViewModel.Remarks,
            });

        }
        #endregion
        public decimal CheckShortBalance(long Rowkey, short branchKey)
        {
            decimal Balance = 0;

            long accountHeadKey = dbContext.AccountHeads.Where(x => x.RowKey == DbConstants.AccountHead.CashAccount).Select(x => x.RowKey).FirstOrDefault();
            decimal totalDebit = dbContext.AccountFlows.Where(x => x.AccountHeadKey == accountHeadKey && x.CashFlowTypeKey == DbConstants.CashFlowType.In && x.BranchKey == branchKey).Select(x => x.Amount).DefaultIfEmpty().Sum();
            decimal totalCredit = dbContext.AccountFlows.Where(x => x.AccountHeadKey == accountHeadKey && x.CashFlowTypeKey == DbConstants.CashFlowType.Out && x.BranchKey == branchKey).Select(x => x.Amount).DefaultIfEmpty().Sum();
            Balance = totalDebit - totalCredit;
            if (Rowkey != 0)
            {
                var purchaseList = dbContext.CashTransactions.SingleOrDefault(x => x.RowKey == Rowkey);


                Balance = Balance + purchaseList.Amount;

            }

            return Balance;
        }


    }
}
