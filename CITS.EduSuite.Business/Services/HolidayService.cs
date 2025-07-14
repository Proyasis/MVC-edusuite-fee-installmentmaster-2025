using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CITS.EduSuite.Business.Common;
using CITS.EduSuite.Business.Interfaces;
using CITS.EduSuite.Business.Models.Resources;
using CITS.EduSuite.Business.Models.ViewModels;
using CITS.EduSuite.Data;

namespace CITS.EduSuite.Business.Services
{
    public class HolidayService : IHolidayService
    {
        private EduSuiteDatabase dbContext;
        public HolidayService(EduSuiteDatabase objDB)
        {
            this.dbContext = objDB;
        }

        public List<HolidayViewModel> GetHolidays(HolidayViewModel model)
        {
            try
            {
                Employee Employee = dbContext.Employees.Where(x => x.AppUserKey == DbConstants.User.UserKey).SingleOrDefault();

                short HolidayYear = (short)(model.HolidayFrom.Value.Month > model.HolidayTo.Value.Month ? model.HolidayFrom.Value.Year : model.HolidayTo.Value.Year);

                List<HolidayViewModel> holidayList = (from H in dbContext.Holidays
                                                          //where ((DbFunctions.TruncateTime(H.HolidayFrom) <= DbFunctions.TruncateTime(lastDayOfMonth) && DbFunctions.TruncateTime(H.HolidayFrom) >= DbFunctions.TruncateTime(firstDayOfMonth))
                                                      where (H.BranchKey == model.BranchKey || H.BranchKey == null)
                                                      select new HolidayViewModel
                                                      {
                                                          BranchKey = H.BranchKey,
                                                          HolidayTypeKey = H.HolidayTypeKey,
                                                          HolidayYearKey = HolidayYear,
                                                          RowKey = H.RowKey,
                                                          HolidayTitle = H.HolidayTitle,
                                                          HolidayFrom = H.HolidayFrom,
                                                          HolidayTo = H.HolidayTo,
                                                          IsDayOff = H.IsDayOff,
                                                          Remarks = H.Remarks

                                                      }).Where(row => (DbFunctions.TruncateTime(row.HolidayFrom) >= DbFunctions.TruncateTime(model.HolidayFrom) && DbFunctions.TruncateTime(row.HolidayTo) <= DbFunctions.TruncateTime(model.HolidayTo))).ToList();

                if (Employee != null)
                {
                    if (Employee.BranchAccess != null)
                    {
                        var Branches = Employee.BranchAccess.Split(',').Select(Int16.Parse).ToList();
                        holidayList = holidayList.Where(row => Branches.Contains(row.BranchKey ?? 0)).ToList();
                    }
                }
                return holidayList.GroupBy(x => x.RowKey).Select(y => y.First()).ToList<HolidayViewModel>();

            }
            catch (Exception ex)
            {
                ActivityLog.CreateActivityLog(MenuConstants.Holiday, ActionConstants.MenuAccess, DbConstants.LogType.Error, DbConstants.User.UserKey, ex.GetBaseException().Message);
                return new List<HolidayViewModel>();
            }
        }

        public HolidayViewModel GetHolidayById(HolidayViewModel model)
        {
            try
            {

                HolidayViewModel holidayViewModel = new HolidayViewModel();
                holidayViewModel = dbContext.Holidays.Select(row => new HolidayViewModel
                {
                    RowKey = row.RowKey,
                    BranchKey = row.BranchKey,
                    HolidayTitle = row.HolidayTitle,
                    HolidayFrom = row.HolidayFrom,
                    HolidayTo = row.HolidayTo,
                    IsDayOff = row.IsDayOff,
                    HolidayTypeKey = row.HolidayTypeKey,
                    Remarks = row.Remarks,
                    ClassKeys = row.HolidayClasses.Select(x => x.ClassDetailsKey).ToList()

                }).Where(x => x.RowKey == model.RowKey).FirstOrDefault();
                if (holidayViewModel == null)
                {
                    holidayViewModel = new HolidayViewModel();
                    holidayViewModel.BranchKey = model.BranchKey;
                    holidayViewModel.IsDayOff = true;
                    holidayViewModel.HolidayTypeKey = DbConstants.HolidayType.Dynamic;
                    holidayViewModel.HolidayFrom = model.HolidayFrom;
                    holidayViewModel.HolidayTo = model.HolidayTo;
                }
                FillDropdownList(holidayViewModel);
                return holidayViewModel;
            }
            catch (Exception ex)
            {
                ActivityLog.CreateActivityLog(MenuConstants.Holiday, (model.RowKey != 0 ? ActionConstants.Edit : ActionConstants.Add), DbConstants.LogType.Error, model.RowKey, ex.GetBaseException().Message);
                return new HolidayViewModel();
            }
        }

        public HolidayViewModel GetHolidayByDate(HolidayViewModel model)
        {
            try
            {

                HolidayViewModel holidayViewModel = new HolidayViewModel();
                holidayViewModel = dbContext.Holidays.Where(x => DbFunctions.TruncateTime(x.HolidayFrom) <= DbFunctions.TruncateTime(model.HolidayFrom) && DbFunctions.TruncateTime(x.HolidayTo) >= DbFunctions.TruncateTime(model.HolidayFrom)
                    && (x.BranchKey == model.BranchKey || x.BranchKey == null))
              .Select(row => new HolidayViewModel
              {
                  RowKey = row.RowKey,
                  BranchKey = row.BranchKey,
                  HolidayTitle = row.HolidayTitle,
                  HolidayFrom = row.HolidayFrom,
                  HolidayTo = row.HolidayTo,
                  Remarks = row.Remarks

              }).FirstOrDefault();
                if (holidayViewModel == null)
                {
                    holidayViewModel = new HolidayViewModel();
                    holidayViewModel.BranchKey = model.BranchKey;
                }
                return holidayViewModel;
            }
            catch (Exception ex)
            {
                return new HolidayViewModel();
            }
        }

        public HolidayViewModel CreateHoliday(HolidayViewModel model)
        {
            Holiday holidayModel = new Holiday();

            FillDropdownList(model);

            using (var transaction = dbContext.Database.BeginTransaction())
            {
                try
                {

                    Int64 maxKey = dbContext.Holidays.Select(p => p.RowKey).DefaultIfEmpty().Max();

                    holidayModel.RowKey = Convert.ToInt64(maxKey + 1);
                    holidayModel.BranchKey = model.BranchKey;
                    holidayModel.HolidayTitle = model.HolidayTitle;
                    holidayModel.HolidayFrom = model.HolidayFrom;

                    holidayModel.HolidayTo = (model.HolidayTo ?? model.HolidayFrom);
                    holidayModel.HolidayTypeKey = model.HolidayTypeKey;
                    holidayModel.IsDayOff = model.IsDayOff;
                    holidayModel.Remarks = model.Remarks;
                    dbContext.Holidays.Add(holidayModel);
                    model.RowKey = holidayModel.RowKey;
                    UpdateClassAllocation(model);
                    dbContext.SaveChanges();
                    transaction.Commit();
                    model.Message = EduSuiteUIResources.Success;
                    model.IsSuccessful = true;
                    ActivityLog.CreateActivityLog(MenuConstants.Holiday, ActionConstants.Add, DbConstants.LogType.Info, holidayModel.RowKey, model.Message);

                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    model.Message = String.Format(EduSuiteUIResources.FailedToSaveErrorMessage, EduSuiteUIResources.Holiday);
                    model.IsSuccessful = false;
                    ActivityLog.CreateActivityLog(MenuConstants.Holiday, ActionConstants.Add, DbConstants.LogType.Error, model.RowKey, ex.GetBaseException().Message);

                }
            }
            return model;
        }

        public HolidayViewModel UpdateHoliday(HolidayViewModel model)
        {
            FillDropdownList(model);

            Holiday holidayModel = new Holiday();

            using (var transaction = dbContext.Database.BeginTransaction())
            {
                try
                {
                    holidayModel = dbContext.Holidays.SingleOrDefault(row => row.RowKey == model.RowKey);
                    holidayModel.BranchKey = model.BranchKey;
                    holidayModel.HolidayTitle = model.HolidayTitle;
                    holidayModel.HolidayFrom = Convert.ToDateTime(model.HolidayFrom);
                    holidayModel.HolidayTo = Convert.ToDateTime((model.HolidayTo ?? model.HolidayFrom));
                    holidayModel.HolidayTypeKey = model.HolidayTypeKey;
                    holidayModel.IsDayOff = model.IsDayOff;
                    holidayModel.Remarks = model.Remarks;
                    UpdateClassAllocation(model);
                    dbContext.SaveChanges();
                    transaction.Commit();
                    model.Message = EduSuiteUIResources.Success;
                    model.IsSuccessful = true;
                    ActivityLog.CreateActivityLog(MenuConstants.Holiday, ActionConstants.Edit, DbConstants.LogType.Info, model.RowKey, model.Message);

                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    model.Message = String.Format(EduSuiteUIResources.FailedToSaveErrorMessage, EduSuiteUIResources.Holiday);
                    model.IsSuccessful = false;
                    ActivityLog.CreateActivityLog(MenuConstants.Holiday, ActionConstants.Edit, DbConstants.LogType.Error, model.RowKey, ex.GetBaseException().Message);

                }
            }
            return model;
        }

        public HolidayViewModel DeleteHoliday(HolidayViewModel model)
        {
            using (var transaction = dbContext.Database.BeginTransaction())
            {
                try
                {
                    Holiday holiday = dbContext.Holidays.SingleOrDefault(row => row.RowKey == model.RowKey);
                    List<HolidayClass> HolidayClasslist = dbContext.HolidayClasses.Where(x => x.HolidayKey == model.RowKey).ToList();
                    dbContext.HolidayClasses.RemoveRange(HolidayClasslist);
                    dbContext.Holidays.Remove(holiday);
                    dbContext.SaveChanges();
                    transaction.Commit();
                    model.Message = EduSuiteUIResources.Success;
                    model.IsSuccessful = true;
                    ActivityLog.CreateActivityLog(MenuConstants.Holiday, ActionConstants.Delete, DbConstants.LogType.Info, model.RowKey, model.Message);
                }
                catch (DbUpdateException ex)
                {
                    transaction.Rollback();
                    if (ex.GetBaseException().Message.ToUpper().Contains(EduSuiteUIResources.ForeignKeyError.ToUpper()))
                    {
                        model.Message = String.Format(EduSuiteUIResources.CantDeleteErrorMessage, EduSuiteUIResources.Holiday);
                        model.IsSuccessful = false;
                    }
                    ActivityLog.CreateActivityLog(MenuConstants.Holiday, ActionConstants.Delete, DbConstants.LogType.Error, model.RowKey, ex.GetBaseException().Message);
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    model.Message = String.Format(EduSuiteUIResources.FailedToDeleteErrorMessage, EduSuiteUIResources.Holiday);
                    model.IsSuccessful = false;
                    ActivityLog.CreateActivityLog(MenuConstants.Holiday, ActionConstants.Delete, DbConstants.LogType.Error, model.RowKey, ex.GetBaseException().Message);
                }
            }
            return model;
        }


        public HolidayViewModel GetBranches(HolidayViewModel model)
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

            return model;
        }


        private void FillDropdownList(HolidayViewModel model)
        {
            FillBranches(model);
            FillHolidayTypes(model);
            FillClass(model);
        }
        public void FillBranches(HolidayViewModel model)
        {
            IQueryable<SelectListModel> BranchQuery = dbContext.vwBranchSelectActiveOnlies.OrderBy(row => row.DisplayOrder).Select(row => new SelectListModel
            {
                RowKey = row.RowKey,
                Text = row.BranchName
            });



            if (!DbConstants.Role.AdminUserTypes.Contains(DbConstants.User.RoleKey))
            {
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
                    if (model.Branches.Count == 1)
                    {
                        long? branchkey = model.Branches.Select(x => x.RowKey).FirstOrDefault();
                        model.BranchKey = Convert.ToInt16(branchkey);

                    }
                }
            }
            else
            {

                model.Branches = BranchQuery.ToList();
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
                if (model.Branches.Count == 1)
                {
                    long? branchkey = model.Branches.Select(x => x.RowKey).FirstOrDefault();
                    model.BranchKey = Convert.ToInt16(branchkey);

                }
            }
        }

        public void FillClass(HolidayViewModel model)
        {
            model.ClassDetails = dbContext.VwClassDetailsSelectActiveOnlies.Where(row => row.IsActive).Select(row => new SelectListModel
            {
                RowKey = row.RowKey,
                Text = row.ClassCode + row.ClassCodeDescription
            }).ToList();
        }
        public void FillHolidayTypes(HolidayViewModel model)
        {
            model.HolidayTypes = typeof(DbConstants.HolidayType).GetFields().Select(row => new SelectListModel
            {
                RowKey = Convert.ToByte((row.GetValue(null).ToString())),
                Text = row.Name
            }).ToList();
        }

        private void UpdateClassAllocation(HolidayViewModel model)
        {
            Int64 MaxKey = dbContext.HolidayClasses.Select(x => x.RowKey).DefaultIfEmpty().Max();
            List<HolidayClass> holidayClasses = dbContext.HolidayClasses.Where(x => x.HolidayKey == model.RowKey).ToList();

            if (holidayClasses.Count > 0)
            {
                dbContext.HolidayClasses.RemoveRange(holidayClasses);
            }
            if (model.ClassKeys != null && model.ClassKeys.Count > 0)
            {
                foreach (long ClassKey in model.ClassKeys)
                {
                    HolidayClass objHolidayClassmodel = new HolidayClass();

                    objHolidayClassmodel.RowKey = MaxKey + 1;
                    objHolidayClassmodel.HolidayKey = model.RowKey;
                    objHolidayClassmodel.ClassDetailsKey = ClassKey;

                    dbContext.HolidayClasses.Add(objHolidayClassmodel);
                    dbContext.SaveChanges();
                    MaxKey++;
                }
            }
        }


    }
}
