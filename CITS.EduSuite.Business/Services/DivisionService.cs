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
    public class DivisionService : IDivisionService
    {
        private EduSuiteDatabase dbContext;
        public DivisionService(EduSuiteDatabase objDB)
        {
            this.dbContext = objDB;
        }

        public List<DivisionViewModel> GetDivision(string searchText)
        {
            try
            {
                var DivisionList = (from p in dbContext.Divisions
                                    orderby p.RowKey descending
                                    where (p.DivisionName.Contains(searchText))
                                    select new DivisionViewModel
                                    {
                                        RowKey = p.RowKey,
                                        DivisionName = p.DivisionName,
                                        //IsActiveText = p.IsActive == true ? ApplicationResources.Yes : ApplicationResources.No
                                        IsActive = p.IsActive,
                                    }).ToList();
                return DivisionList.GroupBy(x => x.RowKey).Select(y => y.First()).ToList<DivisionViewModel>();
            }
            catch (Exception ex)
            {
                ActivityLog.CreateActivityLog(MenuConstants.Division, ActionConstants.View, DbConstants.LogType.Error, null, ex.GetBaseException().Message);
                return new List<DivisionViewModel>();

            }
        }
        public DivisionViewModel GetDivisionById(int? id)
        {
            try
            {
                DivisionViewModel model = new DivisionViewModel();
                model = dbContext.Divisions.Select(row => new DivisionViewModel
                {
                    RowKey = row.RowKey,
                    DivisionName = row.DivisionName,
                    IsActive = row.IsActive
                }).Where(x => x.RowKey == id).FirstOrDefault();
                if (model == null)
                {
                    model = new DivisionViewModel();
                }
                return model;
            }
            catch (Exception ex)
            {
                ActivityLog.CreateActivityLog(MenuConstants.Division, ActionConstants.View, DbConstants.LogType.Error, id, ex.GetBaseException().Message);
                return new DivisionViewModel();
               
            }
        }
        public DivisionViewModel CreateDivision(DivisionViewModel model)
        {
            var DivisionCheck = dbContext.Divisions.Where(row => row.DivisionName.ToLower() == model.DivisionName.ToLower()).Count();
            Division DivisionModel = new Division();
            if (DivisionCheck != 0)
            {
                model.Message = String.Format(EduSuiteUIResources.ExistsErrorMessage, EduSuiteUIResources.Division);
                model.IsSuccessful = false;
                return model;
            }

            using (var transaction = dbContext.Database.BeginTransaction())
            {
                try
                {
                    int MaxKey = dbContext.Divisions.Select(p => p.RowKey).DefaultIfEmpty().Max();

                    DivisionModel.RowKey = Convert.ToInt16(MaxKey + 1);
                    DivisionModel.DivisionName = model.DivisionName;
                    DivisionModel.IsActive = model.IsActive;
                    dbContext.Divisions.Add(DivisionModel);
                    dbContext.SaveChanges();
                    transaction.Commit();
                    model.Message = EduSuiteUIResources.Success;
                    model.IsSuccessful = true;

                    ActivityLog.CreateActivityLog(MenuConstants.Division, ActionConstants.Add, DbConstants.LogType.Info, DivisionModel.RowKey, model.Message);
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    model.Message = String.Format(EduSuiteUIResources.FailedToSaveErrorMessage, EduSuiteUIResources.Division);
                    model.IsSuccessful = false;
                    ActivityLog.CreateActivityLog(MenuConstants.Division, ActionConstants.Add, DbConstants.LogType.Error, DivisionModel.RowKey, ex.GetBaseException().Message);
                }
            }
            return model;
        }
        public DivisionViewModel UpdateDivision(DivisionViewModel model)
        {
            var DivisionCheck = dbContext.Divisions.Where(row => row.DivisionName.ToLower() == model.DivisionName.ToLower()
               && row.RowKey != model.RowKey).ToList();

            Division DivisionModel = new Division();
            if (DivisionCheck.Count != 0)
            {
                model.Message = String.Format(EduSuiteUIResources.ExistsErrorMessage, EduSuiteUIResources.Division);
                model.IsSuccessful = false;
                return model;
            }

            using (var transaction = dbContext.Database.BeginTransaction())
            {
                try
                {
                    DivisionModel = dbContext.Divisions.SingleOrDefault(x => x.RowKey == model.RowKey);
                    DivisionModel.DivisionName = model.DivisionName;
                    DivisionModel.IsActive = model.IsActive;
                    dbContext.SaveChanges();
                    transaction.Commit();
                    model.Message = EduSuiteUIResources.Success;
                    model.IsSuccessful = true;

                    ActivityLog.CreateActivityLog(MenuConstants.Division, ActionConstants.Edit, DbConstants.LogType.Info, model.RowKey, model.Message);
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    model.Message = String.Format(EduSuiteUIResources.FailedToSaveErrorMessage, EduSuiteUIResources.Division);
                    model.IsSuccessful = false;
                    ActivityLog.CreateActivityLog(MenuConstants.Division, ActionConstants.Edit, DbConstants.LogType.Error, model.RowKey, ex.GetBaseException().Message);
                }
            }
            return model;

        }
        public DivisionViewModel DeleteDivision(DivisionViewModel model)
        {
            using (var transaction = dbContext.Database.BeginTransaction())
            {
                try
                {
                    Division DivisionModel = dbContext.Divisions.SingleOrDefault(row => row.RowKey == model.RowKey);
                    dbContext.Divisions.Remove(DivisionModel);
                    dbContext.SaveChanges();
                    transaction.Commit();
                    model.Message = EduSuiteUIResources.Success;
                    model.IsSuccessful = true;
                    ActivityLog.CreateActivityLog(MenuConstants.Division, ActionConstants.Delete, DbConstants.LogType.Info, model.RowKey, model.Message);

                }
                catch (DbUpdateException ex)
                {
                    transaction.Rollback();
                    if (ex.GetBaseException().Message.ToUpper().Contains(EduSuiteUIResources.ForeignKeyError.ToUpper()))
                    {
                        model.Message = String.Format(EduSuiteUIResources.CantDeleteErrorMessage, EduSuiteUIResources.Division);
                        model.IsSuccessful = false;

                        ActivityLog.CreateActivityLog(MenuConstants.Division, ActionConstants.Delete, DbConstants.LogType.Debug, model.RowKey, ex.GetBaseException().Message);
                    }
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    model.Message = String.Format(EduSuiteUIResources.FailedToDeleteErrorMessage, EduSuiteUIResources.Division);
                    model.IsSuccessful = false;

                    ActivityLog.CreateActivityLog(MenuConstants.Division, ActionConstants.Delete, DbConstants.LogType.Debug, model.RowKey, ex.GetBaseException().Message);
                }
            }

            return model;
        }
      
    }
}
