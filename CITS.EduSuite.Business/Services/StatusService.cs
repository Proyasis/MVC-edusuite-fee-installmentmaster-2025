using CITS.EduSuite.Business.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CITS.EduSuite.Data;
using CITS.EduSuite.Business.Models.ViewModels;
using CITS.EduSuite.Business.Common;
using System.Data.Entity.Infrastructure;
using CITS.EduSuite.Business.Models.Resources;

namespace CITS.EduSuite.Business.Services
{
    public class StatusService : IStatusService
    {
        private EduSuiteDatabase dbContext;

        public StatusService(EduSuiteDatabase objDB)
        {
            this.dbContext = objDB;
        }

        public List<StatusViewModel> GetStatuses(string searchText)
        {
            try
            {
                var StatusesList = (from r in dbContext.Status
                                    orderby r.StatusName
                                    where (r.StatusName.Contains(searchText))
                                    select new StatusViewModel
                                    {
                                        RowKey = r.RowKey,
                                        StatusName = r.StatusName,
                                        //IsActiveText = r.IsActive == true ? ApplicationResources.Yes : ApplicationResources.No,
                                    }).ToList();

                return StatusesList.GroupBy(x => x.RowKey).Select(y => y.First()).ToList<StatusViewModel>();
            }
            catch (Exception ex)
            {
                ActivityLog.CreateActivityLog(MenuConstants.Status, ActionConstants.View, DbConstants.LogType.Error, null, ex.GetBaseException().Message);
                return new List<StatusViewModel>();
                

            }
        }

        public StatusViewModel GetStatusById(Int16? id)
        {
            try
            {
                StatusViewModel model = new StatusViewModel();
                model = dbContext.Status.Select(row => new StatusViewModel
                {
                    RowKey = row.RowKey,
                    StatusName = row.StatusName,
                    IsActive = row.IsActive

                }).Where(x => x.RowKey == id).FirstOrDefault();
                if (model == null)
                {
                    model = new StatusViewModel();
                }

                return model;
            }
            catch (Exception ex)
            {
                ActivityLog.CreateActivityLog(MenuConstants.Status, ActionConstants.View, DbConstants.LogType.Error, id, ex.GetBaseException().Message);
                return new StatusViewModel();
                

            }
        }

        public StatusViewModel CreateStatus(StatusViewModel model)
        {
            //using (cPOSEntities dbContext = new cPOSEntities())
            //{
            var statusNameCheck = dbContext.Status.Where(row => row.StatusName.ToLower() == model.StatusName.ToLower()).ToList();
            Status statusModel = new Status();

            if (statusNameCheck.Count != 0)
            {
                model.Message = String.Format(EduSuiteUIResources.ExistsErrorMessage, EduSuiteUIResources.Status);
                model.IsSuccessful = false;
                return model;
            }

            using (var transaction = dbContext.Database.BeginTransaction())
            {
                try
                {

                    Int16 maxKey = dbContext.Status.Select(p => p.RowKey).DefaultIfEmpty().Max();

                    statusModel.RowKey = Convert.ToInt16(maxKey + 1);
                    statusModel.StatusName = model.StatusName;
                    statusModel.IsActive = model.IsActive;
                    dbContext.Status.Add(statusModel);

                    dbContext.SaveChanges();
                    transaction.Commit();
                    model.Message = EduSuiteUIResources.Success;
                    model.IsSuccessful = true;
                    ActivityLog.CreateActivityLog(MenuConstants.Status, ActionConstants.Add, DbConstants.LogType.Info, statusModel.RowKey, model.Message);

                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    model.Message = String.Format(EduSuiteUIResources.FailedToSaveErrorMessage, EduSuiteUIResources.Status);
                    model.IsSuccessful = false;
                    ActivityLog.CreateActivityLog(MenuConstants.Status, ActionConstants.Add, DbConstants.LogType.Error, model.RowKey, ex.GetBaseException

().Message);

                }
            }
            //}
            return model;
        }

        public StatusViewModel UpdateStatus(StatusViewModel model)
        {
            //using (cPOSEntities dbContext = new cPOSEntities())
            //{
            var statusNameCheck = dbContext.Status.Where(row => row.StatusName.ToLower() == model.StatusName.ToLower() && row.RowKey != model.RowKey).ToList();
            Status statusModel = new Status();

            if (statusNameCheck.Count != 0)
            {
                model.Message = String.Format(EduSuiteUIResources.ExistsErrorMessage, EduSuiteUIResources.Status);
                model.IsSuccessful = false;
                return model;
            }

            using (var transaction = dbContext.Database.BeginTransaction())
            {
                try
                {
                    statusModel = dbContext.Status.SingleOrDefault(row => row.RowKey == model.RowKey);
                    statusModel.StatusName = model.StatusName;
                    statusModel.IsActive = model.IsActive;

                    dbContext.SaveChanges();
                    transaction.Commit();
                    model.Message = EduSuiteUIResources.Success;
                    model.IsSuccessful = true;
                    ActivityLog.CreateActivityLog(MenuConstants.Status, ActionConstants.Edit, DbConstants.LogType.Info, model.RowKey, model.Message);

                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    model.Message = String.Format(EduSuiteUIResources.FailedToSaveErrorMessage, EduSuiteUIResources.Status);
                    model.IsSuccessful = false;
                    ActivityLog.CreateActivityLog(MenuConstants.Status, ActionConstants.Edit, DbConstants.LogType.Error, model.RowKey, ex.GetBaseException().Message);
                }
            }
            //}
            return model;
        }

        public StatusViewModel DeleteStatus(StatusViewModel model)
        {
            //using (cPOSEntities dbContext = new cPOSEntities())
            //{
            using (var transaction = dbContext.Database.BeginTransaction())
            {
                try
                {
                    Status status = dbContext.Status.SingleOrDefault(row => row.RowKey == model.RowKey);
                    dbContext.Status.Remove(status);
                    dbContext.SaveChanges();
                    transaction.Commit();
                    model.Message = EduSuiteUIResources.Success;
                    model.IsSuccessful = true;

                    ActivityLog.CreateActivityLog(MenuConstants.Status, ActionConstants.Delete, DbConstants.LogType.Info, model.RowKey, model.Message);

                }
                catch (DbUpdateException ex)
                {
                    transaction.Rollback();
                    if (ex.GetBaseException().Message.ToUpper().Contains(EduSuiteUIResources.ForeignKeyError.ToUpper()))
                    {
                        model.Message = String.Format(EduSuiteUIResources.CantDeleteErrorMessage, EduSuiteUIResources.Status);
                        model.IsSuccessful = false;
                        ActivityLog.CreateActivityLog(MenuConstants.Status, ActionConstants.Delete, DbConstants.LogType.Error, model.RowKey, ex.GetBaseException().Message);
                    }
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    model.Message = String.Format(EduSuiteUIResources.FailedToDeleteErrorMessage, EduSuiteUIResources.Status);
                    model.IsSuccessful = false;
                    ActivityLog.CreateActivityLog(MenuConstants.Status, ActionConstants.Delete, DbConstants.LogType.Error, model.RowKey, ex.GetBaseException().Message);
                }
            }
            //}
            return model;
        }
    }
}
