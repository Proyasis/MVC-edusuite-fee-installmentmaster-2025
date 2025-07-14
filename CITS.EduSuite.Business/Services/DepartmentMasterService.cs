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
    public class DepartmentMasterService : IDepartmentMasterService
    {
        private EduSuiteDatabase dbContext;
        public DepartmentMasterService(EduSuiteDatabase objDB)
        {
            this.dbContext = objDB;
        }
        public List<DepartmentViewModel> GetDepartment(string searchText)
        {
            try
            {
                var DepartmentList = (from p in dbContext.Departments
                                      orderby p.RowKey descending
                                      where (p.DepartmentName.Contains(searchText))
                                      select new DepartmentViewModel
                                      {
                                          RowKey = p.RowKey,
                                          DepartmentName = p.DepartmentName,
                                          DepartmentCode = p.DepartmentCode,
                                          // IsActiveText = p.IsActive == true ? ApplicationResources.Yes : ApplicationResources.No
                                          IsActive = p.IsActive,
                                      }).ToList();
                return DepartmentList.GroupBy(x => x.RowKey).Select(y => y.First()).ToList<DepartmentViewModel>();
            }
            catch (Exception ex)
            {
                ActivityLog.CreateActivityLog(MenuConstants.Department, ActionConstants.View, DbConstants.LogType.Error, null, ex.GetBaseException().Message);
                return new List<DepartmentViewModel>();

            }
        }
        public DepartmentViewModel GetDepartmentById(int? id)
        {
            try
            {
                DepartmentViewModel model = new DepartmentViewModel();
                model = dbContext.Departments.Select(row => new DepartmentViewModel
                {
                    RowKey = row.RowKey,
                    DepartmentName = row.DepartmentName,
                    DepartmentCode = row.DepartmentCode,
                    IsActive = row.IsActive
                }).Where(x => x.RowKey == id).FirstOrDefault();
                if (model == null)
                {
                    model = new DepartmentViewModel();
                }
                return model;
            }
            catch (Exception ex)
            {
                ActivityLog.CreateActivityLog(MenuConstants.Department, ActionConstants.View, DbConstants.LogType.Error, id, ex.GetBaseException().Message);
                return new DepartmentViewModel();

            }
        }
        public DepartmentViewModel CreateDepartment(DepartmentViewModel model)
        {
            Department DepartmentModel = new Department();
            using (var transaction = dbContext.Database.BeginTransaction())
            {
                try
                {
                    long MaxKey = dbContext.Departments.Select(p => p.RowKey).DefaultIfEmpty().Max();
                    long DisplayOrderMax = dbContext.Departments.Select(p => p.DisplayOrder).DefaultIfEmpty().Max();
                    
                    DepartmentModel.RowKey = Convert.ToInt16(MaxKey + 1);
                    DepartmentModel.DepartmentName = model.DepartmentName;
                    DepartmentModel.DepartmentCode = model.DepartmentCode;
                    DepartmentModel.DateAdded = DateTimeUTC.Now;
                    DepartmentModel.DisplayOrder = Convert.ToByte(DisplayOrderMax + 1);
                    DepartmentModel.IsActive = model.IsActive;
                    dbContext.Departments.Add(DepartmentModel);
                    dbContext.SaveChanges();
                    transaction.Commit();
                    model.Message = EduSuiteUIResources.Success;
                    model.IsSuccessful = true;

                    ActivityLog.CreateActivityLog(MenuConstants.Department, ActionConstants.Add, DbConstants.LogType.Info, DepartmentModel.RowKey, model.Message);


                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    model.Message = String.Format(EduSuiteUIResources.FailedToSaveErrorMessage, EduSuiteUIResources.Department);
                    model.IsSuccessful = false;

                    ActivityLog.CreateActivityLog(MenuConstants.Department, ActionConstants.Add, DbConstants.LogType.Error, model.RowKey, ex.GetBaseException().Message);
                }
            }
            return model;
        }
        public DepartmentViewModel UpdateDepartment(DepartmentViewModel model)
        {
            Department DepartmentModel = new Department();
            using (var transaction = dbContext.Database.BeginTransaction())
            {
                try
                {
                    DepartmentModel = dbContext.Departments.SingleOrDefault(x => x.RowKey == model.RowKey);
                    DepartmentModel.DepartmentName = model.DepartmentName;
                    DepartmentModel.DepartmentCode = model.DepartmentCode;
                    DepartmentModel.IsActive = model.IsActive;
                    dbContext.SaveChanges();
                    transaction.Commit();
                    model.Message = EduSuiteUIResources.Success;
                    model.IsSuccessful = true;

                    ActivityLog.CreateActivityLog(MenuConstants.Department, ActionConstants.Edit, DbConstants.LogType.Info, model.RowKey, model.Message);

                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    model.Message = String.Format(EduSuiteUIResources.FailedToSaveErrorMessage, EduSuiteUIResources.Department);
                    model.IsSuccessful = false;

                    ActivityLog.CreateActivityLog(MenuConstants.Department, ActionConstants.Edit, DbConstants.LogType.Error, model.RowKey, ex.GetBaseException().Message);
                }
            }
            return model;

        }
        public DepartmentViewModel DeleteDepartment(DepartmentViewModel model)
        {
            using (var transaction = dbContext.Database.BeginTransaction())
            {
                try
                {
                    Department DepartmentModel = dbContext.Departments.SingleOrDefault(row => row.RowKey == model.RowKey);
                    dbContext.Departments.Remove(DepartmentModel);
                    dbContext.SaveChanges();
                    transaction.Commit();
                    model.Message = EduSuiteUIResources.Success;
                    model.IsSuccessful = true;
                    ActivityLog.CreateActivityLog(MenuConstants.Department, ActionConstants.Delete, DbConstants.LogType.Info, model.RowKey, model.Message);


                }
                catch (DbUpdateException ex)
                {
                    transaction.Rollback();
                    if (ex.GetBaseException().Message.ToUpper().Contains(EduSuiteUIResources.ForeignKeyError.ToUpper()))
                    {
                        model.Message = String.Format(EduSuiteUIResources.CantDeleteErrorMessage, EduSuiteUIResources.Department);
                        model.IsSuccessful = false;

                        ActivityLog.CreateActivityLog(MenuConstants.Department, ActionConstants.Delete, DbConstants.LogType.Error, model.RowKey, ex.GetBaseException().Message);
                    }
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    model.Message = String.Format(EduSuiteUIResources.FailedToDeleteErrorMessage, EduSuiteUIResources.Department);
                    model.IsSuccessful = false;

                    ActivityLog.CreateActivityLog(MenuConstants.Department, ActionConstants.Delete, DbConstants.LogType.Error, model.RowKey, ex.GetBaseException().Message);
                }
            }

            return model;
        }
     
        public DepartmentViewModel CheckDepartmentCodeExists(DepartmentViewModel model)
        {
            if (dbContext.Departments.Where(x => x.DepartmentCode.ToLower() == model.DepartmentCode.ToLower() && x.RowKey != model.RowKey).Any())
            {
                model.IsSuccessful = false;
                model.Message = String.Format(EduSuiteUIResources.ExistsErrorMessage, EduSuiteUIResources.Department);
            }
            else
            {
                model.IsSuccessful = true;
                model.Message = "";
            }
            return model;
        }
        public DepartmentViewModel CheckDepartmentNameExists(DepartmentViewModel model)
        {
            if (dbContext.Departments.Where(x => x.DepartmentName.ToLower() == model.DepartmentName.ToLower() && x.RowKey != model.RowKey).Any())
            {
                model.IsSuccessful = false;
                model.Message = String.Format(EduSuiteUIResources.ExistsErrorMessage, EduSuiteUIResources.Department);
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
