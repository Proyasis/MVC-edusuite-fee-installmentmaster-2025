using CITS.EduSuite.Business.Common;
using CITS.EduSuite.Business.Interfaces;
using CITS.EduSuite.Business.Models.Resources;
using CITS.EduSuite.Business.Models.ViewModels;
using CITS.EduSuite.Data;
using System;
using System.Collections.Generic;
using System.Data.Entity.Infrastructure;
using System.Data.Entity.Validation;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CITS.EduSuite.Business.Services
{
    public class AssetService : IAssetService
    {
        private EduSuiteDatabase dbContext;
        public AssetService(EduSuiteDatabase objDB)
        {
            this.dbContext = objDB;
        }
        public AssetViewModel GetAssetById(long? id)
        {
            try
            {
                AssetViewModel model = new AssetViewModel();
                model = dbContext.AssetDetails.Select(row => new AssetViewModel
                {
                    MasterKey = row.AssetTypeKey,
                    AssetTypeKey = row.AssetTypeKey,
                }).Where(x => x.AssetTypeKey == id).Distinct().FirstOrDefault();
                if (model == null)
                {
                    model = new AssetViewModel();
                }
                AssetDetailList(model);
                FillPeriodType(model.AssetDetailList);
                FillDropDown(model);
                return model;
            }
            catch (Exception ex)
            {
                ActivityLog.CreateActivityLog(MenuConstants.Asset, ((id ?? 0) != 0 ? ActionConstants.Edit : ActionConstants.Add), DbConstants.LogType.Error, id, ex.GetBaseException().Message);
                return new AssetViewModel();
            }
        }
        private List<AssetDetailsViewModel> AssetDetailList(AssetViewModel AssetModel)
        {
            AssetModel.AssetDetailList = dbContext.AssetDetails.Where(x => x.AssetTypeKey == AssetModel.AssetTypeKey).Select(x => new AssetDetailsViewModel
            {
                RowKey = x.RowKey,
                PeriodTypeKey = x.PeriodTypeKey,
                LifePeriod = x.LifePeriod,
                PurchasingDate = x.PurchaseDate,
                AssetDetailName = x.AssetDetailName,
                AssetDetailCode = x.AssetDetailCode,
                PeriodTypeName = x.PeriodType.PeriodTypeName,
                IsActive = x.IsActive,
                Amount = x.Amount,
                AccumulateDepreciation = (System.Data.Entity.DbFunctions.DiffMonths(x.PurchaseDate, DateTimeUTC.Now)) * (x.PeriodTypeKey == DbConstants.PeriodType.Year ? (x.Amount) / ((x.LifePeriod * 12) == 0 ? 1 : (x.LifePeriod * 12)) : x.PeriodTypeKey == DbConstants.PeriodType.Month ? (x.Amount) / ((x.LifePeriod) == 0 ? 1 : (x.LifePeriod)) : (x.Amount) / ((x.LifePeriod / 30) == 0 ? 1 : (x.LifePeriod / 30))),
                BookValue = (x.Amount) - (System.Data.Entity.DbFunctions.DiffMonths(x.PurchaseDate, DateTimeUTC.Now)) * (x.PeriodTypeKey == DbConstants.PeriodType.Year ? (x.Amount) / ((x.LifePeriod * 12) == 0 ? 1 : (x.LifePeriod * 12)) : x.PeriodTypeKey == DbConstants.PeriodType.Month ? (x.Amount) / ((x.LifePeriod) == 0 ? 1 : (x.LifePeriod)) : (x.Amount) / ((x.LifePeriod / 30) == 0 ? 1 : (x.LifePeriod / 30)))
            }).ToList();
            if (AssetModel.AssetDetailList.Count == 0)
            {
                AssetModel.AssetDetailList.Add(new AssetDetailsViewModel());
            }
            return AssetModel.AssetDetailList;
        }
        public AssetViewModel CreateAsset(AssetViewModel model)
        {
            FillDropDown(model);
            using (var transaction = dbContext.Database.BeginTransaction())
            {
                try
                {

                    CreateAssetDetails(model.AssetDetailList.Where(x => x.RowKey == 0 || x.RowKey == null).ToList(), model);
                    dbContext.SaveChanges();
                    transaction.Commit();
                    model.Message = EduSuiteUIResources.Success;
                    model.IsSuccessful = true;
                    ActivityLog.CreateActivityLog(MenuConstants.Asset, ActionConstants.Add, DbConstants.LogType.Info, 0, model.Message);
                }

                catch (Exception ex)
                {
                    transaction.Rollback();
                    model.Message = String.Format(EduSuiteUIResources.FailedToSaveErrorMessage, EduSuiteUIResources.AssetDetails);
                    model.IsSuccessful = false;
                    ActivityLog.CreateActivityLog(MenuConstants.AccountHead, ActionConstants.Add, DbConstants.LogType.Error, 0, ex.GetBaseException().Message);
                }
            }
            return model;
        }
        private void CreateAssetDetails(List<AssetDetailsViewModel> AssetDetails, AssetViewModel model)
        {
            List<AccountFlowViewModel> accountFlowModelList = new List<AccountFlowViewModel>();
            long maxKey = dbContext.AssetDetails.Select(x => x.RowKey).DefaultIfEmpty().Max();
            maxKey = maxKey + 1;
            foreach (AssetDetailsViewModel assetDetailItem in AssetDetails)
            {
                AssetDetail assetDetailModel = new AssetDetail();
                assetDetailModel.RowKey = maxKey;
                assetDetailModel.AssetTypeKey = model.AssetTypeKey;
                assetDetailModel.BranchKey = model.BranchKey;
                assetDetailModel.PeriodTypeKey = assetDetailItem.PeriodTypeKey;
                assetDetailModel.LifePeriod = assetDetailItem.LifePeriod ?? 0;
                assetDetailModel.PurchaseDate = assetDetailItem.PurchasingDate;
                assetDetailModel.IsActive = assetDetailItem.IsActive;
                assetDetailModel.AssetDetailCode = assetDetailItem.AssetDetailCode;
                assetDetailModel.AssetDetailName = assetDetailItem.AssetDetailName;
                assetDetailModel.Amount = assetDetailItem.Amount ?? 0;
                dbContext.AssetDetails.Add(assetDetailModel);
                accountFlowModelList = AssetAmountList(assetDetailModel, accountFlowModelList, false);
                maxKey++;
            }
            if (accountFlowModelList.Count > 0)
            {
                AccountFlowService accounFlowService = new AccountFlowService(dbContext);
                accounFlowService.CreateAccountFlow(accountFlowModelList);
                CreateAccountFlow(DateTimeUTC.Now);
            }

        }
        private void UpdateAssetDetails(List<AssetDetailsViewModel> AssetDetails, AssetViewModel model)
        {
            foreach (AssetDetailsViewModel assetDetailItem in AssetDetails)
            {
                AssetDetail assetDetailModel = new AssetDetail();
                assetDetailModel = dbContext.AssetDetails.SingleOrDefault(x => x.RowKey == assetDetailItem.RowKey);
                assetDetailModel.AssetTypeKey = model.AssetTypeKey;
                assetDetailModel.BranchKey = model.BranchKey;
                assetDetailModel.PeriodTypeKey = assetDetailItem.PeriodTypeKey;
                assetDetailModel.LifePeriod = assetDetailItem.LifePeriod ?? 0;
                assetDetailModel.PurchaseDate = assetDetailItem.PurchasingDate;
                assetDetailModel.AssetDetailCode = assetDetailItem.AssetDetailCode;
                assetDetailModel.AssetDetailName = assetDetailItem.AssetDetailName;
                assetDetailModel.Amount = assetDetailItem.Amount ?? 0;
                assetDetailModel.IsActive = assetDetailItem.IsActive;
            }
        }
        public AssetViewModel UpdateAsset(AssetViewModel model)
        {
            FillDropDown(model);

            AssetDetail AssetModel = new AssetDetail();
            using (var transaction = dbContext.Database.BeginTransaction())
            {
                try
                {
                    CreateAssetDetails(model.AssetDetailList.Where(x => x.RowKey == 0 || x.RowKey == null).ToList(), model);
                    //UpdateAssetDetails(model.AssetDetailList.Where(x => x.RowKey != 0 && x.RowKey != null).ToList(), model);
                    dbContext.SaveChanges();
                    transaction.Commit();
                    model.Message = EduSuiteUIResources.Success;
                    model.IsSuccessful = true;
                    ActivityLog.CreateActivityLog(MenuConstants.AccountHead, ActionConstants.Edit, DbConstants.LogType.Info, 0, model.Message);
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    model.Message = String.Format(EduSuiteUIResources.FailedToSaveErrorMessage, EduSuiteUIResources.AssetDetails);
                    model.IsSuccessful = false;
                    ActivityLog.CreateActivityLog(MenuConstants.AccountHead, ActionConstants.Edit, DbConstants.LogType.Error, 0, ex.GetBaseException().Message);
                }
            }
            return model;

        }
        public AssetViewModel DeleteAsset(AssetViewModel model)
        {
            using (var transaction = dbContext.Database.BeginTransaction())
            {
                try
                {
                    AssetDetail AssetModel = new AssetDetail();
                    List<AssetDetail> AssetList = dbContext.AssetDetails.Where(row => row.AssetTypeKey == model.AssetTypeKey).ToList();
                    AssetList.ForEach(assetDetailLst => dbContext.AssetDetails.Remove(assetDetailLst));
                    dbContext.SaveChanges();
                    transaction.Commit();
                    model.Message = EduSuiteUIResources.Success;
                    model.IsSuccessful = true;
                    ActivityLog.CreateActivityLog(MenuConstants.Asset, ActionConstants.Delete, DbConstants.LogType.Info, model.AssetTypeKey, model.Message);
                }
                catch (DbUpdateException ex)
                {
                    transaction.Rollback();
                    if (ex.GetBaseException().Message.ToUpper().Contains(EduSuiteUIResources.ForeignKeyError.ToUpper()))
                    {
                        
                        model.Message = String.Format(EduSuiteUIResources.CantDeleteErrorMessage, EduSuiteUIResources.AssetDetails);
                        model.IsSuccessful = false;
                    }
                    ActivityLog.CreateActivityLog(MenuConstants.Asset, ActionConstants.Delete, DbConstants.LogType.Error, model.AssetTypeKey, ex.GetBaseException().Message);
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    model.Message = String.Format(EduSuiteUIResources.FailedToDeleteErrorMessage, EduSuiteUIResources.AssetDetails);
                    model.IsSuccessful = false;
                    ActivityLog.CreateActivityLog(MenuConstants.Asset, ActionConstants.Delete, DbConstants.LogType.Error, model.AssetTypeKey, ex.GetBaseException().Message);
                }
            }

            return model;
        }
        public AssetViewModel DeleteAssetDetails(int Id)
        {
            AssetViewModel model = new AssetViewModel();
            using (var transaction = dbContext.Database.BeginTransaction())
            {
                try
                {
                    AssetDetail AssetModel = dbContext.AssetDetails.SingleOrDefault(row => row.RowKey == Id);
                    dbContext.AssetDetails.Remove(AssetModel);
                    dbContext.SaveChanges();
                    transaction.Commit();
                    model.Message = EduSuiteUIResources.Success;
                    model.IsSuccessful = true;
                    ActivityLog.CreateActivityLog(MenuConstants.Asset, ActionConstants.Delete, DbConstants.LogType.Info, Id, model.Message);
                }
                catch (DbUpdateException ex)
                {
                    transaction.Rollback();
                    if (ex.GetBaseException().Message.ToUpper().Contains(EduSuiteUIResources.ForeignKeyError.ToUpper()))
                    {
                        model.Message = String.Format(EduSuiteUIResources.CantDeleteErrorMessage, EduSuiteUIResources.AssetDetails);
                        model.IsSuccessful = false;
                        ActivityLog.CreateActivityLog(MenuConstants.Asset, ActionConstants.Delete, DbConstants.LogType.Error, Id, ex.GetBaseException().Message);
                    }
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    model.Message = String.Format(EduSuiteUIResources.FailedToDeleteErrorMessage, EduSuiteUIResources.AssetDetails);
                    model.IsSuccessful = false;
                    ActivityLog.CreateActivityLog(MenuConstants.Asset, ActionConstants.Delete, DbConstants.LogType.Error, Id, ex.GetBaseException().Message);
                }
            }

            return model;
        }
        public List<AssetViewModel> GetAsset(string searchText)
        {
            try
            {
                var AssetList = (from p in dbContext.AssetTypes
                                 orderby p.RowKey
                                 where (p.AssetTypeName.Contains(searchText))
                                 select new AssetViewModel
                          {
                              MasterKey = p.RowKey,
                              AccountHeadName = p.AssetTypeName,
                              AccountHeadKey = p.AccountHeadKey,
                              AssetCount = p.AssetDetails.Count

                          }).Where(x => x.AssetCount > 0).ToList();
                return AssetList.GroupBy(x => x.MasterKey).Select(y => y.First()).ToList<AssetViewModel>();
            }
            catch (Exception ex)
            {
                return new List<AssetViewModel>();

            }
        }
        private void FillDropDown(AssetViewModel model)
        {
            FillAccountHead(model);
            FillBranches(model);

        }
        public AssetViewModel FillAccountHead(AssetViewModel model)
        {
            model.AssetTypes = dbContext.AssetTypes.Where(x => x.IsActive == true && x.HaveDepreciation == true).Select(row => new SelectListModel
            {
                RowKey = row.RowKey,
                Text = row.AssetTypeName
            }).ToList();
            return model;
        }
        public AssetViewModel FillBranches(AssetViewModel model)
        {
            if (model.BranchKey == 0 || model.BranchKey == null)
            {
                model.BranchKey = dbContext.AppUsers.Where(row => row.RowKey == DbConstants.User.UserKey).Select(row => row.Employees.Select(x=>x.BranchKey).FirstOrDefault()).FirstOrDefault();
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
        private void FillPeriodType(List<AssetDetailsViewModel> model)
        {
            foreach (AssetDetailsViewModel ModelItem in model)
            {
                ModelItem.PeriodType = dbContext.PeriodTypes.Select(row => new SelectListModel
                {
                    RowKey = row.RowKey,
                    Text = row.PeriodTypeName
                }).ToList();
            }
        }
        public AssetViewModel CheckAssetDetailCode(AssetDetailsViewModel model)
        {
            AssetViewModel assetModel = new AssetViewModel();
            if (dbContext.AssetDetails.Where(x => x.AssetDetailCode.ToLower() == model.AssetDetailCode.ToLower() && x.RowKey != model.RowKey).Any())
            {
                assetModel.IsSuccessful = false;
                assetModel.Message = String.Format(EduSuiteUIResources.ExistsErrorMessage, EduSuiteUIResources.AssetCode);
            }
            else
            {
                assetModel.IsSuccessful = true;
                assetModel.Message = "";
            }
            return assetModel;
        }
        public void CreateAccountFlow(DateTime TransactionDate)
        {
            List<AssetDetail> assetDetailList = new List<AssetDetail>();
            List<AssetPurchaseDetail> assetPurchaseDetailList = new List<AssetPurchaseDetail>();
            assetDetailList = dbContext.AssetDetails.Where(x => x.Amount != x.Depreciation).ToList();
            assetPurchaseDetailList = dbContext.AssetPurchaseDetails.Where(x => x.Amount != x.Depreciation).ToList();
            List<AccountFlowViewModel> accountFlowModelList = new List<AccountFlowViewModel>();
            if (assetDetailList.Count != 0)
            {
                foreach (AssetDetail item in assetDetailList)
                {
                    if (item.PeriodTypeKey == DbConstants.PeriodType.Day)
                    {
                        double difference = (TransactionDate - item.PurchaseDate).TotalDays;
                        if (difference != 0)
                        {
                            decimal totalDepreciation = (item.Amount / item.LifePeriod) * (decimal)difference;
                            decimal depreciation = totalDepreciation - item.Depreciation;
                            if (totalDepreciation > item.Depreciation)
                            {
                                item.Depreciation = totalDepreciation;
                                accountFlowModelList = DepreciationAmountList(item, accountFlowModelList, false, item.BranchKey, Convert.ToInt64(difference), depreciation, TransactionDate);
                            }
                        }
                    }
                    else if (item.PeriodTypeKey == DbConstants.PeriodType.Month)
                    {
                        double difference = ((TransactionDate.Year - item.PurchaseDate.Year) * 12) + TransactionDate.Month - item.PurchaseDate.Month;
                        if (difference != 0)
                        {
                            decimal totalDepreciation = (item.Amount / item.LifePeriod) * (decimal)difference;
                            decimal depreciation = totalDepreciation - item.Depreciation;
                            if (totalDepreciation > item.Depreciation)
                            {
                                item.Depreciation = totalDepreciation;
                                accountFlowModelList = DepreciationAmountList(item, accountFlowModelList, false, item.BranchKey, Convert.ToInt64(difference), depreciation, TransactionDate);
                            }
                        }
                    }
                    else if (item.PeriodTypeKey == DbConstants.PeriodType.Year)
                    {
                        double difference = (TransactionDate.Year - item.PurchaseDate.Year);
                        if (difference != 0)
                        {
                            decimal totalDepreciation = (item.Amount / item.LifePeriod) * (decimal)difference;
                            decimal depreciation = totalDepreciation - item.Depreciation;
                            if (totalDepreciation > item.Depreciation)
                            {
                                item.Depreciation = totalDepreciation;
                                accountFlowModelList = DepreciationAmountList(item, accountFlowModelList, false, item.BranchKey, Convert.ToInt64(difference), depreciation, TransactionDate);
                            }
                        }
                    }
                }

            }
            if (assetPurchaseDetailList.Count != 0)
            {
                foreach (AssetPurchaseDetail item in assetPurchaseDetailList)
                {
                    if (item.PeriodTypeKey == DbConstants.PeriodType.Day)
                    {
                        double difference = (TransactionDate - item.AssetPurchaseMaster.BillDate).TotalDays;
                        if (difference != 0)
                        {
                            decimal totalDepreciation = ((item.RowTotal ?? 0) / item.LifePeriod) * (decimal)difference;
                            decimal depreciation = totalDepreciation - (item.Depreciation ?? 0);
                            if (totalDepreciation > item.Depreciation)
                            {
                                item.Depreciation = totalDepreciation;
                                accountFlowModelList = PurchaseDepreciationAmountList(item, accountFlowModelList, false, item.AssetPurchaseMaster.BranchKey, Convert.ToInt64(difference), depreciation, TransactionDate);
                            }
                        }
                    }
                    else if (item.PeriodTypeKey == DbConstants.PeriodType.Month)
                    {
                        double difference = ((TransactionDate.Year - item.AssetPurchaseMaster.BillDate.Year) * 12) + TransactionDate.Month - item.AssetPurchaseMaster.BillDate.Month;
                        if (difference != 0)
                        {
                            decimal totalDepreciation = ((item.RowTotal ?? 0) / item.LifePeriod) * (decimal)difference;
                            decimal depreciation = totalDepreciation - (item.Depreciation ?? 0);
                            if (totalDepreciation > item.Depreciation)
                            {
                                item.Depreciation = totalDepreciation;
                                accountFlowModelList = PurchaseDepreciationAmountList(item, accountFlowModelList, false, item.AssetPurchaseMaster.BranchKey, Convert.ToInt64(difference), depreciation, TransactionDate);
                            }
                        }
                    }
                    else if (item.PeriodTypeKey == DbConstants.PeriodType.Year)
                    {
                        double difference = (TransactionDate.Year - item.AssetPurchaseMaster.BillDate.Year);
                        if (difference != 0)
                        {
                            decimal totalDepreciation = ((item.RowTotal ?? 0) / item.LifePeriod) * (decimal)difference;
                            decimal depreciation = totalDepreciation - (item.Depreciation ?? 0);
                            if (totalDepreciation > item.Depreciation)
                            {
                                item.Depreciation = totalDepreciation;
                                accountFlowModelList = PurchaseDepreciationAmountList(item, accountFlowModelList, false, item.AssetPurchaseMaster.BranchKey, Convert.ToInt64(difference), depreciation, TransactionDate);
                            }
                        }
                    }
                }
            }
            if (accountFlowModelList.Count > 0)
            {
                AccountFlowService accounFlowService = new AccountFlowService(dbContext);
                accounFlowService.CreateAccountFlow(accountFlowModelList);
            }
        }
        private List<AccountFlowViewModel> DepreciationAmountList(AssetDetail model, List<AccountFlowViewModel> accountFlowModelList, bool IsUpdate, short branchKey, long RowKey, decimal depreciation, DateTime TransactionDate)
        {
            long accountHeadKey;
            long ExtraUpdateKey = 0;
            accountHeadKey = dbContext.VwAccountHeadSelectActiveOnlies.Where(x => x.RowKey == DbConstants.AccountHead.Depreciation).Select(x => x.RowKey).FirstOrDefault();
            accountFlowModelList.Add(new AccountFlowViewModel
            {
                CashFlowTypeKey = DbConstants.CashFlowType.In,
                AccountHeadKey = accountHeadKey,
                Amount = depreciation,
                TransactionTypeKey = DbConstants.TransactionType.Depreciation,
                VoucherTypeKey = DbConstants.VoucherType.Depreciation,
                TransactionDate = TransactionDate,
                ExtraUpdateKey = ExtraUpdateKey,
                IsUpdate = IsUpdate,
                Purpose = model.AssetDetailName + EduSuiteUIResources.BlankSpace + model.AssetDetailCode + EduSuiteUIResources.BlankSpace + EduSuiteUIResources.Depreciation,
                BranchKey = branchKey,
                TransactionKey = RowKey,
            });
            accountHeadKey = dbContext.VwAccountHeadSelectActiveOnlies.Where(x => x.RowKey == DbConstants.AccountHead.AccumulatedDepreciation).Select(x => x.RowKey).FirstOrDefault();
            accountFlowModelList.Add(new AccountFlowViewModel
            {
                CashFlowTypeKey = DbConstants.CashFlowType.Out,
                AccountHeadKey = accountHeadKey,
                Amount = depreciation,
                TransactionTypeKey = DbConstants.TransactionType.Depreciation,
                VoucherTypeKey = DbConstants.VoucherType.Depreciation,
                TransactionDate = TransactionDate,
                ExtraUpdateKey = ExtraUpdateKey,
                IsUpdate = IsUpdate,
                Purpose = model.AssetDetailName + EduSuiteUIResources.BlankSpace + model.AssetDetailCode + EduSuiteUIResources.BlankSpace + EduSuiteUIResources.Depreciation,
                BranchKey = branchKey,
                TransactionKey = RowKey,
            });
            return accountFlowModelList;
        }
        private List<AccountFlowViewModel> PurchaseDepreciationAmountList(AssetPurchaseDetail model, List<AccountFlowViewModel> accountFlowModelList, bool IsUpdate, short branchKey, long RowKey, decimal depreciation, DateTime TransactionDate)
        {
            long accountHeadKey;
            long ExtraUpdateKey = 0;
            accountHeadKey = dbContext.VwAccountHeadSelectActiveOnlies.Where(x => x.RowKey == DbConstants.AccountHead.Depreciation).Select(x => x.RowKey).FirstOrDefault();
            accountFlowModelList.Add(new AccountFlowViewModel
            {
                CashFlowTypeKey = DbConstants.CashFlowType.In,
                AccountHeadKey = accountHeadKey,
                Amount = depreciation,
                TransactionTypeKey = DbConstants.TransactionType.PurchaseDepreciation,
                VoucherTypeKey = DbConstants.VoucherType.Depreciation,
                TransactionDate = TransactionDate,
                ExtraUpdateKey = ExtraUpdateKey,
                IsUpdate = IsUpdate,
                Purpose = model.Description + EduSuiteUIResources.BlankSpace + model.ReferenceNumber + EduSuiteUIResources.BlankSpace + EduSuiteUIResources.Depreciation,
                BranchKey = branchKey,
                TransactionKey = RowKey,
            });
            accountHeadKey = dbContext.VwAccountHeadSelectActiveOnlies.Where(x => x.RowKey == DbConstants.AccountHead.AccumulatedDepreciation).Select(x => x.RowKey).FirstOrDefault();
            accountFlowModelList.Add(new AccountFlowViewModel
            {
                CashFlowTypeKey = DbConstants.CashFlowType.Out,
                AccountHeadKey = accountHeadKey,
                Amount = depreciation,
                TransactionTypeKey = DbConstants.TransactionType.PurchaseDepreciation,
                VoucherTypeKey = DbConstants.VoucherType.Depreciation,
                TransactionDate = TransactionDate,
                ExtraUpdateKey = ExtraUpdateKey,
                IsUpdate = IsUpdate,
                Purpose = model.Description + EduSuiteUIResources.BlankSpace + model.ReferenceNumber + EduSuiteUIResources.BlankSpace + EduSuiteUIResources.Depreciation,
                BranchKey = branchKey,
                TransactionKey = RowKey,
            });
            return accountFlowModelList;
        }
        private List<AccountFlowViewModel> AssetAmountList(AssetDetail model, List<AccountFlowViewModel> accountFlowModelList, bool IsUpdate)
        {
            long accountHeadKey;
            long ExtraUpdateKey = 0;
            accountHeadKey = dbContext.VwAccountHeadSelectActiveOnlies.Where(x => x.RowKey == DbConstants.AccountHead.OpeningBalance).Select(x => x.RowKey).FirstOrDefault();
            accountFlowModelList.Add(new AccountFlowViewModel
            {
                CashFlowTypeKey = DbConstants.CashFlowType.Out,
                AccountHeadKey = accountHeadKey,
                Amount = model.Amount,
                TransactionTypeKey = DbConstants.TransactionType.Asset,
                VoucherTypeKey = DbConstants.VoucherType.Asset,
                TransactionDate = model.PurchaseDate,
                ExtraUpdateKey = ExtraUpdateKey,
                IsUpdate = IsUpdate,
                Purpose = model.AssetDetailName + EduSuiteUIResources.BlankSpace + model.AssetDetailCode + EduSuiteUIResources.BlankSpace + EduSuiteUIResources.Asset + EduSuiteUIResources.BlankSpace + EduSuiteUIResources.Purchase,
                BranchKey = model.BranchKey,
                TransactionKey = model.RowKey,
            });
            accountHeadKey = dbContext.AssetTypes.Where(x => x.RowKey == model.AssetTypeKey).Select(x => x.AccountHeadKey).FirstOrDefault();
            accountFlowModelList.Add(new AccountFlowViewModel
            {
                CashFlowTypeKey = DbConstants.CashFlowType.In,
                AccountHeadKey = accountHeadKey,
                Amount = model.Amount,
                TransactionTypeKey = DbConstants.TransactionType.Asset,
                VoucherTypeKey = DbConstants.VoucherType.Asset,
                TransactionDate = model.PurchaseDate,
                ExtraUpdateKey = ExtraUpdateKey,
                IsUpdate = IsUpdate,
                Purpose = model.AssetDetailName + EduSuiteUIResources.BlankSpace + model.AssetDetailCode + EduSuiteUIResources.BlankSpace + EduSuiteUIResources.Asset + EduSuiteUIResources.BlankSpace + EduSuiteUIResources.Purchase,
                BranchKey = model.BranchKey,
                TransactionKey = model.RowKey,
            });
            return accountFlowModelList;
        }

    }
}
