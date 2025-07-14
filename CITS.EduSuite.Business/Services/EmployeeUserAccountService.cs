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
    public class EmployeeUserAccountService : IEmployeeUserAccountService
    {

        private EduSuiteDatabase dbContext;

        public EmployeeUserAccountService(EduSuiteDatabase objDB)
        {
            this.dbContext = objDB;
        }


        public EmployeeUserAccountViewModel GetEmployeeUserAccountById(long id)
        {
            try
            {
                EmployeeUserAccountViewModel model = new EmployeeUserAccountViewModel();

                model = (from AU in dbContext.AppUsers
                         join E in dbContext.Employees.Where(x => x.RowKey == id) on AU.RowKey equals E.AppUserKey 
                         select
                         new EmployeeUserAccountViewModel
                         {
                             RowKey = AU.RowKey ,
                             UserName = AU.AppUserName,
                             RoleKey = AU.RoleKey,
                             IsActive = AU.IsActive,
                             PasswordHint = AU.PasswordHint,
                             EmployeeKey = E.RowKey,
                             EmployeeCode = E.EmployeeCode
                         }).FirstOrDefault();
                if (model == null)
                {
                    model = new EmployeeUserAccountViewModel();
                }
                model.EmployeeKey = id;
                Employee employee = dbContext.Employees.Where(x => x.RowKey == model.EmployeeKey).FirstOrDefault();
                if(employee!=null)
                {
                    model.EmployeeKey = employee.RowKey;
                    model.EmployeeCode = employee.EmployeeCode;
                }
                FillDropdownList(model);
                return model;
            }
            catch (Exception ex)
            {
                ActivityLog.CreateActivityLog(MenuConstants.EmployeeUserAccount, ActionConstants.View, DbConstants.LogType.Error, id, ex.GetBaseException().Message);
                return new EmployeeUserAccountViewModel();


            }

        }

        public EmployeeUserAccountViewModel CreateEmployeeUserAccount(EmployeeUserAccountViewModel model)
        {

            FillDropdownList(model);
            AppUser appUser = new AppUser();
            var Employee = dbContext.Employees.SingleOrDefault(row => row.RowKey == model.EmployeeKey);
            using (var transaction = dbContext.Database.BeginTransaction())
            {
                try
                {

                    Int64 maxKey = dbContext.AppUsers.Select(p => p.RowKey).DefaultIfEmpty().Max();
                    appUser.RowKey = Convert.ToInt32(maxKey + 1);
                    appUser.AppUserName = model.UserName;
                    appUser.FirstName = Employee.FirstName;
                    appUser.MiddleName = Employee.MiddleName;
                    appUser.LastName = Employee.LastName;
                    appUser.Phone1 = Employee.MobileNumber;
                    appUser.Phone2 = Employee.PhoneNumber;
                    appUser.EmailAddress = Employee.EmailAddress ?? "";
                    appUser.Image = Employee.Photo;
                    appUser.Password = SecurityManagement.Encrypt(model.Password);
                    appUser.DesignationKey = Employee.DesignationKey;
                    appUser.RoleKey = model.RoleKey;
                    appUser.IsActive = model.IsActive;
                    appUser.PasswordHint = model.PasswordHint;
                    dbContext.AppUsers.Add(appUser);

                    Employee.AppUserKey = appUser.RowKey;

                    dbContext.SaveChanges();
                    transaction.Commit();
                    model.Message = EduSuiteUIResources.Success;
                    model.IsSuccessful = true;
                    ActivityLog.CreateActivityLog(MenuConstants.EmployeeUserAccount, ActionConstants.Add, DbConstants.LogType.Info, model.EmployeeKey, model.Message);

                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    model.Message = String.Format(EduSuiteUIResources.FailedToSaveErrorMessage, EduSuiteUIResources.EmployeeUserAccount);
                    model.IsSuccessful = false;
                    ActivityLog.CreateActivityLog(MenuConstants.EmployeeUserAccount, ActionConstants.Add, DbConstants.LogType.Error, model.EmployeeKey, ex.GetBaseException().Message);
                }
            }
            return model;
        }

        public EmployeeUserAccountViewModel UpdateEmployeeUserAccount(EmployeeUserAccountViewModel model)
        {
            FillDropdownList(model);
            AppUser appUser = new AppUser();
            var Employee = dbContext.Employees.SingleOrDefault(row => row.RowKey == model.EmployeeKey);
            using (var transaction = dbContext.Database.BeginTransaction())
            {
                try
                {

                    appUser = dbContext.AppUsers.SingleOrDefault(row => row.RowKey == Employee.AppUserKey);
                    appUser.AppUserName = model.UserName;
                    appUser.FirstName = Employee.FirstName;
                    appUser.MiddleName = Employee.MiddleName;
                    appUser.LastName = Employee.LastName;
                    appUser.Phone1 = Employee.MobileNumber;
                    appUser.Phone2 = Employee.PhoneNumber;
                    appUser.EmailAddress = Employee.EmailAddress ?? ""; appUser.Image = Employee.Photo;
                    appUser.DesignationKey = Employee.DesignationKey;
                    appUser.RoleKey = model.RoleKey;
                    appUser.IsActive = model.IsActive;
                    if (model.Password != null)
                    {
                        appUser.Password = SecurityManagement.Encrypt(model.Password);
                        appUser.PasswordHint = model.PasswordHint;
                    }
                    dbContext.SaveChanges();
                    transaction.Commit();
                    model.Message = EduSuiteUIResources.Success;
                    model.IsSuccessful = true;
                    ActivityLog.CreateActivityLog(MenuConstants.EmployeeUserAccount, ActionConstants.Edit, DbConstants.LogType.Info, model.EmployeeKey, model.Message);

                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    model.Message = String.Format(EduSuiteUIResources.FailedToSaveErrorMessage, EduSuiteUIResources.EmployeeUserAccount);
                    model.IsSuccessful = false;
                    ActivityLog.CreateActivityLog(MenuConstants.EmployeeUserAccount, ActionConstants.Edit, DbConstants.LogType.Error, model.EmployeeKey, ex.GetBaseException().Message);
                }
            }
            return model;
        }

        public EmployeeUserAccountViewModel DeleteEmployeeUserAccount(EmployeeUserAccountViewModel model)
        {
            throw new NotImplementedException();
        }

        public EmployeeUserAccountViewModel CheckUserNameExists(string UserName, int RowKey)
        {
            EmployeeUserAccountViewModel model = new EmployeeUserAccountViewModel();
            if (dbContext.AppUsers.Where(row => row.AppUserName == UserName.Trim() && row.RowKey != RowKey).Any())
            {
                model.IsSuccessful = false;

            }
            else
            {
                model.IsSuccessful = true;
            }
            return model;
        }

        private void FillDropdownList(EmployeeUserAccountViewModel model)
        {
            FillRoles(model);

        }

        private void FillRoles(EmployeeUserAccountViewModel model)
        {
            List<short> rolelist = new List<short>();
            rolelist.Add(DbConstants.Role.Parents);
            rolelist.Add(DbConstants.Role.Students);
            rolelist.Add(DbConstants.Role.SuperAdmin);
            rolelist.Add(DbConstants.Role.Admin);
            model.Roles = dbContext.VwRoleSelectActiveOnlies.Where(x => !rolelist.Contains(x.RowKey)).OrderBy(row => row.DisplayOrder).Select(row => new SelectListModel
            {
                RowKey = row.RowKey,
                Text = row.RoleName
            }).ToList();

        }
    }
}
