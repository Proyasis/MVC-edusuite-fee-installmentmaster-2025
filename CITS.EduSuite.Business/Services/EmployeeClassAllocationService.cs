using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CITS.EduSuite.Business.Common;
using CITS.EduSuite.Business.Interfaces;
//using CITS.EduSuite.Business.Models.Common;
using CITS.EduSuite.Business.Models.ViewModels;
using CITS.EduSuite.Data;
using System.Data.Entity.Infrastructure;
using CITS.EduSuite.Business.Models.Resources;

namespace CITS.EduSuite.Business.Services
{
    public class EmployeeClassAllocationService : IEmployeeClassAllocationService
    {
        private EduSuiteDatabase dbcontext;

        public EmployeeClassAllocationService(EduSuiteDatabase objdb)
        {
            this.dbcontext = objdb;
        }

        public EmployeeClassAllocationViewModel GetEmployeeClassAllocationById(EmployeeClassAllocationViewModel model)
        {
            try
            {
                EmployeeClassAllocationViewModel objViewModel = new EmployeeClassAllocationViewModel();

                objViewModel = dbcontext.TeacherClassAllocations.Where(y => y.RowKey == model.RowKey).Select(row => new EmployeeClassAllocationViewModel
                {
                    RowKey = row.RowKey,
                    ClassDetailsKey = row.ClassDetailsKey ?? 0,
                    BatchKey = row.BatchKey,
                    EmployeesKey = row.EmployeeKey,
                    InCharge = row.InCharge,
                    IsAttendance = row.IsAttendance,
                    IsActive = row.IsActive,
                    SubjectKeys = row.TeacherSubjectAllocations.Select(y => y.SubjectKey).ToList(),
                }).FirstOrDefault();

                if (objViewModel == null)
                {
                    objViewModel = new EmployeeClassAllocationViewModel();
                    objViewModel.RowKey = model.RowKey;
                    objViewModel.IsActive = true;
                    objViewModel.EmployeesKey = model.EmployeesKey;
                }

                FillDropdownlists(objViewModel);
                return objViewModel;
            }

            catch (Exception Ex)
            {
                ActivityLog.CreateActivityLog(MenuConstants.EmployeeClassAllocation, ActionConstants.View, DbConstants.LogType.Error, model.EmployeesKey, Ex.GetBaseException().Message);
                return new EmployeeClassAllocationViewModel();
            }
        }

        public EmployeeClassAllocationViewModel UpdateEmployeeClassAllocation(EmployeeClassAllocationViewModel model)
        {
           var TeacherClassAllocationsCheck = 0;
            if (model.InCharge == true)
            {
                TeacherClassAllocationsCheck = dbcontext.TeacherClassAllocations.Where(row => row.ClassDetailsKey == model.ClassDetailsKey && row.BatchKey == model.BatchKey && row.InCharge == true && row.RowKey != model.RowKey).Count();
            }
            else
            {
                TeacherClassAllocationsCheck = dbcontext.TeacherClassAllocations.Where(row => row.EmployeeKey == model.EmployeesKey && row.ClassDetailsKey == model.ClassDetailsKey && row.BatchKey == model.BatchKey && row.RowKey != model.RowKey).Count();
            }
            if (TeacherClassAllocationsCheck != 0)
            {
                model.Message = String.Format(EduSuiteUIResources.ExistsErrorMessage, EduSuiteUIResources.ClassAllocation);
                model.IsSuccessful = false;
                return model;
            }
            using (var transaction = dbcontext.Database.BeginTransaction())
            {
                try
                {
                    TeacherClassAllocation objTeacherClassAllocationmodel = new TeacherClassAllocation();

                    objTeacherClassAllocationmodel = dbcontext.TeacherClassAllocations.Where(x => x.RowKey == model.RowKey).FirstOrDefault();
                    objTeacherClassAllocationmodel.EmployeeKey = model.EmployeesKey;
                    objTeacherClassAllocationmodel.ClassDetailsKey = model.ClassDetailsKey;
                    objTeacherClassAllocationmodel.InCharge = model.InCharge;
                    objTeacherClassAllocationmodel.IsAttendance = model.IsAttendance;
                    objTeacherClassAllocationmodel.IsActive = model.IsActive;
                    objTeacherClassAllocationmodel.BatchKey = model.BatchKey;

                    UpdateSubjectAllocation(model);

                    dbcontext.SaveChanges();
                    transaction.Commit();

                    model.Message = EduSuiteUIResources.Success; /*Pending val*/
                    model.IsSuccessful = true;
                    ActivityLog.CreateActivityLog(MenuConstants.EmployeeClassAllocation, ActionConstants.Edit, DbConstants.LogType.Info, model.EmployeesKey, model.Message);
                }
                catch (Exception Ex)
                {
                    transaction.Rollback();
                    model.Message = String.Format(EduSuiteUIResources.FailedToSaveErrorMessage, EduSuiteUIResources.Employee + EduSuiteUIResources.BlankSpace + EduSuiteUIResources.ClassAllocation);
                    model.IsSuccessful = false;
                    ActivityLog.CreateActivityLog(MenuConstants.EmployeeClassAllocation, ActionConstants.Edit, DbConstants.LogType.Error, model.EmployeesKey, Ex.GetBaseException().Message);
                }
            }
            return model;
        }

        public EmployeeClassAllocationViewModel CreateEmployeeClassAllocation(EmployeeClassAllocationViewModel model)
        {
            var TeacherClassAllocationsCheck = 0;
            if (model.InCharge == true)
            {
                TeacherClassAllocationsCheck = dbcontext.TeacherClassAllocations.Where(row => row.ClassDetailsKey == model.ClassDetailsKey && row.BatchKey == model.BatchKey && row.InCharge == true).Count();
            }
            else
            {
                TeacherClassAllocationsCheck = dbcontext.TeacherClassAllocations.Where(row => row.EmployeeKey == model.EmployeesKey && row.ClassDetailsKey == model.ClassDetailsKey && row.BatchKey == model.BatchKey).Count();
            }
            if (TeacherClassAllocationsCheck != 0)
            {
                model.Message = String.Format(EduSuiteUIResources.ExistsErrorMessage, EduSuiteUIResources.ClassAllocation);
                model.IsSuccessful = false;
                return model;
            }
            using (var transaction = dbcontext.Database.BeginTransaction())
            {
                try
                {
                    Int64 MaxKey = dbcontext.TeacherClassAllocations.Select(x => x.RowKey).DefaultIfEmpty().Max();

                    TeacherClassAllocation objTeacherClassAllocationmodel = new TeacherClassAllocation();

                    objTeacherClassAllocationmodel.RowKey = MaxKey + 1;
                    objTeacherClassAllocationmodel.EmployeeKey = model.EmployeesKey;
                    objTeacherClassAllocationmodel.ClassDetailsKey = model.ClassDetailsKey;
                    objTeacherClassAllocationmodel.InCharge = model.InCharge;
                    objTeacherClassAllocationmodel.IsAttendance = model.IsAttendance;
                    objTeacherClassAllocationmodel.IsActive = model.IsActive;
                    objTeacherClassAllocationmodel.BatchKey = model.BatchKey;
                    dbcontext.TeacherClassAllocations.Add(objTeacherClassAllocationmodel);

                    model.RowKey = objTeacherClassAllocationmodel.RowKey;
                    UpdateSubjectAllocation(model);

                    dbcontext.SaveChanges();
                    transaction.Commit();

                    model.Message = EduSuiteUIResources.Success; /*Pending val*/
                    model.IsSuccessful = true;
                    ActivityLog.CreateActivityLog(MenuConstants.EmployeeClassAllocation, ActionConstants.Edit, DbConstants.LogType.Info, model.EmployeesKey, model.Message);
                }
                catch (Exception Ex)
                {
                    transaction.Rollback();
                    model.Message = String.Format(EduSuiteUIResources.FailedToSaveErrorMessage, EduSuiteUIResources.Employee + EduSuiteUIResources.BlankSpace + EduSuiteUIResources.ClassAllocation);
                    model.IsSuccessful = false;
                    ActivityLog.CreateActivityLog(MenuConstants.EmployeeClassAllocation, ActionConstants.Edit, DbConstants.LogType.Error, model.EmployeesKey, Ex.GetBaseException().Message);
                }
            }
            return model;
        }

        private void UpdateSubjectAllocation(EmployeeClassAllocationViewModel objviewmodel)
        {
            Int64 MaxKey = dbcontext.TeacherSubjectAllocations.Select(x => x.RowKey).DefaultIfEmpty().Max();
            List<TeacherSubjectAllocation> teacherSubjectAllocation = dbcontext.TeacherSubjectAllocations.Where(x => x.TeacherClassAllocationKey == objviewmodel.RowKey).ToList();

            if (teacherSubjectAllocation.Count > 0)
            {
                dbcontext.TeacherSubjectAllocations.RemoveRange(teacherSubjectAllocation);
            }
            if (objviewmodel.SubjectKeys != null && objviewmodel.SubjectKeys.Count > 0)
            {
                foreach (long SubjectKey in objviewmodel.SubjectKeys)
                {
                    TeacherSubjectAllocation objTeacherSubjectAllocationmodel = new TeacherSubjectAllocation();

                    objTeacherSubjectAllocationmodel.RowKey = MaxKey + 1;
                    objTeacherSubjectAllocationmodel.Employeekey = objviewmodel.EmployeesKey;
                    objTeacherSubjectAllocationmodel.SubjectKey = SubjectKey;
                    objTeacherSubjectAllocationmodel.IsActive = true;
                    objTeacherSubjectAllocationmodel.ClassDetailsKey = objviewmodel.ClassDetailsKey;
                    objTeacherSubjectAllocationmodel.TeacherClassAllocationKey = objviewmodel.RowKey;

                    dbcontext.TeacherSubjectAllocations.Add(objTeacherSubjectAllocationmodel);
                    dbcontext.SaveChanges();
                    MaxKey++;
                }
            }
        }

        private void FillClassDetails(EmployeeClassAllocationViewModel model)
        {
            model.ClassDetails = dbcontext.VwClassDetailsSelectActiveOnlies.Select(x => new SelectListModel
            {
                RowKey = x.RowKey,
                Text = x.ClassCode + x.ClassCodeDescription
            }).ToList();
        }
        public EmployeeClassAllocationViewModel FillBatchDetails(EmployeeClassAllocationViewModel model)
        {
            //var BranchKey = dbcontext.Employees.Where(x => x.RowKey == model.EmployeeKey).Select(y => y.BranchKey).SingleOrDefault();
            //model.Batches = (from p in dbcontext.Applications
            //                 join SDA in dbcontext.StudentDivisionAllocations on p.RowKey equals SDA.ApplicationKey
            //                 join B in dbcontext.VwBatchSelectActiveOnlies on p.BatchKey equals B.RowKey
            //                 orderby B.RowKey
            //                 //where (p.CourseKey == model.CourseKey && p.BranchKey == model.BranchKey && p.UniversityMasterKey == model.UniversityMasterKey)
            //                 where (p.ClassDetailsKey == model.ClassDetailsKey && p.BranchKey == BranchKey)
            //                 select new SelectListModel
            //                 {
            //                     RowKey = B.RowKey,
            //                     Text = B.BatchName
            //                 }).Distinct().ToList();

            model.Batches = (from B in dbcontext.VwBatchSelectActiveOnlies
                             orderby B.RowKey
                             select new SelectListModel
                             {
                                 RowKey = B.RowKey,
                                 Text = B.BatchName
                             }).Distinct().ToList();
            return model;
        }

        private void FillDropdownlists(EmployeeClassAllocationViewModel model)
        {
            FillClassDetails(model);
            FillBatchDetails(model);
            GetSubjectByClassDetailskey(model);
        }

        public EmployeeClassAllocationViewModel GetSubjectByClassDetailskey(EmployeeClassAllocationViewModel model)
        {
            //model.SubjectDetails = (from S in dbcontext.VwSubjectSelectActiveOnlies
            //                        join UC in dbcontext.UniversityCourses on new { S.CourseKey, S.UniversityMasterKey, S.AcademicTermKey } equals new { UC.CourseKey, UC.UniversityMasterKey, UC.AcademicTermKey }
            //                        join CD in dbcontext.ClassDetails on new { UniversityCourseKey = UC.RowKey, StudentYear = S.CourseYear } equals new { CD.UniversityCourseKey, CD.StudentYear }
            //                        join TSA in dbcontext.TeacherSubjectAllocations.Where(x => x.Employeekey == model.EmployeeKey) on B.RowKey equals TSA.BookKey into TSAC
            //                        from TSA in TSAC.DefaultIfEmpty()
            //                        where (CD.RowKey == model.ClassDetailsKey)
            //                        select new SelectListModel
            //                        {
            //                            RowKey = S.RowKey,
            //                            Text = S.SubjectName,

            //                        }).ToList();

            ClassDetail classDetailList = dbcontext.ClassDetails.SingleOrDefault(x => x.RowKey == model.ClassDetailsKey);
            if (classDetailList != null)
            {
                model.SubjectDetails = dbcontext.VwSubjectSelectActiveOnlies.Where(x => x.CourseKey == classDetailList.UniversityCourse.CourseKey && x.UniversityMasterKey == classDetailList.UniversityCourse.UniversityMasterKey && x.AcademicTermKey == classDetailList.UniversityCourse.AcademicTermKey && x.CourseYear == classDetailList.StudentYear).Select(x => new SelectListModel
                {
                    RowKey = x.RowKey,
                    Text = x.SubjectName
                }).ToList();
            }
            else
            {
                model.SubjectDetails = dbcontext.VwSubjectSelectActiveOnlies.Select(x => new SelectListModel
                {
                    RowKey = x.RowKey,
                    Text = x.SubjectName
                }).ToList();
            }
            return model;
        }

        public EmployeeClassAllocationViewModel DeleteEmployeeClassAllocation(long Rowkey)
        {
            EmployeeClassAllocationViewModel model = new EmployeeClassAllocationViewModel();
            using (var transaction = dbcontext.Database.BeginTransaction())
            {
                try
                {
                    TeacherClassAllocation teacherClassAllocation = dbcontext.TeacherClassAllocations.SingleOrDefault(x => x.RowKey == Rowkey);


                    //var CheckClassAllocation = dbcontext.TeacherClassAllocations.Any(x => x.ClassDetailsKey == teacherClassAllocation.ClassDetailsKey &&
                    //                         x.EmployeeKey == teacherClassAllocation.EmployeeKey && x.RowKey != Rowkey);

                    //if (!CheckClassAllocation)
                    //{
                    // List<TeacherSubjectAllocation> teacherSubjects = dbcontext.TeacherSubjectAllocations.Where(x => x.Subject.CourseSubjectDetails.Select(y => y.CourseSubjectMaster.UniversityMasterKey).FirstOrDefault() == teacherClassAllocation.ClassDetail.UniversityCourse.UniversityMasterKey &&
                    //  x.Subject.CourseSubjectDetails.Select(y => y.CourseSubjectMaster.CourseKey).FirstOrDefault() == teacherClassAllocation.ClassDetail.UniversityCourse.CourseKey &&
                    // x.Subject.CourseSubjectDetails.Select(y => y.CourseSubjectMaster.AcademicTermKey).FirstOrDefault() == teacherClassAllocation.ClassDetail.UniversityCourse.AcademicTermKey && x.Subject.CourseSubjectDetails.Select(y => y.CourseSubjectMaster.CourseYear).FirstOrDefault() == teacherClassAllocation.ClassDetail.StudentYear
                    // && x.Employeekey == teacherClassAllocation.EmployeeKey).ToList();

                    List<TeacherSubjectModule> teacherSubjectModules = dbcontext.TeacherSubjectModules.Where(x => x.Employeekey == teacherClassAllocation.EmployeeKey && x.TeacherClassAllocationKey == Rowkey).ToList();

                    List<TeacherSubjectAllocation> teacherSubjects = dbcontext.TeacherSubjectAllocations.Where(x => x.Employeekey == teacherClassAllocation.EmployeeKey && x.TeacherClassAllocationKey == Rowkey).ToList();

                    dbcontext.TeacherSubjectModules.RemoveRange(teacherSubjectModules);
                    dbcontext.TeacherSubjectAllocations.RemoveRange(teacherSubjects);

                    //}
                    dbcontext.TeacherClassAllocations.Remove(teacherClassAllocation);

                    dbcontext.SaveChanges();
                    transaction.Commit();
                    model.Message = EduSuiteUIResources.Success;
                    model.IsSuccessful = true;
                    ActivityLog.CreateActivityLog(MenuConstants.EmployeeClassAllocation, ActionConstants.Delete, DbConstants.LogType.Info, Rowkey, model.Message);



                }
                catch (DbUpdateException ex)
                {
                    transaction.Rollback();
                    if (ex.GetBaseException().Message.ToUpper().Contains(EduSuiteUIResources.ForeignKeyError.ToUpper()))
                    {
                        model.Message = String.Format(EduSuiteUIResources.CantDeleteErrorMessage, EduSuiteUIResources.Employee + EduSuiteUIResources.BlankSpace + EduSuiteUIResources.ClassAllocation);
                        model.IsSuccessful = false;
                        ActivityLog.CreateActivityLog(MenuConstants.EmployeeClassAllocation, ActionConstants.Delete, DbConstants.LogType.Error, Rowkey, ex.GetBaseException().Message);
                    }
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    model.Message = String.Format(EduSuiteUIResources.FailedToDeleteErrorMessage, EduSuiteUIResources.Employee + EduSuiteUIResources.BlankSpace + EduSuiteUIResources.ClassAllocation);
                    model.IsSuccessful = false;
                    ActivityLog.CreateActivityLog(MenuConstants.EmployeeClassAllocation, ActionConstants.Delete, DbConstants.LogType.Error, Rowkey, ex.GetBaseException().Message);
                }

            }
            return model;
        }

        public EmployeeClassAllocationViewModel CheckInCharge(long EmployeeKey, long ClassDetailsKey, short BatchKey)
        {
            EmployeeClassAllocationViewModel model = new EmployeeClassAllocationViewModel();

            var InCharge = dbcontext.TeacherClassAllocations.Any(x => x.EmployeeKey != EmployeeKey && x.ClassDetailsKey == ClassDetailsKey && x.BatchKey == BatchKey && x.InCharge == true);
            if (InCharge == true)
            {
                model.Message = EduSuiteUIResources.CheckInChargeMessage;
                model.IsSuccessful = false;
            }
            else
            {
                model.IsSuccessful = true;
            }
            return model;
        }

        public List<EmployeeClassAllocationViewModel> GetEmployeeClassDetails(long EmployeeKey)
        {
            try
            {
                var TeacherClassAllocationsList = (from row in dbcontext.TeacherClassAllocations.Where(row => row.EmployeeKey == EmployeeKey)
                                                   orderby row.RowKey ascending
                                                   select new EmployeeClassAllocationViewModel
                                                   {
                                                       RowKey = row.RowKey,
                                                       ClassDetailsKey = row.ClassDetailsKey ?? 0,
                                                       ClassDetailsName = row.ClassDetail.ClassCode,
                                                       BatchKey = row.BatchKey,
                                                       BatchName = row.Batch.BatchName,
                                                       EmployeesKey = row.EmployeeKey,
                                                       InCharge = row.InCharge,
                                                       IsAttendance = row.IsAttendance,
                                                       IsActive = row.IsActive,
                                                       SubjectKeys = row.TeacherSubjectAllocations.Select(y => y.SubjectKey).ToList(),
                                                       SubjectNames = row.TeacherSubjectAllocations.Select(y => y.Subject.SubjectName).ToList(),
                                                   }).ToList();

                return TeacherClassAllocationsList.ToList<EmployeeClassAllocationViewModel>();

            }
            catch (Exception ex)
            {
                ActivityLog.CreateActivityLog(MenuConstants.StudentDiary, ActionConstants.View, DbConstants.LogType.Debug, EmployeeKey, ex.GetBaseException().Message);
                return new List<EmployeeClassAllocationViewModel>();
            }
        }
    }
}
