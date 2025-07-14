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
    public class TCReasonMasterService : ITCReasonMasterService
    {
        private EduSuiteDatabase dbContext;
        public TCReasonMasterService(EduSuiteDatabase objDB)
        {
            this.dbContext = objDB;
        }

        public List<TCReasonMasterViewModel> GetTCReasonMaster(string searchText)
        {
            try
            {
                var TCReasonMasterList = (from p in dbContext.TCReasonMasters
                                      orderby p.RowKey descending
                                      where (p.ReasonName.Contains(searchText))
                                      select new TCReasonMasterViewModel
                                      {
                                          RowKey = p.RowKey,
                                          ReasonName = p.ReasonName,
                                          IsActive = p.IsActive,
                                       

                                      }).ToList();
                return TCReasonMasterList.GroupBy(x => x.RowKey).Select(y => y.First()).ToList<TCReasonMasterViewModel>();
            }
            catch (Exception ex)
            {
                ActivityLog.CreateActivityLog(MenuConstants.TCReasonMaster, ActionConstants.View, DbConstants.LogType.Error, null, ex.GetBaseException().Message);
                return new List<TCReasonMasterViewModel>();


            }
        }
        public TCReasonMasterViewModel GetTCReasonMasterById(int? id)
        {
            try
            {
                TCReasonMasterViewModel model = new TCReasonMasterViewModel();
                model = dbContext.TCReasonMasters.Select(row => new TCReasonMasterViewModel
                {
                    RowKey = row.RowKey,
                    ReasonName = row.ReasonName,
                    IsActive = row.IsActive
                }).Where(x => x.RowKey == id).FirstOrDefault();
                if (model == null)
                {
                    model = new TCReasonMasterViewModel();
                }
                return model;
            }
            catch (Exception ex)
            {
                ActivityLog.CreateActivityLog(MenuConstants.TCReasonMaster, ActionConstants.View, DbConstants.LogType.Error, id, ex.GetBaseException().Message);
                return new TCReasonMasterViewModel();

            }
        }
        public TCReasonMasterViewModel CreateTCReasonMaster(TCReasonMasterViewModel model)
        {
            var TCReasonMasterCheck = dbContext.TCReasonMasters.Where(row => row.ReasonName.ToLower() == model.ReasonName.ToLower()).Count();
            TCReasonMaster TCReasonMasterModel = new TCReasonMaster();
            if (TCReasonMasterCheck != 0)
            {
                model.Message = String.Format(EduSuiteUIResources.ExistsErrorMessage, EduSuiteUIResources.TCReasonMaster);
                model.IsSuccessful = false;
                return model;
            }

            using (var transaction = dbContext.Database.BeginTransaction())
            {
                try
                {
                    int MaxKey = dbContext.TCReasonMasters.Select(p => p.RowKey).DefaultIfEmpty().Max();

                    TCReasonMasterModel.RowKey = Convert.ToInt16(MaxKey + 1);
                    TCReasonMasterModel.ReasonName = model.ReasonName;
                    TCReasonMasterModel.IsActive = model.IsActive;
                    dbContext.TCReasonMasters.Add(TCReasonMasterModel);
                    dbContext.SaveChanges();
                    transaction.Commit();
                    model.Message = EduSuiteUIResources.Success;
                    model.IsSuccessful = true;

                    ActivityLog.CreateActivityLog(MenuConstants.TCReasonMaster, ActionConstants.Add, DbConstants.LogType.Info, model.RowKey, model.Message);
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    model.Message = String.Format(EduSuiteUIResources.FailedToSaveErrorMessage, EduSuiteUIResources.TCReasonMaster);
                    model.IsSuccessful = false;
                    ActivityLog.CreateActivityLog(MenuConstants.TCReasonMaster, ActionConstants.Add, DbConstants.LogType.Error, model.RowKey, ex.GetBaseException().Message);
                }
            }
            return model;
        }
        public TCReasonMasterViewModel UpdateTCReasonMaster(TCReasonMasterViewModel model)
        {
            var TCReasonMasterCheck = dbContext.TCReasonMasters.Where(row => row.ReasonName.ToLower() == model.ReasonName.ToLower()
               && row.RowKey != model.RowKey).ToList();

            TCReasonMaster TCReasonMasterModel = new TCReasonMaster();
            if (TCReasonMasterCheck.Count != 0)
            {
                model.Message = String.Format(EduSuiteUIResources.ExistsErrorMessage, EduSuiteUIResources.TCReasonMaster);
                model.IsSuccessful = false;
                return model;
            }

            using (var transaction = dbContext.Database.BeginTransaction())
            {
                try
                {
                    TCReasonMasterModel = dbContext.TCReasonMasters.SingleOrDefault(x => x.RowKey == model.RowKey);
                    TCReasonMasterModel.ReasonName = model.ReasonName;
                    TCReasonMasterModel.IsActive = model.IsActive;
                    dbContext.SaveChanges();
                    transaction.Commit();
                    model.Message = EduSuiteUIResources.Success;
                    model.IsSuccessful = true;

                    ActivityLog.CreateActivityLog(MenuConstants.TCReasonMaster, ActionConstants.Edit, DbConstants.LogType.Info, model.RowKey, model.Message);
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    model.Message = String.Format(EduSuiteUIResources.FailedToSaveErrorMessage, EduSuiteUIResources.TCReasonMaster);
                    model.IsSuccessful = false;
                    ActivityLog.CreateActivityLog(MenuConstants.TCReasonMaster, ActionConstants.Edit, DbConstants.LogType.Error, model.RowKey, ex.GetBaseException().Message);

                }
            }
            return model;

        }
        public TCReasonMasterViewModel DeleteTCReasonMaster(TCReasonMasterViewModel model)
        {
            using (var transaction = dbContext.Database.BeginTransaction())
            {
                try
                {
                    TCReasonMaster TCReasonMasterModel = dbContext.TCReasonMasters.SingleOrDefault(row => row.RowKey == model.RowKey);
                    dbContext.TCReasonMasters.Remove(TCReasonMasterModel);
                    dbContext.SaveChanges();
                    transaction.Commit();
                    model.Message = EduSuiteUIResources.Success;
                    model.IsSuccessful = true;
                    ActivityLog.CreateActivityLog(MenuConstants.TCReasonMaster, ActionConstants.Delete, DbConstants.LogType.Info, model.RowKey, model.Message);
                }
                catch (DbUpdateException ex)
                {
                    transaction.Rollback();
                    if (ex.GetBaseException().Message.ToUpper().Contains(EduSuiteUIResources.ForeignKeyError.ToUpper()))
                    {
                        model.Message = String.Format(EduSuiteUIResources.CantDeleteErrorMessage, EduSuiteUIResources.TCReasonMaster);
                        model.IsSuccessful = false;
                        ActivityLog.CreateActivityLog(MenuConstants.TCReasonMaster, ActionConstants.Delete, DbConstants.LogType.Debug, model.RowKey, ex.GetBaseException().Message);
                    }
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    model.Message = String.Format(EduSuiteUIResources.FailedToDeleteErrorMessage, EduSuiteUIResources.TCReasonMaster);
                    model.IsSuccessful = false;
                    ActivityLog.CreateActivityLog(MenuConstants.TCReasonMaster, ActionConstants.Delete, DbConstants.LogType.Debug, model.RowKey, ex.GetBaseException().Message);
                }
            }

            return model;
        }

    }
}
