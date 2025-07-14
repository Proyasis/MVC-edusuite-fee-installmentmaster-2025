using System;
using System.Collections.Generic;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CITS.EduSuite.Business.Common;
using CITS.EduSuite.Business.Interfaces;
using CITS.EduSuite.Business.Models.ViewModels;
using CITS.EduSuite.Data;

namespace CITS.EduSuite.Business.Services
{
    public class EmployeeSalarySettingsService : IEmployeeSalarySettingsService
    {

        private EduSuiteDatabase dbContext;

        public EmployeeSalarySettingsService(EduSuiteDatabase objDB)
        {
            this.dbContext = objDB;
        }



        public EmployeeSalaryMasterViewModel GetEmployeeSalarySettingsById(long EmployeeId)
        {
            try
            {
                EmployeeSalaryMasterViewModel model = new EmployeeSalaryMasterViewModel();
                //model.EmployeeKey = EmployeeId;
          
                //model = dbContext.spSelectEmployeeSalariesByMonth(model.EmployeeKey, 0, 0)
                //                .GroupBy(row => new
                //                {
                //                    row.SalaryTypeKey,
                //                    row.MonthlySalary

                //                }).Select(S => new EmployeeSalaryMasterViewModel
                //                {

                //                    MonthlySalary = S.Key.MonthlySalary ?? 0,
                //                    SalaryTypeKey=S.Key.SalaryTypeKey,
                //                    EmployeeSalaryEarnings = S.Where(x => x.SalaryHeadTypeKey == DbConstants.SalaryHeadType.MonthlyPayments).Select(g =>
                //       new EmployeeSalaryDetailViewModel
                //       {
                //           SalaryHeadTypeKey = g.SalaryHeadTypeKey,
                //           SalaryHeadCode = g.SalaryHeadCode,
                //           SalaryHeadKey = g.SalaryHeadKey,
                //           SalaryHeadName = g.SalaryHeadName,
                //           Amount = g.Amount,
                //           Formula = g.Formula
                //       }).ToList(),
                //                    EmployeeSalaryDeductions = S.Where(x => x.SalaryHeadTypeKey == DbConstants.SalaryHeadType.StatutoryDeductions).Select(g =>
                //                    new EmployeeSalaryDetailViewModel
                //                    {
                //                        SalaryHeadTypeKey = g.SalaryHeadTypeKey,
                //                        SalaryHeadCode = g.SalaryHeadCode,
                //                        SalaryHeadKey = g.SalaryHeadKey,
                //                        SalaryHeadName = g.SalaryHeadName,
                //                        Amount = g.Amount,
                //                        Formula = g.Formula
                //                    }).ToList()
                //                }).FirstOrDefault();
                //if (model == null)
                //{
                //    model = new EmployeeSalaryMasterViewModel();
                //}

                return model;
            }
            catch (Exception ex)
            {
                return new EmployeeSalaryMasterViewModel();
            }
        }


        public EmployeeSalarySettingsViewModel UpdateEmployeeSalarySettings(EmployeeSalarySettingsViewModel model)
        {
            //FillDropdownList(model.AdditionalSalaryComponents);
            //using (var transaction = dbContext.Database.BeginTransaction())
            //{
            //    try
            //    {
            //        EmployeePFSetting employeePFSettingModel = new EmployeePFSetting();
            //        if (model.ProvidentFundKey != 0)
            //        {
            //            employeePFSettingModel = dbContext.EmployeePFSettings.SingleOrDefault(p => p.RowKey==model.ProvidentFundKey);
            //            employeePFSettingModel.ProvidentFundType = model.ProvidentFundType;
            //            employeePFSettingModel.EmployeeShare = Convert.ToDecimal(model.EmployeeShare);
            //            employeePFSettingModel.EmployerShare = Convert.ToDecimal(model.EmployerShare);
            //        }
            //        else
            //        {
            //            Int64 PFmaxKey = dbContext.EmployeePFSettings.Select(p => p.RowKey).DefaultIfEmpty().Max();
            //            employeePFSettingModel.RowKey = Convert.ToInt64(PFmaxKey + 1);
            //            employeePFSettingModel.EmployeeKey = model.EmployeeKey;
            //            employeePFSettingModel.ProvidentFundType = model.ProvidentFundType;
            //            employeePFSettingModel.EmployeeShare = Convert.ToDecimal(model.EmployeeShare);
            //            employeePFSettingModel.EmployerShare = Convert.ToDecimal(model.EmployerShare);
            //            dbContext.EmployeePFSettings.Add(employeePFSettingModel);
            //        }

            //        CreateEmployeeSalarySettings(model.AdditionalSalaryComponents.Where(row => row.RowKey == 0).ToList(), model.EmployeeKey);
            //        UpdateEmployeeSalarySettings(model.AdditionalSalaryComponents.Where(row => row.RowKey != 0).ToList(), model.EmployeeKey);
            //        dbContext.SaveChanges();
            //        transaction.Commit();
            //        model.Message = ApplicationResources.Success;
            //        model.IsSuccessful = true;
            //    }
            //    catch (Exception ex)
            //    {
            //        transaction.Rollback();
            //        model.Message = ApplicationResources.FailedToSaveSalarySettings;
            //        model.IsSuccessful = false;
            //    }

            //}
            return model;
        }








        //private void CreateEmployeeSalarySettings(List<AdditionalSalaryComponentViewModel> modelList, long EmployeeKey)
        //{

        //    Int64 maxKey = dbContext.AdditionalSalaryComponents.Select(p => p.RowKey).DefaultIfEmpty().Max();
        //    foreach (AdditionalSalaryComponentViewModel model in modelList)
        //    {

        //        AdditionalSalaryComponent employeeSalarySettingsModel = new AdditionalSalaryComponent();
        //        employeeSalarySettingsModel.RowKey = Convert.ToInt64(maxKey + 1);
        //        employeeSalarySettingsModel.EmployeeKey = EmployeeKey;
        //        employeeSalarySettingsModel.AmountUnit = Convert.ToDecimal(model.Amount);
        //        employeeSalarySettingsModel.UnitType = model.UnitType;
        //        employeeSalarySettingsModel.AdditionalComponentTypeKey = model.AdditionalComponentTypeKey;
        //        dbContext.AdditionalSalaryComponents.Add(employeeSalarySettingsModel);
        //        maxKey++;

        //    }

        //}



        //public void UpdateEmployeeSalarySettings(List<AdditionalSalaryComponentViewModel> modelList, long EmployeeKey)
        //{

        //    foreach (AdditionalSalaryComponentViewModel model in modelList)
        //    {


        //        AdditionalSalaryComponent employeeSalarySettingsModel = new AdditionalSalaryComponent();
        //        employeeSalarySettingsModel = dbContext.AdditionalSalaryComponents.SingleOrDefault(row => row.RowKey == model.RowKey);
        //        employeeSalarySettingsModel.AmountUnit = Convert.ToDecimal(model.Amount);
        //        employeeSalarySettingsModel.UnitType = model.UnitType;
        //        employeeSalarySettingsModel.AdditionalComponentTypeKey = model.AdditionalComponentTypeKey;


        //    }
        //}


        public EmployeeSalarySettingsViewModel DeleteEmployeeSalarySettings(AdditionalSalaryComponentViewModel model)
        {
            EmployeeSalarySettingsViewModel employeeSalarySettingsModel = new EmployeeSalarySettingsViewModel();
            //FillDropdownList(employeeSalarySettingsModel.AdditionalSalaryComponents);

            //using (var transaction = dbContext.Database.BeginTransaction())
            //{
            //    try
            //    {
            //        AdditionalSalaryComponent EmployeeSalarySettings = dbContext.AdditionalSalaryComponents.SingleOrDefault(row => row.RowKey == model.RowKey);
            //        dbContext.AdditionalSalaryComponents.Remove(EmployeeSalarySettings);
            //        dbContext.SaveChanges();
            //        transaction.Commit();
            //        employeeSalarySettingsModel.Message = ApplicationResources.Success;
            //        employeeSalarySettingsModel.IsSuccessful = true;
            //    }
            //    catch (DbUpdateException ex)
            //    {
            //        transaction.Rollback();
            //        if (ex.GetBaseException().Message.ToUpper().Contains(ApplicationResources.ForeignKeyError.ToUpper()))
            //        {
            //            employeeSalarySettingsModel.Message = ApplicationResources.CantDeleteEmployeeSalarySettings;
            //            employeeSalarySettingsModel.IsSuccessful = false;
            //        }
            //    }
            //    catch (Exception ex)
            //    {
            //        transaction.Rollback();
            //        employeeSalarySettingsModel.Message = ApplicationResources.FailedToDeleteSalarySettings;
            //        employeeSalarySettingsModel.IsSuccessful = false;
            //    }
            //}
            return employeeSalarySettingsModel;
        }



        //public EmployeeSalarySettingsViewModel CheckEmployeeSalarySettingsTypeExists(short AdditionalComponentTypeKey, long EmployeeKey, long RowKey)
        //{
        //    EmployeeSalarySettingsViewModel model = new EmployeeSalarySettingsViewModel();
        //    if (dbContext.AdditionalSalaryComponents.Where(row => row.AdditionalComponentTypeKey == AdditionalComponentTypeKey && row.EmployeeKey == EmployeeKey && row.RowKey != RowKey).Any())
        //    {
        //        model.IsSuccessful = false;

        //    }
        //    else
        //    {
        //        model.IsSuccessful = true;
        //    }
        //    return model;
        //}

        //private void FillProvindentFund(EmployeeSalarySettingsViewModel model)
        //{

        //    EmployeePFSetting employeePFSetting = new EmployeePFSetting();
        //    employeePFSetting = dbContext.EmployeePFSettings.SingleOrDefault(row => row.EmployeeKey == model.EmployeeKey);
        //    if (employeePFSetting != null)
        //    {
        //        model.ProvidentFundKey = employeePFSetting.RowKey;
        //        model.EmployeeShare =employeePFSetting.EmployeeShare;
        //        model.EmployerShare = employeePFSetting.EmployerShare;
        //        model.ProvidentFundType = employeePFSetting.ProvidentFundType;

        //    }


        //}

        private void FillDropdownList(List<AdditionalSalaryComponentViewModel> modelList)
        {
            //modelList.ForEach(x => FillIdentityTypes(x));
        }


        //private void FillIdentityTypes(AdditionalSalaryComponentViewModel model)
        //{
        //    model.AdditionalComponentTypes = dbContext.VwAdditionalComponentTypeSelectActiveOnlies.OrderBy(row => row.DisplayOrder).Select(row => new SelectListModel
        //    {
        //        RowKey = row.RowKey,
        //        Text = row.AdditionalComponentTypeName
        //    }).ToList();
        //}




    }
}
