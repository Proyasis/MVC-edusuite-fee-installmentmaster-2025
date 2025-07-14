using System;
using System.Collections.Generic;
using System.Data.Entity.SqlServer;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CITS.EduSuite.Business.Common;
using CITS.EduSuite.Business.Interfaces;
using CITS.EduSuite.Business.Models.ViewModels;
using CITS.EduSuite.Data;
using System.Data.Entity.Core.Objects;
using System.Data.Common;
using CITS.EduSuite.Business.Extensions;
using CITS.EduSuite.Business.Models.Resources;

namespace CITS.EduSuite.Business.Services
{
    public class ApplicationScheduleService : IApplicationScheduleService
    {
        private EduSuiteDatabase dbContext;

        public ApplicationScheduleService(EduSuiteDatabase objDB)
        {
            this.dbContext = objDB;
        }

        //public List<ApplicationScheduleViewModel> GetApplicationSchedule(ApplicationScheduleViewModel model)
        //{
        //    try
        //    {
        //        var Take = model.PageSize;
        //        var skip = (model.PageIndex - 1) * model.PageSize;

        //        Employee Employee = null;
        //        Employee = dbContext.Employees.Where(x => x.AppUserKey == DbConstants.User.UserKey).SingleOrDefault();

        //        ObjectParameter TotalCount = new ObjectParameter("TotalCount", typeof(Int64));
        //        ObjectParameter TodayCount = new ObjectParameter("TodayCount", typeof(Int64));
        //        ObjectParameter TomorrowCount = new ObjectParameter("TomorrowCount", typeof(Int64));
        //        ObjectParameter PendingCount = new ObjectParameter("PendingCount", typeof(Int64));
        //        ObjectParameter UpcomingCount = new ObjectParameter("UpcomingCount", typeof(Int64));
        //        ObjectParameter HistoryCount = new ObjectParameter("HistoryCount", typeof(Int64));
        //        ObjectParameter TodayRescheduleCount = new ObjectParameter("TodayRescheduleCount", typeof(Int64));
        //        ObjectParameter CouncellingCount = new ObjectParameter("CouncellingCount", typeof(Int64));

        //        string Today = DateTimeUTC.Now.ToString("yyyy-MM-dd");
        //        string Tomorrow = DateTimeUTC.Tomorrow.ToString("yyyy-MM-dd");
        //        string FromDate = model.SearchFromDate != null ? Convert.ToDateTime(model.SearchFromDate).ToString("yyyy-MM-dd") : null;
        //        string ToDate = model.SearchToDate != null ? Convert.ToDateTime(model.SearchToDate).ToString("yyyy-MM-dd") : null;

        //        var enquiryLeadList = dbContext.spApplicationScheduleSelect(
        //            DbConstants.User.UserKey,
        //            model.SearchEmployeeKey,
        //            model.SearchBranchKey,
        //            model.SearchCallStatusKey,
        //            model.SearchCallTypeKey,
        //            FromDate,
        //            ToDate,
        //            model.SearchCountryKey,
        //            model.SearchUniversityKey,
        //            model.SearchProgramTypeKey,
        //            model.SearchProgramKey,
        //            model.SearchApplicationStatusKey,
        //            Today,
        //            Tomorrow,
        //            model.ScheduleStatusKey,
        //            DbConstants.ScheduleStatus.Today,
        //            DbConstants.ScheduleStatus.Tomorrow,
        //            DbConstants.ScheduleStatus.Pending,
        //            DbConstants.ScheduleStatus.Upcoming,
        //            DbConstants.ScheduleStatus.History,
        //            DbConstants.ScheduleStatus.TodayReshceduled,
        //            VerifyData(model.SearchName),
        //           VerifyData(model.SearchPhone),
        //            VerifyData(model.SearchEmail),
        //            model.PageIndex,
        //            model.PageSize,
        //            TotalCount,
        //            TodayCount,
        //            TomorrowCount,
        //            PendingCount,
        //            UpcomingCount,
        //            HistoryCount,
        //            TodayRescheduleCount).Select(row => new ApplicationScheduleViewModel
        //            {

        //                ApplicationKey = row.RowKey,
        //                EmployeeKey = row.EmployeeKey,
        //                CreatedBy = row.CreatedBy,
        //                BranchKey = row.BranchKey,
        //                Name = row.ApplicantName,
        //                Email = row.EmailAddress,
        //                Phone = row.MobileNumber,
        //                FeedbackCreatedDate = row.FeedbackCreatedDate,
        //                Feedback = row.Feedback,
        //                CallTypeKey = row.CallTypeKey,
        //                NextCallScheduleDate = row.NextCallScheduleDate,
        //                CallStatusName = row.EnquiryCallStatusName,
        //                CallTypeName = row.CallTypeName,
        //                CallDuration = row.Duration,
        //                CreateOn = row.CreateOn,
        //                CountryKey = row.CountryKey??0,
        //                ProgramKey = row.ProgramKey,
        //                ProgramTypeKey = row.ProgramTypeKey??0,
        //                UniversityKey = row.UniversityKey,
        //                IsActive = row.IsActive,
        //                ApplicationStatusName = row.ApplicationStatusName,
        //                ApplicationStatusColor = row.ApplicationStatusColor,
        //                ApplicationStatusKey = row.ApplicationStatusKey,
        //                ScheduledBy = row.ScheduledBy,
        //                CounsellingBy = row.CounsellingBy,
        //                ProgramName = row.ProgramName,
        //                ServiceTypeName = row.ServiceTypeName,
        //                FeedbackKey=row.FeedbackKey

        //            }).ToList();
        //        model.TotalRecords = TotalCount.Value != DBNull.Value ? Convert.ToInt64(TotalCount.Value) : 0;
        //        model.TodaysScheduleCount = TodayCount.Value != DBNull.Value ? Convert.ToInt64(TodayCount.Value) : 0;
        //        model.TomorrowCount = TomorrowCount.Value != DBNull.Value ? Convert.ToInt64(TomorrowCount.Value) : 0;
        //        model.PendingScheduleCount = PendingCount.Value != DBNull.Value ? Convert.ToInt64(PendingCount.Value) : 0;
        //        model.UpcomingScheduleCount = UpcomingCount.Value != DBNull.Value ? Convert.ToInt64(UpcomingCount.Value) : 0;
        //        model.HistoryCount = HistoryCount.Value != DBNull.Value ? Convert.ToInt64(HistoryCount.Value) : 0;
        //        model.TodaysRecheduleCount = TodayRescheduleCount.Value != DBNull.Value ? Convert.ToInt64(TodayRescheduleCount.Value) : 0;


        //        switch (model.ScheduleStatusKey)
        //        {
        //            case DbConstants.ScheduleStatus.Today:
        //                model.TodaysScheduleCount = model.TotalRecords;
        //                break;
        //            case DbConstants.ScheduleStatus.Pending:
        //                model.PendingScheduleCount = model.TotalRecords;
        //                break;
        //            case DbConstants.ScheduleStatus.Upcoming:
        //                model.UpcomingScheduleCount = model.TotalRecords;
        //                break;
        //            case DbConstants.ScheduleStatus.Tomorrow:
        //                model.TomorrowCount = model.TotalRecords;
        //                break;
        //            case DbConstants.ScheduleStatus.History:
        //                model.HistoryCount = model.TotalRecords;
        //                break;
        //            case DbConstants.ScheduleStatus.TodayReshceduled:
        //                model.TodaysRecheduleCount = model.TotalRecords;
        //                break;

        //        }


        //        return enquiryLeadList;
        //    }
        //    catch (Exception ex)
        //    {
        //        ActivityLog.CreateActivityLog(MenuConstants.ApplicationSchedule, ActionConstants.MenuAccess, DbConstants.LogType.Error, DbConstants.User.UserKey, ex.GetBaseException().Message);
        //        return new List<ApplicationScheduleViewModel>();
        //    }
        //}

        public ApplicationScheduleViewModel GetApplicationSchedule(ApplicationScheduleViewModel model)
        {
            string Today = DateTimeUTC.Now.ToString("yyyy-MM-dd");
            string Tomorrow = DateTimeUTC.Tomorrow.ToString("yyyy-MM-dd");


            if (model.SearchFrom == "Invalid date")
            {
                if (model.SearchTabKey == DbConstants.ScheduleStatus.History)
                    model.SearchFrom = Today;
                else
                    model.SearchFrom = null;
            }

            if (model.SearchTo == "Invalid date")
            {
                if (model.SearchTabKey == DbConstants.ScheduleStatus.History)
                    model.SearchTo = Today;
                else
                    model.SearchTo = null;

            }
            string FromDate = model.SearchFrom != null ? model.SearchFrom : null;
            string ToDate = model.SearchTo != null ? model.SearchTo : null;

            DbParameter TotalRecords = null;


            dbContext.LoadStoredProc("dbo.SP_ApplicationSchedule")
                        .WithSqlParam("UserKey", DbConstants.User.UserKey)
                        .WithSqlParam("RoleKey", DbConstants.User.RoleKey)
                        .WithSqlParam("startIndex", model.StartIndex)
                        .WithSqlParam("pageSize", model.PageSize)
                        .WithSqlParam("FetchKey", model.FetchKey)
                        .WithSqlParam("DateToday", Today)
                        .WithSqlParam("DateTomorrow", Tomorrow)
                        .WithSqlParam("SearchBranchKey", model.SearchBranchKey)
                        .WithSqlParam("SearchStudentStatusKey", model.SearchStudentStatusKey)
                        .WithSqlParam("SearchProcessStatusKey", model.SearchProcessStatusKey)
                        .WithSqlParam("SearchFromDate", FromDate)
                        .WithSqlParam("SearchToDate", ToDate)
                        .WithSqlParam("SearchAnyText", model.SearchAnyText.VerifyData())
                        .WithSqlParam("SearchTabKey", model.SearchTabKey)
                        .WithSqlParam("TodayTab", DbConstants.ScheduleStatus.Today)
                        .WithSqlParam("TomorrowTab", DbConstants.ScheduleStatus.Tomorrow)
                        .WithSqlParam("PendingTab", DbConstants.ScheduleStatus.Pending)
                        .WithSqlParam("UpcomingTab", DbConstants.ScheduleStatus.Upcoming)
                        .WithSqlParam("HistoryTab", DbConstants.ScheduleStatus.History)
                        .WithSqlParam("TotalLeadTab", DbConstants.ScheduleStatus.Total)
                        .WithSqlParam("SearchUniversityKey", model.SearchUniversityKey)
                        .WithSqlParam("SearchCourseKey", model.SearchCourseKey)
                        .WithSqlParam("SearchBatchKey", model.SearchBatchKey)
                        .WithSqlParam("SearchCallTypeKey", model.SearchCallTypeKey)
                        .WithSqlParam("SearchApplicationScheduleTypeKey", model.SearchApplicationScheduleTypeKey)
                        .WithSqlParam("SearchApplicationCallStatusKey", model.SearchApplicationCallStatusKey)
                        .WithSqlParam("TotalRecords", (dbParam) =>
                        {
                            dbParam.Direction = System.Data.ParameterDirection.Output;
                            dbParam.DbType = System.Data.DbType.Int64;
                            TotalRecords = dbParam;
                        })
                .ExecuteStoredProc((handler) =>
                {
                    model.LeadsModelList = handler.ReadToList<ApplicationScheduleViewModel>() as List<ApplicationScheduleViewModel>;
                });


            model.TotalRecords = Convert.ToInt64((TotalRecords.Value ?? 0));

            dbContext.Dispose();

            return model;
        }
        public ApplicationScheduleDetailsViewModel GetApplicationScheduleById(ApplicationScheduleDetailsViewModel model)
        {
            try
            {
                ApplicationScheduleDetailsViewModel objmodel = new ApplicationScheduleDetailsViewModel();
                objmodel = dbContext.ApplicationSchedules.Select(row => new ApplicationScheduleDetailsViewModel
                {
                    RowKey = row.RowKey,
                    ApplicationKey = row.ApplicationKey,
                    ReminderDate = row.ReminderDate,
                    Duration = row.Duration,
                    Feedback = row.Feedback,
                    ProcessStatusKey = row.ProcessStatusKey ?? 0,
                    CallTypeKey = row.CallTypeKey,
                    ApplicationScheduleTypeKey = row.ApplicationScheduleTypeKey,
                    ApplicationCallStatusKey = row.ApplicationCallStatusKey,
                    IsDuration = row.ApplicationScheduleCallStatu.IsDuration,
                }).Where(row => row.RowKey == model.RowKey).FirstOrDefault();
                if (objmodel == null)
                {
                    objmodel = new ApplicationScheduleDetailsViewModel();
                }
                objmodel.ApplicationKey = model.ApplicationKey;
                objmodel.ApplicationScheduleTypeKey = model.ApplicationScheduleTypeKey;
                FillDropDownLists(objmodel);
                return objmodel;
            }
            catch (Exception ex)
            {
                ActivityLog.CreateActivityLog(MenuConstants.FeeSchdeule, ActionConstants.View, DbConstants.LogType.Error, model.RowKey, ex.GetBaseException().Message);
                return new ApplicationScheduleDetailsViewModel();
            }

        }
        public ApplicationScheduleDetailsViewModel CreateApplicationSchedule(ApplicationScheduleDetailsViewModel model)
        {
            ApplicationSchedule applicationScheduleModel = new ApplicationSchedule();
            using (var transaction = dbContext.Database.BeginTransaction())
            {
                try
                {
                    long MaxKey = dbContext.ApplicationSchedules.Select(p => p.RowKey).DefaultIfEmpty().Max();

                    applicationScheduleModel.RowKey = MaxKey + 1;
                    applicationScheduleModel.ApplicationKey = model.ApplicationKey ?? 0;
                    applicationScheduleModel.ReminderDate = model.ReminderDate;
                    applicationScheduleModel.Duration = model.Duration;
                    applicationScheduleModel.ProcessStatusKey = model.ProcessStatusKey;
                    applicationScheduleModel.Feedback = model.Feedback;
                    applicationScheduleModel.CallTypeKey = model.CallTypeKey;
                    applicationScheduleModel.ApplicationScheduleTypeKey = model.ApplicationScheduleTypeKey;
                    applicationScheduleModel.ApplicationCallStatusKey = model.ApplicationCallStatusKey;
                    dbContext.ApplicationSchedules.Add(applicationScheduleModel);
                    model.RowKey = applicationScheduleModel.RowKey;

                    dbContext.SaveChanges();
                    transaction.Commit();
                    model.Message = EduSuiteUIResources.Success;
                    model.IsSuccessful = true;
                    ActivityLog.CreateActivityLog(MenuConstants.ApplicationSchedule, ActionConstants.Add, DbConstants.LogType.Info, model.RowKey, model.Message);


                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    model.Message = String.Format(EduSuiteUIResources.FailedToSaveErrorMessage, EduSuiteUIResources.ApplicationSchedule);
                    model.IsSuccessful = false;

                    ActivityLog.CreateActivityLog(MenuConstants.ApplicationSchedule, ActionConstants.Add, DbConstants.LogType.Error, model.RowKey, ex.GetBaseException().Message);
                }

                return model;
            }
        }
        public ApplicationScheduleDetailsViewModel UpdateApplicationSchedule(ApplicationScheduleDetailsViewModel model)
        {
            ApplicationSchedule applicationScheduleModel = new ApplicationSchedule();
            using (var transaction = dbContext.Database.BeginTransaction())
            {
                try
                {

                    applicationScheduleModel = dbContext.ApplicationSchedules.SingleOrDefault(x => x.RowKey == model.RowKey);
                    applicationScheduleModel.ApplicationKey = model.ApplicationKey ?? 0;
                    applicationScheduleModel.ReminderDate = model.ReminderDate;
                    applicationScheduleModel.Duration = model.Duration;
                    applicationScheduleModel.ProcessStatusKey = model.ProcessStatusKey;
                    applicationScheduleModel.Feedback = model.Feedback;
                    applicationScheduleModel.CallTypeKey = model.CallTypeKey;
                    applicationScheduleModel.ApplicationScheduleTypeKey = model.ApplicationScheduleTypeKey;
                    applicationScheduleModel.ApplicationCallStatusKey = model.ApplicationCallStatusKey;
                    dbContext.SaveChanges();
                    transaction.Commit();
                    model.Message = EduSuiteUIResources.Success;
                    model.IsSuccessful = true;

                    ActivityLog.CreateActivityLog(MenuConstants.ApplicationSchedule, ActionConstants.Add, DbConstants.LogType.Info, model.RowKey, model.Message);

                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    model.Message = String.Format(EduSuiteUIResources.FailedToSaveErrorMessage, EduSuiteUIResources.ApplicationSchedule);
                    model.IsSuccessful = false;

                    ActivityLog.CreateActivityLog(MenuConstants.ApplicationSchedule, ActionConstants.Add, DbConstants.LogType.Error, model.RowKey, ex.GetBaseException().Message);
                }

                return model;
            }
        }
        public ApplicationScheduleDetailsViewModel DeleteApplicationSchedule(ApplicationScheduleDetailsViewModel model)
        {
            using (var transation = dbContext.Database.BeginTransaction())
            {
                try
                {
                    ApplicationSchedule applicationScheduleModel = dbContext.ApplicationSchedules.SingleOrDefault(x => x.RowKey == model.RowKey);
                    dbContext.ApplicationSchedules.Remove(applicationScheduleModel);
                    dbContext.SaveChanges();
                    transation.Commit();
                    model.Message = EduSuiteUIResources.Success;
                    model.IsSuccessful = true;
                }
                catch (DbUpdateException ex)
                {
                    transation.Rollback();
                    if (ex.GetBaseException().Message.ToUpper().Contains(EduSuiteUIResources.ForeignKeyError.ToUpper()))
                    {
                        model.Message = String.Format(EduSuiteUIResources.CantDeleteErrorMessage, EduSuiteUIResources.ApplicationSchedule);
                        model.IsSuccessful = false;

                    }
                }
                catch (Exception ex)
                {
                    transation.Rollback();
                    model.Message = String.Format(EduSuiteUIResources.FailedToDeleteErrorMessage, EduSuiteUIResources.ApplicationSchedule);
                    model.IsSuccessful = false;
                }
                return model;
            }
        }

        private void FillApplicationScheduleTypes(ApplicationScheduleDetailsViewModel model)
        {
            if (model.ApplicationScheduleTypeKey != 0 && model.RowKey == 0)
            {
                model.ApplicationScheduleTypes = dbContext.ApplicationScheduleTypes.Where(row => row.RowKey == model.ApplicationScheduleTypeKey).Select(row => new SelectListModel
                {
                    RowKey = row.RowKey,
                    Text = row.ScheduleTypeName
                }).ToList();
            }
            else
            {
                model.ApplicationScheduleTypes = dbContext.ApplicationScheduleTypes.Where(row => row.IsActive).Select(row => new SelectListModel
                {
                    RowKey = row.RowKey,
                    Text = row.ScheduleTypeName
                }).ToList();
            }
        }
        public void FillApplicationCallStatus(ApplicationScheduleDetailsViewModel model)
        {
            //string ShowInList = DbConstants.Menu.Application.ToString();
            //model.ApplicationCallStatus = dbContext.ApplicationScheduleCallStatus.Where(x => x.IsActive && x.ShowInMenuKeys.Contains(ShowInList)).Select(row => new SelectListModel
            //{
            //    RowKey = row.RowKey,
            //    Text = row.ApplicationCallStatusName
            //}).ToList();


            model.ApplicationCallStatus = dbContext.ApplicationScheduleCallStatus.Where(x => x.IsActive).Select(row => new SelectListModel
            {
                RowKey = row.RowKey,
                Text = row.ApplicationCallStatusName
            }).ToList();

        }
        private void FillCallTypes(ApplicationScheduleDetailsViewModel model)
        {
            model.CallTypes = dbContext.CallTypes.Select(row => new SelectListModel
            {
                RowKey = row.RowKey,
                Text = row.CallTypeName
            }).ToList();

        }
        private void FillDropDownLists(ApplicationScheduleDetailsViewModel model)
        {
            FillApplicationScheduleTypes(model);
            FillApplicationCallStatus(model);
            FillCallTypes(model);
            FillStudents(model);
            FillProcessStatus(model);
        }

        //public ApplicationScheduleViewModel GetSearchDropDownLists(ApplicationScheduleViewModel model)
        //{
        //    FillApplicationScheduleDrodownLists(model);
        //    FillBranches(model);
        //    GetEmployeesByBranchId(model);            
        //    return model;
        //}

        //private void FillBranches(ApplicationScheduleViewModel model)
        //{

        //    IQueryable<SelectListModel> BranchQuery = dbContext.VwBranchSelectActiveOnlies.OrderBy(row => row.DisplayOrder).Select(row => new SelectListModel
        //    {
        //        RowKey = row.RowKey,
        //        Text = row.BranchName
        //    });

        //    Employee Employee = dbContext.Employees.Where(x => x.AppUserKey == DbConstants.User.UserKey).SingleOrDefault();
        //    if (Employee != null)
        //    {
        //        if (Employee.BranchAccess != null)
        //        {
        //            List<long> Branches = Employee.BranchAccess.Split(',').Select(Int64.Parse).ToList();
        //            model.Branches = BranchQuery.Where(row => Branches.Contains(row.RowKey)).ToList();
        //            model.SearchBranchKey = model.BranchKey = Employee.BranchKey;

        //        }
        //        else
        //        {
        //            model.Branches = BranchQuery.ToList();
        //        }
        //    }
        //    else
        //    {
        //        model.Branches = BranchQuery.ToList();
        //    }
        //}
        //public ApplicationScheduleViewModel GetEmployeesByBranchId(ApplicationScheduleViewModel model)
        //{
        //    Employee Employee = dbContext.Employees.Where(x => x.AppUserKey == DbConstants.User.UserKey).SingleOrDefault();

        //    if (DbConstants.User.UserKey != DbConstants.AdminKey)
        //    {
        //        if (Employee != null)
        //        {
        //            model.Employees = dbContext.Employees.Where(row => row.EmployeeStatusKey == DbConstants.EmployeeStatus.Working && (row.BranchKey == model.BranchKey || model.BranchKey != Employee.BranchKey) && row.RowKey == Employee.RowKey).Select(row => new GroupSelectListModel
        //            {
        //                RowKey = row.RowKey,
        //                Text = row.FirstName + " " + (row.MiddleName ?? "") + " " + row.LastName,
        //                GroupKey = row.DepartmentKey,
        //                GroupName = row.Department.DepartmentName
        //            }).OrderBy(row => row.Text).ToList();
        //            //model.SearchEmployeeKey = model.EmployeeKey = Employee.RowKey;
        //        }
        //    }
        //    else
        //    {
        //        model.Employees = dbContext.Employees.Where(row => row.EmployeeStatusKey == DbConstants.EmployeeStatus.Working && (row.BranchKey == model.BranchKey || model.BranchKey == 0)).Select(row => new GroupSelectListModel
        //        {
        //            RowKey = row.RowKey,
        //            Text = row.FirstName + " " + (row.MiddleName ?? "") + " " + row.LastName,
        //            GroupKey = row.DepartmentKey,
        //            GroupName = row.Department.DepartmentName

        //        }).OrderBy(row => row.Text).ToList();

        //        if (Employee != null)
        //        {
        //            //model.SearchEmployeeKey = model.EmployeeKey = Employee.RowKey;
        //        }
        //    }
        //    return model;
        //}
        public ApplicationScheduleDetailsViewModel CheckDuration(ApplicationScheduleDetailsViewModel model)
        {
            model.IsDuration = dbContext.ApplicationScheduleCallStatus.Where(x => x.RowKey == model.ApplicationCallStatusKey).Select(x => x.IsDuration).SingleOrDefault();
            return model;
        }
        private string VerifyData(string Data)
        {
            if (Data != null && Data != "")
            {
                Data = "%" + Data + "%";
            }
            else
            {
                Data = "%";
            }
            return Data;
        }
        public void FillStudents(ApplicationScheduleDetailsViewModel model)
        {
            if (model.ApplicationKey != 0)
            {
                model.Students = dbContext.Applications.Where(x => x.RowKey == model.ApplicationKey).Select(row => new SelectListModel
                {
                    RowKey = row.RowKey,
                    Text = row.StudentName
                }).ToList();
            }
            else
            {
                model.Students = dbContext.Applications.Where(x => x.StudentStatusKey == DbConstants.StudentStatus.Ongoing).Select(row => new SelectListModel
                {
                    RowKey = row.RowKey,
                    Text = row.StudentName
                }).ToList();
            }
        }
        public void FillProcessStatus(ApplicationScheduleDetailsViewModel model)
        {
            model.ProcessStatuses = dbContext.ProcessStatus.Where(x => x.RowKey != DbConstants.ProcessStatus.Rejected).Select(row => new SelectListModel
            {
                RowKey = row.RowKey,
                Text = row.RowKey == DbConstants.ProcessStatus.Approved ? "Completed" : row.ProcessStatusName
            }).ToList();

        }
        public void FillSearchApplicationScheduleTypes(ApplicationScheduleViewModel model)
        {
            model.ApplicationScheduleTypes = dbContext.ApplicationScheduleTypes.Where(row => row.IsActive).Select(row => new SelectListModel
            {
                RowKey = row.RowKey,
                Text = row.ScheduleTypeName
            }).ToList();
        }
        public void FillSearchApplicationCallStatus(ApplicationScheduleViewModel model)
        {
            model.ApplicationCallStatus = dbContext.ApplicationScheduleCallStatus.Where(x => x.IsActive).Select(row => new SelectListModel
            {
                RowKey = row.RowKey,
                Text = row.ApplicationCallStatusName
            }).ToList();

        }
        public void FillSearchCallTypes(ApplicationScheduleViewModel model)
        {
            model.CallTypes = dbContext.CallTypes.Select(row => new SelectListModel
            {
                RowKey = row.RowKey,
                Text = row.CallTypeName
            }).ToList();

        }
        public void FillSearchProcessStatus(ApplicationScheduleViewModel model)
        {
            model.ProcessStatuses = dbContext.ProcessStatus.Where(x => x.RowKey != DbConstants.ProcessStatus.Rejected).Select(row => new SelectListModel
            {
                RowKey = row.RowKey,
                Text = row.RowKey == DbConstants.ProcessStatus.Approved ? "Completed" : row.ProcessStatusName
            }).ToList();

        }
    }
}
