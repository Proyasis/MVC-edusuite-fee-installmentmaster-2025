using System;
using System.Collections.Generic;
using System.Data.Entity.Infrastructure;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CITS.EduSuite.Business.Common;
using CITS.EduSuite.Business.Interfaces;
using CITS.EduSuite.Business.Models.ViewModels;
using CITS.EduSuite.Data;
using CITS.EduSuite.Business.Models.Resources;

namespace CITS.EduSuite.Business.Services
{
    public class DepartmentShiftService : IDepartmentShiftService
    {
        private EduSuiteDatabase dbContext;

        public DepartmentShiftService(EduSuiteDatabase objDb)
        {
            this.dbContext = objDb;
        }
        public List<DepartmentShiftViewModel> GetDepartmentShifts(string SearchText)
        {
            try
            {
                var DepartmentShifts = (from DS in dbContext.DepartmentShifts
                                        where (DS.Department.DepartmentName.Contains(SearchText) || DS.Shift.ShiftName.Contains(SearchText))
                                        select new DepartmentShiftViewModel
                                        {
                                            RowKey = DS.RowKey,
                                            DepartmentKey = DS.DepartmentKey,
                                            DepartmentName = DS.Department.DepartmentName,
                                            ShiftKey = DS.ShiftKey,
                                            ShiftName = DS.Shift.ShiftName,
                                            FromDate = DS.FromDate,
                                            ToDate = DS.ToDate,
                                        }).ToList();

                return DepartmentShifts.GroupBy(x => x.RowKey).Select(y => y.First()).ToList<DepartmentShiftViewModel>();
            }
            catch (Exception ex)
            {
                ActivityLog.CreateActivityLog(MenuConstants.DepartmentShift, ActionConstants.MenuAccess, DbConstants.LogType.Error, DbConstants.User.UserKey, ex.GetBaseException().Message);
                return new List<DepartmentShiftViewModel>();

            }
        }

        public DepartmentShiftViewModel GetDepartmentShiftById(int id)
        {
            try
            {
                DepartmentShiftViewModel model = new DepartmentShiftViewModel();

                model = dbContext.DepartmentShifts.Select(row => new DepartmentShiftViewModel
                {
                    RowKey = row.RowKey,
                    DepartmentKey = row.DepartmentKey,
                    ShiftKey = row.ShiftKey,
                    FromDate = row.FromDate,
                    ToDate = row.ToDate,
                }).Where(x => x.RowKey == id).FirstOrDefault();
                if (model == null)
                {
                    model = new DepartmentShiftViewModel();
                }
                FillDropdownLists(model);
                return model;
            }
            catch (Exception ex)
            {
                ActivityLog.CreateActivityLog(MenuConstants.DepartmentShift,(id!=0 ? ActionConstants.Edit: ActionConstants.Add), DbConstants.LogType.Error, id, ex.GetBaseException().Message);
                return new DepartmentShiftViewModel();


            }
        }

        public DepartmentShiftViewModel CreateDepartmentShift(DepartmentShiftViewModel model)
        {
            FillDropdownLists(model);
            DepartmentShift DepartmentShiftModel = new DepartmentShift();
            var DepartmentShiftCheck = dbContext.DepartmentShifts.Where(x => DbFunctions.TruncateTime(x.FromDate) == DbFunctions.TruncateTime(model.FromDate) && DbFunctions.TruncateTime(x.ToDate) == DbFunctions.TruncateTime(model.ToDate)
                    && x.DepartmentKey == model.DepartmentKey && x.ShiftKey == model.ShiftKey)
                .ToList();

            if (DepartmentShiftCheck.Count != 0)
            {
                model.Message = String.Format(EduSuiteUIResources.ExistsErrorMessage, EduSuiteUIResources.DepartmentShift);
                model.IsSuccessful = false;
                return model;
            }

            using (var transaction = dbContext.Database.BeginTransaction())
            {
                try
                {
                    long maxKey = dbContext.DepartmentShifts.Select(p => p.RowKey).DefaultIfEmpty().Max();
                    DepartmentShiftModel.RowKey = maxKey + 1;
                    DepartmentShiftModel.DepartmentKey = model.DepartmentKey;
                    DepartmentShiftModel.ShiftKey = model.ShiftKey;
                    DepartmentShiftModel.FromDate = model.FromDate;
                    DepartmentShiftModel.ToDate = model.ToDate;
                    dbContext.DepartmentShifts.Add(DepartmentShiftModel);
                    dbContext.SaveChanges();
                    transaction.Commit();
                    model.Message = EduSuiteUIResources.Success;
                    model.IsSuccessful = true;
                    ActivityLog.CreateActivityLog(MenuConstants.DepartmentShift, ActionConstants.Add, DbConstants.LogType.Info, DepartmentShiftModel.RowKey, model.Message);


                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    model.Message = String.Format(EduSuiteUIResources.FailedToSaveErrorMessage, EduSuiteUIResources.DepartmentShift);
                    model.IsSuccessful = false;
                    ActivityLog.CreateActivityLog(MenuConstants.DepartmentShift, ActionConstants.Add, DbConstants.LogType.Error, model.RowKey, ex.GetBaseException().Message);


                }
            }
            return model;
        }

        public DepartmentShiftViewModel UpdateDepartmentShift(DepartmentShiftViewModel model)
        {
            FillDropdownLists(model);
            DepartmentShift DepartmentShiftModel = new DepartmentShift();
            var DepartmentShiftCheck = dbContext.DepartmentShifts.Where(x => x.RowKey != model.RowKey && DbFunctions.TruncateTime(x.FromDate) == DbFunctions.TruncateTime(model.FromDate) && DbFunctions.TruncateTime(x.ToDate) == DbFunctions.TruncateTime(model.ToDate)
                              && x.DepartmentKey == model.DepartmentKey && x.ShiftKey == model.ShiftKey)
                          .ToList();


            if (DepartmentShiftCheck.Count != 0)
            {
                model.Message = String.Format(EduSuiteUIResources.ExistsErrorMessage, EduSuiteUIResources.DepartmentShift);
                model.IsSuccessful = false;
                return model;
            }

            using (var transaction = dbContext.Database.BeginTransaction())
            {
                try
                {
                    DepartmentShiftModel = dbContext.DepartmentShifts.SingleOrDefault(row => row.RowKey == model.RowKey);
                    DepartmentShiftModel.DepartmentKey = model.DepartmentKey;
                    DepartmentShiftModel.ShiftKey = model.ShiftKey;
                    DepartmentShiftModel.FromDate = model.FromDate;
                    DepartmentShiftModel.ToDate = model.ToDate;


                    dbContext.SaveChanges();
                    transaction.Commit();
                    model.Message = EduSuiteUIResources.Success;
                    model.IsSuccessful = true;
                    ActivityLog.CreateActivityLog(MenuConstants.DepartmentShift, ActionConstants.Edit, DbConstants.LogType.Info, model.RowKey, model.Message);

                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    model.Message = String.Format(EduSuiteUIResources.FailedToSaveErrorMessage, EduSuiteUIResources.DepartmentShift);
                    model.IsSuccessful = false;
                    ActivityLog.CreateActivityLog(MenuConstants.DepartmentShift, ActionConstants.Edit, DbConstants.LogType.Error, model.RowKey, ex.GetBaseException().Message);

                }
            }
            return model;
        }

        public DepartmentShiftViewModel DeleteDepartmentShift(DepartmentShiftViewModel model)
        {
            using (var transaction = dbContext.Database.BeginTransaction())
            {
                try
                {
                    DepartmentShift DepartmentShiftModel = dbContext.DepartmentShifts.SingleOrDefault(row => row.RowKey == model.RowKey);
                    dbContext.DepartmentShifts.Remove(DepartmentShiftModel);
                    dbContext.SaveChanges();
                    transaction.Commit();
                    model.Message = EduSuiteUIResources.Success;
                    model.IsSuccessful = true;
                    ActivityLog.CreateActivityLog(MenuConstants.DepartmentShift, ActionConstants.Delete, DbConstants.LogType.Info, model.RowKey, model.Message);

                }
                catch (DbUpdateException ex)
                {
                    transaction.Rollback();
                    if (ex.GetBaseException().Message.ToUpper().Contains(EduSuiteUIResources.ForeignKeyError.ToUpper()))
                    {
                        model.Message = String.Format(EduSuiteUIResources.CantDeleteErrorMessage, EduSuiteUIResources.DepartmentShift);
                        model.IsSuccessful = false;

                    }
                    ActivityLog.CreateActivityLog(MenuConstants.DepartmentShift, ActionConstants.Delete, DbConstants.LogType.Error, model.RowKey, ex.GetBaseException().Message);

                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    model.Message = String.Format(EduSuiteUIResources.FailedToDeleteErrorMessage, EduSuiteUIResources.DepartmentShift);
                    model.IsSuccessful = false;
                    ActivityLog.CreateActivityLog(MenuConstants.DepartmentShift, ActionConstants.Delete, DbConstants.LogType.Error, model.RowKey, ex.GetBaseException().Message);

                }
            }

            return model;
        }
        private void FillShifts(DepartmentShiftViewModel model)
        {
            model.Shifts = dbContext.Shifts.Where(row => row.IsActive ?? true).Select(row => new SelectListModel
            {

                RowKey = row.RowKey,
                Text = row.ShiftName
            }).ToList();

        }
        private void FillDepartment(DepartmentShiftViewModel model)
        {
            model.Departments = dbContext.Departments.Where(x => x.IsActive == true).Select(row => new SelectListModel
            {

                RowKey = row.RowKey,
                Text = row.DepartmentName
            }).ToList();

        }

        private void FillDropdownLists(DepartmentShiftViewModel model)
        {
            FillShifts(model);
            FillDepartment(model);

        }



    }
}
