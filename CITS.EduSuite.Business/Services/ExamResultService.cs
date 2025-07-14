using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Linq.Expressions;
using System.Threading.Tasks;
using CITS.EduSuite.Business.Interfaces;
using CITS.EduSuite.Business.Common;
using CITS.EduSuite.Data;
using CITS.EduSuite.Business.Models.ViewModels;
using System.Data.Entity.Infrastructure;
using CITS.EduSuite.Business.Models.Resources;

namespace CITS.EduSuite.Business.Services
{
    public class ExamResultService : IExamResultService
    {
        private EduSuiteDatabase dbContext;

        public ExamResultService(EduSuiteDatabase objEduSuiteDataBase)
        {
            this.dbContext = objEduSuiteDataBase;
        }

        public List<ExamResultViewModel> GetExamResult(ExamResultViewModel model, out long TotalRecords)
        {
            try
            {
                var Take = model.PageSize;
                var Skip = (model.PageIndex - 1) * model.PageSize;



                IQueryable<ExamResultViewModel> ExamResult = (from ES in dbContext.ExamSchedules
                                                              join CSD in dbContext.CourseSubjectDetails on ES.SubjectKey equals CSD.SubjectKey
                                                              select new ExamResultViewModel
                                                              {
                                                                  BranchKey = ES.Application.BranchKey,
                                                                  UniversityMasterKey = ES.Application.UniversityMasterKey,
                                                                  CourseKey = ES.Application.CourseKey,
                                                                  AcademicTermKey = ES.Application.AcademicTermKey,
                                                                  BatchKey = ES.Application.BatchKey,
                                                                  ExamTermKey = ES.ExamTermKey,
                                                                  CurrentYear = CSD.CourseSubjectMaster.CourseYear,
                                                                  BranchName = ES.Application.Branch.BranchName,
                                                                  UniversityName = ES.Application.UniversityMaster.UniversityMasterName,
                                                                  CourseName = ES.Application.Course.CourseName,
                                                                  BatchName = ES.Application.Batch.BatchName,
                                                                  ExamTermName = ES.ExamTerm.ExamTermName,
                                                                  CourseDuration = ES.Application.Course.CourseDuration,
                                                                  NoOfSubject = dbContext.ExamSchedules.Join(dbContext.CourseSubjectDetails, p => p.SubjectKey, y => y.SubjectKey,
                                                                 (p, y) => new { p, y }).Where(x => x.p.Application.CourseKey == ES.Application.CourseKey &&
                                                                     x.p.Application.UniversityMasterKey == ES.Application.UniversityMasterKey && x.p.Application.BatchKey == ES.Application.BatchKey &&
                                                                      x.p.Application.AcademicTermKey == ES.Application.AcademicTermKey && x.p.Application.BranchKey == ES.Application.BranchKey &&
                                                                      x.p.ExamTermKey == ES.ExamTermKey && x.p.Subject.CourseSubjectDetails.Select(row => row.CourseSubjectMaster.CourseYear).FirstOrDefault() == CSD.CourseSubjectMaster.CourseYear
                                                                   ).Distinct().Count(),
                                                              });
                Employee Employee = dbContext.Employees.Where(x => x.AppUserKey == DbConstants.User.UserKey).SingleOrDefault();

                if (Employee != null)
                {
                    if (Employee.BranchAccess != null)
                    {
                        var Branches = Employee.BranchAccess.Split(',').Select(Int16.Parse).ToList();
                        ExamResult = ExamResult.Where(row => Branches.Contains(row.BranchKey ?? 0));
                    }
                }
                if (model.BranchKey != 0)
                {
                    ExamResult = ExamResult.Where(row => row.BranchKey == model.BranchKey);
                }
                if (model.BatchKey != 0)
                {
                    ExamResult = ExamResult.Where(row => row.BatchKey == model.BatchKey);
                }
                if (model.CourseKey != 0)
                {
                    ExamResult = ExamResult.Where(row => row.CourseKey == model.CourseKey);
                }
                if (model.UniversityMasterKey != 0)
                {
                    ExamResult = ExamResult.Where(row => row.UniversityMasterKey == model.UniversityMasterKey);
                }

                ExamResult = ExamResult.GroupBy(x => new { x.BranchKey, x.AcademicTermKey, x.CourseKey, x.UniversityMasterKey, x.BatchKey, x.CurrentYear, x.ExamTermKey }).Select(y => y.FirstOrDefault());
                if (model.SortBy != "")
                {
                    ExamResult = SortExamResult(ExamResult, model.SortBy, model.SortOrder);
                }
                TotalRecords = ExamResult.Count();
                return ExamResult.Skip(Skip).Take(Take).ToList<ExamResultViewModel>();


            }
            catch (Exception ex)
            {
                TotalRecords = 0;
                ActivityLog.CreateActivityLog(MenuConstants.ExamResult, ActionConstants.View, DbConstants.LogType.Error, null, ex.GetBaseException().Message);

                return new List<ExamResultViewModel>();

            }

        }
        private IQueryable<ExamResultViewModel> SortExamResult(IQueryable<ExamResultViewModel> Query, string SortName, string SortOrder)
        {

            string command = SortOrder == "desc" ? "OrderByDescending" : "OrderBy";
            var type = typeof(ExamResultViewModel);
            var property = type.GetProperty(SortName);
            var parameter = Expression.Parameter(type, "p");
            var propertyAccess = Expression.MakeMemberAccess(parameter, property);
            var orderByExpression = Expression.Lambda(propertyAccess, parameter);
            var resultExpression = Expression.Call(typeof(Queryable), command, new Type[] { type, property.PropertyType },
                                          Query.Expression, Expression.Quote(orderByExpression));
            return Query.Provider.CreateQuery<ExamResultViewModel>(resultExpression);

        }
        public ExamResultViewModel GetExamResultDetails(ExamResultViewModel model)
        {

            model.ExamResultSubjectDetail = (from IE in dbContext.ExamSchedules
                                             join CSD in dbContext.CourseSubjectDetails on IE.SubjectKey equals CSD.SubjectKey
                                             join IER in dbContext.ExamResults on new { ExamScheduleKey = (IE.RowKey), IE.SubjectKey, IE.ApplicationKey }
                                             equals new { IER.ExamScheduleKey, IER.SubjectKey, IER.ApplicationKey } into IERD
                                             from IER in IERD.DefaultIfEmpty()
                                             where (IE.Application.BranchKey == model.BranchKey && IE.Application.AcademicTermKey == model.AcademicTermKey &&
                                             IE.Application.CourseKey == model.CourseKey && IE.Application.UniversityMasterKey == model.UniversityMasterKey &&
                                             IE.Application.BatchKey == model.BatchKey && IE.ExamTermKey == model.ExamTermKey && CSD.CourseSubjectMaster.CourseYear == model.ExamYear
                                             )
                                             group IER by new
                                             {
                                                 IE.Subject.SubjectName,
                                                 IE.SubjectKey,
                                                 IE.Application.BranchKey,
                                                 IE.Application.AcademicTermKey,
                                                 IE.Application.CourseKey,
                                                 IE.Application.UniversityMasterKey,
                                                 IE.Application.BatchKey,
                                                 IE.ExamTermKey,
                                                 CSD.CourseSubjectMaster.CourseYear
                                             } into g
                                             select new ExamResultSubjectDetail
                                             {
                                                 //ExamScheduleKey = g.Key.RowKey,
                                                 //ExamScheduleDetailsKey = g.Key.RowKey,
                                                 BranchKey = g.Key.BranchKey,
                                                 BatchKey = g.Key.BatchKey,
                                                 AcademicTermKey = g.Key.AcademicTermKey,
                                                 CourseKey = g.Key.CourseKey,
                                                 UniversityMasterKey = g.Key.UniversityMasterKey,
                                                 ExamTermKey = g.Key.ExamTermKey,
                                                 ExamYear = g.Key.CourseYear,
                                                 SubjectKey = g.Key.SubjectKey,
                                                 SubjectName = g.Key.SubjectName,
                                                 Passed = g.Count(x => x.ResultStatus == DbConstants.ResultStatus.Passed),
                                                 Failed = g.Count(x => x.ResultStatus == DbConstants.ResultStatus.Fail),
                                                 Absent = g.Count(x => x.ResultStatus == DbConstants.ResultStatus.Absent),
                                             }).ToList();



            return model;
        }
        public ExamResultViewModel UpdateExamResult(ExamResultViewModel model)
        {
            using (var transaction = dbContext.Database.BeginTransaction())
            {
                try
                {
                    CreateExamResult(model.ExamResultDetail.Where(row => row.RowKey == 0).ToList(), model);
                    UpdateExamResult(model.ExamResultDetail.Where(row => row.RowKey != 0).ToList(), model);
                    dbContext.SaveChanges();
                    transaction.Commit();
                    model.Message = EduSuiteUIResources.Success;
                    //model.AdmissionNo = dbContext.T_Application.Where(row => row.RowKey == model.ApplicationKey).Select(row => row.AdmissionNo).FirstOrDefault();
                    model.IsSuccessful = true;
                    ActivityLog.CreateActivityLog(MenuConstants.ExamResult, (model.ExamResultDetail.Any(row => row.RowKey != 0) ? ActionConstants.Edit : ActionConstants.Add), DbConstants.LogType.Info, model.EmployeeKey, model.Message);
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    model.Message = String.Format(EduSuiteUIResources.FailedToSaveErrorMessage, EduSuiteUIResources.ExamResult);
                    model.IsSuccessful = false;

                    ActivityLog.CreateActivityLog(MenuConstants.ExamResult, (model.ExamResultDetail.Any(row => row.RowKey != 0) ? ActionConstants.Edit : ActionConstants.Add), DbConstants.LogType.Error, model.EmployeeKey, ex.GetBaseException().Message);
                }
            }
            return model;
        }
        private void CreateExamResult(List<ExamResultDetail> modelList, ExamResultViewModel objviewmodel)
        {
            Int64 MaxKey = dbContext.ExamResults.Select(p => p.RowKey).DefaultIfEmpty().Max();

            foreach (ExamResultDetail model in modelList)
            {

                ExamResult ExamResultModel = new ExamResult();
                ExamResultModel.RowKey = Convert.ToInt64(MaxKey + 1);
                ExamResultModel.ApplicationKey = model.ApplicationKey;
                ExamResultModel.SubjectKey = model.SubjectKey;
                ExamResultModel.ResultStatus = model.ResultStatus;
                ExamResultModel.Mark = model.Mark;
                ExamResultModel.Remarks = model.Remarks;
                ExamResultModel.ExamScheduleKey = model.ExamScheduleKey;
                ExamResultModel.ExamStatus = model.ExamStatus;

                if (model.ExamStatus != DbConstants.ExamStatus.Reguler)
                {
                    ExamSchedule ExamScheduleModel = dbContext.ExamSchedules.SingleOrDefault(x => x.RowKey == model.ExamScheduleKey);
                    ExamScheduleModel.IsActive = false;
                }

                dbContext.ExamResults.Add(ExamResultModel);
                MaxKey++;

            }
        }
        private void UpdateExamResult(List<ExamResultDetail> modelList, ExamResultViewModel objviewmodel)
        {
            foreach (ExamResultDetail model in modelList)
            {

                ExamResult ExamResultModel = new ExamResult();
                ExamResultModel = dbContext.ExamResults.SingleOrDefault(row => row.RowKey == model.RowKey);
                ExamResultModel.ApplicationKey = model.ApplicationKey;
                ExamResultModel.SubjectKey = model.SubjectKey;
                ExamResultModel.ResultStatus = model.ResultStatus;
                ExamResultModel.Mark = model.Mark;
                ExamResultModel.Remarks = model.Remarks;
                ExamResultModel.ExamScheduleKey = model.ExamScheduleKey;
                ExamResultModel.ExamStatus = model.ExamStatus;

                if (model.ExamStatus != DbConstants.ExamStatus.Reguler)
                {
                    ExamSchedule ExamScheduleModel = dbContext.ExamSchedules.SingleOrDefault(x => x.RowKey == model.ExamScheduleKey);
                    ExamScheduleModel.IsActive = false;
                }
            }
        }
        public ExamResultViewModel StudentMarkDetils(ExamResultViewModel model)
        {
            model.ExamResultDetail = (from App in dbContext.Applications
                                      join IE in dbContext.ExamSchedules on App.RowKey equals IE.ApplicationKey

                                      join IER in dbContext.ExamResults on new { IE.ApplicationKey, ExamScheduleKey = IE.RowKey, IE.SubjectKey }
                                      equals new { IER.ApplicationKey, IER.ExamScheduleKey, IER.SubjectKey } into IERD
                                      from IER in IERD.DefaultIfEmpty()
                                      where (App.StudentStatusKey == DbConstants.StudentStatus.Ongoing &&
                                             App.BranchKey == model.BranchKey && App.AcademicTermKey == model.AcademicTermKey &&
                                             App.CourseKey == model.CourseKey && App.UniversityMasterKey == model.UniversityMasterKey &&
                                             App.BatchKey == model.BatchKey && IE.ExamTermKey == model.ExamTermKey && //App.CurrentYear == model.ExamYear &&
                                             IE.SubjectKey == model.SubjectKey)
                                      select new ExamResultDetail
                                      {
                                          ApplicationKey = App.RowKey,
                                          StudentName = App.StudentName,
                                          AdmissionNo = App.AdmissionNo,
                                          RowKey = IER.RowKey != null ? IER.RowKey : 0,
                                          SubjectKey = IE.SubjectKey,
                                          ResultStatus = IER.ResultStatus,
                                          Mark = IER.Mark,
                                          MaximumMark = IE.MaximumMark,
                                          MinimumMark = IE.MinimumMark,
                                          Remarks = IER.Remarks,
                                          AbsentStatus = (IER.ResultStatus == DbConstants.ResultStatus.Absent ? true : false),
                                          ExamScheduleKey = IE.RowKey,
                                          ExamStatus = IER.ExamStatus != null ? IER.ExamStatus : DbConstants.ExamStatus.Reguler,
                                          ExamTermName = IE.ExamTerm.ExamTermName,
                                          AppearenceCount = IE.AppearenceCount,
                                          SubjectYear = dbContext.CourseSubjectDetails.Where(row => row.SubjectKey == IE.SubjectKey).Select(x => x.CourseSubjectMaster.CourseYear).FirstOrDefault(),
                                          AcademicTermKey = dbContext.CourseSubjectDetails.Where(row => row.SubjectKey == IE.SubjectKey).Select(x => x.CourseSubjectMaster.AcademicTermKey).FirstOrDefault(),
                                          CourseDuration = dbContext.CourseSubjectDetails.Where(row => row.SubjectKey == IE.SubjectKey).Select(x => x.CourseSubjectMaster.Course.CourseDuration).FirstOrDefault(),

                                      }).OrderBy(y => y.SubjectYear).ToList();
            foreach (ExamResultDetail objmodel in model.ExamResultDetail)
            {
                objmodel.SubjectYearName = CommonUtilities.GetYearDescriptionByCodeDetails(objmodel.CourseDuration ?? 0, objmodel.SubjectYear ?? 0, objmodel.AcademicTermKey ?? 0);

            }

            FillExamStatus(model);
            return model;
        }

        public ExamResultViewModel DeleteExamResult(long? ExamScheduleKey, long? SubjectKey)
        {
            using (var transaction = dbContext.Database.BeginTransaction())
            {
                ExamResultViewModel model = new ExamResultViewModel();
                try
                {
                    List<ExamResult> examResultList = dbContext.ExamResults.Where(x => x.ExamScheduleKey == ExamScheduleKey && x.SubjectKey == SubjectKey).ToList();
                    if (examResultList.Count > 0)
                    {
                        dbContext.ExamResults.RemoveRange(examResultList);
                        dbContext.SaveChanges();
                        transaction.Commit();
                        model.Message = EduSuiteUIResources.Success;
                        model.IsSuccessful = true;
                        ActivityLog.CreateActivityLog(MenuConstants.ExamResult, ActionConstants.Delete, DbConstants.LogType.Info, ExamScheduleKey, model.Message);

                    }
                    else
                    {
                        transaction.Rollback();
                        model.Message = String.Format(EduSuiteUIResources.FailedToDeleteErrorMessage, EduSuiteUIResources.ExamResult);
                        model.IsSuccessful = false;

                    }


                }
                catch (DbUpdateException ex)
                {
                    transaction.Rollback();
                    if (ex.GetBaseException().Message.ToUpper().Contains(EduSuiteUIResources.ForeignKeyError.ToUpper()))
                    {
                        model.Message = String.Format(EduSuiteUIResources.CantDeleteErrorMessage, EduSuiteUIResources.ExamResult);
                        model.IsSuccessful = false;
                        ActivityLog.CreateActivityLog(MenuConstants.ExamResult, ActionConstants.Delete, DbConstants.LogType.Error, ExamScheduleKey, ex.GetBaseException().Message);
                    }
                }
                catch (Exception Ex)
                {
                    transaction.Rollback();
                    model.Message = String.Format(EduSuiteUIResources.FailedToDeleteErrorMessage, EduSuiteUIResources.ExamResult);
                    model.IsSuccessful = false;
                    ActivityLog.CreateActivityLog(MenuConstants.ExamResult, ActionConstants.Delete, DbConstants.LogType.Error, ExamScheduleKey, model.Message);
                }
                return model;
            }

        }

        public ExamResultViewModel ResetExamResult(long? ExamResultKey)
        {
            using (var transaction = dbContext.Database.BeginTransaction())
            {
                ExamResultViewModel model = new ExamResultViewModel();
                try
                {
                    ExamResult examResultList = dbContext.ExamResults.Where(x => x.RowKey == ExamResultKey).SingleOrDefault();

                    dbContext.ExamResults.Remove(examResultList);
                    dbContext.SaveChanges();
                    transaction.Commit();
                    model.Message = EduSuiteUIResources.Success;
                    model.IsSuccessful = true;
                    ActivityLog.CreateActivityLog(MenuConstants.ExamResult, ActionConstants.Delete, DbConstants.LogType.Info, ExamResultKey, model.Message);




                }
                catch (DbUpdateException ex)
                {
                    transaction.Rollback();
                    if (ex.GetBaseException().Message.ToUpper().Contains(EduSuiteUIResources.ForeignKeyError.ToUpper()))
                    {
                        model.Message = String.Format(EduSuiteUIResources.CantDeleteErrorMessage, EduSuiteUIResources.ExamResult);
                        model.IsSuccessful = false;
                        ActivityLog.CreateActivityLog(MenuConstants.ExamResult, ActionConstants.Delete, DbConstants.LogType.Error, ExamResultKey, ex.GetBaseException().Message);
                    }
                }
                catch (Exception Ex)
                {
                    transaction.Rollback();
                    model.Message = String.Format(EduSuiteUIResources.FailedToDeleteErrorMessage, EduSuiteUIResources.ExamResult);
                    model.IsSuccessful = false;
                    ActivityLog.CreateActivityLog(MenuConstants.ExamResult, ActionConstants.Delete, DbConstants.LogType.Error, ExamResultKey, model.Message);
                }
                return model;
            }

        }

        public List<ApplicationViewModel> GetApplications(ApplicationViewModel model, out long TotalRecords)
        {
            try
            {
                var Take = model.PageSize;
                var Skip = (model.PageIndex - 1) * model.PageSize;

                IQueryable<ApplicationViewModel> applicationList = (from a in dbContext.Applications
                                                                    where (a.StudentName.Contains(model.ApplicantName)) && (a.StudentMobile.Contains(model.MobileNumber))
                                                                    select new ApplicationViewModel
                                                                    {
                                                                        RowKey = a.RowKey,
                                                                        AdmissionNo = a.AdmissionNo,
                                                                        ApplicantName = a.StudentName,
                                                                        CourseName = a.Course.CourseName,
                                                                        UniversityName = a.UniversityMaster.UniversityMasterName,
                                                                        MobileNumber = a.StudentMobile,
                                                                        ApplicationStatusName = a.StudentStatu.StudentStatusName,
                                                                        BatchName = a.Batch.BatchName,
                                                                        BatchKey = a.BatchKey,
                                                                        BranchKey = a.BranchKey,
                                                                        CourseKey = a.CourseKey,
                                                                        UniversityKey = a.UniversityMasterKey,
                                                                        NoOfCertificate = dbContext.ExamSchedules.Where(x => x.ApplicationKey == a.RowKey).Count(),
                                                                        CurrentYear = a.CurrentYear,
                                                                        AcademicTermKey = a.AcademicTermKey,
                                                                        CourseDuration = a.Course.CourseDuration
                                                                    });

                Employee Employee = dbContext.Employees.Where(x => x.AppUserKey == DbConstants.User.UserKey).SingleOrDefault();

                if (Employee != null)
                {
                    if (Employee.BranchAccess != null)
                    {
                        var Branches = Employee.BranchAccess.Split(',').Select(Int16.Parse).ToList();
                        applicationList = applicationList.Where(row => Branches.Contains(row.BranchKey ?? 0));
                    }
                }
                if (model.BranchKey != 0)
                {
                    applicationList = applicationList.Where(row => row.BranchKey == model.BranchKey);
                }
                if (model.BatchKey != 0)
                {
                    applicationList = applicationList.Where(row => row.BatchKey == model.BatchKey);
                }
                if (model.CourseKey != 0)
                {
                    applicationList = applicationList.Where(row => row.CourseKey == model.CourseKey);
                }
                if (model.UniversityKey != 0)
                {
                    applicationList = applicationList.Where(row => row.UniversityKey == model.UniversityKey);
                }
                applicationList = applicationList.GroupBy(x => x.RowKey).Select(y => y.FirstOrDefault());
                if (model.SortBy != "")
                {
                    applicationList = SortApplications(applicationList, model.SortBy, model.SortOrder);
                }
                TotalRecords = applicationList.Count();
                return applicationList.Skip(Skip).Take(Take).ToList<ApplicationViewModel>();
            }
            catch (Exception ex)
            {
                TotalRecords = 0;
                ActivityLog.CreateActivityLog(MenuConstants.StudentsCertificateReturn, ActionConstants.View, DbConstants.LogType.Error, null, ex.GetBaseException().Message);
                return new List<ApplicationViewModel>();


            }
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


        public ExamResultViewModel StudentMarkDetilsByIndividual(ExamResultViewModel model)
        {


            model.ExamResultDetail = (from App in dbContext.Applications
                                      join IE in dbContext.ExamSchedules on App.RowKey equals IE.ApplicationKey

                                      join IER in dbContext.ExamResults on new { IE.ApplicationKey, ExamScheduleKey = IE.RowKey }
                                      equals new { IER.ApplicationKey, IER.ExamScheduleKey } into IERD
                                      from IER in IERD.DefaultIfEmpty()
                                      where (App.RowKey == model.ApplicationKey)
                                      select new ExamResultDetail
                                      {
                                          ApplicationKey = App.RowKey,
                                          SubjectName = IE.Subject.SubjectName,
                                          AdmissionNo = App.AdmissionNo,
                                          RowKey = IER.RowKey != null ? IER.RowKey : 0,
                                          SubjectKey = IE.SubjectKey,
                                          ResultStatus = IER.ResultStatus,
                                          Mark = IER.Mark,
                                          MaximumMark = IE.MaximumMark,
                                          MinimumMark = IE.MinimumMark,
                                          Remarks = IER.Remarks,
                                          AbsentStatus = (IER.ResultStatus == DbConstants.ResultStatus.Absent ? true : false),
                                          ExamScheduleKey = IE.RowKey,
                                          ExamStatus = IER.ExamStatus != null ? IER.ExamStatus : IER.ExamSchedule.ExamStatus,
                                          ExamTermName = IE.ExamTerm.ExamTermName,
                                          AppearenceCount = IE.AppearenceCount,
                                          SubjectYear = dbContext.CourseSubjectDetails.Where(row => row.SubjectKey == IE.SubjectKey).Select(x => x.CourseSubjectMaster.CourseYear).FirstOrDefault(),
                                          AcademicTermKey = dbContext.CourseSubjectDetails.Where(row => row.SubjectKey == IE.SubjectKey).Select(x => x.CourseSubjectMaster.AcademicTermKey).FirstOrDefault(),
                                          CourseDuration = dbContext.CourseSubjectDetails.Where(row => row.SubjectKey == IE.SubjectKey).Select(x => x.CourseSubjectMaster.Course.CourseDuration).FirstOrDefault(),
                                      }).OrderBy(y => y.SubjectYear).ToList();

            foreach (ExamResultDetail objmodel in model.ExamResultDetail)
            {
                objmodel.SubjectYearName = CommonUtilities.GetYearDescriptionByCodeDetails(objmodel.CourseDuration ?? 0, objmodel.SubjectYear ?? 0, objmodel.AcademicTermKey ?? 0);

            }

            FillExamStatus(model);
            return model;


        }

        public void FillExamStatus(ExamResultViewModel model)
        {
            model.ExamStatus = typeof(DbConstants.ExamStatus).GetFields().Select(row => new SelectListModel
            {
                RowKey = Convert.ToByte((row.GetValue(null).ToString())),
                Text = row.Name
            }).ToList();
        }

        public ExamResultViewModel UpdateExamResults(ExamResultViewModel MasterModel)
        {
            using (var transaction = dbContext.Database.BeginTransaction())
            {

                try
                {

                    Int64 examResultMaxKey = dbContext.ExamResults.Select(p => p.RowKey).DefaultIfEmpty().Max();
                    Int64 examScheduleMaxKey = dbContext.ExamSchedules.Select(p => p.RowKey).DefaultIfEmpty().Max();
                    dbContext.Configuration.AutoDetectChangesEnabled = false;
                    int Count = 0;

                    foreach (ExamResultDetail model in MasterModel.ExamResultDetail)
                    {
                        ++Count;
                        ExamSchedule examSchedule = dbContext.ExamSchedules.Where(x => x.IsActive && x.ApplicationKey == model.ApplicationKey && x.SubjectKey == model.SubjectKey && x.ExamTermKey == model.ExamTermKey && !x.ExamResults.Any()).FirstOrDefault();
                        if (examSchedule == null)
                        {
                            int? AppearenceCount = dbContext.ExamSchedules.Where(x => x.IsActive && x.ApplicationKey == model.ApplicationKey && x.SubjectKey == model.SubjectKey && x.ExamTermKey == model.ExamTermKey).OrderByDescending(x => x.AppearenceCount).Select(x => x.AppearenceCount).FirstOrDefault();
                            AppearenceCount = (AppearenceCount ?? 0) + 1;
                            byte? ExamStatus = dbContext.ExamResults.Where(x => x.ApplicationKey == model.ApplicationKey && x.SubjectKey == model.SubjectKey && x.ExamSchedule.ExamTermKey == model.ExamTermKey).OrderByDescending(x => x.ExamSchedule.AppearenceCount).Select(x => x.ExamStatus).FirstOrDefault();
                            if ((ExamStatus ?? DbConstants.ExamStatus.Supply) != DbConstants.ExamStatus.Reguler && model.ExamTermKey != null && model.ExamCenterKey != null && model.MaximumMark != null && model.MinimumMark != null && model.ExamDate != null)
                            {
                                model.ExamScheduleKey = ++examScheduleMaxKey;
                                dbContext.AddToContext(new ExamSchedule
                                {
                                    RowKey = model.ExamScheduleKey,
                                    ApplicationKey = model.ApplicationKey,
                                    SubjectKey = model.SubjectKey,
                                    ExamTermKey = model.ExamTermKey ?? 0,
                                    ExamCenterKey = model.ExamCenterKey,
                                    ExamDate = model.ExamDate ?? DateTimeUTC.Now,
                                    MaximumMark = model.MaximumMark,
                                    MinimumMark = model.MinimumMark,
                                    AppearenceCount = AppearenceCount ?? 1,
                                    ExamStatus = ExamStatus ?? DbConstants.ExamStatus.Reguler,
                                    IsActive = true

                                }, Count);
                                ++Count;
                            }
                        }
                        else
                        {
                            model.ExamScheduleKey = examSchedule.RowKey;
                            model.MinimumMark = examSchedule.MinimumMark;
                            model.MaximumMark = examSchedule.MaximumMark;
                        }
                        model.ResultStatus = model.ResultStatus == null ? ((model.MinimumMark ?? 0) > (model.Mark ?? 0) ? "F" : "P") : model.ResultStatus;
                        model.ExamStatus = ((model.MinimumMark ?? 0) > (model.Mark ?? 0) ? DbConstants.ExamStatus.Supply : DbConstants.ExamStatus.Reguler);
                        if (model.ExamScheduleKey != 0)
                        {

                            dbContext.AddToContext(new ExamResult
                            {
                                RowKey = ++examResultMaxKey,
                                ApplicationKey = model.ApplicationKey,
                                ExamScheduleKey = model.ExamScheduleKey,
                                SubjectKey = model.SubjectKey,
                                ResultStatus = model.ResultStatus,
                                Mark = model.Mark,
                                ExamStatus = model.ExamStatus

                            }, Count);
                            ++Count;
                        }


                    }
                    dbContext.SaveChanges();
                    dbContext.Configuration.AutoDetectChangesEnabled = true;
                    transaction.Commit();

                    MasterModel.Message = EduSuiteUIResources.Success;
                    MasterModel.IsSuccessful = true;
                    ActivityLog.CreateActivityLog(MenuConstants.ExamResult, ActionConstants.Edit, DbConstants.LogType.Info, DbConstants.User.UserKey, MasterModel.Message);

                }
                catch (System.Data.Entity.Validation.DbEntityValidationException dbEx)
                {
                    Exception raise = dbEx;
                    foreach (var validationErrors in dbEx.EntityValidationErrors)
                    {
                        foreach (var validationError in validationErrors.ValidationErrors)
                        {
                            string message = string.Format("{0}:{1}", validationErrors.Entry.Entity.ToString(), validationError.ErrorMessage);
                            //raise a new exception inserting the current one as the InnerException
                            raise = new InvalidOperationException(message, raise);
                        }
                    }
                    ActivityLog.CreateActivityLog(MenuConstants.ExamResult, ActionConstants.Edit, DbConstants.LogType.Error, DbConstants.User.UserKey, dbEx.GetBaseException().Message);

                    throw raise;

                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    MasterModel.Message = String.Format(EduSuiteUIResources.FailedToSaveErrorMessage, EduSuiteUIResources.ExamResult);
                    MasterModel.IsSuccessful = false;
                    ActivityLog.CreateActivityLog(MenuConstants.ExamResult, ActionConstants.Edit, DbConstants.LogType.Error, DbConstants.User.UserKey, ex.GetBaseException().Message);

                }
            }
            return MasterModel;
        }

    }
}
