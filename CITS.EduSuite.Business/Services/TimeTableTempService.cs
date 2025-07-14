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
using System.Data.Entity;
using CITS.EduSuite.Business.Models.Resources;

namespace CITS.EduSuite.Business.Services
{
    public class TimeTableTempService : ITimeTableTempService
    {
        private EduSuiteDatabase dbContext;

        public TimeTableTempService(EduSuiteDatabase objdb)
        {
            this.dbContext = objdb;
        }

        public List<TimeTableTempMasterViewModel> GetTimeTableTempMaster(string searchText)
        {
            try
            {
                var TimeTableTempMasterList = (from TTT in dbContext.TimeTableTempMasters
                                               orderby TTT.RowKey
                                               where (TTT.Employee.FirstName.Contains(searchText))
                                               select new TimeTableTempMasterViewModel
                                               {
                                                   RowKey = TTT.RowKey,
                                                   FromDate = TTT.FromDate,
                                                   ToDate = TTT.ToDate,
                                                   EmployeeName = TTT.Employee.FirstName
                                               }).ToList();
                return TimeTableTempMasterList.GroupBy(x => x.RowKey).Select(y => y.First()).ToList<TimeTableTempMasterViewModel>();
            }
            catch (Exception ex)
            {
                ActivityLog.CreateActivityLog(MenuConstants.TimeTableMaster, ActionConstants.View, DbConstants.LogType.Error, null, ex.GetBaseException().Message);
                return new List<TimeTableTempMasterViewModel>();

            }

        }

        public TimeTableTempMasterViewModel GetTimeTableMasterById(TimeTableTempMasterViewModel model)
        {
            try
            {
                model = dbContext.TimeTableTempMasters.Select(row => new TimeTableTempMasterViewModel
                {
                    RowKey = row.RowKey,
                    EmployeeKey = row.EmployeeKey,
                    FromDate = row.FromDate,
                    ToDate = row.ToDate,

                }).Where(row => row.RowKey == model.RowKey).FirstOrDefault();
                if (model == null)
                {
                    model = new TimeTableTempMasterViewModel();
                }
                FillEmployees(model);
                FillWeekDays(model);
                return model;
            }
            catch (Exception Ex)
            {
                ActivityLog.CreateActivityLog(MenuConstants.StudentTimeTable, ActionConstants.View, DbConstants.LogType.Error, null, Ex.GetBaseException().Message);
                return new TimeTableTempMasterViewModel();
            }
        }

        public void FillTimeTableDetails(TimeTableTempMasterViewModel model)
        {

            //var CheckQuery = dbContext.StudentTimeTableMasters.Where(x => x.ClassDetailsKey == model.ClassDetailsKey && x.AcademicYearKey == model.AcademicYearKey).Select(row => row.RowKey);
            //if (CheckQuery.Any())
            //{
            //    model.RowKey = CheckQuery.FirstOrDefault();
            //}
            //else
            //{
            //    model.RowKey = 0;
            //}

            model.TimeTableTempDetailsModel = (from TT in dbContext.StudentTimeTables
                                               join TTD in dbContext.TimeTableTempDetails.Where(row => row.TempMasterKey == model.RowKey && row.TimeTableTempMaster.EmployeeKey == model.EmployeeKey) on new { TT.Day, TT.ClassDetailsKey, TT.PeriodKey } equals new { TTD.Day, TTD.ClassDetailsKey, TTD.PeriodKey } into TTS
                                               from TTD in TTS.DefaultIfEmpty()
                                               where (TT.EmployeeKey == model.EmployeeKey && TT.Day == model.Day)
                                               select new TimeTableTempDetailsModel
                                                  {
                                                      RowKey = TTD.RowKey != null ? TTD.RowKey : 0,
                                                      SubjectKey = TTD.SubjectKey != null ? TTD.SubjectKey : TT.SubjectKey,
                                                      SubjectName = TTD.SubjectKey != null ? TTD.Subject.SubjectName : TT.Subject.SubjectName,
                                                      ToEmployeeKey = TTD.EmployeeKey,
                                                      EmployeeName = TTD.Employee.FirstName,
                                                      Day = TT.Day,
                                                      PeriodKey = TT.PeriodKey,
                                                      TempMasterKey = TTD.RowKey != null ? TTD.TempMasterKey : 0,
                                                      ClassDetailsKey = TT.ClassDetailsKey

                                                  }).ToList();
            if (model.TimeTableTempDetailsModel.Count == 0)
            {
                model.TimeTableTempDetailsModel.Add(new TimeTableTempDetailsModel());
            }
            model.BranchKey = dbContext.Employees.Where(x => x.RowKey == model.EmployeeKey).Select(y => y.BranchKey).FirstOrDefault();
            FillDropDownList(model);
        }

        public void FillEmployees(TimeTableTempMasterViewModel model)
        {
            model.Employees = dbContext.Employees.Where(x => x.IsActive==true).Select(row => new SelectListModel
            {
                RowKey = row.RowKey,
                Text = row.FirstName
            }).ToList();
        }

        private void FillDropDownList(TimeTableTempMasterViewModel model)
        {
            FillClassDetails(model);

            FillWeeklyPeriods(model);

        }

        public void FillClassDetails(TimeTableTempMasterViewModel model)
        {

            model.ClassDetails = (from CD in dbContext.VwClassDetailsSelectActiveOnlies
                                  join SDA in dbContext.TeacherClassAllocations on CD.RowKey equals SDA.ClassDetailsKey
                                  where (SDA.EmployeeKey == model.EmployeeKey && CD.IsActive)
                                  select new SelectListModel
                                  {
                                      RowKey = CD.RowKey,
                                      Text = CD.ClassCode + CD.ClassCodeDescription
                                  }).Distinct().ToList();

        }

        public void FillWeekDays(TimeTableTempMasterViewModel model)
        {
            model.WeekDays = typeof(DbConstants.WeekDays).GetFields().Select(row => new SelectListModel
            {
                RowKey = Convert.ToByte(row.GetValue(null).ToString()),
                Text = row.Name
            }).ToList();
        }

        public void FillWeeklyPeriods(TimeTableTempMasterViewModel model)
        {
            model.WeeklyPeriods = typeof(DbConstants.WeeklyPeriods).GetFields().Select(row => new SelectListModel
            {
                RowKey = Convert.ToByte(row.GetValue(null).ToString()),
                Text = row.Name
            }).ToList();
        }

        public TimeTableTempMasterViewModel CreateTimeTableTemp(TimeTableTempMasterViewModel model)
        {
            using (var transaction = dbContext.Database.BeginTransaction())
            {
                try
                {


                    long maxKey = dbContext.TimeTableTempMasters.Select(p => p.RowKey).DefaultIfEmpty().Max();

                    TimeTableTempMaster timeTableTempMasterModel = new TimeTableTempMaster();
                    timeTableTempMasterModel.RowKey = Convert.ToInt64(maxKey + 1);

                    timeTableTempMasterModel.FromDate = model.FromDate;
                    timeTableTempMasterModel.ToDate = model.ToDate;
                    timeTableTempMasterModel.EmployeeKey = model.EmployeeKey;
                    dbContext.TimeTableTempMasters.Add(timeTableTempMasterModel);
                    model.RowKey = timeTableTempMasterModel.RowKey;
                    CreateTimeTableDetails(model);


                    dbContext.SaveChanges();
                    transaction.Commit();
                    model.Message = EduSuiteUIResources.Success;
                    model.IsSuccessful = true;
                    ActivityLog.CreateActivityLog(MenuConstants.TimeTableMaster, ActionConstants.Add, DbConstants.LogType.Info, model.RowKey, model.Message);

                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    model.Message = String.Format(EduSuiteUIResources.FailedToSaveErrorMessage, EduSuiteUIResources.TimeTableMaster);
                    model.IsSuccessful = false;
                    ActivityLog.CreateActivityLog(MenuConstants.TimeTableMaster, ActionConstants.Add, DbConstants.LogType.Error, model.RowKey, ex.GetBaseException().Message);
                }
            }
            return model;

        }

        public TimeTableTempMasterViewModel UpdateTimeTableTemp(TimeTableTempMasterViewModel model)
        {
            using (var transaction = dbContext.Database.BeginTransaction())
            {
                try
                {

                    TimeTableTempMaster timeTableTempMasterModel = new TimeTableTempMaster();
                    timeTableTempMasterModel = dbContext.TimeTableTempMasters.SingleOrDefault(row => row.RowKey == model.RowKey);


                    timeTableTempMasterModel.FromDate = model.FromDate;
                    timeTableTempMasterModel.ToDate = model.ToDate;
                    timeTableTempMasterModel.EmployeeKey = model.EmployeeKey;

                    CreateTimeTableDetails(model);
                    UpdateTimeTableDetails(model);

                    dbContext.SaveChanges();
                    transaction.Commit();

                    model.Message = EduSuiteUIResources.Success;
                    model.IsSuccessful = true;
                    ActivityLog.CreateActivityLog(MenuConstants.TimeTableMaster, ActionConstants.Edit, DbConstants.LogType.Info, model.RowKey, model.Message);

                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    model.Message = String.Format(EduSuiteUIResources.FailedToSaveErrorMessage, EduSuiteUIResources.TimeTableMaster);
                    model.IsSuccessful = false;
                    ActivityLog.CreateActivityLog(MenuConstants.TimeTableMaster, ActionConstants.Edit, DbConstants.LogType.Error, model.RowKey, ex.GetBaseException().Message);
                }
            }
            return model;
        }
        private void CreateTimeTableDetails(TimeTableTempMasterViewModel modelMaster)
        {
            long MaxKey = dbContext.TimeTableTempDetails.Select(p => p.RowKey).DefaultIfEmpty().Max();
            var modelList = modelMaster.TimeTableTempDetailsModel.Where(row => row.RowKey == 0);

            foreach (TimeTableTempDetailsModel model in modelList)
            {
                TimeTableTempDetail timeTableTempDetailsModel = new TimeTableTempDetail();

                timeTableTempDetailsModel.RowKey = ++MaxKey;
                timeTableTempDetailsModel.EmployeeKey = model.ToEmployeeKey;
                timeTableTempDetailsModel.Day = model.Day;
                timeTableTempDetailsModel.PeriodKey = model.PeriodKey;
                timeTableTempDetailsModel.ClassDetailsKey = model.ClassDetailsKey;
                timeTableTempDetailsModel.SubjectKey = model.SubjectKey;
                timeTableTempDetailsModel.TempMasterKey = modelMaster.RowKey;
                dbContext.TimeTableTempDetails.Add(timeTableTempDetailsModel);

            }
        }

        private void UpdateTimeTableDetails(TimeTableTempMasterViewModel modelMaster)
        {

            var modelList = modelMaster.TimeTableTempDetailsModel.Where(row => row.RowKey != 0);

            foreach (TimeTableTempDetailsModel model in modelList)
            {

                TimeTableTempDetail timeTableTempDetailsModel = new TimeTableTempDetail();

                timeTableTempDetailsModel = dbContext.TimeTableTempDetails.SingleOrDefault(x => x.RowKey == model.RowKey);
                timeTableTempDetailsModel.EmployeeKey = model.ToEmployeeKey;
                timeTableTempDetailsModel.Day = model.Day;
                timeTableTempDetailsModel.PeriodKey = model.PeriodKey;
                timeTableTempDetailsModel.ClassDetailsKey = model.ClassDetailsKey;
                timeTableTempDetailsModel.SubjectKey = model.SubjectKey;
            }
        }


        public TimeTableTempMasterViewModel DeleteTimeTableTempMaster(long? Id)
        {
            TimeTableTempMasterViewModel model = new TimeTableTempMasterViewModel();

            using (var transaction = dbContext.Database.BeginTransaction())
            {
                try
                {
                    TimeTableTempMaster timeTableTempMaster = dbContext.TimeTableTempMasters.SingleOrDefault(x => x.RowKey == Id);
                    List<TimeTableTempDetail> timeTableTempDetailList = dbContext.TimeTableTempDetails.Where(x => x.TempMasterKey == Id).ToList();

                    dbContext.TimeTableTempDetails.RemoveRange(timeTableTempDetailList);
                    dbContext.TimeTableTempMasters.Remove(timeTableTempMaster);

                    dbContext.SaveChanges();
                    transaction.Commit();
                    model.Message = EduSuiteUIResources.Success;
                    model.IsSuccessful = true;

                    ActivityLog.CreateActivityLog(MenuConstants.TimeTableMaster, ActionConstants.Delete, DbConstants.LogType.Info, Id, model.Message);
                }
                catch (DbUpdateException Ex)
                {
                    transaction.Rollback();
                    if (Ex.GetBaseException().Message.ToUpper().Contains(EduSuiteUIResources.ForeignKeyError.ToUpper()))
                    {
                        model.Message = String.Format(EduSuiteUIResources.CantDeleteErrorMessage, EduSuiteUIResources.TimeTableMaster);
                        model.IsSuccessful = false;
                        ActivityLog.CreateActivityLog(MenuConstants.TimeTableMaster, ActionConstants.Delete, DbConstants.LogType.Debug, Id, Ex.GetBaseException().Message);
                    }
                }
                catch (Exception Ex)
                {
                    transaction.Rollback();
                    model.Message = String.Format(EduSuiteUIResources.FailedToDeleteErrorMessage, EduSuiteUIResources.TimeTableMaster);
                    model.IsSuccessful = false;
                    ActivityLog.CreateActivityLog(MenuConstants.TimeTableMaster, ActionConstants.Delete, DbConstants.LogType.Error, Id, Ex.GetBaseException().Message);
                }
            }
            return model;
        }

        public TimeTableTempMasterViewModel ViewTimeTableEmployee(TimeTableTempMasterViewModel model)
        {
            try
            {
                if (!DbConstants.Role.AdminUserTypes.Contains(DbConstants.User.RoleKey))
                {
                    Employee Employee = dbContext.Employees.Where(x => x.AppUserKey == DbConstants.User.UserKey).SingleOrDefault();
                    if (Employee != null)
                    {
                        model.EmployeeKey = Employee.RowKey;
                        model.Day = Convert.ToByte(DateTimeUTC.Now.Day);
                    }
                }
                //model = dbContext.TimeTableTempMasters.Select(row => new TimeTableTempMasterViewModel
                //{
                //    RowKey = row.RowKey,
                //    EmployeeKey = row.EmployeeKey,
                //    FromDate = row.FromDate,
                //    ToDate = row.ToDate,

                //}).Where(row => row.RowKey == model.RowKey).FirstOrDefault();
                //if (model == null)
                //{
                //    model = new TimeTableTempMasterViewModel();
                //}

                FillWeekDays(model);
                return model;
            }
            catch (Exception Ex)
            {
                ActivityLog.CreateActivityLog(MenuConstants.StudentTimeTable, ActionConstants.View, DbConstants.LogType.Error, null, Ex.GetBaseException().Message);
                return new TimeTableTempMasterViewModel();
            }
        }


        public void FillTimeTableEmployee(TimeTableTempMasterViewModel model)
        {
            

            model.TimeTableTempDetailsModel = dbContext.StudentTimeTables.Where(x => x.EmployeeKey == model.EmployeeKey).Select(row => new TimeTableTempDetailsModel
                {
                    RowKey = row.RowKey,
                    SubjectKey = row.SubjectKey,
                    SubjectName = row.Subject.SubjectName,
                    ToEmployeeKey = row.EmployeeKey,
                    EmployeeName = row.Employee.FirstName,
                    Day = row.Day,
                    PeriodKey = row.PeriodKey,
                    TempMasterKey = 0,
                    ClassDetailsKey = row.ClassDetailsKey,
                    ClassDetailsName = row.ClassDetail.ClassCode,
                    FromDate = null,
                    ToDate = null,
                }).Union(dbContext.TimeTableTempDetails.Where(row => row.EmployeeKey == model.EmployeeKey && System.Data.Entity.DbFunctions.TruncateTime(DateTimeUTC.Now) >= System.Data.Entity.DbFunctions.TruncateTime(row.TimeTableTempMaster.FromDate) && System.Data.Entity.DbFunctions.TruncateTime(DateTimeUTC.Now) <= System.Data.Entity.DbFunctions.TruncateTime(row.TimeTableTempMaster.ToDate)).Select(row => new TimeTableTempDetailsModel
                {
                    RowKey = row.RowKey,
                    SubjectKey = row.SubjectKey,
                    SubjectName = row.Subject.SubjectName,
                    ToEmployeeKey = row.EmployeeKey,
                    EmployeeName = row.Employee.FirstName,
                    Day = row.Day,
                    PeriodKey = row.PeriodKey,
                    TempMasterKey = row.TempMasterKey,
                    ClassDetailsKey = row.ClassDetailsKey,
                    ClassDetailsName = row.ClassDetail.ClassCode,
                    FromDate = row.TimeTableTempMaster.FromDate,
                    ToDate = row.TimeTableTempMaster.FromDate,
                })).ToList();

            //&& System.Data.Entity.DbFunctions.TruncateTime(row.TimeTableTempMaster.FromDate) >= System.Data.Entity.DbFunctions.TruncateTime(date) && System.Data.Entity.DbFunctions.TruncateTime(row.TimeTableTempMaster.ToDate) <= System.Data.Entity.DbFunctions.TruncateTime(date)



            if (model.TimeTableTempDetailsModel.Count == 0)
            {
                model.TimeTableTempDetailsModel.Add(new TimeTableTempDetailsModel());
            }
            FillDropDownList(model);
            FillWeekDays(model);

        }

    }
}
