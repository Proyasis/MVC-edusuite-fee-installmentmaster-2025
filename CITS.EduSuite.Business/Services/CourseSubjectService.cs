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

namespace CITS.EduSuite.Business.Services
{
    public class CourseSubjectService : ICourseSubjectService
    {
        private EduSuiteDatabase dbContext;
        public CourseSubjectService(EduSuiteDatabase objEduSuiteDataBase)
        {
            this.dbContext = objEduSuiteDataBase;
        }

        public CourseSubjectMasterViewModel GetCourseSubjectById(CourseSubjectMasterViewModel model)
        {
            try
            {
                model = dbContext.CourseSubjectMasters.Where(row => row.RowKey == model.RowKey).Select(row => new CourseSubjectMasterViewModel
                {
                    RowKey = row.RowKey,

                    AcademicTermKey = row.AcademicTermKey,
                    UniversityMasterKey = row.UniversityMasterKey,
                    CourseYearKey = row.CourseYear,
                    CourseKey = row.CourseKey,
                    CourseTypeKey = row.Course.CourseTypeKey


                }).SingleOrDefault();
                if (model == null)
                {
                    model = new CourseSubjectMasterViewModel();

                }
                FillDropDown(model);
                //FillCourseSubjectDetailsViewModel(model);
                return model;
            }
            catch (Exception Ex)
            {
                ActivityLog.CreateActivityLog(MenuConstants.CourseSubject, ActionConstants.View, DbConstants.LogType.Error, null, Ex.GetBaseException().Message);
                return new CourseSubjectMasterViewModel();

            }
        }
        public void FillCourseSubjectDetailsViewModel(CourseSubjectMasterViewModel model)
        {
            //CourseSubjectMaster m_CourseSubjectMastersModel = new CourseSubjectMaster();

            var CheckQuery = dbContext.CourseSubjectMasters.Where(x => x.UniversityMasterKey == model.UniversityMasterKey && x.CourseYear == model.CourseYearKey && x.CourseKey == model.CourseKey && x.AcademicTermKey == model.AcademicTermKey).Select(row => row.RowKey);
            if (CheckQuery.Any())
            {
                model.RowKey = CheckQuery.FirstOrDefault();
            }
            else
            {
                model.RowKey = 0;
            }

            model.CourseSubjectDetailViewModel = (from row in dbContext.CourseSubjectDetails.Where(x => x.CourseSubjectMasterKey == model.RowKey)
                                                  select new CourseSubjectDetailViewModel
                                                  {
                                                      RowKey = row.RowKey,
                                                      SubjectKey = row.SubjectKey,
                                                      SubjectCode = row.Subject.SubjectCode,
                                                      SubjectName = row.Subject.SubjectName,
                                                      IsElective = row.Subject.IsElective,
                                                      HasStudyMaterial = row.Subject.HasStudyMaterial,
                                                      StudyMaterialCount = row.Subject.StudyMaterialCount ?? 0,
                                                      IsCommonSubject = row.Subject.IsCommonSubject,
                                                      IsActive = row.Subject.IsActive,
                                                      StudyMaterials = row.Subject.StudyMaterials.Select(y => new StudyMaterialModel
                                                      {
                                                          RowKey = y.RowKey,
                                                          StudyMaterialCode = y.StudyMaterialCode,
                                                          StudyMaterialName = y.StudyMaterialName,
                                                          IsActive = y.IsActive

                                                      }).ToList(),
                                                  }).ToList();
            if (model.CourseSubjectDetailViewModel.Count == 0)
            {
                model.CourseSubjectDetailViewModel.Add(new CourseSubjectDetailViewModel());
            }

        }
        public CourseSubjectMasterViewModel CreateCourseSubject(CourseSubjectMasterViewModel model)
        {
            using (var transaction = dbContext.Database.BeginTransaction())
            {
                try
                {


                    long maxKey = dbContext.CourseSubjectMasters.Select(p => p.RowKey).DefaultIfEmpty().Max();

                    CourseSubjectMaster courseSubjectMasterModel = new CourseSubjectMaster();
                    courseSubjectMasterModel.RowKey = Convert.ToInt64(maxKey + 1);
                    courseSubjectMasterModel.UniversityMasterKey = model.UniversityMasterKey;
                    courseSubjectMasterModel.CourseYear = model.CourseYearKey;
                    courseSubjectMasterModel.CourseKey = model.CourseKey;
                    courseSubjectMasterModel.AcademicTermKey = model.AcademicTermKey;
                    dbContext.CourseSubjectMasters.Add(courseSubjectMasterModel);
                    model.RowKey = courseSubjectMasterModel.RowKey;
                    CreateSubjects(model);
                    CreateStudyMaterials(model);
                    CreateCourseSubjectDetails(model);

                    dbContext.SaveChanges();
                    transaction.Commit();
                    model.RowKey = courseSubjectMasterModel.RowKey;
                    model.Message = EduSuiteUIResources.Success;
                    model.IsSuccessful = true;
                    ActivityLog.CreateActivityLog(MenuConstants.InternalExam, ActionConstants.Add, DbConstants.LogType.Info, model.RowKey, model.Message);

                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    model.Message = String.Format(EduSuiteUIResources.FailedToSaveErrorMessage, EduSuiteUIResources.CourseSubject);
                    model.IsSuccessful = false;
                    ActivityLog.CreateActivityLog(MenuConstants.InternalExam, ActionConstants.Add, DbConstants.LogType.Error, model.RowKey, ex.GetBaseException().Message);
                }
            }
            FillDropDown(model);
            return model;

        }

        public CourseSubjectMasterViewModel UpdateCourseSubject(CourseSubjectMasterViewModel model)
        {
            using (var transaction = dbContext.Database.BeginTransaction())
            {
                try
                {

                    CourseSubjectMaster courseSubjectMasterModel = new CourseSubjectMaster();
                    courseSubjectMasterModel = dbContext.CourseSubjectMasters.SingleOrDefault(row => row.RowKey == model.RowKey);
                    courseSubjectMasterModel.UniversityMasterKey = model.UniversityMasterKey;
                    courseSubjectMasterModel.CourseYear = model.CourseYearKey;
                    courseSubjectMasterModel.CourseKey = model.CourseKey;
                    courseSubjectMasterModel.AcademicTermKey = model.AcademicTermKey;

                    UpdateSubjects(model);
                    CreateSubjects(model);
                    CreateCourseSubjectDetails(model);
                    UpdateStudyMaterials(model);
                    CreateStudyMaterials(model);

                    dbContext.SaveChanges();
                    transaction.Commit();

                    model.Message = EduSuiteUIResources.Success;
                    model.IsSuccessful = true;
                    ActivityLog.CreateActivityLog(MenuConstants.InternalExam, ActionConstants.Edit, DbConstants.LogType.Info, model.RowKey, model.Message);

                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    model.Message = String.Format(EduSuiteUIResources.FailedToSaveErrorMessage, EduSuiteUIResources.CourseSubject);
                    model.IsSuccessful = false;
                    ActivityLog.CreateActivityLog(MenuConstants.InternalExam, ActionConstants.Edit, DbConstants.LogType.Error, model.RowKey, ex.GetBaseException().Message);
                }
            }
            FillDropDown(model);
            return model;
        }

        private void CreateCourseSubjectDetails(CourseSubjectMasterViewModel modelMaster)
        {
            long MaxKey = dbContext.CourseSubjectDetails.Select(p => p.RowKey).DefaultIfEmpty().Max();
            var modelList = modelMaster.CourseSubjectDetailViewModel.Where(row => row.RowKey == 0);
            foreach (CourseSubjectDetailViewModel model in modelList)
            {
                CourseSubjectDetail courseSubjectDetailModel = new CourseSubjectDetail();
                courseSubjectDetailModel.RowKey = ++MaxKey;
                courseSubjectDetailModel.CourseSubjectMasterKey = modelMaster.RowKey;
                courseSubjectDetailModel.SubjectKey = model.SubjectKey;
                courseSubjectDetailModel.IsActive = model.IsActive;

                dbContext.CourseSubjectDetails.Add(courseSubjectDetailModel);
            }
        }

        private void CreateSubjects(CourseSubjectMasterViewModel modelMaster)
        {
            long MaxKey = dbContext.Subjects.Select(p => p.RowKey).DefaultIfEmpty().Max();
            var modelList = modelMaster.CourseSubjectDetailViewModel.Where(row => row.SubjectKey == 0);

            foreach (CourseSubjectDetailViewModel model in modelList)
            {
                Subject subjectModel = new Subject();

                subjectModel.RowKey = ++MaxKey;
                subjectModel.SubjectCode = model.SubjectCode;
                subjectModel.SubjectName = model.SubjectName;
                subjectModel.IsElective = model.IsElective;
                subjectModel.HasStudyMaterial = model.HasStudyMaterial;
                if (model.HasStudyMaterial)
                {
                    subjectModel.StudyMaterialCount = 1;
                }
                else
                {
                    subjectModel.StudyMaterialCount = 0;
                }
                subjectModel.IsCommonSubject = false;
                subjectModel.IsActive = model.IsActive;
                dbContext.Subjects.Add(subjectModel);
                model.SubjectKey = subjectModel.RowKey;
            }
        }

        private void UpdateSubjects(CourseSubjectMasterViewModel modelMaster)
        {

            var modelList = modelMaster.CourseSubjectDetailViewModel.Where(row => row.SubjectKey != 0);

            foreach (CourseSubjectDetailViewModel model in modelList)
            {

                Subject subjectModel = new Subject();

                subjectModel = dbContext.Subjects.SingleOrDefault(x => x.RowKey == model.SubjectKey);
                subjectModel.SubjectCode = model.SubjectCode;
                subjectModel.SubjectName = model.SubjectName;
                subjectModel.IsElective = model.IsElective;
                subjectModel.HasStudyMaterial = model.HasStudyMaterial;
                subjectModel.StudyMaterialCount = model.StudyMaterialCount;
                subjectModel.IsCommonSubject = false;
                subjectModel.IsActive = model.IsActive;

            }
        }

        private void CreateStudyMaterials(CourseSubjectMasterViewModel modelMaster)
        {
            long MaxKey = dbContext.StudyMaterials.Select(p => p.RowKey).DefaultIfEmpty().Max();
            foreach (CourseSubjectDetailViewModel modelDetail in modelMaster.CourseSubjectDetailViewModel)
            {
                var modelList = modelDetail.StudyMaterials.Where(row => row.RowKey == 0);

                foreach (StudyMaterialModel model in modelList)
                {
                    StudyMaterial studyMaterialModel = new StudyMaterial();

                    studyMaterialModel.RowKey = ++MaxKey;
                    studyMaterialModel.SubjectKey = modelDetail.SubjectKey;
                    studyMaterialModel.RowKey = MaxKey + 1;
                    studyMaterialModel.StudyMaterialCode = model.StudyMaterialCode;
                    studyMaterialModel.StudyMaterialName = model.StudyMaterialName;
                    studyMaterialModel.IsActive = model.IsActive;
                    dbContext.StudyMaterials.Add(studyMaterialModel);

                }
            }
        }

        private void UpdateStudyMaterials(CourseSubjectMasterViewModel modelMaster)
        {

            foreach (CourseSubjectDetailViewModel modelDetail in modelMaster.CourseSubjectDetailViewModel)
            {
                var modelList = modelDetail.StudyMaterials.Where(row => row.RowKey != 0);

                foreach (StudyMaterialModel model in modelList)
                {
                    StudyMaterial studyMaterialModel = new StudyMaterial();

                    studyMaterialModel = dbContext.StudyMaterials.SingleOrDefault(row => row.RowKey == model.RowKey);
                    studyMaterialModel.StudyMaterialCode = model.StudyMaterialCode;
                    studyMaterialModel.StudyMaterialName = model.StudyMaterialName;
                    studyMaterialModel.IsActive = model.IsActive;

                }
            }
        }

        public CourseSubjectMasterViewModel DeleteCourseSubjectAll(CourseSubjectMasterViewModel model)
        {
            using (var transaction = dbContext.Database.BeginTransaction())
            {
                try
                {

                    CourseSubjectMaster courseSubjectMaster = dbContext.CourseSubjectMasters.SingleOrDefault(x => x.RowKey == model.RowKey);

                    List<CourseSubjectDetail> CourseSubjectDetailList = dbContext.CourseSubjectDetails.Where(x => x.CourseSubjectMasterKey == model.RowKey).ToList();
                    List<long> SubjectKeys = CourseSubjectDetailList.Select(x => x.SubjectKey).ToList();

                    List<Subject> SubjectsList = dbContext.Subjects.Where(x => SubjectKeys.Contains(x.RowKey)).ToList();
                    List<StudyMaterial> StudyMaterialsList = dbContext.StudyMaterials.Where(x => SubjectKeys.Contains(x.SubjectKey)).ToList();


                    dbContext.StudyMaterials.RemoveRange(StudyMaterialsList);
                    dbContext.CourseSubjectDetails.RemoveRange(CourseSubjectDetailList);
                    dbContext.Subjects.RemoveRange(SubjectsList);
                    dbContext.CourseSubjectMasters.Remove(courseSubjectMaster);

                    dbContext.SaveChanges();
                    transaction.Commit();
                    model.Message = EduSuiteUIResources.Success;
                    model.IsSuccessful = true;
                    ActivityLog.CreateActivityLog(MenuConstants.CourseSubject, ActionConstants.Delete, DbConstants.LogType.Info, null, model.Message);
                }
                catch (DbUpdateException ex)
                {
                    transaction.Rollback();
                    if (ex.GetBaseException().Message.ToUpper().Contains(EduSuiteUIResources.ForeignKeyError.ToUpper()))
                    {
                        model.Message = String.Format(EduSuiteUIResources.CantDeleteErrorMessage, EduSuiteUIResources.CourseSubject);
                        model.IsSuccessful = false;
                        ActivityLog.CreateActivityLog(MenuConstants.CourseSubject, ActionConstants.Delete, DbConstants.LogType.Debug, null, ex.GetBaseException().Message);

                    }
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    model.Message = String.Format(EduSuiteUIResources.FailedToDeleteErrorMessage, EduSuiteUIResources.CourseSubject);
                    model.IsSuccessful = false;
                    ActivityLog.CreateActivityLog(MenuConstants.CourseSubject, ActionConstants.Delete, DbConstants.LogType.Debug, null, ex.GetBaseException().Message);
                }
            }
            return model;
        }

        public CourseSubjectMasterViewModel DeleteCourseSubject(long Id)
        {
            CourseSubjectMasterViewModel model = new CourseSubjectMasterViewModel();
            using (var transaction = dbContext.Database.BeginTransaction())
            {
                try
                {
                    CourseSubjectDetail courseSubjectDetails = dbContext.CourseSubjectDetails.SingleOrDefault(x => x.RowKey == Id);

                    Subject subjects = dbContext.Subjects.SingleOrDefault(x => x.RowKey == courseSubjectDetails.SubjectKey);
                    List<StudyMaterial> studyMaterialsList = dbContext.StudyMaterials.Where(x => x.SubjectKey == courseSubjectDetails.SubjectKey).ToList();

                    List<CourseSubjectDetail> courseSubjectDetailList = dbContext.CourseSubjectDetails.Where(x => x.CourseSubjectMasterKey == courseSubjectDetails.CourseSubjectMasterKey).ToList();
                    if (courseSubjectDetailList.Count() <= 1)
                    {
                        CourseSubjectMaster CourseSubjectMasters = dbContext.CourseSubjectMasters.SingleOrDefault(p => p.RowKey == courseSubjectDetails.CourseSubjectMasterKey);
                        dbContext.StudyMaterials.RemoveRange(studyMaterialsList);
                        dbContext.Subjects.Remove(subjects);
                        dbContext.CourseSubjectDetails.Remove(courseSubjectDetails);
                        dbContext.CourseSubjectMasters.Remove(CourseSubjectMasters);

                    }
                    else
                    {
                        dbContext.StudyMaterials.RemoveRange(studyMaterialsList);
                        dbContext.Subjects.Remove(subjects);
                        dbContext.CourseSubjectDetails.Remove(courseSubjectDetails);
                    }
                    //dbContext.CourseSubjectMasters.Remove(CourseSubjectMastermodel);
                    dbContext.SaveChanges();
                    transaction.Commit();
                    model.Message = EduSuiteUIResources.Success;
                    model.IsSuccessful = true;
                    ActivityLog.CreateActivityLog(MenuConstants.CourseSubject, ActionConstants.Delete, DbConstants.LogType.Info, Id, model.Message);
                }
                catch (DbUpdateException ex)
                {
                    transaction.Rollback();
                    if (ex.GetBaseException().Message.ToUpper().Contains(EduSuiteUIResources.ForeignKeyError.ToUpper()))
                    {
                        model.Message = String.Format(EduSuiteUIResources.CantDeleteErrorMessage, EduSuiteUIResources.CourseSubject);
                        model.IsSuccessful = false;
                        ActivityLog.CreateActivityLog(MenuConstants.CourseSubject, ActionConstants.Delete, DbConstants.LogType.Debug, Id, ex.GetBaseException().Message);

                    }
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    model.Message = String.Format(EduSuiteUIResources.FailedToDeleteErrorMessage, EduSuiteUIResources.CourseSubject);
                    model.IsSuccessful = false;
                    ActivityLog.CreateActivityLog(MenuConstants.CourseSubject, ActionConstants.Delete, DbConstants.LogType.Debug, Id, ex.GetBaseException().Message);
                }
            }
            return model;
        }

        public CourseSubjectMasterViewModel DeleteStudyMaterialAll(long Id)
        {
            CourseSubjectMasterViewModel model = new CourseSubjectMasterViewModel();
            using (var transaction = dbContext.Database.BeginTransaction())
            {
                try
                {
                    List<StudyMaterial> StudyMaterialsList = dbContext.StudyMaterials.Where(x => x.SubjectKey == Id).ToList();
                    Subject subjectModel = dbContext.Subjects.SingleOrDefault(x => x.RowKey == Id);
                    subjectModel.HasStudyMaterial = false;
                    dbContext.StudyMaterials.RemoveRange(StudyMaterialsList);
                    dbContext.SaveChanges();
                    transaction.Commit();
                    model.Message = EduSuiteUIResources.Success;
                    model.IsSuccessful = true;
                    ActivityLog.CreateActivityLog(MenuConstants.CourseSubject, ActionConstants.Delete, DbConstants.LogType.Info, Id, model.Message);
                }
                catch (DbUpdateException ex)
                {
                    transaction.Rollback();
                    if (ex.GetBaseException().Message.ToUpper().Contains(EduSuiteUIResources.ForeignKeyError.ToUpper()))
                    {
                        model.Message = String.Format(EduSuiteUIResources.CantDeleteErrorMessage, EduSuiteUIResources.CourseSubject);
                        model.IsSuccessful = false;
                        ActivityLog.CreateActivityLog(MenuConstants.CourseSubject, ActionConstants.Delete, DbConstants.LogType.Debug, Id, ex.GetBaseException().Message);

                    }
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    model.Message = String.Format(EduSuiteUIResources.FailedToDeleteErrorMessage, EduSuiteUIResources.CourseSubject);
                    model.IsSuccessful = false;
                    ActivityLog.CreateActivityLog(MenuConstants.CourseSubject, ActionConstants.Delete, DbConstants.LogType.Debug, Id, ex.GetBaseException().Message);
                }
            }
            return model;
        }

        public CourseSubjectMasterViewModel DeleteStudyMaterial(long Id)
        {
            CourseSubjectMasterViewModel model = new CourseSubjectMasterViewModel();
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
                    ActivityLog.CreateActivityLog(MenuConstants.CourseSubject, ActionConstants.Delete, DbConstants.LogType.Info, Id, model.Message);
                }
                catch (DbUpdateException ex)
                {
                    transaction.Rollback();
                    if (ex.GetBaseException().Message.ToUpper().Contains(EduSuiteUIResources.ForeignKeyError.ToUpper()))
                    {
                        model.Message = String.Format(EduSuiteUIResources.CantDeleteErrorMessage, EduSuiteUIResources.CourseSubject);
                        model.IsSuccessful = false;
                        ActivityLog.CreateActivityLog(MenuConstants.CourseSubject, ActionConstants.Delete, DbConstants.LogType.Debug, Id, ex.GetBaseException().Message);

                    }
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    model.Message = String.Format(EduSuiteUIResources.FailedToDeleteErrorMessage, EduSuiteUIResources.CourseSubject);
                    model.IsSuccessful = false;
                    ActivityLog.CreateActivityLog(MenuConstants.CourseSubject, ActionConstants.Delete, DbConstants.LogType.Debug, Id, ex.GetBaseException().Message);
                }
            }
            return model;
        }

        public List<CourseSubjectMasterViewModel> GetCourseSubject(string SearchText)
        {
            try
            {
                var CourseSubjectMasterList = (from cs in dbContext.CourseSubjectMasters
                                               orderby cs.RowKey
                                               where (cs.Course.CourseName.Contains(SearchText))
                                               select new CourseSubjectMasterViewModel
                                               {
                                                   RowKey = cs.RowKey,
                                                   CourseKey = cs.CourseKey,
                                                   UniversityMasterKey = cs.UniversityMasterKey,
                                                   CourseName = cs.Course.CourseName,
                                                   AcademicTermKey = cs.AcademicTermKey,
                                                   UniversityName = cs.UniversityMaster.UniversityMasterName,
                                                   AcademicTermName = cs.AcademicTerm.AcademicTermName,
                                                   NoOfSubject = dbContext.CourseSubjectDetails.Count(x => x.CourseSubjectMaster.CourseKey == cs.CourseKey && x.CourseSubjectMaster.UniversityMasterKey == cs.UniversityMasterKey && x.CourseSubjectMaster.AcademicTermKey == cs.AcademicTermKey)
                                               }).ToList();
                return CourseSubjectMasterList.GroupBy(x => new { x.CourseKey, x.AcademicTermKey, x.UniversityMasterKey }).Select(y => y.First()).ToList<CourseSubjectMasterViewModel>();

            }
            catch (Exception ex)
            {
                ActivityLog.CreateActivityLog(MenuConstants.CourseSubject, ActionConstants.View, DbConstants.LogType.Error, null, ex.GetBaseException().Message);
                return new List<CourseSubjectMasterViewModel>();

            }
        }

        public bool CheckSubjectCodeExist(string Value, short CourseYear)
        {
            return !dbContext.VwSubjectSelectActiveOnlies.Where(x => x.SubjectCode.ToLower() == Value.ToLower() && x.CourseYear != CourseYear).Any();
        }
        public bool CheckSubjectNameExist(string Value, short CourseYear)
        {
            return !dbContext.VwSubjectSelectActiveOnlies.Where(x => x.SubjectName.ToLower() == Value.ToLower() && x.CourseYear != CourseYear).Any();
        }
        public bool CheckStudyMaterialNameExist(string Value, long SubjectKey)
        {
            return !dbContext.VwStudyMaterialSelectActiveOnlies.Where(x => x.StudyMaterialName.ToLower() == Value.ToLower() && x.SubjectKey != SubjectKey).Any();
        }
        public bool CheckStudyMaterialCodeExist(string Value, long SubjectKey)
        {
            return !dbContext.VwStudyMaterialSelectActiveOnlies.Where(x => x.StudyMaterialCode.ToLower() == Value.ToLower() && x.SubjectKey != SubjectKey).Any();
        }
        private void FillDropDown(CourseSubjectMasterViewModel model)
        {
            FillCourseType(model);
            FillAcedemicTerm(model);
            FillCourse(model);
            FillUniversity(model);
            FillCourseYear(model);
        }
        private void FillCourseType(CourseSubjectMasterViewModel model)
        {
            model.CourseTypes = dbContext.VwCourseTypeSelectActiveOnlies.Select(x => new SelectListModel
            {
                RowKey = x.RowKey,
                Text = x.CourseTypeName
            }).ToList();
        }
        public CourseSubjectMasterViewModel FillCourse(CourseSubjectMasterViewModel model)
        {
            model.Courses = dbContext.VwCourseSelectActiveOnlies.Where(x => x.CourseTypeKey == model.CourseTypeKey).Select(x => new SelectListModel
            {
                RowKey = x.RowKey,
                Text = x.CourseName
            }).ToList();
            return model;
        }
        public CourseSubjectMasterViewModel FillUniversity(CourseSubjectMasterViewModel model)
        {
            model.Universities = dbContext.VwUniversitySelectActiveOnlies.Where(x => x.CourseKey == model.CourseKey && x.AcademicTermKey == model.AcademicTermKey).Select(x => new SelectListModel
            {
                RowKey = x.RowKey,
                Text = x.UniversityMasterName
            }).ToList();
            return model;
        }
        private void FillAcedemicTerm(CourseSubjectMasterViewModel model)
        {
            //model.AcademicTerms = dbContext.AcademicTerms.Select(x => new SelectListModel
            //{
            //    RowKey = x.RowKey,
            //    Text = x.AcademicTermName,

            //}).ToList();
            model.AcademicTerms = dbContext.VwAcadamicTermSelectActiveOnlies.Select(x => new SelectListModel
            {
                RowKey = x.RowKey,
                Text = x.AcademicTermName,
            }).ToList();
            if (model.AcademicTermKey == null)
            {
                model.AcademicTermKey = dbContext.AcademicTerms.OrderByDescending(row => row.UniversityCourses.Count()).Select(row => row.RowKey).FirstOrDefault();
            }
        }

        public CourseSubjectMasterViewModel FillCourseYear(CourseSubjectMasterViewModel model)
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






    }
}
