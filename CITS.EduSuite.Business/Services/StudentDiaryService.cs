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
    public class StudentDiaryService : IStudentDiaryService
    {
        private EduSuiteDatabase dbContext;
        public StudentDiaryService(EduSuiteDatabase objDB)
        {
            this.dbContext = objDB;
        }

        public StudentDiaryViewModel GetStudentDiaryById(StudentDiaryViewModel model)
        {
            try
            {
                StudentDiaryViewModel objViewModel = new StudentDiaryViewModel();
                Application Applications = dbContext.Applications.Where(x => x.RowKey == model.ApplicationsKey).FirstOrDefault();
                objViewModel = dbContext.StudentDiaries.Where(x => x.RowKey == model.RowKey).Select(row => new StudentDiaryViewModel
                {
                    RowKey = row.RowKey,
                    StudentDiaryDate = row.StudentDiaryDate,
                    Subject = row.Subject,
                    Remarks = row.Remarks,
                    AttachmentPath = row.AttachmentPath,
                    ApplicationsKey = row.ApplicationKey,
                }).FirstOrDefault();
                if (objViewModel == null)
                {
                    objViewModel = new StudentDiaryViewModel();
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
                ActivityLog.CreateActivityLog(MenuConstants.StudentDiary, ActionConstants.View, DbConstants.LogType.Debug, model.ApplicationsKey, ex.GetBaseException().Message);
                return new StudentDiaryViewModel();
            }
        }
        public StudentDiaryViewModel CreateStudentDiaryDate(StudentDiaryViewModel model)
        {
            using (var transaction = dbContext.Database.BeginTransaction())
            {
                try
                {



                    StudentDiary StudentDiaryModel = new StudentDiary();
                    long? MaxKey = dbContext.StudentDiaries.Select(x => x.RowKey).DefaultIfEmpty().Max();

                    StudentDiaryModel.RowKey = Convert.ToInt64(MaxKey + 1);
                    StudentDiaryModel.StudentDiaryDate = model.StudentDiaryDate;
                    StudentDiaryModel.Subject = model.Subject;
                    StudentDiaryModel.Remarks = model.Remarks;
                    StudentDiaryModel.AttachmentPath = model.AttachmentPath;
                    StudentDiaryModel.ApplicationKey = model.ApplicationsKey ?? 0;
                    StudentDiaryModel.ClassDetailsKey = dbContext.Applications.Where(x => x.RowKey == model.ApplicationsKey).Select(y => y.ClassDetailsKey).FirstOrDefault();

                    dbContext.StudentDiaries.Add(StudentDiaryModel);


                    dbContext.SaveChanges();
                    transaction.Commit();
                    model.RowKey = StudentDiaryModel.RowKey;
                    model.Message = EduSuiteUIResources.Success;
                    model.IsSuccessful = true;
                    ActivityLog.CreateActivityLog(MenuConstants.StudentDiary, ActionConstants.Add, DbConstants.LogType.Info, model.RowKey, model.Message);

                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    model.Message = String.Format(EduSuiteUIResources.FailedToSaveErrorMessage, EduSuiteUIResources.StudentDiary);
                    model.IsSuccessful = false;
                    ActivityLog.CreateActivityLog(MenuConstants.StudentDiary, ActionConstants.Add, DbConstants.LogType.Error, model.RowKey, ex.GetBaseException().Message);
                }
            }
            return model;

        }

        public StudentDiaryViewModel UpdateStudentDiaryDate(StudentDiaryViewModel model)
        {
            using (var transaction = dbContext.Database.BeginTransaction())
            {
                try
                {
                    StudentDiary StudentDiaryModel = new StudentDiary();

                    StudentDiaryModel = dbContext.StudentDiaries.Where(p => p.RowKey == model.RowKey).FirstOrDefault();
                    StudentDiaryModel.StudentDiaryDate = model.StudentDiaryDate;
                    StudentDiaryModel.Subject = model.Subject;
                    StudentDiaryModel.Remarks = model.Remarks;
                    StudentDiaryModel.AttachmentPath = model.AttachmentPath;


                    dbContext.SaveChanges();
                    transaction.Commit();

                    model.Message = EduSuiteUIResources.Success;
                    model.IsSuccessful = true;
                    ActivityLog.CreateActivityLog(MenuConstants.StudentDiary, ActionConstants.Edit, DbConstants.LogType.Info, model.RowKey, model.Message);

                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    model.Message = String.Format(EduSuiteUIResources.FailedToSaveErrorMessage, EduSuiteUIResources.StudentDiary);
                    model.IsSuccessful = false;
                    ActivityLog.CreateActivityLog(MenuConstants.StudentDiary, ActionConstants.Edit, DbConstants.LogType.Error, model.RowKey, ex.GetBaseException().Message);
                }
            }
            return model;
        }

        public StudentDiaryViewModel DeleteStudentDiary(long? Id)
        {
            StudentDiaryViewModel model = new StudentDiaryViewModel();

            using (var transaction = dbContext.Database.BeginTransaction())
            {
                try
                {
                    StudentDiary StudentDiaryModel = dbContext.StudentDiaries.SingleOrDefault(x => x.RowKey == Id);


                    dbContext.StudentDiaries.Remove(StudentDiaryModel);


                    dbContext.SaveChanges();
                    transaction.Commit();
                    model.Message = EduSuiteUIResources.Success;
                    model.IsSuccessful = true;
                    ActivityLog.CreateActivityLog(MenuConstants.StudentDiary, ActionConstants.Delete, DbConstants.LogType.Info, Id, model.Message);

                }
                catch (DbUpdateException ex)
                {
                    transaction.Rollback();
                    if (ex.GetBaseException().Message.ToUpper().Contains(EduSuiteUIResources.ForeignKeyError.ToUpper()))
                    {
                        model.Message = String.Format(EduSuiteUIResources.CantDeleteErrorMessage, EduSuiteUIResources.StudentDiary);
                        model.IsSuccessful = false;
                        ActivityLog.CreateActivityLog(MenuConstants.StudentDiary, ActionConstants.Delete, DbConstants.LogType.Debug, Id, ex.GetBaseException().Message);
                    }
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    model.Message = String.Format(EduSuiteUIResources.FailedToDeleteErrorMessage, EduSuiteUIResources.StudentDiary);
                    model.IsSuccessful = false;
                    ActivityLog.CreateActivityLog(MenuConstants.StudentDiary, ActionConstants.Delete, DbConstants.LogType.Error, Id, ex.GetBaseException().Message);
                }
            }
            return model;


        }

        public List<StudentDiaryViewModel> GetStudentDiaryDetails(long ApplicationKey)
        {
            try
            {
                long ClassDetailsKey = dbContext.Applications.Where(x => x.RowKey == ApplicationKey).Select(y => y.ClassDetailsKey ?? 0).FirstOrDefault();

                var applicationFeeList = (from es in dbContext.StudentDiaries.Where(row => row.ApplicationKey == ApplicationKey && (row.ClassDetailsKey ?? 0) == ClassDetailsKey)
                                          orderby es.RowKey ascending
                                          select new StudentDiaryViewModel
                                          {
                                              RowKey = es.RowKey,
                                              StudentDiaryDate = es.StudentDiaryDate,
                                              Subject = es.Subject,
                                              Remarks = es.Remarks,
                                              AttachmentPath = es.AttachmentPath,
                                              ApplicationsKey = es.ApplicationKey
                                          }).ToList();

                return applicationFeeList.ToList<StudentDiaryViewModel>();

            }
            catch (Exception ex)
            {
                ActivityLog.CreateActivityLog(MenuConstants.StudentDiary, ActionConstants.View, DbConstants.LogType.Debug, ApplicationKey, ex.GetBaseException().Message);
                return new List<StudentDiaryViewModel>();
            }

        }
        private void FillNotificationDetail(StudentDiaryViewModel model)
        {
            NotificationTemplate notificationTemplateModel = dbContext.NotificationTemplates.SingleOrDefault(row => row.RowKey == DbConstants.NotificationTemplate.StudentDiary);
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
