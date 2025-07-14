using CITS.EduSuite.Business.Common;
using CITS.EduSuite.Business.Interfaces;
using CITS.EduSuite.Business.Models.Resources;
using CITS.EduSuite.Business.Models.Security;
using CITS.EduSuite.Business.Models.ViewModels;
using CITS.EduSuite.Data;
using System;
using System.Collections.Generic;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace CITS.EduSuite.Business.Services
{
    public class FutureTransactionService : IFutureTransactionService
    {
        EduSuiteDatabase dbContext;
        public FutureTransactionService(EduSuiteDatabase objDB)
        {
            this.dbContext = objDB;
        }

        #region FutureTransaction
        public List<FutureTransactionViewModel> GetFutureTransaction(FutureTransactionViewModel model, string searchText)
        {
            try
            {
                var Take = model.PageSize;
                var Skip = (model.PageIndex - 1) * model.PageSize;
                IQueryable<FutureTransactionViewModel> FutureTransactionList = (from op in dbContext.FutureTransactions
                                                                                where (
                                                                                (model.BranchKey != 0) ?
                                                                                op.BranchKey == model.BranchKey &&
                                                                                (op.AccountHead.AccountHeadName.Contains(searchText)) :
                                                                                (op.AccountHead.AccountHeadName.Contains(searchText))
                                                                                )
                                                                                orderby op.RowKey descending
                                                                                select new FutureTransactionViewModel
                                                                                {
                                                                                    BranchKey = op.BranchKey,
                                                                                    BranchName = op.Branch.BranchName,
                                                                                    RowKey = op.RowKey,
                                                                                    AccountHeadName = op.AccountHead.AccountHeadName,
                                                                                    BillNo = op.BillNo,
                                                                                    BillDate = op.BillDate,
                                                                                    TotalAmount = op.TotalAmount,
                                                                                    TotalInAmount = op.FutureTransactionPayments.Where(x => x.CashFlowTypeKey == DbConstants.CashFlowType.In).Select(x => x.PaidAmount ?? 0).DefaultIfEmpty().Sum() + (op.OpeningReceivedAmount ?? 0),
                                                                                    TotalOutAmount = op.FutureTransactionPayments.Where(x => x.CashFlowTypeKey == DbConstants.CashFlowType.Out).Select(x => x.PaidAmount ?? 0).DefaultIfEmpty().Sum() + (op.OpeningPaidAmount ?? 0),
                                                                                });
                Employee Employee = dbContext.Employees.Where(x => x.AppUserKey == DbConstants.User.UserKey).SingleOrDefault();

                if (Employee != null)
                {
                    if (Employee.BranchAccess != null)
                    {
                        var Branches = Employee.BranchAccess.Split(',').Select(Int16.Parse).ToList();
                        FutureTransactionList = FutureTransactionList.Where(row => Branches.Contains(row.BranchKey));
                    }
                }
                if (model.BranchKey != 0)
                {
                    FutureTransactionList = FutureTransactionList.Where(row => row.BranchKey == model.BranchKey);
                }
                if (model.SortBy != "")
                {
                    FutureTransactionList = SortSalesOrder(FutureTransactionList, model.SortBy, model.SortOrder);
                }
                model.TotalRecords = FutureTransactionList.Count();
                return model.PageSize != 0 ? FutureTransactionList.Skip(Skip).Take(Take).ToList() : FutureTransactionList.ToList();

            }
            catch (Exception ex)
            {
                ActivityLog.CreateActivityLog(MenuConstants.FutureTransaction, ActionConstants.View, DbConstants.LogType.Error, model.RowKey, ex.GetBaseException().Message);
                return new List<FutureTransactionViewModel>();
            }
        }
        private IQueryable<FutureTransactionViewModel> SortSalesOrder(IQueryable<FutureTransactionViewModel> Query, string SortName, string SortOrder)
        {

            string command = SortOrder == "desc" ? "OrderByDescending" : "OrderBy";
            var type = typeof(FutureTransactionViewModel);
            var property = type.GetProperty(SortName);
            var parameter = Expression.Parameter(type, "p");
            var propertyAccess = Expression.MakeMemberAccess(parameter, property);
            var orderByExpression = Expression.Lambda(propertyAccess, parameter);
            var resultExpression = Expression.Call(typeof(Queryable), command, new Type[] { type, property.PropertyType },
                                          Query.Expression, Expression.Quote(orderByExpression));
            return Query.Provider.CreateQuery<FutureTransactionViewModel>(resultExpression);

        }
        public FutureTransactionViewModel GetFutureTransactionById(FutureTransactionViewModel objViewModel)
        {
            try
            {
                objViewModel = (from row in dbContext.FutureTransactions.Where(x => x.RowKey == objViewModel.RowKey)
                                join r in dbContext.FutureTransactionPayments.Where(r => r.IsAdavance == true)
                                on row.RowKey equals r.FutureTransactionKey into joined
                                from j in joined.DefaultIfEmpty()
                                select new FutureTransactionViewModel
                                {
                                    RowKey = row.RowKey,
                                    BranchKey = row.BranchKey,
                                    StateKey = row.StateKey,
                                    AccountHeadKey = row.AccountHeadKey,
                                    AccountGroupKey = row.AccountHead.AccountHeadType.AccountGroupKey,
                                    BillDate = row.BillDate,
                                    BillNo = row.BillNo,
                                    TotalAmount = row.TotalAmount,
                                    AmountPaid = row.AmountPaid,
                                    FutureTransactionPaymentRowKey = j.RowKey != null ? j.RowKey : 0,
                                    PaymentModeKey = j.PaymentModeKey != null ? j.PaymentModeKey : DbConstants.PaymentMode.Cash,
                                    MasterCashFlowTypeKey = row.CashFlowTypeKey,
                                    PaymentModeSubKey = j.PaymentModeSubKey,
                                    CardNumber = j.CardNumber,
                                    ReferenceNumber = j.ReferenceNumber,
                                    BankAccountKey = j.BankAccountKey,
                                    ChequeOrDDNumber = j.ChequeOrDDNumber,
                                    ChequeOrDDDate = j.ChequeOrDDDate,
                                    TaxableAmount = row.TaxableAmount,
                                    NonTaxableAmount = row.NonTaxableAmount,
                                    CGSTAmt = row.CGSTAmt,
                                    SGSTAmt = row.SGSTAmt,
                                    IGSTAmt = row.IGSTAmt,
                                    CGSTPer = row.CGSTPer,
                                    SGSTPer = row.SGSTPer,
                                    IGSTPer = row.IGSTPer,
                                    HSNCodeKey = row.HSNCodeMasterKey,
                                    HSNCode = row.HSNCodes,
                                    RoundOff = row.RoundOff,
                                    SubTotal = row.SubTotal,
                                    CompanyStateKey = row.StateKey,

                                    IsTax = row.IsTax,
                                    IsOpeningBalance = row.IsOpeningBalance,
                                    OpeningPaidAmount = row.OpeningPaidAmount,
                                    OpeningReceivedAmount = row.OpeningReceivedAmount,
                                    IsInstallment = row.IsInstallment,
                                    InstallmentAmount = row.InstallmentAmount,
                                    InstallmentPeriod = row.InstallmentPeriod,
                                    InstallmentTypeKey = row.InstallmentTypeKey,
                                    NoOfInstallment = row.NoOfInstallment,
                                    CashFlowTypeKey = j.CashFlowTypeKey,
                                    DownPayment = row.DownPayment,
                                    Amount = row.Amount,
                                    GSTINNumber = row.GSTINNumber,
                                    IsContra = row.IsContra,
                                    InstallmentFlowKey = row.InstallmentFlowKey,
                                    DeductAmount = row.FutureTransactionOtherAmountTypes.Where(x => x.IsAddition == false).Select(x => x.Amount).DefaultIfEmpty().Sum(),
                                    EarningAmount = row.FutureTransactionOtherAmountTypes.Where(x => x.IsAddition == true).Select(x => x.Amount).DefaultIfEmpty().Sum(),
                                }).FirstOrDefault();
                if (objViewModel == null)
                {
                    objViewModel = new FutureTransactionViewModel();
                    objViewModel.CGSTAmt = 0;
                    objViewModel.SGSTAmt = 0;
                    objViewModel.PaymentModeKey = DbConstants.PaymentMode.Cash;
                }

                FillFutureTransactionDropdowns(objViewModel);
                FillTransactionPaymentDropdownLists(objViewModel);
                FillFutureTransactionOtherAmountType(objViewModel);
                //if (objViewModel.BranchKey != null && objViewModel.BranchKey != 0)
                //{
                //    objViewModel.OrderProcessTypeKey = null;
                //}
                return objViewModel;
            }
            catch (Exception ex)
            {
                objViewModel = new FutureTransactionViewModel();
                objViewModel.Message = ex.GetBaseException().Message;
                return objViewModel;
            }
        }
        public FutureTransactionViewModel CreateFutureTransaction(FutureTransactionViewModel model)
        {
            FutureTransaction orderMasterModel = new FutureTransaction();
            using (var transaction = dbContext.Database.BeginTransaction())
            {
                try
                {
                    long MaxKey = dbContext.FutureTransactions.Select(p => p.RowKey).DefaultIfEmpty().Max();
                    orderMasterModel.RowKey = Convert.ToInt64(MaxKey + 1);
                    long oldAccountHeadKey = orderMasterModel.AccountHeadKey;

                    byte oldCashFlowTypeKey = 0;
                    byte oldAccountGroupKey = 0;
                    //string partyName = dbContext.Parties.Where(x => x.RowKey == model.AccountHeadKey).Select(x => x.PartyName).FirstOrDefault();
                    orderMasterModel.AccountHeadKey = model.AccountHeadKey;
                    orderMasterModel.BillDate = model.BillDate;
                    orderMasterModel.BillNo = model.BillNo;
                    orderMasterModel.TotalAmount = Convert.ToDecimal(model.TotalAmount ?? 0);
                    orderMasterModel.AmountPaid = Convert.ToDecimal(model.AmountPaid ?? 0);
                    orderMasterModel.TaxableAmount = model.TaxableAmount;
                    orderMasterModel.NonTaxableAmount = model.NonTaxableAmount;
                    orderMasterModel.RoundOff = model.RoundOff;
                    orderMasterModel.SubTotal = model.SubTotal;
                    orderMasterModel.Amount = model.Amount ?? 0;
                    //orderMasterModel.OrderProcessTypeKey = model.OrderProcessTypeKey;
                    if (model.StateKey == model.CompanyStateKey)
                    {
                        orderMasterModel.CGSTAmt = model.CGSTAmt;
                        orderMasterModel.SGSTAmt = model.SGSTAmt;
                        orderMasterModel.CGSTPer = model.CGSTPer;
                        orderMasterModel.SGSTPer = model.SGSTPer;
                    }
                    else
                    {
                        orderMasterModel.IGSTAmt = model.IGSTAmt;
                        orderMasterModel.IGSTPer = model.IGSTPer;
                    }
                    orderMasterModel.IsInstallment = model.IsInstallment;
                    orderMasterModel.IsTax = model.IsTax;
                    orderMasterModel.IsContra = model.IsContra;
                    orderMasterModel.InstallmentTypeKey = model.InstallmentTypeKey;
                    orderMasterModel.InstallmentAmount = model.InstallmentAmount;
                    orderMasterModel.InstallmentPeriod = model.InstallmentPeriod;
                    orderMasterModel.DownPayment = model.DownPayment;
                    orderMasterModel.NoOfInstallment = model.NoOfInstallment;
                    orderMasterModel.InstallmentFlowKey = model.InstallmentFlowKey;
                    orderMasterModel.HSNCodeMasterKey = model.HSNCodeKey;
                    orderMasterModel.HSNCodes = model.HSNCode;


                    orderMasterModel.GSTINNumber = model.GSTINNumber;
                    orderMasterModel.CashFlowTypeKey = model.MasterCashFlowTypeKey ?? 0;
                    orderMasterModel.StateKey = model.StateKey;
                    orderMasterModel.BranchKey = model.BranchKey;
                    orderMasterModel.IsOpeningBalance = model.IsOpeningBalance;
                    orderMasterModel.OpeningPaidAmount = model.OpeningPaidAmount;
                    orderMasterModel.OpeningReceivedAmount = model.OpeningReceivedAmount;
                    dbContext.FutureTransactions.Add(orderMasterModel);


                    model.RowKey = orderMasterModel.RowKey;
                    CreateFutureTransactionOtherAmountType(model.FutureTransactionOtherAmountTypes.Where(row => row.RowKey == 0 || row.RowKey == null).ToList(), model, orderMasterModel.RowKey);

                    if (model.AmountPaid != null && model.AmountPaid != 0)
                    {
                        CreateFutureTransactionPayment(model, orderMasterModel.RowKey, oldAccountHeadKey);
                    }

                    short? branchKey = model.BranchKey;
                    List<AccountFlowViewModel> AccountFlowList = new List<AccountFlowViewModel>();
                    bool IsUpdate = false;
                    var accountHeadList = dbContext.AccountHeads.SingleOrDefault(x => x.RowKey == model.AccountHeadKey);
                    if (model.AmountPaid != model.TotalAmount)
                    {
                        AccountFlowList = CreditTotalAmountList(orderMasterModel, AccountFlowList, IsUpdate, branchKey, oldAccountHeadKey);
                        //AccountFlowList = CreditTotalAmountList(orderMasterModel, AccountFlowList, IsUpdate, branchKey, oldAccountHeadKey, oldCashFlowTypeKey, oldAccountGroupKey);

                    }
                    else if (accountHeadList.AccountHeadType.AccountGroupKey != DbConstants.AccountGroup.Income && accountHeadList.AccountHeadType.AccountGroupKey != DbConstants.AccountGroup.Expenses)
                    {
                        AccountFlowList = CreditTotalAmountList(orderMasterModel, AccountFlowList, IsUpdate, branchKey, oldAccountHeadKey);
                        //AccountFlowList = CreditTotalAmountList(orderMasterModel, AccountFlowList, IsUpdate, branchKey, oldAccountHeadKey, oldCashFlowTypeKey, oldAccountGroupKey);

                    }
                    //AccountFlowList = DebitTotalAmountList(orderMasterModel, AccountFlowList, IsUpdate, branchKey);

                    if (model.StateKey == model.CompanyStateKey)
                    {
                        if (model.CGSTAmt != null && model.CGSTAmt != 0)
                        {
                            AccountFlowList = CreditCGSTList(orderMasterModel, AccountFlowList, IsUpdate, branchKey);
                        }
                        if (model.SGSTAmt != null && model.SGSTAmt != 0)
                        {
                            AccountFlowList = CreditSGSTList(orderMasterModel, AccountFlowList, IsUpdate, branchKey);
                        }
                    }
                    else
                    {
                        if (model.IGSTAmt != null && model.IGSTAmt != 0)
                        {
                            AccountFlowList = CreditIGSTList(orderMasterModel, AccountFlowList, IsUpdate, branchKey);
                        }
                    }
                    //if (model.IsOpeningBalance == true)
                    //{
                    //    if (model.OpeningPaidAmount != 0 && model.OpeningPaidAmount != null)
                    //    {
                    //        if ((accountHeadList.AccountHeadType.AccountGroupKey == DbConstants.AccountGroup.Income || accountHeadList.AccountHeadType.AccountGroupKey == DbConstants.AccountGroup.Expenses))
                    //        {
                    //            AccountFlowList = OpeningPayableAmountList(orderMasterModel, AccountFlowList, DbConstants.CashFlowType.Out, (model.OpeningPaidAmount ?? 0), false);
                    //        }
                    //        else
                    //        {
                    //            AccountFlowList = OpeningDebitAmountList(orderMasterModel, AccountFlowList, DbConstants.CashFlowType.Out, (model.OpeningPaidAmount ?? 0), false);
                    //        }
                    //        AccountFlowList = OpeningCreditAmountList(orderMasterModel, AccountFlowList, DbConstants.CashFlowType.Out, (model.OpeningPaidAmount ?? 0), false);
                    //    }
                    //    if (model.OpeningReceivedAmount != 0 && model.OpeningReceivedAmount != null)
                    //    {
                    //        if ((accountHeadList.AccountHeadType.AccountGroupKey == DbConstants.AccountGroup.Income || accountHeadList.AccountHeadType.AccountGroupKey == DbConstants.AccountGroup.Expenses))
                    //        {
                    //            AccountFlowList = OpeningPayableAmountList(orderMasterModel, AccountFlowList, DbConstants.CashFlowType.In, (model.OpeningReceivedAmount ?? 0), false);
                    //        }
                    //        else
                    //        {
                    //            AccountFlowList = OpeningDebitAmountList(orderMasterModel, AccountFlowList, DbConstants.CashFlowType.In, (model.OpeningReceivedAmount ?? 0), false);
                    //        }
                    //        AccountFlowList = OpeningCreditAmountList(orderMasterModel, AccountFlowList, DbConstants.CashFlowType.In, (model.OpeningReceivedAmount ?? 0), false);
                    //    }
                    //}
                    CreateAccountFlow(AccountFlowList, IsUpdate);
                    model.RowKey = orderMasterModel.RowKey;

                    dbContext.SaveChanges();
                    transaction.Commit();
                    model.Message = EduSuiteUIResources.Success;
                    model.IsSuccessful = true;

                }
                catch (System.Data.Entity.Validation.DbEntityValidationException dbEx)
                {
                    Exception raise = dbEx;
                    foreach (var validationErrors in dbEx.EntityValidationErrors)
                    {
                        foreach (var validationError in validationErrors.ValidationErrors)
                        {
                            string message = string.Format("{0}:{1}", validationErrors.Entry.Entity.ToString(), validationError.ErrorMessage);
                            //raise a new exception inserting the current one as the InnerException
                            raise = new InvalidOperationException(message, raise);
                        }
                    }
                    ActivityLog.CreateActivityLog(MenuConstants.FutureTransaction, ActionConstants.Add, DbConstants.LogType.Error, model.RowKey, dbEx.GetBaseException().Message);

                    throw raise;
                }
                catch (Exception ex)
                {
                    ActivityLog.CreateActivityLog(MenuConstants.FutureTransaction, ActionConstants.Add, DbConstants.LogType.Error, model.RowKey, ex.GetBaseException().Message);

                    transaction.Rollback();
                    model.Message = ex.GetBaseException().Message;
                    model.Message = String.Format(EduSuiteUIResources.FailedToSaveErrorMessage, EduSuiteUIResources.FutureTransactions);
                    model.IsSuccessful = false;
                }
            }

            return model;
        }
        public FutureTransactionViewModel UpdateFutureTransaction(FutureTransactionViewModel model)
        {
            FutureTransaction FutureTransactionModel = new FutureTransaction();
            using (var transaction = dbContext.Database.BeginTransaction())
            {
                try
                {
                    FutureTransactionModel = dbContext.FutureTransactions.SingleOrDefault(row => row.RowKey == model.RowKey);
                    // string partyName = dbContext.Parties.Where(x => x.RowKey == model.AccountHeadKey).Select(x => x.PartyName).FirstOrDefault();
                    long oldAccountHeadKey = FutureTransactionModel.AccountHeadKey;
                    //byte oldCashFlowTypeKey = FutureTransactionModel.CashFlowTypeKey;
                    //byte oldAccountGroupKey = FutureTransactionModel.AccountHead.AccountHeadType.AccountGroupKey;


                    FutureTransactionModel.AccountHeadKey = model.AccountHeadKey;
                    FutureTransactionModel.BillDate = model.BillDate;
                    FutureTransactionModel.BillNo = model.BillNo;
                    FutureTransactionModel.TotalAmount = Convert.ToDecimal(model.TotalAmount ?? 0);
                    FutureTransactionModel.AmountPaid = Convert.ToDecimal(model.AmountPaid ?? 0);
                    FutureTransactionModel.TaxableAmount = model.TaxableAmount;
                    FutureTransactionModel.NonTaxableAmount = model.NonTaxableAmount;
                    decimal oldCGST = FutureTransactionModel.CGSTAmt ?? 0;
                    decimal oldSGST = FutureTransactionModel.SGSTAmt ?? 0;
                    decimal oldIGST = FutureTransactionModel.IGSTAmt ?? 0;
                    FutureTransactionModel.RoundOff = model.RoundOff;
                    FutureTransactionModel.SubTotal = model.SubTotal;
                    //FutureTransactionModel.OrderProcessTypeKey = model.OrderProcessTypeKey;
                    if (model.StateKey == model.CompanyStateKey)
                    {
                        FutureTransactionModel.CGSTAmt = model.CGSTAmt;
                        FutureTransactionModel.SGSTAmt = model.SGSTAmt;
                        FutureTransactionModel.CGSTPer = model.CGSTPer;
                        FutureTransactionModel.SGSTPer = model.SGSTPer;
                    }
                    else
                    {
                        FutureTransactionModel.IGSTAmt = model.IGSTAmt;
                        FutureTransactionModel.IGSTPer = model.IGSTPer;
                    }
                    FutureTransactionModel.IsOpeningBalance = model.IsOpeningBalance;
                    FutureTransactionModel.OpeningPaidAmount = model.OpeningPaidAmount;
                    FutureTransactionModel.OpeningReceivedAmount = model.OpeningReceivedAmount;
                    FutureTransactionModel.Amount = model.Amount ?? 0;
                    FutureTransactionModel.IsInstallment = model.IsInstallment;
                    FutureTransactionModel.IsContra = model.IsContra;
                    FutureTransactionModel.IsTax = model.IsTax;
                    FutureTransactionModel.InstallmentTypeKey = model.InstallmentTypeKey;
                    FutureTransactionModel.InstallmentAmount = model.InstallmentAmount;
                    FutureTransactionModel.InstallmentPeriod = model.InstallmentPeriod;
                    FutureTransactionModel.DownPayment = model.DownPayment;
                    FutureTransactionModel.NoOfInstallment = model.NoOfInstallment;
                    FutureTransactionModel.InstallmentFlowKey = model.InstallmentFlowKey;
                    FutureTransactionModel.HSNCodeMasterKey = model.HSNCodeKey;
                    FutureTransactionModel.HSNCodes = model.HSNCode;


                    FutureTransactionModel.GSTINNumber = model.GSTINNumber;
                    FutureTransactionModel.CashFlowTypeKey = model.MasterCashFlowTypeKey ?? 0;
                    FutureTransactionModel.StateKey = model.StateKey;

                    UpdateFutureTransactionOtherAmountType(model.FutureTransactionOtherAmountTypes.Where(row => row.RowKey != 0 && row.RowKey != null).ToList(), model, FutureTransactionModel.RowKey);
                    CreateFutureTransactionOtherAmountType(model.FutureTransactionOtherAmountTypes.Where(row => row.RowKey == 0 || row.RowKey == null).ToList(), model, FutureTransactionModel.RowKey);

                    if (model.AmountPaid != null && model.AmountPaid != 0)
                    {
                        CreateFutureTransactionPayment(model, FutureTransactionModel.RowKey, oldAccountHeadKey);
                    }
                    short? branchKey = model.BranchKey;
                    List<AccountFlowViewModel> AccountFlowList = new List<AccountFlowViewModel>();
                    bool IsUpdate = true;

                    AccountFlowList = new List<AccountFlowViewModel>();
                    var accountHeadList = dbContext.AccountHeads.SingleOrDefault(x => x.RowKey == model.AccountHeadKey);
                    if (model.AmountPaid != model.TotalAmount)
                    {
                        AccountFlowList = CreditTotalAmountList(FutureTransactionModel, AccountFlowList, IsUpdate, branchKey, oldAccountHeadKey);
                        //AccountFlowList = CreditTotalAmountList(FutureTransactionModel, AccountFlowList, IsUpdate, branchKey, oldAccountHeadKey, oldCashFlowTypeKey, oldAccountGroupKey);
                    }
                    else if (accountHeadList.AccountHeadType.AccountGroupKey != DbConstants.AccountGroup.Income && accountHeadList.AccountHeadType.AccountGroupKey != DbConstants.AccountGroup.Expenses)
                    {
                        AccountFlowList = CreditTotalAmountList(FutureTransactionModel, AccountFlowList, IsUpdate, branchKey, oldAccountHeadKey);
                        //AccountFlowList = CreditTotalAmountList(FutureTransactionModel, AccountFlowList, IsUpdate, branchKey, oldAccountHeadKey, oldCashFlowTypeKey, oldAccountGroupKey);
                    }
                    if (model.StateKey == model.CompanyStateKey)
                    {
                        if (oldCGST != null && oldCGST != 0)
                        {
                            AccountFlowList = CreditCGSTList(FutureTransactionModel, AccountFlowList, IsUpdate, branchKey);
                        }
                        else
                        {
                            if (model.CGSTAmt != null && model.CGSTAmt != 0)
                            {
                                AccountFlowList = CreditCGSTList(FutureTransactionModel, AccountFlowList, IsUpdate, branchKey);
                            }
                        }
                        if (oldSGST != null && oldSGST != 0)
                        {
                            AccountFlowList = CreditSGSTList(FutureTransactionModel, AccountFlowList, IsUpdate, branchKey);
                        }
                        else
                        {
                            if (model.SGSTAmt != null && model.SGSTAmt != 0)
                            {
                                AccountFlowList = CreditSGSTList(FutureTransactionModel, AccountFlowList, IsUpdate, branchKey);
                            }
                        }
                    }
                    else
                    {
                        if (oldIGST != null && oldIGST != 0)
                        {
                            AccountFlowList = CreditIGSTList(FutureTransactionModel, AccountFlowList, IsUpdate, branchKey);
                        }
                        else
                        {
                            if (model.IGSTAmt != null && model.IGSTAmt != 0)
                            {
                                AccountFlowList = CreditIGSTList(FutureTransactionModel, AccountFlowList, IsUpdate, branchKey);
                            }
                        }
                    }
                    //if ((accountHeadList.AccountHeadType.AccountGroupKey == DbConstants.AccountGroup.Income || accountHeadList.AccountHeadType.AccountGroupKey == DbConstants.AccountGroup.Expenses))
                    //{
                    //    AccountFlowList = OpeningPayableAmountList(FutureTransactionModel, AccountFlowList, DbConstants.CashFlowType.Out, (model.OpeningPaidAmount ?? 0), true);
                    //}
                    //else
                    //{
                    //    AccountFlowList = OpeningDebitAmountList(FutureTransactionModel, AccountFlowList, DbConstants.CashFlowType.Out, (model.OpeningPaidAmount ?? 0), true);
                    //}
                    //AccountFlowList = OpeningCreditAmountList(FutureTransactionModel, AccountFlowList, DbConstants.CashFlowType.Out, (model.OpeningPaidAmount ?? 0), true);

                    //if ((accountHeadList.AccountHeadType.AccountGroupKey == DbConstants.AccountGroup.Income || accountHeadList.AccountHeadType.AccountGroupKey == DbConstants.AccountGroup.Expenses))
                    //{
                    //    AccountFlowList = OpeningPayableAmountList(FutureTransactionModel, AccountFlowList, DbConstants.CashFlowType.In, (model.OpeningReceivedAmount ?? 0), true);
                    //}
                    //else
                    //{
                    //    AccountFlowList = OpeningDebitAmountList(FutureTransactionModel, AccountFlowList, DbConstants.CashFlowType.In, (model.OpeningReceivedAmount ?? 0), true);
                    //}
                    //AccountFlowList = OpeningCreditAmountList(FutureTransactionModel, AccountFlowList, DbConstants.CashFlowType.In, (model.OpeningReceivedAmount ?? 0), true);

                    CreateAccountFlow(AccountFlowList, IsUpdate);
                    dbContext.SaveChanges();
                    transaction.Commit();
                    ActivityLog.CreateActivityLog(MenuConstants.FutureTransaction, ActionConstants.Edit, DbConstants.LogType.Info, model.RowKey, model.Message);

                    model.Message = EduSuiteUIResources.Success;
                    model.IsSuccessful = true;
                }
                catch (Exception ex)
                {

                    transaction.Rollback();
                    model.Message = String.Format(EduSuiteUIResources.FailedToSaveErrorMessage, EduSuiteUIResources.FutureTransactions);
                    model.IsSuccessful = false;
                    ActivityLog.CreateActivityLog(MenuConstants.FutureTransaction, ActionConstants.Edit, DbConstants.LogType.Error, model.RowKey, model.Message);
                }
            }

            return model;

        }
        public FutureTransactionViewModel DeleteFutureTransaction(FutureTransactionViewModel model)
        {
            using (var transaction = dbContext.Database.BeginTransaction())
            {
                try
                {
                    FutureTransaction FutureTransaction = dbContext.FutureTransactions.SingleOrDefault(row => row.RowKey == model.RowKey);

                    List<FutureTransactionOtherAmountType> OtherAmountList = dbContext.FutureTransactionOtherAmountTypes.Where(row => row.FutureTransactionKey == model.RowKey || row.FutureTransactionPayment.FutureTransactionKey == model.RowKey).ToList();
                    List<long> otherKeys = OtherAmountList.Select(x => x.RowKey).ToList();
                    List<FutureTransactionPayment> PaymentList = dbContext.FutureTransactionPayments.Where(row => row.FutureTransactionKey == model.RowKey).ToList();
                    foreach (FutureTransactionPayment payment in PaymentList)
                    {
                        ConfigurationViewModel ConfigModel = new ConfigurationViewModel();
                        ConfigModel.BranchKey = payment.BranchKey ?? 0;
                        ConfigModel.SerialNumber = payment.SerialNumber ?? 0;
                        ConfigModel.IsDelete = true;
                        ConfigModel.ConfigType = payment.CashFlowTypeKey == DbConstants.CashFlowType.In ? DbConstants.PaymentReceiptConfigType.ReceiptVoucher : DbConstants.PaymentReceiptConfigType.Payment; ;
                        Configurations.GenerateReceipt(dbContext, ConfigModel);
                        if (payment.PaymentModeKey == DbConstants.PaymentMode.Bank)
                        {
                            BankAccountService bankAccountService = new BankAccountService(dbContext);
                            BankAccountViewModel bankAccountModel = new BankAccountViewModel();
                            bankAccountModel.RowKey = payment.BankAccountKey ?? 0;
                            if (payment.CashFlowTypeKey == DbConstants.CashFlowType.In)
                            {
                                bankAccountModel.Amount = -(payment.PaidAmount);
                            }
                            else
                            {
                                bankAccountModel.Amount = (payment.PaidAmount);
                            }
                            // bankAccountService.UpdateCurrentAccountBalance(bankAccountModel,true,true, payment.PaidAmount);
                        }
                    }
                    List<long> PaymentKeys = PaymentList.Select(x => x.RowKey).ToList();
                    List<AccountFlow> AccountList = dbContext.AccountFlows.Where(row => (row.TransactionTypeKey == DbConstants.TransactionType.FutureTransaction && row.TransactionKey == model.RowKey) ||
                                                    (row.TransactionTypeKey == DbConstants.TransactionType.FutureTransactionPayment && PaymentKeys.Contains(row.TransactionKey)) ||
                                                    (row.TransactionTypeKey == DbConstants.TransactionType.FutureTransactionOther && otherKeys.Contains(row.TransactionKey)) ||
                                                    (row.TransactionTypeKey == DbConstants.TransactionType.FutureTransactionCGST && row.TransactionKey == model.RowKey) ||
                                                    (row.TransactionTypeKey == DbConstants.TransactionType.FutureTransactionSGST && row.TransactionKey == model.RowKey) ||
                                                    (row.TransactionTypeKey == DbConstants.TransactionType.FutureTransactionInOpeningBalance && row.TransactionKey == model.RowKey) ||
                                                    (row.TransactionTypeKey == DbConstants.TransactionType.FutureTransactionOutOpeningBalance && row.TransactionKey == model.RowKey)).ToList();
                    OtherAmountList.ForEach(row => dbContext.FutureTransactionOtherAmountTypes.Remove(row));
                    PaymentList.ForEach(row => dbContext.FutureTransactionPayments.Remove(row));
                    AccountList.ForEach(row => dbContext.AccountFlows.Remove(row));
                    dbContext.FutureTransactions.Remove(FutureTransaction);
                    //Delete Order Details and Order advance Details
                    dbContext.SaveChanges();
                    transaction.Commit();
                    model.Message = EduSuiteUIResources.Success;
                    model.IsSuccessful = true;
                    ActivityLog.CreateActivityLog(MenuConstants.FutureTransaction, ActionConstants.Delete, DbConstants.LogType.Info, model.RowKey, model.Message);
                }
                catch (DbUpdateException ex)
                {
                    transaction.Rollback();
                    if (ex.GetBaseException().Message.ToUpper().Contains(EduSuiteUIResources.ForeignKeyError.ToUpper()))
                    {
                        model.Message = ex.GetBaseException().Message;
                        model.Message = String.Format(EduSuiteUIResources.CantDeleteErrorMessage, EduSuiteUIResources.FutureTransactions);
                        model.IsSuccessful = false;
                        ActivityLog.CreateActivityLog(MenuConstants.FutureTransaction, ActionConstants.Delete, DbConstants.LogType.Error, model.RowKey, model.Message);
                    }
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    model.Message = ex.GetBaseException().Message;
                    model.Message = String.Format(EduSuiteUIResources.FailedToDeleteErrorMessage, EduSuiteUIResources.FutureTransactions);
                    model.IsSuccessful = false;
                    ActivityLog.CreateActivityLog(MenuConstants.FutureTransaction, ActionConstants.Delete, DbConstants.LogType.Error, model.RowKey, model.Message);
                }
            }

            return model;
        }
        private void FillAccountGroup(FutureTransactionViewModel model)
        {
            model.AccountGroups = dbContext.AccountGroups.Where(x => x.IsActive).Select(row => new SelectListModel
            {
                RowKey = row.RowKey,
                Text = row.AccountGroupName
            }).ToList();
        }
        public FutureTransactionViewModel FillAccountHeads(FutureTransactionViewModel model)
        {
            model.AccountHeads = dbContext.AccountHeads.Where(x => (x.HideFuture ?? false) == false && x.AccountHeadType.AccountGroupKey == model.AccountGroupKey && x.RowKey != DbConstants.AccountHead.CashAccount).Select(row => new SelectListModel
            {
                RowKey = row.RowKey,
                Text = row.AccountHeadName
            }).ToList();
            return model;
        }
        private void FillFutureTransactionDropdowns(FutureTransactionViewModel model)
        {
            FillAccountGroup(model);
            FillAccountHeads(model);
            FillBranches(model);
            FillCashFlowType(model);
            FillStates(model);
            FillHSNCodes(model);
            FillInstallmentType(model);
        }
        public FutureTransactionViewModel FillBranches(FutureTransactionViewModel model)
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
        public short GetComapanyStateKey(short branchKey)
        {
            if (branchKey != 0)
            {
                short stateKey = Convert.ToByte(dbContext.Branches.Where(row => row.RowKey == branchKey).Select(row => row.District.ProvinceKey).FirstOrDefault());
                return stateKey;
            }
            else
            {
                short stateKey = 1;
                return stateKey;
            }

        }
        private void FillCashFlowType(FutureTransactionViewModel model)
        {
            model.CashFlowTypes = dbContext.CashFlowTypes.Select(row => new SelectListModel
            {
                RowKey = row.RowKey,
                Text = row.CashFlowTypeName
            }).ToList();
        }
        private void FillStates(FutureTransactionViewModel model)
        {
            model.States = dbContext.Provinces.Where(x => x.IsActive == true).Select(row => new SelectListModel
            {
                RowKey = row.RowKey,
                Text = row.Provincename
            }).ToList();
        }
        private void FillHSNCodes(FutureTransactionViewModel model)
        {
            model.HSNCodes = dbContext.HSNCodeMasters.Select(row => new SelectListModel
            {
                RowKey = row.RowKey,
                Text = row.ProductName
            }).ToList();
        }
        private void FillInstallmentType(FutureTransactionViewModel model)
        {
            model.InstallmentTypes = dbContext.PeriodTypes.Select(row => new SelectListModel
            {
                RowKey = row.RowKey,
                Text = row.PeriodTypeName
            }).ToList();
        }
        public byte GetAccountGroup(long accountHeadKey)
        {
            byte GroupKey = dbContext.AccountHeads.Where(x => x.RowKey == accountHeadKey).Select(x => x.AccountHeadType.AccountGroupKey).FirstOrDefault();
            return GroupKey;
        }

        public FutureTransactionViewModel GetHSNCodeDetailsById(FutureTransactionViewModel model)
        {
            model = dbContext.HSNCodeMasters.Where(x => x.RowKey == model.HSNCodeKey).Select(x =>
                new FutureTransactionViewModel
                {
                    //RowKey = x.RowKey,
                    HSNCode = x.HSNSACCode,
                    IGSTPer = x.IGST,
                    CGSTPer = x.CGST,
                    SGSTPer = x.SGST,
                    ProductDescription = x.ProductDescription,
                    ProductName = x.ProductName
                }).FirstOrDefault();
            return model;
        }
        #endregion

        #region Payment

        public FutureTransactionPaymentViewModel GetFutureTransactionPaymentById(long Id)
        {
            FutureTransactionPaymentViewModel model = new FutureTransactionPaymentViewModel();
            FutureTransaction FutureTransaction = new FutureTransaction();
            FutureTransaction = dbContext.FutureTransactions.SingleOrDefault(row => row.RowKey == Id);
            model.PaymentModeKey = DbConstants.PaymentMode.Cash;
            model.MasterKey = Id;

            model.CashFlowTypeKey = FutureTransaction.CashFlowTypeKey;
            model.IsContra = FutureTransaction.IsContra;
            model = FutureTransactionCalculation(model);
            model.BranchKey = FutureTransaction.BranchKey;
            model.AccountHeadKey = FutureTransaction.AccountHeadKey;
            model.ShowAdminBankBalance = dbContext.GeneralConfigurations.Select(x => x.ShowAdminBankBalance ?? false).FirstOrDefault();

            FillTransactionPaymentDropdownLists(model);
            FillFutureTransactionOtherAmountType(model);

            return model;
        }
        public FutureTransactionPaymentViewModel FutureTransactionCalculation(FutureTransactionPaymentViewModel model)
        {
            FutureTransaction FutureTransaction = new FutureTransaction();
            FutureTransaction = dbContext.FutureTransactions.SingleOrDefault(row => row.RowKey == model.MasterKey);
            model.TotalReceivedAmount = (dbContext.FutureTransactionPayments.Where(x => x.FutureTransactionKey == model.MasterKey && (x.ChequeStatusKey != DbConstants.ProcessStatus.Rejected) && x.CashFlowTypeKey == model.CashFlowTypeKey).Select(row => row.PaidAmount ?? 0).DefaultIfEmpty().Sum()) + (model.CashFlowTypeKey == DbConstants.CashFlowType.In ? (FutureTransaction.OpeningReceivedAmount ?? 0) : (FutureTransaction.OpeningPaidAmount ?? 0));
            model.EarningAmount = dbContext.FutureTransactionOtherAmountTypes.Where(x => x.FutureTransactionPayment.FutureTransactionKey == model.MasterKey && x.IsAddition == true && x.FutureTransactionPayment.CashFlowTypeKey == model.CashFlowTypeKey).Select(x => x.Amount).DefaultIfEmpty().Sum();
            model.DeductAmount = dbContext.FutureTransactionOtherAmountTypes.Where(x => x.FutureTransactionPayment.FutureTransactionKey == model.MasterKey && x.IsAddition == false && x.FutureTransactionPayment.CashFlowTypeKey == model.CashFlowTypeKey).Select(x => x.Amount).DefaultIfEmpty().Sum();
            if (FutureTransaction.IsContra == false)
            {
                model.AmountToPay = FutureTransaction.TotalAmount;
            }
            else
            {
                if (model.CashFlowTypeKey != FutureTransaction.CashFlowTypeKey)
                {
                    model.AmountToPay = FutureTransaction.Amount;
                }
                else
                {
                    model.AmountToPay = FutureTransaction.TotalAmount;
                }
            }
            model.BalanceAmount = model.AmountToPay - model.TotalReceivedAmount;
            return model;
        }
        public FutureTransactionPaymentViewModel CallFutureTransactionPayment(FutureTransactionPaymentViewModel model)
        {
            using (var transaction = dbContext.Database.BeginTransaction())
            {
                try
                {
                    CreateFutureTransactionPayment(model);
                    transaction.Commit();
                    model.Message = EduSuiteUIResources.Success;
                    model.IsSuccessful = true;
                }
                catch (Exception)
                {
                    transaction.Rollback();
                    model.Message = String.Format(EduSuiteUIResources.FailedToSaveErrorMessage, EduSuiteUIResources.FutureTransactions + EduSuiteUIResources.BlankSpace + EduSuiteUIResources.Payment);
                    model.IsSuccessful = false;
                }

            }
            //if (!model.IsSecond && model.IsSuccessful && dbContext.FutureTransactions.Where(row => row.RowKey == model.MasterKey).Select(row => row.OrderProcessTypeKey).FirstOrDefault() != DbConstants.OrderProcessType.NoTax)
            //{
            //    model.UpdateType = DbConstants.UpdationType.Create;
            //    TriggerDatabaseInBackgroundPayment(model);
            //}
            return model;
        }
        private FutureTransactionPaymentViewModel CreateFutureTransactionPayment(FutureTransactionPaymentViewModel model)
        {
            FutureTransactionPayment FutureTransactionPayment = new FutureTransactionPayment();
            FillTransactionPaymentDropdownLists(model);

            Int64 maxKey = dbContext.FutureTransactionPayments.Select(p => p.RowKey).DefaultIfEmpty().Max();
            ConfigurationViewModel ConfigModel = new ConfigurationViewModel();
            ConfigModel.BranchKey = model.BranchKey ?? 0;
            ConfigModel.ConfigType = model.CashFlowTypeKey == DbConstants.CashFlowType.In ? DbConstants.PaymentReceiptConfigType.ReceiptVoucher : DbConstants.PaymentReceiptConfigType.Payment;
            Configurations.GenerateReceipt(dbContext, ConfigModel);
            decimal balance = (model.AmountToPay) - (model.TotalReceivedAmount ?? 0);
            FutureTransactionPayment.RowKey = Convert.ToInt64(maxKey + 1);
            if (balance < ((model.PaidAmount ?? 0) + (model.OldAdvanceBalance ?? 0)))
            {
                FutureTransactionPayment.PaidAmount = balance;
            }
            else
            {
                FutureTransactionPayment.PaidAmount = Convert.ToDecimal(model.PaidAmount);
            }
            FutureTransactionPayment.Amount = model.AmountToPay;
            FutureTransactionPayment.PaymentDate = Convert.ToDateTime(model.PaymentDate);
            FutureTransactionPayment.PaymentModeKey = model.PaymentModeKey;
            FutureTransactionPayment.PaymentModeSubKey = model.PaymentModeSubKey;
            FutureTransactionPayment.BankAccountKey = model.BankAccountKey;
            FutureTransactionPayment.CardNumber = model.CardNumber;
            FutureTransactionPayment.ChequeOrDDNumber = model.ChequeOrDDNumber;
            FutureTransactionPayment.ChequeOrDDDate = model.ChequeOrDDDate;
            FutureTransactionPayment.Purpose = model.Purpose;
            FutureTransactionPayment.PaidBy = model.PaidBy;
            FutureTransactionPayment.AuthorizedBy = model.AuthorizedBy;
            FutureTransactionPayment.ReceivedBy = model.ReceivedBy;
            FutureTransactionPayment.OnBehalfOf = model.OnBehalfOf;
            FutureTransactionPayment.Remarks = model.Remarks;
            FutureTransactionPayment.FutureTransactionKey = model.MasterKey;
            FutureTransactionPayment.IsAdavance = model.IsAdavance != null ? model.IsAdavance : false;
            FutureTransactionPayment.BranchKey = model.BranchKey;
            FutureTransactionPayment.ReferenceNumber = model.ReferenceNumber;
            FutureTransactionPayment.CashFlowTypeKey = model.CashFlowTypeKey;
            FutureTransactionPayment.SerialNumber = ConfigModel.SerialNumber;
            FutureTransactionPayment.ReceiptNumber = ConfigModel.ReceiptNumber;
            model.BranchKey = FutureTransactionPayment.BranchKey;
            FutureTransactionPayment.BillBalanceAmount = model.BillBalanceAmount ?? model.AmountToPay;
            FutureTransactionPayment.BalanceAmount = model.BalanceAmount;
            if (FutureTransactionPayment.PaymentModeKey == DbConstants.PaymentMode.Cheque)
            {
                FutureTransactionPayment.ChequeStatusKey = DbConstants.ProcessStatus.Pending;
            }
            model.PaymentKey = FutureTransactionPayment.RowKey;
            long oldBankKey = 0;
            dbContext.FutureTransactionPayments.Add(FutureTransactionPayment);
            CreateFutureTransactionOtherAmountType(model.FutureTransactionOtherAmountTypes.Where(row => row.RowKey == 0 || row.RowKey == null).ToList(), model, FutureTransactionPayment.RowKey);
            if (model.BankAccountKey != null && model.BankAccountKey != 0)
            {
                var BankAccountList = dbContext.BankAccounts.SingleOrDefault(x => x.RowKey == model.BankAccountKey);
                model.BankAccountName = (BankAccountList.NameInAccount ?? BankAccountList.AccountNumber) + EduSuiteUIResources.Hyphen + BankAccountList.Bank.BankName;
            }
            if (model.PaymentModeKey == DbConstants.PaymentMode.Bank)
            {
                BankAccountService bankAccountService = new BankAccountService(dbContext);
                BankAccountViewModel bankAccountModel = new BankAccountViewModel();
                bankAccountModel.RowKey = model.BankAccountKey ?? 0;
                if (model.CashFlowTypeKey == DbConstants.CashFlowType.In)
                {
                    bankAccountModel.Amount = (model.PaidAmount);
                }
                else
                {
                    bankAccountModel.Amount = -(model.PaidAmount);
                }
                //bankAccountService.UpdateCurrentAccountBalance(bankAccountModel, false, (model.CashFlowTypeKey == DbConstants.CashFlowType.Out) ? true : false, null);
            }
            List<AccountFlowViewModel> accountFlowModelList = new List<AccountFlowViewModel>();

            if (model.PaidAmount != 0 && model.PaidAmount != null)
            {
                var accountHeadList = dbContext.AccountHeads.SingleOrDefault(x => x.RowKey == model.AccountHeadKey);
                purposeGeneration(model, accountHeadList.AccountHeadName);
                model.IsUpdate = false;
                if (model.PaidAmount != model.AmountToPay && (accountHeadList.AccountHeadType.AccountGroupKey == DbConstants.AccountGroup.Income || accountHeadList.AccountHeadType.AccountGroupKey == DbConstants.AccountGroup.Expenses))
                {
                    accountFlowModelList = PayableAmountList(model, accountFlowModelList, FutureTransactionPayment.PaidAmount ?? 0);
                }
                else if (accountHeadList.AccountHeadType.AccountGroupKey == DbConstants.AccountGroup.Income || accountHeadList.AccountHeadType.AccountGroupKey == DbConstants.AccountGroup.Expenses)
                {
                    accountFlowModelList = DebitExpenseAmountList(model, accountFlowModelList);
                }
                else
                {
                    accountFlowModelList = DebitAmountList(model, accountFlowModelList);
                }
                model.IsUpdate = false;
                accountFlowModelList = CreditAmountList(model, accountFlowModelList, oldBankKey);
                model.IsUpdate = false;
                CreateAccountFlow(accountFlowModelList, model.IsUpdate);
            }
            dbContext.SaveChanges();
            return model;
        }
        private FutureTransactionPaymentViewModel UpdateFutureTransactionPayment(FutureTransactionPaymentViewModel model, long FutureTransactionPaymentRowKey)
        {
            FutureTransactionPayment FutureTransactionPayment = new FutureTransactionPayment();
            FillTransactionPaymentDropdownLists(model);

            FutureTransactionPayment = dbContext.FutureTransactionPayments.SingleOrDefault(row => row.RowKey == FutureTransactionPaymentRowKey);
            decimal oldTotalAmount = FutureTransactionPayment.Amount;
            decimal oldPaidAmount = FutureTransactionPayment.PaidAmount ?? 0;
            decimal oldAmount = FutureTransactionPayment.PaidAmount ?? 0;
            if ((model.AmountToPay) < (model.PaidAmount + (model.OldAdvanceBalance ?? 0)))
            {
                FutureTransactionPayment.PaidAmount = Convert.ToDecimal(model.AmountToPay - (model.OldAdvanceBalance ?? 0));
            }
            else
            {
                FutureTransactionPayment.PaidAmount = Convert.ToDecimal(model.PaidAmount ?? 0);
            }
            FutureTransactionPayment.Amount = model.AmountToPay;
            FutureTransactionPayment.PaymentDate = Convert.ToDateTime(model.PaymentDate);
            model.OldPaymentModeKey = FutureTransactionPayment.PaymentModeKey;
            FutureTransactionPayment.PaymentModeKey = model.PaymentModeKey;
            FutureTransactionPayment.PaymentModeSubKey = model.PaymentModeSubKey;
            long oldBankKey = FutureTransactionPayment.BankAccountKey ?? 0;
            FutureTransactionPayment.BankAccountKey = model.BankAccountKey;
            FutureTransactionPayment.CardNumber = model.CardNumber;
            FutureTransactionPayment.ChequeOrDDNumber = model.ChequeOrDDNumber;
            FutureTransactionPayment.ChequeOrDDDate = model.ChequeOrDDDate;
            FutureTransactionPayment.Purpose = model.Purpose;
            FutureTransactionPayment.PaidBy = model.PaidBy;
            FutureTransactionPayment.AuthorizedBy = model.AuthorizedBy;
            FutureTransactionPayment.ReceivedBy = model.ReceivedBy;
            FutureTransactionPayment.ReferenceNumber = model.ReferenceNumber;
            FutureTransactionPayment.OnBehalfOf = model.OnBehalfOf;
            FutureTransactionPayment.Remarks = model.Remarks;
            byte olCashFlow = FutureTransactionPayment.CashFlowTypeKey ?? 0;
            FutureTransactionPayment.CashFlowTypeKey = model.CashFlowTypeKey;
            FutureTransactionPayment.FutureTransactionKey = model.MasterKey;
            FutureTransactionPayment.IsAdavance = model.IsAdavance != null ? model.IsAdavance : false;
            FutureTransactionPayment.BranchKey = model.BranchKey;
            FutureTransactionPayment.BillBalanceAmount = model.BillBalanceAmount;
            FutureTransactionPayment.BalanceAmount = model.BalanceAmount;
            if (FutureTransactionPayment.PaymentModeKey == DbConstants.PaymentMode.Cheque)
            {
                FutureTransactionPayment.ChequeStatusKey = DbConstants.ProcessStatus.Pending;
            }
            model.PaymentKey = FutureTransactionPayment.RowKey;
            dbContext.SaveChanges();
            if (model.BankAccountKey != null && model.BankAccountKey != 0)
            {
                var BankAccountList = dbContext.BankAccounts.SingleOrDefault(x => x.RowKey == model.BankAccountKey);
                model.BankAccountName = (BankAccountList.NameInAccount ?? BankAccountList.AccountNumber) + EduSuiteUIResources.Hyphen + BankAccountList.Bank.BankName;
            }



            //if (DbConstants.PaymentMode.BankPaymentModes.Contains(model.OldPaymentModeKey) && oldBankKey != (model.BankAccountKey ?? 0))
            //{
            //    BankAccountService bankAccountService = new BankAccountService(dbContext);
            //    BankAccountViewModel bankAccountModel = new BankAccountViewModel();
            //    bankAccountModel.RowKey = oldBankKey;
            //    bankAccountModel.Amount = -(oldAmount);
            //    bankAccountService.UpdateCurrentAccountBalance(bankAccountModel, true, true, oldAmount);
            //}
            //else if (DbConstants.PaymentMode.BankPaymentModes.Contains(model.OldPaymentModeKey) && DbConstants.PaymentMode.BankPaymentModes.Contains(model.PaymentModeKey) && oldBankKey == (model.BankAccountKey ?? 0))
            //{

            //    BankAccountService bankAccountService = new BankAccountService(dbContext);
            //    BankAccountViewModel bankAccountModel = new BankAccountViewModel();
            //    bankAccountModel.RowKey = model.BankAccountKey ?? 0;
            //    bankAccountModel.Amount = model.PaidAmount;
            //    bankAccountService.UpdateCurrentAccountBalance(bankAccountModel, true, false, oldAmount);

            //}
            //else if (!DbConstants.PaymentMode.BankPaymentModes.Contains(model.OldPaymentModeKey) && DbConstants.PaymentMode.BankPaymentModes.Contains(model.PaymentModeKey))
            //{
            //    BankAccountService bankAccountService = new BankAccountService(dbContext);
            //    BankAccountViewModel bankAccountModel = new BankAccountViewModel();
            //    bankAccountModel.RowKey = model.BankAccountKey ?? 0;
            //    bankAccountModel.Amount = model.PaidAmount;
            //    bankAccountService.UpdateCurrentAccountBalance(bankAccountModel, false, false, null);
            //}





            //if (model.PaymentModeKey == DbConstants.PaymentMode.Bank)
            //{
            //    BankAccountService bankAccountService = new BankAccountService(dbContext);
            //    BankAccountViewModel bankAccountModel = new BankAccountViewModel();
            //    bankAccountModel.RowKey = model.BankAccountKey ?? 0;
            //    if (model.CashFlowTypeKey == DbConstants.CashFlowType.In)
            //    {
            //        bankAccountModel.Amount = (model.PaidAmount);
            //    }
            //    else
            //    {
            //        bankAccountModel.Amount = -(model.PaidAmount);
            //    }
            //    bankAccountService.UpdateCurrentAccountBalance(bankAccountModel);
            //    if (model.OldPaymentModeKey == DbConstants.PaymentMode.Bank)
            //    {
            //        bankAccountModel.RowKey = oldBankKey;
            //        if (olCashFlow == DbConstants.CashFlowType.In)
            //        {
            //            bankAccountModel.Amount = -(oldAmount);
            //        }
            //        else
            //        {
            //            bankAccountModel.Amount = oldAmount;
            //        }
            //        bankAccountService.UpdateCurrentAccountBalance(bankAccountModel);
            //    }
            //}
            //else if (model.OldPaymentModeKey == DbConstants.PaymentMode.Bank)
            //{
            //    BankAccountService bankAccountService = new BankAccountService(dbContext);
            //    BankAccountViewModel bankAccountModel = new BankAccountViewModel();
            //    bankAccountModel.RowKey = oldBankKey;
            //    if (olCashFlow == DbConstants.CashFlowType.In)
            //    {
            //        bankAccountModel.Amount = -(oldAmount);
            //    }
            //    else
            //    {
            //        bankAccountModel.Amount = oldAmount;
            //    }
            //    bankAccountService.UpdateCurrentAccountBalance(bankAccountModel);
            //}
            List<AccountFlowViewModel> accountFlowModelList = new List<AccountFlowViewModel>();
            var accountHeadList = dbContext.AccountHeads.SingleOrDefault(x => x.RowKey == model.AccountHeadKey);
            purposeGeneration(model, accountHeadList.AccountHeadName);
            bool IsUpdate = true;
            model.IsUpdate = IsUpdate;
            if (model.PaidAmount != model.AmountToPay && (accountHeadList.AccountHeadType.AccountGroupKey == DbConstants.AccountGroup.Income || accountHeadList.AccountHeadType.AccountGroupKey == DbConstants.AccountGroup.Expenses))
            {
                accountFlowModelList = PayableAmountList(model, accountFlowModelList, FutureTransactionPayment.PaidAmount ?? 0);
            }
            else if (accountHeadList.AccountHeadType.AccountGroupKey == DbConstants.AccountGroup.Income || accountHeadList.AccountHeadType.AccountGroupKey == DbConstants.AccountGroup.Expenses)
            {
                accountFlowModelList = DebitExpenseAmountList(model, accountFlowModelList);
            }
            else
            {
                accountFlowModelList = DebitAmountList(model, accountFlowModelList);
            }
            model.IsUpdate = IsUpdate;
            accountFlowModelList = CreditAmountList(model, accountFlowModelList, oldBankKey);
            model.IsUpdate = IsUpdate;
            CreateAccountFlow(accountFlowModelList, model.IsUpdate);
            return model;
        }
        public FutureTransactionViewModel DeleteFutureTransactionPayment(long RowKey)
        {
            FutureTransactionViewModel FutureTransactionViewModel = new FutureTransactionViewModel();
            FutureTransactionPaymentViewModel objViewModel = new FutureTransactionPaymentViewModel();
            using (var transaction = dbContext.Database.BeginTransaction())
            {
                try
                {
                    FutureTransactionPayment FutureTransactionPayment = dbContext.FutureTransactionPayments.SingleOrDefault(row => row.RowKey == RowKey);
                    ConfigurationViewModel ConfigModel = new ConfigurationViewModel();
                    ConfigModel.BranchKey = FutureTransactionPayment.BranchKey ?? 0;
                    ConfigModel.SerialNumber = FutureTransactionPayment.SerialNumber ?? 0;
                    ConfigModel.IsDelete = true;
                    ConfigModel.ConfigType = DbConstants.PaymentReceiptConfigType.Payment;
                    Configurations.GenerateReceipt(dbContext, ConfigModel);
                    if (FutureTransactionPayment.PaymentModeKey == DbConstants.PaymentMode.Bank)
                    {
                        BankAccountService bankAccountService = new BankAccountService(dbContext);
                        BankAccountViewModel bankAccountModel = new BankAccountViewModel();
                        bankAccountModel.RowKey = FutureTransactionPayment.BankAccountKey ?? 0;
                        if (FutureTransactionPayment.CashFlowTypeKey == DbConstants.CashFlowType.In)
                        {
                            bankAccountModel.Amount = -(FutureTransactionPayment.PaidAmount);
                        }
                        else
                        {
                            bankAccountModel.Amount = (FutureTransactionPayment.PaidAmount);
                        }
                        //bankAccountService.UpdateCurrentAccountBalance(bankAccountModel,true,true, FutureTransactionPayment.PaidAmount);
                    }
                    List<FutureTransactionOtherAmountType> OtherAmountList = dbContext.FutureTransactionOtherAmountTypes.Where(row => row.FutureTransactionPaymentKey == RowKey).ToList();
                    List<long> otherKeys = OtherAmountList.Select(x => x.RowKey).ToList();
                    //objViewModel.RefKey = FutureTransactionPayment.RefKey;
                    List<AccountFlow> AccountList = dbContext.AccountFlows.Where(row => row.TransactionKey == RowKey && row.TransactionTypeKey == DbConstants.TransactionType.FutureTransactionPayment ||
                                                    (row.TransactionTypeKey == DbConstants.TransactionType.FutureTransactionOther && otherKeys.Contains(row.TransactionKey))).ToList();
                    OtherAmountList.ForEach(row => dbContext.FutureTransactionOtherAmountTypes.Remove(row));
                    AccountList.ForEach(row => dbContext.AccountFlows.Remove(row));
                    dbContext.FutureTransactionPayments.Remove(FutureTransactionPayment);

                    dbContext.SaveChanges();
                    transaction.Commit();
                    FutureTransactionViewModel.Message = EduSuiteUIResources.Success;
                    FutureTransactionViewModel.IsSuccessful = true;
                }
                catch (DbUpdateException ex)
                {
                    transaction.Rollback();
                    if (ex.GetBaseException().Message.ToUpper().Contains(EduSuiteUIResources.ForeignKeyError.ToUpper()))
                    {
                        FutureTransactionViewModel.Message = String.Format(EduSuiteUIResources.CantDeleteErrorMessage, EduSuiteUIResources.FutureTransactions);
                        FutureTransactionViewModel.IsSuccessful = false;
                    }
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    FutureTransactionViewModel.Message = String.Format(EduSuiteUIResources.CantDeleteErrorMessage, EduSuiteUIResources.FutureTransactions);
                    FutureTransactionViewModel.IsSuccessful = false;
                }
            }
            //if (!objViewModel.IsSecond && FutureTransactionViewModel.IsSuccessful)
            //{
            //    objViewModel.UpdateType = DbConstants.UpdationType.Delete;
            //    TriggerDatabaseInBackgroundPayment(objViewModel);
            //}
            return FutureTransactionViewModel;
        }
        private void FillTransactionPaymentDropdownLists(FutureTransactionViewModel model)
        {
            FillPaymentModes(model);
            FillPaymentModeSub(model);
            FillBankAccounts(model);
        }
        private void CreateFutureTransactionPayment(FutureTransactionViewModel model, long FutureTransactionKey, long oldAccountHeadKey)
        {
            FutureTransactionPaymentViewModel FutureTransactionPaymentViewModel = new FutureTransactionPaymentViewModel();
            FillTransactionPaymentDropdownLists(model);

            FutureTransactionPaymentViewModel.PaidAmount = Convert.ToDecimal(model.AmountPaid ?? 0);
            FutureTransactionPaymentViewModel.AmountToPay = Convert.ToDecimal(model.TotalAmount ?? 0);
            FutureTransactionPaymentViewModel.PaymentDate = Convert.ToDateTime(DateTimeUTC.Now);
            FutureTransactionPaymentViewModel.TaxableAmount = Convert.ToDecimal(model.Amount ?? 0);
            FutureTransactionPaymentViewModel.PaymentModeKey = (model.PaymentModeKey != 0) ? model.PaymentModeKey : DbConstants.PaymentMode.Cash;
            FutureTransactionPaymentViewModel.PaymentModeSubKey = model.PaymentModeSubKey;
            FutureTransactionPaymentViewModel.BankAccountKey = model.BankAccountKey;
            FutureTransactionPaymentViewModel.CardNumber = model.CardNumber;
            FutureTransactionPaymentViewModel.ReferenceNumber = model.ReferenceNumber;
            FutureTransactionPaymentViewModel.ChequeOrDDNumber = model.ChequeOrDDNumber;
            FutureTransactionPaymentViewModel.MasterKey = FutureTransactionKey;
            FutureTransactionPaymentViewModel.ChequeOrDDDate = model.ChequeOrDDDate;
            FutureTransactionPaymentViewModel.IsAdavance = true;
            FutureTransactionPaymentViewModel.Purpose = model.AccountHeadName;
            FutureTransactionPaymentViewModel.OldAccountHeadKey = oldAccountHeadKey;
            FutureTransactionPaymentViewModel.AccountHeadKey = model.AccountHeadKey;
            FutureTransactionPaymentViewModel.BranchKey = model.BranchKey;
            FutureTransactionPaymentViewModel.BalanceAmount = model.BalanceAmount;
            FutureTransactionPaymentViewModel.CashFlowTypeKey = model.CashFlowTypeKey;
            if (model.FutureTransactionPaymentRowKey == null || model.FutureTransactionPaymentRowKey == 0)
            {
                if (model.AmountPaid != null && model.AmountPaid != 0)
                {
                    CreateFutureTransactionPayment(FutureTransactionPaymentViewModel);
                }
            }
            else
            {
                if (model.AmountPaid != null && model.AmountPaid != 0)
                {
                    UpdateFutureTransactionPayment(FutureTransactionPaymentViewModel, model.FutureTransactionPaymentRowKey);
                }
            }
        }
        private void FillPaymentModes(FutureTransactionViewModel model)
        {
            model.PaymentModes = dbContext.VwPaymentModeSelectActiveOnlies.OrderBy(row => row.DisplayOrder).Select(row => new SelectListModel
            {
                RowKey = row.RowKey,
                Text = row.PaymentModeName
            }).ToList();
        }
        private void FillPaymentModeSub(FutureTransactionViewModel model)
        {
            model.PaymentModeSub = dbContext.PaymentModeSubs.Where(x => x.IsActive && x.PaymentModeKey == DbConstants.PaymentMode.Bank).Select(row => new SelectListModel
            {
                RowKey = row.RowKey,
                Text = row.PaymentModeSubName
            }).ToList();
        }
        public FutureTransactionViewModel FillBankAccounts(FutureTransactionViewModel model)
        {
            //model.BankAccounts = dbContext.BankAccounts.Where(x => x.IsActive == true && ((x.BranchKey ?? model.BranchKey) == model.BranchKey)).Select(row => new SelectListModel
            //{
            //    RowKey = row.RowKey,
            //    Text = (row.NameInAccount ?? row.AccountNumber) + EduSuiteUIResources.Hyphen + row.Bank.BankName
            //}).ToList();

            model.BankAccounts = dbContext.BranchAccounts.Where(x => x.BranchKey == model.BranchKey && x.BankAccount.IsActive).Select(row => new SelectListModel
            {
                RowKey = row.BankAccount.RowKey,
                Text = (row.BankAccount.NameInAccount ?? row.BankAccount.AccountNumber) + EduSuiteUIResources.Hyphen + row.BankAccount.Bank.BankName
            }).ToList();
            return model;
        }

        private void FillTransactionPaymentDropdownLists(FutureTransactionPaymentViewModel model)
        {
            FillPaymentModes(model);
            FillPaymentModeSub(model);
            FillBankAccounts(model);
            FillCashFlowType(model);
        }
        private void FillPaymentModes(FutureTransactionPaymentViewModel model)
        {
            model.PaymentModes = dbContext.VwPaymentModeSelectActiveOnlies.OrderBy(row => row.DisplayOrder).Select(row => new SelectListModel
            {
                RowKey = row.RowKey,
                Text = row.PaymentModeName
            }).ToList();
        }
        public void FillPaymentModeSub(FutureTransactionPaymentViewModel model)
        {
            model.PaymentModeSub = dbContext.PaymentModeSubs.Where(x => x.IsActive && x.PaymentModeKey == DbConstants.PaymentMode.Bank).Select(row => new SelectListModel
            {
                RowKey = row.RowKey,
                Text = row.PaymentModeSubName
            }).ToList();

        }
        private void FillBankAccounts(FutureTransactionPaymentViewModel model)
        {
            //model.BankAccounts = dbContext.BankAccounts.Where(x => x.IsActive == true && ((x.BranchKey ?? model.BranchKey) == model.BranchKey)).Select(row => new SelectListModel
            //{
            //    RowKey = row.RowKey,
            //    Text = (row.NameInAccount ?? row.AccountNumber) + EduSuiteUIResources.Hyphen + row.Bank.BankName
            //}).ToList();
            model.BankAccounts = dbContext.BranchAccounts.Where(x => x.BranchKey == model.BranchKey && x.BankAccount.IsActive).Select(row => new SelectListModel
            {
                RowKey = row.BankAccount.RowKey,
                Text = (row.BankAccount.NameInAccount ?? row.BankAccount.AccountNumber) + EduSuiteUIResources.Hyphen + row.BankAccount.Bank.BankName
            }).ToList();
        }
        private void FillCashFlowType(FutureTransactionPaymentViewModel model)
        {
            model.CashFlowTypes = dbContext.CashFlowTypes.Select(row => new SelectListModel
            {
                RowKey = row.RowKey,
                Text = row.CashFlowTypeName
            }).ToList();
        }
        public decimal CheckShortBalance(short PaymentModeKey, long Rowkey, long BankAccountKey, short branchKey, byte CashFlowTypeKey)
        {
            decimal Balance = 0;
            if (PaymentModeKey == DbConstants.PaymentMode.Cash)
            {
                long accountheadKey = dbContext.AccountHeads.Where(x => x.RowKey == DbConstants.AccountHead.CashAccount).Select(x => x.RowKey).FirstOrDefault();
                decimal totalDebit = dbContext.AccountFlows.Where(x => x.AccountHeadKey == accountheadKey && x.CashFlowTypeKey == DbConstants.CashFlowType.In && x.BranchKey == branchKey).Select(x => x.Amount).DefaultIfEmpty().Sum();
                decimal totalCredit = dbContext.AccountFlows.Where(x => x.AccountHeadKey == accountheadKey && x.CashFlowTypeKey == DbConstants.CashFlowType.Out && x.BranchKey == branchKey).Select(x => x.Amount).DefaultIfEmpty().Sum();
                Balance = totalDebit - totalCredit;
                if (Rowkey != 0)
                {
                    var purchaseList = dbContext.CashFlows.SingleOrDefault(x => x.RowKey == Rowkey);
                    if (PaymentModeKey == purchaseList.PaymentModeKey)
                    {
                        Balance = Balance + (CashFlowTypeKey == DbConstants.CashFlowType.In ? -purchaseList.Amount : purchaseList.Amount);
                    }
                }
            }
            else if (PaymentModeKey == DbConstants.PaymentMode.Bank || PaymentModeKey == DbConstants.PaymentMode.Cheque)
            {
                if (BankAccountKey != 0 && BankAccountKey != null)
                {
                    long accountheadKey = dbContext.BankAccounts.Where(x => x.RowKey == BankAccountKey).Select(x => x.AccountHeadKey).FirstOrDefault();
                    decimal totalDebit = dbContext.AccountFlows.Where(x => x.AccountHeadKey == accountheadKey && x.CashFlowTypeKey == DbConstants.CashFlowType.In && x.BranchKey == branchKey).Select(x => x.Amount).DefaultIfEmpty().Sum();
                    decimal totalCredit = dbContext.AccountFlows.Where(x => x.AccountHeadKey == accountheadKey && x.CashFlowTypeKey == DbConstants.CashFlowType.Out && x.BranchKey == branchKey).Select(x => x.Amount).DefaultIfEmpty().Sum();
                    Balance = totalDebit - totalCredit;

                    if (Rowkey != 0)
                    {
                        var purchaseList = dbContext.CashFlows.SingleOrDefault(x => x.RowKey == Rowkey);
                        if (BankAccountKey == purchaseList.BankAccountKey)
                        {
                            //Balance = Balance + purchaseList.Amount;
                            Balance = Balance + (CashFlowTypeKey == DbConstants.CashFlowType.In ? -purchaseList.Amount : purchaseList.Amount);
                        }
                    }
                }
            }
            return Balance;
        }


        #endregion

        #region Account
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
        private List<AccountFlowViewModel> OpeningPayableAmountList(FutureTransaction model, List<AccountFlowViewModel> accountFlowModelList, byte cashFlowTypeKey, decimal amount, bool IsUpadte)
        {
            long accountheadKey;
            if (cashFlowTypeKey == DbConstants.CashFlowType.Out)
            {
                accountheadKey = dbContext.VwAccountHeadSelectActiveOnlies.Where(x => x.RowKey == DbConstants.AccountHead.AccountsPayable).Select(x => x.RowKey).FirstOrDefault();
            }
            else
            {
                accountheadKey = dbContext.VwAccountHeadSelectActiveOnlies.Where(x => x.RowKey == DbConstants.AccountHead.AccountsReceivable).Select(x => x.RowKey).FirstOrDefault();
            }
            accountFlowModelList.Add(new AccountFlowViewModel
            {
                CashFlowTypeKey = cashFlowTypeKey == DbConstants.CashFlowType.In ? DbConstants.CashFlowType.Out : DbConstants.CashFlowType.In,
                AccountHeadKey = accountheadKey,
                Amount = amount,
                TransactionTypeKey = cashFlowTypeKey == DbConstants.CashFlowType.In ? DbConstants.TransactionType.FutureTransactionInOpeningBalance : DbConstants.TransactionType.FutureTransactionOutOpeningBalance,
                VoucherTypeKey = DbConstants.VoucherType.FutureTransaction,
                TransactionKey = model.RowKey,
                TransactionDate = model.BillDate,
                ExtraUpdateKey = 0,
                IsUpdate = IsUpadte,
                BranchKey = model.BranchKey,
                Purpose = model.AccountHead.AccountHeadName + EduSuiteUIResources.BlankSpace + EduSuiteUIResources.OpeningBalance,
            });
            return accountFlowModelList;
        }
        private List<AccountFlowViewModel> OpeningDebitAmountList(FutureTransaction model, List<AccountFlowViewModel> accountFlowModelList, byte cashFlowTypeKey, decimal amount, bool IsUpdate)
        {
            long accountheadKey;
            long ExtraUpdateKey = 0;
            accountheadKey = dbContext.AccountHeads.Where(x => x.RowKey == model.AccountHeadKey).Select(x => x.RowKey).FirstOrDefault();
            accountFlowModelList.Add(new AccountFlowViewModel
            {
                CashFlowTypeKey = cashFlowTypeKey == DbConstants.CashFlowType.In ? DbConstants.CashFlowType.Out : DbConstants.CashFlowType.In,
                AccountHeadKey = accountheadKey,
                Amount = amount,
                TransactionTypeKey = cashFlowTypeKey == DbConstants.CashFlowType.In ? DbConstants.TransactionType.FutureTransactionInOpeningBalance : DbConstants.TransactionType.FutureTransactionOutOpeningBalance,
                VoucherTypeKey = DbConstants.VoucherType.FutureTransaction,
                TransactionKey = model.RowKey,
                TransactionDate = model.BillDate,
                ExtraUpdateKey = ExtraUpdateKey,
                IsUpdate = IsUpdate,
                BranchKey = model.BranchKey,
                Purpose = model.AccountHead.AccountHeadName + EduSuiteUIResources.BlankSpace + EduSuiteUIResources.OpeningBalance,
            });
            return accountFlowModelList;
        }
        private List<AccountFlowViewModel> OpeningCreditAmountList(FutureTransaction model, List<AccountFlowViewModel> accountFlowModelList, byte cashFlowTypeKey, decimal amount, bool IsUpdate)
        {
            long accountheadKey;
            long ExtraUpdateKey = 0;
            accountheadKey = dbContext.AccountHeads.Where(x => x.RowKey == DbConstants.AccountHead.OpeningBalance).Select(x => x.RowKey).FirstOrDefault();
            accountFlowModelList.Add(new AccountFlowViewModel
            {
                CashFlowTypeKey = cashFlowTypeKey,
                AccountHeadKey = accountheadKey,
                Amount = amount,
                TransactionTypeKey = cashFlowTypeKey == DbConstants.CashFlowType.In ? DbConstants.TransactionType.FutureTransactionInOpeningBalance : DbConstants.TransactionType.FutureTransactionOutOpeningBalance,
                VoucherTypeKey = DbConstants.VoucherType.FutureTransaction,
                TransactionKey = model.RowKey,
                TransactionDate = model.BillDate,
                ExtraUpdateKey = ExtraUpdateKey,
                IsUpdate = IsUpdate,
                BranchKey = model.BranchKey,
                Purpose = model.AccountHead.AccountHeadName + EduSuiteUIResources.BlankSpace + EduSuiteUIResources.OpeningBalance,
            });
            return accountFlowModelList;
        }
        private List<AccountFlowViewModel> PayableAmountList(FutureTransactionPaymentViewModel model, List<AccountFlowViewModel> accountFlowModelList, decimal amount)
        {
            long accountheadKey;
            if (model.CashFlowTypeKey == DbConstants.CashFlowType.Out)
            {
                accountheadKey = dbContext.VwAccountHeadSelectActiveOnlies.Where(x => x.RowKey == DbConstants.AccountHead.AccountsPayable).Select(x => x.RowKey).FirstOrDefault();
            }
            else
            {
                accountheadKey = dbContext.VwAccountHeadSelectActiveOnlies.Where(x => x.RowKey == DbConstants.AccountHead.AccountsReceivable).Select(x => x.RowKey).FirstOrDefault();
            }
            accountFlowModelList.Add(new AccountFlowViewModel
            {
                CashFlowTypeKey = model.CashFlowTypeKey == DbConstants.CashFlowType.In ? DbConstants.CashFlowType.Out : DbConstants.CashFlowType.In,
                AccountHeadKey = accountheadKey,
                Amount = amount,
                TransactionTypeKey = DbConstants.TransactionType.FutureTransactionPayment,
                VoucherTypeKey = DbConstants.VoucherType.FutureTransaction,
                TransactionKey = model.PaymentKey,
                TransactionDate = model.PaymentModeKey != DbConstants.PaymentMode.Cheque ? model.PaymentDate : (model.ChequeOrDDDate ?? model.PaymentDate),
                ExtraUpdateKey = 0,
                IsUpdate = model.IsUpdate,
                BranchKey = model.BranchKey,
                Purpose = model.Purpose + model.PaymentModeName + model.Remarks,
            });
            return accountFlowModelList;
        }
        private List<AccountFlowViewModel> DebitExpenseAmountList(FutureTransactionPaymentViewModel model, List<AccountFlowViewModel> accountFlowModelList)
        {
            // long accountheadKey;
            long ExtraUpdateKey = 0;
            //accountheadKey = dbContext.AccountHeads.Where(x => x.RowKey == model.AccountHeadKey).Select(x => x.RowKey).FirstOrDefault();
            accountFlowModelList.Add(new AccountFlowViewModel
            {
                CashFlowTypeKey = model.CashFlowTypeKey == DbConstants.CashFlowType.In ? DbConstants.CashFlowType.Out : DbConstants.CashFlowType.In,
                AccountHeadKey = model.AccountHeadKey ?? 0,
                Amount = model.TaxableAmount ?? 0,
                TransactionTypeKey = DbConstants.TransactionType.FutureTransactionPayment,
                VoucherTypeKey = DbConstants.VoucherType.FutureTransaction,
                TransactionKey = model.PaymentKey,
                TransactionDate = model.PaymentModeKey != DbConstants.PaymentMode.Cheque ? model.PaymentDate : (model.ChequeOrDDDate ?? model.PaymentDate),
                ExtraUpdateKey = ExtraUpdateKey,
                IsUpdate = model.IsUpdate,
                BranchKey = model.BranchKey,
                Purpose = model.Purpose + model.PaymentModeName + model.Remarks,
            });
            return accountFlowModelList;
        }
        private List<AccountFlowViewModel> DebitAmountList(FutureTransactionPaymentViewModel model, List<AccountFlowViewModel> accountFlowModelList)
        {
            //long accountheadKey;
            long ExtraUpdateKey = 0;
            //accountheadKey = dbContext.AccountHeads.Where(x => x.RowKey == model.AccountHeadKey).Select(x => x.RowKey).FirstOrDefault();
            accountFlowModelList.Add(new AccountFlowViewModel
            {
                CashFlowTypeKey = model.CashFlowTypeKey == DbConstants.CashFlowType.In ? DbConstants.CashFlowType.Out : DbConstants.CashFlowType.In,
                AccountHeadKey = model.AccountHeadKey ?? 0,
                Amount = model.PaidAmount ?? 0,
                TransactionTypeKey = DbConstants.TransactionType.FutureTransactionPayment,
                VoucherTypeKey = DbConstants.VoucherType.FutureTransaction,
                TransactionKey = model.PaymentKey,
                TransactionDate = model.PaymentModeKey != DbConstants.PaymentMode.Cheque ? model.PaymentDate : (model.ChequeOrDDDate ?? model.PaymentDate),
                ExtraUpdateKey = ExtraUpdateKey,
                IsUpdate = model.IsUpdate,
                BranchKey = model.BranchKey,
                Purpose = model.Purpose + model.PaymentModeName + model.Remarks,
            });
            return accountFlowModelList;
        }
        private List<AccountFlowViewModel> CreditAmountList(FutureTransactionPaymentViewModel model, List<AccountFlowViewModel> accountFlowModelList, long oldBankKey)
        {
            long accountHeadKey;
            long ExtraUpdateKey = 0;
            long oldBankAccountHeadKey = 0;
            long oldAccountHeadKey;

            bool IsUpdate = model.IsUpdate;
            short oldPaymentModeKey = 0;



            //if (model.OldPaymentModeKey == DbConstants.PaymentMode.Bank || model.OldPaymentModeKey == DbConstants.PaymentMode.Cheque)
            //{
            //    string bankAccountCode = dbContext.BankAccounts.Where(x => x.RowKey == oldBankKey).Select(x => x.AccountHeadCode).FirstOrDefault();
            //    oldBankAccountHeadKey = dbContext.VwAccountHeadSelectActiveOnlies.Where(x => x.AccountHeadCode == bankAccountCode).Select(x => x.RowKey).FirstOrDefault();
            //}
            if (model.OldPaymentModeKey == DbConstants.PaymentMode.Bank || model.OldPaymentModeKey == DbConstants.PaymentMode.Cheque)
            {
                oldAccountHeadKey = dbContext.BankAccounts.Where(x => x.RowKey == oldBankKey).Select(x => x.AccountHeadKey).FirstOrDefault();
                //oldBankAccountHeadKey = dbContext.VwAccountHeadSelectActiveOnlies.Where(x => x.RowKey == oldAccountHeadKey).Select(x => x.RowKey).FirstOrDefault();
                oldBankAccountHeadKey = oldAccountHeadKey;
            }
            else
            {
                oldAccountHeadKey = dbContext.VwAccountHeadSelectActiveOnlies.Where(x => x.RowKey == DbConstants.AccountHead.CashAccount).Select(x => x.RowKey).FirstOrDefault();

            }

            if (model.PaymentModeKey == DbConstants.PaymentMode.Bank || model.PaymentModeKey == DbConstants.PaymentMode.Cheque)
            {
                accountHeadKey = dbContext.BankAccounts.Where(x => x.RowKey == model.BankAccountKey).Select(x => x.AccountHeadKey).FirstOrDefault();
            }
            else
            {
                accountHeadKey = dbContext.VwAccountHeadSelectActiveOnlies.Where(x => x.RowKey == DbConstants.AccountHead.CashAccount).Select(x => x.RowKey).FirstOrDefault();
            }
            //if (model.OldPaymentModeKey != null && model.OldPaymentModeKey != 0 && model.OldPaymentModeKey != model.PaymentModeKey)
            //{
            //    model.IsUpdate = false;
            //    ExtraUpdateKey = model.OldPaymentModeKey == DbConstants.PaymentMode.Cash ? DbConstants.AccountHead.CashAccount : oldBankAccountHeadKey;
            //}
            if (oldPaymentModeKey != null && oldPaymentModeKey != 0 && oldPaymentModeKey != model.PaymentModeKey)
            {
                IsUpdate = false;
                ExtraUpdateKey = oldPaymentModeKey == DbConstants.PaymentMode.Cash ? DbConstants.AccountHead.CashAccount : oldBankAccountHeadKey;
            }
            accountFlowModelList.Add(new AccountFlowViewModel
            {
                CashFlowTypeKey = model.CashFlowTypeKey ?? 0,
                AccountHeadKey = accountHeadKey,
                Amount = model.PaidAmount ?? 0,
                TransactionTypeKey = DbConstants.TransactionType.FutureTransactionPayment,
                VoucherTypeKey = DbConstants.VoucherType.FutureTransaction,
                TransactionDate = model.PaymentModeKey != DbConstants.PaymentMode.Cheque ? model.PaymentDate : (model.ChequeOrDDDate ?? model.PaymentDate),
                ExtraUpdateKey = ExtraUpdateKey,
                IsUpdate = model.IsUpdate,
                Purpose = model.Purpose + model.Remarks,
                BranchKey = model.BranchKey,
                TransactionKey = model.PaymentKey,
                OldAccountHeadKey = oldAccountHeadKey
            });
            return accountFlowModelList;
        }
        //private List<AccountFlowViewModel> CreditTotalAmountList(FutureTransaction FutureTransaction, List<AccountFlowViewModel> accountFlowModelList, bool IsUpdate, short? branchKey, long oldAccountHeadKey, byte oldCashFlowTypeKey, byte oldAccountGroupKey)
        private List<AccountFlowViewModel> CreditTotalAmountList(FutureTransaction FutureTransaction, List<AccountFlowViewModel> accountFlowModelList, bool IsUpdate, short? branchKey, long oldAccountHeadKey)
        {
            var accountHeadList = dbContext.AccountHeads.SingleOrDefault(x => x.RowKey == FutureTransaction.AccountHeadKey);
            //string oldaccountHeadCode = dbContext.AccountHeads.Where(x => x.RowKey == oldAccountHeadKey).Select(x => x.AccountHeadCode).FirstOrDefault();
            long ExtraUpdateKey = 0;
            long accountHeadKey;


            if (accountHeadList.AccountHeadType.AccountGroupKey == DbConstants.AccountGroup.Income || accountHeadList.AccountHeadType.AccountGroupKey == DbConstants.AccountGroup.Expenses)
            {

                //if (FutureTransaction.CashFlowTypeKey != oldCashFlowTypeKey)
                //{
                //    accountHeadCode = dbContext.VwAccountHeadSelectActiveOnlies.Where(x => x.RowKey == DbConstants.AccountHead.AccountsReceivable).Select(x => x.AccountHeadCode).FirstOrDefault();
                //}
                // else
                // {
                //    accountHeadCode = dbContext.VwAccountHeadSelectActiveOnlies.Where(x => x.RowKey == DbConstants.AccountHead.AccountsPayable).Select(x => x.AccountHeadCode).FirstOrDefault();

                // }
                if (FutureTransaction.CashFlowTypeKey == DbConstants.CashFlowType.Out)
                {
                    accountHeadKey = dbContext.VwAccountHeadSelectActiveOnlies.Where(x => x.RowKey == DbConstants.AccountHead.AccountsPayable).Select(x => x.RowKey).FirstOrDefault();
                }
                else
                {
                    accountHeadKey = dbContext.VwAccountHeadSelectActiveOnlies.Where(x => x.RowKey == DbConstants.AccountHead.AccountsReceivable).Select(x => x.RowKey).FirstOrDefault();
                }
                accountFlowModelList.Add(new AccountFlowViewModel
                {
                    CashFlowTypeKey = FutureTransaction.CashFlowTypeKey,
                    AccountHeadKey = accountHeadKey,
                    Amount = FutureTransaction.Amount,
                    TransactionTypeKey = DbConstants.TransactionType.FutureTransaction,
                    VoucherTypeKey = DbConstants.VoucherType.FutureTransaction,
                    TransactionKey = FutureTransaction.RowKey,
                    TransactionDate = FutureTransaction.BillDate,
                    ExtraUpdateKey = ExtraUpdateKey,
                    IsUpdate = IsUpdate,
                    BranchKey = branchKey,
                    Purpose = FutureTransaction.AccountHead.AccountHeadName,
                    OldAccountHeadKey = oldAccountHeadKey,
                });
                if (FutureTransaction.IsTax == true)
                {
                    accountFlowModelList.Add(new AccountFlowViewModel
                    {
                        CashFlowTypeKey = FutureTransaction.CashFlowTypeKey,
                        AccountHeadKey = accountHeadKey,
                        Amount = (FutureTransaction.IGSTAmt ?? 0) + (FutureTransaction.CGSTAmt ?? 0) + (FutureTransaction.SGSTAmt ?? 0),
                        TransactionTypeKey = DbConstants.TransactionType.FutureTransaction,
                        VoucherTypeKey = DbConstants.VoucherType.FutureTransaction,
                        TransactionKey = FutureTransaction.RowKey,
                        TransactionDate = FutureTransaction.BillDate,
                        ExtraUpdateKey = ExtraUpdateKey,
                        IsUpdate = IsUpdate,
                        BranchKey = branchKey,
                        Purpose = FutureTransaction.AccountHead.AccountHeadName + EduSuiteUIResources.BlankSpace + "GST",
                        OldAccountHeadKey = oldAccountHeadKey,
                    });
                }
                accountHeadKey = dbContext.VwAccountHeadSelectActiveOnlies.Where(x => x.RowKey == FutureTransaction.AccountHeadKey).Select(x => x.RowKey).FirstOrDefault();
                accountFlowModelList.Add(new AccountFlowViewModel
                {
                    CashFlowTypeKey = FutureTransaction.CashFlowTypeKey == DbConstants.CashFlowType.In ? DbConstants.CashFlowType.Out : DbConstants.CashFlowType.In,
                    AccountHeadKey = accountHeadKey,
                    Amount = FutureTransaction.Amount,
                    TransactionTypeKey = DbConstants.TransactionType.FutureTransaction,
                    VoucherTypeKey = DbConstants.VoucherType.FutureTransaction,
                    TransactionKey = FutureTransaction.RowKey,
                    TransactionDate = FutureTransaction.BillDate,
                    ExtraUpdateKey = ExtraUpdateKey,
                    IsUpdate = IsUpdate,
                    BranchKey = branchKey,
                    Purpose = FutureTransaction.AccountHead.AccountHeadName,
                    OldAccountHeadKey = oldAccountHeadKey,
                });
            }
            else
            {
                if (FutureTransaction.IsTax == true)
                {
                    accountHeadKey = dbContext.VwAccountHeadSelectActiveOnlies.Where(x => x.RowKey == FutureTransaction.AccountHeadKey).Select(x => x.RowKey).FirstOrDefault();
                    accountFlowModelList.Add(new AccountFlowViewModel
                    {
                        CashFlowTypeKey = FutureTransaction.CashFlowTypeKey,
                        AccountHeadKey = accountHeadKey,
                        Amount = (FutureTransaction.IGSTAmt ?? 0) + (FutureTransaction.CGSTAmt ?? 0) + (FutureTransaction.SGSTAmt ?? 0),
                        TransactionTypeKey = DbConstants.TransactionType.FutureTransaction,
                        VoucherTypeKey = DbConstants.VoucherType.FutureTransaction,
                        TransactionKey = FutureTransaction.RowKey,
                        TransactionDate = FutureTransaction.BillDate,
                        ExtraUpdateKey = ExtraUpdateKey,
                        IsUpdate = IsUpdate,
                        BranchKey = branchKey,
                        Purpose = FutureTransaction.AccountHead.AccountHeadName + EduSuiteUIResources.BlankSpace + "GST",
                        OldAccountHeadKey = oldAccountHeadKey,
                    });
                }

            }
            return accountFlowModelList;
        }
        private List<AccountFlowViewModel> CreditIGSTList(FutureTransaction FutureTransaction, List<AccountFlowViewModel> accountFlowModelList, bool IsUpdate, short? branchKey)
        {
            long accountHeadKey;
            if (FutureTransaction.CashFlowTypeKey == DbConstants.CashFlowType.Out)
            {
                accountHeadKey = dbContext.VwAccountHeadSelectActiveOnlies.Where(x => x.RowKey == DbConstants.AccountHead.InputTaxIGST).Select(x => x.RowKey).FirstOrDefault();
            }
            else
            {
                accountHeadKey = dbContext.VwAccountHeadSelectActiveOnlies.Where(x => x.RowKey == DbConstants.AccountHead.OutputTaxIGST).Select(x => x.RowKey).FirstOrDefault();
            }
            long ExtraUpdateKey = 0;
            accountFlowModelList.Add(new AccountFlowViewModel
            {
                CashFlowTypeKey = FutureTransaction.CashFlowTypeKey == DbConstants.CashFlowType.In ? DbConstants.CashFlowType.Out : DbConstants.CashFlowType.In,
                AccountHeadKey = accountHeadKey,
                Amount = FutureTransaction.IGSTAmt ?? 0,
                TransactionTypeKey = DbConstants.TransactionType.FutureTransactionCGST,
                VoucherTypeKey = DbConstants.VoucherType.InputTax,
                TransactionKey = FutureTransaction.RowKey,
                TransactionDate = FutureTransaction.BillDate,
                ExtraUpdateKey = ExtraUpdateKey,
                IsUpdate = IsUpdate,
                BranchKey = branchKey,
                Purpose = FutureTransaction.AccountHead.AccountHeadName + " IGST",
            });
            return accountFlowModelList;
        }
        private List<AccountFlowViewModel> CreditCGSTList(FutureTransaction FutureTransaction, List<AccountFlowViewModel> accountFlowModelList, bool IsUpdate, short? branchKey)
        {
            long accountHeadKey;
            if (FutureTransaction.CashFlowTypeKey == DbConstants.CashFlowType.Out)
            {
                accountHeadKey = dbContext.VwAccountHeadSelectActiveOnlies.Where(x => x.RowKey == DbConstants.AccountHead.InputTaxCGST).Select(x => x.RowKey).FirstOrDefault();
            }
            else
            {
                accountHeadKey = dbContext.VwAccountHeadSelectActiveOnlies.Where(x => x.RowKey == DbConstants.AccountHead.OutputTaxCGST).Select(x => x.RowKey).FirstOrDefault();
            }
            long ExtraUpdateKey = 0;
            accountFlowModelList.Add(new AccountFlowViewModel
            {
                CashFlowTypeKey = FutureTransaction.CashFlowTypeKey == DbConstants.CashFlowType.In ? DbConstants.CashFlowType.Out : DbConstants.CashFlowType.In,
                AccountHeadKey = accountHeadKey,
                Amount = FutureTransaction.CGSTAmt ?? 0,
                TransactionTypeKey = DbConstants.TransactionType.FutureTransactionCGST,
                VoucherTypeKey = DbConstants.VoucherType.InputTax,
                TransactionKey = FutureTransaction.RowKey,
                TransactionDate = FutureTransaction.BillDate,
                ExtraUpdateKey = ExtraUpdateKey,
                IsUpdate = IsUpdate,
                BranchKey = branchKey,
                Purpose = FutureTransaction.AccountHead.AccountHeadName + " CGST",
            });
            return accountFlowModelList;
        }
        private List<AccountFlowViewModel> CreditSGSTList(FutureTransaction FutureTransaction, List<AccountFlowViewModel> accountFlowModelList, bool IsUpdate, short? branchKey)
        {
            long accountHeadKey;
            if (FutureTransaction.CashFlowTypeKey == DbConstants.CashFlowType.Out)
            {
                accountHeadKey = dbContext.VwAccountHeadSelectActiveOnlies.Where(x => x.RowKey == DbConstants.AccountHead.InputTaxSGST).Select(x => x.RowKey).FirstOrDefault();
            }
            else
            {
                accountHeadKey = dbContext.VwAccountHeadSelectActiveOnlies.Where(x => x.RowKey == DbConstants.AccountHead.OutputTaxSGST).Select(x => x.RowKey).FirstOrDefault();
            }
            long ExtraUpdateKey = 0;
            accountFlowModelList.Add(new AccountFlowViewModel
            {
                CashFlowTypeKey = FutureTransaction.CashFlowTypeKey == DbConstants.CashFlowType.In ? DbConstants.CashFlowType.Out : DbConstants.CashFlowType.In,
                AccountHeadKey = accountHeadKey,
                Amount = FutureTransaction.SGSTAmt ?? 0,
                TransactionTypeKey = DbConstants.TransactionType.FutureTransactionSGST,
                VoucherTypeKey = DbConstants.VoucherType.InputTax,
                TransactionKey = FutureTransaction.RowKey,
                TransactionDate = FutureTransaction.BillDate,
                ExtraUpdateKey = ExtraUpdateKey,
                IsUpdate = IsUpdate,
                BranchKey = branchKey,
                Purpose = FutureTransaction.AccountHead.AccountHeadName + " SGST",
            });
            return accountFlowModelList;
        }
        private List<AccountFlowViewModel> CreditOtherAmountList(FutureTransactionOtherAmountType OtherChargeModel, FutureTransactionViewModel Mastermodel, List<AccountFlowViewModel> accountFlowModelList, bool IsUpdate)
        {
            byte CashFlowTypeKey = 0;
            if (Mastermodel.MasterCashFlowTypeKey == DbConstants.CashFlowType.Out)
            {
                CashFlowTypeKey = OtherChargeModel.IsAddition == true ? DbConstants.CashFlowType.In : DbConstants.CashFlowType.Out;
            }
            else
            {
                CashFlowTypeKey = OtherChargeModel.IsAddition == true ? DbConstants.CashFlowType.Out : DbConstants.CashFlowType.In;
            }
            //long accountHeadKey = dbContext.VwAccountHeadSelectActiveOnlies.Where(x => x.RowKey == OtherChargeModel.AccountHeadKey).Select(x => x.RowKey).FirstOrDefault();
            long ExtraUpdateKey = 0;
            accountFlowModelList.Add(new AccountFlowViewModel
            {
                CashFlowTypeKey = CashFlowTypeKey,
                AccountHeadKey = OtherChargeModel.AccountHeadKey,
                Amount = OtherChargeModel.Amount,
                TransactionTypeKey = DbConstants.TransactionType.FutureTransactionOther,
                //VoucherTypeKey = DbConstants.VoucherType.Sales,
                VoucherTypeKey = DbConstants.VoucherType.FutureTransaction,
                TransactionKey = OtherChargeModel.RowKey,
                TransactionDate = Mastermodel.BillDate,
                ExtraUpdateKey = ExtraUpdateKey,
                IsUpdate = IsUpdate,
                BranchKey = Mastermodel.BranchKey,
                Purpose = OtherChargeModel.AccountHead.AccountHeadName + EduSuiteUIResources.BlankSpace + EduSuiteUIResources.of + EduSuiteUIResources.BlankSpace + Mastermodel.AccountHeadName,
            });
            return accountFlowModelList;
        }
        private List<AccountFlowViewModel> PayableOtherAmountList(FutureTransactionOtherAmountType OtherChargeModel, FutureTransactionViewModel Mastermodel, List<AccountFlowViewModel> accountFlowModelList, bool IsUpdate)
        {
            long accountHeadKey;
            byte CashFlowTypeKey = 0;
            if (Mastermodel.MasterCashFlowTypeKey == DbConstants.CashFlowType.Out)
            {
                accountHeadKey = dbContext.VwAccountHeadSelectActiveOnlies.Where(x => x.RowKey == DbConstants.AccountHead.AccountsPayable).Select(x => x.RowKey).FirstOrDefault();
                CashFlowTypeKey = OtherChargeModel.IsAddition == true ? DbConstants.CashFlowType.Out : DbConstants.CashFlowType.In;
            }
            else
            {
                accountHeadKey = dbContext.VwAccountHeadSelectActiveOnlies.Where(x => x.RowKey == DbConstants.AccountHead.AccountsReceivable).Select(x => x.RowKey).FirstOrDefault();
                CashFlowTypeKey = OtherChargeModel.IsAddition == true ? DbConstants.CashFlowType.In : DbConstants.CashFlowType.Out;
            }
            long ExtraUpdateKey = 0;
            accountFlowModelList.Add(new AccountFlowViewModel
            {
                CashFlowTypeKey = CashFlowTypeKey,
                AccountHeadKey = accountHeadKey,
                Amount = OtherChargeModel.Amount,
                TransactionTypeKey = DbConstants.TransactionType.FutureTransactionOther,
                // VoucherTypeKey = DbConstants.VoucherType.Sales,
                VoucherTypeKey = DbConstants.VoucherType.FutureTransaction,
                TransactionKey = OtherChargeModel.RowKey,
                TransactionDate = Mastermodel.BillDate,
                ExtraUpdateKey = ExtraUpdateKey,
                IsUpdate = IsUpdate,
                BranchKey = Mastermodel.BranchKey,
                Purpose = OtherChargeModel.AccountHead.AccountHeadName + EduSuiteUIResources.BlankSpace + EduSuiteUIResources.of + EduSuiteUIResources.BlankSpace + Mastermodel.AccountHeadName,
            });
            return accountFlowModelList;
        }
        private List<AccountFlowViewModel> OtherAmountList(FutureTransactionOtherAmountType OtherChargeModel, FutureTransactionViewModel Mastermodel, List<AccountFlowViewModel> accountFlowModelList, bool IsUpdate)
        {
            byte CashFlowTypeKey = 0;
            if (Mastermodel.MasterCashFlowTypeKey == DbConstants.CashFlowType.Out)
            {
                CashFlowTypeKey = OtherChargeModel.IsAddition == true ? DbConstants.CashFlowType.Out : DbConstants.CashFlowType.In;
            }
            else
            {
                CashFlowTypeKey = OtherChargeModel.IsAddition == true ? DbConstants.CashFlowType.In : DbConstants.CashFlowType.Out;
            }
            //string accountHeadCode = dbContext.VwAccountHeadSelectActiveOnlies.Where(x => x.RowKey == Mastermodel.AccountHeadKey).Select(x => x.AccountHeadCode).FirstOrDefault();
            long ExtraUpdateKey = 0;
            accountFlowModelList.Add(new AccountFlowViewModel
            {
                CashFlowTypeKey = CashFlowTypeKey,
                AccountHeadKey = Mastermodel.AccountHeadKey,
                Amount = OtherChargeModel.Amount,
                TransactionTypeKey = DbConstants.TransactionType.FutureTransactionOther,
                // VoucherTypeKey = DbConstants.VoucherType.Sales,
                VoucherTypeKey = DbConstants.VoucherType.FutureTransaction,
                TransactionKey = OtherChargeModel.RowKey,
                TransactionDate = Mastermodel.BillDate,
                ExtraUpdateKey = ExtraUpdateKey,
                IsUpdate = IsUpdate,
                BranchKey = Mastermodel.BranchKey,
                Purpose = OtherChargeModel.AccountHead.AccountHeadName + EduSuiteUIResources.BlankSpace + EduSuiteUIResources.of + EduSuiteUIResources.BlankSpace + Mastermodel.AccountHeadName,
            });
            return accountFlowModelList;
        }
        private List<AccountFlowViewModel> CreditOtherPaymentAmountList(FutureTransactionOtherAmountType OtherChargeModel, FutureTransactionPaymentViewModel Mastermodel, List<AccountFlowViewModel> accountFlowModelList, bool IsUpdate, string AccountHeadName)
        {
            byte CashFlowTypeKey = 0;
            if (Mastermodel.CashFlowTypeKey == DbConstants.CashFlowType.Out)
            {
                CashFlowTypeKey = OtherChargeModel.IsAddition == true ? DbConstants.CashFlowType.In : DbConstants.CashFlowType.Out;
            }
            else
            {
                CashFlowTypeKey = OtherChargeModel.IsAddition == true ? DbConstants.CashFlowType.Out : DbConstants.CashFlowType.In;
            }
            //string accountHeadCode = dbContext.VwAccountHeadSelectActiveOnlies.Where(x => x.RowKey == OtherChargeModel.AccountHeadKey).Select(x => x.AccountHeadCode).FirstOrDefault();
            long ExtraUpdateKey = 0;
            accountFlowModelList.Add(new AccountFlowViewModel
            {
                CashFlowTypeKey = CashFlowTypeKey,
                AccountHeadKey = OtherChargeModel.AccountHeadKey,
                Amount = OtherChargeModel.Amount,
                TransactionTypeKey = DbConstants.TransactionType.FutureTransactionOther,
                // VoucherTypeKey = DbConstants.VoucherType.Sales,
                VoucherTypeKey = DbConstants.VoucherType.FutureTransaction,
                TransactionKey = OtherChargeModel.RowKey,
                TransactionDate = Mastermodel.PaymentDate,
                ExtraUpdateKey = ExtraUpdateKey,
                IsUpdate = IsUpdate,
                BranchKey = Mastermodel.BranchKey,
                Purpose = OtherChargeModel.AccountHead.AccountHeadName + EduSuiteUIResources.BlankSpace + EduSuiteUIResources.of + EduSuiteUIResources.BlankSpace + AccountHeadName,
            });
            return accountFlowModelList;
        }
        private List<AccountFlowViewModel> PayableOtherPaymentAmountList(FutureTransactionOtherAmountType OtherChargeModel, FutureTransactionPaymentViewModel Mastermodel, List<AccountFlowViewModel> accountFlowModelList, bool IsUpdate, string AccountHeadName)
        {
            long accountHeadKey;
            byte CashFlowTypeKey = 0;
            if (Mastermodel.CashFlowTypeKey == DbConstants.CashFlowType.Out)
            {
                accountHeadKey = dbContext.VwAccountHeadSelectActiveOnlies.Where(x => x.RowKey == DbConstants.AccountHead.AccountsPayable).Select(x => x.RowKey).FirstOrDefault();
                CashFlowTypeKey = OtherChargeModel.IsAddition == true ? DbConstants.CashFlowType.Out : DbConstants.CashFlowType.In;
            }
            else
            {
                accountHeadKey = dbContext.VwAccountHeadSelectActiveOnlies.Where(x => x.RowKey == DbConstants.AccountHead.AccountsReceivable).Select(x => x.RowKey).FirstOrDefault();
                CashFlowTypeKey = OtherChargeModel.IsAddition == true ? DbConstants.CashFlowType.In : DbConstants.CashFlowType.Out;
            }
            long ExtraUpdateKey = 0;
            accountFlowModelList.Add(new AccountFlowViewModel
            {
                CashFlowTypeKey = CashFlowTypeKey,
                AccountHeadKey = accountHeadKey,
                Amount = OtherChargeModel.Amount,
                TransactionTypeKey = DbConstants.TransactionType.FutureTransactionOther,
                // VoucherTypeKey = DbConstants.VoucherType.Sales,
                VoucherTypeKey = DbConstants.VoucherType.FutureTransaction,
                TransactionKey = OtherChargeModel.RowKey,
                TransactionDate = Mastermodel.PaymentDate,
                ExtraUpdateKey = ExtraUpdateKey,
                IsUpdate = IsUpdate,
                BranchKey = Mastermodel.BranchKey,
                Purpose = OtherChargeModel.AccountHead.AccountHeadName + EduSuiteUIResources.BlankSpace + EduSuiteUIResources.of + EduSuiteUIResources.BlankSpace + AccountHeadName,
            });
            return accountFlowModelList;
        }
        private List<AccountFlowViewModel> OtherPaymentAmountList(FutureTransactionOtherAmountType OtherChargeModel, FutureTransactionPaymentViewModel Mastermodel, List<AccountFlowViewModel> accountFlowModelList, bool IsUpdate, string AccountHeadName)
        {
            byte CashFlowTypeKey = 0;
            if (Mastermodel.CashFlowTypeKey == DbConstants.CashFlowType.Out)
            {
                CashFlowTypeKey = OtherChargeModel.IsAddition == true ? DbConstants.CashFlowType.Out : DbConstants.CashFlowType.In;
            }
            else
            {
                CashFlowTypeKey = OtherChargeModel.IsAddition == true ? DbConstants.CashFlowType.In : DbConstants.CashFlowType.Out;
            }
            //string accountHeadCode = dbContext.VwAccountHeadSelectActiveOnlies.Where(x => x.RowKey == Mastermodel.AccountHeadKey).Select(x => x.AccountHeadCode).FirstOrDefault();
            long ExtraUpdateKey = 0;
            accountFlowModelList.Add(new AccountFlowViewModel
            {
                CashFlowTypeKey = CashFlowTypeKey,
                AccountHeadKey = Mastermodel.AccountHeadKey ?? 0,
                Amount = OtherChargeModel.Amount,
                TransactionTypeKey = DbConstants.TransactionType.FutureTransactionOther,
                // VoucherTypeKey = DbConstants.VoucherType.Sales,
                VoucherTypeKey = DbConstants.VoucherType.FutureTransaction,
                TransactionKey = OtherChargeModel.RowKey,
                TransactionDate = Mastermodel.PaymentDate,
                ExtraUpdateKey = ExtraUpdateKey,
                IsUpdate = IsUpdate,
                BranchKey = Mastermodel.BranchKey,
                Purpose = OtherChargeModel.AccountHead.AccountHeadName + EduSuiteUIResources.BlankSpace + EduSuiteUIResources.of + EduSuiteUIResources.BlankSpace + AccountHeadName,
            });
            return accountFlowModelList;
        }

        private void purposeGeneration(FutureTransactionPaymentViewModel model, string AccountHeadName)
        {
            model.Purpose = (AccountHeadName + (model.Purpose != null && model.Purpose != "" ? model.Purpose : "")) + EduSuiteUIResources.BlankSpace + (model.CashFlowTypeKey == DbConstants.CashFlowType.In ? EduSuiteUIResources.Receipt : EduSuiteUIResources.Payment) + EduSuiteUIResources.BlankSpace;
            string PaidBy = (model.PaidBy != null && model.PaidBy != "" ? EduSuiteUIResources.Paid + EduSuiteUIResources.BlankSpace + EduSuiteUIResources.By + EduSuiteUIResources.BlankSpace + model.PaidBy + EduSuiteUIResources.BlankSpace : "");
            string ReceivedBy = (model.ReceivedBy != null && model.ReceivedBy != "" ? EduSuiteUIResources.Recieved + EduSuiteUIResources.BlankSpace + EduSuiteUIResources.By + EduSuiteUIResources.BlankSpace + model.ReceivedBy + EduSuiteUIResources.BlankSpace : "");
            string OnBehalfOf = model.OnBehalfOf != null && model.OnBehalfOf != "" ? EduSuiteUIResources.OnBehalfOf + EduSuiteUIResources.BlankSpace + model.OnBehalfOf + EduSuiteUIResources.BlankSpace : "";
            string AuthorizedBy = model.AuthorizedBy != null && model.AuthorizedBy != "" ? EduSuiteUIResources.AuthorizedBy + EduSuiteUIResources.BlankSpace + model.AuthorizedBy + EduSuiteUIResources.BlankSpace : "";
            model.Remarks = model.Remarks != null && model.Remarks != "" ? EduSuiteUIResources.OpenBracket + model.Remarks + EduSuiteUIResources.CloseBracket : "";
            model.Purpose = model.Purpose + (model.CashFlowTypeKey == DbConstants.CashFlowType.In ? PaidBy + ReceivedBy : ReceivedBy + PaidBy) + OnBehalfOf + AuthorizedBy;
            model.PaymentModeName = EduSuiteUIResources.By + EduSuiteUIResources.BlankSpace + (model.PaymentModeKey == DbConstants.PaymentMode.Cash ? EduSuiteUIResources.Cash : model.PaymentModeKey == DbConstants.PaymentMode.Bank ? EduSuiteUIResources.Bank + EduSuiteUIResources.OpenBracket + model.BankAccountName + EduSuiteUIResources.CloseBracket : model.PaymentModeKey == DbConstants.PaymentMode.Cheque ? EduSuiteUIResources.Cheque + EduSuiteUIResources.OpenBracket + model.BankAccountName + EduSuiteUIResources.CloseBracket : "");
        }
        #endregion

        #region View
        public FutureTransactionViewModel ViewFutureTransactionById(int? id)
        {
            try
            {
                FutureTransactionViewModel model = new FutureTransactionViewModel();
                model = (from row in dbContext.FutureTransactions
                         join r in dbContext.FutureTransactionPayments.Where(r => r.IsAdavance == true)
                         on row.RowKey equals r.FutureTransactionKey into joined
                         from j in joined.DefaultIfEmpty()
                         select new FutureTransactionViewModel
                         {
                             RowKey = row.RowKey,
                             BranchKey = row.BranchKey,
                             AccountHeadKey = row.AccountHeadKey,
                             BillDate = row.BillDate,
                             BillNo = row.BillNo,
                             TotalAmount = row.TotalAmount,
                             AmountPaid = row.AmountPaid,
                             FutureTransactionPaymentRowKey = j.RowKey != null ? j.RowKey : 0,
                             PaymentModeKey = j.PaymentModeKey != null ? j.PaymentModeKey : DbConstants.PaymentMode.Cash,
                             MasterCashFlowTypeKey = row.CashFlowTypeKey,
                             PaymentModeSubKey = j.PaymentModeSubKey,
                             ReferenceNumber = j.ReferenceNumber,
                             CardNumber = j.CardNumber,
                             BankAccountKey = j.BankAccountKey,
                             ChequeOrDDNumber = j.ChequeOrDDNumber,
                             ChequeOrDDDate = j.ChequeOrDDDate,
                             TaxableAmount = row.TaxableAmount,
                             NonTaxableAmount = row.NonTaxableAmount,
                             CashFlowTypeName = row.CashFlowType.CashFlowTypeName,
                             CGSTAmt = row.CGSTAmt,
                             SGSTAmt = row.SGSTAmt,
                             IGSTAmt = row.IGSTAmt,
                             CGSTPer = row.CGSTPer,
                             SGSTPer = row.SGSTPer,
                             IGSTPer = row.IGSTPer,
                             HSNCodeKey = row.HSNCodeMasterKey,
                             HSNCode = row.HSNCodes,
                             RoundOff = row.RoundOff,
                             AccountHeadName = row.AccountHead.AccountHeadName,
                             StateName = row.Province.Provincename,
                             SubTotal = row.SubTotal,
                             //OrderProcessTypeKey = row.OrderProcessTypeKey,
                             CompanyStateKey = row.StateKey,
                             StateKey = row.StateKey,
                             IsTax = row.IsTax,
                             InstallmentTypeName = row.PeriodType.PeriodTypeName,
                             IsInstallment = row.IsInstallment,
                             InstallmentAmount = row.InstallmentAmount,
                             InstallmentPeriod = row.InstallmentPeriod,
                             InstallmentTypeKey = row.InstallmentTypeKey,
                             NoOfInstallment = row.NoOfInstallment,
                             CashFlowTypeKey = j.CashFlowTypeKey,
                             DownPayment = row.DownPayment,
                             Amount = row.Amount,
                             GSTINNumber = row.GSTINNumber,
                             IsContra = row.IsContra,
                             OpeningReceivedAmount = row.OpeningReceivedAmount,
                             OpeningPaidAmount = row.OpeningPaidAmount,
                             InstallmentFlowKey = row.InstallmentFlowKey,
                             DeductAmount = dbContext.FutureTransactionOtherAmountTypes.Where(x => x.IsAddition == false && (x.FutureTransactionKey == id || x.FutureTransactionPayment.FutureTransactionKey == id)).Select(x => x.Amount).DefaultIfEmpty().Sum(),
                             EarningAmount = dbContext.FutureTransactionOtherAmountTypes.Where(x => x.IsAddition == true && (x.FutureTransactionKey == id || x.FutureTransactionPayment.FutureTransactionKey == id)).Select(x => x.Amount).DefaultIfEmpty().Sum(),
                             TotalInAmount = row.FutureTransactionPayments.Where(x => x.CashFlowTypeKey == DbConstants.CashFlowType.In).Select(x => x.PaidAmount ?? 0).DefaultIfEmpty().Sum() + (row.OpeningReceivedAmount ?? 0),
                             TotalOutAmount = row.FutureTransactionPayments.Where(x => x.CashFlowTypeKey == DbConstants.CashFlowType.Out).Select(x => x.PaidAmount ?? 0).DefaultIfEmpty().Sum() + (row.OpeningPaidAmount ?? 0),
                         }).Where(x => x.RowKey == id).FirstOrDefault();
                FillViewFutureTransactionOtherAmountType(model);
                GetFutureTransactionPayments(model);
                return model;
            }
            catch (Exception ex)
            {
                FutureTransactionViewModel model = new FutureTransactionViewModel();
                model.Message = ex.GetBaseException().Message;
                return model;
            }
        }

        public void GetFutureTransactionPayments(FutureTransactionViewModel model)
        {
            try
            {
                model.FutureTransactionPaidAmounts = (from d in dbContext.FutureTransactions
                                                      join p in dbContext.FutureTransactionPayments.Where(r => (r.FutureTransactionKey == model.RowKey) && r.CashFlowTypeKey == DbConstants.CashFlowType.Out)
                                                   on d.RowKey equals p.FutureTransactionKey
                                                      select new PaymentWindowViewModel
                                                      {
                                                          RowKey = p.RowKey,
                                                          PaymentDate = p.PaymentDate,
                                                          Purpose = p.Purpose,
                                                          ReceivedAmount = p.PaidAmount ?? 0,
                                                          BalanceAmount = p.BalanceAmount,
                                                          TotalAmount = d.TotalAmount,
                                                          PaymentModeKey = p.PaymentModeKey,
                                                          PaymentModeSubKey = p.PaymentModeSubKey,
                                                          BranchKey = p.FutureTransaction.BranchKey,
                                                          ReceiptNumber = p.ReceiptNumber
                                                      }).ToList();
                model.FutureTransactionRecievedAmounts = (from d in dbContext.FutureTransactions
                                                          join p in dbContext.FutureTransactionPayments.Where(r => (r.FutureTransactionKey == model.RowKey) && r.CashFlowTypeKey == DbConstants.CashFlowType.In)
                                                      on d.RowKey equals p.FutureTransactionKey
                                                          select new PaymentWindowViewModel
                                                          {
                                                              RowKey = p.RowKey,
                                                              PaymentDate = p.PaymentDate,
                                                              Purpose = p.Purpose,
                                                              ReceivedAmount = p.PaidAmount ?? 0,
                                                              BalanceAmount = p.BalanceAmount,
                                                              TotalAmount = d.TotalAmount,
                                                              PaymentModeKey = p.PaymentModeKey,
                                                              PaymentModeSubKey = p.PaymentModeSubKey,
                                                              BranchKey = p.FutureTransaction.BranchKey,
                                                              ReceiptNumber = p.ReceiptNumber
                                                          }).ToList();
                paymentModeDetails(model);
            }
            catch (Exception ex)
            {

            }
        }

        private void paymentModeDetails(FutureTransactionViewModel model)
        {
            foreach (PaymentWindowViewModel modelItem in model.FutureTransactionPaidAmounts)
            {
                PaymentWindowViewModel m = (from p in dbContext.FutureTransactionPayments.Where(r => r.RowKey == modelItem.PaymentKey)
                                            select new PaymentWindowViewModel
                                            {
                                                BankAccountKey = p.BankAccountKey,
                                                BankAccountName = p.BankAccount.Bank.BankName + "(" + p.BankAccount.BranchLocation + ")" + Environment.NewLine + p.BankAccount.AccountNumber,
                                                CardNumber = p.CardNumber,
                                                ChequeOrDDNumber = p.ChequeOrDDNumber,
                                                ChequeOrDDDate = p.ChequeOrDDDate,
                                            }).FirstOrDefault();

                switch (modelItem.PaymentModeKey)
                {
                    case DbConstants.PaymentMode.Cash:
                        modelItem.PaidBy = EduSuiteUIResources.Cash;
                        break;

                    case DbConstants.PaymentMode.Bank:
                        modelItem.PaidBy = EduSuiteUIResources.Bank + EduSuiteUIResources.BlankSpace + m.BankAccountName;
                        break;

                    case DbConstants.PaymentMode.Cheque:
                        modelItem.PaidBy = EduSuiteUIResources.Cheque + "(" + EduSuiteUIResources.BlankSpace + m.ChequeOrDDNumber + "," + EduSuiteUIResources.BlankSpace + Convert.ToDateTime(m.ChequeOrDDDate).ToString("dd/MM/yyyy") + ")" + EduSuiteUIResources.BlankSpace + EduSuiteUIResources.BlankSpace + m.BankAccountName;
                        break;

                    //case DbConstants.PaymentMode.Card:
                    //    modelItem.PaidBy = EduSuiteUIResources.Card + EduSuiteUIResources.BlankSpace + m.CardNumber;
                    //    break;
                    default:
                        modelItem.PaidBy = EduSuiteUIResources.BlankSpace;
                        break;
                }
            }

            foreach (PaymentWindowViewModel modelItem in model.FutureTransactionRecievedAmounts)
            {
                PaymentWindowViewModel m = (from p in dbContext.FutureTransactionPayments.Where(r => r.RowKey == modelItem.PaymentKey)
                                            select new PaymentWindowViewModel
                                            {
                                                BankAccountKey = p.BankAccountKey,
                                                BankAccountName = p.BankAccount.Bank.BankName + "(" + p.BankAccount.BranchLocation + ")" + Environment.NewLine + p.BankAccount.AccountNumber,
                                                CardNumber = p.CardNumber,
                                                ChequeOrDDNumber = p.ChequeOrDDNumber,
                                                ChequeOrDDDate = p.ChequeOrDDDate,
                                            }).FirstOrDefault();

                switch (modelItem.PaymentModeKey)
                {
                    case DbConstants.PaymentMode.Cash:
                        modelItem.PaidBy = EduSuiteUIResources.Cash;
                        break;

                    case DbConstants.PaymentMode.Bank:
                        modelItem.PaidBy = EduSuiteUIResources.Bank + EduSuiteUIResources.BlankSpace + m.BankAccountName;
                        break;

                    case DbConstants.PaymentMode.Cheque:
                        modelItem.PaidBy = EduSuiteUIResources.Cheque + "(" + EduSuiteUIResources.BlankSpace + m.ChequeOrDDNumber + "," + EduSuiteUIResources.BlankSpace + Convert.ToDateTime(m.ChequeOrDDDate).ToString("dd/MM/yyyy") + ")" + EduSuiteUIResources.BlankSpace + EduSuiteUIResources.BlankSpace + m.BankAccountName;
                        break;

                    //case DbConstants.PaymentMode.Card:
                    //    modelItem.PaidBy = EduSuiteUIResources.Card + EduSuiteUIResources.BlankSpace + m.CardNumber;
                    //    break;
                    default:
                        modelItem.PaidBy = EduSuiteUIResources.BlankSpace;
                        break;
                }
            }

        }
        public void FillViewFutureTransactionOtherAmountType(FutureTransactionViewModel model)
        {
            model.FutureTransactionOtherAmountTypes = (from row in dbContext.FutureTransactionOtherAmountTypes.Where(x => x.FutureTransactionKey == model.RowKey || x.FutureTransactionPayment.FutureTransactionKey == model.RowKey)
                                                       select new FutureTransactionOtherAmountTypeViewModel
                                                       {
                                                           RowKey = row.RowKey,
                                                           AccountHeadKey = row.AccountHeadKey,
                                                           Amount = row.Amount,
                                                           AmountPer = row.AmountPer,
                                                           FutureTransactionKey = row.FutureTransactionKey,
                                                           FutureTransactionPaymentKey = row.FutureTransactionPaymentKey,
                                                           IsAddition = row.IsAddition,
                                                           AccountHeadName = row.AccountHead.AccountHeadName
                                                       }).ToList();
        }

        #endregion

        public FutureTransactionViewModel CheckBillNumberExists(string BillNumber, long? RowKey)
        {
            FutureTransactionViewModel model = new FutureTransactionViewModel();
            if (dbContext.FutureTransactions.Where(row => row.BillNo.ToUpper() == BillNumber.ToUpper() && row.RowKey != RowKey).Any())
            {
                model.Message = String.Format(EduSuiteUIResources.ExistsErrorMessage, EduSuiteUIResources.BillNo);
                model.IsSuccessful = false;
            }
            else
            {
                model.Message = "";
                model.IsSuccessful = true;
            }
            return model;
        }

        #region FutureTransaction Other Charges
        public void FillFutureTransactionOtherAmountType(FutureTransactionViewModel model)
        {
            model.FutureTransactionOtherAmountTypes = (from row in dbContext.FutureTransactionOtherAmountTypes.Where(x => x.FutureTransactionKey == model.RowKey)
                                                       select new FutureTransactionOtherAmountTypeViewModel
                                                       {
                                                           RowKey = row.RowKey,
                                                           AccountHeadKey = row.AccountHeadKey,
                                                           Amount = row.Amount,
                                                           AmountPer = row.AmountPer,
                                                           FutureTransactionKey = row.FutureTransactionKey,
                                                           FutureTransactionPaymentKey = row.FutureTransactionPaymentKey,
                                                           IsAddition = row.IsAddition,
                                                           AccountHeadName = row.AccountHead.AccountHeadName
                                                       }).ToList();
            if (model.FutureTransactionOtherAmountTypes.Count == 0)
            {
                model.FutureTransactionOtherAmountTypes.Add(new FutureTransactionOtherAmountTypeViewModel
                {
                    RowKey = 0,
                });
            }
            foreach (FutureTransactionOtherAmountTypeViewModel modelItem in model.FutureTransactionOtherAmountTypes)
            {
                FillAccountHeadsList(modelItem, model.AccountHeadKey);
            }

        }

        public FutureTransactionOtherAmountTypeViewModel FillAccountHeadsList(FutureTransactionOtherAmountTypeViewModel model, long AccountHeadKey)
        {
            string AccountHeads = dbContext.AccountHeads.Where(x => x.RowKey == AccountHeadKey).Select(x => x.FutureAccountHeads).FirstOrDefault();
            List<long> AccountHeadKeys = new List<long>();
            if (AccountHeads != null)
            {
                AccountHeadKeys = AccountHeads.Split(',').Select(Int64.Parse).ToList();
            }
            model.AccountHeads = dbContext.AccountHeads.Where(r => AccountHeadKeys.Contains(r.RowKey)).Select(row => new SelectListModel
            {
                RowKey = row.RowKey,
                Text = row.AccountHeadName
            }).ToList();
            return model;
        }

        private void CreateFutureTransactionOtherAmountType(List<FutureTransactionOtherAmountTypeViewModel> listModel, FutureTransactionViewModel FutureTransactionModel, long SalesOrderMasterKey)
        {
            int i = 0, j = 0;

            long maxKey = dbContext.FutureTransactionOtherAmountTypes.Select(P => P.RowKey).DefaultIfEmpty().Max();
            maxKey = maxKey + 1;
            foreach (FutureTransactionOtherAmountTypeViewModel modelItem in listModel)
            {
                FutureTransactionOtherAmountType FutureTransactionOtherAmountTypeModel = new FutureTransactionOtherAmountType();
                if (modelItem.AccountHeadKey != null && modelItem.Amount != null)
                {
                    FutureTransactionOtherAmountTypeModel.RowKey = Convert.ToInt64(maxKey + j);
                    FutureTransactionOtherAmountTypeModel.FutureTransactionKey = SalesOrderMasterKey;
                    FutureTransactionOtherAmountTypeModel.AccountHeadKey = modelItem.AccountHeadKey ?? 0;
                    FutureTransactionOtherAmountTypeModel.Amount = Convert.ToDecimal(modelItem.Amount);
                    FutureTransactionOtherAmountTypeModel.AmountPer = Convert.ToDecimal(modelItem.AmountPer);
                    FutureTransactionOtherAmountTypeModel.IsAddition = modelItem.IsAddition;
                    FutureTransactionOtherAmountTypeModel.AccountHead = dbContext.AccountHeads.SingleOrDefault(x => x.RowKey == modelItem.AccountHeadKey);

                    dbContext.FutureTransactionOtherAmountTypes.Add(FutureTransactionOtherAmountTypeModel);
                    i++; j++;


                    List<AccountFlowViewModel> accountFlowModelList = new List<AccountFlowViewModel>();
                    bool IsUpdate = false;
                    FutureTransaction futureTransaction = dbContext.FutureTransactions.SingleOrDefault(x => x.RowKey == FutureTransactionModel.RowKey);
                    FutureTransactionModel.AccountHeadName = dbContext.AccountHeads.Where(x => x.RowKey == FutureTransactionModel.AccountHeadKey).Select(x => x.AccountHeadName).FirstOrDefault();
                    if (futureTransaction != null)
                    {
                        if (futureTransaction.AmountPaid != futureTransaction.TotalAmount && (futureTransaction.AccountHead.AccountHeadType.AccountGroupKey == DbConstants.AccountGroup.Income || futureTransaction.AccountHead.AccountHeadType.AccountGroupKey == DbConstants.AccountGroup.Expenses))
                        {
                            accountFlowModelList = PayableOtherAmountList(FutureTransactionOtherAmountTypeModel, FutureTransactionModel, accountFlowModelList, IsUpdate);
                        }
                        else if (futureTransaction.AccountHead.AccountHeadType.AccountGroupKey != DbConstants.AccountGroup.Income && futureTransaction.AccountHead.AccountHeadType.AccountGroupKey != DbConstants.AccountGroup.Expenses)
                        {
                            accountFlowModelList = OtherAmountList(FutureTransactionOtherAmountTypeModel, FutureTransactionModel, accountFlowModelList, IsUpdate);
                        }
                    }
                    else
                    {
                        var accountHeadList = dbContext.AccountHeads.SingleOrDefault(x => x.RowKey == FutureTransactionModel.AccountHeadKey);
                        if (FutureTransactionModel.AmountPaid != FutureTransactionModel.TotalAmount && (accountHeadList.AccountHeadType.AccountGroupKey == DbConstants.AccountGroup.Income || accountHeadList.AccountHeadType.AccountGroupKey == DbConstants.AccountGroup.Expenses))
                        {
                            accountFlowModelList = PayableOtherAmountList(FutureTransactionOtherAmountTypeModel, FutureTransactionModel, accountFlowModelList, IsUpdate);
                        }
                        else if (accountHeadList.AccountHeadType.AccountGroupKey != DbConstants.AccountGroup.Income && accountHeadList.AccountHeadType.AccountGroupKey != DbConstants.AccountGroup.Expenses)
                        {
                            accountFlowModelList = OtherAmountList(FutureTransactionOtherAmountTypeModel, FutureTransactionModel, accountFlowModelList, IsUpdate);
                        }
                    }
                    accountFlowModelList = CreditOtherAmountList(FutureTransactionOtherAmountTypeModel, FutureTransactionModel, accountFlowModelList, IsUpdate);
                    //accountFlowModelList = DebitOtherAmountList(FutureTransactionOtherAmountTypeModel, FutureTransactionModel, accountFlowModelList, IsUpdate);
                    CreateAccountFlow(accountFlowModelList, IsUpdate);

                    modelItem.RowKey = FutureTransactionOtherAmountTypeModel.RowKey;
                    // modelItem.UpdateType = DbConstants.UpdationType.Create;
                }
            }


        }

        private void UpdateFutureTransactionOtherAmountType(List<FutureTransactionOtherAmountTypeViewModel> listModel, FutureTransactionViewModel FutureTransactionModel, long SalesOrderMasterKey)
        {
            FutureTransactionOtherAmountType FutureTransactionOtherAmountTypeModel = new FutureTransactionOtherAmountType();
            foreach (FutureTransactionOtherAmountTypeViewModel modelItem in listModel)
            {
                if (modelItem.AccountHeadKey != null && modelItem.Amount != null)
                {
                    FutureTransactionOtherAmountTypeModel = dbContext.FutureTransactionOtherAmountTypes.SingleOrDefault(row => row.RowKey == modelItem.RowKey);
                    FutureTransactionOtherAmountTypeModel.FutureTransactionKey = SalesOrderMasterKey;
                    FutureTransactionOtherAmountTypeModel.AccountHeadKey = modelItem.AccountHeadKey ?? 0;
                    FutureTransactionOtherAmountTypeModel.Amount = Convert.ToDecimal(modelItem.Amount);
                    FutureTransactionOtherAmountTypeModel.AmountPer = Convert.ToDecimal(modelItem.AmountPer);
                    FutureTransactionOtherAmountTypeModel.IsAddition = modelItem.IsAddition;
                    FutureTransactionOtherAmountTypeModel.AccountHead = dbContext.AccountHeads.SingleOrDefault(x => x.RowKey == modelItem.AccountHeadKey);


                    List<AccountFlowViewModel> accountFlowModelList = new List<AccountFlowViewModel>();
                    bool IsUpdate = true;
                    FutureTransaction futureTransaction = dbContext.FutureTransactions.SingleOrDefault(x => x.RowKey == FutureTransactionModel.RowKey);
                    FutureTransactionModel.AccountHeadName = dbContext.AccountHeads.Where(x => x.RowKey == FutureTransactionModel.AccountHeadKey).Select(x => x.AccountHeadName).FirstOrDefault();
                    if (futureTransaction != null)
                    {
                        if (futureTransaction.AmountPaid != futureTransaction.TotalAmount && (futureTransaction.AccountHead.AccountHeadType.AccountGroupKey == DbConstants.AccountGroup.Income || futureTransaction.AccountHead.AccountHeadType.AccountGroupKey == DbConstants.AccountGroup.Expenses))
                        {
                            accountFlowModelList = PayableOtherAmountList(FutureTransactionOtherAmountTypeModel, FutureTransactionModel, accountFlowModelList, IsUpdate);
                        }
                        else if (futureTransaction.AccountHead.AccountHeadType.AccountGroupKey != DbConstants.AccountGroup.Income && futureTransaction.AccountHead.AccountHeadType.AccountGroupKey != DbConstants.AccountGroup.Expenses)
                        {
                            accountFlowModelList = OtherAmountList(FutureTransactionOtherAmountTypeModel, FutureTransactionModel, accountFlowModelList, IsUpdate);
                        }
                    }
                    else
                    {
                        var accountHeadList = dbContext.AccountHeads.SingleOrDefault(x => x.RowKey == FutureTransactionModel.AccountHeadKey);
                        if (FutureTransactionModel.AmountPaid != FutureTransactionModel.TotalAmount && (accountHeadList.AccountHeadType.AccountGroupKey == DbConstants.AccountGroup.Income || accountHeadList.AccountHeadType.AccountGroupKey == DbConstants.AccountGroup.Expenses))
                        {
                            accountFlowModelList = PayableOtherAmountList(FutureTransactionOtherAmountTypeModel, FutureTransactionModel, accountFlowModelList, IsUpdate);
                        }
                        else if (accountHeadList.AccountHeadType.AccountGroupKey != DbConstants.AccountGroup.Income && accountHeadList.AccountHeadType.AccountGroupKey != DbConstants.AccountGroup.Expenses)
                        {
                            accountFlowModelList = OtherAmountList(FutureTransactionOtherAmountTypeModel, FutureTransactionModel, accountFlowModelList, IsUpdate);
                        }
                    }
                    accountFlowModelList = CreditOtherAmountList(FutureTransactionOtherAmountTypeModel, FutureTransactionModel, accountFlowModelList, IsUpdate);
                    //accountFlowModelList = DebitOtherAmountList(FutureTransactionOtherAmountTypeModel, FutureTransactionModel, accountFlowModelList, IsUpdate);
                    CreateAccountFlow(accountFlowModelList, IsUpdate);

                    //if (modelItem.RefKey == null)
                    //    modelItem.RefKey = FutureTransactionOtherAmountTypeModel.RefKey;
                    //modelItem.UpdateType = DbConstants.UpdationType.Update;
                }
            }
        }

        public FutureTransactionViewModel DeleteFutureTransactionOtherAmountTypeItem(FutureTransactionOtherAmountTypeViewModel objViewModel)
        {
            FutureTransactionViewModel FutureTransactionViewModel = new FutureTransactionViewModel();

            using (var transaction = dbContext.Database.BeginTransaction())
            {
                try
                {
                    FutureTransactionOtherAmountType FutureTransactionOtherAmountType = dbContext.FutureTransactionOtherAmountTypes.SingleOrDefault(row => row.RowKey == objViewModel.RowKey);
                    FutureTransaction FutureTransactions = dbContext.FutureTransactions.SingleOrDefault(row => row.RowKey == FutureTransactionOtherAmountType.FutureTransactionKey);
                    // objViewModel.RefKey = FutureTransactionOtherAmountType.RefKey;
                    if (FutureTransactionOtherAmountType.IsAddition == true)
                    {
                        FutureTransactions.TotalAmount = FutureTransactions.TotalAmount - FutureTransactionOtherAmountType.Amount;
                    }
                    else
                    {
                        FutureTransactions.TotalAmount = FutureTransactions.TotalAmount + FutureTransactionOtherAmountType.Amount;
                    }
                    List<AccountFlow> AccountList = dbContext.AccountFlows.Where(row => row.TransactionKey == objViewModel.RowKey && row.TransactionTypeKey == DbConstants.TransactionType.FutureTransactionOther).ToList();
                    AccountList.ForEach(row => dbContext.AccountFlows.Remove(row));

                    dbContext.FutureTransactionOtherAmountTypes.Remove(FutureTransactionOtherAmountType);

                    dbContext.SaveChanges();
                    transaction.Commit();
                    FutureTransactionViewModel.Message = EduSuiteUIResources.Success;
                    FutureTransactionViewModel.IsSuccessful = true;
                }
                catch (DbUpdateException ex)
                {
                    transaction.Rollback();
                    if (ex.GetBaseException().Message.ToUpper().Contains(EduSuiteUIResources.ForeignKeyError.ToUpper()))
                    {
                        FutureTransactionViewModel.Message = ex.GetBaseException().Message;
                        FutureTransactionViewModel.Message = String.Format(EduSuiteUIResources.FailedToDeleteErrorMessage, EduSuiteUIResources.FutureTransactionsAmountType);
                        FutureTransactionViewModel.IsSuccessful = false;
                    }
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    FutureTransactionViewModel.Message = ex.GetBaseException().Message;
                    FutureTransactionViewModel.Message = String.Format(EduSuiteUIResources.FailedToDeleteErrorMessage, EduSuiteUIResources.FutureTransactionsAmountType);
                    FutureTransactionViewModel.IsSuccessful = false;
                }
            }
            //if (!objViewModel.IsSecond && FutureTransactionViewModel.IsSuccessful)
            //{
            //    objViewModel.UpdateType = DbConstants.UpdationType.Delete;
            //    TriggerDatabaseInBackgroundOtherChargeItem(objViewModel);
            //}
            return FutureTransactionViewModel;
        }


        #endregion

        #region FutureTransaction Payment Other Charges
        public void FillFutureTransactionOtherAmountType(FutureTransactionPaymentViewModel model)
        {
            model.FutureTransactionOtherAmountTypes = (from row in dbContext.FutureTransactionOtherAmountTypes.Where(x => x.FutureTransactionKey == model.PaymentKey)
                                                       select new FutureTransactionOtherAmountTypeViewModel
                                                       {
                                                           RowKey = row.RowKey,
                                                           AccountHeadKey = row.AccountHeadKey,
                                                           Amount = row.Amount,
                                                           AmountPer = row.AmountPer,
                                                           FutureTransactionKey = row.FutureTransactionKey,
                                                           FutureTransactionPaymentKey = row.FutureTransactionPaymentKey,
                                                           IsAddition = row.IsAddition
                                                       }).ToList();
            if (model.FutureTransactionOtherAmountTypes.Count == 0)
            {
                model.FutureTransactionOtherAmountTypes.Add(new FutureTransactionOtherAmountTypeViewModel
                {
                    RowKey = 0,
                });
            }
            foreach (FutureTransactionOtherAmountTypeViewModel modelItem in model.FutureTransactionOtherAmountTypes)
            {
                FillAccountHeadsList(modelItem, model.AccountHeadKey ?? 0);
            }

        }

        private void CreateFutureTransactionOtherAmountType(List<FutureTransactionOtherAmountTypeViewModel> listModel, FutureTransactionPaymentViewModel FutureTransactionModel, long SalesOrderMasterKey)
        {
            int i = 0, j = 0;

            long maxKey = dbContext.FutureTransactionOtherAmountTypes.Select(P => P.RowKey).DefaultIfEmpty().Max();
            maxKey = maxKey + 1;
            foreach (FutureTransactionOtherAmountTypeViewModel modelItem in listModel)
            {
                FutureTransactionOtherAmountType FutureTransactionOtherAmountTypeModel = new FutureTransactionOtherAmountType();
                if (modelItem.AccountHeadKey != null && modelItem.Amount != null)
                {
                    FutureTransactionOtherAmountTypeModel.RowKey = Convert.ToInt64(maxKey + j);
                    FutureTransactionOtherAmountTypeModel.FutureTransactionPaymentKey = SalesOrderMasterKey;
                    FutureTransactionOtherAmountTypeModel.AccountHeadKey = modelItem.AccountHeadKey ?? 0;
                    FutureTransactionOtherAmountTypeModel.Amount = Convert.ToDecimal(modelItem.Amount);
                    FutureTransactionOtherAmountTypeModel.AmountPer = Convert.ToDecimal(modelItem.AmountPer);
                    FutureTransactionOtherAmountTypeModel.IsAddition = modelItem.IsAddition;
                    FutureTransactionOtherAmountTypeModel.AccountHead = dbContext.AccountHeads.SingleOrDefault(x => x.RowKey == modelItem.AccountHeadKey);

                    dbContext.FutureTransactionOtherAmountTypes.Add(FutureTransactionOtherAmountTypeModel);
                    i++; j++;


                    List<AccountFlowViewModel> accountFlowModelList = new List<AccountFlowViewModel>();
                    bool IsUpdate = false;
                    var accountHeadList = dbContext.AccountHeads.SingleOrDefault(x => x.RowKey == FutureTransactionModel.AccountHeadKey);
                    string AccountHeadName = accountHeadList.AccountHeadName;
                    if (FutureTransactionModel.PaidAmount != FutureTransactionModel.TotalAmount && (accountHeadList.AccountHeadType.AccountGroupKey == DbConstants.AccountGroup.Income || accountHeadList.AccountHeadType.AccountGroupKey == DbConstants.AccountGroup.Expenses))
                    {
                        accountFlowModelList = PayableOtherPaymentAmountList(FutureTransactionOtherAmountTypeModel, FutureTransactionModel, accountFlowModelList, IsUpdate, AccountHeadName);
                    }
                    else if (accountHeadList.AccountHeadType.AccountGroupKey != DbConstants.AccountGroup.Income && accountHeadList.AccountHeadType.AccountGroupKey != DbConstants.AccountGroup.Expenses)
                    {
                        accountFlowModelList = OtherPaymentAmountList(FutureTransactionOtherAmountTypeModel, FutureTransactionModel, accountFlowModelList, IsUpdate, AccountHeadName);
                    }
                    accountFlowModelList = CreditOtherPaymentAmountList(FutureTransactionOtherAmountTypeModel, FutureTransactionModel, accountFlowModelList, IsUpdate, AccountHeadName);
                    //accountFlowModelList = DebitOtherAmountList(FutureTransactionOtherAmountTypeModel, FutureTransactionModel, accountFlowModelList, IsUpdate);
                    CreateAccountFlow(accountFlowModelList, IsUpdate);

                    modelItem.RowKey = FutureTransactionOtherAmountTypeModel.RowKey;
                    // modelItem.UpdateType = DbConstants.UpdationType.Create;
                }
            }


        }

        private void UpdateFutureTransactionOtherAmountType(List<FutureTransactionOtherAmountTypeViewModel> listModel, FutureTransactionPaymentViewModel FutureTransactionModel, long SalesOrderMasterKey)
        {
            FutureTransactionOtherAmountType FutureTransactionOtherAmountTypeModel = new FutureTransactionOtherAmountType();
            foreach (FutureTransactionOtherAmountTypeViewModel modelItem in listModel)
            {
                if (modelItem.AccountHeadKey != null && modelItem.Amount != null)
                {
                    FutureTransactionOtherAmountTypeModel = dbContext.FutureTransactionOtherAmountTypes.SingleOrDefault(row => row.RowKey == modelItem.RowKey);
                    FutureTransactionOtherAmountTypeModel.FutureTransactionKey = SalesOrderMasterKey;
                    FutureTransactionOtherAmountTypeModel.AccountHeadKey = modelItem.AccountHeadKey ?? 0;
                    FutureTransactionOtherAmountTypeModel.Amount = Convert.ToDecimal(modelItem.Amount);
                    FutureTransactionOtherAmountTypeModel.AmountPer = Convert.ToDecimal(modelItem.AmountPer);
                    FutureTransactionOtherAmountTypeModel.IsAddition = modelItem.IsAddition;
                    FutureTransactionOtherAmountTypeModel.AccountHead = dbContext.AccountHeads.SingleOrDefault(x => x.RowKey == modelItem.AccountHeadKey);


                    List<AccountFlowViewModel> accountFlowModelList = new List<AccountFlowViewModel>();
                    bool IsUpdate = true;
                    var accountHeadList = dbContext.AccountHeads.SingleOrDefault(x => x.RowKey == FutureTransactionModel.AccountHeadKey);
                    string AccountHeadName = accountHeadList.AccountHeadName;
                    if (FutureTransactionModel.PaidAmount != FutureTransactionModel.TotalAmount && (accountHeadList.AccountHeadType.AccountGroupKey == DbConstants.AccountGroup.Income || accountHeadList.AccountHeadType.AccountGroupKey == DbConstants.AccountGroup.Expenses))
                    {
                        accountFlowModelList = PayableOtherPaymentAmountList(FutureTransactionOtherAmountTypeModel, FutureTransactionModel, accountFlowModelList, IsUpdate, AccountHeadName);
                    }
                    else if (accountHeadList.AccountHeadType.AccountGroupKey != DbConstants.AccountGroup.Income && accountHeadList.AccountHeadType.AccountGroupKey != DbConstants.AccountGroup.Expenses)
                    {
                        accountFlowModelList = OtherPaymentAmountList(FutureTransactionOtherAmountTypeModel, FutureTransactionModel, accountFlowModelList, IsUpdate, AccountHeadName);
                    }
                    accountFlowModelList = CreditOtherPaymentAmountList(FutureTransactionOtherAmountTypeModel, FutureTransactionModel, accountFlowModelList, IsUpdate, AccountHeadName);
                    //accountFlowModelList = DebitOtherAmountList(FutureTransactionOtherAmountTypeModel, FutureTransactionModel, accountFlowModelList, IsUpdate);
                    CreateAccountFlow(accountFlowModelList, IsUpdate);

                    //if (modelItem.RefKey == null)
                    //    modelItem.RefKey = FutureTransactionOtherAmountTypeModel.RefKey;
                    //modelItem.UpdateType = DbConstants.UpdationType.Update;
                }
            }
        }



        #endregion
    }
}
