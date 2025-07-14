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
    public class GSTMasterService : IGSTMasterService
    {
        private EduSuiteDatabase dbContext;
        public GSTMasterService(EduSuiteDatabase objDB)
        {
            this.dbContext = objDB;
        }

        public List<GSTMasterViewModel> GetGSTMaster(string searchText)
        {
            try
            {
                var GSTMasterList = (from p in dbContext.GSTMasters
                                     orderby p.RowKey descending
                                     where (p.Name.Contains(searchText))
                                     select new GSTMasterViewModel
                                     {
                                         RowKey = p.RowKey,
                                         Name = p.Name,
                                         HSNCode = p.HSNCode,
                                         CGSTRate = p.CGSTRate,
                                         SGSTRate = p.SGSTRate,
                                         IGSTRate = p.IGSTRate,
                                         Description = p.Description,
                                         IsActive = p.IsActive,

                                     }).ToList();
                return GSTMasterList.GroupBy(x => x.RowKey).Select(y => y.First()).ToList<GSTMasterViewModel>();
            }
            catch (Exception ex)
            {
                ActivityLog.CreateActivityLog(MenuConstants.GSTMaster, ActionConstants.View, DbConstants.LogType.Error, null, ex.GetBaseException().Message);
                return new List<GSTMasterViewModel>();


            }
        }
        public GSTMasterViewModel GetGSTMasterById(int? id)
        {
            try
            {
                GSTMasterViewModel model = new GSTMasterViewModel();
                model = dbContext.GSTMasters.Select(row => new GSTMasterViewModel
                {
                    RowKey = row.RowKey,
                    Name = row.Name,
                    HSNCode = row.HSNCode,
                    CGSTRate = row.CGSTRate,
                    SGSTRate = row.SGSTRate,
                    IGSTRate = row.IGSTRate,
                    Description = row.Description,
                    IsActive = row.IsActive
                }).Where(x => x.RowKey == id).FirstOrDefault();
                if (model == null)
                {
                    model = new GSTMasterViewModel();
                }
                return model;
            }
            catch (Exception ex)
            {
                ActivityLog.CreateActivityLog(MenuConstants.GSTMaster, ActionConstants.View, DbConstants.LogType.Error, id, ex.GetBaseException().Message);
                return new GSTMasterViewModel();

            }
        }
        public GSTMasterViewModel CreateGSTMaster(GSTMasterViewModel model)
        {
            var GSTMasterCheck = dbContext.GSTMasters.Where(row => row.HSNCode.ToLower() == model.HSNCode.ToLower()).Count();
            GSTMaster GSTMasterModel = new GSTMaster();
            if (GSTMasterCheck != 0)
            {
                model.Message = String.Format(EduSuiteUIResources.ExistsErrorMessage, EduSuiteUIResources.GSTMaster);
                model.IsSuccessful = false;
                return model;
            }

            using (var transaction = dbContext.Database.BeginTransaction())
            {
                try
                {
                    int MaxKey = dbContext.GSTMasters.Select(p => p.RowKey).DefaultIfEmpty().Max();

                    GSTMasterModel.RowKey = Convert.ToInt16(MaxKey + 1);
                    GSTMasterModel.HSNCode = model.HSNCode;
                    GSTMasterModel.Name = model.Name;
                    GSTMasterModel.CGSTRate = model.CGSTRate;
                    GSTMasterModel.SGSTRate = model.SGSTRate;
                    GSTMasterModel.IGSTRate = model.IGSTRate;
                    GSTMasterModel.Description = model.Description;
                    GSTMasterModel.Name = model.Name;
                    GSTMasterModel.IsActive = model.IsActive;
                    dbContext.GSTMasters.Add(GSTMasterModel);
                    dbContext.SaveChanges();
                    transaction.Commit();
                    model.Message = EduSuiteUIResources.Success;
                    model.IsSuccessful = true;

                    ActivityLog.CreateActivityLog(MenuConstants.GSTMaster, ActionConstants.Add, DbConstants.LogType.Info, model.RowKey, model.Message);
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    model.Message = String.Format(EduSuiteUIResources.FailedToSaveErrorMessage, EduSuiteUIResources.GSTMaster);
                    model.IsSuccessful = false;
                    ActivityLog.CreateActivityLog(MenuConstants.GSTMaster, ActionConstants.Add, DbConstants.LogType.Error, model.RowKey, ex.GetBaseException().Message);
                }
            }
            return model;
        }
        public GSTMasterViewModel UpdateGSTMaster(GSTMasterViewModel model)
        {
            var GSTMasterCheck = dbContext.GSTMasters.Where(row => row.HSNCode.ToLower() == model.HSNCode.ToLower()
               && row.RowKey != model.RowKey).ToList();

            GSTMaster GSTMasterModel = new GSTMaster();
            if (GSTMasterCheck.Count != 0)
            {
                model.Message = String.Format(EduSuiteUIResources.ExistsErrorMessage, EduSuiteUIResources.GSTMaster);
                model.IsSuccessful = false;
                return model;
            }

            using (var transaction = dbContext.Database.BeginTransaction())
            {
                try
                {
                    GSTMasterModel = dbContext.GSTMasters.SingleOrDefault(x => x.RowKey == model.RowKey);
                    GSTMasterModel.HSNCode = model.HSNCode;
                    GSTMasterModel.Name = model.Name;
                    GSTMasterModel.CGSTRate = model.CGSTRate;
                    GSTMasterModel.SGSTRate = model.SGSTRate;
                    GSTMasterModel.IGSTRate = model.IGSTRate;
                    GSTMasterModel.Description = model.Description;
                    GSTMasterModel.Name = model.Name;
                    GSTMasterModel.IsActive = model.IsActive;
                    dbContext.SaveChanges();
                    transaction.Commit();
                    model.Message = EduSuiteUIResources.Success;
                    model.IsSuccessful = true;

                    ActivityLog.CreateActivityLog(MenuConstants.GSTMaster, ActionConstants.Edit, DbConstants.LogType.Info, model.RowKey, model.Message);
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    model.Message = String.Format(EduSuiteUIResources.FailedToSaveErrorMessage, EduSuiteUIResources.GSTMaster);
                    model.IsSuccessful = false;
                    ActivityLog.CreateActivityLog(MenuConstants.GSTMaster, ActionConstants.Edit, DbConstants.LogType.Error, model.RowKey, ex.GetBaseException().Message);

                }
            }
            return model;

        }
        public GSTMasterViewModel DeleteGSTMaster(GSTMasterViewModel model)
        {
            using (var transaction = dbContext.Database.BeginTransaction())
            {
                try
                {
                    GSTMaster GSTMasterModel = dbContext.GSTMasters.SingleOrDefault(row => row.RowKey == model.RowKey);
                    dbContext.GSTMasters.Remove(GSTMasterModel);
                    dbContext.SaveChanges();
                    transaction.Commit();
                    model.Message = EduSuiteUIResources.Success;
                    model.IsSuccessful = true;
                    ActivityLog.CreateActivityLog(MenuConstants.GSTMaster, ActionConstants.Delete, DbConstants.LogType.Info, model.RowKey, model.Message);
                }
                catch (DbUpdateException ex)
                {
                    transaction.Rollback();
                    if (ex.GetBaseException().Message.ToUpper().Contains(EduSuiteUIResources.ForeignKeyError.ToUpper()))
                    {
                        model.Message = String.Format(EduSuiteUIResources.CantDeleteErrorMessage, EduSuiteUIResources.GSTMaster);
                        model.IsSuccessful = false;
                        ActivityLog.CreateActivityLog(MenuConstants.GSTMaster, ActionConstants.Delete, DbConstants.LogType.Debug, model.RowKey, ex.GetBaseException().Message);
                    }
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    model.Message = String.Format(EduSuiteUIResources.FailedToDeleteErrorMessage, EduSuiteUIResources.GSTMaster);
                    model.IsSuccessful = false;
                    ActivityLog.CreateActivityLog(MenuConstants.GSTMaster, ActionConstants.Delete, DbConstants.LogType.Debug, model.RowKey, ex.GetBaseException().Message);
                }
            }

            return model;
        }
    }
}
