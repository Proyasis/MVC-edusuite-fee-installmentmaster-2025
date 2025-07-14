using System;
using System.Collections.Generic;
using System.Data.Entity.Infrastructure;
using System.Data.Entity.SqlServer;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CITS.EduSuite.Business.Common;
using CITS.EduSuite.Business.Interfaces;
using CITS.EduSuite.Business.Models.Resources;
//using CITS.EduSuite.Business.Models.Common;
using CITS.EduSuite.Business.Models.ViewModels;
using CITS.EduSuite.Data;

namespace CITS.EduSuite.Business.Services
{
    public class EmployeeExperienceService : IEmployeeExperienceService
    {
        private EduSuiteDatabase dbContext;

        public EmployeeExperienceService(EduSuiteDatabase objDB)
        {
            this.dbContext = objDB;
        }

        public EmployeeExperienceViewModel GetEmployeeExperienceById(long EmployeeId)
        {
            try
            {
                EmployeeExperienceViewModel model = new EmployeeExperienceViewModel();

                model.EmployeeExperiences = dbContext.EmployeeExperiences.Where(x => x.EmployeeKey == EmployeeId).Select(row => new ExperienceViewModel
                {
                    RowKey = row.RowKey,
                    CompanyName = row.CompanyName,
                    CompanyAddress = row.CompanyAddress,
                    CompanyPhoneNumber = row.CompanyPhoneNumber,
                    JobField = row.JobField,
                    JobPostion = row.JobPostion,
                    ExperienceStartDate = row.ExperienceStartDate,
                    ExperianceEndDate = row.ExperianceEndDate,
                    StartingSalary = row.StartingSalary,
                    EndingSalary = row.EndingSalary,
                    ContactPersonName = row.ContactPersonName,
                    ContactPersonNumber = row.ContactPersonNumber,
                    ContactPersonDesignation = row.ContactPersonDesignation,
                    AttanchedFileName = row.AttanchedFileName,
                    AttanchedFileNamePath = UrlConstants.EmployeeUrl + row.Employee.RowKey + "/Experience/" + row.AttanchedFileName

                }).ToList();
                if (model.EmployeeExperiences.Count == 0)
                {
                    model.EmployeeExperiences.Add(new ExperienceViewModel());
                }
                if (model == null)
                {
                    model = new EmployeeExperienceViewModel();
                }
                model.EmployeeKey = EmployeeId;
                return model;
            }
            catch (Exception ex)
            {
                ActivityLog.CreateActivityLog(MenuConstants.EmployeeExperience, ActionConstants.View, DbConstants.LogType.Error, EmployeeId, ex.GetBaseException().Message);
                return new EmployeeExperienceViewModel();
            }
        }

        public EmployeeExperienceViewModel UpdateEmployeeExperience(EmployeeExperienceViewModel model)
        {
            using (var transaction = dbContext.Database.BeginTransaction())
            {
                try
                {
                    CreateExperience(model.EmployeeExperiences.Where(row => row.RowKey == 0).ToList(), model.EmployeeKey);
                    UpdateExperience(model.EmployeeExperiences.Where(row => row.RowKey != 0).ToList(), model.EmployeeKey);
                    dbContext.SaveChanges();
                    transaction.Commit();

                    model.Message = EduSuiteUIResources.Success;
                    model.IsSuccessful = true;
                    ActivityLog.CreateActivityLog(MenuConstants.EmployeeExperience, (model.EmployeeExperiences.Any(row => row.RowKey != 0) ? ActionConstants.Edit : ActionConstants.Add), DbConstants.LogType.Info, model.EmployeeKey, model.Message);
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    model.Message = String.Format(EduSuiteUIResources.FailedToSaveErrorMessage, EduSuiteUIResources.EmployeeExperience);
                    model.IsSuccessful = false;
                    ActivityLog.CreateActivityLog(MenuConstants.EmployeeExperience, (model.EmployeeExperiences.Any(row => row.RowKey != 0) ? ActionConstants.Edit : ActionConstants.Add), DbConstants.LogType.Error, model.EmployeeKey, ex.GetBaseException().Message);
                }
            }
            return model;
        }

        private void CreateExperience(List<ExperienceViewModel> modelList, Int64 EmployeeKey)
        {

            Int64 maxEmployeeExperienceKey = dbContext.EmployeeExperiences.Select(p => p.RowKey).DefaultIfEmpty().Max();
            foreach (ExperienceViewModel model in modelList)
            {
                EmployeeExperience employeeExperienceModel = new EmployeeExperience();

                employeeExperienceModel.RowKey = Convert.ToInt64(maxEmployeeExperienceKey + 1);
                employeeExperienceModel.EmployeeKey = EmployeeKey;
                employeeExperienceModel.CompanyName = model.CompanyName;
                employeeExperienceModel.CompanyAddress = model.CompanyAddress;
                employeeExperienceModel.CompanyPhoneNumber = model.CompanyPhoneNumber;
                employeeExperienceModel.JobField = model.JobField;
                employeeExperienceModel.JobPostion = model.JobPostion;
                employeeExperienceModel.ExperienceStartDate = model.ExperienceStartDate;
                employeeExperienceModel.ExperianceEndDate = model.ExperianceEndDate;
                employeeExperienceModel.StartingSalary = model.StartingSalary;
                employeeExperienceModel.EndingSalary = model.EndingSalary;
                employeeExperienceModel.ContactPersonName = model.ContactPersonName;
                employeeExperienceModel.ContactPersonNumber = model.ContactPersonNumber;
                employeeExperienceModel.ContactPersonDesignation = model.ContactPersonDesignation;
                if (model.AttanchedFile != null)
                {
                    employeeExperienceModel.AttanchedFileName = employeeExperienceModel.RowKey + model.AttanchedFileName;
                }
                dbContext.EmployeeExperiences.Add(employeeExperienceModel);
                model.AttanchedFileName = employeeExperienceModel.AttanchedFileName;
                maxEmployeeExperienceKey++;
            }
        }

        public void UpdateExperience(List<ExperienceViewModel> modelList, Int64 EmployeeKey)
        {
            foreach (ExperienceViewModel model in modelList)
            {
                EmployeeExperience employeeExperienceModel = new EmployeeExperience();
                employeeExperienceModel = dbContext.EmployeeExperiences.SingleOrDefault(row => row.RowKey == model.RowKey);
                employeeExperienceModel.CompanyName = model.CompanyName;
                employeeExperienceModel.CompanyAddress = model.CompanyAddress;
                employeeExperienceModel.CompanyPhoneNumber = model.CompanyPhoneNumber;
                employeeExperienceModel.JobField = model.JobField;
                employeeExperienceModel.JobPostion = model.JobPostion;
                employeeExperienceModel.ExperienceStartDate = model.ExperienceStartDate;
                employeeExperienceModel.ExperianceEndDate = model.ExperianceEndDate;
                employeeExperienceModel.StartingSalary = model.StartingSalary;
                employeeExperienceModel.EndingSalary = model.EndingSalary;
                employeeExperienceModel.ContactPersonName = model.ContactPersonName;
                employeeExperienceModel.ContactPersonNumber = model.ContactPersonNumber;
                if (model.AttanchedFile != null)
                {
                    employeeExperienceModel.AttanchedFileName = employeeExperienceModel.RowKey + model.AttanchedFileName;
                }
                employeeExperienceModel.ContactPersonDesignation = model.ContactPersonDesignation;
            }
        }

        public EmployeeExperienceViewModel DeleteEmployeeExperience(ExperienceViewModel model)
        {
            EmployeeExperienceViewModel employeeExperienceViewModel = new EmployeeExperienceViewModel();
            using (var transaction = dbContext.Database.BeginTransaction())
            {
                try
                {
                    EmployeeExperience employeeExperience = dbContext.EmployeeExperiences.SingleOrDefault(row => row.RowKey == model.RowKey);
                    employeeExperienceViewModel.EmployeeKey = employeeExperience.EmployeeKey;
                    employeeExperienceViewModel.EmployeeExperiences.Add(new ExperienceViewModel { AttanchedFileNamePath = employeeExperience.AttanchedFileName });
                    dbContext.EmployeeExperiences.Remove(employeeExperience);

                    dbContext.SaveChanges();
                    transaction.Commit();
                    employeeExperienceViewModel.Message = EduSuiteUIResources.Success;
                    employeeExperienceViewModel.IsSuccessful = true;
                    ActivityLog.CreateActivityLog(MenuConstants.EmployeeExperience, ActionConstants.Delete, DbConstants.LogType.Info, model.RowKey, employeeExperienceViewModel.Message);
                }
                catch (DbUpdateException ex)
                {
                    transaction.Rollback();
                    if (ex.GetBaseException().Message.ToUpper().Contains(EduSuiteUIResources.ForeignKeyError.ToUpper()))
                    {
                        employeeExperienceViewModel.Message = String.Format(EduSuiteUIResources.CantDeleteErrorMessage, EduSuiteUIResources.EmployeeExperience);
                        employeeExperienceViewModel.IsSuccessful = false;
                        ActivityLog.CreateActivityLog(MenuConstants.EmployeeExperience, ActionConstants.Delete, DbConstants.LogType.Debug, model.RowKey, ex.GetBaseException().Message);
                    }
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    employeeExperienceViewModel.Message = String.Format(EduSuiteUIResources.FailedToDeleteErrorMessage, EduSuiteUIResources.EmployeeExperience);
                    employeeExperienceViewModel.IsSuccessful = false;
                    ActivityLog.CreateActivityLog(MenuConstants.EmployeeExperience, ActionConstants.Delete, DbConstants.LogType.Error, model.RowKey, ex.GetBaseException().Message);
                }
            }
            return employeeExperienceViewModel;
        }
    }
}
