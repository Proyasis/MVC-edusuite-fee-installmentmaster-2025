using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CITS.EduSuite.Business.Interfaces;
using CITS.EduSuite.Business.Common;
using CITS.EduSuite.Business.Models.ViewModels;
using CITS.EduSuite.Data;
using System.Data.Entity.Infrastructure;
using CITS.EduSuite.Business.Models.Resources;

namespace CITS.EduSuite.Business.Services
{
    public class SalaryComponentService : ISalaryComponentService
    {
        private EduSuiteDatabase dbContext;

        public SalaryComponentService(EduSuiteDatabase objDB)
        {
            this.dbContext = objDB;
        }


        public List<SalaryComponentViewModel> GetSalaryComponentsByType(SalaryComponentViewModel model)
        {
            try
            {
                var SalaryComponentList = (from Sl in dbContext.SalaryComponents
                                           orderby Sl.ComponentDate descending
                                           where Sl.ComponentTypeKey == model.SalaryComponentTypeKey
                                           select new SalaryComponentViewModel
                                           {
                                               RowKey = Sl.RowKey,
                                               BranchKey = Sl.Employee.BranchKey,
                                               EmployeeKey = Sl.EmployeeKey,
                                               EmployeeName = Sl.Employee.FirstName + " " + (Sl.Employee.MiddleName ?? "") + " " + Sl.Employee.LastName,
                                               AmountUnit = Sl.ComponentAmount,
                                               SalaryComponentDate = Sl.ComponentDate,
                                               //ComponentTitle = Sl.ComponentTitle,
                                               SalaryComponentTypeKey = Sl.ComponentTypeKey,
                                               ApprovalStatusKey = Sl.ApprovalStatusKey,
                                               ApprovalStatusName = Sl.ProcessStatu.ProcessStatusName,
                                               Remarks = Sl.Remarks

                                           }).ToList();

                if (model.EmployeeKey != 0)
                {
                    SalaryComponentList = SalaryComponentList.Where(row => row.EmployeeKey == model.EmployeeKey).ToList();
                }
                if (model.BranchKey != 0)
                {
                    SalaryComponentList = SalaryComponentList.Where(row => row.BranchKey == model.BranchKey).ToList();
                }

                return SalaryComponentList.GroupBy(x => x.RowKey).Select(y => y.First()).ToList<SalaryComponentViewModel>();
            }
            catch (Exception ex)
            {
                return new List<SalaryComponentViewModel>();
            }
        }
        public SalaryComponentViewModel GetSalaryComponentById(SalaryComponentViewModel model)
        {
            try
            {

                SalaryComponentViewModel salaryComponentViewModel = new SalaryComponentViewModel();

                salaryComponentViewModel = dbContext.SalaryComponents.Select(row => new SalaryComponentViewModel
                {
                    RowKey = row.RowKey,
                    BranchKey = row.Employee.BranchKey,
                    //ComponentTitle = row.ComponentTitle,
                    SalaryComponentTypeKey = row.ComponentTypeKey,
                    SalaryComponentTypeName = row.ComponentType.ComponentTypeName,
                    EmployeeKey = row.EmployeeKey,
                    AmountUnit = row.ComponentAmount,
                    OperationType = row.ComponentType.OperationType,
                    SalaryComponentDate = row.ComponentDate,
                    Remarks = row.Remarks

                }).Where(x => x.RowKey == model.RowKey).FirstOrDefault();
                if (salaryComponentViewModel == null)
                {
                    salaryComponentViewModel = dbContext.ComponentTypes.Where(row => row.RowKey == model.SalaryComponentTypeKey).Select(row => new SalaryComponentViewModel
                    {
                        SalaryComponentTypeName = row.ComponentTypeName,
                        SalaryComponentTypeKey = row.RowKey,
                        OperationType = row.OperationType,
                        EmployeeKey = model.EmployeeKey
                    }).SingleOrDefault();

                }
                FillDropdownList(salaryComponentViewModel);
                return salaryComponentViewModel;
            }
            catch (Exception ex)
            {
                return new SalaryComponentViewModel();
            }
        }

        public SalaryComponentViewModel CreateSalaryComponent(SalaryComponentViewModel model)
        {
            SalaryComponent salaryComponentModel = new SalaryComponent();
            FillDropdownList(model);
            using (var transaction = dbContext.Database.BeginTransaction())
            {

                try
                {
                    Int64 maxKey = dbContext.SalaryComponents.Select(p => p.RowKey).DefaultIfEmpty().Max();
                    salaryComponentModel.RowKey = Convert.ToInt64(maxKey + 1);
                    salaryComponentModel.EmployeeKey = model.EmployeeKey;
                    salaryComponentModel.ComponentTypeKey = model.SalaryComponentTypeKey;
                    salaryComponentModel.ComponentDate = Convert.ToDateTime(model.SalaryComponentDate);
                    salaryComponentModel.ComponentAmount = Convert.ToDecimal(model.AmountUnit);
                    //salaryComponentModel.ComponentTitle = model.ComponentTitle;
                    salaryComponentModel.ApprovalStatusKey = DbConstants.ProcessStatus.Pending;
                    salaryComponentModel.Remarks = model.Remarks;
                    dbContext.SalaryComponents.Add(salaryComponentModel);
                    dbContext.SaveChanges();
                    transaction.Commit();
                    model.Message = EduSuiteUIResources.Success;
                    model.IsSuccessful = true;
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    model.Message = String.Format(EduSuiteUIResources.FailedToSaveErrorMessage, EduSuiteUIResources.SalaryComponent);
                    model.IsSuccessful = false;
                }
            }
            return model;
        }

        public SalaryComponentViewModel UpdateSalaryComponent(SalaryComponentViewModel model)
        {
            SalaryComponent salaryComponentModel = new SalaryComponent();
            FillDropdownList(model);
            using (var transaction = dbContext.Database.BeginTransaction())
            {

                try
                {
                    salaryComponentModel = dbContext.SalaryComponents.SingleOrDefault(p => p.RowKey == model.RowKey);

                    salaryComponentModel.ComponentDate = Convert.ToDateTime(model.SalaryComponentDate);
                    salaryComponentModel.ComponentAmount = Convert.ToDecimal(model.AmountUnit);
                    salaryComponentModel.Remarks = model.Remarks;

                    dbContext.SaveChanges();
                    transaction.Commit();
                    model.Message = EduSuiteUIResources.Success;
                    model.IsSuccessful = true;
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    model.Message = String.Format(EduSuiteUIResources.FailedToSaveErrorMessage, EduSuiteUIResources.SalaryComponent);
                    model.IsSuccessful = false;
                }
            }
            return model;
        }

        public SalaryComponentViewModel DeleteSalaryComponent(SalaryComponentViewModel model)
        {
            using (var transaction = dbContext.Database.BeginTransaction())
            {
                try
                {
                    SalaryComponent salaryComponent = dbContext.SalaryComponents.SingleOrDefault(row => row.RowKey == model.RowKey);
                    if (salaryComponent.EmployeeSalaryKey == null)
                    {
                        dbContext.SalaryComponents.Remove(salaryComponent);
                        dbContext.SaveChanges();
                        transaction.Commit();
                        model.Message = EduSuiteUIResources.Success;
                        model.IsSuccessful = true;
                    }
                    else
                    {
                        model.Message = String.Format(EduSuiteUIResources.CantDeleteErrorMessage, EduSuiteUIResources.SalaryComponent);
                        model.IsSuccessful = false;
                    }



                }
                catch (DbUpdateException ex)
                {
                    transaction.Rollback();
                    if (ex.GetBaseException().Message.ToUpper().Contains(EduSuiteUIResources.ForeignKeyError.ToUpper()))
                    {
                        model.Message = String.Format(EduSuiteUIResources.CantDeleteErrorMessage, EduSuiteUIResources.SalaryComponent);
                        model.IsSuccessful = false;
                    }
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    model.Message = String.Format(EduSuiteUIResources.FailedToDeleteErrorMessage, EduSuiteUIResources.SalaryComponent);
                    model.IsSuccessful = false;
                }
            }
            return model;
        }

        public SalaryComponentViewModel GetComponentTypes(SalaryComponentViewModel model)
        {
            FillComponentTypes(model);
            return model;
        }
        public SalaryComponentViewModel GetBranches(SalaryComponentViewModel model)
        {

            model.Branches = dbContext.vwBranchSelectActiveOnlies.OrderBy(row => row.DisplayOrder).Select(row => new SelectListModel
            {
                RowKey = row.RowKey,
                Text = row.BranchName
            }).ToList();

            return model;
        }
        public SalaryComponentViewModel GetEmployeesByBranchId(SalaryComponentViewModel model)
        {
            model.Employees = dbContext.Employees.Where(row => row.BranchKey == model.BranchKey).Select(row => new GroupSelectListModel
            {
                RowKey = row.RowKey,
                Text = row.FirstName + " " + (row.MiddleName ?? "") + " " + row.LastName,
                GroupKey = row.DepartmentKey,
                GroupName = row.Department.DepartmentName
            }).OrderBy(row => row.Text).ToList();

            return model;
        }

        private void FillComponentTypes(SalaryComponentViewModel model)
        {
            model.SalaryComponentTypes = dbContext.ComponentTypes.Where(x=>x.IsActive).OrderBy(row => row.DisplayOrder).Select(row => new GroupSelectListModel
            {
                RowKey = row.RowKey,
                GroupName = row.OperationType,
                Text = row.ComponentTypeName
            }).ToList();
        }

        private void FillDropdownList(SalaryComponentViewModel model)
        {
            FillComponentTypes(model);
            GetBranches(model);
            GetEmployeesByBranchId(model);
        }

    }
}
