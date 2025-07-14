using CITS.EduSuite.Business.Interfaces;
using CITS.EduSuite.Business.Common;
using CITS.EduSuite.Data;
using CITS.EduSuite.Business.Models.ViewModels;
using System;
using System.Collections.Generic;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.Entity;
using CITS.EduSuite.Business.Models.Resources;
using System.Linq.Expressions;

namespace CITS.EduSuite.Business.Services
{
    public class AttendanceService : IAttendanceService
    {

        private EduSuiteDatabase dbContext;
        long AttendanceDetailsMaxKey;
        public AttendanceService(EduSuiteDatabase objdb)
        {
            this.dbContext = objdb;
        }

        public AttendanceViewModel GetAttendanceById(AttendanceViewModel model)
        {
            try
            {

                AttendanceViewModel attendanceViewModel = new AttendanceViewModel();


                attendanceViewModel = dbContext.Attendances.Where(x => x.RowKey == model.RowKey).Select(row => new AttendanceViewModel
                {
                    RowKey = row.RowKey,
                    IsUpdate = true,
                    BranchKey = row.BranchKey,
                    BatchKey = row.BatchKey ?? 0,
                    EmployeeKey = row.EmployeeKey,
                    AttendanceDate = row.AttendanceDate,
                    ClassDetailsKey = row.ClassDetailsKey
                }).SingleOrDefault();

                if (attendanceViewModel == null)
                {
                    attendanceViewModel = new AttendanceViewModel();

                }


                FillDropDown(attendanceViewModel);
                FillNotificationDetail(attendanceViewModel);


                return attendanceViewModel;
            }
            catch (Exception Ex)
            {
                ActivityLog.CreateActivityLog(MenuConstants.StudentAttendance, ActionConstants.View, DbConstants.LogType.Error, null, Ex.GetBaseException().Message);
                return new AttendanceViewModel();


            }
        }
        public AttendanceViewModel FillAttendanceDetailsViewModel(AttendanceViewModel model)
        {
            model.AttendanceDetails = dbContext.Sp_GetAttendanceDetails(model.AttendanceDate, model.BranchKey, model.ClassDetailsKey, model.BatchKey)
                         .GroupBy(row => new
                         {

                             row.RollNumber,
                             row.AdmissionNo,
                             row.RollNoCode,
                             row.ApplicationKey,
                             row.StudentName,
                             row.StudentEmail,
                             row.StudentMobile,
                             row.GuardianMobile,
                             row.StudentPhotoPath
                         }).Select(row => new AttendanceDetailsViewModel
                         {
                             RowKey = row.Where(x => x.AttendanceDate.Date == model.AttendanceDate.Date && (x.RowKey ?? 0) != 0).OrderByDescending(x => x.AttendanceTypeKey).Select(x => x.RowKey ?? 0).FirstOrDefault(),
                             RollNumber = row.Key.RollNumber,
                             AdmissionNo = row.Key.AdmissionNo,
                             ApplicationKey = row.Key.ApplicationKey ?? 0,
                             RollNoCode = row.Key.RollNoCode,
                             StudentName = row.Key.StudentName,
                             StudentEmail = row.Key.StudentEmail,
                             MobileNumber = row.Key.StudentMobile,
                             GuardianMobileNumber = row.Key.GuardianMobile,
                             ApplicantPhoto = row.Key.StudentPhotoPath,
                             AttendanceStatusDetailsViewModel = row.Select(x => new AttendanceStatusDetailsViewModel
                             {
                                 AttendanceDetailRowKey = x.AttendanceDetailKey ?? 0,
                                 AttendanceTypeKey = x.AttendanceTypeKey ?? 0,
                                 Remarks = x.Remarks,
                                 AttendanceStatus = x.AttendanceStatus ?? false,
                                 AttendanceMasterKey = row.Where(y => y.AttendanceDate.Date == model.AttendanceDate.Date).Select(y => y.RowKey ?? 0).FirstOrDefault(),
                                 AttendanceStatusKey = x.AttendanceStatusKey ?? 0,
                                 AttendanceStatusColor = x.AttendanceStatusColor,
                                 AttendanceStatusCode = x.AttendanceStatusCode,
                                 AttendanceDate = x.AttendanceDate,
                                 StartTime = x.StartTime,
                                 EndTime = x.EndTime
                             }).ToList()

                         }).ToList();
            return model;
        }
        public AttendanceViewModel CreateAttendance(AttendanceViewModel model)
        {

            FillDropDown(model);

            using (var transaction = dbContext.Database.BeginTransaction())
            {
                try
                {
                    long MaxKey = dbContext.Attendances.Select(p => p.RowKey).DefaultIfEmpty().Max();
                    foreach (AttendanceDetailsViewModel modelDetails in model.AttendanceDetails.Where(x => x.RowKey == 0))
                    {
                        Attendance AttendanceModel = new Attendance();
                        AttendanceModel.RowKey = MaxKey + 1;
                        AttendanceModel.AttendanceDate = model.AttendanceDate;
                        AttendanceModel.BranchKey = model.BranchKey ?? 0;
                        AttendanceModel.ClassDetailsKey = model.ClassDetailsKey;
                        AttendanceModel.BatchKey = model.BatchKey;
                        AttendanceModel.EmployeeKey = model.EmployeeKey;
                        AttendanceModel.ApplicationKey = modelDetails.ApplicationKey;
                        AttendanceModel.RollNumber = modelDetails.RollNumber;
                        dbContext.Attendances.Add(AttendanceModel);
                        CreateAttendanceDetails(modelDetails.AttendanceStatusDetailsViewModel.Where(x => x.AttendanceDetailRowKey == 0).ToList(), AttendanceModel, model.IfPresent);
                        MaxKey++;
                    }



                    //CreateAttendanceDetails(model.AttendanceDetails.Where(x => x.RowKey == 0).ToList(), AttendanceModel);
                    //UpdateAttendancedetails(model.AttendanceDetails.Where(x => x.RowKey != 0).ToList(), AttendanceModel);
                    dbContext.SaveChanges();
                    transaction.Commit();
                    model.Message = EduSuiteUIResources.Success;
                    model.IsSuccessful = true;
                    ActivityLog.CreateActivityLog(MenuConstants.StudentAttendance, (model.AttendanceDetails.Any(row => row.RowKey != 0) ? ActionConstants.Edit : ActionConstants.Add), DbConstants.LogType.Info, null, model.Message);

                }
                catch (Exception Ex)
                {
                    transaction.Rollback();
                    model.Message = String.Format(EduSuiteUIResources.FailedToSaveErrorMessage, EduSuiteUIResources.Attendance);
                    model.IsSuccessful = false;
                    ActivityLog.CreateActivityLog(MenuConstants.StudentAttendance, (model.AttendanceDetails.Any(row => row.RowKey != 0) ? ActionConstants.Edit : ActionConstants.Add), DbConstants.LogType.Error, null, Ex.GetBaseException().Message);
                }
            }
            return model;
        }
        public AttendanceViewModel UpdateAttendance(AttendanceViewModel model)
        {
            Attendance AttendanceModel = new Attendance();
            FillDropDown(model);

            using (var transaction = dbContext.Database.BeginTransaction())
            {
                try
                {
                    long MaxKey = dbContext.Attendances.Select(p => p.RowKey).DefaultIfEmpty().Max();
                    foreach (AttendanceDetailsViewModel modelDetails in model.AttendanceDetails)
                    {
                        modelDetails.AttendanceDate = model.AttendanceDate;

                        if (modelDetails.RowKey == 0)
                        {
                            AttendanceModel = new Attendance();
                            AttendanceModel.RowKey = ++MaxKey;
                            AttendanceModel.AttendanceDate = model.AttendanceDate;
                            AttendanceModel.BranchKey = model.BranchKey ?? 0;
                            AttendanceModel.ClassDetailsKey = model.ClassDetailsKey;
                            AttendanceModel.BatchKey = model.BatchKey;
                            AttendanceModel.EmployeeKey = model.EmployeeKey;
                            AttendanceModel.ApplicationKey = modelDetails.ApplicationKey;
                            AttendanceModel.RollNumber = modelDetails.RollNumber;
                            dbContext.Attendances.Add(AttendanceModel);
                            CreateAttendanceDetails(modelDetails.AttendanceStatusDetailsViewModel.Where(x => x.AttendanceDetailRowKey == 0).ToList(), AttendanceModel, model.IfPresent);

                        }
                        else
                        {

                            AttendanceModel = dbContext.Attendances.Where(x => x.RowKey == modelDetails.RowKey).SingleOrDefault();
                            AttendanceModel.AttendanceDate = model.AttendanceDate;
                            AttendanceModel.BranchKey = model.BranchKey ?? 0;
                            AttendanceModel.ClassDetailsKey = model.ClassDetailsKey;
                            AttendanceModel.EmployeeKey = model.EmployeeKey;
                            AttendanceModel.ApplicationKey = modelDetails.ApplicationKey;
                            AttendanceModel.RollNumber = modelDetails.RollNumber;


                            UpdateAttendancedetails(modelDetails.AttendanceStatusDetailsViewModel.Where(x => x.AttendanceDetailRowKey != 0).ToList(), AttendanceModel, model.IfPresent);
                            CreateAttendanceDetails(modelDetails.AttendanceStatusDetailsViewModel.Where(x => x.AttendanceDetailRowKey == 0).ToList(), AttendanceModel, model.IfPresent);
                        }

                    }


                    dbContext.SaveChanges();
                    transaction.Commit();
                    model.Message = EduSuiteUIResources.Success;
                    model.IsSuccessful = true;
                    ActivityLog.CreateActivityLog(MenuConstants.StudentAttendance, (model.AttendanceDetails.Any(row => row.RowKey != 0) ? ActionConstants.Edit : ActionConstants.Add), DbConstants.LogType.Info, null, model.Message);

                }
                catch (Exception Ex)
                {
                    transaction.Rollback();
                    model.Message = String.Format(EduSuiteUIResources.FailedToSaveErrorMessage, EduSuiteUIResources.Attendance);
                    model.IsSuccessful = false;
                    ActivityLog.CreateActivityLog(MenuConstants.StudentAttendance, (model.AttendanceDetails.Any(row => row.RowKey != 0) ? ActionConstants.Edit : ActionConstants.Add), DbConstants.LogType.Error, null, Ex.GetBaseException().Message);
                }
            }
            return model;
        }
        private void CreateAttendanceDetails(List<AttendanceStatusDetailsViewModel> ModelList, Attendance model, bool IsPresent)
        {
            AttendanceDetailsMaxKey = AttendanceDetailsMaxKey == 0 ? dbContext.AttendanceDetails.Select(p => p.RowKey).DefaultIfEmpty().Max() : AttendanceDetailsMaxKey;
            foreach (AttendanceStatusDetailsViewModel modelDetails in ModelList)
            {
                AttendanceDetail attendanceDetailModel = new AttendanceDetail();

                attendanceDetailModel.RowKey = ++AttendanceDetailsMaxKey;
                attendanceDetailModel.AttendanceTypeKey = modelDetails.AttendanceTypeKey;
                //attendanceDetailModel.AttendanceStatus = IsPresent ? modelDetails.AttendanceStatus : !modelDetails.AttendanceStatus;
                attendanceDetailModel.AttendanceStatus = modelDetails.AttendanceStatusKey == DbConstants.AttendanceStatus.Present ? true : false;
                attendanceDetailModel.Remarks = modelDetails.Remarks;
                attendanceDetailModel.AttendanceMasterKey = model.RowKey;
                //attendanceDetailModel.AttendanceStatusKey = attendanceDetailModel.AttendanceStatus ? DbConstants.AttendanceStatus.Present : DbConstants.AttendanceStatus.Absent;
                attendanceDetailModel.AttendanceStatusKey = modelDetails.AttendanceStatusKey;
                dbContext.AttendanceDetails.Add(attendanceDetailModel);
                modelDetails.AttendanceStatusKey = attendanceDetailModel.AttendanceStatusKey;
                modelDetails.AttendanceDetailRowKey = attendanceDetailModel.RowKey;

            }
        }
        private void UpdateAttendancedetails(List<AttendanceStatusDetailsViewModel> ModelList, Attendance model, bool IsPresent)
        {
            foreach (AttendanceStatusDetailsViewModel modelDetails in ModelList)
            {
                AttendanceDetail attendanceDetailModel = new AttendanceDetail();
                attendanceDetailModel = dbContext.AttendanceDetails.SingleOrDefault(p => p.RowKey == modelDetails.AttendanceDetailRowKey);
                attendanceDetailModel.AttendanceTypeKey = modelDetails.AttendanceTypeKey;
                //attendanceDetailModel.AttendanceStatus = IsPresent ? modelDetails.AttendanceStatus : !modelDetails.AttendanceStatus;
                attendanceDetailModel.AttendanceStatus = modelDetails.AttendanceStatusKey == DbConstants.AttendanceStatus.Present ? true : false;
                attendanceDetailModel.Remarks = modelDetails.Remarks;
                //attendanceDetailModel.AttendanceStatusKey = attendanceDetailModel.AttendanceStatus ? DbConstants.AttendanceStatus.Present : DbConstants.AttendanceStatus.Absent;
                attendanceDetailModel.AttendanceStatusKey = modelDetails.AttendanceStatusKey;
                attendanceDetailModel.AttendanceMasterKey = model.RowKey;

            }
        }
        public AttendanceViewModel DeleteAttendance(AttendanceViewModel model)
        {
            using (var transaction = dbContext.Database.BeginTransaction())
            {
                try
                {
                    Attendance attendance = dbContext.Attendances.SingleOrDefault(x => x.RowKey == model.RowKey);
                    List<AttendanceDetail> attendanceDetailslist = dbContext.AttendanceDetails.Where(x => x.AttendanceMasterKey == model.RowKey).ToList();

                    if (attendanceDetailslist.Count > 0)
                    {
                        dbContext.AttendanceDetails.RemoveRange(attendanceDetailslist);
                        dbContext.Attendances.Remove(attendance);

                    }
                    dbContext.SaveChanges();
                    transaction.Commit();
                    model.Message = EduSuiteUIResources.Success;
                    model.IsSuccessful = true;
                    ActivityLog.CreateActivityLog(MenuConstants.StudentAttendance, ActionConstants.Delete, DbConstants.LogType.Info, null, model.Message);
                }
                catch (DbUpdateException ex)
                {
                    transaction.Rollback();
                    if (ex.GetBaseException().Message.ToUpper().Contains(EduSuiteUIResources.ForeignKeyError.ToUpper()))
                    {
                        model.Message = String.Format(EduSuiteUIResources.CantDeleteErrorMessage, EduSuiteUIResources.Attendance);
                        model.IsSuccessful = false;
                        ActivityLog.CreateActivityLog(MenuConstants.StudentAttendance, ActionConstants.Delete, DbConstants.LogType.Error, null, ex.GetBaseException().Message);

                    }
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    model.Message = String.Format(EduSuiteUIResources.FailedToDeleteErrorMessage, EduSuiteUIResources.Attendance);
                    model.IsSuccessful = false;
                    ActivityLog.CreateActivityLog(MenuConstants.StudentAttendance, ActionConstants.Delete, DbConstants.LogType.Debug, null, ex.GetBaseException().Message);
                }
            }
            return model;
        }
        public AttendanceViewModel DeleteBulkAttendance(AttendanceViewModel model, List<long> RowKeys)
        {
            using (var transaction = dbContext.Database.BeginTransaction())
            {
                try
                {
                    List<AttendanceDetail> attendanceDetailslist = dbContext.AttendanceDetails.Where(x => RowKeys.Contains(x.AttendanceMasterKey)).ToList();
                    List<Attendance> attendancelist = dbContext.Attendances.Where(x => RowKeys.Contains(x.RowKey)).ToList();

                    if (attendanceDetailslist.Count > 0)
                    {
                        dbContext.AttendanceDetails.RemoveRange(attendanceDetailslist);
                        if (attendancelist.Count > 0)
                        {
                            dbContext.Attendances.RemoveRange(attendancelist);
                        }
                    }
                    dbContext.SaveChanges();
                    transaction.Commit();
                    model.Message = EduSuiteUIResources.Success;
                    model.IsSuccessful = true;
                    ActivityLog.CreateActivityLog(MenuConstants.StudentAttendance, ActionConstants.Delete, DbConstants.LogType.Info, null, model.Message);
                }
                catch (DbUpdateException ex)
                {
                    transaction.Rollback();
                    if (ex.GetBaseException().Message.ToUpper().Contains(EduSuiteUIResources.ForeignKeyError.ToUpper()))
                    {
                        model.Message = String.Format(EduSuiteUIResources.CantDeleteErrorMessage, EduSuiteUIResources.Attendance);
                        model.IsSuccessful = false;
                        ActivityLog.CreateActivityLog(MenuConstants.StudentAttendance, ActionConstants.Delete, DbConstants.LogType.Error, null, ex.GetBaseException().Message);

                    }
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    model.Message = String.Format(EduSuiteUIResources.FailedToDeleteErrorMessage, EduSuiteUIResources.Attendance);
                    model.IsSuccessful = false;
                    ActivityLog.CreateActivityLog(MenuConstants.StudentAttendance, ActionConstants.Delete, DbConstants.LogType.Debug, null, ex.GetBaseException().Message);
                }
            }
            return model;
        }
        public List<AttendanceViewModel> GetAttendance(AttendanceViewModel model, out long TotalRecords)
        {
            try
            {
                var Take = model.PageSize;
                var Skip = (model.PageIndex - 1) * model.PageSize;

                Employee Employee = dbContext.Employees.Where(x => x.AppUserKey == DbConstants.User.UserKey).SingleOrDefault();

                IQueryable<AttendanceViewModel> AttendanceList = (from p in dbContext.Attendances
                                                                  where ((p.Application.StudentName.Contains(model.SearchText)) &&
                                                                  ((p.AttendanceDate >= model.SearchFromDate && p.AttendanceDate <= model.AttendanceDate)
                                                                  ||
                                                                     (model.SearchFromDate != null ? (System.Data.Entity.DbFunctions.TruncateTime(p.AttendanceDate) >= System.Data.Entity.DbFunctions.TruncateTime(model.SearchFromDate) && System.Data.Entity.DbFunctions.TruncateTime(p.AttendanceDate) <= System.Data.Entity.DbFunctions.TruncateTime(model.AttendanceDate))
                                                                       : System.Data.Entity.DbFunctions.TruncateTime(p.AttendanceDate) <= System.Data.Entity.DbFunctions.TruncateTime(model.AttendanceDate))))
                                                                  select new AttendanceViewModel
                                                                  {
                                                                      StudentName = p.Application.StudentName + EduSuiteUIResources.OpenBracket + p.Application.RollNoCode + EduSuiteUIResources.CloseBracket,
                                                                      AttendanceStatusName = p.AttendanceDetails.Select(x => x.AttendanceStatu.AttendanceStatusName).FirstOrDefault(),
                                                                      AttendanceTypeName = p.AttendanceDetails.Select(x => x.AttendanceType.AttendanceTypeName).FirstOrDefault(),
                                                                      AttendanceStatusKey = p.AttendanceDetails.Select(x => x.AttendanceStatusKey).FirstOrDefault(),
                                                                      RowKey = p.RowKey,
                                                                      BranchKey = p.BranchKey,
                                                                      BranchName = p.Branch.BranchName,
                                                                      ClassDetailsKey = p.ClassDetailsKey,
                                                                      BatchKey = p.BatchKey ?? 0,
                                                                      BatchName = p.Batch.BatchName,
                                                                      ClassDetailsName = p.ClassDetail.ClassCode,
                                                                      CourseName = p.Application.Course.CourseName,
                                                                      AttendanceDate = p.AttendanceDate
                                                                  });
                //}).AsEnumerable()
                //.Select((p, i) => new AttendanceViewModel
                //{
                //    StudentName = p.StudentName,
                //    AttendanceStatusName = p.AttendanceStatusName,
                //    AttendanceTypeName = p.AttendanceTypeName,
                //    AttendanceStatusKey = p.AttendanceStatusKey,
                //    RowKey = p.RowKey,
                //    BranchKey = p.BranchKey,
                //    BranchName = p.BranchName,
                //    ClassDetailsKey = p.ClassDetailsKey,
                //    BatchKey = p.BatchKey,
                //    BatchName = p.BatchName,
                //    ClassDetailsName = p.ClassDetailsName,
                //    CourseName = p.CourseName,
                //    AttendanceDate = p.AttendanceDate,
                //    SlNo = i+1
                //});

                if (Employee != null)
                {
                    if (Employee.BranchAccess != null)
                    {
                        var Branches = Employee.BranchAccess.Split(',').Select(Int16.Parse).ToList();
                        AttendanceList = AttendanceList.Where(row => Branches.Contains(row.BranchKey ?? 0));
                    }
                    if (Employee.IsTeacher)
                    {
                        List<long> TClassKeys = new List<long>();
                        TClassKeys = dbContext.TeacherClassAllocations.Where(x => x.EmployeeKey == Employee.RowKey && x.IsAttendance && x.IsActive).Select(y => y.ClassDetailsKey ?? 0).ToList();
                        AttendanceList = AttendanceList.Where(row => TClassKeys.Contains(row.ClassDetailsKey));

                        List<short> TBatchKeys = new List<short>();
                        TBatchKeys = dbContext.TeacherClassAllocations.Where(x => x.EmployeeKey == Employee.RowKey && x.IsAttendance && x.IsActive).Select(y => y.BatchKey ?? 0).ToList();
                        AttendanceList = AttendanceList.Where(row => TBatchKeys.Contains(row.BatchKey));

                    }
                }
                if (model.BranchKey != 0)
                {
                    AttendanceList = AttendanceList.Where(row => row.BranchKey == model.BranchKey);
                }
                if (model.ClassDetailsKey != 0)
                {
                    AttendanceList = AttendanceList.Where(row => row.ClassDetailsKey == model.ClassDetailsKey);
                }
                if (model.BatchKey != 0)
                {
                    AttendanceList = AttendanceList.Where(row => row.BatchKey == model.BatchKey);
                }
                if (model.AttendanceStatusKey != 0)
                {
                    AttendanceList = AttendanceList.Where(row => row.AttendanceStatusKey == model.AttendanceStatusKey);
                }

                //var withSequence = AttendanceList.AsEnumerable()
                //        .Select((p, index) => new
                //        {
                //            StudentName = p.StudentName,
                //            AttendanceStatusName = p.AttendanceStatusName,
                //            AttendanceTypeName = p.AttendanceTypeName,
                //            AttendanceStatusKey = p.AttendanceStatusKey,
                //            RowKey = p.RowKey,
                //            BranchKey = p.BranchKey,
                //            BranchName = p.BranchName,
                //            ClassDetailsKey = p.ClassDetailsKey,
                //            BatchKey = p.BatchKey,
                //            BatchName = p.BatchName,
                //            ClassDetailsName = p.ClassDetailsName,
                //            CourseName = p.CourseName,
                //            AttendanceDate = p.AttendanceDate,
                //            SlNo = index + 1
                //        });


                AttendanceList = AttendanceList.GroupBy(x => new { x.RowKey }).Select(y => y.FirstOrDefault());
                if (model.SortBy != "")
                {
                    AttendanceList = SortAttendanceResult(AttendanceList, model.SortBy, model.SortOrder);
                }
                TotalRecords = AttendanceList.Count();
                return AttendanceList.Skip(Skip ?? 0).Take(Take ?? 0).ToList<AttendanceViewModel>();
            }
            catch (Exception Ex)
            {
                TotalRecords = 0;
                ActivityLog.CreateActivityLog(MenuConstants.StudentAttendance, ActionConstants.View, DbConstants.LogType.Error, null, Ex.GetBaseException().Message);
                return new List<AttendanceViewModel>();

            }
        }
        private IQueryable<AttendanceViewModel> SortAttendanceResult(IQueryable<AttendanceViewModel> Query, string SortName, string SortOrder)
        {

            string command = SortOrder == "desc" ? "OrderByDescending" : "OrderBy";
            var type = typeof(AttendanceViewModel);
            var property = type.GetProperty(SortName);
            var parameter = Expression.Parameter(type, "p");
            var propertyAccess = Expression.MakeMemberAccess(parameter, property);
            var orderByExpression = Expression.Lambda(propertyAccess, parameter);
            var resultExpression = Expression.Call(typeof(Queryable), command, new Type[] { type, property.PropertyType },
                                          Query.Expression, Expression.Quote(orderByExpression));
            return Query.Provider.CreateQuery<AttendanceViewModel>(resultExpression);

        }

        #region DropDownChange Events
        public AttendanceViewModel GetSearchDropdownList(AttendanceViewModel model)
        {
            FillBranch(model);
            FillSearchClassDetails(model);
            FillSearchBatch(model);
            FillAttendanceStatus(model);
            return model;
        }
        private void FillDropDown(AttendanceViewModel model)
        {
            FillBranch(model);
            FillClassDetails(model);
            FillBatch(model);
            FillAttendanceType(model);
        }
        public void FillBranch(AttendanceViewModel model)
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
        public AttendanceViewModel FillClassDetails(AttendanceViewModel model)
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
                                          where (TCA.EmployeeKey == Employee.RowKey && TCA.IsActive == true && TCA.IsAttendance == true && (model.IsUpdate != true ? SDA.IsActive == true : SDA.IsActive != null) && A.BranchKey == Employee.BranchKey)
                                          select new SelectListModel
                                          {
                                              RowKey = CD.RowKey,
                                              Text = CD.ClassCode + CD.ClassCodeDescription
                                          }).Distinct().ToList();

                }
                else
                {
                    model.ClassDetails = (from CD in dbContext.VwClassDetailsSelectActiveOnlies
                                          join SDA in dbContext.StudentDivisionAllocations on CD.RowKey equals SDA.ClassDetailsKey
                                          join A in dbContext.Applications on CD.RowKey equals A.ClassDetailsKey
                                          where ((model.IsUpdate != true ? SDA.IsActive == true : SDA.IsActive != null) && A.BranchKey == model.BranchKey)
                                          select new SelectListModel
                                          {
                                              RowKey = CD.RowKey,
                                              Text = CD.ClassCode + CD.ClassCodeDescription
                                          }).Distinct().ToList();
                }
            }

            else
            {
                model.ClassDetails = (from CD in dbContext.VwClassDetailsSelectActiveOnlies
                                      join SDA in dbContext.StudentDivisionAllocations on CD.RowKey equals SDA.ClassDetailsKey
                                      join A in dbContext.Applications on CD.RowKey equals A.ClassDetailsKey
                                      where ((model.IsUpdate != true ? SDA.IsActive == true : SDA.IsActive != null) && A.BranchKey == model.BranchKey)
                                      select new SelectListModel
                                      {
                                          RowKey = CD.RowKey,
                                          Text = CD.ClassCode + CD.ClassCodeDescription
                                      }).Distinct().ToList();
            }

            return model;
        }
        public AttendanceViewModel FillBatch(AttendanceViewModel model)
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
                                     where (p.ClassDetailsKey == model.ClassDetailsKey && TCA.EmployeeKey == Employee.RowKey && TCA.IsActive == true && TCA.IsAttendance == true && (model.IsUpdate != true ? SDA.IsActive == true : SDA.IsActive != null) && p.BranchKey == Employee.BranchKey)
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
                                     where (p.ClassDetailsKey == model.ClassDetailsKey && p.BranchKey == model.BranchKey && (model.IsUpdate != true ? SDA.IsActive == true : SDA.IsActive != null) && p.BranchKey == model.BranchKey)
                                     select new SelectListModel
                                     {
                                         RowKey = B.RowKey,
                                         Text = B.BatchName
                                     }).Distinct().ToList();
                }
            }

            else
            {
                model.Batches = (from p in dbContext.Applications
                                 join SDA in dbContext.StudentDivisionAllocations on p.RowKey equals SDA.ApplicationKey
                                 join B in dbContext.VwBatchSelectActiveOnlies on p.BatchKey equals B.RowKey
                                 orderby B.RowKey
                                 where (p.ClassDetailsKey == model.ClassDetailsKey && p.BranchKey == model.BranchKey && (model.IsUpdate != true ? SDA.IsActive == true : SDA.IsActive != null))
                                 select new SelectListModel
                                 {
                                     RowKey = B.RowKey,
                                     Text = B.BatchName
                                 }).Distinct().ToList();
            }

            return model;
        }
        private void FillAttendanceType(AttendanceViewModel model)
        {
            model.AttendanceTypes = dbContext.AttendanceTypes.Where(x => x.IsActive).OrderBy(x => x.RowKey).Select(x => new SelectListModel
            {
                RowKey = x.RowKey,
                Text = x.AttendanceTypeName
            }).ToList();
        }
        public void FillSearchClassDetails(AttendanceViewModel model)
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
                                          where (TCA.EmployeeKey == Employee.RowKey && TCA.IsActive == true && TCA.IsAttendance == true && A.BranchKey == Employee.BranchKey)
                                          select new SelectListModel
                                          {
                                              RowKey = CD.RowKey,
                                              Text = CD.ClassCode + CD.ClassCodeDescription
                                          }).Distinct().ToList();

                }
                else
                {
                    model.ClassDetails = (from CD in dbContext.VwClassDetailsSelectActiveOnlies

                                          join SDA in dbContext.StudentDivisionAllocations on CD.RowKey equals SDA.ClassDetailsKey
                                          join A in dbContext.Applications on CD.RowKey equals A.ClassDetailsKey
                                          where (A.BranchKey == model.BranchKey)
                                          select new SelectListModel
                                          {
                                              RowKey = CD.RowKey,
                                              Text = CD.ClassCode + CD.ClassCodeDescription
                                          }).Distinct().ToList();
                }
            }

            else
            {

                if (model.BranchKey != 0)
                {
                    model.ClassDetails = (from CD in dbContext.VwClassDetailsSelectActiveOnlies

                                          join SDA in dbContext.StudentDivisionAllocations on CD.RowKey equals SDA.ClassDetailsKey
                                          join A in dbContext.Applications on CD.RowKey equals A.ClassDetailsKey
                                          where (SDA.IsActive == true && A.BranchKey == model.BranchKey)
                                          select new SelectListModel
                                          {
                                              RowKey = CD.RowKey,
                                              Text = CD.ClassCode + CD.ClassCodeDescription
                                          }).Distinct().ToList();
                }
                else
                {
                    model.ClassDetails = (from CD in dbContext.VwClassDetailsSelectActiveOnlies

                                          join SDA in dbContext.StudentDivisionAllocations on CD.RowKey equals SDA.ClassDetailsKey
                                          where (SDA.IsActive == true)
                                          select new SelectListModel
                                          {
                                              RowKey = CD.RowKey,
                                              Text = CD.ClassCode + CD.ClassCodeDescription
                                          }).Distinct().ToList();
                }
            }


        }
        public AttendanceViewModel FillSearchBatch(AttendanceViewModel model)
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
                                     where (p.ClassDetailsKey == model.ClassDetailsKey && TCA.EmployeeKey == Employee.RowKey && TCA.IsActive == true && TCA.IsAttendance == true && p.BranchKey == Employee.BranchKey)
                                     select new SelectListModel
                                     {
                                         RowKey = B.RowKey,
                                         Text = B.BatchName
                                     }).Distinct().ToList();
                }
                else
                {
                    if (model.BranchKey != 0 || model.ClassDetailsKey != 0)
                    {
                        model.Batches = (from p in dbContext.Applications
                                         join SDA in dbContext.StudentDivisionAllocations on p.RowKey equals SDA.ApplicationKey
                                         join B in dbContext.VwBatchSelectActiveOnlies on p.BatchKey equals B.RowKey
                                         orderby B.RowKey
                                         where (p.ClassDetailsKey == model.ClassDetailsKey || p.BranchKey == model.BranchKey)
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
                }
            }

            else
            {


                if (model.BranchKey != 0 || model.ClassDetailsKey != 0)
                {
                    model.Batches = (from p in dbContext.Applications
                                     join SDA in dbContext.StudentDivisionAllocations on p.RowKey equals SDA.ApplicationKey
                                     join B in dbContext.VwBatchSelectActiveOnlies on p.BatchKey equals B.RowKey
                                     orderby B.RowKey
                                     where (p.ClassDetailsKey == model.ClassDetailsKey || p.BranchKey == model.BranchKey)
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

            }
            return model;
        }

        public void FillAttendanceStatus(AttendanceViewModel model)
        {
            List<byte> AttStatus = new List<byte>();
            AttStatus.Add(DbConstants.AttendanceStatus.Present);
            AttStatus.Add(DbConstants.AttendanceStatus.Absent);
            model.AttendanceStatus = dbContext.AttendanceStatus.Where(x => x.IsActive && AttStatus.Contains(x.RowKey)).OrderBy(x => x.RowKey).Select(x => new SelectListModel
            {
                RowKey = x.RowKey,
                Text = x.AttendanceStatusName
            }).ToList();
        }

        #endregion DropDownChange Events
        private void FillNotificationDetail(AttendanceViewModel model)
        {
            NotificationTemplate notificationTemplateModel = dbContext.NotificationTemplates.SingleOrDefault(row => row.RowKey == DbConstants.NotificationTemplate.StudentAttendance);
            if (notificationTemplateModel != null)
            {
                model.AutoEmail = notificationTemplateModel.AutoEmail;
                model.AutoSMS = notificationTemplateModel.AutoSMS;
                model.TemplateKey = notificationTemplateModel.RowKey;
            }
        }
        public AttendanceViewModel CheckAttendanceBlocked(long RowKey, long ApplicationKey, short AttendanceTypeKey, DateTime AttendanceDate)
        {
            string StudentName = dbContext.Applications.Where(row => row.RowKey == ApplicationKey).Select(row => row.StudentName).SingleOrDefault();
            AttendanceType attendanceType = dbContext.AttendanceTypes.SingleOrDefault(row => row.RowKey == AttendanceTypeKey);
            AttendanceViewModel model = new AttendanceViewModel();
            DateTime DateBeginTime = AttendanceDate.Date;
            DateBeginTime = DateBeginTime.Add((attendanceType.StartTime ?? TimeSpan.Zero));
            int LateMinutes = (int)(AttendanceDate.Add(DateTimeUTC.Now.TimeOfDay) - DateBeginTime).TotalMinutes;
            model.IsSuccessful = true;
            if (dbContext.AttendanceDetails.Any(x => x.RowKey != RowKey && x.Attendance.ApplicationKey == ApplicationKey && DbFunctions.TruncateTime(x.Attendance.AttendanceDate) <= DbFunctions.TruncateTime(AttendanceDate) && x.AttendanceStatusKey == DbConstants.AttendanceStatus.Absent))
            {
                DateTime AbsentDate = dbContext.AttendanceDetails.Where(x => x.Attendance.ApplicationKey == ApplicationKey && DbFunctions.TruncateTime(x.Attendance.AttendanceDate) <= DbFunctions.TruncateTime(AttendanceDate) && x.AttendanceStatusKey == DbConstants.AttendanceStatus.Absent).Select(x => x.Attendance.AttendanceDate).FirstOrDefault();
                model.IsSuccessful = false;
                model.Message = String.Format(EduSuiteUIResources.AbsentErrorMessage, StudentName, AbsentDate.ToString("dd/MM/yyyy"));
            }
            else if (dbContext.StudentAbsconders.Any(x => x.ApplicationKey == ApplicationKey && DbFunctions.TruncateTime(x.AbscondersDate) <= DbFunctions.TruncateTime(AttendanceDate) && x.IsAbsconders == false))
            {
                DateTime AbscondDate = dbContext.StudentAbsconders.Where(x => x.ApplicationKey == ApplicationKey && DbFunctions.TruncateTime(x.AbscondersDate) <= DbFunctions.TruncateTime(AttendanceDate) && x.IsAbsconders == false).Select(x => x.AbscondersDate ?? DateTimeUTC.Now).FirstOrDefault();
                model.IsSuccessful = false;
                model.Message = String.Format(EduSuiteUIResources.AbsconderErrorMessage, StudentName, AbscondDate.ToString("dd/MM/yyyy"));
            }
            else if (LateMinutes > attendanceType.GraceTime && !DbConstants.Role.AdminUserTypes.Contains(DbConstants.User.RoleKey))
            {

                model.IsSuccessful = false;
                model.Message = String.Format(EduSuiteUIResources.LateComingErrorMessage, StudentName, LateMinutes.ToString());
            }
            return model;
        }


    }
}