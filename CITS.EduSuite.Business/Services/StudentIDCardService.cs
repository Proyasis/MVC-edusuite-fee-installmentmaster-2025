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
using CITS.EduSuite.Business.Models.Resources;

namespace CITS.EduSuite.Business.Services
{
    public class StudentIDCardService : IStudentIDCardService
    {
        private EduSuiteDatabase dbContext;

        public StudentIDCardService(EduSuiteDatabase objDB)
        {
            this.dbContext = objDB;
        }
        public List<StudentIDCardList> GetStudentIDCards(StudentIDCardViewModels model)
        {

            var Take = model.PageSize;
            var skip = (model.PageIndex - 1) * model.PageSize;

            IQueryable<StudentIDCardList> StudentIDCardListQuery = (from Application in dbContext.Applications.Where(x => x.StudentStatusKey == DbConstants.StudentStatus.Ongoing).OrderByDescending(row => new { row.RowKey })
                                                                    join StudentIDCard in dbContext.StudentIDCards on Application.RowKey equals StudentIDCard.ApplicationKey
                                                                    into StudentIDCardTable
                                                                    from StudentIDCard in StudentIDCardTable.DefaultIfEmpty()
                                                                    where ((model.SearchName ?? "") == "" || Application.StudentName.Contains((model.SearchName).Trim()))
                                                                     && ((model.SearchAdmissionNo ?? "") == "" || (Application.AdmissionNo).Contains((model.SearchAdmissionNo).Trim()))
                                                                    select new StudentIDCardList
                                                                    {

                                                                        StudentIDCardRowKey = StudentIDCard.RowKey,
                                                                        AdmissionNo = Application.AdmissionNo.ToUpper(),
                                                                        ApplicationKey = Application.RowKey,
                                                                        StudentName = Application.StudentName,
                                                                        StudentAddress = Application.StudentPresentAddress,
                                                                        StudentMobile = Application.StudentMobile,
                                                                        StudentEmail = Application.StudentEmail,
                                                                        StudentPhotoPath = (Application.OldStudentPhotoPath != null && Application.OldStudentPhotoPath != "") ? Application.OldStudentPhotoPath : UrlConstants.ApplicationUrl + Application.RowKey + "/" + Application.StudentPhotoPath,
                                                                        StudentPhoto = Application.StudentPhotoPath,
                                                                        CourseName = Application.Course.CourseName,
                                                                        UniversityName = Application.UniversityMaster.UniversityMasterName,
                                                                        StudentEnrollmentNo = Application.StudentEnrollmentNo,
                                                                        StudentDateOfAdmission = Application.StudentDateOfAdmission,
                                                                        IsReceived = StudentIDCard.IsReceived ?? false,
                                                                        IsIssued = StudentIDCard.IsIssued ?? false,
                                                                        CourseTypeKey = Application.Course.CourseTypeKey,
                                                                        CourseKey = Application.CourseKey,
                                                                        UniversityKey = Application.UniversityMasterKey,
                                                                        BatchKey = Application.BatchKey,
                                                                        BatchName = Application.Batch.BatchName,
                                                                        BranchName = Application.Branch.BranchName,
                                                                        BranchAddress = Application.Branch.AddressLine1,
                                                                        AcademicTermKey = Application.AcademicTermKey,
                                                                        CurrentYear = Application.CurrentYear,
                                                                        CourseDuration = Application.Course.CourseDuration,
                                                                        BranchKey = Application.BranchKey,
                                                                        BloodGroup = Application.BloodGroup.BloodGroupName,
                                                                        Religion = Application.Religion.ReligionName,
                                                                        Caste = Application.Caste.CasteName,
                                                                        DateOfBirth = Application.StudentDOB
                                                                    });
            Employee Employee = dbContext.Employees.Where(x => x.AppUserKey == DbConstants.User.UserKey).SingleOrDefault();

            if (Employee != null)
            {
                if (Employee.BranchAccess != null)
                {
                    var Branches = Employee.BranchAccess.Split(',').Select(Int16.Parse).ToList();
                    StudentIDCardListQuery = StudentIDCardListQuery.Where(row => Branches.Contains(row.BranchKey ?? 0));
                }
            }
            if (model.SearchBranchKey != null && model.SearchBranchKey != 0)
                StudentIDCardListQuery = StudentIDCardListQuery.Where(x => x.BranchKey == model.SearchBranchKey);

            if (model.SearchCourseTypeKey != null && model.SearchCourseTypeKey != 0)
                StudentIDCardListQuery = StudentIDCardListQuery.Where(x => x.CourseTypeKey == model.SearchCourseTypeKey);


            if (model.SearchCourseKey != null && model.SearchCourseKey != 0)
                StudentIDCardListQuery = StudentIDCardListQuery.Where(x => x.CourseKey == model.SearchCourseKey);

            if (model.SearchUniversityKey != null && model.SearchUniversityKey != 0)
                StudentIDCardListQuery = StudentIDCardListQuery.Where(x => x.UniversityKey == model.SearchUniversityKey);

            if (model.SearchBatchKey != null && model.SearchBatchKey != 0)
                StudentIDCardListQuery = StudentIDCardListQuery.Where(x => x.BatchKey == model.SearchBatchKey);

            model.TotalRecords = StudentIDCardListQuery.Count();

            return StudentIDCardListQuery.OrderByDescending(row => row.StudentDateOfAdmission).Skip(skip).Take(Take).ToList();
        }

        public StudentIDCardViewModels CreateStudentIDCard(StudentIDCardViewModels model)
        {
            using (var transaction = dbContext.Database.BeginTransaction())
            {
                try
                {
                    CreateStudentIDCardList(model);
                    UpdateStudentIDCardList(model);
                    UpdateStudentEnrollmentNo(model);
                    FillDrodownLists(model);
                    model.UpdateDate = null;
                    dbContext.SaveChanges();
                    transaction.Commit();
                    model.IsSuccessful = true;
                    model.Message = EduSuiteUIResources.Success;

                    ActivityLog.CreateActivityLog(MenuConstants.StudentIDCard, ActionConstants.AddEdit, DbConstants.LogType.Info, null, model.Message);
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    model.Message = String.Format(EduSuiteUIResources.FailedToSaveErrorMessage, EduSuiteUIResources.StudentIdCard);
                    model.IsSuccessful = false;

                    ActivityLog.CreateActivityLog(MenuConstants.StudentIDCard, ActionConstants.AddEdit, DbConstants.LogType.Error, null, ex.GetBaseException().Message);
                }
            }

            return model;
        }
        private void CreateStudentIDCardList(StudentIDCardViewModels model)
        {
            List<StudentIDCardList> IDCardList = model.StudentIDCardList.Where(x => x.IsReceived == true && x.StudentIDCardRowKey == null).ToList();
            Int64 maxKey = dbContext.StudentIDCards.Select(p => p.RowKey).DefaultIfEmpty().Max();
            foreach (StudentIDCardList List in IDCardList)
            {
                StudentIDCard StudentIDCard = new StudentIDCard();
                maxKey = maxKey + 1;
                StudentIDCard.RowKey = Convert.ToInt64(maxKey);
                StudentIDCard.ApplicationKey = List.ApplicationKey;
                StudentIDCard.ReceivedBy = DbConstants.User.UserKey;
                StudentIDCard.ReceivedDate = model.UpdateDate;
                StudentIDCard.IsReceived = List.IsReceived;
                dbContext.StudentIDCards.Add(StudentIDCard);
            }
        }
        private void UpdateStudentIDCardList(StudentIDCardViewModels model)
        {
            List<StudentIDCardList> IDCardList = model.StudentIDCardList.Where(x => x.IsIssued == true && x.StudentIDCardRowKey > 0).ToList();
            foreach (StudentIDCardList List in IDCardList)
            {
                StudentIDCard StudentIDCard = dbContext.StudentIDCards.Where(x => x.RowKey == List.StudentIDCardRowKey).SingleOrDefault();
                StudentIDCard.IssuedBy = DbConstants.User.UserKey;
                StudentIDCard.IssuedDate = model.UpdateDate;
                StudentIDCard.IsIssued = List.IsIssued;
            }
        }
        private void UpdateStudentEnrollmentNo(StudentIDCardViewModels model)
        {
            List<StudentIDCardList> IDCardList = model.StudentIDCardList.Where(x => x.IsReceived == true && x.IsIssued == false && x.StudentIDCardRowKey == null).ToList();
            foreach (StudentIDCardList List in IDCardList)
            {
                Application Application = dbContext.Applications.Where(x => x.RowKey == List.ApplicationKey).SingleOrDefault();
                Application.StudentEnrollmentNo = List.StudentEnrollmentNo;
            }
        }
        public StudentIDCardViewModels ResetStudentIDCardList(StudentIDCardViewModels model)
        {

            using (var transaction = dbContext.Database.BeginTransaction())
            {
                try
                {
                    StudentIDCard StudentIDCard = dbContext.StudentIDCards.SingleOrDefault(row => row.RowKey == model.RowKey);
                    dbContext.StudentIDCards.Remove(StudentIDCard);
                    dbContext.SaveChanges();
                    transaction.Commit();
                    model.Message = EduSuiteUIResources.Success;
                    model.IsSuccessful = true;

                    ActivityLog.CreateActivityLog(MenuConstants.StudentIDCard, ActionConstants.Delete, DbConstants.LogType.Info, model.RowKey, model.Message);

                }
                catch (DbUpdateException ex)
                {
                    transaction.Rollback();
                    if (ex.GetBaseException().Message.ToUpper().Contains(EduSuiteUIResources.ForeignKeyError.ToUpper()))
                    {
                        model.Message = String.Format(EduSuiteUIResources.CantDeleteErrorMessage, EduSuiteUIResources.StudentIdCard);
                        model.IsSuccessful = false;

                        ActivityLog.CreateActivityLog(MenuConstants.StudentIDCard, ActionConstants.Delete, DbConstants.LogType.Error, model.RowKey, ex.GetBaseException().Message);
                    }
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    model.Message = String.Format(EduSuiteUIResources.FailedToDeleteErrorMessage, EduSuiteUIResources.StudentIdCard);
                    model.IsSuccessful = false;

                    ActivityLog.CreateActivityLog(MenuConstants.StudentIDCard, ActionConstants.Delete, DbConstants.LogType.Error, model.RowKey, ex.GetBaseException().Message);
                }
            }
            //}
            return model;

        }
        public void FillDrodownLists(StudentIDCardViewModels model)
        {
            FillBranch(model);
            GetCourseType(model);
            GetBatches(model);
            FillNotificationDetail(model);
        }

        /*DROPDOWN BINDINGS*/

        private void FillBranch(StudentIDCardViewModels model)
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
                    model.SearchBranchKey = Employee.BranchKey;
                }
                else
                {
                    model.Branches = BranchQuery.Where(x => x.RowKey == Employee.BranchKey).ToList();
                    model.SearchBranchKey = Employee.BranchKey;
                }
            }
            else
            {
                model.Branches = BranchQuery.ToList();
            }

            if (model.Branches.Count == 1)
            {
                long? branchkey = model.Branches.Select(x => x.RowKey).FirstOrDefault();
                model.SearchBranchKey = Convert.ToInt16(branchkey);
            }
        }
        public StudentIDCardViewModels GetCourseType(StudentIDCardViewModels model)
        {
            model.CourseTypes = dbContext.UniversityCourses.Select(row => new SelectListModel
            {
                RowKey = row.Course.CourseType.RowKey,
                Text = row.Course.CourseType.CourseTypeName

            }).GroupBy(row => row.RowKey).Select(row => row.FirstOrDefault()).ToList();
            return model;
        }
        public StudentIDCardViewModels GetCourseByCourseType(StudentIDCardViewModels model)
        {

            model.Courses = dbContext.UniversityCourses.Where(row => row.Course.CourseTypeKey == model.SearchCourseTypeKey).Select(row => new SelectListModel
            {
                RowKey = row.Course.RowKey,
                Text = row.Course.CourseName
            }).GroupBy(row => row.RowKey).Select(row => row.FirstOrDefault()).ToList();


            return model;
        }
        public StudentIDCardViewModels GetUniversityByCourse(StudentIDCardViewModels model)
        {
            model.Universities = dbContext.UniversityCourses.Where(row => row.CourseKey == model.SearchCourseKey).Select(row => new SelectListModel
            {
                RowKey = row.UniversityMaster.RowKey,
                Text = row.UniversityMaster.UniversityMasterName
            }).GroupBy(row => row.RowKey).Select(row => row.FirstOrDefault()).ToList();

            return model;
        }
        public StudentIDCardViewModels GetBatches(StudentIDCardViewModels model)
        {

            model.Batches = dbContext.Batches.Select(row => new SelectListModel
            {
                RowKey = row.RowKey,
                Text = row.BatchName
            }).ToList();

            return model;
        }

        private void FillNotificationDetail(StudentIDCardViewModels model)
        {
            NotificationTemplate notificationTemplateModel = dbContext.NotificationTemplates.SingleOrDefault(row => row.RowKey == DbConstants.NotificationTemplate.StudentIDCard);
            if (notificationTemplateModel != null)
            {
                model.AutoEmail = notificationTemplateModel.AutoEmail;
                model.AutoSMS = notificationTemplateModel.AutoSMS;
                model.TemplateKey = notificationTemplateModel.RowKey;
            }
        }
    }
}
