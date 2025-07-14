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
    public class DepreciationService : IDepreciationService
    {
        private EduSuiteDatabase dbContext;
        public DepreciationService(EduSuiteDatabase objDB)
        {
            this.dbContext = objDB;
        }
        public DepreciationViewModel GetDepreciationById(long? id, long? AssetDetailKey, int? Period)
        {
            try
            {
                DepreciationViewModel model = new DepreciationViewModel();
                model = dbContext.DepreciationDetails.Select(row => new DepreciationViewModel
                {
                    RowKey = row.RowKey,
                    AssetDetailsKey = row.AssetDetailsKey,
                    AssetName = row.AssetPurchaseDetail.AssetType.AssetTypeName + " " + row.AssetPurchaseDetail.Description,
                    PostDate = row.PostDate,
                    Period = row.Period,
                    PeriodName = row.Period + " " + row.AssetPurchaseDetail.PeriodType.PeriodTypeName,
                    Depreciation = row.Depreciation,
                    ProductionUnit = row.ProductionUnit,
                    BookValue = row.BookValue,
                    ProductionLimit = row.AssetPurchaseDetail.ProductionLimit,
                    DepreciationMethodKey = row.AssetPurchaseDetail.DepreciationMethodKey,
                    PurchaseAmount = row.AssetPurchaseDetail.Amount,
                    OldProduction = dbContext.DepreciationDetails.Where(x => x.AssetDetailsKey == row.AssetDetailsKey).Select(x => x.ProductionUnit ?? 0).DefaultIfEmpty().Sum() - row.ProductionUnit
                }).Where(x => x.RowKey == id).FirstOrDefault();
                if (model == null)
                {
                    model = DepreciationByPeriod(AssetDetailKey, Period ?? 0);
                }
                return model;
            }
            catch (Exception ex)
            {
                DepreciationViewModel model = new DepreciationViewModel();
                ActivityLog.CreateActivityLog(MenuConstants.Depreciation, ((id ?? 0) != 0 ? ActionConstants.Edit : ActionConstants.Add), DbConstants.LogType.Error, id, ex.GetBaseException().Message);
                return model;
            }
        }
        public DepreciationViewModel CreateDepreciation(DepreciationViewModel model)
        {
            DepreciationDetail DepreciationModel = new DepreciationDetail();

            using (var transaction = dbContext.Database.BeginTransaction())
            {
                try
                {
                    long MaxKey = dbContext.DepreciationDetails.Select(p => p.RowKey).DefaultIfEmpty().Max();

                    DepreciationModel.RowKey = Convert.ToInt64(MaxKey + 1);
                    DepreciationModel.Depreciation = model.Depreciation;
                    DepreciationModel.Period = model.Period;
                    DepreciationModel.AssetDetailsKey = model.AssetDetailsKey;
                    DepreciationModel.PostDate = model.PostDate;
                    DepreciationModel.ProductionUnit = model.ProductionUnit;
                    DepreciationModel.BookValue = model.BookValue;
                    model.RowKey = DepreciationModel.RowKey;

                    dbContext.DepreciationDetails.Add(DepreciationModel);
                    AssetPurchaseDetail assetPurchaseDetail = dbContext.AssetPurchaseDetails.SingleOrDefault(x => x.RowKey == model.AssetDetailsKey);
                    assetPurchaseDetail.Depreciation = (assetPurchaseDetail.Depreciation ?? 0) + model.Depreciation;
                    List<AccountFlowViewModel> accountFlowModelList = new List<AccountFlowViewModel>();
                    accountFlowModelList = DepreciationAmountList(model, accountFlowModelList, false, assetPurchaseDetail.AssetPurchaseMaster.BranchKey);
                    CreateAccountFlow(accountFlowModelList, false);

                    dbContext.SaveChanges();
                    transaction.Commit();
                    model.RowKey = DepreciationModel.RowKey;
                    model.Message = EduSuiteUIResources.Success;
                    model.IsSuccessful = true;
                    ActivityLog.CreateActivityLog(MenuConstants.Depreciation, ActionConstants.Add, DbConstants.LogType.Info, DepreciationModel.RowKey, model.Message);
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    model.Message = String.Format(EduSuiteUIResources.FailedToSaveErrorMessage, EduSuiteUIResources.Depreciation);
                    model.IsSuccessful = false;
                    ActivityLog.CreateActivityLog(MenuConstants.Depreciation, ActionConstants.Add, DbConstants.LogType.Info, DepreciationModel.RowKey, ex.GetBaseException().Message);
                }
            }
            return model;
        }
        public DepreciationViewModel UpdateDepreciation(DepreciationViewModel model)
        {
            DepreciationDetail DepreciationModel = new DepreciationDetail();
            using (var transaction = dbContext.Database.BeginTransaction())
            {
                try
                {
                    DepreciationModel = dbContext.DepreciationDetails.SingleOrDefault(row => row.RowKey == model.RowKey);

                    decimal? oldDepreciation = DepreciationModel.Depreciation;
                    DepreciationModel.Depreciation = model.Depreciation;
                    DepreciationModel.Period = model.Period;
                    DepreciationModel.AssetDetailsKey = model.AssetDetailsKey;
                    DepreciationModel.PostDate = model.PostDate;
                    DepreciationModel.ProductionUnit = model.ProductionUnit;
                    DepreciationModel.BookValue = model.BookValue;

                    AssetPurchaseDetail assetPurchaseDetail = dbContext.AssetPurchaseDetails.SingleOrDefault(x => x.RowKey == model.AssetDetailsKey);
                    assetPurchaseDetail.Depreciation = (assetPurchaseDetail.Depreciation ?? 0) - (oldDepreciation ?? 0) + model.Depreciation;
                    List<AccountFlowViewModel> accountFlowModelList = new List<AccountFlowViewModel>();
                    accountFlowModelList = DepreciationAmountList(model, accountFlowModelList, true, assetPurchaseDetail.AssetPurchaseMaster.BranchKey);
                    CreateAccountFlow(accountFlowModelList, true);

                    dbContext.SaveChanges();
                    transaction.Commit();
                    model.Message = EduSuiteUIResources.Success;
                    model.IsSuccessful = true;
                    ActivityLog.CreateActivityLog(MenuConstants.Depreciation, ActionConstants.Edit, DbConstants.LogType.Info, model.RowKey, model.Message);

                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    model.Message = String.Format(EduSuiteUIResources.FailedToSaveErrorMessage, EduSuiteUIResources.Depreciation);
                    model.IsSuccessful = false;
                    ActivityLog.CreateActivityLog(MenuConstants.Depreciation, ActionConstants.Edit, DbConstants.LogType.Error, model.RowKey, ex.GetBaseException().Message);
                }
            }
            return model;

        }
        public DepreciationViewModel DeleteDepreciation(DepreciationViewModel model)
        {
            using (var transaction = dbContext.Database.BeginTransaction())
            {
                try
                {
                    DepreciationDetail Depreciation = dbContext.DepreciationDetails.SingleOrDefault(row => row.RowKey == model.RowKey);
                    AssetPurchaseDetail assetPurchaseDetail = dbContext.AssetPurchaseDetails.SingleOrDefault(x => x.RowKey == Depreciation.AssetDetailsKey);
                    assetPurchaseDetail.Depreciation = (assetPurchaseDetail.Depreciation ?? 0) - Depreciation.Depreciation;
                    List<AccountFlow> AccountList = dbContext.AccountFlows.Where(x => x.TransactionKey == model.RowKey && x.TransactionTypeKey == DbConstants.TransactionType.Depreciation).ToList();
                    dbContext.AccountFlows.RemoveRange(AccountList);
                    dbContext.DepreciationDetails.Remove(Depreciation);
                    dbContext.SaveChanges();
                    transaction.Commit();
                    model.Message = EduSuiteUIResources.Success;
                    model.IsSuccessful = true;
                    ActivityLog.CreateActivityLog(MenuConstants.Depreciation, ActionConstants.Delete, DbConstants.LogType.Info, model.RowKey, model.Message);
                }
                catch (DbUpdateException ex)
                {
                    transaction.Rollback();
                    if (ex.GetBaseException().Message.ToUpper().Contains(EduSuiteUIResources.ForeignKeyError.ToUpper()))
                    {
                        model.Message = String.Format(EduSuiteUIResources.CantDeleteErrorMessage, EduSuiteUIResources.Depreciation);
                        model.IsSuccessful = false;
                        
                    }
                    ActivityLog.CreateActivityLog(MenuConstants.Depreciation, ActionConstants.Delete, DbConstants.LogType.Error, model.RowKey, ex.GetBaseException().Message);
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    model.Message = String.Format(EduSuiteUIResources.FailedToDeleteErrorMessage, EduSuiteUIResources.Depreciation);
                    model.IsSuccessful = false;
                    ActivityLog.CreateActivityLog(MenuConstants.Depreciation, ActionConstants.Delete, DbConstants.LogType.Error, model.RowKey, ex.GetBaseException().Message);
                }
            }
            return model;
        }
        public List<DepreciationViewModel> GetDepreciation(string searchText, DepreciationViewModel model)
        {
            try
            {
                var Take = model.PageSize;
                var Skip = (model.PageIndex - 1) * model.PageSize;

                IQueryable<DepreciationViewModel> DepreciationList = (from p in dbContext.AssetPurchaseDetails.Where(x => x.AssetType.HaveDepreciation)
                                                                      orderby p.RowKey
                                                                      where (p.AssetType.AssetTypeName.Contains(searchText) || p.Description.Contains(searchText))
                                                                      select new DepreciationViewModel
                                                                      {
                                                                          RowKey = p.RowKey,
                                                                          AssetDetailsKey = p.RowKey,
                                                                          AssetName = p.AssetType.AssetTypeName + " " + p.Description,
                                                                          Depreciation = p.Depreciation ?? 0,
                                                                          BookValue = p.Amount - (p.Depreciation ?? 0),
                                                                      });
                if (model.SortBy != "")
                {
                    DepreciationList = SortSalesOrder(DepreciationList, model.SortBy, model.SortOrder);
                }
                model.TotalRecords = DepreciationList.Count();
                return model.PageIndex != 0 ? DepreciationList.Skip(Skip).Take(Take).ToList() : DepreciationList.ToList();
            }
            catch (Exception ex)
            {
                ActivityLog.CreateActivityLog(MenuConstants.Depreciation, ActionConstants.MenuAccess, DbConstants.LogType.Error, DbConstants.User.UserKey, ex.GetBaseException().Message);
                return new List<DepreciationViewModel> ();
            }
        }
        private IQueryable<DepreciationViewModel> SortSalesOrder(IQueryable<DepreciationViewModel> Query, string SortName, string SortOrder)
        {

            string command = SortOrder == "desc" ? "OrderByDescending" : "OrderBy";
            var type = typeof(DepreciationViewModel);
            var property = type.GetProperty(SortName);
            var parameter = Expression.Parameter(type, "p");
            var propertyAccess = Expression.MakeMemberAccess(parameter, property);
            var orderByExpression = Expression.Lambda(propertyAccess, parameter);
            var resultExpression = Expression.Call(typeof(Queryable), command, new Type[] { type, property.PropertyType },
                                          Query.Expression, Expression.Quote(orderByExpression));
            return Query.Provider.CreateQuery<DepreciationViewModel>(resultExpression);

        }
        private DepreciationViewModel DepreciationByPeriod(long? id, int Period)
        {
            try
            {
                DepreciationViewModel model = new DepreciationViewModel();
                List<int> Periods = Enumerable.Range(1, Period).ToList();
                if (model.DepreciationMethodKey == DbConstants.DepreciationMethod.StrightLine)
                {
                    model = (from p in Periods.Where(x => x == Period)
                             join row in dbContext.AssetPurchaseDetails.Where(x => x.RowKey == id) on p equals Period into dj
                             from row in dj.DefaultIfEmpty()
                             select new DepreciationViewModel
                             {
                                 RowKey = 0,
                                 AssetDetailsKey = row.RowKey,
                                 AssetName = row.AssetType.AssetTypeName + " " + row.Description,
                                 Period = Period,
                                 PeriodName = (Period) + " " + row.PeriodType.PeriodTypeName,
                                 Depreciation = row.Amount / row.LifePeriod,
                                 PurchaseAmount = row.Amount,
                                 BookValue = row.Amount - (row.Depreciation ?? 0),
                                 PostDate = (row.PeriodTypeKey == DbConstants.PeriodType.Year ? (row.AssetPurchaseMaster.BillDate).AddYears(Period) :
                                                                               row.PeriodTypeKey == DbConstants.PeriodType.Month ? (row.AssetPurchaseMaster.BillDate).AddMonths(Period) :
                                                                               row.PeriodTypeKey == DbConstants.PeriodType.Week ? (row.AssetPurchaseMaster.BillDate).AddDays(((Period) * 7)) :
                                                                               (row.AssetPurchaseMaster.BillDate).AddDays(Period))
                             }).FirstOrDefault();
                }
                else
                {
                    model = dbContext.AssetPurchaseDetails.Where(x => x.RowKey == id).Select(x => new DepreciationViewModel
                    {
                        AssetDetailsKey = x.RowKey,
                        PurchaseAmount = x.Amount,
                        PeriodTypeKey = x.PeriodTypeKey,
                        PeriodName = x.PeriodType.PeriodTypeName,
                        PurchaseDate = x.AssetPurchaseMaster.BillDate,
                        DepreciationMethodKey = x.DepreciationMethodKey,
                        Period = x.LifePeriod,
                        AssetName = x.AssetType.AssetTypeName + " " + x.Description,
                        ProductionLimit = x.ProductionLimit
                    }).FirstOrDefault();
                    DepreciationDetails(model);
                    model = model.DepreciationList.Where(x => x.Period == Period).Select(x => new DepreciationViewModel
                    {
                        RowKey = x.RowKey,
                        AssetDetailsKey = id ?? 0,
                        AssetName = model.AssetName,
                        Period = x.Period,
                        PeriodName = x.PeriodName,
                        Depreciation = x.Depreciation,
                        BookValue = x.BookValue,
                        PostDate = x.PostDate,
                        ProductionLimit = model.ProductionLimit,
                        PurchaseAmount = model.PurchaseAmount,
                        DepreciationMethodKey = model.DepreciationMethodKey,
                        OldProduction = dbContext.DepreciationDetails.Where(y => y.AssetDetailsKey == id).Select(y => y.ProductionUnit ?? 0).DefaultIfEmpty().Sum()
                    }).FirstOrDefault();
                }
                if (model == null)
                {
                    model = new DepreciationViewModel();
                }
                DepreciationDetails(model);
                return model;
            }
            catch (Exception ex)
            {
                DepreciationViewModel model = new DepreciationViewModel();
                ActivityLog.CreateActivityLog(MenuConstants.Depreciation, ActionConstants.View, DbConstants.LogType.Error, id, ex.GetBaseException().Message);
                return model;
            }
        }
        public DepreciationViewModel ViewDepreciation(long? id)
        {
            try
            {
                DepreciationViewModel model = new DepreciationViewModel();
                model = dbContext.AssetPurchaseDetails.Where(x => x.RowKey == id).Select(row => new DepreciationViewModel
                {
                    AssetDetailsKey = row.RowKey,
                    AssetName = row.AssetType.AssetTypeName + " " + row.Description,
                    Depreciation = row.Depreciation ?? 0,
                    PurchaseAmount = row.Amount,
                    PurchaseDate = row.AssetPurchaseMaster.BillDate,
                    BookValue = row.Amount - (row.Depreciation ?? 0),
                    PeriodTypeKey = row.PeriodTypeKey,
                    PeriodName = row.PeriodType.PeriodTypeName,
                    Period = row.LifePeriod,
                    DepreciationMethodKey = row.DepreciationMethodKey
                }).FirstOrDefault();
                if (model == null)
                {
                    model = new DepreciationViewModel();
                }
                DepreciationDetails(model);
                return model;
            }
            catch (Exception ex)
            {
                DepreciationViewModel model = new DepreciationViewModel();
                ActivityLog.CreateActivityLog(MenuConstants.Depreciation, ActionConstants.View, DbConstants.LogType.Error, id, ex.GetBaseException().Message);
                return model;
            }
        }
        private void DepreciationDetails(DepreciationViewModel model)
        {
            try
            {
                List<int> Periods = Enumerable.Range(1, model.Period).ToList();
                if (model.DepreciationMethodKey == DbConstants.DepreciationMethod.StrightLine)
                {
                    model.DepreciationList = (from p in Periods
                                              join d in dbContext.DepreciationDetails.Where(x => x.AssetDetailsKey == model.AssetDetailsKey) on p equals d.Period into dj
                                              from d in dj.DefaultIfEmpty()
                                              select new DepreciationDetailsViewModel
                                              {
                                                  RowKey = d == null ? 0 : d.RowKey,
                                                  Depreciation = (model.PurchaseAmount ?? 0) / model.Period,
                                                  BookValue = (model.PurchaseAmount ?? 0) - (p * ((model.PurchaseAmount ?? 0) / model.Period)),
                                                  AccumulatedDepreciation = (p * ((model.PurchaseAmount ?? 0) / model.Period)),
                                                  PeriodName = p + " " + model.PeriodName,
                                                  Period = p,
                                                  PostDate = d == null ? (model.PeriodTypeKey == DbConstants.PeriodType.Year ? (model.PurchaseDate ?? DateTimeUTC.Now).AddYears(p) :
                                                                                model.PeriodTypeKey == DbConstants.PeriodType.Month ? (model.PurchaseDate ?? DateTimeUTC.Now).AddMonths(p) :
                                                                                model.PeriodTypeKey == DbConstants.PeriodType.Week ? (model.PurchaseDate ?? DateTimeUTC.Now).AddDays((p * 7)) :
                                                                                (model.PurchaseDate ?? DateTimeUTC.Now).AddDays(p)) : d.PostDate
                                              }).ToList();
                }
                else if (model.DepreciationMethodKey == DbConstants.DepreciationMethod.Doubledecliningbalance)
                {
                    model.BookValue = model.PurchaseAmount ?? 0;
                    decimal depreciationRate = (100 / model.Period) * 2;
                    for (int i = 0; i < model.Period; i++)
                    {
                        decimal depreciation = ((model.BookValue ?? 0) * depreciationRate) / 100;
                        model.BookValue = (model.BookValue ?? 0) - depreciation;
                        model.DepreciationList.Add((from p in Periods.Where(x => x == (i + 1))
                                                    join d in dbContext.DepreciationDetails.Where(x => x.AssetDetailsKey == model.AssetDetailsKey) on p equals d.Period into dj
                                                    from d in dj.DefaultIfEmpty()
                                                    select new DepreciationDetailsViewModel
                                                    {
                                                        RowKey = d == null ? 0 : d.RowKey,
                                                        Depreciation = depreciation,
                                                        BookValue = model.BookValue,
                                                        AccumulatedDepreciation = (model.PurchaseAmount ?? 0) - (model.BookValue ?? 0),
                                                        PeriodName = p + " " + model.PeriodName,
                                                        Period = p,
                                                        PostDate = d == null ? (model.PeriodTypeKey == DbConstants.PeriodType.Year ? (model.PurchaseDate ?? DateTimeUTC.Now).AddYears(p) :
                                                                                      model.PeriodTypeKey == DbConstants.PeriodType.Month ? (model.PurchaseDate ?? DateTimeUTC.Now).AddMonths(p) :
                                                                                      model.PeriodTypeKey == DbConstants.PeriodType.Week ? (model.PurchaseDate ?? DateTimeUTC.Now).AddDays((p * 7)) :
                                                                                      (model.PurchaseDate ?? DateTimeUTC.Now).AddDays(p)) : d.PostDate
                                                    }).FirstOrDefault());
                    }
                }
                else if (model.DepreciationMethodKey == DbConstants.DepreciationMethod.Sumofyearsdigits)
                {
                    model.BookValue = model.PurchaseAmount ?? 0;
                    decimal depreciationRate = (100 / model.Period) * 2;
                    int sumofdigit = Periods.Sum();
                    for (int i = 0; i < model.Period; i++)
                    {
                        decimal depreciation = ((model.Period - i) / sumofdigit) * (model.PurchaseAmount ?? 0);
                        model.BookValue = (model.BookValue ?? 0) - depreciation;
                        model.DepreciationList.Add((from p in Periods.Where(x => x == (i + 1))
                                                    join d in dbContext.DepreciationDetails.Where(x => x.AssetDetailsKey == model.AssetDetailsKey) on p equals d.Period into dj
                                                    from d in dj.DefaultIfEmpty()
                                                    select new DepreciationDetailsViewModel
                                                    {
                                                        RowKey = d == null ? 0 : d.RowKey,
                                                        Depreciation = depreciation,
                                                        BookValue = model.BookValue,
                                                        AccumulatedDepreciation = (model.PurchaseAmount ?? 0) - (model.BookValue ?? 0),
                                                        PeriodName = p + " " + model.PeriodName,
                                                        Period = p,
                                                        PostDate = d == null ? (model.PeriodTypeKey == DbConstants.PeriodType.Year ? (model.PurchaseDate ?? DateTimeUTC.Now).AddYears(p) :
                                                                                      model.PeriodTypeKey == DbConstants.PeriodType.Month ? (model.PurchaseDate ?? DateTimeUTC.Now).AddMonths(p) :
                                                                                      model.PeriodTypeKey == DbConstants.PeriodType.Week ? (model.PurchaseDate ?? DateTimeUTC.Now).AddDays((p * 7)) :
                                                                                      (model.PurchaseDate ?? DateTimeUTC.Now).AddDays(p)) : d.PostDate
                                                    }).FirstOrDefault());
                    }
                }
                else if (model.DepreciationMethodKey == DbConstants.DepreciationMethod.Unitsofproduction)
                {
                    model.DepreciationList = (from p in Periods
                                              join d in dbContext.DepreciationDetails.Where(x => x.AssetDetailsKey == model.AssetDetailsKey) on p equals d.Period into dj
                                              from d in dj.DefaultIfEmpty()
                                              select new DepreciationDetailsViewModel
                                              {
                                                  RowKey = d == null ? 0 : d.RowKey,
                                                  Depreciation = d == null ? 0 : d.Depreciation,
                                                  BookValue = d == null ? 0 : d.BookValue,
                                                  AccumulatedDepreciation = d == null ? 0 : ((model.PurchaseAmount ?? 0) - (d.BookValue ?? 0)),
                                                  PeriodName = p + " " + model.PeriodName,
                                                  Period = p,
                                                  PostDate = d == null ? (model.PeriodTypeKey == DbConstants.PeriodType.Year ? (model.PurchaseDate ?? DateTimeUTC.Now).AddYears(p) :
                                                                                model.PeriodTypeKey == DbConstants.PeriodType.Month ? (model.PurchaseDate ?? DateTimeUTC.Now).AddMonths(p) :
                                                                                model.PeriodTypeKey == DbConstants.PeriodType.Week ? (model.PurchaseDate ?? DateTimeUTC.Now).AddDays((p * 7)) :
                                                                                (model.PurchaseDate ?? DateTimeUTC.Now).AddDays(p)) : d.PostDate
                                              }).ToList();
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }


        #region Account
        private void CreateAccountFlow(List<AccountFlowViewModel> accountFlowModelList, bool isUpadte)
        {
            AccountFlowService accounFlowService = new AccountFlowService(dbContext);
            if (isUpadte == false)
            {
                accounFlowService.CreateAccountFlow(accountFlowModelList);
            }
            else
            {
                accounFlowService.UpdateAccountFlow(accountFlowModelList);
            }
        }
        private List<AccountFlowViewModel> DepreciationAmountList(DepreciationViewModel model, List<AccountFlowViewModel> accountFlowModelList, bool IsUpdate, short branchKey)
        {
            long accountHeadKey;
            long ExtraUpdateKey = 0;
            accountHeadKey = dbContext.VwAccountHeadSelectActiveOnlies.Where(x => x.RowKey == DbConstants.AccountHead.Depreciation).Select(x => x.RowKey).FirstOrDefault();
            accountFlowModelList.Add(new AccountFlowViewModel
            {
                CashFlowTypeKey = DbConstants.CashFlowType.In,
                AccountHeadKey = accountHeadKey,
                Amount = model.Depreciation,
                TransactionTypeKey = DbConstants.TransactionType.Depreciation,
                VoucherTypeKey = DbConstants.VoucherType.Depreciation,
                TransactionDate = model.PostDate,
                ExtraUpdateKey = ExtraUpdateKey,
                IsUpdate = IsUpdate,
                Purpose = model.AssetName + EduSuiteUIResources.BlankSpace + model.PeriodName + EduSuiteUIResources.BlankSpace + EduSuiteUIResources.Depreciation,
                BranchKey = branchKey,
                TransactionKey = model.RowKey,
            });
            accountHeadKey = dbContext.VwAccountHeadSelectActiveOnlies.Where(x => x.RowKey == DbConstants.AccountHead.AccumulatedDepreciation).Select(x => x.RowKey).FirstOrDefault();
            accountFlowModelList.Add(new AccountFlowViewModel
            {
                CashFlowTypeKey = DbConstants.CashFlowType.Out,
                AccountHeadKey = accountHeadKey,
                Amount = model.Depreciation,
                TransactionTypeKey = DbConstants.TransactionType.Depreciation,
                VoucherTypeKey = DbConstants.VoucherType.Depreciation,
                TransactionDate = model.PostDate,
                ExtraUpdateKey = ExtraUpdateKey,
                IsUpdate = IsUpdate,
                Purpose = model.AssetName + EduSuiteUIResources.BlankSpace + model.PeriodName + EduSuiteUIResources.BlankSpace + EduSuiteUIResources.Depreciation,
                BranchKey = branchKey,
                TransactionKey = model.RowKey,
            });
            return accountFlowModelList;
        }
        #endregion
    }
}
