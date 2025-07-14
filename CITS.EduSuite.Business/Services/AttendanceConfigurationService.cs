using System;
using System.Collections.Generic;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CITS.EduSuite.Business.Common;
using CITS.EduSuite.Business.Interfaces;
using CITS.EduSuite.Business.Models.Resources;
using CITS.EduSuite.Business.Models.ViewModels;
using CITS.EduSuite.Data;

namespace CITS.EduSuite.Business.Services
{
    public class AttendanceConfigurationService : IAttendanceConfigurationService
    {
        private EduSuiteDatabase dbContext;

        public AttendanceConfigurationService(EduSuiteDatabase objDB)
        {
            this.dbContext = objDB;
        }

        public List<AttendanceConfigurationViewModel> GetAttendanceConfigurations()
        {
            try
            {
                var attendanceConfigList = (from AC in dbContext.AttendanceConfigurations
                                            select new AttendanceConfigurationViewModel
                                            {
                                                RowKey = AC.RowKey,
                                                BranchName = AC.BranchKey != null ? AC.Branch.BranchName : "",
                                                AttendanceConfigTypeName = AC.AttendanceConfigType.AttendanceConfigTypeName,
                                                //ShiftName = AC.Shift.ShiftName,
                                                TotalWorkingHours = AC.TotalWorkingHours,
                                                OvertimeAdditionAmount = AC.OvertimeAdditionAmount,
                                                UnitTypeName = AC.UnitType.UnitTypeName,
                                                AutoApproval = AC.AutoApproval,
                                                BaseDaysPerMonth = AC.BaseDaysPerMonth

                                            }).ToList();

                return attendanceConfigList.GroupBy(x => x.RowKey).Select(y => y.First()).ToList<AttendanceConfigurationViewModel>();
            }
            catch (Exception ex)
            {
                ActivityLog.CreateActivityLog(MenuConstants.AttendanceConfiguration, ActionConstants.MenuAccess, DbConstants.LogType.Error, DbConstants.User.UserKey, ex.GetBaseException().Message);

                return new List<AttendanceConfigurationViewModel>();
            }
        }

        public AttendanceConfigurationViewModel GetAttendanceConfigurationById(AttendanceConfigurationViewModel model)
        {
            try
            {
                AttendanceConfigurationViewModel objViewModel = new AttendanceConfigurationViewModel();

                objViewModel = dbContext.AttendanceConfigurations.Where(x => x.RowKey == model.RowKey).Select(row => new AttendanceConfigurationViewModel
                {
                    RowKey = row.RowKey,
                    BranchKey = row.BranchKey,
                    AttendanceConfigTypeKey = row.AttendanceConfigTypeKey,
                    //ShiftKey = row.ShiftKey ?? 0,
                    //AttendanceCatagoryKey = row.AttendanceCatagoryKey ?? 0,
                    TotalWorkingHours = row.TotalWorkingHours,
                    OvertimeAdditionAmount = row.OvertimeAdditionAmount,
                    UnitTypeKey = row.UnitTypeKey,
                    AutoApproval = row.AutoApproval,
                    //ShiftAllocationTypeKey = row.ShiftAllocationTypeKey,
                    //BeginTime = row.BeginTime,
                    //EndTime = row.EndTime,
                    //PunchBeginDuration = row.PunchBeginDuration,
                    MinimmDifferencePunch = row.MinimmDifferencePunch,
                    BaseDaysPerMonth = row.BaseDaysPerMonth

                }).FirstOrDefault();
                if (objViewModel == null)
                {
                    objViewModel = new AttendanceConfigurationViewModel();
                }
                FillDropdownLists(objViewModel);
                return objViewModel;
            }
            catch (Exception ex)
            {
                ActivityLog.CreateActivityLog(MenuConstants.AttendanceConfiguration, (model.RowKey != 0 ? ActionConstants.Edit : ActionConstants.Add), DbConstants.LogType.Error, model.RowKey, ex.GetBaseException().Message);

                return new AttendanceConfigurationViewModel();
            }
        }

        public AttendanceConfigurationViewModel CreateAttendanceConfiguration(AttendanceConfigurationViewModel model)
        {
            FillDropdownLists(model);
            var AttendanceConfigCheck = dbContext.AttendanceConfigurations.Where(row => (row.BranchKey ?? model.BranchKey) == model.BranchKey)
                .ToList();
            AttendanceConfiguration attendanceConfigurationModel = new AttendanceConfiguration();

            if (AttendanceConfigCheck.Count != 0)
            {
                model.Message = String.Format(EduSuiteUIResources.ExistsErrorMessage, EduSuiteUIResources.AttendanceConfiguration);
                model.IsSuccessful = false;
                return model;
            }

            using (var transaction = dbContext.Database.BeginTransaction())
            {
                try
                {
                    Int32 maxKey = dbContext.AttendanceConfigurations.Select(p => p.RowKey).DefaultIfEmpty().Max();
                    attendanceConfigurationModel.RowKey = Convert.ToInt32(maxKey + 1);
                    // attendanceConfigurationModel.CompanyKey = model.CompanyKey;
                    attendanceConfigurationModel.BranchKey = model.BranchKey;
                    attendanceConfigurationModel.AttendanceConfigTypeKey = model.AttendanceConfigTypeKey;
                    // attendanceConfigurationModel.ShiftKey = model.ShiftKey;
                    // attendanceConfigurationModel.AttendanceCatagoryKey = model.AttendanceCatagoryKey;
                    attendanceConfigurationModel.TotalWorkingHours = Convert.ToDecimal(model.TotalWorkingHours);
                    attendanceConfigurationModel.OvertimeAdditionAmount = model.OvertimeAdditionAmount;
                    attendanceConfigurationModel.UnitTypeKey = model.UnitTypeKey;
                    attendanceConfigurationModel.AutoApproval = model.AutoApproval;
                    //attendanceConfigurationModel.ShiftAllocationTypeKey = model.ShiftAllocationTypeKey;
                    //attendanceConfigurationModel.BeginTime = model.BeginTime;
                    //attendanceConfigurationModel.EndTime = model.EndTime;
                    //attendanceConfigurationModel.PunchBeginDuration = model.PunchBeginDuration;
                    attendanceConfigurationModel.MinimmDifferencePunch = model.MinimmDifferencePunch;
                    attendanceConfigurationModel.BaseDaysPerMonth = model.BaseDaysPerMonth;

                    dbContext.AttendanceConfigurations.Add(attendanceConfigurationModel);
                    UpdateEmployeeConfigType(model);
                    dbContext.SaveChanges();
                    transaction.Commit();
                    model.Message = EduSuiteUIResources.Success;
                    model.IsSuccessful = true;
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    model.Message = String.Format(EduSuiteUIResources.FailedToSaveErrorMessage, EduSuiteUIResources.AttendanceConfiguration);
                    model.IsSuccessful = false;
                    ActivityLog.CreateActivityLog(MenuConstants.AttendanceConfiguration, ActionConstants.Add, DbConstants.LogType.Error, model.RowKey, ex.GetBaseException().Message);
                }
            }
            return model;
        }

        public AttendanceConfigurationViewModel UpdateAttendanceConfiguration(AttendanceConfigurationViewModel model)
        {
            FillDropdownLists(model);
            var AttendanceConfigCheck = dbContext.AttendanceConfigurations.Where(row => row.RowKey != model.RowKey && (row.BranchKey ?? model.CompanyKey) == model.CompanyKey)
                .ToList();
            AttendanceConfiguration attendanceConfigurationModel = new AttendanceConfiguration();

            if (AttendanceConfigCheck.Count != 0)
            {
                model.Message = String.Format(EduSuiteUIResources.ExistsErrorMessage, EduSuiteUIResources.AttendanceConfiguration);
                model.IsSuccessful = false;
                return model;
            }

            using (var transaction = dbContext.Database.BeginTransaction())
            {
                try
                {
                    attendanceConfigurationModel = dbContext.AttendanceConfigurations.SingleOrDefault(row => row.RowKey == model.RowKey);
                    attendanceConfigurationModel.BranchKey = model.BranchKey;
                    attendanceConfigurationModel.AttendanceConfigTypeKey = model.AttendanceConfigTypeKey;
                    // attendanceConfigurationModel.ShiftKey = model.ShiftKey;
                    // attendanceConfigurationModel.AttendanceCatagoryKey = model.AttendanceCatagoryKey;
                    attendanceConfigurationModel.TotalWorkingHours = Convert.ToDecimal(model.TotalWorkingHours);
                    attendanceConfigurationModel.OvertimeAdditionAmount = model.OvertimeAdditionAmount;
                    attendanceConfigurationModel.UnitTypeKey = model.UnitTypeKey;
                    attendanceConfigurationModel.AutoApproval = model.AutoApproval;
                    //attendanceConfigurationModel.ShiftAllocationTypeKey = model.ShiftAllocationTypeKey;
                    //attendanceConfigurationModel.BeginTime = model.BeginTime;
                    //attendanceConfigurationModel.EndTime = model.EndTime;
                    //attendanceConfigurationModel.PunchBeginDuration = model.PunchBeginDuration;
                    attendanceConfigurationModel.MinimmDifferencePunch = model.MinimmDifferencePunch;
                    attendanceConfigurationModel.BaseDaysPerMonth = model.BaseDaysPerMonth;
                    UpdateEmployeeConfigType(model);
                    dbContext.SaveChanges();
                    transaction.Commit();
                    model.Message = EduSuiteUIResources.Success;
                    model.IsSuccessful = true;
                    ActivityLog.CreateActivityLog(MenuConstants.AttendanceConfiguration, ActionConstants.Edit, DbConstants.LogType.Info, model.RowKey, model.Message);
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    model.Message = String.Format(EduSuiteUIResources.FailedToSaveErrorMessage, EduSuiteUIResources.AttendanceConfiguration);
                    model.IsSuccessful = false;
                    ActivityLog.CreateActivityLog(MenuConstants.AttendanceConfiguration, ActionConstants.Edit, DbConstants.LogType.Error, model.RowKey, ex.GetBaseException().Message);
                }
            }
            return model;
        }

        public AttendanceConfigurationViewModel DeleteAttendanceConfiguration(AttendanceConfigurationViewModel model)
        {
            using (var transaction = dbContext.Database.BeginTransaction())
            {
                try
                {
                    AttendanceConfiguration AttendanceConfigurationModel = dbContext.AttendanceConfigurations.SingleOrDefault(row => row.RowKey == model.RowKey);
                    dbContext.AttendanceConfigurations.Remove(AttendanceConfigurationModel);
                    dbContext.SaveChanges();
                    transaction.Commit();
                    model.Message = EduSuiteUIResources.Success;
                    model.IsSuccessful = true;
                    ActivityLog.CreateActivityLog(MenuConstants.AttendanceConfiguration, ActionConstants.Delete, DbConstants.LogType.Info, model.RowKey, model.Message);
                }
                catch (DbUpdateException ex)
                {
                    transaction.Rollback();
                    if (ex.GetBaseException().Message.ToUpper().Contains(EduSuiteUIResources.ForeignKeyError.ToUpper()))
                    {
                        model.Message = String.Format(EduSuiteUIResources.CantDeleteErrorMessage, EduSuiteUIResources.AttendanceConfiguration);
                        model.IsSuccessful = false;
                    }
                    ActivityLog.CreateActivityLog(MenuConstants.AttendanceConfiguration, ActionConstants.Delete, DbConstants.LogType.Error, model.RowKey, ex.GetBaseException().Message);
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    model.Message = String.Format(EduSuiteUIResources.FailedToDeleteErrorMessage, EduSuiteUIResources.AttendanceConfiguration);
                    model.IsSuccessful = false;
                    ActivityLog.CreateActivityLog(MenuConstants.AttendanceConfiguration, ActionConstants.Delete, DbConstants.LogType.Error, model.RowKey, ex.GetBaseException().Message);
                }
            }

            return model;
        }

        private void FillDropdownLists(AttendanceConfigurationViewModel model)
        {
            FillBranches(model);
            FillAttendanceConfigTypes(model);
            //FillShifts(model);
            FillUnitTypes(model);
            //FillShiftApplocationTypes(model);

        }

        private void FillCompanies(AttendanceConfigurationViewModel model)
        {
            model.Companies = dbContext.Companies.Select(row => new SelectListModel
            {

                RowKey = row.RowKey,
                Text = row.CompanyName
            }).ToList();

        }

        private void FillBranches(AttendanceConfigurationViewModel model)
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
                    //model.BranchKey = Employee.BranchKey;
                }
                else
                {
                    model.Branches = BranchQuery.Where(x => x.RowKey == Employee.BranchKey).ToList();
                    //model.BranchKey = Employee.BranchKey;
                }
            }
            else
            {
                model.Branches = BranchQuery.ToList();
            }

            if (model.Branches.Count == 1)
            {
                long? branchkey = model.Branches.Select(x => x.RowKey).FirstOrDefault();
                model.BranchKey = Convert.ToInt16(branchkey);
            }
        }

        private void FillAttendanceConfigTypes(AttendanceConfigurationViewModel model)
        {
            model.AttendanceConfigTypes = dbContext.AttendanceConfigTypes.Select(row => new SelectListModel
            {

                RowKey = row.RowKey,
                Text = row.AttendanceConfigTypeName
            }).ToList();

        }
        //private void FillShifts(AttendanceConfigurationViewModel model)
        //{
        //    model.Shifts = dbContext.Shifts.Select(row => new SelectListModel
        //    {

        //        RowKey = row.RowKey,
        //        Text = row.ShiftName
        //    }).ToList();

        //}
        private void FillUnitTypes(AttendanceConfigurationViewModel model)
        {
            model.UnitTypes = dbContext.UnitTypes.Select(row => new SelectListModel
            {

                RowKey = row.RowKey,
                Text = row.UnitTypeName
            }).ToList();

        }

        public void UpdateEmployeeConfigType(AttendanceConfigurationViewModel model)
        {
            List<Employee> EmployeeList = dbContext.Employees.Where(x => x.BranchKey == model.BranchKey).ToList();
            EmployeeList.ForEach(x => x.AttendanceConfigTypeKey = model.AttendanceConfigTypeKey);
        }


    }
}
