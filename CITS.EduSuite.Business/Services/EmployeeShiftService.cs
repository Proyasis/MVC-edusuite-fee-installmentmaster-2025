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
    public class EmployeeShiftService : IEmployeeShiftService
    {
        private EduSuiteDatabase dbContext;

        public EmployeeShiftService(EduSuiteDatabase objDb)
        {
            this.dbContext = objDb;
        }
        public List<EmployeeShiftViewModel> GetEmployeeShifts(string SearchText)
        {
            try
            {
                var EmployeeShiftsList = (from ES in dbContext.EmployeeShifts
                                          where (ES.Shift.ShiftName.Contains(SearchText))

                                          select new EmployeeShiftViewModel
                                          {
                                              RowKey = ES.RowKey,
                                              //EmployeeKey = DS.EmployeeKey,
                                              //EmployeeName = DS.Employee.FirstName + " " + (DS.Employee.MiddleName ?? "") + " " + DS.Employee.LastName,
                                              ShiftKey = ES.ShiftKey,
                                              ShiftName = ES.Shift.ShiftName,
                                              FromDate = ES.FromDate,
                                              ToDate = ES.ToDate,
                                              EmployeeCount = ES.EmployeeShiftDetails.Count(x => x.IsActive)
                                          }).ToList();

                return EmployeeShiftsList.GroupBy(x => x.RowKey).Select(y => y.First()).ToList<EmployeeShiftViewModel>();
            }
            catch (Exception ex)
            {
                ActivityLog.CreateActivityLog(MenuConstants.EmployeeShift, ActionConstants.MenuAccess, DbConstants.LogType.Error, DbConstants.User.UserKey, ex.GetBaseException().Message);
                return new List<EmployeeShiftViewModel>();

            }
        }

        public EmployeeShiftViewModel GetEmployeeShiftById(int id)
        {
            try
            {
                EmployeeShiftViewModel model = new EmployeeShiftViewModel();

                model = dbContext.EmployeeShifts.Select(row => new EmployeeShiftViewModel
                {
                    RowKey = row.RowKey,
                    ShiftKey = row.ShiftKey,
                    FromDate = row.FromDate,
                    ToDate = row.ToDate,
                }).Where(x => x.RowKey == id).FirstOrDefault();
                if (model == null)
                {
                    model = new EmployeeShiftViewModel();
                }
                FillEmployee(model);
                FillDropdownLists(model);
                return model;
            }
            catch (Exception ex)
            {
                ActivityLog.CreateActivityLog(MenuConstants.EmployeeShift, (id != 0 ? ActionConstants.Edit : ActionConstants.Add), DbConstants.LogType.Error, id, ex.GetBaseException().Message);
                return new EmployeeShiftViewModel();


            }
        }

        private void FillEmployee(EmployeeShiftViewModel model)
        {
            model.EmployeeShiftDetailsModel = (from E in dbContext.Employees
                                               join ES in dbContext.EmployeeShiftDetails.Where(x => x.EmployeeShiftKey == model.RowKey)
                                               on E.RowKey equals ES.EmployeeKey into ESE
                                               from ES in ESE.DefaultIfEmpty()
                                               select new EmployeeShiftDetailsModel
                                               {
                                                   RowKey = ES.RowKey != null ? ES.RowKey : 0,
                                                   EmployeeKey = E.RowKey,
                                                   EmployeeName = E.FirstName + " " + (E.MiddleName ?? "") + " " + E.LastName,
                                                   EmployeeShiftKey = ES.EmployeeShiftKey != null ? ES.EmployeeShiftKey : 0,
                                                   DepartmentName = E.Department.DepartmentName,
                                                   BranchName = E.Branch.BranchName,
                                                   IsActive = ES.IsActive != null ? ES.IsActive : false,

                                               }).ToList();

        }

        public EmployeeShiftViewModel CreateEmployeeShift(EmployeeShiftViewModel model)
        {
            FillDropdownLists(model);
            EmployeeShift EmployeeShiftModel = new EmployeeShift();
            var EmployeeShiftCheck = dbContext.EmployeeShifts.Where(x => DbFunctions.TruncateTime(x.FromDate) == DbFunctions.TruncateTime(model.FromDate) && DbFunctions.TruncateTime(x.ToDate) == DbFunctions.TruncateTime(model.ToDate)
                                && x.ShiftKey == model.ShiftKey)
                           .ToList();

            if (EmployeeShiftCheck.Count != 0)
            {
                model.Message = String.Format(EduSuiteUIResources.ExistsErrorMessage, EduSuiteUIResources.Employeeshift);
                model.IsSuccessful = false;
                return model;
            }

            using (var transaction = dbContext.Database.BeginTransaction())
            {
                try
                {
                    long maxKey = dbContext.EmployeeShifts.Select(p => p.RowKey).DefaultIfEmpty().Max();
                    EmployeeShiftModel.RowKey = maxKey + 1;
                    //EmployeeShiftModel.EmployeeKey = model.EmployeeKey;

                    EmployeeShiftModel.ShiftKey = model.ShiftKey;

                    EmployeeShiftModel.FromDate = model.FromDate;
                    EmployeeShiftModel.ToDate = model.ToDate;

                    dbContext.EmployeeShifts.Add(EmployeeShiftModel);
                    CreateEmployeeShiftDetails(model.EmployeeShiftDetailsModel.Where(x => x.RowKey == 0 && x.IsActive == true).ToList(), EmployeeShiftModel.RowKey);
                    dbContext.SaveChanges();
                    transaction.Commit();
                    model.Message = EduSuiteUIResources.Success;
                    model.IsSuccessful = true;
                    ActivityLog.CreateActivityLog(MenuConstants.EmployeeShift, ActionConstants.Add, DbConstants.LogType.Info, EmployeeShiftModel.RowKey, model.Message);


                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    model.Message = String.Format(EduSuiteUIResources.FailedToSaveErrorMessage, EduSuiteUIResources.Employeeshift);
                    model.IsSuccessful = false;
                    ActivityLog.CreateActivityLog(MenuConstants.EmployeeShift, ActionConstants.Add, DbConstants.LogType.Error, EmployeeShiftModel.RowKey, ex.GetBaseException().Message);
                }
            }
            return model;
        }

        public EmployeeShiftViewModel UpdateEmployeeShift(EmployeeShiftViewModel model)
        {
            FillDropdownLists(model);
            EmployeeShift EmployeeShiftModel = new EmployeeShift();
            var EmployeeShiftCheck = dbContext.EmployeeShifts.Where(x => x.RowKey != model.RowKey && DbFunctions.TruncateTime(x.FromDate) == DbFunctions.TruncateTime(model.FromDate) && DbFunctions.TruncateTime(x.ToDate) == DbFunctions.TruncateTime(model.ToDate)
                                && x.ShiftKey == model.ShiftKey)
                           .ToList();

            if (EmployeeShiftCheck.Count != 0)
            {
                model.Message = String.Format(EduSuiteUIResources.ExistsErrorMessage, EduSuiteUIResources.Employeeshift);
                model.IsSuccessful = false;
                return model;
            }

            using (var transaction = dbContext.Database.BeginTransaction())
            {
                try
                {
                    EmployeeShiftModel = dbContext.EmployeeShifts.SingleOrDefault(row => row.RowKey == model.RowKey);

                    EmployeeShiftModel.ShiftKey = model.ShiftKey;
                    EmployeeShiftModel.FromDate = model.FromDate;
                    EmployeeShiftModel.ToDate = model.ToDate;
                    CreateEmployeeShiftDetails(model.EmployeeShiftDetailsModel.Where(x => x.RowKey == 0 && x.IsActive == true).ToList(), EmployeeShiftModel.RowKey);
                    UpdateEmployeeShiftDetails(model.EmployeeShiftDetailsModel.Where(x => x.RowKey != 0 && x.IsActive == true).ToList(), EmployeeShiftModel.RowKey);
                    dbContext.SaveChanges();
                    transaction.Commit();
                    model.Message = EduSuiteUIResources.Success;
                    model.IsSuccessful = true;
                    ActivityLog.CreateActivityLog(MenuConstants.EmployeeShift, ActionConstants.Edit, DbConstants.LogType.Info, model.RowKey, model.Message);
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    model.Message = String.Format(EduSuiteUIResources.FailedToSaveErrorMessage, EduSuiteUIResources.Employeeshift);
                    model.IsSuccessful = false;
                    ActivityLog.CreateActivityLog(MenuConstants.EmployeeShift, ActionConstants.Edit, DbConstants.LogType.Error, model.RowKey, ex.GetBaseException().Message);
                }
            }
            return model;
        }

        private void CreateEmployeeShiftDetails(List<EmployeeShiftDetailsModel> ModelList, long EmployeeShiftKey)
        {
            long MaxKey = dbContext.EmployeeShiftDetails.Select(x => x.RowKey).DefaultIfEmpty().Max();
            foreach (EmployeeShiftDetailsModel model in ModelList)
            {
                EmployeeShiftDetail employeeShiftDetail = new EmployeeShiftDetail();
                employeeShiftDetail.RowKey = MaxKey + 1;
                employeeShiftDetail.EmployeeKey = model.EmployeeKey;
                employeeShiftDetail.EmployeeShiftKey = EmployeeShiftKey;
                employeeShiftDetail.IsActive = model.IsActive;
                dbContext.EmployeeShiftDetails.Add(employeeShiftDetail);
                MaxKey++;
            }
        }
        private void UpdateEmployeeShiftDetails(List<EmployeeShiftDetailsModel> ModelList, long EmployeeShiftKey)
        {
            foreach (EmployeeShiftDetailsModel model in ModelList)
            {
                EmployeeShiftDetail employeeShiftDetail = new EmployeeShiftDetail();
                employeeShiftDetail = dbContext.EmployeeShiftDetails.SingleOrDefault(x => x.RowKey == model.RowKey);
                employeeShiftDetail.EmployeeKey = model.EmployeeKey;
                employeeShiftDetail.EmployeeShiftKey = EmployeeShiftKey;
                employeeShiftDetail.IsActive = model.IsActive;
            }
        }
        public EmployeeShiftViewModel DeleteEmployeeShift(EmployeeShiftViewModel model)
        {
            using (var transaction = dbContext.Database.BeginTransaction())
            {
                try
                {
                    List<EmployeeShiftDetail> EmployeeShiftDetailList = dbContext.EmployeeShiftDetails.Where(row => row.EmployeeShiftKey == model.RowKey).ToList(); ;

                    EmployeeShift EmployeeShiftModel = dbContext.EmployeeShifts.SingleOrDefault(row => row.RowKey == model.RowKey);
                    dbContext.EmployeeShiftDetails.RemoveRange(EmployeeShiftDetailList);
                    dbContext.EmployeeShifts.Remove(EmployeeShiftModel);
                    dbContext.SaveChanges();
                    transaction.Commit();
                    model.Message = EduSuiteUIResources.Success;
                    model.IsSuccessful = true;
                    ActivityLog.CreateActivityLog(MenuConstants.EmployeeShift, ActionConstants.Delete, DbConstants.LogType.Info, model.RowKey, model.Message);

                }
                catch (DbUpdateException ex)
                {
                    transaction.Rollback();
                    if (ex.GetBaseException().Message.ToUpper().Contains(EduSuiteUIResources.ForeignKeyError.ToUpper()))
                    {
                        model.Message = String.Format(EduSuiteUIResources.FailedToDeleteErrorMessage, EduSuiteUIResources.Employeeshift);
                        model.IsSuccessful = false;

                    }
                    ActivityLog.CreateActivityLog(MenuConstants.EmployeeShift, ActionConstants.Delete, DbConstants.LogType.Error, model.RowKey, ex.GetBaseException().Message);
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    model.Message = String.Format(EduSuiteUIResources.CantDeleteErrorMessage, EduSuiteUIResources.Employeeshift);
                    model.IsSuccessful = false;
                    ActivityLog.CreateActivityLog(MenuConstants.EmployeeShift, ActionConstants.Delete, DbConstants.LogType.Error, model.RowKey, ex.GetBaseException().Message);

                }
            }

            return model;
        }
        public EmployeeShiftViewModel DeleteEmployeeShiftDetails(long Id)
        {
            EmployeeShiftViewModel model = new EmployeeShiftViewModel();
            using (var transaction = dbContext.Database.BeginTransaction())
            {
                try
                {
                    EmployeeShiftDetail EmployeeShiftDetailModel = dbContext.EmployeeShiftDetails.SingleOrDefault(row => row.RowKey == Id);
                    //List<EmployeeShiftDetail> EmployeeShiftDetailList = dbContext.EmployeeShiftDetails.Where(row => row.EmployeeShiftKey == EmployeeShiftDetailModel.EmployeeShiftKey).ToList();

                    //if (EmployeeShiftDetailList.Count <= 1)
                    //{
                    //    EmployeeShift EmployeeShiftModel = dbContext.EmployeeShifts.SingleOrDefault(row => row.RowKey == EmployeeShiftDetailModel.EmployeeShiftKey);
                    //    dbContext.EmployeeShiftDetails.RemoveRange(EmployeeShiftDetailList);
                    //    dbContext.EmployeeShifts.Remove(EmployeeShiftModel);

                    //}
                    //else
                    //{
                    dbContext.EmployeeShiftDetails.Remove(EmployeeShiftDetailModel);
                    //}


                    dbContext.SaveChanges();
                    transaction.Commit();
                    model.Message = EduSuiteUIResources.Success;
                    model.IsSuccessful = true;
                    ActivityLog.CreateActivityLog(MenuConstants.EmployeeShift, ActionConstants.Delete, DbConstants.LogType.Info, model.RowKey, model.Message);

                }
                catch (DbUpdateException ex)
                {
                    transaction.Rollback();
                    if (ex.GetBaseException().Message.ToUpper().Contains(EduSuiteUIResources.ForeignKeyError.ToUpper()))
                    {
                        model.Message = String.Format(EduSuiteUIResources.CantDeleteErrorMessage, EduSuiteUIResources.Employeeshift);
                        model.IsSuccessful = false;

                    }
                    ActivityLog.CreateActivityLog(MenuConstants.EmployeeShift, ActionConstants.Delete, DbConstants.LogType.Error, model.RowKey, ex.GetBaseException().Message);
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    model.Message = String.Format(EduSuiteUIResources.FailedToDeleteErrorMessage, EduSuiteUIResources.Employeeshift);
                    model.IsSuccessful = false;
                    ActivityLog.CreateActivityLog(MenuConstants.EmployeeShift, ActionConstants.Delete, DbConstants.LogType.Error, model.RowKey, ex.GetBaseException().Message);

                }
            }

            return model;
        }
        private void FillShifts(EmployeeShiftViewModel model)
        {
            model.Shifts = dbContext.Shifts.Select(row => new SelectListModel
            {

                RowKey = row.RowKey,
                Text = row.ShiftName
            }).ToList();

        }

        private void FillDropdownLists(EmployeeShiftViewModel model)
        {
            FillShifts(model);

        }
    }
}
