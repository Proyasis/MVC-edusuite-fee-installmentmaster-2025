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
    public class EmployeeTaskService : IEmployeeTaskService
    {
        private EduSuiteDatabase dbContext;
        public EmployeeTaskService(EduSuiteDatabase objDB)
        {
            this.dbContext = objDB;
        }
        public List<EmployeeTaskViewModel> GetEmployeeTasks(EmployeeTaskViewModel model)
        {

            try
            {
                var employeeTaskList = (from et in dbContext.EmployeeTasks
                                        orderby et.RowKey descending
                                        select new EmployeeTaskViewModel
                                        {
                                            RowKey = et.RowKey,
                                            BranchKey = et.Employee.BranchKey,
                                            EmployeeKey = et.EmployeeKey,
                                            EmployeeName = et.Employee.FirstName + " " + (et.Employee.MiddleName != null ? et.Employee.MiddleName : "") + " " + et.Employee.LastName,
                                            TaskTitle = et.TaskTitle,
                                            StartDate = et.StartDate,
                                            EndDate = et.EndDate,
                                            PriorityKey = et.PriorityKey,
                                            TaskStatusKey = et.TaskStatusKey,
                                            PriorityName = et.Priority.PriorityName,
                                            TaskStatusName = et.TaskStatu.TaskStatusName,
                                            ApprovalStatusKey = et.ApprovalStatusKey,
                                            ApprovalStatusName = et.ProcessStatu.ProcessStatusName,

                                        }).ToList();


                if (model.EmployeeKey != 0)
                {
                    employeeTaskList = employeeTaskList.Where(row => row.EmployeeKey == model.EmployeeKey).ToList();
                }
                if (model.BranchKey != 0)
                {
                    employeeTaskList = employeeTaskList.Where(row => row.BranchKey == model.BranchKey).ToList();
                }
                return employeeTaskList.GroupBy(x => x.RowKey).Select(y => y.First()).ToList<EmployeeTaskViewModel>();
            }
            catch (Exception ex)
            {
                ActivityLog.CreateActivityLog(MenuConstants.EmployeeTask, ActionConstants.View, DbConstants.LogType.Error, model.EmployeeKey, ex.GetBaseException().Message);
                return new List<EmployeeTaskViewModel>();
              

            }

        }

        public EmployeeTaskViewModel GetEmployeeTaskById(EmployeeTaskViewModel model)
        {
            try
            {
                EmployeeTaskViewModel employeeTaskViewModel = new EmployeeTaskViewModel();
                employeeTaskViewModel = dbContext.EmployeeTasks.Select(row => new EmployeeTaskViewModel
                {
                    RowKey = row.RowKey,
                    BranchKey = row.Employee.BranchKey,
                    EmployeeKey = row.EmployeeKey,
                    TaskTitle = row.TaskTitle,
                    StartDate = row.StartDate,
                    EndDate = row.EndDate,
                    PriorityKey = row.PriorityKey,
                    TaskStatusKey = row.TaskStatusKey,
                    ApprovalStatusKey = row.ApprovalStatusKey,


                }).Where(x => x.RowKey == model.RowKey).FirstOrDefault();
                if (employeeTaskViewModel == null)
                {
                    employeeTaskViewModel = new EmployeeTaskViewModel();
                    employeeTaskViewModel.EmployeeKey = model.EmployeeKey;
                    employeeTaskViewModel.BranchKey = model.BranchKey;
                }
                FillDropDownList(employeeTaskViewModel);
                return employeeTaskViewModel;
            }
            catch (Exception ex)
            {
                ActivityLog.CreateActivityLog(MenuConstants.EmployeeTask, ActionConstants.View, DbConstants.LogType.Error, model.EmployeeKey, ex.GetBaseException().Message);
                return new EmployeeTaskViewModel();
               

            }
        }

        public EmployeeTaskViewModel CreateEmployeeTask(EmployeeTaskViewModel model)
        {
            FillDropDownList(model);
            EmployeeTask employeeTaskModel = new EmployeeTask();

            using (var transaction = dbContext.Database.BeginTransaction())
            {
                try
                {
                    Int64 maxKey = dbContext.EmployeeTasks.Select(p => p.RowKey).DefaultIfEmpty().Max();

                    employeeTaskModel.RowKey = Convert.ToInt64(maxKey + 1);
                    employeeTaskModel.EmployeeKey = model.EmployeeKey;
                    employeeTaskModel.TaskTitle = model.TaskTitle;
                    employeeTaskModel.StartDate = Convert.ToDateTime(model.StartDate);
                    employeeTaskModel.EndDate = model.EndDate;
                    employeeTaskModel.PriorityKey = model.PriorityKey;
                    employeeTaskModel.TaskStatusKey = model.TaskStatusKey;
                    employeeTaskModel.ApprovalStatusKey = DbConstants.ProcessStatus.Pending;


                    dbContext.EmployeeTasks.Add(employeeTaskModel);

                    dbContext.SaveChanges();
                    transaction.Commit();
                    model.RowKey = employeeTaskModel.RowKey;
                    model.Message = EduSuiteUIResources.Success;
                    model.IsSuccessful = true;
                    ActivityLog.CreateActivityLog(MenuConstants.EmployeeTask, ActionConstants.Add, DbConstants.LogType.Info, model.RowKey, model.Message);

                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    model.Message = String.Format(EduSuiteUIResources.FailedToSaveErrorMessage, EduSuiteUIResources.EmployeeTask);
                    model.IsSuccessful = false;
                    ActivityLog.CreateActivityLog(MenuConstants.EmployeeTask, ActionConstants.Add, DbConstants.LogType.Error, model.RowKey, ex.GetBaseException().Message);
                }
            }
            return model;
        }

        public EmployeeTaskViewModel UpdateEmployeeTask(EmployeeTaskViewModel model)
        {
            EmployeeTask employeeTaskModel = new EmployeeTask();
            FillDropDownList(model);
            using (var transaction = dbContext.Database.BeginTransaction())
            {
                try
                {
                    employeeTaskModel = dbContext.EmployeeTasks.SingleOrDefault(row => row.RowKey == model.RowKey);
                    employeeTaskModel.EmployeeKey = model.EmployeeKey;
                    employeeTaskModel.TaskTitle = model.TaskTitle;
                    employeeTaskModel.StartDate = Convert.ToDateTime(model.StartDate);
                    employeeTaskModel.EndDate = model.EndDate;
                    employeeTaskModel.PriorityKey = model.PriorityKey;
                    employeeTaskModel.TaskStatusKey = model.TaskStatusKey;

                    dbContext.SaveChanges();
                    transaction.Commit();
                    model.Message = EduSuiteUIResources.Success;
                    model.IsSuccessful = true;
                    ActivityLog.CreateActivityLog(MenuConstants.EmployeeTask, ActionConstants.Edit, DbConstants.LogType.Info, model.EmployeeKey, model.Message);

                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    model.Message = String.Format(EduSuiteUIResources.FailedToSaveErrorMessage, EduSuiteUIResources.EmployeeTask);
                    model.IsSuccessful = false;
                    ActivityLog.CreateActivityLog(MenuConstants.EmployeeTask, ActionConstants.Edit, DbConstants.LogType.Error, model.EmployeeKey, ex.GetBaseException().Message);
                }
            }
            return model;
        }

        public EmployeeTaskViewModel DeleteEmployeeTask(EmployeeTaskViewModel model)
        {
            using (var transaction = dbContext.Database.BeginTransaction())
            {
                try
                {
                    EmployeeTask employeeTask = dbContext.EmployeeTasks.SingleOrDefault(row => row.RowKey == model.RowKey);
                    dbContext.EmployeeTasks.Remove(employeeTask);
                    dbContext.SaveChanges();
                    transaction.Commit();
                    model.Message = EduSuiteUIResources.Success;
                    model.IsSuccessful = true;
                    ActivityLog.CreateActivityLog(MenuConstants.EmployeeTask, ActionConstants.Delete, DbConstants.LogType.Info, model.RowKey, model.Message);

                }
                catch (DbUpdateException ex)
                {
                    transaction.Rollback();
                    if (ex.GetBaseException().Message.ToUpper().Contains(EduSuiteUIResources.ForeignKeyError.ToUpper()))
                    {
                        model.Message = String.Format(EduSuiteUIResources.CantDeleteErrorMessage, EduSuiteUIResources.EmployeeTask);
                        model.IsSuccessful = false;
                        ActivityLog.CreateActivityLog(MenuConstants.EmployeeTask, ActionConstants.Delete, DbConstants.LogType.Error, model.RowKey, ex.GetBaseException().Message);
                    }
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    model.Message = String.Format(EduSuiteUIResources.FailedToDeleteErrorMessage, EduSuiteUIResources.EmployeeTask);
                    model.IsSuccessful = false;
                    ActivityLog.CreateActivityLog(MenuConstants.EmployeeTask, ActionConstants.Delete, DbConstants.LogType.Error, model.RowKey, ex.GetBaseException().Message);
                }
            }
            return model;
        }

        private void FillDropDownList(EmployeeTaskViewModel model)
        {
            GetEmployees(model);
            GetPriorities(model);
            GetTaskStatuses(model);
            GetApproveStatuses(model);
            GetBranches(model);
            GetEmployeesByBranchId(model);
        }

        public EmployeeTaskViewModel GetBranches(EmployeeTaskViewModel model)
        {

            model.Branches = dbContext.vwBranchSelectActiveOnlies.OrderBy(row => row.DisplayOrder).Select(row => new SelectListModel
            {
                RowKey = row.RowKey,
                Text = row.BranchName
            }).ToList();

            return model;
        }
        public EmployeeTaskViewModel GetEmployeesByBranchId(EmployeeTaskViewModel model)
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

        private EmployeeTaskViewModel GetApproveStatuses(EmployeeTaskViewModel model)
        {
            model.ApproveStatuses = dbContext.VwProcessStatusSelectActiveOnlies.Select(row => new SelectListModel
                {
                    RowKey = row.RowKey,
                    Text = row.ProcessStatusName
                }).OrderBy(row => row.Text).ToList();
            return model;
        }

        private EmployeeTaskViewModel GetTaskStatuses(EmployeeTaskViewModel model)
        {
            model.TaskStatuses = dbContext.VwTaskStatusSelectActiveOnlies.Select(row => new SelectListModel
                {
                    RowKey = row.RowKey,
                    Text = row.TaskStatusName
                }).OrderBy(row => row.Text).ToList();
            return model;
        }

        private EmployeeTaskViewModel GetPriorities(EmployeeTaskViewModel model)
        {
            model.Priorities = dbContext.VwPrioritySelectActiveOnlies.Select(row => new SelectListModel
               {
                   RowKey = row.RowKey,
                   Text = row.PriorityName
               }).OrderBy(row => row.Text).ToList();
            return model;
        }

        public EmployeeTaskViewModel GetEmployees(EmployeeTaskViewModel model)
        {
            model.Employees = dbContext.Employees.Select(row => new GroupSelectListModel
            {
                RowKey = row.RowKey,
                Text = row.FirstName + " " + (row.MiddleName ?? "") + " " + row.LastName,
                GroupKey = row.DepartmentKey,
                GroupName = row.Department.DepartmentName
            }).OrderBy(row => row.Text).ToList();

            return model;
        }


    }
}
