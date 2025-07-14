using CITS.EduSuite.Data;
using CITS.EduSuite.Business.Models.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Linq.Expressions;
using CITS.EduSuite.Business.Models.Resources;
using System.Data.Entity.Infrastructure;
using CITS.EduSuite.Business.Interfaces;

namespace CITS.EduSuite.Business.Services
{
    public class EmployeeEnquiryTargetService : IEmployeeEnquiryTargetService
    {
        private EduSuiteDatabase dbContext;
        public EmployeeEnquiryTargetService(EduSuiteDatabase objDB)
        {
            this.dbContext = objDB;
        }
        public List<EmployeeEnquiryTargetViewModel> GetEmployeeList(EmployeeEnquiryTargetViewModel model, out long TotalRecords)
        {
            try
            {
                var Take = model.PageSize;
                var Skip = (model.PageIndex - 1) * model.PageSize;

                IQueryable<EmployeeEnquiryTargetViewModel> EmployeeList = (from e in dbContext.Employees
                                                                           where (e.FirstName.Contains(model.EmployeeName)) || (e.MiddleName.Contains(model.EmployeeName)) || (e.LastName.Contains(model.EmployeeName))
                                                                           select new EmployeeEnquiryTargetViewModel
                                                                           {
                                                                               EmployeeKey = e.RowKey,
                                                                               EmployeeName = e.FirstName + " " + (e.MiddleName ?? "") + " " + e.LastName,
                                                                               EmployeeCode = e.EmployeeCode,
                                                                               MobileNumber = e.MobileNumber,
                                                                               DesignationName = e.Designation.DesignationName,
                                                                               DepartmentName = e.Department.DepartmentName,
                                                                               EmployeeStatusName = e.EmployeeStatu.EmployeeStatusName,
                                                                               BranchKey = e.BranchKey,
                                                                               BranchName = e.Branch.BranchName,
                                                                               EmployeeStatusKey = e.EmployeeStatusKey,
                                                                               RowKey = e.EmployeeEnquiryTargets.Select(x => x.RowKey).FirstOrDefault(),
                                                                               CommonTarget = e.EmployeeEnquiryTargets.Select(x => x.CommonTarget ?? 0).FirstOrDefault(),
                                                                               AllowMonthlyTarget = e.EmployeeEnquiryTargets.Select(x => x.AllowMonthlyTarget).FirstOrDefault(),
                                                                               IsActive = e.EmployeeEnquiryTargets.Select(x => x.IsActive).FirstOrDefault(),
                                                                           });


                Employee Employee = dbContext.Employees.Where(x => x.AppUserKey == DbConstants.User.UserKey).SingleOrDefault();
                if (Employee != null)
                {
                    if (Employee.BranchAccess != null)
                    {
                        var Branches = Employee.BranchAccess.Split(',').Select(Int16.Parse).ToList();
                        EmployeeList = EmployeeList.Where(row => Branches.Contains(row.BranchKey ?? 0));
                    }
                }

                if (model.BranchKey != 0)
                {
                    EmployeeList = EmployeeList.Where(row => row.BranchKey == model.BranchKey);
                }
                if (model.EmployeeStatusKey != 0)
                {
                    EmployeeList = EmployeeList.Where(row => row.EmployeeStatusKey == model.EmployeeStatusKey);
                }


                EmployeeList = EmployeeList.GroupBy(x => x.EmployeeKey).Select(y => y.FirstOrDefault());
                if (model.SortBy != "")
                {
                    EmployeeList = SortApplications(EmployeeList, model.SortBy, model.SortOrder);
                }
                TotalRecords = EmployeeList.Count();
                return EmployeeList.Skip(Skip).Take(Take).ToList<EmployeeEnquiryTargetViewModel>();
            }
            catch (Exception ex)
            {
                TotalRecords = 0;
                ActivityLog.CreateActivityLog(MenuConstants.EmployeeEnquiryTarget, ActionConstants.View, DbConstants.LogType.Error, null, ex.GetBaseException().Message);
                return new List<EmployeeEnquiryTargetViewModel>();

            }
        }
        private IQueryable<EmployeeEnquiryTargetViewModel> SortApplications(IQueryable<EmployeeEnquiryTargetViewModel> Query, string SortName, string SortOrder)
        {

            string command = SortOrder == "desc" ? "OrderByDescending" : "OrderBy";
            var type = typeof(EmployeeEnquiryTargetViewModel);
            var property = type.GetProperty(SortName);
            var parameter = Expression.Parameter(type, "p");
            var propertyAccess = Expression.MakeMemberAccess(parameter, property);
            var orderByExpression = Expression.Lambda(propertyAccess, parameter);
            var resultExpression = Expression.Call(typeof(Queryable), command, new Type[] { type, property.PropertyType },
                                          Query.Expression, Expression.Quote(orderByExpression));
            return Query.Provider.CreateQuery<EmployeeEnquiryTargetViewModel>(resultExpression);

        }

        public EmployeeEnquiryTargetViewModel GetEmployeeEnquiryTargetId(EmployeeEnquiryTargetViewModel objmodel)
        {
            try
            {
                EmployeeEnquiryTargetViewModel model = new EmployeeEnquiryTargetViewModel();
                model = dbContext.EmployeeEnquiryTargets.Where(x => x.EmployeeKey == objmodel.EmployeeKey).Select(row => new EmployeeEnquiryTargetViewModel
                {
                    RowKey = row.RowKey,
                    EmployeeKey = row.EmployeeKey,
                    EmployeeName = row.Employee.FirstName + " " + (row.Employee.MiddleName ?? "") + " " + row.Employee.LastName,
                    IsActive = row.IsActive,
                    CommonTarget = row.CommonTarget ?? 0,
                    AllowMonthlyTarget = row.AllowMonthlyTarget,

                    EmployeeEnquiryTargetDetailsViewModel = dbContext.EmployeeEnquiryTargetDetails.Where(x => x.EnquiryTargetKey == row.RowKey).Select(x => new EmployeeEnquiryTargetDetailsViewModel
                    {
                        RowKey = x.RowKey,
                        EnquiryTargetKey = x.EnquiryTargetKey,
                        MonthlyTarget = x.MonthlyTarget,
                        TargetYear = x.TargetYear ?? 0,
                        TargetMonth = x.TargetMonth ?? 0,
                        Remarks = x.Remarks
                    }).ToList()
                }).FirstOrDefault();
                if (model == null)
                {
                    model = new EmployeeEnquiryTargetViewModel();
                    model.EmployeeKey = objmodel.EmployeeKey;
                }
                if (model.EmployeeEnquiryTargetDetailsViewModel.Count() == 0)
                {
                    model.EmployeeEnquiryTargetDetailsViewModel.Add(new EmployeeEnquiryTargetDetailsViewModel());
                }
                FillTargetMonth(model);

                return model;
            }

            catch (Exception ex)
            {
                ActivityLog.CreateActivityLog(MenuConstants.EmployeeEnquiryTarget, ActionConstants.View, DbConstants.LogType.Error, objmodel.RowKey, ex.GetBaseException().Message);
                return new EmployeeEnquiryTargetViewModel();
            }
        }
        public EmployeeEnquiryTargetViewModel CreateEnquiryTarget(EmployeeEnquiryTargetViewModel model)
        {

            using (var transaction = dbContext.Database.BeginTransaction())
            {
                try
                {
                    EmployeeEnquiryTarget dbEmployeeEnquiryTarget = new EmployeeEnquiryTarget();

                    long Maxkey = dbContext.EmployeeEnquiryTargets.Select(p => p.RowKey).DefaultIfEmpty().Max();

                    dbEmployeeEnquiryTarget.RowKey = Maxkey + 1;
                    dbEmployeeEnquiryTarget.EmployeeKey = model.EmployeeKey;
                    dbEmployeeEnquiryTarget.IsActive = model.IsActive;
                    dbEmployeeEnquiryTarget.CommonTarget = model.CommonTarget;
                    dbEmployeeEnquiryTarget.AllowMonthlyTarget = model.AllowMonthlyTarget;

                    dbContext.EmployeeEnquiryTargets.Add(dbEmployeeEnquiryTarget);
                    model.RowKey = dbEmployeeEnquiryTarget.RowKey;
                    CreateEnquiryTargetDetails(model.EmployeeEnquiryTargetDetailsViewModel.Where(x => x.RowKey == 0).ToList(), model);
                    dbContext.SaveChanges();
                    transaction.Commit();
                    model.Message = EduSuiteUIResources.Success;
                    model.IsSuccessful = true;

                    ActivityLog.CreateActivityLog(MenuConstants.EmployeeEnquiryTarget, ActionConstants.Add, DbConstants.LogType.Info, dbEmployeeEnquiryTarget.RowKey, model.Message);
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    model.Message = String.Format(EduSuiteUIResources.FailedToSaveErrorMessage, EduSuiteUIResources.EmployeeEnquiryTarget);
                    model.IsSuccessful = false;
                    ActivityLog.CreateActivityLog(MenuConstants.EmployeeEnquiryTarget, ActionConstants.Add, DbConstants.LogType.Error, model.RowKey, ex.GetBaseException().Message);
                }
            }
            return model;
        }
        public EmployeeEnquiryTargetViewModel UpdateEnquiryTarget(EmployeeEnquiryTargetViewModel model)
        {

            using (var transaction = dbContext.Database.BeginTransaction())
            {
                try
                {
                    EmployeeEnquiryTarget dbEmployeeEnquiryTarget = new EmployeeEnquiryTarget();

                    dbEmployeeEnquiryTarget = dbContext.EmployeeEnquiryTargets.SingleOrDefault(x => x.RowKey == model.RowKey);
                    dbEmployeeEnquiryTarget.EmployeeKey = model.EmployeeKey;
                    dbEmployeeEnquiryTarget.IsActive = model.IsActive;
                    dbEmployeeEnquiryTarget.CommonTarget = model.CommonTarget;
                    dbEmployeeEnquiryTarget.AllowMonthlyTarget = model.AllowMonthlyTarget;

                    model.RowKey = dbEmployeeEnquiryTarget.RowKey;
                    CreateEnquiryTargetDetails(model.EmployeeEnquiryTargetDetailsViewModel.Where(x => x.RowKey == 0).ToList(), model);
                    UpdateEnquiryTargetDetails(model.EmployeeEnquiryTargetDetailsViewModel.Where(x => x.RowKey != 0).ToList(), model);
                    dbContext.SaveChanges();
                    transaction.Commit();
                    model.Message = EduSuiteUIResources.Success;
                    model.IsSuccessful = true;

                    ActivityLog.CreateActivityLog(MenuConstants.EmployeeEnquiryTarget, ActionConstants.Add, DbConstants.LogType.Info, dbEmployeeEnquiryTarget.RowKey, model.Message);
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    model.Message = String.Format(EduSuiteUIResources.FailedToSaveErrorMessage, EduSuiteUIResources.EmployeeEnquiryTarget);
                    model.IsSuccessful = false;
                    ActivityLog.CreateActivityLog(MenuConstants.EmployeeEnquiryTarget, ActionConstants.Add, DbConstants.LogType.Error, model.RowKey, ex.GetBaseException().Message);
                }
            }
            return model;
        }
        public void CreateEnquiryTargetDetails(List<EmployeeEnquiryTargetDetailsViewModel> modelList, EmployeeEnquiryTargetViewModel model)
        {
            long MaxKey = dbContext.EmployeeEnquiryTargetDetails.Select(x => x.RowKey).DefaultIfEmpty().Max();

            foreach (EmployeeEnquiryTargetDetailsViewModel item in modelList)
            {
                EmployeeEnquiryTargetDetail dbEmployeeEnquiryTargetDetailmodel = new EmployeeEnquiryTargetDetail();

                dbEmployeeEnquiryTargetDetailmodel.RowKey = ++MaxKey;
                dbEmployeeEnquiryTargetDetailmodel.EnquiryTargetKey = model.RowKey;
                dbEmployeeEnquiryTargetDetailmodel.TargetMonth = item.TargetMonth;
                dbEmployeeEnquiryTargetDetailmodel.TargetYear = item.TargetYear;
                dbEmployeeEnquiryTargetDetailmodel.Remarks = item.Remarks;
                dbEmployeeEnquiryTargetDetailmodel.MonthlyTarget = item.MonthlyTarget;
                dbContext.EmployeeEnquiryTargetDetails.Add(dbEmployeeEnquiryTargetDetailmodel);
            }
        }
        public void UpdateEnquiryTargetDetails(List<EmployeeEnquiryTargetDetailsViewModel> modelList, EmployeeEnquiryTargetViewModel model)
        {
            long MaxKey = dbContext.EmployeeEnquiryTargetDetails.Select(x => x.RowKey).DefaultIfEmpty().Max();

            foreach (EmployeeEnquiryTargetDetailsViewModel item in modelList)
            {
                EmployeeEnquiryTargetDetail dbEmployeeEnquiryTargetDetailmodel = new EmployeeEnquiryTargetDetail();
                dbEmployeeEnquiryTargetDetailmodel = dbContext.EmployeeEnquiryTargetDetails.Where(x => x.RowKey == item.RowKey).SingleOrDefault();
                dbEmployeeEnquiryTargetDetailmodel.EnquiryTargetKey = model.RowKey;
                dbEmployeeEnquiryTargetDetailmodel.TargetMonth = item.TargetMonth;
                dbEmployeeEnquiryTargetDetailmodel.TargetYear = item.TargetYear;
                dbEmployeeEnquiryTargetDetailmodel.Remarks = item.Remarks;
                dbEmployeeEnquiryTargetDetailmodel.MonthlyTarget = item.MonthlyTarget;

            }
        }
        public EmployeeEnquiryTargetDetailsViewModel DeleteEnquiryTargetDetails(EmployeeEnquiryTargetDetailsViewModel model)
        {
            using (var transaction = dbContext.Database.BeginTransaction())
            {
                try
                {
                    EmployeeEnquiryTargetDetail dbEmployeeEnquiryTargetDetailModel = dbContext.EmployeeEnquiryTargetDetails.SingleOrDefault(x => x.RowKey == model.RowKey);

                    dbContext.EmployeeEnquiryTargetDetails.Remove(dbEmployeeEnquiryTargetDetailModel);
                    dbContext.SaveChanges();
                    transaction.Commit();
                    model.Message = EduSuiteUIResources.Success;
                    model.IsSuccessful = true;
                    ActivityLog.CreateActivityLog(MenuConstants.EmployeeEnquiryTarget, ActionConstants.Delete, DbConstants.LogType.Info, model.RowKey, model.Message);
                }
                catch (DbUpdateException ex)
                {
                    transaction.Rollback();
                    if (ex.GetBaseException().Message.ToUpper().Contains(EduSuiteUIResources.ForeignKeyError.ToUpper()))
                    {
                        model.Message = String.Format(EduSuiteUIResources.CantDeleteErrorMessage, EduSuiteUIResources.EmployeeEnquiryTarget);
                        model.IsSuccessful = false;
                        ActivityLog.CreateActivityLog(MenuConstants.EmployeeEnquiryTarget, ActionConstants.Delete, DbConstants.LogType.Error, model.RowKey, ex.GetBaseException().Message);
                    }

                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    model.Message = String.Format(EduSuiteUIResources.FailedToDeleteErrorMessage, EduSuiteUIResources.EmployeeEnquiryTarget);
                    model.IsSuccessful = false;
                    ActivityLog.CreateActivityLog(MenuConstants.EmployeeEnquiryTarget, ActionConstants.Delete, DbConstants.LogType.Error, model.RowKey, ex.GetBaseException().Message);
                }
            }
            return model;

        }

        private void FillTargetMonth(EmployeeEnquiryTargetViewModel model)
        {
            var employee = dbContext.Employees.Where(x => x.RowKey == model.EmployeeKey).SingleOrDefault();


            var start = Convert.ToDateTime(employee.JoiningDate);
            var end = Convert.ToDateTime(employee.JoiningDate).AddYears(5);

            // set end-date to end of month
            end = new DateTime(end.Year, end.Month, DateTime.DaysInMonth(end.Year, end.Month));

            var Dates = Enumerable.Range(0, Int32.MaxValue)
                                 .Select(e => start.AddMonths(e))
                                 .TakeWhile(e => e <= end)
                                 .Select(e => e);

            foreach (DateTime Date in Dates)
            {
                model.TargetMonth.Add(new SelectListModel
                {
                    Text = Date.ToString("MMM yyyy"),
                    ValueText = Date.ToString("yyyy-MM")
                });
            }

        }
    }
}
