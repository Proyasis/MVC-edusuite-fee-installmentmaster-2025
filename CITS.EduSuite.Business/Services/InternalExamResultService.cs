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
    public class InternalExamResultService : IInternalExamResultService
    {
        private EduSuiteDatabase dbcontext;

        public InternalExamResultService(EduSuiteDatabase objdb)
        {
            this.dbcontext = objdb;
        }

        public List<InternalExamResultViewModel> GetInternalExamResult(InternalExamResultViewModel model)
        {
            try
            {
                IEnumerable<InternalExamResultViewModel> InternalExamResultList = (from p in dbcontext.InternalExamResult_Select_ByType(model.SearchEmployeeKey)
                                                                                       //where (p.BatchName.Contains(searchText) || p.BranchName.Contains(searchText)
                                                                                       //|| p.CourseName.Contains(searchText) || p.InternalExamTermName.Contains(searchText) || p.UniversityMasterName.Contains(searchText))
                                                                                   select new InternalExamResultViewModel
                                                                                   {
                                                                                       AcademicTermKey = p.AcademicTermKey,
                                                                                       BranchKey = p.BranchKey,
                                                                                       BatchKey = p.BatchKey,
                                                                                       BranchName = p.BranchName,
                                                                                       ClassDetailsName = p.ClassCode,
                                                                                       UniversityName = p.UniversityMasterName,
                                                                                       CourseName = p.CourseName,
                                                                                       BatchName = p.BatchName,
                                                                                       InternalExamTermName = p.InternalExamTermName,
                                                                                       ExamYear = p.ExamYear,
                                                                                       ExamYearText = p.ExamYearText,
                                                                                       NoOfSubject = p.SubjectCount,
                                                                                       InternalExamKey = p.InternalExamKey,
                                                                                       ClassDetailsKey = p.ClassDetailsKey,
                                                                                       InternalExamTermKey = p.InternalExamTermKey
                                                                                   });

                Employee Employee = dbcontext.Employees.Where(x => x.AppUserKey == DbConstants.User.UserKey).SingleOrDefault();

                if (Employee != null)
                {
                    if (Employee.BranchAccess != null)
                    {
                        var Branches = Employee.BranchAccess.Split(',').Select(Int16.Parse).ToList();
                        InternalExamResultList = InternalExamResultList.Where(row => Branches.Contains(row.BranchKey ?? 0));
                    }
                }
                if (model.BranchKey != 0)
                {
                    InternalExamResultList = InternalExamResultList.Where(row => row.BranchKey == model.BranchKey);
                }
                if (model.ClassDetailsKey != 0)
                {
                    InternalExamResultList = InternalExamResultList.Where(row => row.ClassDetailsKey == model.ClassDetailsKey);
                }

                if (model.BatchKey != 0)
                {
                    InternalExamResultList = InternalExamResultList.Where(row => row.BatchKey == model.BatchKey);
                }
                return InternalExamResultList.GroupBy(x => new { x.ClassDetailsKey, x.BranchKey, x.BatchKey, x.InternalExamTermKey }).Select(y => y.FirstOrDefault()).ToList<InternalExamResultViewModel>();

                //return InternalExamResult.ToList<InternalExamResultViewModel>();

            }
            catch (Exception ex)
            {
                ActivityLog.CreateActivityLog(MenuConstants.InternalExamResult, ActionConstants.View, DbConstants.LogType.Error, null, ex.GetBaseException().Message);
                return new List<InternalExamResultViewModel>();

            }

        }

        public InternalExamResultViewModel GetInternalExamResultDetails(InternalExamResultViewModel model)
        {
            model.InternaleExamResultSubjectDetails = (from IE in dbcontext.InternalExams
                                                       join IED in dbcontext.InternalExamDetails on IE.RowKey equals IED.InternalExamKey
                                                       join IEDV in dbcontext.InternalExamDivisions on IE.RowKey equals IEDV.InternalExamKey
                                                       join IER in dbcontext.InternalExamResults on new { InternalExamKey = (IEDV.InternalExamKey ?? 0), IED.SubjectKey, IEDV.ClassDetailsKey }
                                                       equals new { IER.InternalExamKey, IER.SubjectKey, IER.ClassDetailsKey } into IERD
                                                       from IER in IERD.DefaultIfEmpty()
                                                       where (IEDV.ClassDetailsKey == model.ClassDetailsKey && IE.RowKey == model.InternalExamKey && IED.ExamDate <= DateTimeUTC.Now)
                                                       group IER by new { IED.Subject.SubjectName, IED.SubjectKey, IED.InternalExamKey, IED.RowKey } into g
                                                       select new InternaleExamResultSubjectDetail
                                                       {
                                                           InternalExamKey = g.Key.InternalExamKey,
                                                           SubjectKey = g.Key.SubjectKey,
                                                           SubjectName = g.Key.SubjectName,
                                                           ClassDetailsKey = model.ClassDetailsKey,
                                                           InternalExamDetailsKey = g.Key.RowKey,
                                                           Passed = g.Count(x => x.ResultStatus == DbConstants.ResultStatus.Passed),
                                                           Failed = g.Count(x => x.ResultStatus == DbConstants.ResultStatus.Fail),
                                                           Absent = g.Count(x => x.ResultStatus == DbConstants.ResultStatus.Absent),
                                                           AddedResults = g.Count(x => x.ResultStatus != null)
                                                       }).ToList();

            return model;
        }

        public InternalExamResultViewModel UpdateInternalExamResult(InternalExamResultViewModel model)
        {
            using (var transaction = dbcontext.Database.BeginTransaction())
            {
                try
                {
                    CreateInternalExamResult(model.InternalExamResultDetails.Where(row => row.RowKey == 0).ToList(), model);
                    UpdateInternalExamResult(model.InternalExamResultDetails.Where(row => row.RowKey != 0).ToList(), model);
                    dbcontext.SaveChanges();
                    transaction.Commit();
                    model.Message = EduSuiteUIResources.Success;
                    //model.AdmissionNo = dbContext.T_Application.Where(row => row.RowKey == model.ApplicationKey).Select(row => row.AdmissionNo).FirstOrDefault();
                    model.IsSuccessful = true;
                    ActivityLog.CreateActivityLog(MenuConstants.InternalExamResult, (model.InternalExamResultDetails.Any(row => row.RowKey != 0) ? ActionConstants.Edit : ActionConstants.Add), DbConstants.LogType.Info, model.EmployeeKey, model.Message);
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
                    throw raise;
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    model.Message = String.Format(EduSuiteUIResources.FailedToSaveErrorMessage, EduSuiteUIResources.InternalExamResult);
                    model.IsSuccessful = false;

                    ActivityLog.CreateActivityLog(MenuConstants.InternalExamResult, (model.InternalExamResultDetails.Any(row => row.RowKey != 0) ? ActionConstants.Edit : ActionConstants.Add), DbConstants.LogType.Error, model.EmployeeKey, ex.GetBaseException().Message);
                }
            }
            return model;
        }

        private void CreateInternalExamResult(List<InternalExamResultDetail> modelList, InternalExamResultViewModel objviewmodel)
        {
            Int64 MaxKey = dbcontext.InternalExamResults.Select(p => p.RowKey).DefaultIfEmpty().Max();

            foreach (InternalExamResultDetail model in modelList)
            {
                InternalExamResult internalExamResultModel = new InternalExamResult();
                internalExamResultModel.RowKey = Convert.ToInt64(MaxKey + 1);
                internalExamResultModel.ApplicationKey = model.ApplicationKey;
                internalExamResultModel.SubjectKey = model.SubjectKey;
                internalExamResultModel.ResultStatus = model.ResultStatus;
                internalExamResultModel.Mark = model.Mark;
                internalExamResultModel.Remarks = model.Remarks;
                internalExamResultModel.InternalExamDetailsKey = model.InternalExamDetailsKey;
                internalExamResultModel.ClassDetailsKey = objviewmodel.ClassDetailsKey ?? 0;
                internalExamResultModel.InternalExamKey = objviewmodel.InternalExamKey ?? 0;

                dbcontext.InternalExamResults.Add(internalExamResultModel);
                MaxKey++;
            }
        }

        private void UpdateInternalExamResult(List<InternalExamResultDetail> modelList, InternalExamResultViewModel objviewmodel)
        {
            foreach (InternalExamResultDetail model in modelList)
            {

                InternalExamResult internalExamResultModel = new InternalExamResult();
                internalExamResultModel = dbcontext.InternalExamResults.SingleOrDefault(row => row.RowKey == model.RowKey);
                internalExamResultModel.ApplicationKey = model.ApplicationKey;
                internalExamResultModel.SubjectKey = model.SubjectKey;
                internalExamResultModel.ResultStatus = model.ResultStatus;
                //internalExamResultModel.Mark = model.ResultStatus == DbConstants.ResultStatus.Absent ? Convert.ToDecimal(null) : model.Mark ?? 0;
                internalExamResultModel.Mark = model.Mark;
                internalExamResultModel.Remarks = model.Remarks;
                internalExamResultModel.InternalExamDetailsKey = model.InternalExamDetailsKey;
                internalExamResultModel.ClassDetailsKey = objviewmodel.ClassDetailsKey ?? 0;
                internalExamResultModel.InternalExamKey = objviewmodel.InternalExamKey ?? 0;

            }
        }
        public InternalExamResultViewModel StudentMarkDetils(InternalExamResultViewModel model)
        {
            model.InternalExamResultDetails = (from App in dbcontext.Applications
                                               join SDA in dbcontext.StudentDivisionAllocations on App.RowKey equals SDA.ApplicationKey
                                               join IE in dbcontext.InternalExams on new { App.BatchKey, App.UniversityMasterKey, App.BranchKey, App.AcademicTermKey, App.CourseKey }
                                               equals new { IE.BatchKey, IE.UniversityMasterKey, IE.BranchKey, IE.AcademicTermKey, IE.CourseKey }
                                               join IED in dbcontext.InternalExamDetails on IE.RowKey equals IED.InternalExamKey
                                               join IER in dbcontext.InternalExamResults on new { ApplicationKey = App.RowKey, InternalExamKey = IE.RowKey, IED.SubjectKey }
                                               equals new { IER.ApplicationKey, IER.InternalExamKey, IER.SubjectKey } into IERD
                                               from IER in IERD.DefaultIfEmpty()
                                               where (App.StudentStatusKey == DbConstants.StudentStatus.Ongoing && SDA.IsActive == true &&
                                               IE.RowKey == model.InternalExamKey && IED.SubjectKey == model.SubjectKey && (IER.ClassDetailsKey != null ? IER.ClassDetailsKey : App.ClassDetailsKey) == model.ClassDetailsKey)
                                               select new InternalExamResultDetail
                                               {
                                                   ApplicationKey = App.RowKey,
                                                   StudentName = App.StudentName,
                                                   AdmissionNo = App.AdmissionNo,
                                                   RowKey = IER.RowKey != null ? IER.RowKey : 0,
                                                   SubjectKey = IED.SubjectKey,
                                                   ResultStatus = IER.ResultStatus,
                                                   Mark = IER.Mark,
                                                   MaximumMark = IED.MaximumMark,
                                                   MinimumMark = IED.MinimumMark,
                                                   Remarks = IER.Remarks,
                                                   AbsentStatus = (IER.ResultStatus == DbConstants.ResultStatus.Absent ? true : false),
                                                   InternalExamDetailsKey = IED.RowKey,
                                                   StudentEmail = App.StudentEmail,
                                                   MobileNumber = App.StudentMobile,
                                                   CourseName = App.Course.CourseName,
                                                   UniversityName = App.UniversityMaster.UniversityMasterName,
                                                   BatchName = App.Batch.BatchName,
                                                   SubjctName = IER.Subject.SubjectName,
                                                   AcademicTermKey = IE.AcademicTermKey,
                                                   CourseDuration = IE.Course.CourseDuration,
                                                   SubjectYear = IE.ExamYear,
                                                   InternalExamTermName = IE.InternalExamTerm.InternalExamTermName,
                                                   ResultStatusText = (IER.ResultStatus == DbConstants.ResultStatus.Absent ? "Absent" : IER.ResultStatus == DbConstants.ResultStatus.Passed ? "Pass" : "Fail"),
                                               }).ToList();

            foreach (InternalExamResultDetail examresult in model.InternalExamResultDetails)
            {
                examresult.ExamYearText = CommonUtilities.GetYearDescriptionByCodeDetails(examresult.CourseDuration ?? 0, examresult.SubjectYear ?? 0, examresult.AcademicTermKey ?? 0);

            }

            FillNotificationDetail(model);
            return model;
        }

        private void FillBranches(InternalExamResultViewModel model)
        {
            IQueryable<SelectListModel> BranchQuery = dbcontext.vwBranchSelectActiveOnlies.OrderBy(row => row.DisplayOrder).Select(row => new SelectListModel
            {
                RowKey = row.RowKey,
                Text = row.BranchName
            });

            Employee Employee = dbcontext.Employees.Where(x => x.AppUserKey == DbConstants.User.UserKey).SingleOrDefault();
            if (Employee != null)
            {
                if (Employee.BranchAccess != null)
                {
                    List<long> Branches = Employee.BranchAccess.Split(',').Select(Int64.Parse).ToList();
                    model.Branches = BranchQuery.Where(row => Branches.Contains(row.RowKey)).ToList();
                }
                else
                {
                    model.Branches = BranchQuery.Where(x => x.RowKey == Employee.BranchKey).ToList();
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

        public InternalExamResultViewModel GetEmployeesByBranchId(InternalExamResultViewModel model)
        {
            Employee Employee = dbcontext.Employees.Where(x => x.AppUserKey == DbConstants.User.UserKey).SingleOrDefault();

            if (DbConstants.User.UserKey != DbConstants.AdminKey)
            {
                if (Employee != null)
                {
                    model.Employees = dbcontext.Employees.Where(row => (row.BranchKey == model.BranchKey || model.BranchKey == 0) && row.RowKey == Employee.RowKey).Select(row => new SelectListModel
                    {
                        RowKey = row.RowKey,
                        Text = row.FirstName,
                    }).OrderBy(row => row.Text).ToList();
                    model.SearchEmployeeKey = model.EmployeeKey = Employee.RowKey;
                }
            }
            else
            {
                model.Employees = dbcontext.Employees.Where(row => row.IsActive == true && (row.BranchKey == model.BranchKey || model.BranchKey == 0)).Select(row => new SelectListModel
                {
                    RowKey = row.RowKey,
                    Text = row.FirstName,

                }).OrderBy(row => row.Text).ToList();
                if (Employee != null)
                {
                    model.SearchEmployeeKey = model.EmployeeKey = Employee.RowKey;
                }
            }
            return model;
        }

        public InternalExamResultViewModel GetSearchDropDownLists(InternalExamResultViewModel model)
        {
            FillBranches(model);

            //GetEmployeesByBranchId(model);

            return model;
        }

        public InternalExamResultViewModel DeleteInternalExamResult(long? InternalExamKey, long? InternalExamDetailsKey, long? SubjectKey)
        {
            using (var transaction = dbcontext.Database.BeginTransaction())
            {
                InternalExamResultViewModel model = new InternalExamResultViewModel();
                try
                {
                    List<InternalExamResult> internalExamResultList = dbcontext.InternalExamResults.Where(x => x.InternalExamDetailsKey == InternalExamDetailsKey && x.SubjectKey == SubjectKey && x.InternalExamKey == InternalExamKey).ToList();
                    if (internalExamResultList.Count > 0)
                    {
                        dbcontext.InternalExamResults.RemoveRange(internalExamResultList);
                        dbcontext.SaveChanges();
                        transaction.Commit();
                        model.Message = EduSuiteUIResources.Success;
                        model.IsSuccessful = true;
                        ActivityLog.CreateActivityLog(MenuConstants.InternalExamResult, ActionConstants.Delete, DbConstants.LogType.Info, InternalExamDetailsKey, model.Message);
                    }
                    else
                    {
                        transaction.Rollback();
                        model.Message = String.Format(EduSuiteUIResources.FailedToDeleteErrorMessage, EduSuiteUIResources.InternalExamResult);
                        model.IsSuccessful = false;
                    }
                }
                catch (DbUpdateException ex)
                {
                    transaction.Rollback();
                    if (ex.GetBaseException().Message.ToUpper().Contains(EduSuiteUIResources.ForeignKeyError.ToUpper()))
                    {
                        model.Message = String.Format(EduSuiteUIResources.CantDeleteErrorMessage, EduSuiteUIResources.InternalExamResult);
                        model.IsSuccessful = false;
                        ActivityLog.CreateActivityLog(MenuConstants.InternalExamResult, ActionConstants.Delete, DbConstants.LogType.Error, InternalExamDetailsKey, ex.GetBaseException().Message);
                    }
                }
                catch (Exception Ex)
                {
                    transaction.Rollback();
                    model.Message = String.Format(EduSuiteUIResources.FailedToDeleteErrorMessage, EduSuiteUIResources.InternalExamResult);
                    model.IsSuccessful = false;
                    ActivityLog.CreateActivityLog(MenuConstants.InternalExamResult, ActionConstants.Delete, DbConstants.LogType.Error, InternalExamDetailsKey, model.Message);
                }
                return model;
            }

        }

        public InternalExamResultViewModel FillSearchClassDetails(InternalExamResultViewModel model)
        {

            if (model.BranchKey != 0)
            {
                model.ClassDetails = (from CD in dbcontext.VwClassDetailsSelectActiveOnlies

                                      join SDA in dbcontext.StudentDivisionAllocations on CD.RowKey equals SDA.ClassDetailsKey
                                      join A in dbcontext.Applications on CD.RowKey equals A.ClassDetailsKey
                                      where (SDA.IsActive == true && A.BranchKey == model.BranchKey)
                                      select new SelectListModel
                                      {
                                          RowKey = CD.RowKey,
                                          Text = CD.ClassCode + CD.ClassCodeDescription
                                      }).Distinct().ToList();
            }
            else
            {
                model.ClassDetails = (from CD in dbcontext.VwClassDetailsSelectActiveOnlies

                                      join SDA in dbcontext.StudentDivisionAllocations on CD.RowKey equals SDA.ClassDetailsKey
                                      where (SDA.IsActive == true)
                                      select new SelectListModel
                                      {
                                          RowKey = CD.RowKey,
                                          Text = CD.ClassCode + CD.ClassCodeDescription
                                      }).Distinct().ToList();
            }

            return model;
        }
        public InternalExamResultViewModel FillSearchBatch(InternalExamResultViewModel model)
        {

            if (model.BranchKey != 0)
            {
                model.Batches = (from p in dbcontext.Applications
                                 join SDA in dbcontext.StudentDivisionAllocations on p.RowKey equals SDA.ApplicationKey
                                 join B in dbcontext.VwBatchSelectActiveOnlies on p.BatchKey equals B.RowKey
                                 orderby B.RowKey
                                 //where (p.CourseKey == model.CourseKey && p.BranchKey == model.BranchKey && p.UniversityMasterKey == model.UniversityMasterKey)
                                 where (p.BranchKey == model.BranchKey)
                                 select new SelectListModel
                                 {
                                     RowKey = B.RowKey,
                                     Text = B.BatchName
                                 }).Distinct().ToList();
            }
            else
            {
                model.Batches = (from p in dbcontext.Applications
                                 join SDA in dbcontext.StudentDivisionAllocations on p.RowKey equals SDA.ApplicationKey
                                 join B in dbcontext.VwBatchSelectActiveOnlies on p.BatchKey equals B.RowKey
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

        public InternalExamResultViewModel UpdateInternalExamResults(InternalExamResultViewModel MasterModel)
        {
            using (var transaction = dbcontext.Database.BeginTransaction())
            {

                try
                {
                    dbcontext.Configuration.AutoDetectChangesEnabled = false;
                    int Count = 0;

                    InternalExam internalExamModel = dbcontext.InternalExams.Where(x =>
                    x.UniversityMasterKey == MasterModel.UniversityMasterKey
                    && x.BatchKey == MasterModel.BatchKey
                    && x.ExamYear == MasterModel.ExamYear
                    && x.InternalExamTermKey == MasterModel.InternalExamTermKey
                    && x.BranchKey == MasterModel.BranchKey
                    && x.CourseKey == MasterModel.CourseKey
                    && x.AcademicTermKey == MasterModel.AcademicTermKey
                    ).FirstOrDefault();
                    //if (internalExamModel == null)
                    //{
                    //    internalExamModel = new InternalExam();

                    //    long examScheduleMaxKey = dbContext.InternalExams.Select(p => p.RowKey).DefaultIfEmpty().Max();


                    //    internalExamModel.RowKey = ++examScheduleMaxKey;
                    //    internalExamModel.UniversityMasterKey = MasterModel.UniversityMasterKey;
                    //    internalExamModel.BatchKey = MasterModel.BatchKey;
                    //    internalExamModel.ExamYear = (short)MasterModel.ExamYear;
                    //    internalExamModel.InternalExamTermKey = MasterModel.InternalExamTermKey ?? 0;
                    //    internalExamModel.BranchKey = MasterModel.BranchKey ?? 0;
                    //    internalExamModel.CourseKey = MasterModel.CourseKey;
                    //    internalExamModel.AcademicTermKey = MasterModel.AcademicTermKey;
                    //    dbContext.InternalExams.Add(internalExamModel);

                    //}



                    if (internalExamModel != null)
                    {
                        //    List<long> ApplicationKeys = MasterModel.InternalExamResultDetails.Select(x => x.ApplicationKey).ToList();
                        //List<long> Classes = dbContext.Applications.Where(x => ApplicationKeys.Contains(x.RowKey) && x.ClassDetailsKey != null).Select(x => x.ClassDetailsKey ?? 0).ToList();
                        //long examDivisionMaxKey = dbContext.InternalExams.Select(p => p.RowKey).DefaultIfEmpty().Max();
                        //foreach (long ClassKey in Classes)
                        //{
                        //    if (!dbContext.InternalExamDivisions.Any(x => x.InternalExamKey == internalExamModel.RowKey && x.ClassDetailsKey == ClassKey))
                        //    {
                        //        InternalExamDivision internalExamDivisionModel = new InternalExamDivision();
                        //        internalExamDivisionModel.RowKey = ++examDivisionMaxKey;
                        //        internalExamDivisionModel.InternalExamKey = internalExamModel.RowKey;
                        //        internalExamDivisionModel.ClassDetailsKey = ClassKey;

                        //        dbContext.InternalExamDivisions.Add(internalExamDivisionModel);
                        //    }
                        //}
                        long examResultMaxKey = dbcontext.InternalExamResults.Select(p => p.RowKey).DefaultIfEmpty().Max();
                        foreach (InternalExamResultDetail model in MasterModel.InternalExamResultDetails)
                        {
                            ++Count;
                            long? ClassDetailsKey = dbcontext.Applications.Where(x => x.RowKey == model.ApplicationKey).Select(x => x.ClassDetailsKey).SingleOrDefault();
                            var InternalExamDetail = internalExamModel.InternalExamDetails.Where(x => x.SubjectKey == model.SubjectKey).FirstOrDefault();
                            if (InternalExamDetail != null)
                            {
                                model.MinimumMark = InternalExamDetail.MinimumMark;
                                model.MaximumMark = InternalExamDetail.MaximumMark;
                                model.ResultStatus = model.ResultStatus == null ? ((model.MinimumMark ?? 0) > (model.Mark ?? 0) ? "F" : "P") : model.ResultStatus;
                                if (internalExamModel.RowKey != 0 && !dbcontext.InternalExamResults.Any(x => x.SubjectKey == model.SubjectKey && x.InternalExamDetailsKey == InternalExamDetail.RowKey && x.ApplicationKey == model.ApplicationKey))
                                {

                                    dbcontext.AddToContext(new InternalExamResult
                                    {
                                        RowKey = ++examResultMaxKey,
                                        ApplicationKey = model.ApplicationKey,
                                        InternalExamKey = internalExamModel.RowKey,
                                        InternalExamDetailsKey = InternalExamDetail.RowKey,
                                        SubjectKey = model.SubjectKey,
                                        ResultStatus = model.ResultStatus,
                                        Mark = model.Mark,
                                        ClassDetailsKey = ClassDetailsKey

                                    }, Count);
                                    ++Count;
                                }
                            }


                        }
                    }
                    dbcontext.SaveChanges();
                    dbcontext.Configuration.AutoDetectChangesEnabled = true;
                    transaction.Commit();

                    MasterModel.Message = EduSuiteUIResources.Success;
                    MasterModel.IsSuccessful = true;
                    ActivityLog.CreateActivityLog(MenuConstants.InternalExamResult, ActionConstants.Edit, DbConstants.LogType.Info, DbConstants.User.UserKey, MasterModel.Message);

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
                    ActivityLog.CreateActivityLog(MenuConstants.InternalExamResult, ActionConstants.Edit, DbConstants.LogType.Error, DbConstants.User.UserKey, dbEx.GetBaseException().Message);

                    throw raise;

                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    MasterModel.Message = String.Format(EduSuiteUIResources.FailedToSaveErrorMessage, EduSuiteUIResources.InternalExamResult);
                    MasterModel.IsSuccessful = false;
                    ActivityLog.CreateActivityLog(MenuConstants.InternalExamResult, ActionConstants.Edit, DbConstants.LogType.Error, DbConstants.User.UserKey, ex.GetBaseException().Message);

                }
            }
            return MasterModel;
        }

        private void FillNotificationDetail(InternalExamResultViewModel model)
        {
            NotificationTemplate notificationTemplateModel = dbcontext.NotificationTemplates.SingleOrDefault(row => row.RowKey == DbConstants.NotificationTemplate.InternalExamResult);
            if (notificationTemplateModel != null)
            {
                model.AutoEmail = notificationTemplateModel.AutoEmail;
                model.AutoSMS = notificationTemplateModel.AutoSMS;
                model.TemplateKey = notificationTemplateModel.RowKey;
            }
        }


        public bool CheckexistingExam(InternalExamViewModel model)
        {
            bool CheckQuery = dbcontext.InternalExams.Any(x => x.BranchKey == model.BranchKey && x.BatchKey == model.BatchKey && x.UniversityMasterKey == model.UniversityMasterKey && x.CourseKey == model.CourseKey && x.AcademicTermKey == model.AcademicTermKey &&
                     x.ExamYear == model.ExamYear && x.InternalExamTermKey == model.InternalExamTermKey && x.InternalExamDivisions.Any(y => y.ClassDetailsKey == model.ClassDetailsKey));

            return CheckQuery;
        }
        public List<SelectListModel> FillStudentsById(InternalExamViewModel model)
        {
            var Applications = dbcontext.Applications.Where(x => x.StudentStatusKey == DbConstants.StudentStatus.Ongoing && x.BranchKey == model.BranchKey && x.BatchKey == model.BatchKey && x.AcademicTermKey == model.AcademicTermKey && x.CourseKey == model.CourseKey && x.UniversityMasterKey == model.UniversityMasterKey
            && x.ClassDetailsKey == model.ClassDetailsKey);

            return Applications.Select(row => new SelectListModel
            {
                RowKey = row.RowKey,
                ValueText = row.AdmissionNo,
                Text = row.StudentName
            }).ToList();
        }
        public List<SelectListModel> FillSubjectsById(InternalExamViewModel model)
        {
            long RowKey = 0;
            List<long> subjectkeys = new List<long>();
            var CheckQuery = dbcontext.InternalExams.Where(x => x.BranchKey == model.BranchKey && x.BatchKey == model.BatchKey && x.UniversityMasterKey == model.UniversityMasterKey && x.CourseKey == model.CourseKey && x.AcademicTermKey == model.AcademicTermKey &&
                     x.ExamYear == model.ExamYear && x.InternalExamTermKey == model.InternalExamTermKey && x.InternalExamDivisions.Any(y => y.ClassDetailsKey == model.ClassDetailsKey)).Select(row => row.RowKey);
            if (CheckQuery.Any())
            {
                RowKey = CheckQuery.FirstOrDefault();
            }
            else
            {
                RowKey = 0;
            }
            if (RowKey != 0)
            {
                subjectkeys = dbcontext.InternalExamDetails.Where(x => x.InternalExamKey == RowKey).Select(x => x.SubjectKey).ToList();
            }
            return dbcontext.VwSubjectSelectActiveOnlies.Where(x => subjectkeys.Contains(x.RowKey)).Select(row => new SelectListModel
            {
                RowKey = row.RowKey,
                ValueText = row.SubjectCode,
                Text = row.SubjectName
            }).ToList();
        }
        public List<SelectListModel> FillClassDetailsBy(InternalExamViewModel model)
        {
            return dbcontext.InternalExamDivisions.Where(x => x.InternalExam.BranchKey == model.BranchKey && x.InternalExam.BatchKey == model.BatchKey && x.InternalExam.UniversityMasterKey == model.UniversityMasterKey
                     && x.InternalExam.CourseKey == model.CourseKey && x.InternalExam.AcademicTermKey == model.AcademicTermKey && x.InternalExam.ExamYear == model.ExamYear && x.InternalExam.InternalExamTermKey == model.InternalExamTermKey).Select(row => new SelectListModel
                     {
                         RowKey = row.ClassDetailsKey ?? 0,
                         ValueText = row.ClassDetail.ClassCode,
                         Text = row.ClassDetail.ClassCode
                     }).ToList();
        }
    }
}
