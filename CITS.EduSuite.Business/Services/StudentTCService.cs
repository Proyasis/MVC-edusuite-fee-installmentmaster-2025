using CITS.EduSuite.Business.Extensions;
using CITS.EduSuite.Business.Interfaces;
using CITS.EduSuite.Business.Models.Resources;
using CITS.EduSuite.Business.Models.ViewModels;
using CITS.EduSuite.Data;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace CITS.EduSuite.Business.Services
{
    public class StudentTCService : IStudentTCService
    {
        private EduSuiteDatabase dbContext;

        public StudentTCService(EduSuiteDatabase db)
        {
            this.dbContext = db;
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
                dbContext.LoadStoredProc("dbo.SP_StudentTCDetails")
                    .WithSqlParam("@BranchKey", model.BranchKey)
                    .WithSqlParam("@BatchKey", model.BatchKey)
                    .WithSqlParam("@CourseKey", model.CourseKey)
                    .WithSqlParam("@UniversityKey", model.UniversityKey)
                    .WithSqlParam("@SearchText", model.ApplicantName.VerifyData())
                    .WithSqlParam("@SearchMobileNumber", model.MobileNumber.VerifyData())
                    .WithSqlParam("@PageIndex", model.PageIndex)
                    .WithSqlParam("@PageSize", model.PageSize)
                    .WithSqlParam("@UserKey", DbConstants.User.UserKey)
                    .WithSqlParam("@SortBy", model.SortBy)
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
                ActivityLog.CreateActivityLog(MenuConstants.StudentTC, ActionConstants.View, DbConstants.LogType.Error, null, ex.GetBaseException().Message);
                return new List<dynamic>();
            }
        }

        public List<dynamic> FetchTCDetails(StudentTCViewModel model)
        {
            List<dynamic> FetchTCDetailsList = new List<dynamic>();

            dbContext.LoadStoredProc("dbo.SP_FetchTCDetails")
               .WithSqlParam("@ApplicationKey", model.ApplicationKey).ExecuteStoredProc((handler) =>
               {
                   FetchTCDetailsList = handler.ReadToDynamicList<dynamic>() as List<dynamic>;

               });
            return FetchTCDetailsList;
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

        public StudentTCViewModel GetStudentTcById(long ApplicationKey)
        {
            try
            {
                StudentTCViewModel model = new StudentTCViewModel();
                Application Applications = dbContext.Applications.Where(x => x.RowKey == ApplicationKey).FirstOrDefault();
                model = dbContext.StudentTCMasters.Where(x => x.ApplicationKey == ApplicationKey).Select(row => new StudentTCViewModel
                {
                    RowKey = row.RowKey,
                    ApplicationKey = row.ApplicationKey,
                    IssuedDate = row.IssuedDate,
                    IssuedBy = row.IssuedBy,
                    IsIssued = row.IsIssued,
                    IsGenerate = row.IsGenerate,
                    GeneratedBy = row.GeneratedBy,
                    GeneratedDate = row.GeneratedDate,
                    ReasonMasterKey = row.ReasonMasterKey,
                    DateOfApplicationForTC = row.DateOfApplicationForTC,
                    IsActive = row.IsActive,

                }).FirstOrDefault();

                if (model == null)
                {
                    model = new StudentTCViewModel();
                }

                model.ApplicationKey = ApplicationKey;
                model.StudentMobile = Applications != null ? Applications.StudentMobile : null;
                model.StudentEmail = Applications != null ? Applications.StudentEmail : null;
                FillResonMaster(model);
                FillNotificationDetail(model);
                return model;

            }
            catch (Exception ex)
            {
                ActivityLog.CreateActivityLog(MenuConstants.StudentTC, ActionConstants.View, DbConstants.LogType.Error, ApplicationKey, ex.GetBaseException().Message);
                return new StudentTCViewModel();
            }
        }

        public StudentTCViewModel GetTCColumnDetails(StudentTCViewModel model)
        {
            try
            {
                model.StudentTCDetailsViewModel = (from TCC in dbContext.TCConfigColumns
                                                   join STC in dbContext.StudentTCDetails.Where(x => x.StudentTCMaster.ApplicationKey == model.ApplicationKey) on new { TCConfigColumnKey = TCC.RowKey } equals new { TCConfigColumnKey = STC.TCConfigColumnKey ?? 0 } into row
                                                   from STC in row.DefaultIfEmpty()
                                                   select new StudentTCDetailsViewModel
                                                   {
                                                       RowKey = STC.RowKey != null ? STC.RowKey : 0,
                                                       TCMasterKey = STC.TCMasterKey != null ? STC.TCMasterKey : (Int64)0,
                                                       TCConfigColumnKey = TCC.RowKey,
                                                       TCConfigColumnName = TCC.ColumnName,
                                                       TCConfigDescreption = TCC.Descreption,
                                                       Value = STC.Value,
                                                       ColumnValue = TCC.ColumnValue,
                                                       IsActive = STC.IsActive != null ? STC.IsActive : false,
                                                       CondentTypeKey = TCC.CondentTypeKey,
                                                       IsMandatory = TCC.IsMandatory,
                                                       IsDeletable = TCC.IsDeletable,
                                                   }).ToList();



                if (model.StudentTCDetailsViewModel.Count == 0)
                {
                    model.StudentTCDetailsViewModel.Add(new StudentTCDetailsViewModel());
                }
                else
                {
                    foreach (StudentTCDetailsViewModel item in model.StudentTCDetailsViewModel)
                    {
                        if (item.CondentTypeKey == DbConstants.CondentType.Optional && item.ColumnValue != null && item.ColumnValue != "")
                        {
                            List<string> OptionalValues = new List<string>();
                            OptionalValues = item.ColumnValue.Split(',').Select(row => (row)).ToList();
                            item.OptionalValues = OptionalValues.Select(x => new SelectListModel
                            {
                                RowKey = x.IndexOf(x) + 1,
                                Text = x,

                            }).ToList();
                        }
                    }
                }
                return model;

            }
            catch (Exception ex)
            {
                ActivityLog.CreateActivityLog(MenuConstants.StudentTC, ActionConstants.View, DbConstants.LogType.Error, model.ApplicationKey, ex.GetBaseException().Message);
                return new StudentTCViewModel();
            }
        }
        public StudentTCViewModel CreateStudentTC(StudentTCViewModel model)
        {
            using (var transaction = dbContext.Database.BeginTransaction())
            {
                try
                {

                    long Maxkey = dbContext.StudentTCMasters.Select(x => x.RowKey).DefaultIfEmpty().Max();

                    StudentTCMaster TCMaster = new StudentTCMaster();
                    TCMaster.RowKey = ++Maxkey;
                    TCMaster.IsGenerate = true;
                    TCMaster.GeneratedBy = DbConstants.User.UserKey;
                    TCMaster.GeneratedDate = model.GeneratedDate;
                    TCMaster.DateOfApplicationForTC = model.DateOfApplicationForTC;
                    TCMaster.ReasonMasterKey = model.ReasonMasterKey;
                    TCMaster.IsActive = true;
                    TCMaster.ApplicationKey = model.ApplicationKey;

                    dbContext.StudentTCMasters.Add(TCMaster);
                    model.RowKey = TCMaster.RowKey;
                    CreateStudentTCDetails(model.StudentTCDetailsViewModel.Where(row => row.RowKey == 0).ToList(), model);

                    dbContext.SaveChanges();
                    transaction.Commit();
                    model.Message = EduSuiteUIResources.Success;
                    model.IsSuccessful = true;

                    ActivityLog.CreateActivityLog(MenuConstants.StudentTC, ActionConstants.Add, DbConstants.LogType.Info, model.ApplicationKey, model.Message);

                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    model.Message = String.Format(EduSuiteUIResources.FailedToSaveErrorMessage, EduSuiteUIResources.StudentTC);
                    model.IsSuccessful = false;
                    ActivityLog.CreateActivityLog(MenuConstants.StudentTC, ActionConstants.Add, DbConstants.LogType.Error, model.ApplicationKey, ex.GetBaseException().Message);

                }
                return model;
            }
        }

        public StudentTCViewModel UpdateStudentTC(StudentTCViewModel model)
        {
            using (var transaction = dbContext.Database.BeginTransaction())
            {
                try
                {


                    StudentTCMaster TCMaster = new StudentTCMaster();
                    TCMaster = dbContext.StudentTCMasters.Where(x => x.RowKey == model.RowKey).FirstOrDefault();

                    TCMaster.GeneratedDate = model.GeneratedDate;
                    TCMaster.DateOfApplicationForTC = model.DateOfApplicationForTC;
                    TCMaster.ReasonMasterKey = model.ReasonMasterKey;
                    model.IsIssued = TCMaster.IsIssued;
                    UpdateStudentTCDetails(model.StudentTCDetailsViewModel.Where(row => row.RowKey != 0).ToList(), model);
                    CreateStudentTCDetails(model.StudentTCDetailsViewModel.Where(row => row.RowKey == 0).ToList(), model);

                    dbContext.SaveChanges();
                    transaction.Commit();
                    model.Message = EduSuiteUIResources.Success;
                    model.IsSuccessful = true;

                    ActivityLog.CreateActivityLog(MenuConstants.StudentTC, ActionConstants.Add, DbConstants.LogType.Info, model.ApplicationKey, model.Message);

                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    model.Message = String.Format(EduSuiteUIResources.FailedToSaveErrorMessage, EduSuiteUIResources.StudentTC);
                    model.IsSuccessful = false;
                    ActivityLog.CreateActivityLog(MenuConstants.StudentTC, ActionConstants.Add, DbConstants.LogType.Error, model.ApplicationKey, ex.GetBaseException().Message);

                }
                return model;
            }
        }

        private void CreateStudentTCDetails(List<StudentTCDetailsViewModel> ModelList, StudentTCViewModel objViewModel)
        {
            Int64 MaxKey = dbContext.StudentTCDetails.Select(p => p.RowKey).DefaultIfEmpty().Max();

            foreach (StudentTCDetailsViewModel model in ModelList)
            {
                StudentTCDetail StudentTCDetailModel = new StudentTCDetail();

                StudentTCDetailModel.RowKey = Convert.ToInt64(MaxKey + 1);
                StudentTCDetailModel.TCMasterKey = objViewModel.RowKey;
                StudentTCDetailModel.TCConfigColumnKey = model.TCConfigColumnKey;
                StudentTCDetailModel.Value = model.Value;
                StudentTCDetailModel.IsActive = model.IsActive;

                dbContext.StudentTCDetails.Add(StudentTCDetailModel);
                MaxKey++;
            }
        }

        private void UpdateStudentTCDetails(List<StudentTCDetailsViewModel> ModelList, StudentTCViewModel objViewModel)
        {


            foreach (StudentTCDetailsViewModel model in ModelList)
            {
                StudentTCDetail StudentTCDetailModel = new StudentTCDetail();

                StudentTCDetailModel = dbContext.StudentTCDetails.Where(x => x.RowKey == model.RowKey).FirstOrDefault();
                StudentTCDetailModel.TCMasterKey = objViewModel.RowKey;
                StudentTCDetailModel.TCConfigColumnKey = model.TCConfigColumnKey;
                StudentTCDetailModel.Value = model.Value;
                StudentTCDetailModel.IsActive = model.IsActive;

            }
        }
        public StudentTCViewModel IssueStudentTC(StudentTCViewModel model)
        {
            using (var transaction = dbContext.Database.BeginTransaction())
            {
                try
                {
                    StudentTCMaster TCMaster = new StudentTCMaster();
                    TCMaster = dbContext.StudentTCMasters.Where(x => x.RowKey == model.RowKey).FirstOrDefault();
                    TCMaster.IssuedDate = model.IssuedDate;
                    TCMaster.IssuedBy = DbConstants.User.UserKey;
                    TCMaster.IsIssued = model.IsIssued;


                    dbContext.SaveChanges();
                    transaction.Commit();
                    model.Message = EduSuiteUIResources.Success;
                    model.IsSuccessful = true;

                    ActivityLog.CreateActivityLog(MenuConstants.StudentTC, ActionConstants.Edit, DbConstants.LogType.Info, model.ApplicationKey, model.Message);

                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    model.Message = String.Format(EduSuiteUIResources.FailedToSaveErrorMessage, EduSuiteUIResources.StudentTC);
                    model.IsSuccessful = false;
                    ActivityLog.CreateActivityLog(MenuConstants.StudentTC, ActionConstants.Edit, DbConstants.LogType.Error, model.ApplicationKey, ex.GetBaseException().Message);

                }
                return model;
            }
        }
        public StudentTCDetailsViewModel DeleteStudentTCDetails(long RowKey)
        {
            StudentTCDetailsViewModel model = new StudentTCDetailsViewModel();
            using (var Transaction = dbContext.Database.BeginTransaction())
            {
                try
                {
                    StudentTCDetail studentTcdetails = dbContext.StudentTCDetails.Where(x => x.RowKey == RowKey).FirstOrDefault();
                    dbContext.StudentTCDetails.Remove(studentTcdetails);

                    dbContext.SaveChanges();
                    Transaction.Commit();
                    model.Message = EduSuiteUIResources.Success;
                    model.IsSuccessful = true;
                    ActivityLog.CreateActivityLog(MenuConstants.StudentTC, ActionConstants.Delete, DbConstants.LogType.Info, RowKey, model.Message);

                }
                catch (DbUpdateException ex)
                {
                    Transaction.Rollback();
                    if (ex.GetBaseException().Message.ToUpper().Contains(EduSuiteUIResources.ForeignKeyError.ToUpper()))
                    {
                        model.Message = String.Format(EduSuiteUIResources.CantDeleteErrorMessage, EduSuiteUIResources.StudentTC);
                        model.IsSuccessful = false;
                        ActivityLog.CreateActivityLog(MenuConstants.StudentTC, ActionConstants.Delete, DbConstants.LogType.Error, RowKey, ex.GetBaseException().Message);
                    }
                }
                catch (Exception ex)
                {
                    Transaction.Rollback();
                    model.Message = String.Format(EduSuiteUIResources.FailedToDeleteErrorMessage, EduSuiteUIResources.StudentTC);
                    model.IsSuccessful = false;
                    ActivityLog.CreateActivityLog(MenuConstants.StudentTC, ActionConstants.Delete, DbConstants.LogType.Error, RowKey, ex.GetBaseException().Message);
                }
            }
            return model;


        }
        public void FillResonMaster(StudentTCViewModel model)
        {
            model.ReasonMaster = dbContext.TCReasonMasters.Where(x => x.IsActive).Select(row => new SelectListModel
            {
                RowKey = row.RowKey,
                Text = row.ReasonName
            }).ToList();
        }

        public StudentTCViewModel DeleteStudentTC(long RowKey)
        {
            StudentTCViewModel model = new StudentTCViewModel();
            using (var Transaction = dbContext.Database.BeginTransaction())
            {
                try
                {
                    List<StudentTCDetail> studentTcdetailsList = dbContext.StudentTCDetails.Where(x => x.TCMasterKey == RowKey).ToList();
                    dbContext.StudentTCDetails.RemoveRange(studentTcdetailsList);
                    

                    StudentTCMaster studentTCMaster = dbContext.StudentTCMasters.Where(x => x.RowKey == RowKey).FirstOrDefault();
                    dbContext.StudentTCMasters.Remove(studentTCMaster);

                    dbContext.SaveChanges();
                    Transaction.Commit();
                    model.Message = EduSuiteUIResources.Success;
                    model.IsSuccessful = true;
                    ActivityLog.CreateActivityLog(MenuConstants.StudentTC, ActionConstants.Delete, DbConstants.LogType.Info, RowKey, model.Message);

                }
                catch (DbUpdateException ex)
                {
                    Transaction.Rollback();
                    if (ex.GetBaseException().Message.ToUpper().Contains(EduSuiteUIResources.ForeignKeyError.ToUpper()))
                    {
                        model.Message = String.Format(EduSuiteUIResources.CantDeleteErrorMessage, EduSuiteUIResources.StudentTC);
                        model.IsSuccessful = false;
                        ActivityLog.CreateActivityLog(MenuConstants.StudentTC, ActionConstants.Delete, DbConstants.LogType.Error, RowKey, ex.GetBaseException().Message);
                    }
                }
                catch (Exception ex)
                {
                    Transaction.Rollback();
                    model.Message = String.Format(EduSuiteUIResources.FailedToDeleteErrorMessage, EduSuiteUIResources.StudentTC);
                    model.IsSuccessful = false;
                    ActivityLog.CreateActivityLog(MenuConstants.StudentTC, ActionConstants.Delete, DbConstants.LogType.Error, RowKey, ex.GetBaseException().Message);
                }
            }
            return model;


        }

        private void FillNotificationDetail(StudentTCViewModel model)
        {
            NotificationTemplate notificationTemplateModel = dbContext.NotificationTemplates.SingleOrDefault(row => row.RowKey == DbConstants.NotificationTemplate.StudentTC);
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
