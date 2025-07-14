using CITS.EduSuite.Business.Common;
using CITS.EduSuite.Business.Interfaces;
using CITS.EduSuite.Business.Models.Resources;
using CITS.EduSuite.Business.Models.Security;
using CITS.EduSuite.Business.Models.ViewModels;
using CITS.EduSuite.Data;
using System;
using System.Collections.Generic;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace CITS.EduSuite.Business.Services
{
    public class AttendanceCategoryService : IAttendanceCategoryService
    {
        private EduSuiteDatabase dbContext;
        public AttendanceCategoryService(EduSuiteDatabase objDB)
        {
            this.dbContext = objDB;
        }

        public List<AttendanceCategoryViewModel> GetAttendanceCategory(string SearchText)
        {
            try
            {
                var MaterialTypeList = (from AC in dbContext.AttendanceCategories
                                        orderby AC.RowKey
                                        where (AC.AttendanceCategoryName.Contains(SearchText))
                                        select new AttendanceCategoryViewModel
                                        {
                                            MasterRowKey = AC.RowKey,
                                            AttendanceCategoryCode = AC.AttendanceCategoryCode,
                                            AttendanceCategoryName = AC.AttendanceCategoryName,
                                            OverTimeFormulaKey = AC.OverTimeFormulaKey,
                                            MinOvertime = AC.MinOvertime,
                                            MaxOvertime = AC.MaxOvertime,

                                            LateComingGraceTime = AC.LateComingGraceTime,
                                            EarlyGoingGraceTime = AC.EarlyGoingGraceTime,


                                        }).ToList();
                return MaterialTypeList.GroupBy(x => x.MasterRowKey).Select(y => y.First()).ToList<AttendanceCategoryViewModel>();
            }
            catch (Exception ex)
            {
                ActivityLog.CreateActivityLog(MenuConstants.AttendanceCategory, ActionConstants.MenuAccess, DbConstants.LogType.Error, DbConstants.User.UserKey, ex.GetBaseException().Message);
                return new List<AttendanceCategoryViewModel>();

            }
        }
        public AttendanceCategoryViewModel GeAttendanceCategoryById(int? Id)
        {

            try
            {
                AttendanceCategoryViewModel model = new AttendanceCategoryViewModel();

                model = dbContext.AttendanceCategories.Select(row => new AttendanceCategoryViewModel
                {
                    MasterRowKey = row.RowKey,
                    AttendanceCategoryCode = row.AttendanceCategoryCode,
                    AttendanceCategoryName = row.AttendanceCategoryName,
                    OverTimeFormulaKey = row.OverTimeFormulaKey,
                    MinOvertime = row.MinOvertime,
                    MaxOvertime = row.MaxOvertime,
                    ConsiderFirstLastPunch = row.ConsiderFirstLastPunch ?? false,
                    LateComingGraceTime = row.LateComingGraceTime,
                    EarlyGoingGraceTime = row.EarlyGoingGraceTime,
                    ConsiderEarlyPunch = row.ConsiderEarlyPunch ?? false,
                    ConsiderLatePunch = row.ConsiderLatePunch ?? false,
                    DeductBrakeHours = row.DeductBrakeHours ?? false,
                    CalcuteHalfDay = row.CalcuteHalfDay ?? false,
                    HalfDayMins = row.HalfDayMins,
                    CalculateAbsent = row.CalculateAbsent ?? false,
                    AbsentMins = row.AbsentMins,
                    PartCalculateHalfDay = row.PartCalculateHalfDay ?? false,
                    PartHalfDayMins = row.PartHalfDayMins,
                    PartCalculateAbsent = row.PartCalculateAbsent ?? false,
                    PartAbsentMins = row.PartAbsentMins,
                    WOandHAsPreDayAbsent = row.WOandHAsPreDayAbsent ?? false,
                    WOandHAsSufDayAbsent = row.WOandHAsSufDayAbsent ?? false,
                    WOandHAsBothDayAbsent = row.WOandHAsBothDayAbsent ?? false,
                    MarkAsAbsentForLate = row.MarkAsAbsentForLate ?? false,
                    ContiousLateDay = row.ContiousLateDay,
                    AbsentDayType = row.AbsentDayType ?? 0,
                    MarkHalfdayForLaterGoing = row.MarkHalfdayForLaterGoing ?? false,
                    HalfDayLateByMins = row.HalfDayLateByMins,
                    MarkHalfdayForEarlyGoing = row.MarkHalfdayForEarlyGoing ?? false,
                    HalfDayEarlyByMins = row.HalfDayEarlyByMins,
                    IsActive = row.IsActive



                }).Where(p => p.MasterRowKey == Id).FirstOrDefault();
                if (model == null)
                {
                    model = new AttendanceCategoryViewModel();
                }
                FillAttendanceCategoryWeekOffs(model);
                FillDropDownList(model);
                return model;
            }
            catch (Exception ex)
            {
                AttendanceCategoryViewModel model = new AttendanceCategoryViewModel();
                //model.ExceptionMessage = ex.GetBaseException().Message;
                ActivityLog.CreateActivityLog(MenuConstants.AttendanceCategory, ((Id ?? 0) != 0 ? ActionConstants.Edit : ActionConstants.Add), DbConstants.LogType.Error, Id, ex.GetBaseException().Message);

                return model;
            }
        }
        public AttendanceCategoryViewModel CreateAttendanceCategory(AttendanceCategoryViewModel model)
        {
            var AttendanceCategoryName = dbContext.AttendanceCategories.Where(x => x.AttendanceCategoryName.ToLower() == model.AttendanceCategoryName.ToLower()).ToList();
            FillDropDownList(model);

            if (AttendanceCategoryName.Count != 0)
            {
                model.Message = String.Format(EduSuiteUIResources.ExistsErrorMessage, EduSuiteUIResources.AttendanceCategory);
                model.IsSuccessful = false;
                return model;
            }
            using (var transaction = dbContext.Database.BeginTransaction())
            {
                try
                {
                    short Maxkey = dbContext.AttendanceCategories.Select(x => x.RowKey).DefaultIfEmpty().Max();


                    AttendanceCategory AttendanceCategoryViewModel = new AttendanceCategory();
                    AttendanceCategoryViewModel.RowKey = Convert.ToInt16(Maxkey + 1);
                    AttendanceCategoryViewModel.AttendanceCategoryCode = model.AttendanceCategoryCode;
                    AttendanceCategoryViewModel.AttendanceCategoryName = model.AttendanceCategoryName;
                    AttendanceCategoryViewModel.OverTimeFormulaKey = model.OverTimeFormulaKey ?? 0;
                    AttendanceCategoryViewModel.MinOvertime = model.MinOvertime;
                    AttendanceCategoryViewModel.MaxOvertime = model.MaxOvertime;
                    AttendanceCategoryViewModel.ConsiderFirstLastPunch = model.ConsiderFirstLastPunch;
                    AttendanceCategoryViewModel.LateComingGraceTime = model.LateComingGraceTime;
                    AttendanceCategoryViewModel.EarlyGoingGraceTime = model.EarlyGoingGraceTime;
                    AttendanceCategoryViewModel.ConsiderEarlyPunch = model.ConsiderEarlyPunch;
                    AttendanceCategoryViewModel.ConsiderLatePunch = model.ConsiderLatePunch;
                    AttendanceCategoryViewModel.DeductBrakeHours = model.DeductBrakeHours;
                    AttendanceCategoryViewModel.CalcuteHalfDay = model.CalcuteHalfDay;
                    AttendanceCategoryViewModel.HalfDayMins = model.HalfDayMins;
                    AttendanceCategoryViewModel.CalculateAbsent = model.CalculateAbsent;
                    AttendanceCategoryViewModel.AbsentMins = model.AbsentMins;
                    AttendanceCategoryViewModel.PartCalculateHalfDay = model.PartCalculateHalfDay;
                    AttendanceCategoryViewModel.PartHalfDayMins = model.PartHalfDayMins;
                    AttendanceCategoryViewModel.PartCalculateAbsent = model.PartCalculateAbsent;
                    AttendanceCategoryViewModel.PartAbsentMins = model.PartAbsentMins;
                    AttendanceCategoryViewModel.WOandHAsPreDayAbsent = model.WOandHAsPreDayAbsent;
                    AttendanceCategoryViewModel.WOandHAsSufDayAbsent = model.WOandHAsSufDayAbsent;
                    AttendanceCategoryViewModel.WOandHAsBothDayAbsent = model.WOandHAsBothDayAbsent;
                    AttendanceCategoryViewModel.MarkAsAbsentForLate = model.MarkAsAbsentForLate;
                    AttendanceCategoryViewModel.ContiousLateDay = model.ContiousLateDay;
                    AttendanceCategoryViewModel.AbsentDayType = model.AbsentDayType;
                    AttendanceCategoryViewModel.MarkHalfdayForLaterGoing = model.MarkHalfdayForLaterGoing;
                    AttendanceCategoryViewModel.HalfDayLateByMins = model.HalfDayLateByMins;
                    AttendanceCategoryViewModel.MarkHalfdayForEarlyGoing = model.MarkHalfdayForEarlyGoing;
                    AttendanceCategoryViewModel.HalfDayEarlyByMins = model.HalfDayEarlyByMins;
                    AttendanceCategoryViewModel.IsActive = model.IsActive;

                    //AttendanceCategoryViewModel.WeekOffDay1Key = model.WeekOffDay1Key;
                    //AttendanceCategoryViewModel.WeekOffDay2Key = model.WeekOffDay2Key;
                    //AttendanceCategoryViewModel.WeekOffDayWeekKeys = model.WeekOffDayWeekKeys;
                    CreateAttendanceCategoryWeekOffs(model.AttendanceCategoryWeekOffs.Where(row => row.WeekOffDayKey != null).ToList(), AttendanceCategoryViewModel.RowKey);

                    dbContext.AttendanceCategories.Add(AttendanceCategoryViewModel);
                    dbContext.SaveChanges();
                    transaction.Commit();
                    model.MasterRowKey = AttendanceCategoryViewModel.RowKey;
                    model.Message = EduSuiteUIResources.Success;
                    model.IsSuccessful = true;
                    ActivityLog.CreateActivityLog(MenuConstants.AttendanceCategory, ActionConstants.Add, DbConstants.LogType.Info, AttendanceCategoryViewModel.RowKey, model.Message);
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    //model.ExceptionMessage = ex.GetBaseException().Message;
                    model.Message = String.Format(EduSuiteUIResources.FailedToSaveErrorMessage, EduSuiteUIResources.AttendanceCategory);
                    model.IsSuccessful = false;
                    ActivityLog.CreateActivityLog(MenuConstants.AttendanceCategory, ActionConstants.Add, DbConstants.LogType.Error, model.MasterRowKey, ex.GetBaseException().Message);
                }

                return model;



            }

        }
        public AttendanceCategoryViewModel UpdateAttendanceCategory(AttendanceCategoryViewModel model)
        {
            var AttendanceCategoryName = dbContext.AttendanceCategories.Where(x => x.AttendanceCategoryName.ToLower() == model.AttendanceCategoryName.ToLower()
             && x.RowKey != model.MasterRowKey).ToList();
            FillDropDownList(model);
            if (AttendanceCategoryName.Count != 0)
            {
                model.Message = String.Format(EduSuiteUIResources.ExistsErrorMessage, EduSuiteUIResources.AttendanceCategory);
                model.IsSuccessful = false;
                return model;
            }
            using (var transaction = dbContext.Database.BeginTransaction())
            {
                try
                {
                    AttendanceCategory AttendanceCategoryViewModel = new AttendanceCategory();
                    AttendanceCategoryViewModel = dbContext.AttendanceCategories.SingleOrDefault(x => x.RowKey == model.MasterRowKey);
                    AttendanceCategoryViewModel.AttendanceCategoryCode = model.AttendanceCategoryCode;
                    AttendanceCategoryViewModel.AttendanceCategoryName = model.AttendanceCategoryName;
                    AttendanceCategoryViewModel.OverTimeFormulaKey = model.OverTimeFormulaKey ?? 0;
                    AttendanceCategoryViewModel.MinOvertime = model.MinOvertime;
                    AttendanceCategoryViewModel.MaxOvertime = model.MaxOvertime;
                    AttendanceCategoryViewModel.ConsiderFirstLastPunch = model.ConsiderFirstLastPunch;
                    AttendanceCategoryViewModel.LateComingGraceTime = model.LateComingGraceTime;
                    AttendanceCategoryViewModel.EarlyGoingGraceTime = model.EarlyGoingGraceTime;
                    AttendanceCategoryViewModel.ConsiderEarlyPunch = model.ConsiderEarlyPunch;
                    AttendanceCategoryViewModel.ConsiderLatePunch = model.ConsiderLatePunch;
                    AttendanceCategoryViewModel.DeductBrakeHours = model.DeductBrakeHours;
                    AttendanceCategoryViewModel.CalcuteHalfDay = model.CalcuteHalfDay;
                    AttendanceCategoryViewModel.HalfDayMins = model.HalfDayMins;
                    AttendanceCategoryViewModel.CalculateAbsent = model.CalculateAbsent;
                    AttendanceCategoryViewModel.AbsentMins = model.AbsentMins;
                    AttendanceCategoryViewModel.PartCalculateHalfDay = model.PartCalculateHalfDay;
                    AttendanceCategoryViewModel.PartHalfDayMins = model.PartHalfDayMins;
                    AttendanceCategoryViewModel.PartCalculateAbsent = model.PartCalculateAbsent;
                    AttendanceCategoryViewModel.PartAbsentMins = model.PartAbsentMins;
                    AttendanceCategoryViewModel.WOandHAsPreDayAbsent = model.WOandHAsPreDayAbsent;
                    AttendanceCategoryViewModel.WOandHAsSufDayAbsent = model.WOandHAsSufDayAbsent;
                    AttendanceCategoryViewModel.WOandHAsBothDayAbsent = model.WOandHAsBothDayAbsent;
                    AttendanceCategoryViewModel.MarkAsAbsentForLate = model.MarkAsAbsentForLate;
                    AttendanceCategoryViewModel.ContiousLateDay = model.ContiousLateDay;
                    AttendanceCategoryViewModel.AbsentDayType = model.AbsentDayType;
                    AttendanceCategoryViewModel.MarkHalfdayForLaterGoing = model.MarkHalfdayForLaterGoing;
                    AttendanceCategoryViewModel.HalfDayLateByMins = model.HalfDayLateByMins;
                    AttendanceCategoryViewModel.MarkHalfdayForEarlyGoing = model.MarkHalfdayForEarlyGoing;
                    AttendanceCategoryViewModel.HalfDayEarlyByMins = model.HalfDayEarlyByMins;
                    AttendanceCategoryViewModel.IsActive = model.IsActive;
                    //AttendanceCategoryViewModel.WeekOffDay1Key = model.WeekOffDay1Key;
                    //AttendanceCategoryViewModel.WeekOffDay2Key = model.WeekOffDay2Key;
                    //AttendanceCategoryViewModel.WeekOffDayWeekKeys = model.WeekOffDayWeekKeys;

                    UpdateAttendanceCategoryWeekOffs(model.AttendanceCategoryWeekOffs.Where(row => row.WeekOffDayKey != null && row.RowKey != 0).ToList());
                    CreateAttendanceCategoryWeekOffs(model.AttendanceCategoryWeekOffs.Where(row => row.WeekOffDayKey != null && row.RowKey == 0).ToList(), AttendanceCategoryViewModel.RowKey);

                    model.RefKey = AttendanceCategoryViewModel.RefKey;

                    dbContext.SaveChanges();
                    transaction.Commit();
                    model.Message = EduSuiteUIResources.Success;
                    model.IsSuccessful = true;
                    ActivityLog.CreateActivityLog(MenuConstants.AttendanceCategory, ActionConstants.Edit, DbConstants.LogType.Info, model.MasterRowKey, model.Message);

                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    //model.ExceptionMessage = ex.GetBaseException().Message;
                    model.Message = String.Format(EduSuiteUIResources.FailedToSaveErrorMessage, EduSuiteUIResources.AttendanceCategory);
                    model.IsSuccessful = false;
                    ActivityLog.CreateActivityLog(MenuConstants.AttendanceCategory, ActionConstants.Edit, DbConstants.LogType.Error, model.MasterRowKey, ex.GetBaseException().Message);

                }

                return model;



            }

        }

        public AttendanceCategoryViewModel DeleteAttendanceCategory(AttendanceCategoryViewModel model)
        {
            using (var transaction = dbContext.Database.BeginTransaction())
            {
                try
                {
                    AttendanceCategory AttendanceCategoryViewModel = dbContext.AttendanceCategories.SingleOrDefault(row => row.RowKey == model.MasterRowKey);

                    dbContext.AttendanceCategoryWeekOffs.RemoveRange(AttendanceCategoryViewModel.AttendanceCategoryWeekOffs);
                    dbContext.AttendanceCategories.Remove(AttendanceCategoryViewModel);
                    dbContext.SaveChanges();
                    transaction.Commit();
                    model.Message = EduSuiteUIResources.Success;
                    model.IsSuccessful = true;
                    ActivityLog.CreateActivityLog(MenuConstants.AttendanceCategory, ActionConstants.Delete, DbConstants.LogType.Info, model.MasterRowKey, model.Message);

                }
                catch (DbUpdateException ex)
                {
                    transaction.Rollback();
                    if (ex.GetBaseException().Message.ToUpper().Contains(EduSuiteUIResources.ForeignKeyError.ToUpper()))
                    {
                        //model.ExceptionMessage = ex.GetBaseException().Message;
                        model.Message = String.Format(EduSuiteUIResources.CantDeleteErrorMessage, EduSuiteUIResources.AttendanceCategory);
                        model.IsSuccessful = false;
                    }
                    ActivityLog.CreateActivityLog(MenuConstants.AttendanceCategory, ActionConstants.Delete, DbConstants.LogType.Error, model.MasterRowKey, ex.GetBaseException().Message);

                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    //model.ExceptionMessage = ex.GetBaseException().Message;
                    model.Message = String.Format(EduSuiteUIResources.FailedToDeleteErrorMessage, EduSuiteUIResources.AttendanceCategory);
                    model.IsSuccessful = false;
                    ActivityLog.CreateActivityLog(MenuConstants.AttendanceCategory, ActionConstants.Delete, DbConstants.LogType.Error, model.MasterRowKey, ex.GetBaseException().Message);

                }

            }
            return model;
        }

        public AttendanceCategoryViewModel DeleteAttendanceCategoryWeekOff(Int32 RowKey)
        {
            AttendanceCategoryViewModel model = new AttendanceCategoryViewModel();
            using (var transaction = dbContext.Database.BeginTransaction())
            {
                try
                {
                    AttendanceCategoryWeekOff AttendanceCategoryWeekOffModel = dbContext.AttendanceCategoryWeekOffs.SingleOrDefault(row => row.RowKey == RowKey);

                    dbContext.AttendanceCategoryWeekOffs.Remove(AttendanceCategoryWeekOffModel);
                    dbContext.SaveChanges();
                    transaction.Commit();
                    model.Message = EduSuiteUIResources.Success;
                    model.IsSuccessful = true;
                    ActivityLog.CreateActivityLog(MenuConstants.AttendanceCategory, ActionConstants.Delete, DbConstants.LogType.Info, RowKey, model.Message);

                }
                catch (DbUpdateException ex)
                {
                    transaction.Rollback();
                    if (ex.GetBaseException().Message.ToUpper().Contains(EduSuiteUIResources.ForeignKeyError.ToUpper()))
                    {
                        //model.ExceptionMessage = ex.GetBaseException().Message;
                        model.Message = String.Format(EduSuiteUIResources.CantDeleteErrorMessage, EduSuiteUIResources.AttendanceCategory);
                        model.IsSuccessful = false;
                        ActivityLog.CreateActivityLog(MenuConstants.AttendanceCategory, ActionConstants.Delete, DbConstants.LogType.Error, RowKey, ex.GetBaseException().Message);

                    }
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    //model.ExceptionMessage = ex.GetBaseException().Message;
                    model.Message = String.Format(EduSuiteUIResources.FailedToDeleteErrorMessage, EduSuiteUIResources.AttendanceCategory);
                    model.IsSuccessful = false;
                    ActivityLog.CreateActivityLog(MenuConstants.AttendanceCategory, ActionConstants.Delete, DbConstants.LogType.Error, RowKey, ex.GetBaseException().Message);

                }

            }
            return model;
        }

        private void CreateAttendanceCategoryWeekOffs(List<AttendanceCategoryWeekOffModel> modelList, short AttendanceCategoryKey)
        {
            int Maxkey = dbContext.AttendanceCategoryWeekOffs.Select(x => x.RowKey).DefaultIfEmpty().Max();
            foreach (AttendanceCategoryWeekOffModel model in modelList)
            {
                AttendanceCategoryWeekOff attendanceCategoryWeekOff = new AttendanceCategoryWeekOff();
                attendanceCategoryWeekOff.RowKey = ++Maxkey;
                attendanceCategoryWeekOff.AttendaceCategoryKey = AttendanceCategoryKey;
                attendanceCategoryWeekOff.WeekOffDayKey = model.WeekOffDayKey ?? 0;
                if (model.WeekOffDayWeekKeys != null)
                    attendanceCategoryWeekOff.WeekOffDayWeekKeys = String.Join(",", model.WeekOffDayWeekKeys);
                dbContext.AttendanceCategoryWeekOffs.Add(attendanceCategoryWeekOff);
            }
        }
        private void UpdateAttendanceCategoryWeekOffs(List<AttendanceCategoryWeekOffModel> modelList)
        {
            foreach (AttendanceCategoryWeekOffModel model in modelList)
            {
                AttendanceCategoryWeekOff attendanceCategoryWeekOff = dbContext.AttendanceCategoryWeekOffs.SingleOrDefault(x => x.RowKey == model.RowKey);
                attendanceCategoryWeekOff.WeekOffDayKey = model.WeekOffDayKey ?? 0;
                if (model.WeekOffDayWeekKeys != null)
                    attendanceCategoryWeekOff.WeekOffDayWeekKeys = String.Join(",", model.WeekOffDayWeekKeys);
                else
                    attendanceCategoryWeekOff.WeekOffDayWeekKeys = null;
            }
        }

        private void FillAttendanceCategoryWeekOffs(AttendanceCategoryViewModel model)
        {
            byte WeekKey = 0;
            model.AttendanceCategoryWeekOffs = dbContext.AttendanceCategoryWeekOffs.Where(row => row.AttendaceCategoryKey == model.MasterRowKey).AsEnumerable().Select(row => new AttendanceCategoryWeekOffModel
            {
                RowKey = row.RowKey,
                WeekOffDayKey = row.WeekOffDayKey,
                WeekOffDayWeekKeys = row.WeekOffDayWeekKeys != null ? row.WeekOffDayWeekKeys.Split(',').Select(x => { byte.TryParse(x, out WeekKey); return WeekKey; }).ToList() : null
            }).ToList();
            if (model.AttendanceCategoryWeekOffs.Count == 0)
            {
                model.AttendanceCategoryWeekOffs.Add(new AttendanceCategoryWeekOffModel());
            }
        }

        private void FillDropDownList(AttendanceCategoryViewModel model)
        {
            FillOTFormulaType(model);
            FillWeekDays(model);
            FillAbsentTypes(model);
            foreach (AttendanceCategoryWeekOffModel item in model.AttendanceCategoryWeekOffs)
            {
                FillWeeks(item);
            }

        }
        private void FillOTFormulaType(AttendanceCategoryViewModel model)
        {
            model.OverTimeFormulaType = dbContext.OverTimeFormulaTypes.Select(row => new SelectListModel
            {
                RowKey = row.RowKey,
                Text = row.OTFormulaTypeName
            }).ToList();
        }

        private void FillWeekDays(AttendanceCategoryViewModel model)
        {
            model.WeekDays = typeof(DbConstants.WeekDays).GetFields().Select(row => new SelectListModel
            {
                RowKey = Convert.ToByte(row.GetValue(null).ToString()),
                Text = row.Name
            }).ToList();
        }
        private void FillWeeks(AttendanceCategoryWeekOffModel model)
        {
            for (byte i = 1; i < 5; i++)
            {
                model.Weeks.Add(new SelectListModel
                {
                    RowKey = i,
                    Text = StringExtension.NumberToPositionString(i),
                    Selected = model.WeekOffDayWeekKeys != null && model.WeekOffDayWeekKeys.Contains(i)

                });
            }
        }
        private void FillAbsentTypes(AttendanceCategoryViewModel model)
        {
            model.AbsenseDayTypes = typeof(DbConstants.AbsentDayTypes).GetFields().Select(row => new SelectListModel
            {
                RowKey = Convert.ToByte(row.GetValue(null).ToString()),
                Text = row.Name
            }).ToList();
        }


        public AttendanceCategoryViewModel CheckAttendanceCategoryCodeExists(string AttendanceCategoryCode, short RowKey)
        {
            AttendanceCategoryViewModel model = new AttendanceCategoryViewModel();
            if (dbContext.AttendanceCategories.Where(row => row.AttendanceCategoryCode == AttendanceCategoryCode && row.RowKey != RowKey).Any())
            {
                model.IsSuccessful = false;
                model.Message = String.Format(EduSuiteUIResources.ExistsErrorMessage, EduSuiteUIResources.Code);
            }
            else
            {
                model.IsSuccessful = true;
            }
            return model;
        }


    }
}
