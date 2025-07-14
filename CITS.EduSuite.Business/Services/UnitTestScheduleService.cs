using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CITS.EduSuite.Business.Interfaces;
using CITS.EduSuite.Business.Common;
using CITS.EduSuite.Data;
using CITS.EduSuite.Business.Models.ViewModels;
using System.Data.Entity.Infrastructure;
using System.Data.Entity;
using CITS.EduSuite.Business.Models.Resources;
using System.Linq.Expressions;

namespace CITS.EduSuite.Business.Services
{
    public class UnitTestScheduleService : IUnitTestScheduleService
    {
        private EduSuiteDatabase dbContext;

        public UnitTestScheduleService(EduSuiteDatabase Objdb)
        {
            this.dbContext = Objdb;

        }

        public UnitTestScheduleViewModel GetUnitTestScheduleById(UnitTestScheduleViewModel model)
        {
            try
            {
                UnitTestScheduleViewModel objviewmodel = new UnitTestScheduleViewModel();

                objviewmodel = dbContext.UnitTestSchedules.Where(row => row.RowKey == model.RowKey).Select(row => new UnitTestScheduleViewModel
                {
                    RowKey = row.RowKey,
                    IsUpdate = true,
                    BranchKey = row.BranchKey,
                    SubjectKey = row.SubjectKey ?? 0,
                    BatchKey = row.BatchKey,
                    SubjectModuleKey = row.SubjectModuleKey ?? 0,
                    ClassDetailsKey = row.ClassDetailsKey,
                    ExamDate = row.ExamDate,
                    MinimumMark = row.MinimumMark,
                    MaximumMark = row.MaximumMark,
                    ModuleTopicKeys = row.UnitTestTopics.Select(x => x.TopicKey).ToList(),
                    CourseTypeKey = row.ClassDetail.UniversityCourse.Course.CourseTypeKey,
                    EmployeeKey = row.EmployeeKey ?? 0

                }).SingleOrDefault();

                if (objviewmodel == null)
                {
                    objviewmodel = new UnitTestScheduleViewModel();
                }

                FillDropDown(objviewmodel);
                return objviewmodel;
            }
            catch (Exception Ex)
            {
                ActivityLog.CreateActivityLog(MenuConstants.UnitTestResult, ActionConstants.View, DbConstants.LogType.Error, model.RowKey, Ex.GetBaseException().Message);
                return new UnitTestScheduleViewModel();

            }
        }



        public UnitTestScheduleViewModel FillUnitTestDetailsViewModel(UnitTestScheduleViewModel model)
        {
            if (model.RowKey != 0 && model.RowKey != null)
            {
                var CheckQuery = dbContext.UnitTestSchedules.Where(x => x.BranchKey == model.BranchKey && x.BatchKey == model.BatchKey && x.ClassDetailsKey == model.ClassDetailsKey && x.SubjectKey == model.SubjectKey &&
                          x.SubjectModuleKey == model.SubjectModuleKey && x.ExamDate == model.ExamDate && x.RowKey != model.RowKey && x.UnitTestTopics.Any(y => model.ModuleTopicKeys.Contains(y.TopicKey))).Select(row => row.RowKey);
                if (CheckQuery.Any())
                {
                    //model.RowKey = CheckQuery.FirstOrDefault();
                    model.IsSuccessful = false;
                    model.Message = EduSuiteUIResources.UnitTestExist;
                    return model;
                }

            }
            else
            {
                var CheckQuery = dbContext.UnitTestSchedules.Where(x => x.BranchKey == model.BranchKey && x.BatchKey == model.BatchKey && x.ClassDetailsKey == model.ClassDetailsKey && x.SubjectKey == model.SubjectKey &&
                           x.SubjectModuleKey == model.SubjectModuleKey && x.ExamDate == model.ExamDate && x.UnitTestTopics.Any(y => model.ModuleTopicKeys.Contains(y.TopicKey))).Select(row => row.RowKey);
                if (CheckQuery.Any())
                {
                    //model.RowKey = CheckQuery.FirstOrDefault();
                    model.IsSuccessful = false;
                    model.Message = EduSuiteUIResources.UnitTestExist;
                    return model;
                }

            }
            model.UnitTestResultViewModel = dbContext.UnitTestResults.Where(x => x.UnitTestScheduleKey == model.RowKey).Select(x => new UnitTestResultViewModel
            {
                RowKey = x.RowKey,
                UnitTestScheduleKey = x.UnitTestScheduleKey,
                ApplicationKey = x.Application.RowKey,
                StudentName = x.Application.StudentName,
                AdmissionNo = x.Application.AdmissionNo,
                ResultStatus = x.ResultStatus,
                Mark = x.Mark,
                Remarks = x.Remarks,
                AbsentStatus = (x.ResultStatus == DbConstants.ResultStatus.Absent ? true : false),
            }).ToList();



            if (model.UnitTestResultViewModel.Count == 0)
            {
                model.UnitTestResultViewModel = (from A in dbContext.Applications
                                                 where (A.BranchKey == model.BranchKey && A.BatchKey == model.BatchKey &&
                                             A.ClassDetailsKey == model.ClassDetailsKey)
                                                 select new UnitTestResultViewModel
                                                 {
                                                     RowKey = 0,
                                                     UnitTestScheduleKey = 0,
                                                     ApplicationKey = A.RowKey,
                                                     StudentName = A.StudentName,
                                                     AdmissionNo = A.AdmissionNo,
                                                     ResultStatus = "",
                                                     Mark = null,
                                                     Remarks = "",
                                                     AbsentStatus = false,

                                                 }).ToList();
            }

            return model;
        }

        public UnitTestScheduleViewModel CreateUnitTestSchedule(UnitTestScheduleViewModel model)
        {
            using (var transaction = dbContext.Database.BeginTransaction())
            {
                try
                {


                    long maxKey = dbContext.UnitTestSchedules.Select(p => p.RowKey).DefaultIfEmpty().Max();

                    UnitTestSchedule UnitTestScheduleModel = new UnitTestSchedule();
                    UnitTestScheduleModel.RowKey = Convert.ToInt64(maxKey + 1);
                    UnitTestScheduleModel.SubjectKey = model.SubjectKey;
                    UnitTestScheduleModel.BatchKey = model.BatchKey;
                    UnitTestScheduleModel.SubjectModuleKey = model.SubjectModuleKey;
                    UnitTestScheduleModel.ClassDetailsKey = model.ClassDetailsKey;
                    UnitTestScheduleModel.BranchKey = model.BranchKey ?? 0;
                    UnitTestScheduleModel.ExamDate = model.ExamDate;
                    UnitTestScheduleModel.MinimumMark = model.MinimumMark;
                    UnitTestScheduleModel.MaximumMark = model.MaximumMark;
                    UnitTestScheduleModel.EmployeeKey = model.EmployeeKey;
                    dbContext.UnitTestSchedules.Add(UnitTestScheduleModel);

                    CreateUnitTestResult(model.UnitTestResultViewModel.Where(row => row.RowKey == 0).ToList(), UnitTestScheduleModel);
                    CreateUnitTestTopc(model, UnitTestScheduleModel);
                    dbContext.SaveChanges();
                    transaction.Commit();
                    model.RowKey = UnitTestScheduleModel.RowKey;
                    model.Message = EduSuiteUIResources.Success;
                    model.IsSuccessful = true;
                    ActivityLog.CreateActivityLog(MenuConstants.UnitTestResult, ActionConstants.Add, DbConstants.LogType.Info, model.RowKey, model.Message);

                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    model.Message = String.Format(EduSuiteUIResources.FailedToSaveErrorMessage, EduSuiteUIResources.UnitTest);
                    model.IsSuccessful = false;
                    ActivityLog.CreateActivityLog(MenuConstants.UnitTestResult, ActionConstants.Add, DbConstants.LogType.Error, model.RowKey, ex.GetBaseException().Message);
                }
            }
            FillDropDown(model);
            return model;

        }

        public UnitTestScheduleViewModel UpdateUnitTestSchedule(UnitTestScheduleViewModel model)
        {
            using (var transaction = dbContext.Database.BeginTransaction())
            {
                try
                {

                    UnitTestSchedule UnitTestScheduleModel = new UnitTestSchedule();
                    UnitTestScheduleModel = dbContext.UnitTestSchedules.SingleOrDefault(row => row.RowKey == model.RowKey);
                    UnitTestScheduleModel.SubjectKey = model.SubjectKey;
                    UnitTestScheduleModel.BatchKey = model.BatchKey;
                    UnitTestScheduleModel.SubjectModuleKey = model.SubjectModuleKey;
                    UnitTestScheduleModel.ClassDetailsKey = model.ClassDetailsKey;
                    UnitTestScheduleModel.BranchKey = model.BranchKey ?? 0;
                    UnitTestScheduleModel.ExamDate = model.ExamDate;
                    UnitTestScheduleModel.MinimumMark = model.MinimumMark;
                    UnitTestScheduleModel.MaximumMark = model.MaximumMark;
                    UnitTestScheduleModel.EmployeeKey = model.EmployeeKey;

                    CreateUnitTestResult(model.UnitTestResultViewModel.Where(row => row.RowKey == 0).ToList(), UnitTestScheduleModel);
                    UpdateUnitTestResult(model.UnitTestResultViewModel.Where(row => row.RowKey != 0).ToList(), UnitTestScheduleModel);
                    CreateUnitTestTopc(model, UnitTestScheduleModel);

                    dbContext.SaveChanges();
                    transaction.Commit();

                    model.Message = EduSuiteUIResources.Success;
                    model.IsSuccessful = true;
                    ActivityLog.CreateActivityLog(MenuConstants.UnitTestResult, ActionConstants.Edit, DbConstants.LogType.Info, model.RowKey, model.Message);

                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    model.Message = String.Format(EduSuiteUIResources.FailedToSaveErrorMessage, EduSuiteUIResources.UnitTest);
                    model.IsSuccessful = false;
                    ActivityLog.CreateActivityLog(MenuConstants.UnitTestResult, ActionConstants.Edit, DbConstants.LogType.Error, model.RowKey, ex.GetBaseException().Message);
                }
            }
            FillDropDown(model);
            return model;
        }

        private void CreateUnitTestResult(List<UnitTestResultViewModel> modelList, UnitTestSchedule objViewmodel)
        {
            Int64 MaxKey = dbContext.UnitTestResults.Select(p => p.RowKey).DefaultIfEmpty().Max();

            foreach (UnitTestResultViewModel model in modelList)
            {
                UnitTestResult UnitTestResultViewModel = new UnitTestResult();

                UnitTestResultViewModel.RowKey = Convert.ToInt64(MaxKey + 1);
                UnitTestResultViewModel.ApplicationKey = model.ApplicationKey;
                UnitTestResultViewModel.UnitTestScheduleKey = objViewmodel.RowKey;
                UnitTestResultViewModel.ResultStatus = model.ResultStatus;
                UnitTestResultViewModel.Mark = model.Mark;
                UnitTestResultViewModel.Remarks = model.Remarks;

                dbContext.UnitTestResults.Add(UnitTestResultViewModel);
                MaxKey++;
            }
        }

        private void UpdateUnitTestResult(List<UnitTestResultViewModel> modelList, UnitTestSchedule objViewmodel)
        {

            foreach (UnitTestResultViewModel model in modelList)
            {
                UnitTestResult UnitTestResultViewModel = new UnitTestResult();

                UnitTestResultViewModel = dbContext.UnitTestResults.SingleOrDefault(x => x.RowKey == model.RowKey);
                UnitTestResultViewModel.ApplicationKey = model.ApplicationKey;
                UnitTestResultViewModel.UnitTestScheduleKey = objViewmodel.RowKey;
                UnitTestResultViewModel.ResultStatus = model.ResultStatus;
                UnitTestResultViewModel.Mark = model.Mark;
                UnitTestResultViewModel.Remarks = model.Remarks;

            }
        }

        private void CreateUnitTestTopc(UnitTestScheduleViewModel model, UnitTestSchedule objViewmodel)
        {

            Int64 MaxKey = dbContext.UnitTestTopics.Select(p => p.RowKey).DefaultIfEmpty().Max();
            List<UnitTestTopic> UnitTestTopicsList = dbContext.UnitTestTopics.Where(x => x.UnitTestScheduleKey == objViewmodel.RowKey).ToList();
            if (UnitTestTopicsList.Count > 0)
            {
                dbContext.UnitTestTopics.RemoveRange(UnitTestTopicsList);
            }
            foreach (short ModuleTopicKey in model.ModuleTopicKeys)
            {

                UnitTestTopic unitTestTopicModel = new UnitTestTopic();
                unitTestTopicModel.RowKey = Convert.ToInt64(MaxKey + 1);
                unitTestTopicModel.UnitTestScheduleKey = objViewmodel.RowKey;
                unitTestTopicModel.TopicKey = ModuleTopicKey;
                unitTestTopicModel.SubjectKey = objViewmodel.SubjectKey;
                unitTestTopicModel.SubjectModuleKey = objViewmodel.SubjectModuleKey;

                dbContext.UnitTestTopics.Add(unitTestTopicModel);
                MaxKey++;

            }
        }

        public List<UnitTestScheduleViewModel> GetUnitTestSchedule(UnitTestScheduleViewModel model, out long TotalRecords)
        {
            try
            {
                var Take = model.PageSize;
                var Skip = (model.PageIndex - 1) * model.PageSize;

                IQueryable<UnitTestScheduleViewModel> UnitTestList = (from US in dbContext.UnitTestSchedules
                                                                      join UR in dbContext.UnitTestResults on US.RowKey equals UR.UnitTestScheduleKey

                                                                      where ((US.Subject.SubjectName.Contains(model.searchText)) || (US.SubjectModule.ModuleName.Contains(model.searchText)))
                                                                      select new UnitTestScheduleViewModel
                                                                      {
                                                                          RowKey = US.RowKey,
                                                                          ClassDetailsKey = US.ClassDetailsKey,
                                                                          ClassDetailsName = US.ClassDetail.ClassCode,
                                                                          BranchKey = US.BranchKey,
                                                                          BranchName = US.Branch.BranchName,
                                                                          SubjectKey = US.SubjectKey ?? 0,
                                                                          SubjectName = US.Subject.SubjectName,
                                                                          SubjectModuleKey = US.SubjectModuleKey ?? 0,
                                                                          SubjectModuleName = US.SubjectModule.ModuleName,
                                                                          BatchKey = US.BatchKey,
                                                                          BatchName = US.Batch.BatchName,
                                                                          ExamDate = US.ExamDate,
                                                                          ModuleTopicsCount = US.UnitTestTopics.Count(x => x.UnitTestScheduleKey == US.RowKey)
                                                                      });

                Employee Employee = dbContext.Employees.Where(x => x.AppUserKey == DbConstants.User.UserKey).SingleOrDefault();
                if (Employee != null)
                {
                    if (Employee.BranchAccess != null)
                    {
                        var Branches = Employee.BranchAccess.Split(',').Select(Int16.Parse).ToList();
                        UnitTestList = UnitTestList.Where(row => Branches.Contains(row.BranchKey ?? 0));
                    }
                }

                if (model.BranchKey != 0)
                {
                    UnitTestList = UnitTestList.Where(row => row.BranchKey == model.BranchKey);
                }
                if (model.ClassDetailsKey != 0)
                {
                    UnitTestList = UnitTestList.Where(row => row.ClassDetailsKey == model.ClassDetailsKey);
                }
                if (model.BatchKey != 0)
                {
                    UnitTestList = UnitTestList.Where(row => row.BatchKey == model.BatchKey);
                }
                if (model.SearchDate != null)
                {
                    UnitTestList = UnitTestList.Where(x => System.Data.Entity.DbFunctions.TruncateTime(x.ExamDate) == System.Data.Entity.DbFunctions.TruncateTime(model.SearchDate));
                }
                UnitTestList = UnitTestList.GroupBy(x => x.RowKey).Select(y => y.FirstOrDefault());
                if (model.SortBy != "")
                {
                    UnitTestList = SortApplications(UnitTestList, model.SortBy, model.SortOrder);
                }
                TotalRecords = UnitTestList.Count();
                return UnitTestList.Skip(Skip).Take(Take).ToList<UnitTestScheduleViewModel>();


            }
            catch (Exception ex)
            {
                TotalRecords = 0;
                ActivityLog.CreateActivityLog(MenuConstants.UnitTestResult, ActionConstants.View, DbConstants.LogType.Error, null, ex.GetBaseException().Message);
                return new List<UnitTestScheduleViewModel>();
            }
        }

        private IQueryable<UnitTestScheduleViewModel> SortApplications(IQueryable<UnitTestScheduleViewModel> Query, string SortName, string SortOrder)
        {

            string command = SortOrder == "desc" ? "OrderByDescending" : "OrderBy";
            var type = typeof(UnitTestScheduleViewModel);
            var property = type.GetProperty(SortName);
            var parameter = Expression.Parameter(type, "p");
            var propertyAccess = Expression.MakeMemberAccess(parameter, property);
            var orderByExpression = Expression.Lambda(propertyAccess, parameter);
            var resultExpression = Expression.Call(typeof(Queryable), command, new Type[] { type, property.PropertyType },
                                          Query.Expression, Expression.Quote(orderByExpression));
            return Query.Provider.CreateQuery<UnitTestScheduleViewModel>(resultExpression);

        }


        public UnitTestScheduleViewModel DeleteUnitTest(UnitTestScheduleViewModel model)
        {
            using (var transaction = dbContext.Database.BeginTransaction())
            {
                try
                {
                    UnitTestSchedule unitTestSchedule = dbContext.UnitTestSchedules.SingleOrDefault(x => x.RowKey == model.RowKey);

                    List<UnitTestResult> UnitTestResultList = dbContext.UnitTestResults.Where(x => x.UnitTestScheduleKey == model.RowKey).ToList();
                    List<UnitTestTopic> UnitTestTopicList = dbContext.UnitTestTopics.Where(x => x.UnitTestScheduleKey == model.RowKey).ToList();

                    dbContext.UnitTestTopics.RemoveRange(UnitTestTopicList);
                    dbContext.UnitTestResults.RemoveRange(UnitTestResultList);
                    dbContext.UnitTestSchedules.Remove(unitTestSchedule);




                    dbContext.SaveChanges();
                    transaction.Commit();
                    model.Message = EduSuiteUIResources.Success;
                    model.IsSuccessful = true;
                    ActivityLog.CreateActivityLog(MenuConstants.UnitTestResult, ActionConstants.Delete, DbConstants.LogType.Info, model.RowKey, model.Message);

                    //}
                    //else
                    //{
                    //    transaction.Rollback();
                    //    model.Message = EduSuiteUIResources.CantDeleteInternalExamScheduleResult;
                    //    model.IsSuccessful = false;
                    //    ActivityLog.CreateActivityLog(MenuConstants.InternalExam, ActionConstants.Delete, DbConstants.LogType.Error, model.RowKey, model.Message);
                    //}
                }
                catch (DbUpdateException ex)
                {
                    transaction.Rollback();
                    if (ex.GetBaseException().Message.ToUpper().Contains(EduSuiteUIResources.ForeignKeyError.ToUpper()))
                    {
                        model.Message = String.Format(EduSuiteUIResources.CantDeleteErrorMessage, EduSuiteUIResources.UnitTest);
                        model.IsSuccessful = false;
                        ActivityLog.CreateActivityLog(MenuConstants.UnitTestResult, ActionConstants.Delete, DbConstants.LogType.Error, model.RowKey, ex.GetBaseException().Message);
                    }
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    model.Message = String.Format(EduSuiteUIResources.FailedToDeleteErrorMessage, EduSuiteUIResources.UnitTest);
                    model.IsSuccessful = false;
                    ActivityLog.CreateActivityLog(MenuConstants.UnitTestResult, ActionConstants.Delete, DbConstants.LogType.Error, model.RowKey, ex.GetBaseException().Message);
                }
            }
            return model;
        }
        #region DropDownChange Events
        public void FillDropDown(UnitTestScheduleViewModel model)
        {
            FillBranch(model);
            FillCourseType(model);
            FillClassDetails(model);
            FillBatch(model);
            FillTeacher(model);
            FillSubjects(model);

            FillSubjectModules(model);
            FillModuleTopics(model);

            //FillDivision(model);


        }
        private void FillBranch(UnitTestScheduleViewModel model)
        {
            IQueryable<SelectListModel> BranchQuery = dbContext.vwBranchSelectActiveOnlies.Where(x => x.IsActive).OrderBy(row => row.DisplayOrder).Select(row => new SelectListModel
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

        public UnitTestScheduleViewModel FillTeacher(UnitTestScheduleViewModel model)
        {
            if (!DbConstants.Role.AdminUserTypes.Contains(DbConstants.User.RoleKey))
            {
                Employee Employee = dbContext.Employees.Where(x => x.AppUserKey == DbConstants.User.UserKey && x.IsTeacher == true).SingleOrDefault();
                if (Employee != null)
                {
                    model.Teachers = dbContext.VwTeacherSelectActiveOnlies.Where(x => x.RowKey == Employee.RowKey).Select(x => new SelectListModel
                    {
                        RowKey = x.RowKey,
                        Text = x.FirstName
                    }).Distinct().ToList();
                }
                else
                {
                    if (model.ClassDetailsKey != 0 && model.ClassDetailsKey != null)
                    {
                        model.Teachers = dbContext.VwTeacherSelectActiveOnlies.Where(x => x.ClassDetailsKey == model.ClassDetailsKey && x.BatchKey == model.BatchKey).Select(x => new SelectListModel
                        {
                            RowKey = x.RowKey,
                            Text = x.FirstName
                        }).Distinct().ToList();
                    }
                    else
                    {
                        model.Teachers = dbContext.VwTeacherSelectActiveOnlies.Select(x => new SelectListModel
                        {
                            RowKey = x.RowKey,
                            Text = x.FirstName
                        }).Distinct().ToList();
                    }
                }
            }
            else
            {
                model.Teachers = dbContext.VwTeacherSelectActiveOnlies.Select(x => new SelectListModel
                {
                    RowKey = x.RowKey,
                    Text = x.FirstName
                }).Distinct().ToList();

            }

            return model;
        }
        public UnitTestScheduleViewModel FillSubjects(UnitTestScheduleViewModel model)
        {

            IQueryable<SelectListModel> SubjectQuery = dbContext.ModuleTopics.Where(x => x.SubjectModule.Subject.IsActive).Select(row => new SelectListModel
            {
                RowKey = row.SubjectModule.SubjectKey,
                Text = row.SubjectModule.Subject.SubjectName
            });

            if (model.ClassDetailsKey != 0 && model.ClassDetailsKey != null)
            {
                if (!DbConstants.Role.AdminUserTypes.Contains(DbConstants.User.RoleKey))
                {
                    Employee Employee = dbContext.Employees.Where(x => x.AppUserKey == DbConstants.User.UserKey && x.IsTeacher == true).SingleOrDefault();
                    if (Employee != null && model.BatchKey != 0 && model.BatchKey != null)
                    {

                        TeacherClassAllocation teacherclass = dbContext.TeacherClassAllocations.SingleOrDefault(x => x.EmployeeKey == Employee.RowKey && x.ClassDetailsKey == model.ClassDetailsKey && x.BatchKey == model.BatchKey);

                        model.Subjects = (from SQ in SubjectQuery
                                          join TSA in dbContext.TeacherSubjectAllocations on SQ.RowKey equals TSA.SubjectKey
                                          where (TSA.Employeekey == Employee.RowKey && TSA.TeacherClassAllocationKey == teacherclass.RowKey)
                                          select new SelectListModel
                                          {
                                              RowKey = SQ.RowKey,
                                              Text = SQ.Text
                                          }).Distinct().ToList();
                    }
                    else
                    {
                        ClassDetail classDetailList = dbContext.ClassDetails.SingleOrDefault(x => x.RowKey == model.ClassDetailsKey);

                        //model.Subjects = dbContext.VwSubjectSelectActiveOnlies.Where(x => x.CourseKey == classDetailList.UniversityCourse.CourseKey && x.UniversityMasterKey == classDetailList.UniversityCourse.UniversityMasterKey && x.AcademicTermKey == classDetailList.UniversityCourse.AcademicTermKey && x.CourseYear == classDetailList.StudentYear).Select(x => new SelectListModel
                        //{
                        //    RowKey = x.RowKey,
                        //    Text = x.SubjectName
                        //}).ToList();

                        model.Subjects = (from SQ in SubjectQuery
                                          join TSA in dbContext.VwSubjectSelectActiveOnlies on SQ.RowKey equals TSA.RowKey
                                          where (TSA.CourseKey == classDetailList.UniversityCourse.CourseKey && TSA.UniversityMasterKey == classDetailList.UniversityCourse.UniversityMasterKey && TSA.AcademicTermKey == classDetailList.UniversityCourse.AcademicTermKey && TSA.CourseYear == classDetailList.StudentYear)
                                          select new SelectListModel
                                          {
                                              RowKey = SQ.RowKey,
                                              Text = SQ.Text
                                          }).Distinct().ToList();

                    }

                }

                else
                {
                    ClassDetail classDetailList = dbContext.ClassDetails.SingleOrDefault(x => x.RowKey == model.ClassDetailsKey);

                    //model.Subjects = dbContext.VwSubjectSelectActiveOnlies.Where(x => x.CourseKey == classDetailList.UniversityCourse.CourseKey && x.UniversityMasterKey == classDetailList.UniversityCourse.UniversityMasterKey && x.AcademicTermKey == classDetailList.UniversityCourse.AcademicTermKey && x.CourseYear == classDetailList.StudentYear).Select(x => new SelectListModel
                    //{
                    //    RowKey = x.RowKey,
                    //    Text = x.SubjectName
                    //}).ToList();

                    model.Subjects = (from SQ in SubjectQuery
                                      join TSA in dbContext.VwSubjectSelectActiveOnlies on SQ.RowKey equals TSA.RowKey
                                      where (TSA.CourseKey == classDetailList.UniversityCourse.CourseKey && TSA.UniversityMasterKey == classDetailList.UniversityCourse.UniversityMasterKey && TSA.AcademicTermKey == classDetailList.UniversityCourse.AcademicTermKey && TSA.CourseYear == classDetailList.StudentYear)
                                      select new SelectListModel
                                      {
                                          RowKey = SQ.RowKey,
                                          Text = SQ.Text
                                      }).Distinct().ToList();
                }
            }
            return model;

        }
        public UnitTestScheduleViewModel FillSubjectModules(UnitTestScheduleViewModel model)
        {

            model.SubjectModules = dbContext.ModuleTopics.Where(x => x.SubjectModule.SubjectKey == model.SubjectKey).Select(x => new SelectListModel
            {
                RowKey = x.SubjectModule.RowKey,
                Text = x.SubjectModule.ModuleName
            }).Distinct().ToList();


            return model;
        }
        public UnitTestScheduleViewModel FillModuleTopics(UnitTestScheduleViewModel model)
        {

            model.ModuleTopics = dbContext.ModuleTopics.Where(x => x.SubjectModuleKey == model.SubjectModuleKey).Select(x => new SelectListModel
            {
                RowKey = x.RowKey,
                Text = x.TopicName
            }).ToList();


            return model;
        }
        public UnitTestScheduleViewModel FillBatch(UnitTestScheduleViewModel model)
        {
            if (!DbConstants.Role.AdminUserTypes.Contains(DbConstants.User.RoleKey))
            {
                Employee Employee = dbContext.Employees.Where(x => x.AppUserKey == DbConstants.User.UserKey && x.IsTeacher == true).SingleOrDefault();
                if (Employee != null)
                {
                    model.Batches = (from p in dbContext.Applications
                                     join SDA in dbContext.StudentDivisionAllocations on p.RowKey equals SDA.ApplicationKey
                                     join B in dbContext.VwBatchSelectActiveOnlies on p.BatchKey equals B.RowKey
                                     join TCA in dbContext.TeacherClassAllocations on B.RowKey equals TCA.BatchKey
                                     orderby B.RowKey
                                     where (p.ClassDetailsKey == model.ClassDetailsKey && TCA.EmployeeKey == Employee.RowKey && TCA.IsActive == true && TCA.IsAttendance == true && model.IsUpdate != true ? SDA.IsActive == true : SDA.IsActive != null && p.BranchKey == Employee.BranchKey)
                                     select new SelectListModel
                                     {
                                         RowKey = B.RowKey,
                                         Text = B.BatchName
                                     }).Distinct().ToList();
                }
                else
                {
                    model.Batches = (from p in dbContext.Applications
                                     join SDA in dbContext.StudentDivisionAllocations on p.RowKey equals SDA.ApplicationKey
                                     join B in dbContext.VwBatchSelectActiveOnlies on p.BatchKey equals B.RowKey
                                     orderby B.RowKey
                                     where (p.ClassDetailsKey == model.ClassDetailsKey && p.BranchKey == model.BranchKey && model.IsUpdate != true ? SDA.IsActive == true : SDA.IsActive != null && p.BranchKey == model.BranchKey)
                                     select new SelectListModel
                                     {
                                         RowKey = B.RowKey,
                                         Text = B.BatchName
                                     }).Distinct().ToList();
                }
            }

            else
            {
                model.Batches = (from p in dbContext.Applications
                                 join SDA in dbContext.StudentDivisionAllocations on p.RowKey equals SDA.ApplicationKey
                                 join B in dbContext.VwBatchSelectActiveOnlies on p.BatchKey equals B.RowKey
                                 orderby B.RowKey
                                 where (p.ClassDetailsKey == model.ClassDetailsKey && p.BranchKey == model.BranchKey)
                                 select new SelectListModel
                                 {
                                     RowKey = B.RowKey,
                                     Text = B.BatchName
                                 }).Distinct().ToList();
            }

            return model;
        }

        public UnitTestScheduleViewModel FillClassDetails(UnitTestScheduleViewModel model)
        {

            if (!DbConstants.Role.AdminUserTypes.Contains(DbConstants.User.RoleKey))
            {
                Employee Employee = dbContext.Employees.Where(x => x.AppUserKey == DbConstants.User.UserKey && x.IsTeacher == true).SingleOrDefault();
                if (Employee != null)
                {
                    model.ClassDetails = (from CD in dbContext.VwClassDetailsSelectActiveOnlies

                                          join SDA in dbContext.StudentDivisionAllocations on CD.RowKey equals SDA.ClassDetailsKey
                                          join TCA in dbContext.TeacherClassAllocations on CD.RowKey equals TCA.ClassDetailsKey
                                          join A in dbContext.Applications on CD.RowKey equals A.ClassDetailsKey
                                          where (CD.CourseTypeKey == model.CourseTypeKey && TCA.EmployeeKey == Employee.RowKey && TCA.IsActive == true && TCA.IsAttendance == true && (model.IsUpdate != true ? SDA.IsActive == true : SDA.IsActive != null) && A.BranchKey == Employee.BranchKey)
                                          select new SelectListModel
                                          {
                                              RowKey = CD.RowKey,
                                              Text = CD.ClassCode + CD.ClassCodeDescription
                                          }).Distinct().ToList();

                }
                else
                {
                    model.ClassDetails = (from CD in dbContext.VwClassDetailsSelectActiveOnlies

                                          join SDA in dbContext.StudentDivisionAllocations on CD.RowKey equals SDA.ClassDetailsKey
                                          join A in dbContext.Applications on CD.RowKey equals A.ClassDetailsKey
                                          where (CD.CourseTypeKey == model.CourseTypeKey && (model.IsUpdate != true ? SDA.IsActive == true : SDA.IsActive != null) && A.BranchKey == model.BranchKey)
                                          select new SelectListModel
                                          {
                                              RowKey = CD.RowKey,
                                              Text = CD.ClassCode + CD.ClassCodeDescription
                                          }).Distinct().ToList();
                }
            }

            else
            {
                model.ClassDetails = (from CD in dbContext.VwClassDetailsSelectActiveOnlies

                                      join SDA in dbContext.StudentDivisionAllocations on CD.RowKey equals SDA.ClassDetailsKey
                                      join A in dbContext.Applications on CD.RowKey equals A.ClassDetailsKey
                                      where (CD.CourseTypeKey == model.CourseTypeKey && (model.IsUpdate != true ? SDA.IsActive == true : SDA.IsActive != null) && A.BranchKey == model.BranchKey)
                                      select new SelectListModel
                                      {
                                          RowKey = CD.RowKey,
                                          Text = CD.ClassCode + CD.ClassCodeDescription
                                      }).Distinct().ToList();
            }
            return model;
        }
        public UnitTestScheduleViewModel FillCourseType(UnitTestScheduleViewModel model)
        {
            if (!DbConstants.Role.AdminUserTypes.Contains(DbConstants.User.RoleKey))
            {
                Employee Employee = dbContext.Employees.Where(x => x.AppUserKey == DbConstants.User.UserKey && x.IsTeacher == true).SingleOrDefault();
                if (Employee != null)
                {
                    model.CourseTypes = (from CD in dbContext.VwClassDetailsSelectActiveOnlies

                                         join CT in dbContext.CourseTypes on CD.CourseTypeKey equals CT.RowKey
                                         join SDA in dbContext.StudentDivisionAllocations on CD.RowKey equals SDA.ClassDetailsKey
                                         join TCA in dbContext.TeacherClassAllocations on CD.RowKey equals TCA.ClassDetailsKey
                                         join A in dbContext.Applications on CD.RowKey equals A.ClassDetailsKey
                                         where (TCA.EmployeeKey == Employee.RowKey && TCA.IsActive == true && TCA.IsAttendance == true && (model.IsUpdate != true ? SDA.IsActive == true : SDA.IsActive != null) && A.BranchKey == model.BranchKey)
                                         select new SelectListModel
                                         {
                                             RowKey = CT.RowKey,
                                             Text = CT.CourseTypeName
                                         }).Distinct().ToList();
                }
                else
                {
                    model.CourseTypes = (from CD in dbContext.VwClassDetailsSelectActiveOnlies

                                         join CT in dbContext.CourseTypes on CD.CourseTypeKey equals CT.RowKey
                                         join SDA in dbContext.StudentDivisionAllocations on CD.RowKey equals SDA.ClassDetailsKey
                                         join A in dbContext.Applications on CD.RowKey equals A.ClassDetailsKey
                                         where ((model.IsUpdate != true ? SDA.IsActive == true : SDA.IsActive != null) && A.BranchKey == model.BranchKey)
                                         select new SelectListModel
                                         {
                                             RowKey = CT.RowKey,
                                             Text = CT.CourseTypeName
                                         }).Distinct().ToList();
                }
            }
            else
            {
                model.CourseTypes = (from CD in dbContext.VwClassDetailsSelectActiveOnlies

                                     join CT in dbContext.CourseTypes on CD.CourseTypeKey equals CT.RowKey
                                     join SDA in dbContext.StudentDivisionAllocations on CD.RowKey equals SDA.ClassDetailsKey
                                     join A in dbContext.Applications on CD.RowKey equals A.ClassDetailsKey
                                     where ((model.IsUpdate != true ? SDA.IsActive == true : SDA.IsActive != null) && A.BranchKey == model.BranchKey)
                                     select new SelectListModel
                                     {
                                         RowKey = CT.RowKey,
                                         Text = CT.CourseTypeName
                                     }).Distinct().ToList();
            }
            return model;

        }
        public UnitTestScheduleViewModel GetSearchDropdownList(UnitTestScheduleViewModel model)
        {
            FillBranch(model);
            FillSearchClassDetails(model);
            FillSearchBatch(model);
            return model;
        }
        public UnitTestScheduleViewModel FillSearchClassDetails(UnitTestScheduleViewModel model)
        {

            if (model.BranchKey != 0)
            {
                model.ClassDetails = (from CD in dbContext.VwClassDetailsSelectActiveOnlies
                                      join SDA in dbContext.StudentDivisionAllocations on CD.RowKey equals SDA.ClassDetailsKey
                                      join A in dbContext.Applications on CD.RowKey equals A.ClassDetailsKey
                                      where (SDA.IsActive == true && A.BranchKey == model.BranchKey)
                                      select new SelectListModel
                                      {
                                          RowKey = CD.RowKey,
                                          Text = CD.ClassCode + CD.ClassCodeDescription
                                      }).Distinct().ToList();
            }
            else
            {
                model.ClassDetails = (from CD in dbContext.VwClassDetailsSelectActiveOnlies

                                      join SDA in dbContext.StudentDivisionAllocations on CD.RowKey equals SDA.ClassDetailsKey
                                      where (SDA.IsActive == true)
                                      select new SelectListModel
                                      {
                                          RowKey = CD.RowKey,
                                          Text = CD.ClassCode + CD.ClassCodeDescription
                                      }).Distinct().ToList();
            }

            return model;
        }
        public UnitTestScheduleViewModel FillSearchBatch(UnitTestScheduleViewModel model)
        {

            if (model.BranchKey != 0 || model.ClassDetailsKey != 0)
            {
                model.Batches = (from p in dbContext.Applications
                                 join SDA in dbContext.StudentDivisionAllocations on p.RowKey equals SDA.ApplicationKey
                                 join B in dbContext.VwBatchSelectActiveOnlies on p.BatchKey equals B.RowKey
                                 orderby B.RowKey
                                 //where (p.CourseKey == model.CourseKey && p.BranchKey == model.BranchKey && p.UniversityMasterKey == model.UniversityMasterKey)
                                 where (p.ClassDetailsKey == model.ClassDetailsKey || p.BranchKey == model.BranchKey)
                                 select new SelectListModel
                                 {
                                     RowKey = B.RowKey,
                                     Text = B.BatchName
                                 }).Distinct().ToList();
            }
            else
            {
                model.Batches = (from p in dbContext.Applications
                                 join SDA in dbContext.StudentDivisionAllocations on p.RowKey equals SDA.ApplicationKey
                                 join B in dbContext.VwBatchSelectActiveOnlies on p.BatchKey equals B.RowKey
                                 orderby B.RowKey
                                 //where (p.CourseKey == model.CourseKey && p.BranchKey == model.BranchKey && p.UniversityMasterKey == model.UniversityMasterKey)
                                 //where (p.ClassDetailsKey == model.ClassDetailsKey && p.BranchKey == model.BranchKey)
                                 select new SelectListModel
                                 {
                                     RowKey = B.RowKey,
                                     Text = B.BatchName
                                 }).Distinct().ToList();
            }
            return model;
        }
        #endregion DropDownChange Events

    }
}
