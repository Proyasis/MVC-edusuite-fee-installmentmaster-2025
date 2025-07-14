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
    public class InternalExamService : IInternalExamScheduleService
    {

        private EduSuiteDatabase dbContext;

        public InternalExamService(EduSuiteDatabase Objdb)
        {
            this.dbContext = Objdb;

        }

        public InternalExamViewModel GetInternalExamScheduleById(InternalExamViewModel model)
        {
            try
            {
                InternalExamViewModel objviewModel = new InternalExamViewModel();
                objviewModel = dbContext.InternalExams.Where(row => row.RowKey == model.RowKey).Select(row => new InternalExamViewModel
                {
                    RowKey = row.RowKey,
                    BranchKey = row.BranchKey,
                    AcademicTermKey = row.AcademicTermKey,
                    BatchKey = row.BatchKey,
                    UniversityMasterKey = row.UniversityMasterKey,
                    CourseYear = row.ExamYear,
                    InternalExamTermKey = row.InternalExamTermKey,
                    CourseKey = row.CourseKey,
                    CourseTypeKey = row.Course.CourseTypeKey,
                    ClassDetailsKeys = row.InternalExamDivisions.Select(x => x.ClassDetailsKey).ToList()
                }).SingleOrDefault();
                if (objviewModel == null)
                {
                    objviewModel = new InternalExamViewModel();
                }
                FillDropDown(objviewModel);
                FillNotificationDetail(objviewModel);

                return objviewModel;
            }
            catch (Exception Ex)
            {
                ActivityLog.CreateActivityLog(MenuConstants.InternalExam, ActionConstants.View, DbConstants.LogType.Error, model.RowKey, Ex.GetBaseException().Message);
                return new InternalExamViewModel();
            }
        }

        private void FillEditInternalExamDetailsViewModel(InternalExamViewModel model)
        {
            model.RowKey = model.InternalExamDetails.Select(x => x.InternalExamKey).FirstOrDefault();
        }

        public void FillInternalExamDetailsViewModel(InternalExamViewModel model)
        {

            if (model.RowKey != 0)
            {
                model.InternalExamDetails = dbContext.InternalExamDetails.Where(x => x.InternalExamKey == model.RowKey).Select(x => new InternalExamDetailsModel
                {
                    RowKey = x.RowKey,
                    InternalExamKey = x.InternalExamKey,
                    SubjectKey = x.SubjectKey,
                    SubjectName = x.Subject.SubjectName,
                    ExamDate = x.ExamDate,
                    MaximumMark = x.MaximumMark,
                    MinimumMark = x.MinimumMark,
                    ExamStatus = x.ExamStatus,
                    ExamStartTime = x.ExamStartTime,
                    ExamEndTime = x.ExamEndTime
                }).ToList();
                List<long> subjectkeys = model.InternalExamDetails.Select(x => x.SubjectKey).ToList();

                model.InternalExamDetails.AddRange(from B in dbContext.VwSubjectSelectActiveOnlies
                                                   where (B.UniversityMasterKey == model.UniversityMasterKey && B.CourseKey == model.CourseKey &&
                                                   B.AcademicTermKey == model.AcademicTermKey && B.CourseYear == model.CourseYear && !subjectkeys.Contains(B.RowKey))
                                                   select new InternalExamDetailsModel
                                                   {
                                                       RowKey = 0,
                                                       InternalExamKey = 0,
                                                       SubjectKey = B.RowKey,
                                                       SubjectName = B.SubjectName,
                                                       ExamDate = null,
                                                       MaximumMark = null,
                                                       MinimumMark = null,
                                                       ExamStatus = false,
                                                       ExamStartTime = null,
                                                       ExamEndTime = null
                                                   });
            }
            else
            {
                var CheckQuery = dbContext.InternalExams.Where(x => x.BranchKey == model.BranchKey && x.BatchKey == model.BatchKey && x.UniversityMasterKey == model.UniversityMasterKey && x.CourseKey == model.CourseKey &&
                      x.ExamYear == model.CourseYear && x.AcademicTermKey == model.AcademicTermKey && x.InternalExamTermKey == model.InternalExamTermKey && x.InternalExamDivisions.Any(y => model.ClassDetailsKeys.Contains(y.ClassDetailsKey))).Select(row => row.RowKey);
                if (CheckQuery.Any())
                {
                    model.RowKey = CheckQuery.FirstOrDefault();
                }
                else
                {
                    model.RowKey = 0;
                }
                model.InternalExamDetails = dbContext.InternalExamDetails.Where(x => x.InternalExamKey == model.RowKey).Select(x => new InternalExamDetailsModel
                {
                    RowKey = x.RowKey,
                    InternalExamKey = x.InternalExamKey,
                    SubjectKey = x.SubjectKey,
                    SubjectName = x.Subject.SubjectName,
                    ExamDate = x.ExamDate,
                    MaximumMark = x.MaximumMark,
                    MinimumMark = x.MinimumMark,
                    ExamStatus = x.ExamStatus,
                    ExamStartTime = x.ExamStartTime,
                    ExamEndTime = x.ExamEndTime
                }).ToList();
                List<long> subjectkeys = model.InternalExamDetails.Select(x => x.SubjectKey).ToList();

                model.InternalExamDetails.AddRange(from B in dbContext.VwSubjectSelectActiveOnlies
                                                   where (B.UniversityMasterKey == model.UniversityMasterKey && B.CourseKey == model.CourseKey &&
                                                   B.AcademicTermKey == model.AcademicTermKey && B.CourseYear == model.CourseYear && !subjectkeys.Contains(B.RowKey))
                                                   select new InternalExamDetailsModel
                                                   {
                                                       RowKey = 0,
                                                       InternalExamKey = 0,
                                                       SubjectKey = B.RowKey,
                                                       SubjectName = B.SubjectName,
                                                       ExamDate = null,
                                                       MaximumMark = null,
                                                       MinimumMark = null,
                                                       ExamStatus = false,
                                                       ExamStartTime = null,
                                                       ExamEndTime = null
                                                   });
                //model.InternalExamDetails = (from B in dbContext.VwSubjectSelectActiveOnlies
                //                             where (B.UniversityMasterKey == model.UniversityMasterKey && B.CourseKey == model.CourseKey &&
                //                             B.AcademicTermKey == model.AcademicTermKey && B.CourseYear == model.CourseYear)
                //                             select new InternalExamDetailsModel
                //                             {

                //                                 SubjectKey = B.RowKey,
                //                                 SubjectName = B.SubjectName,

                //                             }).ToList();
            }
        }

        public InternalExamViewModel CreateInternalExamSchedule(InternalExamViewModel model)
        {
            using (var transaction = dbContext.Database.BeginTransaction())
            {
                try
                {


                    long maxKey = dbContext.InternalExams.Select(p => p.RowKey).DefaultIfEmpty().Max();

                    InternalExam internalExamModel = new InternalExam();
                    internalExamModel.RowKey = Convert.ToInt64(maxKey + 1);
                    internalExamModel.UniversityMasterKey = model.UniversityMasterKey;
                    internalExamModel.BatchKey = model.BatchKey;
                    internalExamModel.ExamYear = model.CourseYear;
                    internalExamModel.InternalExamTermKey = model.InternalExamTermKey;
                    internalExamModel.BranchKey = model.BranchKey ?? 0;
                    internalExamModel.CourseKey = model.CourseKey;
                    internalExamModel.AcademicTermKey = model.AcademicTermKey;
                    dbContext.InternalExams.Add(internalExamModel);

                    CreateInternalExamDetail(model.InternalExamDetails.Where(row => row.RowKey == 0).ToList(), internalExamModel);
                    CreateInternalExamDivision(model, internalExamModel);
                    dbContext.SaveChanges();
                    transaction.Commit();
                    model.RowKey = internalExamModel.RowKey;
                    model.Message = EduSuiteUIResources.Success;
                    model.IsSuccessful = true;
                    ActivityLog.CreateActivityLog(MenuConstants.InternalExam, ActionConstants.Add, DbConstants.LogType.Info, model.RowKey, model.Message);

                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    model.Message = String.Format(EduSuiteUIResources.FailedToSaveErrorMessage, EduSuiteUIResources.InternalExamSchedule);
                    model.IsSuccessful = false;
                    ActivityLog.CreateActivityLog(MenuConstants.InternalExam, ActionConstants.Add, DbConstants.LogType.Error, model.RowKey, ex.GetBaseException().Message);
                }
            }
            FillDropDown(model);

            return model;

        }

        public InternalExamViewModel UpdateInternalExamSchedule(InternalExamViewModel model)
        {
            using (var transaction = dbContext.Database.BeginTransaction())
            {
                try
                {

                    InternalExam internalExamModel = new InternalExam();
                    internalExamModel = dbContext.InternalExams.SingleOrDefault(row => row.RowKey == model.RowKey);
                    internalExamModel.UniversityMasterKey = model.UniversityMasterKey;
                    internalExamModel.BatchKey = model.BatchKey;
                    internalExamModel.ExamYear = model.CourseYear;
                    internalExamModel.InternalExamTermKey = model.InternalExamTermKey;
                    internalExamModel.BranchKey = model.BranchKey ?? 0;
                    internalExamModel.CourseKey = model.CourseKey;
                    internalExamModel.AcademicTermKey = model.AcademicTermKey;

                    CreateInternalExamDetail(model.InternalExamDetails.Where(row => row.RowKey == 0).ToList(), internalExamModel);
                    UpdateInternalExamDetail(model.InternalExamDetails.Where(row => row.RowKey != 0).ToList(), internalExamModel);
                    CreateInternalExamDivision(model, internalExamModel);

                    dbContext.SaveChanges();
                    transaction.Commit();

                    model.Message = EduSuiteUIResources.Success;
                    model.IsSuccessful = true;
                    ActivityLog.CreateActivityLog(MenuConstants.InternalExam, ActionConstants.Edit, DbConstants.LogType.Info, model.RowKey, model.Message);

                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    model.Message = String.Format(EduSuiteUIResources.FailedToSaveErrorMessage, EduSuiteUIResources.InternalExamSchedule);
                    model.IsSuccessful = false;
                    ActivityLog.CreateActivityLog(MenuConstants.InternalExam, ActionConstants.Edit, DbConstants.LogType.Error, model.RowKey, ex.GetBaseException().Message);
                }
            }
            FillDropDown(model);
            GetStudentMailDetails(model);
            return model;
        }

        private void CreateInternalExamDetail(List<InternalExamDetailsModel> modelList, InternalExam objViewmodel)
        {
            Int64 MaxKey = dbContext.InternalExamDetails.Select(p => p.RowKey).DefaultIfEmpty().Max();

            foreach (InternalExamDetailsModel model in modelList)
            {
                InternalExamDetail internalExamDetailModel = new InternalExamDetail();
                if (model.ExamStatus == true)
                {
                    internalExamDetailModel.RowKey = Convert.ToInt64(MaxKey + 1);
                    internalExamDetailModel.InternalExamKey = objViewmodel.RowKey;
                    internalExamDetailModel.SubjectKey = model.SubjectKey;
                    internalExamDetailModel.ExamDate = model.ExamDate;
                    internalExamDetailModel.ExamStatus = model.ExamStatus;
                    internalExamDetailModel.MaximumMark = model.MaximumMark ?? 0;
                    internalExamDetailModel.MinimumMark = model.MinimumMark ?? 0;
                    internalExamDetailModel.ExamStartTime = model.ExamStartTime;
                    internalExamDetailModel.ExamEndTime = model.ExamEndTime;

                    dbContext.InternalExamDetails.Add(internalExamDetailModel);
                    MaxKey++;
                }



            }
        }

        private void UpdateInternalExamDetail(List<InternalExamDetailsModel> modelList, InternalExam objViewmodel)
        {

            foreach (InternalExamDetailsModel model in modelList)
            {
                InternalExamDetail internalExamDetailModel = new InternalExamDetail();
                if (model.ExamStatus == true)
                {
                    internalExamDetailModel = dbContext.InternalExamDetails.SingleOrDefault(x => x.RowKey == model.RowKey);
                    internalExamDetailModel.InternalExamKey = objViewmodel.RowKey;
                    internalExamDetailModel.SubjectKey = model.SubjectKey;
                    internalExamDetailModel.ExamDate = model.ExamDate;
                    internalExamDetailModel.ExamStatus = model.ExamStatus;
                    internalExamDetailModel.MaximumMark = model.MaximumMark ?? 0;
                    internalExamDetailModel.MinimumMark = model.MinimumMark ?? 0;
                    internalExamDetailModel.ExamStartTime = model.ExamStartTime;
                    internalExamDetailModel.ExamEndTime = model.ExamEndTime;
                }
            }
        }

        private void CreateInternalExamDivision(InternalExamViewModel model, InternalExam objViewmodel)
        {

            Int64 MaxKey = dbContext.InternalExamDivisions.Select(p => p.RowKey).DefaultIfEmpty().Max();
            List<InternalExamDivision> InternaleExamDivisionList = dbContext.InternalExamDivisions.Where(x => x.InternalExamKey == objViewmodel.RowKey).ToList();
            if (InternaleExamDivisionList.Count > 0)
            {
                dbContext.InternalExamDivisions.RemoveRange(InternaleExamDivisionList);
            }
            foreach (short ClassDetailsKey in model.ClassDetailsKeys)
            {

                InternalExamDivision internalExamDivisionModel = new InternalExamDivision();
                internalExamDivisionModel.RowKey = Convert.ToInt64(MaxKey + 1);
                internalExamDivisionModel.InternalExamKey = objViewmodel.RowKey;
                internalExamDivisionModel.ClassDetailsKey = ClassDetailsKey;

                dbContext.InternalExamDivisions.Add(internalExamDivisionModel);
                MaxKey++;

            }
        }

        public List<InternalExamViewModel> GetInternalExamSchedule(InternalExamViewModel model)
        {
            try
            {
                IQueryable<InternalExamViewModel> InternalExamList = (from IE in dbContext.InternalExams
                                                                      join IED in dbContext.InternalExamDetails on IE.RowKey equals IED.InternalExamKey

                                                                      where ((IE.Course.CourseName.Contains(model.SearchText)))
                                                                      select new InternalExamViewModel
                                                                      {
                                                                          RowKey = IE.RowKey,
                                                                          AcademicTermKey = IE.AcademicTermKey,
                                                                          BranchKey = IE.BranchKey,
                                                                          BranchName = IE.Branch.BranchName,
                                                                          CourseKey = IE.CourseKey,
                                                                          CourseName = IE.Course.CourseName,
                                                                          UniversityMasterKey = IE.UniversityMasterKey,
                                                                          UniversityName = IE.UniversityMaster.UniversityMasterName,
                                                                          BatchKey = IE.BatchKey,
                                                                          BatchName = IE.Batch.BatchName,
                                                                          CourseYear = IE.ExamYear,
                                                                          CourseDuration = IE.Course.CourseDuration,
                                                                          InternalExamTermKey = IE.InternalExamTermKey,
                                                                          InternalExamTermName = IE.InternalExamTerm.InternalExamTermName,
                                                                          NoOfSubjects = IE.InternalExamDetails.Count(x => x.InternalExamKey == IE.RowKey),
                                                                          ClassDetailsNames = IE.InternalExamDivisions.Select(x => x.ClassDetail.ClassCode).ToList(),
                                                                      });

                Employee Employee = dbContext.Employees.Where(x => x.AppUserKey == DbConstants.User.UserKey).SingleOrDefault();

                if (Employee != null)
                {
                    if (Employee.BranchAccess != null)
                    {
                        var Branches = Employee.BranchAccess.Split(',').Select(Int16.Parse).ToList();
                        InternalExamList = InternalExamList.Where(row => Branches.Contains(row.BranchKey ?? 0));
                    }
                }
                if (model.BranchKey != 0)
                {
                    InternalExamList = InternalExamList.Where(row => row.BranchKey == model.BranchKey);
                }
                if (model.BatchKey != 0)
                {
                    InternalExamList = InternalExamList.Where(row => row.BatchKey == model.BatchKey);
                }

                return InternalExamList.GroupBy(x => new { x.RowKey }).Select(y => y.FirstOrDefault()).ToList<InternalExamViewModel>();
            }
            catch (Exception ex)
            {
                ActivityLog.CreateActivityLog(MenuConstants.InternalExam, ActionConstants.View, DbConstants.LogType.Error, null, ex.GetBaseException().Message);
                return new List<InternalExamViewModel>();
            }
        }

        public void FillDropDown(InternalExamViewModel model)
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

            FillInternalExamTerm(model);
            FillClassDetails(model);
        }

        private void FillBranch(InternalExamViewModel model)
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
        private void FillCourseType(InternalExamViewModel model)
        {
            model.CourseTypes = dbContext.VwCourseTypeSelectActiveOnlies
                                .Select(x => new SelectListModel
                                {
                                    RowKey = x.RowKey,
                                    Text = x.CourseTypeName
                                }).Distinct().ToList();
        }
        public InternalExamViewModel FillCourse(InternalExamViewModel model)
        {
            model.Courses = dbContext.VwCourseSelectActiveOnlies.Where(x => x.CourseTypeKey == model.CourseTypeKey).Select(x => new SelectListModel
            {
                RowKey = x.RowKey,
                Text = x.CourseName
            }).ToList();

            return model;
        }
        public InternalExamViewModel FillUniversity(InternalExamViewModel model)
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
        private void FillAcademicTerm(InternalExamViewModel model)
        {
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
        public InternalExamViewModel FillCourseYear(InternalExamViewModel model)
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
        public InternalExamViewModel FillBatch(InternalExamViewModel model)
        {

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
        private void FillInternalExamTerm(InternalExamViewModel model)
        {
            model.InternalExamTerm = dbContext.VwInternalExamTermSelectActiveOnlies.Select(row => new SelectListModel
            {
                RowKey = row.RowKey,
                Text = row.InternalExamTermName
            }).ToList();

        }
        public InternalExamViewModel DeleteInternalExamSchedule(InternalExamViewModel model)
        {
            using (var transaction = dbContext.Database.BeginTransaction())
            {
                try
                {
                    var internalExamResult = dbContext.InternalExamResults.Any(x => x.InternalExamKey == model.RowKey);
                    InternalExam internalExam = dbContext.InternalExams.SingleOrDefault(x => x.RowKey == model.RowKey);

                    if (internalExamResult == false)
                    {
                        List<InternalExamDetail> InternalExamDetailList = dbContext.InternalExamDetails.Where(x => x.InternalExamKey == model.RowKey).ToList();

                        if (InternalExamDetailList.Count > 0)
                        {
                            List<InternalExamDivision> internalExamDivision = dbContext.InternalExamDivisions.Where(x => x.InternalExamKey == model.RowKey).ToList();

                            dbContext.InternalExamDivisions.RemoveRange(internalExamDivision);
                            dbContext.InternalExamDetails.RemoveRange(InternalExamDetailList);
                            dbContext.InternalExams.Remove(internalExam);
                        }
                        dbContext.SaveChanges();
                        transaction.Commit();
                        model.Message = EduSuiteUIResources.Success;
                        model.IsSuccessful = true;
                        ActivityLog.CreateActivityLog(MenuConstants.InternalExam, ActionConstants.Delete, DbConstants.LogType.Info, model.RowKey, model.Message);
                    }
                    else
                    {
                        transaction.Rollback();
                        model.Message = String.Format(EduSuiteUIResources.CantDeleteErrorMessage, EduSuiteUIResources.InternalExamSchedule);
                        model.IsSuccessful = false;
                        ActivityLog.CreateActivityLog(MenuConstants.InternalExam, ActionConstants.Delete, DbConstants.LogType.Error, model.RowKey, model.Message);
                    }
                }
                catch (DbUpdateException ex)
                {
                    transaction.Rollback();
                    if (ex.GetBaseException().Message.ToUpper().Contains(EduSuiteUIResources.ForeignKeyError.ToUpper()))
                    {
                        model.Message = String.Format(EduSuiteUIResources.CantDeleteErrorMessage, EduSuiteUIResources.InternalExamSchedule);
                        model.IsSuccessful = false;
                        ActivityLog.CreateActivityLog(MenuConstants.InternalExam, ActionConstants.Delete, DbConstants.LogType.Error, model.RowKey, ex.GetBaseException().Message);
                    }
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    model.Message = String.Format(EduSuiteUIResources.FailedToDeleteErrorMessage, EduSuiteUIResources.InternalExamSchedule);
                    model.IsSuccessful = false;
                    ActivityLog.CreateActivityLog(MenuConstants.InternalExam, ActionConstants.Delete, DbConstants.LogType.Error, model.RowKey, ex.GetBaseException().Message);
                }
            }
            return model;
        }
        public InternalExamViewModel ResetInternalExamSchedule(long Id, long InternalExamKey)
        {
            InternalExamViewModel objviewModel = new InternalExamViewModel();
            using (var transaction = dbContext.Database.BeginTransaction())
            {
                try
                {

                    InternalExamDetail internalExamDetail = dbContext.InternalExamDetails.SingleOrDefault(x => x.RowKey == Id);

                    InternalExamResult internalExamResult = dbContext.InternalExamResults.SingleOrDefault(x => x.InternalExamDetailsKey == Id);

                    if (internalExamResult == null)
                    {

                        dbContext.InternalExamDetails.Remove(internalExamDetail);
                        var internalExamDetails = dbContext.InternalExamDetails.Any(x => x.InternalExamKey == InternalExamKey);

                        if (internalExamDetails == false)
                        {
                            InternalExam internalExam = dbContext.InternalExams.SingleOrDefault(x => x.RowKey == InternalExamKey);

                            List<InternalExamDivision> internalExamDivision = dbContext.InternalExamDivisions.Where(x => x.InternalExamKey == InternalExamKey).ToList();

                            dbContext.InternalExams.Remove(internalExam);
                            dbContext.InternalExamDivisions.RemoveRange(internalExamDivision);
                        }

                        dbContext.SaveChanges();
                        transaction.Commit();
                        objviewModel.Message = EduSuiteUIResources.Success;
                        objviewModel.IsSuccessful = true;
                        ActivityLog.CreateActivityLog(MenuConstants.InternalExam, ActionConstants.Delete, DbConstants.LogType.Info, InternalExamKey, objviewModel.Message);
                    }
                    else
                    {
                        transaction.Rollback();
                        objviewModel.Message = String.Format(EduSuiteUIResources.CantDeleteErrorMessage, EduSuiteUIResources.InternalExamSchedule);
                        objviewModel.IsSuccessful = false;
                        ActivityLog.CreateActivityLog(MenuConstants.InternalExam, ActionConstants.Delete, DbConstants.LogType.Debug, InternalExamKey, objviewModel.Message);
                    }
                }
                catch (DbUpdateException ex)
                {
                    transaction.Rollback();
                    if (ex.GetBaseException().Message.ToUpper().Contains(EduSuiteUIResources.ForeignKeyError.ToUpper()))
                    {
                        objviewModel.Message = String.Format(EduSuiteUIResources.CantDeleteErrorMessage, EduSuiteUIResources.InternalExamSchedule);
                        objviewModel.IsSuccessful = false;
                        ActivityLog.CreateActivityLog(MenuConstants.InternalExam, ActionConstants.Delete, DbConstants.LogType.Error, InternalExamKey, ex.GetBaseException().Message);
                    }
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    objviewModel.Message = String.Format(EduSuiteUIResources.FailedToDeleteErrorMessage, EduSuiteUIResources.InternalExamSchedule);
                    objviewModel.IsSuccessful = false;
                    ActivityLog.CreateActivityLog(MenuConstants.InternalExam, ActionConstants.Delete, DbConstants.LogType.Error, InternalExamKey, ex.GetBaseException().Message);
                }
            }

            return objviewModel;
        }
        public InternalExamViewModel FillClassDetails(InternalExamViewModel model)
        {
            model.ClassDetails = (from CD in dbContext.VwClassDetailsSelectActiveOnlies
                                  join SDA in dbContext.StudentDivisionAllocations on CD.RowKey equals SDA.ClassDetailsKey
                                  where (CD.IsActive == true && CD.AcademicTermKey == model.AcademicTermKey && CD.CourseKey == model.CourseKey
                                  && CD.UniversityMasterKey == model.UniversityMasterKey && CD.StudentYear == model.CourseYear)
                                  select new SelectListModel
                                  {
                                      RowKey = CD.RowKey,
                                      Text = CD.ClassCode + CD.ClassCodeDescription
                                  }).Distinct().ToList();
            return model;
        }
        public InternalExamViewModel GetSearchDropdownList(InternalExamViewModel model)
        {
            FillBranch(model);
            FillSearchBatch(model);
            return model;
        }
        public InternalExamViewModel FillSearchBatch(InternalExamViewModel model)
        {

            if (model.BranchKey != 0 && model.BranchKey != null)
            {
                model.Batches = (from p in dbContext.Applications
                                 join SDA in dbContext.StudentDivisionAllocations on p.RowKey equals SDA.ApplicationKey
                                 join B in dbContext.VwBatchSelectActiveOnlies on p.BatchKey equals B.RowKey
                                 orderby B.RowKey
                                 where (p.BranchKey == model.BranchKey)
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

                                 select new SelectListModel
                                 {
                                     RowKey = B.RowKey,
                                     Text = B.BatchName
                                 }).Distinct().ToList();
            }
            return model;
        }
        private void FillNotificationDetail(InternalExamViewModel model)
        {
            NotificationTemplate notificationTemplateModel = dbContext.NotificationTemplates.SingleOrDefault(row => row.RowKey == DbConstants.NotificationTemplate.InternalExamSchedule);
            if (notificationTemplateModel != null)
            {
                model.AutoEmail = notificationTemplateModel.AutoEmail;
                model.AutoSMS = notificationTemplateModel.AutoSMS;
                model.TemplateKey = notificationTemplateModel.RowKey;
            }

        }
        public void GetStudentMailDetails(InternalExamViewModel model)
        {
            model.StudetMailDetails = dbContext.Applications.Where(x => model.ClassDetailsKeys.Contains(x.ClassDetailsKey)
              && x.BatchKey == model.BatchKey && x.BranchKey == model.BranchKey && x.StudentStatusKey == DbConstants.StudentStatus.Ongoing).Select(row => new StudetMailDetails
              {
                  StudentName = row.StudentName,
                  StudentPhone = row.StudentMobile,
                  StudentEmail = row.StudentEmail,
              }).ToList();
        }
    }
}
