using CITS.EduSuite.Business.Interfaces;
using CITS.EduSuite.Business.Models.Resources;
using CITS.EduSuite.Business.Models.ViewModels;
using CITS.EduSuite.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CITS.EduSuite.Business.Services
{
    public class EmployeeHeirarchyService : IEmployeeHeirarchyService
    {
        private EduSuiteDatabase dbContext;
        public EmployeeHeirarchyService(EduSuiteDatabase objDB)
        {
            this.dbContext = objDB;
        }
        public EmplyeeHeirarchyViewModel GetEmployeeHeirarchyById(long? id)
        {
            try
            {
                EmplyeeHeirarchyViewModel model = new EmplyeeHeirarchyViewModel();
                Employee employee = dbContext.Employees.FirstOrDefault(row => row.RowKey == id);
                List<long> Branches = new List<long>();
                if (employee != null)
                {
                    model.EmployeeName = employee.FirstName + " " + (employee.MiddleName ?? "") + " " + employee.LastName;

                    model.EmployeeKey = id ?? 0;
                    if (employee.BranchAccess != null)
                    {
                        Branches = employee.BranchAccess.Split(',').Select(Int64.Parse).ToList();
                    }
                    else
                    {
                        Branches.Add(employee.BranchKey);
                    }
                }

                model.EmployeeHeirarchyDetails =
                    (from E in dbContext.Employees.Where(y => y.RowKey != id && Branches.Contains(y.BranchKey))
                     join EH in dbContext.EmployeeHierarchies.Where(x => x.EmployeeKey == id)
                     on new { ToEmployeeKey = E.RowKey } equals new { ToEmployeeKey = EH.ToEmployeeKey ?? 0 }
                     into UPL
                     from UP in UPL.DefaultIfEmpty()
                     select new EmployeeHeirarchyDetailsModel
                     {
                         RowKey = UP.RowKey,
                         ToEmployeeKey = UP.RowKey != null ? UP.ToEmployeeKey : E.RowKey,
                         ToEmployeeName = E.FirstName + " " + (E.MiddleName ?? "") + " " + E.LastName,
                         DataAccess = UP.RowKey != null ? UP.DataAccess : false,
                         IsActive = UP.RowKey != null ? UP.IsActive : false,
                     }).ToList();
                return model;
            }
            catch (Exception)
            {
                return new EmplyeeHeirarchyViewModel();

            }
        }

        public EmplyeeHeirarchyViewModel UpdateEmplyeeHeirarchyPermission(EmplyeeHeirarchyViewModel model)
        {
            using (var transaction = dbContext.Database.BeginTransaction())
            {
                try
                {
                    DeleteEmplyeeHeirarchy(model.EmployeeHeirarchyDetails.Where(row => row.RowKey != null && row.IsActive == false).ToList());
                    UpdateEmplyeeHeirarchy(model.EmployeeHeirarchyDetails.Where(row => row.RowKey != null && row.IsActive == true).ToList());
                    CreateEmplyeeHeirarchy(model.EmployeeHeirarchyDetails.Where(row => row.RowKey == null && row.IsActive == true).ToList(), model.EmployeeKey);

                    dbContext.SaveChanges();
                    transaction.Commit();

                    model.Message = EduSuiteUIResources.Success;
                    model.IsSuccessful = true;
                    ActivityLog.CreateActivityLog(MenuConstants.EmployeeHeirarchy, ActionConstants.AddEdit, DbConstants.LogType.Info, model.EmployeeKey, model.Message);

                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    model.Message = String.Format(EduSuiteUIResources.FailedToSaveErrorMessage, EduSuiteUIResources.Agent);
                    model.IsSuccessful = false;
                    ActivityLog.CreateActivityLog(MenuConstants.EmployeeHeirarchy, ActionConstants.AddEdit, DbConstants.LogType.Error, model.EmployeeKey, ex.GetBaseException().Message);
                }
            }

            return model;
        }

        #region Emplyee Heirarchy Permission

        private void CreateEmplyeeHeirarchy(List<EmployeeHeirarchyDetailsModel> modelList, long? EmployeeKey)
        {
            try
            {
                long maxKey = dbContext.EmployeeHierarchies.Select(p => p.RowKey).DefaultIfEmpty().Max();
                foreach (EmployeeHeirarchyDetailsModel modelItem in modelList)
                {
                    EmployeeHierarchy employeeHierarchy = new EmployeeHierarchy();
                    employeeHierarchy.RowKey = ++maxKey;
                    employeeHierarchy.EmployeeKey = EmployeeKey ?? 0;
                    employeeHierarchy.ToEmployeeKey = modelItem.ToEmployeeKey;
                    employeeHierarchy.DataAccess = modelItem.DataAccess;
                    employeeHierarchy.IsActive = modelItem.IsActive;

                    dbContext.EmployeeHierarchies.Add(employeeHierarchy);
                    modelItem.RowKey = employeeHierarchy.RowKey;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        private void UpdateEmplyeeHeirarchy(List<EmployeeHeirarchyDetailsModel> modelList)
        {
            try
            {
                foreach (EmployeeHeirarchyDetailsModel modelItem in modelList)
                {
                    EmployeeHierarchy employeeHierarchy = dbContext.EmployeeHierarchies.SingleOrDefault(p => p.RowKey == modelItem.RowKey);
                    employeeHierarchy.ToEmployeeKey = modelItem.ToEmployeeKey;
                    employeeHierarchy.DataAccess = modelItem.DataAccess;
                    employeeHierarchy.IsActive = modelItem.IsActive;
                    modelItem.RowKey = employeeHierarchy.RowKey;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        private void DeleteEmplyeeHeirarchy(List<EmployeeHeirarchyDetailsModel> modelList)
        {
            foreach (EmployeeHeirarchyDetailsModel model in modelList)
            {
                try
                {
                    EmployeeHierarchy employeeHierarchy = dbContext.EmployeeHierarchies.SingleOrDefault(row => row.RowKey == model.RowKey);
                    if (employeeHierarchy != null)
                    {
                        dbContext.EmployeeHierarchies.Remove(employeeHierarchy);
                        dbContext.SaveChanges();
                    }
                }
                catch (Exception)
                {
                }
            }

        }

        #endregion
    }
}
