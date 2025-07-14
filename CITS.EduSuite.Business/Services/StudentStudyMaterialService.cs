using CITS.EduSuite.Business.Common;
using CITS.EduSuite.Business.Interfaces;
using CITS.EduSuite.Business.Models.ViewModels;
using CITS.EduSuite.Data;
using System;
using System.Collections.Generic;
using System.Data.Entity.Infrastructure;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CITS.EduSuite.Business.Models.Resources;
using System.Data.Common;
using CITS.EduSuite.Business.Extensions;
using System.Linq.Expressions;

namespace CITS.EduSuite.Business.Services
{
    public class StudentStudyMaterialService : IStudentStudyMaterialService
    {

        private EduSuiteDatabase dbContext;
        public StudentStudyMaterialService(EduSuiteDatabase objDB)
        {
            this.dbContext = objDB;
        }

        public List<StudentStudyMaterialViewModel> GetStudyMaterials(StudentStudyMaterialViewModel model, out long TotalRecords)
        {
            try
            {
                var Take = model.PageSize;
                var Skip = (model.PageIndex - 1) * model.PageSize;

                IQueryable<StudentStudyMaterialViewModel> StudyMaterialList = (from a in dbContext.StudentStudyMaterials.Where(x => new[] { x.Subject.SubjectName, x.StudyMaterialTitle }.Any(s => s.Contains(model.SearchText)))
                                                                               select new StudentStudyMaterialViewModel
                                                                               {
                                                                                   RowKey = a.RowKey,
                                                                                   SubjectName = a.Subject.SubjectName,
                                                                                   SubjectKey = a.SubjectKey,
                                                                                   StudyMaterialTitle = a.StudyMaterialTitle,
                                                                                   StudyMaterialDecription = a.StudyMaterialDecription,
                                                                                   IsActive = a.IsActive,
                                                                                   StudyMaterialCount = a.StudentStudyMaterialDetails.Count()
                                                                               });

                if (model.SortBy != "")
                {
                    StudyMaterialList = SortApplications(StudyMaterialList, model.SortBy, model.SortOrder);
                }
                TotalRecords = StudyMaterialList.Count();
                return StudyMaterialList.Skip(Skip).Take(Take).ToList<StudentStudyMaterialViewModel>();
            }
            catch (Exception ex)
            {
                TotalRecords = 0;
                ActivityLog.CreateActivityLog(MenuConstants.StudentStudyMaterial, ActionConstants.MenuAccess, DbConstants.LogType.Error, DbConstants.User.UserKey, ex.GetBaseException().Message);
                return new List<StudentStudyMaterialViewModel>();
            }
        }

        private IQueryable<StudentStudyMaterialViewModel> SortApplications(IQueryable<StudentStudyMaterialViewModel> Query, string SortName, string SortOrder)
        {

            string command = SortOrder == "desc" ? "OrderByDescending" : "OrderBy";
            var type = typeof(StudentStudyMaterialViewModel);
            var property = type.GetProperty(SortName);
            var parameter = Expression.Parameter(type, "p");
            var propertyAccess = Expression.MakeMemberAccess(parameter, property);
            var orderByExpression = Expression.Lambda(propertyAccess, parameter);
            var resultExpression = Expression.Call(typeof(Queryable), command, new Type[] { type, property.PropertyType },
                                          Query.Expression, Expression.Quote(orderByExpression));
            return Query.Provider.CreateQuery<StudentStudyMaterialViewModel>(resultExpression);

        }

        public List<StudentStudyMaterialViewModel> GetStudyMaterialByStudyMaterialKey(StudentStudyMaterialViewModel model)
        {
            try
            {
                IQueryable<StudentStudyMaterialViewModel> StudyMaterialList = (from a in dbContext.StudentStudyMaterials
                                                                               select new StudentStudyMaterialViewModel
                                                                               {
                                                                                   RowKey = a.RowKey,
                                                                                   SubjectName = a.Subject.SubjectName,
                                                                                   SubjectKey = a.SubjectKey,
                                                                                   StudyMaterialTitle = a.StudyMaterialTitle,
                                                                                   StudyMaterialDecription = a.StudyMaterialDecription,
                                                                                   IsActive = a.IsActive,
                                                                                   StudentStudyMaterialDetailsList = a.StudentStudyMaterialDetails.Select(x => new StudentStudyMaterialDetailsViewModel
                                                                                   {
                                                                                       RowKey = x.RowKey,
                                                                                       StudyMaterialName = x.StudyMaterialName,
                                                                                       StudyMaterialFilePath = x.StudyMaterialFileName,
                                                                                       IsActive = x.IsActive,
                                                                                       IsAllowDownload = x.AllowDownload,
                                                                                       IsAllowPreview = x.AllowPreview,
                                                                                       VisibilityTypeKey = x.VisibilityTypeKey,
                                                                                       VisibilityName = x.VisibilityType.VisibilityTypeName,
                                                                                       StudyMaterialViewCount=dbContext.DocumentTracks.Where(y=>y.RowDataKey==x.RowKey && y.DocumentType==DbConstants.DocumentType.PDF && y.IfDownload==false).Count(),
                                                                                       StudyMaterialDownloadCount=dbContext.DocumentTracks.Where(y=>y.RowDataKey==x.RowKey && y.DocumentType==DbConstants.DocumentType.PDF && y.IfDownload==true).Count(),
                                                                                   }).ToList(),
                                                                                   StudyMaterialCount = a.StudentStudyMaterialDetails.Count()
                                                                               });


                if (model.RowKey != 0)
                {
                    StudyMaterialList = StudyMaterialList.Where(x => x.RowKey == model.RowKey);
                }
                if (model.SortBy != "")
                {
                    //StudyMaterialList = SortApplications(StudyMaterialList, model.SortBy, model.SortOrder);
                }

                return StudyMaterialList.ToList<StudentStudyMaterialViewModel>();
            }
            catch (Exception ex)
            {

                ActivityLog.CreateActivityLog(MenuConstants.StudentStudyMaterial, ActionConstants.MenuAccess, DbConstants.LogType.Error, DbConstants.User.UserKey, ex.GetBaseException().Message);
                return new List<StudentStudyMaterialViewModel>();
            }
        }

        public StudentStudyMaterialViewModel GetStudyMaterialById(StudentStudyMaterialViewModel model)
        {
            try
            {

                model = dbContext.StudentStudyMaterials.Where(x => x.RowKey == model.RowKey).Select(row => new StudentStudyMaterialViewModel
                {
                    RowKey = row.RowKey,
                    SubjectKey = row.SubjectKey,
                    StudyMaterialTitle = row.StudyMaterialTitle,
                    SubjectName = row.Subject.SubjectName,
                    StudyMaterialDecription = row.StudyMaterialDecription,
                    IsActive = row.IsActive,
                    StudentStudyMaterialDetailsList = row.StudentStudyMaterialDetails.Select(y => new StudentStudyMaterialDetailsViewModel
                    {
                        RowKey = y.RowKey,
                        StudyMaterialName = y.StudyMaterialName,
                        StudyMaterialFilePath = y.StudyMaterialFileName != null ? UrlConstants.StudyMaterialUpload + row.RowKey + "/" + y.StudyMaterialFileName : y.StudyMaterialFileName,
                        IsAllowPreview=y.AllowPreview,
                        IsActive = y.IsActive,
                        IsAllowDownload = y.AllowDownload,
                        VisibilityTypeKey = y.VisibilityTypeKey,
                        VisibilityName = y.VisibilityType.VisibilityTypeName
                    }).ToList(),
                }).SingleOrDefault();

                if (model == null)
                {
                    model = new StudentStudyMaterialViewModel();
                    model.IsActive = true;
                }
                else
                {
                    //if (model.PlanKeysList != null)
                    //{
                    //    model.PlanKeys = model.PlanKeysList.Split(',').Select(byte.Parse).ToList();
                    //}
                }
                if (model.StudentStudyMaterialDetailsList.Count == 0)
                {
                    model.StudentStudyMaterialDetailsList.Add(new StudentStudyMaterialDetailsViewModel());
                }


                //model.StudyMaterialsList = new List<StudyMaterialsList>();


                FillDropDownList(model);


                return model;
            }
            catch (Exception ex)
            {
                ActivityLog.CreateActivityLog(MenuConstants.StudentStudyMaterial, ActionConstants.View, DbConstants.LogType.Error, model.RowKey, ex.GetBaseException().Message);
                return new StudentStudyMaterialViewModel();
            }
        }

        public StudentStudyMaterialViewModel CreateStudyMaterial(StudentStudyMaterialViewModel model)
        {
            Int64 maxKey = dbContext.StudentStudyMaterials.Select(p => p.RowKey).DefaultIfEmpty().Max();

            using (var transaction = dbContext.Database.BeginTransaction())
            {

                try
                {

                    StudentStudyMaterial dbModel = new StudentStudyMaterial();
                    dbModel.RowKey = (maxKey + 1);
                    dbModel.SubjectKey = model.SubjectKey;
                    dbModel.StudyMaterialTitle = model.StudyMaterialTitle;
                    dbModel.IsActive = model.IsActive;
                    dbModel.StudyMaterialDecription = model.StudyMaterialDecription;
                    dbContext.StudentStudyMaterials.Add(dbModel);
                    model.RowKey = dbModel.RowKey;
                    CreateStudyMaterialDetails(model);
                    dbContext.SaveChanges();
                    transaction.Commit();
                    model.Message = EduSuiteUIResources.Success;
                    model.IsSuccessful = true;
                    ActivityLog.CreateActivityLog(MenuConstants.StudentStudyMaterial, ActionConstants.Add, DbConstants.LogType.Info, dbModel.RowKey, model.Message);
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    model.Message = String.Format(EduSuiteUIResources.FailedToSaveErrorMessage, EduSuiteUIResources.StudyMaterial);
                    model.IsSuccessful = false;
                    ActivityLog.CreateActivityLog(MenuConstants.StudentStudyMaterial, ActionConstants.Add, DbConstants.LogType.Error, model.RowKey, ex.GetBaseException().Message);
                }
            }
            return model;

        }

        public StudentStudyMaterialViewModel UpdateStudyMaterial(StudentStudyMaterialViewModel model)
        {

            using (var transaction = dbContext.Database.BeginTransaction())
            {

                try
                {

                    StudentStudyMaterial dbModel = dbContext.StudentStudyMaterials.Where(x => x.RowKey == model.RowKey).SingleOrDefault();
                    dbModel.SubjectKey = model.SubjectKey;
                    dbModel.StudyMaterialTitle = model.StudyMaterialTitle;
                    dbModel.StudyMaterialDecription = model.StudyMaterialDecription;
                    dbModel.IsActive = model.IsActive;
                    UpdateStudyMaterialDetails(model);
                    CreateStudyMaterialDetails(model);
                    dbContext.SaveChanges();
                    transaction.Commit();
                    model.Message = EduSuiteUIResources.Success;
                    model.IsSuccessful = true;
                    ActivityLog.CreateActivityLog(MenuConstants.StudentStudyMaterial, ActionConstants.Edit, DbConstants.LogType.Info, dbModel.RowKey, model.Message);


                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    model.Message = String.Format(EduSuiteUIResources.FailedToSaveErrorMessage, EduSuiteUIResources.StudyMaterial);
                    model.IsSuccessful = false;
                    ActivityLog.CreateActivityLog(MenuConstants.StudentStudyMaterial, ActionConstants.Add, DbConstants.LogType.Error, model.RowKey, ex.GetBaseException().Message);

                }
            }
            return model;

        }

        public void CreateStudyMaterialDetails(StudentStudyMaterialViewModel model)
        {
            Int64 maxKey = dbContext.StudentStudyMaterialDetails.Select(p => p.RowKey).DefaultIfEmpty().Max();

            foreach (var item in model.StudentStudyMaterialDetailsList.Where(x => x.RowKey == 0))
            {
                StudentStudyMaterialDetail dbModel = new StudentStudyMaterialDetail();


                dbModel.RowKey = (maxKey + 1);
                dbModel.StudyMaterialName = item.StudyMaterialName;
                dbModel.StudentStudyMaterialKey = model.RowKey;
                dbModel.IsActive = item.IsActive;
                dbModel.AllowDownload = item.IsAllowDownload;
                dbModel.AllowPreview = item.IsAllowPreview;
                dbModel.VisibilityTypeKey = item.VisibilityTypeKey ?? 1;

                if (item.StudyMaterialFileAttachment != null)
                {
                    dbModel.StudyMaterialFileName = dbModel.RowKey + item.StudyMaterialFilePath;
                }

                dbContext.StudentStudyMaterialDetails.Add(dbModel);
                maxKey++;
                item.StudyMaterialFilePath = dbModel.StudyMaterialFileName;
            }
        }

        public void UpdateStudyMaterialDetails(StudentStudyMaterialViewModel model)
        {

            foreach (var item in model.StudentStudyMaterialDetailsList.Where(x => x.RowKey > 0))
            {
                StudentStudyMaterialDetail dbModel = dbContext.StudentStudyMaterialDetails.Where(x => x.RowKey == item.RowKey).SingleOrDefault();
                dbModel.StudyMaterialName = item.StudyMaterialName;
                dbModel.StudentStudyMaterialKey = model.RowKey;
                dbModel.AllowDownload = item.IsAllowDownload;
                dbModel.AllowPreview = item.IsAllowPreview;
                dbModel.IsActive = item.IsActive;
                dbModel.VisibilityTypeKey = item.VisibilityTypeKey ?? 1;

                if (item.StudyMaterialFileAttachment != null)
                {
                    dbModel.StudyMaterialFileName = dbModel.RowKey + item.StudyMaterialFilePath;
                }

                item.StudyMaterialFilePath = dbModel.StudyMaterialFileName;

            }
        }

        public void FillDropDownList(StudentStudyMaterialViewModel model)
        {
            FillSubjects(model);
            FillVisibility(model);
        }

        private void FillSubjects(StudentStudyMaterialViewModel model)
        {
            model.Subjects = dbContext.Subjects.Select(row => new SelectListModel
            {
                RowKey = row.RowKey,
                Text = row.SubjectName
            }).ToList();
        }

        private void FillVisibility(StudentStudyMaterialViewModel model)
        {
            model.VisibilityTypes = dbContext.VisibilityTypes.Select(row => new SelectListModel
            {
                RowKey = row.RowKey,
                Text = row.VisibilityTypeName
            }).ToList();
        }


        //private void FillPlans(StudentStudyMaterialViewModel model)
        //{
        //    model.Plans = dbContext.PlanMasters.Select(row => new SelectListModel
        //    {
        //        RowKey = row.RowKey,
        //        Text = row.PlanName
        //    }).ToList();
        //}


        public StudentStudyMaterialViewModel DeleteStudyMaterial(StudentStudyMaterialViewModel model)
        {
            using (var transaction = dbContext.Database.BeginTransaction())
            {
                try
                {
                    StudentStudyMaterial dbStudentStudyMaterial = dbContext.StudentStudyMaterials.SingleOrDefault(row => row.RowKey == model.RowKey);
                    List<StudentStudyMaterialDetail> dbStudentStudyMaterialDetail = dbContext.StudentStudyMaterialDetails.Where(row => row.StudentStudyMaterialKey == model.RowKey).ToList();
                    dbContext.StudentStudyMaterialDetails.RemoveRange(dbStudentStudyMaterialDetail);
                    dbContext.StudentStudyMaterials.Remove(dbStudentStudyMaterial);
                    dbContext.SaveChanges();
                    transaction.Commit();
                    model.Message = EduSuiteUIResources.Success;
                    model.IsSuccessful = true;
                    ActivityLog.CreateActivityLog(MenuConstants.StudentStudyMaterial, ActionConstants.Delete, DbConstants.LogType.Info, model.RowKey, model.Message);
                }
                catch (DbUpdateException ex)
                {
                    transaction.Rollback();
                    if (ex.GetBaseException().Message.ToUpper().Contains(EduSuiteUIResources.ForeignKeyError.ToUpper()))
                    {
                        model.Message = String.Format(EduSuiteUIResources.CantDeleteErrorMessage, EduSuiteUIResources.StudyMaterial);
                        model.IsSuccessful = false;
                    }
                    ActivityLog.CreateActivityLog(MenuConstants.StudentStudyMaterial, ActionConstants.Delete, DbConstants.LogType.Error, model.RowKey, ex.GetBaseException().Message);
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    model.Message = String.Format(EduSuiteUIResources.FailedToDeleteErrorMessage, EduSuiteUIResources.StudyMaterial);
                    model.IsSuccessful = false;
                    ActivityLog.CreateActivityLog(MenuConstants.StudentStudyMaterial, ActionConstants.Delete, DbConstants.LogType.Error, model.RowKey, ex.GetBaseException().Message);
                }

                return model;
            }
        }

        public StudentStudyMaterialViewModel DeleteStudyMaterialDetails(long? RowKey)
        {
            StudentStudyMaterialViewModel model = new StudentStudyMaterialViewModel();
            using (var transaction = dbContext.Database.BeginTransaction())
            {
                try
                {
                    StudentStudyMaterialDetail dbStudyMaterialDetails = dbContext.StudentStudyMaterialDetails.SingleOrDefault(row => row.RowKey == RowKey);
                    model.StudentStudyMaterialDetailsList.Add(new StudentStudyMaterialDetailsViewModel { StudyMaterialFilePath = dbStudyMaterialDetails.StudyMaterialFileName });
                    model.RowKey = dbStudyMaterialDetails.StudentStudyMaterialKey;
                    dbContext.StudentStudyMaterialDetails.Remove(dbStudyMaterialDetails);
                    dbContext.SaveChanges();
                    transaction.Commit();
                    model.Message = EduSuiteUIResources.Success;
                    model.IsSuccessful = true;
                    ActivityLog.CreateActivityLog(MenuConstants.StudentStudyMaterial, ActionConstants.Delete, DbConstants.LogType.Info, RowKey, model.Message);
                }
                catch (DbUpdateException ex)
                {
                    transaction.Rollback();
                    if (ex.GetBaseException().Message.ToUpper().Contains(EduSuiteUIResources.ForeignKeyError.ToUpper()))
                    {
                        model.Message = String.Format(EduSuiteUIResources.CantDeleteErrorMessage, EduSuiteUIResources.Video);
                        model.IsSuccessful = false;
                    }
                    ActivityLog.CreateActivityLog(MenuConstants.StudentStudyMaterial, ActionConstants.Delete, DbConstants.LogType.Error, RowKey, ex.GetBaseException().Message);
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    model.Message = String.Format(EduSuiteUIResources.FailedToDeleteErrorMessage, EduSuiteUIResources.Video);
                    model.IsSuccessful = false;
                    ActivityLog.CreateActivityLog(MenuConstants.StudentStudyMaterial, ActionConstants.Delete, DbConstants.LogType.Error, RowKey, ex.GetBaseException().Message);
                }

                return model;
            }
        }



        //for students
        public StudentStudyMaterialViewModel GetStudyMaterialCategories(StudentStudyMaterialViewModel model)
        {

            long userkey = DbConstants.User.UserKey;

            List<long> SubjectKeys = (from a in dbContext.Applications
                                      join b in dbContext.CourseSubjectMasters on a.CourseKey equals b.CourseKey
                                      join c in dbContext.CourseSubjectDetails on b.RowKey equals c.CourseSubjectMasterKey
                                      join s in dbContext.Subjects on c.SubjectKey equals s.RowKey
                                      where a.UniversityMasterKey == b.UniversityMasterKey && a.AcademicTermKey == b.AcademicTermKey
                                      && a.AppUserKey == userkey
                                      select new CourseSubjectDetailViewModel
                                      {
                                          SubjectKey = c.SubjectKey
                                      }).Select(x => x.SubjectKey).ToList();

            model.SubjectsList = (from a in dbContext.Subjects
                                  where a.IsActive && SubjectKeys.Contains(a.RowKey)
                                  select new StudyMaterialCategoryList
                                  {
                                      SubjectKey = a.RowKey,
                                      SubjectName = a.SubjectName,
                                  }).ToList();

            return model;

        }

        public StudentStudyMaterialViewModel GetStudyMaterialList(StudentStudyMaterialViewModel model)
        {

            var Take = model.PageSize;
            var skip = (model.PageIndex - 1) * model.PageSize;

            IQueryable<StudentStudyMaterialViewModel> StudyMaterialQuarys = (
                               from VD in dbContext.StudentStudyMaterials.OrderByDescending(row => new { row.DateAdded })
                               where VD.IsActive && VD.Subject.IsActive
                               select new StudentStudyMaterialViewModel
                               {
                                   RowKey = VD.RowKey,
                                   SubjectName = VD.Subject.SubjectName,
                                   StudyMaterialTitle = VD.StudyMaterialTitle,
                                   SubjectKey = VD.SubjectKey,
                                   StudyMaterialDecription = VD.StudyMaterialDecription,
                                   StudentStudyMaterialDetailsList = VD.StudentStudyMaterialDetails.Select(x => new StudentStudyMaterialDetailsViewModel
                                   {
                                       RowKey = x.RowKey,
                                       StudyMaterialName = x.StudyMaterialName,
                                       //StudyMaterialFilePath = x.StudyMaterialFileName,
                                       StudyMaterialFilePath = x.StudyMaterialFileName != null ? UrlConstants.StudyMaterialUpload + VD.RowKey + "/" + x.StudyMaterialFileName : x.StudyMaterialFileName,
                                       IsActive = x.IsActive,
                                       IsAllowDownload = x.AllowDownload,
                                       IsAllowPreview = x.AllowPreview,
                                       VisibilityTypeKey = x.VisibilityTypeKey,
                                       VisibilityName = x.VisibilityType.VisibilityTypeName
                                   }).ToList(),


                               });


            if (model.SubjectKey != 0)
            {
                StudyMaterialQuarys = StudyMaterialQuarys.Where(x => x.SubjectKey == model.SubjectKey);
            }

            model.TotalRecords = StudyMaterialQuarys.Count();
            model.StudyMaterials = StudyMaterialQuarys.OrderByDescending(Row => Row.RowKey).Skip(skip).Take(Take).ToList();

            return model;

        }
    }
}
