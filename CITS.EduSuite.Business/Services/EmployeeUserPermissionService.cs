using System;
using System.Collections.Generic;
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
    public class EmployeeUserPermissionService : IEmployeeUserPermissionService
    {
        private EduSuiteDatabase dbContext;

        public EmployeeUserPermissionService()
        {
            this.dbContext = new EduSuiteDatabase();
        }


        public bool CheckUserPermission(UserPermissionViewModel model)
        {

            return (dbContext.UserPermissions.Where(row => row.Menu.MenuCode == model.MenuCode && row.Action.ActionCode == model.ActionCode && row.Employee.AppUserKey == DbConstants.User.UserKey && row.IsActive).Any());
        }

        public EmployeeUserPermissionViewModel GetUserPermissionsById(long EmployeeKey)
        {
            try
            {
                EmployeeUserPermissionViewModel model = new EmployeeUserPermissionViewModel();
                Employee employee = dbContext.Employees.SingleOrDefault(row => row.RowKey == EmployeeKey);
                if (employee != null)
                {
                    //model.CountryAccess = employee.CountryAccess;
                    model.BranchAccess = employee.BranchAccess;
                }
                FillCountries(model);
                FillBranches(model, employee.BranchKey);
                model.UserPermissions =
                    (from M in dbContext.Menus.Where(x => x.IsActive && x.MenuType.IsActive)
                     select new UserPermissionViewModel
                     {
                         RowKey = null,
                         MenuTypeKey = M.MenuTypeKey,
                         MenuTypeName = M.MenuType.MenuTypeName,
                         MenuKey = M.RowKey,
                         MenuName = M.MenuName,
                         ActionKey = null,
                         ActionName = null,
                         IsActive = false,
                         DisplayOrder = M.MenuType.DisplayOrder
                     }).Union(from MA in dbContext.MenuActions
                              join UP in dbContext.UserPermissions.Where(x => x.EmployeeKey == EmployeeKey)
                              on new { MA.MenuKey, MA.ActionKey } equals new { MenuKey = UP.MenuKey ?? 0, ActionKey = UP.ActionKey ?? 0 }
                              into UPL
                              from UP in UPL.DefaultIfEmpty()
                              select new UserPermissionViewModel
                              {
                                  RowKey = UP.RowKey,
                                  MenuTypeKey = UP.Menu.MenuTypeKey,
                                  MenuTypeName = UP.Menu.MenuType.MenuTypeName,
                                  MenuKey = UP.RowKey != null ? UP.MenuKey : MA.MenuKey,
                                  MenuName = MA.Menu.MenuName,
                                  ActionKey = UP.RowKey != null ? UP.ActionKey : MA.ActionKey,
                                  ActionName = MA.Action.ActionName,
                                  IsActive = UP.IsActive,
                                  DisplayOrder = 0
                              }).ToList();


                model.DashBoardPermission = (from M in dbContext.DashBoardTypes.Where(x => x.IsActive)
                                             select new DashBoardPermissionViewModel
                                             {
                                                 RowKey = null,
                                                 DashBoardTypeKey = M.RowKey,
                                                 DashBoardTypeName = M.DashBoardTypeName,
                                                 DashBoardTypeCode = M.DashBoardTypeCode,
                                                 DashBoardContentKey = null,
                                                 DashBoardContentName = null,
                                                 IsActive = false,
                                                 DisplayOrder = M.DisplayOrder
                                             }).Union(from MA in dbContext.DashBoardContents
                                                      join UP in dbContext.DashBoardUserPermissions.Where(x => x.EmployeeKey == EmployeeKey)
                                                      on new { DashBoardContentKey = MA.RowKey } equals new { DashBoardContentKey = UP.DashBoardContentKey ?? 0 }
                                                      into UPL
                                                      from UP in UPL.DefaultIfEmpty()
                                                      select new DashBoardPermissionViewModel
                                                      {
                                                          RowKey = UP.RowKey,
                                                          DashBoardTypeKey = MA.DashBoardTypeKey,
                                                          DashBoardTypeName = MA.DashBoardType.DashBoardTypeName,
                                                          DashBoardTypeCode = MA.DashBoardType.DashBoardTypeCode,
                                                          DashBoardContentKey = UP.RowKey != null ? UP.DashBoardContentKey : MA.RowKey,
                                                          DashBoardContentName = MA.DashBoardContentName,
                                                          IsActive = UP.IsActive,
                                                          DisplayOrder = 0
                                                      }).ToList();


                //model.DashBoardPermission = (from MA in dbContext.DashBoardContents
                //                             join UP in dbContext.DashBoardUserPermissions.Where(x => x.EmployeeKey == EmployeeKey)
                //                             on new { DashBoardContentKey = MA.RowKey } equals new { DashBoardContentKey = UP.DashBoardContentKey ?? 0 }
                //                             into UPL
                //                             from UP in UPL.DefaultIfEmpty()
                //                             select new DashBoardPermissionViewModel
                //                             {
                //                                 RowKey = UP.RowKey,
                //                                 DashBoardTypeKey = MA.DashBoardTypeKey,
                //                                 DashBoardTypeName = MA.DashBoardType.DashBoardTypeName,
                //                                 DashBoardTypeCode = MA.DashBoardType.DashBoardTypeCode,
                //                                 DashBoardContentKey = UP.RowKey != null ? UP.DashBoardContentKey : MA.RowKey,
                //                                 DashBoardContentName = MA.DashBoardContentName,
                //                                 IsActive = UP.IsActive,
                //                                 DisplayOrder = 0
                //                             }).ToList();


                return model;
            }
            catch (Exception ex)
            {
                ActivityLog.CreateActivityLog(MenuConstants.EmployeeUserPermission, ActionConstants.View, DbConstants.LogType.Error, EmployeeKey, ex.GetBaseException().Message);
                return new EmployeeUserPermissionViewModel();
            }
        }

        public EmployeeUserPermissionViewModel UpdateEmployeeUserPermission(EmployeeUserPermissionViewModel model)
        {
            using (var transaction = dbContext.Database.BeginTransaction())
            {
                try
                {
                    Employee employeeModel = new Employee();
                    employeeModel = dbContext.Employees.SingleOrDefault(row => row.RowKey == model.EmployeeKey);
                    //employeeModel.CountryAccess = model.CountryAccess;


                    string employeebranchkey = string.Join(",", employeeModel.BranchKey);
                    if (model.BranchAccess != null)
                    {
                        model.BranchAccess = employeebranchkey + "," + model.BranchAccess;
                    }
                    else
                    {
                        model.BranchAccess = employeebranchkey;
                    }
                    employeeModel.BranchAccess = model.BranchAccess;
                    if (employeeModel != null)
                    {

                        DeleteUserPermission(model.UserPermissions.Where(row => row.RowKey != null && row.IsActive == false).ToList());
                        UpdateUserPermission(model.UserPermissions.Where(row => row.RowKey != null && row.IsActive == true).ToList());
                        CreateUserPermission(model.UserPermissions.Where(row => row.RowKey == null && row.IsActive == true).ToList(), model.EmployeeKey);


                        DeleteDashBoardPermission(model.DashBoardPermission.Where(row => row.RowKey != null && row.IsActive == false).ToList());
                        UpdateDashBoardPermission(model.DashBoardPermission.Where(row => row.RowKey != null && row.IsActive == true).ToList());
                        CreateDashBoardPermission(model.DashBoardPermission.Where(row => row.RowKey == null && row.IsActive == true).ToList(), model.EmployeeKey);

                    }

                    dbContext.SaveChanges();
                    transaction.Commit();
                    model.Message = EduSuiteUIResources.Success;
                    model.IsSuccessful = true;

                    ActivityLog.CreateActivityLog(MenuConstants.EmployeeUserPermission, (model.UserPermissions.Any(row => row.RowKey != 0) ? ActionConstants.Edit : ActionConstants.Add), DbConstants.LogType.Info, model.EmployeeKey, model.Message);
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    model.Message = String.Format(EduSuiteUIResources.FailedToSaveErrorMessage, EduSuiteUIResources.EmployeeUserPermission);
                    model.IsSuccessful = false;
                    ActivityLog.CreateActivityLog(MenuConstants.EmployeeUserPermission, (model.UserPermissions.Any(row => row.RowKey != 0) ? ActionConstants.Edit : ActionConstants.Add), DbConstants.LogType.Error, model.EmployeeKey, ex.GetBaseException().Message);
                }

            }
            return model;
        }

        private void CreateUserPermission(List<UserPermissionViewModel> modelList, long EmployeeKey)
        {
            Int64 maxKey = dbContext.UserPermissions.Select(p => p.RowKey).DefaultIfEmpty().Max();
            foreach (UserPermissionViewModel model in modelList)
            {

                UserPermission userPermissionModel = new UserPermission();
                userPermissionModel.RowKey = Convert.ToInt64(maxKey + 1);
                userPermissionModel.EmployeeKey = EmployeeKey;
                userPermissionModel.MenuKey = model.MenuKey;
                userPermissionModel.ActionKey = model.ActionKey;
                userPermissionModel.IsActive = model.IsActive ?? false;

                dbContext.UserPermissions.Add(userPermissionModel);
                maxKey++;

            }

        }
        private void UpdateUserPermission(List<UserPermissionViewModel> modelList)
        {
            foreach (UserPermissionViewModel model in modelList)
            {

                UserPermission userPermissionModel = new UserPermission();
                userPermissionModel = dbContext.UserPermissions.SingleOrDefault(row => row.RowKey == model.RowKey);
                userPermissionModel.MenuKey = model.MenuKey;
                userPermissionModel.ActionKey = model.ActionKey;
                userPermissionModel.IsActive = model.IsActive ?? false;
            }
        }
        private void DeleteUserPermission(List<UserPermissionViewModel> modelList)
        {
            foreach (UserPermissionViewModel model in modelList)
            {
                try
                {
                    UserPermission userPermissions = dbContext.UserPermissions.SingleOrDefault(row => row.RowKey == model.RowKey);
                    if (userPermissions != null)
                    {
                        dbContext.UserPermissions.Remove(userPermissions);
                        dbContext.SaveChanges();
                    }
                }
                catch (Exception ex)
                {
                }
            }

        }
        private void FillCountries(EmployeeUserPermissionViewModel model)
        {
            List<short> Countries = new List<short>();
            if (model.CountryAccess != null)
            {
                Countries = model.CountryAccess.Split(',').Select(Int16.Parse).ToList();
            }
            model.Countries = dbContext.VwCountrySelectActiveOnlies.OrderBy(row => row.DisplayOrder).Select(row => new SelectListModel
            {
                RowKey = row.RowKey,
                Text = row.CountryName,
                Selected = Countries.Contains(row.RowKey)
            }).ToList();
        }

        private void FillBranches(EmployeeUserPermissionViewModel model, short UserBranchKey)
        {
            List<short> Branches = new List<short>();
            if (model.BranchAccess != null)
            {
                Branches = model.BranchAccess.Split(',').Select(Int16.Parse).ToList();
            }
            model.Branches = dbContext.vwBranchSelectActiveOnlies.Where(x => x.RowKey != UserBranchKey).OrderBy(row => row.DisplayOrder).Select(row => new SelectListModel
            {
                RowKey = row.RowKey,
                Text = row.BranchName,
                Selected = Branches.Contains(row.RowKey)
            }).ToList();
        }

        public void FillMenuTypes(EmployeeUserPermissionViewModel model)
        {
            model.MenuTypes = dbContext.MenuTypes.Where(x => x.IsActive).OrderBy(row => row.DisplayOrder).Select(row => new SelectListModel
            {
                RowKey = row.RowKey,
                Text = row.MenuTypeName
            }).ToList();
        }


        private void CreateDashBoardPermission(List<DashBoardPermissionViewModel> modelList, long EmployeeKey)
        {
            Int64 maxKey = dbContext.DashBoardUserPermissions.Select(p => p.RowKey).DefaultIfEmpty().Max();
            foreach (DashBoardPermissionViewModel model in modelList)
            {

                DashBoardUserPermission dashBoardUserPermissionModel = new DashBoardUserPermission();
                dashBoardUserPermissionModel.RowKey = Convert.ToInt64(maxKey + 1);
                dashBoardUserPermissionModel.EmployeeKey = EmployeeKey;
                dashBoardUserPermissionModel.DashBoardContentKey = model.DashBoardContentKey;
                dashBoardUserPermissionModel.IsActive = model.IsActive ?? false;

                dbContext.DashBoardUserPermissions.Add(dashBoardUserPermissionModel);
                maxKey++;

            }

        }
        private void UpdateDashBoardPermission(List<DashBoardPermissionViewModel> modelList)
        {
            foreach (DashBoardPermissionViewModel model in modelList)
            {
                DashBoardUserPermission dashBoardUserPermissionModel = new DashBoardUserPermission();
                dashBoardUserPermissionModel = dbContext.DashBoardUserPermissions.SingleOrDefault(row => row.RowKey == model.RowKey);
                dashBoardUserPermissionModel.DashBoardContentKey = model.DashBoardContentKey;
                dashBoardUserPermissionModel.IsActive = model.IsActive ?? false;
            }
        }
        private void DeleteDashBoardPermission(List<DashBoardPermissionViewModel> modelList)
        {
            foreach (DashBoardPermissionViewModel model in modelList)
            {
                try
                {
                    DashBoardUserPermission dashBoardUserPermission = dbContext.DashBoardUserPermissions.SingleOrDefault(row => row.RowKey == model.RowKey);
                    if (dashBoardUserPermission != null)
                    {
                        dbContext.DashBoardUserPermissions.Remove(dashBoardUserPermission);
                        dbContext.SaveChanges();
                    }
                }
                catch (Exception ex)
                {
                }
            }

        }

        public bool CheckDashBoardPermission(DashBoardPermissionViewModel model)
        {
            return (dbContext.DashBoardUserPermissions.Where(row => row.DashBoardContentKey == model.DashBoardContentKey && row.Employee.AppUserKey == DbConstants.User.UserKey && row.IsActive && row.DashBoardContent.IsActive && row.DashBoardContent.DashBoardType.IsActive).Any());
        }
    }
}
