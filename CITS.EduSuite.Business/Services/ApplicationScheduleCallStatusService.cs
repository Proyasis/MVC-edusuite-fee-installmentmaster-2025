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
    public class ApplicationScheduleCallStatusService : IApplicationScheduleCallStatusService
    {
        private EduSuiteDatabase dbContext;

        public ApplicationScheduleCallStatusService(EduSuiteDatabase objDb)
        {
            this.dbContext = objDb;
        }
        public List<ApplicationScheduleCallStatusViewModel> GetApplicationScheduleCallStatus(string serachText)
        {
            try
            {
                var ApplicationScheduleCallStatusList = (from PT in dbContext.ApplicationScheduleCallStatus
                                             where (PT.ApplicationCallStatusName.Contains(serachText))
                                             select new ApplicationScheduleCallStatusViewModel
                                             {
                                                 RowKey = PT.RowKey,
                                                 ApplicationCallStatusName = PT.ApplicationCallStatusName,
                                                 IsActive = PT.IsActive,
                                                 IsSystem=PT.IsSystem
                                             }).ToList();
                return ApplicationScheduleCallStatusList.GroupBy(x => x.RowKey).Select(y => y.First()).ToList<ApplicationScheduleCallStatusViewModel>();

            }
            catch (Exception ex)
            {
                ActivityLog.CreateActivityLog(MenuConstants.ApplicationScheduleCallStatus, ActionConstants.View, DbConstants.LogType.Error, null, ex.GetBaseException().Message);
                return new List<ApplicationScheduleCallStatusViewModel>();
               
            }

        }
        public ApplicationScheduleCallStatusViewModel GetApplicationScheduleCallStatusById(short? id)
        {
            try
            {
                ApplicationScheduleCallStatusViewModel model = new ApplicationScheduleCallStatusViewModel();
                model = dbContext.ApplicationScheduleCallStatus.Select(row => new ApplicationScheduleCallStatusViewModel
                {
                    RowKey = row.RowKey,
                    ApplicationCallStatusName = row.ApplicationCallStatusName,
                    IsDuration=row.IsDuration,
                    ShowInMenuKeysList=row.ShowInMenuKeys,
                    IsActive = row.IsActive,
                     IsSystem = row.IsSystem
                }).Where(x => x.RowKey == id).FirstOrDefault();
                if (model == null)
                {
                    model = new ApplicationScheduleCallStatusViewModel();
                }

                if (model.ShowInMenuKeysList != null)
                {
                    model.ShowInMenuKeys = model.ShowInMenuKeysList.Split(',').Select(Int32.Parse).ToList();
                }
               
                FillMenus(model);
                return model;
            }
            catch (Exception ex)
            {
                ActivityLog.CreateActivityLog(MenuConstants.ApplicationScheduleCallStatus, ActionConstants.View, DbConstants.LogType.Error, null, ex.GetBaseException().Message);
                return new ApplicationScheduleCallStatusViewModel();
               
            }
        }
        public ApplicationScheduleCallStatusViewModel CreateApplicationScheduleCallStatus(ApplicationScheduleCallStatusViewModel model)
        {
            ApplicationScheduleCallStatu ApplicationScheduleCallStatusmodel = new ApplicationScheduleCallStatu();
            var ApplicationScheduleCallStatusCheck = dbContext.ApplicationScheduleCallStatus.Where(row => row.ApplicationCallStatusName.ToLower() == model.ApplicationCallStatusName.ToLower()).Count();
         
            if (ApplicationScheduleCallStatusCheck != 0)
            {
                model.Message = String.Format(EduSuiteUIResources.ExistsErrorMessage, EduSuiteUIResources.ApplicationScheduleCallStatus);
                model.IsSuccessful = false;
                return model;
            }

            using (var transaction = dbContext.Database.BeginTransaction())
            {
                try
                {
                    int MaxKey = dbContext.ApplicationScheduleCallStatus.Select(p => p.RowKey).DefaultIfEmpty().Max();

                    ApplicationScheduleCallStatusmodel.RowKey = Convert.ToInt16(MaxKey + 1);
                    ApplicationScheduleCallStatusmodel.ApplicationCallStatusName = model.ApplicationCallStatusName;
                    ApplicationScheduleCallStatusmodel.IsDuration = model.IsDuration;
                    ApplicationScheduleCallStatusmodel.IsActive = model.IsActive;
                    ApplicationScheduleCallStatusmodel.IsSystem = model.IsSystem;
                    ApplicationScheduleCallStatusmodel.ShowInMenuKeys = model.ShowInMenuKeysList;
                    dbContext.ApplicationScheduleCallStatus.Add(ApplicationScheduleCallStatusmodel);
                    dbContext.SaveChanges();
                    transaction.Commit();
                    model.Message = EduSuiteUIResources.Success;
                    model.IsSuccessful = true;
                    ActivityLog.CreateActivityLog(MenuConstants.ApplicationScheduleCallStatus, ActionConstants.Add, DbConstants.LogType.Info, model.RowKey, model.Message);

                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    model.Message = String.Format(EduSuiteUIResources.FailedToSaveErrorMessage, EduSuiteUIResources.ApplicationScheduleCallStatus);
                    model.IsSuccessful = false;
                    ActivityLog.CreateActivityLog(MenuConstants.ApplicationScheduleCallStatus, ActionConstants.Add, DbConstants.LogType.Error, model.RowKey, ex.GetBaseException().Message);

                }
            }
            return model;
        }
        public ApplicationScheduleCallStatusViewModel UpdateApplicationScheduleCallStatus(ApplicationScheduleCallStatusViewModel model)
        {
            var ApplicationScheduleCallStatusCheck = dbContext.ApplicationScheduleCallStatus.Where(row =>row.ApplicationCallStatusName.ToLower() == model.ApplicationCallStatusName.ToLower()
                 && row.RowKey != model.RowKey).ToList();

            ApplicationScheduleCallStatu ApplicationScheduleCallStatusmodel = new ApplicationScheduleCallStatu();
           
            if (ApplicationScheduleCallStatusCheck.Count != 0)
            {
                model.Message = String.Format(EduSuiteUIResources.ExistsErrorMessage, EduSuiteUIResources.ApplicationScheduleCallStatus);
                model.IsSuccessful = false;
                return model;
            }

            using (var transaction = dbContext.Database.BeginTransaction())
            {
                try
                {
                    ApplicationScheduleCallStatusmodel = dbContext.ApplicationScheduleCallStatus.SingleOrDefault(x => x.RowKey == model.RowKey);
                    ApplicationScheduleCallStatusmodel.ApplicationCallStatusName = model.ApplicationCallStatusName;
                    ApplicationScheduleCallStatusmodel.IsDuration = model.IsDuration;
                    ApplicationScheduleCallStatusmodel.ShowInMenuKeys = model.ShowInMenuKeysList;
                    ApplicationScheduleCallStatusmodel.IsActive = model.IsActive;
                    ApplicationScheduleCallStatusmodel.IsSystem = model.IsSystem;
                    dbContext.SaveChanges();
                    transaction.Commit();
                    model.Message = EduSuiteUIResources.Success;
                    model.IsSuccessful = true;
                    ActivityLog.CreateActivityLog(MenuConstants.ApplicationScheduleCallStatus, ActionConstants.Edit, DbConstants.LogType.Info, model.RowKey, model.Message);

                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    model.Message = String.Format(EduSuiteUIResources.FailedToSaveErrorMessage, EduSuiteUIResources.ApplicationScheduleCallStatus);
                    model.IsSuccessful = false;
                    ActivityLog.CreateActivityLog(MenuConstants.ApplicationScheduleCallStatus, ActionConstants.Edit, DbConstants.LogType.Error, model.RowKey, ex.GetBaseException().Message);

                }
            }
            return model;
        }
        public ApplicationScheduleCallStatusViewModel DeleteApplicationScheduleCallStatus(ApplicationScheduleCallStatusViewModel model)
        {
            using (var transaction = dbContext.Database.BeginTransaction())
            {
                try
                {
                    ApplicationScheduleCallStatu ApplicationScheduleCallStatusmodel = dbContext.ApplicationScheduleCallStatus.SingleOrDefault(row => row.RowKey == model.RowKey);
                    dbContext.ApplicationScheduleCallStatus.Remove(ApplicationScheduleCallStatusmodel);
                    dbContext.SaveChanges();
                    transaction.Commit();
                    model.Message = EduSuiteUIResources.Success;
                    model.IsSuccessful = true;
                    ActivityLog.CreateActivityLog(MenuConstants.ApplicationScheduleCallStatus, ActionConstants.Delete, DbConstants.LogType.Info, model.RowKey, model.Message);

        
                }
                catch (DbUpdateException ex)
                {
                    transaction.Rollback();
                    if (ex.GetBaseException().Message.ToUpper().Contains(EduSuiteUIResources.ForeignKeyError.ToUpper()))
                    {
                        model.Message = String.Format(EduSuiteUIResources.CantDeleteErrorMessage, EduSuiteUIResources.ApplicationScheduleCallStatus);
                        model.IsSuccessful = false;
                        ActivityLog.CreateActivityLog(MenuConstants.ApplicationScheduleCallStatus, ActionConstants.Delete, DbConstants.LogType.Debug, model.RowKey, ex.GetBaseException().Message);

                    }
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    model.Message = String.Format(EduSuiteUIResources.FailedToDeleteErrorMessage, EduSuiteUIResources.ApplicationScheduleCallStatus);
                    model.IsSuccessful = false;
                    ActivityLog.CreateActivityLog(MenuConstants.ApplicationScheduleCallStatus, ActionConstants.Delete, DbConstants.LogType.Error, model.RowKey, ex.GetBaseException().Message);

                }
            }

            return model;
        }
        private void FillMenus(ApplicationScheduleCallStatusViewModel model)
        {
            model.MenuList = dbContext.VwMenuSelectActiveOnlies.Where(
                    x=>
                    x.RowKey == DbConstants.Menu.Enquiry
                 ||
                    x.RowKey == DbConstants.Menu.EnquiryLead                     
                ).OrderBy(row => row.RowKey).Select(row => new SelectListModel
            {
                RowKey = row.RowKey,
                Text = row.MenuName
            }).ToList();
        }

    }
}
