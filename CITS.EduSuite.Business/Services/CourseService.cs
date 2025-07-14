using CITS.EduSuite.Business.Common;
using CITS.EduSuite.Business.Interfaces;
using CITS.EduSuite.Business.Models.Resources;
using CITS.EduSuite.Business.Models.ViewModels;
using CITS.EduSuite.Data;
using System;
using System.Collections.Generic;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace CITS.EduSuite.Business.Services
{
    public class CourseService : ICourseService
    {
        private EduSuiteDatabase dbContext;
        private SharedService sharedService;
        public CourseService(EduSuiteDatabase objDB)
        {
            this.dbContext = objDB;
            sharedService = new SharedService(objDB);
        }
        public List<CourseViewModel> GetCourse(CourseViewModel model, out long TotalRecords)
        {
            try
            {
                var Take = model.PageSize;
                var Skip = (model.PageIndex - 1) * model.PageSize;
                IQueryable<CourseViewModel> CourseList = (from p in dbContext.Courses
                                                          orderby p.RowKey descending
                                                          where (p.CourseName.Contains(model.SearchText))
                                                          select new CourseViewModel
                                                          {
                                                              RowKey = p.RowKey,
                                                              CourseName = p.CourseName,
                                                              CourseTypeName = p.CourseType.CourseTypeName,
                                                              CourseCode = p.CourseCode,
                                                              CourseDuration = p.CourseDuration,
                                                              CourseYear = p.CourseYear,
                                                              IsActive = p.IsActive,
                                                              UniversityCourseKey = p.UniversityCourses.Select(x => x.RowKey).FirstOrDefault(),
                                                              CourseTypeKey = p.CourseTypeKey,
                                                              DurationCount = p.DurationCount,
                                                              DurationTypeKey = p.DurationTypeKey,
                                                          });
                if (model.SearchCourseTypeKey != 0)
                {
                    CourseList = CourseList.Where(row => row.CourseTypeKey == model.SearchCourseTypeKey);
                }
                CourseList = CourseList.GroupBy(x => x.RowKey).Select(y => y.FirstOrDefault());
                if (model.SortBy != "")
                {
                    CourseList = SortApplications(CourseList, model.SortBy, model.SortOrder);
                }
                TotalRecords = CourseList.Count();
                return CourseList.Skip(Skip).Take(Take).ToList<CourseViewModel>();
            }
            catch (Exception ex)
            {
                TotalRecords = 0;
                ActivityLog.CreateActivityLog(MenuConstants.Course, ActionConstants.View, DbConstants.LogType.Error, null, ex.GetBaseException().Message);
                return new List<CourseViewModel>();
            }
        }
        private IQueryable<CourseViewModel> SortApplications(IQueryable<CourseViewModel> Query, string SortName, string SortOrder)
        {
            string command = SortOrder == "desc" ? "OrderByDescending" : "OrderBy";
            var type = typeof(CourseViewModel);
            var property = type.GetProperty(SortName);
            var parameter = Expression.Parameter(type, "p");
            var propertyAccess = Expression.MakeMemberAccess(parameter, property);
            var orderByExpression = Expression.Lambda(propertyAccess, parameter);
            var resultExpression = Expression.Call(typeof(Queryable), command, new Type[] { type, property.PropertyType },
                                          Query.Expression, Expression.Quote(orderByExpression));
            return Query.Provider.CreateQuery<CourseViewModel>(resultExpression);
        }
        public CourseViewModel GetCourseById(int? id)
        {
            try
            {
                CourseViewModel model = new CourseViewModel();
                model = dbContext.Courses.Select(row => new CourseViewModel
                {
                    RowKey = row.RowKey,
                    CourseName = row.CourseName,
                    CourseTypeKey = row.CourseTypeKey,
                    CourseCode = row.CourseCode,
                    CourseDuration = row.CourseDuration,
                    CourseYear = row.CourseYear,
                    IsActive = row.IsActive,
                    DurationCount = row.DurationCount,
                    DurationTypeKey = row.DurationTypeKey,
                }).Where(x => x.RowKey == id).FirstOrDefault();
                if (model == null)
                {
                    model = new CourseViewModel();
                }
                BindDropDownList(model);
                return model;
            }
            catch (Exception ex)
            {
                ActivityLog.CreateActivityLog(MenuConstants.Course, ActionConstants.View, DbConstants.LogType.Error, id, ex.GetBaseException().Message);
                return new CourseViewModel();
            }
        }
        public CourseViewModel CreateCourse(CourseViewModel model)
        {

            Course CourseModel = new Course();
            UniversityCourse UniversityCourseModel = new UniversityCourse();
            using (var transaction = dbContext.Database.BeginTransaction())
            {
                try
                {
                    long MaxKey = dbContext.Courses.Select(p => p.RowKey).DefaultIfEmpty().Max();
                    CalCulateDuration(model);
                    CourseModel.RowKey = Convert.ToInt64(MaxKey + 1);
                    CourseModel.CourseName = model.CourseName;
                    CourseModel.CourseTypeKey = model.CourseTypeKey;
                    CourseModel.CourseCode = model.CourseCode;
                    CourseModel.CourseDuration = Convert.ToInt32(model.CourseDuration);
                    CourseModel.CourseYear = Convert.ToInt32(model.CourseYear);
                    CourseModel.IsActive = model.IsActive;
                    CourseModel.DurationTypeKey = model.DurationTypeKey;
                    CourseModel.DurationCount = model.DurationCount;
                    dbContext.Courses.Add(CourseModel);

                    if (!sharedService.CheckMenuActive(MenuConstants.UniversityCourse))
                    {
                        CreateUniversityCourse(CourseModel);
                    }
                    dbContext.SaveChanges();
                    transaction.Commit();
                    model.Message = EduSuiteUIResources.Success;
                    model.IsSuccessful = true;
                    ActivityLog.CreateActivityLog(MenuConstants.Course, ActionConstants.Add, DbConstants.LogType.Info, CourseModel.RowKey, model.Message);

                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    model.Message = String.Format(EduSuiteUIResources.FailedToSaveErrorMessage, EduSuiteUIResources.Course);
                    model.IsSuccessful = false;
                    ActivityLog.CreateActivityLog(MenuConstants.Course, ActionConstants.Add, DbConstants.LogType.Error, model.RowKey, ex.GetBaseException().Message);

                }
            }
            return model;
        }

        private void CreateUniversityCourse(Course model)
        {

            UniversityCourse UniversityCourseModel = new UniversityCourse();

            long MaxKey = dbContext.UniversityCourses.Select(p => p.RowKey).DefaultIfEmpty().Max();

            UniversityCourseModel.RowKey = MaxKey + 1;
            UniversityCourseModel.UniversityMasterKey = dbContext.UniversityMasters.Select(x => x.RowKey).SingleOrDefault();
            UniversityCourseModel.CourseKey = model.RowKey;
            UniversityCourseModel.AcademicTermKey = dbContext.AcademicTerms.Where(x => x.IsActive).Select(row => row.RowKey).FirstOrDefault();
            UniversityCourseModel.IsActive = model.IsActive;
            dbContext.UniversityCourses.Add(UniversityCourseModel);
        }
        public CourseViewModel UpdateCourse(CourseViewModel model)
        {

            Course CourseModel = new Course();
            using (var transaction = dbContext.Database.BeginTransaction())
            {
                try
                {
                    CourseModel = dbContext.Courses.SingleOrDefault(x => x.RowKey == model.RowKey);
                    CalCulateDuration(model);
                    CourseModel.CourseName = model.CourseName;
                    CourseModel.CourseTypeKey = model.CourseTypeKey;
                    CourseModel.CourseCode = model.CourseCode;
                    CourseModel.CourseDuration = Convert.ToInt32(model.CourseDuration);
                    CourseModel.CourseYear = Convert.ToInt32(model.CourseYear);
                    CourseModel.DurationTypeKey = model.DurationTypeKey;
                    CourseModel.DurationCount = model.DurationCount;
                    CourseModel.IsActive = model.IsActive;
                    dbContext.SaveChanges();
                    transaction.Commit();
                    model.Message = EduSuiteUIResources.Success;
                    model.IsSuccessful = true;
                    ActivityLog.CreateActivityLog(MenuConstants.Course, ActionConstants.Edit, DbConstants.LogType.Info, model.RowKey, model.Message);

                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    model.Message = String.Format(EduSuiteUIResources.FailedToSaveErrorMessage, EduSuiteUIResources.Course);
                    model.IsSuccessful = false;

                    ActivityLog.CreateActivityLog(MenuConstants.Course, ActionConstants.Edit, DbConstants.LogType.Error, model.RowKey, ex.GetBaseException().Message);

                }
            }
            return model;

        }
        public CourseViewModel DeleteCourse(CourseViewModel model)
        {
            using (var transaction = dbContext.Database.BeginTransaction())
            {
                try
                {
                    Course CourseModel = dbContext.Courses.SingleOrDefault(row => row.RowKey == model.RowKey);
                    if (!sharedService.CheckMenuActive(MenuConstants.UniversityCourse))
                    {
                        var classDetails = dbContext.ClassDetails.Any(x => x.UniversityCourseKey == model.UniversityCourseKey);
                        //var universityCourseFee = dbContext.UniversityCourseFees.Any(x => x.UniversityCourseKey == model.RowKey);
                        if (classDetails == false)
                        {
                            UniversityCourse UniversityCourseModel = dbContext.UniversityCourses.SingleOrDefault(x => x.RowKey == model.UniversityCourseKey);
                            if (UniversityCourseModel != null)
                            {
                                var appliction = dbContext.Applications.Any(x => x.AcademicTermKey == UniversityCourseModel.AcademicTermKey && x.CourseKey == UniversityCourseModel.CourseKey && x.UniversityMasterKey == UniversityCourseModel.UniversityMasterKey);
                                if (appliction == false)
                                {
                                    List<UniversityCourseFee> UniversityCourseFeeList = dbContext.UniversityCourseFees.Where(x => x.UniversityCourseKey == model.UniversityCourseKey).ToList();
                                    dbContext.UniversityCourseFees.RemoveRange(UniversityCourseFeeList);
                                    dbContext.UniversityCourses.Remove(UniversityCourseModel);
                                }
                                else
                                {
                                    model.Message = String.Format(EduSuiteUIResources.CantDeleteErrorMessage, EduSuiteUIResources.Course);
                                    model.IsSuccessful = false;
                                    return model;
                                }
                            }
                        }
                        else
                        {
                            model.Message = String.Format(EduSuiteUIResources.CantDeleteErrorMessage, EduSuiteUIResources.Course);
                            model.IsSuccessful = false;
                            return model;
                        }
                    }
                    dbContext.Courses.Remove(CourseModel);
                    dbContext.SaveChanges();
                    transaction.Commit();
                    model.Message = EduSuiteUIResources.Success;
                    model.IsSuccessful = true;
                    ActivityLog.CreateActivityLog(MenuConstants.Course, ActionConstants.Delete, DbConstants.LogType.Info, model.RowKey, model.Message);

                }
                catch (DbUpdateException ex)
                {
                    transaction.Rollback();
                    if (ex.GetBaseException().Message.ToUpper().Contains(EduSuiteUIResources.ForeignKeyError.ToUpper()))
                    {
                        model.Message = String.Format(EduSuiteUIResources.CantDeleteErrorMessage, EduSuiteUIResources.Course);
                        model.IsSuccessful = false;
                        ActivityLog.CreateActivityLog(MenuConstants.Course, ActionConstants.Delete, DbConstants.LogType.Error, model.RowKey, ex.GetBaseException().Message);

                    }
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    model.Message = String.Format(EduSuiteUIResources.FailedToDeleteErrorMessage, EduSuiteUIResources.Course);
                    model.IsSuccessful = false;
                    ActivityLog.CreateActivityLog(MenuConstants.Course, ActionConstants.Delete, DbConstants.LogType.Error, model.RowKey, ex.GetBaseException().Message);

                }
            }

            return model;
        }
        private void FillCourseType(CourseViewModel model)
        {
            model.CourseTypeList = dbContext.VwCourseTypeSelectActiveOnlies.Select(x => new SelectListModel
            {
                RowKey = x.RowKey,
                Text = x.CourseTypeName
            }).ToList();
        }
        public CourseViewModel CheckCourseCodeExists(CourseViewModel model)
        {
            if (dbContext.Courses.Where(x => x.CourseCode.ToLower() == model.CourseCode.ToLower() && x.RowKey != model.RowKey).Any())
            {
                model.IsSuccessful = false;
                model.Message = String.Format(EduSuiteUIResources.ExistsErrorMessage, EduSuiteUIResources.Course + EduSuiteUIResources.BlankSpace + EduSuiteUIResources.Code);
            }
            else
            {
                model.IsSuccessful = true;
                model.Message = "";
            }
            return model;
        }
        public CourseViewModel CheckCourseNameExists(CourseViewModel model)
        {
            if (dbContext.Courses.Where(x => x.CourseName.ToLower() == model.CourseName.ToLower() && x.RowKey != model.RowKey && x.CourseTypeKey == model.CourseTypeKey).Any())
            {
                model.IsSuccessful = false;
                model.Message = String.Format(EduSuiteUIResources.ExistsErrorMessage, EduSuiteUIResources.Course + EduSuiteUIResources.BlankSpace + EduSuiteUIResources.Name);
            }
            else
            {
                model.IsSuccessful = true;
                model.Message = "";
            }
            return model;
        }

        private void FillCourseDurationType(CourseViewModel model)
        {
            model.CourseDurationTypeList = dbContext.CourseDurationTypes.Where(x => x.IsActive).Select(x => new SelectListModel
            {
                RowKey = x.RowKey,
                Text = x.DurationType
            }).ToList();
        }
        private void BindDropDownList(CourseViewModel model)
        {
            FillCourseType(model);
            FillCourseDurationType(model);
        }

        private void CalCulateDuration(CourseViewModel model)
        {
            if (model.DurationTypeKey == DbConstants.CourseDurationType.Years)
            {
                model.CourseDuration = model.DurationCount * 12;
            }
            else if (model.DurationTypeKey == DbConstants.CourseDurationType.Days)
            {
                model.CourseDuration = model.DurationCount / 30;
                model.CourseDuration = model.CourseDuration < 1 ? 1 : model.CourseDuration;
            }
            else
            {
                model.CourseDuration = Convert.ToInt32(model.DurationCount);
            }
        }
    }
}
