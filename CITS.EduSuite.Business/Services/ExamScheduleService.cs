using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CITS.EduSuite.Business.Interfaces;
using CITS.EduSuite.Business.Models.ViewModels;
using CITS.EduSuite.Data;
using System.Data.Entity;
using CITS.EduSuite.Business.Common;
using System.Data.Entity.Infrastructure;
using CITS.EduSuite.Business.Models.Security;
using System.Threading;
using System.Linq.Expressions;
using CITS.EduSuite.Business.Models.Resources;

namespace CITS.EduSuite.Business.Services
{
    public class ExamScheduleService : IExamScheduleService
    {
        private EduSuiteDatabase dbContext;
        public ExamScheduleService(EduSuiteDatabase objDB)
        {
            this.dbContext = objDB;
        }

        public ExamScheduleViewModel GetExamScheduleById(ExamScheduleViewModel model)
        {
            try
            {
                model = (from ES in dbContext.ExamSchedules
                         join CSD in dbContext.CourseSubjectDetails on ES.SubjectKey equals CSD.SubjectKey
                         where ES.Application.BranchKey == model.BranchKey && ES.Application.AcademicTermKey == model.AcademicTermKey && ES.Application.CourseKey == model.CourseKey &&
                         ES.Application.UniversityMasterKey == model.UniversityMasterKey && CSD.CourseSubjectMaster.CourseYear == model.CourseYear &&
                         ES.ExamTermKey == model.ExamTermKey && ES.Application.BatchKey == model.BatchKey && ES.SubjectKey == model.SubjectKey
                         select new ExamScheduleViewModel
                         {
                             AcademicTermKey = ES.Application.AcademicTermKey,
                             BranchKey = ES.Application.BranchKey,
                             CourseKey = ES.Application.CourseKey,
                             UniversityMasterKey = ES.Application.UniversityMasterKey,
                             BatchKey = ES.Application.BatchKey,
                             CourseYear = CSD.CourseSubjectMaster.CourseYear,
                             ExamTermKey = ES.ExamTermKey,
                             ExamDate = ES.ExamDate,
                             CourseTypeKey = ES.Application.Course.CourseTypeKey,
                             SubjectKey = ES.SubjectKey,
                             MinimumMark = ES.MinimumMark,
                             MaximumMark = ES.MaximumMark,
                         }).FirstOrDefault();

                if (model == null)
                {
                    model = new ExamScheduleViewModel();

                }



                FillDropDown(model);
                //model.IsSuccessful = true;

                model.IsMultipleExamSchedule = true;

                return model;
            }
            catch (Exception Ex)
            {
                ActivityLog.CreateActivityLog(MenuConstants.ExamSchedule, ActionConstants.View, DbConstants.LogType.Error, "", Ex.GetBaseException().Message);
                return new ExamScheduleViewModel();


            }
        }

        public void FillExamDetailsViewModel(ExamScheduleViewModel model)
        {
            short? SubjectYear = null;
            if (model.SubjectKey != 0 && model.SubjectKey != null)
            {
                SubjectYear = dbContext.CourseSubjectDetails.Where(x => x.SubjectKey == model.SubjectKey).Select(row => row.CourseSubjectMaster.CourseYear).FirstOrDefault();
            }
            model.ExamScheduleDetailsModel = (from A in dbContext.Applications
                                              join CSD in dbContext.CourseSubjectDetails on new { A.CourseKey, A.UniversityMasterKey, A.AcademicTermKey }
                                              equals new { CSD.CourseSubjectMaster.CourseKey, CSD.CourseSubjectMaster.UniversityMasterKey, CSD.CourseSubjectMaster.AcademicTermKey }
                                              join es in dbContext.ExamSchedules
                                              on new { CSD.SubjectKey, ApplicationKey = A.RowKey, model.ExamTermKey } equals new { es.SubjectKey, es.ApplicationKey, es.ExamTermKey } into esa
                                              from es in esa.DefaultIfEmpty()
                                              where (A.UniversityMasterKey == model.UniversityMasterKey &&
                                                A.CourseKey == model.CourseKey && A.AcademicTermKey == model.AcademicTermKey && A.BatchKey == model.BatchKey &&
                                                A.BranchKey == model.BranchKey && CSD.CourseSubjectMaster.CourseYear == SubjectYear && (es != null ? es.ExamTermKey : model.ExamTermKey) == model.ExamTermKey && CSD.SubjectKey == model.SubjectKey
                                                && A.StudentStatusKey == DbConstants.StudentStatus.Ongoing)
                                              select new ExamScheduleDetailsModel
                                              {
                                                  RowKey = es.RowKey != null ? es.RowKey : 0,
                                                  SubjectKey = es.SubjectKey != null ? es.SubjectKey : model.SubjectKey,
                                                  ApplicationKey = A.RowKey,
                                                  StudentName = A.StudentName,
                                                  AdmissionNo = A.AdmissionNo,
                                                  AppearenceCount = es.AppearenceCount != null ? es.AppearenceCount : 1,
                                                  ExamCenterKey = es.ExamCenterKey != null ? es.ExamCenterKey : 0,
                                                  ExamRegisterNumber = A.ExamRegisterNo,
                                                  IsActive = es.IsActive != null ? es.IsActive : false,
                                                  MaximumMark = es.MaximumMark ?? model.MaximumMark,
                                                  MinimumMark = es.MinimumMark ?? model.MinimumMark,
                                                  ExamStartTime = es.ExamStartTime ?? model.ExamStartTime,
                                                  ExamEndTime = es.ExamEndTime ?? model.ExamEndTime,
                                                  ExamTermKey = es.ExamTermKey != null ? es.ExamTermKey : model.ExamTermKey,
                                                  ExamStatus = es.ExamStatus ?? DbConstants.ExamStatus.Reguler,
                                                  AcademicTermKey = A.AcademicTermKey,
                                                  CourseDuration = A.Course.CourseDuration,
                                                  CurrentYear = A.CurrentYear
                                              }).ToList();

            foreach (ExamScheduleDetailsModel item in model.ExamScheduleDetailsModel)
            {
                item.CurrentYearText = CommonUtilities.GetYearDescriptionByCodeDetails(item.CourseDuration ?? 0, item.CurrentYear ?? 0, item.AcademicTermKey ?? 0);

            }
            model.ExamScheduleDetailsModel = model.ExamScheduleDetailsModel.GroupBy(x => x.ApplicationKey).Distinct().Select(x => x.First()).ToList();
            //}
            FillExamCenter(model);

        }

        public ExamScheduleViewModel UpdateExamSchedule(ExamScheduleViewModel model)
        {
            using (var transaction = dbContext.Database.BeginTransaction())
            {
                try
                {


                    CreateExamScheduleDetail(model.ExamScheduleDetailsModel.Where(row => row.RowKey == 0).ToList(), model);
                    UpdateExamScheduleDetail(model.ExamScheduleDetailsModel.Where(row => row.RowKey != 0).ToList(), model);

                    dbContext.SaveChanges();
                    transaction.Commit();

                    model.Message = EduSuiteUIResources.Success;
                    model.IsSuccessful = true;
                    ActivityLog.CreateActivityLog(MenuConstants.ExamSchedule, ActionConstants.Edit, DbConstants.LogType.Info, (model.ExamScheduleDetailsModel.Any(row => row.RowKey != 0) ? ActionConstants.Edit : ActionConstants.Add), model.Message);

                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    model.Message = String.Format(EduSuiteUIResources.FailedToSaveErrorMessage, EduSuiteUIResources.ExamSchedule);
                    model.IsSuccessful = false;
                    ActivityLog.CreateActivityLog(MenuConstants.ExamSchedule, ActionConstants.Edit, DbConstants.LogType.Error, (model.ExamScheduleDetailsModel.Any(row => row.RowKey != 0) ? ActionConstants.Edit : ActionConstants.Add), ex.GetBaseException().Message);
                }
            }
            FillDropDown(model);
            return model;
        }

        private void CreateExamScheduleDetail(List<ExamScheduleDetailsModel> modelList, ExamScheduleViewModel objViewmodel)
        {
            Int64 MaxKey = dbContext.ExamSchedules.Select(p => p.RowKey).DefaultIfEmpty().Max();

            foreach (ExamScheduleDetailsModel model in modelList)
            {
                ExamSchedule ExamDetailModel = new ExamSchedule();
                if (model.IsActive == true)
                {
                    ExamDetailModel.RowKey = Convert.ToInt64(++MaxKey);
                    ExamDetailModel.SubjectKey = model.SubjectKey ?? 0;
                    ExamDetailModel.ApplicationKey = model.ApplicationKey;
                    ExamDetailModel.ExamRegisterNumber = model.ExamRegisterNumber;
                    ExamDetailModel.IsActive = model.IsActive;
                    ExamDetailModel.ExamCenterKey = model.ExamCenterKey ?? 0;
                    ExamDetailModel.AppearenceCount = model.AppearenceCount ?? 0;
                    ExamDetailModel.ExamDate = model.ExamDate;
                    ExamDetailModel.ExamTermKey = model.ExamTermKey;
                    ExamDetailModel.MaximumMark = model.MaximumMark ?? 0;
                    ExamDetailModel.MinimumMark = model.MinimumMark ?? 0;
                    ExamDetailModel.ExamStartTime = model.ExamStartTime;
                    ExamDetailModel.ExamEndTime = model.ExamEndTime;
                    ExamDetailModel.ExamStatus = model.ExamStatus;
                    dbContext.ExamSchedules.Add(ExamDetailModel);
                }
            }
        }

        private void UpdateExamScheduleDetail(List<ExamScheduleDetailsModel> modelList, ExamScheduleViewModel objViewmodel)
        {

            foreach (ExamScheduleDetailsModel model in modelList)
            {
                ExamSchedule ExamDetailModel = new ExamSchedule();
                if (model.IsActive == true)
                {
                    ExamDetailModel = dbContext.ExamSchedules.SingleOrDefault(x => x.RowKey == model.RowKey);
                    ExamDetailModel.SubjectKey = model.SubjectKey ?? 0;
                    ExamDetailModel.ApplicationKey = model.ApplicationKey;
                    ExamDetailModel.ExamRegisterNumber = model.ExamRegisterNumber;
                    ExamDetailModel.IsActive = model.IsActive;
                    ExamDetailModel.ExamCenterKey = model.ExamCenterKey ?? 0;
                    ExamDetailModel.AppearenceCount = model.AppearenceCount ?? 0;
                    ExamDetailModel.ExamDate = model.ExamDate;
                    ExamDetailModel.ExamTermKey = model.ExamTermKey;
                    ExamDetailModel.MaximumMark = model.MaximumMark ?? 0;
                    ExamDetailModel.MinimumMark = model.MinimumMark ?? 0;
                    ExamDetailModel.ExamStartTime = model.ExamStartTime;
                    ExamDetailModel.ExamEndTime = model.ExamEndTime;
                    ExamDetailModel.ExamStatus = model.ExamStatus;

                }
            }
        }
        public List<ExamScheduleViewModel> GetExamSchedule(ExamScheduleViewModel model, out long TotalRecords)
        {
            try
            {
                var Take = model.PageSize;
                var Skip = (model.PageIndex - 1) * model.PageSize;

                IQueryable<ExamScheduleViewModel> ExamScheduleList = (from row in dbContext.ExamSchedules
                                                                      join CSD in dbContext.CourseSubjectDetails on row.SubjectKey equals CSD.SubjectKey
                                                                      select new ExamScheduleViewModel
                                                                      {

                                                                          AcademicTermKey = row.Application.AcademicTermKey,
                                                                          BranchKey = row.Application.BranchKey,
                                                                          BranchName = row.Application.Branch.BranchName,
                                                                          CourseKey = row.Application.CourseKey,
                                                                          CourseName = row.Application.Course.CourseName,
                                                                          UniversityMasterKey = row.Application.UniversityMasterKey,
                                                                          UniversityName = row.Application.UniversityMaster.UniversityMasterName,
                                                                          BatchKey = row.Application.BatchKey,
                                                                          BatchName = row.Application.Batch.BatchName,
                                                                          CurrentYear = CSD.CourseSubjectMaster.CourseYear,
                                                                          ExamTermKey = row.ExamTermKey,
                                                                          ExamTermName = row.ExamTerm.ExamTermName,
                                                                          SubjectKey = row.SubjectKey,
                                                                          SubjectName = row.Subject.SubjectName,
                                                                          ExamDate = row.ExamDate,
                                                                          CourseDuration = row.Application.Course.CourseDuration,
                                                                          NoOfStudents = dbContext.ExamSchedules.Count(x => x.Application.CourseKey == row.Application.CourseKey &&
                                                                              x.Application.UniversityMasterKey == row.Application.UniversityMasterKey &&
                                                                              x.Application.BatchKey == row.Application.BatchKey && x.Application.AcademicTermKey == row.Application.AcademicTermKey
                                                                              && x.Application.BranchKey == row.Application.BranchKey && //x.Application.CurrentYear == row.Application.CurrentYear &&
                                                                              x.ExamTermKey == row.ExamTermKey && x.SubjectKey == row.SubjectKey),
                                                                      });

                Employee Employee = dbContext.Employees.Where(x => x.AppUserKey == DbConstants.User.UserKey).SingleOrDefault();
                if (Employee != null)
                {
                    if (Employee.BranchAccess != null)
                    {
                        var Branches = Employee.BranchAccess.Split(',').Select(Int16.Parse).ToList();
                        ExamScheduleList = ExamScheduleList.Where(row => Branches.Contains(row.BranchKey ?? 0));
                    }
                }
                if (model.SearchText != "")
                {
                    ExamScheduleList = ExamScheduleList.Where(row => row.CourseName.Contains(model.SearchText) || row.BatchName.Contains(model.SearchText) || row.SubjectName.Contains(model.SearchText));
                }
                if (model.BranchKey != 0)
                {
                    ExamScheduleList = ExamScheduleList.Where(row => row.BranchKey == model.BranchKey);
                }
                if (model.BatchKey != 0)
                {
                    ExamScheduleList = ExamScheduleList.Where(row => row.BatchKey == model.BatchKey);
                }
                if (model.CourseKey != 0)
                {
                    ExamScheduleList = ExamScheduleList.Where(row => row.CourseKey == model.CourseKey);
                }
                if (model.UniversityMasterKey != 0)
                {
                    ExamScheduleList = ExamScheduleList.Where(row => row.UniversityMasterKey == model.UniversityMasterKey);
                }
                ExamScheduleList = ExamScheduleList.GroupBy(x => new { x.BranchKey, x.AcademicTermKey, x.CourseKey, x.UniversityMasterKey, x.BatchKey, x.CurrentYear, x.SubjectKey, x.ExamTermKey }).Select(y => y.FirstOrDefault());
                if (model.SortBy != "")
                {
                    ExamScheduleList = SortExamSchedule(ExamScheduleList, model.SortBy, model.SortOrder);
                }
                TotalRecords = ExamScheduleList.Count();
                return ExamScheduleList.Skip(Skip).Take(Take).ToList<ExamScheduleViewModel>();

            }
            catch (Exception ex)
            {
                TotalRecords = 0;
                ActivityLog.CreateActivityLog(MenuConstants.ExamSchedule, ActionConstants.View, DbConstants.LogType.Error, null, ex.GetBaseException().Message);
                return new List<ExamScheduleViewModel>();
            }
        }

        private IQueryable<ExamScheduleViewModel> SortExamSchedule(IQueryable<ExamScheduleViewModel> Query, string SortName, string SortOrder)
        {

            string command = SortOrder == "desc" ? "OrderByDescending" : "OrderBy";
            var type = typeof(ExamScheduleViewModel);
            var property = type.GetProperty(SortName);
            var parameter = Expression.Parameter(type, "p");
            var propertyAccess = Expression.MakeMemberAccess(parameter, property);
            var orderByExpression = Expression.Lambda(propertyAccess, parameter);
            var resultExpression = Expression.Call(typeof(Queryable), command, new Type[] { type, property.PropertyType },
                                          Query.Expression, Expression.Quote(orderByExpression));
            return Query.Provider.CreateQuery<ExamScheduleViewModel>(resultExpression);

        }

        public void FillDropDown(ExamScheduleViewModel model)
        {
            FillBranch(model);
            FillCourseType(model);
            FillAcademicTerm(model);
            if (model.CourseTypeKey == 0)
            {
                model.CourseTypeKey = dbContext.Courses.Where(row => row.RowKey == model.CourseKey).Select(row => row.CourseTypeKey).FirstOrDefault();
            }
            FillCourse(model);
            FillUniversity(model);
            FillCourseYear(model);
            FillBatch(model);
            FillExamScheduleTerm(model);
            FillSubjects(model);
            FillExamCenter(model);
        }

        private void FillBranch(ExamScheduleViewModel model)
        {
            IQueryable<SelectListModel> BranchQuery = dbContext.vwBranchSelectActiveOnlies.OrderBy(row => row.DisplayOrder).Select(row => new SelectListModel
            {
                RowKey = row.RowKey,
                Text = row.BranchName
            });

            Employee Employee = dbContext.Employees.Where(x => x.AppUserKey == DbConstants.User.UserKey).SingleOrDefault();
            if (Employee != null)
            {
                if (Employee.BranchAccess != null)
                {
                    List<long> Branches = Employee.BranchAccess.Split(',').Select(Int64.Parse).ToList();
                    model.Branches = BranchQuery.Where(row => Branches.Contains(row.RowKey)).ToList();
                    //model.BranchKey = Employee.BranchKey;
                }
                else
                {
                    model.Branches = BranchQuery.Where(x => x.RowKey == Employee.BranchKey).ToList();
                    //model.BranchKey = Employee.BranchKey;
                }
            }
            else
            {
                model.Branches = BranchQuery.ToList();
            }

            if (model.Branches.Count == 1)
            {
                long? branchkey = model.Branches.Select(x => x.RowKey).FirstOrDefault();
                model.BranchKey = Convert.ToInt16(branchkey);
            }
        }

        private void FillCourseType(ExamScheduleViewModel model)
        {
            model.CourseTypes = dbContext.VwCourseTypeSelectActiveOnlies
                                .Select(x => new SelectListModel
                                {
                                    RowKey = x.RowKey,
                                    Text = x.CourseTypeName
                                }).Distinct().ToList();


        }

        public ExamScheduleViewModel FillCourse(ExamScheduleViewModel model)
        {

            model.Courses = dbContext.VwCourseSelectActiveOnlies.Where(x => x.CourseTypeKey == model.CourseTypeKey).Select(x => new SelectListModel
            {
                RowKey = x.RowKey,
                Text = x.CourseName
            }).ToList();


            return model;
        }

        public ExamScheduleViewModel FillUniversity(ExamScheduleViewModel model)
        {

            model.Universities = (from p in dbContext.Applications
                                  join U in dbContext.VwUniversitySelectActiveOnlies on p.UniversityMasterKey equals U.RowKey
                                  orderby U.RowKey
                                  where (p.CourseKey == model.CourseKey && U.AcademicTermKey == model.AcademicTermKey)
                                  select new SelectListModel
                                  {
                                      RowKey = U.RowKey,
                                      Text = U.UniversityMasterName
                                  }).Distinct().ToList();


            return model;
        }
        private void FillAcademicTerm(ExamScheduleViewModel model)
        {
            model.AcademicTerms = dbContext.VwAcadamicTermSelectActiveOnlies.Select(x => new SelectListModel
            {
                RowKey = x.RowKey,
                Text = x.AcademicTermName,
            }).ToList();
        }
        public ExamScheduleViewModel FillCourseYear(ExamScheduleViewModel model)
        {

            var CourseDuration = dbContext.Courses.Where(row => row.RowKey == model.CourseKey).Select(row => row.CourseDuration).FirstOrDefault();

            if (CourseDuration != 0 && CourseDuration != null)
            {
                var duration = Math.Ceiling((Convert.ToDecimal(model.AcademicTermKey == DbConstants.AcademicTerm.Semester ? CourseDuration / 6 : CourseDuration / 12)));

                var StartYear = 1;
                if (duration < 1)
                {
                    model.CourseYears.Add(new SelectListModel
                    {
                        RowKey = 1,
                        Text = " Short Term"
                    });
                }
                else
                {
                    for (int i = StartYear; i <= duration; i++)
                    {
                        model.CourseYears.Add(new SelectListModel
                        {
                            RowKey = i,
                            Text = i + (model.AcademicTermKey == DbConstants.AcademicTerm.Semester ? " Semester" : " Year")
                        });
                    }
                }

            }
            return model;
        }
        public ExamScheduleViewModel FillBatch(ExamScheduleViewModel model)
        {

            model.Batches = (from p in dbContext.Applications

                             join B in dbContext.VwBatchSelectActiveOnlies on p.BatchKey equals B.RowKey
                             orderby B.RowKey
                             where (p.CourseKey == model.CourseKey && p.UniversityMasterKey == model.UniversityMasterKey && p.BranchKey == model.BranchKey)
                             select new SelectListModel
                             {
                                 RowKey = B.RowKey,
                                 Text = B.BatchName
                             }).Distinct().ToList();


            return model;
        }
        private void FillExamScheduleTerm(ExamScheduleViewModel model)
        {
            model.ExamTerm = dbContext.ExamTerms.Where(x => x.IsActive == true).Select(row => new SelectListModel
            {
                RowKey = row.RowKey,
                Text = row.ExamTermName
            }).ToList();

        }
        private void FillExamCenter(ExamScheduleViewModel model)
        {
            model.ExamCenter = dbContext.ExamCenters.Where(x => x.IsActive == true).Select(row => new SelectListModel
            {
                RowKey = row.RowKey,
                Text = row.ExamCentreName
            }).ToList();

        }
        public ExamScheduleViewModel DeleteExamSchedule(ExamScheduleViewModel model)
        {
            using (var transaction = dbContext.Database.BeginTransaction())
            {
                try
                {
                    var ExamResult = (from ER in dbContext.ExamResults
                                      join CSD in dbContext.CourseSubjectDetails on ER.SubjectKey equals CSD.SubjectKey
                                      where ER.Application.BranchKey == model.BranchKey && ER.Application.AcademicTermKey == model.AcademicTermKey && ER.Application.CourseKey == model.CourseKey &&
                                      ER.Application.UniversityMasterKey == model.UniversityMasterKey && CSD.CourseSubjectMaster.CourseYear == model.CourseYear &&
                                      ER.ExamSchedule.ExamTermKey == model.ExamTermKey && ER.Application.BatchKey == model.BatchKey && ER.SubjectKey == model.SubjectKey
                                      select ER).ToList();




                    if (ExamResult.Count() <= 0)
                    {
                        List<ExamSchedule> examScheduleList = (from ER in dbContext.ExamSchedules
                                                               join CSD in dbContext.CourseSubjectDetails on ER.SubjectKey equals CSD.SubjectKey
                                                               where ER.Application.BranchKey == model.BranchKey && ER.Application.AcademicTermKey == model.AcademicTermKey && ER.Application.CourseKey == model.CourseKey &&
                                                               ER.Application.UniversityMasterKey == model.UniversityMasterKey && CSD.CourseSubjectMaster.CourseYear == model.CourseYear &&
                                                               ER.ExamTermKey == model.ExamTermKey && ER.Application.BatchKey == model.BatchKey && ER.SubjectKey == model.SubjectKey
                                                               select ER).ToList();

                        if (examScheduleList.Count > 0)
                        {

                            dbContext.ExamSchedules.RemoveRange(examScheduleList);
                        }
                        dbContext.SaveChanges();
                        transaction.Commit();
                        model.Message = EduSuiteUIResources.Success;
                        model.IsSuccessful = true;
                        ActivityLog.CreateActivityLog(MenuConstants.ExamSchedule, ActionConstants.Delete, DbConstants.LogType.Info, null, model.Message);

                    }
                    else
                    {
                        transaction.Rollback();
                        model.Message = String.Format(EduSuiteUIResources.CantDeleteErrorMessage, EduSuiteUIResources.ExamSchedule);
                        model.IsSuccessful = false;
                        ActivityLog.CreateActivityLog(MenuConstants.ExamSchedule, ActionConstants.Delete, DbConstants.LogType.Error, null, model.Message);
                    }
                }
                catch (DbUpdateException ex)
                {
                    transaction.Rollback();
                    if (ex.GetBaseException().Message.ToUpper().Contains(EduSuiteUIResources.ForeignKeyError.ToUpper()))
                    {
                        model.Message = String.Format(EduSuiteUIResources.CantDeleteErrorMessage, EduSuiteUIResources.ExamSchedule);
                        model.IsSuccessful = false;
                        ActivityLog.CreateActivityLog(MenuConstants.ExamSchedule, ActionConstants.Delete, DbConstants.LogType.Error, null, ex.GetBaseException().Message);
                    }
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    model.Message = String.Format(EduSuiteUIResources.FailedToDeleteErrorMessage, EduSuiteUIResources.ExamSchedule);
                    model.IsSuccessful = false;
                    ActivityLog.CreateActivityLog(MenuConstants.ExamSchedule, ActionConstants.Delete, DbConstants.LogType.Error, null, ex.GetBaseException().Message);
                }
            }
            return model;
        }
        public ExamScheduleViewModel ResetExamSchedule(long Id)
        {
            ExamScheduleViewModel objviewModel = new ExamScheduleViewModel();
            using (var transaction = dbContext.Database.BeginTransaction())
            {
                try
                {

                    ExamSchedule examSchedules = dbContext.ExamSchedules.SingleOrDefault(x => x.RowKey == Id);

                    ExamResult examResults = dbContext.ExamResults.SingleOrDefault(x => x.ExamScheduleKey == Id);
                    if (examResults == null)
                    {

                        dbContext.ExamSchedules.Remove(examSchedules);

                        dbContext.SaveChanges();
                        transaction.Commit();
                        objviewModel.Message = EduSuiteUIResources.Success;
                        objviewModel.IsSuccessful = true;
                        ActivityLog.CreateActivityLog(MenuConstants.ExamSchedule, ActionConstants.Delete, DbConstants.LogType.Info, Id, objviewModel.Message);

                    }
                    else
                    {
                        transaction.Rollback();

                        objviewModel.Message = String.Format(EduSuiteUIResources.CantDeleteErrorMessage, EduSuiteUIResources.ExamSchedule);
                        objviewModel.IsSuccessful = false;
                        ActivityLog.CreateActivityLog(MenuConstants.ExamSchedule, ActionConstants.Delete, DbConstants.LogType.Debug, Id, objviewModel.Message);

                    }
                }
                catch (DbUpdateException ex)
                {
                    transaction.Rollback();
                    if (ex.GetBaseException().Message.ToUpper().Contains(EduSuiteUIResources.ForeignKeyError.ToUpper()))
                    {
                        objviewModel.Message = String.Format(EduSuiteUIResources.CantDeleteErrorMessage, EduSuiteUIResources.ExamSchedule);
                        objviewModel.IsSuccessful = false;
                        ActivityLog.CreateActivityLog(MenuConstants.ExamSchedule, ActionConstants.Delete, DbConstants.LogType.Error, Id, ex.GetBaseException().Message);
                    }
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    objviewModel.Message = String.Format(EduSuiteUIResources.FailedToDeleteErrorMessage, EduSuiteUIResources.ExamSchedule);
                    objviewModel.IsSuccessful = false;
                    ActivityLog.CreateActivityLog(MenuConstants.ExamSchedule, ActionConstants.Delete, DbConstants.LogType.Error, Id, ex.GetBaseException().Message);
                }
            }
            //}
            return objviewModel;
        }
        public ExamScheduleViewModel FillSubjects(ExamScheduleViewModel model)
        {
            model.Subjects = (from s in dbContext.Subjects
                              join cs in dbContext.CourseSubjectDetails on s.RowKey equals cs.SubjectKey
                              where (s.IsActive == true && cs.CourseSubjectMaster.CourseKey == model.CourseKey
                              && cs.CourseSubjectMaster.UniversityMasterKey == model.UniversityMasterKey &&
                              cs.CourseSubjectMaster.AcademicTermKey == model.AcademicTermKey &&
                              cs.CourseSubjectMaster.CourseYear == model.CourseYear)
                              select new SelectListModel
                              {
                                  RowKey = s.RowKey,
                                  Text = s.SubjectName
                              }).ToList();
            return model;
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
        public ExamScheduleViewModel GetExamScheduleByIndividualId(ExamScheduleViewModel model)
        {
            try
            {
                Application application = dbContext.Applications.SingleOrDefault(x => x.RowKey == model.ApplicationKey);

                model = dbContext.Applications.Where(x => x.RowKey == model.ApplicationKey).Select(row => new ExamScheduleViewModel
                {
                    ApplicationKey = row.RowKey,
                    AcademicTermKey = row.AcademicTermKey,
                    AcademicTermName = row.AcademicTerm.AcademicTermName,
                    BranchKey = row.BranchKey,
                    BranchName = row.Branch.BranchName,
                    CourseKey = row.CourseKey,
                    CourseName = row.Course.CourseName,
                    UniversityMasterKey = row.UniversityMasterKey,
                    UniversityName = row.UniversityMaster.UniversityMasterName,
                    BatchKey = row.BatchKey,
                    BatchName = row.Batch.BatchName,
                    CourseYear = row.CurrentYear,
                    //ExamTermKey = row.ExamTermKey,
                    //ExamDate = row.ExamDate,
                    CourseTypeKey = row.Course.CourseTypeKey,
                    //SubjectKey = row.SubjectKey,
                    //MinimumMark = row.MinimumMark,
                    //MaximumMark = row.MaximumMark,
                    ExamRegisterNumber = row.ExamRegisterNo,
                    StudentName = row.StudentName + EduSuiteUIResources.OpenBracket + row.AdmissionNo + EduSuiteUIResources.CloseBracket,
                    StudentEmail = row.StudentEmail,
                    StudentPhone = row.StudentMobile,
                }).SingleOrDefault();
                if (model == null)
                {
                    model = new ExamScheduleViewModel();

                }



                FillDropDown(model);
                FillNotificationDetail(model);
                //model.IsSuccessful = true;

                model.IsMultipleExamSchedule = false;
                return model;
            }
            catch (Exception Ex)
            {
                ActivityLog.CreateActivityLog(MenuConstants.ExamSchedule, ActionConstants.View, DbConstants.LogType.Error, "", Ex.GetBaseException().Message);
                return new ExamScheduleViewModel();

            }
        }

        public void FillExamDetailsIndividualViewModel(ExamScheduleViewModel model)
        {

            Application application = dbContext.Applications.SingleOrDefault(x => x.RowKey == model.ApplicationKey);



            model.ExamScheduleDetailsModel = (from CSD in dbContext.CourseSubjectDetails.Where(row => !row.Subject.ExamResults.Any(x => x.ExamSchedule.ExamTermKey == model.ExamTermKey || (x.ExamSchedule.ExamTermKey != model.ExamTermKey
                                                    && x.ExamStatus != DbConstants.ExamStatus.Reguler)) && row.CourseSubjectMaster.AcademicTermKey == application.AcademicTermKey
                                                    && row.CourseSubjectMaster.CourseKey == application.CourseKey && row.CourseSubjectMaster.UniversityMasterKey == application.UniversityMasterKey)//&& row.CourseSubjectMaster.CourseYear == model.CourseYear)
                                              join es in dbContext.ExamSchedules.Where(x => x.IsActive == true)
                                              on new { CSD.SubjectKey, ApplicationKey = application.RowKey, model.ExamTermKey } equals new { es.SubjectKey, es.ApplicationKey, es.ExamTermKey } into esa
                                              from es in esa.DefaultIfEmpty()
                                              join er in dbContext.ExamResults on es.RowKey equals er.ExamScheduleKey into erj
                                              from er in erj.DefaultIfEmpty()
                                              where ((es != null ? es.ExamTermKey : model.ExamTermKey) == model.ExamTermKey) //&&CSD.CourseSubjectMaster.CourseYear == model.CourseYear 
                                              select new ExamScheduleDetailsModel
                                              {
                                                  RowKey = es.RowKey != null ? es.RowKey : 0,
                                                  SubjectKey = CSD.SubjectKey,
                                                  ApplicationKey = application.RowKey,
                                                  SubjectName = CSD.Subject.SubjectName,
                                                  AppearenceCount = es.AppearenceCount != null ? es.AppearenceCount : 1,
                                                  ExamCenterKey = es.ExamCenterKey != null ? es.ExamCenterKey : 0,
                                                  ExamCenterName = es.ExamCenterKey != null ? dbContext.ExamCenters.Where(x => x.RowKey == es.ExamCenterKey).Select(y => y.ExamCentreName).FirstOrDefault() : "",
                                                  ExamRegisterNumber = application.ExamRegisterNo,
                                                  IsActive = es.IsActive != null ? es.IsActive : false,
                                                  MaximumMark = es.MaximumMark ?? null,
                                                  MinimumMark = es.MinimumMark ?? null,
                                                  ExamStartTime = es.ExamStartTime,
                                                  ExamEndTime = es.ExamEndTime,
                                                  ExamDate = es.ExamDate != null ? es.ExamDate : DateTimeUTC.Now,
                                                  ExamTermKey = es.ExamTermKey != null ? es.ExamTermKey : model.ExamTermKey,
                                                  ExamTermName = dbContext.ExamTerms.Where(x => x.RowKey == (es.ExamTermKey != null ? es.ExamTermKey : model.ExamTermKey)).Select(y => y.ExamTermName).FirstOrDefault(),
                                                  ExamAttempName = "",
                                                  AppearenceCountText = "",
                                                  ExamStatus = es.ExamStatus ?? DbConstants.ExamStatus.Reguler,
                                                  SubjectYear = CSD.CourseSubjectMaster.CourseYear,
                                                  AcademicTermKey = CSD.CourseSubjectMaster.AcademicTermKey,
                                                  CourseDuration = CSD.CourseSubjectMaster.Course.CourseDuration
                                              }).Union(from CSD in dbContext.CourseSubjectDetails.Where(row => row.CourseSubjectMaster.AcademicTermKey == application.AcademicTermKey
                                                  && row.CourseSubjectMaster.CourseKey == application.CourseKey && row.CourseSubjectMaster.UniversityMasterKey == application.UniversityMasterKey
                                                  )//&& row.CourseSubjectMaster.CourseYear == model.CourseYear)
                                                       join es in dbContext.ExamSchedules.GroupBy(row => row.ExamTermKey).Select(row => row.OrderByDescending(x => x.AppearenceCount).FirstOrDefault())
                                                       on new { CSD.SubjectKey, ApplicationKey = application.RowKey } equals new { es.SubjectKey, es.ApplicationKey }
                                                       join er in dbContext.ExamResults.Where(row => row.ExamSchedule.IsActive == false && (row.ExamStatus != DbConstants.ExamStatus.Reguler && row.ExamSchedule.ExamTermKey != model.ExamTermKey)).GroupBy(row => row.SubjectKey).Select(row => row.OrderByDescending(x => x.ExamSchedule.AppearenceCount).FirstOrDefault())
                                                       on es.RowKey equals er.ExamScheduleKey
                                                       //where (CSD.CourseSubjectMaster.CourseYear == model.CourseYear)
                                                       select new ExamScheduleDetailsModel
                                                       {
                                                           RowKey = 0,
                                                           SubjectKey = CSD.SubjectKey,
                                                           ApplicationKey = application.RowKey,
                                                           SubjectName = CSD.Subject.SubjectName,
                                                           AppearenceCount = er.ExamSchedule.AppearenceCount + 1,
                                                           ExamCenterKey = 0,
                                                           ExamCenterName = "",
                                                           ExamRegisterNumber = application.ExamRegisterNo,
                                                           IsActive = false,
                                                           MaximumMark = null,
                                                           MinimumMark = null,
                                                           ExamStartTime = null,
                                                           ExamEndTime = null,
                                                           ExamDate = DateTime.UtcNow,
                                                           ExamTermKey = model.ExamTermKey,
                                                           ExamTermName = dbContext.ExamTerms.Where(x => x.RowKey == model.ExamTermKey).Select(y => y.ExamTermName).FirstOrDefault(),
                                                           ExamAttempName = (er.ExamStatus == DbConstants.ExamStatus.Supply ? EduSuiteUIResources.SupplyExam : er.ExamStatus == DbConstants.ExamStatus.Improvement ? EduSuiteUIResources.ImprovementExam : ""),
                                                           AppearenceCountText = "",
                                                           ExamStatus = er.ExamStatus ?? DbConstants.ExamStatus.Reguler,
                                                           SubjectYear = CSD.CourseSubjectMaster.CourseYear,
                                                           AcademicTermKey = CSD.CourseSubjectMaster.AcademicTermKey,
                                                           CourseDuration = CSD.CourseSubjectMaster.Course.CourseDuration
                                                       }).ToList();
            foreach (ExamScheduleDetailsModel objmodel in model.ExamScheduleDetailsModel)
            {
                objmodel.SubjectYearName = CommonUtilities.GetYearDescriptionByCodeDetails(objmodel.CourseDuration ?? 0, objmodel.SubjectYear ?? 0, objmodel.AcademicTermKey ?? 0);

            }

            //model.ExamScheduleDetailsModel =(from row in dbContext)

            FillExamCenter(model);

        }



        private void FillNotificationDetail(ExamScheduleViewModel model)
        {
            NotificationTemplate notificationTemplateModel = dbContext.NotificationTemplates.SingleOrDefault(row => row.RowKey == DbConstants.NotificationTemplate.ExamSchedule);
            if (notificationTemplateModel != null)
            {
                model.AutoEmail = notificationTemplateModel.AutoEmail;
                model.AutoSMS = notificationTemplateModel.AutoSMS;
                model.TemplateKey = notificationTemplateModel.RowKey;
            }
        }

    }
}
