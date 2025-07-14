using System;
using System.Collections.Generic;
using System.Data.Entity.Infrastructure;
using System.Data.Entity.SqlServer;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using CITS.EduSuite.Business.Common;
using CITS.EduSuite.Business.Interfaces;
using CITS.EduSuite.Business.Models.ViewModels;
using CITS.EduSuite.Data;
using System.Linq.Expressions;
using System.Data.Entity;
using CITS.EduSuite.Business.Models.Resources;

namespace CITS.EduSuite.Business.Services
{
    public class EmployeeSalaryService : IEmployeeSalaryService
    {
        private EduSuiteDatabase dbContext;
        Int64 maxDetailsKey { get; set; }

        Int64 maxOtherAmountKey { get; set; }

        public EmployeeSalaryService(EduSuiteDatabase objDB)
        {
            this.dbContext = objDB;
        }

        public List<EmployeeSalaryMasterViewModel> GetEmployeeSalaries(EmployeeSalaryMasterViewModel model, out long TotalRecords)
        {
            try
            {
                var Take = model.PageSize;
                var Skip = (model.PageIndex - 1) * model.PageSize;

                IQueryable<EmployeeSalaryMasterViewModel> employeeSalaryList = (from es in dbContext.EmployeeSalaryMasters.Where(row => row.SalaryMonth == model.SalaryMonthKey && row.SalaryYear == model.SalaryYearKey)
                                                                                orderby es.DateAdded descending
                                                                                select new EmployeeSalaryMasterViewModel
                                                                                {
                                                                                    SalaryMasterKey = es.RowKey,
                                                                                    BranchKey = es.Employee.BranchKey,
                                                                                    EmployeeKey = es.EmployeeKey,
                                                                                    EmployeeName = es.Employee.FirstName + " " + (es.Employee.MiddleName ?? "") + " " + es.Employee.LastName,
                                                                                    SalaryMonthKey = es.SalaryMonth,
                                                                                    SalaryYearKey = es.SalaryYear,
                                                                                    MonthlySalary = es.MonthlySalary,
                                                                                    TotalSalary = es.TotalSalary,
                                                                                    PaidAmount = es.EmployeeSalaryPayments.Select(row => row.PaidAmount).DefaultIfEmpty().Sum(),
                                                                                    SalaryStatusName = es.ProcessStatu.ProcessStatusName,
                                                                                    SalaryStatusKey = es.SalaryStatusKey,
                                                                                    SalaryPaymentKey = es.EmployeeSalaryPayments.Select(SP => SP.RowKey).FirstOrDefault(),
                                                                                    NoOfDaysWorked = es.NoOfDaysWorked,
                                                                                    PaySlipFileName = es.PaySlipFileName,
                                                                                    SalaryTypeKey = es.SalaryTypeKey,
                                                                                    SalaryTypeName = es.SalaryType.SalaryTypeName,
                                                                                    VoucherNumber = es.VoucherNumber,
                                                                                    LOP = es.LOP,
                                                                                    OvertimePerAHour = es.OverTimeAmount ?? 0,
                                                                                    OverTimeHours = es.OverTimeHours,
                                                                                    OverTimeMinutes = es.OverTimeMinutes,
                                                                                    OverTimeTotalAmount = es.OverTimeTotalAmount,
                                                                                    AdditionalDayAmount = es.AdditionalDayAmount ?? 0,
                                                                                    AdditionalDayCount = es.AdditionalDayWorked,
                                                                                    TotalWorkingDays = es.TotalWorkingDays,
                                                                                    BaseWorkingDays = es.BaseWorkingDays ?? 0,
                                                                                    AbsentDays = es.AbsentDays,
                                                                                    WeekOffCount = es.WeekOffCount,
                                                                                    OffdayCount = es.OffdayCount,
                                                                                    HolidayCount = es.HolidayCount,
                                                                                });
                Employee Employee = dbContext.Employees.Where(x => x.AppUserKey == DbConstants.User.UserKey).SingleOrDefault();
                if (Employee != null)
                {
                    if (Employee.BranchAccess != null)
                    {
                        var Branches = Employee.BranchAccess.Split(',').Select(Int16.Parse).ToList();
                        employeeSalaryList = employeeSalaryList.Where(row => Branches.Contains(row.BranchKey));
                    }
                }

                if (model.EmployeeKey != 0)
                {
                    employeeSalaryList = employeeSalaryList.Where(row => row.EmployeeKey == model.EmployeeKey);
                }
                if (model.BranchKey != 0)
                {
                    employeeSalaryList = employeeSalaryList.Where(row => row.BranchKey == model.BranchKey);
                }
                employeeSalaryList = employeeSalaryList.GroupBy(x => x.SalaryMasterKey).Select(y => y.FirstOrDefault());
                if (model.SortBy != "")
                {
                    employeeSalaryList = SortApplications(employeeSalaryList, model.SortBy, model.SortOrder);
                }
                TotalRecords = employeeSalaryList.Count();
                return employeeSalaryList.Skip(Skip).Take(Take).ToList<EmployeeSalaryMasterViewModel>();
            }
            catch (Exception ex)
            {
                TotalRecords = 0;
                ActivityLog.CreateActivityLog(MenuConstants.Employee, ActionConstants.View, DbConstants.LogType.Error, null, ex.GetBaseException().Message);
                return new List<EmployeeSalaryMasterViewModel>();
            }
        }
        private IQueryable<EmployeeSalaryMasterViewModel> SortApplications(IQueryable<EmployeeSalaryMasterViewModel> Query, string SortName, string SortOrder)
        {

            string command = SortOrder == "desc" ? "OrderByDescending" : "OrderBy";
            var type = typeof(EmployeeSalaryMasterViewModel);
            var property = type.GetProperty(SortName);
            var parameter = Expression.Parameter(type, "p");
            var propertyAccess = Expression.MakeMemberAccess(parameter, property);
            var orderByExpression = Expression.Lambda(propertyAccess, parameter);
            var resultExpression = Expression.Call(typeof(Queryable), command, new Type[] { type, property.PropertyType },
                                          Query.Expression, Expression.Quote(orderByExpression));
            return Query.Provider.CreateQuery<EmployeeSalaryMasterViewModel>(resultExpression);

        }


        public EmployeeSalaryMasterViewModel GetEmployeeSalaryById(EmployeeSalaryMasterViewModel model)
        {
            EmployeeSalaryMasterViewModel employeeSalaryMasterViewModel = new EmployeeSalaryMasterViewModel();

            try
            {

                return employeeSalaryMasterViewModel;
            }
            catch (Exception ex)
            {
                model = new EmployeeSalaryMasterViewModel();
                //model.ExceptionMessage = ex.GetBaseException().Message;
                ActivityLog.CreateActivityLog(MenuConstants.EmployeeSalary, ActionConstants.Edit, DbConstants.LogType.Error, model.EmployeeKey, ex.GetBaseException().Message);

                return model;
            }
        }

        public EmployeeSalaryMasterViewModel GetEmployeeSalaryByMonth(EmployeeSalaryMasterViewModel model)
        {

            EmployeeSalaryMasterViewModel employeeSalaryMasterViewModel = new EmployeeSalaryMasterViewModel();
            try
            {
                return employeeSalaryMasterViewModel;
            }
            catch (Exception ex)
            {
                model = new EmployeeSalaryMasterViewModel();
                ActivityLog.CreateActivityLog(MenuConstants.EmployeeSalary, ActionConstants.Edit, DbConstants.LogType.Error, model.EmployeeKey, ex.GetBaseException().Message);

                return model;
            }
        }

        public List<EmployeeSalaryMasterViewModel> GetEmployeesSalaryByMonth(EmployeeSalaryMasterViewModel model)
        {
            //model.IsAttendance  = dbContext.MenuTypes.Any(x => x.IsActive == true && x.RowKey == DbConstants.MenuType.Attendance);
            model.IsAttendance = false;
            AttendanceConfiguration attendanceConfiguration = dbContext.AttendanceConfigurations.Where(x => (x.BranchKey ?? model.BranchKey) == model.BranchKey).FirstOrDefault();
            model.DaysInMonth = Convert.ToByte(DateTime.DaysInMonth(model.SalaryYearKey, model.SalaryMonthKey));

            model.HolidayCount = dbContext.Holidays.Where(x => (x.BranchKey ?? model.BranchKey) == model.BranchKey && (x.HolidayFrom.Value.Month == model.SalaryMonthKey && (x.HolidayTypeKey == DbConstants.HolidayType.Dynamic ? x.HolidayFrom.Value.Year : model.SalaryYearKey) == model.SalaryYearKey) && (x.HolidayTo.Value.Month == model.SalaryMonthKey && x.HolidayTo.Value.Year == model.SalaryYearKey) && !x.IsDayOff).Select(y => DbFunctions.DiffDays(y.HolidayFrom, y.HolidayTo) + 1).DefaultIfEmpty().Sum();
            model.OffdayCount = dbContext.Holidays.Where(x => (x.BranchKey ?? model.BranchKey) == model.BranchKey && (x.HolidayFrom.Value.Month == model.SalaryMonthKey && (x.HolidayTypeKey == DbConstants.HolidayType.Dynamic ? x.HolidayFrom.Value.Year : model.SalaryYearKey) == model.SalaryYearKey) && (x.HolidayTo.Value.Month == model.SalaryMonthKey && x.HolidayTo.Value.Year == model.SalaryYearKey) && x.IsDayOff).Select(y => DbFunctions.DiffDays(y.HolidayFrom, y.HolidayTo) + 1).DefaultIfEmpty().Sum();


            
            List<EmployeeSalaryMasterViewModel> employeeSalaryMasterViewModelList = dbContext.spSelectEmployeeSalariesByMonth(model.BranchKey, model.EmployeeKey, model.SalaryMonthKey, model.SalaryYearKey)
                   .Select(S => new EmployeeSalaryMasterViewModel
                   {
                       SalaryMasterKey = S.SalaryMasterKey,
                       SalaryMonthKey = model.SalaryMonthKey,
                       SalaryYearKey = model.SalaryYearKey,
                       EmployeeKey = S.EmployeeKey,
                       BranchKey = S.BranchKey,
                       EmployeeName = S.EmployeeName,
                       SalaryTypeKey = S.SalaryTypeKey,
                       BranchName = S.BranchName,
                       OtherAmount = S.OtherAmount,
                       OtherAmountType = S.OtherAmountType == "A" ? true : false,
                       SalaryStatusKey = S.SalaryStatusKey ?? 0,
                       PaySlipFileName = S.PaySlipFileName,

                       DateCreated = S.DateAdded ?? DateTimeUTC.Now,
                       Remarks = S.Remarks,
                       MonthlySalary = S.MonthlySalary ?? 0,
                       BaseWorkingHours = attendanceConfiguration.TotalWorkingHours,
                       IsAttendance = model.IsAttendance,
                       OvertimePerAHour = S.OvertimeAmount ?? 0,
                       OverTimeHours = S.OverTimeHours,
                       OverTimeMinutes = S.OverTimeMinutes,
                       OverTimeTotalAmount = S.OverTimeTotalAmount,
                       AdditionalDayAmount = S.AdditionalDayAmount,
                       AdditionalDayCount = S.AdditionalDayWorked,
                       NoOfDaysWorked = S.NoOfDaysWorked,
                       TotalWorkingDays = S.TotalWorkingDays,
                       BaseWorkingDays = (S.BaseWorkingDays ?? (attendanceConfiguration.BaseDaysPerMonth ?? 0)),
                       AbsentDays = S.AbsentDays,
                       WeekOffCount = S.WeekOffCount,
                       OffdayCount = S.OffdayCount,
                       HolidayCount = S.HolidayCount,
                   }).ToList();
            
            
            
            

            foreach (EmployeeSalaryMasterViewModel item in employeeSalaryMasterViewModelList)
            {
                Employee employee = dbContext.Employees.Where(row => row.RowKey == item.EmployeeKey).SingleOrDefault();
                item.WeekOffCount = item.WeekOffCount ?? Enumerable.Range(1, DateTime.DaysInMonth(model.SalaryYearKey, model.SalaryMonthKey)).Select(day => new DateTime(model.SalaryYearKey, model.SalaryMonthKey, day)).Where(d => employee.AttendanceCategory.AttendanceCategoryWeekOffs.Where(date => date.WeekOffDayKey == (byte)(((int)d.DayOfWeek + 6) % 7 + 1)).Any()).Select(x => new { date = x, week = StringExtension.GetWeekNumberOfMonth(x) })
                       .Where(w => employee.AttendanceCategory.AttendanceCategoryWeekOffs.Where(x => x.WeekOffDayKey == (byte)(((int)w.date.DayOfWeek + 6) % 7 + 1) && (x.WeekOffDayWeekKeys != null ? x.WeekOffDayWeekKeys.Split(',').Select(int.Parse).ToList().Contains(w.week) : true)).Any()).Count();
                item.HolidayCount = item.HolidayCount ?? model.HolidayCount;
                item.OffdayCount = item.OffdayCount ?? model.OffdayCount;

                item.DaysInMonth = model.DaysInMonth;

                item.SalaryTypeKey = employee.SalaryTypeKey;


                if(item.SalaryTypeKey==3)
                {
                    item.OvertimePerAHour = item.MonthlySalary;
                }



                //item.TotalWorkingDays = (item.TotalWorkingDays ?? (model.DaysInMonth));
                //if (item.BaseWorkingDays == 0)
                //{
                //    item.BaseWorkingDays = (item.TotalWorkingDays ?? (model.DaysInMonth));
                //}
                FillOverTime(item, attendanceConfiguration);
                item.EmployeeSalaryEarnings = (
                     from dg in dbContext.DesignationGradeDetails.Where(row => row.SalaryHead.IsActive && row.SalaryHead.SalaryHeadType.IsActive && row.DesignationGrade.RowKey == employee.GradeKey && row.SalaryHead.SalaryHeadTypeKey == DbConstants.SalaryHeadType.MonthlyPayments)
                     join g in dbContext.EmployeeSalaryDetails.Where(x => x.SalaryHead.SalaryHeadTypeKey == DbConstants.SalaryHeadType.MonthlyPayments && x.EmployeeSalaryMasterKey == item.SalaryMasterKey) on new { dg.DesignationGradeKey, dg.SalaryHeadKey } equals new { DesignationGradeKey = g.EmployeeSalaryMaster.Employee.GradeKey ?? 0, g.SalaryHeadKey }
                     into gj
                     from g in gj.DefaultIfEmpty()
                     select new EmployeeSalaryDetailViewModel
                     {
                         SalaryHeadTypeKey = g != null ? g.SalaryHead.SalaryHeadTypeKey : dg.SalaryHead.SalaryHeadTypeKey,
                         SalaryHeadCode = g != null ? g.SalaryHead.SalaryHeadCode : dg.SalaryHead.SalaryHeadCode,
                         SalaryHeadKey = g != null ? g.SalaryHeadKey : dg.SalaryHeadKey,
                         SalaryHeadName = g != null ? g.SalaryHead.SalaryHeadName : dg.SalaryHead.SalaryHeadName,
                         Amount = g != null ? g.Amount : dg.AmountUnit,
                         Formula = dg.Formula,
                         Applicable = dg.ApplicableFormula,
                         IsFixed = dg.IsFixed ?? true,
                         IsInclude = dg.IsInclude ?? true
                     }).ToList();
                item.EmployeeSalaryDeductions = (
                    from dg in dbContext.DesignationGradeDetails.Where(row => row.SalaryHead.IsActive && row.SalaryHead.SalaryHeadType.IsActive && row.DesignationGrade.RowKey == employee.GradeKey && row.SalaryHead.SalaryHeadTypeKey == DbConstants.SalaryHeadType.StatutoryDeductions)
                    join g in dbContext.EmployeeSalaryDetails.Where(x => x.SalaryHead.SalaryHeadTypeKey == DbConstants.SalaryHeadType.StatutoryDeductions && x.EmployeeSalaryMasterKey == item.SalaryMasterKey) on new { dg.DesignationGradeKey, dg.SalaryHeadKey } equals new { DesignationGradeKey = g.EmployeeSalaryMaster.Employee.GradeKey ?? 0, g.SalaryHeadKey }
                    into gj
                    from g in gj.DefaultIfEmpty()
                    select new EmployeeSalaryDetailViewModel
                    {
                        SalaryHeadTypeKey = g != null ? g.SalaryHead.SalaryHeadTypeKey : dg.SalaryHead.SalaryHeadTypeKey,
                        SalaryHeadCode = g != null ? g.SalaryHead.SalaryHeadCode : dg.SalaryHead.SalaryHeadCode,
                        SalaryHeadKey = g != null ? g.SalaryHeadKey : dg.SalaryHeadKey,
                        SalaryHeadName = g != null ? g.SalaryHead.SalaryHeadName : dg.SalaryHead.SalaryHeadName,
                        Amount = g != null ? g.Amount : dg.AmountUnit,
                        Formula = dg.Formula,
                        Applicable = dg.ApplicableFormula,
                        IsFixed = dg.IsFixed ?? true,
                        IsInclude = dg.IsInclude ?? true
                    }).ToList();
                if (item.EmployeeSalaryEarnings.Count() == 0 && employee.GradeKey == null)
                {
                    item.EmployeeSalaryEarnings.Add(new EmployeeSalaryDetailViewModel
                    {
                        SalaryHeadTypeKey = DbConstants.SalaryHeadType.MonthlyPayments,
                        SalaryHeadCode = "A1",
                        SalaryHeadKey = 1,
                        SalaryHeadName = "Basic",
                        Amount = item.MonthlySalary,
                        Formula = "",
                        Applicable = null,
                        IsFixed = true,
                        IsInclude = true
                    });
                }

                FillEmployeeSalaryOtherAmounts(item);
                if (item.SalaryMasterKey != 0)
                {
                    item.EmployeeSalaryAdvances = dbContext.EmployeeSalaryAdvanceDetails.Where(x => x.SalaryMasterKey == item.SalaryMasterKey).Select(x => new SalaryAdvanceViewModel
                    {
                        RowKey = x.RowKey,
                        PaymentKey = x.PaymentKey,
                        Purpose = x.EmployeeSalaryAdvancePayment.Purpose,
                        PaymentDate = x.EmployeeSalaryAdvancePayment.PaymentDate,
                        PaidAmount = x.PaidAmount,
                        BeforeTakenAdvance = (x.EmployeeSalaryAdvancePayment.PaidAmount) - (((x.EmployeeSalaryAdvancePayment.ClearedAmount ?? 0) - (dbContext.EmployeeSalaryAdvanceDetails.Where(y => y.DateAdded > x.DateAdded && y.PaymentKey == x.PaymentKey).Select(y => y.PaidAmount).DefaultIfEmpty().Sum())) - x.PaidAmount),
                        IsDeduct = true
                    }).Union(dbContext.EmployeeSalaryAdvancePayments.Where(x => x.EmployeeKey == model.EmployeeKey && (x.ClearedAmount ?? 0) == 0 && ((x.ChequeStatusKey ?? DbConstants.ProcessStatus.Approved) == DbConstants.ProcessStatus.Approved) && System.Data.Entity.DbFunctions.TruncateTime(x.PaymentDate) <= System.Data.Entity.DbFunctions.TruncateTime(item.DateCreated)).Select(x => new SalaryAdvanceViewModel
                    {
                        RowKey = 0,
                        PaymentKey = x.RowKey,
                        Purpose = x.Purpose,
                        PaymentDate = x.PaymentDate,
                        PaidAmount = (x.PaidAmount),
                        BeforeTakenAdvance = x.PaidAmount,
                        IsDeduct = false
                    })).ToList();
                }
                else
                {
                    item.EmployeeSalaryAdvances = dbContext.EmployeeSalaryAdvancePayments.Where(x => x.EmployeeKey == model.EmployeeKey && x.IsCleared == false && ((x.ChequeStatusKey ?? DbConstants.ProcessStatus.Approved) == DbConstants.ProcessStatus.Approved)).Select(x => new SalaryAdvanceViewModel
                    {
                        RowKey = 0,
                        PaymentKey = x.RowKey,
                        Purpose = x.Purpose,
                        PaymentDate = x.PaymentDate,
                        PaidAmount = x.PaidAmount - (x.ClearedAmount ?? 0),
                        BeforeTakenAdvance = x.PaidAmount - (x.ClearedAmount ?? 0),
                        IsDeduct = false
                    }).ToList();
                }
                item.TotalWorkingDays = item.TotalWorkingDays != null ? item.TotalWorkingDays : (item.TotalWorkingDays ?? (model.DaysInMonth)) - (item.WeekOffCount ?? 0) - (item.OffdayCount ?? 0) - (item.HolidayCount ?? 0);

                item.LeaveTypes = dbContext.LeaveApplications.Where(x => x.EmployeeKey == model.EmployeeKey && x.LeaveStatusKey == DbConstants.ProcessStatus.Approved && (x.LeaveFrom.Month == model.SalaryMonthKey && x.LeaveFrom.Year == model.SalaryYearKey) && (x.LeaveTo.Month == model.SalaryMonthKey && x.LeaveTo.Year == model.SalaryYearKey)).GroupBy(y => new { y.LeaveType.SalaryDeductionForAdditional, y.LeaveTypeKey, y.LeaveType.LeaveTypeName, AvailableLeaveCount = (y.LeaveType.LeaveCount + y.LeaveType.LeaveCarryForwards.Where(x => x.EmployeeKey == y.EmployeeKey && x.LeaveBalance > 0).Select(x => x.LeaveBalance).DefaultIfEmpty().Sum()) }).Select(row => new LeaveTypeViewModel
                {
                    LeaveTypeCount = row.Select(y => DbFunctions.DiffDays(y.LeaveFrom, y.LeaveTo) + 1).DefaultIfEmpty().Sum(),
                    LeaveTypeName = row.Key.LeaveTypeName,
                    LeaveAvailableCount = row.Key.AvailableLeaveCount,
                    LeaveTypeKey = row.Key.LeaveTypeKey,
                    SalaryDeductionForAdditional = row.Key.SalaryDeductionForAdditional
                }).ToList();

            }

            return employeeSalaryMasterViewModelList;
        }

        public EmployeeSalaryMasterViewModel CreateEmployeeSalary(EmployeeSalaryMasterViewModel model)
        {
            EmployeeSalaryMaster employeeSalaryModel = new EmployeeSalaryMaster();
            var salaryCheck = dbContext.EmployeeSalaryMasters.Where(row => row.SalaryMonth == model.SalaryMonthKey && row.SalaryYear == model.SalaryYearKey && row.EmployeeKey == model.EmployeeKey).ToList();
            FillMasterDropdownLists(model);
            //CalculateTotalSalary(model);
            if (salaryCheck.Count != 0)
            {
                model.Message = String.Format(EduSuiteUIResources.ExistsErrorMessage, EduSuiteUIResources.EmployeeSalary);
                model.IsSuccessful = false;
                return model;
            }

            using (var transaction = dbContext.Database.BeginTransaction())
            {
                try
                {

                    Int64 maxKey = dbContext.EmployeeSalaryMasters.Select(p => p.RowKey).DefaultIfEmpty().Max();
                    maxDetailsKey = dbContext.EmployeeSalaryDetails.Select(p => p.RowKey).DefaultIfEmpty().Max();
                    maxOtherAmountKey = dbContext.EmployeeSalaryOtherAmounts.Select(p => p.RowKey).DefaultIfEmpty().Max();
                    CreateSalary(model, maxKey);
                    model.SalaryMasterKeyList.Add(maxKey + 1);
                    dbContext.SaveChanges();
                    transaction.Commit();
                    model.Message = EduSuiteUIResources.Success;
                    model.IsSuccessful = true;
                    ActivityLog.CreateActivityLog(MenuConstants.EmployeeSalary, ActionConstants.Add, DbConstants.LogType.Info, model.EmployeeKey, model.Message);

                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    //model.ExceptionMessage = ex.GetBaseException().Message;
                    model.Message = String.Format(EduSuiteUIResources.FailedToSaveErrorMessage, EduSuiteUIResources.EmployeeSalary);
                    model.IsSuccessful = false;
                    ActivityLog.CreateActivityLog(MenuConstants.EmployeeSalary, ActionConstants.Add, DbConstants.LogType.Error, model.EmployeeKey, ex.GetBaseException().Message);
                }

            }

            return model;
        }

        public EmployeeSalaryMasterViewModel UpdateEmployeeSalary(EmployeeSalaryMasterViewModel model)
        {
            EmployeeSalaryMaster employeeSalaryModel = new EmployeeSalaryMaster();
            var salaryCheck = dbContext.EmployeeSalaryMasters.Where(row => row.RowKey != model.SalaryMasterKey && row.SalaryMonth == model.SalaryMonthKey && row.SalaryYear == model.SalaryYearKey && row.EmployeeKey == model.EmployeeKey).ToList();
            FillMasterDropdownLists(model);
            //CalculateTotalSalary(model);
            if (salaryCheck.Count != 0)
            {
                model.Message = EduSuiteUIResources.ErrorEmployeeSalaryExists;
                model.IsSuccessful = false;

                return model;
            }

            using (var transaction = dbContext.Database.BeginTransaction())
            {
                try
                {
                    maxOtherAmountKey = dbContext.EmployeeSalaryOtherAmounts.Select(p => p.RowKey).DefaultIfEmpty().Max();
                    UpdateSalary(model);
                    dbContext.SaveChanges();
                    transaction.Commit();
                    model.Message = EduSuiteUIResources.Success;
                    model.IsSuccessful = true;
                    ActivityLog.CreateActivityLog(MenuConstants.EmployeeSalary, ActionConstants.Edit, DbConstants.LogType.Info, model.EmployeeKey, model.Message);
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    //model.ExceptionMessage = ex.GetBaseException().Message;
                    model.Message = String.Format(EduSuiteUIResources.FailedToSaveErrorMessage, EduSuiteUIResources.EmployeeSalary);
                    model.IsSuccessful = false;
                    ActivityLog.CreateActivityLog(MenuConstants.EmployeeSalary, ActionConstants.Edit, DbConstants.LogType.Error, model.EmployeeKey, ex.GetBaseException().Message);
                }

            }

            return model;
        }

        public EmployeeSalaryMasterViewModel UpdateEmployeesSalaryList(List<EmployeeSalaryMasterViewModel> modelList)
        {
            EmployeeSalaryMasterViewModel model = new EmployeeSalaryMasterViewModel();

            //model.Branches= selectListService.GetBranches();

            using (var transaction = dbContext.Database.BeginTransaction())
            {
                try
                {

                    CreateEmployeesSalary(modelList.Where(row => row.SalaryMasterKey == 0).ToList(), model.SalaryMasterKeyList);
                    UpdateEmployeesSalary(modelList.Where(row => row.SalaryMasterKey != 0).ToList());
                    dbContext.SaveChanges();
                    transaction.Commit();
                    model.Message = EduSuiteUIResources.Success;
                    model.IsSuccessful = true;
                    ActivityLog.CreateActivityLog(MenuConstants.EmployeeSalary, ActionConstants.Edit, DbConstants.LogType.Info, DbConstants.User.UserKey, model.Message);
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    //model.ExceptionMessage = ex.GetBaseException().Message;
                    model.Message = String.Format(EduSuiteUIResources.FailedToSaveErrorMessage, EduSuiteUIResources.EmployeeSalary);
                    model.IsSuccessful = false;
                    ActivityLog.CreateActivityLog(MenuConstants.EmployeeSalary, ActionConstants.Edit, DbConstants.LogType.Error, DbConstants.User.UserKey, ex.GetBaseException().Message);
                }

            }
            return model;
        }

        public List<EmployeeSalaryMasterViewModel> GetPaySlipDetailsByEmployee(EmployeeSalaryMasterViewModel model)
        {
            List<EmployeeSalaryMasterViewModel> modelList = new List<EmployeeSalaryMasterViewModel>();

            modelList = dbContext.EmployeeSalaryMasters.Where(row => model.SalaryMasterKeyList.Contains(row.RowKey)).Select(row => new EmployeeSalaryMasterViewModel
            {
                SalaryMasterKey = row.RowKey,
                EmployeeName = row.Employee.FirstName + (row.Employee.MiddleName ?? "") + row.Employee.LastName,
                EmployeeKey = row.EmployeeKey,
                //EmployeeCode = row.AppUser.EmployeeCode,
                SalaryMonthKey = row.SalaryMonth,
                SalaryYearKey = row.SalaryYear

            }).ToList();
            if (modelList.Count == 0)
            {
                modelList = new List<EmployeeSalaryMasterViewModel>();
            }
            return modelList;
        }

        private void CreateEmployeesSalary(List<EmployeeSalaryMasterViewModel> modelList, List<long> SalaryMasterKeyList)
        {
            Int64 maxKey = dbContext.EmployeeSalaryMasters.Select(p => p.RowKey).DefaultIfEmpty().Max();
            maxDetailsKey = dbContext.EmployeeSalaryDetails.Select(p => p.RowKey).DefaultIfEmpty().Max();
            maxOtherAmountKey = dbContext.EmployeeSalaryOtherAmounts.Select(p => p.RowKey).DefaultIfEmpty().Max();
            foreach (EmployeeSalaryMasterViewModel model in modelList)
            {
                CreateSalary(model, maxKey);
                SalaryMasterKeyList.Add(maxKey + 1);
                maxKey++;
            }
        }
        private void UpdateEmployeesSalary(List<EmployeeSalaryMasterViewModel> modelList)
        {
            maxOtherAmountKey = dbContext.EmployeeSalaryOtherAmounts.Select(p => p.RowKey).DefaultIfEmpty().Max();
            foreach (EmployeeSalaryMasterViewModel model in modelList)
            {
                UpdateSalary(model);
            }
        }

        private void CreateSalary(EmployeeSalaryMasterViewModel model, Int64 maxKey)
        {
            ConfigurationViewModel ConfigModel = new ConfigurationViewModel();
            ConfigModel.BranchKey = model.BranchKey;
            ConfigModel.ConfigType = DbConstants.PaymentReceiptConfigType.SalaryVoucher;
            Configurations.GenerateReceipt(dbContext, ConfigModel);


            Employee employee = dbContext.Employees.SingleOrDefault(row => row.RowKey == model.EmployeeKey);
            EmployeeSalaryMaster employeeSalaryModel = new EmployeeSalaryMaster();
            employeeSalaryModel.RowKey = Convert.ToInt64(maxKey + 1);
            model.SalaryMasterKey = employeeSalaryModel.RowKey;
            employeeSalaryModel.EmployeeKey = model.EmployeeKey;
            employeeSalaryModel.SalaryMonth = model.SalaryMonthKey;
            employeeSalaryModel.SalaryYear = model.SalaryYearKey;
            employeeSalaryModel.MonthlySalary = model.MonthlySalary;
            employeeSalaryModel.TotalSalary = model.TotalSalary;
            employeeSalaryModel.OtherAmountType = model.OtherAmountType ? "A" : "D";
            employeeSalaryModel.OtherAmount = model.OtherAmount;
            employeeSalaryModel.SalaryTypeKey = model.SalaryTypeKey;

            employeeSalaryModel.LOP = model.LOP;
            employeeSalaryModel.NoOfDaysWorked = model.NoOfDaysWorked ?? 0;
            employeeSalaryModel.TotalWorkingDays = model.TotalWorkingDays;

            employeeSalaryModel.BaseWorkingDays = model.BaseWorkingDays;
            employeeSalaryModel.AbsentDays = model.AbsentDays;
            employeeSalaryModel.WeekOffCount = model.WeekOffCount;
            employeeSalaryModel.OffdayCount = model.OffdayCount;
            employeeSalaryModel.HolidayCount = model.HolidayCount;
            employeeSalaryModel.SerialNumber = ConfigModel.SerialNumber;
            employeeSalaryModel.VoucherNumber = ConfigModel.ReceiptNumber;

            if (model.OverTimeHours > 0 || model.OverTimeMinutes > 0)
            {
                employeeSalaryModel.OverTimeAmount = model.OvertimePerAHour;
                employeeSalaryModel.OverTimeMinutes = model.OverTimeMinutes;
                employeeSalaryModel.OverTimeHours = model.OverTimeHours;
                employeeSalaryModel.OverTimeTotalAmount = model.OverTimeTotalAmount;
            }

            if (model.AdditionalDayCount > 0)
            {
                employeeSalaryModel.AdditionalDayWorked = model.AdditionalDayCount;
                employeeSalaryModel.AdditionalDayAmount = model.AdditionalDayAmount;
                employeeSalaryModel.OverTimeTotalAmount = model.OverTimeTotalAmount;
            }


            employeeSalaryModel.Remarks = model.Remarks;

            employeeSalaryModel.PaySlipFileName = GetMonthNameFromNumber(model.SalaryMonthKey) + "" + model.SalaryYearKey;
            //var AutoApproval = dbContext.SalaryConfigurations.Where(row => row.CompanyKey == employee.Branch.CompanyKey).Select(row => row.AutoApproval).FirstOrDefault();
            employeeSalaryModel.SalaryStatusKey = DbConstants.ProcessStatus.Approved;
            dbContext.EmployeeSalaryMasters.Add(employeeSalaryModel);

            model.SalaryMasterKey = employeeSalaryModel.RowKey;

            model.SalaryMonthName = CultureInfo.CurrentCulture.DateTimeFormat.GetMonthName(model.SalaryMonthKey) + EduSuiteUIResources.BlankSpace + model.SalaryYearKey;
            model.EmployeeName = dbContext.Employees.Where(x => x.RowKey == model.EmployeeKey).Select(x => x.FirstName).FirstOrDefault();
            List<AccountFlowViewModel> accountFlowModelList = new List<AccountFlowViewModel>();
            decimal expense = 0;
            if (model.EmployeeSalaryAdvances.Where(x => x.IsDeduct == true).Count() != 0)
            {
                decimal advance = 0;
                decimal Amount = model.TotalSalary + (model.EmployeeSalaryAdvances.Select(x => x.PaidAmount ?? 0).DefaultIfEmpty().Sum());
                long maxkey = dbContext.EmployeeSalaryAdvanceDetails.Select(x => x.RowKey).DefaultIfEmpty().Max();
                foreach (SalaryAdvanceViewModel advanceList in model.EmployeeSalaryAdvances.Where(x => x.IsDeduct == true))
                {
                    if (Amount != 0)
                    {
                        EmployeeSalaryAdvancePayment AdvancePayment = new EmployeeSalaryAdvancePayment();
                        EmployeeSalaryAdvanceDetail AdvanceDetail = new EmployeeSalaryAdvanceDetail();
                        AdvancePayment = dbContext.EmployeeSalaryAdvancePayments.SingleOrDefault(x => x.RowKey == advanceList.PaymentKey);
                        if (advanceList.PaidAmount <= Amount)
                        {
                            if (advanceList.PaidAmount != (AdvancePayment.PaidAmount - (AdvancePayment.ClearedAmount ?? 0)))
                            {
                                AdvancePayment.IsCleared = false;
                            }
                            else
                            {
                                AdvancePayment.IsCleared = true;
                            }
                            AdvancePayment.ClearedAmount = (AdvancePayment.ClearedAmount ?? 0) + (advanceList.PaidAmount ?? 0);
                            advance = advance + (advanceList.PaidAmount ?? 0);
                            Amount = Amount - AdvancePayment.PaidAmount;
                            AdvanceDetail.RowKey = maxkey + 1;
                            AdvanceDetail.SalaryMasterKey = employeeSalaryModel.RowKey;
                            AdvanceDetail.PaymentKey = advanceList.PaymentKey;
                            AdvanceDetail.PaidAmount = advanceList.PaidAmount ?? 0;
                        }
                        else
                        {
                            AdvancePayment.ClearedAmount = (AdvancePayment.ClearedAmount ?? 0) + Amount;
                            advance = advance + Amount;
                            AdvancePayment.IsCleared = false;
                            AdvanceDetail.RowKey = maxkey + 1;
                            AdvanceDetail.SalaryMasterKey = employeeSalaryModel.RowKey;
                            AdvanceDetail.PaymentKey = advanceList.PaymentKey;
                            AdvanceDetail.PaidAmount = Amount;
                            Amount = 0;
                        }
                        dbContext.EmployeeSalaryAdvanceDetails.Add(AdvanceDetail);
                        maxkey++;
                    }
                }
                expense = advance;
                accountFlowModelList = SalaryAdvanceAmountList(model, accountFlowModelList, advance, false);
            }
            decimal totalSalary = (model.TotalSalary > 0 ? model.TotalSalary : 0);
            expense = expense + totalSalary;
            accountFlowModelList = SalaryAmountList(model, accountFlowModelList, expense, false);
            accountFlowModelList = PayableAmountList(model, accountFlowModelList, totalSalary, false);
            CreateAccountFlow(accountFlowModelList, false);
            CreateSalaryDetails(model.EmployeeSalaryDetails.Where(row => row.SalaryHeadKey != 0).ToList(), employeeSalaryModel.RowKey);
            CreateSalaryOtherAmounts(model.EmployeeSalaryOtherAmounts.Where(row => row.RowKey == 0 && row.OtherAmountTypeKey != null).ToList(), employeeSalaryModel.RowKey);
            UpdateOvertimeAmount(model);
        }

        private void UpdateSalary(EmployeeSalaryMasterViewModel model)
        {

            EmployeeSalaryMaster employeeSalaryModel = new EmployeeSalaryMaster();
            employeeSalaryModel = dbContext.EmployeeSalaryMasters.SingleOrDefault(row => row.RowKey == model.SalaryMasterKey);
            employeeSalaryModel.EmployeeKey = model.EmployeeKey;
            employeeSalaryModel.SalaryMonth = model.SalaryMonthKey;
            employeeSalaryModel.SalaryYear = model.SalaryYearKey;
            employeeSalaryModel.MonthlySalary = model.MonthlySalary;
            employeeSalaryModel.TotalSalary = model.TotalSalary;
            employeeSalaryModel.OtherAmountType = model.OtherAmountType ? "A" : "D";
            employeeSalaryModel.OtherAmount = model.OtherAmount;
            employeeSalaryModel.SalaryTypeKey = model.SalaryTypeKey;

            employeeSalaryModel.LOP = model.LOP;
            employeeSalaryModel.NoOfDaysWorked = model.NoOfDaysWorked ?? 0;
            employeeSalaryModel.TotalWorkingDays = model.TotalWorkingDays;

            employeeSalaryModel.BaseWorkingDays = model.BaseWorkingDays;
            employeeSalaryModel.AbsentDays = model.AbsentDays;
            employeeSalaryModel.WeekOffCount = model.WeekOffCount;
            employeeSalaryModel.OffdayCount = model.OffdayCount;
            employeeSalaryModel.HolidayCount = model.HolidayCount;

            if (model.OverTimeHours > 0 || model.OverTimeMinutes > 0)
            {
                employeeSalaryModel.OverTimeAmount = model.OvertimePerAHour;
                employeeSalaryModel.OverTimeMinutes = model.OverTimeMinutes;
                employeeSalaryModel.OverTimeHours = model.OverTimeHours;
                employeeSalaryModel.OverTimeTotalAmount = model.OverTimeTotalAmount;
            }

            if (model.AdditionalDayCount > 0)
            {
                employeeSalaryModel.AdditionalDayWorked = model.AdditionalDayCount;
                employeeSalaryModel.AdditionalDayAmount = model.AdditionalDayAmount;
                employeeSalaryModel.OverTimeTotalAmount = model.OverTimeTotalAmount;
            }

            employeeSalaryModel.Remarks = model.Remarks;

            model.EmployeeName = dbContext.Employees.Where(x => x.RowKey == model.EmployeeKey).Select(x => x.FirstName).FirstOrDefault();
            model.SalaryMonthName = CultureInfo.CurrentCulture.DateTimeFormat.GetMonthName(model.SalaryMonthKey) + EduSuiteUIResources.BlankSpace + model.SalaryYearKey;



            UpdateSalaryOtherAmounts(model.EmployeeSalaryOtherAmounts.Where(row => row.RowKey != 0 && row.OtherAmountTypeKey != 0).ToList());
            CreateSalaryOtherAmounts(model.EmployeeSalaryOtherAmounts.Where(row => row.RowKey == 0 && row.OtherAmountTypeKey != null).ToList(), employeeSalaryModel.RowKey);

            List<AccountFlowViewModel> accountFlowModelList = new List<AccountFlowViewModel>();
            long maxkey = dbContext.EmployeeSalaryAdvanceDetails.Select(x => x.RowKey).DefaultIfEmpty().Max();
            decimal expense = 0;
            if (model.EmployeeSalaryAdvances.Count() != 0)
            {
                decimal advance = 0;
                decimal Amount = model.TotalSalary + (model.EmployeeSalaryAdvances.Where(x => x.IsDeduct == true).Select(x => x.PaidAmount ?? 0).DefaultIfEmpty().Sum());
                foreach (SalaryAdvanceViewModel advanceList in model.EmployeeSalaryAdvances)
                {
                    if (Amount != 0)
                    {
                        if (dbContext.EmployeeSalaryAdvanceDetails.Any(x => x.PaymentKey == advanceList.PaymentKey))
                        {
                            EmployeeSalaryAdvancePayment AdvancePayment = new EmployeeSalaryAdvancePayment();
                            EmployeeSalaryAdvanceDetail AdvanceDetail = new EmployeeSalaryAdvanceDetail();
                            AdvanceDetail = dbContext.EmployeeSalaryAdvanceDetails.SingleOrDefault(x => x.RowKey == advanceList.RowKey);
                            AdvancePayment = dbContext.EmployeeSalaryAdvancePayments.SingleOrDefault(x => x.RowKey == advanceList.PaymentKey);
                            if (advanceList.IsDeduct == true)
                            {
                                if (advanceList.PaidAmount <= Amount)
                                {
                                    if (advanceList.PaidAmount != (AdvancePayment.PaidAmount - ((AdvancePayment.ClearedAmount ?? 0) - AdvanceDetail.PaidAmount)))
                                    {
                                        AdvancePayment.IsCleared = false;
                                    }
                                    else
                                    {
                                        AdvancePayment.IsCleared = true;
                                    }
                                    AdvancePayment.ClearedAmount = (AdvancePayment.ClearedAmount - AdvanceDetail.PaidAmount) + (advanceList.PaidAmount ?? 0);
                                    advance = advance + (advanceList.PaidAmount ?? 0);
                                    Amount = Amount - (advanceList.PaidAmount ?? 0);
                                    AdvanceDetail.SalaryMasterKey = employeeSalaryModel.RowKey;
                                    AdvanceDetail.PaymentKey = advanceList.PaymentKey;
                                    AdvanceDetail.PaidAmount = (advanceList.PaidAmount ?? 0);
                                }
                                else
                                {
                                    AdvancePayment.ClearedAmount = (AdvancePayment.ClearedAmount - AdvanceDetail.PaidAmount) + Amount;
                                    advance = advance + Amount;
                                    AdvancePayment.IsCleared = false;
                                    AdvanceDetail.SalaryMasterKey = employeeSalaryModel.RowKey;
                                    AdvanceDetail.PaymentKey = advanceList.PaymentKey;
                                    AdvanceDetail.PaidAmount = Amount;
                                    Amount = 0;
                                }
                            }
                            else
                            {
                                AdvancePayment.ClearedAmount = (AdvancePayment.ClearedAmount ?? 0) - AdvanceDetail.PaidAmount;
                                AdvancePayment.IsCleared = false;
                                dbContext.EmployeeSalaryAdvanceDetails.Remove(AdvanceDetail);
                            }
                        }
                        else
                        {
                            if (advanceList.IsDeduct == true)
                            {
                                EmployeeSalaryAdvancePayment AdvancePayment = new EmployeeSalaryAdvancePayment();
                                EmployeeSalaryAdvanceDetail AdvanceDetail = new EmployeeSalaryAdvanceDetail();
                                AdvancePayment = dbContext.EmployeeSalaryAdvancePayments.SingleOrDefault(x => x.RowKey == advanceList.PaymentKey);
                                if (advanceList.PaidAmount <= Amount)
                                {
                                    if (advanceList.PaidAmount != (AdvancePayment.PaidAmount - (AdvancePayment.ClearedAmount ?? 0)))
                                    {
                                        AdvancePayment.IsCleared = false;
                                    }
                                    else
                                    {
                                        AdvancePayment.IsCleared = true;
                                    }
                                    AdvancePayment.ClearedAmount = (AdvancePayment.ClearedAmount ?? 0) + (advanceList.PaidAmount ?? 0);
                                    advance = advance + (advanceList.PaidAmount ?? 0);
                                    Amount = Amount - (advanceList.PaidAmount ?? 0);
                                    AdvanceDetail.RowKey = maxkey + 1;
                                    AdvanceDetail.SalaryMasterKey = employeeSalaryModel.RowKey;
                                    AdvanceDetail.PaymentKey = advanceList.PaymentKey;
                                    AdvanceDetail.PaidAmount = advanceList.PaidAmount ?? 0;
                                }
                                else
                                {
                                    AdvancePayment.ClearedAmount = (AdvancePayment.ClearedAmount ?? 0) + Amount;
                                    advance = advance + Amount;
                                    AdvancePayment.IsCleared = false;
                                    AdvanceDetail.RowKey = maxkey + 1;
                                    AdvanceDetail.SalaryMasterKey = employeeSalaryModel.RowKey;
                                    AdvanceDetail.PaymentKey = advanceList.PaymentKey;
                                    AdvanceDetail.PaidAmount = Amount;
                                    Amount = 0;
                                }
                                dbContext.EmployeeSalaryAdvanceDetails.Add(AdvanceDetail);
                                maxkey++;
                            }
                        }
                    }
                }
                expense = advance;
                if (dbContext.AccountFlows.Any(x => x.TransactionTypeKey == DbConstants.TransactionType.SalaryAdvanceRecieved && x.TransactionKey == model.SalaryMasterKey))
                {
                    accountFlowModelList = SalaryAdvanceAmountList(model, accountFlowModelList, advance, true);
                }
                else
                {
                    accountFlowModelList = SalaryAdvanceAmountList(model, accountFlowModelList, advance, true);
                    CreateAccountFlow(accountFlowModelList, false);
                    accountFlowModelList = new List<AccountFlowViewModel>();
                }
            }
            decimal totalSalary = (model.TotalSalary > 0 ? model.TotalSalary : 0);
            expense = expense + totalSalary;
            accountFlowModelList = SalaryAmountList(model, accountFlowModelList, expense, true);
            accountFlowModelList = PayableAmountList(model, accountFlowModelList, totalSalary, true);
            CreateAccountFlow(accountFlowModelList, true);

            UpdateOvertimeAmount(model);
        }
        private void CreateSalaryDetails(List<EmployeeSalaryDetailViewModel> modelList, Int64 SalaryMasterKey)
        {


            foreach (EmployeeSalaryDetailViewModel model in modelList)
            {

                EmployeeSalaryDetail employeeSalaryModel = new EmployeeSalaryDetail();
                employeeSalaryModel.RowKey = Convert.ToInt64(maxDetailsKey + 1);
                employeeSalaryModel.EmployeeSalaryMasterKey = SalaryMasterKey;
                employeeSalaryModel.SalaryHeadKey = model.SalaryHeadKey;
                employeeSalaryModel.Amount = Convert.ToDecimal(model.Amount);
                dbContext.EmployeeSalaryDetails.Add(employeeSalaryModel);
                maxDetailsKey++;

            }

        }

        private void UpdateSalaryDetails(List<EmployeeSalaryDetailViewModel> modelList)
        {


            foreach (EmployeeSalaryDetailViewModel model in modelList)
            {

                EmployeeSalaryDetail employeeSalaryModel = new EmployeeSalaryDetail();
                employeeSalaryModel = dbContext.EmployeeSalaryDetails.SingleOrDefault(row => row.RowKey == model.RowKey);
                employeeSalaryModel.Amount = Convert.ToDecimal(model.Amount);
            }

        }

        private void CreateSalaryOtherAmounts(List<EmployeeSalaryOtherAmountViewModel> modelList, Int64 SalaryMasterKey)
        {


            foreach (EmployeeSalaryOtherAmountViewModel model in modelList)
            {

                EmployeeSalaryOtherAmount EmployeeSalaryOtherAmountModel = new EmployeeSalaryOtherAmount();
                EmployeeSalaryOtherAmountModel.RowKey = ++maxOtherAmountKey;
                EmployeeSalaryOtherAmountModel.EmployeeSalaryMasterKey = SalaryMasterKey;
                EmployeeSalaryOtherAmountModel.OtherAmountTypeKey = model.OtherAmountTypeKey ?? 0;
                EmployeeSalaryOtherAmountModel.Amount = Convert.ToDecimal(model.Amount);
                EmployeeSalaryOtherAmountModel.IsAddition = model.IsAddition;
                dbContext.EmployeeSalaryOtherAmounts.Add(EmployeeSalaryOtherAmountModel);

            }

        }

        private void UpdateSalaryOtherAmounts(List<EmployeeSalaryOtherAmountViewModel> modelList)
        {


            foreach (EmployeeSalaryOtherAmountViewModel model in modelList)
            {

                EmployeeSalaryOtherAmount employeeSalaryModel = new EmployeeSalaryOtherAmount();
                employeeSalaryModel = dbContext.EmployeeSalaryOtherAmounts.SingleOrDefault(row => row.RowKey == model.RowKey);
                employeeSalaryModel.Amount = Convert.ToDecimal(model.Amount);
            }

        }

        private void FillEmployeeSalaryOtherAmounts(EmployeeSalaryMasterViewModel model)
        {
            model.EmployeeSalaryOtherEarnings = dbContext.EmployeeSalaryOtherAmounts.Where(row => row.EmployeeSalaryMasterKey == model.SalaryMasterKey && row.IsAddition).Select(row => new EmployeeSalaryOtherAmountViewModel
            {
                RowKey = row.RowKey,
                OtherAmountTypeKey = row.OtherAmountTypeKey,
                Amount = row.Amount,
                IsAddition = true

            }).ToList();
            model.EmployeeSalaryOtherDeductions = dbContext.EmployeeSalaryOtherAmounts.Where(row => row.EmployeeSalaryMasterKey == model.SalaryMasterKey && !row.IsAddition).Select(row => new EmployeeSalaryOtherAmountViewModel
            {
                RowKey = row.RowKey,
                OtherAmountTypeKey = row.OtherAmountTypeKey,
                Amount = row.Amount,
                IsAddition = true

            }).ToList();
            if (model.EmployeeSalaryOtherEarnings.Count == 0)
            {
                model.EmployeeSalaryOtherEarnings.Add(new EmployeeSalaryOtherAmountViewModel { IsAddition = true });
            }
            if (model.EmployeeSalaryOtherDeductions.Count == 0)
            {
                model.EmployeeSalaryOtherDeductions.Add(new EmployeeSalaryOtherAmountViewModel { IsAddition = false });
            }
            FillOtherAmountTypes(model.EmployeeSalaryOtherEarnings, true);
            FillOtherAmountTypes(model.EmployeeSalaryOtherDeductions, false);
        }

        public void UpdateOvertimeAmount(EmployeeSalaryMasterViewModel model)
        {
            EmployeeOvertime employeeOvertime = dbContext.EmployeeOvertimes.SingleOrDefault(x => x.EmployeeKey == model.EmployeeKey && x.AttendanceMonth == model.SalaryMonthKey && x.AttendanceYear == model.SalaryYearKey);
            if (employeeOvertime != null)
            {
                employeeOvertime.OvertimeAmount = model.OverTimeTotalAmount;
            }

        }

        public EmployeeSalaryMasterViewModel DeleteEmployeeSalary(EmployeeSalaryMasterViewModel model)
        {
            using (var transaction = dbContext.Database.BeginTransaction())
            {
                try
                {
                    EmployeeSalaryMaster employeeSalaryMaster = dbContext.EmployeeSalaryMasters.SingleOrDefault(row => row.RowKey == model.SalaryMasterKey);
                    long RowKey = employeeSalaryMaster.RowKey;


                    List<EmployeeSalaryDetail> employeeSalaryDetail = dbContext.EmployeeSalaryDetails.Where(row => row.EmployeeSalaryMasterKey == model.SalaryMasterKey).ToList();

                    dbContext.EmployeeSalaryDetails.RemoveRange(employeeSalaryDetail);
                    List<EmployeeSalaryOtherAmount> employeeSalaryOtherAmounts = dbContext.EmployeeSalaryOtherAmounts.Where(row => row.EmployeeSalaryMasterKey == model.SalaryMasterKey).ToList();
                    dbContext.EmployeeSalaryOtherAmounts.RemoveRange(employeeSalaryOtherAmounts);

                    List<EmployeeSalaryAdvanceDetail> employeeSalaryAdvanceDetail = dbContext.EmployeeSalaryAdvanceDetails.Where(row => row.SalaryMasterKey == model.SalaryMasterKey).ToList();

                    if (employeeSalaryAdvanceDetail.Count > 0)
                    {
                        foreach (EmployeeSalaryAdvanceDetail row in employeeSalaryAdvanceDetail)
                        {
                            EmployeeSalaryAdvancePayment employeeSalaryAdvancePayment = new EmployeeSalaryAdvancePayment();
                            employeeSalaryAdvancePayment = dbContext.EmployeeSalaryAdvancePayments.SingleOrDefault(x => x.RowKey == row.PaymentKey);
                            employeeSalaryAdvancePayment.ClearedAmount = employeeSalaryAdvancePayment.ClearedAmount - row.PaidAmount;
                            employeeSalaryAdvancePayment.IsCleared = false;
                        }
                        employeeSalaryAdvanceDetail.ForEach(row => dbContext.EmployeeSalaryAdvanceDetails.Remove(row));
                    }
                    ConfigurationViewModel ConfigModel = new ConfigurationViewModel();
                    ConfigModel.BranchKey = employeeSalaryMaster.Employee.BranchKey;
                    ConfigModel.SerialNumber = employeeSalaryMaster.SerialNumber ?? 0;
                    ConfigModel.IsDelete = true;
                    ConfigModel.ConfigType = DbConstants.PaymentReceiptConfigType.SalaryVoucher;
                    Configurations.GenerateReceipt(dbContext, ConfigModel);

                    AccountFlowViewModel accountFlowModel = new AccountFlowViewModel();
                    AccountFlowService accountFlowService = new AccountFlowService(dbContext);
                    accountFlowModel.TransactionTypeKey = DbConstants.TransactionType.SalaryPayable;
                    accountFlowModel.TransactionKey = RowKey;
                    accountFlowService.DeleteAccountFlow(accountFlowModel);
                    if (dbContext.AccountFlows.Any(x => x.TransactionTypeKey == DbConstants.TransactionType.SalaryAdvanceRecieved && x.TransactionKey == RowKey))
                    {
                        accountFlowModel.TransactionTypeKey = DbConstants.TransactionType.SalaryAdvanceRecieved;
                        accountFlowService.DeleteAccountFlow(accountFlowModel);
                    }
                    //List<SalaryComponent> salaryComponent = dbContext.SalaryComponents.Where(row => row.EmployeeSalaryKey == model.SalaryMasterKey).ToList();
                    //if (salaryComponent != null)
                    //{
                    //    salaryComponent.ForEach(row => row.EmployeeSalaryKey = null);
                    //}
                    //EmployeeMonthlyPF employeeMonthlyPF = dbContext.EmployeeMonthlyPFs.SingleOrDefault(row => row.EmployeeSalaryKey == model.SalaryMasterKey);
                    //if (employeeMonthlyPF != null)
                    //{
                    //    dbContext.EmployeeMonthlyPFs.Remove(employeeMonthlyPF);
                    //}
                    //List<LoanMonthlyRepayment> loanMonthlyRepayment = dbContext.LoanMonthlyRepayments.Where(row => row.SalaryMasterKey == model.SalaryMasterKey).ToList();
                    //if (loanMonthlyRepayment != null)
                    //{
                    //    loanMonthlyRepayment.ForEach(row => dbContext.LoanMonthlyRepayments.Remove(row));
                    //}
                    EmployeeOvertime employeeOvertime = dbContext.EmployeeOvertimes.SingleOrDefault(x => x.EmployeeKey == employeeSalaryMaster.EmployeeKey && x.AttendanceMonth == employeeSalaryMaster.SalaryMonth && x.AttendanceYear == employeeSalaryMaster.SalaryYear);
                    if (employeeOvertime != null)
                    {
                        employeeOvertime.OvertimeAmount = null;
                    }
                    EmployeeSalaryPayment employeeSalaryPayment = dbContext.EmployeeSalaryPayments.SingleOrDefault(row => row.EmployeeSalaryMasterKey == model.SalaryMasterKey);

                    if (employeeSalaryPayment != null)
                    {
                        RowKey = employeeSalaryPayment.RowKey;
                        dbContext.EmployeeSalaryPayments.Remove(employeeSalaryPayment);
                        accountFlowModel.TransactionTypeKey = DbConstants.TransactionType.Salary;
                        accountFlowModel.TransactionKey = RowKey;
                        accountFlowService.DeleteAccountFlow(accountFlowModel);
                    }
                    dbContext.EmployeeSalaryMasters.Remove(employeeSalaryMaster);
                    dbContext.SaveChanges();
                    transaction.Commit();
                    model.Message = EduSuiteUIResources.Success;
                    model.IsSuccessful = true;
                    ActivityLog.CreateActivityLog(MenuConstants.EmployeeSalary, ActionConstants.Delete, DbConstants.LogType.Info, model.SalaryMasterKey, model.Message);
                }
                catch (DbUpdateException ex)
                {
                    transaction.Rollback();
                    //model.ExceptionMessage = ex.GetBaseException().Message;
                    if (ex.GetBaseException().Message.ToUpper().Contains(EduSuiteUIResources.ForeignKeyError.ToUpper()))
                    {
                        model.Message = String.Format(EduSuiteUIResources.CantDeleteErrorMessage, EduSuiteUIResources.EmployeeSalary);
                        model.IsSuccessful = false;
                    }
                    ActivityLog.CreateActivityLog(MenuConstants.EmployeeSalary, ActionConstants.Delete, DbConstants.LogType.Error, model.SalaryMasterKey, ex.GetBaseException().Message);
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    //model.ExceptionMessage = ex.GetBaseException().Message;
                    model.Message = String.Format(EduSuiteUIResources.FailedToDeleteErrorMessage, EduSuiteUIResources.EmployeeSalary);
                    model.IsSuccessful = false;
                    ActivityLog.CreateActivityLog(MenuConstants.EmployeeSalary, ActionConstants.Delete, DbConstants.LogType.Error, model.SalaryMasterKey, ex.GetBaseException().Message);
                }
            }
            return model;
        }
        public EmployeeSalaryMasterViewModel DeleteEmployeeSalaryComponent(EmployeeSalaryDetailViewModel model)
        {
            EmployeeSalaryMasterViewModel employeeSalaryMasterModel = new EmployeeSalaryMasterViewModel();
            using (var transaction = dbContext.Database.BeginTransaction())
            {
                try
                {
                    EmployeeSalaryDetail employeeSalaryDetail = dbContext.EmployeeSalaryDetails.SingleOrDefault(row => row.RowKey == model.RowKey);
                    dbContext.EmployeeSalaryDetails.Remove(employeeSalaryDetail);
                    dbContext.SaveChanges();
                    transaction.Commit();
                    employeeSalaryMasterModel.Message = EduSuiteUIResources.Success;
                    employeeSalaryMasterModel.IsSuccessful = true;
                    ActivityLog.CreateActivityLog(MenuConstants.EmployeeSalary, ActionConstants.Delete, DbConstants.LogType.Info, model.RowKey, employeeSalaryMasterModel.Message);
                }
                catch (DbUpdateException ex)
                {
                    transaction.Rollback();
                    // employeeSalaryMasterModel.ExceptionMessage = ex.GetBaseException().Message;
                    if (ex.GetBaseException().Message.ToUpper().Contains(EduSuiteUIResources.ForeignKeyError.ToUpper()))
                    {
                        employeeSalaryMasterModel.Message = String.Format(EduSuiteUIResources.CantDeleteErrorMessage, EduSuiteUIResources.EmployeeSalary);
                        employeeSalaryMasterModel.IsSuccessful = false;
                    }
                    ActivityLog.CreateActivityLog(MenuConstants.EmployeeSalary, ActionConstants.Delete, DbConstants.LogType.Error, model.RowKey, ex.GetBaseException().Message);
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    //employeeSalaryMasterModel.ExceptionMessage = ex.GetBaseException().Message;
                    employeeSalaryMasterModel.Message = String.Format(EduSuiteUIResources.FailedToDeleteErrorMessage, EduSuiteUIResources.EmployeeSalary);
                    employeeSalaryMasterModel.IsSuccessful = false;
                    ActivityLog.CreateActivityLog(MenuConstants.EmployeeSalary, ActionConstants.Delete, DbConstants.LogType.Error, model.RowKey, ex.GetBaseException().Message);
                }
            }
            return employeeSalaryMasterModel;
        }

        //public List<EmployeeSalaryComponentViewModel> GetSalaryComponentsByMonth(EmployeeSalaryMasterViewModel model)
        //{
        //    FillSalaryComponents(model);
        //    return model.SalaryComponents;
        //}

        //public EmployeeSalaryMasterViewModel GetBranches(EmployeeSalaryMasterViewModel model)
        //{

        //    model.Branches = dbContext.VwBranchSelectActiveOnlies.OrderBy(row => row.DisplayOrder).Select(row => new SelectListModel
        //    {
        //        RowKey = row.RowKey,
        //        Text = row.BranchName
        //    }).ToList();

        //    return model;
        //}
        //public EmployeeSalaryMasterViewModel GetEmployeesByBranchId(EmployeeSalaryMasterViewModel model)
        //{
        //    //model.Employees = dbContext.AppUsers.Where(x => (x.GradeKey != null && x.DesignationGrade.DesignationGradeDetails.Select(y => y.AmountUnit).FirstOrDefault() != 0 && x.BranchKey == model.BranchKey) || (x.RowKey == model.EmployeeKey)).Select(row => new GroupSelectListModel
        //    //{
        //    //    RowKey = row.RowKey,
        //    //    Text = row.FirstName + " " + (row.MiddleName ?? "") + " " + row.LastName,
        //    //}).OrderBy(row => row.Text).ToList();
        //    model.Employees = dbContext.Employees.Where(x => (x.SalaryTypeKey != null && x.BranchKey == model.BranchKey) || (x.RowKey == model.EmployeeKey)).Select(row => new GroupSelectListModel
        //    {
        //        RowKey = row.RowKey,
        //        Text = row.FirstName + " " + (row.MiddleName ?? "") + " " + row.LastName,
        //    }).OrderBy(row => row.Text).ToList();

        //    return model;
        //}

        public EmployeeSalaryMasterViewModel GetSalaryHeads(EmployeeSalaryMasterViewModel model)
        {
            //List<short> SalaryHeadtypeKeys = new List<short>() { DbConstants.SalaryHeadType.MonthlyPayments, DbConstants.SalaryHeadType.StatutoryDeductions };

            model.EmployeeSalaryDetails = dbContext.SalaryHeads.Select(row => new EmployeeSalaryDetailViewModel
            {
                SalaryHeadName = row.SalaryHeadName
            }).ToList();
            return model;
        }

        public void FillMasterDropdownLists(EmployeeSalaryMasterViewModel model)
        {
            FillEmployeesMaster(model);
        }

        private void FillOverTime(EmployeeSalaryMasterViewModel model, AttendanceConfiguration attendanceConfiguration)
        {
            model.AdditionalDayAmount = model.AdditionalDayAmount != 0 ? model.AdditionalDayAmount : model.MonthlySalary / (model.BaseWorkingDays == 0 ? 1 : model.BaseWorkingDays);
            model.OvertimePerAHour = model.OvertimePerAHour != 0 ? model.OvertimePerAHour : (model.AdditionalDayAmount / (model.BaseWorkingHours == 0 ? 1 : model.BaseWorkingHours));
            if (attendanceConfiguration.OvertimeAdditionAmount != null)
            {
                if (attendanceConfiguration.UnitTypeKey == DbConstants.UnitType.Amount)
                {
                    model.OvertimePerAHour = Convert.ToDecimal(model.OvertimePerAHour + attendanceConfiguration.OvertimeAdditionAmount);
                }
                else
                {
                    model.OvertimePerAHour = model.OvertimePerAHour + Convert.ToDecimal((model.MonthlySalary / 100) * attendanceConfiguration.OvertimeAdditionAmount);
                }
            }
        }

        private void FillOtherAmountTypes(List<EmployeeSalaryOtherAmountViewModel> modelList, bool IsAddition)
        {
            EmployeeSalaryOtherAmountViewModel objViewModel = new EmployeeSalaryOtherAmountViewModel();
            objViewModel.SalaryOtherAmountTypes = dbContext.SalaryOtherAmountTypes.Where(row => row.IsActive && row.IsAddition == IsAddition).OrderBy(row => row.DisplayOrder)
                    .Select(row => new SelectListModel
                    {
                        RowKey = row.RowKey,
                        Text = row.OtherSalaryHeadName
                    }).ToList();
            foreach (EmployeeSalaryOtherAmountViewModel model in modelList)
            {
                model.SalaryOtherAmountTypes = objViewModel.SalaryOtherAmountTypes;
            }
        }

        public EmployeeSalaryMasterViewModel FillEmployeesMaster(EmployeeSalaryMasterViewModel model)
        {
            model.Employees = dbContext.Employees.Where(x => x.BranchKey == model.BranchKey).Select(row => new SelectListModel
            {
                RowKey = row.RowKey,
                Text = row.FirstName + " " + (row.MiddleName ?? "") + " " + row.LastName,
            }).OrderBy(row => row.Text).ToList();
            return model;
        }
        private string GetMonthNameFromNumber(int MonthNumber)
        {
            DateTimeFormatInfo mfi = new DateTimeFormatInfo();
            return (mfi.GetAbbreviatedMonthName(MonthNumber).ToString());
        }

        #region Salary Payment

        public List<PaymentWindowViewModel> GetEmployeePaymentDetails(long SalaryMasterKey)
        {

            try
            {
                List<PaymentWindowViewModel> TotalPaymentDetails = new List<PaymentWindowViewModel>();

                TotalPaymentDetails = dbContext.EmployeeSalaryPayments.Where(x => x.EmployeeSalaryMasterKey == SalaryMasterKey).Select(row => new PaymentWindowViewModel
                {
                    PaymentKey = row.RowKey,
                    MasterKey = row.EmployeeSalaryMasterKey,
                    PaymentModeKey = row.PaymentModeKey,
                    PaymentModeName = row.PaymentMode.PaymentModeName,
                    PaymentModeSubKey = row.PaymentModeSubKey,
                    PaymentModeSubName = row.PaymentModeSub.PaymentModeSubName,
                    PaidAmount = row.PaidAmount,
                    BalanceAmount = row.BalanceAmount,
                    PaymentDate = row.PaymentDate,
                    BankAccountKey = row.BankAccountKey,
                    BankAccountBalance = row.BankAccount.CurrentAccountBalance,
                    CardNumber = row.CardNumber,
                    ChequeOrDDNumber = row.ChequeOrDDNumber,
                    ChequeOrDDDate = row.ChequeOrDDDate,
                    Purpose = row.Purpose,
                    ReceivedBy = row.ReceivedBy,
                    OnBehalfOf = row.OnBehalfOf,
                    PaidBy = row.PaidBy,
                    AuthorizedBy = row.AuthorizedBy,
                    AmountToPay = row.EmployeeSalaryMaster.TotalSalary,
                    Remarks = row.Remarks,
                    ReceiptNumber = row.VoucherNumber,
                    BranchKey = row.PaidBranchKey != null ? row.PaidBranchKey : row.EmployeeSalaryMaster.Employee.BranchKey
                }).ToList();

                return TotalPaymentDetails;
            }
            catch (Exception)
            {
                return new List<PaymentWindowViewModel>();
            }
        }
        public PaymentWindowViewModel GetEmployeePaymentById(long Id)
        {
            try
            {
                PaymentWindowViewModel model = new PaymentWindowViewModel();

                model = dbContext.EmployeeSalaryPayments.Where(x => x.RowKey == Id).Select(row => new PaymentWindowViewModel
                {
                    PaymentKey = row.RowKey,
                    MasterKey = row.EmployeeSalaryMasterKey,
                    PaymentModeKey = row.PaymentModeKey,
                    PaymentModeName = row.PaymentMode.PaymentModeName,
                    PaymentModeSubKey = row.PaymentModeSubKey,
                    PaymentModeSubName = row.PaymentModeSub.PaymentModeSubName,
                    PaidAmount = row.PaidAmount,
                    BalanceAmount = row.BalanceAmount,
                    PaymentDate = row.PaymentDate,
                    BankAccountKey = row.BankAccountKey,
                    BankAccountBalance = row.BankAccount.CurrentAccountBalance,
                    CardNumber = row.CardNumber,
                    ChequeOrDDNumber = row.ChequeOrDDNumber,
                    ChequeOrDDDate = row.ChequeOrDDDate,
                    Purpose = row.Purpose,
                    ReceivedBy = row.ReceivedBy,
                    OnBehalfOf = row.OnBehalfOf,
                    PaidBy = row.PaidBy,
                    AuthorizedBy = row.AuthorizedBy,
                    AmountToPay = row.EmployeeSalaryMaster.TotalSalary,
                    Remarks = row.Remarks,
                    PaidBranchKey = row.PaidBranchKey,
                }).FirstOrDefault();
                if (model == null)
                {
                    model = new PaymentWindowViewModel();
                }
                FillSalaryPaymentDropdownLists(model);
                return model;
            }
            catch (Exception)
            {
                return new PaymentWindowViewModel();
            }
        }
        public PaymentWindowViewModel GetEmployeeSalaryPaymentById(long Id)
        {
            //PaymentWindowViewModel model = new PaymentWindowViewModel();
            //EmployeeSalaryMaster employeeSalaryMaster = new EmployeeSalaryMaster();
            //employeeSalaryMaster = dbContext.EmployeeSalaryMasters.SingleOrDefault(row => row.RowKey == Id);
            //model = dbContext.EmployeeSalaryPayments.Where(x => x.EmployeeSalaryMasterKey == Id).Select(row => new PaymentWindowViewModel
            //{
            //    PaymentKey = row.RowKey,
            //    MasterKey = row.EmployeeSalaryMasterKey,
            //    PaymentModeKey = row.PaymentModeKey,
            //    PaidAmount = row.PaidAmount,
            //    BalanceAmount = row.BalanceAmount,
            //    PaymentDate = row.PaymentDate,
            //    BankAccountKey = row.BankAccountKey,
            //    BankAccountBalance = row.BankAccount.CurrentAccountBalance,
            //    CardNumber = row.CardNumber,
            //    ChequeOrDDNumber = row.ChequeOrDDNumber,
            //    ChequeOrDDDate = row.ChequeOrDDDate,
            //    Purpose = row.Purpose,
            //    ReceivedBy = row.ReceivedBy,
            //    OnBehalfOf = row.OnBehalfOf,
            //    PaidBy = row.PaidBy,
            //    AuthorizedBy = row.AuthorizedBy,
            //    AmountToPay = row.EmployeeSalaryMaster.TotalSalary,
            //    Remarks = row.Remarks

            //}).FirstOrDefault();
            //if (model == null)
            //{
            //    model = new PaymentWindowViewModel();
            //    model.PaymentModeKey = DbConstants.PaymentMode.Cash;
            //    model.MasterKey = Id;
            //    model.Purpose = GetMonthNameFromNumber(employeeSalaryMaster.SalaryMonth) + " " + employeeSalaryMaster.SalaryYear.ToString() + " " + EduSuiteUIResources.Salary;
            //    model.ReceivedBy = employeeSalaryMaster.AppUser.FirstName + " " + (employeeSalaryMaster.AppUser.MiddleName ?? "") + " " + employeeSalaryMaster.AppUser.LastName;
            //    model.AmountToPay = dbContext.EmployeeSalaryMasters.Where(row => row.RowKey == Id).Select(row => row.TotalSalary).FirstOrDefault();
            //}

            try
            {
                PaymentWindowViewModel model = new PaymentWindowViewModel();
                EmployeeSalaryMaster employeeSalaryMaster = new EmployeeSalaryMaster();
                employeeSalaryMaster = dbContext.EmployeeSalaryMasters.SingleOrDefault(row => row.RowKey == Id);
                model = dbContext.EmployeeSalaryPayments.Where(x => x.EmployeeSalaryMasterKey == Id && (x.ChequeStatusKey != DbConstants.ProcessStatus.Rejected)).Select(row => new PaymentWindowViewModel
                {
                    MasterKey = row.EmployeeSalaryMasterKey,
                    ReceivedBy = row.ReceivedBy

                }).FirstOrDefault();

                if (model != null)
                {
                    string value = (dbContext.EmployeeSalaryPayments.Where(x => x.EmployeeSalaryMasterKey == Id && (x.ChequeStatusKey != DbConstants.ProcessStatus.Rejected)).Sum(row => row.PaidAmount)).ToString();
                    model.TotalReceivedAmount = value != null && value != "" ? Convert.ToDecimal(value) : 0;
                    model.TotalReceivedAmount = model.TotalReceivedAmount != null ? Convert.ToDecimal(model.TotalReceivedAmount) : 0;

                    string AmountToPay = employeeSalaryMaster.TotalSalary.ToString();
                    model.AmountToPay = AmountToPay != null && AmountToPay != "" ? Convert.ToDecimal(AmountToPay) : 0;

                    model.AmountToPay = model.AmountToPay - (model.TotalReceivedAmount ?? 0);
                    model.BillBalanceAmount = model.AmountToPay;
                    model.CashFlowTypeKey = DbConstants.CashFlowType.Out;
                    model.BranchKey = employeeSalaryMaster.Employee.BranchKey;
                    model.Purpose = GetMonthNameFromNumber(employeeSalaryMaster.SalaryMonth) + " " + employeeSalaryMaster.SalaryYear.ToString() + " " + EduSuiteUIResources.Salary;
                    model.ReceivedBy = employeeSalaryMaster.Employee.FirstName + " " + (employeeSalaryMaster.Employee.MiddleName ?? "") + " " + employeeSalaryMaster.Employee.LastName;
                    //model.AmountToPay = dbContext.EmployeeSalaryMasters.Where(row => row.RowKey == Id).Select(row => row.TotalSalary).FirstOrDefault();
                }
                else if (model == null)
                {
                    model = new PaymentWindowViewModel();
                    model.PaymentModeKey = DbConstants.PaymentMode.Cash;
                    model.MasterKey = Id;
                    string value = employeeSalaryMaster.TotalSalary.ToString();
                    model.AmountToPay = value != null && value != "" ? Convert.ToDecimal(value) : 0;
                    model.TotalReceivedAmount = 0;
                    model.AmountToPay = model.AmountToPay - (model.TotalReceivedAmount ?? 0);
                    model.CashFlowTypeKey = DbConstants.CashFlowType.Out;
                    model.BranchKey = employeeSalaryMaster.Employee.BranchKey;
                    model.Purpose = GetMonthNameFromNumber(employeeSalaryMaster.SalaryMonth) + " " + employeeSalaryMaster.SalaryYear.ToString() + " " + EduSuiteUIResources.Salary;
                    model.ReceivedBy = employeeSalaryMaster.Employee.FirstName + " " + (employeeSalaryMaster.Employee.MiddleName ?? "") + " " + employeeSalaryMaster.Employee.LastName;
                    //model.AmountToPay = dbContext.EmployeeSalaryMasters.Where(row => row.RowKey == Id).Select(row => row.TotalSalary).FirstOrDefault();
                }


                FillSalaryPaymentDropdownLists(model);

                return model;
            }
            catch (Exception ex)
            {
                PaymentWindowViewModel model = new PaymentWindowViewModel();
                //model.ExceptionMessage = ex.GetBaseException().Message;
                return model;
            }
        }
        public PaymentWindowViewModel CreateSalaryPayment(PaymentWindowViewModel model)
        {
            EmployeeSalaryPayment EmployeeSalaryPaymentModel = new EmployeeSalaryPayment();
            FillSalaryPaymentDropdownLists(model);

            using (var transaction = dbContext.Database.BeginTransaction())
            {
                try
                {
                    ConfigurationViewModel ConfigModel = new ConfigurationViewModel();
                    ConfigModel.BranchKey = model.PaidBranchKey ?? 0;
                    ConfigModel.ConfigType = DbConstants.PaymentReceiptConfigType.Payment;
                    Configurations.GenerateReceipt(dbContext, ConfigModel);
                    model.ReceiptNumber = ConfigModel.ReceiptNumber;

                    Int64 maxKey = dbContext.EmployeeSalaryPayments.Select(p => p.RowKey).DefaultIfEmpty().Max();
                    EmployeeSalaryPaymentModel.RowKey = Convert.ToInt64(maxKey + 1);
                    EmployeeSalaryPaymentModel.EmployeeSalaryMasterKey = model.MasterKey;
                    EmployeeSalaryPaymentModel.PaidAmount = Convert.ToDecimal(model.PaidAmount);
                    EmployeeSalaryPaymentModel.BalanceAmount = model.BalanceAmount;
                    EmployeeSalaryPaymentModel.PaymentDate = Convert.ToDateTime(model.PaymentDate);
                    EmployeeSalaryPaymentModel.PaymentModeKey = model.PaymentModeKey;
                    EmployeeSalaryPaymentModel.BankAccountKey = model.BankAccountKey;
                    EmployeeSalaryPaymentModel.PaymentModeKey = model.PaymentModeKey;
                    EmployeeSalaryPaymentModel.PaymentModeSubKey = model.PaymentModeSubKey;
                    EmployeeSalaryPaymentModel.CardNumber = model.CardNumber;
                    EmployeeSalaryPaymentModel.BankAccountKey = model.BankAccountKey;
                    EmployeeSalaryPaymentModel.ChequeOrDDNumber = model.ChequeOrDDNumber;
                    EmployeeSalaryPaymentModel.ReferenceNumber = model.ReferenceNumber;
                    EmployeeSalaryPaymentModel.ChequeOrDDDate = model.ChequeOrDDDate;
                    EmployeeSalaryPaymentModel.Purpose = model.Purpose;
                    EmployeeSalaryPaymentModel.PaidBy = model.PaidBy;
                    EmployeeSalaryPaymentModel.AuthorizedBy = model.AuthorizedBy;
                    EmployeeSalaryPaymentModel.ReceivedBy = model.ReceivedBy;
                    EmployeeSalaryPaymentModel.OnBehalfOf = model.OnBehalfOf;
                    EmployeeSalaryPaymentModel.Remarks = model.Remarks;
                    EmployeeSalaryPaymentModel.PaidBranchKey = model.PaidBranchKey;
                    EmployeeSalaryPaymentModel.SerialNumber = ConfigModel.SerialNumber;
                    EmployeeSalaryPaymentModel.VoucherNumber = ConfigModel.ReceiptNumber;
                    if (model.PaymentModeKey == DbConstants.PaymentMode.Cheque)
                    {
                        EmployeeSalaryPaymentModel.ChequeStatusKey = DbConstants.ProcessStatus.Pending;
                    }
                    long bankKey = 0;
                    model.PaymentKey = EmployeeSalaryPaymentModel.RowKey;
                    //CreateAccount(model);
                    dbContext.EmployeeSalaryPayments.Add(EmployeeSalaryPaymentModel);
                    var employeeMaster = dbContext.EmployeeSalaryMasters.SingleOrDefault(x => x.RowKey == model.MasterKey);
                    model.BranchKey = employeeMaster.Employee.BranchKey;
                    model.EmployeeName = employeeMaster.Employee.FirstName;
                    model.IsUpdate = false;
                    if (model.BankAccountKey != null && model.BankAccountKey != 0)
                    {
                        var BankAccountList = dbContext.BankAccounts.SingleOrDefault(x => x.RowKey == model.BankAccountKey);
                        model.BankAccountName = (BankAccountList.NameInAccount ?? BankAccountList.AccountNumber) + "-" + BankAccountList.Bank.BankName;
                    }
                    //if (model.PaymentModeKey == DbConstants.PaymentMode.Bank)
                    //{
                    //    BankAccountService bankAccountService = new BankAccountService(dbContext);
                    //    BankAccountViewModel bankAccountModel = new BankAccountViewModel();
                    //    bankAccountModel.RowKey = model.BankAccountKey ?? 0;
                    //    bankAccountModel.Amount = model.PaidAmount;
                    //    bankAccountService.UpdateCurrentAccountBalance(bankAccountModel, false, true, null);
                    //}
                    List<AccountFlowViewModel> accountFlowModelList = new List<AccountFlowViewModel>();
                    accountFlowModelList = SalaryPaidAmountList(model, accountFlowModelList, bankKey, employeeMaster.EmployeeKey);
                    CreateAccountFlow(accountFlowModelList, false);

                    dbContext.SaveChanges();
                    transaction.Commit();
                    model.Message = EduSuiteUIResources.Success;
                    model.IsSuccessful = true;

                    model.PaymentKey = EmployeeSalaryPaymentModel.RowKey;

                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    //model.ExceptionMessage = ex.GetBaseException().Message;
                    model.Message = String.Format(EduSuiteUIResources.FailedToSaveErrorMessage, EduSuiteUIResources.SalaryPayment);
                    model.IsSuccessful = false;
                }

            }
            return model;
        }
        public PaymentWindowViewModel UpdateSalaryPayment(PaymentWindowViewModel model)
        {
            EmployeeSalaryPayment EmployeeSalaryPaymentModel = new EmployeeSalaryPayment();
            FillSalaryPaymentDropdownLists(model);

            using (var transaction = dbContext.Database.BeginTransaction())
            {
                try
                {
                    EmployeeSalaryPaymentModel = dbContext.EmployeeSalaryPayments.SingleOrDefault(row => row.RowKey == model.PaymentKey);
                    EmployeeSalaryPaymentModel.PaidAmount = Convert.ToDecimal(model.PaidAmount);
                    EmployeeSalaryPaymentModel.BalanceAmount = model.BalanceAmount;
                    EmployeeSalaryPaymentModel.PaymentDate = Convert.ToDateTime(model.PaymentDate);
                    model.OldPaymentModeKey = EmployeeSalaryPaymentModel.PaymentModeKey;
                    long oldBank = EmployeeSalaryPaymentModel.BankAccountKey ?? 0;
                    EmployeeSalaryPaymentModel.PaymentModeKey = model.PaymentModeKey;
                    EmployeeSalaryPaymentModel.PaymentModeSubKey = model.PaymentModeSubKey;
                    EmployeeSalaryPaymentModel.BankAccountKey = model.BankAccountKey;
                    EmployeeSalaryPaymentModel.CardNumber = model.CardNumber;
                    EmployeeSalaryPaymentModel.BankAccountKey = model.BankAccountKey;
                    EmployeeSalaryPaymentModel.ChequeOrDDNumber = model.ChequeOrDDNumber;
                    EmployeeSalaryPaymentModel.ChequeOrDDDate = model.ChequeOrDDDate;
                    EmployeeSalaryPaymentModel.Purpose = model.Purpose;
                    EmployeeSalaryPaymentModel.PaidBy = model.PaidBy;
                    EmployeeSalaryPaymentModel.ReferenceNumber = model.ReferenceNumber;
                    EmployeeSalaryPaymentModel.AuthorizedBy = model.AuthorizedBy;
                    EmployeeSalaryPaymentModel.ReceivedBy = model.ReceivedBy;
                    EmployeeSalaryPaymentModel.OnBehalfOf = model.OnBehalfOf;
                    EmployeeSalaryPaymentModel.Remarks = model.Remarks;
                    EmployeeSalaryPaymentModel.PaidBranchKey = model.PaidBranchKey;
                    if (model.PaymentModeKey == DbConstants.PaymentMode.Cheque)
                    {
                        EmployeeSalaryPaymentModel.ChequeStatusKey = DbConstants.ProcessStatus.Pending;
                    }
                    var employeeMaster = dbContext.EmployeeSalaryMasters.SingleOrDefault(x => x.RowKey == model.MasterKey);
                    model.BranchKey = employeeMaster.Employee.BranchKey;
                    model.EmployeeName = employeeMaster.Employee.FirstName;
                    if (model.BankAccountKey != null && model.BankAccountKey != 0)
                    {
                        var BankAccountList = dbContext.BankAccounts.SingleOrDefault(x => x.RowKey == model.BankAccountKey);
                        model.BankAccountName = (BankAccountList.NameInAccount ?? BankAccountList.AccountNumber) + "-" + BankAccountList.Bank.BankName;
                    }

                    //if (DbConstants.PaymentMode.BankPaymentModes.Contains(model.OldPaymentModeKey) && oldBank != (model.BankAccountKey ?? 0))
                    //{
                    //    BankAccountService bankAccountService = new BankAccountService(dbContext);
                    //    BankAccountViewModel bankAccountModel = new BankAccountViewModel();
                    //    bankAccountModel.RowKey = oldBank;
                    //    bankAccountModel.Amount = -(model.OldAmount);
                    //    bankAccountService.UpdateCurrentAccountBalance(bankAccountModel, true, true, model.OldAmount);
                    //}

                    //if (DbConstants.PaymentMode.BankPaymentModes.Contains(model.PaymentModeKey))
                    //{
                    //    BankAccountService bankAccountService = new BankAccountService(dbContext);
                    //    BankAccountViewModel bankAccountModel = new BankAccountViewModel();
                    //    bankAccountModel.RowKey = model.BankAccountKey ?? 0;
                    //    bankAccountModel.Amount = model.PaidAmount;
                    //    bankAccountService.UpdateCurrentAccountBalance(bankAccountModel, true, true, model.OldAmount);
                    //}

                    //if (model.PaymentModeKey == DbConstants.PaymentMode.Bank)
                    //{
                    //    BankAccountService bankAccountService = new BankAccountService(dbContext);
                    //    BankAccountViewModel bankAccountModel = new BankAccountViewModel();
                    //    bankAccountModel.RowKey = model.BankAccountKey ?? 0;
                    //    bankAccountModel.Amount = model.PaidAmount;
                    //    bankAccountService.UpdateCurrentAccountBalance(bankAccountModel);
                    //    if (model.OldPaymentModeKey == DbConstants.PaymentMode.Bank)
                    //    {
                    //        bankAccountModel.RowKey = oldBank;
                    //        bankAccountModel.Amount = -(model.OldAmount);
                    //        bankAccountService.UpdateCurrentAccountBalance(bankAccountModel);
                    //    }
                    //}
                    //else if (model.OldPaymentModeKey == DbConstants.PaymentMode.Bank)
                    //{
                    //    BankAccountService bankAccountService = new BankAccountService(dbContext);
                    //    BankAccountViewModel bankAccountModel = new BankAccountViewModel();
                    //    bankAccountModel.RowKey = oldBank;
                    //    bankAccountModel.Amount = -(model.OldAmount);
                    //    bankAccountService.UpdateCurrentAccountBalance(bankAccountModel);
                    //}
                    //UpdateAccount(model);
                    model.IsUpdate = true;
                    List<AccountFlowViewModel> accountFlowModelList = new List<AccountFlowViewModel>();
                    accountFlowModelList = SalaryPaidAmountList(model, accountFlowModelList, oldBank, employeeMaster.EmployeeKey);
                    CreateAccountFlow(accountFlowModelList, true);

                    dbContext.SaveChanges();
                    transaction.Commit();
                    model.Message = EduSuiteUIResources.Success;
                    model.IsSuccessful = true;
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    //model.ExceptionMessage = ex.GetBaseException().Message;
                    model.Message = String.Format(EduSuiteUIResources.FailedToSaveErrorMessage, EduSuiteUIResources.SalaryPayment);
                    model.IsSuccessful = false;
                }

            }
            return model;
        }
        public PaymentWindowViewModel DeleteSalaryPayment(long? PaymentKey)
        {
            PaymentWindowViewModel model = new PaymentWindowViewModel();
            using (var Transaction = dbContext.Database.BeginTransaction())
            {
                try
                {
                    EmployeeSalaryPayment EmployeeSalaryPaymentsmodel = dbContext.EmployeeSalaryPayments.Where(row => row.RowKey == PaymentKey).FirstOrDefault();
                    long RowKey = EmployeeSalaryPaymentsmodel.RowKey;

                    ConfigurationViewModel ConfigModel = new ConfigurationViewModel();
                    ConfigModel.BranchKey = EmployeeSalaryPaymentsmodel.PaidBranchKey ?? 0;
                    ConfigModel.SerialNumber = EmployeeSalaryPaymentsmodel.SerialNumber ?? 0;
                    ConfigModel.IsDelete = true;
                    ConfigModel.ConfigType = DbConstants.PaymentReceiptConfigType.Payment;
                    Configurations.GenerateReceipt(dbContext, ConfigModel);

                    dbContext.EmployeeSalaryPayments.Remove(EmployeeSalaryPaymentsmodel);
                    AccountFlowViewModel accountFlowModel = new AccountFlowViewModel();
                    AccountFlowService accountFlowService = new AccountFlowService(dbContext);
                    accountFlowModel.TransactionTypeKey = DbConstants.TransactionType.Salary;
                    accountFlowModel.TransactionKey = RowKey;
                    accountFlowService.DeleteAccountFlow(accountFlowModel);
                    dbContext.SaveChanges();
                    Transaction.Commit();
                    model.Message = EduSuiteUIResources.Success;
                    model.IsSuccessful = true;
                    ActivityLog.CreateActivityLog(MenuConstants.EmployeeSalary, ActionConstants.Delete, DbConstants.LogType.Info, PaymentKey, model.Message);

                }
                catch (Exception ex)
                {
                    Transaction.Rollback();
                    model.Message = String.Format(EduSuiteUIResources.FailedToSaveErrorMessage, EduSuiteUIResources.SalaryPayment);
                    model.IsSuccessful = false;
                    ActivityLog.CreateActivityLog(MenuConstants.EmployeeSalary, ActionConstants.Delete, DbConstants.LogType.Error, PaymentKey, ex.GetBaseException().Message);
                }
            }

            return model;
        }
        private void CreateAccount(PaymentWindowViewModel model)
        {
            AccountManagement accountManagement = new AccountManagement(dbContext);
            CashFlowViewModel cashFlowModel = new CashFlowViewModel();
            EmployeeSalaryMaster employeeSalaryMaster = dbContext.EmployeeSalaryMasters.SingleOrDefault(row => row.RowKey == model.MasterKey);
            cashFlowModel.CashFlowDate = Convert.ToDateTime(model.PaymentDate);
            cashFlowModel.CashFlowTypeKey = DbConstants.CashFlowType.Out;
            cashFlowModel.PartyKey = employeeSalaryMaster.EmployeeKey;
            //cashFlowModel.PartyTypeKey = DbConstants.PartyType.AppUser;
            //cashFlowModel.VoucherNumber = model.VoucherNumber;
            cashFlowModel.Amount = Convert.ToDecimal(model.PaidAmount);
            cashFlowModel.PaymentModeKey = model.PaymentModeKey;
            cashFlowModel.BankAccountKey = model.BankAccountKey;
            //cashFlowModel.TransactionTypeKey = DbConstants.TransactionType.Salary;
            cashFlowModel.TransactionKey = model.PaymentKey;
            cashFlowModel.Purpose = model.Purpose;
            cashFlowModel.PaidBy = model.PaidBy;
            cashFlowModel.AuthorizedBy = model.AuthorizedBy;
            cashFlowModel.ReceivedBy = model.ReceivedBy;
            cashFlowModel.OnBehalfOf = model.OnBehalfOf;
            cashFlowModel.Remarks = model.Remarks;
            cashFlowModel.BranchKey = employeeSalaryMaster.Employee.BranchKey;

            //accountManagement.CreateCashFlowAccount(cashFlowModel);
        }

        private void UpdateAccount(PaymentWindowViewModel model)
        {
            AccountManagement accountManagement = new AccountManagement(dbContext);
            CashFlowViewModel cashFlowModel = new CashFlowViewModel();
            EmployeeSalaryMaster employeeSalaryMaster = dbContext.EmployeeSalaryMasters.SingleOrDefault(row => row.RowKey == model.MasterKey);

            cashFlowModel.CashFlowDate = Convert.ToDateTime(model.PaymentDate);
            cashFlowModel.CashFlowTypeKey = DbConstants.CashFlowType.Out;
            cashFlowModel.PartyKey = employeeSalaryMaster.EmployeeKey;
            // cashFlowModel.PartyTypeKey = DbConstants.PartyType.AppUser;
            //cashFlowModel.VoucherNumber = model.VoucherNumber;
            cashFlowModel.Amount = Convert.ToDecimal(model.PaidAmount);
            cashFlowModel.PaymentModeKey = model.PaymentModeKey;
            cashFlowModel.BankAccountKey = model.BankAccountKey;
            //cashFlowModel.TransactionTypeKey = DbConstants.TransactionType.Salary;
            cashFlowModel.TransactionKey = model.PaymentKey;
            cashFlowModel.Purpose = model.Purpose;
            cashFlowModel.PaidBy = model.PaidBy;
            cashFlowModel.AuthorizedBy = model.AuthorizedBy;
            cashFlowModel.ReceivedBy = model.ReceivedBy;
            cashFlowModel.OnBehalfOf = model.OnBehalfOf;
            cashFlowModel.Remarks = model.Remarks;
            cashFlowModel.BranchKey = employeeSalaryMaster.Employee.BranchKey;

            accountManagement.UpdateCashFlowAccount(cashFlowModel);
        }

        private void FillSalaryPaymentDropdownLists(PaymentWindowViewModel model)
        {
            FillPaymentModes(model);
            FillPaymentModeSub(model);
            FillBankAccounts(model);
            FillPaidBranches(model);
        }

        private void FillPaymentModes(PaymentWindowViewModel model)
        {
            model.PaymentModes = dbContext.VwPaymentModeSelectActiveOnlies.OrderBy(row => row.DisplayOrder).Select(row => new SelectListModel
            {
                RowKey = row.RowKey,
                Text = row.PaymentModeName
            }).ToList();
        }

        public PaymentWindowViewModel FillPaymentModeSub(PaymentWindowViewModel model)
        {
            model.PaymentModeSub = dbContext.PaymentModeSubs.Where(x => x.IsActive && x.PaymentModeKey == DbConstants.PaymentMode.Bank).Select(row => new SelectListModel
            {
                RowKey = row.RowKey,
                Text = row.PaymentModeSubName
            }).ToList();
            return model;
        }

        private void FillBankAccounts(PaymentWindowViewModel model)
        {
            var BranchKey = dbContext.EmployeeSalaryMasters.Where(row => row.RowKey == model.MasterKey).Select(row => row.Employee.BranchKey).FirstOrDefault();
            //model.BankAccounts = dbContext.BankAccounts.Where(row => row.IsActive == true && (row.BranchKey == BranchKey || row.BranchKey == null)).Select(row => new SelectListModel
            //{
            //    RowKey = row.RowKey,
            //    Text = (row.NameInAccount ?? row.AccountNumber) + "-" + row.Bank.BankName
            //}).ToList();

            model.BankAccounts = dbContext.BranchAccounts.Where(x => x.BranchKey == BranchKey && x.BankAccount.IsActive).Select(row => new SelectListModel
            {
                RowKey = row.BankAccount.RowKey,
                Text = (row.BankAccount.NameInAccount ?? row.BankAccount.AccountNumber) + EduSuiteUIResources.Hyphen + row.BankAccount.Bank.BankName
            }).ToList();
        }

        private void FillPaymentModes(EmployeeSalaryAdvanceViewModel model)
        {
            model.PaymentModes = dbContext.VwPaymentModeSelectActiveOnlies.OrderBy(row => row.DisplayOrder).Select(row => new SelectListModel
            {
                RowKey = row.RowKey,
                Text = row.PaymentModeName
            }).ToList();
        }

        public EmployeeSalaryAdvanceViewModel FillPaymentModeSub(EmployeeSalaryAdvanceViewModel model)
        {
            model.PaymentModeSub = dbContext.PaymentModeSubs.Where(x => x.IsActive && x.PaymentModeKey == DbConstants.PaymentMode.Bank).Select(row => new SelectListModel
            {
                RowKey = row.RowKey,
                Text = row.PaymentModeSubName
            }).ToList();
            return model;
        }
        public EmployeeSalaryAdvanceViewModel FillBankAccounts(EmployeeSalaryAdvanceViewModel model)
        {
            model.BankAccounts = dbContext.BranchAccounts.Where(x => x.BranchKey == model.BranchKey && x.BankAccount.IsActive).Select(row => new SelectListModel
            {
                RowKey = row.BankAccount.RowKey,
                Text = (row.BankAccount.NameInAccount ?? row.BankAccount.AccountNumber) + EduSuiteUIResources.Hyphen + row.BankAccount.Bank.BankName
            }).ToList();

            return model;
        }

        public decimal GetBalanceforAdvance(short PaymentModeKey, long Rowkey, long BankAccountKey, short branchKey)
        {
            decimal Balance = 0;
            if (PaymentModeKey == DbConstants.PaymentMode.Cash)
            {
                long accountHeadKey = dbContext.AccountHeads.Where(x => x.RowKey == DbConstants.AccountHead.CashAccount).Select(x => x.RowKey).FirstOrDefault();
                decimal totalDebit = dbContext.AccountFlows.Where(x => x.AccountHeadKey == accountHeadKey && x.CashFlowTypeKey == DbConstants.CashFlowType.In && x.BranchKey == branchKey).Select(x => x.Amount).DefaultIfEmpty().Sum();
                decimal totalCredit = dbContext.AccountFlows.Where(x => x.AccountHeadKey == accountHeadKey && x.CashFlowTypeKey == DbConstants.CashFlowType.Out && x.BranchKey == branchKey).Select(x => x.Amount).DefaultIfEmpty().Sum();
                Balance = totalDebit - totalCredit;
                if (Rowkey != 0)
                {
                    var purchaseList = dbContext.EmployeeSalaryAdvancePayments.SingleOrDefault(x => x.RowKey == Rowkey);
                    if (purchaseList != null)
                        if (PaymentModeKey == purchaseList.PaymentModeKey)
                        {
                            Balance = Balance + purchaseList.PaidAmount;
                        }
                }
            }
            else if (PaymentModeKey == DbConstants.PaymentMode.Bank || PaymentModeKey == DbConstants.PaymentMode.Cheque)
            {
                if (BankAccountKey != 0 && BankAccountKey != null)
                {
                    long accountHeadKey = dbContext.BankAccounts.Where(x => x.RowKey == BankAccountKey).Select(x => x.AccountHeadKey).FirstOrDefault();
                    decimal totalDebit = dbContext.AccountFlows.Where(x => x.AccountHeadKey == accountHeadKey && x.CashFlowTypeKey == DbConstants.CashFlowType.In).Select(x => x.Amount).DefaultIfEmpty().Sum();
                    decimal totalCredit = dbContext.AccountFlows.Where(x => x.AccountHeadKey == accountHeadKey && x.CashFlowTypeKey == DbConstants.CashFlowType.Out).Select(x => x.Amount).DefaultIfEmpty().Sum();
                    Balance = totalDebit - totalCredit;

                    if (Rowkey != 0)
                    {
                        var purchaseList = dbContext.EmployeeSalaryAdvancePayments.SingleOrDefault(x => x.RowKey == Rowkey);
                        if (purchaseList != null)
                            if (BankAccountKey == purchaseList.BankAccountKey)
                            {
                                Balance = Balance + purchaseList.PaidAmount;
                            }
                    }
                }
            }
            return Balance;
        }

        #endregion

        #region Account
        private void CreateAccountFlow(List<AccountFlowViewModel> accountFlowModelList, bool isUpadte)
        {
            AccountFlowService accounFlowService = new AccountFlowService(dbContext);
            if (isUpadte == false)
            {
                accounFlowService.CreateAccountFlow(accountFlowModelList);
            }
            else
            {
                accounFlowService.UpdateAccountFlow(accountFlowModelList);
            }
        }
        private List<AccountFlowViewModel> PayableAmountList(EmployeeSalaryMasterViewModel model, List<AccountFlowViewModel> accountFlowModelList, decimal amount, bool isUpdate)
        {
            long accountHeadKey;
            long ExtraUpdateKey = 0;
            //accountHeadKey = dbContext.VwAccountHeadSelectActiveOnlies.Where(x => x.RowKey == DbConstants.AccountHead.SalaryPayable).Select(x => x.RowKey).FirstOrDefault();
            accountHeadKey = dbContext.Employees.Where(x => x.RowKey == model.EmployeeKey).Select(x => x.AccountHeadKey ?? 0).FirstOrDefault();
            accountFlowModelList.Add(new AccountFlowViewModel
            {
                CashFlowTypeKey = DbConstants.CashFlowType.Out,
                AccountHeadKey = accountHeadKey,
                Amount = amount,
                TransactionTypeKey = DbConstants.TransactionType.SalaryPayable,
                VoucherTypeKey = DbConstants.VoucherType.Salary,
                TransactionKey = model.SalaryMasterKey,
                TransactionDate = new DateTime(model.SalaryYearKey, model.SalaryMonthKey, DateTime.DaysInMonth(model.SalaryYearKey, model.SalaryMonthKey)),
                ExtraUpdateKey = 0,
                IsUpdate = isUpdate,
                BranchKey = model.BranchKey,
                Purpose = model.SalaryMonthName + EduSuiteUIResources.BlankSpace + EduSuiteUIResources.Salary + EduSuiteUIResources.BlankSpace + EduSuiteUIResources.of + EduSuiteUIResources.BlankSpace + model.EmployeeName,
            });
            return accountFlowModelList;
        }
        private List<AccountFlowViewModel> SalaryAmountList(EmployeeSalaryMasterViewModel model, List<AccountFlowViewModel> accountFlowModelList, decimal amount, bool isUpdate)
        {
            long accountHeadKey;
            long ExtraUpdateKey = 0;
            accountHeadKey = dbContext.VwAccountHeadSelectActiveOnlies.Where(x => x.RowKey == DbConstants.AccountHead.Salary).Select(x => x.RowKey).FirstOrDefault();
            accountFlowModelList.Add(new AccountFlowViewModel
            {
                CashFlowTypeKey = DbConstants.CashFlowType.In,
                AccountHeadKey = accountHeadKey,
                Amount = amount,
                TransactionTypeKey = DbConstants.TransactionType.SalaryPayable,
                VoucherTypeKey = DbConstants.VoucherType.Salary,
                TransactionKey = model.SalaryMasterKey,
                TransactionDate = new DateTime(model.SalaryYearKey, model.SalaryMonthKey, DateTime.DaysInMonth(model.SalaryYearKey, model.SalaryMonthKey)),
                ExtraUpdateKey = 0,
                IsUpdate = isUpdate,
                BranchKey = model.BranchKey,
                Purpose = model.SalaryMonthName + EduSuiteUIResources.BlankSpace + EduSuiteUIResources.Salary + EduSuiteUIResources.BlankSpace + EduSuiteUIResources.of + EduSuiteUIResources.BlankSpace + model.EmployeeName,
            });
            return accountFlowModelList;
        }
        private List<AccountFlowViewModel> AdvanceAmountList(EmployeeSalaryAdvanceViewModel model, List<AccountFlowViewModel> accountFlowModelList, long oldBankKey)
        {
            long accountHeadKey;
            long ExtraUpdateKey = 0;
            //accountHeadKey = dbContext.VwAccountHeadSelectActiveOnlies.Where(x => x.RowKey == DbConstants.AccountHead.SalaryAdvanceRecievable).Select(x => x.RowKey).FirstOrDefault();
            accountHeadKey = dbContext.Employees.Where(x => x.RowKey == model.EmployeeKey).Select(x => x.AccountHeadKey ?? 0).FirstOrDefault();
            accountFlowModelList.Add(new AccountFlowViewModel
            {
                CashFlowTypeKey = DbConstants.CashFlowType.In,
                AccountHeadKey = accountHeadKey,
                Amount = model.PaidAmount ?? 0,
                TransactionTypeKey = DbConstants.TransactionType.SalaryAdvance,
                VoucherTypeKey = DbConstants.VoucherType.SalaryAdvance,
                TransactionKey = model.PaymentKey,
                TransactionDate = model.PaymentModeKey != DbConstants.PaymentMode.Cheque ? model.PaymentDate : (model.ChequeOrDDDate ?? model.PaymentDate),
                ExtraUpdateKey = ExtraUpdateKey,
                IsUpdate = model.IsUpdate,
                BranchKey = model.PaidBranchKey != null ? model.PaidBranchKey : model.BranchKey,
                Purpose = EduSuiteUIResources.Salary + EduSuiteUIResources.BlankSpace + EduSuiteUIResources.Advance + EduSuiteUIResources.BlankSpace + EduSuiteUIResources.Recieved + EduSuiteUIResources.BlankSpace + model.EmployeeName + EduSuiteUIResources.BlankSpace + (model.Purpose != null ? (EduSuiteUIResources.For + EduSuiteUIResources.BlankSpace + model.Purpose) : "") +
                          EduSuiteUIResources.BlankSpace + EduSuiteUIResources.By + EduSuiteUIResources.BlankSpace + (model.PaymentModeKey == DbConstants.PaymentMode.Cash ? EduSuiteUIResources.Cash : model.PaymentModeKey == DbConstants.PaymentMode.Bank ? EduSuiteUIResources.Bank + EduSuiteUIResources.OpenBracket + model.BankAccountName + EduSuiteUIResources.CloseBracket : model.PaymentModeKey == DbConstants.PaymentMode.Cheque ? EduSuiteUIResources.Cheque + EduSuiteUIResources.OpenBracket + model.BankAccountName + EduSuiteUIResources.OpenBracket : ""),
            });

            long oldBankAccountHeadKey = 0;
            if (model.OldPaymentModeKey == DbConstants.PaymentMode.Bank || model.OldPaymentModeKey == DbConstants.PaymentMode.Cheque)
            {
                long bankaccountHeadKey = dbContext.BankAccounts.Where(x => x.RowKey == oldBankKey).Select(x => x.AccountHeadKey).FirstOrDefault();
                //oldBankAccountHeadKey = dbContext.VwAccountHeadSelectActiveOnlies.Where(x => x.AccountHeadCode == bankAccountCode).Select(x => x.RowKey).FirstOrDefault();
                oldBankAccountHeadKey = bankaccountHeadKey;
            }
            if (model.PaymentModeKey == DbConstants.PaymentMode.Bank || model.PaymentModeKey == DbConstants.PaymentMode.Cheque)
            {
                accountHeadKey = dbContext.BankAccounts.Where(x => x.RowKey == model.BankAccountKey).Select(x => x.AccountHeadKey).FirstOrDefault();
            }
            else
            {
                accountHeadKey = dbContext.VwAccountHeadSelectActiveOnlies.Where(x => x.RowKey == DbConstants.AccountHead.CashAccount).Select(x => x.RowKey).FirstOrDefault();
            }
            if (model.OldPaymentModeKey != null && model.OldPaymentModeKey != 0 && model.OldPaymentModeKey != model.PaymentModeKey)
            {
                model.IsUpdate = false;
                ExtraUpdateKey = model.OldPaymentModeKey == DbConstants.PaymentMode.Cash ? DbConstants.AccountHead.CashAccount : oldBankAccountHeadKey;
            }
            accountFlowModelList.Add(new AccountFlowViewModel
            {
                CashFlowTypeKey = DbConstants.CashFlowType.Out,
                AccountHeadKey = accountHeadKey,
                Amount = model.PaidAmount ?? 0,
                TransactionTypeKey = DbConstants.TransactionType.SalaryAdvance,
                VoucherTypeKey = DbConstants.VoucherType.SalaryAdvance,
                TransactionDate = model.PaymentModeKey != DbConstants.PaymentMode.Cheque ? model.PaymentDate : (model.ChequeOrDDDate ?? model.PaymentDate),
                ExtraUpdateKey = ExtraUpdateKey,
                IsUpdate = model.IsUpdate,
                Purpose = EduSuiteUIResources.Salary + EduSuiteUIResources.BlankSpace + EduSuiteUIResources.Advance + EduSuiteUIResources.BlankSpace + EduSuiteUIResources.Recieved + EduSuiteUIResources.BlankSpace + model.EmployeeName + (model.Purpose != null ? (EduSuiteUIResources.For + EduSuiteUIResources.BlankSpace + model.Purpose) : ""),
                BranchKey = model.PaidBranchKey != null ? model.PaidBranchKey : model.BranchKey,
                TransactionKey = model.PaymentKey,
            });
            return accountFlowModelList;
        }
        private List<AccountFlowViewModel> SalaryPaidAmountList(PaymentWindowViewModel model, List<AccountFlowViewModel> accountFlowModelList, long oldBankKey, long EmployeeKey)
        {
            string VoucherNumber = EduSuiteUIResources.OpenBracketWithSpace + model.ReceiptNumber + EduSuiteUIResources.ClosingBracketWithSpace;
            long accountHeadKey;
            long ExtraUpdateKey = 0;
            // accountHeadKey = dbContext.VwAccountHeadSelectActiveOnlies.Where(x => x.RowKey == DbConstants.AccountHead.SalaryPayable).Select(x => x.RowKey).FirstOrDefault();
            accountHeadKey = dbContext.Employees.Where(x => x.RowKey == EmployeeKey).Select(x => x.AccountHeadKey ?? 0).FirstOrDefault();
            accountFlowModelList.Add(new AccountFlowViewModel
            {
                CashFlowTypeKey = DbConstants.CashFlowType.In,
                AccountHeadKey = accountHeadKey,
                Amount = model.PaidAmount ?? 0,
                TransactionTypeKey = DbConstants.TransactionType.Salary,
                VoucherTypeKey = DbConstants.VoucherType.Salary,
                TransactionKey = model.PaymentKey,
                TransactionDate = model.PaymentModeKey != DbConstants.PaymentMode.Cheque ? model.PaymentDate : (model.ChequeOrDDDate ?? model.PaymentDate),
                ExtraUpdateKey = ExtraUpdateKey,
                IsUpdate = model.IsUpdate,
                BranchKey = model.PaidBranchKey != null ? model.PaidBranchKey : model.BranchKey,
                Purpose = model.Purpose + EduSuiteUIResources.BlankSpace + EduSuiteUIResources.PaidTo + EduSuiteUIResources.BlankSpace + model.EmployeeName + EduSuiteUIResources.BlankSpace + EduSuiteUIResources.By + EduSuiteUIResources.BlankSpace + (model.PaymentModeKey == DbConstants.PaymentMode.Cash ? EduSuiteUIResources.Cash : model.PaymentModeKey == DbConstants.PaymentMode.Bank ? EduSuiteUIResources.Bank + EduSuiteUIResources.OpenBracket + model.BankAccountName + EduSuiteUIResources.CloseBracket : model.PaymentModeKey == DbConstants.PaymentMode.Cheque ? EduSuiteUIResources.Cheque + EduSuiteUIResources.OpenBracket + model.BankAccountName + EduSuiteUIResources.CloseBracket : "") + VoucherNumber,
            });

            long oldBankAccountHeadKey = 0;
            if (model.OldPaymentModeKey == DbConstants.PaymentMode.Bank || model.OldPaymentModeKey == DbConstants.PaymentMode.Cheque)
            {
                long bankaccountHeadKey = dbContext.BankAccounts.Where(x => x.RowKey == oldBankKey).Select(x => x.AccountHeadKey).FirstOrDefault();
                //oldBankAccountHeadKey = dbContext.VwAccountHeadSelectActiveOnlies.Where(x => x.AccountHeadCode == bankAccountCode).Select(x => x.RowKey).FirstOrDefault();
                oldBankAccountHeadKey = bankaccountHeadKey;
            }
            if (model.PaymentModeKey == DbConstants.PaymentMode.Bank || model.PaymentModeKey == DbConstants.PaymentMode.Cheque)
            {
                accountHeadKey = dbContext.BankAccounts.Where(x => x.RowKey == model.BankAccountKey).Select(x => x.AccountHeadKey).FirstOrDefault();
            }
            else
            {
                accountHeadKey = dbContext.VwAccountHeadSelectActiveOnlies.Where(x => x.RowKey == DbConstants.AccountHead.CashAccount).Select(x => x.RowKey).FirstOrDefault();
            }
            if (model.OldPaymentModeKey != null && model.OldPaymentModeKey != 0 && model.OldPaymentModeKey != model.PaymentModeKey)
            {
                model.IsUpdate = false;
                ExtraUpdateKey = model.OldPaymentModeKey == DbConstants.PaymentMode.Cash ? DbConstants.AccountHead.CashAccount : oldBankAccountHeadKey;
            }
            accountFlowModelList.Add(new AccountFlowViewModel
            {
                CashFlowTypeKey = DbConstants.CashFlowType.Out,
                AccountHeadKey = accountHeadKey,
                Amount = model.PaidAmount ?? 0,
                TransactionTypeKey = DbConstants.TransactionType.Salary,
                VoucherTypeKey = DbConstants.VoucherType.Salary,
                TransactionDate = model.PaymentModeKey != DbConstants.PaymentMode.Cheque ? model.PaymentDate : (model.ChequeOrDDDate ?? model.PaymentDate),
                ExtraUpdateKey = ExtraUpdateKey,
                IsUpdate = model.IsUpdate,
                Purpose = model.Purpose + EduSuiteUIResources.BlankSpace + EduSuiteUIResources.PaidTo + EduSuiteUIResources.BlankSpace + model.EmployeeName + VoucherNumber,
                BranchKey = model.PaidBranchKey != null ? model.PaidBranchKey : model.BranchKey,
                TransactionKey = model.PaymentKey,
            });
            return accountFlowModelList;
        }
        private List<AccountFlowViewModel> SalaryAdvanceAmountList(EmployeeSalaryMasterViewModel model, List<AccountFlowViewModel> accountFlowModelList, decimal Amount, bool isUpdate)
        {
            long accountHeadKey;
            long ExtraUpdateKey = 0;
            // accountHeadKey = dbContext.VwAccountHeadSelectActiveOnlies.Where(x => x.RowKey == DbConstants.AccountHead.SalaryAdvanceRecievable).Select(x => x.RowKey).FirstOrDefault();
            accountHeadKey = dbContext.Employees.Where(x => x.RowKey == model.EmployeeKey).Select(x => x.AccountHeadKey ?? 0).FirstOrDefault();
            accountFlowModelList.Add(new AccountFlowViewModel
            {
                CashFlowTypeKey = DbConstants.CashFlowType.Out,
                AccountHeadKey = accountHeadKey,
                Amount = Amount,
                TransactionTypeKey = DbConstants.TransactionType.SalaryAdvanceRecieved,
                VoucherTypeKey = DbConstants.VoucherType.Salary,
                TransactionKey = model.SalaryMasterKey,
                TransactionDate = DateTimeUTC.Now,
                ExtraUpdateKey = ExtraUpdateKey,
                IsUpdate = isUpdate,
                BranchKey = model.BranchKey,
                Purpose = model.SalaryMonthName + EduSuiteUIResources.BlankSpace + EduSuiteUIResources.Salary + EduSuiteUIResources.BlankSpace + EduSuiteUIResources.of + EduSuiteUIResources.BlankSpace + model.EmployeeName,
            });
            return accountFlowModelList;
        }

        #endregion

        public List<SalaryPaymentSlipViewModel> GetSalaryPaymentSlipByIds(long Id)
        {
            List<SalaryPaymentSlipViewModel> modelList = new List<SalaryPaymentSlipViewModel>();
            //EmployeeSalaryMaster employeeSalaryMaster = (from EmpSal in dbContext.EmployeeSalaryPayments
            //                                             where EmpSal.RowKey == Id
            //                                             select EmpSal.EmployeeSalaryMaster).SingleOrDefault();

            modelList = dbContext.EmployeeSalaryMasters.Where(x => x.RowKey == Id).Select(row => new SalaryPaymentSlipViewModel
            {
                EmployeeKey = row.EmployeeKey,
                SalaryMasterKey = row.RowKey,
                SalaryMonthKey = row.SalaryMonth,
                SalaryYearKey = row.SalaryYear,
                Remarks = row.Remarks,
                EmployeeDateOfBirth = row.Employee.DateOfBirth,
                EmployeeCode = row.Employee.EmployeeCode,
                EmployeeName = row.Employee.FirstName + " " + (row.Employee.MiddleName ?? "") + " " + row.Employee.LastName,
                EmployeeEmailAddress = row.Employee.EmailAddress,
                EmployeeMobileNumber = row.Employee.MobileNumber,
                DesignationName = row.Employee.Designation.DesignationName,
                DepartmentName = row.Employee.Department.DepartmentName,
                DateOfJoining = row.Employee.JoiningDate,


                OvertimePerAHour = row.OverTimeAmount ?? 0,
                OverTimeHours = row.OverTimeHours,
                OverTimeMinutes = row.OverTimeMinutes,
                OverTimeTotalAmount = row.OverTimeTotalAmount ?? 0,

                AdditionalDayAmount = row.AdditionalDayAmount,
                AdditionalDayCount = row.AdditionalDayWorked,

                WeekOffCount = row.WeekOffCount,
                OffdayCount = row.OffdayCount,
                HolidayCount = row.HolidayCount,
                VoucherNumber = row.VoucherNumber,
                AbsentDays = row.AbsentDays,
                NoOfDaysWorked = row.NoOfDaysWorked,
                BaseWorkingDays = row.BaseWorkingDays,
                TotalWorkingDays = row.TotalWorkingDays,
                LOP = row.LOP,
                TotalSalary = row.TotalSalary,
                MonthlySalary = row.MonthlySalary,

                CompanyName = dbContext.Companies.Where(C => C.RowKey == row.Employee.Branch.CompanyKey).Select(C => C.CompanyName).FirstOrDefault(),
                CompanySubName = dbContext.Companies.Where(C => C.RowKey == row.Employee.Branch.CompanyKey).Select(C => C.CompanySubName).FirstOrDefault(),
                CompanyLogo = row.Employee.Branch.IsFranchise == true ? row.Employee.Branch.BranchLogo : row.Employee.Branch.Company.CompanyLogo,
                CompanyLogoPath = row.Employee.Branch.IsFranchise == true ? UrlConstants.BranchLogo + row.Employee.Branch.BranchLogo : UrlConstants.CompanyLogo + row.Employee.Branch.Company.CompanyLogo,

                CompanyAddress = dbContext.Branches.Where(C => C.RowKey == row.Employee.BranchKey).Select(C =>
                    C.AddressLine1 + " " + (C.AddressLine2 ?? "") + " " + ", " + (C.CityName ?? "") + ", " + (C.PostalCode ?? "")
                    + ", Tele : " + (C.PhoneNumber1 ?? "") + ", " + (C.PhoneNumber2 ?? "")
                      ).FirstOrDefault(),
                SalaryPayments = row.EmployeeSalaryDetails.Where(X => X.SalaryHead.SalaryHeadTypeKey == DbConstants.SalaryHeadType.MonthlyPayments).Select(item => new EmployeeSalaryDetailViewModel
                {
                    SalaryHeadName = item.SalaryHead.SalaryHeadName,
                    Amount = item.Amount
                }).Concat(row.EmployeeSalaryOtherAmounts.Where(X => X.IsAddition).Select(item => new EmployeeSalaryDetailViewModel
                {
                    SalaryHeadName = item.SalaryOtherAmountType.OtherSalaryHeadName,
                    Amount = item.Amount
                })).ToList(),
                SalaryDeductions = row.EmployeeSalaryDetails.Where(X => X.SalaryHead.SalaryHeadTypeKey == DbConstants.SalaryHeadType.StatutoryDeductions).Select(item => new EmployeeSalaryDetailViewModel
                {
                    SalaryHeadName = item.SalaryHead.SalaryHeadName,
                    Amount = item.Amount
                }).Concat(row.EmployeeSalaryOtherAmounts.Where(X => !X.IsAddition).Select(item => new EmployeeSalaryDetailViewModel
                {
                    SalaryHeadName = item.SalaryOtherAmountType.OtherSalaryHeadName,
                    Amount = item.Amount
                })).ToList(),

                SalaryAdvances = row.EmployeeSalaryAdvanceDetails.Select(x => new EmployeeSalaryDetailViewModel
                {
                    SalaryHeadName = EduSuiteUIResources.Salary + EduSuiteUIResources.BlankSpace + EduSuiteUIResources.Advance,
                    Amount = (x.PaidAmount)
                }).ToList(),


                BankName = dbContext.EmployeeAccounts.Where(x => x.EmployeeKey == row.EmployeeKey).Select(y => y.Bank.BankName).FirstOrDefault(),
                IFSCCode = dbContext.EmployeeAccounts.Where(x => x.EmployeeKey == row.EmployeeKey).Select(y => y.IFSCCode).FirstOrDefault(),
                AdharNumber = dbContext.EmployeeAccounts.Where(x => x.EmployeeKey == row.EmployeeKey).Select(y => y.AdharNumber).FirstOrDefault(),
                UANNumber = dbContext.EmployeeAccounts.Where(x => x.EmployeeKey == row.EmployeeKey).Select(y => y.UANNumber).FirstOrDefault(),
                MICRCode = dbContext.EmployeeAccounts.Where(x => x.EmployeeKey == row.EmployeeKey).Select(y => y.MICRCode).FirstOrDefault(),
                AccountNumber = dbContext.EmployeeAccounts.Where(x => x.EmployeeKey == row.EmployeeKey).Select(y => y.AccountNumber).FirstOrDefault(),
                AccountName = dbContext.EmployeeAccounts.Where(x => x.EmployeeKey == row.EmployeeKey).Select(y => y.NameInAccount).FirstOrDefault(),
                //DateOfJoining = dbContext.Employees.Where(x => x.RowKey == row.EmployeeKey).Select(y => y.JoiningDate).FirstOrDefault(),



                AdvanceBalance = (dbContext.EmployeeSalaryAdvancePayments.Where(x => x.EmployeeKey == row.EmployeeKey && ((x.ChequeStatusKey ?? DbConstants.ProcessStatus.Approved) == DbConstants.ProcessStatus.Approved) && System.Data.Entity.DbFunctions.TruncateTime(x.PaymentDate) <= System.Data.Entity.DbFunctions.TruncateTime(row.DateAdded)).Select(x => (x.PaidAmount - (x.ClearedAmount ?? 0))).DefaultIfEmpty().Sum())
                //SalaryComponents = row.SalaryComponents.Select(item => new SalaryComponentViewModel
                //{
                //    SalaryComponentTypeName = item.ComponentType.ComponentTypeName,
                //    AmountUnit = item.ComponentAmount,
                //    OperationType = item.ComponentType.OperationType
                //}).Union(row.LoanMonthlyRepayments.Select(item => new SalaryComponentViewModel
                //{
                //    SalaryComponentTypeName = "Loan Amount",
                //    AmountUnit = item.RepaymentAmount,
                //    OperationType = "D"
                //})).ToList()

            }).ToList();
            foreach (SalaryPaymentSlipViewModel model in modelList)
            {

                SalaryComponentViewModel LOP = dbContext.EmployeeSalaryMasters.Where(x => x.RowKey == model.SalaryMasterKey).Select(item => new SalaryComponentViewModel
                {
                    SalaryComponentTypeName = "LOP",
                    AmountUnit = item.LOP,
                    OperationType = "D"
                }).FirstOrDefault();
                if (LOP != null)
                    model.SalaryComponents.Add(LOP);
                //model.DaysInMonth = DateTime.DaysInMonth(model.SalaryYearKey, model.SalaryMonthKey);
                model.SalaryMonthName = GetMonthNameFromNumber(model.SalaryMonthKey) + " " + model.SalaryYearKey;
                SalaryComponentViewModel OvertimeItem = dbContext.EmployeeSalaryMasters.Where(x => x.RowKey == model.SalaryMasterKey).Select(item => new SalaryComponentViewModel
                {
                    SalaryComponentTypeName = "Overtime",
                    AmountUnit = item.OverTimeTotalAmount,
                    OperationType = "A"
                }).FirstOrDefault();

                if (OvertimeItem != null)
                    model.SalaryComponents.Add(OvertimeItem);

                //SalaryComponentViewModel OtherItem = dbContext.EmployeeSalaryMasters.Where(x => x.RowKey == model.SalaryMasterKey).Select(item => new SalaryComponentViewModel
                //{
                //    SalaryComponentTypeName = "Other",
                //    AmountUnit = item.OtherAmount,
                //    OperationType = item.OtherAmountType
                //}).FirstOrDefault();

                //if (OtherItem != null)
                //    model.SalaryComponents.Add(OtherItem);

            }



            return modelList;
        }

        #region SalaryAdvance

        public List<EmployeeSalaryAdvanceViewModel> GetEmployeeSalaryAdvances(EmployeeSalaryAdvanceViewModel model, string fromDate, string toDate)
        {
            try
            {
                var Take = model.PageSize;
                var Skip = (model.PageIndex - 1) * model.PageSize;
                DateTime? ToDate = null;
                DateTime? FromDate = null;
                ToDate = (toDate ?? "") != "" ? Convert.ToDateTime(toDate) : ToDate;
                FromDate = (fromDate ?? "") != "" ? Convert.ToDateTime(fromDate) : FromDate;
                IQueryable<EmployeeSalaryAdvanceViewModel> employeeSalaryList = (from c in dbContext.EmployeeSalaryAdvancePayments
                                                                                 select new EmployeeSalaryAdvanceViewModel
                                                                                 {
                                                                                     PaymentKey = c.RowKey,
                                                                                     PaymentModeKey = c.PaymentModeKey,
                                                                                     PaymentModeSubKey = c.PaymentModeSubKey,
                                                                                     PaymentModeName = c.PaymentMode.PaymentModeName,
                                                                                     CardNumber = c.CardNumber,
                                                                                     BankAccountName = c.BankAccount.Bank.BankName,
                                                                                     ChequeOrDDNumber = c.ChequeOrDDNumber,
                                                                                     ChequeOrDDDate = c.ChequeOrDDDate,
                                                                                     Purpose = c.Purpose,
                                                                                     PaidBy = c.PaidBy,
                                                                                     AuthorizedBy = c.AuthorizedBy,
                                                                                     ReceivedBy = c.ReceivedBy,
                                                                                     OnBehalfOf = c.OnBehalfOf,
                                                                                     Remarks = c.Remarks,
                                                                                     EmployeeName = c.Employee.FirstName + " " + (c.Employee.MiddleName ?? "") + " " + (c.Employee.LastName ?? ""),
                                                                                     PaidAmount = c.PaidAmount,
                                                                                     IsCleared = c.IsCleared,
                                                                                     ClearedAmount = c.ClearedAmount ?? 0,
                                                                                     BalanceAmount = c.PaidAmount - (c.ClearedAmount ?? 0),
                                                                                     ChequeStatusKey = c.ChequeStatusKey,
                                                                                     EmployeeKey = c.EmployeeKey,
                                                                                     PaymentDate = c.PaymentDate,
                                                                                     ReceiptNumber = c.VoucherNumber,
                                                                                     BranchKey = c.PaidBranchKey != null ? c.PaidBranchKey : c.Employee.BranchKey,
                                                                                     Status = c.ChequeStatusKey == DbConstants.ProcessStatus.Pending ? EduSuiteUIResources.Pending : (c.ChequeStatusKey == DbConstants.ProcessStatus.Rejected ? EduSuiteUIResources.Rejected : EduSuiteUIResources.Approved)
                                                                                 });
                if (model.BranchKey != 0)
                {
                    employeeSalaryList = employeeSalaryList.Where(row => row.BranchKey == model.BranchKey);
                }
                if (model.EmployeeKey != 0)
                {
                    employeeSalaryList = employeeSalaryList.Where(row => row.EmployeeKey == model.EmployeeKey);
                }
                if (FromDate != null)
                {
                    employeeSalaryList = employeeSalaryList.Where(row => row.PaymentDate >= FromDate);
                }
                if (ToDate != null)
                {
                    employeeSalaryList = employeeSalaryList.Where(row => row.PaymentDate <= ToDate);
                }
                if (model.SortBy != "")
                {
                    employeeSalaryList = SortSalesOrder(employeeSalaryList, model.SortBy, model.SortOrder);
                }
                model.TotalRecords = employeeSalaryList.Count();
                return model.PageSize != 0 ? employeeSalaryList.Skip(Skip).Take(Take).GroupBy(x => x.PaymentKey).Select(y => y.FirstOrDefault()).ToList() : employeeSalaryList.GroupBy(x => x.PaymentKey).Select(y => y.FirstOrDefault()).ToList();

                ////employeeSalaryList.ForEach(x => x.SalaryMonthName = GetMonthNameFromNumber(x.SalaryMonthKey) + " " + x.SalaryYearKey.ToString());
                //return employeeSalaryList.GroupBy(x => x.PaymentKey).Select(y => y.First()).ToList<EmployeeSalaryAdvanceViewModel>();
            }
            catch (Exception ex)
            {
                ActivityLog.CreateActivityLog(MenuConstants.EmployeeSalaryAdvance, ActionConstants.MenuAccess, DbConstants.LogType.Error, DbConstants.User.UserKey, ex.GetBaseException().Message);
                return new List<EmployeeSalaryAdvanceViewModel>();
            }
        }

        private IQueryable<EmployeeSalaryAdvanceViewModel> SortSalesOrder(IQueryable<EmployeeSalaryAdvanceViewModel> Query, string SortName, string SortOrder)
        {

            string command = SortOrder == "desc" ? "OrderByDescending" : "OrderBy";
            var type = typeof(EmployeeSalaryAdvanceViewModel);
            var property = type.GetProperty(SortName);
            var parameter = Expression.Parameter(type, "p");
            var propertyAccess = Expression.MakeMemberAccess(parameter, property);
            var orderByExpression = Expression.Lambda(propertyAccess, parameter);
            var resultExpression = Expression.Call(typeof(Queryable), command, new Type[] { type, property.PropertyType },
                                          Query.Expression, Expression.Quote(orderByExpression));
            return Query.Provider.CreateQuery<EmployeeSalaryAdvanceViewModel>(resultExpression);

        }
        public EmployeeSalaryAdvanceViewModel GetEmployeeSalaryAdvancePaymentById(EmployeeSalaryAdvanceViewModel model)
        {
            var UserKey = DbConstants.User.UserKey;
            try
            {
                model = dbContext.EmployeeSalaryAdvancePayments.Where(x => x.RowKey == model.PaymentKey).Select(row => new EmployeeSalaryAdvanceViewModel
                {
                    PaymentKey = row.RowKey,
                    EmployeeKey = row.EmployeeKey,
                    PaymentModeKey = row.PaymentModeKey,
                    PaidAmount = row.PaidAmount,
                    BalanceAmount = row.BalanceAmount,
                    PaymentDate = row.PaymentDate,
                    BankAccountKey = row.BankAccountKey,
                    BankAccountBalance = row.BankAccount.CurrentAccountBalance,
                    CardNumber = row.CardNumber,
                    ChequeOrDDNumber = row.ChequeOrDDNumber,
                    ChequeOrDDDate = row.ChequeOrDDDate,
                    Purpose = row.Purpose,
                    ReceivedBy = row.ReceivedBy,
                    OnBehalfOf = row.OnBehalfOf,
                    PaidBy = row.PaidBy,
                    AuthorizedBy = row.AuthorizedBy,
                    AmountToPay = 0,
                    Remarks = row.Remarks,
                    CashFlowTypeKey = DbConstants.CashFlowType.Out,
                    BranchKey = row.Employee.BranchKey,
                    ReferenceNumber = row.ReferenceNumber,
                    PaidBranchKey = row.PaidBranchKey

                }).FirstOrDefault();
                if (model == null)
                {
                    model = new EmployeeSalaryAdvanceViewModel();
                    model.PaymentModeKey = DbConstants.PaymentMode.Cash;
                }

                FillSalaryAdvancePaymentDropdownLists(model);

                return model;
            }
            catch (Exception ex)
            {
                model = new EmployeeSalaryAdvanceViewModel();
                ActivityLog.CreateActivityLog(MenuConstants.EmployeeSalaryAdvance, (model.PaymentKey != 0 ? ActionConstants.Edit : ActionConstants.Add), DbConstants.LogType.Error, model.PaymentKey, ex.GetBaseException().Message);
                return model;
            }
        }
        public EmployeeSalaryAdvanceViewModel CreateSalaryAdvancePayment(EmployeeSalaryAdvanceViewModel model)
        {
            EmployeeSalaryAdvancePayment EmployeeSalaryPaymentModel = new EmployeeSalaryAdvancePayment();
            FillSalaryAdvancePaymentDropdownLists(model);

            using (var transaction = dbContext.Database.BeginTransaction())
            {
                try
                {
                    ConfigurationViewModel ConfigModel = new ConfigurationViewModel();
                    ConfigModel.BranchKey = model.PaidBranchKey ?? 0;
                    ConfigModel.ConfigType = DbConstants.PaymentReceiptConfigType.Payment;
                    Configurations.GenerateReceipt(dbContext, ConfigModel);


                    Int64 maxKey = dbContext.EmployeeSalaryAdvancePayments.Select(p => p.RowKey).DefaultIfEmpty().Max();
                    EmployeeSalaryPaymentModel.RowKey = Convert.ToInt64(maxKey + 1);
                    EmployeeSalaryPaymentModel.EmployeeKey = Convert.ToInt32(model.EmployeeKey);
                    EmployeeSalaryPaymentModel.PaidAmount = Convert.ToDecimal(model.PaidAmount);
                    EmployeeSalaryPaymentModel.BalanceAmount = model.BalanceAmount;
                    EmployeeSalaryPaymentModel.PaymentDate = Convert.ToDateTime(model.PaymentDate);
                    EmployeeSalaryPaymentModel.PaymentModeKey = model.PaymentModeKey;
                    EmployeeSalaryPaymentModel.BankAccountKey = model.BankAccountKey;
                    EmployeeSalaryPaymentModel.PaymentModeSubKey = model.PaymentModeSubKey;
                    EmployeeSalaryPaymentModel.CardNumber = model.CardNumber;
                    EmployeeSalaryPaymentModel.BankAccountKey = model.BankAccountKey;
                    EmployeeSalaryPaymentModel.ChequeOrDDNumber = model.ChequeOrDDNumber;
                    EmployeeSalaryPaymentModel.ChequeOrDDDate = model.ChequeOrDDDate;
                    EmployeeSalaryPaymentModel.Purpose = model.Purpose;
                    EmployeeSalaryPaymentModel.ReferenceNumber = model.ReferenceNumber;
                    EmployeeSalaryPaymentModel.PaidBy = model.PaidBy;
                    EmployeeSalaryPaymentModel.AuthorizedBy = model.AuthorizedBy;
                    EmployeeSalaryPaymentModel.ReceivedBy = model.ReceivedBy;
                    EmployeeSalaryPaymentModel.OnBehalfOf = model.OnBehalfOf;
                    EmployeeSalaryPaymentModel.Remarks = model.Remarks;
                    EmployeeSalaryPaymentModel.PaidBranchKey = model.PaidBranchKey;
                    EmployeeSalaryPaymentModel.SerialNumber = ConfigModel.SerialNumber;
                    EmployeeSalaryPaymentModel.VoucherNumber = ConfigModel.ReceiptNumber;
                    if (model.PaymentModeKey == DbConstants.PaymentMode.Cheque)
                    {
                        EmployeeSalaryPaymentModel.ChequeStatusKey = DbConstants.ProcessStatus.Pending;
                    }
                    EmployeeSalaryPaymentModel.IsCleared = false;
                    long bankKey = 0;
                    model.PaymentKey = EmployeeSalaryPaymentModel.RowKey;
                    model.EmployeeName = dbContext.Employees.Where(x => x.RowKey == model.EmployeeKey).Select(x => x.FirstName).FirstOrDefault();
                    //CreateAccount(model);
                    dbContext.EmployeeSalaryAdvancePayments.Add(EmployeeSalaryPaymentModel);
                    if (model.BankAccountKey != null && model.BankAccountKey != 0)
                    {
                        var BankAccountList = dbContext.BankAccounts.SingleOrDefault(x => x.RowKey == model.BankAccountKey);
                        model.BankAccountName = (BankAccountList.NameInAccount ?? BankAccountList.AccountNumber) + "-" + BankAccountList.Bank.BankName;
                    }
                    //if (model.PaymentModeKey == DbConstants.PaymentMode.Bank)
                    //{
                    //    BankAccountService bankAccountService = new BankAccountService(dbContext);
                    //    BankAccountViewModel bankAccountModel = new BankAccountViewModel();
                    //    bankAccountModel.RowKey = model.BankAccountKey ?? 0;
                    //    bankAccountModel.Amount = model.PaidAmount;
                    //    bankAccountService.UpdateCurrentAccountBalance(bankAccountModel, false, true, null);
                    //}
                    model.IsUpdate = false;
                    List<AccountFlowViewModel> accountFlowModelList = new List<AccountFlowViewModel>();
                    accountFlowModelList = AdvanceAmountList(model, accountFlowModelList, bankKey);
                    CreateAccountFlow(accountFlowModelList, false);
                    dbContext.SaveChanges();
                    transaction.Commit();
                    model.Message = EduSuiteUIResources.Success;
                    model.IsSuccessful = true;

                    model.PaymentKey = EmployeeSalaryPaymentModel.RowKey;
                    ActivityLog.CreateActivityLog(MenuConstants.EmployeeSalaryAdvance, ActionConstants.Add, DbConstants.LogType.Info, model.PaymentKey, model.Message);
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    //model.ExceptionMessage = ex.GetBaseException().Message;
                    model.Message = String.Format(EduSuiteUIResources.FailedToSaveErrorMessage, EduSuiteUIResources.SalaryPayment);
                    model.IsSuccessful = false;
                    ActivityLog.CreateActivityLog(MenuConstants.EmployeeSalaryAdvance, ActionConstants.Add, DbConstants.LogType.Error, model.PaymentKey, ex.GetBaseException().Message);
                }

            }

            return model;
        }
        public EmployeeSalaryAdvanceViewModel UpdateSalaryAdvancePayment(EmployeeSalaryAdvanceViewModel model)
        {
            EmployeeSalaryAdvancePayment EmployeeSalaryPaymentModel = new EmployeeSalaryAdvancePayment();
            FillSalaryAdvancePaymentDropdownLists(model);

            using (var transaction = dbContext.Database.BeginTransaction())
            {
                try
                {
                    EmployeeSalaryPaymentModel = dbContext.EmployeeSalaryAdvancePayments.SingleOrDefault(row => row.RowKey == model.PaymentKey);
                    EmployeeSalaryPaymentModel.PaidAmount = Convert.ToDecimal(model.PaidAmount);
                    EmployeeSalaryPaymentModel.BalanceAmount = model.BalanceAmount;
                    EmployeeSalaryPaymentModel.PaymentDate = Convert.ToDateTime(model.PaymentDate);
                    model.OldPaymentModeKey = EmployeeSalaryPaymentModel.PaymentModeKey;
                    long oldBank = EmployeeSalaryPaymentModel.BankAccountKey ?? 0;
                    EmployeeSalaryPaymentModel.PaymentModeKey = model.PaymentModeKey;
                    EmployeeSalaryPaymentModel.BankAccountKey = model.BankAccountKey;
                    EmployeeSalaryPaymentModel.PaymentModeSubKey = model.PaymentModeSubKey;
                    EmployeeSalaryPaymentModel.CardNumber = model.CardNumber;

                    EmployeeSalaryPaymentModel.BankAccountKey = model.BankAccountKey;
                    EmployeeSalaryPaymentModel.ChequeOrDDNumber = model.ChequeOrDDNumber;
                    EmployeeSalaryPaymentModel.ChequeOrDDDate = model.ChequeOrDDDate;
                    EmployeeSalaryPaymentModel.ReferenceNumber = model.ReferenceNumber;
                    EmployeeSalaryPaymentModel.Purpose = model.Purpose;
                    EmployeeSalaryPaymentModel.PaidBy = model.PaidBy;
                    EmployeeSalaryPaymentModel.AuthorizedBy = model.AuthorizedBy;
                    EmployeeSalaryPaymentModel.ReceivedBy = model.ReceivedBy;
                    EmployeeSalaryPaymentModel.OnBehalfOf = model.OnBehalfOf;
                    EmployeeSalaryPaymentModel.Remarks = model.Remarks;
                    EmployeeSalaryPaymentModel.IsCleared = false;
                    EmployeeSalaryPaymentModel.PaidBranchKey = model.PaidBranchKey;
                    if (model.PaymentModeKey == DbConstants.PaymentMode.Cheque)
                    {
                        EmployeeSalaryPaymentModel.ChequeStatusKey = DbConstants.ProcessStatus.Pending;
                    }
                    model.EmployeeName = dbContext.Employees.Where(x => x.RowKey == model.EmployeeKey).Select(x => x.FirstName).FirstOrDefault();
                    //UpdateAccount(model);
                    if (model.BankAccountKey != null && model.BankAccountKey != 0)
                    {
                        var BankAccountList = dbContext.BankAccounts.SingleOrDefault(x => x.RowKey == model.BankAccountKey);
                        model.BankAccountName = (BankAccountList.NameInAccount ?? BankAccountList.AccountNumber) + "-" + BankAccountList.Bank.BankName;
                    }

                    //if (DbConstants.PaymentMode.BankPaymentModes.Contains(model.OldPaymentModeKey) && oldBank != (model.BankAccountKey ?? 0))
                    //{
                    //    BankAccountService bankAccountService = new BankAccountService(dbContext);
                    //    BankAccountViewModel bankAccountModel = new BankAccountViewModel();
                    //    bankAccountModel.RowKey = oldBank;
                    //    bankAccountModel.Amount = -(model.OldAmount);
                    //    bankAccountService.UpdateCurrentAccountBalance(bankAccountModel, true, true, model.OldAmount);
                    //}

                    //if (DbConstants.PaymentMode.BankPaymentModes.Contains(model.PaymentModeKey))
                    //{
                    //    BankAccountService bankAccountService = new BankAccountService(dbContext);
                    //    BankAccountViewModel bankAccountModel = new BankAccountViewModel();
                    //    bankAccountModel.RowKey = model.BankAccountKey ?? 0;
                    //    bankAccountModel.Amount = model.PaidAmount;
                    //    bankAccountService.UpdateCurrentAccountBalance(bankAccountModel, true, true, model.OldAmount);
                    //}

                    //if (model.PaymentModeKey == DbConstants.PaymentMode.Bank)
                    //{
                    //    BankAccountService bankAccountService = new BankAccountService(dbContext);
                    //    BankAccountViewModel bankAccountModel = new BankAccountViewModel();
                    //    bankAccountModel.RowKey = model.BankAccountKey ?? 0;
                    //    bankAccountModel.Amount = model.PaidAmount;
                    //    bankAccountService.UpdateCurrentAccountBalance(bankAccountModel);
                    //    if (model.OldPaymentModeKey == DbConstants.PaymentMode.Bank)
                    //    {
                    //        bankAccountModel.RowKey = oldBank;
                    //        bankAccountModel.Amount = -(model.OldAmount);
                    //        bankAccountService.UpdateCurrentAccountBalance(bankAccountModel);
                    //    }
                    //}
                    //else if (model.OldPaymentModeKey == DbConstants.PaymentMode.Bank)
                    //{
                    //    BankAccountService bankAccountService = new BankAccountService(dbContext);
                    //    BankAccountViewModel bankAccountModel = new BankAccountViewModel();
                    //    bankAccountModel.RowKey = oldBank;
                    //    bankAccountModel.Amount = -(model.OldAmount);
                    //    bankAccountService.UpdateCurrentAccountBalance(bankAccountModel);
                    //}
                    model.IsUpdate = true;
                    List<AccountFlowViewModel> accountFlowModelList = new List<AccountFlowViewModel>();
                    accountFlowModelList = AdvanceAmountList(model, accountFlowModelList, oldBank);
                    CreateAccountFlow(accountFlowModelList, true);
                    dbContext.SaveChanges();
                    transaction.Commit();
                    model.Message = EduSuiteUIResources.Success;
                    model.IsSuccessful = true;
                    ActivityLog.CreateActivityLog(MenuConstants.EmployeeSalaryAdvance, ActionConstants.Edit, DbConstants.LogType.Info, model.PaymentKey, model.Message);

                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    ////model.ExceptionMessage = ex.GetBaseException().Message;
                    model.Message = String.Format(EduSuiteUIResources.FailedToSaveErrorMessage, EduSuiteUIResources.SalaryPayment);
                    model.IsSuccessful = false;
                    ActivityLog.CreateActivityLog(MenuConstants.EmployeeSalaryAdvance, ActionConstants.Edit, DbConstants.LogType.Error, model.PaymentKey, ex.GetBaseException().Message);
                }

            }

            return model;
        }
        private void purposeGeneration(EmployeeSalaryAdvanceViewModel model)
        {
            model.Purpose = (model.Purpose != null && model.Purpose != "" ? (EduSuiteUIResources.For + EduSuiteUIResources.BlankSpace + model.Purpose) : "");
            string ReceivedBy = EduSuiteUIResources.Salary + EduSuiteUIResources.BlankSpace + EduSuiteUIResources.Advance + EduSuiteUIResources.BlankSpace + EduSuiteUIResources.PaidTo + EduSuiteUIResources.BlankSpace + (model.ReceivedBy != null && model.ReceivedBy != "" ? model.ReceivedBy : model.EmployeeName) + EduSuiteUIResources.BlankSpace;
            string PaidBy = model.PaidBy != null && model.PaidBy != "" ? EduSuiteUIResources.Paid + EduSuiteUIResources.BlankSpace + EduSuiteUIResources.By + EduSuiteUIResources.BlankSpace + model.PaidBy + EduSuiteUIResources.BlankSpace : "";
            string OnBehalfOf = model.OnBehalfOf != null && model.OnBehalfOf != "" ? EduSuiteUIResources.OnBehalfOf + EduSuiteUIResources.BlankSpace + model.OnBehalfOf + EduSuiteUIResources.BlankSpace : "";
            string AuthorizedBy = model.AuthorizedBy != null && model.AuthorizedBy != "" ? EduSuiteUIResources.AuthorizedBy + EduSuiteUIResources.BlankSpace + model.AuthorizedBy + EduSuiteUIResources.BlankSpace : "";
            model.Remarks = model.Remarks != null && model.Remarks != "" ? EduSuiteUIResources.OpenBracket + model.Remarks + EduSuiteUIResources.CloseBracket : "";
            model.Purpose = ReceivedBy + model.Purpose + PaidBy + OnBehalfOf + AuthorizedBy;
            model.PaymentModeName = EduSuiteUIResources.By + EduSuiteUIResources.BlankSpace + (model.PaymentModeKey == DbConstants.PaymentMode.Cash ? EduSuiteUIResources.Cash : model.PaymentModeKey == DbConstants.PaymentMode.Bank ? EduSuiteUIResources.Bank + EduSuiteUIResources.OpenBracket + model.BankAccountName + EduSuiteUIResources.CloseBracket : model.PaymentModeKey == DbConstants.PaymentMode.Cheque ? EduSuiteUIResources.Cheque + EduSuiteUIResources.OpenBracket + model.BankAccountName + EduSuiteUIResources.CloseBracket : "");
        }
        public EmployeeSalaryAdvanceViewModel DeleteSalaryAdvancePayment(EmployeeSalaryAdvanceViewModel model)
        {
            EmployeeSalaryAdvancePayment EmployeeSalaryPaymentModel = new EmployeeSalaryAdvancePayment();
            using (var transaction = dbContext.Database.BeginTransaction())
            {
                try
                {
                    EmployeeSalaryPaymentModel = dbContext.EmployeeSalaryAdvancePayments.SingleOrDefault(row => row.RowKey == model.PaymentKey);
                    long RowKey = EmployeeSalaryPaymentModel.RowKey;

                    ConfigurationViewModel ConfigModel = new ConfigurationViewModel();
                    ConfigModel.BranchKey = EmployeeSalaryPaymentModel.PaidBranchKey ?? 0;
                    ConfigModel.SerialNumber = EmployeeSalaryPaymentModel.SerialNumber ?? 0;
                    ConfigModel.IsDelete = true;
                    ConfigModel.ConfigType = DbConstants.PaymentReceiptConfigType.Payment;
                    Configurations.GenerateReceipt(dbContext, ConfigModel);

                    dbContext.EmployeeSalaryAdvancePayments.Remove(EmployeeSalaryPaymentModel);

                    AccountFlowViewModel accountFlowModel = new AccountFlowViewModel();
                    AccountFlowService accountFlowService = new AccountFlowService(dbContext);
                    accountFlowModel.TransactionTypeKey = DbConstants.TransactionType.SalaryAdvance;
                    accountFlowModel.TransactionKey = RowKey;
                    accountFlowService.DeleteAccountFlow(accountFlowModel);
                    dbContext.SaveChanges();
                    transaction.Commit();
                    model.Message = EduSuiteUIResources.Success;
                    model.IsSuccessful = true;
                    ActivityLog.CreateActivityLog(MenuConstants.EmployeeSalaryAdvance, ActionConstants.Delete, DbConstants.LogType.Info, model.PaymentKey, model.Message);
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    //model.ExceptionMessage = ex.GetBaseException().Message;
                    model.Message = String.Format(EduSuiteUIResources.FailedToSaveErrorMessage, EduSuiteUIResources.SalaryPayment);
                    model.IsSuccessful = false;
                    ActivityLog.CreateActivityLog(MenuConstants.EmployeeSalaryAdvance, ActionConstants.Delete, DbConstants.LogType.Error, model.PaymentKey, ex.GetBaseException().Message);
                }

            }

            return model;
        }
        private void FillSalaryAdvancePaymentDropdownLists(EmployeeSalaryAdvanceViewModel model)
        {
            GetBranches(model);
            FillEmployees(model);
            FillPaymentModes(model);
            FillPaymentModeSub(model);
            FillBankAccounts(model);
            FillPaidBranches(model);

        }
        public EmployeeSalaryAdvanceViewModel FillEmployees(EmployeeSalaryAdvanceViewModel model)
        {
            model.Employees = dbContext.Employees.Where(x => x.BranchKey == model.BranchKey).Select(row => new SelectListModel
            {
                RowKey = row.RowKey,
                Text = row.FirstName + " " + (row.MiddleName ?? "") + " " + row.LastName,
                //GroupKey = row.DepartmentKey,
                //GroupName = row.Department.DepartmentName
            }).OrderBy(row => row.Text).ToList();
            return model;
        }
        public void GetBranches(EmployeeSalaryAdvanceViewModel model)
        {
            IQueryable<SelectListModel> BranchQuery = dbContext.vwBranchSelectActiveOnlies.OrderBy(row => row.DisplayOrder).Select(row => new SelectListModel
            {
                RowKey = row.RowKey,
                Text = row.BranchName
            });

            Employee Employee = dbContext.Employees.Where(x => x.AppUserKey == DbConstants.User.UserKey).SingleOrDefault();
            if (Employee != null)
            {
                if (Employee.BranchAccess != null)
                {
                    List<long> Branches = Employee.BranchAccess.Split(',').Select(Int64.Parse).ToList();
                    model.Branches = BranchQuery.Where(row => Branches.Contains(row.RowKey)).ToList();
                }
                else
                {
                    model.Branches = BranchQuery.Where(x => x.RowKey == Employee.BranchKey).ToList();
                }
            }
            else
            {
                model.Branches = BranchQuery.ToList();
            }

        }

        private void FillPaidBranches(EmployeeSalaryAdvanceViewModel model)
        {
            model.PaidBranches = dbContext.Branches.Where(row => row.IsActive).Select(row => new SelectListModel
            {
                RowKey = row.RowKey,
                Text = row.BranchName
            }).ToList();
        }
        private void FillPaidBranches(PaymentWindowViewModel model)
        {
            model.PaidBranches = dbContext.Branches.Where(row => row.IsActive).Select(row => new SelectListModel
            {
                RowKey = row.RowKey,
                Text = row.BranchName
            }).ToList();
        }

        #endregion

        #region AdvanceReturn

        public List<EmployeeSalaryAdvanceReturnViewModel> GetEmployeeSalaryAdvanceReturn(EmployeeSalaryAdvanceReturnViewModel model, string fromDate, string toDate)
        {
            try
            {
                var Take = model.PageSize;
                var Skip = (model.PageIndex - 1) * model.PageSize;
                DateTime? ToDate = null;
                DateTime? FromDate = null;
                ToDate = (toDate ?? "") != "" ? Convert.ToDateTime(toDate) : ToDate;
                FromDate = (fromDate ?? "") != "" ? Convert.ToDateTime(fromDate) : FromDate;
                IQueryable<EmployeeSalaryAdvanceReturnViewModel> employeeSalaryList = (from c in dbContext.EmployeeSalaryAdvanceReturnMasters
                                                                                       select new EmployeeSalaryAdvanceReturnViewModel
                                                                                       {
                                                                                           PaymentKey = c.RowKey,
                                                                                           PaymentModeKey = c.PaymentModeKey,
                                                                                           PaymentModeName = c.PaymentMode.PaymentModeName,
                                                                                           PaymentModeSubKey = c.PaymentModeSubKey,
                                                                                           PaymentModeSubName = c.PaymentModeSub.PaymentModeSubName,
                                                                                           CardNumber = c.CardNumber,
                                                                                           BankAccountName = c.BankAccount.Bank.BankName,
                                                                                           ChequeOrDDNumber = c.ChequeOrDDNumber,
                                                                                           ChequeOrDDDate = c.ChequeOrDDDate,
                                                                                           Purpose = c.Purpose,
                                                                                           PaidBy = c.PaidBy,
                                                                                           AuthorizedBy = c.AuthorizedBy,
                                                                                           ReceivedBy = c.ReceivedBy,
                                                                                           OnBehalfOf = c.OnBehalfOf,
                                                                                           Remarks = c.Remarks,
                                                                                           EmployeeName = c.Employee.FirstName + " " + (c.Employee.MiddleName ?? "") + " " + (c.Employee.LastName ?? ""),
                                                                                           PaidAmount = c.PaidAmount,
                                                                                           ChequeStatusKey = c.ChequeStatusKey,
                                                                                           BranchKey = c.PaidBranchKey != null ? c.PaidBranchKey : c.Employee.BranchKey,
                                                                                           EmployeeKey = c.EmployeeKey,
                                                                                           PaymentDate = c.PaymentDate,
                                                                                           ReceiptNumber = c.ReceiptNumber,
                                                                                           Status = c.ChequeStatusKey == DbConstants.ProcessStatus.Pending ? EduSuiteUIResources.Pending : (c.ChequeStatusKey == DbConstants.ProcessStatus.Rejected ? EduSuiteUIResources.Rejected : EduSuiteUIResources.Approved)
                                                                                       });
                if (model.BranchKey != 0)
                {
                    employeeSalaryList = employeeSalaryList.Where(row => row.BranchKey == model.BranchKey);
                }
                if (model.EmployeeKey != 0)
                {
                    employeeSalaryList = employeeSalaryList.Where(row => row.EmployeeKey == model.EmployeeKey);
                }
                if (FromDate != null)
                {
                    employeeSalaryList = employeeSalaryList.Where(row => row.PaymentDate >= FromDate);
                }
                if (ToDate != null)
                {
                    employeeSalaryList = employeeSalaryList.Where(row => row.PaymentDate <= ToDate);
                }
                if (model.SortBy != "")
                {
                    employeeSalaryList = SortSalesOrder(employeeSalaryList, model.SortBy, model.SortOrder);
                }
                model.TotalRecords = employeeSalaryList.Count();
                return model.PageSize != 0 ? employeeSalaryList.Skip(Skip).Take(Take).GroupBy(x => x.PaymentKey).Select(y => y.FirstOrDefault()).ToList() : employeeSalaryList.GroupBy(x => x.PaymentKey).Select(y => y.FirstOrDefault()).ToList();

                ////employeeSalaryList.ForEach(x => x.SalaryMonthName = GetMonthNameFromNumber(x.SalaryMonthKey) + " " + x.SalaryYearKey.ToString());
                //return employeeSalaryList.GroupBy(x => x.PaymentKey).Select(y => y.First()).ToList<EmployeeSalaryAdvanceViewModel>();
            }
            catch (Exception ex)
            {
                ActivityLog.CreateActivityLog(MenuConstants.EmployeeSalaryAdvanceReturn, ActionConstants.MenuAccess, DbConstants.LogType.Error, DbConstants.User.UserKey, ex.GetBaseException().Message);
                return new List<EmployeeSalaryAdvanceReturnViewModel>();
            }
        }

        private IQueryable<EmployeeSalaryAdvanceReturnViewModel> SortSalesOrder(IQueryable<EmployeeSalaryAdvanceReturnViewModel> Query, string SortName, string SortOrder)
        {

            string command = SortOrder == "desc" ? "OrderByDescending" : "OrderBy";
            var type = typeof(EmployeeSalaryAdvanceReturnViewModel);
            var property = type.GetProperty(SortName);
            var parameter = Expression.Parameter(type, "p");
            var propertyAccess = Expression.MakeMemberAccess(parameter, property);
            var orderByExpression = Expression.Lambda(propertyAccess, parameter);
            var resultExpression = Expression.Call(typeof(Queryable), command, new Type[] { type, property.PropertyType },
                                          Query.Expression, Expression.Quote(orderByExpression));
            return Query.Provider.CreateQuery<EmployeeSalaryAdvanceReturnViewModel>(resultExpression);

        }
        public EmployeeSalaryAdvanceReturnViewModel GetEmployeeSalaryAdvanceReturnById(EmployeeSalaryAdvanceReturnViewModel model)
        {
            var UserKey = DbConstants.User.UserKey;
            try
            {
                model = dbContext.EmployeeSalaryAdvanceReturnMasters.Where(x => x.RowKey == model.PaymentKey).Select(row => new EmployeeSalaryAdvanceReturnViewModel
                {
                    PaymentKey = row.RowKey,
                    EmployeeKey = row.EmployeeKey,
                    PaymentModeKey = row.PaymentModeKey,
                    PaymentModeSubKey = row.PaymentModeSubKey,
                    PaidAmount = row.PaidAmount,
                    PaymentDate = row.PaymentDate,
                    BankAccountKey = row.BankAccountKey,
                    BankAccountBalance = row.BankAccount.CurrentAccountBalance,
                    CardNumber = row.CardNumber,
                    ChequeOrDDNumber = row.ChequeOrDDNumber,
                    ChequeOrDDDate = row.ChequeOrDDDate,
                    Purpose = row.Purpose,
                    ReceivedBy = row.ReceivedBy,
                    OnBehalfOf = row.OnBehalfOf,
                    PaidBy = row.PaidBy,
                    AuthorizedBy = row.AuthorizedBy,
                    AmountToPay = 0,
                    Remarks = row.Remarks,
                    CashFlowTypeKey = DbConstants.CashFlowType.Out,
                    BranchKey = row.Employee.BranchKey,
                    ReferenceNumber = row.ReferenceNumber,
                    PaidBranchKey = row.PaidBranchKey

                }).FirstOrDefault();
                if (model == null)
                {
                    model = new EmployeeSalaryAdvanceReturnViewModel();
                    model.PaymentModeKey = DbConstants.PaymentMode.Cash;
                }

                FillSalaryAdvanceReturnDropdownLists(model);

                return model;
            }
            catch (Exception ex)
            {
                model = new EmployeeSalaryAdvanceReturnViewModel();
                ActivityLog.CreateActivityLog(MenuConstants.EmployeeSalaryAdvanceReturn, (model.PaymentKey != 0 ? ActionConstants.Edit : ActionConstants.Add), DbConstants.LogType.Error, DbConstants.User.UserKey, ex.GetBaseException().Message);

                //model.ExceptionMessage = ex.GetBaseException().Message;
                return model;
            }
        }
        public EmployeeSalaryAdvanceReturnViewModel CreateSalaryAdvanceReturn(EmployeeSalaryAdvanceReturnViewModel model)
        {
            EmployeeSalaryAdvanceReturnMaster EmployeeSalaryPaymentModel = new EmployeeSalaryAdvanceReturnMaster();
            FillSalaryAdvanceReturnDropdownLists(model);

            using (var transaction = dbContext.Database.BeginTransaction())
            {
                try
                {
                    ConfigurationViewModel ConfigModel = new ConfigurationViewModel();
                    ConfigModel.BranchKey = model.PaidBranchKey ?? 0;
                    ConfigModel.ConfigType = DbConstants.PaymentReceiptConfigType.ReceiptVoucher;
                    Configurations.GenerateReceipt(dbContext, ConfigModel);

                    Int64 maxKey = dbContext.EmployeeSalaryAdvanceReturnMasters.Select(p => p.RowKey).DefaultIfEmpty().Max();
                    EmployeeSalaryPaymentModel.RowKey = Convert.ToInt64(maxKey + 1);
                    EmployeeSalaryPaymentModel.EmployeeKey = Convert.ToInt32(model.EmployeeKey);
                    EmployeeSalaryPaymentModel.PaidAmount = Convert.ToDecimal(model.PaidAmount);
                    EmployeeSalaryPaymentModel.PaymentDate = Convert.ToDateTime(model.PaymentDate);
                    EmployeeSalaryPaymentModel.PaymentModeKey = model.PaymentModeKey;
                    EmployeeSalaryPaymentModel.BankAccountKey = model.BankAccountKey;
                    EmployeeSalaryPaymentModel.PaymentModeSubKey = model.PaymentModeSubKey;
                    EmployeeSalaryPaymentModel.CardNumber = model.CardNumber;
                    EmployeeSalaryPaymentModel.BankAccountKey = model.BankAccountKey;
                    EmployeeSalaryPaymentModel.ChequeOrDDNumber = model.ChequeOrDDNumber;
                    EmployeeSalaryPaymentModel.ChequeOrDDDate = model.ChequeOrDDDate;
                    EmployeeSalaryPaymentModel.Purpose = model.Purpose;
                    EmployeeSalaryPaymentModel.ReferenceNumber = model.ReferenceNumber;
                    EmployeeSalaryPaymentModel.PaidBy = model.PaidBy;
                    EmployeeSalaryPaymentModel.AuthorizedBy = model.AuthorizedBy;
                    EmployeeSalaryPaymentModel.ReceivedBy = model.ReceivedBy;
                    EmployeeSalaryPaymentModel.OnBehalfOf = model.OnBehalfOf;
                    EmployeeSalaryPaymentModel.Remarks = model.Remarks;
                    EmployeeSalaryPaymentModel.PaidBranchKey = model.PaidBranchKey;
                    EmployeeSalaryPaymentModel.SerialNumber = ConfigModel.SerialNumber;
                    EmployeeSalaryPaymentModel.ReceiptNumber = ConfigModel.ReceiptNumber;
                    if (model.PaymentModeKey == DbConstants.PaymentMode.Cheque)
                    {
                        EmployeeSalaryPaymentModel.ChequeStatusKey = DbConstants.ProcessStatus.Pending;
                    }
                    long bankKey = 0;
                    model.PaymentKey = EmployeeSalaryPaymentModel.RowKey;
                    
                    model.EmployeeName = dbContext.Employees.Where(x => x.RowKey == model.EmployeeKey).Select(x => x.FirstName).FirstOrDefault();
                    //CreateAccount(model);
                    dbContext.EmployeeSalaryAdvanceReturnMasters.Add(EmployeeSalaryPaymentModel);
                    if (model.BankAccountKey != null && model.BankAccountKey != 0)
                    {
                        var BankAccountList = dbContext.BankAccounts.SingleOrDefault(x => x.RowKey == model.BankAccountKey);
                        model.BankAccountName = (BankAccountList.NameInAccount ?? BankAccountList.AccountNumber) + "-" + BankAccountList.Bank.BankName;
                    }
                    //if (model.PaymentModeKey == DbConstants.PaymentMode.Bank)
                    //{
                    //    BankAccountService bankAccountService = new BankAccountService(dbContext);
                    //    BankAccountViewModel bankAccountModel = new BankAccountViewModel();
                    //    bankAccountModel.RowKey = model.BankAccountKey ?? 0;
                    //    bankAccountModel.Amount = model.PaidAmount;
                    //    bankAccountService.UpdateCurrentAccountBalance(bankAccountModel, false, false, null);
                    //}

                    purposeGeneration(model);
                    createAdvanceReturnDetail(model);
                    List<AccountFlowViewModel> accountFlowModelList = new List<AccountFlowViewModel>();
                    accountFlowModelList = ReturnAmountList(model, accountFlowModelList, bankKey, false);
                    CreateAccountFlow(accountFlowModelList, false);
                    dbContext.SaveChanges();
                    transaction.Commit();
                    model.Message = EduSuiteUIResources.Success;
                    model.IsSuccessful = true;

                    model.PaymentKey = EmployeeSalaryPaymentModel.RowKey;
                    ActivityLog.CreateActivityLog(MenuConstants.EmployeeSalaryAdvanceReturn, ActionConstants.Add, DbConstants.LogType.Info, model.PaymentKey, model.Message);

                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    //model.ExceptionMessage = ex.GetBaseException().Message;
                    model.Message = String.Format(EduSuiteUIResources.FailedToSaveErrorMessage, EduSuiteUIResources.SalaryPayment);
                    model.IsSuccessful = false;
                    ActivityLog.CreateActivityLog(MenuConstants.EmployeeSalaryAdvanceReturn, ActionConstants.Add, DbConstants.LogType.Error, model.PaymentKey, ex.GetBaseException().Message);

                }

            }

            return model;
        }
        public EmployeeSalaryAdvanceReturnViewModel UpdateSalaryAdvanceReturn(EmployeeSalaryAdvanceReturnViewModel model)
        {
            EmployeeSalaryAdvanceReturnMaster EmployeeSalaryPaymentModel = new EmployeeSalaryAdvanceReturnMaster();
            FillSalaryAdvanceReturnDropdownLists(model);

            using (var transaction = dbContext.Database.BeginTransaction())
            {
                try
                {
                    EmployeeSalaryPaymentModel = dbContext.EmployeeSalaryAdvanceReturnMasters.SingleOrDefault(row => row.RowKey == model.PaymentKey);
                    EmployeeSalaryPaymentModel.PaidAmount = Convert.ToDecimal(model.PaidAmount);
                    EmployeeSalaryPaymentModel.PaymentDate = Convert.ToDateTime(model.PaymentDate);
                    model.OldPaymentModeKey = EmployeeSalaryPaymentModel.PaymentModeKey;
                    long oldBank = EmployeeSalaryPaymentModel.BankAccountKey ?? 0;
                    EmployeeSalaryPaymentModel.PaymentModeKey = model.PaymentModeKey;
                    EmployeeSalaryPaymentModel.BankAccountKey = model.BankAccountKey;
                    EmployeeSalaryPaymentModel.PaymentModeSubKey = model.PaymentModeSubKey;
                    EmployeeSalaryPaymentModel.CardNumber = model.CardNumber;

                    EmployeeSalaryPaymentModel.BankAccountKey = model.BankAccountKey;
                    EmployeeSalaryPaymentModel.ChequeOrDDNumber = model.ChequeOrDDNumber;
                    EmployeeSalaryPaymentModel.ChequeOrDDDate = model.ChequeOrDDDate;
                    EmployeeSalaryPaymentModel.ReferenceNumber = model.ReferenceNumber;
                    EmployeeSalaryPaymentModel.Purpose = model.Purpose;
                    EmployeeSalaryPaymentModel.PaidBy = model.PaidBy;
                    EmployeeSalaryPaymentModel.AuthorizedBy = model.AuthorizedBy;
                    EmployeeSalaryPaymentModel.ReceivedBy = model.ReceivedBy;
                    EmployeeSalaryPaymentModel.OnBehalfOf = model.OnBehalfOf;
                    EmployeeSalaryPaymentModel.Remarks = model.Remarks;
                    EmployeeSalaryPaymentModel.PaidBranchKey = model.PaidBranchKey;
                    if (model.PaymentModeKey == DbConstants.PaymentMode.Cheque)
                    {
                        EmployeeSalaryPaymentModel.ChequeStatusKey = DbConstants.ProcessStatus.Pending;
                    }
                    model.EmployeeName = dbContext.Employees.Where(x => x.RowKey == model.EmployeeKey).Select(x => x.FirstName).FirstOrDefault();
                    //UpdateAccount(model);
                    if (model.BankAccountKey != null && model.BankAccountKey != 0)
                    {
                        var BankAccountList = dbContext.BankAccounts.SingleOrDefault(x => x.RowKey == model.BankAccountKey);
                        model.BankAccountName = (BankAccountList.NameInAccount ?? BankAccountList.AccountNumber) + "-" + BankAccountList.Bank.BankName;
                    }




                    //if (model.PaymentModeKey == DbConstants.PaymentMode.Bank)
                    //{
                    //    BankAccountService bankAccountService = new BankAccountService(dbContext);
                    //    BankAccountViewModel bankAccountModel = new BankAccountViewModel();
                    //    bankAccountModel.RowKey = model.BankAccountKey ?? 0;
                    //    bankAccountModel.Amount = model.PaidAmount;
                    //    bankAccountService.UpdateCurrentAccountBalance(bankAccountModel);
                    //    if (model.OldPaymentModeKey == DbConstants.PaymentMode.Bank)
                    //    {
                    //        bankAccountModel.RowKey = oldBank;
                    //        bankAccountModel.Amount = -(model.OldAmount);
                    //        bankAccountService.UpdateCurrentAccountBalance(bankAccountModel);
                    //    }
                    //}
                    //else if (model.OldPaymentModeKey == DbConstants.PaymentMode.Bank)
                    //{
                    //    BankAccountService bankAccountService = new BankAccountService(dbContext);
                    //    BankAccountViewModel bankAccountModel = new BankAccountViewModel();
                    //    bankAccountModel.RowKey = oldBank;
                    //    bankAccountModel.Amount = -(model.OldAmount);
                    //    bankAccountService.UpdateCurrentAccountBalance(bankAccountModel);
                    //}
                    purposeGeneration(model);
                    updateAdvanceReturnDetail(model);
                    List<AccountFlowViewModel> accountFlowModelList = new List<AccountFlowViewModel>();
                    accountFlowModelList = ReturnAmountList(model, accountFlowModelList, oldBank, true);
                    CreateAccountFlow(accountFlowModelList, true);
                    dbContext.SaveChanges();
                    transaction.Commit();
                    model.Message = EduSuiteUIResources.Success;
                    model.IsSuccessful = true;
                    ActivityLog.CreateActivityLog(MenuConstants.EmployeeSalaryAdvanceReturn, ActionConstants.Edit, DbConstants.LogType.Info, model.PaymentKey, model.Message);



                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    //model.ExceptionMessage = ex.GetBaseException().Message;
                    model.Message = String.Format(EduSuiteUIResources.FailedToSaveErrorMessage, EduSuiteUIResources.SalaryPayment);
                    model.IsSuccessful = false;
                    ActivityLog.CreateActivityLog(MenuConstants.EmployeeSalaryAdvanceReturn, ActionConstants.Edit, DbConstants.LogType.Error, model.PaymentKey, ex.GetBaseException().Message);

                }

            }

            return model;
        }
        private void updateAdvanceReturnDetail(EmployeeSalaryAdvanceReturnViewModel model)
        {
            long maxkey = dbContext.EmployeeSalaryAdvanceReturnDetails.Select(x => x.RowKey).DefaultIfEmpty().Max();
            if (model.ReturnDetails.Count() != 0)
            {
                decimal advance = 0;
                decimal Amount = (model.PaidAmount ?? 0);
                foreach (EmployeeSalaryAdvanceReturnDetailViewModel advanceList in model.ReturnDetails)
                {

                    if (dbContext.EmployeeSalaryAdvanceReturnDetails.Any(x => x.RowKey == advanceList.RowKey))
                    {
                        EmployeeSalaryAdvancePayment AdvancePayment = new EmployeeSalaryAdvancePayment();
                        EmployeeSalaryAdvanceReturnDetail AdvanceDetail = new EmployeeSalaryAdvanceReturnDetail();
                        AdvanceDetail = dbContext.EmployeeSalaryAdvanceReturnDetails.SingleOrDefault(x => x.RowKey == advanceList.RowKey);
                        AdvancePayment = dbContext.EmployeeSalaryAdvancePayments.SingleOrDefault(x => x.RowKey == advanceList.AdvanceKey);
                        if (advanceList.IsDeduct == true)
                        {
                            if (Amount != 0)
                            {
                                if (advanceList.ReturnAmount <= Amount)
                                {
                                    if (advanceList.ReturnAmount != (AdvancePayment.PaidAmount - ((AdvancePayment.ClearedAmount ?? 0) - AdvanceDetail.Amount)))
                                    {
                                        AdvancePayment.IsCleared = false;
                                    }
                                    else
                                    {
                                        AdvancePayment.IsCleared = true;
                                    }
                                    AdvancePayment.ClearedAmount = (AdvancePayment.ClearedAmount - AdvanceDetail.Amount) + (advanceList.ReturnAmount);
                                    Amount = Amount - (advanceList.ReturnAmount);
                                    AdvanceDetail.ReturnMasterKey = model.PaymentKey;
                                    AdvanceDetail.AdvanceKey = advanceList.AdvanceKey;
                                    AdvanceDetail.Amount = (advanceList.ReturnAmount);
                                }
                                else
                                {
                                    AdvancePayment.ClearedAmount = (AdvancePayment.ClearedAmount - AdvanceDetail.Amount) + Amount;
                                    advance = advance + Amount;
                                    AdvancePayment.IsCleared = false;
                                    AdvanceDetail.ReturnMasterKey = model.PaymentKey;
                                    AdvanceDetail.AdvanceKey = advanceList.AdvanceKey;
                                    AdvanceDetail.Amount = Amount;
                                    Amount = 0;
                                }
                            }
                        }
                        else
                        {
                            AdvancePayment.ClearedAmount = (AdvancePayment.ClearedAmount ?? 0) - AdvanceDetail.Amount;
                            AdvancePayment.IsCleared = false;
                            dbContext.EmployeeSalaryAdvanceReturnDetails.Remove(AdvanceDetail);
                        }
                    }
                    else
                    {
                        if (advanceList.IsDeduct == true && Amount != 0)
                        {
                            EmployeeSalaryAdvancePayment AdvancePayment = new EmployeeSalaryAdvancePayment();
                            EmployeeSalaryAdvanceReturnDetail AdvanceDetail = new EmployeeSalaryAdvanceReturnDetail();
                            AdvancePayment = dbContext.EmployeeSalaryAdvancePayments.SingleOrDefault(x => x.RowKey == advanceList.AdvanceKey);
                            if (advanceList.ReturnAmount <= Amount)
                            {
                                if (advanceList.ReturnAmount != (AdvancePayment.PaidAmount - (AdvancePayment.ClearedAmount ?? 0)))
                                {
                                    AdvancePayment.IsCleared = false;
                                }
                                else
                                {
                                    AdvancePayment.IsCleared = true;
                                }
                                AdvancePayment.ClearedAmount = (AdvancePayment.ClearedAmount ?? 0) + (advanceList.ReturnAmount);
                                Amount = Amount - (advanceList.ReturnAmount);
                                AdvanceDetail.RowKey = maxkey + 1;
                                AdvanceDetail.ReturnMasterKey = model.PaymentKey;
                                AdvanceDetail.AdvanceKey = advanceList.AdvanceKey;
                                AdvanceDetail.Amount = advanceList.ReturnAmount;
                            }
                            else
                            {
                                AdvancePayment.ClearedAmount = (AdvancePayment.ClearedAmount ?? 0) + Amount;
                                advance = advance + Amount;
                                AdvancePayment.IsCleared = false;
                                AdvanceDetail.RowKey = maxkey + 1;
                                AdvanceDetail.ReturnMasterKey = model.PaymentKey;
                                AdvanceDetail.AdvanceKey = advanceList.AdvanceKey;
                                AdvanceDetail.Amount = Amount;
                                Amount = 0;
                            }
                            dbContext.EmployeeSalaryAdvanceReturnDetails.Add(AdvanceDetail);
                            maxkey++;
                        }
                    }
                }
            }
        }
        private void createAdvanceReturnDetail(EmployeeSalaryAdvanceReturnViewModel model)
        {
            if (model.ReturnDetails.Where(x => x.IsDeduct == true).Count() != 0)
            {
                decimal Amount = model.PaidAmount ?? 0;
                long maxkey = dbContext.EmployeeSalaryAdvanceReturnDetails.Select(x => x.RowKey).DefaultIfEmpty().Max();
                foreach (EmployeeSalaryAdvanceReturnDetailViewModel advanceList in model.ReturnDetails.Where(x => x.IsDeduct == true))
                {
                    if (Amount != 0)
                    {
                        EmployeeSalaryAdvancePayment AdvancePayment = new EmployeeSalaryAdvancePayment();
                        EmployeeSalaryAdvanceReturnDetail AdvanceDetail = new EmployeeSalaryAdvanceReturnDetail();
                        AdvancePayment = dbContext.EmployeeSalaryAdvancePayments.SingleOrDefault(x => x.RowKey == advanceList.AdvanceKey);
                        if (advanceList.ReturnAmount <= Amount)
                        {
                            if (advanceList.ReturnAmount != (AdvancePayment.PaidAmount - (AdvancePayment.ClearedAmount ?? 0)))
                            {
                                AdvancePayment.IsCleared = false;
                            }
                            else
                            {
                                AdvancePayment.IsCleared = true;
                            }
                            AdvancePayment.ClearedAmount = (AdvancePayment.ClearedAmount ?? 0) + (advanceList.ReturnAmount);
                            Amount = Amount - AdvancePayment.PaidAmount;
                            AdvanceDetail.RowKey = maxkey + 1;
                            AdvanceDetail.ReturnMasterKey = model.PaymentKey;
                            AdvanceDetail.AdvanceKey = advanceList.AdvanceKey;
                            AdvanceDetail.Amount = advanceList.ReturnAmount;
                        }
                        else
                        {
                            AdvancePayment.ClearedAmount = (AdvancePayment.ClearedAmount ?? 0) + Amount;
                            AdvancePayment.IsCleared = false;
                            AdvanceDetail.RowKey = maxkey + 1;
                            AdvanceDetail.ReturnMasterKey = model.PaymentKey;
                            AdvanceDetail.AdvanceKey = advanceList.AdvanceKey;
                            AdvanceDetail.Amount = Amount;
                            Amount = 0;
                        }
                        dbContext.EmployeeSalaryAdvanceReturnDetails.Add(AdvanceDetail);
                        maxkey++;
                    }
                }
            }
        }
        private void purposeGeneration(EmployeeSalaryAdvanceReturnViewModel model)
        {
            model.Purpose = (model.Purpose != null && model.Purpose != "" ? (EduSuiteUIResources.For + EduSuiteUIResources.BlankSpace + model.Purpose) : "");
            string ReceivedBy = EduSuiteUIResources.Salary + EduSuiteUIResources.BlankSpace + EduSuiteUIResources.Advance + EduSuiteUIResources.BlankSpace + EduSuiteUIResources.Return + EduSuiteUIResources.BlankSpace + EduSuiteUIResources.RecievedFrom + EduSuiteUIResources.BlankSpace + (model.ReceivedBy != null && model.ReceivedBy != "" ? model.ReceivedBy : model.EmployeeName) + EduSuiteUIResources.BlankSpace;
            string PaidBy = model.PaidBy != null && model.PaidBy != "" ? EduSuiteUIResources.PaidTo + EduSuiteUIResources.BlankSpace + model.PaidBy + EduSuiteUIResources.BlankSpace : "";
            string OnBehalfOf = model.OnBehalfOf != null && model.OnBehalfOf != "" ? EduSuiteUIResources.OnBehalfOf + EduSuiteUIResources.BlankSpace + model.OnBehalfOf + EduSuiteUIResources.BlankSpace : "";
            string AuthorizedBy = model.AuthorizedBy != null && model.AuthorizedBy != "" ? EduSuiteUIResources.AuthorizedBy + EduSuiteUIResources.BlankSpace + model.AuthorizedBy + EduSuiteUIResources.BlankSpace : "";
            model.Remarks = model.Remarks != null && model.Remarks != "" ? EduSuiteUIResources.OpenBracket + model.Remarks + EduSuiteUIResources.CloseBracket : "";
            model.Purpose = ReceivedBy + model.Purpose + PaidBy + OnBehalfOf + AuthorizedBy;
            model.PaymentModeName = EduSuiteUIResources.By + EduSuiteUIResources.BlankSpace + (model.PaymentModeKey == DbConstants.PaymentMode.Cash ? EduSuiteUIResources.Cash : model.PaymentModeKey == DbConstants.PaymentMode.Bank ? EduSuiteUIResources.Bank + EduSuiteUIResources.OpenBracket + model.BankAccountName + EduSuiteUIResources.CloseBracket : model.PaymentModeKey == DbConstants.PaymentMode.Cheque ? EduSuiteUIResources.Cheque + EduSuiteUIResources.OpenBracket + model.BankAccountName + EduSuiteUIResources.CloseBracket : "");
        }
        public EmployeeSalaryAdvanceReturnViewModel DeleteSalaryAdvanceReturn(EmployeeSalaryAdvanceReturnViewModel model)
        {
            EmployeeSalaryAdvanceReturnMaster EmployeeSalaryAdvanceReturnModel = new EmployeeSalaryAdvanceReturnMaster();
            using (var transaction = dbContext.Database.BeginTransaction())
            {
                try
                {
                    EmployeeSalaryAdvanceReturnModel = dbContext.EmployeeSalaryAdvanceReturnMasters.SingleOrDefault(row => row.RowKey == model.PaymentKey);
                    long RowKey = EmployeeSalaryAdvanceReturnModel.RowKey;


                    List<EmployeeSalaryAdvanceReturnDetail> DetailList = dbContext.EmployeeSalaryAdvanceReturnDetails.Where(x => x.ReturnMasterKey == model.PaymentKey).ToList();
                    if (DetailList.Count > 0)
                    {
                        foreach (EmployeeSalaryAdvanceReturnDetail row in DetailList)
                        {
                            EmployeeSalaryAdvancePayment employeeSalaryAdvancePayment = new EmployeeSalaryAdvancePayment();
                            employeeSalaryAdvancePayment = dbContext.EmployeeSalaryAdvancePayments.SingleOrDefault(x => x.RowKey == row.AdvanceKey);
                            employeeSalaryAdvancePayment.ClearedAmount = employeeSalaryAdvancePayment.ClearedAmount - row.Amount;
                            employeeSalaryAdvancePayment.IsCleared = false;
                        }
                        dbContext.EmployeeSalaryAdvanceReturnDetails.RemoveRange(DetailList);
                    }
                    ConfigurationViewModel ConfigModel = new ConfigurationViewModel();
                    ConfigModel.BranchKey = EmployeeSalaryAdvanceReturnModel.PaidBranchKey ?? 0;
                    ConfigModel.SerialNumber = EmployeeSalaryAdvanceReturnModel.SerialNumber ?? 0;
                    ConfigModel.IsDelete = true;
                    ConfigModel.ConfigType = DbConstants.PaymentReceiptConfigType.ReceiptVoucher;
                    Configurations.GenerateReceipt(dbContext, ConfigModel);

                    dbContext.EmployeeSalaryAdvanceReturnMasters.Remove(EmployeeSalaryAdvanceReturnModel);
                    AccountFlowViewModel accountFlowModel = new AccountFlowViewModel();
                    AccountFlowService accountFlowService = new AccountFlowService(dbContext);
                    accountFlowModel.TransactionTypeKey = DbConstants.TransactionType.SalaryAdvanceReturn;
                    accountFlowModel.TransactionKey = RowKey;
                    accountFlowService.DeleteAccountFlow(accountFlowModel);
                    dbContext.SaveChanges();
                    transaction.Commit();
                    model.Message = EduSuiteUIResources.Success;
                    model.IsSuccessful = true;
                    ActivityLog.CreateActivityLog(MenuConstants.EmployeeSalaryAdvanceReturn, ActionConstants.Delete, DbConstants.LogType.Info, model.PaymentKey, model.Message);

                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    //model.ExceptionMessage = ex.GetBaseException().Message;
                    model.Message = String.Format(EduSuiteUIResources.FailedToSaveErrorMessage, EduSuiteUIResources.SalaryPayment);
                    model.IsSuccessful = false;
                    ActivityLog.CreateActivityLog(MenuConstants.EmployeeSalaryAdvanceReturn, ActionConstants.Delete, DbConstants.LogType.Error, model.PaymentKey, ex.GetBaseException().Message);

                }

            }

            return model;
        }
        private void FillSalaryAdvanceReturnDropdownLists(EmployeeSalaryAdvanceReturnViewModel model)
        {
            EmployeeSalaryAdvanceViewModel advanceModel = new EmployeeSalaryAdvanceViewModel();
            advanceModel.BranchKey = model.BranchKey;
            GetBranches(advanceModel);
            FillEmployees(advanceModel);
            FillPaymentModes(advanceModel);
            FillPaymentModeSub(advanceModel);
            FillBankAccounts(advanceModel);
            FillPaidBranches(advanceModel);
            model.Branches = advanceModel.Branches;
            model.Employees = advanceModel.Employees;
            model.PaymentModes = advanceModel.PaymentModes;
            model.PaymentModeSub = advanceModel.PaymentModeSub;
            model.BankAccounts = advanceModel.BankAccounts;
            model.PaidBranches = advanceModel.PaidBranches;

        }
        public EmployeeSalaryAdvanceReturnViewModel fillAdvances(EmployeeSalaryAdvanceReturnViewModel model)
        {
            if (model.PaymentKey != 0)
            {
                model.ReturnDetails = dbContext.EmployeeSalaryAdvanceReturnDetails.Where(x => x.ReturnMasterKey == model.PaymentKey).Select(x => new EmployeeSalaryAdvanceReturnDetailViewModel
                {
                    RowKey = x.RowKey,
                    AdvanceKey = x.AdvanceKey,
                    Purpose = x.EmployeeSalaryAdvancePayment.Purpose,
                    PaymentDate = x.EmployeeSalaryAdvancePayment.PaymentDate,
                    ReturnAmount = x.Amount,
                    BeforeTakenAdvance = (x.EmployeeSalaryAdvancePayment.PaidAmount) - (((x.EmployeeSalaryAdvancePayment.ClearedAmount ?? 0) - (dbContext.EmployeeSalaryAdvanceReturnDetails.Where(y => y.DateAdded > x.DateAdded && y.AdvanceKey == x.AdvanceKey).Select(y => y.Amount).DefaultIfEmpty().Sum())) - x.Amount),
                    IsDeduct = true
                }).Union(dbContext.EmployeeSalaryAdvancePayments.Where(x => x.EmployeeKey == model.EmployeeKey
                    && (x.ClearedAmount ?? 0) == 0 && ((x.ChequeStatusKey ?? DbConstants.ProcessStatus.Approved) == DbConstants.ProcessStatus.Approved)
                    && System.Data.Entity.DbFunctions.TruncateTime(x.PaymentDate) <= System.Data.Entity.DbFunctions.TruncateTime(dbContext.EmployeeSalaryAdvanceReturnMasters.Where(y => y.RowKey == model.PaymentKey).Select(y => y.DateAdded).FirstOrDefault())).Select(x => new EmployeeSalaryAdvanceReturnDetailViewModel
                    {
                        RowKey = 0,
                        AdvanceKey = x.RowKey,
                        Purpose = x.Purpose,
                        PaymentDate = x.PaymentDate,
                        ReturnAmount = (x.PaidAmount),
                        BeforeTakenAdvance = x.PaidAmount,
                        IsDeduct = false
                    })).ToList();
            }
            else
            {
                model.ReturnDetails = dbContext.EmployeeSalaryAdvancePayments.Where(x => x.EmployeeKey == model.EmployeeKey && x.IsCleared == false && ((x.ChequeStatusKey ?? DbConstants.ProcessStatus.Approved) == DbConstants.ProcessStatus.Approved)).Select(x => new EmployeeSalaryAdvanceReturnDetailViewModel
                {
                    RowKey = 0,
                    AdvanceKey = x.RowKey,
                    Purpose = x.Purpose,
                    PaymentDate = x.PaymentDate,
                    ReturnAmount = x.PaidAmount - (x.ClearedAmount ?? 0),
                    BeforeTakenAdvance = x.PaidAmount - (x.ClearedAmount ?? 0),
                    IsDeduct = true
                }).ToList();
            }
            return model;
        }


        private List<AccountFlowViewModel> ReturnAmountList(EmployeeSalaryAdvanceReturnViewModel model, List<AccountFlowViewModel> accountFlowModelList, long oldBankKey, bool IsUpdate)
        {
            long accountHeadKey;
            long ExtraUpdateKey = 0;
            //accountHeadCode = dbContext.VwAccountHeadSelectActiveOnlies.Where(x => x.RowKey == DbConstants.AccountHead.SalaryAdvanceRecievable).Select(x => x.AccountHeadCode).FirstOrDefault();
            accountHeadKey = dbContext.Employees.Where(x => x.RowKey == model.EmployeeKey).Select(x => x.AccountHeadKey ?? 0).FirstOrDefault();
            accountFlowModelList.Add(new AccountFlowViewModel
            {
                CashFlowTypeKey = DbConstants.CashFlowType.Out,
                AccountHeadKey = accountHeadKey,
                Amount = model.PaidAmount ?? 0,
                TransactionTypeKey = DbConstants.TransactionType.SalaryAdvanceReturn,
                VoucherTypeKey = DbConstants.VoucherType.AdvanceReturn,
                TransactionKey = model.PaymentKey,
                TransactionDate = model.PaymentModeKey != DbConstants.PaymentMode.Cheque ? model.PaymentDate : (model.ChequeOrDDDate ?? model.PaymentDate),
                ExtraUpdateKey = ExtraUpdateKey,
                IsUpdate = IsUpdate,
                BranchKey = model.PaidBranchKey != null ? model.PaidBranchKey : model.BranchKey,
                Purpose = model.Purpose + model.PaymentModeName + model.Remarks,
            });

            long oldBankAccountHeadKey = 0;
            if (model.OldPaymentModeKey == DbConstants.PaymentMode.Bank || model.OldPaymentModeKey == DbConstants.PaymentMode.Cheque)
            {
                long AccountHeadKey = dbContext.BankAccounts.Where(x => x.RowKey == oldBankKey).Select(x => x.AccountHeadKey).FirstOrDefault();
                //oldBankAccountHeadKey = dbContext.VwAccountHeadSelectActiveOnlies.Where(x => x.AccountHeadCode == bankAccountCode).Select(x => x.RowKey).FirstOrDefault();
                oldBankAccountHeadKey = AccountHeadKey;
            }
            if (model.PaymentModeKey == DbConstants.PaymentMode.Bank || model.PaymentModeKey == DbConstants.PaymentMode.Cheque)
            {
                accountHeadKey = dbContext.BankAccounts.Where(x => x.RowKey == model.BankAccountKey).Select(x => x.AccountHeadKey).FirstOrDefault();
            }
            else
            {
                accountHeadKey = dbContext.VwAccountHeadSelectActiveOnlies.Where(x => x.RowKey == DbConstants.AccountHead.CashAccount).Select(x => x.RowKey).FirstOrDefault();
            }
            if (model.OldPaymentModeKey != null && model.OldPaymentModeKey != 0 && model.OldPaymentModeKey != model.PaymentModeKey)
            {
                IsUpdate = false;
                ExtraUpdateKey = model.OldPaymentModeKey == DbConstants.PaymentMode.Cash ? DbConstants.AccountHead.CashAccount : oldBankAccountHeadKey;
            }
            accountFlowModelList.Add(new AccountFlowViewModel
            {
                CashFlowTypeKey = DbConstants.CashFlowType.In,
                AccountHeadKey = accountHeadKey,
                Amount = model.PaidAmount ?? 0,
                TransactionTypeKey = DbConstants.TransactionType.SalaryAdvanceReturn,
                VoucherTypeKey = DbConstants.VoucherType.AdvanceReturn,
                TransactionDate = model.PaymentModeKey != DbConstants.PaymentMode.Cheque ? model.PaymentDate : (model.ChequeOrDDDate ?? model.PaymentDate),
                ExtraUpdateKey = ExtraUpdateKey,
                IsUpdate = IsUpdate,
                Purpose = model.Purpose + model.Remarks,
                BranchKey = model.PaidBranchKey != null ? model.PaidBranchKey : model.BranchKey,
                TransactionKey = model.PaymentKey,
            });
            return accountFlowModelList;
        }

        #endregion

    }
}
