using CITS.EduSuite.Business.Common;
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
    public class ReligionService : IReligionService
    {
        private EduSuiteDatabase dbContext;
        public ReligionService(EduSuiteDatabase objDB)
        {
            this.dbContext = objDB;
        }
        public ReligionViewModel GetReligionById(int? id)
        {
            try
            {
                ReligionViewModel model = new ReligionViewModel();
                model = dbContext.Religions.Select(row => new ReligionViewModel
                {
                    RowKey = row.RowKey,
                    ReligionName = row.ReligionName,

                    IsActive = row.IsActive
                }).Where(x => x.RowKey == id).FirstOrDefault();
                if (model == null)
                {
                    model = new ReligionViewModel();
                }
                return model;
            }
            catch (Exception ex)
            {
                ActivityLog.CreateActivityLog(MenuConstants.Religion, ActionConstants.View, DbConstants.LogType.Error, id, ex.GetBaseException().Message);
                return new ReligionViewModel();

            }
        }
        public ReligionViewModel CreateReligion(ReligionViewModel model)
        {
            Religion ReligionModel = new Religion();
            using (var transaction = dbContext.Database.BeginTransaction())
            {
                try
                {
                    long MaxKey = dbContext.Religions.Select(p => p.RowKey).DefaultIfEmpty().Max();
                    long DisplayOrderMax = dbContext.Religions.Select(p => p.DisplayOrder).DefaultIfEmpty().Max();

                    ReligionModel.RowKey = Convert.ToInt16(MaxKey + 1);
                    ReligionModel.ReligionName = model.ReligionName;
                    ReligionModel.DisplayOrder = Convert.ToByte(DisplayOrderMax + 1);
                    ReligionModel.IsActive = model.IsActive;
                    ReligionModel.DateAdded = DateTimeUTC.Now;
                    dbContext.Religions.Add(ReligionModel);
                    dbContext.SaveChanges();
                    transaction.Commit();
                    model.Message = EduSuiteUIResources.Success;
                    model.IsSuccessful = true;
                    ActivityLog.CreateActivityLog(MenuConstants.Religion, ActionConstants.Add, DbConstants.LogType.Info, ReligionModel.RowKey, model.Message);
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    model.Message = String.Format(EduSuiteUIResources.FailedToSaveErrorMessage, EduSuiteUIResources.Religion);
                    model.IsSuccessful = false;
                    ActivityLog.CreateActivityLog(MenuConstants.Religion, ActionConstants.Add, DbConstants.LogType.Error, model.RowKey, ex.GetBaseException().Message);
                }
            }
            return model;
        }
        public ReligionViewModel UpdateReligion(ReligionViewModel model)
        {
            Religion ReligionModel = new Religion();
            using (var transaction = dbContext.Database.BeginTransaction())
            {
                try
                {
                    ReligionModel = dbContext.Religions.SingleOrDefault(x => x.RowKey == model.RowKey);
                    ReligionModel.ReligionName = model.ReligionName;
                    ReligionModel.IsActive = model.IsActive;
                    dbContext.SaveChanges();
                    transaction.Commit();
                    model.Message = EduSuiteUIResources.Success;
                    model.IsSuccessful = true;
                    ActivityLog.CreateActivityLog(MenuConstants.Religion, ActionConstants.Edit, DbConstants.LogType.Info, model.RowKey, model.Message);
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    model.Message = String.Format(EduSuiteUIResources.FailedToSaveErrorMessage, EduSuiteUIResources.Religion);
                    model.IsSuccessful = false;
                    ActivityLog.CreateActivityLog(MenuConstants.Religion, ActionConstants.Edit, DbConstants.LogType.Error, model.RowKey, ex.GetBaseException().Message);
                }
            }
            return model;

        }
        public ReligionViewModel DeleteReligion(ReligionViewModel model)
        {
            using (var transaction = dbContext.Database.BeginTransaction())
            {
                try
                {
                    Religion ReligionModel = dbContext.Religions.SingleOrDefault(row => row.RowKey == model.RowKey);
                    dbContext.Religions.Remove(ReligionModel);
                    dbContext.SaveChanges();
                    transaction.Commit();
                    model.Message = EduSuiteUIResources.Success;
                    model.IsSuccessful = true;
                    ActivityLog.CreateActivityLog(MenuConstants.Religion, ActionConstants.Delete, DbConstants.LogType.Info, model.RowKey, model.Message);


                }
                catch (DbUpdateException ex)
                {
                    transaction.Rollback();
                    if (ex.GetBaseException().Message.ToUpper().Contains(EduSuiteUIResources.ForeignKeyError.ToUpper()))
                    {
                        model.Message = String.Format(EduSuiteUIResources.CantDeleteErrorMessage, EduSuiteUIResources.Religion);
                        model.IsSuccessful = false;

                        ActivityLog.CreateActivityLog(MenuConstants.Religion, ActionConstants.Delete, DbConstants.LogType.Error, model.RowKey, ex.GetBaseException().Message);
                    }
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    model.Message = String.Format(EduSuiteUIResources.FailedToDeleteErrorMessage, EduSuiteUIResources.Religion);
                    model.IsSuccessful = false;

                    ActivityLog.CreateActivityLog(MenuConstants.Religion, ActionConstants.Delete, DbConstants.LogType.Error, model.RowKey, ex.GetBaseException().Message);
                }
            }

            return model;
        }
        public List<ReligionViewModel> GetReligion(string searchText)
        {
            try
            {
                var ReligionList = (from p in dbContext.Religions
                                    orderby p.RowKey descending
                                    where (p.ReligionName.Contains(searchText))
                                    select new ReligionViewModel
                                    {
                                        RowKey = p.RowKey,
                                        ReligionName = p.ReligionName,

                                        // IsActiveText = p.IsActive == true ? ApplicationResources.Yes : ApplicationResources.No
                                        IsActive = p.IsActive,
                                    }).ToList();
                return ReligionList.GroupBy(x => x.RowKey).Select(y => y.First()).ToList<ReligionViewModel>();
            }
            catch (Exception ex)
            {
                ActivityLog.CreateActivityLog(MenuConstants.Religion, ActionConstants.View, DbConstants.LogType.Error, null, ex.GetBaseException().Message);
                return new List<ReligionViewModel>();

            }
        }

        public ReligionViewModel CheckReligionNameExists(ReligionViewModel model)
        {
            if (dbContext.Religions.Where(x => x.ReligionName.ToLower() == model.ReligionName.ToLower() && x.RowKey != model.RowKey).Any())
            {
                model.IsSuccessful = false;
                model.Message = String.Format(EduSuiteUIResources.ExistsErrorMessage, EduSuiteUIResources.Religion);
            }
            else
            {
                model.IsSuccessful = true;
                model.Message = "";
            }
            return model;
        }

    }
}
