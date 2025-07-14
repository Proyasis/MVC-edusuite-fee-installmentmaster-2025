using System;
using System.Collections.Generic;
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
    public class DesignationService : IDesignationService
    {
        private EduSuiteDatabase dbContext;

        public DesignationService(EduSuiteDatabase objDB)
        {
            this.dbContext = objDB;
        }

        public List<DesignationViewModel> GetDesignations(string searchText)
        {
            try
            {
                var designationsList = (from d in dbContext.Designations
                                        orderby d.RowKey descending
                                        where (d.DesignationName.Contains(searchText))
                                        select new DesignationViewModel
                                        {
                                            RowKey = d.RowKey,
                                            DesignationName = d.DesignationName,
                                            HigherDesignationName = d.Designation2.DesignationName,
                                            IsActive = d.IsActive
                                        }).ToList();

                return designationsList.GroupBy(x => x.RowKey).Select(y => y.First()).ToList<DesignationViewModel>();
            }
            catch (Exception ex)
            {
                ActivityLog.CreateActivityLog(MenuConstants.Designation, ActionConstants.View, DbConstants.LogType.Error, null, ex.GetBaseException().Message);
                return new List<DesignationViewModel>();
                

            }
        }

        public DesignationViewModel GetDesignationById(int? id)
        {
            try
            {
                DesignationViewModel model = new DesignationViewModel();
                model = dbContext.Designations.Select(row => new DesignationViewModel
                {
                    RowKey = row.RowKey,
                    DesignationName = row.DesignationName,
                    HigherDesignationKey = row.HigherDesignationKey,
                    IsActive = row.IsActive,

                }).Where(x => x.RowKey == id).FirstOrDefault();
                if (model == null)
                {
                    model = new DesignationViewModel();
                }
                FillDesignations(model);
                return model;
            }
            catch (Exception ex)
            {
                ActivityLog.CreateActivityLog(MenuConstants.Designation, ActionConstants.View, DbConstants.LogType.Error, null, ex.GetBaseException().Message);
                return new DesignationViewModel();
                

            }
        }

        public DesignationViewModel CreateDesignation(DesignationViewModel model)
        {
            Designation DesignationModel = new Designation();
            var DesignationnameCheck = dbContext.Designations.Where(row => row.DesignationName.ToLower() == model.DesignationName.ToLower()).ToList();
            FillDesignations(model);
            if (DesignationnameCheck.Count != 0)
            {
                model.Message = String.Format(EduSuiteUIResources.ExistsErrorMessage, EduSuiteUIResources.Designation);
                model.IsSuccessful = false;
                return model;
            }

            using (var transaction = dbContext.Database.BeginTransaction())
            {

                try
                {
                    short maxKey = dbContext.Designations.Select(p => p.RowKey).DefaultIfEmpty().Max();
                    DesignationModel.RowKey = Convert.ToInt16(maxKey + 1);
                    DesignationModel.DesignationName = model.DesignationName;
                    DesignationModel.HigherDesignationKey = model.HigherDesignationKey;
                    DesignationModel.IsActive = model.IsActive;
                    DesignationModel.DisplayOrder = Convert.ToInt16(maxKey + 1);
                    dbContext.Designations.Add(DesignationModel);
                    dbContext.SaveChanges();
                    transaction.Commit();

                    model.Message = EduSuiteUIResources.Success;
                    model.IsSuccessful = true;
                    ActivityLog.CreateActivityLog(MenuConstants.Designation, ActionConstants.Add, DbConstants.LogType.Info, model.RowKey, model.Message);

                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    model.Message = String.Format(EduSuiteUIResources.FailedToSaveErrorMessage, EduSuiteUIResources.Designation);
                    model.IsSuccessful = false;
                    ActivityLog.CreateActivityLog(MenuConstants.Designation, ActionConstants.Add, DbConstants.LogType.Error, model.RowKey, ex.GetBaseException().Message);

                }
            }
            return model;
        }

        public DesignationViewModel UpdateDesignation(DesignationViewModel model)
        {
            Designation DesignationModel = new Designation();
            var designationCheck = dbContext.Designations.Where(row => row.DesignationName.ToLower() == model.DesignationName.ToLower() && row.RowKey != model.RowKey).ToList();

            FillDesignations(model);
            if (designationCheck.Count != 0)
            {
                model.Message = String.Format(EduSuiteUIResources.ExistsErrorMessage, EduSuiteUIResources.Designation);
                model.IsSuccessful = false;
                return model;
            }
            using (var transaction = dbContext.Database.BeginTransaction())
            {
                try
                {
                    DesignationModel = dbContext.Designations.SingleOrDefault(row => row.RowKey == model.RowKey);
                    DesignationModel.DesignationName = model.DesignationName;
                    DesignationModel.HigherDesignationKey = model.HigherDesignationKey;
                    DesignationModel.IsActive = model.IsActive;
                    dbContext.SaveChanges();
                    transaction.Commit();
                    model.Message = EduSuiteUIResources.Success;
                    model.IsSuccessful = true;
                    ActivityLog.CreateActivityLog(MenuConstants.Designation, ActionConstants.Edit, DbConstants.LogType.Info, model.RowKey, model.Message);

                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    model.Message = String.Format(EduSuiteUIResources.FailedToSaveErrorMessage, EduSuiteUIResources.Designation);
                    model.IsSuccessful = false;
                    ActivityLog.CreateActivityLog(MenuConstants.Designation, ActionConstants.Edit, DbConstants.LogType.Error, model.RowKey, ex.GetBaseException().Message);

                }
            }
            return model;
        }

        public DesignationViewModel DeleteDesignation(DesignationViewModel model)
        {
            using (var transaction = dbContext.Database.BeginTransaction())
            {
                try
                {
                    Designation DesignationModel = dbContext.Designations.SingleOrDefault(row => row.RowKey == model.RowKey);
                    dbContext.Designations.Remove(DesignationModel);
                    dbContext.SaveChanges();
                    transaction.Commit();
                    model.Message = EduSuiteUIResources.Success;
                    model.IsSuccessful = true;
                    ActivityLog.CreateActivityLog(MenuConstants.Designation, ActionConstants.Delete, DbConstants.LogType.Info, model.RowKey, model.Message);

                }
                catch (DbUpdateException ex)
                {
                    transaction.Rollback();
                    if (ex.GetBaseException().Message.ToUpper().Contains(EduSuiteUIResources.ForeignKeyError.ToUpper()))
                    {
                        model.Message = String.Format(EduSuiteUIResources.CantDeleteErrorMessage, EduSuiteUIResources.Designation);
                        model.IsSuccessful = false;
                        ActivityLog.CreateActivityLog(MenuConstants.Designation, ActionConstants.Delete, DbConstants.LogType.Debug, model.RowKey, ex.GetBaseException().Message);

                    }
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    model.Message = String.Format(EduSuiteUIResources.FailedToDeleteErrorMessage, EduSuiteUIResources.Designation);
                    model.IsSuccessful = false;
                    ActivityLog.CreateActivityLog(MenuConstants.Designation, ActionConstants.Delete, DbConstants.LogType.Error, model.RowKey, ex.GetBaseException().Message);

                }

            }
            return model;
        }
        private void FillDesignations(DesignationViewModel model)
        {
            model.HigherDesignations = dbContext.Designations.OrderBy(row => row.DesignationName).Select(row => new SelectListModel
            {
                RowKey = row.RowKey,
                Text = row.DesignationName
            }).ToList();
        }

        public List<DesignationViewModel> GetDesignationChart()
        {
            try
            {
                var designationList = (from d in dbContext.Designations
                                       select new DesignationViewModel
                                       {
                                           RowKey = d.RowKey,
                                           DesignationName = d.DesignationName,
                                           HigherDesignationKey = d.HigherDesignationKey
                                       }).OrderBy(row => (row.RowKey < row.HigherDesignationKey)).ToList();
                return designationList;

            }
            catch (Exception ex)
            {
                ActivityLog.CreateActivityLog(MenuConstants.Designation, ActionConstants.View, DbConstants.LogType.Error, null, ex.GetBaseException().Message);
                return new List<DesignationViewModel>();
                

            }
        }


        public DesignationViewModel UpdateDesignationChart(List<DesignationViewModel> modelList)
        {
            DesignationViewModel model = new DesignationViewModel();
            using (var transaction = dbContext.Database.BeginTransaction())
            {
                try
                {
                    foreach (DesignationViewModel modelItem in modelList)
                    {
                        Designation designationModel = dbContext.Designations.SingleOrDefault(row => row.RowKey == modelItem.RowKey);
                        designationModel.HigherDesignationKey = modelItem.HigherDesignationKey;
                    }

                    dbContext.SaveChanges();
                    transaction.Commit();
                    model.Message = EduSuiteUIResources.Success;
                    model.IsSuccessful = true;
                    ActivityLog.CreateActivityLog(MenuConstants.Designation, ActionConstants.Edit, DbConstants.LogType.Info, model.RowKey, model.Message);

                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    model.Message = String.Format(EduSuiteUIResources.FailedToSaveErrorMessage, EduSuiteUIResources.Designation);
                    model.IsSuccessful = false;
                    ActivityLog.CreateActivityLog(MenuConstants.Designation, ActionConstants.Edit, DbConstants.LogType.Error, model.RowKey, ex.GetBaseException().Message);

                }
            }
            return model;
        }


        #region Permissions
        public DesignationViewModel GetDesignationPermissionsById(short DesignationKey)
        {
            try
            {
                DesignationViewModel model = new DesignationViewModel();
                model.DesignationPermissions =
                    (from M in dbContext.Menus
                     select new UserPermissionViewModel
                     {
                         RowKey = null,
                         MenuKey = M.RowKey,
                         MenuName = M.MenuName,
                         ActionKey = null,
                         ActionName = null,
                         IsActive = false
                     }).Union(from MA in dbContext.MenuActions
                              join UP in dbContext.DesignationPermissions.Where(x => x.DesignationKey == DesignationKey)
                              on new { MA.MenuKey, MA.ActionKey } equals new { MenuKey = UP.MenuKey ?? 0, ActionKey = UP.ActionKey ?? 0 }
                              into UPL
                              from UP in UPL.DefaultIfEmpty()
                              select new UserPermissionViewModel
                   {
                       RowKey = UP.RowKey,
                       MenuKey = UP.RowKey != null ? UP.MenuKey : MA.MenuKey,
                       MenuName = MA.Menu.MenuName,
                       ActionKey = UP.RowKey != null ? UP.ActionKey : MA.ActionKey,
                       ActionName = MA.Action.ActionName,
                       IsActive = UP.IsActive
                   }).ToList();

                return model;
            }
            catch (Exception ex)
            {
                ActivityLog.CreateActivityLog(MenuConstants.Designation, ActionConstants.View, DbConstants.LogType.Error, null, ex.GetBaseException().Message);
                return new DesignationViewModel();
                
            }
        }

        public DesignationViewModel UpdateDesignationPermission(DesignationViewModel model)
        {
            using (var transaction = dbContext.Database.BeginTransaction())
            {
                try
                {

                    CreatePermission(model.DesignationPermissions.Where(row => row.RowKey == null).ToList(), model.RowKey);
                    UpdatePermission(model.DesignationPermissions.Where(row => row.RowKey != null).ToList());
                    dbContext.SaveChanges();
                    transaction.Commit();
                    model.Message = EduSuiteUIResources.Success;
                    model.IsSuccessful = true;
                    ActivityLog.CreateActivityLog(MenuConstants.Designation, (model.DesignationPermissions.Any(row => row.RowKey != 0) ? ActionConstants.Edit : ActionConstants.Add), DbConstants.LogType.Info, model.RowKey, model.Message);
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    model.Message = String.Format(EduSuiteUIResources.FailedToSaveErrorMessage, EduSuiteUIResources.EmployeeUserPermission);
                    model.IsSuccessful = false;

                    ActivityLog.CreateActivityLog(MenuConstants.Designation, (model.DesignationPermissions.Any(row => row.RowKey != 0) ? ActionConstants.Edit : ActionConstants.Add), DbConstants.LogType.Error, model.RowKey, ex.GetBaseException().Message);
                }

            }
            return model;
        }

        private void CreatePermission(List<UserPermissionViewModel> modelList, short DesignationKey)
        {
            Int32 maxKey = dbContext.DesignationPermissions.Select(p => p.RowKey).DefaultIfEmpty().Max();
            foreach (UserPermissionViewModel model in modelList)
            {

                DesignationPermission designationPermissionModel = new DesignationPermission();
                designationPermissionModel.RowKey = Convert.ToInt32(maxKey + 1);
                designationPermissionModel.DesignationKey = DesignationKey;
                designationPermissionModel.MenuKey = model.MenuKey;
                designationPermissionModel.ActionKey = model.ActionKey;
                designationPermissionModel.IsActive = model.IsActive ?? false;

                dbContext.DesignationPermissions.Add(designationPermissionModel);
                maxKey++;

            }

        }
        private void UpdatePermission(List<UserPermissionViewModel> modelList)
        {
            foreach (UserPermissionViewModel model in modelList)
            {
                DesignationPermission designationPermissionModel = new DesignationPermission();
                designationPermissionModel = dbContext.DesignationPermissions.SingleOrDefault(row => row.RowKey == model.RowKey);
                designationPermissionModel.MenuKey = model.MenuKey;
                designationPermissionModel.ActionKey = model.ActionKey;
                designationPermissionModel.IsActive = model.IsActive ?? false;
            }
        }

        #endregion


    }
}
