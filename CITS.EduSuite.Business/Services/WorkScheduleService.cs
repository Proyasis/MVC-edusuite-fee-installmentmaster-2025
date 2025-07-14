using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CITS.EduSuite.Data;
using CITS.EduSuite.Business.Models.ViewModels;
using CITS.EduSuite.Business.Common;
using CITS.EduSuite.Business.Interfaces;
using System.Data.Entity.Infrastructure;
using CITS.EduSuite.Business.Models.Resources;

namespace CITS.EduSuite.Business.Services
{
    public class WorkScheduleService : IWorkScheduleService
    {
        private EduSuiteDatabase dbContext;

        public WorkScheduleService(EduSuiteDatabase objEduSuiteDatabase)
        {
            this.dbContext = objEduSuiteDatabase;
        }

        public WorkScheduleViewModel AddEditWorkSchedule(WorkScheduleViewModel model)
        {
            try
            {
                WorkScheduleViewModel objmodel = new WorkScheduleViewModel();
                objmodel = new WorkScheduleViewModel();

                FillDropdownList(objmodel);
                return objmodel;
            }

            catch (Exception Ex)
            {
                ActivityLog.CreateActivityLog(MenuConstants.EmployeeWorkSchedule, ActionConstants.View, DbConstants.LogType.Error, null, Ex.GetBaseException().Message);
                return new WorkScheduleViewModel();

            }
        }

        public WorkScheduleViewModel FIllWorkscheduleSubjectDetails(WorkScheduleViewModel model)
        {
            long RowKey = 0;
            var CheckQuery = dbContext.TeacherWorkScheduleMasters.Where(x => x.BranchKey == model.BranchKey && x.BatchKey == model.BatchKey && x.SubjectKey == model.SubjectKey
                    && x.ClassDetailsKey == model.ClassDetailsKey).Select(row => row.RowKey);
            if (CheckQuery.Any())
            {
                RowKey = CheckQuery.FirstOrDefault();
            }

            model.WorkscheduleSubjectmodel = (from MT in dbContext.ModuleTopics
                                              join TSM in dbContext.TeacherSubjectModules on new { ModuleKey = MT.SubjectModuleKey ?? 0 } equals new { ModuleKey = TSM.ModuleKey ?? 0 }
                                              join TSA in dbContext.TeacherSubjectAllocations on MT.SubjectModule.SubjectKey equals TSA.SubjectKey
                                              join TCA in dbContext.TeacherClassAllocations on TSA.TeacherClassAllocationKey equals TCA.RowKey
                                              join TWS in dbContext.TeacherWorkScheduleMasters on new { ClassDetailsKey = TCA.ClassDetailsKey ?? 0, BatchKey = TCA.BatchKey ?? 0, TSA.SubjectKey, MT.SubjectModuleKey, TopicKey = MT.RowKey }
                                              equals new { TWS.ClassDetailsKey, TWS.BatchKey, TWS.SubjectKey, TWS.SubjectModuleKey, TopicKey = TWS.TopicKey ?? 0 } into TWSD
                                              from TWS in TWSD.DefaultIfEmpty()
                                              where (MT.SubjectModule.SubjectKey == model.SubjectKey && TCA.ClassDetailsKey == model.ClassDetailsKey && TCA.BatchKey == model.BatchKey && TCA.Employee.BranchKey == model.BranchKey && TCA.EmployeeKey == model.EmployeeKey && TSM.IsActive == true)
                                              select new WorkscheduleSubjectmodel
                                              {
                                                  MasterRowKey = TWS.RowKey != null ? TWS.RowKey : 0,
                                                  TopicKey = MT.RowKey,
                                                  SubjectModuleKey = MT.SubjectModuleKey,
                                                  ModuleName = MT.SubjectModule.ModuleName,
                                                  TopicName = MT.TopicName,
                                                  TotalWorkDuration = TWS.RowKey != null ? TWS.TeacherWorkScheduleDetails.Sum(x => x.Duration) : 0,
                                                  ProgressStatus = TWS.RowKey != null ? TWS.TeacherWorkScheduleDetails.Sum(x => x.CurrentProgressStatus) : 0
                                              }).Distinct().ToList();

            return model;
        }

        public WorkscheduleSubjectmodel CreateWorkSchedule(WorkscheduleSubjectmodel model)
        {
            using (var transaction = dbContext.Database.BeginTransaction())
            {
                try
                {
                    long maxKey = dbContext.TeacherWorkScheduleMasters.Select(p => p.RowKey).DefaultIfEmpty().Max();

                    TeacherWorkScheduleMaster TeacherWorkScheduleMasterModel = new TeacherWorkScheduleMaster();
                    TeacherWorkScheduleMasterModel.RowKey = Convert.ToInt64(maxKey + 1);
                    TeacherWorkScheduleMasterModel.SubjectKey = model.SubjectKey ?? 0;
                    TeacherWorkScheduleMasterModel.BatchKey = model.BatchKey ?? 0;
                    TeacherWorkScheduleMasterModel.SubjectModuleKey = model.SubjectModuleKey;
                    TeacherWorkScheduleMasterModel.ClassDetailsKey = model.ClassDetailsKey ?? 0;
                    TeacherWorkScheduleMasterModel.BranchKey = model.BranchKey ?? 0;
                    TeacherWorkScheduleMasterModel.TopicKey = model.TopicKey;
                    TeacherWorkScheduleMasterModel.ProgressStatus = model.CurrentProgressStatus;

                    dbContext.TeacherWorkScheduleMasters.Add(TeacherWorkScheduleMasterModel);
                    model.MasterRowKey = TeacherWorkScheduleMasterModel.RowKey;
                    CreateWorkScheduleDetails(model);

                    dbContext.SaveChanges();
                    transaction.Commit();
                    model.RowKey = TeacherWorkScheduleMasterModel.RowKey;
                    model.Message = EduSuiteUIResources.Success;
                    model.IsSuccessful = true;
                    ActivityLog.CreateActivityLog(MenuConstants.EmployeeWorkSchedule, ActionConstants.Add, DbConstants.LogType.Info, model.RowKey, model.Message);

                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    model.Message = String.Format(EduSuiteUIResources.FailedToSaveErrorMessage, EduSuiteUIResources.WorkSchedule);
                    model.IsSuccessful = false;
                    ActivityLog.CreateActivityLog(MenuConstants.EmployeeWorkSchedule, ActionConstants.Add, DbConstants.LogType.Error, model.RowKey, ex.GetBaseException().Message);
                }
            }
            return model;

        }

        public WorkscheduleSubjectmodel UpdateWorkSchedule(WorkscheduleSubjectmodel model)
        {
            using (var transaction = dbContext.Database.BeginTransaction())
            {
                try
                {
                    TeacherWorkScheduleMaster TeacherWorkScheduleMasterModel = new TeacherWorkScheduleMaster();
                    TeacherWorkScheduleMasterModel = dbContext.TeacherWorkScheduleMasters.SingleOrDefault(row => row.RowKey == model.MasterRowKey);

                    if (model.RowKey == 0)
                    {
                        var checkWorkSchedule = dbContext.TeacherWorkScheduleDetails.Where(x => x.EmployeeKey == model.EmployeeKey && x.WorkScheduleDate == model.WorkScheduleDate
                            && x.TeacherWorkScheduleMaster.SubjectModuleKey == model.SubjectModuleKey && x.TeacherWorkScheduleMaster.SubjectKey == model.SubjectKey
                            && x.TeacherWorkScheduleMaster.TopicKey == model.TopicKey && x.TeacherWorkScheduleMaster.BatchKey == model.BatchKey
                            && x.TeacherWorkScheduleMaster.ClassDetailsKey == model.ClassDetailsKey && x.TeacherWorkScheduleMaster.BranchKey == model.BranchKey).Count();
                        if (checkWorkSchedule != 0)
                        {
                            model.Message = EduSuiteUIResources.WorkSheduleExist;
                            model.IsSuccessful = false;
                            return model;
                        }
                        CreateWorkScheduleDetails(model);
                    }
                    else if (model.RowKey != 0)
                    {
                        var checkWorkSchedule = dbContext.TeacherWorkScheduleDetails.Where(x => x.EmployeeKey == model.EmployeeKey && x.WorkScheduleDate == model.WorkScheduleDate
                           && x.TeacherWorkScheduleMaster.SubjectModuleKey == model.SubjectModuleKey && x.TeacherWorkScheduleMaster.SubjectKey == model.SubjectKey
                           && x.TeacherWorkScheduleMaster.TopicKey == model.TopicKey && x.TeacherWorkScheduleMaster.BatchKey == model.BatchKey
                           && x.TeacherWorkScheduleMaster.ClassDetailsKey == model.ClassDetailsKey && x.TeacherWorkScheduleMaster.BranchKey == model.BranchKey && x.RowKey != model.RowKey).Count();
                        if (checkWorkSchedule != 0)
                        {
                            model.Message = EduSuiteUIResources.WorkSheduleExist;
                            model.IsSuccessful = false;
                            return model;
                        }
                        UpdateWorkScheduleDetails(model);
                    }
                    dbContext.SaveChanges();
                    transaction.Commit();

                    model.Message = EduSuiteUIResources.Success;
                    model.IsSuccessful = true;
                    ActivityLog.CreateActivityLog(MenuConstants.EmployeeWorkSchedule, ActionConstants.Edit, DbConstants.LogType.Info, model.RowKey, model.Message);

                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    model.Message = String.Format(EduSuiteUIResources.FailedToSaveErrorMessage, EduSuiteUIResources.WorkSchedule);
                    model.IsSuccessful = false;
                    ActivityLog.CreateActivityLog(MenuConstants.EmployeeWorkSchedule, ActionConstants.Edit, DbConstants.LogType.Error, model.RowKey, ex.GetBaseException().Message);
                }
            }
            return model;
        }

        private void CreateWorkScheduleDetails(WorkscheduleSubjectmodel model)
        {
            Int64 MaxKey = dbContext.TeacherWorkScheduleDetails.Select(p => p.RowKey).DefaultIfEmpty().Max();


            TeacherWorkScheduleDetail TeacherWorkScheduleDetailViewModel = new TeacherWorkScheduleDetail();

            TeacherWorkScheduleDetailViewModel.RowKey = Convert.ToInt64(MaxKey + 1);
            TeacherWorkScheduleDetailViewModel.TecherScheduleMasterKey = model.MasterRowKey; ;
            TeacherWorkScheduleDetailViewModel.WorkScheduleDate = model.WorkScheduleDate;
            TeacherWorkScheduleDetailViewModel.Duration = model.Duration ?? 0;
            TeacherWorkScheduleDetailViewModel.TimeIn = model.TimeIn;
            TeacherWorkScheduleDetailViewModel.TimeOut = model.TimeOut;
            TeacherWorkScheduleDetailViewModel.CurrentProgressStatus = model.CurrentProgressStatus;
            TeacherWorkScheduleDetailViewModel.EmployeeKey = model.EmployeeKey;

            dbContext.TeacherWorkScheduleDetails.Add(TeacherWorkScheduleDetailViewModel);


        }

        private void UpdateWorkScheduleDetails(WorkscheduleSubjectmodel model)
        {

            TeacherWorkScheduleDetail TeacherWorkScheduleDetailViewModel = new TeacherWorkScheduleDetail();

            TeacherWorkScheduleDetailViewModel = dbContext.TeacherWorkScheduleDetails.SingleOrDefault(x => x.RowKey == model.RowKey);
            TeacherWorkScheduleDetailViewModel.TecherScheduleMasterKey = model.MasterRowKey; ;
            TeacherWorkScheduleDetailViewModel.WorkScheduleDate = model.WorkScheduleDate;
            TeacherWorkScheduleDetailViewModel.Duration = model.Duration ?? 0;
            TeacherWorkScheduleDetailViewModel.TimeIn = model.TimeIn;
            TeacherWorkScheduleDetailViewModel.TimeOut = model.TimeOut;
            TeacherWorkScheduleDetailViewModel.CurrentProgressStatus = model.CurrentProgressStatus;
            TeacherWorkScheduleDetailViewModel.EmployeeKey = model.EmployeeKey;

        }

        public WorkscheduleSubjectmodel AddEditWorkScheduleDetail(WorkscheduleSubjectmodel model)
        {
            try
            {
                if (model.RowKey != 0)
                {
                    WorkscheduleSubjectmodel objmodel = new WorkscheduleSubjectmodel();
                    objmodel.ProgressStatus = model.ProgressStatus;
                    model = dbContext.TeacherWorkScheduleDetails.Where(x => x.RowKey == model.RowKey).Select(row => new WorkscheduleSubjectmodel
                    {
                        RowKey = row.RowKey,
                        MasterRowKey = row.TecherScheduleMasterKey,
                        WorkScheduleDate = row.WorkScheduleDate,
                        Duration = row.Duration,
                        TimeIn = row.TimeIn,
                        TimeOut = row.TimeOut,
                        CurrentProgressStatus = row.CurrentProgressStatus,
                        OldCurrentProgressStatus = row.CurrentProgressStatus,
                        EmployeeKey = row.EmployeeKey,
                    }).FirstOrDefault();
                    model.ProgressStatus = objmodel.ProgressStatus;
                }
                return model;
            }

            catch (Exception Ex)
            {
                ActivityLog.CreateActivityLog(MenuConstants.EmployeeWorkSchedule, ActionConstants.View, DbConstants.LogType.Error, null, Ex.GetBaseException().Message);
                return new WorkscheduleSubjectmodel();

            }
        }

        public List<WorkscheduleSubjectmodel> GetHistoryWorkSchedule(WorkscheduleSubjectmodel model)
        {
            try
            {
                List<WorkscheduleSubjectmodel> objviewModel = new List<WorkscheduleSubjectmodel>();
                objviewModel = dbContext.TeacherWorkScheduleDetails.Where(x => x.TecherScheduleMasterKey == model.MasterRowKey).Select(row => new WorkscheduleSubjectmodel
                {
                    RowKey = row.RowKey,
                    TopicKey = row.TeacherWorkScheduleMaster.TopicKey,
                    TopicName = row.TeacherWorkScheduleMaster.ModuleTopic.TopicName,
                    ModuleName = row.TeacherWorkScheduleMaster.SubjectModule.ModuleName,
                    MasterRowKey = row.TecherScheduleMasterKey,
                    WorkScheduleDate = row.WorkScheduleDate,
                    Duration = row.Duration,
                    TimeIn = row.TimeIn,
                    TimeOut = row.TimeOut,
                    CurrentProgressStatus = row.CurrentProgressStatus,
                    EmployeeKey = row.EmployeeKey,
                    EmployeeName = row.Employee.FirstName,
                    AppUserKey = row.Employee.AppUserKey,
                    ProgressStatus = dbContext.TeacherWorkScheduleDetails.Where(x => x.TecherScheduleMasterKey == model.MasterRowKey).Select(y => y.CurrentProgressStatus).Sum(),
                }).ToList();
                if (model == null)
                {
                    model = new WorkscheduleSubjectmodel();
                }
                return objviewModel;
            }

            catch (Exception Ex)
            {
                ActivityLog.CreateActivityLog(MenuConstants.EmployeeWorkSchedule, ActionConstants.View, DbConstants.LogType.Error, null, Ex.GetBaseException().Message);
                return new List<WorkscheduleSubjectmodel>();

            }
        }

        public WorkscheduleSubjectmodel DeleteWorkSchedule(WorkscheduleSubjectmodel model)
        {
            using (var transaction = dbContext.Database.BeginTransaction())
            {
                try
                {
                    TeacherWorkScheduleDetail TeacherWorkScheduleDetailModel = dbContext.TeacherWorkScheduleDetails.SingleOrDefault(row => row.RowKey == model.RowKey);
                    long TecherScheduleMasterKey = TeacherWorkScheduleDetailModel != null ? TeacherWorkScheduleDetailModel.TecherScheduleMasterKey : 0;
                    dbContext.TeacherWorkScheduleDetails.Remove(TeacherWorkScheduleDetailModel);
                    dbContext.SaveChanges();

                    if (TecherScheduleMasterKey != 0)
                    {
                        int TeacherWorkScheduleDetailListCount = dbContext.TeacherWorkScheduleDetails.Where(row => row.TecherScheduleMasterKey == TecherScheduleMasterKey).Count();
                        if (TeacherWorkScheduleDetailListCount <= 0)
                        {
                            TeacherWorkScheduleMaster TeacherWorkScheduleMasterModel = dbContext.TeacherWorkScheduleMasters.SingleOrDefault(row => row.RowKey == TecherScheduleMasterKey);
                            dbContext.TeacherWorkScheduleMasters.Remove(TeacherWorkScheduleMasterModel);
                            dbContext.SaveChanges();
                        }
                    }

                    transaction.Commit();
                    model.Message = EduSuiteUIResources.Success;
                    model.IsSuccessful = true;
                    ActivityLog.CreateActivityLog(MenuConstants.EmployeeWorkSchedule, ActionConstants.Delete, DbConstants.LogType.Info, model.RowKey, model.Message);
                }
                catch (DbUpdateException ex)
                {
                    transaction.Rollback();
                    if (ex.GetBaseException().Message.ToUpper().Contains(EduSuiteUIResources.ForeignKeyError.ToUpper()))
                    {
                        model.Message = String.Format(EduSuiteUIResources.CantDeleteErrorMessage, EduSuiteUIResources.WorkSchedule);
                        model.IsSuccessful = false;
                        ActivityLog.CreateActivityLog(MenuConstants.EmployeeWorkSchedule, ActionConstants.Delete, DbConstants.LogType.Debug, model.RowKey, ex.GetBaseException().Message);
                    }
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    model.Message = String.Format(EduSuiteUIResources.FailedToDeleteErrorMessage, EduSuiteUIResources.WorkSchedule);
                    model.IsSuccessful = false;
                    ActivityLog.CreateActivityLog(MenuConstants.EmployeeWorkSchedule, ActionConstants.Delete, DbConstants.LogType.Debug, model.RowKey, ex.GetBaseException().Message);
                }
            }

            return model;
        }

        #region DropDownChange Events
        private void FillDropdownList(WorkScheduleViewModel model)
        {
            FillBranch(model);
            FillTeacher(model);
            FillClassDetails(model);
            FillBatch(model);
            FillSubjects(model);
        }
        private void FillBranch(WorkScheduleViewModel model)
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
        public WorkScheduleViewModel FillTeacher(WorkScheduleViewModel model)
        {
            if (!DbConstants.Role.AdminUserTypes.Contains(DbConstants.User.RoleKey))
            {
                Employee Employee = dbContext.Employees.Where(x => x.AppUserKey == DbConstants.User.UserKey && x.IsTeacher == true).SingleOrDefault();
                if (Employee != null)
                {
                    model.Employees = dbContext.VwTeacherSelectActiveOnlies.Where(x => x.RowKey == Employee.RowKey).Select(x => new SelectListModel
                    {
                        RowKey = x.RowKey,
                        Text = x.FirstName
                    }).Distinct().ToList();
                }
                else
                {
                    //if (model.ClassDetailsKey != 0 && model.ClassDetailsKey != null)
                    //{
                    //    model.Employees = dbContext.VwTeacherSelectActiveOnlies.Where(x => x.ClassDetailsKey == model.ClassDetailsKey && x.BatchKey == model.BatchKey).Select(x => new SelectListModel
                    //    {
                    //        RowKey = x.RowKey,
                    //        Text = x.FirstName
                    //    }).Distinct().ToList();
                    //}
                    //else
                    //{
                    model.Employees = dbContext.VwTeacherSelectActiveOnlies.Where(y => y.BranchKey == model.BranchKey).Select(x => new SelectListModel
                    {
                        RowKey = x.RowKey,
                        Text = x.FirstName
                    }).Distinct().ToList();
                    //}
                }
            }
            else
            {
                model.Employees = dbContext.VwTeacherSelectActiveOnlies.Where(y => y.BranchKey == model.BranchKey).Select(x => new SelectListModel
                {
                    RowKey = x.RowKey,
                    Text = x.FirstName
                }).Distinct().ToList();

            }

            return model;
        }
        public WorkScheduleViewModel FillClassDetails(WorkScheduleViewModel model)
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
                                          where (TCA.EmployeeKey == Employee.RowKey && TCA.IsActive == true)// && A.BranchKey == Employee.BranchKey && (model.IsUpdate != true ? SDA.IsActive == true : SDA.IsActive != null))
                                          select new SelectListModel
                                          {
                                              RowKey = CD.RowKey,
                                              Text = CD.ClassCode + CD.ClassCodeDescription
                                          }).Distinct().ToList();

                }
                else
                {
                    if (model.EmployeeKey != 0 && model.EmployeeKey != null)
                    {
                        model.ClassDetails = (from CD in dbContext.VwClassDetailsSelectActiveOnlies

                                              join SDA in dbContext.StudentDivisionAllocations on CD.RowKey equals SDA.ClassDetailsKey
                                              join TCA in dbContext.TeacherClassAllocations on CD.RowKey equals TCA.ClassDetailsKey
                                              join A in dbContext.Applications on CD.RowKey equals A.ClassDetailsKey
                                              where (TCA.EmployeeKey == model.EmployeeKey && TCA.IsActive == true)// && A.BranchKey == model.BranchKey && (model.IsUpdate != true ? SDA.IsActive == true : SDA.IsActive != null))
                                              select new SelectListModel
                                              {
                                                  RowKey = CD.RowKey,
                                                  Text = CD.ClassCode + CD.ClassCodeDescription
                                              }).Distinct().ToList();
                    }
                }
            }

            else
            {
                if (model.EmployeeKey != 0 && model.EmployeeKey != null)
                {
                    model.ClassDetails = (from CD in dbContext.VwClassDetailsSelectActiveOnlies

                                          join SDA in dbContext.StudentDivisionAllocations on CD.RowKey equals SDA.ClassDetailsKey
                                          join TCA in dbContext.TeacherClassAllocations on CD.RowKey equals TCA.ClassDetailsKey
                                          join A in dbContext.Applications on CD.RowKey equals A.ClassDetailsKey
                                          where (TCA.EmployeeKey == model.EmployeeKey && TCA.IsActive == true)// && A.BranchKey == model.BranchKey && (model.IsUpdate != true ? SDA.IsActive == true : SDA.IsActive != null))
                                          select new SelectListModel
                                          {
                                              RowKey = CD.RowKey,
                                              Text = CD.ClassCode + CD.ClassCodeDescription
                                          }).Distinct().ToList();
                }

            }
            return model;
        }
        public WorkScheduleViewModel FillBatch(WorkScheduleViewModel model)
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
                                     where (p.ClassDetailsKey == model.ClassDetailsKey && TCA.EmployeeKey == Employee.RowKey && TCA.IsActive == true)//&& p.BranchKey == Employee.BranchKey && model.IsUpdate != true ? SDA.IsActive == true : SDA.IsActive != null)
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
                                     join TCA in dbContext.TeacherClassAllocations on B.RowKey equals TCA.BatchKey
                                     orderby B.RowKey
                                     where (p.ClassDetailsKey == model.ClassDetailsKey && TCA.EmployeeKey == model.EmployeeKey && TCA.IsActive == true)//&& p.BranchKey == Employee.BranchKey && model.IsUpdate != true ? SDA.IsActive == true : SDA.IsActive != null)
                                     select new SelectListModel
                                     {
                                         RowKey = B.RowKey,
                                         Text = B.BatchName
                                     }).Distinct().ToList();
                }
            }

            else
            {
                if (model.EmployeeKey != 0 && model.EmployeeKey != null && model.ClassDetailsKey != 0 && model.ClassDetailsKey != null)
                {
                    model.Batches = (from p in dbContext.Applications
                                     join SDA in dbContext.StudentDivisionAllocations on p.RowKey equals SDA.ApplicationKey
                                     join B in dbContext.VwBatchSelectActiveOnlies on p.BatchKey equals B.RowKey
                                     join TCA in dbContext.TeacherClassAllocations on B.RowKey equals TCA.BatchKey
                                     orderby B.RowKey
                                     where (p.ClassDetailsKey == model.ClassDetailsKey && TCA.EmployeeKey == model.EmployeeKey && TCA.IsActive == true && TCA.ClassDetailsKey == model.ClassDetailsKey)//&& p.BranchKey == model.BranchKey && model.IsUpdate != true ? SDA.IsActive == true : SDA.IsActive != null)
                                     select new SelectListModel
                                     {
                                         RowKey = B.RowKey,
                                         Text = B.BatchName
                                     }).Distinct().ToList();
                }
                //model.Batches = (from p in dbContext.Applications
                //                 join SDA in dbContext.StudentDivisionAllocations on p.RowKey equals SDA.ApplicationKey
                //                 join B in dbContext.VwBatchSelectActiveOnlies on p.BatchKey equals B.RowKey
                //                 orderby B.RowKey
                //                 where (p.ClassDetailsKey == model.ClassDetailsKey && p.BranchKey == model.BranchKey)
                //                 select new SelectListModel
                //                 {
                //                     RowKey = B.RowKey,
                //                     Text = B.BatchName
                //                 }).Distinct().ToList();
            }

            return model;
        }

        public WorkScheduleViewModel FillSubjects(WorkScheduleViewModel model)
        {
            if (model.ClassDetailsKey != 0 && model.ClassDetailsKey != null)
            {
                if (!DbConstants.Role.AdminUserTypes.Contains(DbConstants.User.RoleKey))
                {
                    Employee Employee = dbContext.Employees.Where(x => x.AppUserKey == DbConstants.User.UserKey && x.IsTeacher == true).SingleOrDefault();
                    if (Employee != null && model.BatchKey != 0 && model.BatchKey != null)
                    {

                        TeacherClassAllocation teacherclass = dbContext.TeacherClassAllocations.SingleOrDefault(x => x.EmployeeKey == Employee.RowKey && x.ClassDetailsKey == model.ClassDetailsKey && x.BatchKey == model.BatchKey);

                        model.Subjects = dbContext.TeacherSubjectAllocations.Where(x => x.Employeekey == Employee.RowKey && x.TeacherClassAllocationKey == teacherclass.RowKey).Select(x => new SelectListModel
                        {
                            RowKey = x.SubjectKey,
                            Text = x.Subject.SubjectName
                        }).ToList();
                    }
                    //else
                    //{
                    //    ClassDetail classDetailList = dbContext.ClassDetails.SingleOrDefault(x => x.RowKey == model.ClassDetailsKey);

                    //    model.Subjects = dbContext.VwSubjectSelectActiveOnlies.Where(x => x.CourseKey == classDetailList.UniversityCourse.CourseKey && x.UniversityMasterKey == classDetailList.UniversityCourse.UniversityMasterKey && x.AcademicTermKey == classDetailList.UniversityCourse.AcademicTermKey && x.CourseYear == classDetailList.StudentYear).Select(x => new SelectListModel
                    //    {
                    //        RowKey = x.RowKey,
                    //        Text = x.SubjectName
                    //    }).ToList();
                    //}

                }

                else
                {
                    if (model.EmployeeKey != 0 && model.EmployeeKey != null && model.ClassDetailsKey != 0 && model.ClassDetailsKey != null && model.BatchKey != 0 && model.BatchKey != null)
                    {
                        TeacherClassAllocation teacherclass = dbContext.TeacherClassAllocations.SingleOrDefault(x => x.EmployeeKey == model.EmployeeKey && x.ClassDetailsKey == model.ClassDetailsKey && x.BatchKey == model.BatchKey);

                        model.Subjects = dbContext.TeacherSubjectAllocations.Where(x => x.Employeekey == model.EmployeeKey && x.TeacherClassAllocationKey == teacherclass.RowKey).Select(x => new SelectListModel
                        {
                            RowKey = x.SubjectKey,
                            Text = x.Subject.SubjectName
                        }).ToList();
                    }

                }
            }
            return model;

        }

        #endregion DropDownChange Events
    }
}
