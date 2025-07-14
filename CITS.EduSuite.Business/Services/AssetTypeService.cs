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
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace CITS.EduSuite.Business.Services
{
    public class AssetTypeService : IAssetTypeService
    {
        private EduSuiteDatabase dbContext;
        public AssetTypeService(EduSuiteDatabase objDB)
        {
            this.dbContext = objDB;
        }
        public List<AssetTypeViewModel> GetAssetType(string SearchText)
        {
            try
            {
                var AssetTypeList = (from MT in dbContext.AssetTypes
                                     orderby MT.RowKey
                                     where (MT.AssetTypeName.Contains(SearchText))
                                     select new AssetTypeViewModel
                                     {
                                         RowKey = MT.RowKey,
                                         AssetTypeName = MT.AssetTypeName,
                                         IsActive = MT.IsActive,
                                         IsTax = MT.IsTax,
                                         SGSTPer = MT.SGSTPer,
                                         HSNCode = MT.HSNCode,
                                         HSNCodeName = MT.HSNCodeMaster.HSNSACCode,
                                         IGSTPer = MT.IGSTPer,
                                         CGSTPer = MT.CGSTPer,
                                     }).ToList();
                return AssetTypeList.GroupBy(x => x.RowKey).Select(y => y.First()).ToList<AssetTypeViewModel>();
            }
            catch (Exception ex)
            {
                ActivityLog.CreateActivityLog(MenuConstants.AssetType, ActionConstants.MenuAccess, DbConstants.LogType.Error, DbConstants.User.UserKey, ex.GetBaseException().Message);
                return new List<AssetTypeViewModel> ();
            }
        }
        public AssetTypeViewModel GetAssetTypeById(int? Id)
        {
            try
            {
                AssetTypeViewModel model = new AssetTypeViewModel();
                model = dbContext.AssetTypes.Select(row => new AssetTypeViewModel
                {
                    RowKey = row.RowKey,
                    AssetTypeName = row.AssetTypeName,
                    IsActive = row.IsActive,
                    IsTax = row.IsTax,
                    SGSTPer = row.SGSTPer,
                    HSNCode = row.HSNCode,
                    HSNCodeKey = row.HSNCodeKey,
                    CGSTPer = row.CGSTPer,
                    IGSTPer = row.IGSTPer,
                    HaveDepreciation = row.HaveDepreciation,
                    DepreciationMethodKey = row.DepreciationMethodKey
                }).Where(p => p.RowKey == Id).FirstOrDefault();
                if (model == null)
                {
                    model = new AssetTypeViewModel();
                }
                FillHSNCodes(model);
                FillDepreciationMethod(model);
                return model;
            }
            catch (Exception ex)
            {
                AssetTypeViewModel model = new AssetTypeViewModel();
                ActivityLog.CreateActivityLog(MenuConstants.AssetType, ((Id ?? 0) != 0 ? ActionConstants.Edit : ActionConstants.Add), DbConstants.LogType.Error, Id, ex.GetBaseException().Message);
                return model;
            }
        }
        public AssetTypeViewModel CreateAssetType(AssetTypeViewModel model)
        {
            var AssetTypeNameCheck = dbContext.AssetTypes.Where(x => x.AssetTypeName.ToLower() == model.AssetTypeName.ToLower()).ToList();
            AssetType AssetTypeModel = new AssetType();
            if (AssetTypeNameCheck.Count != 0)
            {
                model.Message = String.Format(EduSuiteUIResources.ExistsErrorMessage, EduSuiteUIResources.AssetType);
                model.IsSuccessful = false;
                return model;
            }
            using (var transaction = dbContext.Database.BeginTransaction())
            {
                try
                {
                    AccountHeadViewModel accountHeadViewModel = new AccountHeadViewModel();
                    if (model.HaveDepreciation)
                    {
                        AccountHeadService accountHeadService = new AccountHeadService(dbContext);
                        accountHeadViewModel.AccountHeadName = model.AssetTypeName;
                        accountHeadViewModel.AccountHeadTypeKey = DbConstants.AccountHeadType.FixedAssets;
                        accountHeadViewModel.IsActive = true;
                        accountHeadViewModel.IsSystemAccount = true;
                        accountHeadViewModel = accountHeadService.createAccountChart(accountHeadViewModel);
                    }
                    else
                    {
                        var accountHead = dbContext.AccountHeads.FirstOrDefault(x => x.AccountHeadName == EduSuiteUIResources.Stationary);
                        if (accountHead == null)
                        {
                            AccountHeadService accountHeadService = new AccountHeadService(dbContext);
                            accountHeadViewModel.AccountHeadName = EduSuiteUIResources.Stationary;
                            accountHeadViewModel.AccountHeadTypeKey = DbConstants.AccountHeadType.DirectExpense;
                            accountHeadViewModel.IsActive = true;
                            accountHeadViewModel.IsSystemAccount = true;
                            accountHeadViewModel = accountHeadService.createAccountChart(accountHeadViewModel);
                        }
                        else
                        {
                            accountHeadViewModel.AccountHeadCode = accountHead.AccountHeadCode;
                        }
                    }

                    int MaxRowkey = dbContext.AssetTypes.Select(x => x.RowKey).DefaultIfEmpty().Max();
                    AssetTypeModel.RowKey = (int)(MaxRowkey + 1);
                    AssetTypeModel.AssetTypeName = model.AssetTypeName;
                    AssetTypeModel.IsActive = model.IsActive;
                    AssetTypeModel.IsTax = model.IsTax;
                    AssetTypeModel.DepreciationMethodKey = model.DepreciationMethodKey;
                    AssetTypeModel.SGSTPer = model.IsTax == true ? model.SGSTPer : 0;
                    AssetTypeModel.CGSTPer = model.IsTax == true ? model.CGSTPer : 0;
                    AssetTypeModel.IGSTPer = model.IsTax == true ? model.IGSTPer : 0;
                    AssetTypeModel.HSNCode = model.HSNCode;
                    AssetTypeModel.HaveDepreciation = model.HaveDepreciation;
                    AssetTypeModel.AccountHeadKey = accountHeadViewModel.RowKey;
                    AssetTypeModel.HSNCodeKey = model.HSNCodeKey;

                    dbContext.AssetTypes.Add(AssetTypeModel);
                    dbContext.SaveChanges();
                    transaction.Commit();
                    model.RowKey = AssetTypeModel.RowKey;
                    model.Message = EduSuiteUIResources.Success;
                    model.IsSuccessful = true;
                    ActivityLog.CreateActivityLog(MenuConstants.AssetType, ActionConstants.Add, DbConstants.LogType.Info, AssetTypeModel.RowKey, model.Message);

                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    model.Message = String.Format(EduSuiteUIResources.FailedToSaveErrorMessage, EduSuiteUIResources.AssetType);
                    model.IsSuccessful = false;
                    ActivityLog.CreateActivityLog(MenuConstants.AssetType, ActionConstants.Add, DbConstants.LogType.Error, AssetTypeModel.RowKey, ex.GetBaseException().Message);
                }
                return model;
            }
        }
        public AssetTypeViewModel UpdateAssetType(AssetTypeViewModel model)
        {
            var AssetTypeCheck = dbContext.AssetTypes.Where(x => x.AssetTypeName.ToLower() == model.AssetTypeName.ToLower()
                && x.RowKey != model.RowKey).ToList();
            if (AssetTypeCheck.Count != 0)
            {
                model.Message = String.Format(EduSuiteUIResources.ExistsErrorMessage, EduSuiteUIResources.AssetType);
                model.IsSuccessful = false;
                return model;
            }
            AssetType AssetTypeModel = new AssetType();
            using (var transaction = dbContext.Database.BeginTransaction())
            {
                try
                {
                    AssetTypeModel = dbContext.AssetTypes.SingleOrDefault(row => row.RowKey == model.RowKey);
                    AccountHeadViewModel accountHeadViewModel = new AccountHeadViewModel();
                    if (AssetTypeModel.HaveDepreciation != model.HaveDepreciation)
                    {
                        if (model.HaveDepreciation)
                        {
                            AccountHeadService accountHeadService = new AccountHeadService(dbContext);
                            accountHeadViewModel.AccountHeadName = model.AssetTypeName;
                            accountHeadViewModel.AccountHeadTypeKey = DbConstants.AccountHeadType.FixedAssets;
                            accountHeadViewModel.IsActive = true;
                            accountHeadViewModel.IsSystemAccount = true;
                            accountHeadViewModel = accountHeadService.createAccountChart(accountHeadViewModel);
                        }
                        else
                        {
                            var accountHead = dbContext.AccountHeads.FirstOrDefault(x => x.AccountHeadName == EduSuiteUIResources.Stationary);
                            AccountFlowService accountFlowService = new AccountFlowService(dbContext);
                            if (accountHead == null)
                            {
                                AccountHeadService accountHeadService = new AccountHeadService(dbContext);
                                accountHeadViewModel.AccountHeadName = EduSuiteUIResources.Stationary;
                                accountHeadViewModel.AccountHeadTypeKey = DbConstants.AccountHeadType.DirectExpense;
                                accountHeadViewModel.IsActive = true;
                                accountHeadViewModel.IsSystemAccount = true;
                                accountHeadViewModel = accountHeadService.createAccountChart(accountHeadViewModel);
                            }
                            else
                            {
                                accountHeadViewModel.AccountHeadCode = accountHead.AccountHeadCode;
                            }
                            accountFlowService.DeleteAccountHead(AssetTypeModel.AccountHeadKey);
                        }
                    }
                    else
                    {
                        if (model.HaveDepreciation)
                        {
                            AccountHeadService accountHeadService = new AccountHeadService(dbContext);
                            accountHeadViewModel.AccountHeadName = model.AssetTypeName;
                            accountHeadViewModel.AccountHeadTypeKey = DbConstants.AccountHeadType.FixedAssets;
                            accountHeadViewModel.IsActive = true;
                            accountHeadViewModel.IsSystemAccount = true;
                            accountHeadViewModel.RowKey = AssetTypeModel.AccountHeadKey;
                            accountHeadViewModel.RowKey = dbContext.AccountHeads.Where(x => x.RowKey == AssetTypeModel.AccountHeadKey).Select(x => x.RowKey).FirstOrDefault();
                            accountHeadViewModel = accountHeadService.updateAccountChart(accountHeadViewModel);
                        }
                    }

                    AssetTypeModel.AssetTypeName = model.AssetTypeName;
                    AssetTypeModel.IsActive = model.IsActive;
                    AssetTypeModel.IsTax = model.IsTax;
                    AssetTypeModel.DepreciationMethodKey = model.DepreciationMethodKey;
                    AssetTypeModel.SGSTPer = model.IsTax == true ? model.SGSTPer : 0;
                    AssetTypeModel.CGSTPer = model.IsTax == true ? model.CGSTPer : 0;
                    AssetTypeModel.IGSTPer = model.IsTax == true ? model.IGSTPer : 0;
                    AssetTypeModel.HSNCode = model.HSNCode;
                    AssetTypeModel.HaveDepreciation = model.HaveDepreciation;
                    AssetTypeModel.HSNCodeKey = model.HSNCodeKey;
                    dbContext.SaveChanges();
                    transaction.Commit();
                    model.Message = EduSuiteUIResources.Success;
                    model.IsSuccessful = true;
                    ActivityLog.CreateActivityLog(MenuConstants.AssetType, ActionConstants.Edit, DbConstants.LogType.Info, model.RowKey, model.Message);
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    model.Message = String.Format(EduSuiteUIResources.FailedToSaveErrorMessage, EduSuiteUIResources.AssetType);
                    model.IsSuccessful = false;
                    ActivityLog.CreateActivityLog(MenuConstants.AssetType, ActionConstants.Edit, DbConstants.LogType.Error, model.RowKey, ex.GetBaseException().Message);

                }
                return model;
            }

        }
        public AssetTypeViewModel DeleteAssetType(AssetTypeViewModel model)
        {
            using (var transaction = dbContext.Database.BeginTransaction())
            {
                try
                {
                    AssetType AssetTypemodel = dbContext.AssetTypes.SingleOrDefault(row => row.RowKey == model.RowKey);
                    long AccountHeadKey = AssetTypemodel.AccountHeadKey;
                    dbContext.AssetTypes.Remove(AssetTypemodel);
                    AccountFlowService accountFlowService = new AccountFlowService(dbContext);
                    accountFlowService.DeleteAccountHead(AccountHeadKey);
                    dbContext.SaveChanges();
                    transaction.Commit();
                    model.Message = EduSuiteUIResources.Success;
                    model.IsSuccessful = true;
                    ActivityLog.CreateActivityLog(MenuConstants.AssetType, ActionConstants.Delete, DbConstants.LogType.Info, model.RowKey, model.Message);
                }
                catch (DbUpdateException ex)
                {
                    transaction.Rollback();
                    if (ex.GetBaseException().Message.ToUpper().Contains(EduSuiteUIResources.ForeignKeyError.ToUpper()))
                    {
                        model.Message = String.Format(EduSuiteUIResources.CantDeleteErrorMessage, EduSuiteUIResources.AssetType);
                        model.IsSuccessful = false;
                        ActivityLog.CreateActivityLog(MenuConstants.AssetType, ActionConstants.Delete, DbConstants.LogType.Error, model.RowKey, ex.GetBaseException().Message);
                    }
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    model.Message = String.Format(EduSuiteUIResources.CantDeleteErrorMessage, EduSuiteUIResources.AssetType);
                    model.IsSuccessful = false;
                    ActivityLog.CreateActivityLog(MenuConstants.AssetType, ActionConstants.Delete, DbConstants.LogType.Error, model.RowKey, ex.GetBaseException().Message);
                }
            }
            return model;
        }
        public AssetTypeViewModel CheckHSNCodeExists(string HSNCode, int RowKey)
        {
            AssetTypeViewModel model = new AssetTypeViewModel();
            if (dbContext.AssetTypes.Any(row => row.HSNCode == HSNCode && row.RowKey != RowKey))
            {
                model.IsSuccessful = false;
            }
            else
            {
                model.IsSuccessful = true;
            }
            return model;
        }
        private void FillHSNCodes(AssetTypeViewModel model)
        {
            model.HSNCodes = dbContext.HSNCodeMasters.Select(row => new SelectListModel
            {
                RowKey = row.RowKey,
                Text = row.ProductName
            }).ToList();
        }
        private void FillDepreciationMethod(AssetTypeViewModel model)
        {
            model.DepreciationMethods = dbContext.DepreciationMethods.Select(row => new SelectListModel
            {
                RowKey = row.RowKey,
                Text = row.MethodName
            }).ToList();
        }
        public HSNCodeMasterViewModel GetHSNCodeDetailsById(HSNCodeMasterViewModel model)
        {
            model = dbContext.HSNCodeMasters.Where(x => x.RowKey == model.RowKey).Select(x =>
                new HSNCodeMasterViewModel
                {
                    RowKey = x.RowKey,
                    HSNSACCode = x.HSNSACCode,
                    HSNIGSTPer = x.IGST,
                    HSNCGSTPer = x.CGST,
                    HSNSGSTPer = x.SGST,
                    ProductDescription = x.ProductDescription,
                    ProductName = x.ProductName
                }).FirstOrDefault();
            return model;
        }

        
    }
}
