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
    public class EmployeeAccountService : IEmployeeAccountService
    {

        private EduSuiteDatabase dbContext;

        public EmployeeAccountService(EduSuiteDatabase objDB)
        {
            this.dbContext = objDB;
        }


        public EmployeeAccountViewModel GetEmployeeAccountById(Int64 id)
        {
            try
            {
                EmployeeAccountViewModel model = new EmployeeAccountViewModel();

                model = dbContext.EmployeeAccounts.Where(x => x.EmployeeKey == id).Select(row => new EmployeeAccountViewModel
                {
                    RowKey = row.RowKey,
                    AccountNumber = row.AccountNumber,
                    IFSCCode = row.IFSCCode,
                    MICRCode = row.MICRCode,
                    BankKey = row.BankKey,
                    BranchLocation = row.BranchLocation,
                    NameInAccount = row.NameInAccount,
                    AccountTypeKey = row.AccountTypeKey,
                    AdharNumber = row.AdharNumber,
                    UANNumber = row.UANNumber
                }).FirstOrDefault();
                if (model == null)
                {
                    model = new EmployeeAccountViewModel();
                    model.AdharNumber = dbContext.EmployeeIdentities.Where(row => row.EmployeeKey == id && row.IdentityTypeKey == DbConstants.IdentityType.AdharNumber).Select(row => row.IdentyUniqueID).SingleOrDefault();
                }
                model.EmployeeKey = id;
                FillDropdownList(model);
                return model;
            }
            catch (Exception ex)
            {
                ActivityLog.CreateActivityLog(MenuConstants.EmployeeAccount, ActionConstants.View, DbConstants.LogType.Error, id, ex.GetBaseException().Message);
                return new EmployeeAccountViewModel();

            }
        }

        public EmployeeAccountViewModel CreateEmployeeAccount(EmployeeAccountViewModel model)
        {           
           
            EmployeeAccount employeeaccountModel = new EmployeeAccount();           

            using (var transaction = dbContext.Database.BeginTransaction())
            {
                try
                {
                    Int64 maxKey = dbContext.EmployeeAccounts.Select(p => p.RowKey).DefaultIfEmpty().Max();
                    employeeaccountModel.RowKey = Convert.ToInt64(maxKey + 1);
                    employeeaccountModel.EmployeeKey = model.EmployeeKey;
                    employeeaccountModel.AccountNumber = model.AccountNumber;
                    employeeaccountModel.IFSCCode = model.IFSCCode;
                    employeeaccountModel.MICRCode = model.MICRCode;
                    employeeaccountModel.BankKey = model.BankKey;
                    employeeaccountModel.BranchLocation = model.BranchLocation;
                    employeeaccountModel.NameInAccount = model.NameInAccount;
                    employeeaccountModel.AccountTypeKey = model.AccountTypeKey;
                    employeeaccountModel.AdharNumber = model.AdharNumber;
                    employeeaccountModel.UANNumber = model.UANNumber;
                    dbContext.EmployeeAccounts.Add(employeeaccountModel);
                    dbContext.SaveChanges();
                    transaction.Commit();
                    model.Message = EduSuiteUIResources.Success;
                    model.IsSuccessful = true;
                    ActivityLog.CreateActivityLog(MenuConstants.EmployeeAccount, ActionConstants.Add, DbConstants.LogType.Info, employeeaccountModel.RowKey, model.Message);
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    model.Message = String.Format(EduSuiteUIResources.FailedToSaveErrorMessage, EduSuiteUIResources.Employee + EduSuiteUIResources.BlankSpace + EduSuiteUIResources.BankAccount);
                    model.IsSuccessful = false;

                    ActivityLog.CreateActivityLog(MenuConstants.EmployeeAccount, ActionConstants.Add, DbConstants.LogType.Error, model.RowKey, ex.GetBaseException().Message);
                }
            }
            //}
            return model;
        }

        public EmployeeAccountViewModel UpdateEmployeeAccount(EmployeeAccountViewModel model)
        {
                
            EmployeeAccount employeeaccountModel = new EmployeeAccount();
            using (var transaction = dbContext.Database.BeginTransaction())
            {
                try
                {
                    employeeaccountModel = dbContext.EmployeeAccounts.SingleOrDefault(row => row.EmployeeKey == model.EmployeeKey);
                    employeeaccountModel.EmployeeKey = model.EmployeeKey;
                    employeeaccountModel.AccountNumber = model.AccountNumber;
                    employeeaccountModel.IFSCCode = model.IFSCCode;
                    employeeaccountModel.MICRCode = model.MICRCode;
                    employeeaccountModel.BankKey = model.BankKey;
                    employeeaccountModel.BranchLocation = model.BranchLocation;
                    employeeaccountModel.NameInAccount = model.NameInAccount;
                    employeeaccountModel.AccountTypeKey = model.AccountTypeKey;
                    employeeaccountModel.AdharNumber = model.AdharNumber;
                    employeeaccountModel.UANNumber = model.UANNumber;
                    dbContext.SaveChanges();
                    transaction.Commit();
                    model.Message = EduSuiteUIResources.Success;
                    model.IsSuccessful = true;

                    ActivityLog.CreateActivityLog(MenuConstants.EmployeeAccount, ActionConstants.Edit, DbConstants.LogType.Info, model.RowKey, model.Message);
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    model.Message = String.Format(EduSuiteUIResources.FailedToSaveErrorMessage, EduSuiteUIResources.Employee + EduSuiteUIResources.BlankSpace + EduSuiteUIResources.BankAccount);
                    model.IsSuccessful = false;
                    ActivityLog.CreateActivityLog(MenuConstants.EmployeeAccount, ActionConstants.Edit, DbConstants.LogType.Error, model.RowKey, ex.GetBaseException().Message);
                }
            }
            return model;
        }

        public EmployeeAccountViewModel DeleteEmployeeAccount(EmployeeAccountViewModel model)
        {

            using (var transaction = dbContext.Database.BeginTransaction())
            {
                try
                {
                    EmployeeAccount employeeaccountModel = dbContext.EmployeeAccounts.SingleOrDefault(row => row.EmployeeKey == model.EmployeeKey);
                    dbContext.EmployeeAccounts.Remove(employeeaccountModel);
                    dbContext.SaveChanges();
                    transaction.Commit();
                    model.Message = EduSuiteUIResources.Success;
                    model.IsSuccessful = true;
                    ActivityLog.CreateActivityLog(MenuConstants.EmployeeAccount, ActionConstants.Delete, DbConstants.LogType.Info, model.RowKey, model.Message);
                }
                catch (DbUpdateException ex)
                {
                    transaction.Rollback();
                    if (ex.GetBaseException().Message.ToUpper().Contains(EduSuiteUIResources.ForeignKeyError.ToUpper()))
                    {
                        model.Message = String.Format(EduSuiteUIResources.CantDeleteErrorMessage, EduSuiteUIResources.Employee + EduSuiteUIResources.BlankSpace + EduSuiteUIResources.BankAccount);
                        model.IsSuccessful = false;
                        ActivityLog.CreateActivityLog(MenuConstants.EmployeeAccount, ActionConstants.Delete, DbConstants.LogType.Debug, model.RowKey, ex.GetBaseException().Message);
                    }
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    model.Message = String.Format(EduSuiteUIResources.FailedToDeleteErrorMessage, EduSuiteUIResources.Employee + EduSuiteUIResources.BlankSpace + EduSuiteUIResources.BankAccount);
                    model.IsSuccessful = false;
                    ActivityLog.CreateActivityLog(MenuConstants.EmployeeAccount, ActionConstants.Delete, DbConstants.LogType.Debug, model.RowKey, ex.GetBaseException().Message);
                }
            }

            return model;
        }

        public EmployeeAccountViewModel CheckAccountNumberExists(string AccountNumber, Int64 RowKey)
        {
            EmployeeAccountViewModel model = new EmployeeAccountViewModel();
            if (dbContext.EmployeeAccounts.Where(row => row.AccountNumber == AccountNumber && row.RowKey != RowKey).Any())
            {
                model.IsSuccessful = false;

            }
            else
            {
                model.IsSuccessful = true;
            }
            return model;
        }

        public EmployeeAccountViewModel CheckAdharNumberExists(string AdharNumber, Int64 RowKey)
        {
            EmployeeAccountViewModel model = new EmployeeAccountViewModel();
            if (dbContext.EmployeeAccounts.Where(row => row.AdharNumber == AdharNumber && row.RowKey != RowKey).Any())
            {
                model.IsSuccessful = false;

            }
            else
            {
                model.IsSuccessful = true;
            }
            return model;
        }
        private void FillAccountTypes(EmployeeAccountViewModel model)
        {
            model.AccountTypes = dbContext.VwBankAccountTypeSelectActiveOnlies.Select(row => new SelectListModel
            {
                RowKey = row.RowKey,
                Text = row.AccountTypeName

            }).ToList();
        }
        private void FillBanks(EmployeeAccountViewModel model)
        {
            model.Banks = dbContext.VwBankSelectActiveOnlies.Select(row => new SelectListModel
            {

                RowKey = row.RowKey,
                Text = row.BankName
            }).ToList();

        }
        private void FillDropdownList(EmployeeAccountViewModel model)
        {
            FillAccountTypes(model);
            FillBanks(model);

        }
    }
}
