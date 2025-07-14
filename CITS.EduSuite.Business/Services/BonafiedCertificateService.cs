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
   public class BonafiedCertificateService:IBonafiedCertificateService
    {
        private EduSuiteDatabase dbContext;

        public BonafiedCertificateService(EduSuiteDatabase objEduSuiteDatabase)
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
                dbContext.LoadStoredProc("dbo.SP_StudentBonafiedCertificateDetails")
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
                ActivityLog.CreateActivityLog(MenuConstants.BonafiedCertificate, ActionConstants.View, DbConstants.LogType.Error, null, ex.GetBaseException().Message);
                return new List<dynamic>();
            }
        }

        public List<dynamic> FetchBonafiedCertificate(BonafiedCertificateViewModel model)
        {
            List<dynamic> FetchBonafiedCertificateDetailsList = new List<dynamic>();

            dbContext.LoadStoredProc("dbo.SP_FetchBonafiedCertificateDetails")
               .WithSqlParam("@ApplicationKey", model.ApplicationKey).ExecuteStoredProc((handler) =>
               {
                   FetchBonafiedCertificateDetailsList = handler.ReadToDynamicList<dynamic>() as List<dynamic>;

               });
            return FetchBonafiedCertificateDetailsList;
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

        public BonafiedCertificateViewModel GetBonafiedCertificateById(BonafiedCertificateViewModel model)
        {
            try
            {
                BonafiedCertificateViewModel objmodel = new BonafiedCertificateViewModel();
                Application Applications = dbContext.Applications.Where(x => x.RowKey == model.ApplicationKey).FirstOrDefault();
                objmodel = dbContext.BonafiedCertificates.Where(x => x.RowKey == model.RowKey).Select(row => new BonafiedCertificateViewModel
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
                    objmodel = new BonafiedCertificateViewModel();
                }

                objmodel.ApplicationKey = model.ApplicationKey;
                objmodel.StudentMobile = Applications != null ? Applications.StudentMobile : null;
                objmodel.StudentEmail = Applications != null ? Applications.StudentEmail : null;

                FillNotificationDetail(objmodel);
                return objmodel;

            }
            catch (Exception ex)
            {
                ActivityLog.CreateActivityLog(MenuConstants.BonafiedCertificate, ActionConstants.View, DbConstants.LogType.Error, model.RowKey, ex.GetBaseException().Message);
                return new BonafiedCertificateViewModel();
            }
        }

        public BonafiedCertificateViewModel CreateBonafiedCertificate(BonafiedCertificateViewModel model)
        {
            using (var transaction = dbContext.Database.BeginTransaction())
            {
                try
                {

                    long Maxkey = dbContext.BonafiedCertificates.Select(x => x.RowKey).DefaultIfEmpty().Max();

                    BonafiedCertificate BonafiedCertificate = new BonafiedCertificate();
                    BonafiedCertificate.RowKey = ++Maxkey;
                    BonafiedCertificate.IsIssued = model.IsIssued;
                    BonafiedCertificate.IssuedBy = DbConstants.User.UserKey;
                    BonafiedCertificate.IssuedDate = model.IssuedDate;
                    BonafiedCertificate.Remarks = model.Remarks;
                    BonafiedCertificate.ApplicationKey = model.ApplicationKey;

                    dbContext.BonafiedCertificates.Add(BonafiedCertificate);

                    dbContext.SaveChanges();
                    transaction.Commit();
                    model.Message = EduSuiteUIResources.Success;
                    model.IsSuccessful = true;

                    ActivityLog.CreateActivityLog(MenuConstants.BonafiedCertificate, ActionConstants.Add, DbConstants.LogType.Info, model.ApplicationKey, model.Message);

                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    model.Message = String.Format(EduSuiteUIResources.FailedToSaveErrorMessage, EduSuiteUIResources.BonafiedCertificate);
                    model.IsSuccessful = false;
                    ActivityLog.CreateActivityLog(MenuConstants.BonafiedCertificate, ActionConstants.Add, DbConstants.LogType.Error, model.ApplicationKey, ex.GetBaseException().Message);

                }
                return model;
            }
        }

        public BonafiedCertificateViewModel UpdateBonafiedCertificate(BonafiedCertificateViewModel model)
        {
            using (var transaction = dbContext.Database.BeginTransaction())
            {
                try
                {


                    BonafiedCertificate BonafiedCertificate = new BonafiedCertificate();
                    BonafiedCertificate = dbContext.BonafiedCertificates.Where(x => x.RowKey == model.RowKey).FirstOrDefault();

                    BonafiedCertificate.IsIssued = model.IsIssued;
                    BonafiedCertificate.IssuedBy = DbConstants.User.UserKey;
                    BonafiedCertificate.IssuedDate = model.IssuedDate;
                    BonafiedCertificate.Remarks = model.Remarks;
                    BonafiedCertificate.ApplicationKey = model.ApplicationKey;

                    dbContext.SaveChanges();
                    transaction.Commit();
                    model.Message = EduSuiteUIResources.Success;
                    model.IsSuccessful = true;

                    ActivityLog.CreateActivityLog(MenuConstants.BonafiedCertificate, ActionConstants.Add, DbConstants.LogType.Info, model.ApplicationKey, model.Message);

                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    model.Message = String.Format(EduSuiteUIResources.FailedToSaveErrorMessage, EduSuiteUIResources.BonafiedCertificate);
                    model.IsSuccessful = false;
                    ActivityLog.CreateActivityLog(MenuConstants.BonafiedCertificate, ActionConstants.Add, DbConstants.LogType.Error, model.ApplicationKey, ex.GetBaseException().Message);

                }
                return model;
            }
        }

        public BonafiedCertificateViewModel DeleteBonafiedCertificate(long RowKey)
        {
            BonafiedCertificateViewModel model = new BonafiedCertificateViewModel();
            using (var Transaction = dbContext.Database.BeginTransaction())
            {
                try
                {


                    BonafiedCertificate BonafiedCertificate = dbContext.BonafiedCertificates.Where(x => x.RowKey == RowKey).FirstOrDefault();
                    dbContext.BonafiedCertificates.Remove(BonafiedCertificate);

                    dbContext.SaveChanges();
                    Transaction.Commit();
                    model.Message = EduSuiteUIResources.Success;
                    model.IsSuccessful = true;
                    ActivityLog.CreateActivityLog(MenuConstants.BonafiedCertificate, ActionConstants.Delete, DbConstants.LogType.Info, RowKey, model.Message);

                }
                catch (DbUpdateException ex)
                {
                    Transaction.Rollback();
                    if (ex.GetBaseException().Message.ToUpper().Contains(EduSuiteUIResources.ForeignKeyError.ToUpper()))
                    {
                        model.Message = String.Format(EduSuiteUIResources.CantDeleteErrorMessage, EduSuiteUIResources.BonafiedCertificate);
                        model.IsSuccessful = false;
                        ActivityLog.CreateActivityLog(MenuConstants.BonafiedCertificate, ActionConstants.Delete, DbConstants.LogType.Error, RowKey, ex.GetBaseException().Message);
                    }
                }
                catch (Exception ex)
                {
                    Transaction.Rollback();
                    model.Message = String.Format(EduSuiteUIResources.FailedToDeleteErrorMessage, EduSuiteUIResources.BonafiedCertificate);
                    model.IsSuccessful = false;
                    ActivityLog.CreateActivityLog(MenuConstants.BonafiedCertificate, ActionConstants.Delete, DbConstants.LogType.Error, RowKey, ex.GetBaseException().Message);
                }
            }
            return model;


        }

        private void FillNotificationDetail(BonafiedCertificateViewModel model)
        {
            NotificationTemplate notificationTemplateModel = dbContext.NotificationTemplates.SingleOrDefault(row => row.RowKey == DbConstants.NotificationTemplate.BonafiedCertificate);
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
