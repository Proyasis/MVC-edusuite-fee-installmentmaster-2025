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
    public class SecondLanguageService : ISecondLanguageService
    {
        private EduSuiteDatabase dbContext;
        public SecondLanguageService(EduSuiteDatabase objDB)
        {
            this.dbContext = objDB;
        }
        public SecondLanguageViewModel GetSecondLanguageById(int? id)
        {
            try
            {
                SecondLanguageViewModel model = new SecondLanguageViewModel();
                model = dbContext.SecondLanguages.Select(row => new SecondLanguageViewModel
                {
                    RowKey = row.RowKey,
                    SecondLanguageName = row.SecondLanguageName,
                    IsActive = row.IsActive
                }).Where(x => x.RowKey == id).FirstOrDefault();
                if (model == null)
                {
                    model = new SecondLanguageViewModel();
                }
                return model;
            }
            catch (Exception ex)
            {
                ActivityLog.CreateActivityLog(MenuConstants.SecondLanguage, ActionConstants.View, DbConstants.LogType.Error, id, ex.GetBaseException().Message);
                return new SecondLanguageViewModel();
               

            }
        }
        public SecondLanguageViewModel CreateSecondLanguage(SecondLanguageViewModel model)
        {
            var SecondLanguageCheck = dbContext.SecondLanguages.Where(row => row.SecondLanguageName.ToLower() == model.SecondLanguageName.ToLower()).Count();
            SecondLanguage SecondLanguageModel = new SecondLanguage();
            if (SecondLanguageCheck != 0)
            {
                model.Message = String.Format(EduSuiteUIResources.ExistsErrorMessage, EduSuiteUIResources.SecoundLanguage);
                model.IsSuccessful = false;
                return model;
            }

            using (var transaction = dbContext.Database.BeginTransaction())
            {
                try
                {
                    int MaxKey = dbContext.SecondLanguages.Select(p => p.RowKey).DefaultIfEmpty().Max();

                    SecondLanguageModel.RowKey = Convert.ToInt16(MaxKey + 1);
                    SecondLanguageModel.SecondLanguageName = model.SecondLanguageName;
                    SecondLanguageModel.IsActive = model.IsActive;
                    dbContext.SecondLanguages.Add(SecondLanguageModel);
                    dbContext.SaveChanges();
                    transaction.Commit();
                    model.Message = EduSuiteUIResources.Success;
                    model.IsSuccessful = true;
                    ActivityLog.CreateActivityLog(MenuConstants.SecondLanguage, ActionConstants.Add, DbConstants.LogType.Info, model.RowKey, model.Message);


                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    model.Message = String.Format(EduSuiteUIResources.FailedToSaveErrorMessage, EduSuiteUIResources.SecoundLanguage);
                    model.IsSuccessful = false;
                    ActivityLog.CreateActivityLog(MenuConstants.SecondLanguage, ActionConstants.Add, DbConstants.LogType.Error, model.RowKey, ex.GetBaseException().Message);
                }
            }
            return model;
        }
        public SecondLanguageViewModel UpdateSecondLanguage(SecondLanguageViewModel model)
        {
            var SecondLanguageCheck = dbContext.SecondLanguages.Where(row => row.SecondLanguageName.ToLower() == model.SecondLanguageName.ToLower()
               && row.RowKey != model.RowKey).ToList();

            SecondLanguage SecondLanguageModel = new SecondLanguage();
            if (SecondLanguageCheck.Count != 0)
            {
                model.Message = String.Format(EduSuiteUIResources.ExistsErrorMessage, EduSuiteUIResources.SecoundLanguage);
                model.IsSuccessful = false;
                return model;
            }

            using (var transaction = dbContext.Database.BeginTransaction())
            {
                try
                {
                    SecondLanguageModel = dbContext.SecondLanguages.SingleOrDefault(x => x.RowKey == model.RowKey);
                    SecondLanguageModel.SecondLanguageName = model.SecondLanguageName;
                    SecondLanguageModel.IsActive = model.IsActive;
                    dbContext.SaveChanges();
                    transaction.Commit();
                    model.Message = EduSuiteUIResources.Success;
                    model.IsSuccessful = true;
                    ActivityLog.CreateActivityLog(MenuConstants.SecondLanguage, ActionConstants.Edit, DbConstants.LogType.Info, model.RowKey, model.Message);

                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    model.Message = String.Format(EduSuiteUIResources.FailedToSaveErrorMessage, EduSuiteUIResources.SecoundLanguage);
                    model.IsSuccessful = false;
                    ActivityLog.CreateActivityLog(MenuConstants.SecondLanguage, ActionConstants.Edit, DbConstants.LogType.Error, model.RowKey, ex.GetBaseException().Message);
                }
            }
            return model;

        }
        public SecondLanguageViewModel DeleteSecondLanguage(SecondLanguageViewModel model)
        {
            using (var transaction = dbContext.Database.BeginTransaction())
            {
                try
                {
                    SecondLanguage SecondLanguageModel = dbContext.SecondLanguages.SingleOrDefault(row => row.RowKey == model.RowKey);
                    dbContext.SecondLanguages.Remove(SecondLanguageModel);
                    dbContext.SaveChanges();
                    transaction.Commit();
                    model.Message = EduSuiteUIResources.Success;
                    model.IsSuccessful = true;
                    ActivityLog.CreateActivityLog(MenuConstants.SecondLanguage, ActionConstants.Delete, DbConstants.LogType.Info, model.RowKey, model.Message);

                }
                catch (DbUpdateException ex)
                {
                    transaction.Rollback();
                    if (ex.GetBaseException().Message.ToUpper().Contains(EduSuiteUIResources.ForeignKeyError.ToUpper()))
                    {
                        model.Message = String.Format(EduSuiteUIResources.CantDeleteErrorMessage, EduSuiteUIResources.SecoundLanguage);
                        model.IsSuccessful = false;
                        ActivityLog.CreateActivityLog(MenuConstants.SecondLanguage, ActionConstants.Delete, DbConstants.LogType.Error, model.RowKey, ex.GetBaseException().Message);
                    }
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    model.Message = String.Format(EduSuiteUIResources.FailedToDeleteErrorMessage, EduSuiteUIResources.SecoundLanguage);
                    model.IsSuccessful = false;
                    ActivityLog.CreateActivityLog(MenuConstants.SecondLanguage, ActionConstants.Delete, DbConstants.LogType.Error, model.RowKey, ex.GetBaseException().Message);
                }
            }

            return model;
        }
        public List<SecondLanguageViewModel> GetSecondLanguage(string searchText)
        {
            try
            {
                var SecondLanguageList = (from p in dbContext.SecondLanguages
                                          orderby p.RowKey
                                          where (p.SecondLanguageName.Contains(searchText))
                                          select new SecondLanguageViewModel
                                   {
                                       RowKey = p.RowKey,
                                       SecondLanguageName = p.SecondLanguageName,
                                       //IsActiveText = p.IsActive == true ? ApplicationResources.Yes : ApplicationResources.No
                                       IsActive=p.IsActive
                                   }).ToList();
                return SecondLanguageList.GroupBy(x => x.RowKey).Select(y => y.First()).ToList<SecondLanguageViewModel>();
            }
            catch (Exception ex)
            {
                ActivityLog.CreateActivityLog(MenuConstants.SecondLanguage, ActionConstants.View, DbConstants.LogType.Error, null, ex.GetBaseException().Message);
                return new List<SecondLanguageViewModel>();
               

            }
        }
    }
}
