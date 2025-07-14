using CITS.EduSuite.Business.Common;
using CITS.EduSuite.Business.Interfaces;
using CITS.EduSuite.Business.Models.Resources;
using CITS.EduSuite.Business.Models.ViewModels;
using CITS.EduSuite.Data;
using System;
using System.Collections.Generic;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CITS.EduSuite.Business.Services
{
    public class UniversityMasterService : IUniversityMasterService
    {
        private EduSuiteDatabase dbContext;
        public UniversityMasterService(EduSuiteDatabase objDB)
        {
            this.dbContext = objDB;
        }
        public UniversityMasterViewModel GetUniversityMasterById(int? id)
        {
            try
            {
                UniversityMasterViewModel model = new UniversityMasterViewModel();
                model = dbContext.UniversityMasters.Select(row => new UniversityMasterViewModel
                {
                    RowKey = row.RowKey,
                    UniversityMasterName = row.UniversityMasterName,
                    UniversityMasterCode = row.UniversityMasterCode,
                    IsActive = row.IsActive,
                    AccountHeadKey = row.AccountHeadKey ?? 0,
                }).Where(x => x.RowKey == id).FirstOrDefault();
                if (model == null)
                {
                    model = new UniversityMasterViewModel();
                }
                else
                {
                    if (model.AccountHeadKey != 0)
                    {
                        UniversityMaster UniversityMasters = dbContext.UniversityMasters.Where(x => x.RowKey == model.RowKey).FirstOrDefault();
                        model.AccountGroupKey = UniversityMasters.AccountHead.AccountHeadType.AccountGroupKey;
                        model.AccountHeadTypeKey = UniversityMasters.AccountHead.AccountHeadTypeKey;
                    }
                }
                model.AllowUniversityAccountHead = DbConstants.GeneralConfiguration.AllowUniversityAccountHead;


                BindDropDownList(model);
                return model;
            }
            catch (Exception ex)
            {
                ActivityLog.CreateActivityLog(MenuConstants.UniversityMaster, ActionConstants.View, DbConstants.LogType.Error, id, ex.GetBaseException().Message);
                return new UniversityMasterViewModel();

            }
        }
        public UniversityMasterViewModel CreateUniversityMaster(UniversityMasterViewModel model)
        {
            UniversityMaster UniversityMasterModel = new UniversityMaster();

            var UniversityMasterCheck = dbContext.UniversityMasters.Where(row => row.UniversityMasterCode.ToLower() == model.UniversityMasterCode.ToLower()).Count();

            if (UniversityMasterCheck != 0)
            {
                model.Message = String.Format(EduSuiteUIResources.ExistsErrorMessage, EduSuiteUIResources.University);
                model.IsSuccessful = false;
                return model;
            }
            using (var transaction = dbContext.Database.BeginTransaction())
            {
                try
                {
                    AccountHeadViewModel accountHeadViewModel = new AccountHeadViewModel();

                    if (DbConstants.GeneralConfiguration.AllowUniversityAccountHead)
                    {
                        AccountHeadService accountHeadService = new AccountHeadService(dbContext);

                        accountHeadViewModel.AccountHeadName = model.UniversityMasterName;
                        accountHeadViewModel.AccountHeadTypeKey = model.AccountHeadTypeKey ?? 0;
                        accountHeadViewModel.IsActive = true;
                        accountHeadViewModel.IsSystemAccount = true;
                        accountHeadViewModel.HideDaily = true;
                        accountHeadViewModel.HideFuture = true;
                        accountHeadViewModel.PaymentConfigTypeKey = DbConstants.PaymentReceiptConfigType.Payment;
                        accountHeadViewModel = accountHeadService.createAccountChart(accountHeadViewModel);
                        UniversityMasterModel.AccountHeadKey = accountHeadViewModel.RowKey;
                    }
                    long MaxKey = dbContext.UniversityMasters.Select(p => p.RowKey).DefaultIfEmpty().Max();

                    UniversityMasterModel.RowKey = Convert.ToInt16(MaxKey + 1);
                    UniversityMasterModel.UniversityMasterName = model.UniversityMasterName;
                    UniversityMasterModel.UniversityMasterCode = model.UniversityMasterCode;
                    UniversityMasterModel.IsActive = model.IsActive;

                    dbContext.UniversityMasters.Add(UniversityMasterModel);
                    dbContext.SaveChanges();
                    transaction.Commit();
                    model.Message = EduSuiteUIResources.Success;
                    model.IsSuccessful = true;

                    ActivityLog.CreateActivityLog(MenuConstants.UniversityMaster, ActionConstants.Add, DbConstants.LogType.Info, UniversityMasterModel.RowKey, model.Message);


                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    model.Message = String.Format(EduSuiteUIResources.FailedToSaveErrorMessage, EduSuiteUIResources.AffiliationsTieUps);
                    model.IsSuccessful = false;

                    ActivityLog.CreateActivityLog(MenuConstants.UniversityMaster, ActionConstants.Add, DbConstants.LogType.Error, model.RowKey, ex.GetBaseException().Message);
                }
            }
            return model;
        }
        public UniversityMasterViewModel UpdateUniversityMaster(UniversityMasterViewModel model)
        {
            UniversityMaster UniversityMasterModel = new UniversityMaster();

            var UniversityMasterCheck = dbContext.UniversityMasters.Where(row => row.UniversityMasterCode.ToLower() == model.UniversityMasterCode.ToLower()
               && row.RowKey != model.RowKey).ToList();


            if (UniversityMasterCheck.Count != 0)
            {
                model.Message = String.Format(EduSuiteUIResources.ExistsErrorMessage, EduSuiteUIResources.University);
                model.IsSuccessful = false;
                return model;
            }
            using (var transaction = dbContext.Database.BeginTransaction())
            {
                try
                {
                    UniversityMasterModel = dbContext.UniversityMasters.SingleOrDefault(x => x.RowKey == model.RowKey);

                    if (DbConstants.GeneralConfiguration.AllowUniversityAccountHead)
                    {

                        if (UniversityMasterModel.AccountHeadKey == null || UniversityMasterModel.AccountHeadKey == 0)
                        {
                            AccountHeadService accountHeadService = new AccountHeadService(dbContext);
                            AccountHeadViewModel accountHeadViewModel = new AccountHeadViewModel();
                            accountHeadViewModel.AccountHeadName = model.UniversityMasterName;
                            accountHeadViewModel.AccountHeadTypeKey = model.AccountHeadTypeKey ?? 0;
                            accountHeadViewModel.IsActive = true;
                            accountHeadViewModel.IsSystemAccount = true;
                            accountHeadViewModel.PaymentConfigTypeKey = DbConstants.PaymentReceiptConfigType.Payment;
                            accountHeadViewModel = accountHeadService.createAccountChart(accountHeadViewModel);
                            UniversityMasterModel.AccountHeadKey = accountHeadViewModel.RowKey;
                        }
                        else
                        {
                            AccountHeadService accountHeadService = new AccountHeadService(dbContext);
                            AccountHeadViewModel accountHeadViewModel = new AccountHeadViewModel();
                            accountHeadViewModel.AccountHeadName = model.UniversityMasterName;
                            accountHeadViewModel.AccountHeadTypeKey = model.AccountHeadTypeKey ?? 0;
                            accountHeadViewModel.IsActive = true;
                            accountHeadViewModel.IsSystemAccount = true;
                            accountHeadViewModel.RowKey = UniversityMasterModel.AccountHeadKey ?? 0;
                            accountHeadViewModel.PaymentConfigTypeKey = DbConstants.PaymentReceiptConfigType.Payment;
                            accountHeadViewModel = accountHeadService.updateAccountChart(accountHeadViewModel);

                        }
                    }



                    UniversityMasterModel.UniversityMasterName = model.UniversityMasterName;
                    UniversityMasterModel.UniversityMasterCode = model.UniversityMasterCode;
                    UniversityMasterModel.IsActive = model.IsActive;
                    dbContext.SaveChanges();
                    transaction.Commit();
                    model.Message = EduSuiteUIResources.Success;
                    model.IsSuccessful = true;

                    ActivityLog.CreateActivityLog(MenuConstants.UniversityMaster, ActionConstants.Edit, DbConstants.LogType.Info, model.RowKey, model.Message);

                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    model.Message = String.Format(EduSuiteUIResources.FailedToSaveErrorMessage, EduSuiteUIResources.AffiliationsTieUps);
                    model.IsSuccessful = false;

                    ActivityLog.CreateActivityLog(MenuConstants.UniversityMaster, ActionConstants.Edit, DbConstants.LogType.Error, model.RowKey, ex.GetBaseException().Message);
                }
            }
            return model;

        }
        public UniversityMasterViewModel DeleteUniversityMaster(UniversityMasterViewModel model)
        {
            using (var transaction = dbContext.Database.BeginTransaction())
            {
                try
                {
                    UniversityMaster UniversityMasterModel = dbContext.UniversityMasters.SingleOrDefault(row => row.RowKey == model.RowKey);

                    long AccountHeadKey = UniversityMasterModel.AccountHeadKey ?? 0;
                    dbContext.UniversityMasters.Remove(UniversityMasterModel);

                    if (AccountHeadKey != 0)
                    {
                        AccountFlowService accountFlowService = new AccountFlowService(dbContext);
                        accountFlowService.DeleteAccountHead(AccountHeadKey);

                    }

                    dbContext.SaveChanges();
                    transaction.Commit();
                    model.Message = EduSuiteUIResources.Success;
                    model.IsSuccessful = true;
                    ActivityLog.CreateActivityLog(MenuConstants.UniversityMaster, ActionConstants.Delete, DbConstants.LogType.Info, model.RowKey, model.Message);


                }
                catch (DbUpdateException ex)
                {
                    transaction.Rollback();
                    if (ex.GetBaseException().Message.ToUpper().Contains(EduSuiteUIResources.ForeignKeyError.ToUpper()))
                    {
                        model.Message = String.Format(EduSuiteUIResources.CantDeleteErrorMessage, EduSuiteUIResources.AffiliationsTieUps);
                        model.IsSuccessful = false;

                        ActivityLog.CreateActivityLog(MenuConstants.UniversityMaster, ActionConstants.Delete, DbConstants.LogType.Error, model.RowKey, ex.GetBaseException().Message);
                    }
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    model.Message = String.Format(EduSuiteUIResources.FailedToDeleteErrorMessage, EduSuiteUIResources.AffiliationsTieUps);
                    model.IsSuccessful = false;

                    ActivityLog.CreateActivityLog(MenuConstants.UniversityMaster, ActionConstants.Delete, DbConstants.LogType.Error, model.RowKey, ex.GetBaseException().Message);
                }
            }

            return model;
        }
        public List<UniversityMasterViewModel> GetUniversityMaster(string searchText)
        {
            try
            {
                var UniversityMasterList = (from p in dbContext.UniversityMasters
                                            orderby p.RowKey
                                            where (p.UniversityMasterName.Contains(searchText))
                                            select new UniversityMasterViewModel
                                            {
                                                RowKey = p.RowKey,
                                                UniversityMasterName = p.UniversityMasterName,
                                                UniversityMasterCode = p.UniversityMasterCode,
                                                //IsActiveText = p.IsActive == true ? ApplicationResources.Yes : ApplicationResources.No
                                                IsActive = p.IsActive,
                                            }).ToList();
                return UniversityMasterList.GroupBy(x => x.RowKey).Select(y => y.First()).ToList<UniversityMasterViewModel>();
            }
            catch (Exception ex)
            {
                ActivityLog.CreateActivityLog(MenuConstants.UniversityMaster, ActionConstants.View, DbConstants.LogType.Error, null, ex.GetBaseException().Message);
                return new List<UniversityMasterViewModel>();

            }
        }
        public UniversityMasterViewModel CheckUniversityMasterCodeExists(UniversityMasterViewModel model)
        {
            if (dbContext.UniversityMasters.Where(x => x.UniversityMasterCode.ToLower() == model.UniversityMasterCode.ToLower() && x.RowKey != model.RowKey).Any())
            {
                model.IsSuccessful = false;
                model.Message = String.Format(EduSuiteUIResources.ExistsErrorMessage, EduSuiteUIResources.AffiliationsTieUps + EduSuiteUIResources.BlankSpace + EduSuiteUIResources.Code);
            }
            else
            {
                model.IsSuccessful = true;
                model.Message = "";
            }
            return model;
        }
        public UniversityMasterViewModel CheckUniversityMasterNameExists(UniversityMasterViewModel model)
        {
            if (dbContext.UniversityMasters.Where(x => x.UniversityMasterName.ToLower() == model.UniversityMasterName.ToLower() && x.RowKey != model.RowKey).Any())
            {
                model.IsSuccessful = false;
                model.Message = String.Format(EduSuiteUIResources.ExistsErrorMessage, EduSuiteUIResources.AffiliationsTieUps + EduSuiteUIResources.BlankSpace + EduSuiteUIResources.Name);
            }
            else
            {
                model.IsSuccessful = true;
                model.Message = "";
            }
            return model;
        }

        public UniversityMasterViewModel FillAccountHeadType(UniversityMasterViewModel model)
        {
            model.AccountHeadType = dbContext.VwAccountHeadTypes.Where(x => x.AccountGroupKey == model.AccountGroupKey).Select(row => new SelectListModel
            {
                RowKey = row.RowKey,
                Text = row.AccountHeadTypeName
            }).ToList();
            return model;
        }
        private void FillAccountGroup(UniversityMasterViewModel model)
        {
            List<long> AccountGroupKeys = new List<long>();
            AccountGroupKeys.Add(DbConstants.AccountGroup.Expenses);
            AccountGroupKeys.Add(DbConstants.AccountGroup.Income);

            model.AccountGroup = dbContext.VwAccountGroups.Where(x => AccountGroupKeys.Contains(x.RowKey)).Select(row => new SelectListModel
            {
                RowKey = row.RowKey,
                Text = row.AccountGroupName
            }).ToList();
        }
        public void BindDropDownList(UniversityMasterViewModel model)
        {
            FillAccountGroup(model);
            FillAccountHeadType(model);
        }
    }
}
