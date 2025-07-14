using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CITS.EduSuite.Business.Common;
using CITS.EduSuite.Business.Interfaces;
using CITS.EduSuite.Business.Models.ViewModels;
using CITS.EduSuite.Data;
using System.Data.Entity.Infrastructure;
using CITS.EduSuite.Business.Models.Resources;

namespace CITS.EduSuite.Business.Services
{
    public class ClassModeService : IClassModeService
    {
        private EduSuiteDatabase dbContext;
        public ClassModeService(EduSuiteDatabase objDB)
        {
            this.dbContext = objDB;
        }
        public List<ClassModeViewModel> GetClassMode(string searchText)
        {
            try
            {
                var ClassModeList = (from p in dbContext.ClassModes
                                     orderby p.RowKey descending
                                     where (p.ClassModeName.Contains(searchText))
                                     select new ClassModeViewModel
                                     {
                                         RowKey = p.RowKey,
                                         ClassModeName = p.ClassModeName,
                                         IsActive = p.IsActive
                                     }).ToList();
                return ClassModeList.GroupBy(x => x.RowKey).Select(y => y.First()).ToList<ClassModeViewModel>();
            }
            catch (Exception ex)
            {
                ActivityLog.CreateActivityLog(MenuConstants.ClassMode, ActionConstants.View, DbConstants.LogType.Error, null, ex.GetBaseException().Message);
                return new List<ClassModeViewModel>();
            }
        }
        public ClassModeViewModel GetClassModeById(int? id)
        {
            try
            {
                ClassModeViewModel model = new ClassModeViewModel();
                model = dbContext.ClassModes.Select(row => new ClassModeViewModel
                {
                    RowKey = row.RowKey,
                    ClassModeName = row.ClassModeName,
                    IsActive = row.IsActive
                }).Where(x => x.RowKey == id).FirstOrDefault();
                if (model == null)
                {
                    model = new ClassModeViewModel();
                }
                return model;
            }
            catch (Exception ex)
            {
                ActivityLog.CreateActivityLog(MenuConstants.ClassMode, ActionConstants.View, DbConstants.LogType.Error, id, ex.GetBaseException().Message);
                return new ClassModeViewModel();

            }
        }
        public ClassModeViewModel CreateClassMode(ClassModeViewModel model)
        {
            var ClassModeCheck = dbContext.ClassModes.Where(row => row.ClassModeName.ToLower() == model.ClassModeName.ToLower()).Count();
            ClassMode ClassModeModel = new ClassMode();
            if (ClassModeCheck != 0)
            {
                model.Message = String.Format(EduSuiteUIResources.ExistsErrorMessage, EduSuiteUIResources.ClassMode);
                model.IsSuccessful = false;
                return model;
            }

            using (var transaction = dbContext.Database.BeginTransaction())
            {
                try
                {
                    int MaxKey = dbContext.ClassModes.Select(p => p.RowKey).DefaultIfEmpty().Max();

                    ClassModeModel.RowKey = Convert.ToInt16(MaxKey + 1);
                    ClassModeModel.ClassModeName = model.ClassModeName;
                    ClassModeModel.IsActive = model.IsActive;
                    dbContext.ClassModes.Add(ClassModeModel);
                    dbContext.SaveChanges();
                    transaction.Commit();
                    model.Message = EduSuiteUIResources.Success;
                    model.IsSuccessful = true;

                    ActivityLog.CreateActivityLog(MenuConstants.ClassMode, ActionConstants.Add, DbConstants.LogType.Info, ClassModeModel.RowKey, model.Message);
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    model.Message = String.Format(EduSuiteUIResources.FailedToSaveErrorMessage, EduSuiteUIResources.ClassMode);
                    model.IsSuccessful = false;
                    ActivityLog.CreateActivityLog(MenuConstants.ClassMode, ActionConstants.Add, DbConstants.LogType.Error, model.RowKey, ex.GetBaseException

().Message);
                }
            }
            return model;
        }
        public ClassModeViewModel UpdateClassMode(ClassModeViewModel model)
        {
            var ClassModeCheck = dbContext.ClassModes.Where(row => row.ClassModeName.ToLower() == model.ClassModeName.ToLower()
               && row.RowKey != model.RowKey).ToList();

            ClassMode ClassModeModel = new ClassMode();
            if (ClassModeCheck.Count != 0)
            {
                model.Message = String.Format(EduSuiteUIResources.ExistsErrorMessage, EduSuiteUIResources.ClassMode);
                model.IsSuccessful = false;
                return model;
            }

            using (var transaction = dbContext.Database.BeginTransaction())
            {
                try
                {
                    ClassModeModel = dbContext.ClassModes.SingleOrDefault(x => x.RowKey == model.RowKey);
                    ClassModeModel.ClassModeName = model.ClassModeName;
                    ClassModeModel.IsActive = model.IsActive;
                    dbContext.SaveChanges();
                    transaction.Commit();
                    model.Message = EduSuiteUIResources.Success;
                    model.IsSuccessful = true;

                    ActivityLog.CreateActivityLog(MenuConstants.ClassMode, ActionConstants.Edit, DbConstants.LogType.Info, model.RowKey, model.Message);
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    model.Message = String.Format(EduSuiteUIResources.FailedToSaveErrorMessage, EduSuiteUIResources.ClassMode);
                    model.IsSuccessful = false;

                    ActivityLog.CreateActivityLog(MenuConstants.ClassMode, ActionConstants.Edit, DbConstants.LogType.Error, model.RowKey, ex.GetBaseException

().Message);
                }
            }
            return model;

        }
        public ClassModeViewModel DeleteClassMode(ClassModeViewModel model)
        {
            using (var transaction = dbContext.Database.BeginTransaction())
            {
                try
                {
                    ClassMode ClassModeModel = dbContext.ClassModes.SingleOrDefault(row => row.RowKey == model.RowKey);
                    dbContext.ClassModes.Remove(ClassModeModel);
                    dbContext.SaveChanges();
                    transaction.Commit();
                    model.Message = EduSuiteUIResources.Success;
                    model.IsSuccessful = true;
                    ActivityLog.CreateActivityLog(MenuConstants.ClassMode, ActionConstants.Delete, DbConstants.LogType.Info, model.RowKey, model.Message);
                }
                catch (DbUpdateException ex)
                {
                    transaction.Rollback();
                    if (ex.GetBaseException().Message.ToUpper().Contains(EduSuiteUIResources.ForeignKeyError.ToUpper()))
                    {
                        model.Message = String.Format(EduSuiteUIResources.CantDeleteErrorMessage, EduSuiteUIResources.ClassMode);
                        model.IsSuccessful = false;
                        ActivityLog.CreateActivityLog(MenuConstants.ClassMode, ActionConstants.Delete, DbConstants.LogType.Debug, model.RowKey, ex.GetBaseException().Message);
                    }
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    model.Message = String.Format(EduSuiteUIResources.FailedToDeleteErrorMessage, EduSuiteUIResources.ClassMode);
                    model.IsSuccessful = false;
                    ActivityLog.CreateActivityLog(MenuConstants.ClassMode, ActionConstants.Delete, DbConstants.LogType.Debug, model.RowKey, ex.GetBaseException().Message);
                }
            }

            return model;
        }
      
    }
}
