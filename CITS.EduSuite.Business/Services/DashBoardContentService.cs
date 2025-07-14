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
    public class DashBoardContentService : IDashBoardContentService
    {
        private EduSuiteDatabase dbContext;
        public DashBoardContentService(EduSuiteDatabase objDB)
        {
            this.dbContext = objDB;
        }

        public List<DashBoardContentViewModel> GetDashBoardContent(string searchText)
        {
            try
            {
                var DashBoardContentList = (from p in dbContext.DashBoardContents
                                            orderby p.RowKey descending
                                            where (p.DashBoardContentName.Contains(searchText))
                                            select new DashBoardContentViewModel
                                            {
                                                RowKey = p.RowKey,
                                                DashBoardContentName = p.DashBoardContentName,
                                                DashBoardTypeKey = p.DashBoardTypeKey,
                                                DisplayOrder = p.DisplayOrder ?? 0,
                                                IsActive = p.IsActive,
                                            }).ToList();
                return DashBoardContentList.GroupBy(x => x.RowKey).Select(y => y.First()).ToList<DashBoardContentViewModel>();
            }
            catch (Exception ex)
            {
                ActivityLog.CreateActivityLog(MenuConstants.DashBoardContent, ActionConstants.View, DbConstants.LogType.Error, null, ex.GetBaseException().Message);
                return new List<DashBoardContentViewModel>();
            }
        }
        public DashBoardContentViewModel GetDashBoardContentById(int? id)
        {
            try
            {
                DashBoardContentViewModel model = new DashBoardContentViewModel();
                model = dbContext.DashBoardContents.Select(row => new DashBoardContentViewModel
                {
                    RowKey = row.RowKey,
                    DashBoardContentName = row.DashBoardContentName,
                    DashBoardTypeKey = row.DashBoardTypeKey,
                    DisplayOrder = row.DisplayOrder??0,
                    IsActive = row.IsActive
                }).Where(x => x.RowKey == id).FirstOrDefault();
                if (model == null)
                {
                    model = new DashBoardContentViewModel();
                }
                return model;
            }
            catch (Exception ex)
            {
                ActivityLog.CreateActivityLog(MenuConstants.DashBoardContent, ActionConstants.View, DbConstants.LogType.Error, id, ex.GetBaseException().Message);
                return new DashBoardContentViewModel();
            }
        }
        public DashBoardContentViewModel CreateDashBoardContent(DashBoardContentViewModel model)
        {
            var DashBoardContentCheck = dbContext.DashBoardContents.Where(row => row.DashBoardContentName.ToLower() == model.DashBoardContentName.ToLower()).Count();
            DashBoardContent DashBoardContentModel = new DashBoardContent();
            if (DashBoardContentCheck != 0)
            {
                model.Message = String.Format(EduSuiteUIResources.ExistsErrorMessage, EduSuiteUIResources.DashBoardContent);
                model.IsSuccessful = false;
                return model;
            }

            using (var transaction = dbContext.Database.BeginTransaction())
            {
                try
                {
                    int MaxKey = dbContext.DashBoardContents.Select(p => p.RowKey).DefaultIfEmpty().Max();

                    DashBoardContentModel.RowKey = Convert.ToInt16(MaxKey + 1);
                    DashBoardContentModel.DashBoardContentName = model.DashBoardContentName;
                    DashBoardContentModel.DashBoardTypeKey = model.DashBoardTypeKey;
                    DashBoardContentModel.DisplayOrder = DashBoardContentModel.RowKey;
                    DashBoardContentModel.IsActive = model.IsActive;
                    dbContext.DashBoardContents.Add(DashBoardContentModel);
                    dbContext.SaveChanges();
                    transaction.Commit();
                    model.Message = EduSuiteUIResources.Success;
                    model.IsSuccessful = true;

                    ActivityLog.CreateActivityLog(MenuConstants.DashBoardContent, ActionConstants.Add, DbConstants.LogType.Info, model.RowKey, model.Message);
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    model.Message = String.Format(EduSuiteUIResources.FailedToSaveErrorMessage, EduSuiteUIResources.DashBoardContent);
                    model.IsSuccessful = false;
                    ActivityLog.CreateActivityLog(MenuConstants.DashBoardContent, ActionConstants.Add, DbConstants.LogType.Error, model.RowKey, ex.GetBaseException().Message);
                }
            }
            return model;
        }
        public DashBoardContentViewModel UpdateDashBoardContent(DashBoardContentViewModel model)
        {
            var DashBoardContentCheck = dbContext.DashBoardContents.Where(row => row.DashBoardContentName.ToLower() == model.DashBoardContentName.ToLower()
               && row.RowKey != model.RowKey).ToList();

            DashBoardContent DashBoardContentModel = new DashBoardContent();
            if (DashBoardContentCheck.Count != 0)
            {
                model.Message = String.Format(EduSuiteUIResources.ExistsErrorMessage, EduSuiteUIResources.DashBoardContent);
                model.IsSuccessful = false;
                return model;
            }

            using (var transaction = dbContext.Database.BeginTransaction())
            {
                try
                {
                    DashBoardContentModel = dbContext.DashBoardContents.SingleOrDefault(x => x.RowKey == model.RowKey);
                    DashBoardContentModel.DashBoardContentName = model.DashBoardContentName;
                    DashBoardContentModel.DashBoardTypeKey = model.DashBoardTypeKey;
                    DashBoardContentModel.IsActive = model.IsActive;
                    dbContext.SaveChanges();
                    transaction.Commit();
                    model.Message = EduSuiteUIResources.Success;
                    model.IsSuccessful = true;

                    ActivityLog.CreateActivityLog(MenuConstants.DashBoardContent, ActionConstants.Edit, DbConstants.LogType.Info, model.RowKey, model.Message);
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    model.Message = String.Format(EduSuiteUIResources.FailedToSaveErrorMessage, EduSuiteUIResources.DashBoardContent);
                    model.IsSuccessful = false;
                    ActivityLog.CreateActivityLog(MenuConstants.DashBoardContent, ActionConstants.Edit, DbConstants.LogType.Error, model.RowKey, ex.GetBaseException().Message);
                }
            }
            return model;

        }
        public DashBoardContentViewModel DeleteDashBoardContent(DashBoardContentViewModel model)
        {
            using (var transaction = dbContext.Database.BeginTransaction())
            {
                try
                {
                    DashBoardContent DashBoardContentModel = dbContext.DashBoardContents.SingleOrDefault(row => row.RowKey == model.RowKey);
                    dbContext.DashBoardContents.Remove(DashBoardContentModel);
                    dbContext.SaveChanges();
                    transaction.Commit();
                    model.Message = EduSuiteUIResources.Success;
                    model.IsSuccessful = true;
                    ActivityLog.CreateActivityLog(MenuConstants.DashBoardContent, ActionConstants.Delete, DbConstants.LogType.Info, model.RowKey, model.Message);
                }
                catch (DbUpdateException ex)
                {
                    transaction.Rollback();
                    if (ex.GetBaseException().Message.ToUpper().Contains(EduSuiteUIResources.ForeignKeyError.ToUpper()))
                    {
                        model.Message = String.Format(EduSuiteUIResources.CantDeleteErrorMessage, EduSuiteUIResources.DashBoardContent);
                        model.IsSuccessful = false;
                        ActivityLog.CreateActivityLog(MenuConstants.DashBoardContent, ActionConstants.Delete, DbConstants.LogType.Debug, model.RowKey, ex.GetBaseException().Message);
                    }
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    model.Message = String.Format(EduSuiteUIResources.FailedToDeleteErrorMessage, EduSuiteUIResources.DashBoardContent);
                    model.IsSuccessful = false;
                    ActivityLog.CreateActivityLog(MenuConstants.DashBoardContent, ActionConstants.Delete, DbConstants.LogType.Debug, model.RowKey, ex.GetBaseException().Message);
                }
            }

            return model;
        }
    }
}
