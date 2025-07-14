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
    public class CasteService : ICasteService
    {
        private EduSuiteDatabase dbContext;
        public CasteService(EduSuiteDatabase objDB)
        {
            this.dbContext = objDB;
        }

        public List<CasteViewModel> GetCaste(string searchText)
        {
            try
            {
                var CasteList = (from p in dbContext.Castes
                                 orderby p.RowKey descending
                                 where (p.CasteName.Contains(searchText))
                                 select new CasteViewModel
                                 {
                                     RowKey = p.RowKey,
                                     CasteName = p.CasteName,
                                     ReligionKey = p.ReligionKey,
                                     ReligionName = p.Religion.ReligionName,
                                     DisplayOrder = p.DisplayOrder ?? 0,
                                     IsActive = p.IsActive,
                                 }).ToList();
                return CasteList.GroupBy(x => x.RowKey).Select(y => y.First()).ToList<CasteViewModel>();
            }
            catch (Exception ex)
            {
                ActivityLog.CreateActivityLog(MenuConstants.Caste, ActionConstants.View, DbConstants.LogType.Error, null, ex.GetBaseException().Message);
                return new List<CasteViewModel>();


            }
        }
        public CasteViewModel GetCasteById(int? id)
        {
            try
            {
                CasteViewModel model = new CasteViewModel();
                model = dbContext.Castes.Select(row => new CasteViewModel
                {
                    RowKey = row.RowKey,
                    CasteName = row.CasteName,
                    ReligionKey = row.ReligionKey,
                    DisplayOrder = row.DisplayOrder ?? 0,
                    IsActive = row.IsActive
                }).Where(x => x.RowKey == id).FirstOrDefault();
                if (model == null)
                {
                    model = new CasteViewModel();
                }
                FillReligion(model);
                return model;
            }
            catch (Exception ex)
            {
                ActivityLog.CreateActivityLog(MenuConstants.Caste, ActionConstants.View, DbConstants.LogType.Error, id, ex.GetBaseException().Message);
                return new CasteViewModel();
            }
        }
        public CasteViewModel CreateCaste(CasteViewModel model)
        {
            var CasteCheck = dbContext.Castes.Where(row => row.CasteName.ToLower() == model.CasteName.ToLower()).Count();
            Caste CasteModel = new Caste();
            if (CasteCheck != 0)
            {
                model.Message = String.Format(EduSuiteUIResources.ExistsErrorMessage, EduSuiteUIResources.Caste);
                model.IsSuccessful = false;
                return model;
            }

            using (var transaction = dbContext.Database.BeginTransaction())
            {
                try
                {
                    int MaxKey = dbContext.Castes.Select(p => p.RowKey).DefaultIfEmpty().Max();

                    CasteModel.RowKey = Convert.ToInt16(MaxKey + 1);
                    CasteModel.CasteName = model.CasteName;
                    CasteModel.ReligionKey = model.ReligionKey;
                    CasteModel.DisplayOrder = CasteModel.RowKey;
                    CasteModel.IsActive = model.IsActive;
                    dbContext.Castes.Add(CasteModel);
                    dbContext.SaveChanges();
                    transaction.Commit();
                    model.Message = EduSuiteUIResources.Success;
                    model.IsSuccessful = true;

                    ActivityLog.CreateActivityLog(MenuConstants.Caste, ActionConstants.Add, DbConstants.LogType.Info, model.RowKey, model.Message);
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    model.Message = String.Format(EduSuiteUIResources.FailedToSaveErrorMessage, EduSuiteUIResources.Caste);
                    model.IsSuccessful = false;
                    ActivityLog.CreateActivityLog(MenuConstants.Caste, ActionConstants.Add, DbConstants.LogType.Error, model.RowKey, ex.GetBaseException().Message);
                }
            }
            return model;
        }
        public CasteViewModel UpdateCaste(CasteViewModel model)
        {
            var CasteCheck = dbContext.Castes.Where(row => row.CasteName.ToLower() == model.CasteName.ToLower()
               && row.RowKey != model.RowKey).ToList();

            Caste CasteModel = new Caste();
            if (CasteCheck.Count != 0)
            {
                model.Message = String.Format(EduSuiteUIResources.ExistsErrorMessage, EduSuiteUIResources.Caste);
                model.IsSuccessful = false;
                return model;
            }

            using (var transaction = dbContext.Database.BeginTransaction())
            {
                try
                {
                    CasteModel = dbContext.Castes.SingleOrDefault(x => x.RowKey == model.RowKey);
                    CasteModel.CasteName = model.CasteName;
                    CasteModel.ReligionKey = model.ReligionKey;
                    CasteModel.IsActive = model.IsActive;
                    dbContext.SaveChanges();
                    transaction.Commit();
                    model.Message = EduSuiteUIResources.Success;
                    model.IsSuccessful = true;

                    ActivityLog.CreateActivityLog(MenuConstants.Caste, ActionConstants.Edit, DbConstants.LogType.Info, model.RowKey, model.Message);
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    model.Message = String.Format(EduSuiteUIResources.FailedToSaveErrorMessage, EduSuiteUIResources.Caste);
                    model.IsSuccessful = false;
                    ActivityLog.CreateActivityLog(MenuConstants.Caste, ActionConstants.Edit, DbConstants.LogType.Error, model.RowKey, ex.GetBaseException().Message);

                }
            }
            return model;

        }
        public CasteViewModel DeleteCaste(CasteViewModel model)
        {
            using (var transaction = dbContext.Database.BeginTransaction())
            {
                try
                {
                    Caste CasteModel = dbContext.Castes.SingleOrDefault(row => row.RowKey == model.RowKey);
                    dbContext.Castes.Remove(CasteModel);
                    dbContext.SaveChanges();
                    transaction.Commit();
                    model.Message = EduSuiteUIResources.Success;
                    model.IsSuccessful = true;
                    ActivityLog.CreateActivityLog(MenuConstants.Caste, ActionConstants.Delete, DbConstants.LogType.Info, model.RowKey, model.Message);
                }
                catch (DbUpdateException ex)
                {
                    transaction.Rollback();
                    if (ex.GetBaseException().Message.ToUpper().Contains(EduSuiteUIResources.ForeignKeyError.ToUpper()))
                    {
                        model.Message = String.Format(EduSuiteUIResources.CantDeleteErrorMessage, EduSuiteUIResources.Caste);
                        model.IsSuccessful = false;
                        ActivityLog.CreateActivityLog(MenuConstants.Caste, ActionConstants.Delete, DbConstants.LogType.Debug, model.RowKey, ex.GetBaseException().Message);
                    }
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    model.Message = String.Format(EduSuiteUIResources.FailedToDeleteErrorMessage, EduSuiteUIResources.Caste);
                    model.IsSuccessful = false;
                    ActivityLog.CreateActivityLog(MenuConstants.Caste, ActionConstants.Delete, DbConstants.LogType.Debug, model.RowKey, ex.GetBaseException().Message);
                }
            }

            return model;
        }

        public void FillReligion(CasteViewModel model)
        {
            model.ReligionList = dbContext.Religions.Select(x => new SelectListModel
            {
                RowKey = x.RowKey,
                Text = x.ReligionName
            }).ToList();
        }
    }
}
