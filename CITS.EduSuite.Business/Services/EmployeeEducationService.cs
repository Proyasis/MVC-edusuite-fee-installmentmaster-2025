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
    public class EmployeeEducationService : IEmployeeEducationService
    {
        private EduSuiteDatabase dbContext;

        public EmployeeEducationService(EduSuiteDatabase objDB)
        {
            this.dbContext = objDB;
        }


        public EmployeeEducationViewModel GetEmployeeEducationById(long EmployeeId)
        {
            try
            {
                EmployeeEducationViewModel model = new EmployeeEducationViewModel();
                model.EmployeeEducations = dbContext.EmployeeEducations.Where(x => x.EmployeeKey == EmployeeId).Select(ed => new EducationViewModel
                {
                    RowKey = ed.RowKey,
                    EducationName = ed.EducationType.EducationTypeName,
                    SubjectName = ed.SubjectName,
                    CertifiedBy = ed.CertifiedBy,
                    CompletedYear = ed.CompletedYear,
                    EducationTypeKey = ed.EducationTypeKey,
                    Mark = ed.Mark,
                    GradigSystemKey = ed.GradigSystemKey,
                    EmployeeKey = ed.EmployeeKey,
                    AttanchedFileName = ed.AttanchedFileName,
                    AttanchedFileNamePath = UrlConstants.EmployeeUrl + SqlFunctions.StringConvert((double)ed.Employee.RowKey).Trim() + "/Education/" + ed.AttanchedFileName,
                }).ToList();
                if (model.EmployeeEducations.Count == 0)
                {
                    model.EmployeeEducations.Add(new EducationViewModel());
                }
                if (model == null)
                {
                    model = new EmployeeEducationViewModel();
                }
                model.EmployeeKey = EmployeeId;
                FillDropdownLists(model.EmployeeEducations);
                return model;
            }
            catch (Exception ex)
            {
                ActivityLog.CreateActivityLog(MenuConstants.EmployeeEducation, ActionConstants.View, DbConstants.LogType.Error, EmployeeId, ex.GetBaseException().Message);
                return new EmployeeEducationViewModel();
            }
        }

        public EmployeeEducationViewModel UpdateEmployeeEducation(EmployeeEducationViewModel model)
        {
            FillDropdownLists(model.EmployeeEducations);


            using (var transaction = dbContext.Database.BeginTransaction())
            {
                try
                {


                    CreateEducation(model.EmployeeEducations.Where(row => row.RowKey == 0).ToList(), model.EmployeeKey);
                    UpdateEducation(model.EmployeeEducations.Where(row => row.RowKey != 0).ToList(), model.EmployeeKey);
                    dbContext.SaveChanges();
                    transaction.Commit();

                    model.Message = EduSuiteUIResources.Success; /*Pending val*/
                    model.IsSuccessful = true;
                    ActivityLog.CreateActivityLog(MenuConstants.EmployeeEducation, (model.EmployeeEducations.Any(row => row.RowKey != 0) ? ActionConstants.Edit : ActionConstants.Add), DbConstants.LogType.Info, model.EmployeeKey, model.Message);
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    model.Message = String.Format(EduSuiteUIResources.FailedToSaveErrorMessage, EduSuiteUIResources.EmployeeEducation);
                    model.IsSuccessful = false;
                    ActivityLog.CreateActivityLog(MenuConstants.EmployeeEducation, (model.EmployeeEducations.Any(row => row.RowKey != 0) ? ActionConstants.Edit : ActionConstants.Add), DbConstants.LogType.Error, model.EmployeeKey, ex.GetBaseException().Message);
                }
            }
            return model;
        }


        private void CreateEducation(List<EducationViewModel> modelList, Int64 EmployeeKey)
        {

            Int64 maxEmployeeEducationTypeKey = dbContext.EmployeeEducations.Select(p => p.RowKey).DefaultIfEmpty().Max();
            foreach (EducationViewModel model in modelList)
            {

                EmployeeEducation EducationModel = new EmployeeEducation();

                EducationModel.RowKey = Convert.ToInt64(maxEmployeeEducationTypeKey + 1);
                EducationModel.EmployeeKey = EmployeeKey;
                EducationModel.SubjectName = model.SubjectName;
                EducationModel.CertifiedBy = model.CertifiedBy;
                EducationModel.CompletedYear = model.CompletedYear;
                EducationModel.GradigSystemKey = model.GradigSystemKey;
                EducationModel.Mark = model.Mark;
                EducationModel.EducationTypeKey = model.EducationTypeKey;
                if (model.AttanchedFile != null)
                {
                    EducationModel.AttanchedFileName = EducationModel.RowKey + model.AttanchedFileName;
                }
                dbContext.EmployeeEducations.Add(EducationModel);
                model.AttanchedFileName = EducationModel.AttanchedFileName;

                maxEmployeeEducationTypeKey++;

            }

        }

        public void UpdateEducation(List<EducationViewModel> modelList, Int64 EmployeeKey)
        {

            foreach (EducationViewModel model in modelList)
            {

                EmployeeEducation EducationModel = new EmployeeEducation();
                EducationModel = dbContext.EmployeeEducations.SingleOrDefault(row => row.RowKey == model.RowKey);
                EducationModel.SubjectName = model.SubjectName;
                EducationModel.CertifiedBy = model.CertifiedBy;
                EducationModel.EducationTypeKey = model.EducationTypeKey;
                EducationModel.CompletedYear = model.CompletedYear;
                EducationModel.GradigSystemKey = model.GradigSystemKey;
                EducationModel.Mark = model.Mark;
                if (model.AttanchedFile != null)
                {
                    EducationModel.AttanchedFileName = EducationModel.RowKey + model.AttanchedFileName;
                }
            }
        }


        public EmployeeEducationViewModel DeleteEmployeeEducation(EducationViewModel model)
        {
            EmployeeEducationViewModel employeeEducationModel = new EmployeeEducationViewModel();
            FillDropdownLists(employeeEducationModel.EmployeeEducations);
            using (var transaction = dbContext.Database.BeginTransaction())
            {
                try
                {
                    EmployeeEducation EmployeeEducation = dbContext.EmployeeEducations.SingleOrDefault(row => row.RowKey == model.RowKey);
                    employeeEducationModel.EmployeeKey = EmployeeEducation.EmployeeKey;
                    employeeEducationModel.EmployeeEducations.Add(new EducationViewModel { AttanchedFileNamePath = EmployeeEducation.AttanchedFileName });
                    dbContext.EmployeeEducations.Remove(EmployeeEducation);

                    dbContext.SaveChanges();
                    transaction.Commit();
                    employeeEducationModel.Message = EduSuiteUIResources.Success;
                    employeeEducationModel.IsSuccessful = true;
                    ActivityLog.CreateActivityLog(MenuConstants.EmployeeEducation, ActionConstants.Delete, DbConstants.LogType.Info, model.RowKey, employeeEducationModel.Message);
                }
                catch (DbUpdateException ex)
                {
                    transaction.Rollback();
                    if (ex.GetBaseException().Message.ToUpper().Contains(EduSuiteUIResources.ForeignKeyError.ToUpper()))
                    {
                        employeeEducationModel.Message = String.Format(EduSuiteUIResources.CantDeleteErrorMessage, EduSuiteUIResources.Employee);
                        employeeEducationModel.IsSuccessful = false;
                        ActivityLog.CreateActivityLog(MenuConstants.EmployeeEducation, ActionConstants.Delete, DbConstants.LogType.Error, model.RowKey, ex.GetBaseException().Message);
                    }
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    employeeEducationModel.Message = String.Format(EduSuiteUIResources.FailedToDeleteErrorMessage, EduSuiteUIResources.EmployeeEducation);
                    employeeEducationModel.IsSuccessful = false;
                    ActivityLog.CreateActivityLog(MenuConstants.EmployeeEducation, ActionConstants.Delete, DbConstants.LogType.Error, model.RowKey, ex.GetBaseException().Message);
                }
            }
            return employeeEducationModel;
        }

        public EmployeeEducationViewModel CheckEducationTypeExists(Int16 EducationTypeKey, Int64 EmployeeKey, Int64 RowKey)
        {
            EmployeeEducationViewModel model = new EmployeeEducationViewModel();
            if (dbContext.EmployeeEducations.Where(row => row.EducationTypeKey == EducationTypeKey && row.EmployeeKey == EmployeeKey && row.RowKey != RowKey).Any())
            {
                model.IsSuccessful = false;

            }
            else
            {
                model.IsSuccessful = true;
            }
            return model;
        }


        private void FillDropdownLists(List<EducationViewModel> modelList)
        {
            foreach (var model in modelList)
            {
                FillEducationType(model);
                FillGradigSystem(model);
            }

        }


        private void FillEducationType(EducationViewModel model)
        {
            model.EducationType = dbContext.VwEducationTypeSelectActiveOnlies.Select(row => new SelectListModel
            {
                RowKey = row.RowKey,
                Text = row.EducationTypeName
            }).ToList();
        }

        private void FillGradigSystem(EducationViewModel model)
        {
            model.GradigSystem = dbContext.VwGradingSystemSelectActiveOnlies.Select(row => new SelectListModel
            {
                RowKey = row.RowKey,
                Text = row.GradingSystemName
            }).ToList();
        }




    }
}
