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
using CITS.EduSuite.Business.Models.Resources;
using System.Linq.Expressions;

namespace CITS.EduSuite.Business.Services
{
    public class SyllabusAndStudyMaterialService : ISyllabusAndStudyMaterialService
    {
        private EduSuiteDatabase dbContext;

        public SyllabusAndStudyMaterialService(EduSuiteDatabase ObjViewDataBase)
        {
            this.dbContext = ObjViewDataBase;
        }
        public List<SyllabusAndStudyMaterialViewModel> GetSubject(SyllabusAndStudyMaterialViewModel model, out long TotalRecords)
        {
            try
            {
                var Take = model.PageSize;
                var Skip = (model.PageIndex - 1) * model.PageSize;

                IQueryable<SyllabusAndStudyMaterialViewModel> SubjectList = (from s in dbContext.Subjects
                                                                             where (s.SubjectCode.Contains(model.SubjectName) || s.SubjectName.Contains(model.SubjectName))
                                                                             select new SyllabusAndStudyMaterialViewModel
                                                                             {
                                                                                 Course = s.CourseSubjectDetails.Where(p => p.SubjectKey == s.RowKey).Select(x => x.CourseSubjectMaster.Course.CourseName).FirstOrDefault(),
                                                                                 CourseKey = s.CourseSubjectDetails.Where(p => p.SubjectKey == s.RowKey).Select(x => x.CourseSubjectMaster.CourseKey).FirstOrDefault(),
                                                                                 University = s.CourseSubjectDetails.Where(p => p.SubjectKey == s.RowKey).Select(x => x.CourseSubjectMaster.UniversityMaster.UniversityMasterName).FirstOrDefault(),
                                                                                 UniversityMasterKey = s.CourseSubjectDetails.Where(p => p.SubjectKey == s.RowKey).Select(x => x.CourseSubjectMaster.UniversityMasterKey).FirstOrDefault(),
                                                                                 AcademicTermName = s.CourseSubjectDetails.Where(p => p.SubjectKey == s.RowKey).Select(x => x.CourseSubjectMaster.AcademicTerm.AcademicTermName).FirstOrDefault(),
                                                                                 AcademicTermKey = s.CourseSubjectDetails.Where(p => p.SubjectKey == s.RowKey).Select(x => x.CourseSubjectMaster.AcademicTermKey).FirstOrDefault(),
                                                                                 SubjectYear = s.CourseSubjectDetails.Where(p => p.SubjectKey == s.RowKey).Select(x => x.CourseSubjectMaster.CourseYear).FirstOrDefault(),
                                                                                 CourseDuration = s.CourseSubjectDetails.Where(p => p.SubjectKey == s.RowKey).Select(x => x.CourseSubjectMaster.Course.CourseDuration).FirstOrDefault(),
                                                                                 SubjectKey = s.RowKey,
                                                                                 SubjectCode = s.SubjectCode,
                                                                                 SubjectName = s.SubjectName,
                                                                                 IsElective = s.IsElective == true ? EduSuiteUIResources.Yes : EduSuiteUIResources.No,
                                                                                 HasStudyMaterial = s.HasStudyMaterial == true ? EduSuiteUIResources.Yes : EduSuiteUIResources.No,
                                                                                 IsCommonSubject = s.IsCommonSubject == true ? EduSuiteUIResources.Yes : EduSuiteUIResources.No,
                                                                                 //StudyMaterialCount = s.StudyMaterialCount,
                                                                                 StudyMaterialCount = (Int16)s.StudyMaterials.Count(),
                                                                                 CourseSubjectMasterKey = s.CourseSubjectDetails.Where(p => p.SubjectKey == s.RowKey).Select(x => x.CourseSubjectMasterKey).FirstOrDefault(),

                                                                             });

                if (model.CourseKey != 0)
                {
                    SubjectList = SubjectList.Where(row => row.CourseKey == model.CourseKey);
                }
                if (model.UniversityMasterKey != 0)
                {
                    SubjectList = SubjectList.Where(row => row.UniversityMasterKey == model.UniversityMasterKey);
                }
                if (model.AcademicTermKey != 0)
                {
                    SubjectList = SubjectList.Where(row => row.AcademicTermKey == model.AcademicTermKey);
                }
                if (model.SubjectYear != 0)
                {
                    SubjectList = SubjectList.Where(row => row.SubjectYear == model.SubjectYear);
                }
                SubjectList = SubjectList.GroupBy(x => x.SubjectKey).Select(y => y.FirstOrDefault()).OrderBy(row => row.CourseSubjectMasterKey);

                //SubjectList = SubjectList.GroupBy(x => x.SubjectKey).Select(y => y.FirstOrDefault());
                if (model.SortBy != "")
                {
                    //SubjectList = SortApplications(SubjectList, model.SortBy, model.SortOrder);
                }





                TotalRecords = SubjectList.Count();
                return SubjectList.Skip(Skip).Take(Take).ToList<SyllabusAndStudyMaterialViewModel>();


            }
            catch (Exception ex)
            {
                TotalRecords = 0;
                ActivityLog.CreateActivityLog(MenuConstants.SyllabusAndStudyMaterial, ActionConstants.View, DbConstants.LogType.Error, null, ex.GetBaseException().Message);
                return new List<SyllabusAndStudyMaterialViewModel>();

            }
        }


        private IQueryable<SyllabusAndStudyMaterialViewModel> SortApplications(IQueryable<SyllabusAndStudyMaterialViewModel> Query, string SortName, string SortOrder)
        {

            string command = SortOrder == "desc" ? "OrderByDescending" : "OrderBy";
            var type = typeof(SyllabusAndStudyMaterialViewModel);
            var property = type.GetProperty(SortName);
            var parameter = Expression.Parameter(type, "p");
            var propertyAccess = Expression.MakeMemberAccess(parameter, property);
            var orderByExpression = Expression.Lambda(propertyAccess, parameter);
            var resultExpression = Expression.Call(typeof(Queryable), command, new Type[] { type, property.PropertyType },
                                          Query.Expression, Expression.Quote(orderByExpression));
            return Query.Provider.CreateQuery<SyllabusAndStudyMaterialViewModel>(resultExpression);

        }

        public void FillDropDown(SyllabusAndStudyMaterialViewModel model)
        {
            FillAcedemicTerm(model);
            FillCourse(model);
            FillUniversity(model);
            FillCourseYear(model);
        }

        //private void FillUniversityCourse(SyllabusAndStudyMaterialViewModel model)
        //{

        //    var SubjectList = (from s in dbContext.Subjects
        //                       orderby s.RowKey
        //                       select new SyllabusAndStudyMaterialViewModel
        //                       {
        //                           Course = s.CourseSubjectDetails.Where(p => p.SubjectKey == s.RowKey).Select(x => x.CourseSubjectMaster.Course.CourseName).FirstOrDefault(),
        //                           University = s.CourseSubjectDetails.Where(p => p.SubjectKey == s.RowKey).Select(x => x.CourseSubjectMaster.UniversityMaster.UniversityMasterName).FirstOrDefault(),
        //                           AcademicTermName = s.CourseSubjectDetails.Where(p => p.SubjectKey == s.RowKey).Select(x => x.CourseSubjectMaster.AcademicTerm.AcademicTermName).FirstOrDefault(),
        //                           SubjectYear = s.CourseSubjectDetails.Where(p => p.SubjectKey == s.RowKey).Select(x => x.CourseSubjectMaster.CourseYear).FirstOrDefault(),
        //                           AcademicTermKey = s.CourseSubjectDetails.Where(p => p.SubjectKey == s.RowKey).Select(x => x.CourseSubjectMaster.AcademicTermKey).FirstOrDefault(),
        //                           CourseDuration = s.CourseSubjectDetails.Where(p => p.SubjectKey == s.RowKey).Select(x => x.CourseSubjectMaster.Course.CourseDuration).FirstOrDefault(),

        //                           CourseSubjectMasterKey = s.CourseSubjectDetails.Where(p => p.SubjectKey == s.RowKey).Select(x => x.CourseSubjectMasterKey).FirstOrDefault(),

        //                       }).ToList();


        //    SubjectList = SubjectList.GroupBy(x => new { x.CourseSubjectMasterKey }).Select(y => y.First()).ToList<SyllabusAndStudyMaterialViewModel>();

        //    foreach (SyllabusAndStudyMaterialViewModel objmodel in SubjectList)
        //    {
        //        objmodel.SubjectYearText = CommonUtilities.GetYearDescriptionByCodeDetails(objmodel.CourseDuration ?? 0, objmodel.SubjectYear ?? 0, objmodel.AcademicTermKey ?? 0);
        //        objmodel.UniversityCourse = objmodel.Course + " - " + objmodel.University;

        //    }
        //    model.UniversityCourses = SubjectList.Select(x => new SelectListModel
        //    {
        //        RowKey = x.CourseSubjectMasterKey ?? 0,
        //        Text = x.UniversityCourse
        //    }).Distinct().ToList();
        //}

        //public SyllabusAndStudyMaterialViewModel FillCourseYear(SyllabusAndStudyMaterialViewModel model)
        //{
        //    if (model.CourseSubjectMasterKey != 0 && model.CourseSubjectMasterKey != null)
        //    {


        //        var SubjectList = (from s in dbContext.Subjects
        //                           orderby s.RowKey
        //                           select new SyllabusAndStudyMaterialViewModel
        //                           {
        //                               Course = s.CourseSubjectDetails.Where(p => p.SubjectKey == s.RowKey).Select(x => x.CourseSubjectMaster.Course.CourseName).FirstOrDefault(),
        //                               University = s.CourseSubjectDetails.Where(p => p.SubjectKey == s.RowKey).Select(x => x.CourseSubjectMaster.UniversityMaster.UniversityMasterName).FirstOrDefault(),
        //                               AcademicTermName = s.CourseSubjectDetails.Where(p => p.SubjectKey == s.RowKey).Select(x => x.CourseSubjectMaster.AcademicTerm.AcademicTermName).FirstOrDefault(),
        //                               SubjectYear = s.CourseSubjectDetails.Where(p => p.SubjectKey == s.RowKey).Select(x => x.CourseSubjectMaster.CourseYear).FirstOrDefault(),
        //                               AcademicTermKey = s.CourseSubjectDetails.Where(p => p.SubjectKey == s.RowKey).Select(x => x.CourseSubjectMaster.AcademicTermKey).FirstOrDefault(),
        //                               CourseDuration = s.CourseSubjectDetails.Where(p => p.SubjectKey == s.RowKey).Select(x => x.CourseSubjectMaster.Course.CourseDuration).FirstOrDefault(),
        //                               CourseSubjectMasterKey = s.CourseSubjectDetails.Where(p => p.SubjectKey == s.RowKey).Select(x => x.CourseSubjectMasterKey).FirstOrDefault(),

        //                           }).ToList();
        //        SubjectList = SubjectList.GroupBy(x => new { x.CourseSubjectMasterKey }).Select(y => y.First()).ToList<SyllabusAndStudyMaterialViewModel>();

        //        foreach (SyllabusAndStudyMaterialViewModel objmodel in SubjectList.Where(x => x.CourseSubjectMasterKey == model.CourseSubjectMasterKey))
        //        {
        //            objmodel.SubjectYearText = CommonUtilities.GetYearDescriptionByCodeDetails(objmodel.CourseDuration ?? 0, objmodel.SubjectYear ?? 0, objmodel.AcademicTermKey ?? 0);
        //            objmodel.UniversityCourse = objmodel.Course + " - " + objmodel.University;

        //        }
        //        model.CourseYear = SubjectList.Select(x => new SelectListModel
        //        {
        //            RowKey = x.SubjectYear ?? 0,
        //            Text = x.SubjectYearText
        //        }).Distinct().ToList();
        //    }
        //    return model;

        //}

        public void FillCourse(SyllabusAndStudyMaterialViewModel model)
        {
            model.Courses = dbContext.VwCourseSelectActiveOnlies.Select(x => new SelectListModel
            {
                RowKey = x.RowKey,
                Text = x.CourseName
            }).ToList();

        }
        public SyllabusAndStudyMaterialViewModel FillUniversity(SyllabusAndStudyMaterialViewModel model)
        {
            model.Universities = dbContext.VwUniversitySelectActiveOnlies.Where(x => x.CourseKey == model.CourseKey && x.AcademicTermKey == model.AcademicTermKey).Select(x => new SelectListModel
            {
                RowKey = x.RowKey,
                Text = x.UniversityMasterName
            }).ToList();
            return model;
        }

        private void FillAcedemicTerm(SyllabusAndStudyMaterialViewModel model)
        {
            model.AcademicTerms = dbContext.VwAcadamicTermSelectActiveOnlies.Select(x => new SelectListModel
            {
                RowKey = x.RowKey,
                Text = x.AcademicTermName,
            }).ToList();

        }

        public SyllabusAndStudyMaterialViewModel FillCourseYear(SyllabusAndStudyMaterialViewModel model)
        {
            var CourseDuration = dbContext.Courses.Where(row => row.RowKey == model.CourseKey).Select(row => row.CourseDuration).FirstOrDefault();

            if (CourseDuration != 0 && CourseDuration != null)
            {
                var duration = Math.Ceiling((Convert.ToDecimal(model.AcademicTermKey == DbConstants.AcademicTerm.Semester ? CourseDuration / 6 : CourseDuration / 12)));

                var StartYear = 1;
                if (duration < 1)
                {
                    model.CourseYear.Add(new SelectListModel
                    {
                        RowKey = 1,
                        Text = " Short Term"
                    });
                }
                else
                {
                    for (int i = StartYear; i <= duration; i++)
                    {
                        model.CourseYear.Add(new SelectListModel
                        {
                            RowKey = i,
                            Text = i + (model.AcademicTermKey == DbConstants.AcademicTerm.Semester ? " Semester" : " Year")
                        });
                    }
                }

            }
            return model;

        }

        #region Study Material
        public SyllabusAndStudyMaterialViewModel GetStudyMaterialById(long SubjectKey)
        {
            SyllabusAndStudyMaterialViewModel model = new SyllabusAndStudyMaterialViewModel();

            model.StudyMaterials = dbContext.StudyMaterials.Where(x => x.SubjectKey == SubjectKey).Select(row => new StudyMaterialModel
            {
                RowKey = row.RowKey,
                StudyMaterialCode = row.StudyMaterialCode,
                StudyMaterialName = row.StudyMaterialName,
                IsActive = row.IsActive
            }).ToList();

            if (model.StudyMaterials.Count == 0)
            {
                model.StudyMaterials.Add(new StudyMaterialModel());
            }
            if (model == null)
            {
                model = new SyllabusAndStudyMaterialViewModel();
            }
            model.SubjectKey = SubjectKey;
            return model;
        }

        public SyllabusAndStudyMaterialViewModel UpdateStudyMaterials(SyllabusAndStudyMaterialViewModel model)
        {
            using (var transaction = dbContext.Database.BeginTransaction())
            {
                try
                {
                    CreateStudyMaterialDetails(model.StudyMaterials.Where(x => x.RowKey == 0).ToList(), model);
                    UpdateStudyMaterialDetails(model.StudyMaterials.Where(x => x.RowKey != 0).ToList(), model);

                    Subject subject = dbContext.Subjects.SingleOrDefault(x => x.RowKey == model.SubjectKey);
                    if (subject != null)
                    {
                        int count = model.StudyMaterials.Count(x => x.IsActive == true);
                        subject.StudyMaterialCount = Convert.ToInt16(count);
                    }
                    dbContext.SaveChanges();
                    transaction.Commit();
                    model.Message = EduSuiteUIResources.Success;
                    model.IsSuccessful = true;

                    ActivityLog.CreateActivityLog(MenuConstants.SyllabusAndStudyMaterial, (model.StudyMaterials.Any(row => row.RowKey != 0) ? ActionConstants.Edit : ActionConstants.Add), DbConstants.LogType.Info, model.SubjectKey, model.Message);

                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    model.Message = String.Format(EduSuiteUIResources.FailedToSaveErrorMessage, EduSuiteUIResources.StudyMaterial);
                    model.IsSuccessful = false;

                    ActivityLog.CreateActivityLog(MenuConstants.SyllabusAndStudyMaterial, (model.StudyMaterials.Any(row => row.RowKey != 0) ? ActionConstants.Edit : ActionConstants.Add), DbConstants.LogType.Error, model.SubjectKey, ex.GetBaseException().Message);
                }
                return model;
            }
        }

        private void CreateStudyMaterialDetails(List<StudyMaterialModel> ModelList, SyllabusAndStudyMaterialViewModel objViewModel)
        {
            long MaxKey = dbContext.StudyMaterials.Select(p => p.RowKey).DefaultIfEmpty().Max();

            foreach (StudyMaterialModel model in ModelList)
            {
                StudyMaterial studyMaterialModel = new StudyMaterial();

                //studyMaterialModel.RowKey = ++MaxKey;
                studyMaterialModel.SubjectKey = objViewModel.SubjectKey ?? 0;
                studyMaterialModel.RowKey = MaxKey + 1;
                studyMaterialModel.StudyMaterialCode = model.StudyMaterialCode;
                studyMaterialModel.StudyMaterialName = model.StudyMaterialName;
                studyMaterialModel.IsActive = model.IsActive;
                dbContext.StudyMaterials.Add(studyMaterialModel);

            }
        }

        private void UpdateStudyMaterialDetails(List<StudyMaterialModel> ModelList, SyllabusAndStudyMaterialViewModel objViewModel)
        {

            foreach (StudyMaterialModel model in ModelList)
            {
                StudyMaterial studyMaterialModel = new StudyMaterial();

                studyMaterialModel = dbContext.StudyMaterials.SingleOrDefault(row => row.RowKey == model.RowKey);
                studyMaterialModel.StudyMaterialCode = model.StudyMaterialCode;
                studyMaterialModel.StudyMaterialName = model.StudyMaterialName;
                studyMaterialModel.IsActive = model.IsActive;

            }

        }

        public SyllabusAndStudyMaterialViewModel CheckStudyMaterialNameExist(StudyMaterialModel model)
        {
            SyllabusAndStudyMaterialViewModel syllabusAndStudyMaterialViewModel = new SyllabusAndStudyMaterialViewModel();
            syllabusAndStudyMaterialViewModel.IsSuccessful = !dbContext.StudyMaterials.Where(x => x.StudyMaterialName.ToLower() == model.StudyMaterialName.ToLower() && x.RowKey != model.RowKey).Any();

            return syllabusAndStudyMaterialViewModel;
        }
        public SyllabusAndStudyMaterialViewModel CheckStudyMaterialCodeExist(StudyMaterialModel model)
        {
            SyllabusAndStudyMaterialViewModel syllabusAndStudyMaterialViewModel = new SyllabusAndStudyMaterialViewModel();
            syllabusAndStudyMaterialViewModel.IsSuccessful = !dbContext.StudyMaterials.Where(x => x.StudyMaterialCode.ToLower() == model.StudyMaterialCode.ToLower() && x.RowKey != model.RowKey).Any();

            return syllabusAndStudyMaterialViewModel;
        }

        public SyllabusAndStudyMaterialViewModel DeleteStudyMaterial(long Id)
        {
            SyllabusAndStudyMaterialViewModel model = new SyllabusAndStudyMaterialViewModel();
            using (var transaction = dbContext.Database.BeginTransaction())
            {
                try
                {
                    StudyMaterial studyMaterialsModel = dbContext.StudyMaterials.SingleOrDefault(x => x.RowKey == Id);

                    List<StudyMaterial> studyMaterialsModellist = dbContext.StudyMaterials.Where(x => x.SubjectKey == studyMaterialsModel.SubjectKey).ToList();

                    if (studyMaterialsModellist.Count <= 1)
                    {
                        Subject subjectModel = dbContext.Subjects.SingleOrDefault(x => x.RowKey == studyMaterialsModel.SubjectKey);
                        subjectModel.HasStudyMaterial = false;
                    }
                    dbContext.StudyMaterials.Remove(studyMaterialsModel);

                    dbContext.SaveChanges();
                    transaction.Commit();
                    model.Message = EduSuiteUIResources.Success;
                    model.IsSuccessful = true;
                    ActivityLog.CreateActivityLog(MenuConstants.SyllabusAndStudyMaterial, ActionConstants.Delete, DbConstants.LogType.Info, Id, model.Message);
                }
                catch (DbUpdateException ex)
                {
                    transaction.Rollback();
                    if (ex.GetBaseException().Message.ToUpper().Contains(EduSuiteUIResources.ForeignKeyError.ToUpper()))
                    {
                        model.Message = String.Format(EduSuiteUIResources.CantDeleteErrorMessage, EduSuiteUIResources.StudyMaterial);
                        model.IsSuccessful = false;
                        ActivityLog.CreateActivityLog(MenuConstants.SyllabusAndStudyMaterial, ActionConstants.Delete, DbConstants.LogType.Debug, Id, ex.GetBaseException().Message);

                    }
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    model.Message = String.Format(EduSuiteUIResources.FailedToDeleteErrorMessage, EduSuiteUIResources.StudyMaterial);
                    model.IsSuccessful = false;
                    ActivityLog.CreateActivityLog(MenuConstants.CourseSubject, ActionConstants.Delete, DbConstants.LogType.Debug, Id, ex.GetBaseException().Message);
                }
            }
            return model;
        }

        #endregion Study Material


        #region Subject Modules
        public SyllabusAndStudyMaterialViewModel GetSubjectModulesById(long SubjectKey)
        {
            SyllabusAndStudyMaterialViewModel model = new SyllabusAndStudyMaterialViewModel();

            model.SubjectModulesModel = dbContext.SubjectModules.Where(x => x.SubjectKey == SubjectKey).Select(row => new SubjectModulesModel
            {
                RowKey = row.RowKey,
                ModuleName = row.ModuleName,
                SubjectKey = row.SubjectKey,

                IsActive = row.IsActive,
                Duration = row.Duration,
                HasTopics = row.HasTopics,
                ModulesTopicModel = row.ModuleTopics.Select(y => new ModulesTopicModel
                {
                    RowKey = y.RowKey,
                    TopicName = y.TopicName,
                    SubjectModuleKey = y.SubjectModuleKey ?? 0,
                }).ToList()
            }).ToList();

            if (model.SubjectModulesModel.Count == 0)
            {
                model.SubjectModulesModel.Add(new SubjectModulesModel());
            }
            if (model == null)
            {
                model = new SyllabusAndStudyMaterialViewModel();
            }

            model.SubjectKey = SubjectKey;
            Subject subject = dbContext.Subjects.FirstOrDefault(x => x.RowKey == SubjectKey);
            if (subject != null)
            {
                model.SubjectName = subject.SubjectName;
                model.SubjectCode = subject.SubjectCode;

                model.Course = subject.CourseSubjectDetails.Where(p => p.SubjectKey == subject.RowKey).Select(x => x.CourseSubjectMaster.Course.CourseName).FirstOrDefault();
                //model.CourseKey = subject.CourseSubjectDetails.Where(p => p.SubjectKey == subject.RowKey).Select(x => x.CourseSubjectMaster.CourseKey).FirstOrDefault();
                model.University = subject.CourseSubjectDetails.Where(p => p.SubjectKey == subject.RowKey).Select(x => x.CourseSubjectMaster.UniversityMaster.UniversityMasterName).FirstOrDefault();
                // model.UniversityMasterKey = subject.CourseSubjectDetails.Where(p => p.SubjectKey == subject.RowKey).Select(x => x.CourseSubjectMaster.UniversityMasterKey).FirstOrDefault();
                //model.AcademicTermName = subject.CourseSubjectDetails.Where(p => p.SubjectKey == subject.RowKey).Select(x => x.CourseSubjectMaster.AcademicTerm.AcademicTermName).FirstOrDefault();
                model.AcademicTermKey = subject.CourseSubjectDetails.Where(p => p.SubjectKey == subject.RowKey).Select(x => x.CourseSubjectMaster.AcademicTermKey).FirstOrDefault();
                model.SubjectYear = subject.CourseSubjectDetails.Where(p => p.SubjectKey == subject.RowKey).Select(x => x.CourseSubjectMaster.CourseYear).FirstOrDefault();
                model.CourseDuration = subject.CourseSubjectDetails.Where(p => p.SubjectKey == subject.RowKey).Select(x => x.CourseSubjectMaster.Course.CourseDuration).FirstOrDefault();


                model.SubjectYearText = CommonUtilities.GetYearDescriptionByCodeDetails(model.CourseDuration ?? 0, model.SubjectYear ?? 0, model.AcademicTermKey ?? 0);
                model.UniversityCourse = model.Course + " - " + model.University;
            }
            return model;
        }


        public SyllabusAndStudyMaterialViewModel UpdateSubjectModules(SyllabusAndStudyMaterialViewModel model)
        {
            using (var transaction = dbContext.Database.BeginTransaction())
            {
                try
                {
                    CreateSubjectModulesDetails(model.SubjectModulesModel.Where(x => x.RowKey == 0).ToList(), model);
                    UpdateSubjectModulesDetails(model.SubjectModulesModel.Where(x => x.RowKey != 0).ToList(), model);
                    List<ModulesTopicModel> ModuleTopicList = new List<ModulesTopicModel>();
                    foreach (SubjectModulesModel item in model.SubjectModulesModel)
                    {
                        ModuleTopicList.AddRange(item.ModulesTopicModel);
                    }
                    CreateModuleTopic(ModuleTopicList.Where(row => row.RowKey == 0).ToList());
                    UpdateModuleTopic(ModuleTopicList.Where(row => row.RowKey != 0).ToList());

                    dbContext.SaveChanges();
                    transaction.Commit();
                    model.Message = EduSuiteUIResources.Success;
                    model.IsSuccessful = true;

                    ActivityLog.CreateActivityLog(MenuConstants.SyllabusAndStudyMaterial, (model.StudyMaterials.Any(row => row.RowKey != 0) ? ActionConstants.Edit : ActionConstants.Add), DbConstants.LogType.Info, model.SubjectKey, model.Message);

                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    model.Message = String.Format(EduSuiteUIResources.FailedToSaveErrorMessage, EduSuiteUIResources.Subject + EduSuiteUIResources.BlankSpace + EduSuiteUIResources.Module);
                    model.IsSuccessful = false;

                    ActivityLog.CreateActivityLog(MenuConstants.SyllabusAndStudyMaterial, (model.StudyMaterials.Any(row => row.RowKey != 0) ? ActionConstants.Edit : ActionConstants.Add), DbConstants.LogType.Error, model.SubjectKey, ex.GetBaseException().Message);
                }
                return model;
            }
        }

        private void CreateSubjectModulesDetails(List<SubjectModulesModel> ModelList, SyllabusAndStudyMaterialViewModel objViewModel)
        {
            long MaxKey = dbContext.SubjectModules.Select(p => p.RowKey).DefaultIfEmpty().Max();

            foreach (SubjectModulesModel model in ModelList)
            {
                SubjectModule SubjectModulelModel = new SubjectModule();

                //studyMaterialModel.RowKey = ++MaxKey;
                SubjectModulelModel.SubjectKey = objViewModel.SubjectKey ?? 0;
                SubjectModulelModel.RowKey = ++MaxKey;
                SubjectModulelModel.ModuleName = model.ModuleName;
                SubjectModulelModel.Duration = model.Duration;
                SubjectModulelModel.HasTopics = model.HasTopics;
                SubjectModulelModel.IsActive = model.IsActive;
                dbContext.SubjectModules.Add(SubjectModulelModel);
                model.ModulesTopicModel.ForEach(row => row.SubjectModuleKey = SubjectModulelModel.RowKey);
            }

        }

        private void UpdateSubjectModulesDetails(List<SubjectModulesModel> ModelList, SyllabusAndStudyMaterialViewModel objViewModel)
        {

            foreach (SubjectModulesModel model in ModelList)
            {
                SubjectModule SubjectModulelModel = new SubjectModule();

                SubjectModulelModel = dbContext.SubjectModules.SingleOrDefault(row => row.RowKey == model.RowKey);
                SubjectModulelModel.ModuleName = model.ModuleName;
                SubjectModulelModel.Duration = model.Duration;
                SubjectModulelModel.HasTopics = model.HasTopics;
                SubjectModulelModel.IsActive = model.IsActive;
                model.ModulesTopicModel.ForEach(row => row.SubjectModuleKey = SubjectModulelModel.RowKey);
            }


        }

        //public SyllabusAndStudyMaterialViewModel CheckStudyMaterialNameExist(StudyMaterialModel model)
        //{
        //    SyllabusAndStudyMaterialViewModel syllabusAndStudyMaterialViewModel = new SyllabusAndStudyMaterialViewModel();
        //    syllabusAndStudyMaterialViewModel.IsSuccessful = !dbContext.StudyMaterials.Where(x => x.StudyMaterialName.ToLower() == model.StudyMaterialName.ToLower() && x.RowKey != model.RowKey).Any();

        //    return syllabusAndStudyMaterialViewModel;
        //}
        //public SyllabusAndStudyMaterialViewModel CheckStudyMaterialCodeExist(StudyMaterialModel model)
        //{
        //    SyllabusAndStudyMaterialViewModel syllabusAndStudyMaterialViewModel = new SyllabusAndStudyMaterialViewModel();
        //    syllabusAndStudyMaterialViewModel.IsSuccessful = !dbContext.StudyMaterials.Where(x => x.StudyMaterialCode.ToLower() == model.StudyMaterialCode.ToLower() && x.RowKey != model.RowKey).Any();

        //    return syllabusAndStudyMaterialViewModel;
        //}

        public SyllabusAndStudyMaterialViewModel DeleteSubjectModule(long Id)
        {
            SyllabusAndStudyMaterialViewModel model = new SyllabusAndStudyMaterialViewModel();
            using (var transaction = dbContext.Database.BeginTransaction())
            {
                try
                {
                    SubjectModule subjectModulesModel = dbContext.SubjectModules.SingleOrDefault(x => x.RowKey == Id);

                    List<ModuleTopic> moduleTopiclist = dbContext.ModuleTopics.Where(x => x.SubjectModuleKey == Id).ToList();

                    if (moduleTopiclist.Count >= 1)
                    {
                        dbContext.ModuleTopics.RemoveRange(moduleTopiclist);

                    }
                    dbContext.SubjectModules.Remove(subjectModulesModel);

                    dbContext.SaveChanges();
                    transaction.Commit();
                    model.Message = EduSuiteUIResources.Success;
                    model.IsSuccessful = true;
                    ActivityLog.CreateActivityLog(MenuConstants.SyllabusAndStudyMaterial, ActionConstants.Delete, DbConstants.LogType.Info, Id, model.Message);
                }
                catch (DbUpdateException ex)
                {
                    transaction.Rollback();
                    if (ex.GetBaseException().Message.ToUpper().Contains(EduSuiteUIResources.ForeignKeyError.ToUpper()))
                    {
                        model.Message = String.Format(EduSuiteUIResources.CantDeleteErrorMessage, EduSuiteUIResources.Subject + EduSuiteUIResources.BlankSpace + EduSuiteUIResources.Module);
                        model.IsSuccessful = false;
                        ActivityLog.CreateActivityLog(MenuConstants.SyllabusAndStudyMaterial, ActionConstants.Delete, DbConstants.LogType.Debug, Id, ex.GetBaseException().Message);

                    }
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    model.Message = String.Format(EduSuiteUIResources.FailedToDeleteErrorMessage, EduSuiteUIResources.Subject + EduSuiteUIResources.BlankSpace + EduSuiteUIResources.Module);
                    model.IsSuccessful = false;
                    ActivityLog.CreateActivityLog(MenuConstants.SyllabusAndStudyMaterial, ActionConstants.Delete, DbConstants.LogType.Debug, Id, ex.GetBaseException().Message);
                }
            }
            return model;
        }


        private void CreateModuleTopic(List<ModulesTopicModel> modelList)
        {
            long MaxKey = dbContext.ModuleTopics.Select(p => p.RowKey).DefaultIfEmpty().Max();

            foreach (ModulesTopicModel model in modelList)
            {
                ModuleTopic ModuleTopicModel = new ModuleTopic();

                ModuleTopicModel.RowKey = ++MaxKey;
                ModuleTopicModel.SubjectModuleKey = model.SubjectModuleKey;
                ModuleTopicModel.TopicName = model.TopicName;
                dbContext.ModuleTopics.Add(ModuleTopicModel);

            }

        }
        private void UpdateModuleTopic(List<ModulesTopicModel> modelList)
        {

            foreach (ModulesTopicModel model in modelList)
            {
                ModuleTopic ModuleTopicModel = new ModuleTopic();

                ModuleTopicModel = dbContext.ModuleTopics.SingleOrDefault(x => x.RowKey == model.RowKey);
                ModuleTopicModel.SubjectModuleKey = model.SubjectModuleKey;
                ModuleTopicModel.TopicName = model.TopicName;

            }
        }

        public SyllabusAndStudyMaterialViewModel DeleteModuleTopic(long Id)
        {
            SyllabusAndStudyMaterialViewModel model = new SyllabusAndStudyMaterialViewModel();
            using (var transaction = dbContext.Database.BeginTransaction())
            {
                try
                {
                    ModuleTopic moduleTopicModel = dbContext.ModuleTopics.SingleOrDefault(x => x.RowKey == Id);

                    List<SubjectModule> SubjectModuleModellist = dbContext.SubjectModules.Where(x => x.SubjectKey == moduleTopicModel.SubjectModuleKey).ToList();

                    //if (SubjectModuleModellist.Count <= 1)
                    //{
                    //    SubjectModule subjectModule = dbContext.SubjectModules.SingleOrDefault(x => x.RowKey == moduleTopicModel.SubjectModuleKey);

                    //}
                    dbContext.ModuleTopics.Remove(moduleTopicModel);

                    dbContext.SaveChanges();
                    transaction.Commit();
                    model.Message = EduSuiteUIResources.Success;
                    model.IsSuccessful = true;
                    ActivityLog.CreateActivityLog(MenuConstants.SyllabusAndStudyMaterial, ActionConstants.Delete, DbConstants.LogType.Info, Id, model.Message);
                }
                catch (DbUpdateException ex)
                {
                    transaction.Rollback();
                    if (ex.GetBaseException().Message.ToUpper().Contains(EduSuiteUIResources.ForeignKeyError.ToUpper()))
                    {
                        model.Message = String.Format(EduSuiteUIResources.CantDeleteErrorMessage, EduSuiteUIResources.Module + EduSuiteUIResources.BlankSpace + EduSuiteUIResources.Topic);
                        model.IsSuccessful = false;
                        ActivityLog.CreateActivityLog(MenuConstants.SyllabusAndStudyMaterial, ActionConstants.Delete, DbConstants.LogType.Debug, Id, ex.GetBaseException().Message);

                    }
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    model.Message = String.Format(EduSuiteUIResources.FailedToDeleteErrorMessage, EduSuiteUIResources.Module + EduSuiteUIResources.BlankSpace + EduSuiteUIResources.Topic);
                    model.IsSuccessful = false;
                    ActivityLog.CreateActivityLog(MenuConstants.SyllabusAndStudyMaterial, ActionConstants.Delete, DbConstants.LogType.Debug, Id, ex.GetBaseException().Message);
                }
            }
            return model;
        }


        public SyllabusAndStudyMaterialViewModel DeleteModuleTopicAll(long Id)
        {
            SyllabusAndStudyMaterialViewModel model = new SyllabusAndStudyMaterialViewModel();
            using (var transaction = dbContext.Database.BeginTransaction())
            {
                try
                {


                    List<ModuleTopic> ModuleTopicllist = dbContext.ModuleTopics.Where(x => x.SubjectModuleKey == Id).ToList();
                    SubjectModule subjectModulesModel = dbContext.SubjectModules.SingleOrDefault(x => x.RowKey == Id);
                    subjectModulesModel.HasTopics = false;
                    dbContext.ModuleTopics.RemoveRange(ModuleTopicllist);

                    dbContext.SaveChanges();
                    transaction.Commit();
                    model.Message = EduSuiteUIResources.Success;
                    model.IsSuccessful = true;
                    ActivityLog.CreateActivityLog(MenuConstants.SyllabusAndStudyMaterial, ActionConstants.Delete, DbConstants.LogType.Info, Id, model.Message);
                }
                catch (DbUpdateException ex)
                {
                    transaction.Rollback();
                    if (ex.GetBaseException().Message.ToUpper().Contains(EduSuiteUIResources.ForeignKeyError.ToUpper()))
                    {
                        model.Message = String.Format(EduSuiteUIResources.CantDeleteErrorMessage, EduSuiteUIResources.Module + EduSuiteUIResources.BlankSpace + EduSuiteUIResources.Topic);
                        model.IsSuccessful = false;
                        ActivityLog.CreateActivityLog(MenuConstants.SyllabusAndStudyMaterial, ActionConstants.Delete, DbConstants.LogType.Debug, Id, ex.GetBaseException().Message);

                    }
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    model.Message = String.Format(EduSuiteUIResources.FailedToDeleteErrorMessage, EduSuiteUIResources.Module + EduSuiteUIResources.BlankSpace + EduSuiteUIResources.Topic);
                    model.IsSuccessful = false;
                    ActivityLog.CreateActivityLog(MenuConstants.SyllabusAndStudyMaterial, ActionConstants.Delete, DbConstants.LogType.Debug, Id, ex.GetBaseException().Message);
                }
            }
            return model;
        }

        #endregion Subject Modules

    }
}
