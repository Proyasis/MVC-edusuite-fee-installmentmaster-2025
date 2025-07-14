using System;
using System.Collections.Generic;
using System.Data.Entity.Infrastructure;
using System.Globalization;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using CITS.EduSuite.Business.Common;
using CITS.EduSuite.Business.Interfaces;
using CITS.EduSuite.Business.Models.Resources;
using CITS.EduSuite.Business.Models.ViewModels;
using CITS.EduSuite.Data;

namespace CITS.EduSuite.Business.Services
{
    public class EmployeeAttendanceService : IEmployeeAttendanceService
    {
        private EduSuiteDatabase dbContext;
        long attendanceMaxKey;
        long attendanceLogMaxKey;

        int ComittedCount = 0;
        string[] cols = new[] { "RowKey", "AttendanceDate", "InDateTime", "OutDateTime", "EmployeeKey", "AttendanceStatusKey", "AttendancePresentStatusKey", "BiomatricId", "Remarks" };

        public EmployeeAttendanceService()
        {
            this.dbContext = new EduSuiteDatabase();
            attendanceMaxKey = 0;
            attendanceLogMaxKey = 0;
        }

        public List<EmployeeAttendanceViewModel> GetEmployeeAttendance(EmployeeAttendanceViewModel model)
        {

            try
            {
                var Take = model.PageSize;
                var Skip = (model.PageIndex - 1) * model.PageSize;

                var employeeAttendanceList = (from ea in dbContext.EmployeeAttendances
                                              orderby ea.AttendanceDate descending
                                              select new EmployeeAttendanceViewModel
                                              {
                                                  RowKey = ea.RowKey,
                                                  BranchKey = ea.Employee.BranchKey,
                                                  EmployeeKey = ea.EmployeeKey,
                                                  EmployeeName = ea.Employee.FirstName + " " + (ea.Employee.MiddleName ?? "") + " " + ea.Employee.LastName,
                                                  BranchName = ea.Branch.BranchName,
                                                  //DepartmentName = ea.Department.DepartmentName,
                                                  AttendanceDate = ea.AttendanceDate,
                                                  InDateTime = ea.InTime,
                                                  OutDateTime = ea.OutTime,
                                                  Remarks = ea.Remarks,
                                                  ApprovalStatusKey = ea.ApprovalStatusKey,
                                                  ApprovalStatusName = ea.ProcessStatu.ProcessStatusName,
                                                  AttendanceStatusKey = ea.AttendanceStatusKey ?? 0,
                                                  AttendanceStatusName = ea.LeaveTypeKey != null ? ea.LeaveType.LeaveTypeName : ea.EmployeeAttendanceStatu1.AttendanceStatusName,
                                                  AttendanceStatusColor = ea.LeaveTypeKey != null ? ea.LeaveType.LeaveTypeColor : ea.EmployeeAttendanceStatu1.AttendanceStatusColor,
                                                  ClockInStatus = ea.OutTime == null && ea.InTime != null ? true : false,
                                                  AttendanceConfigType = DbConstants.AttendanceConfigType.MarkPresent
                                              });

                if (model.EmployeeKey != 0)
                {
                    employeeAttendanceList = employeeAttendanceList.Where(row => row.EmployeeKey == model.EmployeeKey);
                }
                if (model.BranchKey != 0)
                {
                    employeeAttendanceList = employeeAttendanceList.Where(row => row.BranchKey == model.BranchKey);
                }
                if (model.AttendanceStatusKey != 0)
                {
                    employeeAttendanceList = employeeAttendanceList.Where(row => row.AttendanceStatusKey == model.AttendanceStatusKey);
                }
                if (model.SearchFromDate != null)
                {
                    employeeAttendanceList = employeeAttendanceList.Where(row => row.AttendanceDate >= model.SearchFromDate);
                }
                if (model.SearchToDate != null)
                {
                    employeeAttendanceList = employeeAttendanceList.Where(row => row.AttendanceDate <= model.SearchToDate);
                }


                if (model.SortBy != "")
                {
                    employeeAttendanceList = SortEmployeeAttendance(employeeAttendanceList, model.SortBy, model.SortOrder);
                }
                model.TotalRecords = employeeAttendanceList.Count();
                return employeeAttendanceList.Skip(Skip).Take(Take).ToList<EmployeeAttendanceViewModel>();
            }
            catch (Exception ex)
            {
                //////model.ExceptionMessage = ex.GetBaseException().Message;
                ActivityLog.CreateActivityLog(MenuConstants.EmployeeAttendance, ActionConstants.MenuAccess, DbConstants.LogType.Error, DbConstants.User.UserKey, ex.GetBaseException().Message);
                return new List<EmployeeAttendanceViewModel>();
            }

        }
        private IQueryable<EmployeeAttendanceViewModel> SortEmployeeAttendance(IQueryable<EmployeeAttendanceViewModel> Query, string SortName, string SortOrder)
        {

            var type = typeof(EmployeeAttendanceViewModel);

            string command = SortOrder == "desc" ? "OrderByDescending" : "OrderBy";
            var property = type.GetProperty(SortName);
            var parameter = Expression.Parameter(type, "p");
            var propertyAccess = Expression.MakeMemberAccess(parameter, property);
            var orderByExpression = Expression.Lambda(propertyAccess, parameter);
            var resultExpression = Expression.Call(typeof(Queryable), command, new Type[] { type, property.PropertyType },
                                          Query.Expression, Expression.Quote(orderByExpression));
            return Query.Provider.CreateQuery<EmployeeAttendanceViewModel>(resultExpression);


        }
        public List<EmployeeAttendanceViewModel> GetEmployeesForQuickAttendance(EmployeeAttendanceViewModel model)
        {
            var DateNow = model.AttendanceDate != null ? model.AttendanceDate : DateTimeUTC.Now.Date;

            DateTime date = Convert.ToDateTime(DateNow);
            DayOfWeek Dayweek = date.DayOfWeek;
            int Week = CultureInfo.InvariantCulture.Calendar.GetWeekOfYear(date, System.Globalization.CalendarWeekRule.FirstDay, DayOfWeek.Sunday);

            //string DayName = Dayweek.ToString();
            //int id = (int)date.DayOfWeek;
            int day = ((int)date.DayOfWeek + 6) % 7 + 1;

            AttendanceConfiguration attendanceConfigurationlist = dbContext.AttendanceConfigurations.Where(row => (row.BranchKey ?? model.BranchKey) == model.BranchKey).SingleOrDefault();

            //Shift shiftList = dbContext.Shifts
            //    //.Where(x => x.RowKey == attendanceConfigurationlist.ShiftKey)
            //    .SingleOrDefault();
            //DateTime ShiftListBeginTime = date;
            //DateTime ShiftListEndTime = date;
            //DateTime ShiftListPartialDayBeginTime = date;
            //DateTime ShiftListPartialDayEndTime = date;


            //ShiftListBeginTime = ShiftListBeginTime + shiftList.BeginTime;
            //ShiftListEndTime = ShiftListEndTime + shiftList.EndTime;

            //bool partialday = false;
            //if (shiftList.PartialDay == true)
            //{
            //    if (day == shiftList.PartialDayKey)
            //    {
            //        partialday = true;

            //        //ShiftListPartialDayBeginTime.Add((shiftList.PartialDayBeginTime ?? TimeSpan.Zero));
            //        //ShiftListPartialDayEndTime.Add((shiftList.PartialDayEndTime ?? TimeSpan.Zero));
            //        ShiftListPartialDayBeginTime = ShiftListPartialDayBeginTime + (shiftList.PartialDayBeginTime ?? TimeSpan.Zero);
            //        ShiftListPartialDayEndTime = ShiftListPartialDayEndTime + (shiftList.PartialDayEndTime ?? TimeSpan.Zero);
            //    }
            //}

            var IfHolidays = dbContext.Holidays.Any(x => x.BranchKey == model.BranchKey && (x.HolidayFrom <= DateNow && x.HolidayTo >= DateNow));

            Holiday Holidays = new Holiday();

            Holidays = dbContext.Holidays.SingleOrDefault(x => x.BranchKey == model.BranchKey && (x.HolidayFrom <= DateNow && x.HolidayTo >= DateNow));


            //bool WeeklyOffDay = false;

            //AttendanceCategory attendanceCategoryList = dbContext.AttendanceCategories
            //    //.Where(x => x.RowKey == attendanceConfigurationlist.AttendanceCatagoryKey)
            //    .SingleOrDefault();
            //if (attendanceCategoryList.WeekOffDay1Key != null || attendanceCategoryList.WeekOffDay2Key != null)
            //{
            //    if (day == attendanceCategoryList.WeekOffDay1Key)
            //    {
            //        WeeklyOffDay = true;
            //    }
            //    if (day == attendanceCategoryList.WeekOffDay2Key)
            //    {
            //        WeeklyOffDay = true;
            //    }
            //}




            try
            {
                IQueryable<EmployeeAttendanceViewModel> employeeAttendanceList = (from a in dbContext.Employees
                                                                                  join ac in dbContext.AttendanceCategories on a.AttendanceCategoryKey equals ac.RowKey
                                                                                  from es in dbContext.fnGetEmployeeShiftByDate(model.AttendanceDate, a.RowKey, a.BranchKey, a.DepartmentKey)

                                                                                  join s in dbContext.Shifts on es.ShiftKey equals s.RowKey
                                                                                  join ea in dbContext.EmployeeAttendances.Where(row => System.Data.Entity.DbFunctions.TruncateTime(row.AttendanceDate) == DateNow)
                                                                                  on a.RowKey equals ea.EmployeeKey into att
                                                                                  from ea in att.DefaultIfEmpty()

                                                                                  select new EmployeeAttendanceViewModel
                                                                                  {
                                                                                      RowKey = ea.RowKey != null ? ea.RowKey : 0,
                                                                                      EmployeeKey = a.RowKey,
                                                                                      BranchKey = a.BranchKey,
                                                                                      //DepartmentKey = e.DepartmentKey,
                                                                                      EmployeeName = a.FirstName + " " + (a.MiddleName != null ? a.MiddleName : "") + " " + a.LastName + EduSuiteUIResources.BlankSpace + EduSuiteUIResources.OpenBracket + a.EmployeeCode + EduSuiteUIResources.CloseBracket,
                                                                                      BranchName = a.Branch.BranchName,
                                                                                      // DepartmentName = e.Department.DepartmentName,
                                                                                      AttendanceDate = ea.AttendanceDate != null ? ea.AttendanceDate : date,
                                                                                      //InDateTime = ea.InTime,
                                                                                      //OutDateTime = ea.OutTime,
                                                                                      InDateTime = ea.InTime != null ? ea.InTime : es.InDateTime,
                                                                                      OutDateTime = ea.OutTime != null ? ea.OutTime : es.OutDateTime,

                                                                                      ClockInStatus = ea.OutTime == null && ea.InTime != null ? true : false,
                                                                                      AttendanceConfigType = ea.AttendanceConfigTypeKey == null ? dbContext.AttendanceConfigurations
                                                                                      //.Where(row => row.CompanyKey == a.Branch.CompanyKey)
                                                                                      .Select(x => x.AttendanceConfigTypeKey).FirstOrDefault() : ea.AttendanceConfigTypeKey ?? 0,
                                                                                      //AttendanceStatusKey = ea.AttendanceStatusKey ?? 0,
                                                                                      AttendanceStatusKey = ea.AttendanceStatusKey ?? (a.LeaveApplications.Any(x => x.EmployeeKey == a.RowKey && (x.LeaveFrom <= DateNow && x.LeaveTo >= DateNow) && x.LeaveStatusKey == DbConstants.ProcessStatus.Approved) == true ? DbConstants.EmployeeAttendanceStatus.Leave : (a.AttendanceCategory.AttendanceCategoryWeekOffs.Where(x => x.WeekOffDayKey == day && x.WeekOffDayWeekKeys.Split(',').Select(int.Parse).ToList().Contains(Week))).Any() ? DbConstants.EmployeeAttendanceStatus.Off : (IfHolidays == true ? DbConstants.EmployeeAttendanceStatus.Holyday : (ea.AttendanceStatusKey ?? 0))),
                                                                                      AttendanceStatusName = ea.LeaveTypeKey != null ? ea.LeaveType.LeaveTypeName : ea.EmployeeAttendanceStatu1.AttendanceStatusName,
                                                                                      AttendanceStatusColor = ea.LeaveTypeKey != null ? ea.LeaveType.LeaveTypeColor : ea.EmployeeAttendanceStatu1.AttendanceStatusColor,
                                                                                      AttendancePresentStatusKey = ea.AttendancePresentStatusKey ?? 0,
                                                                                      LeaveTypeKey = ea.LeaveTypeKey ?? a.LeaveApplications.Where(x => x.EmployeeKey == a.RowKey && (x.LeaveFrom <= DateNow && x.LeaveTo >= DateNow) && x.LeaveStatusKey == DbConstants.ProcessStatus.Approved).Select(row => row.LeaveTypeKey).FirstOrDefault(),
                                                                                      //AttendanceStatusRemarks = ea.Remarks != null ? ea.Remarks : ((ac.WeekOffDay1Key == day || ac.WeekOffDay2Key == day) ? EduSuiteUIResources.Today + EduSuiteUIResources.BlankSpace + EduSuiteUIResources.Is + EduSuiteUIResources.BlankSpace + EduSuiteUIResources.WeeklyOffDay : Holidays != null ? EduSuiteUIResources.Today + EduSuiteUIResources.BlankSpace + EduSuiteUIResources.Is + EduSuiteUIResources.BlankSpace + EduSuiteUIResources.Holiday + EduSuiteUIResources.OpenBracket + Holidays.HolidayTitle + EduSuiteUIResources.CloseBracket : ""),


                                                                                  });
                if (model.BranchKey != 0)
                {
                    employeeAttendanceList = employeeAttendanceList.Where(row => row.BranchKey == model.BranchKey);
                    short companyKey = dbContext.Branches.Where(x => x.RowKey == model.BranchKey).Select(row => row.CompanyKey).SingleOrDefault();

                }
                return employeeAttendanceList.GroupBy(x => x.EmployeeKey).Select(y => y.FirstOrDefault()).ToList<EmployeeAttendanceViewModel>();
            }
            catch (Exception ex)
            {
                ActivityLog.CreateActivityLog(MenuConstants.EmployeeAttendance, ActionConstants.MenuAccess, DbConstants.LogType.Error, DbConstants.User.UserKey, ex.GetBaseException().Message);
                return new List<EmployeeAttendanceViewModel>();
            }

        }
        public EmployeeAttendanceViewModel GetEmployeeAttendanceById(EmployeeAttendanceViewModel model)
        {
            try
            {
                EmployeeAttendanceViewModel employeeAttendanceViewModel = new EmployeeAttendanceViewModel();
                employeeAttendanceViewModel = dbContext.EmployeeAttendances.Select(row => new EmployeeAttendanceViewModel
                {
                    RowKey = row.RowKey,
                    EmployeeKey = row.EmployeeKey,
                    BranchKey = row.Employee.BranchKey,
                    AttendanceDate = row.AttendanceDate,
                    InDateTime = row.InTime,
                    OutDateTime = row.OutTime,
                    Remarks = row.Remarks,
                    ClockInStatus = row.OutTime == null && row.InTime != null ? true : false,

                    AttendanceStatusKey = (row.IsHalfDay ?? false) && row.AttendanceConfigTypeKey == DbConstants.AttendanceConfigType.MarkPresent ? DbConstants.EmployeeAttendanceStatus.Halfday : (row.IsPresent ? DbConstants.EmployeeAttendanceStatus.Present : DbConstants.EmployeeAttendanceStatus.Absent),
                    LeaveTypeKey = row.LeaveTypeKey ?? 0,
                    AttendancePresentStatusKey = row.AttendancePresentStatusKey ?? 0,
                    AttendanceConfigType = row.AttendanceConfigTypeKey ?? 0
                }).Where(x => x.RowKey == model.RowKey).FirstOrDefault();

                if (employeeAttendanceViewModel == null)
                {

                    employeeAttendanceViewModel = new EmployeeAttendanceViewModel();
                    employeeAttendanceViewModel.EmployeeKey = model.EmployeeKey;
                    employeeAttendanceViewModel.BranchKey = model.BranchKey;
                    employeeAttendanceViewModel.AttendanceConfigType = dbContext.Employees.Where(row => row.RowKey == model.EmployeeKey).Select(row => row.AttendanceConfigTypeKey ?? 0).SingleOrDefault();
                }
                // employeeAttendanceViewModel.AttendanceConfigType = DbConstants.AttendanceConfigType.MarkPresent;
                if (employeeAttendanceViewModel.AttendanceConfigType != DbConstants.AttendanceConfigType.MarkPresent)
                {
                    //employeeAttendanceViewModel.OutTime = employeeAttendanceViewModel.InTime != null && employeeAttendanceViewModel.OutTime == null ? DateTimeUTC.Now.TimeOfDay : employeeAttendanceViewModel.OutTime;
                    //employeeAttendanceViewModel.InTime = employeeAttendanceViewModel.InTime == null ? DateTimeUTC.Now.TimeOfDay : employeeAttendanceViewModel.InTime;
                    employeeAttendanceViewModel.OutDateTime = employeeAttendanceViewModel.InDateTime != null && employeeAttendanceViewModel.OutDateTime == null ? DateTimeUTC.Now : employeeAttendanceViewModel.OutDateTime;
                    employeeAttendanceViewModel.InDateTime = employeeAttendanceViewModel.InDateTime == null ? DateTimeUTC.Now : employeeAttendanceViewModel.InDateTime;
                }

                FillDropdownList(employeeAttendanceViewModel);
                return employeeAttendanceViewModel;
            }
            catch (Exception ex)
            {
                model = new EmployeeAttendanceViewModel();
                ////model.ExceptionMessage = ex.GetBaseException().Message;
                ActivityLog.CreateActivityLog(MenuConstants.EmployeeAttendance, (model.RowKey != 0 ? ActionConstants.Edit : ActionConstants.Add), DbConstants.LogType.Error, model.RowKey, ex.GetBaseException().Message);

                return model;

                //ActivityLog.CreateActivityLog(MenuConstants.EmployeeAttendance, ActionConstants.View, DbConstants.LogType.Error, model.RowKey, ex.GetBaseException().Message);
            }
        }
        public EmployeeAttendanceViewModel UpdateEmployeesAttendance(List<EmployeeAttendanceViewModel> modelList, bool IsMultiple)
        {
            EmployeeAttendanceViewModel employeeAttendanceModel = new EmployeeAttendanceViewModel();

            using (var transaction = dbContext.Database.BeginTransaction())
            {
                try
                {
                    //dbContext.Configuration.AutoDetectChangesEnabled = false;

                    //if (isExcel)
                    //{
                    //    var EmployeeKey = 0;
                    //    foreach (EmployeeAttendanceViewModel model in modelList)
                    //    {
                    //        model.RowKey = dbContext.EmployeeAttendances.Where(row => row.EmployeeKey == model.EmployeeKey && row.AttendanceDate == model.AttendanceDate).Select(row => row.RowKey).SingleOrDefault();
                    //        model.LeaveTypeKey = dbContext.LeaveTypes.Where(row => row.LeaveTypeShortName == model.AttendanceStatusCode).Select(row => row.RowKey).FirstOrDefault();
                    //        model.AttendanceStatusKey = dbContext.EmployeeAttendanceStatus.Where(row => row.AttendanceStatusCode == model.AttendanceStatusCode).Select(row => row.RowKey).FirstOrDefault();
                    //        if (model.LeaveTypeKey != null)
                    //        {
                    //            model.AttendanceStatusKey = DbConstants.EmployeeAttendanceStatus.Leave;
                    //        }

                    //    }

                    //}

                    //foreach (EmployeeAttendanceViewModel model in modelList)
                    //{
                    //    CalculateOvertime(model);
                    //    if (model.RowKey != 0)
                    //    {
                    //        UpdateEmpoloyeeAttendance(model);
                    //    }
                    //    else
                    //    {
                    //        CreateEmpoloyeeAttendance(model);
                    //    }
                    //    CreateEmployeeAttendanceLog(model, IsMultiple);

                    //}
                    attendanceMaxKey = dbContext.EmployeeAttendances.Select(p => p.RowKey).DefaultIfEmpty().Max();
                    attendanceLogMaxKey = dbContext.EmployeeAttendanceLogs.Select(p => p.RowKey).DefaultIfEmpty().Max();

                    string xml = StringExtension.ListToXmlString(modelList, cols);
                    List<EmployeeAttendance> AttendanceModel = dbContext.spAttendanceCalculation(xml).Select(row => new EmployeeAttendance
                    {
                        RowKey = row.RowKey ?? 0,
                        ShiftKey = row.ShiftKey,
                        EmployeeKey = row.EmployeeKey,
                        BranchKey = row.BranchKey,
                        AttendanceDate = row.AttendanceDate ?? (row.InDateTime ?? DateTime.UtcNow),
                        InTime = row.InDateTime,
                        OutTime = row.OutDateTime,
                        Remarks = row.Remarks,
                        AttendanceStatusKey = (short?)(row.AttendanceStatusKey ?? DbConstants.EmployeeAttendanceStatus.Absent),
                        AttendanceDayStatusKey = (short?)row.AttendanceDayStatusKey,
                        ApprovalStatusKey = (short)(row.ApprovalStatusKey),
                        Duration = row.TotalDuration,
                        TotalBreak = row.TotalBreak,
                        LateComing = row.LateComingMinutes,
                        EarlyGoing = row.EarlyGoingMinutes,
                        LateGoing = row.LateGoingMinutes,
                        EarlyComing = row.EarlyComingMinutes,
                        OverTime = ((row.OverTime ?? 0) > 0 && (row.OverTime ?? 0) >= (row.MinOvertime ?? 0) && (row.OverTime ?? 0) <= (row.MaxOvertime ?? (row.OverTime ?? 0))) ? row.OverTime : null,
                        MissedOutPunch = row.AttendanceConfigTypeKey != DbConstants.AttendanceConfigType.MarkPresent && (new[] { DbConstants.AttendancePresentStatus.CheckIn, DbConstants.AttendancePresentStatus.BreakIn }).Contains((byte)(row.AttendancePresentStatusKey)),
                        AttendancePresentStatusKey = (byte?)row.AttendancePresentStatusKey,
                        AttendanceConfigTypeKey = row.AttendanceConfigTypeKey,
                        IsPresent = row.AttendanceStatusKey != DbConstants.EmployeeAttendanceStatus.Absent,
                        IsHalfDay = row.AttendanceStatusKey == DbConstants.EmployeeAttendanceStatus.Halfday,


                    }).ToList();

                    foreach (EmployeeAttendance model in AttendanceModel)
                    {
                        if (model.AttendanceConfigTypeKey == DbConstants.AttendanceConfigType.MarkPresent)
                        {
                            model.InTime = null;
                            model.OutTime = null;
                        }
                        else
                        {
                            if (model.AttendanceStatusKey != DbConstants.EmployeeAttendanceStatus.Present)
                            {
                                model.InTime = null;
                                model.OutTime = null;
                            }
                        }

                        //CalculateOvertime(model);
                        if (model.RowKey == 0)
                        {
                            model.RowKey = ++attendanceMaxKey;
                            dbContext.AddToContext(model, ++ComittedCount);
                        }
                        else
                        {
                            dbContext.AttachToContext(model, ++ComittedCount);
                        }
                        if (model.AttendanceConfigTypeKey != DbConstants.AttendanceConfigType.MarkPresent)
                        {
                            if (model.AttendanceStatusKey == DbConstants.EmployeeAttendanceStatus.Present)
                            {
                                CreateEmployeeAttendanceLog(model, IsMultiple);
                            }
                        }
                    }


                    //dbContext.ChangeTracker.DetectChanges();
                    dbContext.SaveChanges();
                    transaction.Commit();
                    employeeAttendanceModel.Message = EduSuiteUIResources.Success;
                    employeeAttendanceModel.IsSuccessful = true;
                    ActivityLog.CreateActivityLog(MenuConstants.EmployeeAttendance, ActionConstants.Edit, DbConstants.LogType.Info, modelList.Select(row => row.EmployeeKey).FirstOrDefault(), employeeAttendanceModel.Message);
                }
                catch (Exception ex)
                {

                    transaction.Rollback();

                    employeeAttendanceModel.Message = String.Format(EduSuiteUIResources.FailedToSaveErrorMessage, EduSuiteUIResources.EmployeeAttendance);
                    //employeeAttendance////model.ExceptionMessage = ex.GetBaseException().Message;
                    ActivityLog.CreateActivityLog(MenuConstants.EmployeeAttendance, ActionConstants.Edit, DbConstants.LogType.Error, modelList.Select(row => row.EmployeeKey).FirstOrDefault(), ex.GetBaseException().Message);
                    employeeAttendanceModel.IsSuccessful = false;
                }
            }
            return employeeAttendanceModel;
        }
        private void CreateEmpoloyeeAttendance(EmployeeAttendanceViewModel model)
        {

            if (attendanceMaxKey == 0)
            {
                attendanceMaxKey = dbContext.EmployeeAttendances.Select(p => p.RowKey).DefaultIfEmpty().Max();
            }

            EmployeeAttendance AttendanceModel = new EmployeeAttendance();

            Employee Employee = dbContext.Employees.SingleOrDefault(x => x.RowKey == model.EmployeeKey);
            AttendanceModel = new EmployeeAttendance();
            AttendanceModel.RowKey = ++attendanceMaxKey;
            AttendanceModel.Remarks = model.Remarks;
            AttendanceModel.BranchKey = Employee.BranchKey;
            // AttendanceModel.DepartmentKey = employee.DepartmentKey;
            AttendanceModel.EmployeeKey = model.EmployeeKey;
            var AutoApproval = dbContext.AttendanceConfigurations
                //.Where(row => row.CompanyKey == model.BranchKey)
                .Select(row => row.AutoApproval).FirstOrDefault();
            AttendanceModel.ApprovalStatusKey = AutoApproval ? DbConstants.ProcessStatus.Approved : DbConstants.ProcessStatus.Pending;
            AttendanceModel.AttendanceDate = Convert.ToDateTime(model.AttendanceDate);
            AttendanceModel.AttendanceStatusKey = model.AttendanceStatusKey;
            AttendanceModel.AttendancePresentStatusKey = model.AttendancePresentStatusKey != 0 ? model.AttendancePresentStatusKey : (byte?)null;


            AttendanceModel.InTime = model.InDateTime;
            AttendanceModel.OutTime = model.OutDateTime ?? model.ShiftOutTime;
            AttendanceModel.MissedOutPunch = model.AttendanceConfigType != DbConstants.AttendanceConfigType.MarkPresent && (new[] { DbConstants.AttendancePresentStatus.CheckIn, DbConstants.AttendancePresentStatus.BreakIn }).Contains(model.AttendancePresentStatusKey);


            AttendanceModel.LeaveTypeKey = model.LeaveTypeKey;
            model.EmployeeKey = AttendanceModel.EmployeeKey;
            model.AttendanceYearKey = Convert.ToInt16(AttendanceModel.AttendanceDate.Year);
            model.AttendanceMonthKey = Convert.ToByte(AttendanceModel.AttendanceDate.Month);

            AttendanceModel.AttendanceConfigTypeKey = model.AttendanceConfigType;
            AttendanceModel.Duration = model.Duration;
            AttendanceModel.EarlyGoing = model.EarlyBy;
            AttendanceModel.LateComing = model.LateBy;
            AttendanceModel.ShiftKey = model.ShiftKey;
            AttendanceModel.OverTime = model.OverTime;
            AttendanceModel.OverTimeE = model.OverTimeE;
            AttendanceModel.IsPresent = AttendanceModel.AttendanceStatusKey != DbConstants.EmployeeAttendanceStatus.Absent;
            AttendanceModel.IsHalfDay = model.IsHalfDay;


            dbContext.EmployeeAttendances.Add(AttendanceModel);
            if (model.LeaveTypeKey != null)
            {
                model.LeaveTypeKey = AttendanceModel.LeaveTypeKey ?? 0;
                //UpdateLeaveCarryForward(model);
            }


            model.RowKey = AttendanceModel.RowKey;

        }
        private void UpdateEmpoloyeeAttendance(EmployeeAttendanceViewModel model)
        {

            //long maxKey = dbContext.EmployeeAttendances.Select(p => p.RowKey).DefaultIfEmpty().Max();

            //long OvertimeKey = dbContext.EmployeeOvertimes.Select(p => p.RowKey).DefaultIfEmpty().Max();


            Employee Employee = dbContext.Employees.SingleOrDefault(x => x.RowKey == model.EmployeeKey);

            EmployeeAttendance AttendanceModel = new EmployeeAttendance();

            EmployeeAttendanceViewModel oldModel = new EmployeeAttendanceViewModel();

            AttendanceModel = dbContext.EmployeeAttendances.SingleOrDefault(x => x.RowKey == model.RowKey);
            AttendanceModel.Remarks = model.Remarks;
            AttendanceModel.BranchKey = Employee.BranchKey;
            AttendanceModel.EmployeeKey = model.EmployeeKey;
            AttendanceModel.ApprovalStatusKey = DbConstants.ProcessStatus.Pending;
            AttendanceModel.AttendanceDate = Convert.ToDateTime(model.AttendanceDate);
            AttendanceModel.AttendanceStatusKey = model.AttendanceStatusKey;
            AttendanceModel.AttendancePresentStatusKey = model.AttendancePresentStatusKey != 0 ? model.AttendancePresentStatusKey : (byte?)null;


            AttendanceModel.InTime = model.InDateTime;
            AttendanceModel.OutTime = model.OutDateTime ?? model.ShiftOutTime;
            AttendanceModel.MissedOutPunch = model.AttendanceConfigType != DbConstants.AttendanceConfigType.MarkPresent && (new[] { DbConstants.AttendancePresentStatus.CheckIn, DbConstants.AttendancePresentStatus.BreakIn }).Contains(model.AttendancePresentStatusKey);




            model.EmployeeKey = AttendanceModel.EmployeeKey;
            model.AttendanceYearKey = Convert.ToInt16(AttendanceModel.AttendanceDate.Year);
            model.AttendanceMonthKey = Convert.ToByte(AttendanceModel.AttendanceDate.Month);

            AttendanceModel.AttendanceConfigTypeKey = model.AttendanceConfigType;
            AttendanceModel.Duration = model.Duration;
            AttendanceModel.EarlyGoing = model.EarlyBy;
            AttendanceModel.LateComing = model.LateBy;
            AttendanceModel.ShiftKey = model.ShiftKey;
            AttendanceModel.OverTime = model.OverTime;
            AttendanceModel.OverTimeE = model.OverTimeE;
            AttendanceModel.IsPresent = AttendanceModel.AttendanceStatusKey != DbConstants.EmployeeAttendanceStatus.Absent;
            AttendanceModel.IsHalfDay = model.IsHalfDay;

            AttendanceModel.LeaveTypeKey = model.LeaveTypeKey = model.LeaveTypeKey;

            if (AttendanceModel.LeaveTypeKey != null)
            {

                oldModel.EmployeeKey = AttendanceModel.EmployeeKey;
                oldModel.LeaveTypeKey = AttendanceModel.LeaveTypeKey ?? 0;
                oldModel.AttendanceYearKey = Convert.ToInt16(AttendanceModel.AttendanceDate.Year);
                oldModel.AttendanceMonthKey = Convert.ToByte(AttendanceModel.AttendanceDate.Month);

            }
            if (oldModel != null && oldModel.LeaveTypeKey != model.LeaveTypeKey)
            {
                if (oldModel != null && oldModel.LeaveTypeKey != null && model.LeaveTypeKey != null)
                {
                    model.LeaveTypeKey = AttendanceModel.LeaveTypeKey ?? 0;

                    //UpdateLeaveCarryForwardWhenUpdate(model, oldModel);
                }
                else if (model != null && model.LeaveTypeKey != null)
                {
                    model.LeaveTypeKey = AttendanceModel.LeaveTypeKey ?? 0;
                    //UpdateLeaveCarryForward(model);

                }
                else if (oldModel != null && oldModel.LeaveTypeKey != null)
                {
                    //UpdateLeaveCarryForwardWhenDelete(oldModel);
                }


                //if (model.TotalOvertime > 0)
                //{
                //    EmployeeOvertime employeeOvertime = new EmployeeOvertime();
                //    employeeOvertime = dbContext.EmployeeOvertimes.SingleOrDefault(row => row.EmployeeKey == model.EmployeeKey && row.AttendanceMonth == model.AttendanceMonthKey && row.AttendanceYear == model.AttendanceYearKey);
                //    if (employeeOvertime == null)
                //    {
                //        employeeOvertime = new EmployeeOvertime();
                //        CreateOverTime(employeeOvertime, model, OvertimeKey);
                //        OvertimeKey++;
                //    }
                //    else
                //    {
                //        UpdateOverTime(employeeOvertime, model);
                //    }
                //}
            }
        }
        private void CreateEmployeeAttendanceLog(EmployeeAttendance model, bool IsMultiple)
        {

            List<byte> AttendancePresentStatuses = new List<byte>();
            if (IsMultiple)
            {
                if (model.InTime != null)
                {
                    AttendancePresentStatuses.Add(DbConstants.AttendancePresentStatus.CheckIn);
                }
                if (model.OutTime != null)
                {
                    AttendancePresentStatuses.Add(DbConstants.AttendancePresentStatus.CheckOut);
                }
            }
            else
            {
                if (model.AttendanceConfigTypeKey == DbConstants.AttendanceConfigType.MarkPresent)
                {
                    if (model.InTime != null)
                    {
                        AttendancePresentStatuses.Add(DbConstants.AttendancePresentStatus.CheckIn);
                    }
                    if (model.OutTime != null)
                    {
                        AttendancePresentStatuses.Add(DbConstants.AttendancePresentStatus.CheckOut);
                    }
                }
                else
                {
                    AttendancePresentStatuses.Add(model.AttendancePresentStatusKey ?? 0);
                }
            }
            foreach (byte AttendancePresentStatusKey in AttendancePresentStatuses)
            {
                model.AttendancePresentStatusKey = AttendancePresentStatusKey;
                EmployeeAttendanceLog EmployeeAttendanceLogModel = new EmployeeAttendanceLog();
                if (attendanceLogMaxKey == 0)
                {
                    attendanceLogMaxKey = dbContext.EmployeeAttendanceLogs.Select(p => p.RowKey).DefaultIfEmpty().Max();
                }


                if (model.InTime != null || model.OutTime != null)
                {

                    EmployeeAttendanceLogModel = dbContext.EmployeeAttendanceLogs.SingleOrDefault(row => row.AttendanceKey == model.RowKey && row.AttendancePresentStatusKey == model.AttendancePresentStatusKey && row.AttendancePresentStatusKey != DbConstants.AttendancePresentStatus.BreakIn && row.AttendancePresentStatusKey != DbConstants.AttendancePresentStatus.BreakOut);
                    if (EmployeeAttendanceLogModel == null)
                    {
                        EmployeeAttendanceLogModel = new EmployeeAttendanceLog();

                        EmployeeAttendanceLogModel.RowKey = ++attendanceLogMaxKey;
                        EmployeeAttendanceLogModel.AttendanceKey = model.RowKey;
                        EmployeeAttendanceLogModel.AttendanceDate = Convert.ToDateTime(model.AttendanceDate);
                        EmployeeAttendanceLogModel.AttendancePresentStatusKey = model.AttendancePresentStatusKey != 0 ? model.AttendancePresentStatusKey : (byte?)null;
                        EmployeeAttendanceLogModel.AttendanceStatusKey = model.AttendanceStatusKey;
                        if (model.AttendancePresentStatusKey == DbConstants.AttendancePresentStatus.CheckIn)
                        {
                            EmployeeAttendanceLogModel.InTime = model.InTime;
                        }
                        else
                        {
                            EmployeeAttendanceLogModel.InTime = model.OutTime;
                        }

                        dbContext.AddToContext(EmployeeAttendanceLogModel, ++ComittedCount);

                    }
                    else
                    {
                        EmployeeAttendanceLogModel.AttendanceDate = Convert.ToDateTime(model.AttendanceDate);
                        EmployeeAttendanceLogModel.AttendancePresentStatusKey = model.AttendancePresentStatusKey != 0 ? model.AttendancePresentStatusKey : (byte?)null;
                        EmployeeAttendanceLogModel.AttendanceStatusKey = model.AttendanceStatusKey;
                        if (model.AttendanceStatusKey == DbConstants.EmployeeAttendanceStatus.Present || model.AttendanceStatusKey == DbConstants.EmployeeAttendanceStatus.Absent || model.AttendanceStatusKey == DbConstants.EmployeeAttendanceStatus.Halfday)
                        {

                            if (model.AttendancePresentStatusKey == DbConstants.AttendancePresentStatus.CheckIn)
                            {
                                EmployeeAttendanceLogModel.InTime = model.InTime;
                            }
                            else
                            {
                                EmployeeAttendanceLogModel.InTime = model.OutTime;
                            }
                        }
                        else
                        {
                            EmployeeAttendanceLogModel.InTime = null;
                        }
                        dbContext.AttachToContext(EmployeeAttendanceLogModel, ++ComittedCount);
                    }
                }
            }

        }
        public EmployeeAttendanceViewModel UpdateAttendanceModelFromDevice(List<EmployeeAttendanceViewModel> modelList)
        {
            EmployeeAttendanceViewModel employeeAttendanceModel = new EmployeeAttendanceViewModel();

            using (var transaction = dbContext.Database.BeginTransaction())
            {
                try
                {

                    foreach (EmployeeAttendanceViewModel model in modelList)
                    {
                        model.AttendanceStatusKey = DbConstants.EmployeeAttendanceStatus.Present;
                        Employee Employee = dbContext.Employees.Where(row => row.BiomatricID == model.BiomatricId).SingleOrDefault();
                        if (Employee != null)
                        {
                            model.EmployeeKey = Employee.RowKey;

                            AttendanceConfiguration attendanceConfigurationlist = dbContext.AttendanceConfigurations.Where(row => (row.BranchKey ?? Employee.BranchKey) == Employee.BranchKey).SingleOrDefault();

                            var ShiftKeyDetail = dbContext.fnGetEmployeeShiftByDate(model.InDateTime, model.EmployeeKey, Employee.BranchKey, Employee.DepartmentKey).FirstOrDefault();
                            model.AttendanceDate = ShiftKeyDetail.AttendanceDate;
                            model.ShiftInTime = ShiftKeyDetail.InDateTime;
                            model.ShiftOutTime = ShiftKeyDetail.OutDateTime;
                            model.RowKey = ShiftKeyDetail.RowKey;

                            var LastAttendance = dbContext.EmployeeAttendances.SingleOrDefault(row => row.RowKey == model.RowKey);


                            if (LastAttendance != null)
                            {
                                if (LastAttendance.AttendancePresentStatusKey == DbConstants.AttendancePresentStatus.CheckIn)
                                {
                                    model.OutDateTime = model.InDateTime;
                                    model.InDateTime = LastAttendance.InTime;
                                    model.AttendancePresentStatusKey = DbConstants.AttendancePresentStatus.CheckOut;
                                }
                                else if (LastAttendance.AttendancePresentStatusKey == DbConstants.AttendancePresentStatus.CheckOut)
                                {
                                    model.OutDateTime = model.InDateTime;
                                    model.InDateTime = LastAttendance.InTime;
                                    model.AttendancePresentStatusKey = DbConstants.AttendancePresentStatus.BreakIn;
                                    var LastAttendanceLog = dbContext.EmployeeAttendanceLogs.Where(row => row.AttendanceKey == model.RowKey).OrderByDescending(row => row.InTime).FirstOrDefault();
                                    if (LastAttendanceLog != null)
                                    {
                                        LastAttendanceLog.AttendancePresentStatusKey = DbConstants.AttendancePresentStatus.BreakOut;
                                        dbContext.SaveChanges();
                                    }

                                }
                                else if (LastAttendance.AttendancePresentStatusKey == DbConstants.AttendancePresentStatus.BreakOut)
                                {
                                    model.OutDateTime = model.InDateTime;
                                    model.InDateTime = LastAttendance.InTime;

                                    model.AttendancePresentStatusKey = DbConstants.AttendancePresentStatus.BreakIn;
                                }
                                else if (LastAttendance.AttendancePresentStatusKey == DbConstants.AttendancePresentStatus.BreakIn)
                                {
                                    model.OutDateTime = model.InDateTime;
                                    model.InDateTime = LastAttendance.InTime;

                                    model.AttendancePresentStatusKey = DbConstants.AttendancePresentStatus.CheckOut;
                                }
                            }
                            else
                            {
                                model.AttendanceDate = model.InDateTime.Value.Date;
                                model.AttendancePresentStatusKey = DbConstants.AttendancePresentStatus.CheckIn;
                            }
                            model.AttendanceMonthKey = (byte)model.AttendanceDate.Value.Month;
                            model.AttendanceYearKey = (short)model.AttendanceDate.Value.Year;
                            model.ShiftKey = ShiftKeyDetail.ShiftKey;



                            //CalculateOvertime(model);
                            //if (model.RowKey != 0)
                            //{
                            //    UpdateEmpoloyeeAttendance(model);
                            //}
                            //else
                            //{
                            //    CreateEmpoloyeeAttendance(model);
                            //}

                            //CreateEmployeeAttendanceLog(model, false);

                            //dbContext.SaveChanges();

                        }
                    }

                    string xml = StringExtension.ListToXmlString(modelList, cols);
                    List<EmployeeAttendance> AttendanceModel = dbContext.spAttendanceCalculation(xml).Select(row => new EmployeeAttendance
                    {
                        RowKey = row.RowKey ?? 0,
                        ShiftKey = row.ShiftKey,
                        EmployeeKey = row.EmployeeKey,
                        BranchKey = row.BranchKey,
                        AttendanceDate = row.AttendanceDate ?? DateTimeUTC.Now,
                        InTime = row.InDateTime,
                        OutTime = row.OutDateTime,
                        Remarks = row.Remarks,
                        AttendanceStatusKey = (short?)(row.AttendanceStatusKey ?? DbConstants.EmployeeAttendanceStatus.Absent),
                        AttendanceDayStatusKey = (short?)row.AttendanceDayStatusKey,
                        ApprovalStatusKey = (short)(row.ApprovalStatusKey),
                        Duration = row.TotalDuration,
                        TotalBreak = row.TotalBreak,
                        LateComing = row.LateComingMinutes,
                        EarlyGoing = row.EarlyGoingMinutes,
                        LateGoing = row.LateGoingMinutes,
                        EarlyComing = row.EarlyComingMinutes,
                        OverTime = ((row.OverTime ?? 0) > 0 && (row.OverTime ?? 0) >= (row.MinOvertime ?? 0) && (row.OverTime ?? 0) <= (row.MaxOvertime ?? (row.OverTime ?? 0))) ? row.OverTime : null,
                        MissedOutPunch = row.AttendanceConfigTypeKey != DbConstants.AttendanceConfigType.MarkPresent && (new[] { DbConstants.AttendancePresentStatus.CheckIn, DbConstants.AttendancePresentStatus.BreakIn }).Contains((byte)(row.AttendancePresentStatusKey)),
                        //AttendancePresentStatusKey = (byte?)(row.AttendancePresentStatusKey == null ? row.AttendancePresentStatusKey : DbConstants.AttendancePresentStatus.CheckIn),
                        AttendancePresentStatusKey = (byte?)row.AttendancePresentStatusKey,
                        AttendanceConfigTypeKey = row.AttendanceConfigTypeKey,
                        IsPresent = row.AttendanceStatusKey != DbConstants.EmployeeAttendanceStatus.Absent,
                        IsHalfDay = row.AttendanceStatusKey == DbConstants.EmployeeAttendanceStatus.Halfday,


                    }).ToList();

                    foreach (EmployeeAttendance model in AttendanceModel)
                    {

                        //CalculateOvertime(model);
                        if (model.RowKey != 0)
                        {
                            dbContext.AddToContext(model, ++ComittedCount);
                        }
                        else
                        {
                            dbContext.AttachToContext(model, ++ComittedCount);
                        }
                        CreateEmployeeAttendanceLog(model, false);

                    }


                    transaction.Commit();
                    employeeAttendanceModel.Message = EduSuiteUIResources.Success;
                    employeeAttendanceModel.IsSuccessful = true;
                    ActivityLog.CreateActivityLog(MenuConstants.EmployeeAttendance, ActionConstants.Edit, DbConstants.LogType.Info, modelList.Select(row => row.EmployeeKey).FirstOrDefault(), employeeAttendanceModel.Message);

                }
                catch (Exception ex)
                {

                    transaction.Rollback();

                    employeeAttendanceModel.Message = String.Format(EduSuiteUIResources.FailedToSaveErrorMessage, EduSuiteUIResources.EmployeeAttendance);
                    //employeeAttendance.ExceptionMessage = ex.GetBaseException().Message;
                    employeeAttendanceModel.IsSuccessful = false;
                    ActivityLog.CreateActivityLog(MenuConstants.EmployeeAttendance, ActionConstants.Edit, DbConstants.LogType.Error, modelList.Select(row => row.EmployeeKey).FirstOrDefault(), ex.GetBaseException().Message);

                }
            }
            return employeeAttendanceModel;
        }
        private void CalculateOvertime(EmployeeAttendanceViewModel model)
        {

            Employee Employee = dbContext.Employees.Where(row => row.RowKey == model.EmployeeKey).SingleOrDefault();
            if (Employee != null)
            {
                AttendanceCategory attendanceCategoryList = Employee.AttendanceCategory;

                if (model.RowKey == 0 && model.BiomatricId == null)
                {
                    model.RowKey = Employee.EmployeeAttendances.Where(row => row.AttendanceDate.Date == model.AttendanceDate.Value.Date).Select(row => row.RowKey).SingleOrDefault();
                }
                int day = ((int)Convert.ToDateTime(model.AttendanceDate).DayOfWeek + 6) % 7 + 1;
                int Week = CultureInfo.InvariantCulture.Calendar.GetWeekOfYear(Convert.ToDateTime(model.AttendanceDate), CalendarWeekRule.FirstDay, DayOfWeek.Sunday);

                if (model.AttendanceConfigType != DbConstants.AttendanceConfigType.MarkPresent)
                {
                    if (model.ShiftKey == null)
                    {
                        var ShiftKeyDetail = dbContext.fnGetEmployeeShiftByDate(model.InDateTime, model.EmployeeKey, Employee.BranchKey, Employee.DepartmentKey).FirstOrDefault();
                        model.AttendanceDate = ShiftKeyDetail.AttendanceDate;
                        model.ShiftInTime = ShiftKeyDetail.InDateTime;
                        model.ShiftOutTime = ShiftKeyDetail.OutDateTime;
                        model.ShiftKey = ShiftKeyDetail.ShiftKey;
                        model.RowKey = ShiftKeyDetail.RowKey;
                    }



                    if (model.AttendanceConfigType == 0)
                    {
                        model.AttendanceConfigType = Employee.AttendanceConfigTypeKey ?? 0;
                    }
                    if (model.AttendanceStatusKey != DbConstants.EmployeeAttendanceStatus.Absent)
                    {
                        DateTime NowDate = Convert.ToDateTime(model.AttendanceDate);
                        DateTime DateBeginTime = NowDate;
                        DateTime DateEndTime = NowDate;

                        //DateTime ShiftListPartialDayBeginTime = NowDate;
                        //DateTime ShiftListPartialDayEndTime = NowDate;

                        //bool Partialday = false;
                        //if (shiftList.PartialDay == true)
                        //{
                        //    if (day == shiftList.PartialDayKey)
                        //    {
                        //        Partialday = true;
                        //        if (((shiftList.PartialDayEndTime ?? TimeSpan.Zero) - (shiftList.PartialDayBeginTime ?? TimeSpan.Zero)).Ticks < 0)
                        //        {
                        //            ShiftListPartialDayBeginTime = ShiftListPartialDayBeginTime + (shiftList.PartialDayBeginTime ?? TimeSpan.Zero);
                        //            ShiftListPartialDayEndTime = ShiftListPartialDayEndTime.AddDays(1) + (shiftList.PartialDayEndTime ?? TimeSpan.Zero);
                        //        }
                        //        else
                        //        {
                        //            ShiftListPartialDayBeginTime = ShiftListPartialDayBeginTime + (shiftList.PartialDayBeginTime ?? TimeSpan.Zero);
                        //            ShiftListPartialDayEndTime = ShiftListPartialDayEndTime + (shiftList.PartialDayEndTime ?? TimeSpan.Zero);
                        //        }
                        //    }
                        //}


                        DateBeginTime = model.ShiftInTime ?? DateBeginTime.Add(TimeSpan.Zero);
                        DateEndTime = model.ShiftOutTime ?? DateBeginTime.Add(TimeSpan.Zero);
                        var ShiftDuration = Convert.ToInt32((DateEndTime - DateBeginTime).TotalMinutes);
                        if ((attendanceCategoryList.DeductBrakeHours ?? false) && model.ShiftKey != 0)
                        {
                            Shift shift = dbContext.Shifts.SingleOrDefault(row => row.RowKey == model.ShiftKey);
                            if (shift != null)
                            {
                                foreach (ShiftBreak shiftBreak in shift.ShiftBreaks)
                                {
                                    DateTime datetimeBreak = new DateTime().Date;
                                    int BreakHours = Convert.ToInt32((datetimeBreak.Add(shiftBreak.BreakEndTime ?? TimeSpan.Zero) - datetimeBreak.Add(shiftBreak.BreakBeginTime ?? TimeSpan.Zero)).TotalMinutes);
                                    ShiftDuration = ShiftDuration - BreakHours;
                                }
                            }

                        }
                        DateTime DateinTime = Convert.ToDateTime(model.InDateTime);

                        //if (Partialday == true)
                        //{
                        //    DateBeginTime = ShiftListPartialDayBeginTime;
                        //}
                        //else
                        //{
                        //    DateBeginTime = DateBeginTime.Add(shiftList.BeginTime);
                        //}
                        // var diff = DateBeginTime.Subtract(DateinTime);
                        // var hours = (DateinTime - DateBeginTime).TotalHours;

                        var LateByMinutes = (DateinTime - DateBeginTime).TotalMinutes;

                        if (LateByMinutes >= (attendanceCategoryList.LateComingGraceTime ?? 0))
                        {
                            model.LateBy = Convert.ToInt32(LateByMinutes);
                        }
                        if ((model.Duration ?? 0) >= ShiftDuration)
                        {
                            model.AttendanceStatusKey = DbConstants.EmployeeAttendanceStatus.Present;
                        }




                        DateTime DateOutTime = Convert.ToDateTime(model.OutDateTime ?? model.ShiftOutTime);

                        //if (Partialday == true)
                        //{
                        //    DateEndTime = ShiftListPartialDayEndTime;
                        //}
                        //else
                        //{
                        //    DateEndTime = DateEndTime.Add(shiftList.EndTime);
                        //}
                        //var EarlyMinutes = (DateEndTime - DateOutTime).TotalMinutes;
                        var EarlyMinutes = (DateEndTime - DateOutTime).TotalMinutes;
                        var OvertimeMinutes = (DateOutTime - DateEndTime).TotalMinutes;

                        if (EarlyMinutes >= (attendanceCategoryList.EarlyGoingGraceTime ?? 0))
                        {
                            model.EarlyBy = Convert.ToInt32(EarlyMinutes);
                        }

                        model.Duration = Convert.ToInt32((DateOutTime - DateinTime).TotalMinutes);
                        if (!(attendanceCategoryList.ConsiderFirstLastPunch ?? false))
                        {
                            int BreakMins = 0;
                            List<EmployeeAttendanceLog> BreakList = dbContext.EmployeeAttendanceLogs.Where(row => row.AttendanceKey == model.RowKey && (row.AttendancePresentStatusKey == DbConstants.AttendancePresentStatus.BreakIn || row.AttendancePresentStatusKey == DbConstants.AttendancePresentStatus.BreakOut)).ToList<EmployeeAttendanceLog>();
                            for (int i = 0; i < BreakList.Count; i++)
                            {
                                if (BreakList[i].AttendancePresentStatusKey == DbConstants.AttendancePresentStatus.BreakIn)
                                {
                                    BreakMins = BreakMins + (BreakList[i].InTime - BreakList[i - 1].InTime).Value.Minutes;
                                }
                            }
                            model.Duration = (model.Duration ?? 0) - BreakMins;
                        }
                        if (attendanceCategoryList.OverTimeFormulaKey == DbConstants.OverTimeFormulaType.OutPunchMinusShiftEndTime)
                        {
                            if (OvertimeMinutes > 0 && OvertimeMinutes >= (attendanceCategoryList.MinOvertime ?? 0))
                            {

                                model.OverTime = Convert.ToInt32(EarlyMinutes);

                            }

                        }
                        else if (attendanceCategoryList.OverTimeFormulaKey == DbConstants.OverTimeFormulaType.TotalDurationMinusShiftHours)
                        {
                            // if (model.Duration >= attendanceCategoryList.MinOvertime)
                            // {
                            int Overtime = (model.Duration ?? 0) - ShiftDuration;
                            if (Overtime > 0 && Overtime >= (attendanceCategoryList.MinOvertime ?? 0))
                            {
                                model.OverTime = Convert.ToInt32(Overtime);
                            }


                            // }
                        }
                        else if (attendanceCategoryList.OverTimeFormulaKey == DbConstants.OverTimeFormulaType.EarlyComingPlusLateGoing)
                        {
                            var LateGoingMinutes = (DateOutTime - DateEndTime).TotalMinutes;
                            var EarlyComingMinutes = (DateBeginTime - DateinTime).TotalMinutes;
                            var Overtime = LateGoingMinutes + EarlyComingMinutes;

                            if (Overtime > 0 && Overtime >= (attendanceCategoryList.MinOvertime ?? 0))
                            {
                                model.OverTime = Convert.ToInt32(Overtime);
                            }

                        }
                        else
                        {
                            model.OverTime = null;
                        }
                        if (model.AttendancePresentStatusKey == 0 || model.AttendancePresentStatusKey == null)
                        {
                            model.AttendancePresentStatusKey = DbConstants.AttendancePresentStatus.CheckOut;
                        }

                        if (model.AttendancePresentStatusKey == DbConstants.AttendancePresentStatus.CheckOut)
                        {
                            bool checkStatus = false;

                            if (attendanceCategoryList.MarkHalfdayForLaterGoing == true)
                            {
                                if ((model.LateBy ?? 0) > (attendanceCategoryList.HalfDayLateByMins ?? 0))
                                {
                                    model.AttendanceStatusKey = DbConstants.EmployeeAttendanceStatus.Halfday;
                                    model.IsHalfDay = true;
                                    checkStatus = true;
                                }
                            }
                            if (attendanceCategoryList.MarkHalfdayForEarlyGoing == true)
                            {
                                if ((model.EarlyBy ?? 0) > (attendanceCategoryList.HalfDayEarlyByMins ?? 0))
                                {
                                    model.AttendanceStatusKey = DbConstants.EmployeeAttendanceStatus.Halfday;
                                    model.IsHalfDay = true;
                                    checkStatus = true;
                                }

                            }
                            if (attendanceCategoryList.CalcuteHalfDay == true)
                            {
                                if ((model.Duration ?? 0) < (attendanceCategoryList.HalfDayMins ?? 0))
                                {
                                    model.AttendanceStatusKey = DbConstants.EmployeeAttendanceStatus.Halfday;
                                    model.IsHalfDay = true;
                                    checkStatus = true;
                                }

                            }
                            if (attendanceCategoryList.CalculateAbsent == true)
                            {
                                if ((model.Duration ?? 0) < (attendanceCategoryList.AbsentMins ?? 0))
                                {
                                    model.AttendanceStatusKey = DbConstants.EmployeeAttendanceStatus.Absent;
                                    checkStatus = true;
                                }
                            }
                        }
                    }
                }

                if ((Employee.AttendanceCategory.AttendanceCategoryWeekOffs.Where(x => x.WeekOffDayKey == day && x.WeekOffDayWeekKeys.Split(',').Select(int.Parse).ToList().Contains(Week))).Any())
                {
                    model.AttendanceStatusKey = DbConstants.EmployeeAttendanceStatus.WeeklyOff;
                }
                else if (dbContext.Holidays.Any(row => (row.BranchKey ?? model.BranchKey) == model.BranchKey && row.IsDayOff
                    && System.Data.Entity.DbFunctions.TruncateTime(row.HolidayFrom) <= System.Data.Entity.DbFunctions.TruncateTime(model.AttendanceDate)
                    && System.Data.Entity.DbFunctions.TruncateTime(row.HolidayTo) >= System.Data.Entity.DbFunctions.TruncateTime(model.AttendanceDate)
                    ))
                {
                    model.AttendanceStatusKey = DbConstants.EmployeeAttendanceStatus.Off;
                }
                else if (dbContext.Holidays.Any(row => (row.BranchKey ?? model.BranchKey) == model.BranchKey && !row.IsDayOff
                   && System.Data.Entity.DbFunctions.TruncateTime(row.HolidayFrom) <= System.Data.Entity.DbFunctions.TruncateTime(model.AttendanceDate)
                   && System.Data.Entity.DbFunctions.TruncateTime(row.HolidayTo) >= System.Data.Entity.DbFunctions.TruncateTime(model.AttendanceDate)
                   ))
                {
                    model.AttendanceStatusKey = DbConstants.EmployeeAttendanceStatus.Holyday;
                }
                else if (Employee.LeaveApplications.Any(row => row.EmployeeKey == model.EmployeeKey && row.LeaveStatusKey == DbConstants.ProcessStatus.Approved
                 && row.LeaveFrom.Date <= model.AttendanceDate.Value.Date
                 && row.LeaveTo.Date >= model.AttendanceDate.Value.Date
                 ))
                {
                    model.AttendanceStatusKey = DbConstants.EmployeeAttendanceStatus.Leave;
                }



            }

        }
        #region oldupdateFunctions

        //public EmployeeAttendanceViewModel CreateEmployeeAttendance(EmployeeAttendanceViewModel model)
        //{
        //    FillDropdownList(model);
        //    Employee Employee = dbContext.Employees.SingleOrDefault(row => row.RowKey == model.EmployeeKey);
        //    using (var transaction = dbContext.Database.BeginTransaction())
        //    {
        //        try
        //        {

        //            var AttendanceExistCheck = dbContext.EmployeeAttendances.Where(row => row.EmployeeKey == model.EmployeeKey && System.Data.Entity.DbFunctions.TruncateTime(row.AttendanceDate) == System.Data.Entity.DbFunctions.TruncateTime(model.AttendanceDate)).ToList();

        //            if (AttendanceExistCheck.Count != 0)
        //            {
        //                model.Message = EduSuiteUIResources.ErrorAttendanceAlreadyExists;
        //                model.IsSuccessful = false;
        //                return model;
        //            }

        //            EmployeeAttendance AttendanceModel = new EmployeeAttendance();


        //            long maxKey = dbContext.EmployeeAttendances.Select(p => p.RowKey).DefaultIfEmpty().Max();
        //            AttendanceModel.RowKey = Convert.ToInt64(maxKey + 1);
        //            AttendanceModel.Remarks = model.Remarks;
        //            AttendanceModel.BranchKey = Employee.BranchKey ?? 0;
        //            //AttendanceModel.DepartmentKey = employee.DepartmentKey;
        //            AttendanceModel.EmployeeKey = model.EmployeeKey;
        //            AttendanceModel.AttendanceDate = Convert.ToDateTime(model.AttendanceDate);
        //            AttendanceModel.InTime = model.InTime;
        //            AttendanceModel.OutTime = model.OutTime;
        //            var AutoApproval = dbContext.AttendanceConfigurations.Where(row => row.CompanyKey == Employee.Branch.CompanyKey).Select(row => row.AutoApproval).FirstOrDefault();
        //            AttendanceModel.ApprovalStatusKey = AutoApproval ? DbConstants.ProcessStatus.Approved : DbConstants.ProcessStatus.Pending;
        //            AttendanceModel.LeaveTypeKey = model.LeaveTypeKey;
        //            AttendanceModel.AttendanceStatusKey = model.AttendanceStatusKey;
        //            dbContext.EmployeeAttendances.Add(AttendanceModel);
        //            dbContext.SaveChanges();
        //            transaction.Commit();
        //            model.Message = EduSuiteUIResources.Success;
        //            model.IsSuccessful = true;
        //            // ActivityLog.CreateActivityLog(MenuConstants.EmployeeAttendance, ActionConstants.Add, DbConstants.LogType.Info, AttendanceModel.RowKey, model.Message);
        //        }
        //        catch (Exception ex)
        //        {
        //            transaction.Rollback();
        //            model.Message = EduSuiteUIResources.FailedToSaveEmployeeAttendance;
        //            model.IsSuccessful = false;
        //            //ActivityLog.CreateActivityLog(MenuConstants.EmployeeAttendance, ActionConstants.Add, DbConstants.LogType.Error, model.RowKey, ex.GetBaseException().Message);
        //        }
        //    }
        //    //}
        //    return model;
        //}

        //public EmployeeAttendanceViewModel UpdateEmployeeAttendance(EmployeeAttendanceViewModel model)
        //{
        //    Employee Employee = dbContext.Employees.SingleOrDefault(row => row.RowKey == model.EmployeeKey);
        //    using (var transaction = dbContext.Database.BeginTransaction())
        //    {
        //        try
        //        {
        //            EmployeeAttendance AttendanceModel = new EmployeeAttendance();
        //            AttendanceModel = dbContext.EmployeeAttendances.SingleOrDefault(row => row.RowKey == model.RowKey);
        //            AttendanceModel.EmployeeKey = model.EmployeeKey;
        //            AttendanceModel.BranchKey = Employee.BranchKey ?? 0;
        //            //AttendanceModel.DepartmentKey = employee.DepartmentKey;
        //            AttendanceModel.AttendanceDate = Convert.ToDateTime(model.AttendanceDate);
        //            AttendanceModel.InTime = model.InTime;
        //            AttendanceModel.OutTime = model.OutTime;
        //            AttendanceModel.Remarks = model.Remarks;
        //            AttendanceModel.AttendanceStatusKey = model.AttendanceStatusKey;
        //            AttendanceModel.LeaveTypeKey = model.LeaveTypeKey;
        //            dbContext.SaveChanges();
        //            transaction.Commit();
        //            model.Message = EduSuiteUIResources.Success;
        //            model.IsSuccessful = true;
        //            // ActivityLog.CreateActivityLog(MenuConstants.EmployeeAttendance, ActionConstants.Edit, DbConstants.LogType.Info, model.RowKey, model.Message);
        //        }
        //        catch (Exception ex)
        //        {
        //            transaction.Rollback();
        //            model.Message = EduSuiteUIResources.FailedToSaveEmployeeAttendance;
        //            model.IsSuccessful = false;
        //            //ActivityLog.CreateActivityLog(MenuConstants.EmployeeAttendance, ActionConstants.Edit, DbConstants.LogType.Error, model.RowKey, ex.GetBaseException().Message);
        //        }
        //    }
        //    return model;
        //}

        //public EmployeeAttendanceViewModel UpdateEmployeesAttendance(List<EmployeeAttendanceViewModel> modelList)
        //{
        //    EmployeeAttendanceViewModel employeeAttendanceModel = new EmployeeAttendanceViewModel();


        //    using (var transaction = dbContext.Database.BeginTransaction())
        //    {
        //        try
        //        {
        //            dbContext.Configuration.AutoDetectChangesEnabled = false;

        //            long maxKey = dbContext.EmployeeAttendances.Select(p => p.RowKey).DefaultIfEmpty().Max();

        //            long OvertimeKey = dbContext.EmployeeOvertimes.Select(p => p.RowKey).DefaultIfEmpty().Max();
        //            long EmployeeKey = 0;
        //            foreach (EmployeeAttendanceViewModel model in modelList)
        //            {

        //                EmployeeAttendance AttendanceModel = new EmployeeAttendance();
        //                Employee Employee = dbContext.Employees.FirstOrDefault(row => row.RowKey == model.EmployeeKey);
        //                AttendanceModel = dbContext.EmployeeAttendances.FirstOrDefault(row => row.EmployeeKey == model.EmployeeKey && row.AttendanceDate == model.AttendanceDate);

        //                short LeaveTypeKey = dbContext.LeaveTypes.Where(row => row.LeaveTypeShortName == model.AttendanceStatusCode).Select(row => row.RowKey).FirstOrDefault();
        //                short AttendanceStatusKey = dbContext.EmployeeAttendanceStatus.Where(row => row.AttendanceStatusCode == model.AttendanceStatusCode).Select(row => row.RowKey).FirstOrDefault();

        //                if (Employee != null)
        //                {
        //                    if (AttendanceModel == null)
        //                    {
        //                        AttendanceModel = new EmployeeAttendance();
        //                        AttendanceModel.RowKey = Convert.ToInt16(maxKey + 1);
        //                        AttendanceModel.Remarks = model.Remarks;
        //                        AttendanceModel.BranchKey = Employee.BranchKey ?? 0;
        //                        // AttendanceModel.DepartmentKey = employee.DepartmentKey;
        //                        AttendanceModel.EmployeeKey = Employee.RowKey;
        //                        var AutoApproval = dbContext.AttendanceConfigurations.Where(row => row.CompanyKey == Employee.Branch.CompanyKey).Select(row => row.AutoApproval).FirstOrDefault();
        //                        AttendanceModel.ApprovalStatusKey = AutoApproval ? DbConstants.ProcessStatus.Approved : DbConstants.ProcessStatus.Pending;
        //                        AttendanceModel.AttendanceDate = Convert.ToDateTime(model.AttendanceDate);
        //                        AttendanceModel.InTime = model.InTime;
        //                        AttendanceModel.OutTime = model.OutTime;

        //                        model.EmployeeKey = AttendanceModel.EmployeeKey;
        //                        model.AttendanceYearKey = Convert.ToInt16(AttendanceModel.AttendanceDate.Year);
        //                        model.AttendanceMonthKey = Convert.ToByte(AttendanceModel.AttendanceDate.Month);

        //                        if (AttendanceStatusKey != 0)
        //                            AttendanceModel.AttendanceStatusKey = AttendanceStatusKey;
        //                        else
        //                            AttendanceModel.AttendanceStatusKey = DbConstants.EmployeeAttendanceStatus.Leave;

        //                        if (LeaveTypeKey != 0)
        //                            AttendanceModel.LeaveTypeKey = LeaveTypeKey;
        //                        else
        //                            AttendanceModel.LeaveTypeKey = null;

        //                        dbContext.EmployeeAttendances.Add(AttendanceModel);
        //                        if (LeaveTypeKey != 0)
        //                        {
        //                            model.LeaveTypeKey = AttendanceModel.LeaveTypeKey ?? 0;
        //                            UpdateLeaveCarryForward(model);
        //                        }
        //                        maxKey++;
        //                    }
        //                    else
        //                    {
        //                        EmployeeAttendanceViewModel oldModel = new EmployeeAttendanceViewModel();
        //                        if (AttendanceModel.LeaveTypeKey != null)
        //                        {

        //                            oldModel.EmployeeKey = AttendanceModel.EmployeeKey;
        //                            oldModel.LeaveTypeKey = AttendanceModel.LeaveTypeKey ?? 0;
        //                            oldModel.AttendanceYearKey = Convert.ToInt16(AttendanceModel.AttendanceDate.Year);
        //                            oldModel.AttendanceMonthKey = Convert.ToByte(AttendanceModel.AttendanceDate.Month);

        //                        }
        //                        AttendanceModel.Remarks = model.Remarks;
        //                        AttendanceModel.BranchKey = Employee.BranchKey ?? 0;
        //                        //AttendanceModel.DepartmentKey = employee.DepartmentKey;
        //                        AttendanceModel.EmployeeKey = Employee.RowKey;
        //                        AttendanceModel.ApprovalStatusKey = DbConstants.ProcessStatus.Pending;
        //                        AttendanceModel.AttendanceDate = Convert.ToDateTime(model.AttendanceDate);
        //                        AttendanceModel.InTime = model.InTime;
        //                        AttendanceModel.OutTime = model.OutTime;

        //                        model.EmployeeKey = AttendanceModel.EmployeeKey;
        //                        model.AttendanceYearKey = Convert.ToInt16(AttendanceModel.AttendanceDate.Year);
        //                        model.AttendanceMonthKey = Convert.ToByte(AttendanceModel.AttendanceDate.Month);
        //                        if (AttendanceStatusKey != 0)
        //                            AttendanceModel.AttendanceStatusKey = AttendanceStatusKey;
        //                        else
        //                            AttendanceModel.AttendanceStatusKey = DbConstants.EmployeeAttendanceStatus.Leave;

        //                        if (LeaveTypeKey != 0)
        //                        {
        //                            AttendanceModel.LeaveTypeKey = model.LeaveTypeKey = LeaveTypeKey;
        //                        }
        //                        else
        //                        {
        //                            AttendanceModel.LeaveTypeKey = null;
        //                        }
        //                        if (oldModel != null && oldModel.LeaveTypeKey != model.LeaveTypeKey)
        //                        {
        //                            if (oldModel != null && oldModel.LeaveTypeKey != 0 && model.LeaveTypeKey != 0)
        //                            {
        //                                model.LeaveTypeKey = AttendanceModel.LeaveTypeKey ?? 0;

        //                                UpdateLeaveCarryForwardWhenUpdate(model, oldModel);
        //                            }
        //                            else if (model != null && model.LeaveTypeKey != 0)
        //                            {
        //                                model.LeaveTypeKey = AttendanceModel.LeaveTypeKey ?? 0;
        //                                UpdateLeaveCarryForward(model);

        //                            }
        //                            else if (oldModel != null && oldModel.LeaveTypeKey != 0)
        //                            {
        //                                UpdateLeaveCarryForwardWhenDelete(oldModel);
        //                            }
        //                        }
        //                    }

        //                }
        //                if (Employee != null && EmployeeKey != model.EmployeeKey)
        //                {
        //                    EmployeeOvertime employeeOvertime = new EmployeeOvertime();
        //                    employeeOvertime = dbContext.EmployeeOvertimes.SingleOrDefault(row => row.EmployeeKey == model.EmployeeKey && row.AttendanceMonth == model.AttendanceMonthKey && row.AttendanceYear == model.AttendanceYearKey);
        //                    if (employeeOvertime == null)
        //                    {
        //                        employeeOvertime = new EmployeeOvertime();
        //                        CreateOverTime(employeeOvertime, model, OvertimeKey);
        //                        OvertimeKey++;
        //                    }
        //                    else
        //                    {
        //                        UpdateOverTime(employeeOvertime, model);
        //                    }
        //                }
        //                EmployeeKey = model.EmployeeKey;

        //            }
        //            dbContext.ChangeTracker.DetectChanges();
        //            dbContext.SaveChanges();
        //            transaction.Commit();
        //            employeeAttendanceModel.Message = EduSuiteUIResources.Success;
        //            employeeAttendanceModel.IsSuccessful = true;
        //            //ActivityLog.CreateActivityLog(MenuConstants.EmployeeAttendance, ActionConstants.Edit, DbConstants.LogType.Info, employeeAttendanceModel.RowKey, employeeAttendanceModel.Message);

        //        }
        //        catch (Exception ex)
        //        {
        //            transaction.Rollback();
        //            employeeAttendanceModel.Message = EduSuiteUIResources.FailedToSaveEmployeeAttendance;
        //            employeeAttendanceModel.IsSuccessful = false;
        //            //ActivityLog.CreateActivityLog(MenuConstants.EmployeeAttendance, ActionConstants.Edit, DbConstants.LogType.Error, employeeAttendanceModel.RowKey, ex.GetBaseException().Message);
        //        }
        //    }
        //    return employeeAttendanceModel;
        //}
        #endregion oldupdateFunctions
        public EmployeeAttendanceViewModel DeleteEmployeeAttendance(EmployeeAttendanceViewModel model)
        {
            //using (cPOSEntities dbContext = new cPOSEntities())
            //{
            using (var transaction = dbContext.Database.BeginTransaction())
            {
                try
                {

                    EmployeeAttendance AttendanceModel = new EmployeeAttendance();
                    AttendanceModel = dbContext.EmployeeAttendances.SingleOrDefault(row => row.RowKey == model.RowKey);
                    List<EmployeeAttendanceLog> EmployeeAttendanceLogList = dbContext.EmployeeAttendanceLogs.Where(x => x.AttendanceKey == AttendanceModel.RowKey).ToList();

                    dbContext.EmployeeAttendanceLogs.RemoveRange(EmployeeAttendanceLogList);
                    dbContext.EmployeeAttendances.Remove(AttendanceModel);
                    dbContext.SaveChanges();
                    transaction.Commit();
                    model.Message = EduSuiteUIResources.Success;
                    model.IsSuccessful = true;
                    ActivityLog.CreateActivityLog(MenuConstants.EmployeeAttendance, ActionConstants.Delete, DbConstants.LogType.Info, model.RowKey, model.Message);

                }

                catch (Exception ex)
                {
                    transaction.Rollback();
                    model.Message = String.Format(EduSuiteUIResources.FailedToDeleteErrorMessage, EduSuiteUIResources.EmployeeAttendance);
                    model.IsSuccessful = false;
                    ActivityLog.CreateActivityLog(MenuConstants.EmployeeAttendance, ActionConstants.Delete, DbConstants.LogType.Error, model.RowKey, ex.GetBaseException().Message);
                }
            }
            //}
            return model;
        }

        //private void CreateOverTime(EmployeeOvertime employeeOvertime, EmployeeAttendanceViewModel model, long OvertimeKey)
        //{
        //    employeeOvertime.RowKey = Convert.ToInt64(OvertimeKey + 1);
        //    employeeOvertime.EmployeeKey = model.EmployeeKey;
        //    employeeOvertime.AttendanceMonth = model.AttendanceMonthKey;
        //    employeeOvertime.AttendanceYear = model.AttendanceYearKey;
        //    employeeOvertime.TotalOvertime = model.TotalOvertime;
        //    dbContext.EmployeeOvertimes.Add(employeeOvertime);

        //}
        //private void UpdateOverTime(EmployeeOvertime employeeOvertime, EmployeeAttendanceViewModel model)
        //{

        //    employeeOvertime.EmployeeKey = model.EmployeeKey;
        //    employeeOvertime.AttendanceMonth = model.AttendanceMonthKey;
        //    employeeOvertime.AttendanceYear = model.AttendanceYearKey;
        //    employeeOvertime.TotalOvertime = model.TotalOvertime;

        //}

        //private void UpdateLeaveCarryForward(EmployeeAttendanceViewModel model)
        //{
        //    Int64 maxCarryForwardKey = dbContext.LeaveCarryForwards.Select(p => p.RowKey).DefaultIfEmpty().Max();
        //    var leaveType = dbContext.VwLeaveTypeSelectActiveOnlies.SingleOrDefault(row => row.RowKey == model.LeaveTypeKey);

        //    LeaveCarryForward leaveCarryForward = dbContext.LeaveCarryForwards.FirstOrDefault(row => row.EmployeeKey == model.EmployeeKey
        //        && row.LeaveTypeKey == model.LeaveTypeKey && row.MonthKey == model.AttendanceMonthKey && row.YearKey == model.AttendanceYearKey);
        //    if (leaveCarryForward == null)
        //    {
        //        leaveCarryForward = new LeaveCarryForward();
        //        if (leaveType.LeaveBalanceCarryForward)
        //        {
        //            LeaveCarryForward prevLeaveCarryForward = dbContext.LeaveCarryForwards.FirstOrDefault(row => row.EmployeeKey == model.EmployeeKey
        //          && row.LeaveTypeKey == model.LeaveTypeKey && row.MonthKey == (model.AttendanceMonthKey - 1) && row.YearKey == model.AttendanceYearKey);
        //            if (prevLeaveCarryForward == null)
        //            {
        //                prevLeaveCarryForward = dbContext.LeaveCarryForwards.OrderByDescending(row => new { row.YearKey, row.MonthKey }).Where(row => row.EmployeeKey == model.EmployeeKey
        //                && row.LeaveTypeKey == model.LeaveTypeKey).FirstOrDefault();

        //            }
        //            if (prevLeaveCarryForward != null && leaveType.LeaveBalanceCarryForward && prevLeaveCarryForward.LeaveBalance > 0 && (leaveType.LeaveCountTypeKey == 1 || (leaveType.LeaveCountTypeKey == 2 && prevLeaveCarryForward.YearKey != model.AttendanceYearKey)))
        //            {

        //                leaveCarryForward.LeaveBalance = prevLeaveCarryForward.LeaveBalance + leaveType.LeaveCount - 1;
        //            }
        //            else
        //            {
        //                DateTime MinAttendanceDate = dbContext.EmployeeAttendances.Where(row => row.EmployeeKey == model.EmployeeKey).Select(row => row.AttendanceDate).DefaultIfEmpty().Min();
        //                if (MinAttendanceDate.Year != 1)
        //                {
        //                    int YearDifference = model.AttendanceYearKey - MinAttendanceDate.Year + 1;
        //                    int MonthDifference = ((model.AttendanceYearKey - MinAttendanceDate.Year) * 12) + model.AttendanceMonthKey - MinAttendanceDate.Month + 1;

        //                    int LeaveBalance = dbContext.LeaveTypes.Where(row => row.RowKey == model.LeaveTypeKey).Select(row => leaveType.LeaveCountTypeKey == 1 ? row.LeaveCount * MonthDifference : row.LeaveCount * YearDifference).FirstOrDefault();
        //                    leaveCarryForward.LeaveBalance = LeaveBalance - 1;
        //                }
        //                else
        //                {
        //                    leaveCarryForward.LeaveBalance = leaveType.LeaveCount - 1;
        //                }
        //            }
        //        }
        //        else
        //        {
        //            leaveCarryForward.LeaveBalance = leaveType.LeaveCount - 1;
        //        }
        //        leaveCarryForward.RowKey = Convert.ToInt64(maxCarryForwardKey + 1);
        //        leaveCarryForward.EmployeeKey = model.EmployeeKey;
        //        leaveCarryForward.MonthKey = model.AttendanceMonthKey;
        //        leaveCarryForward.YearKey = model.AttendanceYearKey;
        //        leaveCarryForward.LeaveTypeKey = model.LeaveTypeKey ?? 0;

        //        dbContext.LeaveCarryForwards.Add(leaveCarryForward);
        //        maxCarryForwardKey++;
        //    }
        //    else
        //    {
        //        leaveCarryForward.LeaveBalance = leaveCarryForward.LeaveBalance - 1;
        //    }
        //    dbContext.SaveChanges();
        //}

        //private void UpdateLeaveCarryForwardWhenUpdate(EmployeeAttendanceViewModel model, EmployeeAttendanceViewModel modelOld)
        //{
        //    var leaveType = dbContext.VwLeaveTypeSelectActiveOnlies.SingleOrDefault(row => row.RowKey == model.LeaveTypeKey);

        //    LeaveCarryForward leaveCarryForwardOld = dbContext.LeaveCarryForwards.FirstOrDefault(row => row.EmployeeKey == modelOld.EmployeeKey
        //        && row.LeaveTypeKey == modelOld.LeaveTypeKey && row.MonthKey == modelOld.AttendanceMonthKey && row.YearKey == modelOld.AttendanceYearKey);
        //    if (leaveCarryForwardOld != null)
        //    {
        //        leaveCarryForwardOld.LeaveBalance = leaveCarryForwardOld.LeaveBalance + 1;
        //        UpdateLeaveCarryForward(model);
        //        dbContext.SaveChanges();

        //    }


        //}

        //private void UpdateLeaveCarryForwardWhenDelete(EmployeeAttendanceViewModel model)
        //{
        //    LeaveCarryForward leaveCarryForward = dbContext.LeaveCarryForwards.FirstOrDefault(row => row.EmployeeKey == model.EmployeeKey
        //        && row.LeaveTypeKey == model.LeaveTypeKey && row.MonthKey == model.AttendanceMonthKey && row.YearKey == model.AttendanceYearKey);
        //    if (leaveCarryForward != null)
        //    {

        //        leaveCarryForward.LeaveBalance = leaveCarryForward.LeaveBalance + 1;
        //        dbContext.SaveChanges();
        //    }



        // }
        public EmployeeAttendanceViewModel GetBranches(EmployeeAttendanceViewModel model)
        {
            model.Branches = dbContext.vwBranchSelectActiveOnlies.OrderBy(row => row.DisplayOrder).Select(row => new SelectListModel
            {
                RowKey = row.RowKey,
                Text = row.BranchName
            }).ToList();
            return model;
        }
        public EmployeeAttendanceViewModel GetEmployeesByBranchId(EmployeeAttendanceViewModel model)
        {
            model.Employees = dbContext.Employees.Where(row => row.BranchKey == model.BranchKey).Select(row => new GroupSelectListModel
            {
                RowKey = row.RowKey,
                Text = row.FirstName + " " + (row.MiddleName ?? "") + " " + row.LastName + EduSuiteUIResources.BlankSpace + EduSuiteUIResources.OpenBracket + row.EmployeeCode + EduSuiteUIResources.CloseBracket,
                //GroupKey = row.DepartmentKey,
                GroupName = row.EmployeeCode
            }).OrderBy(row => row.Text).ToList();

            return model;
        }
        public List<GroupSelectListModel> FillAttendanceStatuses()
        {
            List<GroupSelectListModel> attendanceStatuses = new List<GroupSelectListModel>();
            var leaveTypes = dbContext.VwLeaveTypeSelectActiveOnlies.OrderBy(row => row.DisplayOrder).Select(row => new GroupSelectListModel
            {
                RowKey = row.RowKey,
                Text = row.LeaveTypeShortName,
                GroupName = row.LeaveTypeColor
            }).ToList();
            attendanceStatuses = dbContext.EmployeeAttendanceStatus.OrderBy(row => row.DisplayOrder).Select(row => new GroupSelectListModel
            {
                RowKey = row.RowKey,
                Text = row.AttendanceStatusCode,
                GroupName = row.AttendanceStatusColor
            }).ToList();
            if (leaveTypes.Count > 0)
            {
                attendanceStatuses = attendanceStatuses.Where(row => row.RowKey != DbConstants.EmployeeAttendanceStatus.Leave).ToList().Union(leaveTypes).ToList();
            }
            return attendanceStatuses;
        }
        private void FillAttendanceStatuses(EmployeeAttendanceViewModel model)
        {
            List<short> AttendenceStatusKeys = new List<short> { DbConstants.EmployeeAttendanceStatus.Present, DbConstants.EmployeeAttendanceStatus.Absent };
            if (model.AttendanceConfigType == DbConstants.AttendanceConfigType.MarkPresent)
            {
                AttendenceStatusKeys = new List<short> { DbConstants.EmployeeAttendanceStatus.Present, DbConstants.EmployeeAttendanceStatus.Absent, DbConstants.EmployeeAttendanceStatus.Halfday };
            }
            model.AttendanceStatuses = dbContext.EmployeeAttendanceStatus.Where(row => row.IsActive == true && AttendenceStatusKeys.Contains(row.RowKey)).OrderBy(row => row.DisplayOrder).Select(row => new GroupSelectListModel
            {
                RowKey = row.RowKey,
                Text = row.AttendanceStatusName
            }).ToList();
        }
        private void FillAttendanceStatusesByCode(EmployeeAttendanceViewModel model)
        {
            List<short> AttendenceStatusKeys = new List<short> { DbConstants.EmployeeAttendanceStatus.Present, DbConstants.EmployeeAttendanceStatus.Absent };
            if (model.AttendanceConfigType == DbConstants.AttendanceConfigType.MarkPresent)
            {
                AttendenceStatusKeys = new List<short> { DbConstants.EmployeeAttendanceStatus.Present, DbConstants.EmployeeAttendanceStatus.Absent, DbConstants.EmployeeAttendanceStatus.Halfday };
            }
            model.AttendanceStatuses = dbContext.EmployeeAttendanceStatus.Where(row => row.IsActive == true && AttendenceStatusKeys.Contains(row.RowKey)).OrderBy(row => row.DisplayOrder).Select(row => new GroupSelectListModel
            {
                RowKey = row.RowKey,
                Text = row.AttendanceStatusCode,
                GroupName = row.AttendanceStatusColor
            }).ToList();
        }
        private void FillLeaveTypes(EmployeeAttendanceViewModel model)
        {
            model.LeaveTypes = dbContext.VwLeaveTypeSelectActiveOnlies.OrderBy(row => row.DisplayOrder).Select(row => new SelectListModel
            {
                RowKey = row.RowKey,
                Text = row.LeaveTypeName
            }).ToList();
        }
        public void FillDropdownList(EmployeeAttendanceViewModel model)
        {
            GetBranches(model);
            GetEmployeesByBranchId(model);
            FillAttendanceStatuses(model);
            FillLeaveTypes(model);
        }
        public void FillMultipleDropdownList(List<EmployeeAttendanceViewModel> modelList)
        {

            foreach (EmployeeAttendanceViewModel model in modelList)
            {
                model.AttendanceConfigType = GetAttendanceConfigTypeForAttendanceSheet(model);
                FillAttendanceStatusesByCode(model);
                GetEmployeesByBranchId(model);
            }

        }
        public List<EmployeeAttendanceViewModel> GetMultipleEmployeeAttendance(EmployeeAttendanceViewModel model, bool IsQuick)
        {
            DateTime DateNow = Convert.ToDateTime(model.AttendanceDate != null ? model.AttendanceDate : DateTimeUTC.Now.Date);
            DayOfWeek Dayweek = DateNow.DayOfWeek;
            //string DayName = Dayweek.ToString();
            //int id = (int)date.DayOfWeek;
            int day = ((int)DateNow.DayOfWeek + 6) % 7 + 1;
            int Week = StringExtension.GetWeekNumberOfMonth(DateNow);

            AttendanceConfiguration attendanceConfigurationlist = dbContext.AttendanceConfigurations.Where(row => (row.BranchKey ?? model.BranchKey) == model.BranchKey).SingleOrDefault();

            //Shift shiftList = dbContext.Shifts
            //    //.Where(x => x.RowKey == ShiftKeyDetail.ShiftKey)
            //    .FirstOrDefault();
            //DateTime ShiftListBeginTime = DateNow;
            //DateTime ShiftListEndTime = DateNow;
            //DateTime ShiftListPartialDayBeginTime = DateNow;
            //DateTime ShiftListPartialDayEndTime = DateNow;

            //ShiftListBeginTime = ShiftListBeginTime + shiftList.BeginTime;
            //ShiftListEndTime = ShiftListEndTime + shiftList.EndTime;

            //bool partialday = false;
            //if (shiftList.PartialDay == true)
            //{
            //    if (day == shiftList.PartialDayKey)
            //    {
            //        partialday = true;
            //        //ShiftListPartialDayBeginTime.AddTicks((shiftList.PartialDayBeginTime ?? TimeSpan.Zero).Ticks);
            //        //ShiftListPartialDayEndTime.AddTicks((shiftList.PartialDayEndTime ?? TimeSpan.Zero).Ticks);
            //        ShiftListPartialDayBeginTime = ShiftListPartialDayBeginTime + (shiftList.PartialDayBeginTime ?? TimeSpan.Zero);
            //        ShiftListPartialDayEndTime = ShiftListPartialDayEndTime + (shiftList.PartialDayEndTime ?? TimeSpan.Zero);
            //    }
            //}

            Holiday Holidays = new Holiday();

            Holidays = dbContext.Holidays.SingleOrDefault(x => x.BranchKey == model.BranchKey && !x.IsDayOff && (x.HolidayFrom <= DateNow && x.HolidayTo >= DateNow));

            Holiday Offdays = null;
            if (Holidays == null)
            {
                Offdays = dbContext.Holidays.SingleOrDefault(x => x.BranchKey == model.BranchKey && x.IsDayOff && (x.HolidayFrom <= DateNow && x.HolidayTo >= DateNow));
            }
            // bool WeeklyOffDay = false;

            //AttendanceCategory attendanceCategoryList = dbContext.AttendanceCategories
            //    //.Where(x => x.RowKey == attendanceConfigurationlist.AttendanceCatagoryKey)
            //    .SingleOrDefault();
            //if (attendanceCategoryList.WeekOffDay1Key != null || attendanceCategoryList.WeekOffDay2Key != null)
            //{
            //    if (day == attendanceCategoryList.WeekOffDay1Key)
            //    {
            //        WeeklyOffDay = true;
            //    }
            //    if (day == attendanceCategoryList.WeekOffDay2Key)
            //    {
            //        WeeklyOffDay = true;
            //    }

            //}
            try
            {
                IQueryable<EmployeeAttendanceViewModel> employeeAttendanceList = (from ea in dbContext.EmployeeAttendances.Where(row => System.Data.Entity.DbFunctions.TruncateTime(row.AttendanceDate) == System.Data.Entity.DbFunctions.TruncateTime(DateNow))

                                                                                  select new EmployeeAttendanceViewModel
                                                                                  {
                                                                                      RowKey = ea.RowKey,
                                                                                      EmployeeKey = ea.EmployeeKey,
                                                                                      BranchKey = ea.Employee.BranchKey,
                                                                                      //DepartmentKey = e.DepartmentKey,
                                                                                      EmployeeName = ea.Employee.FirstName + " " + (ea.Employee.MiddleName != null ? ea.Employee.MiddleName : "") + " " + ea.Employee.LastName + EduSuiteUIResources.BlankSpace + EduSuiteUIResources.OpenBracket + ea.Employee.EmployeeCode + EduSuiteUIResources.CloseBracket,
                                                                                      //BranchName = a.Branch.BranchName,
                                                                                      // DepartmentName = e.Department.DepartmentName,
                                                                                      AttendanceDate = ea.AttendanceDate,
                                                                                      AttendanceConfigType = ea.AttendanceConfigTypeKey ?? 0,
                                                                                      InDateTime = ea.InTime,
                                                                                      OutDateTime = !(ea.MissedOutPunch ?? false) ? ea.OutTime : null,
                                                                                      ClockInStatus = (ea.MissedOutPunch ?? false),
                                                                                      Remarks = ea.Remarks,
                                                                                      AttendanceStatusKey = (ea.IsHalfDay ?? false) && ea.AttendanceConfigTypeKey == DbConstants.AttendanceConfigType.MarkPresent ? DbConstants.EmployeeAttendanceStatus.Halfday : (ea.IsPresent ? DbConstants.EmployeeAttendanceStatus.Present : DbConstants.EmployeeAttendanceStatus.Absent),
                                                                                      LeaveTypeKey = ea.LeaveTypeKey,
                                                                                      AttendanceStatusName = ea.LeaveTypeKey != null ? ea.LeaveType.LeaveTypeName : ea.EmployeeAttendanceStatu1.AttendanceStatusName,
                                                                                      AttendanceStatusColor = ea.LeaveTypeKey != null ? ea.LeaveType.LeaveTypeColor : ea.EmployeeAttendanceStatu1.AttendanceStatusColor,
                                                                                      AttendancePresentStatusKey = ea.AttendancePresentStatusKey ?? 0,
                                                                                      AttendanceStatusRemarks = ea.EmployeeAttendanceStatu1.AttendanceStatusName,

                                                                                  });
                if (model.BranchKey != 0)
                {
                    employeeAttendanceList = employeeAttendanceList.Where(row => row.BranchKey == model.BranchKey);
                }
                if (model.EmployeeKey != 0)
                {
                    employeeAttendanceList = employeeAttendanceList.Where(row => row.EmployeeKey == model.EmployeeKey);
                }
                var employeeAttendances = employeeAttendanceList.GroupBy(x => x.EmployeeKey).Select(y => y.FirstOrDefault()).ToList<EmployeeAttendanceViewModel>();
                List<long> EmployeeKeys = employeeAttendances.Select(row => row.EmployeeKey).ToList();

                var employeeList = dbContext.Employees.Where(row => !EmployeeKeys.Contains(row.RowKey));
                if (model.BranchKey != 0)
                {
                    employeeList = employeeList.Where(row => row.BranchKey == model.BranchKey);
                }
                if (model.EmployeeKey != 0)
                {
                    employeeList = employeeList.Where(row => row.RowKey == model.EmployeeKey);
                }
                foreach (var a in employeeList.ToList())
                {
                    fnGetEmployeeShiftByDate_Result es = new fnGetEmployeeShiftByDate_Result();
                    if (!IsQuick || attendanceConfigurationlist.AttendanceConfigTypeKey == DbConstants.AttendanceConfigType.MarkPresent)
                    {
                        es = dbContext.fnGetEmployeeShiftByDate(DateNow, a.RowKey, a.BranchKey, a.DepartmentKey).FirstOrDefault();
                    }
                    bool IsWeekOff = a.AttendanceCategory != null ? (a.AttendanceCategory.AttendanceCategoryWeekOffs.Where(x => x.WeekOffDayKey == day && (x.WeekOffDayWeekKeys != null ? x.WeekOffDayWeekKeys.Split(',').Select(int.Parse).ToList().Contains(Week) : true))).Any() : false;
                    employeeAttendances.Add(new EmployeeAttendanceViewModel
                    {
                        EmployeeKey = a.RowKey,
                        BranchKey = a.BranchKey,
                        //DepartmentKey = e.DepartmentKey,
                        EmployeeName = a.FirstName + " " + (a.MiddleName != null ? a.MiddleName : "") + " " + a.LastName + EduSuiteUIResources.BlankSpace + EduSuiteUIResources.OpenBracket + a.EmployeeCode + EduSuiteUIResources.CloseBracket,
                        //BranchName = a.Branch.BranchName,
                        // DepartmentName = e.Department.DepartmentName,
                        AttendanceDate = DateNow,
                        AttendanceConfigType = a.AttendanceConfigTypeKey ?? 0,
                        InDateTime = es != null ? es.InDateTime : null,
                        OutDateTime = es != null ? es.OutDateTime : null,
                        ClockInStatus = false,

                        AttendanceStatusKey = a.LeaveApplications != null ? (a.LeaveApplications.Any(x => (x.LeaveFrom <= DateNow && x.LeaveTo >= DateNow) && x.LeaveStatusKey == DbConstants.ProcessStatus.Approved) == true ? DbConstants.EmployeeAttendanceStatus.Leave : IsWeekOff ? DbConstants.EmployeeAttendanceStatus.WeeklyOff : Offdays != null ? DbConstants.EmployeeAttendanceStatus.Off : (Holidays != null ? DbConstants.EmployeeAttendanceStatus.Holyday : DbConstants.EmployeeAttendanceStatus.Absent)) : DbConstants.EmployeeAttendanceStatus.Absent,
                        LeaveTypeKey = a.LeaveApplications != null ? (a.LeaveApplications.Where(x => (x.LeaveFrom <= DateNow && x.LeaveTo >= DateNow) && x.LeaveStatusKey == DbConstants.ProcessStatus.Approved).Select(row => row.LeaveTypeKey).FirstOrDefault()) : Int16.MinValue,
                        AttendanceStatusRemarks = (IsWeekOff ? EduSuiteUIResources.WeeklyOffDay : Offdays != null ? EduSuiteUIResources.Off : Holidays != null ? Holidays.HolidayTitle : ""),

                    });



                }


                FillMultipleDropdownList(employeeAttendances);
                return employeeAttendances;
            }
            catch (Exception ex)
            {
                ActivityLog.CreateActivityLog(MenuConstants.EmployeeAttendance, ActionConstants.MenuAccess, DbConstants.LogType.Error, DbConstants.User.UserKey, ex.GetBaseException().Message);
                ////model.ExceptionMessage = ex.GetBaseException().Message;
                return new List<EmployeeAttendanceViewModel>();

                //ActivityLog.CreateActivityLog(MenuConstants.EmployeeAttendance, ActionConstants.View, DbConstants.LogType.Error, model.RowKey, ex.GetBaseException().Message);
            }


        }
        public List<EmployeeAttendanceViewModel> GetEmployeesAttendanceLog(long RowKey)
        {

            return (from ea in dbContext.EmployeeAttendanceLogs.Where(x => x.AttendanceKey == RowKey)
                    select new EmployeeAttendanceViewModel
                    {
                        RowKey = ea.RowKey,
                        EmployeeKey = ea.EmployeeAttendance.EmployeeKey,

                        EmployeeName = ea.EmployeeAttendance.Employee.FirstName + " " + (ea.EmployeeAttendance.Employee.MiddleName != null ? ea.EmployeeAttendance.Employee.MiddleName : "") + " " + ea.EmployeeAttendance.Employee.LastName + EduSuiteUIResources.BlankSpace + EduSuiteUIResources.OpenBracket + ea.EmployeeAttendance.Employee.EmployeeCode + EduSuiteUIResources.CloseBracket,
                        BranchName = ea.EmployeeAttendance.Employee.Branch.BranchName,
                        // DepartmentName = e.Department.DepartmentName,
                        AttendanceDate = ea.AttendanceDate,
                        InDateTime = ea.InTime,
                        AttendanceStatusKey = ea.AttendanceStatusKey ?? 0,
                        AttendanceStatusName = ea.EmployeeAttendanceStatu.AttendanceStatusName,
                        AttendanceStatusColor = ea.EmployeeAttendanceStatu.AttendanceStatusColor,
                        AttendancePresentStatusKey = ea.AttendancePresentStatusKey ?? 0,
                        AttendancePresentStatusName = ea.AttendencePresentStatu.AttendancePrecentStatusName
                    }).ToList();
        }
        public byte GetAttendanceConfigTypeForQuickAttendance(EmployeeAttendanceViewModel model)
        {
            Company company = dbContext.Companies.FirstOrDefault();
            return model.AttendanceConfigType = dbContext.AttendanceConfigurations
                //.Where(row => row.CompanyKey == company.RowKey)
                .Select(x => x.AttendanceConfigTypeKey)
                .FirstOrDefault();

        }
        public byte GetAttendanceConfigTypeForAttendanceSheet(EmployeeAttendanceViewModel model)
        {

            return model.AttendanceConfigType = dbContext.AttendanceConfigurations.Where(row => row.BranchKey == model.BranchKey)
                .Select(x => x.AttendanceConfigTypeKey)
                .FirstOrDefault();

        }
        public EmployeeAttendanceViewModel DeleteBulkEmployeeAttendance(EmployeeAttendanceViewModel model, List<long> RowKeys)
        {
            using (var transaction = dbContext.Database.BeginTransaction())
            {
                try
                {
                    List<EmployeeAttendanceLog> attendanceDetailslist = dbContext.EmployeeAttendanceLogs.Where(x => RowKeys.Contains(x.AttendanceKey)).ToList();
                    List<EmployeeAttendance> attendancelist = dbContext.EmployeeAttendances.Where(x => RowKeys.Contains(x.RowKey)).ToList();

                    if (attendanceDetailslist.Count > 0)
                    {
                        dbContext.EmployeeAttendanceLogs.RemoveRange(attendanceDetailslist);
                    }
                    if (attendancelist.Count > 0)
                    {
                        dbContext.EmployeeAttendances.RemoveRange(attendancelist);
                    }
                    dbContext.SaveChanges();
                    transaction.Commit();
                    model.Message = EduSuiteUIResources.Success;
                    model.IsSuccessful = true;
                    ActivityLog.CreateActivityLog(MenuConstants.EmployeeAttendance, ActionConstants.Delete, DbConstants.LogType.Info, null, model.Message);
                }
                catch (DbUpdateException ex)
                {
                    transaction.Rollback();
                    if (ex.GetBaseException().Message.ToUpper().Contains(EduSuiteUIResources.ForeignKeyError.ToUpper()))
                    {
                        model.Message = String.Format(EduSuiteUIResources.CantDeleteErrorMessage, EduSuiteUIResources.EmployeeAttendance);
                        model.IsSuccessful = false;
                        ActivityLog.CreateActivityLog(MenuConstants.EmployeeAttendance, ActionConstants.Delete, DbConstants.LogType.Error, null, ex.GetBaseException().Message);

                    }
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    model.Message = String.Format(EduSuiteUIResources.FailedToDeleteErrorMessage, EduSuiteUIResources.EmployeeAttendance);
                    model.IsSuccessful = false;
                    ActivityLog.CreateActivityLog(MenuConstants.EmployeeAttendance, ActionConstants.Delete, DbConstants.LogType.Debug, null, ex.GetBaseException().Message);
                }
            }
            return model;
        }

    }
}
