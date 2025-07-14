using System;
using System.Collections.Generic;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CITS.EduSuite.Business.Interfaces;
using CITS.EduSuite.Business.Models.Resources;
using CITS.EduSuite.Business.Models.ViewModels;
using CITS.EduSuite.Data;

namespace CITS.EduSuite.Business.Services
{
    public class CounsellingTimeService : ICounsellingTimeService
    {
        private EduSuiteDatabase dbContext;
        public CounsellingTimeService(EduSuiteDatabase objDB)
        {
            this.dbContext = objDB;
        }

        public List<CounsellingTimeViewModel> GetCounsellingTime(string searchText)
        {
            try
            {
                var CounsellingTimeList = (from p in dbContext.CounsellingTimes
                                           orderby p.RowKey descending
                                           where (p.Times.Contains(searchText))
                                           select new CounsellingTimeViewModel
                                           {
                                               RowKey = p.RowKey,
                                               Times = p.Times,
                                               IsActive = p.IsActive ?? false

                                           }).ToList();
                return CounsellingTimeList.GroupBy(x => x.RowKey).Select(y => y.First()).ToList<CounsellingTimeViewModel>();
            }
            catch (Exception ex)
            {
                ActivityLog.CreateActivityLog(MenuConstants.CounsellingTime, ActionConstants.View, DbConstants.LogType.Error, null, ex.GetBaseException().Message);
                return new List<CounsellingTimeViewModel>();


            }
        }
        public CounsellingTimeViewModel GetCounsellingTimeById(int? id)
        {
            try
            {
                CounsellingTimeViewModel model = new CounsellingTimeViewModel();
                model = dbContext.CounsellingTimes.Select(row => new CounsellingTimeViewModel
                {
                    RowKey = row.RowKey,
                    Times = row.Times,
                    IsActive = row.IsActive ?? false
                }).Where(x => x.RowKey == id).FirstOrDefault();
                if (model == null)
                {
                    model = new CounsellingTimeViewModel();
                }
                return model;
            }
            catch (Exception ex)
            {
                ActivityLog.CreateActivityLog(MenuConstants.CounsellingTime, ActionConstants.View, DbConstants.LogType.Error, id, ex.GetBaseException().Message);
                return new CounsellingTimeViewModel();

            }
        }
        public CounsellingTimeViewModel CreateCounsellingTime(CounsellingTimeViewModel model)
        {
            var CounsellingTimeCheck = dbContext.CounsellingTimes.Where(row => row.Times.ToLower() == model.Times.ToLower()).Count();
            CounsellingTime CounsellingTimeModel = new CounsellingTime();
            if (CounsellingTimeCheck != 0)
            {
                model.Message = String.Format(EduSuiteUIResources.ExistsErrorMessage, EduSuiteUIResources.CounsellingTime);
                model.IsSuccessful = false;
                return model;
            }

            using (var transaction = dbContext.Database.BeginTransaction())
            {
                try
                {
                    int MaxKey = dbContext.CounsellingTimes.Select(p => p.RowKey).DefaultIfEmpty().Max();

                    CounsellingTimeModel.RowKey = Convert.ToInt16(MaxKey + 1);
                    CounsellingTimeModel.Times = model.Times;
                    CounsellingTimeModel.IsActive = model.IsActive;
                    dbContext.CounsellingTimes.Add(CounsellingTimeModel);
                    dbContext.SaveChanges();
                    transaction.Commit();
                    model.Message = EduSuiteUIResources.Success;
                    model.IsSuccessful = true;

                    ActivityLog.CreateActivityLog(MenuConstants.CounsellingTime, ActionConstants.Add, DbConstants.LogType.Info, model.RowKey, model.Message);
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    model.Message = String.Format(EduSuiteUIResources.FailedToSaveErrorMessage, EduSuiteUIResources.CounsellingTime);
                    model.IsSuccessful = false;
                    ActivityLog.CreateActivityLog(MenuConstants.CounsellingTime, ActionConstants.Add, DbConstants.LogType.Error, model.RowKey, ex.GetBaseException().Message);
                }
            }
            return model;
        }
        public CounsellingTimeViewModel UpdateCounsellingTime(CounsellingTimeViewModel model)
        {
            var CounsellingTimeCheck = dbContext.CounsellingTimes.Where(row => row.Times.ToLower() == model.Times.ToLower()
               && row.RowKey != model.RowKey).ToList();

            CounsellingTime CounsellingTimeModel = new CounsellingTime();
            if (CounsellingTimeCheck.Count != 0)
            {
                model.Message = String.Format(EduSuiteUIResources.ExistsErrorMessage, EduSuiteUIResources.CounsellingTime);
                model.IsSuccessful = false;
                return model;
            }

            using (var transaction = dbContext.Database.BeginTransaction())
            {
                try
                {
                    CounsellingTimeModel = dbContext.CounsellingTimes.SingleOrDefault(x => x.RowKey == model.RowKey);
                    CounsellingTimeModel.Times = model.Times;
                    CounsellingTimeModel.IsActive = model.IsActive;
                    dbContext.SaveChanges();
                    transaction.Commit();
                    model.Message = EduSuiteUIResources.Success;
                    model.IsSuccessful = true;

                    ActivityLog.CreateActivityLog(MenuConstants.CounsellingTime, ActionConstants.Edit, DbConstants.LogType.Info, model.RowKey, model.Message);
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    model.Message = String.Format(EduSuiteUIResources.FailedToSaveErrorMessage, EduSuiteUIResources.CounsellingTime);
                    model.IsSuccessful = false;
                    ActivityLog.CreateActivityLog(MenuConstants.CounsellingTime, ActionConstants.Edit, DbConstants.LogType.Error, model.RowKey, ex.GetBaseException().Message);

                }
            }
            return model;

        }
        public CounsellingTimeViewModel DeleteCounsellingTime(CounsellingTimeViewModel model)
        {
            using (var transaction = dbContext.Database.BeginTransaction())
            {
                try
                {
                    CounsellingTime CounsellingTimeModel = dbContext.CounsellingTimes.SingleOrDefault(row => row.RowKey == model.RowKey);
                    dbContext.CounsellingTimes.Remove(CounsellingTimeModel);
                    dbContext.SaveChanges();
                    transaction.Commit();
                    model.Message = EduSuiteUIResources.Success;
                    model.IsSuccessful = true;
                    ActivityLog.CreateActivityLog(MenuConstants.CounsellingTime, ActionConstants.Delete, DbConstants.LogType.Info, model.RowKey, model.Message);
                }
                catch (DbUpdateException ex)
                {
                    transaction.Rollback();
                    if (ex.GetBaseException().Message.ToUpper().Contains(EduSuiteUIResources.ForeignKeyError.ToUpper()))
                    {
                        model.Message = String.Format(EduSuiteUIResources.CantDeleteErrorMessage, EduSuiteUIResources.CounsellingTime);
                        model.IsSuccessful = false;
                        ActivityLog.CreateActivityLog(MenuConstants.CounsellingTime, ActionConstants.Delete, DbConstants.LogType.Debug, model.RowKey, ex.GetBaseException().Message);
                    }
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    model.Message = String.Format(EduSuiteUIResources.FailedToDeleteErrorMessage, EduSuiteUIResources.CounsellingTime);
                    model.IsSuccessful = false;
                    ActivityLog.CreateActivityLog(MenuConstants.CounsellingTime, ActionConstants.Delete, DbConstants.LogType.Debug, model.RowKey, ex.GetBaseException().Message);
                }
            }

            return model;
        }
    }
}
