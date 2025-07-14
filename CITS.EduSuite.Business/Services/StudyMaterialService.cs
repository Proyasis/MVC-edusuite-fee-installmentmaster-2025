using System;
using System.Collections.Generic;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CITS.EduSuite.Business.Common;
using CITS.EduSuite.Business.Interfaces;
using CITS.EduSuite.Business.Models.ViewModels;
using CITS.EduSuite.Data;
using System.Linq.Expressions;
using CITS.EduSuite.Business.Models.Resources;
using System.Data.Common;
using CITS.EduSuite.Business.Extensions;

namespace CITS.EduSuite.Business.Services
{
    public class StudyMaterialService : IStudyMaterialService
    {
        private EduSuiteDatabase dbContext;

        public StudyMaterialService(EduSuiteDatabase objDB)
        {
            this.dbContext = objDB;
        }


        public StudyMaterialViewModel GetStudyMaterialById(StudyMaterialViewModel model)
        {
            try
            {

                IQueryable<StudyMaterialDetailsModel> StudyMaterialList = dbContext.IssueOfStudyMaterials.Where(x => x.ApplicationKey == model.ApplicationKey).Select(row => new StudyMaterialDetailsModel
                {
                    RowKey = row.RowKey,
                    StudyMaterialKey = row.StudyMaterialKey,
                    StudyMaterialName = row.StudyMaterial.StudyMaterialName,
                    StudyMaterialCode = row.StudyMaterial.StudyMaterialCode,
                    SubjectYear = row.StudyMaterial.Subject.CourseSubjectDetails.Select(y => y.CourseSubjectMaster.CourseYear).FirstOrDefault(),
                    IsAvailable = row.IsAvailable,
                    AcademicTermKey = row.StudyMaterial.Subject.CourseSubjectDetails.Select(x => x.CourseSubjectMaster.AcademicTermKey).FirstOrDefault(),
                    CourseDuration = row.StudyMaterial.Subject.CourseSubjectDetails.Select(x => x.CourseSubjectMaster.Course.CourseDuration).FirstOrDefault(),
                    IsIssued = row.IsIssued,
                    SubjectYearText = "",
                    IssuedDate = row.IssuedDate,
                    IssuedByText = dbContext.AppUsers.Where(x => x.RowKey == row.IssuedBy).Select(y => y.AppUserName).FirstOrDefault(),
                });

                if (model.StudyMaterialName != "")
                {
                    StudyMaterialList = StudyMaterialList.Where(x => x.StudyMaterialName.Contains(model.StudyMaterialName));
                }
                if (model.SubjectYear != 0)
                {
                    StudyMaterialList = StudyMaterialList.Where(x => x.SubjectYear == model.SubjectYear);
                }


                model.StudyMaterialList = StudyMaterialList.OrderBy(x => x.SubjectYear).ToList();
                foreach (StudyMaterialDetailsModel MaterialList in model.StudyMaterialList)
                {
                    MaterialList.SubjectYearText = CommonUtilities.GetYearDescriptionByCodeDetails(MaterialList.CourseDuration ?? 0, MaterialList.SubjectYear ?? 0, MaterialList.AcademicTermKey ?? 0);

                }


                model.ApplicationKey = model.ApplicationKey;

                return model;
            }
            catch (Exception ex)
            {
                ActivityLog.CreateActivityLog(MenuConstants.StudyMaterial, ActionConstants.View, DbConstants.LogType.Error, model.ApplicationKey, ex.GetBaseException().Message);
                return new StudyMaterialViewModel();


            }
        }

        public void BindAvailableBooks(StudyMaterialViewModel model)
        {

            IQueryable<StudyMaterialDetailsModel> AvailableBookDetails = (from A in dbContext.F_StudentBooks(model.ApplicationKey).Where(x => x.HasStudyMaterial)
                                                                          join B in dbContext.IssueOfStudyMaterials
                                                                          on new { StudyMaterialKey = A.RowKey, A.ApplicationKey } equals new { B.StudyMaterialKey, B.ApplicationKey } into B
                                                                          from A1 in B.DefaultIfEmpty()
                                                                          where A1.RowKey == null
                                                                          select new StudyMaterialDetailsModel
                                                                          {
                                                                              StudyMaterialKey = A.RowKey,
                                                                              StudyMaterialName = A.StudyMaterialName,
                                                                              StudyMaterialCode = A.StudyMaterialCode,
                                                                              SubjectYear = A.CourseYear,
                                                                              CourseDuration = A.CourseDuration,
                                                                              AcademicTermKey = A.AcademicTermKey,
                                                                              SubjectYearText = "",
                                                                          });
            if (model.StudyMaterialName != "")
            {
                AvailableBookDetails = AvailableBookDetails.Where(x => x.StudyMaterialName.Contains(model.StudyMaterialName));
            }
            if (model.SubjectYear != 0)
            {
                AvailableBookDetails = AvailableBookDetails.Where(x => x.SubjectYear == model.SubjectYear);
            }


            model.StudyMaterialList = AvailableBookDetails.ToList();
            foreach (StudyMaterialDetailsModel MaterialList in model.StudyMaterialList)
            {
                MaterialList.SubjectYearText = CommonUtilities.GetYearDescriptionByCodeDetails(MaterialList.CourseDuration ?? 0, MaterialList.SubjectYear ?? 0, MaterialList.AcademicTermKey ?? 0);

            }
        }

        public StudyMaterialViewModel UpdateStudyMaterial(StudyMaterialViewModel model)
        {
            using (var transaction = dbContext.Database.BeginTransaction())
            {
                try
                {
                    //List<long> BookKeyList = model.BookKeys.Split(',').Select(Int64.Parse).ToList();

                    foreach (StudyMaterialDetailsModel StudyMaterial in model.StudyMaterialList)
                    {
                        IssueOfStudyMaterial issueOfStudyMaterialsModel = new IssueOfStudyMaterial();
                        issueOfStudyMaterialsModel = dbContext.IssueOfStudyMaterials.SingleOrDefault(row => row.RowKey == StudyMaterial.RowKey);
                        issueOfStudyMaterialsModel.ApplicationKey = model.ApplicationKey;
                        issueOfStudyMaterialsModel.StudyMaterialKey = StudyMaterial.StudyMaterialKey;
                        issueOfStudyMaterialsModel.IsAvailable = true;
                        issueOfStudyMaterialsModel.IsIssued = true;
                        issueOfStudyMaterialsModel.IssuedDate = DateTimeUTC.Now;
                        issueOfStudyMaterialsModel.IssuedBy = DbConstants.User.UserKey;

                    }
                    dbContext.SaveChanges();
                    transaction.Commit();
                    model.Message = EduSuiteUIResources.Success;

                    model.IsSuccessful = true;

                    ActivityLog.CreateActivityLog(MenuConstants.StudyMaterial, ActionConstants.Edit, DbConstants.LogType.Info, model.ApplicationKey, model.Message);

                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    model.Message = String.Format(EduSuiteUIResources.FailedToSaveErrorMessage, EduSuiteUIResources.StudyMaterial);
                    model.IsSuccessful = false;
                    ActivityLog.CreateActivityLog(MenuConstants.StudyMaterial, ActionConstants.Edit, DbConstants.LogType.Error, model.ApplicationKey, ex.GetBaseException().Message);
                }

            }
            return model;
        }

        public StudyMaterialViewModel CreateStudyMaterial(StudyMaterialViewModel model)
        {
            using (var transaction = dbContext.Database.BeginTransaction())
            {

                try
                {

                    if (model.StudyMaterialKeys != null)
                    {
                        List<long> StudyMaterialKeyList = model.StudyMaterialKeys.Split(',').Select(Int64.Parse).ToList();

                        Int64 maxKey = dbContext.IssueOfStudyMaterials.Select(p => p.RowKey).DefaultIfEmpty().Max();
                        foreach (long StudyMaterialKey in StudyMaterialKeyList)
                        {

                            IssueOfStudyMaterial issueOfStudyMaterialsModel = new IssueOfStudyMaterial();
                            issueOfStudyMaterialsModel.RowKey = Convert.ToInt64(maxKey + 1);
                            issueOfStudyMaterialsModel.ApplicationKey = model.ApplicationKey;
                            issueOfStudyMaterialsModel.StudyMaterialKey = StudyMaterialKey;
                            issueOfStudyMaterialsModel.IsAvailable = true;
                            issueOfStudyMaterialsModel.AvailableDate = DateTimeUTC.Now;
                            issueOfStudyMaterialsModel.AvailableBy = DbConstants.User.UserKey;
                            issueOfStudyMaterialsModel.IsIssued = false;
                            dbContext.IssueOfStudyMaterials.Add(issueOfStudyMaterialsModel);

                            maxKey++;
                        }
                        dbContext.SaveChanges();
                        transaction.Commit();
                        model.Message = EduSuiteUIResources.Success;

                        model.IsSuccessful = true;
                        ActivityLog.CreateActivityLog(MenuConstants.StudyMaterial, ActionConstants.Add, DbConstants.LogType.Info, model.ApplicationKey, model.Message);

                    }
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    model.Message = String.Format(EduSuiteUIResources.FailedToSaveErrorMessage, EduSuiteUIResources.StudyMaterial);
                    model.IsSuccessful = false;
                    ActivityLog.CreateActivityLog(MenuConstants.StudyMaterial, ActionConstants.Add, DbConstants.LogType.Error, model.ApplicationKey, ex.GetBaseException().Message);
                }

            }
            return model;
        }

        public StudyMaterialViewModel DeleteStudyMaterial(StudyMaterialDetailsModel model)
        {
            StudyMaterialViewModel studyMaterialModel = new StudyMaterialViewModel();
            using (var transaction = dbContext.Database.BeginTransaction())
            {
                try
                {
                    IssueOfStudyMaterial IssueOfStudyMaterials = dbContext.IssueOfStudyMaterials.SingleOrDefault(row => row.RowKey == model.RowKey);

                    dbContext.IssueOfStudyMaterials.Remove(IssueOfStudyMaterials);
                    dbContext.SaveChanges();
                    transaction.Commit();
                    studyMaterialModel.Message = EduSuiteUIResources.Success;
                    studyMaterialModel.IsSuccessful = true;
                    ActivityLog.CreateActivityLog(MenuConstants.StudyMaterial, ActionConstants.Delete, DbConstants.LogType.Info, model.RowKey, studyMaterialModel.Message);



                }
                catch (DbUpdateException ex)
                {
                    transaction.Rollback();
                    if (ex.GetBaseException().Message.ToUpper().Contains(EduSuiteUIResources.ForeignKeyError.ToUpper()))
                    {
                        studyMaterialModel.Message = String.Format(EduSuiteUIResources.CantDeleteErrorMessage, EduSuiteUIResources.StudyMaterial);
                        studyMaterialModel.IsSuccessful = false;
                        ActivityLog.CreateActivityLog(MenuConstants.StudyMaterial, ActionConstants.Delete, DbConstants.LogType.Error, model.RowKey, ex.GetBaseException().Message);
                    }
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    studyMaterialModel.Message = String.Format(EduSuiteUIResources.FailedToDeleteErrorMessage, EduSuiteUIResources.StudyMaterial);
                    studyMaterialModel.IsSuccessful = false;
                    ActivityLog.CreateActivityLog(MenuConstants.StudyMaterial, ActionConstants.Delete, DbConstants.LogType.Error, model.RowKey, ex.GetBaseException().Message);
                }
            }
            return studyMaterialModel;
        }

        public List<dynamic> GetApplications(ApplicationViewModel model, out long TotalRecords)
        {
            try
            {
                var Take = model.PageSize;
                var Skip = (model.PageIndex - 1) * model.PageSize;


                List<dynamic> applicationList = new List<dynamic>();
                DbParameter TotalRecordsParam = null;

                if (model.SortBy != "")
                {
                    model.SortBy = model.SortBy + " " + model.SortOrder;
                }
                dbContext.LoadStoredProc("dbo.SP_StudentStudyMaterialDetails")
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
                ActivityLog.CreateActivityLog(MenuConstants.StudyMaterial, ActionConstants.View, DbConstants.LogType.Error, null, ex.GetBaseException().Message);
                return new List<dynamic>();


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

        public void FillFeeYears(StudyMaterialViewModel model)
        {
            Application Application = dbContext.Applications.SingleOrDefault(row => row.RowKey == model.ApplicationKey);

            if (Application != null)
            {
                var CourseDuration = Application.Course.CourseDuration;
                //bool IsSemester = Application.M_UniversityMaster.M_UniversityCourse.Select(row => row.IsSemester).FirstOrDefault();
                //byte? IsSemester = Application.AcademicTermKey;

                // var duration = Math.Ceiling((Convert.ToDecimal(CourseDuration ?? 0) / 12));
                var duration = Math.Ceiling((Convert.ToDecimal(Application.AcademicTermKey == DbConstants.AcademicTerm.Semester ? CourseDuration / 6 : CourseDuration / 12)));

                var StartYear = 0;
                //if (Application.ModeKey == DbConstants.Mode.RE_ADMISSION)
                //{
                //    StartYear = 1;
                //}
                //else
                //{
                //    StartYear = Application.StartYear ?? 0;

                //}
                StartYear = Application.StartYear ?? 0;
                if (duration < 1)
                {
                    model.SubjectYears.Add(new SelectListModel
                    {
                        RowKey = 1,
                        Text = " Short Term"
                    });
                }
                else
                {
                    for (int i = StartYear; i <= duration; i++)
                    {
                        model.SubjectYears.Add(new SelectListModel
                        {
                            RowKey = i,
                            Text = i + (Application.AcademicTermKey == DbConstants.AcademicTerm.Semester ? " Semester" : " Year")
                        });
                    }
                }
            }


        }

        public StudyMaterialViewModel UpdateStudyMaterials(StudyMaterialViewModel MasterModel)
        {
            using (var transaction = dbContext.Database.BeginTransaction())
            {

                try
                {

                    MasterModel.StudyMaterialList = MasterModel.StudyMaterialList.Except(CheckStudyMarialAvailableExists(MasterModel.StudyMaterialList)).ToList();
                    Int64 maxKey = dbContext.IssueOfStudyMaterials.Select(p => p.RowKey).DefaultIfEmpty().Max();
                    dbContext.Configuration.AutoDetectChangesEnabled = false;
                    int Count = 0;
                    foreach (StudyMaterialDetailsModel model in MasterModel.StudyMaterialList)
                    {
                        ++Count;

                        dbContext.AddToContext(new IssueOfStudyMaterial
                        {
                            RowKey = ++maxKey,
                            ApplicationKey = model.ApplicationKey ?? 0,
                            StudyMaterialKey = model.StudyMaterialKey,
                            IsAvailable = model.IsAvailable,
                            AvailableBy = DbConstants.User.UserKey,
                            AvailableDate = DateTimeUTC.Now

                        }, Count);



                    }
                    dbContext.SaveChanges();
                    dbContext.Configuration.AutoDetectChangesEnabled = true;
                    transaction.Commit();

                    MasterModel.Message = EduSuiteUIResources.Success;
                    MasterModel.IsSuccessful = true;
                    ActivityLog.CreateActivityLog(MenuConstants.BookIssueReturn, ActionConstants.Edit, DbConstants.LogType.Info, DbConstants.User.UserKey, MasterModel.Message);

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
                    ActivityLog.CreateActivityLog(MenuConstants.BookIssueReturn, ActionConstants.Edit, DbConstants.LogType.Error, DbConstants.User.UserKey, dbEx.GetBaseException().Message);

                    throw raise;

                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    MasterModel.Message = String.Format(EduSuiteUIResources.FailedToSaveErrorMessage, EduSuiteUIResources.BookIssue);
                    MasterModel.IsSuccessful = false;
                    ActivityLog.CreateActivityLog(MenuConstants.BookIssueReturn, ActionConstants.Edit, DbConstants.LogType.Error, DbConstants.User.UserKey, ex.GetBaseException().Message);

                }
            }
            return MasterModel;
        }
        public List<StudyMaterialDetailsModel> CheckStudyMarialAvailableExists(List<StudyMaterialDetailsModel> modelList)
        {
            EnquiryLeadViewModel model = new EnquiryLeadViewModel();
            var result = (from DL in modelList
                          join EL in dbContext.IssueOfStudyMaterials
                          on new { ApplicationKey = DL.ApplicationKey ?? 0, DL.StudyMaterialKey } equals new { EL.ApplicationKey, EL.StudyMaterialKey }
                          select DL).ToList();
            return result;
        }

    }
}
