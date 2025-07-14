using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using CITS.EduSuite.Business.Extensions;
using CITS.EduSuite.Business.Interfaces;
using CITS.EduSuite.Business.Models.Resources;
using CITS.EduSuite.Business.Models.ViewModels;
using CITS.EduSuite.Data;

namespace CITS.EduSuite.Business.Services
{
    public class CourseCompletionCertificateService : ICourseCompletionCertificateService
    {
        private EduSuiteDatabase dbContext;

        public CourseCompletionCertificateService(EduSuiteDatabase objEduSuiteDatabase)
        {
            this.dbContext = objEduSuiteDatabase;
        }

        public List<dynamic> GetApplications(ApplicationViewModel model, out long TotalRecords)
        {
            try
            {

                List<dynamic> applicationList = new List<dynamic>();
                DbParameter TotalRecordsParam = null;

                if (model.SortBy != "")
                {
                    model.SortBy = model.SortBy + " " + model.SortOrder;
                }
                dbContext.LoadStoredProc("dbo.SP_StudentCourseCompleteCertificateDetails")
                    .WithSqlParam("@BranchKey", model.BranchKey)
                    .WithSqlParam("@BatchKey", model.BatchKey)
                    .WithSqlParam("@CourseKey", model.CourseKey)
                    .WithSqlParam("@UniversityKey", model.UniversityKey)
                    .WithSqlParam("@SearchText", model.ApplicantName.VerifyData())
                    .WithSqlParam("@SearchMobileNumber", model.MobileNumber.VerifyData())
                    .WithSqlParam("@PageIndex", model.PageIndex)
                    .WithSqlParam("@PageSize", model.PageSize)
                    .WithSqlParam("@SortBy", model.SortBy)
                    .WithSqlParam("@UserKey", DbConstants.User.UserKey)
                    .WithSqlParam("@TotalRecords", (dbParam) =>
                    {
                        dbParam.Direction = System.Data.ParameterDirection.Output;
                        dbParam.DbType = System.Data.DbType.Int64;
                        TotalRecordsParam = dbParam;
                    }).ExecuteStoredProc((handler) =>
                    {
                        applicationList = handler.ReadToDynamicList<dynamic>() as List<dynamic>;
                        //applicationList = handler.ReadToList<ApplicationViewModel>() as List<ApplicationViewModel>;
                    });
                TotalRecords = Convert.ToInt64((TotalRecordsParam.Value ?? 0));
                return applicationList;
            }
            catch (Exception ex)
            {
                TotalRecords = 0;
                ActivityLog.CreateActivityLog(MenuConstants.CourseCompletionCertificate, ActionConstants.View, DbConstants.LogType.Error, null, ex.GetBaseException().Message);
                return new List<dynamic>();
            }
        }

        public List<dynamic> FetchCourseCompleteCertificate(CourseCompletionCertificateViewModel model)
        {
            List<dynamic> FetchCourseCompleteCertificateDetailsList = new List<dynamic>();

            dbContext.LoadStoredProc("dbo.SP_FetchCourseCompletionCertificateDetails")
               .WithSqlParam("@ApplicationKey", model.ApplicationKey).ExecuteStoredProc((handler) =>
               {
                   FetchCourseCompleteCertificateDetailsList = handler.ReadToDynamicList<dynamic>() as List<dynamic>;

               });
            return FetchCourseCompleteCertificateDetailsList;
        }

        private IQueryable<ApplicationViewModel> SortApplications(IQueryable<ApplicationViewModel> Query, string SortName, string SortOrder)
        {

            string command = SortOrder == "desc" ? "OrderByDescending" : "OrderBy";
            var type = typeof(ApplicationViewModel);
            var property = type.GetProperty(SortName);
            var parameter = Expression.Parameter(type, "p");
            var propertyAccess = Expression.MakeMemberAccess(parameter, property);
            var orderByExpression = Expression.Lambda(propertyAccess, parameter);
            var resultExpression = Expression.Call(typeof(Queryable), command, new Type[] { type, property.PropertyType },
                                          Query.Expression, Expression.Quote(orderByExpression));
            return Query.Provider.CreateQuery<ApplicationViewModel>(resultExpression);

        }

        public CourseCompletionCertificateViewModel GetCourseCompletionCertificateById(CourseCompletionCertificateViewModel model)
        {
            try
            {
                CourseCompletionCertificateViewModel objmodel = new CourseCompletionCertificateViewModel();
                Application Applications = dbContext.Applications.Where(x => x.RowKey == model.ApplicationKey).FirstOrDefault();
                objmodel = dbContext.CourseCompletionCertificates.Where(x => x.ApplicationKey == model.RowKey).Select(row => new CourseCompletionCertificateViewModel
                {
                    RowKey = row.RowKey,
                    ApplicationKey = row.ApplicationKey ?? 0,
                    IssuedDate = row.IssuedDate,
                    IssuedBy = row.IssuedBy,
                    IsIssued = row.IsIssued,
                    Remarks = row.Remarks,

                }).FirstOrDefault();

                if (objmodel == null)
                {
                    objmodel = new CourseCompletionCertificateViewModel();
                }

                objmodel.ApplicationKey = model.ApplicationKey;
                objmodel.StudentMobile = Applications != null ? Applications.StudentMobile : null;
                objmodel.StudentEmail = Applications != null ? Applications.StudentEmail : null;

                FillNotificationDetail(model);
                return model;

            }
            catch (Exception ex)
            {
                ActivityLog.CreateActivityLog(MenuConstants.CourseCompletionCertificate, ActionConstants.View, DbConstants.LogType.Error, model.RowKey, ex.GetBaseException().Message);
                return new CourseCompletionCertificateViewModel();
            }
        }

        public CourseCompletionCertificateViewModel CreateCourseCompletionCertificate(CourseCompletionCertificateViewModel model)
        {
            using (var transaction = dbContext.Database.BeginTransaction())
            {
                try
                {

                    long Maxkey = dbContext.CourseCompletionCertificates.Select(x => x.RowKey).DefaultIfEmpty().Max();

                    CourseCompletionCertificate courseCompletionCertificate = new CourseCompletionCertificate();
                    courseCompletionCertificate.RowKey = ++Maxkey;
                    courseCompletionCertificate.IsIssued = model.IsIssued;
                    courseCompletionCertificate.IssuedBy = DbConstants.User.UserKey;
                    courseCompletionCertificate.IssuedDate = model.IssuedDate;
                    courseCompletionCertificate.Remarks = model.Remarks;
                    courseCompletionCertificate.ApplicationKey = model.ApplicationKey;

                    dbContext.CourseCompletionCertificates.Add(courseCompletionCertificate);

                    dbContext.SaveChanges();
                    transaction.Commit();
                    model.Message = EduSuiteUIResources.Success;
                    model.IsSuccessful = true;

                    ActivityLog.CreateActivityLog(MenuConstants.CourseCompletionCertificate, ActionConstants.Add, DbConstants.LogType.Info, model.ApplicationKey, model.Message);

                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    model.Message = String.Format(EduSuiteUIResources.FailedToSaveErrorMessage, EduSuiteUIResources.CourseCompletionCertificate);
                    model.IsSuccessful = false;
                    ActivityLog.CreateActivityLog(MenuConstants.CourseCompletionCertificate, ActionConstants.Add, DbConstants.LogType.Error, model.ApplicationKey, ex.GetBaseException().Message);

                }
                return model;
            }
        }

        public CourseCompletionCertificateViewModel UpdateCourseCompletionCertificate(CourseCompletionCertificateViewModel model)
        {
            using (var transaction = dbContext.Database.BeginTransaction())
            {
                try
                {


                    CourseCompletionCertificate courseCompletionCertificate = new CourseCompletionCertificate();
                    courseCompletionCertificate = dbContext.CourseCompletionCertificates.Where(x => x.RowKey == model.RowKey).FirstOrDefault();

                    courseCompletionCertificate.IsIssued = model.IsIssued;
                    courseCompletionCertificate.IssuedBy = DbConstants.User.UserKey;
                    courseCompletionCertificate.IssuedDate = model.IssuedDate;
                    courseCompletionCertificate.Remarks = model.Remarks;
                    courseCompletionCertificate.ApplicationKey = model.ApplicationKey;

                    dbContext.SaveChanges();
                    transaction.Commit();
                    model.Message = EduSuiteUIResources.Success;
                    model.IsSuccessful = true;

                    ActivityLog.CreateActivityLog(MenuConstants.CourseCompletionCertificate, ActionConstants.Add, DbConstants.LogType.Info, model.ApplicationKey, model.Message);

                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    model.Message = String.Format(EduSuiteUIResources.FailedToSaveErrorMessage, EduSuiteUIResources.CourseCompletionCertificate);
                    model.IsSuccessful = false;
                    ActivityLog.CreateActivityLog(MenuConstants.CourseCompletionCertificate, ActionConstants.Add, DbConstants.LogType.Error, model.ApplicationKey, ex.GetBaseException().Message);

                }
                return model;
            }
        }

        public CourseCompletionCertificateViewModel DeleteCourseCompletionCertificate(long RowKey)
        {
            CourseCompletionCertificateViewModel model = new CourseCompletionCertificateViewModel();
            using (var Transaction = dbContext.Database.BeginTransaction())
            {
                try
                {


                    CourseCompletionCertificate courseCompletionCertificate = dbContext.CourseCompletionCertificates.Where(x => x.RowKey == RowKey).FirstOrDefault();
                    dbContext.CourseCompletionCertificates.Remove(courseCompletionCertificate);

                    dbContext.SaveChanges();
                    Transaction.Commit();
                    model.Message = EduSuiteUIResources.Success;
                    model.IsSuccessful = true;
                    ActivityLog.CreateActivityLog(MenuConstants.CourseCompletionCertificate, ActionConstants.Delete, DbConstants.LogType.Info, RowKey, model.Message);

                }
                catch (DbUpdateException ex)
                {
                    Transaction.Rollback();
                    if (ex.GetBaseException().Message.ToUpper().Contains(EduSuiteUIResources.ForeignKeyError.ToUpper()))
                    {
                        model.Message = String.Format(EduSuiteUIResources.CantDeleteErrorMessage, EduSuiteUIResources.CourseCompletionCertificate);
                        model.IsSuccessful = false;
                        ActivityLog.CreateActivityLog(MenuConstants.CourseCompletionCertificate, ActionConstants.Delete, DbConstants.LogType.Error, RowKey, ex.GetBaseException().Message);
                    }
                }
                catch (Exception ex)
                {
                    Transaction.Rollback();
                    model.Message = String.Format(EduSuiteUIResources.FailedToDeleteErrorMessage, EduSuiteUIResources.CourseCompletionCertificate);
                    model.IsSuccessful = false;
                    ActivityLog.CreateActivityLog(MenuConstants.CourseCompletionCertificate, ActionConstants.Delete, DbConstants.LogType.Error, RowKey, ex.GetBaseException().Message);
                }
            }
            return model;


        }

        private void FillNotificationDetail(CourseCompletionCertificateViewModel model)
        {
            NotificationTemplate notificationTemplateModel = dbContext.NotificationTemplates.SingleOrDefault(row => row.RowKey == DbConstants.NotificationTemplate.CourseCompletionCertificate);
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
