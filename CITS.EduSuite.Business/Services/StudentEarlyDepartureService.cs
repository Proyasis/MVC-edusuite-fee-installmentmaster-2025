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
    public class StudentEarlyDepartureService : IStudentEarlyDepartureService
    {
        private EduSuiteDatabase dbContext;
        public StudentEarlyDepartureService(EduSuiteDatabase objDB)
        {
            this.dbContext = objDB;
        }

        public StudentEarlyDepartureViewModel GetStudentEarlyDepartureById(StudentEarlyDepartureViewModel model)
        {
            try
            {
                StudentEarlyDepartureViewModel objViewModel = new StudentEarlyDepartureViewModel();
                Application Applications = dbContext.Applications.Where(x => x.RowKey == model.ApplicationsKey).FirstOrDefault();

                objViewModel = dbContext.StudentEarlyDepartures.Where(x => x.RowKey == model.RowKey).Select(row => new StudentEarlyDepartureViewModel
                {
                    RowKey = row.RowKey,
                    EarlyDepartureDate = row.EarlyDepartureDate,
                    EarlyDepartureTime = row.EarlyDepartureTime,
                    Remarks = row.Remarks,
                    AttachmentPath = row.AttachmentPath,
                    ApplicationsKey = row.ApplicationKey,
                    AttendanceTypeKey = row.AttendanceTypeKey,
                    MarkHalfDay = row.MarkHalfDay
                }).FirstOrDefault();
                if (objViewModel == null)
                {


                    objViewModel = new StudentEarlyDepartureViewModel();
                    objViewModel.RowKey = model.RowKey;
                    objViewModel.ApplicationsKey = model.ApplicationsKey;

                }
                objViewModel.StudentMobile = Applications != null ? Applications.StudentMobile : null;
                objViewModel.StudentEmail = Applications != null ? Applications.StudentEmail : null;
                //objViewModel.StudentGuardianPhone = Applications != null ? Applications.StudentGuardianPhone : null;
                FillNotificationDetail(objViewModel);
                return objViewModel;
            }
            catch (Exception ex)
            {
                ActivityLog.CreateActivityLog(MenuConstants.StudentEarlyDeparture, ActionConstants.View, DbConstants.LogType.Debug, model.ApplicationsKey, ex.GetBaseException().Message);
                return new StudentEarlyDepartureViewModel();
            }
        }
        public StudentEarlyDepartureViewModel CreateStudentEarlyDepartureDate(StudentEarlyDepartureViewModel model)
        {
            using (var transaction = dbContext.Database.BeginTransaction())
            {
                try
                {
                    StudentEarlyDeparture StudentEarlyDepartureModel = new StudentEarlyDeparture();
                    long? MaxKey = dbContext.StudentEarlyDepartures.Select(x => x.RowKey).DefaultIfEmpty().Max();

                    StudentEarlyDepartureModel.RowKey = Convert.ToInt64(MaxKey + 1);
                    StudentEarlyDepartureModel.EarlyDepartureDate = model.EarlyDepartureDate;
                    StudentEarlyDepartureModel.EarlyDepartureTime = model.EarlyDepartureTime;
                    StudentEarlyDepartureModel.Remarks = model.Remarks;
                    StudentEarlyDepartureModel.AttachmentPath = model.AttachmentPath;
                    StudentEarlyDepartureModel.ApplicationKey = model.ApplicationsKey ?? 0;
                    StudentEarlyDepartureModel.ClassDetailsKey = dbContext.Applications.Where(x => x.RowKey == model.ApplicationsKey).Select(y => y.ClassDetailsKey).FirstOrDefault();
                    StudentEarlyDepartureModel.MarkHalfDay = model.MarkHalfDay;

                    dbContext.StudentEarlyDepartures.Add(StudentEarlyDepartureModel);

                    List<AttendanceDetail> AttendanceDetailList = dbContext.AttendanceDetails.Where(row => System.Data.Entity.DbFunctions.TruncateTime(row.Attendance.AttendanceDate) == System.Data.Entity.DbFunctions.TruncateTime(model.EarlyDepartureDate) && row.Attendance.ApplicationKey == model.ApplicationsKey && (row.AttendanceTypeKey == (model.AttendanceTypeKey ?? row.AttendanceTypeKey))).ToList();

                    foreach (AttendanceDetail AttendanceDetail in AttendanceDetailList)
                    {
                        if (model.MarkHalfDay == true)
                        {
                            AttendanceDetail.AttendanceStatusKey = DbConstants.AttendanceStatus.HalfDay;
                        }
                        else
                        {
                            AttendanceDetail.AttendanceStatusKey = DbConstants.AttendanceStatus.EarlyDeparture;
                        }
                    }

                    dbContext.SaveChanges();
                    transaction.Commit();
                    model.RowKey = StudentEarlyDepartureModel.RowKey;
                    model.Message = EduSuiteUIResources.Success;
                    model.IsSuccessful = true;
                    ActivityLog.CreateActivityLog(MenuConstants.StudentEarlyDeparture, ActionConstants.Add, DbConstants.LogType.Info, model.RowKey, model.Message);

                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    model.Message = String.Format(EduSuiteUIResources.FailedToSaveErrorMessage, EduSuiteUIResources.StudentEarlyDeparture);
                    model.IsSuccessful = false;
                    ActivityLog.CreateActivityLog(MenuConstants.StudentEarlyDeparture, ActionConstants.Add, DbConstants.LogType.Error, model.RowKey, ex.GetBaseException().Message);
                }
            }
            return model;

        }

        public StudentEarlyDepartureViewModel UpdateStudentEarlyDepartureDate(StudentEarlyDepartureViewModel model)
        {
            using (var transaction = dbContext.Database.BeginTransaction())
            {
                try
                {
                    StudentEarlyDeparture StudentEarlyDepartureModel = new StudentEarlyDeparture();

                    StudentEarlyDepartureModel = dbContext.StudentEarlyDepartures.Where(p => p.RowKey == model.RowKey).FirstOrDefault();
                    var oldAttendanceTypeKey = StudentEarlyDepartureModel.AttendanceTypeKey;

                    StudentEarlyDepartureModel.EarlyDepartureDate = model.EarlyDepartureDate;
                    StudentEarlyDepartureModel.EarlyDepartureTime = model.EarlyDepartureTime;
                    StudentEarlyDepartureModel.Remarks = model.Remarks;
                    StudentEarlyDepartureModel.AttachmentPath = model.AttachmentPath;
                    StudentEarlyDepartureModel.MarkHalfDay = model.MarkHalfDay;

                    List<AttendanceDetail> AttendanceDetailList = dbContext.AttendanceDetails.Where(row => System.Data.Entity.DbFunctions.TruncateTime(row.Attendance.AttendanceDate) == System.Data.Entity.DbFunctions.TruncateTime(model.EarlyDepartureDate) && row.Attendance.ApplicationKey == model.ApplicationsKey && (row.AttendanceTypeKey == (model.AttendanceTypeKey ?? row.AttendanceTypeKey))).ToList();

                    foreach (AttendanceDetail AttendanceDetail in AttendanceDetailList)
                    {
                        if (model.MarkHalfDay == true)
                        {
                            AttendanceDetail.AttendanceStatusKey = DbConstants.AttendanceStatus.HalfDay;
                        }
                        else
                        {
                            AttendanceDetail.AttendanceStatusKey = DbConstants.AttendanceStatus.EarlyDeparture;
                        }
                    }
                    if (model.AttendanceTypeKey != oldAttendanceTypeKey)
                    {
                        AttendanceDetailList = dbContext.AttendanceDetails.Where(row => System.Data.Entity.DbFunctions.TruncateTime(row.Attendance.AttendanceDate) == System.Data.Entity.DbFunctions.TruncateTime(model.EarlyDepartureDate) && row.Attendance.ApplicationKey == model.ApplicationsKey && model.AttendanceTypeKey != row.AttendanceTypeKey && oldAttendanceTypeKey == row.AttendanceTypeKey).ToList();
                        foreach (AttendanceDetail AttendanceDetail in AttendanceDetailList)
                        {
                            AttendanceDetail.AttendanceStatusKey = DbConstants.AttendanceStatus.Present;
                        }
                    }

                    dbContext.SaveChanges();
                    transaction.Commit();

                    model.Message = EduSuiteUIResources.Success;
                    model.IsSuccessful = true;
                    ActivityLog.CreateActivityLog(MenuConstants.StudentEarlyDeparture, ActionConstants.Edit, DbConstants.LogType.Info, model.RowKey, model.Message);

                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    model.Message = String.Format(EduSuiteUIResources.FailedToSaveErrorMessage, EduSuiteUIResources.StudentEarlyDeparture);
                    model.IsSuccessful = false;
                    ActivityLog.CreateActivityLog(MenuConstants.StudentEarlyDeparture, ActionConstants.Edit, DbConstants.LogType.Error, model.RowKey, ex.GetBaseException().Message);
                }
            }
            return model;
        }

        public StudentEarlyDepartureViewModel DeleteStudentEarlyDeparture(long? Id)
        {
            StudentEarlyDepartureViewModel model = new StudentEarlyDepartureViewModel();

            using (var transaction = dbContext.Database.BeginTransaction())
            {
                try
                {
                    StudentEarlyDeparture StudentEarlyDepartureModel = dbContext.StudentEarlyDepartures.SingleOrDefault(x => x.RowKey == Id);

                    List<AttendanceDetail> AttendanceDetailList = dbContext.AttendanceDetails.Where(row => System.Data.Entity.DbFunctions.TruncateTime(row.Attendance.AttendanceDate) == System.Data.Entity.DbFunctions.TruncateTime(StudentEarlyDepartureModel.EarlyDepartureDate) && row.Attendance.ApplicationKey == StudentEarlyDepartureModel.ApplicationKey && model.AttendanceTypeKey != row.AttendanceTypeKey && StudentEarlyDepartureModel.AttendanceTypeKey == row.AttendanceTypeKey).ToList();
                    foreach (AttendanceDetail AttendanceDetail in AttendanceDetailList)
                    {
                        AttendanceDetail.AttendanceStatusKey = DbConstants.AttendanceStatus.Present;
                    }
                    dbContext.StudentEarlyDepartures.Remove(StudentEarlyDepartureModel);


                    dbContext.SaveChanges();
                    transaction.Commit();
                    model.Message = EduSuiteUIResources.Success;
                    model.IsSuccessful = true;
                    ActivityLog.CreateActivityLog(MenuConstants.StudentEarlyDeparture, ActionConstants.Delete, DbConstants.LogType.Info, Id, model.Message);

                }
                catch (DbUpdateException ex)
                {
                    transaction.Rollback();
                    if (ex.GetBaseException().Message.ToUpper().Contains(EduSuiteUIResources.ForeignKeyError.ToUpper()))
                    {
                        model.Message = String.Format(EduSuiteUIResources.CantDeleteErrorMessage, EduSuiteUIResources.StudentEarlyDeparture);
                        model.IsSuccessful = false;
                        ActivityLog.CreateActivityLog(MenuConstants.StudentEarlyDeparture, ActionConstants.Delete, DbConstants.LogType.Debug, Id, ex.GetBaseException().Message);
                    }
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    model.Message = String.Format(EduSuiteUIResources.FailedToDeleteErrorMessage, EduSuiteUIResources.StudentEarlyDeparture);
                    model.IsSuccessful = false;
                    ActivityLog.CreateActivityLog(MenuConstants.StudentEarlyDeparture, ActionConstants.Delete, DbConstants.LogType.Error, Id, ex.GetBaseException().Message);
                }
            }
            return model;


        }

        public List<StudentEarlyDepartureViewModel> GetStudentEarlyDepartureDetails(long ApplicationKey)
        {
            try
            {
                long ClassDetailsKey = dbContext.Applications.Where(x => x.RowKey == ApplicationKey).Select(y => y.ClassDetailsKey ?? 0).FirstOrDefault();


                var applicationFeeList = (from es in dbContext.StudentEarlyDepartures.Where(row => row.ApplicationKey == ApplicationKey && (row.ClassDetailsKey ?? 0) == ClassDetailsKey)
                                          orderby es.RowKey ascending
                                          select new StudentEarlyDepartureViewModel
                                          {
                                              RowKey = es.RowKey,
                                              EarlyDepartureDate = es.EarlyDepartureDate,
                                              EarlyDepartureTime = es.EarlyDepartureTime,
                                              Remarks = es.Remarks,
                                              AttachmentPath = es.AttachmentPath,
                                              ApplicationsKey = es.ApplicationKey,
                                              MarkHalfDay = es.MarkHalfDay
                                          }).ToList();

                return applicationFeeList.ToList<StudentEarlyDepartureViewModel>();

            }
            catch (Exception ex)
            {
                ActivityLog.CreateActivityLog(MenuConstants.StudentEarlyDeparture, ActionConstants.View, DbConstants.LogType.Debug, ApplicationKey, ex.GetBaseException().Message);
                return new List<StudentEarlyDepartureViewModel>();
            }

        }
        private void FillNotificationDetail(StudentEarlyDepartureViewModel model)
        {
            NotificationTemplate notificationTemplateModel = dbContext.NotificationTemplates.SingleOrDefault(row => row.RowKey == DbConstants.NotificationTemplate.StudentEarlyDeparture);
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
