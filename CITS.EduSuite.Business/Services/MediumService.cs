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
    public class MediumService : IMediumService
    {
        private EduSuiteDatabase dbContext;
        public MediumService(EduSuiteDatabase objDB)
        {
            this.dbContext = objDB;
        }
        public MediumViewModel GetMediumById(int? id)
        {
            try
            {
                MediumViewModel model = new MediumViewModel();
                model = dbContext.Media.Select(row => new MediumViewModel
                {
                    RowKey = row.RowKey,
                    MediumName = row.MediumName,
                    IsActive = row.IsActive
                }).Where(x => x.RowKey == id).FirstOrDefault();
                if (model == null)
                {
                    model = new MediumViewModel();
                }
                return model;
            }
            catch (Exception ex)
            {
                ActivityLog.CreateActivityLog(MenuConstants.Medium, ActionConstants.View, DbConstants.LogType.Error, id, ex.GetBaseException().Message);
                return new MediumViewModel();
                

            }
        }
        public MediumViewModel CreateMedium(MediumViewModel model)
        {
            var MediumCheck = dbContext.Media.Where(row => row.MediumName.ToLower() == model.MediumName.ToLower()).Count();
            Medium MediumModel = new Medium();
            if (MediumCheck != 0)
            {
                model.Message = String.Format(EduSuiteUIResources.ExistsErrorMessage, EduSuiteUIResources.Medium+ EduSuiteUIResources.BlankSpace+ EduSuiteUIResources.Name);
                model.IsSuccessful = false;
                return model;
            }

            using (var transaction = dbContext.Database.BeginTransaction())
            {
                try
                {
                    int MaxKey = dbContext.Media.Select(p => p.RowKey).DefaultIfEmpty().Max();

                    MediumModel.RowKey = Convert.ToInt16(MaxKey + 1);
                    MediumModel.MediumName = model.MediumName;
                    MediumModel.IsActive = model.IsActive;
                    dbContext.Media.Add(MediumModel);
                    dbContext.SaveChanges();
                    transaction.Commit();
                    model.Message = EduSuiteUIResources.Success;
                    model.IsSuccessful = true;
                    ActivityLog.CreateActivityLog(MenuConstants.Medium, ActionConstants.Add, DbConstants.LogType.Info, model.RowKey, model.Message);


                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    model.Message = String.Format(EduSuiteUIResources.FailedToSaveErrorMessage, EduSuiteUIResources.Medium);
                    model.IsSuccessful = false;

                    ActivityLog.CreateActivityLog(MenuConstants.Medium, ActionConstants.Add, DbConstants.LogType.Error, model.RowKey, ex.GetBaseException().Message);
                }
            }
            return model;
        }
        public MediumViewModel UpdateMedium(MediumViewModel model)
        {
            var MediumCheck = dbContext.Media.Where(row => row.MediumName.ToLower() == model.MediumName.ToLower()
               && row.RowKey != model.RowKey).ToList();

            Medium MediumModel = new Medium();
            if (MediumCheck.Count != 0)
            {
                model.Message = String.Format(EduSuiteUIResources.ExistsErrorMessage, EduSuiteUIResources.Medium + EduSuiteUIResources.BlankSpace + EduSuiteUIResources.Name);
                model.IsSuccessful = false;
                return model;
            }

            using (var transaction = dbContext.Database.BeginTransaction())
            {
                try
                {
                    MediumModel = dbContext.Media.SingleOrDefault(x => x.RowKey == model.RowKey);
                    MediumModel.MediumName = model.MediumName;
                    MediumModel.IsActive = model.IsActive;
                    dbContext.SaveChanges();
                    transaction.Commit();
                    model.Message = EduSuiteUIResources.Success;
                    model.IsSuccessful = true;
                    ActivityLog.CreateActivityLog(MenuConstants.Medium, ActionConstants.Edit, DbConstants.LogType.Info, model.RowKey, model.Message);

                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    model.Message = String.Format(EduSuiteUIResources.FailedToSaveErrorMessage, EduSuiteUIResources.Medium);
                    model.IsSuccessful = false;
                    ActivityLog.CreateActivityLog(MenuConstants.Medium, ActionConstants.Edit, DbConstants.LogType.Error, model.RowKey, ex.GetBaseException().Message);
                }
            }
            return model;

        }
        public MediumViewModel DeleteMedium(MediumViewModel model)
        {
            using (var transaction = dbContext.Database.BeginTransaction())
            {
                try
                {
                    Medium MediumModel = dbContext.Media.SingleOrDefault(row => row.RowKey == model.RowKey);
                    dbContext.Media.Remove(MediumModel);
                    dbContext.SaveChanges();
                    transaction.Commit();
                    model.Message = EduSuiteUIResources.Success;
                    model.IsSuccessful = true;
                }
                catch (DbUpdateException ex)
                {
                    transaction.Rollback();
                    if (ex.GetBaseException().Message.ToUpper().Contains(EduSuiteUIResources.ForeignKeyError.ToUpper()))
                    {
                        model.Message = String.Format(EduSuiteUIResources.CantDeleteErrorMessage, EduSuiteUIResources.Medium);
                        model.IsSuccessful = false;
                        ActivityLog.CreateActivityLog(MenuConstants.Medium, ActionConstants.Delete, DbConstants.LogType.Info, model.RowKey, model.Message);

                    }
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    model.Message = String.Format(EduSuiteUIResources.FailedToDeleteErrorMessage, EduSuiteUIResources.Medium);
                    model.IsSuccessful = false;
                    ActivityLog.CreateActivityLog(MenuConstants.Medium, ActionConstants.Delete, DbConstants.LogType.Error, model.RowKey, ex.GetBaseException().Message);
                }
            }

            return model;
        }
        public List<MediumViewModel> GetMedium(string searchText)
        {
            try
            {
                var MediumList = (from p in dbContext.Media
                                  orderby p.RowKey
                                  where (p.MediumName.Contains(searchText))
                                  select new MediumViewModel
                           {
                               RowKey = p.RowKey,
                               MediumName = p.MediumName,
                               //IsActiveText = p.IsActive == true ? ApplicationResources.Yes : ApplicationResources.No
                               IsActive=p.IsActive,
                           }).ToList();
                return MediumList.GroupBy(x => x.RowKey).Select(y => y.First()).ToList<MediumViewModel>();
            }
            catch (Exception ex)
            {
                ActivityLog.CreateActivityLog(MenuConstants.Medium, ActionConstants.View, DbConstants.LogType.Error, null, ex.GetBaseException().Message);
                return new List<MediumViewModel>();
                

            }
        }
    }
}
