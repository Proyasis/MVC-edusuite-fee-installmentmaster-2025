using System;
using System.Collections.Generic;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CITS.EduSuite.Business.Interfaces;
using CITS.EduSuite.Data;
using CITS.EduSuite.Business.Models.ViewModels;
using System.Linq.Expressions;
using CITS.EduSuite.Business.Common;
using CITS.EduSuite.Business.Models.Resources;

namespace CITS.EduSuite.Business.Services
{
    public class StudentsCertificateReturnService : IStudentsCertificateReturnService
    {
        private EduSuiteDatabase dbContext;
        public StudentsCertificateReturnService(EduSuiteDatabase objDB)
        {
            this.dbContext = objDB;
        }

        public StudentsCertificateReturnViewModel GetStudentsCertificateById(StudentsCertificateReturnViewModel model)
        {
            try
            {
                var StudentCertificatedetails = dbContext.EducationQualifications.Where(row => row.ApplicationKey == model.ApplicationKey && row.IsOriginalIssued);

                if (model.CertificateStatusKey == DbConstants.CertificateProcessType.Received)
                {
                    StudentCertificatedetails = StudentCertificatedetails.Where(row => row.CertificateStatusKey == DbConstants.CertificateProcessType.Returned && row.OriginalReturnDate == null);
                }
                else if (model.CertificateStatusKey == DbConstants.CertificateProcessType.Verified)
                {
                    StudentCertificatedetails = StudentCertificatedetails.Where(row => row.CertificateStatusKey == DbConstants.CertificateProcessType.Received && row.VerifiedDate == null);
                }
                else if (model.CertificateStatusKey == DbConstants.CertificateProcessType.Returned)
                {
                    StudentCertificatedetails = StudentCertificatedetails.Where(row => row.CertificateStatusKey != DbConstants.CertificateProcessType.Returned);
                }
                model.StudentCertificatedetails = StudentCertificatedetails.Select(EQ => new StudentsCertificateReturnDetail
                {
                    EducationQualificationName = EQ.EducationQualificationCourse,
                    EducationQualificationUniversity = EQ.EducationQualificationUniversity,
                    EducationQualificationKey = EQ.RowKey,
                    IsVerified = EQ.IsVerified ?? false
                }).ToList();


                return model;
            }
            catch (Exception ex)
            {
                ActivityLog.CreateActivityLog(MenuConstants.StudentsCertificateReturn, ActionConstants.View, DbConstants.LogType.Error, "", ex.GetBaseException().Message);
                return new StudentsCertificateReturnViewModel();


            }

        }
        public StudentsCertificateReturnViewModel UpdateStudentsCertificateProcess(StudentsCertificateReturnViewModel model)
        {
            using (var transaction = dbContext.Database.BeginTransaction())
            {
                try
                {
                    foreach (StudentsCertificateReturnDetail item in model.StudentCertificatedetails.Where(row => row.CertificateStatus).ToList())
                    {
                        EducationQualification EducationQualificationModel = new EducationQualification();
                        EducationQualificationModel = dbContext.EducationQualifications.SingleOrDefault(row => row.RowKey == item.EducationQualificationKey);
                        if (model.CertificateStatusKey == DbConstants.CertificateProcessType.Verified)
                        {
                            EducationQualificationModel.IsVerified = true;
                            EducationQualificationModel.VerifiedDate = model.CommonDate;
                            EducationQualificationModel.VerifiedBy = DbConstants.User.UserKey;
                        }
                        else if (model.CertificateStatusKey == DbConstants.CertificateProcessType.Returned && model.IsPermenant)
                        {
                            EducationQualificationModel.IsOriginalReturn = true;
                            EducationQualificationModel.OriginalReturnDate = model.CommonDate;
                            EducationQualificationModel.OriginalReturnBy = DbConstants.User.UserKey;
                        }
                        EducationQualificationModel.CertificateStatusKey = model.CertificateStatusKey;
                    }

                    CreateCertificateProcessLog(model.StudentCertificatedetails.Where(row => row.RowKey == 0).ToList(), model);
                    dbContext.SaveChanges();
                    transaction.Commit();
                    model.Message = EduSuiteUIResources.Success;
                    //model.AdmissionNo = dbContext.T_Application.Where(row => row.RowKey == model.ApplicationKey).Select(row => row.AdmissionNo).FirstOrDefault();
                    model.IsSuccessful = true;
                    ActivityLog.CreateActivityLog(MenuConstants.StudentsCertificateReturn, (model.StudentCertificatedetails.Any(row => row.RowKey != 0) ? ActionConstants.Edit : ActionConstants.Add), DbConstants.LogType.Info, model.ApplicationKey, model.Message);

                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    model.Message = String.Format(EduSuiteUIResources.FailedToSaveErrorMessage, EduSuiteUIResources.StudentCertificateReturn);
                    model.IsSuccessful = false;
                    ActivityLog.CreateActivityLog(MenuConstants.StudentsCertificateReturn, (model.StudentCertificatedetails.Any(row => row.RowKey != 0) ? ActionConstants.Edit : ActionConstants.Add), DbConstants.LogType.Error, model.ApplicationKey, ex.GetBaseException().Message);
                }
            }
            return model;
        }

        private void CreateCertificateProcessLog(List<StudentsCertificateReturnDetail> modelList, StudentsCertificateReturnViewModel objViewModel)
        {
            Int64 MaxKey = dbContext.StudentsCertificateReturns.Select(p => p.RowKey).DefaultIfEmpty().Max();

            foreach (StudentsCertificateReturnDetail model in modelList.Where(row => row.CertificateStatus))
            {

                StudentsCertificateReturn studentsCertificateReturnModel = new StudentsCertificateReturn();
                studentsCertificateReturnModel.RowKey = Convert.ToInt64(MaxKey + 1);
                studentsCertificateReturnModel.ApplicationKey = objViewModel.ApplicationKey;
                studentsCertificateReturnModel.EducationQualificationKey = model.EducationQualificationKey;
                studentsCertificateReturnModel.CertificateStatusKey = objViewModel.CertificateStatusKey;
                studentsCertificateReturnModel.IssuedDate = objViewModel.CommonDate;
                studentsCertificateReturnModel.IssuedBy = DbConstants.User.UserKey;
                studentsCertificateReturnModel.IsTempReturn = (objViewModel.CertificateStatusKey == DbConstants.CertificateProcessType.Returned && objViewModel.IsPermenant == false) ? true : false;

                dbContext.StudentsCertificateReturns.Add(studentsCertificateReturnModel);
                MaxKey++;

            }
        }

        public StudentsCertificateReturnViewModel DeleteStudentsCertificateProcess(StudentsCertificateReturnDetail model)
        {
            StudentsCertificateReturnViewModel objViewModel = new StudentsCertificateReturnViewModel();
            using (var transaction = dbContext.Database.BeginTransaction())
            {
                try
                {

                    StudentsCertificateReturn StudentsCertificateReturn = dbContext.StudentsCertificateReturns.SingleOrDefault(row => row.RowKey == model.RowKey);
                    List<StudentsCertificateReturn> StudentsCertificateReturnList = dbContext.StudentsCertificateReturns.Where(row => row.EducationQualificationKey == StudentsCertificateReturn.EducationQualificationKey).ToList();
                    EducationQualification EducationQualifications = dbContext.EducationQualifications.SingleOrDefault(row => row.RowKey == StudentsCertificateReturn.EducationQualificationKey);
                    if (StudentsCertificateReturn.CertificateStatusKey == DbConstants.CertificateProcessType.Verified)
                    {
                        EducationQualifications.IsVerified = false;
                        EducationQualifications.VerifiedBy = null;
                        EducationQualifications.VerifiedDate = null;

                    }
                    //else if (StudentsCertificateReturn.CertificateStatusKey == DbConstants.CertificateProcessType.Returned && !dbContext.StudentsCertificateReturns.Any(row => row.EducationQualificationKey == StudentsCertificateReturn.EducationQualificationKey && row.CertificateStatusKey == DbConstants.CertificateProcessType.Received && row.RowKey != model.RowKey))
                    else if (StudentsCertificateReturn.CertificateStatusKey == DbConstants.CertificateProcessType.Returned && EducationQualifications.IsOriginalReturn == true)
                    {
                        EducationQualifications.IsOriginalReturn = false;
                        EducationQualifications.OriginalReturnBy = null;
                        EducationQualifications.OriginalReturnDate = null;

                    }
                    var CertificateStatusKey = StudentsCertificateReturnList.Where(x => x.RowKey != model.RowKey).OrderByDescending(row => row.RowKey).Select(y => y.CertificateStatusKey).FirstOrDefault();

                    EducationQualifications.CertificateStatusKey = CertificateStatusKey;

                    dbContext.SaveChanges();
                    dbContext.StudentsCertificateReturns.Remove(StudentsCertificateReturn);

                    dbContext.SaveChanges();
                    transaction.Commit();
                    objViewModel.Message = EduSuiteUIResources.Success;
                    objViewModel.IsSuccessful = true;
                    ActivityLog.CreateActivityLog(MenuConstants.StudentsCertificateReturn, ActionConstants.Delete, DbConstants.LogType.Info, model.RowKey, objViewModel.Message);

                }
                catch (DbUpdateException ex)
                {
                    transaction.Rollback();
                    if (ex.GetBaseException().Message.ToUpper().Contains(EduSuiteUIResources.ForeignKeyError.ToUpper()))
                    {
                        objViewModel.Message = String.Format(EduSuiteUIResources.CantDeleteErrorMessage, EduSuiteUIResources.StudentCertificateReturn);
                        objViewModel.IsSuccessful = false;
                        ActivityLog.CreateActivityLog(MenuConstants.StudentsCertificateReturn, ActionConstants.Delete, DbConstants.LogType.Error, model.RowKey, ex.GetBaseException().Message);
                    }
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    objViewModel.Message = String.Format(EduSuiteUIResources.FailedToDeleteErrorMessage, EduSuiteUIResources.StudentCertificateReturn);
                    objViewModel.IsSuccessful = false;
                    ActivityLog.CreateActivityLog(MenuConstants.StudentsCertificateReturn, ActionConstants.Delete, DbConstants.LogType.Error, model.RowKey, ex.GetBaseException().Message);
                }
            }
            return objViewModel;
        }

        public StudentsCertificateReturnViewModel ResetCertificateVerify(long EducationQualificationKey)
        {
            StudentsCertificateReturnViewModel model = new StudentsCertificateReturnViewModel();

            using (var transaction = dbContext.Database.BeginTransaction())
            {
                try
                {

                    EducationQualification educationQualification = dbContext.EducationQualifications.SingleOrDefault(row => row.RowKey == EducationQualificationKey);
                    educationQualification.IsVerified = false;
                    educationQualification.VerifiedBy = null;
                    educationQualification.VerifiedDate = null;
                    dbContext.SaveChanges();
                    transaction.Commit();
                    model.Message = EduSuiteUIResources.Success;
                    model.IsSuccessful = true;
                    ActivityLog.CreateActivityLog(MenuConstants.StudentsCertificateReturn, ActionConstants.Delete, DbConstants.LogType.Info, EducationQualificationKey, model.Message);

                }
                catch (DbUpdateException ex)
                {
                    transaction.Rollback();
                    if (ex.GetBaseException().Message.ToUpper().Contains(EduSuiteUIResources.ForeignKeyError.ToUpper()))
                    {
                        model.Message = String.Format(EduSuiteUIResources.CantDeleteErrorMessage, EduSuiteUIResources.StudentCertificateReturn);
                        model.IsSuccessful = false;
                        ActivityLog.CreateActivityLog(MenuConstants.StudentsCertificateReturn, ActionConstants.Delete, DbConstants.LogType.Error, EducationQualificationKey, ex.GetBaseException().Message);
                    }
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    model.Message = String.Format(EduSuiteUIResources.FailedToDeleteErrorMessage, EduSuiteUIResources.StudentCertificateReturn);
                    model.IsSuccessful = false;
                    ActivityLog.CreateActivityLog(MenuConstants.StudentsCertificateReturn, ActionConstants.Delete, DbConstants.LogType.Error, EducationQualificationKey, ex.GetBaseException().Message);
                }
            }
            return model;
        }
        public StudentsCertificateReturnViewModel ResetCertificateReturn(long EducationQualificationKey)
        {
            StudentsCertificateReturnViewModel model = new StudentsCertificateReturnViewModel();

            using (var transaction = dbContext.Database.BeginTransaction())
            {
                try
                {

                    EducationQualification educationQualification = dbContext.EducationQualifications.SingleOrDefault(row => row.RowKey == EducationQualificationKey);
                    educationQualification.IsVerified = false;
                    educationQualification.VerifiedBy = null;
                    educationQualification.VerifiedDate = null;

                    dbContext.SaveChanges();
                    transaction.Commit();
                    model.Message = EduSuiteUIResources.Success;
                    model.IsSuccessful = true;
                    ActivityLog.CreateActivityLog(MenuConstants.StudentsCertificateReturn, ActionConstants.Delete, DbConstants.LogType.Info, EducationQualificationKey, model.Message);

                }
                catch (DbUpdateException ex)
                {
                    transaction.Rollback();
                    if (ex.GetBaseException().Message.ToUpper().Contains(EduSuiteUIResources.ForeignKeyError.ToUpper()))
                    {
                        model.Message = String.Format(EduSuiteUIResources.CantDeleteErrorMessage, EduSuiteUIResources.StudentCertificateReturn);
                        model.IsSuccessful = false;
                        ActivityLog.CreateActivityLog(MenuConstants.StudentsCertificateReturn, ActionConstants.Delete, DbConstants.LogType.Error, EducationQualificationKey, ex.GetBaseException().Message);
                    }
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    model.Message = String.Format(EduSuiteUIResources.FailedToDeleteErrorMessage, EduSuiteUIResources.StudentCertificateReturn);
                    model.IsSuccessful = false;
                    ActivityLog.CreateActivityLog(MenuConstants.StudentsCertificateReturn, ActionConstants.Delete, DbConstants.LogType.Error, EducationQualificationKey, ex.GetBaseException().Message);
                }
            }
            return model;
        }
        public List<ApplicationViewModel> GetApplications(ApplicationViewModel model, out long TotalRecords)
        {
            try
            {
                var Take = model.PageSize;
                var Skip = (model.PageIndex - 1) * model.PageSize;

                IQueryable<ApplicationViewModel> applicationList = (from a in dbContext.Applications
                                                                    where (a.StudentName.Contains(model.ApplicantName)) && (a.StudentMobile.Contains(model.MobileNumber)) && (a.EducationQualifications.Any(x => x.IsOriginalIssued))
                                                                    select new ApplicationViewModel
                                                                    {
                                                                        RowKey = a.RowKey,
                                                                        AdmissionNo = a.AdmissionNo,
                                                                        ApplicantName = a.StudentName,

                                                                        CourseName = a.Course.CourseName,
                                                                        UniversityName = a.UniversityMaster.UniversityMasterName,
                                                                        MobileNumber = a.StudentMobile,
                                                                        ApplicationStatusName = a.StudentStatu.StudentStatusName,
                                                                        BatchName = a.Batch.BatchName,
                                                                        BranchKey = a.BranchKey,
                                                                        BatchKey = a.BatchKey,
                                                                        CourseKey = a.CourseKey,
                                                                        UniversityKey = a.UniversityMasterKey,
                                                                        BranchName = a.Branch.BranchName,
                                                                        AcademicTermKey = a.AcademicTermKey,
                                                                        CurrentYear = a.CurrentYear,
                                                                        CourseDuration = a.Course.CourseDuration ?? 0,
                                                                        NoOfCertificate = a.EducationQualifications.Count(x => x.IsOriginalIssued),
                                                                        NoOfVerified = a.EducationQualifications.Count(x => x.IsVerified ?? false),
                                                                        NoOfReturned = a.EducationQualifications.Count(x => x.IsOriginalReturn ?? false),
                                                                        AvailableCertificates = a.EducationQualifications.Count(x => x.IsOriginalIssued && x.CertificateStatusKey != DbConstants.CertificateProcessType.Returned),

                                                                        RecievePending = a.EducationQualifications.Count(x => x.CertificateStatusKey == DbConstants.CertificateProcessType.Returned && x.OriginalReturnDate == null)
                                                                    });


                Employee Employee = dbContext.Employees.Where(x => x.AppUserKey == DbConstants.User.UserKey).SingleOrDefault();
                if (Employee != null)
                {
                    if (Employee.BranchAccess != null)
                    {
                        var Branches = Employee.BranchAccess.Split(',').Select(Int16.Parse).ToList();
                        applicationList = applicationList.Where(row => Branches.Contains(row.BranchKey ?? 0));
                    }
                }
                if (model.BranchKey != 0)
                {
                    applicationList = applicationList.Where(row => row.BranchKey == model.BranchKey);
                }
                if (model.BatchKey != 0)
                {
                    applicationList = applicationList.Where(row => row.BatchKey == model.BatchKey);
                }
                if (model.CourseKey != 0)
                {
                    applicationList = applicationList.Where(row => row.CourseKey == model.CourseKey);
                }
                if (model.UniversityKey != 0)
                {
                    applicationList = applicationList.Where(row => row.UniversityKey == model.UniversityKey);
                }

                applicationList = applicationList.GroupBy(x => x.RowKey).Select(y => y.FirstOrDefault());
                if (model.SortBy != "")
                {
                    applicationList = SortApplications(applicationList, model.SortBy, model.SortOrder);
                }
                TotalRecords = applicationList.Count();
                return applicationList.Skip(Skip).Take(Take).ToList<ApplicationViewModel>();
            }
            catch (Exception ex)
            {
                TotalRecords = 0;
                ActivityLog.CreateActivityLog(MenuConstants.StudentsCertificateReturn, ActionConstants.View, DbConstants.LogType.Error, null, ex.GetBaseException().Message);
                return new List<ApplicationViewModel>();

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

        public List<StudentsCertificateReturnDetail> GetCertificateDetailsByApplication(long ApplicationKey)
        {

            return (from scr in dbContext.StudentsCertificateReturns.Where(x => x.ApplicationKey == ApplicationKey && x.EducationQualification.IsOriginalIssued)
                    select new StudentsCertificateReturnDetail
                    {
                        EducationQualificationName = scr.EducationQualification.EducationQualificationCourse,
                        EducationQualificationUniversity = scr.EducationQualification.EducationQualificationUniversity,
                        EducationQualificationKey = scr.EducationQualification.RowKey,
                        RowKey = scr.RowKey,
                        CertificateStatusName = (scr.CertificateStatusKey == DbConstants.CertificateProcessType.Received ? EduSuiteUIResources.Recieved : (scr.CertificateStatusKey == DbConstants.CertificateProcessType.Returned && scr.IsTempReturn == true ? EduSuiteUIResources.TempReturned : (scr.CertificateStatusKey == DbConstants.CertificateProcessType.Returned && scr.EducationQualification.IsOriginalReturn == true ? EduSuiteUIResources.Returned : EduSuiteUIResources.Verified))),
                        CertificateStatusBy = dbContext.AppUsers.Where(x => x.RowKey == scr.IssuedBy).Select(y => y.AppUserName).FirstOrDefault(),
                        IssuedDate = scr.IssuedDate,
                        LastKey = dbContext.StudentsCertificateReturns.Where(x => x.ApplicationKey == ApplicationKey && x.EducationQualificationKey == scr.EducationQualification.RowKey).OrderByDescending(p => p.RowKey).Select(row => row.RowKey).FirstOrDefault(),
                        ListCount = dbContext.StudentsCertificateReturns.Where(x => x.ApplicationKey == ApplicationKey && x.EducationQualificationKey == scr.EducationQualification.RowKey).ToList().Count()

                    }).ToList();
        }

    }
}
