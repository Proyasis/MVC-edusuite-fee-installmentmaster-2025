using CITS.EduSuite.Business.Common;
using CITS.EduSuite.Business.Interfaces;
using CITS.EduSuite.Business.Models.ViewModels;
using CITS.EduSuite.Data;
using System;
using System.Collections.Generic;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using CITS.EduSuite.Business.Models.Resources;

namespace CITS.EduSuite.Business.Services
{
   public class MarkGroupService:IMarkGroupService
    {

        private EduSuiteDatabase dbContext;
        public MarkGroupService(EduSuiteDatabase objDB)
        {
            this.dbContext = objDB;
        }
        public MarkGroupViewModel GetMarkGroupById(int? id)
        {
            try
            {
                MarkGroupViewModel model = new MarkGroupViewModel();
                model = dbContext.MarkGroups.Select(row => new MarkGroupViewModel
                {
                    RowKey = row.RowKey,
                    MarkGroupName = row.MarkGroupName,
                    IsActive = row.IsActive,
                    NegativeMark=row.NegativeMark,
                    Mark=row.Mark??0
                   
                }).Where(x => x.RowKey == id).FirstOrDefault();

                if (model == null)
                {
                    model = new MarkGroupViewModel();
                    model.IsActive = true;
                }
                return model;
            }
            catch (Exception ex)
            {
                ActivityLog.CreateActivityLog(MenuConstants.MarkGroup, ActionConstants.View, DbConstants.LogType.Error, id, ex.GetBaseException().Message);
                return new MarkGroupViewModel();

            }
        }
        public MarkGroupViewModel CreateMarkGroup(MarkGroupViewModel model)
        {
            var MarkGroupCheck = dbContext.MarkGroups.Where(row => row.MarkGroupName.ToLower() == model.MarkGroupName.ToLower()).Count();
            MarkGroup MarkGroupModel = new MarkGroup();
            if (MarkGroupCheck != 0)
            {
                model.Message = String.Format(EduSuiteUIResources.ExistsErrorMessage, EduSuiteUIResources.Batch);
                model.IsSuccessful = false;
                return model;
            }

            using (var transaction = dbContext.Database.BeginTransaction())
            {
                try
                {
                    long MaxKey = dbContext.MarkGroups.Select(p => p.RowKey).DefaultIfEmpty().Max();

                    MarkGroupModel.RowKey = Convert.ToInt16(MaxKey + 1);
                    MarkGroupModel.MarkGroupName = model.MarkGroupName;
                    MarkGroupModel.IsActive = model.IsActive;
                    MarkGroupModel.NegativeMark = model.NegativeMark;
                    MarkGroupModel.Mark = model.Mark;
                    dbContext.MarkGroups.Add(MarkGroupModel);            
                    dbContext.SaveChanges();
                    transaction.Commit();
                    model.Message = EduSuiteUIResources.Success;
                    model.IsSuccessful = true;

                    ActivityLog.CreateActivityLog(MenuConstants.MarkGroup, ActionConstants.Add, DbConstants.LogType.Info, model.RowKey, model.Message);
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    model.Message = String.Format(EduSuiteUIResources.CantDeleteErrorMessage, EduSuiteUIResources.MarkGroup);
                    model.IsSuccessful = false;
                    ActivityLog.CreateActivityLog(MenuConstants.MarkGroup, ActionConstants.Add, DbConstants.LogType.Error, model.RowKey, ex.GetBaseException().Message);
                }
            }
            return model;
        }
        public MarkGroupViewModel UpdateMarkGroup(MarkGroupViewModel model)
        {
            var SubjectsCheck = dbContext.MarkGroups.Where(row => row.MarkGroupName.ToLower() == model.MarkGroupName.ToLower()
               && row.RowKey != model.RowKey).ToList();

            MarkGroup MarkGroupModel = new MarkGroup();
            if (SubjectsCheck.Count != 0)
            {
                model.Message = String.Format(EduSuiteUIResources.ExistsErrorMessage, EduSuiteUIResources.MarkGroup);
                model.IsSuccessful = false;
                return model;
            }

            using (var transaction = dbContext.Database.BeginTransaction())
            {
                try
                {
                    MarkGroupModel = dbContext.MarkGroups.SingleOrDefault(x => x.RowKey == model.RowKey);
                    MarkGroupModel.MarkGroupName = model.MarkGroupName;
                    MarkGroupModel.IsActive = model.IsActive;
                    MarkGroupModel.NegativeMark = model.NegativeMark;
                    MarkGroupModel.Mark = model.Mark;
                    dbContext.SaveChanges();
                    transaction.Commit();
                    model.Message = EduSuiteUIResources.Success;
                    model.IsSuccessful = true;

                    ActivityLog.CreateActivityLog(MenuConstants.MarkGroup, ActionConstants.Edit, DbConstants.LogType.Info, model.RowKey, model.Message);
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    model.Message = String.Format(EduSuiteUIResources.FailedToSaveErrorMessage, EduSuiteUIResources.MarkGroup);
                    model.IsSuccessful = false;
                    ActivityLog.CreateActivityLog(MenuConstants.MarkGroup, ActionConstants.Edit, DbConstants.LogType.Error, model.RowKey, ex.GetBaseException().Message);

                }
            }
            return model;

        }

        public MarkGroupViewModel DeleteMarkGroup(MarkGroupViewModel model)
        {
            using (var transaction = dbContext.Database.BeginTransaction())
            {
                try
                {
                    MarkGroup MarkGroupModel = dbContext.MarkGroups.SingleOrDefault(row => row.RowKey == model.RowKey);
                    dbContext.MarkGroups.Remove(MarkGroupModel);
                    dbContext.SaveChanges();
                    transaction.Commit();
                    model.Message = EduSuiteUIResources.Success;
                    model.IsSuccessful = true;
                    ActivityLog.CreateActivityLog(MenuConstants.MarkGroup, ActionConstants.Delete, DbConstants.LogType.Info, model.RowKey, model.Message);
                }
                catch (DbUpdateException ex)
                {
                    transaction.Rollback();
                    if (ex.GetBaseException().Message.ToUpper().Contains(EduSuiteUIResources.ForeignKeyError.ToUpper()))
                    {
                        model.Message = String.Format(EduSuiteUIResources.CantDeleteErrorMessage, EduSuiteUIResources.MarkGroup);
                        model.IsSuccessful = false;
                        ActivityLog.CreateActivityLog(MenuConstants.MarkGroup, ActionConstants.Delete, DbConstants.LogType.Debug, model.RowKey, ex.GetBaseException().Message);
                    }
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    model.Message = String.Format(EduSuiteUIResources.FailedToDeleteErrorMessage, EduSuiteUIResources.MarkGroup);
                    model.IsSuccessful = false;
                    ActivityLog.CreateActivityLog(MenuConstants.MarkGroup, ActionConstants.Delete, DbConstants.LogType.Debug, model.RowKey, ex.GetBaseException().Message);
                }
            }

            return model;
        }

        public List<MarkGroupViewModel> GetMarkGroup(string searchText)
        {
            try
            {
                var MarkGroupList = (from p in dbContext.MarkGroups
                                    orderby p.RowKey descending
                                    where (p.MarkGroupName.Contains(searchText))
                                    select new MarkGroupViewModel
                                    {
                                        RowKey = p.RowKey,
                                        MarkGroupName = p.MarkGroupName,
                                        IsActive = p.IsActive,
                                        Mark = p.Mark??0,
                                        NegativeMark = p.NegativeMark
                                    }).ToList();
                return MarkGroupList.GroupBy(x => x.RowKey).Select(y => y.First()).ToList<MarkGroupViewModel>();
            }
            catch (Exception ex)
            {
                ActivityLog.CreateActivityLog(MenuConstants.MarkGroup, ActionConstants.View, DbConstants.LogType.Error, null, ex.GetBaseException().Message);
                return new List<MarkGroupViewModel>();


            }
        }

    }
}
