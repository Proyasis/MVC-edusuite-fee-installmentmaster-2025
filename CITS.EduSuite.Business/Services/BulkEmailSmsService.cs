using System;
using System.Collections.Generic;
using System.Data.Entity.Infrastructure;
using System.Data.Entity.SqlServer;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using CITS.EduSuite.Business.Common;
using CITS.EduSuite.Business.Interfaces;
using CITS.EduSuite.Business.Models.ViewModels;
using CITS.EduSuite.Data;
using System.Data.Entity.Core.Objects;
using CITS.EduSuite.Business.Models.Resources;
using System.Data.Common;
using CITS.EduSuite.Business.Extensions;

namespace CITS.EduSuite.Business.Services
{
    public class BulkEmailSmsService : IBulkEmailSmsService
    {

        private EduSuiteDatabase dbContext;
        public BulkEmailSmsService(EduSuiteDatabase objDB)
        {
            this.dbContext = objDB;
        }

        public BulkEmailSmsViewModel GetBulkEmailSms(BulkEmailSmsViewModel model)
        {
            try
            {
                Employee Employee = dbContext.Employees.Where(x => x.AppUserKey == DbConstants.User.UserKey).SingleOrDefault();
                string EmployeeAccessList = "";
                if (Employee != null)
                {

                    if (!DbConstants.Role.AdminUserTypes.Contains(DbConstants.User.RoleKey))
                    {

                        var ChildEmployees = dbContext.fnChildEmployees(DbConstants.User.UserKey).Where(row => (row.BranchKey == model.BranchKey || model.BranchKey == 0)).Select(row => new GroupSelectListModel
                        {
                            RowKey = row.RowKey ?? 0,
                            Text = row.EmployeeName,

                        }).OrderBy(row => row.Text).ToList();

                        List<long> branches = Employee.BranchAccess.ToString().Split(',').Select(Int64.Parse).ToList();

                        var OtherEmployees = dbContext.Employees.Where(row => branches.Contains(row.BranchKey) && row.BranchKey != Employee.BranchKey).Select(row => new GroupSelectListModel
                        {
                            RowKey = row.RowKey,
                            Text = row.FirstName + " " + (row.MiddleName ?? "") + " " + row.LastName,

                        }).OrderBy(row => row.Text).ToList();

                        EmployeeAccessList = String.Join(",", OtherEmployees.Select(x => x.RowKey).ToList().Union(ChildEmployees.Select(x => x.RowKey)));
                    }

                }

                //ObjectParameter TotalRecordsCount = new ObjectParameter("TotalRecordCount", typeof(Int64));
                //model.BulkEmailSMSList = dbContext.SpSelectBulkEmailSms
                //    (
                //    DbConstants.User.UserKey,
                //    DbConstants.User.RoleKey,
                //    model.SearchUserTypeKey,
                //    model.SearchText.VerifyData(),
                //    model.SearchApplicationStatusKey,
                //    String.Join(",", model.CourseKeys),
                //    String.Join(",", model.CourseTypeKeys),
                //    String.Join(",", model.UniversityMasterKeys),
                //    String.Join(",", model.BatchKeys),
                //    String.Join(",", model.BranchKeys),
                //    model.AcademicTermKey,
                //    String.Join(",", model.CourseYearsKeys),
                //    model.PageIndex,
                //    model.PageSize,
                //    TotalRecordsCount,
                //    "",
                //    "").Select(
                //                 row => new BulkEmailSmsViewModel
                //                 {
                //                     RowKey = row.RowKey,
                //                     Name = row.Name,
                //                     MobileNumber = row.MobileNumber,
                //                     EmailAddress = row.EmailAddress,
                //                     RoleKey = row.RoleKey ?? 0
                //                 }
                //            ).ToList();
                //model.TotalRecords = TotalRecords.Value != DBNull.Value ? Convert.ToInt64(TotalRecords.Value) : 0;

                long TotalRecords = 0;
                List<BulkEmailSmsViewModel> BulkEmailSMSList = new List<BulkEmailSmsViewModel>();
                DbParameter TotalRecordsParam = null;

                if (model.SortBy != "")
                {
                    model.SortBy = model.SortBy + " " + model.SortOrder;
                }
                dbContext.LoadStoredProc("dbo.SpSelectBulkEmailSms")
                    .WithSqlParam("@UserKey", DbConstants.User.UserKey)
                    .WithSqlParam("@RoleKey", DbConstants.User.RoleKey)
                    .WithSqlParam("@SearchUserTypeKey", model.SearchUserTypeKey)
                    .WithSqlParam("@SearchText", model.SearchText.VerifyData())
                    .WithSqlParam("@SearchApplicationStatusKey", model.SearchApplicationStatusKey)
                    .WithSqlParam("@CourseKeyList", String.Join(",", model.CourseKeys))
                    .WithSqlParam("@CourseTypeKeyList", String.Join(",", model.CourseTypeKeys))
                    .WithSqlParam("@UniversityMasterKeyList", String.Join(",", model.UniversityMasterKeys))
                    .WithSqlParam("@BatchKeyList", String.Join(",", model.BatchKeys))
                    .WithSqlParam("@BranchKeyList", String.Join(",", model.BranchKeys))
                    .WithSqlParam("@AcademicTermKey", model.AcademicTermKey)
                    .WithSqlParam("@CourseYearsKeyList", String.Join(",", model.CourseYearsKeys))
                    .WithSqlParam("@PageIndex", model.PageIndex)
                    .WithSqlParam("@PageSize", model.PageSize)
                    .WithSqlParam("@SortBy", "")
                    .WithSqlParam("@SortOrder", "")
                    .WithSqlParam("@TotalRecordCount", (dbParam) =>
                    {
                        dbParam.Direction = System.Data.ParameterDirection.Output;
                        dbParam.DbType = System.Data.DbType.Int64;
                        TotalRecordsParam = dbParam;
                    })

    .ExecuteStoredProc((handler) =>
    {
        //applicationList = handler.ReadToDynamicList<dynamic>() as List<dynamic>;
        BulkEmailSMSList = handler.ReadToList<BulkEmailSmsViewModel>() as List<BulkEmailSmsViewModel>;
    });
                model.TotalRecords = Convert.ToInt64((TotalRecordsParam.Value ?? 0));
                model.BulkEmailSMSList = BulkEmailSMSList;
            }
            catch (Exception ex)
            {

                ActivityLog.CreateActivityLog(MenuConstants.BulkEmailSMS, ActionConstants.View, DbConstants.LogType.Error, null, ex.GetBaseException().Message);
                return model = new BulkEmailSmsViewModel();


            }

            return model;

        }
        public SendBulkEmailViewModel CreateBulkEmailTrack(SendBulkEmailViewModel model)
        {
            using (var transaction = dbContext.Database.BeginTransaction())
            {
                try
                {
                    long MaxKey = dbContext.BulkEmailTracks.Select(p => p.RowKey).DefaultIfEmpty().Max();
                    BulkEmailTrack DbBulkEmailTrack = new BulkEmailTrack();
                    DbBulkEmailTrack.RowKey = (MaxKey + 1);
                    DbBulkEmailTrack.EmailSubject = model.Subject;
                    DbBulkEmailTrack.EmailContentFileName = MaxKey.ToString() + ".html";
                    dbContext.BulkEmailTracks.Add(DbBulkEmailTrack);
                    dbContext.SaveChanges();
                    model.BulkEmailTrackKey = DbBulkEmailTrack.RowKey;
                    model.EmailFileName = DbBulkEmailTrack.EmailContentFileName;
                    CreateBulkEmailTrackDetails(model);

                    dbContext.SaveChanges();
                    transaction.Commit();
                    model.Message = EduSuiteUIResources.Success;
                    model.IsSuccessful = true;
                    ActivityLog.CreateActivityLog(MenuConstants.BulkEmailSMS, ActionConstants.Add, DbConstants.LogType.Info, DbBulkEmailTrack.RowKey, model.Message);

                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    model.Message = ex.GetBaseException().Message;
                    model.Message = String.Format(EduSuiteUIResources.FailedToSaveErrorMessage, EduSuiteUIResources.BulkEmailDetails);
                    model.IsSuccessful = false;
                    ActivityLog.CreateActivityLog(MenuConstants.BulkEmailSMS, ActionConstants.Add, DbConstants.LogType.Error, model.BulkEmailTrackKey, model.Message);

                }
            }
            return model;
        }
        public SendBulkEmailViewModel CreateBulkEmailTrackDetails(SendBulkEmailViewModel model)
        {

            long MaxKey = dbContext.BulkEmailTrackDetails.Select(p => p.RowKey).DefaultIfEmpty().Max();
            foreach (var item in model.BulkEmailList)
            {
                BulkEmailTrackDetail DbBulkEmailTrackDetails = new BulkEmailTrackDetail();
                DbBulkEmailTrackDetails.RowKey = (MaxKey + 1);
                DbBulkEmailTrackDetails.ToEmail = item.EmailAddress;
                DbBulkEmailTrackDetails.UserRowKey = item.RowKey;
                DbBulkEmailTrackDetails.UserRowKey = item.RoleKey;
                DbBulkEmailTrackDetails.BulkEmailTrackKey = model.BulkEmailTrackKey;
                dbContext.BulkEmailTrackDetails.Add(DbBulkEmailTrackDetails);
                MaxKey++;
            }

            return model;
        }
        public SendBulkSMSViewModel CreateBulkSMSTrack(SendBulkSMSViewModel model)
        {
            using (var transaction = dbContext.Database.BeginTransaction())
            {
                try
                {
                    long MaxKey = dbContext.BulkSMSTracks.Select(p => p.RowKey).DefaultIfEmpty().Max();
                    BulkSMSTrack DbBulkSMSTrack = new BulkSMSTrack();
                    DbBulkSMSTrack.RowKey = (MaxKey + 1);
                    DbBulkSMSTrack.SMSContent = model.SMSContent;
                    dbContext.BulkSMSTracks.Add(DbBulkSMSTrack);
                    dbContext.SaveChanges();
                    model.BulkSMSTrackKey = DbBulkSMSTrack.RowKey;
                    CreateBulkSMSTrackDetails(model);
                    dbContext.SaveChanges();
                    transaction.Commit();
                    model.Message = EduSuiteUIResources.Success;
                    model.IsSuccessful = true;
                    ActivityLog.CreateActivityLog(MenuConstants.BulkEmailSMS, ActionConstants.Add, DbConstants.LogType.Info, DbBulkSMSTrack.RowKey, model.Message);

                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    model.Message = ex.GetBaseException().Message;
                    model.Message = String.Format(EduSuiteUIResources.FailedToSaveErrorMessage, EduSuiteUIResources.BulkSMSDetails);
                    model.IsSuccessful = false;
                    ActivityLog.CreateActivityLog(MenuConstants.BulkEmailSMS, ActionConstants.Add, DbConstants.LogType.Error, model.BulkSMSTrackKey, model.Message);

                }
            }
            return model;
        }
        public SendBulkSMSViewModel CreateBulkSMSTrackDetails(SendBulkSMSViewModel model)
        {

            long MaxKey = dbContext.BulkSMSTrackDetails.Select(p => p.RowKey).DefaultIfEmpty().Max();
            foreach (var item in model.BulkSMSList)
            {
                BulkSMSTrackDetail DbBulkSMSTrackDetails = new BulkSMSTrackDetail();
                DbBulkSMSTrackDetails.RowKey = (MaxKey + 1);
                DbBulkSMSTrackDetails.ToMobileNumber = item.MobileNumber;
                DbBulkSMSTrackDetails.UserRowKey = item.RowKey;
                DbBulkSMSTrackDetails.UserRowKey = item.RoleKey;
                DbBulkSMSTrackDetails.BulkSMSTrackKey = model.BulkSMSTrackKey;
                dbContext.BulkSMSTrackDetails.Add(DbBulkSMSTrackDetails);
                MaxKey++;
            }

            return model;
        }


        #region Searching
        public BulkEmailSmsViewModel GetSearchDropdownList(BulkEmailSmsViewModel model)
        {
            GetUserTypes(model);
            GetApplicationStatus(model);

            FillAcademicTerms(model);
            FillCourseTypes(model);
            FillCourse(model);
            FillUniversityMasters(model);
            FillBatches(model);
            FillBranches(model);
            return model;
        }

        private void FillBranches(BulkEmailSmsViewModel model)
        {

            IQueryable<SelectListModel> BranchQuery = dbContext.vwBranchSelectActiveOnlies.OrderBy(row => row.DisplayOrder).Select(row => new SelectListModel
            {
                RowKey = row.RowKey,
                Text = row.BranchName
            });

            Employee Employee = dbContext.Employees.Where(x => x.AppUserKey == DbConstants.User.UserKey).FirstOrDefault();
            if (Employee != null)
            {
                if (Employee.BranchAccess != null)
                {
                    List<long> Branches = Employee.BranchAccess.Split(',').Select(Int64.Parse).ToList();
                    model.Branches = BranchQuery.Where(row => Branches.Contains(row.RowKey)).ToList();
                    model.BranchKeys.Add(Employee.BranchKey);
                }
                else
                {
                    model.Branches = BranchQuery.Where(x => x.RowKey == Employee.BranchKey).ToList();
                    model.BranchKeys.Add(Employee.BranchKey);
                }
            }
            else
            {
                model.Branches = BranchQuery.ToList();
            }

            if (model.Branches.Count == 1)
            {
                long? branchkey = model.Branches.Select(x => x.RowKey).FirstOrDefault();

                model.BranchKeys.Add(branchkey ?? 0);
            }


        }

        public void GetUserTypes(BulkEmailSmsViewModel model)
        {
            List<short> RoleList = new List<short>();
            RoleList.Add(DbConstants.Role.Staff);
            RoleList.Add(DbConstants.Role.Students);

            model.UserTypes = dbContext.Roles.Where(x => x.IsActive && RoleList.Contains(x.RowKey)).Select(row => new SelectListModel
            {
                RowKey = row.RowKey,
                Text = row.RoleName
            }).GroupBy(row => row.RowKey).Select(row => row.FirstOrDefault()).ToList();

        }

        private void GetApplicationStatus(BulkEmailSmsViewModel model)
        {
            model.ApplicationStatuses = dbContext.StudentStatus.OrderBy(x => x.RowKey).Where(row => row.IsActive == true).Select(row => new SelectListModel
            {
                RowKey = row.RowKey,
                Text = row.StudentStatusName
            }).ToList();
        }



        #endregion



        public void FillAcademicTerms(BulkEmailSmsViewModel model)
        {
            model.AcademicTerms = dbContext.VwAcadamicTermSelectActiveOnlies.Select(row => new SelectListModel
            {
                RowKey = row.RowKey,
                Text = row.AcademicTermName
            }).ToList();
        }

        public void FillCourseTypes(BulkEmailSmsViewModel model)
        {

            if (model.AcademicTermKey != null)
            {
                model.CourseTypes = dbContext.UniversityCourses.Where(x => (x.AcademicTermKey == model.AcademicTermKey) && (x.Course.CourseType.IsActive == true) && x.IsActive == true).Select(row => new SelectListModel
                {
                    RowKey = row.Course.CourseType.RowKey,
                    Text = row.Course.CourseType.CourseTypeName

                }).GroupBy(row => row.RowKey).Select(row => row.FirstOrDefault()).ToList();
            }
            else
            {
                model.CourseTypes = dbContext.VwCourseTypeSelectActiveOnlies.Select(row => new SelectListModel
                {
                    RowKey = row.RowKey,
                    Text = row.CourseTypeName
                }).ToList();
            }
        }
        public void FillCourse(BulkEmailSmsViewModel model)
        {

            var Courses = dbContext.UniversityCourses.Select(row => new UniversityCourseViewModel
            {
                CourseKey = row.CourseKey,
                CourseName = row.Course.CourseName,
                CourseTypeKey = row.Course.CourseTypeKey
            });
            if (model.CourseTypeKeys.Count > 0)
            {
                Courses = Courses.Where(x => model.CourseTypeKeys.Contains(x.CourseTypeKey));
            }
            model.Courses = Courses.Select(row => new SelectListModel
            {
                RowKey = row.CourseKey,
                Text = row.CourseName
            }).GroupBy(row => row.RowKey).Select(row => row.FirstOrDefault()).ToList();

        }


        public void FillUniversityMasters(BulkEmailSmsViewModel model)
        {


            var UniversityMasters = dbContext.UniversityCourses.Select(row => new UniversityCourseViewModel
            {
                CourseKey = row.CourseKey,
                UniversityName = row.UniversityMaster.UniversityMasterName,
                UniversityMasterKey = row.UniversityMasterKey
            });

            if (model.CourseKeys.Count > 0)
            {
                UniversityMasters = UniversityMasters.Where(x => model.CourseKeys.Contains(x.CourseKey));
            }
            model.UniversityMasters = UniversityMasters.Select(row => new SelectListModel
            {
                RowKey = row.UniversityMasterKey,
                Text = row.UniversityName
            }).GroupBy(row => row.RowKey).Select(row => row.FirstOrDefault()).ToList();

        }


        public void FillYears(BulkEmailSmsViewModel model)
        {


            model.CourseYears = dbContext.fnStudentYear(model.AcademicTermKey).Select(x => new SelectListModel
            {
                RowKey = x.RowKey ?? 0,
                Text = x.YearName
            }).ToList();

        }

        private void FillBatches(BulkEmailSmsViewModel model)
        {
            model.Batches = dbContext.VwBatchSelectActiveOnlies.Select(row => new SelectListModel
            {
                RowKey = row.RowKey,
                Text = row.BatchName
            }).ToList();
        }
    }
}
