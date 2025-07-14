using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using CITS.EduSuite.Business.Extensions;
using CITS.EduSuite.Business.Interfaces;
using CITS.EduSuite.Business.Models.Resources;
using CITS.EduSuite.Business.Models.ViewModels;
using CITS.EduSuite.Data;


namespace CITS.EduSuite.Business.Services
{
    public class CourseTransferService : ICourseTransferService
    {

        private EduSuiteDatabase dbContext;

        public CourseTransferService(EduSuiteDatabase objEduSuiteDatabase)
        {
            this.dbContext = objEduSuiteDatabase;
        }
        public List<dynamic> GetApplications(ApplicationViewModel model, out long TotalRecords)
        {
            try
            {
                List<dynamic> applicationList = new List<dynamic>();
                DbParameter TotalRecordsParam = null;

                if (model.SortBy != "")
                {
                    model.SortBy = model.SortBy + " " + model.SortOrder;
                }
                dbContext.LoadStoredProc("dbo.SP_StudentCourseTransferDetails")
                    .WithSqlParam("@BranchKey", model.BranchKey)
                    .WithSqlParam("@BatchKey", model.BatchKey)
                    .WithSqlParam("@CourseKey", model.CourseKey)
                    .WithSqlParam("@UniversityKey", model.UniversityKey)
                    .WithSqlParam("@SearchText", model.ApplicantName.VerifyData())
                    .WithSqlParam("@SearchMobileNumber", model.MobileNumber.VerifyData())
                    .WithSqlParam("@PageIndex", model.PageIndex)
                    .WithSqlParam("@PageSize", model.PageSize)
                    .WithSqlParam("@SortBy", model.SortBy)
                    .WithSqlParam("@UserKey", DbConstants.User.UserKey)
                    .WithSqlParam("@TotalRecords", (dbParam) =>
                    {
                        dbParam.Direction = System.Data.ParameterDirection.Output;
                        dbParam.DbType = System.Data.DbType.Int64;
                        TotalRecordsParam = dbParam;
                    }).ExecuteStoredProc((handler) =>
                    {
                        applicationList = handler.ReadToDynamicList<dynamic>() as List<dynamic>;
                        //applicationList = handler.ReadToList<ApplicationViewModel>() as List<ApplicationViewModel>;
                    });
                TotalRecords = Convert.ToInt64((TotalRecordsParam.Value ?? 0));
                return applicationList;
            }
            catch (Exception ex)
            {
                TotalRecords = 0;
                ActivityLog.CreateActivityLog(MenuConstants.CourseTransfer, ActionConstants.View, DbConstants.LogType.Error, null, ex.GetBaseException().Message);
                return new List<dynamic>();
            }
        }
        public CourseTransferViewModel GetCourseTransferById(CourseTransferViewModel model)
        {
            try
            {
                CourseTransferViewModel objmodel = new CourseTransferViewModel();
                Application Applications = dbContext.Applications.Where(x => x.RowKey == model.ApplicationKey).FirstOrDefault();
                objmodel = dbContext.StudentCourseTransfers.Where(x => x.RowKey == model.RowKey).Select(row => new CourseTransferViewModel
                {
                    RowKey = row.RowKey,
                    ApplicationKey = row.ApplicationKey ?? 0,
                    FromAcademicTermKey = row.FromAcademicTermKey,
                    FromCourseKey = row.FromCourseKey,
                    FromUniverisityKey = row.FromUniverisityKey,
                    ToAcademicTermKey = row.ToAcademicTermKey,
                    ToCourseKey = row.ToCourseKey ?? 0,
                    ToUniverisityKey = row.ToUniverisityKey ?? 0,
                    IsActive = row.IsActive ?? false,
                    Remarks = row.Remarks,

                }).FirstOrDefault();

                if (objmodel == null)
                {
                    objmodel = new CourseTransferViewModel();
                    objmodel.FromAcademicTermKey = Applications.AcademicTermKey;
                    objmodel.FromCourseKey = Applications.CourseKey;
                    objmodel.FromUniverisityKey = Applications.UniversityMasterKey;

                    GeneralConfiguration GNC = dbContext.GeneralConfigurations.FirstOrDefault();
                    if (GNC.AllowChangeAdmissionNo)
                    {
                        AdmissionNoConfig objAdmissionNoConfig = dbContext.AdmissionNoConfigs.FirstOrDefault();
                        bool isuniversity = dbContext.VwUniversityMasterSelectActiveOnlies.Count() != 1;
                        if (objAdmissionNoConfig.IsUniversity && isuniversity)
                        {
                            objmodel.IsChangeAdmissionNo = true;
                        }
                        else
                        {
                            objmodel.IsChangeAdmissionNo = false;
                        }
                    }
                    else
                    {
                        objmodel.IsChangeAdmissionNo = false;
                    }
                }
                FillDropDownList(objmodel);
                objmodel.ApplicationKey = model.ApplicationKey;

                objmodel.StudentMobile = Applications != null ? Applications.StudentMobile : null;
                objmodel.StudentEmail = Applications != null ? Applications.StudentEmail : null;

                FillNotificationDetail(objmodel);
                return objmodel;

            }
            catch (Exception ex)
            {
                ActivityLog.CreateActivityLog(MenuConstants.CourseTransfer, ActionConstants.View, DbConstants.LogType.Error, model.RowKey, ex.GetBaseException().Message);
                return new CourseTransferViewModel();
            }
        }
        public CourseTransferViewModel CreateCourseTransfer(CourseTransferViewModel model)
        {

            if (model.FromAcademicTermKey == model.ToAcademicTermKey && model.FromCourseKey == model.ToCourseKey && model.FromUniverisityKey == model.ToUniverisityKey)
            {
                model.Message = EduSuiteUIResources.CourseTransferExist;
                model.IsSuccessful = false;
                ActivityLog.CreateActivityLog(MenuConstants.CourseTransfer, ActionConstants.AddEdit, DbConstants.LogType.Error, null, model.Message);
                return model;
            }
            using (var transaction = dbContext.Database.BeginTransaction())
            {
                try
                {

                    long Maxkey = dbContext.StudentCourseTransfers.Select(x => x.RowKey).DefaultIfEmpty().Max();

                    StudentCourseTransfer CourseTransfer = new StudentCourseTransfer();

                    CourseTransfer.RowKey = ++Maxkey;
                    CourseTransfer.FromAcademicTermKey = model.FromAcademicTermKey;
                    CourseTransfer.FromCourseKey = model.FromCourseKey;
                    CourseTransfer.FromUniverisityKey = model.FromUniverisityKey;
                    CourseTransfer.ToAcademicTermKey = model.ToAcademicTermKey;
                    CourseTransfer.ToCourseKey = model.ToCourseKey;
                    CourseTransfer.ToUniverisityKey = model.ToUniverisityKey;
                    CourseTransfer.Remarks = model.Remarks;
                    CourseTransfer.IsActive = true;
                    CourseTransfer.ApplicationKey = model.ApplicationKey;
                    CourseTransfer.IsChangeAdmissionNo = model.IsChangeAdmissionNo;

                    dbContext.StudentCourseTransfers.Add(CourseTransfer);

                    Application Applications = dbContext.Applications.Where(x => x.RowKey == model.ApplicationKey).FirstOrDefault();

                    Applications.AcademicTermKey = model.ToAcademicTermKey ?? 0;
                    Applications.CourseKey = model.ToCourseKey ?? 0;
                    Applications.UniversityMasterKey = model.ToUniverisityKey ?? 0;
                    Applications.HasConcession = false;
                    Applications.RollNoCode = "";
                    Applications.RollNumber = null;
                    Applications.ClassDetailsKey = null;
                    GeneralConfiguration GNC = dbContext.GeneralConfigurations.FirstOrDefault();
                    if (GNC.AllowChangeAdmissionNo)
                    {
                        if (model.IsChangeAdmissionNo)
                        {
                            Applications.AdmissionNo = dbContext.Database.SqlQuery<string>("select dbo.F_GenerateAdmissionNo(" + Applications.BranchKey + "," + Applications.BatchKey + "," + model.ToUniverisityKey + ")").Single().ToString();
                            Applications.SerialNumber = dbContext.Database.SqlQuery<int>("select dbo.F_GenerateSerialNo(" + Applications.BranchKey + "," + Applications.BatchKey + "," + model.ToUniverisityKey + ")").Single();
                        }
                    }

                    StudentDivisionAllocation objStudentDivisionAllocation = dbContext.StudentDivisionAllocations.Where(x => x.ApplicationKey == model.ApplicationKey && x.IsActive == true).FirstOrDefault();
                    if (objStudentDivisionAllocation !=null)
                    {
                        dbContext.StudentDivisionAllocations.Remove(objStudentDivisionAllocation);
                    }

                    model.RowKey = CourseTransfer.RowKey;

                    CreateTransferAdmissionFee(model);
                    dbContext.SaveChanges();
                    transaction.Commit();
                    model.Message = EduSuiteUIResources.Success;
                    model.IsSuccessful = true;

                    ActivityLog.CreateActivityLog(MenuConstants.CourseTransfer, ActionConstants.Add, DbConstants.LogType.Info, model.ApplicationKey, model.Message);

                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    model.Message = String.Format(EduSuiteUIResources.FailedToSaveErrorMessage, EduSuiteUIResources.CourseTransfer);
                    model.IsSuccessful = false;
                    ActivityLog.CreateActivityLog(MenuConstants.CourseTransfer, ActionConstants.Add, DbConstants.LogType.Error, model.ApplicationKey, ex.GetBaseException().Message);

                }
                return model;
            }
        }
        public CourseTransferViewModel UpdateCourseTransfer(CourseTransferViewModel model)
        {
            using (var transaction = dbContext.Database.BeginTransaction())
            {
                try
                {


                    StudentCourseTransfer CourseTransfer = new StudentCourseTransfer();
                    CourseTransfer = dbContext.StudentCourseTransfers.Where(x => x.RowKey == model.RowKey).FirstOrDefault();

                    CourseTransfer.FromAcademicTermKey = model.FromAcademicTermKey;
                    CourseTransfer.FromCourseKey = model.FromCourseKey;
                    CourseTransfer.FromUniverisityKey = model.FromUniverisityKey;
                    CourseTransfer.ToAcademicTermKey = model.ToAcademicTermKey;
                    CourseTransfer.ToCourseKey = model.ToCourseKey;
                    CourseTransfer.ToUniverisityKey = model.ToUniverisityKey;
                    CourseTransfer.Remarks = model.Remarks;
                    CourseTransfer.IsActive = true;
                    CourseTransfer.ApplicationKey = model.ApplicationKey;

                    Application Applications = dbContext.Applications.Where(x => x.RowKey == model.ApplicationKey).FirstOrDefault();

                    Applications.AcademicTermKey = model.ToAcademicTermKey ?? 0;
                    Applications.CourseKey = model.ToCourseKey ?? 0;
                    Applications.UniversityMasterKey = model.ToUniverisityKey ?? 0;
                    Applications.HasConcession = false;
                    if (model.IsChangeAdmissionNo)
                    {
                        Applications.AdmissionNo = dbContext.Database.SqlQuery<string>("select dbo.F_GenerateAdmissionNo(" + Applications.BranchKey + "," + Applications.BatchKey + "," + model.ToUniverisityKey + ")").Single().ToString();
                        Applications.SerialNumber = dbContext.Database.SqlQuery<int>("select dbo.F_GenerateSerialNo(" + Applications.BranchKey + "," + Applications.BatchKey + "," + model.ToUniverisityKey + ")").Single();
                    }

                    dbContext.SaveChanges();
                    transaction.Commit();
                    model.Message = EduSuiteUIResources.Success;
                    model.IsSuccessful = true;

                    ActivityLog.CreateActivityLog(MenuConstants.CourseTransfer, ActionConstants.Add, DbConstants.LogType.Info, model.ApplicationKey, model.Message);

                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    model.Message = String.Format(EduSuiteUIResources.FailedToSaveErrorMessage, EduSuiteUIResources.CourseTransfer);
                    model.IsSuccessful = false;
                    ActivityLog.CreateActivityLog(MenuConstants.CourseTransfer, ActionConstants.Add, DbConstants.LogType.Error, model.ApplicationKey, ex.GetBaseException().Message);

                }
                return model;
            }
        }
        public CourseTransferViewModel DeleteCourseTransfer(long RowKey)
        {
            CourseTransferViewModel model = new CourseTransferViewModel();
            using (var Transaction = dbContext.Database.BeginTransaction())
            {
                try
                {


                    StudentCourseTransfer CourseTransfer = dbContext.StudentCourseTransfers.Where(x => x.RowKey == RowKey).FirstOrDefault();
                    dbContext.StudentCourseTransfers.Remove(CourseTransfer);

                    dbContext.SaveChanges();
                    Transaction.Commit();
                    model.Message = EduSuiteUIResources.Success;
                    model.IsSuccessful = true;
                    ActivityLog.CreateActivityLog(MenuConstants.CourseTransfer, ActionConstants.Delete, DbConstants.LogType.Info, RowKey, model.Message);

                }
                catch (DbUpdateException ex)
                {
                    Transaction.Rollback();
                    if (ex.GetBaseException().Message.ToUpper().Contains(EduSuiteUIResources.ForeignKeyError.ToUpper()))
                    {
                        model.Message = String.Format(EduSuiteUIResources.CantDeleteErrorMessage, EduSuiteUIResources.CourseTransfer);
                        model.IsSuccessful = false;
                        ActivityLog.CreateActivityLog(MenuConstants.CourseTransfer, ActionConstants.Delete, DbConstants.LogType.Error, RowKey, ex.GetBaseException().Message);
                    }
                }
                catch (Exception ex)
                {
                    Transaction.Rollback();
                    model.Message = String.Format(EduSuiteUIResources.FailedToDeleteErrorMessage, EduSuiteUIResources.CourseTransfer);
                    model.IsSuccessful = false;
                    ActivityLog.CreateActivityLog(MenuConstants.CourseTransfer, ActionConstants.Delete, DbConstants.LogType.Error, RowKey, ex.GetBaseException().Message);
                }
            }
            return model;


        }
        private void FillNotificationDetail(CourseTransferViewModel model)
        {
            NotificationTemplate notificationTemplateModel = dbContext.NotificationTemplates.SingleOrDefault(row => row.RowKey == DbConstants.NotificationTemplate.CourseTransfer);
            if (notificationTemplateModel != null)
            {
                model.AutoEmail = notificationTemplateModel.AutoEmail;
                model.AutoSMS = notificationTemplateModel.AutoSMS;
                //model.GuardianSMS = notificationTemplateModel.GuardianSMS;
                model.TemplateKey = notificationTemplateModel.RowKey;
            }
        }
        private void CreateTransferAdmissionFee(CourseTransferViewModel model)
        {

            List<AdmissionFee> dbAdmissionFeeList = dbContext.AdmissionFees.Where(x => x.ApplicationKey == model.ApplicationKey && x.IsActive).ToList();

            long TransferAdmissionFeeMaxKey = dbContext.TransferAdmissionFees.Select(p => p.RowKey).DefaultIfEmpty().Max();
            foreach (AdmissionFee item in dbAdmissionFeeList)
            {
                TransferAdmissionFee TransferAdmissionFeeModels = new TransferAdmissionFee();

                TransferAdmissionFeeModels.RowKey = Convert.ToInt64(TransferAdmissionFeeMaxKey + 1);
                TransferAdmissionFeeModels.ApplicationKey = item.ApplicationKey;
                TransferAdmissionFeeModels.AdmissionFeeYear = item.AdmissionFeeYear;
                TransferAdmissionFeeModels.AdmissionFeeAmount = Convert.ToDecimal(item.AdmissionFeeAmount);
                TransferAdmissionFeeModels.ConcessionAmount = item.ConcessionAmount;
                TransferAdmissionFeeModels.ActualAmount = Convert.ToDecimal(item.ActualAmount);
                TransferAdmissionFeeModels.FeeTypeKey = item.FeeTypeKey;
                TransferAdmissionFeeModels.OldPaid = item.OldPaid;
                TransferAdmissionFeeModels.StudentCourseTransferKey = model.RowKey;
                TransferAdmissionFeeModels.IsActive = true;

                dbContext.TransferAdmissionFees.Add(TransferAdmissionFeeModels);
                TransferAdmissionFeeMaxKey++;
            }


            if (dbAdmissionFeeList != null && dbAdmissionFeeList.Count > 0)
            {
                dbContext.AdmissionFees.RemoveRange(dbAdmissionFeeList);
            }

            long AdmissionFeeMaxKey = dbContext.AdmissionFees.Select(p => p.RowKey).DefaultIfEmpty().Max();

            List<AdmissionFeeModel> NewAdmissionFeeModelList = GetAdmissionFeesByCourse(model);

            foreach (AdmissionFeeModel Viewmodel in NewAdmissionFeeModelList)
            {

                AdmissionFee FeeModels = new AdmissionFee();

                FeeModels.RowKey = Convert.ToInt64(AdmissionFeeMaxKey + 1);
                FeeModels.ApplicationKey = model.ApplicationKey ?? 0;
                FeeModels.AdmissionFeeYear = Viewmodel.AdmissionFeeYear;
                FeeModels.AdmissionFeeAmount = Convert.ToDecimal(Viewmodel.AdmissionFeeAmount);
                FeeModels.ConcessionAmount = Viewmodel.ConcessionAmount;
                FeeModels.ActualAmount = Convert.ToDecimal(Viewmodel.ActualAmount);
                FeeModels.FeeTypeKey = Viewmodel.FeeTypeKey;
                FeeModels.OldPaid = Viewmodel.OldPaid;
                FeeModels.IsActive = true;

                dbContext.AdmissionFees.Add(FeeModels);
                AdmissionFeeMaxKey++;
            }




        }
        private List<AdmissionFeeModel> GetAdmissionFeesByCourse(CourseTransferViewModel model)
        {
            List<AdmissionFeeModel> AdmissionFeeModelList = new List<AdmissionFeeModel>();
            UniversityCourse universityCourse = dbContext.UniversityCourses.SingleOrDefault(row => row.CourseKey == model.ToCourseKey && row.UniversityMasterKey == model.ToUniverisityKey && row.AcademicTermKey == model.ToAcademicTermKey && row.IsActive == true);
            long? UniversityCourseKey;
            if (universityCourse != null)
            {
                UniversityCourseKey = universityCourse.RowKey;
            }
            decimal duration;
            decimal Cduration;

            if (universityCourse != null)
            {
                var CourseDuration = universityCourse.Course.CourseDuration;
                duration = Math.Ceiling((Convert.ToDecimal(universityCourse.AcademicTermKey == DbConstants.AcademicTerm.Semester ? CourseDuration / 6 : CourseDuration / 12)));
                Cduration = Math.Ceiling((Convert.ToDecimal(universityCourse.AcademicTermKey == DbConstants.AcademicTerm.Semester ? CourseDuration / 6 : CourseDuration / 12)));
            }
            else
            {
                var CourseDuration = dbContext.VwCourseSelectActiveOnlies.Where(row => row.RowKey == model.ToCourseKey).Select(row => row.CourseDuration).SingleOrDefault();
                duration = Math.Ceiling((Convert.ToDecimal(Convert.ToBoolean(model.ToAcademicTermKey ?? 0) ? ((CourseDuration ?? 0) / 6) : ((CourseDuration ?? 0) / 12))));
                Cduration = Math.Ceiling((Convert.ToDecimal(Convert.ToBoolean(model.ToAcademicTermKey ?? 0) ? ((CourseDuration ?? 0) / 6) : ((CourseDuration ?? 0) / 12))));
            }

            var StartYear = 1;

            if (duration == 0)
            {
                duration = 1;
            }



            if (universityCourse != null)
            {

                AdmissionFeeModelList = universityCourse.UniversityCourseFees.Where(x => x.IsActive == true && x.FeeAmount != null && (x.FeeYear == null || (x.FeeYear >= StartYear && x.FeeYear <= duration))).Select(x => new AdmissionFeeModel
                {
                    AdmissionFeeYear = x.FeeYear,
                    ActualAmount = x.FeeAmount,
                    AdmissionFeeAmount = x.FeeAmount,
                    FeeTypeKey = x.FeeTypeKey,
                    FeeTypeName = x.FeeType.FeeTypeName,

                    IsUniversity = x.FeeType.IsUniverisity,
                    CenterShareAmountPer = x.CenterShareAmountPer
                }).OrderBy(x => x.AdmissionFeeYear).ToList();

            }

            return AdmissionFeeModelList;
        }
        private void FillDropDownList(CourseTransferViewModel model)
        {
            FillAcademicTerm(model);
            GetCourseType(model);
            GetCourseByCourseType(model);
            GetUniversity(model);
        }
        private void FillAcademicTerm(CourseTransferViewModel model)
        {
            model.AcademicTerms = dbContext.VwAcadamicTermSelectActiveOnlies.Select(x => new SelectListModel
            {
                RowKey = x.RowKey,
                Text = x.AcademicTermName,
            }).ToList();

        }
        public void GetCourseType(CourseTransferViewModel model)
        {

            model.CourseTypes = dbContext.CourseTypes.Where(row => row.IsActive && row.Courses.Any(x => x.UniversityCourses.Any(y => y.AcademicTermKey == model.ToAcademicTermKey && y.IsActive))).Select(row => new SelectListModel
            {
                RowKey = row.RowKey,
                Text = row.CourseTypeName
            }).ToList();

        }
        public void GetCourseByCourseType(CourseTransferViewModel model)
        {

            List<UniversityCourse> UniversityCoursesList = dbContext.UniversityCourses.Where(row => row.Course.CourseTypeKey == model.CourseTypeKey && row.AcademicTermKey == model.ToAcademicTermKey && row.IsActive).Distinct().ToList();

            model.Courses = UniversityCoursesList.Where(x => x.CourseKey != model.FromCourseKey || x.AcademicTermKey != model.FromAcademicTermKey || x.UniversityMasterKey != model.FromUniverisityKey).Select(row => new SelectListModel
            {
                RowKey = row.Course.RowKey,
                Text = row.Course.CourseName,
            }).Distinct().ToList();
        }
        public void GetUniversity(CourseTransferViewModel model)
        {
            model.Universities = dbContext.UniversityCourses.Where(row => row.CourseKey == model.ToCourseKey && row.AcademicTermKey == model.ToAcademicTermKey && row.IsActive).Select(row => new SelectListModel
            {
                RowKey = row.UniversityMaster.RowKey,
                Text = row.UniversityMaster.UniversityMasterName
            }).Distinct().ToList();


        }
        public List<CourseTransferViewModel> BindAllCourseTansferDetailsById(CourseTransferViewModel model)
        {
            try
            {
                List<CourseTransferViewModel> CourseTransfer = new List<CourseTransferViewModel>();


                CourseTransfer = dbContext.StudentCourseTransfers.Where(x => x.ApplicationKey == model.ApplicationKey)
                                     .Select(row => new CourseTransferViewModel
                                     {
                                         RowKey = row.RowKey,
                                         ApplicationKey = row.ApplicationKey ?? 0,
                                         FromAcademicTermKey = row.FromAcademicTermKey,
                                         FromCourseKey = row.FromCourseKey,
                                         FromUniverisityKey = row.FromUniverisityKey,
                                         ToAcademicTermKey = row.ToAcademicTermKey,
                                         ToCourseKey = row.ToCourseKey ?? 0,
                                         ToUniverisityKey = row.ToUniverisityKey ?? 0,
                                         IsActive = row.IsActive ?? false,
                                         Remarks = row.Remarks,
                                         FromAcademicTermName = row.AcademicTerm.AcademicTermName,
                                         ToAcademicTermName = row.AcademicTerm1.AcademicTermName,
                                         FromCourseName = row.Course.CourseName,
                                         ToCourseName = row.Course1.CourseName,
                                         FromUniversityName = row.UniversityMaster.UniversityMasterName,
                                         ToUniversityName = row.UniversityMaster1.UniversityMasterName,
                                         TransferDate = row.DateAdded
                                     }).OrderBy(x => x.RowKey).ToList();



                return CourseTransfer;
            }
            catch (Exception)
            {
                return new List<CourseTransferViewModel>();
            }
        }
    }
}
