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
    public class ApplicationScheduleTypeService : IApplicationScheduleTypeService
    {
        private EduSuiteDatabase dbContext;
        public ApplicationScheduleTypeService(EduSuiteDatabase objDb)
        {
            this.dbContext = objDb;
        }
        public List<ApplicationScheduleTypeViewModel> GetApplicationScheduleType(string serachText)
        {
            try
            {
                var ApplicationScheduleTypeList = (from PT in dbContext.ApplicationScheduleTypes
                                                   where (PT.ScheduleTypeName.Contains(serachText))
                                                   select new ApplicationScheduleTypeViewModel
                                                   {
                                                       RowKey = PT.RowKey,
                                                       ScheduleTypeName = PT.ScheduleTypeName,
                                                       IsActive = PT.IsActive,
                                                       IsSystem = PT.IsSystem

                                                   }).ToList();
                return ApplicationScheduleTypeList.GroupBy(x => x.RowKey).Select(y => y.First()).ToList<ApplicationScheduleTypeViewModel>();

            }
            catch (Exception ex)
            {
                ActivityLog.CreateActivityLog(MenuConstants.ApplicationScheduleType, ActionConstants.View, DbConstants.LogType.Error, null, ex.GetBaseException().Message);
                return new List<ApplicationScheduleTypeViewModel>();

            }

        }
        public ApplicationScheduleTypeViewModel GetApplicationScheduleTypeById(short? id)
        {
            try
            {
                ApplicationScheduleTypeViewModel model = new ApplicationScheduleTypeViewModel();
                model = dbContext.ApplicationScheduleTypes.Select(row => new ApplicationScheduleTypeViewModel
                {
                    RowKey = row.RowKey,
                    ScheduleTypeName = row.ScheduleTypeName,
                    IsActive = row.IsActive,
                    IsSystem = row.IsSystem
                }).Where(x => x.RowKey == id).FirstOrDefault();
                if (model == null)
                {
                    model = new ApplicationScheduleTypeViewModel();
                }


                return model;
            }
            catch (Exception ex)
            {
                ActivityLog.CreateActivityLog(MenuConstants.ApplicationScheduleType, ActionConstants.View, DbConstants.LogType.Error, null, ex.GetBaseException().Message);
                return new ApplicationScheduleTypeViewModel();

            }
        }
        public ApplicationScheduleTypeViewModel CreateApplicationScheduleType(ApplicationScheduleTypeViewModel model)
        {
            ApplicationScheduleType ApplicationScheduleTypemodel = new ApplicationScheduleType();
            var ApplicationScheduleTypeCheck = dbContext.ApplicationScheduleTypes.Where(row => row.ScheduleTypeName.ToLower() == model.ScheduleTypeName.ToLower()).Count();

            if (ApplicationScheduleTypeCheck != 0)
            {
                model.Message = String.Format(EduSuiteUIResources.ExistsErrorMessage, EduSuiteUIResources.ScheduleType);
                model.IsSuccessful = false;
                return model;
            }

            using (var transaction = dbContext.Database.BeginTransaction())
            {
                try
                {
                    int MaxKey = dbContext.ApplicationScheduleTypes.Select(p => p.RowKey).DefaultIfEmpty().Max();

                    ApplicationScheduleTypemodel.RowKey = Convert.ToInt16(MaxKey + 1);
                    ApplicationScheduleTypemodel.ScheduleTypeName = model.ScheduleTypeName;
                    ApplicationScheduleTypemodel.IsSystem = false;
                    ApplicationScheduleTypemodel.IsActive = model.IsActive;
                    dbContext.ApplicationScheduleTypes.Add(ApplicationScheduleTypemodel);
                    dbContext.SaveChanges();
                    transaction.Commit();
                    model.Message = EduSuiteUIResources.Success;
                    model.IsSuccessful = true;
                    ActivityLog.CreateActivityLog(MenuConstants.ApplicationScheduleType, ActionConstants.Add, DbConstants.LogType.Info, model.RowKey, model.Message);

                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    model.Message = String.Format(EduSuiteUIResources.FailedToSaveErrorMessage, EduSuiteUIResources.ScheduleType);
                    model.IsSuccessful = false;
                    ActivityLog.CreateActivityLog(MenuConstants.ApplicationScheduleType, ActionConstants.Add, DbConstants.LogType.Error, model.RowKey, ex.GetBaseException().Message);

                }
            }
            return model;
        }
        public ApplicationScheduleTypeViewModel UpdateApplicationScheduleType(ApplicationScheduleTypeViewModel model)
        {
            var ApplicationScheduleTypeCheck = dbContext.ApplicationScheduleTypes.Where(row => row.ScheduleTypeName.ToLower() == model.ScheduleTypeName.ToLower()
                 && row.RowKey != model.RowKey).ToList();

            ApplicationScheduleType ApplicationScheduleTypemodel = new ApplicationScheduleType();

            if (ApplicationScheduleTypeCheck.Count != 0)
            {
                model.Message = String.Format(EduSuiteUIResources.ExistsErrorMessage, EduSuiteUIResources.ScheduleType);
                model.IsSuccessful = false;
                return model;
            }

            using (var transaction = dbContext.Database.BeginTransaction())
            {
                try
                {
                    ApplicationScheduleTypemodel = dbContext.ApplicationScheduleTypes.SingleOrDefault(x => x.RowKey == model.RowKey);
                    ApplicationScheduleTypemodel.ScheduleTypeName = model.ScheduleTypeName;                
                    ApplicationScheduleTypemodel.IsActive = model.IsActive;
                    dbContext.SaveChanges();
                    transaction.Commit();
                    model.Message = EduSuiteUIResources.Success;
                    model.IsSuccessful = true;
                    ActivityLog.CreateActivityLog(MenuConstants.ApplicationScheduleType, ActionConstants.Edit, DbConstants.LogType.Info, model.RowKey, model.Message);

                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    model.Message = String.Format(EduSuiteUIResources.FailedToSaveErrorMessage, EduSuiteUIResources.ScheduleType);
                    model.IsSuccessful = false;
                    ActivityLog.CreateActivityLog(MenuConstants.ApplicationScheduleType, ActionConstants.Edit, DbConstants.LogType.Error, model.RowKey, ex.GetBaseException().Message);

                }
            }
            return model;
        }
        public ApplicationScheduleTypeViewModel DeleteApplicationScheduleType(ApplicationScheduleTypeViewModel model)
        {
            using (var transaction = dbContext.Database.BeginTransaction())
            {
                try
                {
                    ApplicationScheduleType ApplicationScheduleTypemodel = dbContext.ApplicationScheduleTypes.SingleOrDefault(row => row.RowKey == model.RowKey);
                    dbContext.ApplicationScheduleTypes.Remove(ApplicationScheduleTypemodel);
                    dbContext.SaveChanges();
                    transaction.Commit();
                    model.Message = EduSuiteUIResources.Success;
                    model.IsSuccessful = true;
                    ActivityLog.CreateActivityLog(MenuConstants.ApplicationScheduleType, ActionConstants.Delete, DbConstants.LogType.Info, model.RowKey, model.Message);
                }
                catch (DbUpdateException ex)
                {
                    transaction.Rollback();
                    if (ex.GetBaseException().Message.ToUpper().Contains(EduSuiteUIResources.ForeignKeyError.ToUpper()))
                    {
                        model.Message = String.Format(EduSuiteUIResources.CantDeleteErrorMessage, EduSuiteUIResources.ScheduleType);
                        model.IsSuccessful = false;
                        ActivityLog.CreateActivityLog(MenuConstants.ApplicationScheduleType, ActionConstants.Delete, DbConstants.LogType.Debug, model.RowKey, ex.GetBaseException().Message);

                    }
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    model.Message = String.Format(EduSuiteUIResources.FailedToDeleteErrorMessage, EduSuiteUIResources.ScheduleType);
                    model.IsSuccessful = false;
                    ActivityLog.CreateActivityLog(MenuConstants.ApplicationScheduleType, ActionConstants.Delete, DbConstants.LogType.Error, model.RowKey, ex.GetBaseException().Message);
                }
            }

            return model;
        }

    }
}
