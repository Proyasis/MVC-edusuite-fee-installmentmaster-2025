using CITS.EduSuite.Business.Interfaces;
using CITS.EduSuite.Business.Common;
using CITS.EduSuite.Data;
using CITS.EduSuite.Business.Models.ViewModels;
using System.Data.Entity.Infrastructure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CITS.EduSuite.Business.Models.Resources;
using System.Linq.Expressions;
using System.IO;


namespace CITS.EduSuite.Business.Services
{
    public class UniversityCourseService : IUniversityCourseService
    {
        private EduSuiteDatabase dbContext;
        public UniversityCourseService(EduSuiteDatabase db)
        {
            this.dbContext = db;
        }
        public UniversityCourseViewModel GetUniversityCourseById(int? Id)
        {
            try
            {
                UniversityCourseViewModel model = new UniversityCourseViewModel();
                model = dbContext.UniversityCourses.Select(row => new UniversityCourseViewModel
                {
                    RowKey = row.RowKey,
                    CourseTypeKey = row.Course.CourseTypeKey,
                    UniversityMasterKey = row.UniversityMasterKey,
                    CourseKey = row.CourseKey,
                    AcademicTermKey = row.AcademicTermKey,
                    IsActive = row.IsActive,
                    IsUpdate = dbContext.Applications.Any(x => x.AcademicTermKey == row.AcademicTermKey && x.CourseKey == row.CourseKey && x.UniversityMasterKey == row.UniversityMasterKey)
                }).Where(row => row.RowKey == Id).FirstOrDefault();
                if (model == null)
                {
                    model = new UniversityCourseViewModel();
                }
                FillDropDown(model);
                return model;
            }
            catch (Exception ex)
            {
                ActivityLog.CreateActivityLog(MenuConstants.UniversityCourse, ActionConstants.View, DbConstants.LogType.Error, Id, ex.GetBaseException().Message);
                return new UniversityCourseViewModel();
            }

        }
        public UniversityCourseViewModel CreateUniversityCourse(UniversityCourseViewModel model)
        {
            FillDropDown(model);
            var UniversityCourseCheck = dbContext.UniversityCourses.Where(row => row.UniversityMasterKey == model.UniversityMasterKey && row.CourseKey == model.CourseKey
                && row.AcademicTermKey == model.AcademicTermKey).Count();
            if (UniversityCourseCheck != 0)
            {
                model.Message = String.Format(EduSuiteUIResources.ExistsErrorMessage, EduSuiteUIResources.AffiliationsTieUpsCourse);
                model.IsSuccessful = false;
                return model;
            }
            UniversityCourse UniversityCourseModel = new UniversityCourse();
            using (var transaction = dbContext.Database.BeginTransaction())
            {
                try
                {
                    long MaxKey = dbContext.UniversityCourses.Select(p => p.RowKey).DefaultIfEmpty().Max();

                    UniversityCourseModel.RowKey = MaxKey + 1;
                    UniversityCourseModel.UniversityMasterKey = model.UniversityMasterKey;
                    UniversityCourseModel.CourseKey = model.CourseKey;
                    UniversityCourseModel.AcademicTermKey = model.AcademicTermKey;
                    UniversityCourseModel.IsActive = model.IsActive;
                    dbContext.UniversityCourses.Add(UniversityCourseModel);
                    dbContext.SaveChanges();
                    transaction.Commit();
                    model.Message = EduSuiteUIResources.Success;
                    model.IsSuccessful = true;
                    ActivityLog.CreateActivityLog(MenuConstants.UniversityCourse, ActionConstants.Add, DbConstants.LogType.Info, model.RowKey, model.Message);


                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    model.Message = String.Format(EduSuiteUIResources.FailedToSaveErrorMessage, EduSuiteUIResources.AffiliationsTieUpsCourse);
                    model.IsSuccessful = false;

                    ActivityLog.CreateActivityLog(MenuConstants.UniversityCourse, ActionConstants.Add, DbConstants.LogType.Error, model.RowKey, ex.GetBaseException().Message);
                }
                FillDropDown(model);
                return model;
            }
        }
        public UniversityCourseViewModel UpdateUniversityCourse(UniversityCourseViewModel model)
        {

            FillDropDown(model);
            var UniversityCourseCheck = dbContext.UniversityCourses.Where(row => row.UniversityMasterKey == model.UniversityMasterKey && row.CourseKey == model.CourseKey
                && row.AcademicTermKey == model.AcademicTermKey && row.RowKey != model.RowKey).ToList();
            if (UniversityCourseCheck.Count != 0)
            {
                model.Message = String.Format(EduSuiteUIResources.ExistsErrorMessage, EduSuiteUIResources.AffiliationsTieUpsCourse);
                model.IsSuccessful = false;
                return model;
            }
            UniversityCourse UniversityCourseModel = new UniversityCourse();
            using (var transaction = dbContext.Database.BeginTransaction())
            {
                try
                {

                    UniversityCourseModel = dbContext.UniversityCourses.SingleOrDefault(x => x.RowKey == model.RowKey);
                    UniversityCourseModel.UniversityMasterKey = model.UniversityMasterKey;
                    UniversityCourseModel.CourseKey = model.CourseKey;
                    UniversityCourseModel.AcademicTermKey = model.AcademicTermKey;
                    UniversityCourseModel.IsActive = model.IsActive;
                    dbContext.SaveChanges();
                    transaction.Commit();
                    model.Message = EduSuiteUIResources.Success;
                    model.IsSuccessful = true;

                    ActivityLog.CreateActivityLog(MenuConstants.UniversityCourse, ActionConstants.Add, DbConstants.LogType.Info, model.RowKey, model.Message);

                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    model.Message = String.Format(EduSuiteUIResources.FailedToSaveErrorMessage, EduSuiteUIResources.AffiliationsTieUpsCourse);
                    model.IsSuccessful = false;

                    ActivityLog.CreateActivityLog(MenuConstants.UniversityCourse, ActionConstants.Add, DbConstants.LogType.Error, model.RowKey, ex.GetBaseException().Message);
                }
                FillDropDown(model);
                return model;
            }
        }
        public UniversityCourseViewModel DeleteUniversityCourse(UniversityCourseViewModel model)
        {
            using (var transation = dbContext.Database.BeginTransaction())
            {
                try
                {
                    var classDetails = dbContext.ClassDetails.Any(x => x.UniversityCourseKey == model.RowKey);
                    //var universityCourseFee = dbContext.UniversityCourseFees.Any(x => x.UniversityCourseKey == model.RowKey);
                    if (classDetails == false)
                    {
                        UniversityCourse UniversityCourseModel = dbContext.UniversityCourses.SingleOrDefault(x => x.RowKey == model.RowKey);
                        var appliction = dbContext.Applications.Any(x => x.AcademicTermKey == UniversityCourseModel.AcademicTermKey && x.CourseKey == UniversityCourseModel.CourseKey && x.UniversityMasterKey == UniversityCourseModel.UniversityMasterKey);
                        if (appliction == false)
                        {
                            List<UniversityCourseFee> UniversityCourseFeeList = dbContext.UniversityCourseFees.Where(x => x.UniversityCourseKey == model.RowKey).ToList();
                            dbContext.UniversityCourseFees.RemoveRange(UniversityCourseFeeList);
                            dbContext.UniversityCourses.Remove(UniversityCourseModel);
                        }
                        else
                        {
                            model.Message = String.Format(EduSuiteUIResources.CantDeleteErrorMessage, EduSuiteUIResources.AffiliationsTieUpsCourse);
                            model.IsSuccessful = false;
                            return model;
                        }
                    }
                    else
                    {
                        model.Message = String.Format(EduSuiteUIResources.CantDeleteErrorMessage, EduSuiteUIResources.AffiliationsTieUpsCourse);
                        model.IsSuccessful = false;
                        return model;
                    }

                    dbContext.SaveChanges();
                    transation.Commit();
                    model.Message = EduSuiteUIResources.Success;
                    model.IsSuccessful = true;
                }
                catch (DbUpdateException ex)
                {
                    transation.Rollback();
                    if (ex.GetBaseException().Message.ToUpper().Contains(EduSuiteUIResources.ForeignKeyError.ToUpper()))
                    {
                        model.Message = String.Format(EduSuiteUIResources.CantDeleteErrorMessage, EduSuiteUIResources.AffiliationsTieUpsCourse);
                        model.IsSuccessful = false;

                    }
                }
                catch (Exception ex)
                {
                    transation.Rollback();
                    model.Message = String.Format(EduSuiteUIResources.FailedToDeleteErrorMessage, EduSuiteUIResources.AffiliationsTieUpsCourse);
                    model.IsSuccessful = false;
                }
                return model;
            }
        }
        public List<UniversityCourseViewModel> GetUniversityCourse(UniversityCourseViewModel model, out long TotalRecords)
        {
            try
            {
                var Take = model.PageSize;
                var Skip = (model.PageIndex - 1) * model.PageSize;

                IQueryable<UniversityCourseViewModel> UniversityCourseList = (from UC in dbContext.UniversityCourses
                                                                              orderby UC.RowKey
                                                                              where (UC.Course.CourseName.Contains(model.SearchText))
                                                                              select new UniversityCourseViewModel
                                                                              {
                                                                                  RowKey = UC.RowKey,
                                                                                  UniversityName = UC.UniversityMaster.UniversityMasterName,
                                                                                  CourseName = UC.Course.CourseName,
                                                                                  AcademicTermName = UC.AcademicTerm.AcademicTermName,
                                                                                  Duration = UC.AcademicTermKey == DbConstants.AcademicTerm.Semester ? (UC.Course.CourseDuration / 6) + " semester" : (UC.Course.CourseDuration / 12) + " Year",
                                                                                  TotalUniversityCoursefee = UC.UniversityCourseFees.Where(x => x.IsActive == true).Sum(x => x.FeeAmount),
                                                                                  CourseDuration = UC.AcademicTermKey == DbConstants.AcademicTerm.Semester ? (UC.Course.CourseDuration / 6) : (UC.Course.CourseDuration / 12),
                                                                                  UniversityMasterKey = UC.UniversityMasterKey,
                                                                                  CourseKey = UC.CourseKey,
                                                                                  AcademicTermKey = UC.AcademicTermKey,
                                                                                  CourseTypeKey = UC.Course.CourseTypeKey,
                                                                                  DurationTypeKey = UC.Course.DurationTypeKey,
                                                                                  DurationCount = UC.Course.DurationCount,
                                                                              });

                if (model.SearchCourseKey != 0)
                {
                    UniversityCourseList = UniversityCourseList.Where(row => row.CourseKey == model.SearchCourseKey);
                }
                if (model.SearchCourseTypeKey != 0)
                {
                    UniversityCourseList = UniversityCourseList.Where(row => row.CourseTypeKey == model.SearchCourseTypeKey);
                }
                if (model.SearchAcademicTermKey != 0)
                {
                    UniversityCourseList = UniversityCourseList.Where(row => row.AcademicTermKey == model.SearchAcademicTermKey);
                }
                if (model.SearchUniversityMasterKey != 0)
                {
                    UniversityCourseList = UniversityCourseList.Where(row => row.UniversityMasterKey == model.SearchUniversityMasterKey);
                }
                UniversityCourseList = UniversityCourseList.GroupBy(x => x.RowKey).Select(y => y.FirstOrDefault());
                if (model.SortBy != "")
                {
                    UniversityCourseList = SortApplications(UniversityCourseList, model.SortBy, model.SortOrder);
                }
                TotalRecords = UniversityCourseList.Count();
                return UniversityCourseList.Skip(Skip).Take(Take).ToList<UniversityCourseViewModel>();
            }
            catch (Exception ex)
            {
                TotalRecords = 0;
                ActivityLog.CreateActivityLog(MenuConstants.UniversityCourse, ActionConstants.View, DbConstants.LogType.Error, null, ex.GetBaseException().Message);
                return new List<UniversityCourseViewModel>();
            }
        }
        private IQueryable<UniversityCourseViewModel> SortApplications(IQueryable<UniversityCourseViewModel> Query, string SortName, string SortOrder)
        {
            string command = SortOrder == "desc" ? "OrderByDescending" : "OrderBy";
            var type = typeof(UniversityCourseViewModel);
            var property = type.GetProperty(SortName);
            var parameter = Expression.Parameter(type, "p");
            var propertyAccess = Expression.MakeMemberAccess(parameter, property);
            var orderByExpression = Expression.Lambda(propertyAccess, parameter);
            var resultExpression = Expression.Call(typeof(Queryable), command, new Type[] { type, property.PropertyType },
                                          Query.Expression, Expression.Quote(orderByExpression));
            return Query.Provider.CreateQuery<UniversityCourseViewModel>(resultExpression);
        }
        private void FillDropDown(UniversityCourseViewModel model)
        {
            FillCourseType(model);
            FillAcademicTerm(model);
            FillCourse(model);
            FillUniversity(model);

        }
        private void FillCourseType(UniversityCourseViewModel model)
        {
            model.CourseTypes = dbContext.VwCourseTypeSelectActiveOnlies.Select(x => new SelectListModel
            {
                RowKey = x.RowKey,
                Text = x.CourseTypeName
            }).ToList();
        }
        private void FillUniversity(UniversityCourseViewModel model)
        {
            model.Universities = dbContext.VwUniversityMasterSelectActiveOnlies.Select(x => new SelectListModel
            {
                RowKey = x.RowKey,
                Text = x.UniversityMasterName
            }).ToList();

        }
        public UniversityCourseViewModel FillAcademicTerm(UniversityCourseViewModel model)
        {
            var courseYear = dbContext.Courses.Any(x => x.RowKey == model.CourseKey && x.CourseDuration >= 12);
            if (courseYear == true)
            {
                model.AcademicTerms = dbContext.VwAcadamicTermSelectActiveOnlies.Where(x => x.RowKey != DbConstants.AcademicTerm.ShortTerm).Select(x => new SelectListModel
                {
                    RowKey = x.RowKey,
                    Text = x.AcademicTermName,
                }).ToList();
            }
            else
            {
                model.AcademicTerms = dbContext.VwAcadamicTermSelectActiveOnlies.Where(x => x.RowKey == DbConstants.AcademicTerm.ShortTerm).Select(x => new SelectListModel
                {
                    RowKey = x.RowKey,
                    Text = x.AcademicTermName,
                }).ToList();
                model.AcademicTermKey = dbContext.VwAcadamicTermSelectActiveOnlies.Where(x => x.RowKey == DbConstants.AcademicTerm.ShortTerm).Select(x => x.RowKey).FirstOrDefault();
            }
            return model;
        }
        //private void FillSyllabus(UniversityCourseViewModel model)
        //{
        //    model.Syllabuses = dbContext.fnSyllabus().Select(x => new SelectListModel
        //    {
        //        RowKey = x.RowKey ?? 0,
        //        Text = x.SyllabusName,
        //        //IntValue = x.DefaultValue ?? 0
        //    }).ToList();
        //}

        public UniversityCourseViewModel FillCourse(UniversityCourseViewModel model)
        {
            model.Courses = dbContext.VwCourseSelectActiveOnlies.Where(x => x.CourseTypeKey == model.CourseTypeKey).Select(x => new SelectListModel
            {
                RowKey = x.RowKey,
                Text = x.CourseName
            }).ToList();
            return model;
        }

        #region Class Details
        private void FillCourseYears(UniversityCourseViewModel model)
        {
            UniversityCourse universityCourse = dbContext.UniversityCourses.SingleOrDefault(row => row.RowKey == model.RowKey);

            if (universityCourse != null)
            {
                var CourseDuration = universityCourse.Course.CourseDuration;
                //bool IsSemester = Application.M_UniversityMaster.M_UniversityCourse.Select(row => row.IsSemester).FirstOrDefault();
                //byte? IsSemester = Application.AcademicTermKey;

                // var duration = Math.Ceiling((Convert.ToDecimal(CourseDuration ?? 0) / 12));
                var duration = Math.Ceiling((Convert.ToDecimal(universityCourse.AcademicTermKey == DbConstants.AcademicTerm.Semester ? CourseDuration / 6 : CourseDuration / 12)));

                var StartYear = 1;
                if (duration < 1)
                {
                    model.YearList.Add(new SelectListModel
                    {
                        RowKey = 1,
                        Text = " Short Term"
                    });
                }
                else
                {
                    for (int i = StartYear; i <= duration; i++)
                    {
                        model.YearList.Add(new SelectListModel
                        {
                            RowKey = i,
                            Text = i + (universityCourse.AcademicTermKey == DbConstants.AcademicTerm.Semester ? " Semester" : " Year")
                        });
                    }
                }
            }
        }

        private void FillDivision(UniversityCourseViewModel model)
        {
            model.DivisionList = dbContext.VwDivisionSelectActiveOnlies.Select(x => new SelectListModel
            {
                RowKey = x.RowKey,
                Text = x.DivisionName
            }).ToList();
        }

        private void FillBuildingDetails(UniversityCourseViewModel model)
        {
            model.BuildingDetailsList = dbContext.VWBuildingDetailsSelectActiveOnlies.Select(x => new SelectListModel
            {
                RowKey = x.RowKey,
                Text = x.BuildingDetailsName
            }).ToList();
        }

        private void FillDropDownList(UniversityCourseViewModel model)
        {
            FillDivision(model);
            FillCourseYears(model);
            FillBuildingDetails(model);
        }

        public UniversityCourseViewModel UpdateClassModule(UniversityCourseViewModel model)
        {
            using (var transaction = dbContext.Database.BeginTransaction())
            {
                try
                {
                    CreateClassModule(model.ClassDetailsModel.Where(x => x.RowKey == 0).ToList(), model.StudentYear ?? 0, model.RowKey);
                    UpdateClassModule(model.ClassDetailsModel.Where(x => x.RowKey != 0).ToList());
                    dbContext.SaveChanges();
                    transaction.Commit();
                    model.Message = EduSuiteUIResources.Success;
                    model.IsSuccessful = true;
                }
                catch (Exception Ex)
                {
                    transaction.Rollback();
                    model.Message = String.Format(EduSuiteUIResources.FailedToSaveErrorMessage, EduSuiteUIResources.AffiliationsTieUpsClass);
                    model.IsSuccessful = false;
                }
            }
            return model;
        }

        private void CreateClassModule(List<ClassDetailsModel> modelList, short StudentYear, long UniversityCourseKey)
        {
            Int64 MaxKey = dbContext.ClassDetails.Select(x => x.RowKey).DefaultIfEmpty().Max();
            foreach (ClassDetailsModel model in modelList)
            {
                ClassDetail classDetailModel = new ClassDetail();

                classDetailModel.RowKey = Convert.ToInt64(MaxKey + 1);
                classDetailModel.DivisionKey = model.DivisionKey;
                classDetailModel.ClassCode = model.ClassCode;
                classDetailModel.BuildingDetailsKey = model.BuildingDetailsKey;
                classDetailModel.UniversityCourseKey = UniversityCourseKey;
                classDetailModel.StudentYear = StudentYear;
                classDetailModel.IsActive = model.IsActive;
                classDetailModel.StartTime = model.StartTime;
                classDetailModel.EndTime = model.EndTime;
                classDetailModel.LateMinutes = model.LateMinutes;

                dbContext.ClassDetails.Add(classDetailModel);

                MaxKey++;
            }
        }

        private void UpdateClassModule(List<ClassDetailsModel> modelList)
        {
            foreach (ClassDetailsModel model in modelList)
            {
                ClassDetail classDetailModel = new ClassDetail();

                classDetailModel = dbContext.ClassDetails.SingleOrDefault(x => x.RowKey == model.RowKey);
                classDetailModel.DivisionKey = model.DivisionKey;
                classDetailModel.ClassCode = model.ClassCode;
                classDetailModel.BuildingDetailsKey = model.BuildingDetailsKey;
                //classDetailModel.UniversityCourseKey = model.UniversityCourseKey;
                classDetailModel.StudentYear = model.StudentYear;
                classDetailModel.IsActive = model.IsActive;
                classDetailModel.StartTime = model.StartTime;
                classDetailModel.EndTime = model.EndTime;
                classDetailModel.LateMinutes = model.LateMinutes;

            }
        }

        public UniversityCourseViewModel GetClassDetailsById(UniversityCourseViewModel model)
        {
            try
            {
                UniversityCourseViewModel objviewmodel = new UniversityCourseViewModel();
                if (model.StudentYear == null || model.StudentYear == 0)
                {
                    model.StudentYear = 1;
                }
                objviewmodel.RowKey = model.RowKey;
                objviewmodel.StudentYear = model.StudentYear;
                objviewmodel.ClassDetailsModel = dbContext.ClassDetails.Where(x => x.UniversityCourseKey == model.RowKey && x.StudentYear == model.StudentYear).Select(row => new ClassDetailsModel
                {
                    RowKey = row.RowKey,
                    DivisionKey = row.DivisionKey,
                    ClassCode = row.ClassCode,
                    BuildingDetailsKey = row.BuildingDetailsKey,
                    UniversityCourseKey = row.UniversityCourseKey,
                    StudentYear = row.StudentYear,
                    ClassDetialsKey = row.RowKey,
                    IsActive = row.IsActive,
                    StartTime=row.StartTime,
                    EndTime = row.EndTime,
                    LateMinutes=row.LateMinutes,
                }).ToList();



                if (objviewmodel.ClassDetailsModel.Count == 0)
                {
                    objviewmodel.ClassDetailsModel.Add(new ClassDetailsModel());
                }
                if (objviewmodel == null)
                {
                    objviewmodel = new UniversityCourseViewModel();
                }

                UniversityCourse universityCourse = dbContext.UniversityCourses.SingleOrDefault(row => row.RowKey == model.RowKey);
                if (universityCourse != null)
                {
                    var cduration = universityCourse.Course.CourseDuration / universityCourse.AcademicTerm.Duration;
                    objviewmodel.CourseName = universityCourse.Course.CourseName;
                    objviewmodel.AcademicTermName = universityCourse.AcademicTerm.AcademicTermName;
                    objviewmodel.UniversityName = universityCourse.UniversityMaster.UniversityMasterName;
                    objviewmodel.Duration = cduration < 1 ? "Short Term" : universityCourse.AcademicTermKey == DbConstants.AcademicTerm.Semester ? (universityCourse.Course.CourseDuration / 6) + " semester" : (universityCourse.Course.CourseDuration / 12) + " Year";
                    objviewmodel.CourseDuration = universityCourse.AcademicTermKey == DbConstants.AcademicTerm.Semester ? (universityCourse.Course.CourseDuration / 6) : (universityCourse.Course.CourseDuration / 12);
                }
                FillDropDownList(objviewmodel);
                return objviewmodel;
            }
            catch (Exception ex)
            {
                ActivityLog.CreateActivityLog(MenuConstants.UniversityCourse, ActionConstants.View, DbConstants.LogType.Error, null, ex.GetBaseException().Message);
                return new UniversityCourseViewModel();
            }
        }

        public UniversityCourseViewModel DeleteClassDetails(ClassDetailsModel model)
        {
            UniversityCourseViewModel universityCoursemodel = new UniversityCourseViewModel();

            using (var transaction = dbContext.Database.BeginTransaction())
            {
                try
                {
                    ClassDetail classDetails = dbContext.ClassDetails.SingleOrDefault(row => row.RowKey == model.RowKey);

                    dbContext.ClassDetails.Remove(classDetails);
                    dbContext.SaveChanges();
                    transaction.Commit();
                    universityCoursemodel.Message = EduSuiteUIResources.Success;
                    universityCoursemodel.IsSuccessful = true;
                }
                catch (DbUpdateException ex)
                {
                    transaction.Rollback();
                    if (ex.GetBaseException().Message.ToUpper().Contains(EduSuiteUIResources.ForeignKeyError.ToUpper()))
                    {
                        universityCoursemodel.Message = String.Format(EduSuiteUIResources.CantDeleteErrorMessage, EduSuiteUIResources.AffiliationsTieUpsClass);
                        universityCoursemodel.IsSuccessful = false;
                    }
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    universityCoursemodel.Message = String.Format(EduSuiteUIResources.FailedToDeleteErrorMessage, EduSuiteUIResources.AffiliationsTieUpsClass);
                    universityCoursemodel.IsSuccessful = false;
                }
            }
            return universityCoursemodel;
        }

        public UniversityCourseViewModel CheckClassCodeExists(string ClassCode, long RowKey)
        {
            UniversityCourseViewModel model = new UniversityCourseViewModel();

            if (dbContext.ClassDetails.Where(row => row.ClassCode.ToLower() == ClassCode.ToLower() && row.RowKey != RowKey).Any())
            {
                model.IsSuccessful = false;
            }
            else
            {
                model.IsSuccessful = true;
            }
            return model;
        }

        public UniversityCourseViewModel CheckRoomNoExists(long? BuildingDetailsKey, long RowKey)
        {
            UniversityCourseViewModel model = new UniversityCourseViewModel();
            if (dbContext.ClassDetails.Where(row => row.BuildingDetailsKey == BuildingDetailsKey && row.RowKey != RowKey).Any())
            {
                model.IsSuccessful = false;
            }
            else
            {
                model.IsSuccessful = true;
            }
            return model;
        }

        public List<UniversityCourseViewModel> GetClassDetailsList(UniversityCourseViewModel model)
        {
            try
            {
                var ClassDetailsList = (from CD in dbContext.ClassDetails
                                        orderby CD.RowKey
                                        where (CD.ClassCode.Contains(model.ClassName) || CD.UniversityCourse.CourseKey == model.CourseKey)
                                        select new UniversityCourseViewModel
                                        {
                                            ClassDetailsKey = CD.RowKey,
                                            CourseKey = CD.UniversityCourse.CourseKey,
                                            UniversityMasterKey = CD.UniversityCourse.UniversityMasterKey,
                                            AcademicTermKey = CD.UniversityCourse.AcademicTermKey,
                                            StudentYear = CD.StudentYear,
                                            ClassName = CD.ClassCode
                                        }).ToList();
                return ClassDetailsList.GroupBy(x => x.ClassDetailsKey).Select(y => y.First()).ToList<UniversityCourseViewModel>();
            }
            catch (Exception ex)
            {
                ActivityLog.CreateActivityLog(MenuConstants.UniversityCourse, ActionConstants.View, DbConstants.LogType.Error, null, ex.GetBaseException().Message);
                return new List<UniversityCourseViewModel>();

            }
        }

        #endregion Class Details

        #region University Course Fee
        public UniversityCourseViewModel UpdateUniversityCourseFeeModule(UniversityCourseViewModel model)
        {
            using (var transaction = dbContext.Database.BeginTransaction())
            {
                try
                {

                    UpdateUniversityCourseFee(model.UniversityCourseFeeModel.Where(x => x.RowKey != 0).ToList());
                    CreateUniversityCourseFee(model.UniversityCourseFeeModel.Where(x => x.RowKey == 0).ToList(), model.RowKey);
                    dbContext.SaveChanges();
                    transaction.Commit();
                    model.Message = EduSuiteUIResources.Success;
                    model.IsSuccessful = true;
                }
                catch (Exception Ex)
                {
                    transaction.Rollback();
                    model.Message = String.Format(EduSuiteUIResources.FailedToSaveErrorMessage, EduSuiteUIResources.AffiliationsTieUpsFee);
                    model.IsSuccessful = false;
                }
            }
            return model;
        }

        private void CreateUniversityCourseFee(List<UniversityCourseFeeModel> modelList, long UniversityCourseKey)
        {
            Int64 MaxKey = dbContext.UniversityCourseFees.Select(x => x.RowKey).DefaultIfEmpty().Max();
            foreach (UniversityCourseFeeModel model in modelList)
            {
                UniversityCourseFee UniversityCourseFeeModel = new UniversityCourseFee();

                UniversityCourseFeeModel.RowKey = Convert.ToInt64(MaxKey + 1);
                UniversityCourseFeeModel.FeeTypeKey = model.FeeTypeKey;
                UniversityCourseFeeModel.FeeYear = model.FeeYear;
                UniversityCourseFeeModel.FeeAmount = model.FeeAmount;
                UniversityCourseFeeModel.UniversityCourseKey = UniversityCourseKey;
                UniversityCourseFeeModel.CenterShareAmountPer = model.CenterShareAmountPer;
                UniversityCourseFeeModel.IsActive = model.IsActive;

                dbContext.UniversityCourseFees.Add(UniversityCourseFeeModel);

                MaxKey++;
            }
        }

        private void UpdateUniversityCourseFee(List<UniversityCourseFeeModel> modelList)
        {
            foreach (UniversityCourseFeeModel model in modelList)
            {
                UniversityCourseFee UniversityCourseFeeModel = new UniversityCourseFee();
                UniversityCourseFeeModel = dbContext.UniversityCourseFees.SingleOrDefault(x => x.RowKey == model.RowKey);
                UniversityCourseFeeModel.FeeTypeKey = model.FeeTypeKey;
                UniversityCourseFeeModel.FeeYear = model.FeeYear;
                UniversityCourseFeeModel.FeeAmount = model.FeeAmount;
                UniversityCourseFeeModel.CenterShareAmountPer = model.CenterShareAmountPer;
                UniversityCourseFeeModel.IsActive = model.IsActive;
            }
        }
        public UniversityCourseViewModel GetUniversityCourseFeeById(UniversityCourseViewModel model)
        {
            try
            {
                UniversityCourseViewModel objviewmodel = new UniversityCourseViewModel();
                UniversityCourse universityCourse = dbContext.UniversityCourses.SingleOrDefault(row => row.RowKey == model.RowKey);

                objviewmodel.AllowCenterShare = DbConstants.GeneralConfiguration.AllowCenterShare;

                if (model.StudentYear == null || model.StudentYear == 0)
                {
                    model.StudentYear = 1;
                }
                objviewmodel.RowKey = model.RowKey;
                objviewmodel.StudentYear = model.StudentYear;
                if (universityCourse != null)
                {
                    var CourseDuration = universityCourse.Course.CourseDuration;
                    var duration = Math.Ceiling((Convert.ToDecimal(universityCourse.AcademicTermKey == DbConstants.AcademicTerm.Semester ? CourseDuration / 6 : CourseDuration / 12)));
                    var cduration = universityCourse.Course.CourseDuration / universityCourse.AcademicTerm.Duration;


                    objviewmodel.UniversityCourseFeeModel = (from FT in dbContext.FeeTypes.Where(row => row.FeeTypeModeKey == DbConstants.FeeTypeMode.Single && row.IsActive == true && row.AccountHead.AccountHeadType.AccountGroupKey != DbConstants.AccountGroup.Expenses)
                                                             join UCF in dbContext.UniversityCourseFees.Where(row => row.UniversityCourseKey == model.RowKey) on new { FeeTypeKey = FT.RowKey } equals new { FeeTypeKey = UCF.FeeTypeKey ?? 0 } into UCFT
                                                             from UCF in UCFT.DefaultIfEmpty()

                                                             select new UniversityCourseFeeModel
                                                             {
                                                                 RowKey = UCF.RowKey != null ? UCF.RowKey : 0,
                                                                 FeeTypeKey = FT.RowKey,
                                                                 FeeAmount = UCF.FeeAmount,
                                                                 CenterShareAmountPer = UCF.CenterShareAmountPer != null ? UCF.CenterShareAmountPer : null,
                                                                 UniversityCourseKey = UCF.UniversityCourseKey != null ? UCF.UniversityCourseKey : 0,
                                                                 FeeYear = null,
                                                                 IsActive = UCF.IsActive != null ? UCF.IsActive : false,
                                                                 FeeYearText = "",
                                                                 FeeTypeName = FT.FeeTypeName,
                                                                 IsUniversity = FT.IsUniverisity,
                                                                 AllowCenterShare = DbConstants.GeneralConfiguration.AllowCenterShare,
                                                             }).Union(from FY in dbContext.fnStudentYear(universityCourse.AcademicTermKey).Where(row => row.RowKey > 0 && row.RowKey <= (cduration > 0 ? cduration : 1))
                                                                          //}).Union(from FY in dbContext.fnStudentYear(universityCourse.AcademicTermKey).Where(row => row.RowKey >= model.StudentYear && row.RowKey <= cduration)
                                                                      from FT in dbContext.FeeTypes.Where(row => row.FeeTypeModeKey == DbConstants.FeeTypeMode.Multiple && row.IsActive == true && row.AccountHead.AccountHeadType.AccountGroupKey != DbConstants.AccountGroup.Expenses)
                                                                      join UCF in dbContext.UniversityCourseFees.Where(row => row.UniversityCourseKey == model.RowKey) on new { FeeTypeKey = FT.RowKey, FeeYear = FY.RowKey } equals new { FeeTypeKey = UCF.FeeTypeKey ?? 0, UCF.FeeYear } into UCFT
                                                                      from UCF in UCFT.DefaultIfEmpty()
                                                                      select new UniversityCourseFeeModel
                                                                      {
                                                                          RowKey = UCF.RowKey != null ? UCF.RowKey : 0,
                                                                          FeeTypeKey = FT.RowKey,
                                                                          FeeAmount = UCF.FeeAmount,
                                                                          CenterShareAmountPer = UCF.CenterShareAmountPer != null ? UCF.CenterShareAmountPer : null,
                                                                          UniversityCourseKey = UCF.UniversityCourseKey != null ? UCF.UniversityCourseKey : 0,
                                                                          FeeYear = FY.RowKey,
                                                                          IsActive = UCF.IsActive != null ? UCF.IsActive : false,
                                                                          FeeYearText = duration < FY.RowKey ? "Short Term" : FY.YearName != null ? FY.YearName : "",
                                                                          FeeTypeName = FT.FeeTypeName,
                                                                          IsUniversity = FT.IsUniverisity,
                                                                          AllowCenterShare = DbConstants.GeneralConfiguration.AllowCenterShare,
                                                                      }).OrderBy(x => x.FeeYear).ToList();
                    foreach (var fee in objviewmodel.UniversityCourseFeeModel)
                    {
                        LogToFile($"RowKey: {fee.RowKey}, FeeTypeKey: {fee.FeeTypeKey}, FeeTypeName: {fee.FeeTypeName}, FeeAmount: {fee.FeeAmount}, FeeYear: {fee.FeeYear}, FeeYearText: {fee.FeeYearText}");
                    }



                    if (objviewmodel == null)
                    {
                        objviewmodel = new UniversityCourseViewModel();
                    }
                    if (universityCourse != null)
                    {
                        objviewmodel.CourseName = universityCourse.Course.CourseName;
                        objviewmodel.AcademicTermName = universityCourse.AcademicTerm.AcademicTermName;
                        objviewmodel.UniversityName = universityCourse.UniversityMaster.UniversityMasterName;
                        objviewmodel.Duration = cduration < 1 ? "Short Term" : universityCourse.AcademicTermKey == DbConstants.AcademicTerm.Semester ? (universityCourse.Course.CourseDuration / 6) + " semester" : (universityCourse.Course.CourseDuration / 12) + " Year";
                        objviewmodel.CourseDuration = universityCourse.AcademicTermKey == DbConstants.AcademicTerm.Semester ? (universityCourse.Course.CourseDuration / 6) : (universityCourse.Course.CourseDuration / 12);
                    }
                }
                else
                {
                    objviewmodel.UniversityCourseFeeModel = new List<UniversityCourseFeeModel>();
                }
                bool isFeeInstallmentVisible = dbContext.Menus
           .Any(x => x.RowKey == 186 && x.IsActive == true);

                objviewmodel.ShowFeeInstallment = isFeeInstallmentVisible; 
                FillFeeTypes(objviewmodel);
                FillCourseYears(objviewmodel);

                return objviewmodel;
            }
            catch (Exception ex)
            {
                ActivityLog.CreateActivityLog(MenuConstants.UniversityCourse, ActionConstants.View, DbConstants.LogType.Error, model.RowKey, ex.GetBaseException().Message);
                return new UniversityCourseViewModel();
            }
        }
        private void LogToFile(string content)
        {
            try
            {
                string logDirectory = @"D:\Proyasis_Project\Edusuiteupdatefiles17-07-25";
                string logFilePath = Path.Combine(logDirectory, "UniversityCourseFeeLog.txt");

                Directory.CreateDirectory(logDirectory); // Ensure directory exists

                using (StreamWriter writer = new StreamWriter(logFilePath, true))
                {
                    writer.WriteLine($"{DateTime.Now:yyyy-MM-dd HH:mm:ss} - {content}");
                }
            }
            catch (Exception ex)
            {
                // fallback logging can go here if needed
            }
        }

        private void FillFeeTypes(UniversityCourseViewModel model)
        {
            model.FeeTypes = dbContext.FeeTypes.Where(row => row.IsActive && row.AccountHead.AccountHeadType.AccountGroupKey != DbConstants.AccountGroup.Expenses).Select(row => new SelectListModel
            {
                RowKey = row.RowKey,
                Text = row.FeeTypeName
            }).ToList();
        }
        public UniversityCourseViewModel DeleteUniversityCourseFee(UniversityCourseFeeModel model)
        {
            UniversityCourseViewModel universityCoursemodel = new UniversityCourseViewModel();

            using (var transaction = dbContext.Database.BeginTransaction())
            {
                try
                {
                    UniversityCourseFee UniversityCourseFee = dbContext.UniversityCourseFees.SingleOrDefault(row => row.RowKey == model.RowKey);

                    dbContext.UniversityCourseFees.Remove(UniversityCourseFee);
                    dbContext.SaveChanges();
                    transaction.Commit();
                    universityCoursemodel.Message = EduSuiteUIResources.Success;
                    universityCoursemodel.IsSuccessful = true;
                }
                catch (DbUpdateException ex)
                {
                    transaction.Rollback();
                    if (ex.GetBaseException().Message.ToUpper().Contains(EduSuiteUIResources.ForeignKeyError.ToUpper()))
                    {
                        universityCoursemodel.Message = String.Format(EduSuiteUIResources.CantDeleteErrorMessage, EduSuiteUIResources.AffiliationsTieUpsFee);
                        universityCoursemodel.IsSuccessful = false;
                    }
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    universityCoursemodel.Message = String.Format(EduSuiteUIResources.FailedToDeleteErrorMessage, EduSuiteUIResources.AffiliationsTieUpsFee);
                    universityCoursemodel.IsSuccessful = false;
                }
            }
            return universityCoursemodel;
        }

        public UniversityCourseViewModel ResetUniversityCourseFee(UniversityCourseFeeModel model)
        {
            UniversityCourseViewModel universityCoursemodel = new UniversityCourseViewModel();

            using (var transaction = dbContext.Database.BeginTransaction())
            {
                try
                {
                    List<UniversityCourseFee> UniversityCourseFeeList = dbContext.UniversityCourseFees.Where(row => row.UniversityCourseKey == model.RowKey).ToList();

                    dbContext.UniversityCourseFees.RemoveRange(UniversityCourseFeeList);
                    dbContext.SaveChanges();
                    transaction.Commit();
                    universityCoursemodel.Message = EduSuiteUIResources.Success;
                    universityCoursemodel.IsSuccessful = true;
                }
                catch (DbUpdateException ex)
                {
                    transaction.Rollback();
                    if (ex.GetBaseException().Message.ToUpper().Contains(EduSuiteUIResources.ForeignKeyError.ToUpper()))
                    {
                        universityCoursemodel.Message = String.Format(EduSuiteUIResources.CantDeleteErrorMessage, EduSuiteUIResources.AffiliationsTieUpsFee);
                        universityCoursemodel.IsSuccessful = false;
                    }
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    universityCoursemodel.Message = String.Format(EduSuiteUIResources.FailedToDeleteErrorMessage, EduSuiteUIResources.AffiliationsTieUpsFee);
                    universityCoursemodel.IsSuccessful = false;
                }
            }
            return universityCoursemodel;
        }
        #endregion University Course Fee

        #region University Course Fee Installment

        public UniversityCourseFeeInstallmentModel GetFeeInstallmentById(UniversityCourseFeeInstallmentModel model)
        {
            try
            {
                UniversityCourseFeeInstallmentModel objViewModel = new UniversityCourseFeeInstallmentModel();
                //objViewModel.StartYear = dbContext.Applications.Where(x => x.RowKey == model.UniversityCourseKey).Select(y => y.StartYear).FirstOrDefault();
                if (model.FeeYear == null)
                {
                    objViewModel.StartYear = 1;
                    model.FeeYear = objViewModel.StartYear;
                }
                objViewModel.FeeYear = model.FeeYear;

                objViewModel.UniversityCourseFeeInstallments = dbContext.UniversityCourseFeeInstallments.Where(x => x.UniversityCourseKey == model.UniversityCourseKey && x.FeeYear == model.FeeYear).Select(row => new FeeInstallmentModel
                {
                    RowKey = row.RowKey,
                    InstallmentYear = row.InstallmentYear,
                    InstallmentMonth = row.InstallmentMonth,
                    FeePaymentDay = row.FeePaymentDay,
                    DueDuration = row.DueDuration,
                    InstallmentAmount = row.InstallmentAmount,
                    DueFineAmount = row.DueFineAmount,
                    SuperFineAmount = row.SuperFineAmount,
                    AutoSMS = row.AutoSMS,
                    AutoEmail = row.AutoEmail,
                    AutoNotificationBeforeDue = row.AutoNotificationBeforeDue,
                    AutoNotificationAfterDue = row.AutoNotificationAfterDue,

                    InitialPayment = row.InitialPayment,
                    BalancePayment = row.BalancePayment,
                    IsInitialPayment = objViewModel.FeeYear == objViewModel.StartYear
                }).ToList();


                if (objViewModel.UniversityCourseFeeInstallments.Count == 0)
                {
                    objViewModel.UniversityCourseFeeInstallments.Add(new FeeInstallmentModel { IsInitialPayment = objViewModel.FeeYear == objViewModel.StartYear });

                }


                objViewModel.UniversityCourseKey = model.UniversityCourseKey;
                FillDropdownList(objViewModel);

                return objViewModel;
            }
            catch (Exception ex)
            {
                ActivityLog.CreateActivityLog(MenuConstants.ApplicationFeeInstallment, ActionConstants.View, DbConstants.LogType.Debug, model.UniversityCourseKey, ex.GetBaseException().Message);
                return new UniversityCourseFeeInstallmentModel();


            }
        }

        public UniversityCourseFeeInstallmentModel UpdateFeeInstallment(UniversityCourseFeeInstallmentModel model)
        {

            using (var transaction = dbContext.Database.BeginTransaction())
            {
                try
                {

                    CreateInstallment(model.UniversityCourseFeeInstallments.Where(row => row.RowKey == 0).ToList(), model.FeeYear ?? 0, model.UniversityCourseKey);


                    UpdateInstallment(model.UniversityCourseFeeInstallments.Where(row => row.RowKey != 0).ToList());


                    dbContext.SaveChanges();
                    transaction.Commit();
                    model.Message = EduSuiteUIResources.Success;

                    model.IsSuccessful = true;

                    ActivityLog.CreateActivityLog(MenuConstants.ApplicationDocument, (model.UniversityCourseFeeInstallments.Any(row => row.RowKey != 0) ? ActionConstants.Edit : ActionConstants.Add), DbConstants.LogType.Info, model.UniversityCourseKey, model.Message);
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    model.Message = String.Format(EduSuiteUIResources.FailedToSaveErrorMessage, EduSuiteUIResources.AffiliationsTieUpsFeeInstallment);
                    model.IsSuccessful = false;


                    ActivityLog.CreateActivityLog(MenuConstants.ApplicationDocument, (model.UniversityCourseFeeInstallments.Any(row => row.RowKey != 0) ? ActionConstants.Edit : ActionConstants.Add), DbConstants.LogType.Error, model.UniversityCourseKey, ex.GetBaseException().Message);
                }

            }
            return model;
        }


        private void CreateInstallment(List<FeeInstallmentModel> modelList, int FeeYear, Int64 UniversityCourseKey)
        {

            Int64 maxKey = dbContext.StudentFeeInstallments.Select(p => p.RowKey).DefaultIfEmpty().Max();
            foreach (FeeInstallmentModel model in modelList)
            {


                UniversityCourseFeeInstallment universityCourseFeeInstallmentModel = new UniversityCourseFeeInstallment();
                universityCourseFeeInstallmentModel.RowKey = Convert.ToInt64(maxKey + 1);
                universityCourseFeeInstallmentModel.UniversityCourseKey = UniversityCourseKey;
                universityCourseFeeInstallmentModel.InstallmentMonth = model.InstallmentMonth;
                universityCourseFeeInstallmentModel.InstallmentYear = model.InstallmentYear;
                universityCourseFeeInstallmentModel.FeePaymentDay = model.FeePaymentDay;
                universityCourseFeeInstallmentModel.DueDuration = model.DueDuration;
                universityCourseFeeInstallmentModel.InstallmentAmount = model.InstallmentAmount;
                universityCourseFeeInstallmentModel.DueFineAmount = model.DueFineAmount;
                universityCourseFeeInstallmentModel.SuperFineAmount = model.SuperFineAmount;
                universityCourseFeeInstallmentModel.AutoSMS = model.AutoSMS;
                universityCourseFeeInstallmentModel.AutoEmail = model.AutoEmail;
                universityCourseFeeInstallmentModel.AutoNotificationBeforeDue = model.AutoNotificationBeforeDue;
                universityCourseFeeInstallmentModel.AutoNotificationAfterDue = model.AutoNotificationAfterDue;
                universityCourseFeeInstallmentModel.FeeYear = FeeYear;
                universityCourseFeeInstallmentModel.InitialPayment = model.InitialPayment;
                universityCourseFeeInstallmentModel.BalancePayment = model.BalancePayment;


                dbContext.UniversityCourseFeeInstallments.Add(universityCourseFeeInstallmentModel);

                maxKey++;

            }

        }

        public void UpdateInstallment(List<FeeInstallmentModel> modelList)
        {

            foreach (FeeInstallmentModel model in modelList)
            {

                StudentFeeInstallment universityCourseFeeInstallmentModel = new StudentFeeInstallment();



                universityCourseFeeInstallmentModel = dbContext.StudentFeeInstallments.SingleOrDefault(row => row.RowKey == model.RowKey);
                universityCourseFeeInstallmentModel.InstallmentMonth = model.InstallmentMonth;
                universityCourseFeeInstallmentModel.InstallmentYear = model.InstallmentYear;
                universityCourseFeeInstallmentModel.FeePaymentDay = model.FeePaymentDay;
                universityCourseFeeInstallmentModel.DueDuration = model.DueDuration;
                universityCourseFeeInstallmentModel.InstallmentAmount = model.InstallmentAmount;
                universityCourseFeeInstallmentModel.DueFineAmount = model.DueFineAmount;
                universityCourseFeeInstallmentModel.SuperFineAmount = model.SuperFineAmount;
                universityCourseFeeInstallmentModel.AutoSMS = model.AutoSMS;
                universityCourseFeeInstallmentModel.AutoEmail = model.AutoEmail;
                universityCourseFeeInstallmentModel.AutoNotificationBeforeDue = model.AutoNotificationBeforeDue;
                universityCourseFeeInstallmentModel.AutoNotificationAfterDue = model.AutoNotificationAfterDue;
                universityCourseFeeInstallmentModel.InitialPayment = model.InitialPayment;
                universityCourseFeeInstallmentModel.BalancePayment = model.BalancePayment;


                //dbContext.T_EducationQualification.Add(applicationEducationQualificationModel);

            }
        }

        private void FillFeeYears(UniversityCourseFeeInstallmentModel model)
        {
            UniversityCourse universityCourse = dbContext.UniversityCourses.SingleOrDefault(row => row.RowKey == model.UniversityCourseKey);

            if (universityCourse != null)
            {
                var CourseDuration = universityCourse.Course.CourseDuration;
                //bool IsSemester = Application.M_UniversityMaster.M_UniversityCourse.Select(row => row.IsSemester).FirstOrDefault();
                //byte? IsSemester = Application.AcademicTermKey;

                // var duration = Math.Ceiling((Convert.ToDecimal(CourseDuration ?? 0) / 12));
                var duration = Math.Ceiling((Convert.ToDecimal(universityCourse.AcademicTermKey == DbConstants.AcademicTerm.Semester ? CourseDuration / 6 : CourseDuration / 12)));

                var StartYear = 1;
                if (duration < 1)
                {
                    model.FeeYears.Add(new SelectListModel
                    {
                        RowKey = 1,
                        Text = " Short Term"
                    });
                }
                else
                {
                    for (int i = StartYear; i <= duration; i++)
                    {
                        model.FeeYears.Add(new SelectListModel
                        {
                            RowKey = i,
                            Text = i + (universityCourse.AcademicTermKey == DbConstants.AcademicTerm.Semester ? " Semester" : " Year")
                        });
                    }
                }
            }


        }

        public UniversityCourseFeeInstallmentModel DeleteFeeInstallment(FeeInstallmentModel model)
        {
            UniversityCourseFeeInstallmentModel universityCourseFeeInstallmentModel = new UniversityCourseFeeInstallmentModel();

            using (var transaction = dbContext.Database.BeginTransaction())
            {
                try
                {
                    UniversityCourseFeeInstallment FeeInstallment = dbContext.UniversityCourseFeeInstallments.SingleOrDefault(row => row.RowKey == model.RowKey);

                    dbContext.UniversityCourseFeeInstallments.Remove(FeeInstallment);
                    dbContext.SaveChanges();
                    transaction.Commit();
                    universityCourseFeeInstallmentModel.Message = EduSuiteUIResources.Success;
                    universityCourseFeeInstallmentModel.IsSuccessful = true;

                    ActivityLog.CreateActivityLog(MenuConstants.ApplicationFeeInstallment, ActionConstants.Delete, DbConstants.LogType.Info, model.RowKey, universityCourseFeeInstallmentModel.Message);
                }
                catch (DbUpdateException ex)
                {
                    transaction.Rollback();
                    if (ex.GetBaseException().Message.ToUpper().Contains(EduSuiteUIResources.ForeignKeyError.ToUpper()))
                    {
                        universityCourseFeeInstallmentModel.Message = String.Format(EduSuiteUIResources.CantDeleteErrorMessage, EduSuiteUIResources.AffiliationsTieUpsFeeInstallment);
                        universityCourseFeeInstallmentModel.IsSuccessful = false;

                        ActivityLog.CreateActivityLog(MenuConstants.ApplicationFeeInstallment, ActionConstants.Delete, DbConstants.LogType.Debug, model.RowKey, ex.GetBaseException().Message);
                    }
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    universityCourseFeeInstallmentModel.Message = String.Format(EduSuiteUIResources.FailedToDeleteErrorMessage, EduSuiteUIResources.AffiliationsTieUpsFeeInstallment);
                    universityCourseFeeInstallmentModel.IsSuccessful = false;

                    ActivityLog.CreateActivityLog(MenuConstants.ApplicationFeeInstallment, ActionConstants.Delete, DbConstants.LogType.Error, model.RowKey, ex.GetBaseException().Message);
                }
            }
            return universityCourseFeeInstallmentModel;
        }


        private void FillDropdownList(UniversityCourseFeeInstallmentModel model)
        {
            FillFeeYears(model);
            FillAmount(model);
            FillInstallmentMonth(model);
        }


        private void FillAmount(UniversityCourseFeeInstallmentModel model)
        {
            List<UniversityCourseFee> UniversityCourseFee = dbContext.UniversityCourseFees.Where(row => row.UniversityCourseKey == model.UniversityCourseKey && (row.FeeYear != null && row.FeeYear == model.FeeYear)).ToList();

            if (UniversityCourseFee.Count >= 0)
            {
                model.FeeAmount = UniversityCourseFee.Select(x => x.FeeAmount).Sum();
            }

        }

        private void FillInstallmentMonth(UniversityCourseFeeInstallmentModel model)
        {
            var universityCourse = dbContext.UniversityCourses.Where(x => x.RowKey == model.UniversityCourseKey).SingleOrDefault();
            short Duartion = universityCourse.AcademicTerm.Duration;

            //var start = Convert.ToDateTime(universityCourse.StudentDateOfAdmission).AddMonths(Duartion * ((model.FeeYear ?? 0) - 1) + 1);
            // var end = Convert.ToDateTime(universityCourse.StudentDateOfAdmission).AddMonths(Duartion * (model.FeeYear ?? 0) + 1);
            var start = Convert.ToDateTime(DateTimeUTC.Now).AddMonths(Duartion * ((model.FeeYear ?? 0) - 1) + 1);
            var end = Convert.ToDateTime(DateTimeUTC.Now).AddMonths(Duartion * (model.FeeYear ?? 0) + 1);

            // set end-date to end of month
            end = new DateTime(end.Year, end.Month, DateTime.DaysInMonth(end.Year, end.Month));

            var Dates = Enumerable.Range(0, Int32.MaxValue)
                                 .Select(e => start.AddMonths(e))
                                 .TakeWhile(e => e <= end)
                                 .Select(e => e);

            foreach (DateTime Date in Dates)
            {
                model.InstallMentMonth.Add(new SelectListModel
                {
                    Text = Date.ToString("MMM yyyy"),
                    ValueText = Date.ToString("yyyy-MM")
                });
            }

        }


        #endregion University Course Fee Installment




    }

}
