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
    public class LanguageService : ILanguageService
    {
        private EduSuiteDatabase dbContext;

        public LanguageService(EduSuiteDatabase objDB)
        {
            this.dbContext = objDB;
        }
        public List<LanguageViewModel> GetLanguages(string searchText)
        {
            try
            {
                var languageList = (from l in dbContext.Languages
                                    orderby l.RowKey descending
                                    where (l.LanguageName.Contains(searchText))
                                    select new LanguageViewModel
                                    {
                                        RowKey = l.RowKey,
                                        LanguageName = l.LanguageName,
                                        IsActive = l.IsActive,
                                        LanguageShortName = l.LanguageShortName

                                    }).ToList();

                return languageList.GroupBy(x => x.RowKey).Select(y => y.First()).ToList<LanguageViewModel>();
            }
            catch (Exception ex)
            {

                ActivityLog.CreateActivityLog(MenuConstants.Language, ActionConstants.View, DbConstants.LogType.Error, null, ex.GetBaseException().Message);
                return new List<LanguageViewModel>();
            }
        }

        public LanguageViewModel GetLanguagesById(Int16? id)
        {
            try
            {
                LanguageViewModel model = new LanguageViewModel();
                model = dbContext.Languages.Select(row => new LanguageViewModel
                {
                    RowKey = row.RowKey,
                    LanguageName = row.LanguageName,
                    LanguageShortName = row.LanguageShortName,
                    IsActive = row.IsActive

                }).Where(x => x.RowKey == id).FirstOrDefault();
                if (model == null)
                {
                    model = new LanguageViewModel();
                }
                return model;
            }
            catch (Exception ex)
            {
               
                ActivityLog.CreateActivityLog(MenuConstants.Language, ActionConstants.View, DbConstants.LogType.Error, id, ex.GetBaseException().Message);
                return new LanguageViewModel();
            }
        }

      
        public LanguageViewModel CreateLanguage(LanguageViewModel model)
        {
            Language languageModel = new Language();

            var LanguageNameCheck = dbContext.Languages.Where(row => row.LanguageName.ToLower().Trim() == model.LanguageName.ToLower().Trim()).ToList();
            if (LanguageNameCheck.Count != 0)
            {
                model.Message = String.Format(EduSuiteUIResources.ExistsErrorMessage, EduSuiteUIResources.Language);
                model.IsSuccessful = false;
                return model;
            }
            else
            {
                //var LanguageShortNameExistCheck = dbContext.Languages.Where(row => row.LanguageShortName.ToLower().Trim() == model.LanguageShortName.ToLower().Trim()).ToList();
                //if (LanguageShortNameExistCheck.Count != 0)
                //{
                //    model.Message = String.Format(EduSuiteUIResources.ExistsErrorMessage, EduSuiteUIResources.LanguageShortName);
                //    model.IsSuccessful = false;
                //    return model;
                //}
            }

            using (var transaction = dbContext.Database.BeginTransaction())
            {
                try
                {
                    Int16 maxKey = dbContext.Languages.Select(l => l.RowKey).DefaultIfEmpty().Max();
                    languageModel.RowKey = Convert.ToInt16(maxKey + 1);
                    languageModel.LanguageName = model.LanguageName;
                    //languageModel.LanguageShortName = model.LanguageShortName;
                    languageModel.IsActive = model.IsActive;
                    dbContext.Languages.Add(languageModel);
                    dbContext.SaveChanges();
                    transaction.Commit();
                    model.Message = EduSuiteUIResources.Success;
                    model.IsSuccessful = true;
                    ActivityLog.CreateActivityLog(MenuConstants.Language, ActionConstants.Add, DbConstants.LogType.Info, languageModel.RowKey, model.Message);

                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    model.Message = String.Format(EduSuiteUIResources.FailedToSaveErrorMessage, EduSuiteUIResources.Language);
                    model.IsSuccessful = false;
                    ActivityLog.CreateActivityLog(MenuConstants.Language, ActionConstants.Add, DbConstants.LogType.Error, languageModel.RowKey, ex.GetBaseException().Message);
                }
            }
            return model;
        }

        public LanguageViewModel UpdateLanguage(LanguageViewModel model)
        {
            Language languageModel = new Language();

            var LanguageNameCheck = dbContext.Languages.Where(row => row.LanguageName.ToLower().Trim() == model.LanguageName.ToLower().Trim() && row.RowKey != model.RowKey).ToList();
            if (LanguageNameCheck.Count != 0)
            {
                model.Message = String.Format(EduSuiteUIResources.ExistsErrorMessage, EduSuiteUIResources.Language);
                model.IsSuccessful = false;
                return model;
            }
            else
            {
                //var LanguageShortNameExistCheck = dbContext.Languages.Where(row => row.LanguageShortName.ToLower().Trim() == model.LanguageShortName.ToLower().Trim() && row.RowKey != model.RowKey).ToList();
                //if (LanguageShortNameExistCheck.Count != 0)
                //{
                //    model.Message = String.Format(EduSuiteUIResources.ExistsErrorMessage, EduSuiteUIResources.LanguageShortName);
                //    model.IsSuccessful = false;
                //    return model;
                //}
            }

            using (var transaction = dbContext.Database.BeginTransaction())
            {
                try
                {
                    languageModel = dbContext.Languages.SingleOrDefault(row => row.RowKey == model.RowKey);
                    languageModel.LanguageName = model.LanguageName;
                    languageModel.LanguageShortName = model.LanguageShortName;
                    languageModel.IsActive = model.IsActive;
                    dbContext.SaveChanges();
                    transaction.Commit();
                    model.Message = EduSuiteUIResources.Success;
                    model.IsSuccessful = true;
                    ActivityLog.CreateActivityLog(MenuConstants.Language, ActionConstants.Edit, DbConstants.LogType.Info, model.RowKey, model.Message);

                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    model.Message = String.Format(EduSuiteUIResources.FailedToSaveErrorMessage, EduSuiteUIResources.Language);
                    model.IsSuccessful = false;
                    ActivityLog.CreateActivityLog(MenuConstants.Language, ActionConstants.Edit, DbConstants.LogType.Error, model.RowKey, ex.GetBaseException().Message);
                }
            }
            return model;
        }

        public LanguageViewModel DeleteLanguage(LanguageViewModel model)
        {
            using (var transaction = dbContext.Database.BeginTransaction())
            {
                try
                {
                    Language language = dbContext.Languages.SingleOrDefault(row => row.RowKey == model.RowKey);
                    dbContext.Languages.Remove(language);
                    dbContext.SaveChanges();
                    transaction.Commit();
                    model.Message = EduSuiteUIResources.Success;
                    model.IsSuccessful = true;
                    ActivityLog.CreateActivityLog(MenuConstants.Language, ActionConstants.Delete, DbConstants.LogType.Info, model.RowKey, model.Message);

                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    if (ex.GetBaseException().Message.ToUpper().Contains(EduSuiteUIResources.ForeignKeyError.ToUpper()))
                    {
                        model.Message = String.Format(EduSuiteUIResources.CantDeleteErrorMessage, EduSuiteUIResources.Language);
                        model.IsSuccessful = false;
                        ActivityLog.CreateActivityLog(MenuConstants.Language, ActionConstants.Delete, DbConstants.LogType.Error, model.RowKey, ex.GetBaseException().Message);
                    }
                }
            }
            return model;
        }



    }
}
