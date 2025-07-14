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
    public class StudentLateService : IStudentLateService
    {
        private EduSuiteDatabase dbContext;
        public StudentLateService(EduSuiteDatabase objDB)
        {
            this.dbContext = objDB;
        }

        public StudentLateViewModel GetStudentLateById(StudentLateViewModel model)
        {
            try
            {
                StudentLateViewModel objViewModel = new StudentLateViewModel();
                Application Applications = dbContext.Applications.Where(x => x.RowKey == model.ApplicationsKey).FirstOrDefault();

                objViewModel = dbContext.StudentLateDetails.Where(x => x.RowKey == model.RowKey).Select(row => new StudentLateViewModel
                {
                    RowKey = row.RowKey,
                    LateDate = row.LateDate,
                    LateMinutes = row.LateMinutes,
                    Remarks = row.Remarks,
                    AttachmentPath = row.AttachmentPath,
                    ApplicationsKey = row.ApplicationKey,
                    AttendanceTypeKey = row.AttendanceTypeKey ?? 0,
                    MarkHalfDay = row.MarkHalfDay 
                }).FirstOrDefault();

                if (objViewModel == null)
                {
                    objViewModel = dbContext.AttendanceDetails.Where(x => x.Attendance.ApplicationKey == model.ApplicationsKey && x.AttendanceStatusKey == DbConstants.AttendanceStatus.Absent).Select(row => new StudentLateViewModel
                    {
                        RowKey = 0,
                        AttendanceDetailsKey = row.RowKey,
                        LateDate = row.Attendance.AttendanceDate,
                        LateMinutes = null,
                        Remarks = "Late",
                        AttachmentPath = "",
                        ApplicationsKey = model.ApplicationsKey,
                    }).FirstOrDefault();
                    if (objViewModel == null)
                    {

                        objViewModel = new StudentLateViewModel();
                        objViewModel.Message = String.Format(EduSuiteUIResources.ExistsErrorMessage, EduSuiteUIResources.StudentLate);
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
                ActivityLog.CreateActivityLog(MenuConstants.StudentLate, ActionConstants.View, DbConstants.LogType.Debug, model.ApplicationsKey, ex.GetBaseException().Message);
                return new StudentLateViewModel();
            }
        }


        public StudentLateViewModel CreateStudentLateDate(StudentLateViewModel model)
        {
            using (var transaction = dbContext.Database.BeginTransaction())
            {
                try
                {
                    StudentLateDetail StudentLateDetailModel = new StudentLateDetail();
                    long? MaxKey = dbContext.StudentLateDetails.Select(x => x.RowKey).DefaultIfEmpty().Max();

                    StudentLateDetailModel.RowKey = Convert.ToInt64(MaxKey + 1);
                    StudentLateDetailModel.ApplicationKey = model.ApplicationsKey ?? 0;
                    StudentLateDetailModel.LateDate = model.LateDate;
                    StudentLateDetailModel.LateMinutes = model.LateMinutes;
                    StudentLateDetailModel.Remarks = model.Remarks;
                    StudentLateDetailModel.AttachmentPath = model.AttachmentPath;
                    StudentLateDetailModel.AttendanceTypeKey = model.AttendanceTypeKey;
                    StudentLateDetailModel.ClassDetailsKey = dbContext.Applications.Where(x => x.RowKey == model.ApplicationsKey).Select(y => y.ClassDetailsKey).FirstOrDefault();
                    StudentLateDetailModel.BatchKey = dbContext.Applications.Where(x => x.RowKey == model.ApplicationsKey).Select(y => y.BatchKey).FirstOrDefault();
                    StudentLateDetailModel.MarkHalfDay = model.MarkHalfDay;
                    List<AttendanceDetail> AttendanceDetailList = dbContext.AttendanceDetails.Where(row => System.Data.Entity.DbFunctions.TruncateTime(row.Attendance.AttendanceDate) == System.Data.Entity.DbFunctions.TruncateTime(model.LateDate) && row.Attendance.ApplicationKey == model.ApplicationsKey && (row.AttendanceTypeKey == (model.AttendanceTypeKey ?? row.AttendanceTypeKey))).ToList();

                    foreach (AttendanceDetail AttendanceDetail in AttendanceDetailList)
                    {
                        AttendanceDetail.StudentLateKey = StudentLateDetailModel.RowKey;
                        AttendanceDetail.AttendanceStatus = true;
                        AttendanceDetail.AttendanceStatusKey = DbConstants.AttendanceStatus.Late;
                        if (model.MarkHalfDay == true)
                        {
                            AttendanceDetail.AttendanceStatusKey = DbConstants.AttendanceStatus.HalfDay;
                        }
                        else
                        {
                            AttendanceDetail.AttendanceStatusKey = DbConstants.AttendanceStatus.Late;
                        }
                    }
                    dbContext.StudentLateDetails.Add(StudentLateDetailModel);
                    dbContext.SaveChanges();
                    transaction.Commit();
                    model.RowKey = StudentLateDetailModel.RowKey;
                    model.Message = EduSuiteUIResources.Success;
                    model.IsSuccessful = true;
                    ActivityLog.CreateActivityLog(MenuConstants.StudentLate, ActionConstants.Add, DbConstants.LogType.Info, model.RowKey, model.Message);

                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    model.Message = String.Format(EduSuiteUIResources.FailedToSaveErrorMessage, EduSuiteUIResources.StudentLate);
                    model.IsSuccessful = false;
                    ActivityLog.CreateActivityLog(MenuConstants.StudentLate, ActionConstants.Add, DbConstants.LogType.Error, model.RowKey, ex.GetBaseException().Message);
                }
            }
            return model;

        }

        public StudentLateViewModel UpdateStudentLateDate(StudentLateViewModel model)
        {
            using (var transaction = dbContext.Database.BeginTransaction())
            {
                try
                {
                    StudentLateDetail StudentLateDetailModel = new StudentLateDetail();

                    StudentLateDetailModel = dbContext.StudentLateDetails.Where(p => p.RowKey == model.RowKey).FirstOrDefault();
                    var oldAttendanceTypeKey = StudentLateDetailModel.AttendanceTypeKey;
                    StudentLateDetailModel.LateDate = model.LateDate;
                    StudentLateDetailModel.LateMinutes = model.LateMinutes;
                    StudentLateDetailModel.Remarks = model.Remarks;
                    StudentLateDetailModel.AttachmentPath = model.AttachmentPath;
                    StudentLateDetailModel.AttendanceTypeKey = model.AttendanceTypeKey;
                    StudentLateDetailModel.MarkHalfDay = model.MarkHalfDay;

                    List<AttendanceDetail> AttendanceDetailList = dbContext.AttendanceDetails.Where(row => System.Data.Entity.DbFunctions.TruncateTime(row.Attendance.AttendanceDate) == System.Data.Entity.DbFunctions.TruncateTime(model.LateDate) && row.Attendance.ApplicationKey == model.ApplicationsKey && (row.AttendanceTypeKey == (model.AttendanceTypeKey ?? row.AttendanceTypeKey))).ToList();

                    foreach (AttendanceDetail AttendanceDetail in AttendanceDetailList)
                    {
                        AttendanceDetail.StudentLateKey = StudentLateDetailModel.RowKey;
                        AttendanceDetail.AttendanceStatus = true;
                        AttendanceDetail.AttendanceStatusKey = DbConstants.AttendanceStatus.Late;
                        if (model.MarkHalfDay == true)
                        {
                            AttendanceDetail.AttendanceStatusKey = DbConstants.AttendanceStatus.HalfDay;
                        }
                        else
                        {
                            AttendanceDetail.AttendanceStatusKey = DbConstants.AttendanceStatus.Late;
                        }
                    }
                    if (model.AttendanceTypeKey != oldAttendanceTypeKey)
                    {
                        AttendanceDetailList = dbContext.AttendanceDetails.Where(row => System.Data.Entity.DbFunctions.TruncateTime(row.Attendance.AttendanceDate) == System.Data.Entity.DbFunctions.TruncateTime(model.LateDate) && row.Attendance.ApplicationKey == model.ApplicationsKey && model.AttendanceTypeKey != row.AttendanceTypeKey && oldAttendanceTypeKey == row.AttendanceTypeKey).ToList();
                        foreach (AttendanceDetail AttendanceDetail in AttendanceDetailList)
                        {
                            AttendanceDetail.StudentLateKey = null;
                            AttendanceDetail.AttendanceStatus = false;
                            AttendanceDetail.AttendanceStatusKey = DbConstants.AttendanceStatus.Absent;
                        }
                    }
                    dbContext.SaveChanges();
                    transaction.Commit();

                    model.Message = EduSuiteUIResources.Success;
                    model.IsSuccessful = true;
                    ActivityLog.CreateActivityLog(MenuConstants.StudentLate, ActionConstants.Edit, DbConstants.LogType.Info, model.RowKey, model.Message);

                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    model.Message = String.Format(EduSuiteUIResources.FailedToSaveErrorMessage, EduSuiteUIResources.StudentLate);
                    model.IsSuccessful = false;
                    ActivityLog.CreateActivityLog(MenuConstants.StudentLate, ActionConstants.Edit, DbConstants.LogType.Error, model.RowKey, ex.GetBaseException().Message);
                }
            }
            return model;
        }


        public StudentLateViewModel DeleteStudentLate(long? Id)
        {
            StudentLateViewModel model = new StudentLateViewModel();

            using (var transaction = dbContext.Database.BeginTransaction())
            {
                try
                {
                    StudentLateDetail StudentLateDetails = dbContext.StudentLateDetails.SingleOrDefault(x => x.RowKey == Id);

                    //var AttendanceTypeKeys = StudentLateDetails.AttendanceTypeKeys != null ? StudentLateDetails.AttendanceTypeKeys.Split(',').Select(Int16.Parse).ToList() : new List<short>();
                    //List<AttendanceDetail> AttendanceDetailList = dbContext.AttendanceDetails.Where(row => System.Data.Entity.DbFunctions.TruncateTime(row.Attendance.AttendanceDate) == System.Data.Entity.DbFunctions.TruncateTime(StudentLateDetails.LateDate) && row.Attendance.ApplicationKey == StudentLateDetails.ApplicationKey && AttendanceTypeKeys.Contains(row.AttendanceTypeKey)).ToList();


                    List<AttendanceDetail> AttendanceDetailList = dbContext.AttendanceDetails.Where(row => System.Data.Entity.DbFunctions.TruncateTime(row.Attendance.AttendanceDate) == System.Data.Entity.DbFunctions.TruncateTime(StudentLateDetails.LateDate) && row.Attendance.ApplicationKey == StudentLateDetails.ApplicationKey && (row.AttendanceTypeKey == (StudentLateDetails.AttendanceTypeKey ?? row.AttendanceTypeKey))).ToList();

                    foreach (AttendanceDetail AttendanceDetail in AttendanceDetailList)
                    {
                        AttendanceDetail.StudentLateKey = null;
                        AttendanceDetail.AttendanceStatus = false;
                        AttendanceDetail.AttendanceStatusKey = DbConstants.AttendanceStatus.Absent;
                    }

                    dbContext.StudentLateDetails.Remove(StudentLateDetails);


                    dbContext.SaveChanges();
                    transaction.Commit();
                    model.Message = EduSuiteUIResources.Success;
                    model.IsSuccessful = true;
                    ActivityLog.CreateActivityLog(MenuConstants.StudentLate, ActionConstants.Delete, DbConstants.LogType.Info, Id, model.Message);

                }
                catch (DbUpdateException ex)
                {
                    transaction.Rollback();
                    if (ex.GetBaseException().Message.ToUpper().Contains(EduSuiteUIResources.ForeignKeyError.ToUpper()))
                    {
                        model.Message = String.Format(EduSuiteUIResources.CantDeleteErrorMessage, EduSuiteUIResources.StudentLate);
                        model.IsSuccessful = false;
                        ActivityLog.CreateActivityLog(MenuConstants.StudentLate, ActionConstants.Delete, DbConstants.LogType.Debug, Id, ex.GetBaseException().Message);
                    }
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    model.Message = String.Format(EduSuiteUIResources.FailedToDeleteErrorMessage, EduSuiteUIResources.StudentLate);
                    model.IsSuccessful = false;
                    ActivityLog.CreateActivityLog(MenuConstants.StudentLate, ActionConstants.Delete, DbConstants.LogType.Error, Id, ex.GetBaseException().Message);
                }
            }
            return model;


        }


        public List<StudentLateViewModel> GetStudentLateDetails(long ApplicationKey)
        {
            try
            {
                long ClassDetailsKey = dbContext.Applications.Where(x => x.RowKey == ApplicationKey).Select(y => y.ClassDetailsKey ?? 0).FirstOrDefault();


                var applicationFeeList = (from es in dbContext.StudentLateDetails.Where(row => row.ApplicationKey == ApplicationKey && (row.ClassDetailsKey ?? 0) == ClassDetailsKey)
                                          orderby es.RowKey ascending
                                          select new StudentLateViewModel
                                          {
                                              RowKey = es.RowKey,
                                              LateDate = es.LateDate,
                                              LateMinutes = es.LateMinutes,
                                              Remarks = es.Remarks,
                                              AttachmentPath = es.AttachmentPath,
                                              ApplicationsKey = es.ApplicationKey,
                                          }).ToList();

                return applicationFeeList.ToList<StudentLateViewModel>();

            }
            catch (Exception ex)
            {
                ActivityLog.CreateActivityLog(MenuConstants.StudentLate, ActionConstants.View, DbConstants.LogType.Debug, ApplicationKey, ex.GetBaseException().Message);
                return new List<StudentLateViewModel>();
            }

        }

        public StudentLateViewModel GetLateMinuteById(short Id, DateTime Date, long ApplicationKey)
        {
            StudentLateViewModel model = new StudentLateViewModel();
            int? Lateminutes = 0;
            TimeSpan? startTime = dbContext.AttendanceTypes.Where(row => row.RowKey == Id).Select(row => row.StartTime).FirstOrDefault();
            if (startTime != null)
            {
                Lateminutes = (int)(DateTimeUTC.Now.TimeOfDay - (startTime ?? TimeSpan.Zero)).TotalMinutes;
            }

            if (dbContext.AttendanceDetails.Any(row => System.Data.Entity.DbFunctions.TruncateTime(row.Attendance.AttendanceDate) < System.Data.Entity.DbFunctions.TruncateTime(Date) && row.Attendance.ApplicationKey == ApplicationKey && row.AttendanceStatusKey == DbConstants.AttendanceStatus.Absent))
            {
                var AttendanceDetails = dbContext.AttendanceDetails.Where(row => System.Data.Entity.DbFunctions.TruncateTime(row.Attendance.AttendanceDate) < System.Data.Entity.DbFunctions.TruncateTime(Date) && row.Attendance.ApplicationKey == ApplicationKey && row.AttendanceStatusKey == DbConstants.AttendanceStatus.Absent).Select(x => new StudentLateViewModel
                    {
                        Message = x.Attendance.Application.StudentName + " is Absent in ",
                        LateDate = x.Attendance.AttendanceDate,
                        LateMinutes = Lateminutes,
                        IsSuccessful = false
                    }).FirstOrDefault();
                model = AttendanceDetails;
            }
            else
            {
                model.LateMinutes = Lateminutes;
                model.IsSuccessful = true;
            }
            return model;
        }

        private void FillNotificationDetail(StudentLateViewModel model)
        {
            NotificationTemplate notificationTemplateModel = dbContext.NotificationTemplates.SingleOrDefault(row => row.RowKey == DbConstants.NotificationTemplate.StudentLate);
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
