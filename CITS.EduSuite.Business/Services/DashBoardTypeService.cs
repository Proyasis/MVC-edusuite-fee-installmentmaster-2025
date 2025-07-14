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
   public class DashBoardTypeService: IDashBoardTypeService
    {
        private EduSuiteDatabase dbContext;
        public DashBoardTypeService(EduSuiteDatabase objDB)
        {
            this.dbContext = objDB;
        }

        public List<DashBoardTypeViewModel> GetDashBoardType(string searchText)
        {
            try
            {
                var DashBoardTypeList = (from p in dbContext.DashBoardTypes
                                orderby p.RowKey descending
                                where (p.DashBoardTypeName.Contains(searchText))
                                select new DashBoardTypeViewModel
                                {
                                    RowKey = p.RowKey,
                                    DashBoardTypeName = p.DashBoardTypeName,
                                    DashBoardTypeCode = p.DashBoardTypeCode,
                                    DisplayOrder = p.DisplayOrder,
                                    IconCLassName = p.IconCLassName,
                                    IsActive = p.IsActive,
                                }).ToList();
                return DashBoardTypeList.GroupBy(x => x.RowKey).Select(y => y.First()).ToList<DashBoardTypeViewModel>();
            }
            catch (Exception ex)
            {
                ActivityLog.CreateActivityLog(MenuConstants.DashBoardType, ActionConstants.View, DbConstants.LogType.Error, null, ex.GetBaseException().Message);
                return new List<DashBoardTypeViewModel>();
            }
        }
        public DashBoardTypeViewModel GetDashBoardTypeById(int? id)
        {
            try
            {
                DashBoardTypeViewModel model = new DashBoardTypeViewModel();
                model = dbContext.DashBoardTypes.Select(row => new DashBoardTypeViewModel
                {
                    RowKey = row.RowKey,
                    DashBoardTypeName = row.DashBoardTypeName,
                    DashBoardTypeCode = row.DashBoardTypeCode,
                    DisplayOrder = row.DisplayOrder,
                    IconCLassName = row.IconCLassName,
                    IsActive = row.IsActive
                }).Where(x => x.RowKey == id).FirstOrDefault();
                if (model == null)
                {
                    model = new DashBoardTypeViewModel();
                }
                return model;
            }
            catch (Exception ex)
            {
                ActivityLog.CreateActivityLog(MenuConstants.DashBoardType, ActionConstants.View, DbConstants.LogType.Error, id, ex.GetBaseException().Message);
                return new DashBoardTypeViewModel();
            }
        }
        public DashBoardTypeViewModel CreateDashBoardType(DashBoardTypeViewModel model)
        {
            var DashBoardTypeCheck = dbContext.DashBoardTypes.Where(row => row.DashBoardTypeName.ToLower() == model.DashBoardTypeName.ToLower()).Count();
            DashBoardType DashBoardTypeModel = new DashBoardType();
            if (DashBoardTypeCheck != 0)
            {
                model.Message = String.Format(EduSuiteUIResources.ExistsErrorMessage, EduSuiteUIResources.DashBoardType);
                model.IsSuccessful = false;
                return model;
            }

            using (var transaction = dbContext.Database.BeginTransaction())
            {
                try
                {
                    int MaxKey = dbContext.DashBoardTypes.Select(p => p.RowKey).DefaultIfEmpty().Max();

                    DashBoardTypeModel.RowKey = Convert.ToInt16(MaxKey + 1);
                    DashBoardTypeModel.DashBoardTypeName = model.DashBoardTypeName;
                    DashBoardTypeModel.DashBoardTypeCode = model.DashBoardTypeCode;
                    DashBoardTypeModel.DisplayOrder = DashBoardTypeModel.RowKey;
                    DashBoardTypeModel.IsActive = model.IsActive;
                    dbContext.DashBoardTypes.Add(DashBoardTypeModel);
                    dbContext.SaveChanges();
                    transaction.Commit();
                    model.Message = EduSuiteUIResources.Success;
                    model.IsSuccessful = true;

                    ActivityLog.CreateActivityLog(MenuConstants.DashBoardType, ActionConstants.Add, DbConstants.LogType.Info, model.RowKey, model.Message);
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    model.Message = String.Format(EduSuiteUIResources.FailedToSaveErrorMessage, EduSuiteUIResources.DashBoardType);
                    model.IsSuccessful = false;
                    ActivityLog.CreateActivityLog(MenuConstants.DashBoardType, ActionConstants.Add, DbConstants.LogType.Error, model.RowKey, ex.GetBaseException().Message);
                }
            }
            return model;
        }
        public DashBoardTypeViewModel UpdateDashBoardType(DashBoardTypeViewModel model)
        {
            var DashBoardTypeCheck = dbContext.DashBoardTypes.Where(row => row.DashBoardTypeName.ToLower() == model.DashBoardTypeName.ToLower()
               && row.RowKey != model.RowKey).ToList();

            DashBoardType DashBoardTypeModel = new DashBoardType();
            if (DashBoardTypeCheck.Count != 0)
            {
                model.Message = String.Format(EduSuiteUIResources.ExistsErrorMessage, EduSuiteUIResources.DashBoardType);
                model.IsSuccessful = false;
                return model;
            }

            using (var transaction = dbContext.Database.BeginTransaction())
            {
                try
                {
                    DashBoardTypeModel = dbContext.DashBoardTypes.SingleOrDefault(x => x.RowKey == model.RowKey);
                    DashBoardTypeModel.DashBoardTypeName = model.DashBoardTypeName;
                    DashBoardTypeModel.DashBoardTypeCode = model.DashBoardTypeCode;
                    DashBoardTypeModel.IsActive = model.IsActive;
                    dbContext.SaveChanges();
                    transaction.Commit();
                    model.Message = EduSuiteUIResources.Success;
                    model.IsSuccessful = true;

                    ActivityLog.CreateActivityLog(MenuConstants.DashBoardType, ActionConstants.Edit, DbConstants.LogType.Info, model.RowKey, model.Message);
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    model.Message = String.Format(EduSuiteUIResources.FailedToSaveErrorMessage, EduSuiteUIResources.DashBoardType);
                    model.IsSuccessful = false;
                    ActivityLog.CreateActivityLog(MenuConstants.DashBoardType, ActionConstants.Edit, DbConstants.LogType.Error, model.RowKey, ex.GetBaseException().Message);
                }
            }
            return model;

        }
        public DashBoardTypeViewModel DeleteDashBoardType(DashBoardTypeViewModel model)
        {
            using (var transaction = dbContext.Database.BeginTransaction())
            {
                try
                {
                    DashBoardType DashBoardTypeModel = dbContext.DashBoardTypes.SingleOrDefault(row => row.RowKey == model.RowKey);
                    dbContext.DashBoardTypes.Remove(DashBoardTypeModel);
                    dbContext.SaveChanges();
                    transaction.Commit();
                    model.Message = EduSuiteUIResources.Success;
                    model.IsSuccessful = true;
                    ActivityLog.CreateActivityLog(MenuConstants.DashBoardType, ActionConstants.Delete, DbConstants.LogType.Info, model.RowKey, model.Message);
                }
                catch (DbUpdateException ex)
                {
                    transaction.Rollback();
                    if (ex.GetBaseException().Message.ToUpper().Contains(EduSuiteUIResources.ForeignKeyError.ToUpper()))
                    {
                        model.Message = String.Format(EduSuiteUIResources.CantDeleteErrorMessage, EduSuiteUIResources.DashBoardType);
                        model.IsSuccessful = false;
                        ActivityLog.CreateActivityLog(MenuConstants.DashBoardType, ActionConstants.Delete, DbConstants.LogType.Debug, model.RowKey, ex.GetBaseException().Message);
                    }
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    model.Message = String.Format(EduSuiteUIResources.FailedToDeleteErrorMessage, EduSuiteUIResources.DashBoardType);
                    model.IsSuccessful = false;
                    ActivityLog.CreateActivityLog(MenuConstants.DashBoardType, ActionConstants.Delete, DbConstants.LogType.Debug, model.RowKey, ex.GetBaseException().Message);
                }
            }

            return model;
        }
    }
}
