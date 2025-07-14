using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CITS.EduSuite.Business.Common;
using CITS.EduSuite.Business.Interfaces;
using CITS.EduSuite.Business.Models.ViewModels;
using CITS.EduSuite.Data;
using System.Globalization;
using System.Data.Entity.Infrastructure;
using CITS.EduSuite.Business.Models.Resources;

namespace CITS.EduSuite.Business.Services
{
    public class StudentLeaveService : IStudentLeaveService
    {
        private EduSuiteDatabase dbContext;
        public StudentLeaveService(EduSuiteDatabase objDB)
        {
            this.dbContext = objDB;
        }

        public StudentLeaveViewModel GetStudentLeaveById(StudentLeaveViewModel model)
        {
            try
            {
                StudentLeaveViewModel objViewModel = new StudentLeaveViewModel();
                Application Applications = dbContext.Applications.Where(x => x.RowKey == model.ApplicationsKey).FirstOrDefault();

                objViewModel = dbContext.StudentLeaveDetails.Where(x => x.RowKey == model.RowKey).Select(row => new StudentLeaveViewModel
                {
                    RowKey = row.RowKey,
                    LeaveDateFrom = row.LeaveDateFrom,
                    LeaveDateTo = row.LeaveDateTo,
                    IsApprove = row.IsApprove ?? false,
                    Remarks = row.Remarks,
                    AttachmentPath = row.AttachmentPath,
                    ApplicationsKey = row.ApplicationKey,
                    AttendanceTypeKey = row.AttendanceTypeKey
                }).FirstOrDefault();

                if (objViewModel == null)
                {

                    objViewModel = dbContext.AttendanceDetails.Where(x => x.Attendance.ApplicationKey == model.ApplicationsKey && x.AttendanceStatusKey == DbConstants.AttendanceStatus.Leave).Select(row => new StudentLeaveViewModel
                    {
                        RowKey = 0,
                        AttendanceDetailsKey = row.RowKey,
                        LeaveDateFrom = row.Attendance.AttendanceDate,

                        Remarks = "Leave",
                        AttachmentPath = "",
                        ApplicationsKey = model.ApplicationsKey,
                    }).FirstOrDefault();
                    if (objViewModel == null)
                    {

                        objViewModel = new StudentLeaveViewModel();
                        objViewModel.Message = String.Format(EduSuiteUIResources.ExistsErrorMessage, EduSuiteUIResources.StudentLeave);
                        objViewModel.IsSuccessful = false;
                    }
                    else
                    {
                        objViewModel.Message = EduSuiteUIResources.Success;
                        objViewModel.IsSuccessful = true;
                    }
                    objViewModel.RowKey = model.RowKey;
                    objViewModel.ApplicationsKey = model.ApplicationsKey;

                }
                else
                {
                    objViewModel.Message = EduSuiteUIResources.Success;
                    objViewModel.IsSuccessful = true;
                }
                objViewModel.StudentMobile = Applications != null ? Applications.StudentMobile : null;
                objViewModel.StudentEmail = Applications != null ? Applications.StudentEmail : null;
                //objViewModel.StudentGuardianPhone = Applications != null ? Applications.StudentGuardianPhone : null;
                FillNotificationDetail(objViewModel);
                return objViewModel;
            }
            catch (Exception ex)
            {
                ActivityLog.CreateActivityLog(MenuConstants.StudentLeave, ActionConstants.View, DbConstants.LogType.Debug, model.ApplicationsKey, ex.GetBaseException().Message);
                return new StudentLeaveViewModel();
            }
        }


        public StudentLeaveViewModel CreateStudentLeaveDate(StudentLeaveViewModel model)
        {
            using (var transaction = dbContext.Database.BeginTransaction())
            {
                try
                {

                    StudentLeaveDetail StudentLeaveDetailModel = new StudentLeaveDetail();
                    long? MaxKey = dbContext.StudentLeaveDetails.Select(x => x.RowKey).DefaultIfEmpty().Max();

                    StudentLeaveDetailModel.RowKey = Convert.ToInt64(MaxKey + 1);
                    StudentLeaveDetailModel.ApplicationKey = model.ApplicationsKey ?? 0;
                    StudentLeaveDetailModel.LeaveDateFrom = model.LeaveDateFrom;
                    StudentLeaveDetailModel.LeaveDateTo = model.LeaveDateTo;
                    StudentLeaveDetailModel.Remarks = model.Remarks;
                    StudentLeaveDetailModel.AttachmentPath = model.AttachmentPath;
                    StudentLeaveDetailModel.AttendanceTypeKey = model.AttendanceTypeKey;
                    StudentLeaveDetailModel.ClassDetailsKey = dbContext.Applications.Where(x => x.RowKey == model.ApplicationsKey).Select(y => y.ClassDetailsKey).FirstOrDefault();
                    StudentLeaveDetailModel.BatchKey = dbContext.Applications.Where(x => x.RowKey == model.ApplicationsKey).Select(y => y.BatchKey).FirstOrDefault();

                    List<AttendanceDetail> AttendanceDetailList = dbContext.AttendanceDetails.Where(row => System.Data.Entity.DbFunctions.TruncateTime(row.Attendance.AttendanceDate) >= System.Data.Entity.DbFunctions.TruncateTime(model.LeaveDateFrom)
                                          && System.Data.Entity.DbFunctions.TruncateTime(row.Attendance.AttendanceDate) <= System.Data.Entity.DbFunctions.TruncateTime(model.LeaveDateTo)
                                          && row.Attendance.ApplicationKey == model.ApplicationsKey && (row.AttendanceTypeKey == (model.AttendanceTypeKey ?? row.AttendanceTypeKey))).ToList();

                    foreach (AttendanceDetail AttendanceDetail in AttendanceDetailList)
                    {
                        AttendanceDetail.StudentLeaveKey = StudentLeaveDetailModel.RowKey;
                        AttendanceDetail.AttendanceStatusKey = DbConstants.AttendanceStatus.Leave;
                    }
                    dbContext.StudentLeaveDetails.Add(StudentLeaveDetailModel);


                    dbContext.SaveChanges();
                    transaction.Commit();
                    model.RowKey = StudentLeaveDetailModel.RowKey;
                    model.Message = EduSuiteUIResources.Success;
                    model.IsSuccessful = true;
                    ActivityLog.CreateActivityLog(MenuConstants.StudentLeave, ActionConstants.Add, DbConstants.LogType.Info, model.RowKey, model.Message);

                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    model.Message = String.Format(EduSuiteUIResources.FailedToSaveErrorMessage, EduSuiteUIResources.StudentLeave);
                    model.IsSuccessful = false;
                    ActivityLog.CreateActivityLog(MenuConstants.StudentLeave, ActionConstants.Add, DbConstants.LogType.Error, model.RowKey, ex.GetBaseException().Message);
                }
            }
            return model;

        }

        public StudentLeaveViewModel UpdateStudentLeaveDate(StudentLeaveViewModel model)
        {
            using (var transaction = dbContext.Database.BeginTransaction())
            {
                try
                {
                    StudentLeaveDetail StudentLeaveDetailModel = new StudentLeaveDetail();

                    StudentLeaveDetailModel = dbContext.StudentLeaveDetails.Where(p => p.RowKey == model.RowKey).FirstOrDefault();
                    DateTime? OldLeaveDateFrom = StudentLeaveDetailModel.LeaveDateFrom;
                    DateTime? OldLeaveDateTo = StudentLeaveDetailModel.LeaveDateTo;
                    short? OldAttendanceTypeKey = StudentLeaveDetailModel.AttendanceTypeKey;

                    StudentLeaveDetailModel.LeaveDateFrom = model.LeaveDateFrom;
                    StudentLeaveDetailModel.LeaveDateTo = model.LeaveDateTo;
                    StudentLeaveDetailModel.Remarks = model.Remarks;
                    StudentLeaveDetailModel.AttachmentPath = model.AttachmentPath;
                    StudentLeaveDetailModel.AttendanceTypeKey = model.AttendanceTypeKey;

                    List<AttendanceDetail> AttendanceDetailList = new List<AttendanceDetail>();
                    if (System.Data.Entity.DbFunctions.TruncateTime(model.LeaveDateFrom) != System.Data.Entity.DbFunctions.TruncateTime(OldLeaveDateFrom) || System.Data.Entity.DbFunctions.TruncateTime(model.LeaveDateTo) != System.Data.Entity.DbFunctions.TruncateTime(OldLeaveDateTo))
                    {
                        AttendanceDetailList = dbContext.AttendanceDetails.Where(row => !(System.Data.Entity.DbFunctions.TruncateTime(row.Attendance.AttendanceDate) <= System.Data.Entity.DbFunctions.TruncateTime(model.LeaveDateFrom)
                           && System.Data.Entity.DbFunctions.TruncateTime(row.Attendance.AttendanceDate) >= System.Data.Entity.DbFunctions.TruncateTime(model.LeaveDateFrom))
                            && System.Data.Entity.DbFunctions.TruncateTime(row.Attendance.AttendanceDate) >= System.Data.Entity.DbFunctions.TruncateTime(OldLeaveDateFrom)
                            && System.Data.Entity.DbFunctions.TruncateTime(row.Attendance.AttendanceDate) <= System.Data.Entity.DbFunctions.TruncateTime(OldLeaveDateTo) && row.Attendance.ApplicationKey == model.ApplicationsKey).ToList();

                        foreach (AttendanceDetail AttendanceDetail in AttendanceDetailList)
                        {
                            AttendanceDetail.StudentLeaveKey = null;
                            AttendanceDetail.AttendanceStatusKey = DbConstants.AttendanceStatus.Absent;
                        }
                    }
                    AttendanceDetailList = dbContext.AttendanceDetails.Where(row => System.Data.Entity.DbFunctions.TruncateTime(row.Attendance.AttendanceDate) >= System.Data.Entity.DbFunctions.TruncateTime(model.LeaveDateFrom)
                        && System.Data.Entity.DbFunctions.TruncateTime(row.Attendance.AttendanceDate) <= System.Data.Entity.DbFunctions.TruncateTime(model.LeaveDateTo)
                        && row.Attendance.ApplicationKey == model.ApplicationsKey && (row.AttendanceTypeKey == (model.AttendanceTypeKey ?? row.AttendanceTypeKey))).ToList();

                    foreach (AttendanceDetail AttendanceDetail in AttendanceDetailList)
                    {
                        AttendanceDetail.StudentLeaveKey = StudentLeaveDetailModel.RowKey;
                        AttendanceDetail.AttendanceStatusKey = DbConstants.AttendanceStatus.Leave;
                    }

                    dbContext.SaveChanges();
                    transaction.Commit();

                    model.Message = EduSuiteUIResources.Success;
                    model.IsSuccessful = true;
                    ActivityLog.CreateActivityLog(MenuConstants.StudentLeave, ActionConstants.Edit, DbConstants.LogType.Info, model.RowKey, model.Message);

                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    model.Message = String.Format(EduSuiteUIResources.FailedToSaveErrorMessage, EduSuiteUIResources.StudentLeave);
                    model.IsSuccessful = false;
                    ActivityLog.CreateActivityLog(MenuConstants.StudentLeave, ActionConstants.Edit, DbConstants.LogType.Error, model.RowKey, ex.GetBaseException().Message);
                }
            }
            return model;
        }


        public StudentLeaveViewModel DeleteStudentLeave(long? Id)
        {
            StudentLeaveViewModel model = new StudentLeaveViewModel();

            using (var transaction = dbContext.Database.BeginTransaction())
            {
                try
                {
                    StudentLeaveDetail StudentLeaveDetails = dbContext.StudentLeaveDetails.SingleOrDefault(x => x.RowKey == Id);
                    List<AttendanceDetail> AttendanceDetailList = dbContext.AttendanceDetails.Where(row => row.Attendance.ApplicationKey == StudentLeaveDetails.ApplicationKey &&
                           System.Data.Entity.DbFunctions.TruncateTime(row.Attendance.AttendanceDate) >= System.Data.Entity.DbFunctions.TruncateTime(StudentLeaveDetails.LeaveDateFrom)
                           && System.Data.Entity.DbFunctions.TruncateTime(row.Attendance.AttendanceDate) <= System.Data.Entity.DbFunctions.TruncateTime(StudentLeaveDetails.LeaveDateTo)).ToList();

                    foreach (AttendanceDetail AttendanceDetail in AttendanceDetailList)
                    {
                        AttendanceDetail.StudentLeaveKey = null;
                        AttendanceDetail.StudentLeaveKey = null;
                        AttendanceDetail.AttendanceStatusKey = DbConstants.AttendanceStatus.Absent;
                    }

                    dbContext.StudentLeaveDetails.Remove(StudentLeaveDetails);


                    dbContext.SaveChanges();
                    transaction.Commit();
                    model.Message = EduSuiteUIResources.Success;
                    model.IsSuccessful = true;
                    ActivityLog.CreateActivityLog(MenuConstants.StudentLeave, ActionConstants.Delete, DbConstants.LogType.Info, Id, model.Message);

                }
                catch (DbUpdateException ex)
                {
                    transaction.Rollback();
                    if (ex.GetBaseException().Message.ToUpper().Contains(EduSuiteUIResources.ForeignKeyError.ToUpper()))
                    {
                        model.Message = String.Format(EduSuiteUIResources.CantDeleteErrorMessage, EduSuiteUIResources.StudentEarlyDeparture);
                        model.IsSuccessful = false;
                        ActivityLog.CreateActivityLog(MenuConstants.StudentLeave, ActionConstants.Delete, DbConstants.LogType.Debug, Id, ex.GetBaseException().Message);
                    }
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    model.Message = String.Format(EduSuiteUIResources.FailedToDeleteErrorMessage, EduSuiteUIResources.StudentEarlyDeparture);
                    model.IsSuccessful = false;
                    ActivityLog.CreateActivityLog(MenuConstants.StudentLeave, ActionConstants.Delete, DbConstants.LogType.Error, Id, ex.GetBaseException().Message);
                }
            }
            return model;


        }


        public List<StudentLeaveViewModel> GetStudentLeaveDetails(long ApplicationKey)
        {
            try
            {
                long ClassDetailsKey = dbContext.Applications.Where(x => x.RowKey == ApplicationKey).Select(y => y.ClassDetailsKey ?? 0).FirstOrDefault();


                var applicationFeeList = (from es in dbContext.StudentLeaveDetails.Where(row => row.ApplicationKey == ApplicationKey && (row.ClassDetailsKey ?? 0) == ClassDetailsKey)
                                          orderby es.RowKey ascending
                                          select new StudentLeaveViewModel
                                          {
                                              RowKey = es.RowKey,
                                              LeaveDateFrom = es.LeaveDateFrom,
                                              LeaveDateTo = es.LeaveDateTo,
                                              Remarks = es.Remarks,
                                              AttachmentPath = es.AttachmentPath,
                                              ApplicationsKey = es.ApplicationKey,
                                          }).ToList();

                return applicationFeeList.ToList<StudentLeaveViewModel>();

            }
            catch (Exception ex)
            {
                ActivityLog.CreateActivityLog(MenuConstants.StudentLeave, ActionConstants.View, DbConstants.LogType.Debug, ApplicationKey, ex.GetBaseException().Message);
                return new List<StudentLeaveViewModel>();
            }

        }


        private void FillNotificationDetail(StudentLeaveViewModel model)
        {
            NotificationTemplate notificationTemplateModel = dbContext.NotificationTemplates.SingleOrDefault(row => row.RowKey == DbConstants.NotificationTemplate.StudentLeave);
            if (notificationTemplateModel != null)
            {
                model.AutoEmail = notificationTemplateModel.AutoEmail;
                model.AutoSMS = notificationTemplateModel.AutoSMS;
                //model.GuardianSMS = notificationTemplateModel.GuardianSMS;
                model.TemplateKey = notificationTemplateModel.RowKey;
            }
        }
    }
}
