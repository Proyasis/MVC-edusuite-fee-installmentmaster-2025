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


                model = dbContext.ExamScheduleMasters.Where(row => row.RowKey == model.RowKey).Select(row => new ExamScheduleViewModel
                {
                    RowKey = row.RowKey,
                    BranchKey = row.BranchKey,
                    AcademicTermKey = row.AcademicTermKey,
                    BatchKey = row.BatchKey,
                    UniversityMasterKey = row.UniversityMasterKey,
                    CourseYear = row.ExamYear,
                    ExamTermKey = row.ExamTermKey,
                    CourseKey = row.CourseKey,
                    CourseTypeKey = row.Course.CourseTypeKey,
                    SubjectKey = row.SubjectKey,
                    ExamDate = row.ExamDate,
                    MinimumMark = row.MinimumMark,
                    MaximumMark = row.MaximumMark,

                }).SingleOrDefault();
                if (model == null)
                {
                    model = new ExamScheduleViewModel();

                }



                FillDropDown(model);
                //model.IsSuccessful = true;


                return model;
            }
            catch (Exception Ex)
            {
                return new ExamScheduleViewModel();
                ActivityLog.CreateActivityLog(MenuConstants.ExamSchedule, ActionConstants.View, DbConstants.LogType.Error, model.RowKey, Ex.GetBaseException().Message);

            }
        }

        public void FillExamDetailsViewModel(ExamScheduleViewModel model)
        {
            //var CheckQuery = dbContext.ExamScheduleMasters.Where(x => x.BranchKey == model.BranchKey && x.BatchKey == model.BatchKey && x.UniversityMasterKey == model.UniversityMasterKey &&
            //          x.CourseKey == model.CourseKey && x.ExamYear == model.CourseYear && x.ExamTermKey == model.ExamTermKey && x.SubjectKey == model.SubjectKey).Select(row => row.RowKey);
            //if (CheckQuery.Any())
            //{
            //    model.RowKey = CheckQuery.FirstOrDefault();
            //}
            var CheckQuery = dbContext.ExamScheduleMasters.SingleOrDefault(x => x.BranchKey == model.BranchKey && x.BatchKey == model.BatchKey && x.UniversityMasterKey == model.UniversityMasterKey &&
                      x.CourseKey == model.CourseKey && x.ExamYear == model.CourseYear && x.ExamTermKey == model.ExamTermKey && x.SubjectKey == model.SubjectKey);
            if (CheckQuery != null)
            {
                model.RowKey = CheckQuery.RowKey;
                model.MinimumMark = CheckQuery.MinimumMark;
                model.MaximumMark = CheckQuery.MaximumMark;
                model.ExamStartTime = CheckQuery.ExamStartTime;
                model.ExamEndTime = CheckQuery.ExamEndTime;
                model.ExamDate = CheckQuery.ExamDate;


            }
            else
            {
                model.RowKey = 0;
                model.MinimumMark = null;
                model.MaximumMark = null;
                model.ExamStartTime = null;
                model.ExamEndTime = null;
            }
            model.ExamScheduleDetailsModel = (from a in dbContext.Applications.Where(x => x.UniversityMasterKey == model.UniversityMasterKey &&
                                                x.CourseKey == model.CourseKey && x.AcademicTermKey == model.AcademicTermKey && x.BatchKey == model.BatchKey &&
                                                x.BranchKey == model.BranchKey && x.CurrentYear == model.CourseYear && x.StudentStatusKey == DbConstants.StudentStatus.Ongoing)
                                              join es in dbContext.ExamScheduleDetails.Where(x => x.ExamScheduleMasterKey == model.RowKey)
                                              on a.RowKey equals es.ApplicationKey into esa
                                              from es in esa.DefaultIfEmpty()
                                              select new ExamScheduleDetailsModel
                                              {
                                                  RowKey = es.RowKey != null ? es.RowKey : 0,
                                                  ExamScheduleMasterKey = es.ExamScheduleMasterKey != null ? es.ExamScheduleMasterKey : 0,
                                                  ApplicationKey = a.RowKey,
                                                  StudentName = a.StudentName,
                                                  AppearenceCount = es.AppearenceCount,
                                                  ExamCenterKey = es.ExamCenterKey != null ? es.ExamCenterKey : 0,
                                                  ExamRegisterNumber = a.ExamRegisterNo,
                                                  IsActive = es.IsActive != null ? es.IsActive : false,
                                                  ExamCenter = dbContext.ExamCenters.Where(x => x.IsActive == true).Select(row => new SelectListModel
                                                  {
                                                      RowKey = row.RowKey,
                                                      Text = row.ExamCentreName
                                                  }).ToList(),
                                              }).ToList();
            if (model.ExamScheduleDetailsModel.Count == 0)
            {
                model.ExamScheduleDetailsModel = (from a in dbContext.Applications
                                                  where (a.UniversityMasterKey == model.UniversityMasterKey && a.CourseKey == model.CourseKey &&
                                                  a.AcademicTermKey == model.AcademicTermKey && a.BatchKey == model.BatchKey && a.BranchKey == model.BranchKey
                                                  && a.CurrentYear == model.CourseYear && a.StudentStatusKey == DbConstants.StudentStatus.Ongoing)
                                                  select new ExamScheduleDetailsModel
                                                  {
                                                      //RowKey = 0,
                                                      //ExamScheduleMasterKey = 0,
                                                      AppearenceCount = 0,
                                                      ApplicationKey = a.RowKey,
                                                      StudentName = a.StudentName,
                                                      ExamRegisterNumber = a.ExamRegisterNo,
                                                      //ExamCenterKey = 0,
                                                      //IsActive = false,
                                                      ExamCenter = dbContext.ExamCenters.Where(x => x.IsActive == true).Select(row => new SelectListModel
                                                      {
                                                          RowKey = row.RowKey,
                                                          Text = row.ExamCentreName
                                                      }).ToList(),
                                                  }).ToList();
            }


        }

        public ExamScheduleViewModel CreateExamSchedule(ExamScheduleViewModel model)
        {
            using (var transaction = dbContext.Database.BeginTransaction())
            {
                try
                {


                    long maxKey = dbContext.ExamScheduleMasters.Select(p => p.RowKey).DefaultIfEmpty().Max();

                    ExamScheduleMaster ExamScheduleMasterModel = new ExamScheduleMaster();
                    ExamScheduleMasterModel.RowKey = Convert.ToInt64(maxKey + 1);
                    ExamScheduleMasterModel.UniversityMasterKey = model.UniversityMasterKey;
                    ExamScheduleMasterModel.BatchKey = model.BatchKey;
                    ExamScheduleMasterModel.ExamYear = model.CourseYear;
                    ExamScheduleMasterModel.ExamTermKey = model.ExamTermKey;
                    ExamScheduleMasterModel.BranchKey = model.BranchKey;
                    ExamScheduleMasterModel.CourseKey = model.CourseKey;
                    ExamScheduleMasterModel.AcademicTermKey = model.AcademicTermKey;
                    ExamScheduleMasterModel.SubjectKey = model.SubjectKey;
                    ExamScheduleMasterModel.ExamDate = model.ExamDate;
                    ExamScheduleMasterModel.MaximumMark = model.MaximumMark ?? 0;
                    ExamScheduleMasterModel.MinimumMark = model.MinimumMark ?? 0;
                    ExamScheduleMasterModel.ExamStartTime = model.ExamStartTime;
                    ExamScheduleMasterModel.ExamEndTime = model.ExamEndTime;
                    dbContext.ExamScheduleMasters.Add(ExamScheduleMasterModel);

                    CreateExamScheduleDetail(model.ExamScheduleDetailsModel.Where(row => row.RowKey == 0).ToList(), ExamScheduleMasterModel);
                    dbContext.SaveChanges();
                    transaction.Commit();
                    model.RowKey = ExamScheduleMasterModel.RowKey;
                    model.Message = ApplicationResources.Success;
                    model.IsSuccessful = true;
                    ActivityLog.CreateActivityLog(MenuConstants.ExamSchedule, ActionConstants.Add, DbConstants.LogType.Info, model.RowKey, model.Message);

                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    model.Message = ApplicationResources.FailedToSaveExamSchedule;
                    model.IsSuccessful = false;
                    ActivityLog.CreateActivityLog(MenuConstants.ExamSchedule, ActionConstants.Add, DbConstants.LogType.Error, model.RowKey, ex.GetBaseException().Message);
                }
            }
            FillDropDown(model);
            return model;

        }

        public ExamScheduleViewModel UpdateExamSchedule(ExamScheduleViewModel model)
        {
            using (var transaction = dbContext.Database.BeginTransaction())
            {
                try
                {

                    ExamScheduleMaster ExamScheduleMasterModel = new ExamScheduleMaster();
                    ExamScheduleMasterModel = dbContext.ExamScheduleMasters.SingleOrDefault(row => row.RowKey == model.RowKey);
                    ExamScheduleMasterModel.UniversityMasterKey = model.UniversityMasterKey;
                    ExamScheduleMasterModel.BatchKey = model.BatchKey;
                    ExamScheduleMasterModel.ExamYear = model.CourseYear;
                    ExamScheduleMasterModel.ExamTermKey = model.ExamTermKey;
                    ExamScheduleMasterModel.BranchKey = model.BranchKey;
                    ExamScheduleMasterModel.CourseKey = model.CourseKey;
                    ExamScheduleMasterModel.AcademicTermKey = model.AcademicTermKey;
                    ExamScheduleMasterModel.SubjectKey = model.SubjectKey;
                    ExamScheduleMasterModel.ExamDate = model.ExamDate;
                    ExamScheduleMasterModel.MaximumMark = model.MaximumMark ?? 0;
                    ExamScheduleMasterModel.MinimumMark = model.MinimumMark ?? 0;
                    ExamScheduleMasterModel.ExamStartTime = model.ExamStartTime;
                    ExamScheduleMasterModel.ExamEndTime = model.ExamEndTime;
                    CreateExamScheduleDetail(model.ExamScheduleDetailsModel.Where(row => row.RowKey == 0).ToList(), ExamScheduleMasterModel);
                    UpdateExamScheduleDetail(model.ExamScheduleDetailsModel.Where(row => row.RowKey != 0).ToList(), ExamScheduleMasterModel);

                    dbContext.SaveChanges();
                    transaction.Commit();

                    model.Message = ApplicationResources.Success;
                    model.IsSuccessful = true;
                    ActivityLog.CreateActivityLog(MenuConstants.ExamSchedule, ActionConstants.Edit, DbConstants.LogType.Info, model.RowKey, model.Message);

                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    model.Message = ApplicationResources.FailedToSaveExamSchedule;
                    model.IsSuccessful = false;
                    ActivityLog.CreateActivityLog(MenuConstants.ExamSchedule, ActionConstants.Edit, DbConstants.LogType.Error, model.RowKey, ex.GetBaseException().Message);
                }
            }
            FillDropDown(model);
            return model;
        }

        private void CreateExamScheduleDetail(List<ExamScheduleDetailsModel> modelList, ExamScheduleMaster objViewmodel)
        {
            Int64 MaxKey = dbContext.ExamScheduleDetails.Select(p => p.RowKey).DefaultIfEmpty().Max();

            foreach (ExamScheduleDetailsModel model in modelList)
            {
                ExamScheduleDetail ExamDetailModel = new ExamScheduleDetail();
                if (model.IsActive == true)
                {
                    ExamDetailModel.RowKey = Convert.ToInt64(++MaxKey);
                    ExamDetailModel.ExamScheduleMasterKey = objViewmodel.RowKey;
                    ExamDetailModel.ApplicationKey = model.ApplicationKey;
                    ExamDetailModel.ExamRegisterNumber = model.ExamRegisterNumber;
                    ExamDetailModel.IsActive = model.IsActive;
                    ExamDetailModel.ExamCenterKey = model.ExamCenterKey ?? 0;
                    ExamDetailModel.AppearenceCount = model.AppearenceCount ?? 0;

                    dbContext.ExamScheduleDetails.Add(ExamDetailModel);

                }



            }
        }

        private void UpdateExamScheduleDetail(List<ExamScheduleDetailsModel> modelList, ExamScheduleMaster objViewmodel)
        {

            foreach (ExamScheduleDetailsModel model in modelList)
            {
                ExamScheduleDetail ExamDetailModel = new ExamScheduleDetail();
                if (model.IsActive == true)
                {
                    ExamDetailModel = dbContext.ExamScheduleDetails.SingleOrDefault(x => x.RowKey == model.RowKey);
                    ExamDetailModel.ExamScheduleMasterKey = objViewmodel.RowKey;
                    ExamDetailModel.ApplicationKey = model.ApplicationKey;
                    ExamDetailModel.ExamRegisterNumber = model.ExamRegisterNumber;
                    ExamDetailModel.IsActive = model.IsActive;
                    ExamDetailModel.ExamCenterKey = model.ExamCenterKey ?? 0;
                    ExamDetailModel.AppearenceCount = model.AppearenceCount ?? 0;
                }
            }
        }
        public List<ExamScheduleViewModel> GetExamSchedule(string searchText)
        {
            try
            {
                var ExamScheduleList = dbContext.ExamScheduleMasters.Where(x => x.Course.CourseName.Contains(searchText)).ToList()
                                        .Select(row => new ExamScheduleViewModel
                                        {

                                            AcademicTermKey = row.AcademicTermKey,
                                            BranchKey = row.BranchKey,
                                            BranchName = row.Branch.BranchName,
                                            CourseKey = row.CourseKey,
                                            CourseName = row.Course.CourseName + ApplicationResources.BracketOpen + CommonUtilities.GetYearDescriptionByCodeDetails(row.Course.CourseDuration ?? 0, row.ExamYear ?? 0, row.AcademicTermKey ?? 0) + ApplicationResources.BracketClose,
                                            UniversityMasterKey = row.UniversityMasterKey,
                                            UniversityName = row.UniversityMaster.UniversityMasterName,
                                            BatchKey = row.BatchKey,
                                            BatchName = row.Batch.BatchName,
                                            CourseYear = row.ExamYear,
                                            RowKey = row.RowKey,
                                            ExamTermKey = row.ExamTermKey,
                                            ExamTermName = row.ExamTerm.ExamTermName,
                                            SubjectName = row.Subject.SubjectName,
                                            ExamDate = row.ExamDate,
                                            CourseYearName = CommonUtilities.GetYearDescriptionByCodeDetails(row.Course.CourseDuration ?? 0, row.ExamYear ?? 0, row.AcademicTermKey ?? 0),
                                            //CourseYearName = CommonUtilities.GetYearDescriptionByCode(row.ExamYear ?? 0, row.AcademicTermKey ?? 0),
                                            NoOfStudents = row.ExamScheduleDetails.Count(x => x.ExamScheduleMasterKey == row.RowKey),
                                        }).ToList();



                //var ExamScheduleList = (from IE in dbContext.ExamScheduleMasters
                //                        join IED in dbContext.ExamScheduleDetails on IE.RowKey equals IED.ExamScheduleMasterKey
                //                        where ((IE.Course.CourseName.Contains(searchText)))
                //                        select new ExamScheduleViewModel
                //                        {

                //                            AcademicTermKey = IE.AcademicTermKey,
                //                            BranchKey = IE.BranchKey,
                //                            BranchName = IE.Branch.BranchName,
                //                            CourseKey = IE.CourseKey,
                //                            CourseName = IE.Course.CourseName,
                //                            UniversityMasterKey = IE.UniversityMasterKey,
                //                            UniversityName = IE.UniversityMaster.UniversityMasterName,
                //                            BatchKey = IE.BatchKey,
                //                            BatchName = IE.Batch.BatchName,
                //                            CourseYear = IE.ExamYear,
                //                            RowKey = IE.RowKey,
                //                            ExamTermKey = IE.ExamTermKey,
                //                            ExamTermName = IE.ExamTerm.ExamTermName,
                //                            SubjectName = IE.Subject.SubjectName,
                //                            ExamDate = IE.ExamDate,
                //                            //CourseYearName = CommonUtilities.GetYearDescriptionByCodeByKhaleefa(IE.Course.CourseDuration ?? 0, IE.ExamYear ?? 0, IE.AcademicTermKey ?? 0),
                //                            CourseYearName = CommonUtilities.GetYearDescriptionByCode(IE.ExamYear ?? 0, IE.AcademicTermKey ?? 0),
                //                            NoOfStudents = IE.ExamScheduleDetails.Count(x => x.ExamScheduleMasterKey == IE.RowKey),

                //                        }).ToList();

                return ExamScheduleList.GroupBy(x => new { x.RowKey }).Select(y => y.First()).ToList<ExamScheduleViewModel>();

            }
            catch (Exception ex)
            {
                return new List<ExamScheduleViewModel>();
                ActivityLog.CreateActivityLog(MenuConstants.ExamSchedule, ActionConstants.View, DbConstants.LogType.Error, null, ex.GetBaseException().Message);

            }
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
            //FillExamCenter(model);
        }

        private void FillBranch(ExamScheduleViewModel model)
        {
            model.Branches = dbContext.vwBranchSelectActiveOnlies.Select(x => new SelectListModel
            {
                RowKey = x.RowKey,
                Text = x.BranchName
            }).ToList();
        }


        private void FillCourseType(ExamScheduleViewModel model)
        {
            model.CourseTypes = dbContext.VwCourseTypeSelectActiveOnlies
                                .Select(x => new SelectListModel
                                {
                                    RowKey = x.RowKey,
                                    Text = x.CourseTypeName
                                }).Distinct().ToList();

            //model.CourseTypes = (from VW in dbContext.VwCourseTypeSelectActiveOnlies
            //                     join C in dbContext.Courses on VW.RowKey equals C.CourseTypeKey
            //                     join UC in dbContext.UniversityCourses on C.RowKey equals UC.CourseKey
            //                     orderby VW.RowKey
            //                     where (UC.AcademicTermKey == model.AcademicTermKey)
            //                     select new SelectListModel
            //                     {
            //                         RowKey = VW.RowKey,
            //                         Text = VW.CourseTypeName
            //                     }).Distinct().ToList();

            //return model;
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
            //if (model.ClassDetailsKey != 0)
            //{
            //    ClassDetail classDetailList = dbContext.ClassDetails.SingleOrDefault(x => x.RowKey == model.ClassDetailsKey);

            //    model.CourseKey = classDetailList.UniversityCourse.CourseKey;
            //    model.UniversityMasterKey = classDetailList.UniversityCourse.UniversityMasterKey;
            //    model.CourseYear = classDetailList.StudentYear;
            //    model.DivisionKey = classDetailList.DivisionKey ?? 0;
            //    model.AcademicTermKey = classDetailList.UniversityCourse.AcademicTermKey;

            //}
            model.Batches = (from p in dbContext.Applications
                             join SDA in dbContext.StudentDivisionAllocations on p.RowKey equals SDA.ApplicationKey
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
        //private void FillExamCenter(ExamScheduleViewModel model)
        //{
        //    model.ExamCenter = dbContext.ExamCenters.Where(x => x.IsActive == true).Select(row => new SelectListModel
        //    {
        //        RowKey = row.RowKey,
        //        Text = row.ExamCentreName
        //    }).ToList();

        //}


        public ExamScheduleViewModel DeleteExamSchedule(ExamScheduleViewModel model)
        {
            using (var transaction = dbContext.Database.BeginTransaction())
            {
                try
                {
                    var ExamResult = dbContext.ExamResults.Any(x => x.ExamScheduleMasterKey == model.RowKey);
                    ExamScheduleMaster examScheduleMaster = dbContext.ExamScheduleMasters.SingleOrDefault(x => x.RowKey == model.RowKey);


                    if (ExamResult == false)
                    {

                        List<ExamScheduleDetail> ExamScheduleDetailList = dbContext.ExamScheduleDetails.Where(x => x.ExamScheduleMasterKey == model.RowKey).ToList();

                        if (ExamScheduleDetailList.Count > 0)
                        {

                            dbContext.ExamScheduleDetails.RemoveRange(ExamScheduleDetailList);
                            dbContext.ExamScheduleMasters.Remove(examScheduleMaster);
                        }
                        dbContext.SaveChanges();
                        transaction.Commit();
                        model.Message = ApplicationResources.Success;
                        model.IsSuccessful = true;
                        ActivityLog.CreateActivityLog(MenuConstants.ExamSchedule, ActionConstants.Delete, DbConstants.LogType.Info, model.RowKey, model.Message);

                    }
                    else
                    {
                        transaction.Rollback();
                        model.Message = ApplicationResources.CantDeleteExamScheduleResult;
                        model.IsSuccessful = false;
                        ActivityLog.CreateActivityLog(MenuConstants.ExamSchedule, ActionConstants.Delete, DbConstants.LogType.Error, model.RowKey, model.Message);
                    }
                }
                catch (DbUpdateException ex)
                {
                    transaction.Rollback();
                    if (ex.GetBaseException().Message.ToUpper().Contains(ApplicationResources.ForeignKeyError.ToUpper()))
                    {
                        model.Message = ApplicationResources.CantDeleteExamSchedule;
                        model.IsSuccessful = false;
                        ActivityLog.CreateActivityLog(MenuConstants.ExamSchedule, ActionConstants.Delete, DbConstants.LogType.Error, model.RowKey, ex.GetBaseException().Message);
                    }
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    model.Message = ApplicationResources.FailedToDeleteInternaleExamSchedule;
                    model.IsSuccessful = false;
                    ActivityLog.CreateActivityLog(MenuConstants.ExamSchedule, ActionConstants.Delete, DbConstants.LogType.Error, model.RowKey, ex.GetBaseException().Message);
                }
            }
            return model;
        }


        public ExamScheduleViewModel ResetExamSchedule(long Id, long ExamScheduleKey)
        {
            ExamScheduleViewModel objviewModel = new ExamScheduleViewModel();
            using (var transaction = dbContext.Database.BeginTransaction())
            {
                try
                {

                    ExamScheduleDetail examScheduleDetails = dbContext.ExamScheduleDetails.SingleOrDefault(x => x.RowKey == Id);

                    ExamResult examResults = dbContext.ExamResults.SingleOrDefault(x => x.RowKey == Id);
                    if (examResults == null)
                    {

                        dbContext.ExamScheduleDetails.Remove(examScheduleDetails);
                        var examDetails = dbContext.ExamScheduleDetails.Any(x => x.RowKey == Id);

                        if (examDetails == false)
                        {
                            ExamScheduleMaster examScheduleMaster = dbContext.ExamScheduleMasters.SingleOrDefault(x => x.RowKey == ExamScheduleKey);
                            dbContext.ExamScheduleMasters.Remove(examScheduleMaster);
                        }


                        dbContext.SaveChanges();
                        transaction.Commit();
                        objviewModel.Message = ApplicationResources.Success;
                        objviewModel.IsSuccessful = true;
                        ActivityLog.CreateActivityLog(MenuConstants.ExamSchedule, ActionConstants.Delete, DbConstants.LogType.Info, ExamScheduleKey, objviewModel.Message);

                    }
                    else
                    {
                        transaction.Rollback();

                        objviewModel.Message = ApplicationResources.CantDeleteExamScheduleResult;
                        objviewModel.IsSuccessful = false;
                        ActivityLog.CreateActivityLog(MenuConstants.ExamSchedule, ActionConstants.Delete, DbConstants.LogType.Debug, ExamScheduleKey, objviewModel.Message);

                    }
                }
                catch (DbUpdateException ex)
                {
                    transaction.Rollback();
                    if (ex.GetBaseException().Message.ToUpper().Contains(ApplicationResources.ForeignKeyError.ToUpper()))
                    {
                        objviewModel.Message = ApplicationResources.CantDeleteExamSchedule;
                        objviewModel.IsSuccessful = false;
                        ActivityLog.CreateActivityLog(MenuConstants.ExamSchedule, ActionConstants.Delete, DbConstants.LogType.Error, ExamScheduleKey, ex.GetBaseException().Message);
                    }
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    objviewModel.Message = ApplicationResources.FailedToDeleteExamSchedule;
                    objviewModel.IsSuccessful = false;
                    ActivityLog.CreateActivityLog(MenuConstants.ExamSchedule, ActionConstants.Delete, DbConstants.LogType.Error, ExamScheduleKey, ex.GetBaseException().Message);
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










        #region Old Code

        //public List<ExamScheduleList> GetExamScheduleStudentsList(ExamScheduleViewModel model)
        //{

        //    var Take = model.PageSize;
        //    var skip = (model.PageIndex - 1) * model.PageSize;

        //    var ExamScheduleListQuery = (
        //                                 from a in dbContext.SpExamScheduleSelect(
        //                                     model.SearchExamSubjectKey,
        //                                     model.SearchCourseTypeKey,
        //                                     model.SearchCourseKey,
        //                                     model.SearchUniversityKey,
        //                                     model.SearchBatchKey,
        //                                     model.SearchExamYearKey,
        //                                    0,
        //                                    0,
        //                                    model.SearchAcademicTermKey
        //                                     )
        //                                 select new ExamScheduleList
        //                                 {
        //                                     RowKey = a.RowKey,
        //                                     AdmissionNo = a.AdmissionNo.ToUpper(),
        //                                     StudentEnrollmentNo = a.StudentEnrollmentNo,
        //                                     ApplicationKey = a.ApplicationKey,
        //                                     StudentName = a.StudentName,
        //                                     CourseName = a.CourseName,
        //                                     UniversityName = a.UniversityName,
        //                                     CourseTypeKey = a.CourseTypeKey,
        //                                     CourseKey = a.CourseKey,
        //                                     UniversityKey = a.UniversityMasterKey,
        //                                     BatchKey = a.BatchKey,
        //                                     //IsApplied = Application.IsApplied??false,
        //                                     IsApplied = a.IsApplied ?? false,
        //                                     ExamCentreKey = a.ExamCentreKey,
        //                                     ExamTermKey = a.ExamKey,
        //                                     ExamRegisterNumber = a.ExamRegisterNumber,
        //                                     AppliedDate = a.AppliedDate,
        //                                     CurrentYear = a.CurrentYear,
        //                                     Remarks = a.StudentResultRemaks,
        //                                     AppearanceNo = a.AppearanceNo,
        //                                     SubjectId = a.SubjectId,
        //                                     DataPageIndex = model.PageIndex,
        //                                     SubjectYear = a.BookYear,
        //                                     ScheduleStatus = a.ScheduleStatus ?? false,
        //                                     ExamCentre = dbContext.ExamCentres.Select(row => new SelectListModel
        //                                     {
        //                                         RowKey = row.RowKey,
        //                                         Text = row.ExamCentreName

        //                                     }).ToList(),
        //                                     ExamTerm = dbContext.ExamNames.Select(row => new SelectListModel
        //                                     {
        //                                         RowKey = row.RowKey,
        //                                         Text = row.ExamName1

        //                                     }).ToList()
        //                                 }).ToList();



        //    //if (model.SearchCourseTypeKey != 0)
        //    //    ExamScheduleListQuery = ExamScheduleListQuery.Where(x => x.CourseTypeKey == model.SearchCourseTypeKey).ToList();

        //    //if (model.SearchCourseKey != 0)
        //    //    ExamScheduleListQuery = ExamScheduleListQuery.Where(x => x.CourseKey == model.SearchCourseKey).ToList();

        //    //if (model.SearchUniversityKey != 0)
        //    //    ExamScheduleListQuery = ExamScheduleListQuery.Where(x => x.UniversityKey == model.SearchUniversityKey).ToList();

        //    //if (model.SearchBatchKey != 0)
        //    //    ExamScheduleListQuery = ExamScheduleListQuery.Where(x => x.BatchKey == model.SearchBatchKey).ToList();

        //    ////var UnionList = ExamScheduleListQuery.Where(x => (x.IsPassed==false && x.SubjectId==model.SearchExamSubjectKey)  ).ToList();


        //    //if (model.SearchExamYearKey != 0)
        //    //    ExamScheduleListQuery = ExamScheduleListQuery.Where(x => x.SubjectYear == model.SearchExamYearKey).ToList();

        //    //if (model.SearchExamSubjectKey != 0)
        //    //   ExamScheduleListQuery = ExamScheduleListQuery.Where(x => x.SubjectId == model.SearchExamSubjectKey).ToList();


        //    model.TotalRecords = ExamScheduleListQuery.Count();

        //    ExamScheduleListQuery = ExamScheduleListQuery.OrderByDescending(Row => Row.ExamRegisterNumber).Skip(skip).Take(Take).ToList();
        //    return ExamScheduleListQuery;
        //}

        ////public List<ExamScheduleList> GetExamScheduleStudentsList(ExamScheduleViewModel model)
        ////{

        ////    var ExamScheduleListQuery = (

        ////                                from Application in dbContext.T_Application.OrderByDescending(row => new { row.RowKey })

        ////                                join ExamSchedule in dbContext.T_ExamSchedule on Application.RowKey equals ExamSchedule.ApplicationKey
        ////                                into ExamScheduleTable
        ////                                from ExamSchedule in ExamScheduleTable.DefaultIfEmpty()

        ////                                join Course  in dbContext.M_Course on Application.CourseKey equals Course.RowKey 
        ////                                into CourseTable
        ////                                from Course in CourseTable.DefaultIfEmpty()


        ////                                join Books in dbContext.M_Books on Application.CourseKey equals Books.CourseKey
        ////                                into BooksTable
        ////                                from Books in BooksTable.DefaultIfEmpty()


        ////                                join UniversityCourse in dbContext.M_UniversityCourse on Course.RowKey equals UniversityCourse.CourseKey
        ////                                into UniversityCourseTable
        ////                                from UniversityCourse in UniversityCourseTable

        ////                                join UniversityMaster in dbContext.M_UniversityMaster on UniversityCourse.UniversityMasterKey equals UniversityMaster.RowKey
        ////                                into UniversityMasterTable
        ////                                from UniversityMaster in UniversityMasterTable

        ////                                select new ExamScheduleList
        ////                                {
        ////                                    RowKey = ExamSchedule.RowKey,
        ////                                    AdmissionNo = Application.AdmissionNo.ToUpper(),
        ////                                    StudentEnrollmentNo = Application.StudentEnrollmentNo,
        ////                                    ApplicationKey = Application.RowKey,
        ////                                    StudentName = Application.StudentName,
        ////                                    CourseName = Application.M_Course.CourseName,
        ////                                    UniversityName = Application.M_UniversityMaster.UniversityMasterName,
        ////                                    CourseTypeKey = Application.M_Course.CourseTypeKey,
        ////                                    CourseKey = Application.CourseKey,
        ////                                    UniversityKey = Application.UniversityMasterKey,
        ////                                    BatchKey = Application.BatchKey,
        ////                                    IsAppliedNull = ExamSchedule.IsApplied,
        ////                                    ExamCentreKey = ExamSchedule.ExamCentreKey,
        ////                                    ExamTermKey = ExamSchedule.ExamKey,
        ////                                    CurrentYear = Application.CurrentYear,
        ////                                    Remarks = ExamSchedule.StudentResultRemaks,
        ////                                    AppearanceNo = ExamSchedule.AppearanceNo,
        ////                                    SubjectId=ExamSchedule.SubjectId,
        ////                                    ExamCentre = dbContext.M_ExamCentre.Select(row => new SelectListModel
        ////                                    {
        ////                                        RowKey = row.RowKey,
        ////                                        Text = row.ExamCentreName
        ////                                    }).ToList(),

        ////                                    ExamTerm = dbContext.M_ExamName.Select(row => new SelectListModel
        ////                                    {
        ////                                        RowKey = row.RowKey,
        ////                                        Text = row.ExamName
        ////                                    }).ToList()

        ////                                }).ToList();

        ////    if (model.SearchCourseTypeKey != 0)
        ////        ExamScheduleListQuery = ExamScheduleListQuery.Where(x => x.CourseTypeKey == model.SearchCourseTypeKey).ToList();

        ////    if (model.SearchCourseKey != 0)
        ////        ExamScheduleListQuery = ExamScheduleListQuery.Where(x => x.CourseKey == model.SearchCourseKey).ToList();

        ////    if (model.SearchUniversityKey != 0)
        ////        ExamScheduleListQuery = ExamScheduleListQuery.Where(x => x.UniversityKey == model.SearchUniversityKey).ToList();

        ////    if (model.SearchBatchKey != 0)
        ////        ExamScheduleListQuery = ExamScheduleListQuery.Where(x => x.BatchKey == model.SearchBatchKey).ToList();

        ////    //var UnionList = ExamScheduleListQuery.Where(x => (x.IsPassed==false && x.SubjectId==model.SearchExamSubjectKey)  ).ToList();

        ////    if (model.SearchExamYearKey != 0)
        ////        ExamScheduleListQuery = ExamScheduleListQuery.Where(x => x.CurrentYear == model.SearchExamYearKey).ToList();

        ////    if (model.SearchExamSubjectKey != 0)
        ////        ExamScheduleListQuery = ExamScheduleListQuery.Where(x => x.SubjectId == model.SearchExamSubjectKey).ToList();

        ////    model.TotalRecords = ExamScheduleListQuery.Count();
        ////    return ExamScheduleListQuery.ToList();
        ////}
        //public ExamScheduleViewModel CreateExamSchedule(ExamScheduleViewModel model)
        //{
        //    using (var transaction = dbContext.Database.BeginTransaction())
        //    {
        //        try
        //        {
        //            model.IsSuccessful = CreateExamScheduleList(model);
        //            if (model.IsSuccessful == false)
        //            {
        //                transaction.Rollback();
        //            }
        //            else
        //            {
        //                UpdateExamScheduleList(model);
        //                dbContext.SaveChanges();
        //                transaction.Commit();
        //                model.IsSuccessful = true;
        //                model.Message = ApplicationResources.Success;

        //            }


        //            FillDrodownLists(model);
        //            ActivityLog.CreateActivityLog(MenuConstants.ExamSchedule, (model.ExamScheduleList.Any(row => row.RowKey != 0) ? ActionConstants.Edit : ActionConstants.Add), DbConstants.LogType.Info, model.RowKey, model.Message);
        //        }
        //        catch (Exception ex)
        //        {
        //            transaction.Rollback();
        //            model.Message = ApplicationResources.FailedToSaveExamSchedule;
        //            model.IsSuccessful = false;
        //            ActivityLog.CreateActivityLog(MenuConstants.ExamSchedule, (model.ExamScheduleList.Any(row => row.RowKey != 0) ? ActionConstants.Edit : ActionConstants.Add), DbConstants.LogType.Error, model.RowKey, ex.GetBaseException().Message);
        //        }
        //    }
        //    return model;
        //}
        //private bool CreateExamScheduleList(ExamScheduleViewModel model)
        //{

        //    List<ExamScheduleList> ExamScheduleList = model.ExamScheduleList.Where(x => x.RowKey == 0 && x.IsApplied == true).ToList();
        //    Int64 maxKey = dbContext.ExamSchedules.Select(p => p.RowKey).DefaultIfEmpty().Max();
        //    model.PageIndex = model.ExamScheduleList.Select(x => x.DataPageIndex).DefaultIfEmpty().Max();
        //    foreach (ExamScheduleList List in ExamScheduleList)
        //    {
        //        int CheckExist = dbContext.ExamSchedules.Where(x => x.ApplicationKey == List.ApplicationKey && x.SubjectId == List.SubjectId).Count();

        //        if (CheckExist > 0)
        //        {

        //            model.Message = ApplicationResources.FailedToSaveExamScheduleExists;
        //            return false;
        //        }
        //        else
        //        {
        //            ExamSchedule ExamSchedule = new ExamSchedule();
        //            maxKey = maxKey + 1;
        //            ExamSchedule.RowKey = Convert.ToInt64(maxKey);
        //            ExamSchedule.ApplicationKey = List.ApplicationKey;
        //            ExamSchedule.AppearanceNo = List.AppearanceNo ?? 0;
        //            ExamSchedule.ExamCentreKey = List.ExamCentreKey ?? 0;
        //            ExamSchedule.ExamKey = List.ExamTermKey ?? 0;
        //            ExamSchedule.IsApplied = List.IsApplied;
        //            ExamSchedule.AppliedDate = Convert.ToDateTime(List.AppliedDate);
        //            ExamSchedule.SubjectId = List.SubjectId ?? 0;
        //            ExamSchedule.SubjectYear = List.SubjectYear;
        //            ExamSchedule.SubjectMark = List.SubjectMark;
        //            ExamSchedule.StudentResultRemaks = List.Remarks;
        //            ExamSchedule.ExamRegisterNumber = List.ExamRegisterNumber;
        //            ExamSchedule.ScheduleStatus = true;
        //            dbContext.ExamSchedules.Add(ExamSchedule);
        //        }


        //    }
        //    return true;
        //}
        //private bool UpdateExamScheduleList(ExamScheduleViewModel model)
        //{
        //    List<ExamScheduleList> ExamScheduleList = model.ExamScheduleList.Where(x => x.RowKey > 0 && x.ScheduleStatus == false).ToList();
        //    Int64 maxKey = dbContext.ExamSchedules.Select(p => p.RowKey).DefaultIfEmpty().Max();
        //    model.PageIndex = model.ExamScheduleList.Select(x => x.DataPageIndex).DefaultIfEmpty().Max();
        //    foreach (ExamScheduleList List in ExamScheduleList)
        //    {
        //        ExamSchedule ExamSchedule = dbContext.ExamSchedules.Where(x => x.RowKey == List.RowKey).SingleOrDefault();
        //        ExamSchedule.AppearanceNo = List.AppearanceNo ?? 0;
        //        ExamSchedule.ExamCentreKey = List.ExamCentreKey ?? 0;
        //        ExamSchedule.ExamKey = List.ExamTermKey ?? 0;
        //        ExamSchedule.AppliedDate = Convert.ToDateTime(List.AppliedDate);
        //        ExamSchedule.SubjectId = List.SubjectId ?? 0;
        //        ExamSchedule.SubjectYear = List.SubjectYear;
        //        ExamSchedule.SubjectMark = List.SubjectMark;
        //        ExamSchedule.StudentResultRemaks = List.Remarks;
        //        ExamSchedule.IsApplied = List.IsApplied;
        //        ExamSchedule.ScheduleStatus = List.IsApplied;
        //        ExamSchedule.ExamRegisterNumber = List.ExamRegisterNumber;
        //    }
        //    return true;
        //}
        ///*DROPDOWN BINDINGS*/
        //public void FillDrodownLists(ExamScheduleViewModel model)
        //{
        //    GetSyllabus(model);
        //    GetCourseTypeBySyllabus(model);
        //    GetCourseByCourseType(model);
        //    GetUniversityByCourse(model);
        //    GetBatches(model);
        //    GetExamCentre(model);
        //    GetExamTerm(model);
        //    GetExamSubjects(model);
        //    GetYears(model);
        //}
        //public ExamScheduleViewModel GetCourseTypeBySyllabus(ExamScheduleViewModel model)
        //{
        //    short SearchAcademicTermKey = model.SearchAcademicTermKey;
        //    model.CourseTypes = dbContext.UniversityCourses.Where(x => x.AcademicTermKey == SearchAcademicTermKey).Select(row => new SelectListModel
        //    {
        //        RowKey = row.Course.CourseType.RowKey,
        //        Text = row.Course.CourseType.CourseTypeName

        //    }).GroupBy(row => row.RowKey).Select(row => row.FirstOrDefault()).ToList();


        //    return model;
        //}
        //public ExamScheduleViewModel GetCourseByCourseType(ExamScheduleViewModel model)
        //{
        //    short SearchAcademicTermKey = model.SearchAcademicTermKey;
        //    model.Courses = dbContext.UniversityCourses.Where(row => row.Course.CourseTypeKey == model.SearchCourseTypeKey && row.AcademicTermKey == SearchAcademicTermKey).Select(row => new SelectListModel
        //    {
        //        RowKey = row.Course.RowKey,
        //        Text = row.Course.CourseName
        //    }).GroupBy(row => row.RowKey).Select(row => row.FirstOrDefault()).ToList();


        //    return model;
        //}
        //public ExamScheduleViewModel GetUniversityByCourse(ExamScheduleViewModel model)
        //{
        //    model.Universities = dbContext.UniversityCourses.Where(row => row.CourseKey == model.SearchCourseKey).Select(row => new SelectListModel
        //    {
        //        RowKey = row.UniversityMaster.RowKey,
        //        Text = row.UniversityMaster.UniversityMasterName
        //    }).GroupBy(row => row.RowKey).Select(row => row.FirstOrDefault()).ToList();

        //    return model;
        //}
        //public ExamScheduleViewModel GetBatches(ExamScheduleViewModel model)
        //{

        //    model.Batches = dbContext.Batches.Select(row => new SelectListModel
        //    {
        //        RowKey = row.RowKey,
        //        Text = row.BatchName
        //    }).ToList();

        //    return model;
        //}
        //public ExamScheduleViewModel GetSyllabus(ExamScheduleViewModel model)
        //{
        //    model.AcademicTerms = dbContext.AcademicTerms.Select(x => new SelectListModel
        //    {
        //        RowKey = x.RowKey,
        //        Text = x.AcademicTermName,

        //    }).ToList();

        //    return model;
        //}
        //public ExamScheduleViewModel GetYears(ExamScheduleViewModel model)
        //{
        //    short SearchSyllabusKey = model.SearchAcademicTermKey;

        //    model.CourseYears = dbContext.fnStudentYear(SearchSyllabusKey).Select(x => new SelectListModel
        //     {
        //         RowKey = x.RowKey ?? 0,
        //         Text = x.YearName
        //     }).ToList();

        //    var CourseDuration = dbContext.Courses.Where(row => row.RowKey == model.SearchCourseKey).Select(row => row.CourseDuration).FirstOrDefault();
        //    if (CourseDuration != 0)
        //    {
        //        CourseDuration = Convert.ToBoolean(model.SearchAcademicTermKey) ? CourseDuration / 6 : CourseDuration / 12;

        //        if (CourseDuration < 1)
        //        {
        //            model.CourseYears = model.CourseYears.Where(row => row.RowKey == 0).ToList();

        //        }
        //        else
        //        {
        //            model.CourseYears = model.CourseYears.Where(row => row.RowKey <= CourseDuration && row.RowKey > 0).ToList();
        //        }
        //    }

        //    return model;
        //}
        //public ExamScheduleViewModel GetExamSubjects(ExamScheduleViewModel model)
        //{

        //    model.ExamSubjects = dbContext.Books.Where(row => row.CourseKey == model.SearchCourseKey
        //     && row.UniversityKey == model.SearchUniversityKey
        //     && row.BookYear == model.SearchExamYearKey).Select(x => new SelectListModel
        //    {
        //        RowKey = x.RowKey,
        //        Text = x.BookName,
        //    }).ToList();

        //    return model;
        //}
        //private void GetExamCentre(ExamScheduleViewModel model)
        //{
        //    model.ExamCentre = dbContext.ExamCentres.Select(row => new SelectListModel
        //    {
        //        RowKey = row.RowKey,
        //        Text = row.ExamCentreName
        //    }).ToList();
        //}
        //private void GetExamTerm(ExamScheduleViewModel model)
        //{
        //    model.ExamTerm = dbContext.ExamNames.Select(row => new SelectListModel
        //    {
        //        RowKey = row.RowKey,
        //        Text = row.ExamName1
        //    }).ToList();
        //}
        //public ExamScheduleViewModel DeleteExamSchedule(ExamScheduleViewModel model)
        //{
        //    using (var transaction = dbContext.Database.BeginTransaction())
        //    {
        //        try
        //        {
        //            ExamSchedule examSchedule = dbContext.ExamSchedules.SingleOrDefault(row => row.RowKey == model.RowKey);
        //            dbContext.ExamSchedules.Remove(examSchedule);
        //            dbContext.SaveChanges();
        //            transaction.Commit();
        //            model.Message = ApplicationResources.Success;
        //            model.IsSuccessful = true;

        //            ActivityLog.CreateActivityLog(MenuConstants.ExamSchedule, ActionConstants.Delete, DbConstants.LogType.Info, model.RowKey, model.Message);
        //        }
        //        catch (DbUpdateException ex)
        //        {
        //            transaction.Rollback();
        //            if (ex.GetBaseException().Message.ToUpper().Contains(ApplicationResources.ForeignKeyError.ToUpper()))
        //            {
        //                model.Message = ApplicationResources.CantDeleteExamSchedule;
        //                model.IsSuccessful = false;
        //                ActivityLog.CreateActivityLog(MenuConstants.ExamSchedule, ActionConstants.Delete, DbConstants.LogType.Error, model.RowKey, ex.GetBaseException().Message);

        //            }
        //        }
        //        catch (Exception ex)
        //        {
        //            transaction.Rollback();
        //            model.Message = ApplicationResources.FailedToDeleteExamSchedule;
        //            model.IsSuccessful = false;
        //            ActivityLog.CreateActivityLog(MenuConstants.ExamSchedule, ActionConstants.Delete, DbConstants.LogType.Error, model.RowKey, ex.GetBaseException().Message);
        //        }
        //    }
        //    //}
        //    return model;
        //}
        #endregion Old Code



    }
}
