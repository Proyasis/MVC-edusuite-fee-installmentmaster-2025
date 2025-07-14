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
    public class RoleService : IRoleService
    {
        private EduSuiteDatabase dbContext;

        public RoleService(EduSuiteDatabase objDB)
        {
            this.dbContext = objDB;
        }

        public List<RolesViewModel> GetRoles(string searchText)
        {
            try
            {
                var rolesList = (from r in dbContext.Roles
                                 join u in dbContext.AppUsers
                                 on r.RowKey equals u.RoleKey into joined
                                 from j in joined.DefaultIfEmpty()
                                 orderby r.RoleName
                                 where (r.RoleName.Contains(searchText))
                                 select new RolesViewModel
                                 {
                                     RowKey = r.RowKey,
                                     RoleName = r.RoleName,
                                     RoleNameLocal = r.RoleNameLocal,
                                     StatusName = r.IsActive ? "True" : "False",
                                     NoOfPeople = j.Role.AppUsers.Count
                                 }).ToList();

                return rolesList.GroupBy(x => x.RowKey).Select(y => y.First()).ToList<RolesViewModel>();
            }
            catch (Exception ex)
            {
                ActivityLog.CreateActivityLog(MenuConstants.Role, ActionConstants.View, DbConstants.LogType.Error, null, ex.GetBaseException().Message);
                return new List<RolesViewModel>();
               

            }
        }


        public RolesViewModel GetRoleById(Int16? id)
        {
            try
            {
                RolesViewModel model = new RolesViewModel();
                model = dbContext.Roles.Select(row => new RolesViewModel
                {
                    RowKey = row.RowKey,
                    RoleName = row.RoleName,
                    RoleNameLocal = row.RoleNameLocal,
                    StatusKey = row.IsActive ? Convert.ToInt16(1) : Convert.ToInt16(2)

                }).Where(x => x.RowKey == id).FirstOrDefault();
                if (model == null)
                {
                    model = new RolesViewModel();
                }
                FillStatus(model);
                return model;
            }
            catch (Exception ex)
            {
                ActivityLog.CreateActivityLog(MenuConstants.Role, ActionConstants.View, DbConstants.LogType.Error, id, ex.GetBaseException().Message);
                return new RolesViewModel();
               

            }
        }

        public RolesViewModel CreateRole(RolesViewModel model)
        {
            //using (cPOSEntities dbContext = new cPOSEntities())
            //{
            var roleNameCheck = dbContext.Roles.Where(row => row.RoleName.ToLower() == model.RoleName.ToLower()).ToList();
            Role roleModel = new Role();

            FillStatus(model);
            if (roleNameCheck.Count != 0)
            {
                model.Message = String.Format(EduSuiteUIResources.ExistsErrorMessage, EduSuiteUIResources.Role);
                model.IsSuccessful = false;
                return model;
            }

            using (var transaction = dbContext.Database.BeginTransaction())
            {
                try
                {

                    Int16 maxKey = dbContext.Roles.Select(p => p.RowKey).DefaultIfEmpty().Max();

                    roleModel.RowKey = Convert.ToInt16(maxKey + 1);
                    roleModel.RoleName = model.RoleName;
                    roleModel.RoleNameLocal = model.RoleNameLocal;
                    roleModel.IsActive = model.StatusKey == 1 ? true : false;
                    dbContext.Roles.Add(roleModel);

                    dbContext.SaveChanges();
                    transaction.Commit();
                    model.Message = EduSuiteUIResources.Success;
                    model.IsSuccessful = true;
                    ActivityLog.CreateActivityLog(MenuConstants.Role, ActionConstants.Add, DbConstants.LogType.Info, model.RowKey, model.Message);

                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    model.Message = String.Format(EduSuiteUIResources.FailedToSaveErrorMessage, EduSuiteUIResources.Role);
                    model.IsSuccessful = false;
                    ActivityLog.CreateActivityLog(MenuConstants.Role, ActionConstants.Add, DbConstants.LogType.Error, model.RowKey, ex.GetBaseException().Message);
                }
            }
            //}
            return model;
        }

        public RolesViewModel UpdateRole(RolesViewModel model)
        {
            FillStatus(model);
            //using (cPOSEntities dbContext = new cPOSEntities())
            //{
            var roleNameCheck = dbContext.Roles.Where(row => row.RoleName.ToLower() == model.RoleName.ToLower() && row.RowKey != model.RowKey).ToList();
            Role roleModel = new Role();

            if (roleNameCheck.Count != 0)
            {
                model.Message = String.Format(EduSuiteUIResources.ExistsErrorMessage, EduSuiteUIResources.Role);
                model.IsSuccessful = false;
                return model;
            }

            using (var transaction = dbContext.Database.BeginTransaction())
            {
                try
                {
                    roleModel = dbContext.Roles.SingleOrDefault(row => row.RowKey == model.RowKey);
                    roleModel.RoleName = model.RoleName;
                    roleModel.RoleNameLocal = model.RoleNameLocal;
                    roleModel.IsActive = model.StatusKey == 1 ? true : false;

                    dbContext.SaveChanges();
                    transaction.Commit();
                    model.Message = EduSuiteUIResources.Success;
                    model.IsSuccessful = true;
                    ActivityLog.CreateActivityLog(MenuConstants.Role, ActionConstants.Add, DbConstants.LogType.Info, model.RowKey, model.Message);

                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    model.Message = String.Format(EduSuiteUIResources.FailedToSaveErrorMessage, EduSuiteUIResources.Role);
                    model.IsSuccessful = false;
                    ActivityLog.CreateActivityLog(MenuConstants.Role, ActionConstants.Add, DbConstants.LogType.Error, model.RowKey, ex.GetBaseException().Message);
                }
            }
            //}
            return model;
        }

        public RolesViewModel DeleteRole(RolesViewModel model)
        {
            //using (cPOSEntities dbContext = new cPOSEntities())
            //{
            using (var transaction = dbContext.Database.BeginTransaction())
            {
                try
                {
                    Role role = dbContext.Roles.SingleOrDefault(row => row.RowKey == model.RowKey);
                    dbContext.Roles.Remove(role);
                    dbContext.SaveChanges();
                    transaction.Commit();
                    model.Message = EduSuiteUIResources.Success;
                    model.IsSuccessful = true;
                     ActivityLog.CreateActivityLog(MenuConstants.Role, ActionConstants.Delete, DbConstants.LogType.Info, model.RowKey,model.Message);

                }
                catch (DbUpdateException ex)
                {
                    transaction.Rollback();
                    if (ex.GetBaseException().Message.ToUpper().Contains(EduSuiteUIResources.ForeignKeyError.ToUpper()))
                    {
                        model.Message = String.Format(EduSuiteUIResources.CantDeleteErrorMessage, EduSuiteUIResources.Role);
                        model.IsSuccessful = false;
                          ActivityLog.CreateActivityLog(MenuConstants.Role, ActionConstants.Delete, DbConstants.LogType.Error, model.RowKey, ex.GetBaseException().Message);
                    }
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    model.Message = String.Format(EduSuiteUIResources.FailedToDeleteErrorMessage, EduSuiteUIResources.Role);
                    model.IsSuccessful = false;
                      ActivityLog.CreateActivityLog(MenuConstants.Role, ActionConstants.Delete, DbConstants.LogType.Error, model.RowKey, ex.GetBaseException().Message);
                }
            }
            //}
            return model;
        }

        private void FillStatus(RolesViewModel model)
        {
            model.Statuses = dbContext.Status.Select(row => new SelectListModel
                {
                    RowKey = row.RowKey,
                    Text = row.StatusName
                }).ToList();
        }

    }
}
