using System;
using System.Collections.Generic;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CITS.EduSuite.Business.Common;
using CITS.EduSuite.Business.Interfaces;
using CITS.EduSuite.Business.Models.Resources;
using CITS.EduSuite.Business.Models.ViewModels;
using CITS.EduSuite.Data;

namespace CITS.EduSuite.Business.Services
{
    public class EmployeeLanguageSkillService : IEmployeeLanguageSkillService
    {
        private EduSuiteDatabase dbContext;

        public EmployeeLanguageSkillService(EduSuiteDatabase objDB)
        {
            this.dbContext = objDB;
        }

        public EmployeeLanguageSkillViewModel GetEmployeeLanguageSkillsById(long EmployeeId)
        {
            try
            {
                EmployeeLanguageSkillViewModel model = new EmployeeLanguageSkillViewModel();
                model.EmployeeLanguageSkills = dbContext.EmployeeLanguageSkills.Where(x => x.EmployeeKey == EmployeeId).Select(row => new languageSkillsViewModel
                {
                    RowKey = row.RowKey,
                    LanguageKey = row.LanguageKey,
                    IsRead = row.IsRead,
                    IsWrite = row.IsWrite,
                    IsSpeak = row.IsSpeak,
                    SkillLevelKey = row.SkillLevelKey,

                }).ToList();
                if (model.EmployeeLanguageSkills.Count == 0)
                {
                    model.EmployeeLanguageSkills.Add(new languageSkillsViewModel
                    {
                        RowKey = 0,
                        LanguageKey = 0,
                        IsRead = false,
                        IsWrite = false,
                        IsSpeak = false,
                        SkillLevelKey = 0,
                    });
                }
                if (model == null)
                {
                    model = new EmployeeLanguageSkillViewModel();
                }
                model.EmployeeKey = EmployeeId;
                FillDropdownList(model.EmployeeLanguageSkills);
                return model;
            }
            catch (Exception ex)
            {
                ActivityLog.CreateActivityLog(MenuConstants.EmployeeLanguageSkill, ActionConstants.View, DbConstants.LogType.Error, EmployeeId, ex.GetBaseException().Message);
                return new EmployeeLanguageSkillViewModel();
               
            }
        }

        public EmployeeLanguageSkillViewModel UpdateEmployeeLanguageSkills(EmployeeLanguageSkillViewModel model)
        {
            FillDropdownList(model.EmployeeLanguageSkills);
            using (var transaction = dbContext.Database.BeginTransaction())
            {
                try
                {
                    CreateLanguageSkills(model.EmployeeLanguageSkills.Where(row => row.RowKey == 0).ToList(), model.EmployeeKey);
                    UpdateLanguageSkills(model.EmployeeLanguageSkills.Where(row => row.RowKey != 0).ToList(), model.EmployeeKey);
                    dbContext.SaveChanges();
                    transaction.Commit();
                    model.Message = EduSuiteUIResources.Success;
                    model.IsSuccessful = true;
                    ActivityLog.CreateActivityLog(MenuConstants.EmployeeLanguageSkill, (model.EmployeeLanguageSkills.Any(row => row.RowKey != 0) ? ActionConstants.Edit :ActionConstants.Add), DbConstants.LogType.Info, model.EmployeeKey, model.Message);
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    model.Message = String.Format(EduSuiteUIResources.FailedToSaveErrorMessage, EduSuiteUIResources.EmployeeLanguageSkill);
                    model.IsSuccessful = false;
                    ActivityLog.CreateActivityLog(MenuConstants.EmployeeLanguageSkill, (model.EmployeeLanguageSkills.Any(row => row.RowKey != 0) ? ActionConstants.Edit :ActionConstants.Add), DbConstants.LogType.Error, model.EmployeeKey, ex.GetBaseException().Message);
                }

            }
            return model;
        }
        private void CreateLanguageSkills(List<languageSkillsViewModel> modelList, Int64 EmployeeKey)
        {

            Int64 maxKey = dbContext.EmployeeLanguageSkills.Select(p => p.RowKey).DefaultIfEmpty().Max();
            foreach (languageSkillsViewModel model in modelList)
            {

                EmployeeLanguageSkill employeeLanguageSkillsModel = new EmployeeLanguageSkill();
                employeeLanguageSkillsModel.RowKey = Convert.ToInt64(maxKey + 1);
                employeeLanguageSkillsModel.EmployeeKey = EmployeeKey;
                employeeLanguageSkillsModel.LanguageKey = model.LanguageKey;
                employeeLanguageSkillsModel.IsRead = model.IsRead;
                employeeLanguageSkillsModel.IsWrite = model.IsWrite;
                employeeLanguageSkillsModel.IsSpeak = model.IsSpeak;
                employeeLanguageSkillsModel.SkillLevelKey = model.SkillLevelKey;
                dbContext.EmployeeLanguageSkills.Add(employeeLanguageSkillsModel);
                maxKey++;

            }

        }

        private void UpdateLanguageSkills(List<languageSkillsViewModel> modelList, Int64 EmployeeKey)
        {

            foreach (languageSkillsViewModel model in modelList)
            {

                EmployeeLanguageSkill employeeLanguageSkillsModel = new EmployeeLanguageSkill();
                employeeLanguageSkillsModel = dbContext.EmployeeLanguageSkills.SingleOrDefault(row => row.RowKey == model.RowKey);
                employeeLanguageSkillsModel.LanguageKey = model.LanguageKey;
                employeeLanguageSkillsModel.IsRead = model.IsRead;
                employeeLanguageSkillsModel.IsWrite = model.IsWrite;
                employeeLanguageSkillsModel.IsSpeak = model.IsSpeak;
                employeeLanguageSkillsModel.SkillLevelKey = model.SkillLevelKey;
            }
        }

        public EmployeeLanguageSkillViewModel CheckIdentityTypeExists(short LanguageKey, long EmployeeKey, long RowKey)
        {
            EmployeeLanguageSkillViewModel model = new EmployeeLanguageSkillViewModel();
            if (dbContext.EmployeeLanguageSkills.Where(row => row.LanguageKey == LanguageKey && row.EmployeeKey == EmployeeKey && row.RowKey != RowKey).Any())
            {
                model.IsSuccessful = false;

            }
            else
            {
                model.IsSuccessful = true;
            }
            return model;
        }


        public EmployeeLanguageSkillViewModel DeleteEmployeeLanguageSkills(languageSkillsViewModel model)
        {
            EmployeeLanguageSkillViewModel employeeLanguageSkillsModel = new EmployeeLanguageSkillViewModel();
            FillDropdownList(employeeLanguageSkillsModel.EmployeeLanguageSkills);
            using (var transaction = dbContext.Database.BeginTransaction())
            {
                try
                {
                    EmployeeLanguageSkill employeeemployeeLanguageSkills = dbContext.EmployeeLanguageSkills.SingleOrDefault(row => row.RowKey == model.RowKey);
                    dbContext.EmployeeLanguageSkills.Remove(employeeemployeeLanguageSkills);
                    dbContext.SaveChanges();
                    transaction.Commit();
                    employeeLanguageSkillsModel.Message = EduSuiteUIResources.Success;
                    employeeLanguageSkillsModel.IsSuccessful = true;

                    ActivityLog.CreateActivityLog(MenuConstants.EmployeeLanguageSkill, ActionConstants.Delete, DbConstants.LogType.Info, model.RowKey, employeeLanguageSkillsModel.Message);
                }
                catch (DbUpdateException ex)
                {
                    transaction.Rollback();
                    if (ex.GetBaseException().Message.ToUpper().Contains(EduSuiteUIResources.ForeignKeyError.ToUpper()))
                    {
                        employeeLanguageSkillsModel.Message = String.Format(EduSuiteUIResources.CantDeleteErrorMessage, EduSuiteUIResources.EmployeeLanguageSkill);
                        employeeLanguageSkillsModel.IsSuccessful = false;
                        ActivityLog.CreateActivityLog(MenuConstants.EmployeeLanguageSkill, ActionConstants.Delete, DbConstants.LogType.Error, model.RowKey, ex.GetBaseException().Message);
                    }
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    employeeLanguageSkillsModel.Message = String.Format(EduSuiteUIResources.FailedToDeleteErrorMessage, EduSuiteUIResources.EmployeeLanguageSkill);
                    employeeLanguageSkillsModel.IsSuccessful = false;
                    ActivityLog.CreateActivityLog(MenuConstants.EmployeeLanguageSkill, ActionConstants.Delete, DbConstants.LogType.Error, model.RowKey, ex.GetBaseException().Message);
                }
            }
            return employeeLanguageSkillsModel;
        }

        private void FillDropdownList(List<languageSkillsViewModel> modelList)
        {
            foreach (var model in modelList)
            {
                FillLanguages(model);
                FillSkillLevels(model);
            }
        }

        private void FillLanguages(languageSkillsViewModel model)
        {
            model.Languages = dbContext.VwLanguageSelectActiveOnlies.OrderBy(row => row.DisplayOrder).Select(row => new SelectListModel
            {
                RowKey = row.RowKey,
                Text = row.LanguageName
            }).ToList();
        }

        private void FillSkillLevels(languageSkillsViewModel model)
        {
            model.SkillLevels = dbContext.VwSkillLevelSelectActiveOnlies.OrderBy(row => row.DisplayOrder).Select(row => new SelectListModel
            {
                RowKey = row.RowKey,
                Text = row.SkillLevelName
            }).ToList();
        }


    }
}
