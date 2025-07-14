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
    public class LeaveApplicationService : ILeaveApplicationService
    {

        private EduSuiteDatabase dbContext;
        public LeaveApplicationService(EduSuiteDatabase objDB)
        {
            this.dbContext = objDB;
        }

        public List<LeaveApplicationViewModel> GetLeaveApplication(LeaveApplicationViewModel model)
        {
            try
            {
                var leaveApplicationList = (from l in dbContext.LeaveApplications
                                            select new LeaveApplicationViewModel
                                            {
                                                BranchKey = l.Employee.BranchKey,
                                                EmployeeKey = l.EmployeeKey,
                                                EmployeeName = l.Employee.FirstName + " " + (l.Employee.MiddleName != null ? l.Employee.MiddleName : "") + " " + l.Employee.LastName,
                                                RowKey = l.RowKey,
                                                LeaveDurationTypeName = l.LeaveDurationType.LeaveDurationTypeName,
                                                LeaveFrom = l.LeaveFrom,
                                                LeaveTo = l.LeaveTo,
                                                LeaveTypeName = l.LeaveType.LeaveTypeName,
                                                LeaveReason = l.LeaveReason,
                                                LeaveStatusKey = l.LeaveStatusKey,
                                                LeaveStatusName = l.ProcessStatu.ProcessStatusName

                                            }).ToList();
                if (model.EmployeeKey != 0)
                {
                    leaveApplicationList = leaveApplicationList.Where(row => row.EmployeeKey == model.EmployeeKey).ToList();
                }
                if (model.BranchKey != 0)
                {
                    leaveApplicationList = leaveApplicationList.Where(row => row.BranchKey == model.BranchKey).ToList();
                }
                return leaveApplicationList.GroupBy(x => x.RowKey).Select(y => y.First()).ToList<LeaveApplicationViewModel>();
            }
            catch (Exception ex)
            {
                ActivityLog.CreateActivityLog(MenuConstants.LeaveApplication, ActionConstants.MenuAccess, DbConstants.LogType.Error, DbConstants.User.UserKey, ex.GetBaseException().Message);
                return new List<LeaveApplicationViewModel>();
            }
        }

        public LeaveApplicationViewModel GetLeaveApplicationById(LeaveApplicationViewModel model)
        {
            try
            {
                LeaveApplicationViewModel leaveApplicationViewModel = new LeaveApplicationViewModel();
                leaveApplicationViewModel = dbContext.LeaveApplications.Select(row => new LeaveApplicationViewModel
                {
                    RowKey = row.RowKey,
                    BranchKey = row.Employee.BranchKey,
                    EmployeeKey = row.EmployeeKey,
                    LeaveDurationTypeKey = row.LeaveDurationTypeKey,
                    LeaveDurationTypeName = row.LeaveDurationType.LeaveDurationTypeName,
                    LeaveFrom = row.LeaveFrom,
                    LeaveTo = row.LeaveTo,
                    LeaveTypeKey = row.LeaveTypeKey,
                    LeaveTypeName = row.LeaveType.LeaveTypeName,
                    LeaveReason = row.LeaveReason,
                    LeaveStatusName = row.ProcessStatu.ProcessStatusName,

                }).Where(x => x.RowKey == model.RowKey).FirstOrDefault();
                if (leaveApplicationViewModel == null)
                {
                    leaveApplicationViewModel = new LeaveApplicationViewModel();
                    leaveApplicationViewModel.EmployeeKey = model.EmployeeKey;
                    leaveApplicationViewModel.BranchKey = model.BranchKey;
                }
                FillDropdownList(leaveApplicationViewModel);
                return leaveApplicationViewModel;
            }
            catch (Exception ex)
            {
                ActivityLog.CreateActivityLog(MenuConstants.LeaveApplication, (model.RowKey != 0 ? ActionConstants.Edit : ActionConstants.Add), DbConstants.LogType.Error, model.RowKey, ex.GetBaseException().Message);
                return new LeaveApplicationViewModel();
            }
        }

        public LeaveApplicationViewModel CreateLeaveApplication(LeaveApplicationViewModel model)
        {
            LeaveApplication leaveApplicationModel = new LeaveApplication();

            FillDropdownList(model);

            using (var transaction = dbContext.Database.BeginTransaction())
            {
                try
                {

                    Int64 maxKey = dbContext.LeaveApplications.Select(p => p.RowKey).DefaultIfEmpty().Max();

                    leaveApplicationModel.RowKey = Convert.ToInt64(maxKey + 1);
                    leaveApplicationModel.LeaveDurationTypeKey = model.LeaveDurationTypeKey;
                    leaveApplicationModel.EmployeeKey = model.EmployeeKey;
                    leaveApplicationModel.LeaveFrom = Convert.ToDateTime(model.LeaveFrom);
                    leaveApplicationModel.LeaveTo = model.LeaveTo ?? Convert.ToDateTime(model.LeaveFrom);
                    leaveApplicationModel.LeaveTypeKey = model.LeaveTypeKey;
                    leaveApplicationModel.LeaveReason = model.LeaveReason;
                    leaveApplicationModel.LeaveStatusKey = DbConstants.ProcessStatus.Pending;
                    dbContext.LeaveApplications.Add(leaveApplicationModel);

                    dbContext.SaveChanges();
                    transaction.Commit();
                    model.Message = EduSuiteUIResources.Success;
                    model.IsSuccessful = true;
                    ActivityLog.CreateActivityLog(MenuConstants.LeaveApplication, ActionConstants.Add, DbConstants.LogType.Info, leaveApplicationModel.RowKey, model.Message);

                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    model.Message = String.Format(EduSuiteUIResources.FailedToSaveErrorMessage, EduSuiteUIResources.LeaveApplication);
                    model.IsSuccessful = false;
                    ActivityLog.CreateActivityLog(MenuConstants.LeaveApplication, ActionConstants.Add, DbConstants.LogType.Error, model.RowKey, ex.GetBaseException().Message);
                }
            }
            return model;
        }

        public LeaveApplicationViewModel UpdateLeaveApplication(LeaveApplicationViewModel model)
        {
            FillDropdownList(model);

            LeaveApplication leaveApplicationModel = new LeaveApplication();
            LeaveApplication leaveApplicationOldModel = new LeaveApplication();

            using (var transaction = dbContext.Database.BeginTransaction())
            {
                try
                {
                    leaveApplicationModel = dbContext.LeaveApplications.SingleOrDefault(row => row.RowKey == model.RowKey);
                    leaveApplicationOldModel = leaveApplicationModel;
                    leaveApplicationModel.LeaveDurationTypeKey = model.LeaveDurationTypeKey;
                    leaveApplicationModel.EmployeeKey = model.EmployeeKey;
                    leaveApplicationModel.LeaveFrom = Convert.ToDateTime(model.LeaveFrom);
                    leaveApplicationModel.LeaveTo = model.LeaveTo ?? Convert.ToDateTime(model.LeaveFrom);
                    leaveApplicationModel.LeaveTypeKey = model.LeaveTypeKey;
                    leaveApplicationModel.LeaveReason = model.LeaveReason;

                    dbContext.SaveChanges();
                    transaction.Commit();
                    model.Message = EduSuiteUIResources.Success;
                    model.IsSuccessful = true;
                    ActivityLog.CreateActivityLog(MenuConstants.LeaveApplication, ActionConstants.Edit, DbConstants.LogType.Info, model.RowKey, model.Message);

                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    model.Message = String.Format(EduSuiteUIResources.FailedToSaveErrorMessage, EduSuiteUIResources.LeaveApplication);
                    model.IsSuccessful = false;
                    ActivityLog.CreateActivityLog(MenuConstants.LeaveApplication, ActionConstants.Edit, DbConstants.LogType.Error, model.RowKey, ex.GetBaseException().Message);

                }
            }
            return model;
        }

        public LeaveApplicationViewModel DeleteLeaveApplication(LeaveApplicationViewModel model)
        {
            using (var transaction = dbContext.Database.BeginTransaction())
            {
                try
                {
                    LeaveApplication leaveApplication = dbContext.LeaveApplications.SingleOrDefault(row => row.RowKey == model.RowKey);
                    dbContext.LeaveApplications.Remove(leaveApplication);
                    dbContext.SaveChanges();
                    transaction.Commit();
                    model.Message = EduSuiteUIResources.Success;
                    model.IsSuccessful = true;
                    ActivityLog.CreateActivityLog(MenuConstants.LeaveApplication, ActionConstants.Delete, DbConstants.LogType.Info, model.RowKey, model.Message);
                }
                catch (DbUpdateException ex)
                {
                    transaction.Rollback();
                    if (ex.GetBaseException().Message.ToUpper().Contains(EduSuiteUIResources.ForeignKeyError.ToUpper()))
                    {
                        model.Message = String.Format(EduSuiteUIResources.CantDeleteErrorMessage, EduSuiteUIResources.LeaveApplication);
                        model.IsSuccessful = false;
                    }
                    ActivityLog.CreateActivityLog(MenuConstants.LeaveApplication, ActionConstants.Delete, DbConstants.LogType.Error, model.RowKey, ex.GetBaseException().Message);
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    model.Message = String.Format(EduSuiteUIResources.FailedToDeleteErrorMessage, EduSuiteUIResources.LeaveApplication);
                    model.IsSuccessful = false;
                    ActivityLog.CreateActivityLog(MenuConstants.LeaveApplication, ActionConstants.Delete, DbConstants.LogType.Error, model.RowKey, ex.GetBaseException().Message);
                }
            }
            return model;
        }


        public LeaveApplicationViewModel GetBranches(LeaveApplicationViewModel model)
        {

            model.Branches = dbContext.vwBranchSelectActiveOnlies.OrderBy(row => row.DisplayOrder).Select(row => new SelectListModel
            {
                RowKey = row.RowKey,
                Text = row.BranchName
            }).ToList();

            return model;
        }
        public LeaveApplicationViewModel GetEmployeesByBranchId(LeaveApplicationViewModel model)
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

        private void FillDropdownList(LeaveApplicationViewModel model)
        {
            GetBranches(model);
            GetEmployeesByBranchId(model);
            FillLeaveStatus(model);
            FillLeaveTypeName(model);
            FillLeaveDurationTypeName(model);
        }

        private void FillLeaveDurationTypeName(LeaveApplicationViewModel model)
        {
            model.LeaveDurationTypes = dbContext.LeaveDurationTypes.Where(x => x.IsActive).Select(row => new SelectListModel
            {
                RowKey = row.RowKey,
                Text = row.LeaveDurationTypeName
            }).ToList();
        }

        private void FillLeaveTypeName(LeaveApplicationViewModel model)
        {
            model.LeaveTypes = dbContext.VwLeaveTypeSelectActiveOnlies.OrderBy(row => row.DisplayOrder).Select(row => new SelectListModel
            {
                RowKey = row.RowKey,
                Text = row.LeaveTypeName,
            }).ToList();
        }

        private void FillLeaveStatus(LeaveApplicationViewModel model)
        {
            model.LeaveStatuses = dbContext.VwProcessStatusSelectActiveOnlies.OrderBy(row => row.DisplayOrder).Select(row => new SelectListModel
            {
                RowKey = row.RowKey,
                Text = row.ProcessStatusName,
            }).ToList();
        }
        public LeaveApplicationViewModel ApproveLeaveApplication(LeaveApplicationViewModel model)
        {
            using (var transaction = dbContext.Database.BeginTransaction())
            {
                try
                {
                    LeaveApplication leaveApplication = dbContext.LeaveApplications.SingleOrDefault(row => row.RowKey == model.RowKey);
                    if (model.LeaveStatusKey == DbConstants.ProcessStatus.Approved)
                    {
                        leaveApplication.LeaveStatusKey = DbConstants.ProcessStatus.Approved;
                        //UpdateLeaveCarryForward(leaveApplication);
                    }
                    else
                    {
                        leaveApplication.LeaveStatusKey = DbConstants.ProcessStatus.Rejected;

                    }
                    dbContext.SaveChanges();
                    transaction.Commit();
                    model.Message = EduSuiteUIResources.Success;
                    model.IsSuccessful = true;
                    ActivityLog.CreateActivityLog(MenuConstants.LeaveApplication, ActionConstants.Edit, DbConstants.LogType.Info, model.RowKey, model.Message);
                }

                catch (Exception ex)
                {
                    transaction.Rollback();
                    model.Message = String.Format(EduSuiteUIResources.FailedToSaveErrorMessage, EduSuiteUIResources.LeaveApplication);
                    model.IsSuccessful = false;
                    ActivityLog.CreateActivityLog(MenuConstants.LeaveApplication, ActionConstants.Edit, DbConstants.LogType.Error, model.RowKey, ex.GetBaseException().Message);
                }
            }
            return model;
        }

        //private void UpdateLeaveCarryForward(LeaveApplication model)
        //{
        //    Int64 maxCarryForwardKey = dbContext.LeaveCarryForwards.Select(p => p.RowKey).DefaultIfEmpty().Max();
        //    var leaveType = dbContext.VwLeaveTypeSelectActiveOnlies.SingleOrDefault(row => row.RowKey == model.LeaveTypeKey);
        //    var Dates = new List<DateTime>();

        //    var UsedCount = 0;
        //    for (var dt = Convert.ToDateTime(model.LeaveFrom); dt <= Convert.ToDateTime(model.LeaveTo); dt = dt.AddDays(1))
        //    {
        //        Dates.Add(dt);
        //    }
        //    var CountList = Dates.GroupBy(x => new { x.Month, x.Year }).Select(x => new
        //    {
        //        YearKey = x.Key.Year,
        //        MonthKey = x.Key.Month,
        //        Count = x.Count()
        //    }).ToList();
        //    foreach (var currDate in CountList)
        //    {
        //        var CurrMonth = new DateTime(currDate.YearKey, currDate.MonthKey, 1);
        //        var MonthKey = CurrMonth.Month;
        //        var YearKey = CurrMonth.Year;
        //        var PrevMonth = CurrMonth.AddMonths(-1);
        //        var PrevYear = CurrMonth.AddYears(-1);
        //        LeaveCarryForward leaveCarryForward = dbContext.LeaveCarryForwards.FirstOrDefault(row => row.EmployeeKey == model.EmployeeKey
        //            && row.LeaveTypeKey == model.LeaveTypeKey && row.MonthKey == MonthKey && row.YearKey == YearKey);
        //        if (leaveCarryForward == null)
        //        {
        //            leaveCarryForward = new LeaveCarryForward();
        //            if (leaveType.LeaveBalanceCarryForward)
        //            {
        //                LeaveCarryForward prevLeaveCarryForward = dbContext.LeaveCarryForwards.OrderByDescending(row => new { row.YearKey, row.MonthKey }).FirstOrDefault(row => row.EmployeeKey == model.EmployeeKey
        //              && row.LeaveTypeKey == model.LeaveTypeKey && (model.LeaveType.LeaveCountTypeKey == DbConstants.LeaveCountType.Monthly ? row.MonthKey == PrevMonth.Month : row.YearKey == PrevYear.Year));
        //                if (prevLeaveCarryForward == null)
        //                {
        //                    prevLeaveCarryForward = dbContext.LeaveCarryForwards.OrderByDescending(row => new { row.YearKey, row.MonthKey }).Where(row => row.EmployeeKey == model.EmployeeKey
        //                    && row.LeaveTypeKey == model.LeaveTypeKey).FirstOrDefault();

        //                }
        //                if (prevLeaveCarryForward != null && prevLeaveCarryForward.LeaveBalance > 0 && (leaveType.LeaveCountTypeKey == 1 || (leaveType.LeaveCountTypeKey == 2 && prevLeaveCarryForward.YearKey != YearKey)))
        //                {

        //                    leaveCarryForward.LeaveBalance = prevLeaveCarryForward.LeaveBalance + leaveType.LeaveCount - 1;
        //                }
        //                else
        //                {
        //                    //DateTime MinAttendanceDate = dbContext.LeaveApplications.Where(row => row.EmployeeKey == model.EmployeeKey).Select(row => row.LeaveFrom).DefaultIfEmpty().Min();
        //                    //if (MinAttendanceDate.Year != 1)
        //                    //{
        //                    //    int YearDifference = YearKey - MinAttendanceDate.Year + 1;
        //                    //    int MonthDifference = ((YearKey - MinAttendanceDate.Year) * 12) + MonthKey - MinAttendanceDate.Month + 1;

        //                    //    int LeaveBalance = dbContext.LeaveTypes.Where(row => row.RowKey == model.LeaveTypeKey).Select(row => leaveType.LeaveCountTypeKey == 1 ? row.LeaveCount * MonthDifference : row.LeaveCount * YearDifference).FirstOrDefault();
        //                    //    leaveCarryForward.LeaveBalance = LeaveBalance - 1;
        //                    //}
        //                    //else
        //                    //{
        //                    if (leaveType.LeaveCount > currDate.Count)
        //                    {
        //                        leaveCarryForward.LeaveBalance = leaveType.LeaveCount - currDate.Count;
        //                        UsedCount = currDate.Count;
        //                    }
        //                    else
        //                    {
        //                        leaveCarryForward.LeaveBalance = 0;
        //                    }
        //                    //}
        //                }
        //            }
        //            else
        //            {
        //                if (leaveCarryForward.LeaveBalance > currDate.Count)
        //                {
        //                    leaveCarryForward.LeaveBalance = leaveCarryForward.LeaveBalance - currDate.Count;
        //                    UsedCount = currDate.Count;
        //                }
        //                else
        //                {
        //                    leaveCarryForward.LeaveBalance = 0;
        //                }
        //            }
        //            leaveCarryForward.RowKey = Convert.ToInt64(maxCarryForwardKey + 1);
        //            leaveCarryForward.EmployeeKey = model.EmployeeKey;
        //            leaveCarryForward.MonthKey = Convert.ToByte(MonthKey);
        //            leaveCarryForward.YearKey = Convert.ToInt16(YearKey);
        //            leaveCarryForward.LeaveTypeKey = model.LeaveTypeKey;

        //            dbContext.LeaveCarryForwards.Add(leaveCarryForward);
        //            maxCarryForwardKey++;
        //        }
        //        else
        //        {
        //            leaveCarryForward.LeaveBalance = leaveCarryForward.LeaveBalance - 1;
        //        }
        //        model.LeaveUsedCount = UsedCount;
        //        dbContext.SaveChanges();
        //    }

        //}

        //private void UpdateLeaveCarryForwardWhenUpdate(LeaveApplication model, LeaveApplication modelOld)
        //{
        //    var leaveType = dbContext.VwLeaveTypeSelectActiveOnlies.SingleOrDefault(row => row.RowKey == model.LeaveTypeKey);
        //    var Dates = new List<DateTime>();


        //    for (var dt = Convert.ToDateTime(modelOld.LeaveFrom); dt <= Convert.ToDateTime(modelOld.LeaveTo); dt = dt.AddDays(1))
        //    {
        //        Dates.Add(dt);
        //    }
        //    var CountList = Dates.GroupBy(x => new { x.Month, x.Year }).Select(x => new
        //    {
        //        YearKey = x.Key.Year,
        //        MonthKey = x.Key.Month,
        //        Count = x.Count()
        //    }).ToList();
        //    foreach (var currDate in CountList)
        //    {
        //        var MonthKey = currDate.MonthKey;
        //        var YearKey = currDate.YearKey;
        //        LeaveCarryForward leaveCarryForwardOld = dbContext.LeaveCarryForwards.FirstOrDefault(row => row.EmployeeKey == modelOld.EmployeeKey
        //            && row.LeaveTypeKey == modelOld.LeaveTypeKey && row.MonthKey == MonthKey && row.YearKey == YearKey);
        //        if (leaveCarryForwardOld != null)
        //        {
        //            var Total = leaveCarryForwardOld.LeaveBalance + currDate.Count;
        //            leaveCarryForwardOld.LeaveBalance = leaveType.LeaveCount > Total ? Total : leaveType.LeaveCount;

        //            dbContext.SaveChanges();

        //        }
        //    }

        //    UpdateLeaveCarryForward(model);
        //}

        //private void UpdateLeaveCarryForwardWhenDelete(LeaveApplication model)
        //{
        //    var Dates = new List<DateTime>();

        //    for (var dt = Convert.ToDateTime(model.LeaveFrom); dt <= Convert.ToDateTime(model.LeaveTo); dt = dt.AddDays(1))
        //    {
        //        Dates.Add(dt);
        //    }
        //    foreach (DateTime currDate in Dates)
        //    {
        //        var MonthKey = currDate.Month;
        //        var YearKey = currDate.Year;
        //        LeaveCarryForward leaveCarryForward = dbContext.LeaveCarryForwards.FirstOrDefault(row => row.EmployeeKey == model.EmployeeKey
        //            && row.LeaveTypeKey == model.LeaveTypeKey && row.MonthKey == MonthKey && row.YearKey == YearKey);
        //        if (leaveCarryForward != null)
        //        {

        //            leaveCarryForward.LeaveBalance = leaveCarryForward.LeaveBalance + 1;
        //            dbContext.SaveChanges();
        //        }
        //    }


        //}
    }
}
