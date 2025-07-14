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
    public class EmployeeIdentityService : IEmployeeIdentityService
    {
        private EduSuiteDatabase dbContext;

        public EmployeeIdentityService(EduSuiteDatabase objDB)
        {
            this.dbContext = objDB;
        }

        public EmployeeIdentityViewModel GetEmployeeIdentitiesById(Int64 EmployeeId)
        {
            try
            {
                EmployeeIdentityViewModel model = new EmployeeIdentityViewModel();
                model.EmployeeIdentities = dbContext.EmployeeIdentities.Where(x => x.EmployeeKey == EmployeeId).Select(row => new IdentityViewModel
                {
                    RowKey = row.RowKey,
                    IdentityTypeKey = row.IdentityTypeKey,
                    IdentyUniqueID = row.IdentyUniqueID,
                    IdentityIssuedDate = row.IdentityIssuedDate,
                    IdentityExpiryDate = row.IdentityExpiryDate,
                    AttanchedFileName = row.AttanchedFileName,
                    AttanchedFileNamePath = UrlConstants.EmployeeUrl + row.Employee.RowKey + "/Identity/" + row.AttanchedFileName,
                }).ToList();
                if (model.EmployeeIdentities.Count == 0)
                {
                    model.EmployeeIdentities.Add(new IdentityViewModel());
                }
                if (model == null)
                {
                    model = new EmployeeIdentityViewModel();
                }
                model.EmployeeKey = EmployeeId;
                FillDropdownList(model.EmployeeIdentities);
                return model;
            }
            catch (Exception ex)
            {
                ActivityLog.CreateActivityLog(MenuConstants.EmployeeIdentity, ActionConstants.View, DbConstants.LogType.Error, EmployeeId, ex.GetBaseException().Message);
                return new EmployeeIdentityViewModel();

            }
        }

        public EmployeeIdentityViewModel UpdateEmployeeIdentity(EmployeeIdentityViewModel model)
        {
            FillDropdownList(model.EmployeeIdentities);
            using (var transaction = dbContext.Database.BeginTransaction())
            {
                try
                {
                    CreateIdentity(model.EmployeeIdentities.Where(row => row.RowKey == 0).ToList(), model.EmployeeKey);
                    UpdateIdentity(model.EmployeeIdentities.Where(row => row.RowKey != 0).ToList(), model.EmployeeKey);
                    dbContext.SaveChanges();
                    transaction.Commit();
                    model.Message = EduSuiteUIResources.Success;
                    model.IsSuccessful = true;
                    ActivityLog.CreateActivityLog(MenuConstants.EmployeeIdentity, (model.EmployeeIdentities.Any(row => row.RowKey != 0) ? ActionConstants.Edit : ActionConstants.Add), DbConstants.LogType.Info, model.EmployeeKey, model.Message);
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    model.Message = String.Format(EduSuiteUIResources.FailedToSaveErrorMessage, EduSuiteUIResources.EmployeeIdentity);
                    model.IsSuccessful = false;
                    ActivityLog.CreateActivityLog(MenuConstants.EmployeeIdentity, (model.EmployeeIdentities.Any(row => row.RowKey != 0) ? ActionConstants.Edit : ActionConstants.Add), DbConstants.LogType.Error, model.EmployeeKey, ex.GetBaseException().Message);
                }

            }
            return model;
        }

        private void CreateIdentity(List<IdentityViewModel> modelList, Int64 EmployeeKey)
        {

            Int64 maxKey = dbContext.EmployeeIdentities.Select(p => p.RowKey).DefaultIfEmpty().Max();
            foreach (IdentityViewModel model in modelList)
            {

                EmployeeIdentity employeeIdentityModel = new EmployeeIdentity();
                employeeIdentityModel.RowKey = Convert.ToInt64(maxKey + 1);
                employeeIdentityModel.EmployeeKey = EmployeeKey;
                employeeIdentityModel.IdentityTypeKey = model.IdentityTypeKey;
                employeeIdentityModel.IdentyUniqueID = model.IdentyUniqueID;
                employeeIdentityModel.IdentityIssuedDate = model.IdentityIssuedDate;
                employeeIdentityModel.IdentityExpiryDate = model.IdentityExpiryDate;
                if (model.AttanchedFile != null)
                {
                    employeeIdentityModel.AttanchedFileName = employeeIdentityModel.RowKey + model.AttanchedFileName;
                }
                dbContext.EmployeeIdentities.Add(employeeIdentityModel);
                model.AttanchedFileName = employeeIdentityModel.AttanchedFileName;
                maxKey++;
            }
        }

        public void UpdateIdentity(List<IdentityViewModel> modelList, Int64 EmployeeKey)
        {
            foreach (IdentityViewModel model in modelList)
            {
                EmployeeIdentity employeeIdentityModel = new EmployeeIdentity();
                employeeIdentityModel = dbContext.EmployeeIdentities.SingleOrDefault(row => row.RowKey == model.RowKey);
                employeeIdentityModel.IdentityTypeKey = model.IdentityTypeKey;
                employeeIdentityModel.IdentyUniqueID = model.IdentyUniqueID;
                employeeIdentityModel.IdentityIssuedDate = model.IdentityIssuedDate;
                employeeIdentityModel.IdentityExpiryDate = model.IdentityExpiryDate;
                if (model.AttanchedFile != null)
                {
                    employeeIdentityModel.AttanchedFileName = employeeIdentityModel.RowKey + model.AttanchedFileName;
                }
            }
        }

        public EmployeeIdentityViewModel DeleteEmployeeIdentity(IdentityViewModel model)
        {
            EmployeeIdentityViewModel employeeIdentityModel = new EmployeeIdentityViewModel();
            FillDropdownList(employeeIdentityModel.EmployeeIdentities);
            using (var transaction = dbContext.Database.BeginTransaction())
            {
                try
                {
                    EmployeeIdentity employeeIdentity = dbContext.EmployeeIdentities.SingleOrDefault(row => row.RowKey == model.RowKey);
                    employeeIdentityModel.EmployeeKey = employeeIdentity.EmployeeKey;
                    employeeIdentityModel.EmployeeIdentities.Add(new IdentityViewModel { AttanchedFileNamePath = employeeIdentity.AttanchedFileName });
                    dbContext.EmployeeIdentities.Remove(employeeIdentity);
                    dbContext.SaveChanges();
                    transaction.Commit();
                    employeeIdentityModel.Message = EduSuiteUIResources.Success;
                    employeeIdentityModel.IsSuccessful = true;
                    ActivityLog.CreateActivityLog(MenuConstants.EmployeeIdentity, ActionConstants.Delete, DbConstants.LogType.Info, model.RowKey, employeeIdentityModel.Message);
                }
                catch (DbUpdateException ex)
                {
                    transaction.Rollback();
                    if (ex.GetBaseException().Message.ToUpper().Contains(EduSuiteUIResources.ForeignKeyError.ToUpper()))
                    {
                        employeeIdentityModel.Message = String.Format(EduSuiteUIResources.CantDeleteErrorMessage, EduSuiteUIResources.EmployeeIdentity);
                        employeeIdentityModel.IsSuccessful = false;
                        ActivityLog.CreateActivityLog(MenuConstants.EmployeeIdentity, ActionConstants.Delete, DbConstants.LogType.Debug, model.RowKey, ex.GetBaseException().Message);
                    }
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    employeeIdentityModel.Message = String.Format(EduSuiteUIResources.FailedToDeleteErrorMessage, EduSuiteUIResources.EmployeeIdentity);
                    employeeIdentityModel.IsSuccessful = false;
                    ActivityLog.CreateActivityLog(MenuConstants.EmployeeIdentity, ActionConstants.Delete, DbConstants.LogType.Error, model.RowKey, ex.GetBaseException().Message);
                }
            }
            return employeeIdentityModel;
        }

        public EmployeeIdentityViewModel CheckIdentityTypeExists(Int16 IdentityTypeKey, Int64 EmployeeKey, Int64 RowKey)
        {
            EmployeeIdentityViewModel model = new EmployeeIdentityViewModel();
            if (dbContext.EmployeeIdentities.Where(row => row.IdentityTypeKey == IdentityTypeKey && row.EmployeeKey == EmployeeKey && row.RowKey != RowKey).Any())
            {
                model.IsSuccessful = false;

            }
            else
            {
                model.IsSuccessful = true;
            }
            return model;
        }

        public EmployeeIdentityViewModel CheckAdharNumberExists(string IdentyUniqueID, Int64 RowKey)
        {
            EmployeeIdentityViewModel model = new EmployeeIdentityViewModel();
            if (dbContext.EmployeeIdentities.Where(row => row.IdentyUniqueID == IdentyUniqueID && row.IdentityTypeKey == DbConstants.IdentityType.AdharNumber && row.RowKey != RowKey).Any())
            {
                model.IsSuccessful = false;

            }
            else
            {
                model.IsSuccessful = true;
            }
            return model;
        }

        private void FillDropdownList(List<IdentityViewModel> modelList)
        {
            modelList.ForEach(x => FillIdentityTypes(x));
        }
        private void FillIdentityTypes(IdentityViewModel model)
        {
            model.IdentityTypes = dbContext.VwIdentityTypeSelectActiveOnlies.OrderBy(row => row.DisplayOrder).Select(row => new SelectListModel
            {
                RowKey = row.RowKey,
                Text = row.IdentityTypeName
            }).ToList();
        }
    }
}
