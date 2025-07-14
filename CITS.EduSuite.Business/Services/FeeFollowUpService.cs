using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CITS.EduSuite.Business.Interfaces;
using CITS.EduSuite.Business.Models.ViewModels;
using CITS.EduSuite.Data;
using System.Data.Entity;
using System.Data.Entity.Core.Objects;
using System.Data.Common;
using CITS.EduSuite.Business.Extensions;
using CITS.EduSuite.Business.Models.Resources;
using System.Data.Entity.Infrastructure;

namespace CITS.EduSuite.Business.Services
{
    public class FeeFollowUpService : IFeeFollowUpService
    {
        private EduSuiteDatabase dbContext;
        public FeeFollowUpService(EduSuiteDatabase objDB)
        {
            this.dbContext = objDB;
        }
        public ApplicationFeeFollowupViewModel GetFollowup(ApplicationFeeFollowupViewModel model)
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


            dbContext.LoadStoredProc("dbo.SP_StudentFeeFollowup")
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
                        .WithSqlParam("TotalRecords", (dbParam) =>
                        {
                            dbParam.Direction = System.Data.ParameterDirection.Output;
                            dbParam.DbType = System.Data.DbType.Int64;
                            TotalRecords = dbParam;
                        })
                .ExecuteStoredProc((handler) =>
                {
                    model.LeadsModelList = handler.ReadToList<ApplicationFeeFollowupViewModel>() as List<ApplicationFeeFollowupViewModel>;
                });


            model.TotalRecords = Convert.ToInt64((TotalRecords.Value ?? 0));

            dbContext.Dispose();

            return model;
        }
        public ApplicationFeeFollowupDetailsViewModel GetFeeFollowupById(ApplicationFeeFollowupDetailsViewModel model)
        {
            try
            {
                ApplicationFeeFollowupDetailsViewModel objmodel = new ApplicationFeeFollowupDetailsViewModel();
                objmodel = dbContext.ApplicationFeeFollowups.Select(row => new ApplicationFeeFollowupDetailsViewModel
                {
                    RowKey = row.RowKey,
                    ApplicationKey = row.ApplicationKey,
                    FollowupDate = row.FollowupDate,
                    FeeTypeKeys = dbContext.FeeFollowUpFeeTypes.Where(y => y.ApplicationFeeFollowUpKey == row.RowKey).Select(x => x.FeeTypeKey).ToList(),
                    Remarks = row.Remarks,
                    ProcessStatusKey = row.ProcessStatusKey ?? 0,
                    Amount = row.Amount
                }).Where(row => row.RowKey == model.RowKey).FirstOrDefault();
                if (objmodel == null)
                {
                    objmodel = new ApplicationFeeFollowupDetailsViewModel();
                }
                objmodel.ApplicationKey = model.ApplicationKey;
                FillDropDown(objmodel);
                return objmodel;
            }
            catch (Exception ex)
            {
                ActivityLog.CreateActivityLog(MenuConstants.FeeSchdeule, ActionConstants.View, DbConstants.LogType.Error, model.RowKey, ex.GetBaseException().Message);
                return new ApplicationFeeFollowupDetailsViewModel();
            }

        }
        public ApplicationFeeFollowupDetailsViewModel CreateFeeFollowup(ApplicationFeeFollowupDetailsViewModel model)
        {
            ApplicationFeeFollowup ApplicationFeeFollowupModel = new ApplicationFeeFollowup();
            using (var transaction = dbContext.Database.BeginTransaction())
            {
                try
                {
                    long MaxKey = dbContext.ApplicationFeeFollowups.Select(p => p.RowKey).DefaultIfEmpty().Max();

                    ApplicationFeeFollowupModel.RowKey = MaxKey + 1;
                    ApplicationFeeFollowupModel.ApplicationKey = model.ApplicationKey ?? 0;
                    ApplicationFeeFollowupModel.FollowupDate = model.FollowupDate;
                    ApplicationFeeFollowupModel.Remarks = model.Remarks;
                    ApplicationFeeFollowupModel.ProcessStatusKey = model.ProcessStatusKey;
                    ApplicationFeeFollowupModel.IfExtendDate = model.IfExtendDate;
                    dbContext.ApplicationFeeFollowups.Add(ApplicationFeeFollowupModel);
                    model.RowKey = ApplicationFeeFollowupModel.RowKey;
                    UpdateFeeTypes(model);
                    dbContext.SaveChanges();
                    transaction.Commit();
                    model.Message = EduSuiteUIResources.Success;
                    model.IsSuccessful = true;
                    ActivityLog.CreateActivityLog(MenuConstants.FeeSchdeule, ActionConstants.Add, DbConstants.LogType.Info, model.RowKey, model.Message);


                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    model.Message = String.Format(EduSuiteUIResources.FailedToSaveErrorMessage, EduSuiteUIResources.FeeSchdeule);
                    model.IsSuccessful = false;

                    ActivityLog.CreateActivityLog(MenuConstants.FeeSchdeule, ActionConstants.Add, DbConstants.LogType.Error, model.RowKey, ex.GetBaseException().Message);
                }
                FillDropDown(model);
                return model;
            }
        }
        public ApplicationFeeFollowupDetailsViewModel UpdateFeeFollowup(ApplicationFeeFollowupDetailsViewModel model)
        {
            ApplicationFeeFollowup ApplicationFeeFollowupModel = new ApplicationFeeFollowup();
            using (var transaction = dbContext.Database.BeginTransaction())
            {
                try
                {

                    ApplicationFeeFollowupModel = dbContext.ApplicationFeeFollowups.SingleOrDefault(x => x.RowKey == model.RowKey);
                    ApplicationFeeFollowupModel.ApplicationKey = model.ApplicationKey ?? 0;
                    ApplicationFeeFollowupModel.Remarks = model.Remarks;
                    ApplicationFeeFollowupModel.ProcessStatusKey = model.ProcessStatusKey;
                    ApplicationFeeFollowupModel.IfExtendDate = model.IfExtendDate;
                    if (model.IfExtendDate)
                    {
                        ApplicationFeeFollowupModel.FollowupDate = model.NextFollowUpDate;
                    }
                    else
                    {
                        ApplicationFeeFollowupModel.FollowupDate = model.FollowupDate;
                    }
                    UpdateFeeTypes(model);
                    CreateExtendDate(model);
                    dbContext.SaveChanges();
                    transaction.Commit();
                    model.Message = EduSuiteUIResources.Success;
                    model.IsSuccessful = true;

                    ActivityLog.CreateActivityLog(MenuConstants.FeeSchdeule, ActionConstants.Add, DbConstants.LogType.Info, model.RowKey, model.Message);

                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    model.Message = String.Format(EduSuiteUIResources.FailedToSaveErrorMessage, EduSuiteUIResources.FeeSchdeule);
                    model.IsSuccessful = false;

                    ActivityLog.CreateActivityLog(MenuConstants.FeeSchdeule, ActionConstants.Add, DbConstants.LogType.Error, model.RowKey, ex.GetBaseException().Message);
                }
                FillDropDown(model);
                return model;
            }
        }
        public ApplicationFeeFollowupDetailsViewModel DeleteFeeFollowup(ApplicationFeeFollowupDetailsViewModel model)
        {
            using (var transation = dbContext.Database.BeginTransaction())
            {
                try
                {
                    ApplicationFeeFollowup ApplicationFeeFollowupModel = dbContext.ApplicationFeeFollowups.SingleOrDefault(x => x.RowKey == model.RowKey);
                    List<FeeFollowUpFeeType> FeeFollowUpFeeTypesList = dbContext.FeeFollowUpFeeTypes.Where(x => x.ApplicationFeeFollowUpKey == model.RowKey).ToList();
                    if (FeeFollowUpFeeTypesList.Count > 0)
                    {
                        dbContext.FeeFollowUpFeeTypes.RemoveRange(FeeFollowUpFeeTypesList);
                    }
                    List<ExtendFeeFollowUpDate> ExtendFeeFollowUpDatesList = dbContext.ExtendFeeFollowUpDates.Where(x => x.ApplicationFeeFollowupKey == model.RowKey).ToList();
                    if (ExtendFeeFollowUpDatesList.Count > 0)
                    {
                        dbContext.ExtendFeeFollowUpDates.RemoveRange(ExtendFeeFollowUpDatesList);
                    }
                    dbContext.ApplicationFeeFollowups.Remove(ApplicationFeeFollowupModel);
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
                        model.Message = String.Format(EduSuiteUIResources.CantDeleteErrorMessage, EduSuiteUIResources.FeeSchdeule);
                        model.IsSuccessful = false;

                    }
                }
                catch (Exception ex)
                {
                    transation.Rollback();
                    model.Message = String.Format(EduSuiteUIResources.FailedToDeleteErrorMessage, EduSuiteUIResources.FeeSchdeule);
                    model.IsSuccessful = false;
                }
                return model;
            }
        }
        private void UpdateFeeTypes(ApplicationFeeFollowupDetailsViewModel model)
        {
            long MaxKey = dbContext.FeeFollowUpFeeTypes.Select(x => x.RowKey).DefaultIfEmpty().Max();
            List<FeeFollowUpFeeType> FeeFollowUpFeeTypes = dbContext.FeeFollowUpFeeTypes.Where(x => x.ApplicationFeeFollowUpKey == model.RowKey).ToList();
            if (FeeFollowUpFeeTypes.Count > 0)
            {
                dbContext.FeeFollowUpFeeTypes.RemoveRange(FeeFollowUpFeeTypes);
            }

            if (model.FeeTypeKeys != null && model.FeeTypeKeys.Count > 0)
            {
                foreach (short FeeTypekey in model.FeeTypeKeys)
                {
                    FeeFollowUpFeeType FeeFollowUpFeeTypemodel = new FeeFollowUpFeeType();

                    FeeFollowUpFeeTypemodel.RowKey = MaxKey + 1;
                    FeeFollowUpFeeTypemodel.ApplicationFeeFollowUpKey = model.RowKey;
                    FeeFollowUpFeeTypemodel.FeeTypeKey = FeeTypekey;

                    dbContext.FeeFollowUpFeeTypes.Add(FeeFollowUpFeeTypemodel);
                    MaxKey++;
                }
            }
        }
        private void CreateExtendDate(ApplicationFeeFollowupDetailsViewModel model)
        {
            long MaxKey = dbContext.ExtendFeeFollowUpDates.Select(x => x.RowKey).DefaultIfEmpty().Max();

            if (model.IfExtendDate == true)
            {
                ExtendFeeFollowUpDate FeeFollowUpFeeTypemodel = new ExtendFeeFollowUpDate();

                FeeFollowUpFeeTypemodel.RowKey = MaxKey + 1;
                FeeFollowUpFeeTypemodel.ApplicationFeeFollowupKey = model.RowKey;
                FeeFollowUpFeeTypemodel.OldFollowUpDate = model.FollowupDate;
                FeeFollowUpFeeTypemodel.NextFollowUpDate = model.NextFollowUpDate;
                dbContext.ExtendFeeFollowUpDates.Add(FeeFollowUpFeeTypemodel);
            }
        }
        public void FillStudents(ApplicationFeeFollowupDetailsViewModel model)
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
        public void FillProcessStatus(ApplicationFeeFollowupDetailsViewModel model)
        {
            model.ProcessStatuses = dbContext.ProcessStatus.Where(x => x.RowKey != DbConstants.ProcessStatus.Rejected).Select(row => new SelectListModel
            {
                RowKey = row.RowKey,
                Text = row.RowKey == DbConstants.ProcessStatus.Approved ? "Completed" : row.ProcessStatusName
            }).ToList();

        }
        public void FillSearchProcessStatus(ApplicationFeeFollowupViewModel model)
        {
            model.ProcessStatuses = dbContext.ProcessStatus.Where(x => x.RowKey != DbConstants.ProcessStatus.Rejected).Select(row => new SelectListModel
            {
                RowKey = row.RowKey,
                Text = row.RowKey == DbConstants.ProcessStatus.Approved ? "Completed" : row.ProcessStatusName
            }).ToList();

        }
        public void FillFeeTypes(ApplicationFeeFollowupDetailsViewModel model)
        {
            model.FeeTypes = dbContext.FeeTypes.Where(x => x.IsActive).Select(row => new SelectListModel
            {
                RowKey = row.RowKey,
                Text = row.FeeTypeName
            }).ToList();

        }
        public void FillDropDown(ApplicationFeeFollowupDetailsViewModel model)
        {
            FillProcessStatus(model);
            FillStudents(model);
            FillFeeTypes(model);
        }
    }
}
