using CITS.EduSuite.Business.Common;
using CITS.EduSuite.Business.Interfaces;
using CITS.EduSuite.Business.Models.Resources;
using CITS.EduSuite.Business.Models.ViewModels;
using CITS.EduSuite.Data;
using System;
using System.Collections.Generic;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CITS.EduSuite.Business.Services
{
    public class ShiftService : IShiftService
    {
        private EduSuiteDatabase dbContext;

        public ShiftService(EduSuiteDatabase objDB)
        {
            this.dbContext = objDB;
        }
        public List<ShiftViewModel> GetShift(string searchText)
        {
            try
            {
                var shiftList = (from e in dbContext.Shifts
                                 select new ShiftViewModel
                                 {
                                     MasterRowKey = e.RowKey,
                                     ShiftName = e.ShiftName,
                                     ShiftCode = e.ShiftCode,
                                     BeginTime = e.BeginTime,
                                     EndTime = e.EndTime,
                                     //IfBreak1=e.IfBreak1,
                                     //Break1BeginTime = e.Break1BeginTime,
                                     //Break1EndTime = e.Break1EndTime,
                                     //IfBreak2 = e.IfBreak2,
                                     //Break2BeginTime = e.Break2BeginTime,
                                     //Break2EndTime = e.Break2EndTime,
                                     PartialDayBeginTime = e.PartialDayBeginTime,
                                     PartialDayEndTime = e.PartialDayEndTime,
                                     PunchBeginBefore = e.PunchBeginBefore,
                                     PunchEndAfter = e.PunchEndAfter
                                 }).ToList();

                return shiftList.GroupBy(x => x.MasterRowKey).Select(y => y.First()).ToList<ShiftViewModel>();
            }
            catch (Exception ex)
            {
                ActivityLog.CreateActivityLog(MenuConstants.Shift, ActionConstants.MenuAccess, DbConstants.LogType.Error, DbConstants.User.UserKey, ex.GetBaseException().Message);
                return new List<ShiftViewModel>();
            }
        }

        public ShiftViewModel GetShiftById(int? id)
        {
            try
            {
                ShiftViewModel model = new ShiftViewModel();
                model = dbContext.Shifts.Select(row => new ShiftViewModel
                {
                    MasterRowKey = row.RowKey,
                    ShiftName = row.ShiftName,
                    ShiftCode = row.ShiftCode,
                    BeginTime = row.BeginTime,
                    EndTime = row.EndTime,
                    //IfBreak1 = row.IfBreak1,
                    //IfBreak2 = row.IfBreak2,
                    //Break1BeginTime = row.Break1BeginTime,
                    //Break1EndTime = row.Break1EndTime,
                    //Break2BeginTime = row.Break2BeginTime,
                    //Break2EndTime = row.Break2EndTime,
                    PartialDay = row.PartialDay,
                    PartialDayKey = row.PartialDayKey,
                    PartialDayBeginTime = row.PartialDayBeginTime,
                    PartialDayEndTime = row.PartialDayEndTime,
                    PunchBeginBefore = row.PunchBeginBefore,
                    PunchEndAfter = row.PunchEndAfter,
                    IsActive = row.IsActive ?? true

                }).Where(x => x.MasterRowKey == id).FirstOrDefault();
                if (model == null)
                {
                    model = new ShiftViewModel();
                }
                FillShiftBreaks(model);
                FillWeekDays(model);
                return model;
            }
            catch (Exception ex)
            {
                ActivityLog.CreateActivityLog(MenuConstants.Shift, ((id ?? 0) != 0 ? ActionConstants.Edit : ActionConstants.Add), DbConstants.LogType.Error, id, ex.GetBaseException().Message);
                return new ShiftViewModel();
            }
        }

        public ShiftViewModel CreateShift(ShiftViewModel model)
        {

            var ShiftExist = dbContext.Shifts.Where(row => row.BeginTime == model.BeginTime && row.EndTime == model.EndTime).ToList();
            FillWeekDays(model);
            Shift shiftModel = new Shift();

            if (ShiftExist.Count != 0)
            {
                model.Message = String.Format(EduSuiteUIResources.ExistsErrorMessage, EduSuiteUIResources.Shift);
                model.IsSuccessful = false;
                return model;
            }

            using (var transaction = dbContext.Database.BeginTransaction())
            {
                try
                {
                    //TimeSpan BeginTime = (TimeSpan)model.BeginTime;
                    //TimeSpan EndTime = (TimeSpan)model.EndTime;
                    //TimeSpan Break1BeginTime = (TimeSpan)model.Break1BeginTime;
                    //TimeSpan Break1EndTime = (TimeSpan)model.Break1EndTime;
                    //TimeSpan Break2BeginTime = (TimeSpan)model.Break2BeginTime;
                    //TimeSpan Break2EndTime = (TimeSpan)model.Break2EndTime;
                    //TimeSpan PartialDayBeginTime = (TimeSpan)model.PartialDayBeginTime;
                    //TimeSpan PartialDayEndTime = (TimeSpan)model.PartialDayEndTime;





                    int maxKey = dbContext.Shifts.Select(p => p.RowKey).DefaultIfEmpty().Max();

                    shiftModel.RowKey = maxKey + 1;
                    shiftModel.ShiftName = model.ShiftName;
                    shiftModel.ShiftCode = model.ShiftCode;
                    shiftModel.BeginTime = model.BeginTime ?? TimeSpan.Zero;
                    shiftModel.EndTime = model.EndTime ?? TimeSpan.Zero;
                    //shiftModel.IfBreak1 = model.IfBreak1;
                    //shiftModel.Break1BeginTime = model.Break1BeginTime;
                    //shiftModel.Break1EndTime = model.Break1EndTime;
                    //shiftModel.IfBreak2 = model.IfBreak2;
                    //shiftModel.Break2BeginTime = model.Break2BeginTime;
                    //shiftModel.Break2EndTime = model.Break2EndTime;
                    shiftModel.PunchBeginBefore = model.PunchBeginBefore;
                    shiftModel.PunchEndAfter = model.PunchEndAfter;
                    shiftModel.PartialDay = model.PartialDay;
                    shiftModel.PartialDayKey = model.PartialDayKey;
                    shiftModel.PartialDayBeginTime = model.PartialDayBeginTime;
                    shiftModel.PartialDayEndTime = model.PartialDayEndTime;
                    shiftModel.IsActive = model.IsActive;
                    CreateShiftBreaks(model.ShiftBreaks.Where(row => row.BreakBeginTime != null).ToList(), shiftModel.RowKey);

                    dbContext.Shifts.Add(shiftModel);
                    dbContext.SaveChanges();
                    transaction.Commit();
                    model.Message = EduSuiteUIResources.Success;
                    model.IsSuccessful = true;
                    ActivityLog.CreateActivityLog(MenuConstants.Shift, ActionConstants.Add, DbConstants.LogType.Info, shiftModel.RowKey, model.Message);
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    model.Message = String.Format(EduSuiteUIResources.FailedToSaveErrorMessage, EduSuiteUIResources.Shift);
                    model.IsSuccessful = false;
                    ActivityLog.CreateActivityLog(MenuConstants.Shift, ActionConstants.Add, DbConstants.LogType.Error, shiftModel.RowKey, ex.GetBaseException().Message);
                }
            }
            //}
            return model;
        }

        public ShiftViewModel UpdateShift(ShiftViewModel model)
        {

            var shiftCheck = dbContext.Shifts.Where(row => row.BeginTime == model.BeginTime && row.EndTime == model.EndTime && row.RowKey != model.MasterRowKey).ToList();
            FillWeekDays(model);
            Shift shiftModel = new Shift();

            if (shiftCheck.Count != 0)
            {
                model.Message = String.Format(EduSuiteUIResources.ExistsErrorMessage, EduSuiteUIResources.Shift);
                model.IsSuccessful = false;
                return model;
            }

            using (var transaction = dbContext.Database.BeginTransaction())
            {
                try
                {
                    //TimeSpan BeginTime = (TimeSpan)model.BeginTime;
                    //TimeSpan EndTime = (TimeSpan)model.EndTime;
                    //TimeSpan Break1BeginTime = (TimeSpan)model.Break1BeginTime;
                    //TimeSpan Break1EndTime = (TimeSpan)model.Break1EndTime;
                    //TimeSpan Break2BeginTime = (TimeSpan)model.Break2BeginTime;
                    //TimeSpan Break2EndTime = (TimeSpan)model.Break2EndTime;
                    //TimeSpan PartialDayBeginTime = (TimeSpan)model.PartialDayBeginTime;
                    //TimeSpan PartialDayEndTime = (TimeSpan)model.PartialDayEndTime;

                    shiftModel = dbContext.Shifts.SingleOrDefault(row => row.RowKey == model.MasterRowKey);
                    shiftModel.ShiftName = model.ShiftName;
                    shiftModel.ShiftCode = model.ShiftCode;
                    shiftModel.BeginTime = model.BeginTime ?? TimeSpan.Zero;
                    shiftModel.EndTime = model.EndTime ?? TimeSpan.Zero;
                    //shiftModel.IfBreak1 = model.IfBreak1;
                    //shiftModel.Break1BeginTime = model.Break1BeginTime;
                    //shiftModel.Break1EndTime = model.Break1EndTime;
                    //shiftModel.IfBreak2 = model.IfBreak2;
                    //shiftModel.Break2BeginTime = model.Break2BeginTime;
                    //shiftModel.Break2EndTime = model.Break2EndTime;
                    shiftModel.PunchBeginBefore = model.PunchBeginBefore;
                    shiftModel.PunchEndAfter = model.PunchEndAfter;
                    shiftModel.PartialDay = model.PartialDay;
                    shiftModel.PartialDayKey = model.PartialDayKey;
                    shiftModel.PartialDayBeginTime = model.PartialDayBeginTime;
                    shiftModel.PartialDayEndTime = model.PartialDayEndTime;
                    shiftModel.IsActive = model.IsActive;

                    UpdateShiftBreaks(model.ShiftBreaks.Where(row => row.BreakBeginTime != null && row.RowKey != 0).ToList());
                    CreateShiftBreaks(model.ShiftBreaks.Where(row => row.BreakBeginTime != null && row.RowKey == 0).ToList(), shiftModel.RowKey);

                    dbContext.SaveChanges();
                    transaction.Commit();
                    model.Message = EduSuiteUIResources.Success;
                    model.IsSuccessful = true;
                    ActivityLog.CreateActivityLog(MenuConstants.Shift, ActionConstants.Edit, DbConstants.LogType.Info, model.MasterRowKey, model.Message);
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    model.Message = String.Format(EduSuiteUIResources.FailedToSaveErrorMessage, EduSuiteUIResources.Shift);
                    model.IsSuccessful = false;
                    ActivityLog.CreateActivityLog(MenuConstants.Shift, ActionConstants.Edit, DbConstants.LogType.Error, model.MasterRowKey, ex.GetBaseException().Message);
                }
            }
            return model;
        }

        public ShiftViewModel DeleteShift(ShiftViewModel model)
        {
            //using (cPOSEntities dbContext = new cPOSEntities())
            //{
            using (var transaction = dbContext.Database.BeginTransaction())
            {
                try
                {
                    Shift shift = dbContext.Shifts.SingleOrDefault(row => row.RowKey == model.MasterRowKey);
                    dbContext.Shifts.Remove(shift);
                    dbContext.SaveChanges();
                    transaction.Commit();
                    model.Message = EduSuiteUIResources.Success;
                    model.IsSuccessful = true;
                    ActivityLog.CreateActivityLog(MenuConstants.Shift, ActionConstants.Delete, DbConstants.LogType.Info, model.MasterRowKey, model.Message);
                }
                catch (DbUpdateException ex)
                {
                    transaction.Rollback();
                    if (ex.GetBaseException().Message.ToUpper().Contains(EduSuiteUIResources.ForeignKeyError.ToUpper()))
                    {
                        model.Message = String.Format(EduSuiteUIResources.CantDeleteErrorMessage, EduSuiteUIResources.Shift);
                        model.IsSuccessful = false;
                    }
                    ActivityLog.CreateActivityLog(MenuConstants.Shift, ActionConstants.Delete, DbConstants.LogType.Error, model.MasterRowKey, ex.GetBaseException().Message);
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    model.Message = String.Format(EduSuiteUIResources.FailedToDeleteErrorMessage, EduSuiteUIResources.Shift);
                    model.IsSuccessful = false;
                    ActivityLog.CreateActivityLog(MenuConstants.Shift, ActionConstants.Delete, DbConstants.LogType.Error, model.MasterRowKey, ex.GetBaseException().Message);
                }
            }
            //}
            return model;
        }

        public ShiftViewModel DeleteShiftBreak(ShiftBreakModel objViewModel)
        {
            ShiftViewModel model = new ShiftViewModel();
            using (var transaction = dbContext.Database.BeginTransaction())
            {
                try
                {
                    ShiftBreak ShiftModel = dbContext.ShiftBreaks.SingleOrDefault(row => row.RowKey == objViewModel.RowKey);

                    dbContext.ShiftBreaks.Remove(ShiftModel);
                    dbContext.SaveChanges();
                    transaction.Commit();
                    model.Message = EduSuiteUIResources.Success;
                    model.IsSuccessful = true;
                    ActivityLog.CreateActivityLog(MenuConstants.Shift, ActionConstants.Delete, DbConstants.LogType.Info, objViewModel.RowKey, model.Message);

                }
                catch (DbUpdateException ex)
                {
                    transaction.Rollback();
                    if (ex.GetBaseException().Message.ToUpper().Contains(EduSuiteUIResources.ForeignKeyError.ToUpper()))
                    {
                        //model.ExceptionMessage = ex.GetBaseException().Message;
                        model.Message = String.Format(EduSuiteUIResources.CantDeleteErrorMessage, EduSuiteUIResources.Shift);
                        model.IsSuccessful = false;
                    }
                    ActivityLog.CreateActivityLog(MenuConstants.Shift, ActionConstants.Delete, DbConstants.LogType.Error, objViewModel.RowKey, ex.GetBaseException().Message);

                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    //model.ExceptionMessage = ex.GetBaseException().Message;
                    model.Message = String.Format(EduSuiteUIResources.FailedToDeleteErrorMessage, EduSuiteUIResources.Shift);
                    model.IsSuccessful = false;
                    ActivityLog.CreateActivityLog(MenuConstants.Shift, ActionConstants.Delete, DbConstants.LogType.Error, objViewModel.RowKey, ex.GetBaseException().Message);

                }

            }
            return model;
        }

        private void CreateShiftBreaks(List<ShiftBreakModel> modelList, int ShiftKey)
        {
            long Maxkey = dbContext.ShiftBreaks.Select(x => x.RowKey).DefaultIfEmpty().Max();
            foreach (ShiftBreakModel model in modelList)
            {
                ShiftBreak ShiftBreak = new ShiftBreak();
                ShiftBreak.RowKey = ++Maxkey;
                ShiftBreak.ShiftKey = ShiftKey;
                ShiftBreak.BreakName = model.BreakName;
                ShiftBreak.BreakBeginTime = model.BreakBeginTime;
                ShiftBreak.BreakEndTime = model.BreakEndTime;

                dbContext.ShiftBreaks.Add(ShiftBreak);
            }
        }
        private void UpdateShiftBreaks(List<ShiftBreakModel> modelList)
        {
            foreach (ShiftBreakModel model in modelList)
            {
                ShiftBreak ShiftBreak = dbContext.ShiftBreaks.SingleOrDefault(x => x.RowKey == model.RowKey);
                ShiftBreak.BreakName = model.BreakName;
                ShiftBreak.BreakBeginTime = model.BreakBeginTime;
                ShiftBreak.BreakEndTime = model.BreakEndTime;
            }
        }

        private void FillShiftBreaks(ShiftViewModel model)
        {
            model.ShiftBreaks = dbContext.ShiftBreaks.Where(row => row.ShiftKey == model.MasterRowKey).AsEnumerable().Select(row => new ShiftBreakModel
            {
                RowKey = row.RowKey,
                BreakName = row.BreakName,
                BreakBeginTime = row.BreakBeginTime,
                BreakEndTime = row.BreakEndTime,
            }).ToList();
            if (model.ShiftBreaks.Count == 0)
            {
                model.ShiftBreaks.Add(new ShiftBreakModel());
            }
        }

        private void FillWeekDays(ShiftViewModel model)
        {
            model.WeekDays = typeof(DbConstants.WeekDays).GetFields().Select(row => new SelectListModel
            {
                RowKey = Convert.ToByte(row.GetValue(null).ToString()),
                Text = row.Name
            }).ToList();
        }

        public ShiftViewModel CheckshiftCodeExists(string ShiftCode, int RowKey)
        {
            ShiftViewModel model = new ShiftViewModel();
            if (dbContext.Shifts.Where(row => row.ShiftCode == ShiftCode && row.RowKey != RowKey).Any())
            {
                model.IsSuccessful = false;
                model.Message = String.Format(EduSuiteUIResources.ExistsErrorMessage, EduSuiteUIResources.ShiftCode);
            }
            else
            {
                model.IsSuccessful = true;
            }
            return model;
        }



    }
}
