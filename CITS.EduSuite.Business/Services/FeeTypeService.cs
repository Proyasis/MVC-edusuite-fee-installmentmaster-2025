using CITS.EduSuite.Business.Interfaces;
using CITS.EduSuite.Data;
using CITS.EduSuite.Business.Common;
using CITS.EduSuite.Business.Models.ViewModels;
using System;
using System.Collections.Generic;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CITS.EduSuite.Business.Models.Resources;

namespace CITS.EduSuite.Business.Services
{
    public class FeeTypeService : IFeeTypeService
    {
        private EduSuiteDatabase dbContext;

        public FeeTypeService(EduSuiteDatabase objDb)
        {
            this.dbContext = objDb;
        }
        public FeeTypeViewModel GetFeeTypeById(int? Id)
        {
            try
            {
                FeeTypeViewModel model = new FeeTypeViewModel();
                model = dbContext.FeeTypes.Where(x => x.RowKey == Id).Select(row => new FeeTypeViewModel
                {
                    RowKey = row.RowKey,
                    FeeTypeName = row.FeeTypeName,
                    CashFlowTypeKey = row.CashFlowTypeKey,
                    IsActive = row.IsActive,
                    FeeTypeModeKey = row.FeeTypeModeKey,
                    IsUniverisity = row.IsUniverisity,
                    IsTax = row.IsTax,
                    IsDeduct = row.IsDeduct,
                    GSTMasterkey = row.GSTMasterkey,
                    AccountHeadKey = row.AccountHeadKey ?? 0,
                    AccountHeadTypeKey = row.AccountHeadKey != null ? row.AccountHead.AccountHeadTypeKey: DbConstants.AccountHeadType.DirectIncome,
                    AccountGroupKey = row.AccountHeadKey !=null? row.AccountHead.AccountHeadType.AccountGroupKey: DbConstants.AccountGroup.Income,
                    ReceiptNumberConfigurationKey = row.ReceiptNumberConfigurationKey ?? 0,

                }).FirstOrDefault();
                if (model == null)
                {
                    model = new FeeTypeViewModel();
                }
                else
                {
                    //if (model.AccountHeadKey != 0)
                    //{
                    //    FeeType feetype = dbContext.FeeTypes.Where(x => x.RowKey == model.RowKey).FirstOrDefault();
                    //    model.AccountGroupKey = feetype.AccountHead.AccountHeadType.AccountGroupKey;
                    //    model.AccountHeadTypeKey = feetype.AccountHead.AccountHeadTypeKey;
                    //}
                }
                model.AllowFeetypeOnlyIncome = dbContext.GeneralConfigurations.Select(x => x.AllowFeetypeOnlyIncome).FirstOrDefault();
                model.AllowHideFeeBalance = dbContext.GeneralConfigurations.Select(x => x.AllowHideFeeBalance).FirstOrDefault();

                if (model.AllowFeetypeOnlyIncome)
                {
                    if (model.AccountHeadKey == null)
                    {
                        model.AccountGroupKey = DbConstants.AccountGroup.Income;
                        model.AccountHeadTypeKey = DbConstants.AccountHeadType.DirectIncome;
                        model.CashFlowTypeKey = DbConstants.CashFlowType.In;
                    }
                }

                FillDropDown(model);

                return model;
            }

            catch (Exception ex)
            {
                ActivityLog.CreateActivityLog(MenuConstants.FeeType, ActionConstants.View, DbConstants.LogType.Error, Id, ex.GetBaseException().Message);
                return new FeeTypeViewModel();
            }
        }

        public FeeTypeViewModel CreateFeeType(FeeTypeViewModel model)
        {
            FeeType FeeTypeModel = new FeeType();
            FillDropDown(model);

            using (var transaction = dbContext.Database.BeginTransaction())
            {
                try
                {

                    if (model.AllowFeetypeOnlyIncome)
                    {
                        if (model.IsUniverisity == true)
                        {
                            model.AccountGroupKey = DbConstants.AccountGroup.Expenses;
                            model.AccountHeadTypeKey = DbConstants.AccountHeadType.DirectExpense;
                            model.CashFlowTypeKey = DbConstants.CashFlowType.Out;
                        }
                        else
                        {
                            model.AccountGroupKey = DbConstants.AccountGroup.Income;
                            model.AccountHeadTypeKey = DbConstants.AccountHeadType.DirectIncome;
                            model.CashFlowTypeKey = DbConstants.CashFlowType.In;
                        }
                    }

                    AccountHeadService accountHeadService = new AccountHeadService(dbContext);
                    AccountHeadViewModel accountHeadViewModel = new AccountHeadViewModel();
                    accountHeadViewModel.AccountHeadName = model.FeeTypeName;
                    accountHeadViewModel.AccountHeadTypeKey = model.AccountHeadTypeKey;
                    accountHeadViewModel.IsActive = true;
                    accountHeadViewModel.IsSystemAccount = true;
                    accountHeadViewModel = accountHeadService.createAccountChart(accountHeadViewModel);
                    int Maxkey = dbContext.FeeTypes.Select(p => p.RowKey).DefaultIfEmpty().Max();

                    FeeTypeModel.RowKey = Convert.ToInt16(Maxkey + 1);
                    FeeTypeModel.FeeTypeName = model.FeeTypeName;
                    FeeTypeModel.CashFlowTypeKey = model.CashFlowTypeKey;
                    FeeTypeModel.IsActive = model.IsActive;
                    FeeTypeModel.FeeTypeModeKey = model.FeeTypeModeKey;
                    FeeTypeModel.IsUniverisity = model.IsUniverisity;
                    FeeTypeModel.IsTax = model.IsTax;
                    FeeTypeModel.IsDeduct = model.IsDeduct;
                    if (model.IsTax == true)
                    {
                        FeeTypeModel.GSTMasterkey = model.GSTMasterkey;
                    }
                    FeeTypeModel.AccountHeadKey = accountHeadViewModel.RowKey;
                    FeeTypeModel.ReceiptNumberConfigurationKey = model.ReceiptNumberConfigurationKey;

                    dbContext.FeeTypes.Add(FeeTypeModel);
                    dbContext.SaveChanges();
                    transaction.Commit();
                    model.Message = EduSuiteUIResources.Success;
                    model.IsSuccessful = true;

                    ActivityLog.CreateActivityLog(MenuConstants.FeeType, ActionConstants.Add, DbConstants.LogType.Info, FeeTypeModel.RowKey, model.Message);
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    model.Message = String.Format(EduSuiteUIResources.FailedToSaveErrorMessage, EduSuiteUIResources.FeeType);
                    model.IsSuccessful = false;
                    ActivityLog.CreateActivityLog(MenuConstants.FeeType, ActionConstants.Add, DbConstants.LogType.Error, model.RowKey, ex.GetBaseException().Message);
                }
            }
            return model;
        }

        public FeeTypeViewModel UpdateFeeType(FeeTypeViewModel model)
        {
            FeeType FeeTypeModel = new FeeType();
            FillDropDown(model);

            using (var transaction = dbContext.Database.BeginTransaction())
            {
                try
                {
                    if (model.AllowFeetypeOnlyIncome)
                    {
                        if (model.IsUniverisity == true)
                        {
                            model.AccountGroupKey = DbConstants.AccountGroup.Expenses;
                            model.AccountHeadTypeKey = DbConstants.AccountHeadType.DirectExpense;
                            model.CashFlowTypeKey = DbConstants.CashFlowType.Out;
                        }
                        else
                        {
                            model.AccountGroupKey = DbConstants.AccountGroup.Income;
                            model.AccountHeadTypeKey = DbConstants.AccountHeadType.DirectIncome;
                            model.CashFlowTypeKey = DbConstants.CashFlowType.In;
                        }
                    }
                    FeeTypeModel = dbContext.FeeTypes.SingleOrDefault(x => x.RowKey == model.RowKey);
                    if (FeeTypeModel.AccountHeadKey == null || FeeTypeModel.AccountHeadKey == 0)
                    {
                        AccountHeadService accountHeadService = new AccountHeadService(dbContext);
                        AccountHeadViewModel accountHeadViewModel = new AccountHeadViewModel();
                        accountHeadViewModel.AccountHeadName = model.FeeTypeName;
                        accountHeadViewModel.AccountHeadTypeKey = model.AccountHeadTypeKey;
                        accountHeadViewModel.IsActive = true;
                        accountHeadViewModel.IsSystemAccount = true;
                        accountHeadViewModel = accountHeadService.createAccountChart(accountHeadViewModel);
                        FeeTypeModel.AccountHeadKey = accountHeadViewModel.RowKey;
                    }
                    else
                    {
                        AccountHeadService accountHeadService = new AccountHeadService(dbContext);
                        AccountHeadViewModel accountHeadViewModel = new AccountHeadViewModel();
                        accountHeadViewModel.AccountHeadName = model.FeeTypeName;
                        accountHeadViewModel.AccountHeadTypeKey = model.AccountHeadTypeKey;
                        accountHeadViewModel.IsActive = true;
                        accountHeadViewModel.IsSystemAccount = true;
                        accountHeadViewModel.RowKey = FeeTypeModel.AccountHeadKey ?? 0;
                        accountHeadViewModel = accountHeadService.updateAccountChart(accountHeadViewModel);

                    }


                    FeeTypeModel.FeeTypeName = model.FeeTypeName;
                    FeeTypeModel.CashFlowTypeKey = model.CashFlowTypeKey;
                    FeeTypeModel.IsActive = model.IsActive;
                    FeeTypeModel.ReceiptNumberConfigurationKey = model.ReceiptNumberConfigurationKey;

                    FeeTypeModel.FeeTypeModeKey = model.FeeTypeModeKey;
                    FeeTypeModel.IsUniverisity = model.IsUniverisity;
                    FeeTypeModel.IsTax = model.IsTax;
                    FeeTypeModel.IsDeduct = model.IsDeduct;
                    if (model.IsTax == true)
                    {
                        FeeTypeModel.GSTMasterkey = model.GSTMasterkey;
                    }

                    dbContext.SaveChanges();
                    transaction.Commit();
                    model.Message = EduSuiteUIResources.Success;
                    model.IsSuccessful = true;
                    ActivityLog.CreateActivityLog(MenuConstants.FeeType, ActionConstants.Edit, DbConstants.LogType.Info, model.RowKey, model.Message);

                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    model.Message = String.Format(EduSuiteUIResources.FailedToSaveErrorMessage, EduSuiteUIResources.FeeType);
                    model.IsSuccessful = false;
                    ActivityLog.CreateActivityLog(MenuConstants.FeeType, ActionConstants.Edit, DbConstants.LogType.Error, model.RowKey, ex.GetBaseException().Message);
                }
            }
            return model;
        }

        public FeeTypeViewModel DeleteFeeType(FeeTypeViewModel model)
        {
            using (var transaction = dbContext.Database.BeginTransaction())
            {
                try
                {
                    FeeType FeeTypeModel = dbContext.FeeTypes.SingleOrDefault(x => x.RowKey == model.RowKey);

                    long AccountHeadKey = FeeTypeModel.AccountHeadKey ?? 0;
                    dbContext.FeeTypes.Remove(FeeTypeModel);
                    AccountFlowService accountFlowService = new AccountFlowService(dbContext);
                    accountFlowService.DeleteAccountHead(AccountHeadKey);

                    dbContext.SaveChanges();
                    transaction.Commit();
                    model.Message = EduSuiteUIResources.Success;
                    model.IsSuccessful = true;
                    ActivityLog.CreateActivityLog(MenuConstants.FeeType, ActionConstants.Delete, DbConstants.LogType.Info, model.RowKey, model.Message);
                }
                catch (DbUpdateException ex)
                {
                    transaction.Rollback();
                    if (ex.GetBaseException().Message.ToUpper().Contains(EduSuiteUIResources.ForeignKeyError.ToUpper()))
                    {
                        model.Message = String.Format(EduSuiteUIResources.CantDeleteErrorMessage, EduSuiteUIResources.FeeType);
                        model.IsSuccessful = false;
                        ActivityLog.CreateActivityLog(MenuConstants.FeeType, ActionConstants.Delete, DbConstants.LogType.Error, model.RowKey, ex.GetBaseException().Message);
                    }

                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    model.Message = String.Format(EduSuiteUIResources.FailedToDeleteErrorMessage, EduSuiteUIResources.FeeType);
                    model.IsSuccessful = false;
                    ActivityLog.CreateActivityLog(MenuConstants.FeeType, ActionConstants.Delete, DbConstants.LogType.Error, model.RowKey, ex.GetBaseException().Message);
                }
            }
            return model;

        }

        public List<FeeTypeViewModel> GetFeeType(string SearchText)
        {
            try
            {
                var FeeTypelist = (from p in dbContext.FeeTypes
                                   orderby p.RowKey descending
                                   where (p.FeeTypeName.Contains(SearchText))
                                   select new FeeTypeViewModel
                                   {
                                       RowKey = p.RowKey,
                                       FeeTypeName = p.FeeTypeName,
                                       FeeTypeModeName = p.FeeTypeMode.FeeTypeModeName,
                                       CashFlowTypeName = p.CashFlowType.CashFlowTypeName,
                                       IsTaxName = p.IsTax == true ? "Yes" : "No",
                                       IsUniverisityName = p.IsUniverisity == true ? "Yes" : "No",
                                       IsActive = p.IsActive,
                                       AccountGroupName = p.AccountHead.AccountHeadType.AccountGroup.AccountGroupName + " - " + p.AccountHead.AccountHeadType.AccountHeadTypeName,
                                   }).ToList();
                return FeeTypelist.GroupBy(x => x.RowKey).Select(y => y.First()).ToList<FeeTypeViewModel>();
            }
            catch (Exception ex)
            {
                ActivityLog.CreateActivityLog(MenuConstants.FeeType, ActionConstants.View, DbConstants.LogType.Error, null, ex.GetBaseException().Message);
                return new List<FeeTypeViewModel>();


            }
        }

        public FeeTypeViewModel CheckFeeTypeExist(FeeTypeViewModel model)
        {
            if (dbContext.FeeTypes.Where(x => x.FeeTypeName.Trim().ToLower() == model.FeeTypeName.Trim().ToLower() && x.RowKey != model.RowKey).Any())
            {
                model.IsSuccessful = false;
                model.Message = String.Format(EduSuiteUIResources.ExistsErrorMessage, EduSuiteUIResources.FeeType);
            }
            else
            {
                model.IsSuccessful = true;
                model.Message = "";
            }
            return model;
        }

        private void FillDropDown(FeeTypeViewModel model)
        {
            FillCashFlowType(model);
            FillAccountHeadType(model);
            FillAccountGroup(model);
            FillFeeTypeCatagory(model);
            FillReceiptType(model);
            FillHSNCode(model);
        }
        private void FillFeeTypeCatagory(FeeTypeViewModel model)
        {
            model.FeeTypeMode = dbContext.FeeTypeModes.Where(x => x.IsActive).OrderBy(row => row.RowKey).Select(row => new SelectListModel
            {
                RowKey = row.RowKey,
                Text = row.FeeTypeModeName
            }).ToList();
        }
        private void FillCashFlowType(FeeTypeViewModel model)
        {
            model.CashFlowTypeList = dbContext.CashFlowTypes.OrderBy(row => row.RowKey).Select(row => new SelectListModel
            {
                RowKey = row.RowKey,
                Text = row.CashFlowTypeName
            }).ToList();
        }
        public FeeTypeViewModel FillAccountHeadType(FeeTypeViewModel model)
        {
            model.AccountHeadType = dbContext.VwAccountHeadTypes.Where(x => x.AccountGroupKey == model.AccountGroupKey).Select(row => new SelectListModel
            {
                RowKey = row.RowKey,
                Text = row.AccountHeadTypeName
            }).ToList();
            return model;
        }
        private void FillAccountGroup(FeeTypeViewModel model)
        {

            List<long> AccountGroupKeys = new List<long>();
            AccountGroupKeys.Add(DbConstants.AccountGroup.Income);
            AccountGroupKeys.Add(DbConstants.AccountGroup.Liability);
            AccountGroupKeys.Add(DbConstants.AccountGroup.Asset);
            if (!DbConstants.GeneralConfiguration.AllowUniversityAccountHead)
            {
                AccountGroupKeys.Add(DbConstants.AccountGroup.Expenses);
            }
            model.AccountGroup = dbContext.VwAccountGroups.Where(x => AccountGroupKeys.Contains(x.RowKey)).Select(row => new SelectListModel
            {
                RowKey = row.RowKey,
                Text = row.AccountGroupName
            }).ToList();
        }

        private void FillHSNCode(FeeTypeViewModel model)
        {
            model.HSNCodes = dbContext.GSTMasters.Where(x => x.IsActive == true).Select(row => new SelectListModel
            {
                RowKey = row.RowKey,
                Text = row.HSNCode,
            }).ToList();
        }
        private void FillReceiptType(FeeTypeViewModel model)
        {
            List<short> ReceiptTypeKeys = new List<short>();
            ReceiptTypeKeys.Add(DbConstants.PaymentReceiptConfigType.Payment);
            ReceiptTypeKeys.Add(DbConstants.PaymentReceiptConfigType.Receipt);
            ReceiptTypeKeys.Add(DbConstants.PaymentReceiptConfigType.Refund);
            ReceiptTypeKeys.Add(DbConstants.PaymentReceiptConfigType.ReceiptVoucher);
            ReceiptTypeKeys.Add(DbConstants.PaymentReceiptConfigType.PaymentAndReceipt);
            ReceiptTypeKeys.Add(DbConstants.PaymentReceiptConfigType.SalaryVoucher);

            model.ReceiptTypes = dbContext.PaymentReceiptNumberConfigurations.Where(x => x.IsActive == true && !ReceiptTypeKeys.Contains(x.Type)).Select(row => new SelectListModel
            {
                RowKey = row.RowKey,
                Text = row.ConfigName,
            }).ToList();
        }
    }
}
