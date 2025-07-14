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
    public class AssetPurchaseService : IAssetPurchaseService
    {
        EduSuiteDatabase dbContext;
        public AssetPurchaseService(EduSuiteDatabase objDB)
        {
            this.dbContext = objDB;
        }

        #region AssetPurchaseMaster
        public List<AssetPurchaseMasterViewModel> GetAssetPurchaseMaster(AssetPurchaseMasterViewModel model, string searchText)
        {
            try
            {
                var Take = model.PageSize;
                var Skip = (model.PageIndex - 1) * model.PageSize;
                IQueryable<AssetPurchaseMasterViewModel> AssetPurchaseMasterList = (from op in dbContext.AssetPurchaseMasters
                                                                                    where (
                                                                                    (model.BranchKey != 0) ?
                                                                                    op.BranchKey == model.BranchKey &&
                                                                                    (op.OrderNumber.Contains(searchText) || op.Party.PartyName.Contains(searchText) || (op.Party.MobileNumber2 ?? "").Contains(searchText) || (op.Party.MobileNumber1 ?? "").Contains(searchText)) :
                                                                                    (op.OrderNumber.Contains(searchText) || op.Party.PartyName.Contains(searchText) || (op.Party.MobileNumber2 ?? "").Contains(searchText) || (op.Party.MobileNumber1 ?? "").Contains(searchText))
                                                                                    )
                                                                                    orderby op.RowKey descending
                                                                                    select new AssetPurchaseMasterViewModel
                                                                                    {
                                                                                        RowKey = op.RowKey,
                                                                                        PartyName = op.Party.PartyName,
                                                                                        BillNo = op.BillNo,
                                                                                        BillDate = op.BillDate,
                                                                                        TotalAmount = op.TotalAmount,
                                                                                        AmountPaid = op.AssetPurchasePayments.Where(x => x.ChequeStatusKey != DbConstants.ProcessStatus.Rejected).Sum(SR => SR.PaidAmount) + (op.OldAdvanceBalance ?? 0),
                                                                                        BalanceAmount = op.TotalAmount - (op.AssetPurchasePayments.Where(x => x.ChequeStatusKey != DbConstants.ProcessStatus.Rejected).Sum(SR => SR.PaidAmount) + (op.OldAdvanceBalance ?? 0)),
                                                                                        OrderNumber = op.OrderNumber
                                                                                    });

                if (model.SortBy != "")
                {
                    AssetPurchaseMasterList = SortSalesOrder(AssetPurchaseMasterList, model.SortBy, model.SortOrder);
                }
                model.TotalRecords = AssetPurchaseMasterList.Count();
                return model.PageSize != 0 ? AssetPurchaseMasterList.Skip(Skip).Take(Take).ToList() : AssetPurchaseMasterList.ToList();

            }
            catch (Exception ex)
            {
                ActivityLog.CreateActivityLog(MenuConstants.AssetPurchase, ActionConstants.MenuAccess, DbConstants.LogType.Error, DbConstants.User.UserKey, ex.GetBaseException().Message);
                return new List<AssetPurchaseMasterViewModel>();
            }
        }
        private IQueryable<AssetPurchaseMasterViewModel> SortSalesOrder(IQueryable<AssetPurchaseMasterViewModel> Query, string SortName, string SortOrder)
        {

            string command = SortOrder == "desc" ? "OrderByDescending" : "OrderBy";
            var type = typeof(AssetPurchaseMasterViewModel);
            var property = type.GetProperty(SortName);
            var parameter = Expression.Parameter(type, "p");
            var propertyAccess = Expression.MakeMemberAccess(parameter, property);
            var orderByExpression = Expression.Lambda(propertyAccess, parameter);
            var resultExpression = Expression.Call(typeof(Queryable), command, new Type[] { type, property.PropertyType },
                                          Query.Expression, Expression.Quote(orderByExpression));
            return Query.Provider.CreateQuery<AssetPurchaseMasterViewModel>(resultExpression);

        }
        public AssetPurchaseMasterViewModel GetAssetPurchaseMasterById(AssetPurchaseMasterViewModel model)
        {
            try
            {
                AssetPurchaseMasterViewModel objViewModel = new AssetPurchaseMasterViewModel();

                objViewModel = (from row in dbContext.AssetPurchaseMasters
                                join r in dbContext.AssetPurchasePayments.Where(r => r.IsAdavance == true)
                                on row.RowKey equals r.PurchaseOrderMasterKey into joined
                                from j in joined.DefaultIfEmpty()
                                select new AssetPurchaseMasterViewModel
                                {
                                    RowKey = row.RowKey,
                                    BranchKey = row.BranchKey,
                                    PartyKey = row.PartyKey,
                                    BillDate = row.BillDate,
                                    BillNo = row.BillNo,
                                    TotalAmount = row.TotalAmount,
                                    OrderNumber = row.OrderNumber,
                                    AmountPaid = row.AmountPaid,
                                    AssetPurchasePaymentRowKey = j.RowKey != null ? j.RowKey : 0,
                                    PaymentModeKey = j.PaymentModeKey != null ? j.PaymentModeKey : DbConstants.PaymentMode.Cash,
                                    CardNumber = j.CardNumber,
                                    BankAccountKey = j.BankAccountKey,
                                    ChequeOrDDNumber = j.ChequeOrDDNumber,
                                    ChequeOrDDDate = j.ChequeOrDDDate,
                                    TaxableAmount = row.TaxableAmount,
                                    NonTaxableAmount = row.NonTaxableAmount,
                                    CGSTAmt = row.CGSTAmt,
                                    SGSTAmt = row.SGSTAmt,
                                    IGSTAmt = row.IGSTAmt,
                                    Discount = row.Discount,
                                    RoundOff = row.RoundOff,
                                    SubTotal = row.SubTotal,
                                    OldAdvanceBalance = row.OldAdvanceBalanceTotal,
                                }).Where(x => x.RowKey == model.RowKey).FirstOrDefault();
                if (objViewModel == null)
                {
                    objViewModel = new AssetPurchaseMasterViewModel();
                    objViewModel.CGSTAmt = 0;
                    objViewModel.SGSTAmt = 0;
                    objViewModel.OrderNumber = EduSuiteUIResources.AP + Convert.ToString(dbContext.AssetPurchaseMasters.Select(p => p.RowKey).DefaultIfEmpty().Max() + 1);
                    objViewModel.PaymentModeKey = DbConstants.PaymentMode.Cash;
                }
                
                FillPurchaseDetails(objViewModel);
                FillPurchaseMasterDropdowns(objViewModel);
                FillTransactionPaymentDropdownLists(objViewModel);
                if (objViewModel.BranchKey != null && objViewModel.BranchKey != 0)
                {
                    objViewModel.StateKey = 1;
                }
                return objViewModel;
            }
            catch (Exception ex)
            {
                ActivityLog.CreateActivityLog(MenuConstants.AssetPurchase, ((model.RowKey) != 0 ? ActionConstants.Edit : ActionConstants.Add), DbConstants.LogType.Error, model.RowKey, ex.GetBaseException().Message);
                return new AssetPurchaseMasterViewModel();
            }
        }
        public AssetPurchaseMasterViewModel CreateAssetPurchaseMaster(AssetPurchaseMasterViewModel model)
        {
            AssetPurchaseMaster orderMasterModel = new AssetPurchaseMaster();

            using (var transaction = dbContext.Database.BeginTransaction())
            {
                try
                {
                    long MaxKey = dbContext.AssetPurchaseMasters.Select(p => p.RowKey).DefaultIfEmpty().Max();
                    orderMasterModel.RowKey = Convert.ToInt64(MaxKey + 1);
                    long oldPartyKey = orderMasterModel.PartyKey;
                    string partyName = dbContext.Parties.Where(x => x.RowKey == model.PartyKey).Select(x => x.PartyName).FirstOrDefault();
                    orderMasterModel.OrderNumber = EduSuiteUIResources.AP + (MaxKey + 1);
                    orderMasterModel.PartyKey = model.PartyKey;
                    orderMasterModel.BillDate = model.BillDate;
                    orderMasterModel.BillNo = model.BillNo;
                    orderMasterModel.TotalAmount = Convert.ToDecimal(model.TotalAmount ?? 0);
                    orderMasterModel.AmountPaid = Convert.ToDecimal(model.AmountPaid ?? 0);
                    orderMasterModel.TaxableAmount = model.TaxableAmount;
                    orderMasterModel.NonTaxableAmount = model.NonTaxableAmount;
                    orderMasterModel.Discount = model.Discount;
                    orderMasterModel.RoundOff = model.RoundOff;
                    orderMasterModel.SubTotal = model.SubTotal;
                    orderMasterModel.OldAdvanceBalanceTotal = model.OldAdvanceBalance;
                    decimal createAdvanceBalance = orderMasterModel.OldAdvanceBalance ?? 0;
                    orderMasterModel.OldAdvanceBalance = (model.OldAdvanceBalance ?? 0) > (model.TotalAmount ?? 0) ? model.TotalAmount : model.OldAdvanceBalance;
                    model.OldAdvanceBalance = orderMasterModel.OldAdvanceBalance;
                    if (model.StateKey == model.PartyStateKey)
                    {
                        orderMasterModel.CGSTAmt = model.CGSTAmt;
                        orderMasterModel.SGSTAmt = model.SGSTAmt;
                    }
                    else
                    {
                        orderMasterModel.IGSTAmt = model.IGSTAmt;
                    }
                    orderMasterModel.BranchKey = model.BranchKey;
                    dbContext.AssetPurchaseMasters.Add(orderMasterModel);


                    model.RowKey = orderMasterModel.RowKey;
                    CreateAssetPurchaseDetails(model.AssetPurchaseDetails.Where(row => row.RowKey == 0 || row.RowKey == null).ToList(), model, orderMasterModel.RowKey, orderMasterModel.OrderNumber, orderMasterModel.BillDate, partyName, model.BranchKey);

                    if (model.AmountPaid != null && model.AmountPaid != 0)
                    {
                        CreateAssetPurchasePayment(model, orderMasterModel.RowKey, oldPartyKey, createAdvanceBalance);
                    }
                    short? branchKey = model.BranchKey;
                    List<AccountFlowViewModel> AccountFlowList = new List<AccountFlowViewModel>();
                    bool IsUpdate = false;
                    AccountFlowList = CreditTotalAmountList(orderMasterModel, AccountFlowList, IsUpdate, branchKey);
                    //AccountFlowList = DebitTotalAmountList(orderMasterModel, AccountFlowList, IsUpdate, branchKey);
                    if (model.RoundOff != null && model.RoundOff != 0)
                    {
                        AccountFlowList = RoundOffAmountList(orderMasterModel, AccountFlowList, IsUpdate, branchKey);
                    }
                    if (model.Discount != null && model.Discount != 0)
                    {
                        AccountFlowList = DiscountAmountList(orderMasterModel, AccountFlowList, IsUpdate, branchKey);
                    }
                    if (model.StateKey == model.PartyStateKey)
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
                    CreateAccountFlow(AccountFlowList, IsUpdate);
                    model.RowKey = orderMasterModel.RowKey;


                    dbContext.SaveChanges();
                    transaction.Commit();
                    model.Message = EduSuiteUIResources.Success;
                    model.IsSuccessful = true;
                    ActivityLog.CreateActivityLog(MenuConstants.AssetPurchase, ActionConstants.Add, DbConstants.LogType.Info, orderMasterModel.RowKey, model.Message);

                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    model.Message = String.Format(EduSuiteUIResources.FailedToSaveErrorMessage, EduSuiteUIResources.AssetPurchase);
                    model.IsSuccessful = false;
                    ActivityLog.CreateActivityLog(MenuConstants.AssetPurchase, ActionConstants.Add, DbConstants.LogType.Error, model.RowKey, ex.GetBaseException().Message);
                }
            }
            return model;
        }
        public AssetPurchaseMasterViewModel UpdateAssetPurchaseMaster(AssetPurchaseMasterViewModel model)
        {
            AssetPurchaseMaster AssetPurchaseMasterModel = new AssetPurchaseMaster();
            using (var transaction = dbContext.Database.BeginTransaction())
            {
                try
                {
                    AssetPurchaseMasterModel = dbContext.AssetPurchaseMasters.SingleOrDefault(row => row.RowKey == model.RowKey);
                    string partyName = dbContext.Parties.Where(x => x.RowKey == model.PartyKey).Select(x => x.PartyName).FirstOrDefault();
                    long oldPartyKey = AssetPurchaseMasterModel.PartyKey;
                    AssetPurchaseMasterModel.PartyKey = model.PartyKey;
                    AssetPurchaseMasterModel.BillDate = model.BillDate;
                    AssetPurchaseMasterModel.BillNo = model.BillNo;
                    AssetPurchaseMasterModel.TotalAmount = Convert.ToDecimal(model.TotalAmount ?? 0);
                    AssetPurchaseMasterModel.AmountPaid = Convert.ToDecimal(model.AmountPaid ?? 0);
                    AssetPurchaseMasterModel.TaxableAmount = model.TaxableAmount;
                    AssetPurchaseMasterModel.NonTaxableAmount = model.NonTaxableAmount;
                    decimal oldCGST = AssetPurchaseMasterModel.CGSTAmt ?? 0;
                    decimal oldSGST = AssetPurchaseMasterModel.SGSTAmt ?? 0;
                    decimal oldIGST = AssetPurchaseMasterModel.IGSTAmt ?? 0;
                    AssetPurchaseMasterModel.Discount = model.Discount;
                    AssetPurchaseMasterModel.RoundOff = model.RoundOff;
                    AssetPurchaseMasterModel.SubTotal = model.SubTotal;
                    AssetPurchaseMasterModel.OldAdvanceBalanceTotal = model.OldAdvanceBalance;
                    decimal createAdvanceBalance = AssetPurchaseMasterModel.OldAdvanceBalance ?? 0;
                    AssetPurchaseMasterModel.OldAdvanceBalance = (model.OldAdvanceBalance ?? 0) > model.TotalAmount ? model.TotalAmount : (model.OldAdvanceBalance ?? 0);
                    model.OldAdvanceBalance = AssetPurchaseMasterModel.OldAdvanceBalance;
                    if (model.StateKey == model.PartyStateKey)
                    {
                        AssetPurchaseMasterModel.CGSTAmt = model.CGSTAmt;
                        AssetPurchaseMasterModel.SGSTAmt = model.SGSTAmt;
                    }
                    else
                    {
                        AssetPurchaseMasterModel.IGSTAmt = model.IGSTAmt;
                    }

                    UpdateAssetPurchaseDetails(model.AssetPurchaseDetails.Where(row => row.RowKey != 0 && row.RowKey != null).ToList(), model, model.RowKey, AssetPurchaseMasterModel.OrderNumber, AssetPurchaseMasterModel.BillDate, partyName, model.BranchKey);
                    CreateAssetPurchaseDetails(model.AssetPurchaseDetails.Where(row => row.RowKey == 0 || row.RowKey == null).ToList(), model, model.RowKey, AssetPurchaseMasterModel.OrderNumber, AssetPurchaseMasterModel.BillDate, partyName, model.BranchKey);

                    if (model.AmountPaid != null && model.AmountPaid != 0)
                    {
                        CreateAssetPurchasePayment(model, AssetPurchaseMasterModel.RowKey, oldPartyKey, createAdvanceBalance);
                    }
                    short? branchKey = model.BranchKey;
                    List<AccountFlowViewModel> AccountFlowList = new List<AccountFlowViewModel>();
                    bool IsUpdate = true;

                    if (dbContext.AccountFlows.Where(x => x.TransactionTypeKey == DbConstants.TransactionType.RoundOffPurchase && x.TransactionKey == AssetPurchaseMasterModel.RowKey).Any())
                    {
                        AccountFlowList = RoundOffAmountList(AssetPurchaseMasterModel, AccountFlowList, IsUpdate, branchKey);
                        CreateAccountFlow(AccountFlowList, IsUpdate);
                    }
                    else
                    {
                        AccountFlowList = new List<AccountFlowViewModel>();
                        AccountFlowList = RoundOffAmountList(AssetPurchaseMasterModel, AccountFlowList, false, branchKey);
                        CreateAccountFlow(AccountFlowList, false);
                    }
                    if (dbContext.AccountFlows.Where(x => x.TransactionTypeKey == DbConstants.TransactionType.DiscountPurchase && x.TransactionKey == AssetPurchaseMasterModel.RowKey).Any())
                    {
                        AccountFlowList = new List<AccountFlowViewModel>();
                        AccountFlowList = DiscountAmountList(AssetPurchaseMasterModel, AccountFlowList, IsUpdate, branchKey);
                        CreateAccountFlow(AccountFlowList, IsUpdate);
                    }
                    else
                    {
                        AccountFlowList = new List<AccountFlowViewModel>();
                        AccountFlowList = DiscountAmountList(AssetPurchaseMasterModel, AccountFlowList, false, branchKey);
                        CreateAccountFlow(AccountFlowList, false);
                    }

                    AccountFlowList = new List<AccountFlowViewModel>();
                    AccountFlowList = CreditTotalAmountList(AssetPurchaseMasterModel, AccountFlowList, IsUpdate, branchKey);
                    if (model.StateKey == model.PartyStateKey)
                    {
                        if (oldCGST != null && oldCGST != 0)
                        {
                            AccountFlowList = CreditCGSTList(AssetPurchaseMasterModel, AccountFlowList, IsUpdate, branchKey);
                        }
                        else
                        {
                            if (model.CGSTAmt != null && model.CGSTAmt != 0)
                            {
                                AccountFlowList = CreditCGSTList(AssetPurchaseMasterModel, AccountFlowList, IsUpdate, branchKey);
                            }
                        }
                        if (oldSGST != null && oldSGST != 0)
                        {
                            AccountFlowList = CreditSGSTList(AssetPurchaseMasterModel, AccountFlowList, IsUpdate, branchKey);
                        }
                        else
                        {
                            if (model.SGSTAmt != null && model.SGSTAmt != 0)
                            {
                                AccountFlowList = CreditSGSTList(AssetPurchaseMasterModel, AccountFlowList, IsUpdate, branchKey);
                            }
                        }
                    }
                    else
                    {
                        if (oldIGST != null && oldIGST != 0)
                        {
                            AccountFlowList = CreditIGSTList(AssetPurchaseMasterModel, AccountFlowList, IsUpdate, branchKey);
                        }
                        else
                        {
                            if (model.IGSTAmt != null && model.IGSTAmt != 0)
                            {
                                AccountFlowList = CreditIGSTList(AssetPurchaseMasterModel, AccountFlowList, IsUpdate, branchKey);
                            }
                        }
                    }
                    CreateAccountFlow(AccountFlowList, IsUpdate);
                    dbContext.SaveChanges();
                    transaction.Commit();
                    model.Message = EduSuiteUIResources.Success;
                    model.IsSuccessful = true;
                    ActivityLog.CreateActivityLog(MenuConstants.AssetPurchase, ActionConstants.Edit, DbConstants.LogType.Info, model.RowKey, model.Message);
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    model.Message =String.Format(EduSuiteUIResources.FailedToSaveErrorMessage, EduSuiteUIResources.AssetPurchase);
                    model.IsSuccessful = false;
                    ActivityLog.CreateActivityLog(MenuConstants.AssetPurchase, ActionConstants.Edit, DbConstants.LogType.Error, model.RowKey, ex.GetBaseException().Message);
                }
            }
            return model;

        }
        public AssetPurchaseMasterViewModel DeleteAssetPurchaseMaster(AssetPurchaseMasterViewModel model)
        {
            using (var transaction = dbContext.Database.BeginTransaction())
            {
                try
                {
                    AssetPurchaseMaster AssetPurchaseMaster = dbContext.AssetPurchaseMasters.SingleOrDefault(row => row.RowKey == model.RowKey);
                    dbContext.AssetPurchaseMasters.Remove(AssetPurchaseMaster);
                    //Delete Order Details and Order advance Details
                    dbContext.SaveChanges();
                    transaction.Commit();
                    model.Message = EduSuiteUIResources.Success;
                    model.IsSuccessful = true;
                    ActivityLog.CreateActivityLog(MenuConstants.AssetPurchase, ActionConstants.Delete, DbConstants.LogType.Info, model.RowKey, model.Message);
                }
                catch (DbUpdateException ex)
                {
                    transaction.Rollback();
                    if (ex.GetBaseException().Message.ToUpper().Contains(EduSuiteUIResources.ForeignKeyError.ToUpper()))
                    {
                        model.Message = String.Format(EduSuiteUIResources.CantDeleteErrorMessage, EduSuiteUIResources.AssetPurchaseMaster);
                        model.IsSuccessful = false;
                        ActivityLog.CreateActivityLog(MenuConstants.AssetPurchase, ActionConstants.Delete, DbConstants.LogType.Error, model.RowKey, ex.GetBaseException().Message);
                    }
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    model.Message = String.Format(EduSuiteUIResources.FailedToDeleteErrorMessage, EduSuiteUIResources.AssetPurchaseMaster);
                    model.IsSuccessful = false;
                    ActivityLog.CreateActivityLog(MenuConstants.AssetPurchase, ActionConstants.Delete, DbConstants.LogType.Error, model.RowKey, ex.GetBaseException().Message);
                }
            }
            return model;
        }
        public void GetMaterialGSTByMaterialType(AssetPurchaseDetailsViewModel model)
        {
            AssetPurchaseDetailsViewModel objViewModel = new AssetPurchaseDetailsViewModel();
            objViewModel = (from row in dbContext.AssetTypes
                            where row.RowKey == model.AssetTypeKey
                            select new AssetPurchaseDetailsViewModel
                            {
                                CGSTPer = row.CGSTPer,
                                SGSTPer = row.SGSTPer,
                                IGSTPer = row.IGSTPer,
                                IsTax = row.IsTax,
                                DepreciationMethodKey = row.DepreciationMethodKey
                            }).FirstOrDefault();
            if (objViewModel != null)
            {
                model.CGSTPer = objViewModel.CGSTPer;
                model.SGSTPer = objViewModel.SGSTPer;
                model.IGSTPer = objViewModel.IGSTPer;
                model.IsTax = objViewModel.IsTax;
                model.DepreciationMethodKey = objViewModel.DepreciationMethodKey;
            }
        }
        public void FillRawMaterialsById(AssetPurchaseDetailsViewModel model)
        {
            model.AssetTypes = dbContext.AssetTypes.Select(row => new SelectListModel
            {
                RowKey = row.RowKey,
                Text = row.AssetTypeName
            }).ToList();
        }
        private void FillParties(AssetPurchaseMasterViewModel model)
        {
            model.Parties = dbContext.Parties.Where(r => r.PartyTypeKey == DbConstants.PartyType.Other && r.IsActive).Select(row => new SelectListModel
            {
                RowKey = row.RowKey,
                Text = row.PartyName
            }).ToList();
        }
        private void FillDepreciationMethod(AssetPurchaseDetailsViewModel model)
        {
            model.DepreciationMethods = dbContext.DepreciationMethods.Select(row => new SelectListModel
            {
                RowKey = row.RowKey,
                Text = row.MethodName
            }).ToList();
        }
        private void FillPurchaseMasterDropdowns(AssetPurchaseMasterViewModel model)
        {
            FillParties(model);
            FillBranches(model);
            GetPartyByPartyType(model);
        }
        public AssetPurchaseMasterViewModel FillBranches(AssetPurchaseMasterViewModel model)
        {
            if (model.BranchKey == 0)
            {
                model.BranchKey = dbContext.AppUsers.Where(row => row.RowKey == DbConstants.User.UserKey).Select(row => row.Employees.Select(x => x.BranchKey).FirstOrDefault()).FirstOrDefault();
                if (model.BranchKey == 0)
                {
                    model.Branches = dbContext.vwBranchSelectActiveOnlies.Select(row => new SelectListModel
                    {
                        RowKey = row.RowKey,
                        Text = row.BranchName
                    }).ToList();
                    if (model.Branches.Count == 1)
                    {
                        model.BranchKey = dbContext.vwBranchSelectActiveOnlies.Select(x => x.RowKey).FirstOrDefault();
                    }
                }
            }
            return model;
        }
        public AssetPurchaseMasterViewModel GetPartyByPartyType(AssetPurchaseMasterViewModel model)
        {
            model.Parties = dbContext.Parties.Where(row => row.PartyTypeKey == DbConstants.PartyType.Other && row.IsActive && row.CompanyBranchKey == model.BranchKey).Select(row => new SelectListModel
            {
                RowKey = row.RowKey,
                Text = row.PartyName
            }).ToList();
            if (model.BranchKey != null && model.BranchKey != 0)
            {
                model.StateKey = 1;
            }
            return model;
        }
        public AssetPurchaseMasterViewModel GetPartyDetailsByPartyKey(long? PartyKey)
        {
            AssetPurchaseMasterViewModel model = new AssetPurchaseMasterViewModel();
            FillPartyDetailsById(model, PartyKey);
            return model;
        }
        private void FillPartyDetailsById(AssetPurchaseMasterViewModel model, long? PartyKey)
        {
            try
            {
                model.PartyDetails_Order = dbContext.Parties.Where(row => row.RowKey == PartyKey).Select(p => new PartyViewModel
                {
                    RowKey = p.RowKey,
                    PartyName = p.PartyName,
                    MobileNumber1 = p.MobileNumber1,
                    MobileNumber2 = p.MobileNumber2,
                    Address = p.Address,
                    PartyTypeKey = p.PartyTypeKey,
                    PartyTypeName = p.PartyType.PartyTypeName,
                    LocationName = p.Location,
                    IsActive = p.IsActive,
                    GSTINNumber = p.GSTINNumber,
                    CreditBalance = p.CreditBalance,
                    CashFlowTypeKey = p.CashFlowTypeKey ?? 0,
                    StateKey = p.ProvinceKey,
                }).ToList();
                if (model.PartyDetails_Order == null)
                {
                    model.PartyDetails_Order = new List<PartyViewModel>();
                }
            }
            catch (Exception ex)
            {
            }
        }
        #endregion

        #region AssetPurchase Details
        private void CreateAssetPurchaseDetails(List<AssetPurchaseDetailsViewModel> listModel, AssetPurchaseMasterViewModel model, long AssetPurchaseMasterKey, string orderNumber, DateTime BillDate, string partyName, short BranchKey)
        {
            int i = 0, j = 0;

            long maxKey = dbContext.AssetPurchaseDetails.Select(P => P.RowKey).DefaultIfEmpty().Max();
            maxKey = maxKey + 1;
            foreach (AssetPurchaseDetailsViewModel modelItem in listModel)
            {
                AssetPurchaseDetail AssetPurchaseDetailModel = new AssetPurchaseDetail();

                AssetPurchaseDetailModel.RowKey = Convert.ToInt64(maxKey + j);
                AssetPurchaseDetailModel.OrderMasterKey = AssetPurchaseMasterKey;
                AssetPurchaseDetailModel.AssetTypeKey = modelItem.AssetTypeKey;
                AssetPurchaseDetailModel.Quantity = Convert.ToInt32(modelItem.Quantity ?? 0);
                AssetPurchaseDetailModel.Amount = Convert.ToDecimal(modelItem.Amount ?? 0);
                AssetPurchaseDetailModel.RateTypeUnitLength = modelItem.RateTypeUnitLength;
                AssetPurchaseDetailModel.RateTypeKey = modelItem.RateTypeKey;
                AssetPurchaseDetailModel.Description = modelItem.Description;
                AssetPurchaseDetailModel.ReferenceNumber = modelItem.ReferenceNumber;
                AssetPurchaseDetailModel.PeriodTypeKey = modelItem.PeriodTypeKey;
                AssetPurchaseDetailModel.DepreciationMethodKey = modelItem.DepreciationMethodKey;
                AssetPurchaseDetailModel.ProductionLimit = modelItem.ProductionLimit;
                AssetPurchaseDetailModel.LifePeriod = modelItem.LifePeriod ?? 0;
                AssetPurchaseDetailModel.RowTotal = Convert.ToDecimal(modelItem.RowTotal ?? 0);
                if (model.StateKey == model.PartyStateKey)
                {
                    AssetPurchaseDetailModel.CGSTPer = Convert.ToDecimal(modelItem.CGSTPer ?? 0);
                    AssetPurchaseDetailModel.SGSTPer = Convert.ToDecimal(modelItem.SGSTPer ?? 0);
                    AssetPurchaseDetailModel.CGSTAmt = Convert.ToDecimal(modelItem.CGSTAmt ?? 0);
                    AssetPurchaseDetailModel.SGSTAmt = Convert.ToDecimal(modelItem.SGSTAmt ?? 0);
                }
                else
                {
                    AssetPurchaseDetailModel.IGSTPer = Convert.ToDecimal(modelItem.IGSTPer ?? 0);
                    AssetPurchaseDetailModel.IGSTAmt = Convert.ToDecimal(modelItem.IGSTAmt ?? 0);
                }
                AssetPurchaseDetailModel.RowTotalGST = Convert.ToDecimal(modelItem.RowTotalGST ?? 0);

                dbContext.AssetPurchaseDetails.Add(AssetPurchaseDetailModel);
                

                AssetPurchaseDetailModel.AssetType = dbContext.AssetTypes.SingleOrDefault(x => x.RowKey == AssetPurchaseDetailModel.AssetTypeKey);

                i++; j++;
                List<AccountFlowViewModel> AccountFlowList = new List<AccountFlowViewModel>();
                bool IsUpdate = false;
                short branchKey = model.BranchKey;
                AccountFlowList = DebitMaterialList(AssetPurchaseDetailModel, AccountFlowList, IsUpdate, branchKey, orderNumber, BillDate, partyName);
                CreateAccountFlow(AccountFlowList, IsUpdate);
                model.RowKey = AssetPurchaseDetailModel.RowKey;
            }
            dbContext.SaveChanges();
        }
        private void UpdateAssetPurchaseDetails(List<AssetPurchaseDetailsViewModel> listModel, AssetPurchaseMasterViewModel model, long AssetPurchaseMasterKey, string orderNumber, DateTime BillDate, string partyName, short BranchKey)
        {
            AssetPurchaseDetail AssetPurchaseDetailModel = new AssetPurchaseDetail();
            foreach (AssetPurchaseDetailsViewModel modelItem in listModel)
            {
                AssetPurchaseDetailModel = dbContext.AssetPurchaseDetails.SingleOrDefault(row => row.RowKey == modelItem.RowKey);
                
                AssetPurchaseDetailModel.AssetTypeKey = modelItem.AssetTypeKey;
                AssetPurchaseDetailModel.Quantity = Convert.ToInt32(modelItem.Quantity ?? 0);
                AssetPurchaseDetailModel.Amount = Convert.ToDecimal(modelItem.Amount ?? 0);
                AssetPurchaseDetailModel.RateTypeKey = modelItem.RateTypeKey;
                AssetPurchaseDetailModel.RowTotal = Convert.ToDecimal(modelItem.RowTotal);
                AssetPurchaseDetailModel.RateTypeUnitLength = modelItem.RateTypeUnitLength;
                AssetPurchaseDetailModel.Description = modelItem.Description;
                AssetPurchaseDetailModel.ReferenceNumber = modelItem.ReferenceNumber;
                AssetPurchaseDetailModel.PeriodTypeKey = modelItem.PeriodTypeKey;
                AssetPurchaseDetailModel.DepreciationMethodKey = modelItem.DepreciationMethodKey;
                AssetPurchaseDetailModel.ProductionLimit = modelItem.ProductionLimit;
                AssetPurchaseDetailModel.LifePeriod = modelItem.LifePeriod ?? 0;
                if (model.StateKey == model.PartyStateKey)
                {
                    AssetPurchaseDetailModel.CGSTPer = Convert.ToDecimal(modelItem.CGSTPer ?? 0);
                    AssetPurchaseDetailModel.SGSTPer = Convert.ToDecimal(modelItem.SGSTPer ?? 0);
                    AssetPurchaseDetailModel.CGSTAmt = Convert.ToDecimal(modelItem.CGSTAmt ?? 0);
                    AssetPurchaseDetailModel.SGSTAmt = Convert.ToDecimal(modelItem.SGSTAmt ?? 0);
                }
                else
                {
                    AssetPurchaseDetailModel.IGSTPer = Convert.ToDecimal(modelItem.IGSTPer ?? 0);
                    AssetPurchaseDetailModel.IGSTAmt = Convert.ToDecimal(modelItem.IGSTAmt ?? 0);
                }
                AssetPurchaseDetailModel.RowTotalGST = Convert.ToDecimal(modelItem.RowTotalGST ?? 0);
                List<AccountFlowViewModel> AccountFlowList = new List<AccountFlowViewModel>();
                bool IsUpdate = true;
                short branchKey = model.BranchKey;
                AccountFlowList = DebitMaterialList(AssetPurchaseDetailModel, AccountFlowList, IsUpdate, branchKey, orderNumber, BillDate, partyName);
                CreateAccountFlow(AccountFlowList, IsUpdate);

            }
            dbContext.SaveChanges();
        }
        public AssetPurchaseMasterViewModel DeleteAssetPurchaseItem(AssetPurchaseDetailsViewModel objViewModel)
        {
            AssetPurchaseMasterViewModel AssetPurchaseMasterViewModel = new AssetPurchaseMasterViewModel();


            using (var transaction = dbContext.Database.BeginTransaction())
            {
                try
                {
                    AssetPurchaseDetail AssetPurchaseDetail = dbContext.AssetPurchaseDetails.SingleOrDefault(row => row.RowKey == objViewModel.RowKey);
                    AssetPurchaseMaster AssetPurchaseMaster = dbContext.AssetPurchaseMasters.SingleOrDefault(row => row.RowKey == AssetPurchaseDetail.OrderMasterKey);
                    AssetPurchaseMaster.TotalAmount = (AssetPurchaseMaster.TotalAmount - (AssetPurchaseDetail.Amount * AssetPurchaseDetail.Quantity));

                    dbContext.AssetPurchaseDetails.Remove(AssetPurchaseDetail);
                    dbContext.SaveChanges();
                    transaction.Commit();
                    AssetPurchaseMasterViewModel.Message = EduSuiteUIResources.Success;
                    AssetPurchaseMasterViewModel.IsSuccessful = true;
                    ActivityLog.CreateActivityLog(MenuConstants.AssetPurchase, ActionConstants.Delete, DbConstants.LogType.Info, objViewModel.RowKey, AssetPurchaseMasterViewModel.Message);
                }
                catch (DbUpdateException ex)
                {
                    transaction.Rollback();
                    if (ex.GetBaseException().Message.ToUpper().Contains(EduSuiteUIResources.ForeignKeyError.ToUpper()))
                    {
                        AssetPurchaseMasterViewModel.Message = String.Format(EduSuiteUIResources.CantDeleteErrorMessage, EduSuiteUIResources.AssetPurchaseItem);
                        AssetPurchaseMasterViewModel.IsSuccessful = false;
                    }
                    ActivityLog.CreateActivityLog(MenuConstants.AssetPurchase, ActionConstants.Delete, DbConstants.LogType.Error, objViewModel.RowKey, ex.GetBaseException().Message);
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    AssetPurchaseMasterViewModel.Message = String.Format(EduSuiteUIResources.FailedToDeleteErrorMessage, EduSuiteUIResources.AssetPurchaseItem);
                    AssetPurchaseMasterViewModel.IsSuccessful = false;
                    ActivityLog.CreateActivityLog(MenuConstants.AssetPurchase, ActionConstants.Delete, DbConstants.LogType.Error, objViewModel.RowKey, ex.GetBaseException().Message);
                }
            }
            return AssetPurchaseMasterViewModel;
        }
        private void FillPurchaseDetails(AssetPurchaseMasterViewModel model)
        {
            AssetPurchaseDetail AssetPurchaseDetailModel = new AssetPurchaseDetail();

            model.AssetPurchaseDetails = (from pod in dbContext.AssetPurchaseDetails.Where(x => x.OrderMasterKey == model.RowKey)

                                          select new AssetPurchaseDetailsViewModel
                                          {
                                              RowKey = pod.RowKey,
                                              AssetTypeKey = pod.AssetTypeKey,
                                              Quantity = pod.Quantity,
                                              Amount = pod.Amount,
                                              MaterialName = pod.AssetType.AssetTypeName,
                                              RateTypeKey = pod.RateTypeKey,
                                              RateTypeCode = pod.RateTypeKey == DbConstants.RateTypes.Numbers ? EduSuiteUIResources.Num : (pod.RateTypeKey == DbConstants.RateTypes.SqFeet ? EduSuiteUIResources.SqFeet : EduSuiteUIResources.SqMeter),
                                              RowTotal = pod.RowTotal,
                                              CGSTPer = pod.CGSTPer,
                                              SGSTPer = pod.SGSTPer,
                                              CGSTAmt = pod.CGSTAmt,
                                              SGSTAmt = pod.SGSTAmt,
                                              IGSTPer = pod.IGSTPer,
                                              IGSTAmt = pod.IGSTAmt,
                                              IsTax = pod.AssetType.IsTax,
                                              RowTotalGST = pod.RowTotalGST,
                                              RateTypeUnitLength = pod.RateTypeUnitLength,
                                              PeriodTypeKey = pod.PeriodTypeKey,
                                              LifePeriod = pod.LifePeriod,
                                              Description = pod.Description,
                                              DepreciationMethodKey = pod.DepreciationMethodKey,
                                              ProductionLimit = pod.ProductionLimit,
                                              ReferenceNumber = pod.ReferenceNumber,
                                              Period = pod.LifePeriod + EduSuiteUIResources.BlankSpace + pod.PeriodType.PeriodTypeName,
                                              IsUsed = dbContext.DepreciationDetails.Any(x => x.AssetDetailsKey == pod.RowKey)
                                          }).ToList();

            if (model.AssetPurchaseDetails.Count == 0)
            {
                model.AssetPurchaseDetails.Add(new AssetPurchaseDetailsViewModel
                {
                    RowKey = 0,
                    Quantity = null,
                    Amount = null,
                    RowTotal = 0,
                    CGSTPer = 0,
                    RowTotalGST = 0,
                    SGSTPer = 0,
                    IGSTPer = 0,
                    PeriodTypeKey = DbConstants.PeriodType.Year
                });
            }
            AssetPurchaseDetailsViewModel AssetPurchaseDetailsViewModel = new AssetPurchaseDetailsViewModel();
            //FillRawMaterials(AssetPurchaseDetailsViewModel);
            if (AssetPurchaseDetailsViewModel != null)
            {
                foreach (AssetPurchaseDetailsViewModel Objmodel in model.AssetPurchaseDetails)
                {

                    Objmodel.AssetTypes = AssetPurchaseDetailsViewModel.AssetTypes;
                    FillRateTypes(Objmodel);
                    FillRawMaterialsById(Objmodel);
                    FillPeriodType(Objmodel);
                    FillDepreciationMethod(Objmodel);
                }

            }
        }

        private void FillPeriodType(AssetPurchaseDetailsViewModel model)
        {
            model.PeriodType = dbContext.PeriodTypes.Select(row => new SelectListModel
            {
                RowKey = row.RowKey,
                Text = row.PeriodTypeName
            }).ToList();
        }
        #endregion

        #region Payment

        public PaymentWindowViewModel GetAssetPurchasePaymentById(long Id)
        {
            PaymentWindowViewModel model = new PaymentWindowViewModel();
            AssetPurchaseMaster PurchaaseOrderMaster = new AssetPurchaseMaster();
            PurchaaseOrderMaster = dbContext.AssetPurchaseMasters.SingleOrDefault(row => row.RowKey == Id);
            model = dbContext.AssetPurchasePayments.Where(x => x.PurchaseOrderMasterKey == Id && (x.ChequeStatusKey == null || x.ChequeStatusKey == DbConstants.ProcessStatus.Approved)).Select(row => new PaymentWindowViewModel
            {
                MasterKey = row.PurchaseOrderMasterKey,
                ReceivedBy = row.ReceivedBy

            }).FirstOrDefault();

            if (model != null)
            {
                string value = (dbContext.AssetPurchasePayments.Where(x => x.PurchaseOrderMasterKey == Id && (x.ChequeStatusKey == null || x.ChequeStatusKey == DbConstants.ProcessStatus.Approved)).Sum(row => row.PaidAmount).Value + PurchaaseOrderMaster.OldAdvanceBalance).ToString();
                model.TotalReceivedAmount = value != null && value != "" ? Convert.ToDecimal(value) : 0;
                model.TotalReceivedAmount = model.TotalReceivedAmount != null ? Convert.ToDecimal(model.TotalReceivedAmount) : 0;

                string AmountToPay = PurchaaseOrderMaster.TotalAmount.ToString();
                model.AmountToPay = AmountToPay != null && AmountToPay != "" ? Convert.ToDecimal(AmountToPay) : 0;
                model.OrderNumber = PurchaaseOrderMaster.OrderNumber;

                model.BalanceAmount = model.AmountToPay - model.TotalReceivedAmount;
                model.BillBalanceAmount = model.BalanceAmount;
                model.CashFlowTypeKey = DbConstants.CashFlowType.Out;
                model.BranchKey = PurchaaseOrderMaster.BranchKey;
            }
            else if (model == null)
            {
                model = new PaymentWindowViewModel();
                model.PaymentModeKey = DbConstants.PaymentMode.Cash;
                model.MasterKey = Id;
                model.PaidBy = PurchaaseOrderMaster.Party.PartyName;
                string value = PurchaaseOrderMaster.TotalAmount.ToString();
                model.AmountToPay = value != null && value != "" ? Convert.ToDecimal(value) : 0;
                string TotalReceivedAmount = (PurchaaseOrderMaster.AmountPaid + PurchaaseOrderMaster.OldAdvanceBalance).ToString();
                model.TotalReceivedAmount = TotalReceivedAmount != null && TotalReceivedAmount != "" ? Convert.ToDecimal(TotalReceivedAmount) : 0;
                model.BalanceAmount = model.AmountToPay - model.TotalReceivedAmount;
                model.CashFlowTypeKey = DbConstants.CashFlowType.Out;
                model.OrderNumber = PurchaaseOrderMaster.OrderNumber;
                model.BranchKey = PurchaaseOrderMaster.BranchKey;
            }

            FillTransactionPaymentDropdownLists(model);

            return model;
        }
        public PaymentWindowViewModel CallAssetPurchasePayment(PaymentWindowViewModel model)
        {
            using (var transaction = dbContext.Database.BeginTransaction())
            {
                try
                {
                    CreateAssetPurchasePayment(model);
                    transaction.Commit();
                    model.Message = EduSuiteUIResources.Success;
                    model.IsSuccessful = true;
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    model.Message = String.Format(EduSuiteUIResources.FailedToSaveErrorMessage, EduSuiteUIResources.Payment);
                    model.IsSuccessful = false;
                    ActivityLog.CreateActivityLog(MenuConstants.AssetPurchase, ActionConstants.Payment, DbConstants.LogType.Info, model.PaymentKey, model.Message);
                }

            }
            return model;
        }
        private PaymentWindowViewModel CreateAssetPurchasePayment(PaymentWindowViewModel model)
        {
            AssetPurchasePayment AssetPurchasePayment = new AssetPurchasePayment();
            FillTransactionPaymentDropdownLists(model);

            Int64 maxKey = dbContext.AssetPurchasePayments.Select(p => p.RowKey).DefaultIfEmpty().Max();
            decimal balance = (model.AmountToPay) - (model.TotalReceivedAmount ?? 0);
            AssetPurchasePayment.RowKey = Convert.ToInt64(maxKey + 1);
            if (balance < ((model.PaidAmount ?? 0) + (model.OldAdvanceBalance ?? 0)))
            {
                AssetPurchasePayment.PaidAmount = balance;
            }
            else
            {
                AssetPurchasePayment.PaidAmount = Convert.ToDecimal(model.PaidAmount);
            }
            AssetPurchasePayment.Amount = model.AmountToPay;
            AssetPurchasePayment.PaymentDate = Convert.ToDateTime(model.PaymentDate);
            AssetPurchasePayment.PaymentModeKey = model.PaymentModeKey;
            AssetPurchasePayment.BankAccountKey = model.BankAccountKey;
            AssetPurchasePayment.CardNumber = model.CardNumber;
            AssetPurchasePayment.ChequeOrDDNumber = model.ChequeOrDDNumber;
            AssetPurchasePayment.ChequeOrDDDate = model.ChequeOrDDDate;
            AssetPurchasePayment.Purpose = model.Purpose;
            AssetPurchasePayment.PaidBy = model.PaidBy;
            AssetPurchasePayment.AuthorizedBy = model.AuthorizedBy;
            AssetPurchasePayment.ReceivedBy = model.ReceivedBy;
            AssetPurchasePayment.OnBehalfOf = model.OnBehalfOf;
            AssetPurchasePayment.Remarks = model.Remarks;
            AssetPurchasePayment.PurchaseOrderMasterKey = model.MasterKey;
            AssetPurchasePayment.IsAdavance = model.IsAdavance != null ? model.IsAdavance : false;
            AssetPurchasePayment.BranchKey = model.BranchKey;
            model.BranchKey = AssetPurchasePayment.BranchKey;
            AssetPurchasePayment.BillBalanceAmount = model.BillBalanceAmount ?? model.AmountToPay;
            AssetPurchasePayment.BalanceAmount = model.BalanceAmount;
            if (AssetPurchasePayment.PaymentModeKey == DbConstants.PaymentMode.Cheque)
            {
                AssetPurchasePayment.ChequeStatusKey = DbConstants.ProcessStatus.Pending;
            }
            model.PaymentKey = AssetPurchasePayment.RowKey;
            long oldBankKey = 0;
            dbContext.AssetPurchasePayments.Add(AssetPurchasePayment);
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
                bankAccountModel.Amount = -(model.PaidAmount);
                bankAccountService.UpdateCurrentAccountBalance(bankAccountModel,false,false,null);
            }
            if (model.PartyKey == null)
            {
                model.PartyKey = dbContext.AssetPurchaseMasters.Where(x => x.RowKey == model.MasterKey).Select(x => x.PartyKey).FirstOrDefault();
            }
            model.PartyName = dbContext.Parties.Where(x => x.RowKey == model.PartyKey).Select(x => x.PartyName).FirstOrDefault();
            List<AccountFlowViewModel> accountFlowModelList = new List<AccountFlowViewModel>();
           
            if (model.PaidAmount != 0 && model.PaidAmount != null)
            {
                model.Purpose = dbContext.AssetPurchaseMasters.Where(x => x.RowKey == model.MasterKey).Select(x => x.OrderNumber).FirstOrDefault() + " Payment";
                model.IsUpdate = false;
                accountFlowModelList = DebitAmountList(model, accountFlowModelList);
                model.IsUpdate = false;
                accountFlowModelList = CreditAmountList(model, accountFlowModelList, oldBankKey);
                model.IsUpdate = false;
                accountFlowModelList = PayableAmountList(model, accountFlowModelList, AssetPurchasePayment.PaidAmount ?? 0);
                model.IsUpdate = false;
                CreateAccountFlow(accountFlowModelList, model.IsUpdate);
            }
            dbContext.SaveChanges();
            return model;
        }
        private PaymentWindowViewModel UpdateAssetPurchasePayment(PaymentWindowViewModel model, long AssetPurchasePaymentRowKey, decimal createAdvanceBalance)
        {
            AssetPurchasePayment AssetPurchasePayment = new AssetPurchasePayment();
            FillTransactionPaymentDropdownLists(model);

            AssetPurchasePayment = dbContext.AssetPurchasePayments.SingleOrDefault(row => row.RowKey == AssetPurchasePaymentRowKey);
            decimal oldTotalAmount = AssetPurchasePayment.Amount;
            decimal oldPaidAmount = AssetPurchasePayment.PaidAmount ?? 0;
            decimal oldAmount = AssetPurchasePayment.PaidAmount ?? 0;
            if ((model.AmountToPay) < (model.PaidAmount + (model.OldAdvanceBalance ?? 0)))
            {
                AssetPurchasePayment.PaidAmount = Convert.ToDecimal(model.AmountToPay - (model.OldAdvanceBalance ?? 0));
            }
            else
            {
                AssetPurchasePayment.PaidAmount = Convert.ToDecimal(model.PaidAmount ?? 0);
            }
            AssetPurchasePayment.Amount = model.AmountToPay;
            AssetPurchasePayment.PaymentDate = Convert.ToDateTime(model.PaymentDate);
            model.OldPaymentModeKey = AssetPurchasePayment.PaymentModeKey;
            AssetPurchasePayment.PaymentModeKey = model.PaymentModeKey;
            long oldBankKey = AssetPurchasePayment.BankAccountKey ?? 0;
            AssetPurchasePayment.BankAccountKey = model.BankAccountKey;
            AssetPurchasePayment.CardNumber = model.CardNumber;
            AssetPurchasePayment.ChequeOrDDNumber = model.ChequeOrDDNumber;
            AssetPurchasePayment.ChequeOrDDDate = model.ChequeOrDDDate;
            AssetPurchasePayment.Purpose = model.Purpose;
            AssetPurchasePayment.PaidBy = model.PaidBy;
            AssetPurchasePayment.AuthorizedBy = model.AuthorizedBy;
            AssetPurchasePayment.ReceivedBy = model.ReceivedBy;
            AssetPurchasePayment.OnBehalfOf = model.OnBehalfOf;
            AssetPurchasePayment.Remarks = model.Remarks;
            AssetPurchasePayment.PurchaseOrderMasterKey = model.MasterKey;
            AssetPurchasePayment.IsAdavance = model.IsAdavance != null ? model.IsAdavance : false;
            AssetPurchasePayment.BranchKey = model.BranchKey;
            AssetPurchasePayment.BillBalanceAmount = model.BillBalanceAmount;
            AssetPurchasePayment.BalanceAmount = model.BalanceAmount;
            if (AssetPurchasePayment.PaymentModeKey == DbConstants.PaymentMode.Cheque)
            {
                AssetPurchasePayment.ChequeStatusKey = DbConstants.ProcessStatus.Pending;
            }
            model.PaymentKey = AssetPurchasePayment.RowKey;
            dbContext.SaveChanges();
            if (model.BankAccountKey != null && model.BankAccountKey != 0)
            {
                var BankAccountList = dbContext.BankAccounts.SingleOrDefault(x => x.RowKey == model.BankAccountKey);
                model.BankAccountName = (BankAccountList.NameInAccount ?? BankAccountList.AccountNumber) + EduSuiteUIResources.Hyphen + BankAccountList.Bank.BankName;
            }

            if (DbConstants.PaymentMode.BankPaymentModes.Contains(model.OldPaymentModeKey) && oldBankKey != (model.BankAccountKey ?? 0))
            {
                BankAccountService bankAccountService = new BankAccountService(dbContext);
                BankAccountViewModel bankAccountModel = new BankAccountViewModel();
                bankAccountModel.RowKey = oldBankKey;
                bankAccountModel.Amount = -(oldAmount);
                bankAccountService.UpdateCurrentAccountBalance(bankAccountModel, true, (model.CashFlowTypeKey == DbConstants.CashFlowType.Out) ? true : false, oldAmount);
            }

            if (DbConstants.PaymentMode.BankPaymentModes.Contains(model.PaymentModeKey))
            {
                BankAccountService bankAccountService = new BankAccountService(dbContext);
                BankAccountViewModel bankAccountModel = new BankAccountViewModel();
                bankAccountModel.RowKey = model.BankAccountKey ?? 0;
                bankAccountModel.Amount = model.PaidAmount;
                bankAccountService.UpdateCurrentAccountBalance(bankAccountModel, true, (model.CashFlowTypeKey == DbConstants.CashFlowType.Out) ? true : false, oldAmount);
            }

            //if (model.PaymentModeKey == DbConstants.PaymentMode.Bank)
            //{
            //    BankAccountService bankAccountService = new BankAccountService(dbContext);
            //    BankAccountViewModel bankAccountModel = new BankAccountViewModel();
            //    bankAccountModel.RowKey = model.BankAccountKey ?? 0;
            //    bankAccountModel.Amount = -(model.PaidAmount ?? 0);
            //    bankAccountService.UpdateCurrentAccountBalance(bankAccountModel);
            //    if (model.OldPaymentModeKey == DbConstants.PaymentMode.Bank)
            //    {
            //        bankAccountModel.RowKey = oldBankKey;
            //        bankAccountModel.Amount = oldAmount;
            //        bankAccountService.UpdateCurrentAccountBalance(bankAccountModel);
            //    }
            //}
            //else if (model.OldPaymentModeKey == DbConstants.PaymentMode.Bank)
            //{
            //    BankAccountService bankAccountService = new BankAccountService(dbContext);
            //    BankAccountViewModel bankAccountModel = new BankAccountViewModel();
            //    bankAccountModel.RowKey = oldBankKey;
            //    bankAccountModel.Amount = oldAmount;
            //    bankAccountService.UpdateCurrentAccountBalance(bankAccountModel);
            //}
            model.PartyName = dbContext.Parties.Where(x => x.RowKey == model.PartyKey).Select(x => x.PartyName).FirstOrDefault();
            List<AccountFlowViewModel> accountFlowModelList = new List<AccountFlowViewModel>();
            
            bool IsUpdate = true;
            if (dbContext.AccountFlows.Where(x => x.TransactionTypeKey == DbConstants.TransactionType.Purchase && x.TransactionKey == model.PaymentKey).Any())
            {
                IsUpdate = true;
                model.IsUpdate = IsUpdate;
                accountFlowModelList = DebitAmountList(model, accountFlowModelList);
                model.IsUpdate = IsUpdate;
                accountFlowModelList = CreditAmountList(model, accountFlowModelList, oldBankKey);
                model.IsUpdate = IsUpdate;
                accountFlowModelList = PayableAmountList(model, accountFlowModelList, AssetPurchasePayment.PaidAmount ?? 0);
                model.IsUpdate = IsUpdate;
                CreateAccountFlow(accountFlowModelList, model.IsUpdate);
            }
            else
            {
                if (model.PaidAmount != 0)
                {
                    IsUpdate = false;
                    model.IsUpdate = IsUpdate;
                    accountFlowModelList = DebitAmountList(model, accountFlowModelList);
                    model.IsUpdate = IsUpdate;
                    accountFlowModelList = CreditAmountList(model, accountFlowModelList, oldBankKey);
                    model.IsUpdate = IsUpdate;
                    accountFlowModelList = PayableAmountList(model, accountFlowModelList, AssetPurchasePayment.PaidAmount ?? 0);
                    model.IsUpdate = IsUpdate;
                    CreateAccountFlow(accountFlowModelList, model.IsUpdate);
                }
            }
            return model;
        }

        private void FillTransactionPaymentDropdownLists(AssetPurchaseMasterViewModel model)
        {
            FillPaymentModes(model);
            FillBankAccounts(model);
        }
        private void CreateAssetPurchasePayment(AssetPurchaseMasterViewModel model, long AssetPurchaseMasterKey, long oldPartyKey, decimal createAdvanceBalance)
        {
            PaymentWindowViewModel paymentWindowViewModel = new PaymentWindowViewModel();
            FillTransactionPaymentDropdownLists(model);

            paymentWindowViewModel.PaidAmount = Convert.ToDecimal(model.AmountPaid ?? 0);
            paymentWindowViewModel.AmountToPay = Convert.ToDecimal(model.TotalAmount ?? 0);
            paymentWindowViewModel.PaymentDate = Convert.ToDateTime(DateTimeUTC.Now);
            paymentWindowViewModel.PaymentModeKey = (model.PaymentModeKey != 0) ? model.PaymentModeKey : DbConstants.PaymentMode.Cash;
            paymentWindowViewModel.BankAccountKey = model.BankAccountKey;
            paymentWindowViewModel.CardNumber = model.CardNumber;
            paymentWindowViewModel.ChequeOrDDNumber = model.ChequeOrDDNumber;
            paymentWindowViewModel.MasterKey = AssetPurchaseMasterKey;
            paymentWindowViewModel.ChequeOrDDDate = model.ChequeOrDDDate;
            paymentWindowViewModel.IsAdavance = true;
            paymentWindowViewModel.Purpose = model.OrderNumber + " Order Payment";
            paymentWindowViewModel.OldPartyKey = oldPartyKey;
            paymentWindowViewModel.PartyKey = model.PartyKey;
            paymentWindowViewModel.BranchKey = model.BranchKey;
            paymentWindowViewModel.BalanceAmount = model.BalanceAmount;
            paymentWindowViewModel.OldAdvanceBalance = model.OldAdvanceBalance;
            if (model.AssetPurchasePaymentRowKey == null || model.AssetPurchasePaymentRowKey == 0)
            {
                if (model.AmountPaid != null && model.AmountPaid != 0)
                {
                    CreateAssetPurchasePayment(paymentWindowViewModel);
                }
            }
            else
            {
                if (model.AmountPaid != null && model.AmountPaid != 0)
                {
                    UpdateAssetPurchasePayment(paymentWindowViewModel, model.AssetPurchasePaymentRowKey, createAdvanceBalance);
                }
            }
            dbContext.SaveChanges();
        }
        private void FillPaymentModes(AssetPurchaseMasterViewModel model)
        {
            model.PaymentModes = dbContext.VwPaymentModeSelectActiveOnlies.OrderBy(row => row.DisplayOrder).Select(row => new SelectListModel
            {
                RowKey = row.RowKey,
                Text = row.PaymentModeName
            }).ToList();
        }
        private void FillBankAccounts(AssetPurchaseMasterViewModel model)
        {
            model.BankAccounts = dbContext.BankAccounts.Where(x => x.IsActive == true).Select(row => new SelectListModel
            {
                RowKey = row.RowKey,
                Text = (row.NameInAccount ?? row.AccountNumber) + EduSuiteUIResources.Hyphen + row.Bank.BankName
            }).ToList();
        }

        private void FillTransactionPaymentDropdownLists(PaymentWindowViewModel model)
        {
            FillPaymentModes(model);
            FillBankAccounts(model);
        }
        private void FillPaymentModes(PaymentWindowViewModel model)
        {
            model.PaymentModes = dbContext.VwPaymentModeSelectActiveOnlies.OrderBy(row => row.DisplayOrder).Select(row => new SelectListModel
            {
                RowKey = row.RowKey,
                Text = row.PaymentModeName
            }).ToList();
        }
        private void FillBankAccounts(PaymentWindowViewModel model)
        {
            model.BankAccounts = dbContext.BankAccounts.Where(x => x.IsActive == true).Select(row => new SelectListModel
            {
                RowKey = row.RowKey,
                Text = (row.NameInAccount ?? row.AccountNumber) + EduSuiteUIResources.Hyphen + row.Bank.BankName
            }).ToList();
        }
        public decimal CheckShortBalance(short PaymentModeKey, long Rowkey, long BankAccountKey)
        {
            decimal Balance = 0;
            if (PaymentModeKey == DbConstants.PaymentMode.Cash)
            {
                long accountHeadKey = dbContext.AccountHeads.Where(x => x.RowKey == DbConstants.AccountHead.CashAccount && x.IsActive).Select(x => x.RowKey).FirstOrDefault();
                decimal totalDebit = dbContext.AccountFlows.Where(x => x.AccountHeadKey == accountHeadKey && x.CashFlowTypeKey == DbConstants.CashFlowType.In).Select(x => x.Amount).DefaultIfEmpty().Sum();
                decimal totalCredit = dbContext.AccountFlows.Where(x => x.AccountHeadKey == accountHeadKey && x.CashFlowTypeKey == DbConstants.CashFlowType.Out).Select(x => x.Amount).DefaultIfEmpty().Sum();
                Balance = totalDebit - totalCredit;
                if (Rowkey != 0)
                {
                    var purchaseList = dbContext.AssetPurchasePayments.SingleOrDefault(x => x.RowKey == Rowkey);
                    if (PaymentModeKey == purchaseList.PaymentModeKey)
                    {
                        Balance = Balance + (purchaseList.PaidAmount ?? 0);
                    }
                }
            }
            else if (PaymentModeKey == DbConstants.PaymentMode.Bank || PaymentModeKey == DbConstants.PaymentMode.Cheque)
            {
                if (BankAccountKey != 0 && BankAccountKey != null)
                {
                    Balance = dbContext.BankAccounts.Where(x => x.RowKey == BankAccountKey).Select(x => x.CurrentAccountBalance ?? 0).FirstOrDefault();
                    if (Rowkey != 0)
                    {
                        var purchaseList = dbContext.AssetPurchasePayments.SingleOrDefault(x => x.RowKey == Rowkey);
                        if (BankAccountKey == purchaseList.BankAccountKey)
                        {
                            Balance = Balance + (purchaseList.PaidAmount ?? 0);
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
        private List<AccountFlowViewModel> PayableAmountList(PaymentWindowViewModel model, List<AccountFlowViewModel> accountFlowModelList, decimal amount)
        {
            long accountHeadKey;
            accountHeadKey = dbContext.VwAccountHeadSelectActiveOnlies.Where(x => x.RowKey == DbConstants.AccountHead.AccountsPayable).Select(x => x.RowKey).FirstOrDefault();
            accountFlowModelList.Add(new AccountFlowViewModel
            {
                CashFlowTypeKey = DbConstants.CashFlowType.In,
                AccountHeadKey = accountHeadKey,
                Amount = amount,
                TransactionTypeKey = DbConstants.TransactionType.AssetPurchasePayable,
                VoucherTypeKey = DbConstants.VoucherType.Purchase,
                TransactionKey = model.PaymentKey,
                TransactionDate = model.PaymentDate,
                ExtraUpdateKey = 0,
                IsUpdate = model.IsUpdate,
                BranchKey = model.BranchKey,
                Purpose = model.Purpose + EduSuiteUIResources.BlankSpace + EduSuiteUIResources.By + EduSuiteUIResources.BlankSpace + (model.PaymentModeKey == DbConstants.PaymentMode.Cash ? EduSuiteUIResources.Cash : model.PaymentModeKey == DbConstants.PaymentMode.Bank ? EduSuiteUIResources.Bank + EduSuiteUIResources.OpenBracket + model.BankAccountName + EduSuiteUIResources.CloseBracket : model.PaymentModeKey == DbConstants.PaymentMode.Cheque ? EduSuiteUIResources.Cheque + EduSuiteUIResources.OpenBracket + model.BankAccountName + EduSuiteUIResources.CloseBracket : ""),
            });
            return accountFlowModelList;
        }
        private List<AccountFlowViewModel> AdvanceAmountList(PaymentWindowViewModel model, List<AccountFlowViewModel> accountFlowModelList, decimal amount)
        {
            long accountHeadKey;
            long ExtraUpdateKey = 0;
            accountHeadKey = dbContext.VwAccountHeadSelectActiveOnlies.Where(x => x.RowKey == DbConstants.AccountHead.AdvanceReceivable).Select(x => x.RowKey).FirstOrDefault();
            accountFlowModelList.Add(new AccountFlowViewModel
            {
                CashFlowTypeKey = DbConstants.CashFlowType.In,
                AccountHeadKey = accountHeadKey,
                Amount = amount,
                TransactionTypeKey = DbConstants.TransactionType.AssetPurchaseAdvance,
                VoucherTypeKey = DbConstants.VoucherType.Purchase,
                TransactionDate = model.PaymentDate,
                ExtraUpdateKey = ExtraUpdateKey,
                IsUpdate = model.IsUpdate,
                Purpose = model.Purpose + EduSuiteUIResources.BlankSpace + EduSuiteUIResources.RecievedFrom + EduSuiteUIResources.BlankSpace + model.PartyName,
                BranchKey = model.BranchKey,
                TransactionKey = model.PaymentKey,
            });
            return accountFlowModelList;
        }
        private List<AccountFlowViewModel> DebitAmountList(PaymentWindowViewModel model, List<AccountFlowViewModel> accountFlowModelList)
        {
            long accountHeadKey;
            long ExtraUpdateKey = 0;
            accountHeadKey = dbContext.Parties.Where(x => x.RowKey == model.PartyKey).Select(x => x.AccountHeadKey).FirstOrDefault();
            if (model.OldPartyKey != null && model.OldPartyKey != 0 && model.OldPartyKey != model.PartyKey)
            {
                model.IsUpdate = false;
                long oldAccountHeadKey = dbContext.Parties.Where(x => x.RowKey == model.OldPartyKey).Select(x => x.AccountHeadKey).FirstOrDefault();
                //ExtraUpdateKey = dbContext.VwAccountHeadSelectActiveOnlies.Where(x => x.RowKey == oldAccountHeadKey).Select(x => x.RowKey).FirstOrDefault();
                ExtraUpdateKey = oldAccountHeadKey;
            }
            accountFlowModelList.Add(new AccountFlowViewModel
            {
                CashFlowTypeKey = DbConstants.CashFlowType.In,
                AccountHeadKey = accountHeadKey,
                Amount = model.PaidAmount ?? 0,
                TransactionTypeKey = DbConstants.TransactionType.AssetPurchase,
                VoucherTypeKey = DbConstants.VoucherType.Purchase,
                TransactionKey = model.PaymentKey,
                TransactionDate = model.PaymentDate,
                ExtraUpdateKey = ExtraUpdateKey,
                IsUpdate = model.IsUpdate,
                BranchKey = model.BranchKey,
                Purpose = model.Purpose + EduSuiteUIResources.BlankSpace + EduSuiteUIResources.By + EduSuiteUIResources.BlankSpace + (model.PaymentModeKey == DbConstants.PaymentMode.Cash ? EduSuiteUIResources.Cash : model.PaymentModeKey == DbConstants.PaymentMode.Bank ? EduSuiteUIResources.Bank + EduSuiteUIResources.OpenBracket + model.BankAccountName + EduSuiteUIResources.CloseBracket : model.PaymentModeKey == DbConstants.PaymentMode.Cheque ? EduSuiteUIResources.Cheque + EduSuiteUIResources.OpenBracket + model.BankAccountName + EduSuiteUIResources.CloseBracket : ""),
            });
            return accountFlowModelList;
        }
        private List<AccountFlowViewModel> CreditAmountList(PaymentWindowViewModel model, List<AccountFlowViewModel> accountFlowModelList, long oldBankKey)
        {
            long oldBankAccountHeadKey = 0;
            if (model.OldPaymentModeKey == DbConstants.PaymentMode.Bank || model.OldPaymentModeKey == DbConstants.PaymentMode.Cheque)
            {
                long bankAccountHeadKey = dbContext.BankAccounts.Where(x => x.RowKey == oldBankKey).Select(x => x.AccountHeadKey).FirstOrDefault();
                //oldBankAccountHeadKey = dbContext.VwAccountHeadSelectActiveOnlies.Where(x => x.AccountHeadCode == bankAccountCode).Select(x => x.RowKey).FirstOrDefault();
                oldBankAccountHeadKey = bankAccountHeadKey;
            }
            long accountHeadKey;
            long ExtraUpdateKey = 0;
            if (model.PaymentModeKey == DbConstants.PaymentMode.Bank || model.PaymentModeKey == DbConstants.PaymentMode.Cheque)
            {
                accountHeadKey = dbContext.BankAccounts.Where(x => x.RowKey == model.BankAccountKey).Select(x => x.AccountHeadKey).FirstOrDefault();
            }
            else
            {
                accountHeadKey = dbContext.VwAccountHeadSelectActiveOnlies.Where(x => x.RowKey == DbConstants.AccountHead.CashAccount).Select(x => x.RowKey).FirstOrDefault();
            }
            if (model.OldPaymentModeKey != null && model.OldPaymentModeKey != 0 && model.OldPaymentModeKey != model.PaymentModeKey)
            {
                model.IsUpdate = false;
                ExtraUpdateKey = model.OldPaymentModeKey == DbConstants.PaymentMode.Cash ? DbConstants.AccountHead.CashAccount : oldBankAccountHeadKey;
            }
            accountFlowModelList.Add(new AccountFlowViewModel
            {
                CashFlowTypeKey = DbConstants.CashFlowType.Out,
                AccountHeadKey = accountHeadKey,
                Amount = model.PaidAmount ?? 0,
                TransactionTypeKey = DbConstants.TransactionType.AssetPurchase,
                VoucherTypeKey = DbConstants.VoucherType.Purchase,
                TransactionDate = model.PaymentDate,
                ExtraUpdateKey = ExtraUpdateKey,
                IsUpdate = model.IsUpdate,
                Purpose = model.Purpose + EduSuiteUIResources.BlankSpace + EduSuiteUIResources.PaidTo + EduSuiteUIResources.BlankSpace + EduSuiteUIResources.Supplier + EduSuiteUIResources.BlankSpace + model.PartyName,
                BranchKey = model.BranchKey,
                TransactionKey = model.PaymentKey,
            });
            return accountFlowModelList;
        }
        private List<AccountFlowViewModel> DebitMaterialList(AssetPurchaseDetail AssetPurchaseDetailModel, List<AccountFlowViewModel> accountFlowModelList, bool IsUpdate, short? branchKey, string orderNumber, DateTime BillDate, string partyName)
        {
            long ExtraUpdateKey = 0;
            long accountHeadKey = AssetPurchaseDetailModel.AssetType.AccountHeadKey;
            accountFlowModelList.Add(new AccountFlowViewModel
            {
                CashFlowTypeKey = DbConstants.CashFlowType.In,
                AccountHeadKey = accountHeadKey,
                Amount = AssetPurchaseDetailModel.RowTotal ?? 0,
                TransactionTypeKey = DbConstants.TransactionType.AssetPurchaseInventory,
                VoucherTypeKey = DbConstants.VoucherType.Stock,
                TransactionKey = AssetPurchaseDetailModel.RowKey,
                TransactionDate = BillDate,
                ExtraUpdateKey = ExtraUpdateKey,
                IsUpdate = IsUpdate,
                BranchKey = branchKey,
                Purpose = orderNumber + EduSuiteUIResources.BlankSpace + EduSuiteUIResources.Order + EduSuiteUIResources.BlankSpace + EduSuiteUIResources.OpenBracket + AssetPurchaseDetailModel.AssetType.AssetTypeName + EduSuiteUIResources.BlankSpace + EduSuiteUIResources.CloseBracket + EduSuiteUIResources.BlankSpace + EduSuiteUIResources.Purchase + " From " + EduSuiteUIResources.Supplier + EduSuiteUIResources.BlankSpace + partyName,
            });
            return accountFlowModelList;
        }
        private List<AccountFlowViewModel> CreditTotalAmountList(AssetPurchaseMaster AssetPurchaseMaster, List<AccountFlowViewModel> accountFlowModelList, bool IsUpdate, short? branchKey)
        {
            long ExtraUpdateKey = 0;
            var PartyList = dbContext.Parties.SingleOrDefault(x => x.RowKey == AssetPurchaseMaster.PartyKey);
            long accountHeadKey = PartyList.AccountHeadKey;
            accountFlowModelList.Add(new AccountFlowViewModel
            {
                CashFlowTypeKey = DbConstants.CashFlowType.Out,
                AccountHeadKey = accountHeadKey,
                Amount = AssetPurchaseMaster.SubTotal ?? 0,
                TransactionTypeKey = DbConstants.TransactionType.AssetPurchaseMaster,
                VoucherTypeKey = DbConstants.VoucherType.Purchase,
                TransactionKey = AssetPurchaseMaster.RowKey,
                TransactionDate = AssetPurchaseMaster.BillDate,
                ExtraUpdateKey = ExtraUpdateKey,
                IsUpdate = IsUpdate,
                BranchKey = branchKey,
                Purpose = AssetPurchaseMaster.OrderNumber + " Order Purchase",
            });
            accountFlowModelList.Add(new AccountFlowViewModel
            {
                CashFlowTypeKey = DbConstants.CashFlowType.In,
                AccountHeadKey = accountHeadKey,
                Amount = AssetPurchaseMaster.OldAdvanceBalance ?? 0,
                TransactionTypeKey = DbConstants.TransactionType.AssetPurchaseMaster,
                VoucherTypeKey = DbConstants.VoucherType.Purchase,
                TransactionKey = AssetPurchaseMaster.RowKey,
                TransactionDate = AssetPurchaseMaster.BillDate,
                ExtraUpdateKey = ExtraUpdateKey,
                IsUpdate = IsUpdate,
                BranchKey = branchKey,
                Purpose = AssetPurchaseMaster.OrderNumber + "Excess Amount Taken",
            });
            accountHeadKey = dbContext.VwAccountHeadSelectActiveOnlies.Where(x => x.RowKey == DbConstants.AccountHead.AccountsPayable).Select(x => x.RowKey).FirstOrDefault();
            accountFlowModelList.Add(new AccountFlowViewModel
            {
                CashFlowTypeKey = DbConstants.CashFlowType.Out,
                AccountHeadKey = accountHeadKey,
                Amount = AssetPurchaseMaster.SubTotal ?? 0,
                TransactionTypeKey = DbConstants.TransactionType.AssetPurchasePayable,
                VoucherTypeKey = DbConstants.VoucherType.Purchase,
                TransactionKey = AssetPurchaseMaster.RowKey,
                TransactionDate = AssetPurchaseMaster.BillDate,
                ExtraUpdateKey = ExtraUpdateKey,
                IsUpdate = IsUpdate,
                BranchKey = branchKey,
                Purpose = AssetPurchaseMaster.OrderNumber + " Order Purchase From " + PartyList.PartyName,
            });
            accountHeadKey = dbContext.VwAccountHeadSelectActiveOnlies.Where(x => x.RowKey == DbConstants.AccountHead.AccountsPayable).Select(x => x.RowKey).FirstOrDefault();
            accountFlowModelList.Add(new AccountFlowViewModel
            {
                CashFlowTypeKey = DbConstants.CashFlowType.In,
                AccountHeadKey = accountHeadKey,
                Amount = AssetPurchaseMaster.OldAdvanceBalance ?? 0,
                TransactionTypeKey = DbConstants.TransactionType.AssetPurchaseExcessAmount,
                VoucherTypeKey = DbConstants.VoucherType.Purchase,
                TransactionKey = AssetPurchaseMaster.RowKey,
                TransactionDate = AssetPurchaseMaster.BillDate,
                ExtraUpdateKey = ExtraUpdateKey,
                IsUpdate = IsUpdate,
                BranchKey = branchKey,
                Purpose = AssetPurchaseMaster.OrderNumber + " Order Purchase Excess Amount From " + PartyList.PartyName,
            });
            accountHeadKey = dbContext.VwAccountHeadSelectActiveOnlies.Where(x => x.RowKey == DbConstants.AccountHead.AdvanceReceivable).Select(x => x.RowKey).FirstOrDefault();
            accountFlowModelList.Add(new AccountFlowViewModel
            {
                CashFlowTypeKey = DbConstants.CashFlowType.Out,
                AccountHeadKey = accountHeadKey,
                Amount = AssetPurchaseMaster.OldAdvanceBalance ?? 0,
                TransactionTypeKey = DbConstants.TransactionType.AssetPurchaseExcessAmount,
                VoucherTypeKey = DbConstants.VoucherType.Purchase,
                TransactionKey = AssetPurchaseMaster.RowKey,
                TransactionDate = AssetPurchaseMaster.BillDate,
                ExtraUpdateKey = ExtraUpdateKey,
                IsUpdate = IsUpdate,
                BranchKey = branchKey,
                Purpose = AssetPurchaseMaster.OrderNumber + " Order Purchase Excess Amount From " + PartyList.PartyName,
            });
            return accountFlowModelList;
        }
        private List<AccountFlowViewModel> CreditIGSTList(AssetPurchaseMaster AssetPurchaseMaster, List<AccountFlowViewModel> accountFlowModelList, bool IsUpdate, short? branchKey)
        {
            long accountHeadKey = dbContext.VwAccountHeadSelectActiveOnlies.Where(x => x.RowKey == DbConstants.AccountHead.InputTaxIGST).Select(x => x.RowKey).FirstOrDefault();
            long ExtraUpdateKey = 0;
            accountFlowModelList.Add(new AccountFlowViewModel
            {
                CashFlowTypeKey = DbConstants.CashFlowType.In,
                AccountHeadKey = accountHeadKey,
                Amount = AssetPurchaseMaster.IGSTAmt ?? 0,
                TransactionTypeKey = DbConstants.TransactionType.AssetPurchaseCGST,
                VoucherTypeKey = DbConstants.VoucherType.InputTax,
                TransactionKey = AssetPurchaseMaster.RowKey,
                TransactionDate = AssetPurchaseMaster.BillDate,
                ExtraUpdateKey = ExtraUpdateKey,
                IsUpdate = IsUpdate,
                BranchKey = branchKey,
                Purpose = AssetPurchaseMaster.OrderNumber + " IGST",
            });
            return accountFlowModelList;
        }
        private List<AccountFlowViewModel> CreditCGSTList(AssetPurchaseMaster AssetPurchaseMaster, List<AccountFlowViewModel> accountFlowModelList, bool IsUpdate, short? branchKey)
        {
            long accountHeadKey = dbContext.VwAccountHeadSelectActiveOnlies.Where(x => x.RowKey == DbConstants.AccountHead.InputTaxCGST).Select(x => x.RowKey).FirstOrDefault();
            long ExtraUpdateKey = 0;
            accountFlowModelList.Add(new AccountFlowViewModel
            {
                CashFlowTypeKey = DbConstants.CashFlowType.In,
                AccountHeadKey = accountHeadKey,
                Amount = AssetPurchaseMaster.CGSTAmt ?? 0,
                TransactionTypeKey = DbConstants.TransactionType.AssetPurchaseCGST,
                VoucherTypeKey = DbConstants.VoucherType.InputTax,
                TransactionKey = AssetPurchaseMaster.RowKey,
                TransactionDate = AssetPurchaseMaster.BillDate,
                ExtraUpdateKey = ExtraUpdateKey,
                IsUpdate = IsUpdate,
                BranchKey = branchKey,
                Purpose = AssetPurchaseMaster.OrderNumber + " CGST",
            });
            return accountFlowModelList;
        }
        private List<AccountFlowViewModel> CreditSGSTList(AssetPurchaseMaster AssetPurchaseMaster, List<AccountFlowViewModel> accountFlowModelList, bool IsUpdate, short? branchKey)
        {
            long accountHeadKey = dbContext.VwAccountHeadSelectActiveOnlies.Where(x => x.RowKey == DbConstants.AccountHead.InputTaxSGST).Select(x => x.RowKey).FirstOrDefault();
            long ExtraUpdateKey = 0;
            accountFlowModelList.Add(new AccountFlowViewModel
            {
                CashFlowTypeKey = DbConstants.CashFlowType.In,
                AccountHeadKey = accountHeadKey,
                Amount = AssetPurchaseMaster.SGSTAmt ?? 0,
                TransactionTypeKey = DbConstants.TransactionType.AssetPurchaseSGST,
                VoucherTypeKey = DbConstants.VoucherType.InputTax,
                TransactionKey = AssetPurchaseMaster.RowKey,
                TransactionDate = AssetPurchaseMaster.BillDate,
                ExtraUpdateKey = ExtraUpdateKey,
                IsUpdate = IsUpdate,
                BranchKey = branchKey,
                Purpose = AssetPurchaseMaster.OrderNumber + " SGST",
            });
            return accountFlowModelList;
        }
        private List<AccountFlowViewModel> DiscountAmountList(AssetPurchaseMaster AssetPurchaseMaster, List<AccountFlowViewModel> accountFlowModelList, bool IsUpdate, short? branchKey)
        {
            long accountHeadKey;
            long ExtraUpdateKey = 0;
            if (AssetPurchaseMaster.PartyKey != null)
            {
                accountHeadKey = dbContext.Parties.Where(x => x.RowKey == AssetPurchaseMaster.PartyKey).Select(x => x.AccountHeadKey).FirstOrDefault();
            }
            else
            {
                accountHeadKey = dbContext.AccountHeads.Where(x => x.RowKey == DbConstants.AccountHead.CashSale && x.IsActive).Select(x => x.RowKey).FirstOrDefault();
            }
            accountFlowModelList.Add(new AccountFlowViewModel
            {
                CashFlowTypeKey = DbConstants.CashFlowType.In,
                AccountHeadKey = accountHeadKey,
                Amount = AssetPurchaseMaster.Discount ?? 0,
                TransactionTypeKey = DbConstants.TransactionType.DiscountAssetPurchase,
                VoucherTypeKey = DbConstants.VoucherType.Purchase,
                TransactionKey = AssetPurchaseMaster.RowKey,
                TransactionDate = AssetPurchaseMaster.BillDate,
                ExtraUpdateKey = ExtraUpdateKey,
                IsUpdate = false,
                BranchKey = branchKey,
                Purpose = AssetPurchaseMaster.OrderNumber + EduSuiteUIResources.BlankSpace + EduSuiteUIResources.Order + EduSuiteUIResources.BlankSpace + EduSuiteUIResources.DiscountTaken,
            });
            accountHeadKey = dbContext.VwAccountHeadSelectActiveOnlies.Where(x => x.RowKey == DbConstants.AccountHead.AccountsPayable).Select(x => x.RowKey).FirstOrDefault();
            accountFlowModelList.Add(new AccountFlowViewModel
            {
                CashFlowTypeKey = DbConstants.CashFlowType.In,
                AccountHeadKey = accountHeadKey,
                Amount = AssetPurchaseMaster.Discount ?? 0,
                TransactionTypeKey = DbConstants.TransactionType.DiscountAssetPurchasePayable,
                VoucherTypeKey = DbConstants.VoucherType.Purchase,
                TransactionKey = AssetPurchaseMaster.RowKey,
                TransactionDate = AssetPurchaseMaster.BillDate,
                ExtraUpdateKey = 0,
                IsUpdate = false,
                BranchKey = branchKey,
                Purpose = AssetPurchaseMaster.OrderNumber + EduSuiteUIResources.BlankSpace + EduSuiteUIResources.Order + EduSuiteUIResources.BlankSpace + EduSuiteUIResources.DiscountTaken,
            });
            accountHeadKey = dbContext.VwAccountHeadSelectActiveOnlies.Where(x => x.RowKey == DbConstants.AccountHead.DiscountAllowed).Select(x => x.RowKey).FirstOrDefault();
            accountFlowModelList.Add(new AccountFlowViewModel
            {
                CashFlowTypeKey = DbConstants.CashFlowType.Out,
                AccountHeadKey = accountHeadKey,
                Amount = AssetPurchaseMaster.Discount ?? 0,
                TransactionTypeKey = DbConstants.TransactionType.DiscountAssetPurchase,
                VoucherTypeKey = DbConstants.VoucherType.Purchase,
                TransactionKey = AssetPurchaseMaster.RowKey,
                TransactionDate = AssetPurchaseMaster.BillDate,
                ExtraUpdateKey = ExtraUpdateKey,
                IsUpdate = false,
                BranchKey = branchKey,
                Purpose = AssetPurchaseMaster.OrderNumber + EduSuiteUIResources.BlankSpace + EduSuiteUIResources.Order + EduSuiteUIResources.BlankSpace + EduSuiteUIResources.DiscountTaken,
            });
            return accountFlowModelList;
        }
        private List<AccountFlowViewModel> RoundOffAmountList(AssetPurchaseMaster AssetPurchaseMaster, List<AccountFlowViewModel> accountFlowModelList, bool IsUpdate, short? branchKey)
        {
            long accountHeadKey;
            long ExtraUpdateKey = 0;
            if (AssetPurchaseMaster.PartyKey != null)
            {
                accountHeadKey = dbContext.Parties.Where(x => x.RowKey == AssetPurchaseMaster.PartyKey).Select(x => x.AccountHeadKey).FirstOrDefault();
            }
            else
            {
                accountHeadKey = dbContext.AccountHeads.Where(x => x.RowKey == DbConstants.AccountHead.CashSale).Select(x => x.RowKey).FirstOrDefault();
            }
            accountFlowModelList.Add(new AccountFlowViewModel
            {
                CashFlowTypeKey = DbConstants.CashFlowType.In,
                AccountHeadKey = accountHeadKey,
                Amount = AssetPurchaseMaster.RoundOff ?? 0,
                TransactionTypeKey = DbConstants.TransactionType.RoundOffAssetPurchase,
                VoucherTypeKey = DbConstants.VoucherType.Purchase,
                TransactionKey = AssetPurchaseMaster.RowKey,
                TransactionDate = AssetPurchaseMaster.BillDate,
                ExtraUpdateKey = ExtraUpdateKey,
                IsUpdate = false,
                BranchKey = branchKey,
                Purpose = AssetPurchaseMaster.OrderNumber + EduSuiteUIResources.BlankSpace + EduSuiteUIResources.Order + EduSuiteUIResources.BlankSpace + EduSuiteUIResources.RoundOff,
            });
            accountHeadKey = dbContext.VwAccountHeadSelectActiveOnlies.Where(x => x.RowKey == DbConstants.AccountHead.AccountsPayable).Select(x => x.RowKey).FirstOrDefault();
            accountFlowModelList.Add(new AccountFlowViewModel
            {
                CashFlowTypeKey = DbConstants.CashFlowType.In,
                AccountHeadKey = accountHeadKey,
                Amount = AssetPurchaseMaster.RoundOff ?? 0,
                TransactionTypeKey = DbConstants.TransactionType.RoundOffAssetPurchasePayable,
                VoucherTypeKey = DbConstants.VoucherType.Purchase,
                TransactionKey = AssetPurchaseMaster.RowKey,
                TransactionDate = AssetPurchaseMaster.BillDate,
                ExtraUpdateKey = 0,
                IsUpdate = false,
                BranchKey = branchKey,
                Purpose = AssetPurchaseMaster.OrderNumber + EduSuiteUIResources.BlankSpace + EduSuiteUIResources.Order + EduSuiteUIResources.BlankSpace + EduSuiteUIResources.RoundOff,
            });
            accountHeadKey = dbContext.VwAccountHeadSelectActiveOnlies.Where(x => x.RowKey == DbConstants.AccountHead.DirectMaterials).Select(x => x.RowKey).FirstOrDefault();
            accountFlowModelList.Add(new AccountFlowViewModel
            {
                CashFlowTypeKey = DbConstants.CashFlowType.Out,
                AccountHeadKey = accountHeadKey,
                Amount = AssetPurchaseMaster.RoundOff ?? 0,
                TransactionTypeKey = DbConstants.TransactionType.RoundOffAssetPurchase,
                VoucherTypeKey = DbConstants.VoucherType.Purchase,
                TransactionKey = AssetPurchaseMaster.RowKey,
                TransactionDate = AssetPurchaseMaster.BillDate,
                ExtraUpdateKey = ExtraUpdateKey,
                IsUpdate = false,
                BranchKey = branchKey,
                Purpose = AssetPurchaseMaster.OrderNumber + EduSuiteUIResources.BlankSpace + EduSuiteUIResources.Order + EduSuiteUIResources.BlankSpace + EduSuiteUIResources.RoundOff,
            });
            return accountFlowModelList;
        }

        #endregion

        #region View
        public AssetPurchaseMasterViewModel ViewAssetPurchaseMasterById(int? id)
        {
            try
            {
                AssetPurchaseMasterViewModel model = new AssetPurchaseMasterViewModel();
                model = (from row in dbContext.AssetPurchaseMasters
                         join r in dbContext.AssetPurchasePayments.Where(r => r.IsAdavance == true)
                         on row.RowKey equals r.PurchaseOrderMasterKey into joined
                         from j in joined.DefaultIfEmpty()
                         select new AssetPurchaseMasterViewModel
                         {
                             RowKey = row.RowKey,
                             PartyKey = row.PartyKey,
                             PartyName = row.Party.PartyName,
                             BillDate = row.BillDate,
                             OrderNumber = row.OrderNumber,
                             TaxableAmount = row.TaxableAmount,
                             NonTaxableAmount = row.NonTaxableAmount,
                             CGSTAmt = row.CGSTAmt,
                             SGSTAmt = row.SGSTAmt,
                             IGSTAmt = row.IGSTAmt,
                             TotalAmount = row.TotalAmount,
                             AmountPaid = row.AmountPaid,
                             CardNumber = j.CardNumber,
                             BankAccountKey = j.BankAccountKey,
                             ChequeOrDDNumber = j.ChequeOrDDNumber,
                             ChequeOrDDDate = j.ChequeOrDDDate,
                             RoundOff = (row.RoundOff ?? 0),
                             Discount = (row.Discount ?? 0),
                             OldAdvanceBalance = row.OldAdvanceBalance ?? 0,
                         }).Where(x => x.RowKey == id).FirstOrDefault();
                model.BalanceAmount = (model.TotalAmount - model.OldAdvanceBalance) - Convert.ToDecimal(dbContext.AssetPurchasePayments.Where(x => x.ChequeStatusKey != DbConstants.ProcessStatus.Rejected && x.PurchaseOrderMasterKey == id).Sum(x => x.PaidAmount));
                FillPurchaseDetails(model);
                GetPurchasePayments(model);
                return model;
            }
            catch (Exception ex)
            {
                AssetPurchaseMasterViewModel model = new AssetPurchaseMasterViewModel();
                ActivityLog.CreateActivityLog(MenuConstants.AssetPurchase, ActionConstants.View, DbConstants.LogType.Error, model.RowKey, ex.GetBaseException().Message);
                return model;
            }
        }

        public void GetPurchasePayments(AssetPurchaseMasterViewModel model)
        {
            try
            {
                model.PurchasePayments = (from d in dbContext.AssetPurchaseMasters
                                          join p in dbContext.AssetPurchasePayments.Where(r => r.PurchaseOrderMasterKey == model.RowKey)
                                          on d.RowKey equals p.PurchaseOrderMasterKey
                                          select new PaymentWindowViewModel
                                          {
                                              RowKey = p.RowKey,
                                              PaymentDate = p.PaymentDate,
                                              Purpose = p.Purpose,
                                              ReceivedAmount = p.PaidAmount??0,
                                              BalanceAmount = p.BalanceAmount,
                                              TotalAmount = d.TotalAmount,
                                              PaymentModeKey = p.PaymentModeKey
                                          }).ToList();
                paymentModeDetails(model);
            }
            catch (Exception ex)
            {

            }
        }

        private void paymentModeDetails(AssetPurchaseMasterViewModel model)
        {
            foreach (PaymentWindowViewModel modelItem in model.PurchasePayments)
            {
                PaymentWindowViewModel m = (from p in dbContext.AssetPurchasePayments.Where(r => r.RowKey == modelItem.RowKey)
                                            select new PaymentWindowViewModel
                                            {
                                                BankAccountKey = p.BankAccountKey,
                                                BankAccountDetails = p.BankAccount.Bank.BankName + "(" + p.BankAccount.BranchLocation + ")" + Environment.NewLine + p.BankAccount.AccountNumber,
                                                CardNumber = p.CardNumber,
                                                ChequeOrDDNumber = p.ChequeOrDDNumber,
                                                ChequeOrDDDate = p.ChequeOrDDDate,
                                            }).FirstOrDefault();

                switch (modelItem.PaymentModeKey)
                {
                    case DbConstants.PaymentMode.Cash:
                        modelItem.PaymentModeDetail = EduSuiteUIResources.Cash;
                        break;

                    case DbConstants.PaymentMode.Bank:
                        modelItem.PaymentModeDetail = EduSuiteUIResources.Bank + EduSuiteUIResources.BlankSpace + m.BankAccountDetails;
                        break;

                    case DbConstants.PaymentMode.Cheque:
                        modelItem.PaymentModeDetail = EduSuiteUIResources.Cheque + "(" + EduSuiteUIResources.BlankSpace + m.ChequeOrDDNumber + "," + EduSuiteUIResources.BlankSpace + Convert.ToDateTime(m.ChequeOrDDDate).ToString("dd/MM/yyyy") + ")" + EduSuiteUIResources.BlankSpace + EduSuiteUIResources.BlankSpace + m.BankAccountDetails;
                        break;
                    default:
                        modelItem.PaymentModeDetail = EduSuiteUIResources.BlankSpace;
                        break;
                }
            }

        }

        #endregion

        public AssetPurchaseDetailsViewModel GetRawMaterialDetailsById(int? id, short branchKey)
        {
            AssetPurchaseDetailsViewModel model = new AssetPurchaseDetailsViewModel();

            model = (from m in dbContext.AssetTypes.Where(x => x.RowKey == id)
                     select new AssetPurchaseDetailsViewModel
                     {
                         CGSTPer = m.CGSTPer,
                         SGSTPer = m.SGSTPer,
                         IGSTPer = m.IGSTPer,
                         IsTax = m.IsTax,
                         DepreciationMethodKey = m.DepreciationMethodKey
                     }).FirstOrDefault();
            return model;

        }
        public AssetPurchaseDetailsViewModel FillRateTypes(AssetPurchaseDetailsViewModel model)
        {
            model.RateTypes.Add(new SelectListModel
            {
                RowKey = DbConstants.RateTypes.Numbers,
                Text = EduSuiteUIResources.Num
            });
            return model;

        }
        public AssetPurchaseMasterViewModel CheckBillNumberExists(string BillNumber, long? RowKey)
        {
            AssetPurchaseMasterViewModel model = new AssetPurchaseMasterViewModel();
            if (dbContext.AssetPurchaseMasters.Where(row => row.BillNo.ToUpper() == BillNumber.ToUpper() && row.RowKey != RowKey).Any())
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
        
    }
}
