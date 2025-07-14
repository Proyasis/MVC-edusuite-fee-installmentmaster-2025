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
    public class StudentAbscondersService : IStudentAbscondersService
    {
        private EduSuiteDatabase dbContext;
        public StudentAbscondersService(EduSuiteDatabase objDB)
        {
            this.dbContext = objDB;
        }

        public StudentAbscondersViewModel GetStudentAbscondersById(StudentAbscondersViewModel model)
        {
            try
            {
                StudentAbscondersViewModel objViewModel = new StudentAbscondersViewModel();
                Application Applications = dbContext.Applications.Where(x => x.RowKey == model.ApplicationsKey).FirstOrDefault();

                objViewModel = dbContext.StudentAbsconders.Where(x => x.RowKey == model.RowKey).Select(row => new StudentAbscondersViewModel
                {
                    RowKey = row.RowKey,
                    AbscondersDate = row.AbscondersDate,
                    IsAbsconders = row.IsAbsconders ?? false,
                    Remarks = row.Remarks,
                    AttachmentPath = row.AttachmentPath,
                    ApplicationsKey = row.ApplicationKey,
                    AttendanceTypeKey = row.AttendanceTypeKey,
                    MarkHalfDay = row.MarkHalfDay
                }).FirstOrDefault();
                if (objViewModel == null)
                {


                    objViewModel = new StudentAbscondersViewModel();
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
                ActivityLog.CreateActivityLog(MenuConstants.StudentAbsconders, ActionConstants.View, DbConstants.LogType.Debug, model.ApplicationsKey, ex.GetBaseException().Message);
                return new StudentAbscondersViewModel();
            }
        }
        public StudentAbscondersViewModel CreateStudentAbscondersDate(StudentAbscondersViewModel model)
        {
            using (var transaction = dbContext.Database.BeginTransaction())
            {
                try
                {
                    StudentAbsconder StudentAbsconderModel = new StudentAbsconder();
                    long? MaxKey = dbContext.StudentAbsconders.Select(x => x.RowKey).DefaultIfEmpty().Max();

                    StudentAbsconderModel.RowKey = Convert.ToInt64(MaxKey + 1);
                    StudentAbsconderModel.AbscondersDate = model.AbscondersDate;
                    StudentAbsconderModel.IsAbsconders = model.IsAbsconders;
                    StudentAbsconderModel.Remarks = model.Remarks;
                    StudentAbsconderModel.AttachmentPath = model.AttachmentPath;
                    StudentAbsconderModel.ApplicationKey = model.ApplicationsKey ?? 0;
                    StudentAbsconderModel.AttendanceTypeKey = model.AttendanceTypeKey;
                    StudentAbsconderModel.MarkHalfDay = model.MarkHalfDay;
                    StudentAbsconderModel.ClassDetailsKey = dbContext.Applications.Where(x => x.RowKey == model.ApplicationsKey).Select(y => y.ClassDetailsKey).FirstOrDefault();

                    List<AttendanceDetail> AttendanceDetailList = dbContext.AttendanceDetails.Where(row => System.Data.Entity.DbFunctions.TruncateTime(row.Attendance.AttendanceDate) == System.Data.Entity.DbFunctions.TruncateTime(model.AbscondersDate) && row.Attendance.ApplicationKey == model.ApplicationsKey && (row.AttendanceTypeKey == (model.AttendanceTypeKey ?? row.AttendanceTypeKey))).ToList();

                    foreach (AttendanceDetail AttendanceDetail in AttendanceDetailList)
                    {
                        if (model.MarkHalfDay == true)
                        {
                            AttendanceDetail.AttendanceStatusKey = DbConstants.AttendanceStatus.HalfDay;
                        }
                        else
                        {
                            AttendanceDetail.AttendanceStatusKey = DbConstants.AttendanceStatus.Absconders;
                        }
                       
                    }
                   
                   

                    dbContext.StudentAbsconders.Add(StudentAbsconderModel);


                    dbContext.SaveChanges();
                    transaction.Commit();
                    model.RowKey = StudentAbsconderModel.RowKey;
                    model.Message = EduSuiteUIResources.Success;
                    model.IsSuccessful = true;
                    ActivityLog.CreateActivityLog(MenuConstants.StudentAbsconders, ActionConstants.Add, DbConstants.LogType.Info, model.RowKey, model.Message);

                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    model.Message = String.Format(EduSuiteUIResources.FailedToSaveErrorMessage, EduSuiteUIResources.StudentAbsconders);
                    model.IsSuccessful = false;
                    ActivityLog.CreateActivityLog(MenuConstants.StudentAbsconders, ActionConstants.Add, DbConstants.LogType.Error, model.RowKey, ex.GetBaseException().Message);
                }
            }
            return model;

        }

        public StudentAbscondersViewModel UpdateStudentAbscondersDate(StudentAbscondersViewModel model)
        {
            using (var transaction = dbContext.Database.BeginTransaction())
            {
                try
                {
                    StudentAbsconder StudentAbscondersModel = new StudentAbsconder();

                    StudentAbscondersModel = dbContext.StudentAbsconders.Where(p => p.RowKey == model.RowKey).FirstOrDefault();
                    var oldAttendanceTypeKey = StudentAbscondersModel.AttendanceTypeKey;

                    StudentAbscondersModel.AbscondersDate = model.AbscondersDate;
                    StudentAbscondersModel.IsAbsconders = model.IsAbsconders;
                    StudentAbscondersModel.Remarks = model.Remarks;
                    StudentAbscondersModel.AttachmentPath = model.AttachmentPath;
                    StudentAbscondersModel.AttendanceTypeKey = model.AttendanceTypeKey;
                    StudentAbscondersModel.MarkHalfDay = model.MarkHalfDay;
                    List<AttendanceDetail> AttendanceDetailList = dbContext.AttendanceDetails.Where(row => System.Data.Entity.DbFunctions.TruncateTime(row.Attendance.AttendanceDate) == System.Data.Entity.DbFunctions.TruncateTime(model.AbscondersDate) && row.Attendance.ApplicationKey == model.ApplicationsKey && (row.AttendanceTypeKey == (model.AttendanceTypeKey ?? row.AttendanceTypeKey))).ToList();

                    foreach (AttendanceDetail AttendanceDetail in AttendanceDetailList)
                    {                       
                        if (model.MarkHalfDay == true)
                        {
                            AttendanceDetail.AttendanceStatusKey = DbConstants.AttendanceStatus.HalfDay;
                        }
                        else
                        {
                            AttendanceDetail.AttendanceStatusKey = DbConstants.AttendanceStatus.Absconders;
                        }
                    }
                    if (model.AttendanceTypeKey != oldAttendanceTypeKey)
                    {
                        AttendanceDetailList = dbContext.AttendanceDetails.Where(row => System.Data.Entity.DbFunctions.TruncateTime(row.Attendance.AttendanceDate) == System.Data.Entity.DbFunctions.TruncateTime(model.AbscondersDate) && row.Attendance.ApplicationKey == model.ApplicationsKey && model.AttendanceTypeKey != row.AttendanceTypeKey && oldAttendanceTypeKey == row.AttendanceTypeKey).ToList();
                        foreach (AttendanceDetail AttendanceDetail in AttendanceDetailList)
                        {
                            AttendanceDetail.AttendanceStatusKey = DbConstants.AttendanceStatus.Present;
                        }
                    }
                    dbContext.SaveChanges();
                    transaction.Commit();

                    model.Message = EduSuiteUIResources.Success;
                    model.IsSuccessful = true;
                    ActivityLog.CreateActivityLog(MenuConstants.StudentAbsconders, ActionConstants.Edit, DbConstants.LogType.Info, model.RowKey, model.Message);

                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    model.Message = String.Format(EduSuiteUIResources.FailedToSaveErrorMessage, EduSuiteUIResources.StudentAbsconders);
                    model.IsSuccessful = false;
                    ActivityLog.CreateActivityLog(MenuConstants.StudentAbsconders, ActionConstants.Edit, DbConstants.LogType.Error, model.RowKey, ex.GetBaseException().Message);
                }
            }
            return model;
        }

        public StudentAbscondersViewModel DeleteStudentAbsconders(long? Id)
        {
            StudentAbscondersViewModel model = new StudentAbscondersViewModel();

            using (var transaction = dbContext.Database.BeginTransaction())
            {
                try
                {
                    StudentAbsconder StudentAbscondersModel = dbContext.StudentAbsconders.SingleOrDefault(x => x.RowKey == Id);

                    List<AttendanceDetail> AttendanceDetailList = dbContext.AttendanceDetails.Where(row => System.Data.Entity.DbFunctions.TruncateTime(row.Attendance.AttendanceDate) == System.Data.Entity.DbFunctions.TruncateTime(StudentAbscondersModel.AbscondersDate) && row.Attendance.ApplicationKey == StudentAbscondersModel.ApplicationKey && (row.AttendanceTypeKey == (StudentAbscondersModel.AttendanceTypeKey ?? row.AttendanceTypeKey))).ToList();

                    foreach (AttendanceDetail AttendanceDetail in AttendanceDetailList)
                    {
                        AttendanceDetail.AttendanceStatusKey = DbConstants.AttendanceStatus.Present;
                    }
                    dbContext.StudentAbsconders.Remove(StudentAbscondersModel);


                    dbContext.SaveChanges();
                    transaction.Commit();
                    model.Message = EduSuiteUIResources.Success;
                    model.IsSuccessful = true;
                    ActivityLog.CreateActivityLog(MenuConstants.StudentAbsconders, ActionConstants.Delete, DbConstants.LogType.Info, Id, model.Message);

                }
                catch (DbUpdateException ex)
                {
                    transaction.Rollback();
                    if (ex.GetBaseException().Message.ToUpper().Contains(EduSuiteUIResources.ForeignKeyError.ToUpper()))
                    {
                        model.Message = String.Format(EduSuiteUIResources.CantDeleteErrorMessage, EduSuiteUIResources.StudentAbsconders);
                        model.IsSuccessful = false;
                        ActivityLog.CreateActivityLog(MenuConstants.StudentAbsconders, ActionConstants.Delete, DbConstants.LogType.Debug, Id, ex.GetBaseException().Message);
                    }
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    model.Message = String.Format(EduSuiteUIResources.FailedToDeleteErrorMessage, EduSuiteUIResources.StudentAbsconders);
                    model.IsSuccessful = false;
                    ActivityLog.CreateActivityLog(MenuConstants.StudentAbsconders, ActionConstants.Delete, DbConstants.LogType.Error, Id, ex.GetBaseException().Message);
                }
            }
            return model;


        }

        public StudentAbscondersViewModel UpdateStatusStudentAbsconders(long? Id)
        {
            StudentAbscondersViewModel model = new StudentAbscondersViewModel();

            using (var transaction = dbContext.Database.BeginTransaction())
            {
                try
                {
                    StudentAbsconder StudentAbscondersModel = new StudentAbsconder();

                    StudentAbscondersModel = dbContext.StudentAbsconders.Where(p => p.RowKey == Id).FirstOrDefault();

                    StudentAbscondersModel.IsAbsconders = true;


                    dbContext.SaveChanges();
                    transaction.Commit();
                    model.Message = EduSuiteUIResources.Success;
                    model.IsSuccessful = true;
                    ActivityLog.CreateActivityLog(MenuConstants.StudentAbsconders, ActionConstants.Edit, DbConstants.LogType.Info, Id, model.Message);

                }
                catch (DbUpdateException ex)
                {
                    transaction.Rollback();
                    if (ex.GetBaseException().Message.ToUpper().Contains(EduSuiteUIResources.ForeignKeyError.ToUpper()))
                    {
                        model.Message = String.Format(EduSuiteUIResources.FailedToSaveErrorMessage, EduSuiteUIResources.StudentAbsconders);
                        model.IsSuccessful = false;
                        ActivityLog.CreateActivityLog(MenuConstants.StudentAbsconders, ActionConstants.Edit, DbConstants.LogType.Debug, Id, ex.GetBaseException().Message);
                    }
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    model.Message = String.Format(EduSuiteUIResources.FailedToSaveErrorMessage, EduSuiteUIResources.StudentAbsconders);
                    model.IsSuccessful = false;
                    ActivityLog.CreateActivityLog(MenuConstants.StudentAbsconders, ActionConstants.Delete, DbConstants.LogType.Error, Id, ex.GetBaseException().Message);
                }
            }
            return model;


        }

        public List<StudentAbscondersViewModel> GetStudentAbscondersDetails(long ApplicationKey)
        {
            try
            {
                long ClassDetailsKey = dbContext.Applications.Where(x => x.RowKey == ApplicationKey).Select(y => y.ClassDetailsKey ?? 0).FirstOrDefault();

                var applicationFeeList = (from es in dbContext.StudentAbsconders.Where(row => row.ApplicationKey == ApplicationKey && (row.ClassDetailsKey ?? 0) == ClassDetailsKey)
                                          orderby es.RowKey ascending
                                          select new StudentAbscondersViewModel
                                          {
                                              RowKey = es.RowKey,
                                              AbscondersDate = es.AbscondersDate,
                                              IsAbsconders = es.IsAbsconders ?? false,
                                              Remarks = es.Remarks,
                                              AttachmentPath = es.AttachmentPath,
                                              ApplicationsKey = es.ApplicationKey,
                                              MarkHalfDay = es.MarkHalfDay
                                          }).ToList();

                return applicationFeeList.ToList<StudentAbscondersViewModel>();

            }
            catch (Exception ex)
            {
                ActivityLog.CreateActivityLog(MenuConstants.StudentAbsconders, ActionConstants.View, DbConstants.LogType.Debug, ApplicationKey, ex.GetBaseException().Message);
                return new List<StudentAbscondersViewModel>();
            }

        }
        private void FillNotificationDetail(StudentAbscondersViewModel model)
        {
            NotificationTemplate notificationTemplateModel = dbContext.NotificationTemplates.SingleOrDefault(row => row.RowKey == DbConstants.NotificationTemplate.StudentAbsconders);
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
