using CITS.EduSuite.Business.Interfaces;
using CITS.EduSuite.Business.Common;
using CITS.EduSuite.Data;
using CITS.EduSuite.Business.Models.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.Entity.Infrastructure;
using CITS.EduSuite.Business.Models.Resources;

namespace CITS.EduSuite.Business.Services
{
    public class DivisionAllocationService : IDivisionAllocationService
    {
        private EduSuiteDatabase dbContext;

        public DivisionAllocationService(EduSuiteDatabase Objdb)
        {
            this.dbContext = Objdb;
        }

        public DivisionAllocationViewModel GetDivisionAllocationById(DivisionAllocationViewModel model)
        {
            try
            {
                if (model.ClassDetailsKey != 0)
                {
                    ClassDetail classDetailList = dbContext.ClassDetails.SingleOrDefault(x => x.RowKey == model.ClassDetailsKey);

                    model.CourseKey = classDetailList.UniversityCourse.CourseKey;
                    model.UniversityMasterKey = classDetailList.UniversityCourse.UniversityMasterKey;
                    model.CourseYear = classDetailList.StudentYear;
                    model.CourseTypeKey = classDetailList.UniversityCourse.Course.CourseTypeKey;
                    //model.DivisionKey = classDetailList.DivisionKey;
                    model.AcademicTermKey = classDetailList.UniversityCourse.AcademicTermKey;

                }

                if (!dbContext.StudentDivisionAllocations.Where(x => x.Application.BranchKey == model.BranchKey && x.Application.CourseKey == model.CourseKey && x.Application.BatchKey == model.BatchKey && x.Application.UniversityMasterKey == model.UniversityMasterKey &&
                    x.ClassDetailsKey == model.ClassDetailsKey && x.Application.CurrentYear == model.CourseYear && x.Application.AcademicTermKey == model.AcademicTermKey).Any())
                {
                    var result = dbContext.StudentDivisionAllocations.Where(x => x.Application.BranchKey == model.BranchKey && x.Application.CourseKey == model.CourseKey && x.Application.BatchKey == model.BatchKey && x.Application.UniversityMasterKey == model.UniversityMasterKey &&
                    x.ClassDetailsKey == model.ClassDetailsKey && x.Application.CurrentYear == model.CourseYear && x.Application.AcademicTermKey == model.AcademicTermKey).Select(row => new DivisionAllocationViewModel
                    {

                        CourseTypeKey = row.Application.Course.CourseTypeKey,
                    }).FirstOrDefault();

                }
                FillDropDown(model);
                FillDivisionAllocationDetailsViewModel(model);
                if (model.CourseTypeKey == 0)
                {
                    model.CourseTypeKey = dbContext.Courses.Where(row => row.RowKey == model.CourseKey).Select(row => row.CourseTypeKey).FirstOrDefault();
                }

                GeneralConfiguration generalconfiguration = dbContext.GeneralConfigurations.FirstOrDefault();
                model.GenderPriority = generalconfiguration.GenderPriority;
                model.GenderPriorityCount = generalconfiguration.GenderPriorityCount;

                
                //model.IsSuccessful = true;


                return model;
            }
            catch (Exception Ex)
            {
                ActivityLog.CreateActivityLog(MenuConstants.GenerateRollNumber, ActionConstants.View, DbConstants.LogType.Error, null, Ex.GetBaseException().Message);
                return new DivisionAllocationViewModel();

            }
        }

        private void FillDivisionAllocationDetailsViewModel(DivisionAllocationViewModel model)
        {

            var DivisionAllocationDetails = dbContext.SP_StudentDivisionAllocation_Select_ByType(model.UniversityMasterKey, model.CourseKey, model.BatchKey, model.BranchKey, model.CourseYear, model.ClassDetailsKey, model.AcademicTermKey)
                                                 .Select(row => new DivisionAllocationDetailsModel
                                                 {
                                                     StudentDivisionAllocationKey = Convert.ToInt64(row.StudentDivisionAllocationKey),
                                                     RollNumber = row.RollNumber ?? 0,
                                                     RollNoCode = row.RollnoCode,
                                                     ApplicationKey = row.RowKey ?? 0,
                                                     Name = row.StudentName,
                                                     AdmissionNo = row.AdmissionNo,
                                                     StudentYearText = row.CurrentYearText,
                                                     StudentYear = row.CurrentYear ?? 0,
                                                     IsActive = Convert.ToBoolean(row.CheckStatus),
                                                     GenderKey = row.Gender,
                                                     Gender = row.Gender == 1 ? "Male" : "Female"
                                                 });
            model.DivisionAllocationDetails = model.GenderPriority == 1 ? DivisionAllocationDetails.OrderBy(x => x.GenderKey).ToList() : DivisionAllocationDetails.OrderBy(x => x.Name).ToList();


        }

        public DivisionAllocationViewModel UpdateDivisionAllocation(DivisionAllocationViewModel model)
        {
            StudentDivisionAllocation StudentDivisionAllocationmodel = new StudentDivisionAllocation();

            FillDropDown(model);

            using (var transaction = dbContext.Database.BeginTransaction())
            {
                try
                {
                    CreateDivisionAllocationDetails(model);
                    UpdateDivisionAllocationdetails(model);
                    dbContext.SaveChanges();
                    transaction.Commit();
                    model.Message = EduSuiteUIResources.Success;
                    model.IsSuccessful = true;
                    ActivityLog.CreateActivityLog(MenuConstants.GenerateRollNumber, (model.DivisionAllocationDetails.Any(row => row.RowKey != 0) ? ActionConstants.Edit : ActionConstants.Add), DbConstants.LogType.Info, null, model.Message);
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

                    ActivityLog.CreateActivityLog(MenuConstants.GenerateRollNumber, (model.DivisionAllocationDetails.Any(row => row.RowKey != 0) ? ActionConstants.Edit : ActionConstants.Add), DbConstants.LogType.Error, null, dbEx.GetBaseException().Message);
                }
                catch (Exception Ex)
                {
                    transaction.Rollback();
                    model.Message = String.Format(EduSuiteUIResources.FailedToSaveErrorMessage, EduSuiteUIResources.GenerateRollNo);
                    model.IsSuccessful = false;
                    ActivityLog.CreateActivityLog(MenuConstants.GenerateRollNumber, (model.DivisionAllocationDetails.Any(row => row.RowKey != 0) ? ActionConstants.Edit : ActionConstants.Add), DbConstants.LogType.Error, null, Ex.GetBaseException().Message);
                }
            }
            return model;
        }

        private void CreateDivisionAllocationDetails(DivisionAllocationViewModel model)
        {
            long MaxKey = dbContext.StudentDivisionAllocations.Select(p => p.RowKey).DefaultIfEmpty().Max();
            foreach (DivisionAllocationDetailsModel modelDetails in model.DivisionAllocationDetails.Where(row => row.RowKey == 0))
            {
                StudentDivisionAllocation StudentDivisionAllocationmodel = new StudentDivisionAllocation();
                Application applicationDetails = dbContext.Applications.SingleOrDefault(x => x.RowKey == modelDetails.ApplicationKey);
                if (applicationDetails != null)
                {
                    //applicationDetails.DivisionKey = model.DivisionKey != 0 ? model.DivisionKey : dbContext.Divisions.Select(x => x.RowKey).FirstOrDefault();
                    applicationDetails.RollNumber = modelDetails.RollNumber;
                    applicationDetails.ClassDetailsKey = model.ClassDetailsKey;
                    applicationDetails.RollNoCode = modelDetails.RollNoCode;
                }

                StudentDivisionAllocationmodel.RowKey = MaxKey + 1;
                StudentDivisionAllocationmodel.ApplicationKey = modelDetails.ApplicationKey;
                StudentDivisionAllocationmodel.RollNumber = modelDetails.RollNumber ?? 0;
                StudentDivisionAllocationmodel.RollNoCode = modelDetails.RollNoCode;
                //StudentDivisionAllocationmodel.StudentYear = modelDetails.StudentYear;
                StudentDivisionAllocationmodel.ClassDetailsKey = model.ClassDetailsKey;
                StudentDivisionAllocationmodel.BatchKey = model.BatchKey;

                StudentDivisionAllocationmodel.IsActive = true;
                dbContext.StudentDivisionAllocations.Add(StudentDivisionAllocationmodel);
                MaxKey++;
            }
        }

        private void UpdateDivisionAllocationdetails(DivisionAllocationViewModel model)
        {
            //long MaxKey = dbContext.StudentDivisionAllocations.Select(p => p.RowKey).DefaultIfEmpty().Max();
            foreach (DivisionAllocationDetailsModel modelDetails in model.DivisionAllocationDetails.Where(row => row.RowKey != 0))
            {
                StudentDivisionAllocation StudentDivisionAllocationmodel = dbContext.StudentDivisionAllocations.SingleOrDefault(p => p.RowKey == modelDetails.RowKey);
                Application applicationDetails = dbContext.Applications.SingleOrDefault(x => x.RowKey == modelDetails.ApplicationKey);
                if (applicationDetails != null)
                {
                    //applicationDetails.DivisionKey = model.DivisionKey != 0 ? model.DivisionKey : dbContext.Divisions.Select(x => x.RowKey).FirstOrDefault();
                    applicationDetails.RollNumber = modelDetails.RollNumber;
                    applicationDetails.ClassDetailsKey = model.ClassDetailsKey;
                    applicationDetails.RollNoCode = modelDetails.RollNoCode;

                }
                StudentDivisionAllocationmodel.ApplicationKey = modelDetails.ApplicationKey;
                StudentDivisionAllocationmodel.RollNumber = modelDetails.RollNumber ?? 0;
                StudentDivisionAllocationmodel.RollNoCode = modelDetails.RollNoCode;
                //StudentDivisionAllocationmodel.StudentYear = modelDetails.StudentYear;
                StudentDivisionAllocationmodel.ClassDetailsKey = model.ClassDetailsKey;
                StudentDivisionAllocationmodel.BatchKey = model.BatchKey;
                StudentDivisionAllocationmodel.IsActive = true;

            }
        }

        public DivisionAllocationViewModel DeleteDivisionAllocation(DivisionAllocationViewModel model)
        {
            using (var transaction = dbContext.Database.BeginTransaction())
            {
                try
                {
                    //var attendanceDetails = dbContext.Attendances.Any(x => x.ClassDetailsKey == model.ClassDetailsKey);

                    //if (attendanceDetails == false)
                    //{

                    List<StudentDivisionAllocation> DivisionAllocationList = dbContext.StudentDivisionAllocations.Where(x => x.Application.CourseKey == model.CourseKey && x.Application.BatchKey == model.BatchKey && x.Application.UniversityMasterKey == model.UniversityMasterKey &&
                     x.ClassDetailsKey == model.ClassDetailsKey && x.Application.BranchKey == model.BranchKey).ToList();

                    if (DivisionAllocationList.Count > 0)
                    {
                        //DivisionAllocationList.ForEach(DivisionAllocation => dbContext.T_StudentDivisionAllocation.Remove(DivisionAllocation));

                        List<Application> applicationDetails = (from A in dbContext.Applications
                                                                join DL in dbContext.StudentDivisionAllocations.Where(x => x.Application.CourseKey == model.CourseKey && x.Application.BatchKey == model.BatchKey && x.Application.UniversityMasterKey == model.UniversityMasterKey &&
                                                                    x.ClassDetailsKey == model.ClassDetailsKey && x.Application.BranchKey == model.BranchKey) on A.RowKey equals DL.ApplicationKey
                                                                select A).ToList();

                        foreach (Application modelDetails in applicationDetails)
                        {
                            modelDetails.RollNumber = null;
                            modelDetails.RollNoCode = null;
                            modelDetails.ClassDetailsKey = null;
                        }

                        dbContext.StudentDivisionAllocations.RemoveRange(DivisionAllocationList);


                    }
                    dbContext.SaveChanges();
                    transaction.Commit();
                    model.Message = EduSuiteUIResources.Success;
                    model.IsSuccessful = true;
                    ActivityLog.CreateActivityLog(MenuConstants.GenerateRollNumber, ActionConstants.Delete, DbConstants.LogType.Info, null, model.Message);

                    //}
                    //else
                    //{
                    //    transaction.Rollback();
                    //    model.Message = EduSuiteUIResources.CantDeleteDivisionAllocation;
                    //    model.IsSuccessful = false;
                    //    ActivityLog.CreateActivityLog(MenuConstants.GenerateRollNumber, ActionConstants.Delete, DbConstants.LogType.Debug, null, model.Message);
                    //}
                }
                catch (DbUpdateException ex)
                {
                    transaction.Rollback();
                    if (ex.GetBaseException().Message.ToUpper().Contains(EduSuiteUIResources.ForeignKeyError.ToUpper()))
                    {
                        model.Message = String.Format(EduSuiteUIResources.CantDeleteErrorMessage, EduSuiteUIResources.GenerateRollNo);
                        model.IsSuccessful = false;
                        ActivityLog.CreateActivityLog(MenuConstants.GenerateRollNumber, ActionConstants.Delete, DbConstants.LogType.Debug, null, ex.GetBaseException().Message);

                    }
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    model.Message = String.Format(EduSuiteUIResources.FailedToDeleteErrorMessage, EduSuiteUIResources.GenerateRollNo);
                    model.IsSuccessful = false;
                    ActivityLog.CreateActivityLog(MenuConstants.GenerateRollNumber, ActionConstants.Delete, DbConstants.LogType.Debug, null, ex.GetBaseException().Message);
                }
            }
            return model;
        }

        public List<DivisionAllocationViewModel> GetDivisionAllocation(DivisionAllocationViewModel model)
        {
            try
            {

                Employee Employee = dbContext.Employees.Where(x => x.AppUserKey == DbConstants.User.UserKey).SingleOrDefault();

                IQueryable<DivisionAllocationViewModel> DivisionAllocationList = (from p in dbContext.StudentDivisionAllocations.Where(x => x.IsActive)
                                                                                  join App in dbContext.Applications.Where(x => x.StudentStatusKey == DbConstants.ApplicationStatus.OnGoing).GroupBy(x => new { x.ClassDetailsKey, x.CurrentYear, x.BatchKey, x.BranchKey }).Select(x => new { ClassDetailsKey = x.Key.ClassDetailsKey, NoOfStudents = x.Count(), BranchKey = x.Key.BranchKey, BatchKey = x.Key.BatchKey, CurrentYear = x.Key.CurrentYear })
                                                                                  on new { p.ClassDetailsKey, p.Application.BranchKey, p.Application.CurrentYear, p.Application.BatchKey } equals new { ClassDetailsKey = App.ClassDetailsKey ?? 0, App.BranchKey, App.CurrentYear, App.BatchKey }

                                                                                  where ((p.ClassDetail.ClassCode.Contains(model.searchText)) || (p.Application.Branch.BranchName.Contains(model.searchText)) || (p.Application.Course.CourseName.Contains(model.searchText)))
                                                                                  select new DivisionAllocationViewModel
                                                                                  {
                                                                                      AcademicTermKey = p.Application.AcademicTermKey,
                                                                                      BranchKey = p.Application.BranchKey,
                                                                                      BranchName = p.Application.Branch.BranchName,
                                                                                      ClassDetailsKey = p.ClassDetailsKey,
                                                                                      ClassDetailsName = p.ClassDetail.ClassCode,
                                                                                      CourseKey = p.Application.CourseKey,
                                                                                      CourseName = p.Application.Course.CourseName,
                                                                                      UniversityMasterKey = p.Application.UniversityMasterKey,
                                                                                      UniversityName = p.Application.UniversityMaster.UniversityMasterName,
                                                                                      BatchKey = p.Application.BatchKey,
                                                                                      BatchName = p.Application.Batch.BatchName,
                                                                                      CourseYear = p.Application.CurrentYear,
                                                                                      CourseDuration = p.Application.Course.CourseDuration ?? 0,
                                                                                      NoOfStudents = App.NoOfStudents
                                                                                  });


                if (Employee != null)
                {
                    if (Employee.BranchAccess != null)
                    {
                        var Branches = Employee.BranchAccess.Split(',').Select(Int16.Parse).ToList();
                        DivisionAllocationList = DivisionAllocationList.Where(row => Branches.Contains(row.BranchKey));
                    }
                }

                if (model.BranchKey != 0)
                {
                    DivisionAllocationList = DivisionAllocationList.Where(row => row.BranchKey == model.BranchKey);
                }
                if (model.BatchKey != 0)
                {
                    DivisionAllocationList = DivisionAllocationList.Where(row => row.BatchKey == model.BatchKey);
                }
                return DivisionAllocationList.GroupBy(x => new { x.ClassDetailsKey, x.CourseYear, x.BatchKey, x.BranchKey }).Select(y => y.FirstOrDefault()).ToList<DivisionAllocationViewModel>();

            }
            catch (Exception Ex)
            {
                ActivityLog.CreateActivityLog(MenuConstants.GenerateRollNumber, ActionConstants.View, DbConstants.LogType.Error, null, Ex.GetBaseException().Message);
                return new List<DivisionAllocationViewModel>();

            }
        }

        private void FillDropDown(DivisionAllocationViewModel model)
        {
            FillBranch(model);
            FillCourseType(model);
            FillClassDetails(model);

            FillBatch(model);

        }

        private void FillBranch(DivisionAllocationViewModel model)
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

        public void FillCourseType(DivisionAllocationViewModel model)
        {
            model.CourseTypes = dbContext.VwCourseTypeSelectActiveOnlies
                                 .Select(x => new SelectListModel
                                 {
                                     RowKey = x.RowKey,
                                     Text = x.CourseTypeName
                                 }).Distinct().ToList();



        }

        public DivisionAllocationViewModel FillClassDetails(DivisionAllocationViewModel model)
        {
            //if (model.IfEdit == true)
            //{
            //    UniversityCourse universityCourse = dbContext.UniversityCourses.SingleOrDefault(x => x.UniversityMasterKey == model.UniversityMasterKey && x.CourseKey == model.CourseKey && x.AcademicTermKey == model.AcademicTermKey);

            //    ClassDetail classDetails = dbContext.ClassDetails.SingleOrDefault(x => x.UniversityCourseKey == universityCourse.RowKey && x.StudentYear == model.CourseYear && x.DivisionKey == model.DivisionKey);
            //    if (classDetails.RowKey != 0)
            //    {
            //        model.ClassDetailsKey = classDetails.RowKey;
            //    }

            //}
            model.ClassDetails = dbContext.VwClassDetailsSelectActiveOnlies.Where(x => x.CourseTypeKey == model.CourseTypeKey).Select(x => new SelectListModel
            {
                RowKey = x.RowKey,
                Text = x.ClassCode + x.ClassCodeDescription
            }).ToList();
            return model;
        }

        public DivisionAllocationViewModel FillBatch(DivisionAllocationViewModel model)
        {

            if (model.ClassDetailsKey != 0)
            {
                //List<ClassDetail> classDetailList = dbContext.ClassDetails.Where(x => x.RowKey == model.ClassDetailsKey).ToList();

                ClassDetail classDetailList = dbContext.ClassDetails.SingleOrDefault(x => x.RowKey == model.ClassDetailsKey);

                model.CourseKey = classDetailList.UniversityCourse.CourseKey;
                model.UniversityMasterKey = classDetailList.UniversityCourse.UniversityMasterKey;


            }

            model.Batches = (from p in dbContext.Applications
                             join B in dbContext.VwBatchSelectActiveOnlies on p.BatchKey equals B.RowKey
                             orderby B.RowKey
                             //where (p.CourseKey == model.CourseKey && p.BranchKey == model.BranchKey && p.UniversityMasterKey == model.UniversityMasterKey)
                             where (p.CourseKey == model.CourseKey && p.UniversityMasterKey == model.UniversityMasterKey && p.BranchKey == model.BranchKey)
                             select new SelectListModel
                             {
                                 RowKey = B.RowKey,
                                 Text = B.BatchName
                             }).Distinct().ToList();


            return model;
        }

        public DivisionAllocationViewModel GenerateRollnumber(DivisionAllocationViewModel model)
        {
            try
            {
                //if (model.ApplicationKeys != null)
                //{
                //List<long> ApplicationKeysList = model.ApplicationKeys.Split(',').Select(Int64.Parse).ToList();
                //int RollNumber = 0;
                //string RollNoCode;
                //if (model.IfResetRollNo == false)
                //{
                //    RollNumber = dbContext.StudentDivisionAllocations.Where(row => row.Application.BatchKey == model.BatchKey &&

                //                      row.Application.StudentStatusKey == DbConstants.ApplicationStatus.OnGoing && row.IsActive == true && row.ClassDetailsKey == model.ClassDetailsKey)
                //                      .Select(p => p.RollNumber).DefaultIfEmpty().Max();
                //}
                //model.DivisionAllocationDetails = dbContext.Applications.Where(x => ApplicationKeysList.Contains(x.RowKey)).ToList()

                //                            .Select(row => new DivisionAllocationDetailsModel
                //                            {
                //                                RowKey = row.StudentDivisionAllocations.Where(x => x.IsActive).Select(DA => DA.RowKey).DefaultIfEmpty().FirstOrDefault(),
                //                                RollNumber = row.RollNumber,
                //                                ApplicationKey = row.RowKey,
                //                                Name = row.StudentName,
                //                                AdmissionNo = row.AdmissionNo,
                //                                RollNoCode = row.RollNoCode,
                //                                //IsActive = row.T_StudentDivisionAllocation.Select(p => p.IsActive).SingleOrDefault(),
                //                            }).OrderBy(x => x.RowKey).ToList();
                model.DivisionAllocationDetails = model.DivisionAllocationDetails.Where(X => X.IsActive).ToList();
                //if (model.IfResetRollNo == false)
                //{

                foreach (DivisionAllocationDetailsModel listmodel in model.DivisionAllocationDetails.Where(row => row.IsActive).OrderBy(row => row.RollNumber))
                {
                    //listmodel.RollNumber = ++RollNumber;
                    listmodel.RollNoCode = dbContext.Database.SqlQuery<string>("select dbo.F_GenerateRollNoCode(" + model.ClassDetailsKey + "," + (listmodel.RollNumber - 1) + ")").Single().ToString();
                    listmodel.RowKey = listmodel.StudentDivisionAllocationKey;
                }
                //}
                //else
                //{

                //    foreach (DivisionAllocationDetailsModel listmodel in model.DivisionAllocationDetails.OrderBy(row => row.Name))
                //    {
                //        //listmodel.RollNumber = ++RollNumber;
                //        listmodel.RollNoCode = dbContext.Database.SqlQuery<string>("select dbo.F_GenerateRollNoCode(" + model.ClassDetailsKey + "," + (listmodel.RollNumber - 1) + ")").Single().ToString();
                //    }

                //}
                model.DivisionAllocationDetails = model.DivisionAllocationDetails.OrderBy(row => row.RollNumber).ToList();

                var attendanceDetails = dbContext.Attendances.Any(x => x.Application.StudentStatusKey == DbConstants.ApplicationStatus.OnGoing && x.ClassDetailsKey == model.ClassDetailsKey);

                #region old Query
                //var attendanceDetails = dbContext.Attendances.Any(x => x.Application.BranchKey == model.BranchKey && x.Application.CourseKey == model.CourseKey && x.Application.BatchKey == model.BatchKey && x.Application.UniversityMasterKey == model.UniversityMasterKey &&
                //  x.Application.CurrentYear == model.CourseYear && x.Application.StudentStatusKey == DbConstants.ApplicationStatus.OnGoing && x.ClassDetailsKey == model.ClassDetailsKey);
                #endregion End Query

                if (attendanceDetails == false)
                {
                    model.IfResetRollNo = true;
                }
                else
                {
                    model.IfResetRollNo = false;
                }

                model.IsSuccessful = true;

                //}

                return model;
            }
            catch (Exception Ex)
            {
                ActivityLog.CreateActivityLog(MenuConstants.GenerateRollNumber, ActionConstants.View, DbConstants.LogType.Error, null, Ex.GetBaseException().Message);
                return new DivisionAllocationViewModel();

            }
        }

        public DivisionAllocationViewModel ResetDivisionAllocation(long Id)
        {
            DivisionAllocationViewModel objviewModel = new DivisionAllocationViewModel();
            using (var transaction = dbContext.Database.BeginTransaction())
            {
                try
                {

                    StudentDivisionAllocation divisionAllocationList = dbContext.StudentDivisionAllocations.SingleOrDefault(x => x.RowKey == Id);

                    //Attendance attendanceDetails = dbContext.Attendances.SingleOrDefault(x => x.ApplicationKey == divisionAllocationList.ApplicationKey);

                    //if (attendanceDetails == null)
                    //{
                    Application applicationDetails = dbContext.Applications.SingleOrDefault(x => x.RowKey == divisionAllocationList.ApplicationKey);
                    if (applicationDetails != null)
                    {
                        //applicationDetails.DivisionKey = null;
                        applicationDetails.RollNumber = null;
                        applicationDetails.ClassDetailsKey = null;
                        applicationDetails.RollNoCode = null;

                    }
                    dbContext.StudentDivisionAllocations.Remove(divisionAllocationList);

                    dbContext.SaveChanges();
                    transaction.Commit();
                    objviewModel.Message = EduSuiteUIResources.Success;
                    objviewModel.IsSuccessful = true;
                    ActivityLog.CreateActivityLog(MenuConstants.GenerateRollNumber, ActionConstants.Delete, DbConstants.LogType.Info, Id, objviewModel.Message);
                    //}
                    //else
                    //{
                    //    transaction.Rollback();

                    //    objviewModel.Message = ApplicationResources.CantDeleteDivisionAllocation;
                    //    objviewModel.IsSuccessful = false;
                    //    ActivityLog.CreateActivityLog(MenuConstants.GenerateRollNumber, ActionConstants.Delete, DbConstants.LogType.Info, Id, objviewModel.Message);

                    //}
                }
                catch (DbUpdateException ex)
                {
                    transaction.Rollback();
                    if (ex.GetBaseException().Message.ToUpper().Contains(EduSuiteUIResources.ForeignKeyError.ToUpper()))
                    {
                        objviewModel.Message = String.Format(EduSuiteUIResources.CantDeleteErrorMessage, EduSuiteUIResources.GenerateRollNo);
                        objviewModel.IsSuccessful = false;

                        ActivityLog.CreateActivityLog(MenuConstants.GenerateRollNumber, ActionConstants.Delete, DbConstants.LogType.Debug, Id, ex.GetBaseException().Message);
                    }
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    objviewModel.Message = String.Format(EduSuiteUIResources.FailedToDeleteErrorMessage, EduSuiteUIResources.GenerateRollNo);
                    objviewModel.IsSuccessful = false;

                    ActivityLog.CreateActivityLog(MenuConstants.GenerateRollNumber, ActionConstants.Delete, DbConstants.LogType.Debug, Id, ex.GetBaseException().Message);
                }
            }
            //}
            return objviewModel;
        }

        public DivisionAllocationViewModel GetSearchDropdownList(DivisionAllocationViewModel model)
        {
            FillBranch(model);
            FillSearchBatch(model);
            return model;
        }
        public DivisionAllocationViewModel FillSearchBatch(DivisionAllocationViewModel model)
        {

            if (model.BranchKey != null && model.BranchKey != 0)
            {
                model.Batches = (from p in dbContext.Applications
                                 join SDA in dbContext.StudentDivisionAllocations on p.RowKey equals SDA.ApplicationKey
                                 join B in dbContext.VwBatchSelectActiveOnlies on p.BatchKey equals B.RowKey
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
                model.Batches = (from p in dbContext.Applications
                                 join SDA in dbContext.StudentDivisionAllocations on p.RowKey equals SDA.ApplicationKey
                                 join B in dbContext.VwBatchSelectActiveOnlies on p.BatchKey equals B.RowKey
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
    }
}
