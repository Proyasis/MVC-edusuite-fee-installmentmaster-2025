using CITS.EduSuite.Business.Interfaces;
using CITS.EduSuite.Business.Common;
using CITS.EduSuite.Data;
using CITS.EduSuite.Business.Models.ViewModels;
using System;
using System.Collections.Generic;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CITS.EduSuite.Business.Models.Resources;
using System.IO;

namespace CITS.EduSuite.Business.Services
{
    public class BranchService : IBranchService
    {
        private EduSuiteDatabase dbContext;
        public BranchService(EduSuiteDatabase objDB)
        {
            this.dbContext = objDB;
        }

        public List<BranchViewModel> GetBranches(string searchText)
        {
            try
            {
                var branchesList = (from b in dbContext.Branches
                                    orderby b.RowKey descending
                                    where (b.BranchName.Contains(searchText))
                                    select new BranchViewModel
                                    {
                                        RowKey = b.RowKey,
                                        BranchName = b.BranchName,
                                        BranchNameLocal = b.BranchNameLocal,
                                        BranchCode = b.BranchCode,
                                        CityName = b.CityName,
                                        PhoneNumber1 = b.PhoneNumber1,
                                        PhoneNumber2 = b.PhoneNumber2,
                                        ContactPersonPhone = b.ContactPersonPhone,
                                        PostalCode = b.PostalCode,
                                        ContactPersonName = b.ContactPersonName,
                                        AddressLine1 = b.AddressLine1,
                                        AddressLine2 = b.AddressLine2,
                                        AddressLine3 = b.AddressLine3,
                                        EmailAddress = b.EmailAddress,
                                        FaxNumber = b.FaxNumber,
                                        DesignationName = b.Designation.DesignationName,
                                        DistrictName = b.District.DistrictName,
                                        DisplayOrder = b.DisplayOrder,
                                        OpeningCashBalance = b.OpeningCashBalance,
                                        CurrentCashBalance = b.CurrentCashBalance,
                                        IsActive = b.IsActive,
                                        DepartmentCount = b.BranchDepartments.Count(),
                                        IsFranchise = b.IsFranchise,
                                        BranchLogo = b.BranchLogo,
                                        BranchLogoPath = b.BranchLogo != null ? UrlConstants.BranchLogo + "/" + b.BranchLogo : EduSuiteUIResources.DefaultPhotoUrl,
                                    }).ToList();

                return branchesList.GroupBy(x => x.RowKey).Select(y => y.First()).ToList<BranchViewModel>();
            }
            catch (Exception ex)
            {
                ActivityLog.CreateActivityLog(MenuConstants.Branch, ActionConstants.View, DbConstants.LogType.Error, null, ex.GetBaseException().Message);
                return new List<BranchViewModel>();
            }
        }
        public BranchViewModel GetBranchById(short? id)
        {
            try
            {
                BranchViewModel model = new BranchViewModel();
                model = dbContext.Branches.Select(row => new BranchViewModel
                {
                    RowKey = row.RowKey,
                    BranchCode = row.BranchCode,
                    BranchName = row.BranchName,
                    BranchNameLocal = row.BranchNameLocal,
                    CityName = row.CityName,
                    PhoneNumber1 = row.PhoneNumber1,
                    PhoneNumber2 = row.PhoneNumber2,
                    ContactPersonPhone = row.ContactPersonPhone,
                    PostalCode = row.PostalCode,
                    ContactPersonName = row.ContactPersonName,
                    AddressLine1 = row.AddressLine1,
                    AddressLine2 = row.AddressLine2,
                    AddressLine3 = row.AddressLine3,
                    EmailAddress = row.EmailAddress,
                    FaxNumber = row.FaxNumber,
                    CountryKey = row.District.Province.CountryKey,
                    ProvinceKey = row.District.ProvinceKey,
                    DistrictKey = row.DistrictKey,
                    TelephoneCode = row.District.Province.Country.TelephoneCode,
                    DesignationKey = row.DesignationKey,
                    DisplayOrder = row.DisplayOrder,
                    IsActive = row.IsActive,
                    OpeningCashBalance = row.OpeningCashBalance,
                    CurrentCashBalance = row.CurrentCashBalance,
                    DepartmentKeys = row.BranchDepartments.Select(Dep => Dep.DepartmentKey).ToList(),
                    IsFranchise = row.IsFranchise,
                    BranchLogo = row.BranchLogo,
                    BranchLogoPath = row.BranchLogo != null ? UrlConstants.BranchLogo + "/" + row.BranchLogo : EduSuiteUIResources.DefaultPhotoUrl,
                }).Where(x => x.RowKey == id).FirstOrDefault();
                if (model == null)
                {
                    model = new BranchViewModel();
                }
                FillDropdownLists(model);
                return model;
            }
            catch (Exception ex)
            {
                ActivityLog.CreateActivityLog(MenuConstants.Branch, ActionConstants.View, DbConstants.LogType.Error, null, ex.GetBaseException().Message);
                return new BranchViewModel();
            }
        }
        public BranchViewModel CreateBranch(BranchViewModel model)
        {
            FillDropdownLists(model);
            Branch branchModel = new Branch();
            var branchCodeCheck = dbContext.Branches.Where(row => row.BranchCode.ToLower() == model.BranchCode.ToLower()).ToList();

            if (branchCodeCheck.Count != 0)
            {
                model.Message = String.Format(EduSuiteUIResources.ExistsErrorMessage, EduSuiteUIResources.Branch);
                model.IsSuccessful = false;
                return model;
            }

            using (var transaction = dbContext.Database.BeginTransaction())
            {
                try
                {

                    Int16 maxKey = dbContext.Branches.Select(p => p.RowKey).DefaultIfEmpty().Max();

                    branchModel.RowKey = Convert.ToInt16(maxKey + 1);
                    branchModel.BranchCode = model.BranchCode;
                    branchModel.BranchName = model.BranchName;
                    branchModel.BranchNameLocal = model.BranchNameLocal;
                    branchModel.CityName = model.CityName;
                    branchModel.DistrictKey = model.DistrictKey;
                    branchModel.DesignationKey = model.DesignationKey;
                    branchModel.AddressLine1 = model.AddressLine1;
                    branchModel.AddressLine2 = model.AddressLine2;
                    branchModel.AddressLine3 = model.AddressLine3;
                    branchModel.PhoneNumber1 = model.PhoneNumber1;
                    branchModel.PhoneNumber2 = model.PhoneNumber2;
                    branchModel.PostalCode = model.PostalCode;
                    branchModel.EmailAddress = model.EmailAddress;
                    branchModel.FaxNumber = model.FaxNumber;
                    branchModel.ContactPersonName = model.ContactPersonName;
                    branchModel.ContactPersonPhone = model.ContactPersonPhone;
                    branchModel.IsActive = model.IsActive;
                    branchModel.DisplayOrder = Convert.ToInt16(maxKey + 1);
                    branchModel.OpeningCashBalance = model.OpeningCashBalance;
                    branchModel.CurrentCashBalance = model.OpeningCashBalance;
                    branchModel.IsFranchise = model.IsFranchise;
                    if (model.PhotoFile != null)
                    {
                        string Extension = Path.GetExtension(model.PhotoFile.FileName);
                        string FileName = model.BranchCode + Extension;
                        branchModel.BranchLogo = FileName;
                    }

                    branchModel.CompanyKey = 1;//For temporary allocation
                    dbContext.Branches.Add(branchModel);

                    //List<AccountFlowViewModel> accountFlowModelList = new List<AccountFlowViewModel>();
                    //CreditAmountList(accountFlowModelList, false, branchModel);
                    //CreateAccountFlow(accountFlowModelList, false);

                    model.RowKey = branchModel.RowKey;
                    CreateDepartment(model);
                    CreateAttendanceConfiguration(model);
                    dbContext.SaveChanges();
                    transaction.Commit();
                    model.Message = EduSuiteUIResources.Success;
                    model.IsSuccessful = true;
                    ActivityLog.CreateActivityLog(MenuConstants.Branch, ActionConstants.Add, DbConstants.LogType.Info, model.RowKey, model.Message);
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    model.Message = String.Format(EduSuiteUIResources.FailedToSaveErrorMessage, EduSuiteUIResources.Branch);
                    model.IsSuccessful = false;
                    ActivityLog.CreateActivityLog(MenuConstants.Branch, ActionConstants.Add, DbConstants.LogType.Error, model.RowKey, ex.GetBaseException().Message);
                }
            }
            //}
            return model;
        }
        public BranchViewModel UpdateBranch(BranchViewModel model)
        {
            FillDropdownLists(model);
            Branch branchModel = new Branch();
            var branchCodeCheck = dbContext.Branches.Where(row => row.RowKey != model.RowKey && row.BranchCode.ToLower() == model.BranchCode.ToLower()).ToList();
            if (branchCodeCheck.Count != 0)
            {
                model.Message = String.Format(EduSuiteUIResources.ExistsErrorMessage, EduSuiteUIResources.Branch);
                model.IsSuccessful = false;
                return model;
            }

            using (var transaction = dbContext.Database.BeginTransaction())
            {
                try
                {
                    branchModel = dbContext.Branches.SingleOrDefault(row => row.RowKey == model.RowKey);
                    branchModel.BranchCode = model.BranchCode;
                    branchModel.BranchName = model.BranchName;
                    branchModel.BranchNameLocal = model.BranchNameLocal;
                    branchModel.CityName = model.CityName;
                    branchModel.DistrictKey = model.DistrictKey;
                    branchModel.DesignationKey = model.DesignationKey;
                    branchModel.AddressLine1 = model.AddressLine1;
                    branchModel.AddressLine2 = model.AddressLine2;
                    branchModel.AddressLine3 = model.AddressLine3;
                    branchModel.PhoneNumber1 = model.PhoneNumber1;
                    branchModel.PhoneNumber2 = model.PhoneNumber2;
                    branchModel.PostalCode = model.PostalCode;
                    branchModel.EmailAddress = model.EmailAddress;
                    branchModel.FaxNumber = model.FaxNumber;
                    branchModel.ContactPersonName = model.ContactPersonName;
                    branchModel.ContactPersonPhone = model.ContactPersonPhone;
                    branchModel.IsActive = model.IsActive;
                    branchModel.OpeningCashBalance = model.OpeningCashBalance;
                    branchModel.IsFranchise = model.IsFranchise;
                    if (model.PhotoFile != null)
                    {
                        string Extension = Path.GetExtension(model.PhotoFile.FileName);
                        string FileName = model.BranchCode + Extension;
                        branchModel.BranchLogo = FileName;
                    }

                    //if (!dbContext.CashFlows.Where(row => row.BranchKey == model.RowKey).Any())
                    //{
                    //    branchModel.CurrentCashBalance = model.OpeningCashBalance;
                    //}

                    //List<AccountFlowViewModel> accountFlowModelList = new List<AccountFlowViewModel>();
                    //CreditAmountList(accountFlowModelList, true, branchModel);
                    //CreateAccountFlow(accountFlowModelList, true);

                    UpdateDepartment(model);

                    dbContext.SaveChanges();
                    transaction.Commit();
                    model.Message = EduSuiteUIResources.Success;
                    model.IsSuccessful = true;
                    ActivityLog.CreateActivityLog(MenuConstants.Branch, ActionConstants.Edit, DbConstants.LogType.Info, model.RowKey, model.Message);
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    model.Message = String.Format(EduSuiteUIResources.FailedToSaveErrorMessage, EduSuiteUIResources.Branch);
                    model.IsSuccessful = false;
                    ActivityLog.CreateActivityLog(MenuConstants.Branch, ActionConstants.Edit, DbConstants.LogType.Error, model.RowKey, ex.GetBaseException().Message);
                }
            }
            return model;
        }
        #region Account
        private void CreateAccountFlow(List<AccountFlowViewModel> modelList, bool IsUpdate)
        {
            AccountFlowService accounFlowService = new AccountFlowService(dbContext);
            List<AccountFlowViewModel> accountFlowModelList = new List<AccountFlowViewModel>();
            if (IsUpdate != true)
            {
                accounFlowService.CreateAccountFlow(modelList);
            }
            else
            {
                accounFlowService.UpdateAccountFlow(modelList);
            }
        }
        private void CreditAmountList(List<AccountFlowViewModel> accountFlowModelList, bool IsUpdate, Branch branchModel)
        {
            long ExtraUpdateKey = 0;
            long accountHeadKey;

            accountHeadKey = dbContext.AccountHeads.Where(x => x.RowKey == DbConstants.AccountHead.CashAccount && x.IsActive).Select(x => x.RowKey).FirstOrDefault();
            accountFlowModelList.Add(new AccountFlowViewModel
            {
                CashFlowTypeKey = DbConstants.CashFlowType.In,
                AccountHeadKey = accountHeadKey,
                Amount = branchModel.OpeningCashBalance ?? 0,
                TransactionTypeKey = DbConstants.TransactionType.Branch,
                TransactionDate = DateTimeUTC.Now,
                TransactionKey = branchModel.RowKey,
                ExtraUpdateKey = ExtraUpdateKey,
                IsUpdate = IsUpdate,
                VoucherTypeKey = DbConstants.VoucherType.OpeningBalance,
                BranchKey = branchModel.RowKey,
                Purpose = branchModel.BranchName + EduSuiteUIResources.OpenBracket + EduSuiteUIResources.BlankSpace + branchModel.BranchCode + EduSuiteUIResources.BlankSpace + EduSuiteUIResources.CloseBracket + EduSuiteUIResources.BlankSpace + EduSuiteUIResources.Hyphen + EduSuiteUIResources.BlankSpace + EduSuiteUIResources.OpeningBalance,
            });

            accountHeadKey = dbContext.AccountHeads.Where(x => x.RowKey == DbConstants.AccountHead.OpeningBalance && x.IsActive).Select(x => x.RowKey).FirstOrDefault();
            accountFlowModelList.Add(new AccountFlowViewModel
            {
                CashFlowTypeKey = DbConstants.CashFlowType.Out,
                AccountHeadKey = accountHeadKey,
                Amount = branchModel.OpeningCashBalance ?? 0,
                TransactionTypeKey = DbConstants.TransactionType.Branch,
                TransactionDate = DateTimeUTC.Now,
                TransactionKey = branchModel.RowKey,
                ExtraUpdateKey = ExtraUpdateKey,
                IsUpdate = IsUpdate,
                VoucherTypeKey = DbConstants.VoucherType.OpeningBalance,
                BranchKey = branchModel.RowKey,
                Purpose = branchModel.BranchName + EduSuiteUIResources.OpenBracket + EduSuiteUIResources.BlankSpace + branchModel.BranchCode + EduSuiteUIResources.BlankSpace + EduSuiteUIResources.CloseBracket + EduSuiteUIResources.BlankSpace + EduSuiteUIResources.Hyphen + EduSuiteUIResources.BlankSpace + EduSuiteUIResources.OpeningBalance,

            });

        }

        #endregion
        public BranchViewModel DeleteBranch(BranchViewModel model)
        {
            using (var transaction = dbContext.Database.BeginTransaction())
            {
                try
                {
                    Branch Branch = dbContext.Branches.SingleOrDefault(row => row.RowKey == model.RowKey);
                    List<BranchDepartment> branchDepertment = dbContext.BranchDepartments.Where(x => x.BranchKey == model.RowKey).ToList();
                    if (branchDepertment.Count > 0)
                    {
                        dbContext.BranchDepartments.RemoveRange(branchDepertment);
                    }
                    AttendanceConfiguration AttendanceConfigurationModel = dbContext.AttendanceConfigurations.FirstOrDefault(row => row.BranchKey == model.RowKey);
                    if (AttendanceConfigurationModel != null)
                    {
                        dbContext.AttendanceConfigurations.Remove(AttendanceConfigurationModel);
                    }
                    AccountFlowViewModel accountFlowModel = new AccountFlowViewModel();
                    accountFlowModel.TransactionTypeKey = DbConstants.TransactionType.Branch;
                    accountFlowModel.TransactionKey = model.RowKey;
                    accountFlowModel.IsDelete = false;
                    AccountFlowService accountFlowService = new AccountFlowService(dbContext);
                    accountFlowService.DeleteAccountFlow(accountFlowModel);
                    dbContext.Branches.Remove(Branch);
                    dbContext.SaveChanges();
                    transaction.Commit();
                    model.Message = EduSuiteUIResources.Success;
                    model.IsSuccessful = true;

                    ActivityLog.CreateActivityLog(MenuConstants.Branch, ActionConstants.Delete, DbConstants.LogType.Info, model.RowKey, model.Message);
                }
                catch (DbUpdateException ex)
                {
                    transaction.Rollback();
                    if (ex.GetBaseException().Message.ToUpper().Contains(EduSuiteUIResources.ForeignKeyError.ToUpper()))
                    {
                        model.Message = String.Format(EduSuiteUIResources.CantDeleteErrorMessage, EduSuiteUIResources.Branch);
                        model.IsSuccessful = false;
                        ActivityLog.CreateActivityLog(MenuConstants.Branch, ActionConstants.Delete, DbConstants.LogType.Debug, model.RowKey, ex.GetBaseException().Message);
                    }
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    model.Message = String.Format(EduSuiteUIResources.FailedToDeleteErrorMessage, EduSuiteUIResources.Branch);
                    model.IsSuccessful = false;
                    ActivityLog.CreateActivityLog(MenuConstants.Branch, ActionConstants.Delete, DbConstants.LogType.Debug, model.RowKey, ex.GetBaseException().Message);
                }
            }
            //}
            return model;
        }
        public void CreateDepartment(BranchViewModel model)
        {

            int maxKey = dbContext.BranchDepartments.Select(p => p.RowKey).DefaultIfEmpty().Max();

            foreach (SelectListModel modelItem in model.Departments)
            {
                BranchDepartment BranchDepartmentModel = new BranchDepartment();
                BranchDepartmentModel.RowKey = maxKey + 1;
                BranchDepartmentModel.BranchKey = model.RowKey;
                BranchDepartmentModel.DepartmentKey = Convert.ToInt16(modelItem.RowKey);
                dbContext.BranchDepartments.Add(BranchDepartmentModel);
                maxKey++;
            }

        }
        public void UpdateDepartment(BranchViewModel model)
        {

            List<BranchDepartment> branchDepartmentList = new List<BranchDepartment>();
            //List<BranchDepartmentViewModel> branchDepartmentListInsert = new List<BranchDepartmentViewModel>();
            branchDepartmentList = dbContext.BranchDepartments.Where(row => row.BranchKey == model.RowKey).ToList();
            branchDepartmentList.Where(row => !model.DepartmentKeys.Contains(row.DepartmentKey)).ToList().ForEach(BranchDepartment => dbContext.BranchDepartments.Remove(BranchDepartment));
            int maxKey = dbContext.BranchDepartments.Select(p => p.RowKey).DefaultIfEmpty().Max();
            foreach (short DepartmentKey in model.DepartmentKeys)
            {
                if (!branchDepartmentList.Where(row => row.DepartmentKey == DepartmentKey).Any())
                {

                    BranchDepartment BranchDepartmentModel = new BranchDepartment();
                    BranchDepartmentModel.RowKey = maxKey + 1;
                    BranchDepartmentModel.BranchKey = model.RowKey;
                    BranchDepartmentModel.DepartmentKey = Convert.ToInt16(DepartmentKey);
                    dbContext.BranchDepartments.Add(BranchDepartmentModel);
                    maxKey++;
                }
            }
        }
        public BranchViewModel GetProvinceAndCodeByCountry(short CountryKey)
        {
            BranchViewModel model = new BranchViewModel();
            model.CountryKey = CountryKey;
            FillProvinceById(model);
            model.TelephoneCode = dbContext.Countries.Where(row => row.RowKey == CountryKey).Select(row => row.TelephoneCode).SingleOrDefault();
            return model;
        }
        public BranchViewModel GetDistrictByProvince(int ProvinceKey)
        {
            BranchViewModel model = new BranchViewModel();
            model.ProvinceKey = ProvinceKey;
            FillDistrictsById(model);
            return model;
        }
        public void FillDropdownLists(BranchViewModel model)
        {
            FillCountries(model);
            FillProvinceById(model);
            FillDistrictsById(model);
            FillDesignation(model);
            FillDepartment(model);
        }
        private void FillDepartment(BranchViewModel model)
        {
            model.Departments = dbContext.VwDepartmentSelectActiveOnlies.OrderBy(row => row.DisplayOrder).Select(row => new SelectListModel
            {
                RowKey = row.RowKey,
                Text = row.DepartmentName
            }).ToList();
        }
        private void FillDesignation(BranchViewModel model)
        {
            model.Designations = dbContext.VwDesignationWithoutAdminSelectActiveOnlies.OrderBy(row => row.DisplayOrder).Select(row => new SelectListModel
            {
                RowKey = row.RowKey,
                Text = row.DesignationName
            }).ToList();
        }
        private void FillCountries(BranchViewModel model)
        {
            model.Countries = dbContext.VwCountrySelectActiveOnlies.OrderBy(row => row.DisplayOrder).Select(row => new SelectListModel
            {
                RowKey = row.RowKey,
                Text = row.CountryName
            }).ToList();
            if (model.Countries.Count == 1)
            {
                long? CountryKey = model.Countries.Select(x => x.RowKey).FirstOrDefault();
                model.CountryKey = Convert.ToInt16(CountryKey);
            }
        }
        private void FillProvinceById(BranchViewModel model)
        {
            model.Provinces = dbContext.VwProvinceSelectActiveOnlies.OrderBy(row => row.DisplayOrder).Where(row => row.CountryKey == model.CountryKey).Select(row => new SelectListModel
            {
                RowKey = row.RowKey,
                Text = row.Provincename
            }).ToList();

            if (model.Provinces.Count == 1)
            {
                long? ProvinceKey = model.Provinces.Select(x => x.RowKey).FirstOrDefault();
                model.ProvinceKey = Convert.ToInt16(ProvinceKey);
            }
        }
        private void FillDistrictsById(BranchViewModel model)
        {
            model.Districts = dbContext.VwDistrictSelectActiveOnlies.OrderBy(row => row.DisplayOrder).Where(row => row.ProvinceKey == model.ProvinceKey).Select(row => new SelectListModel
            {
                RowKey = row.RowKey,
                Text = row.DistrictName
            }).ToList();
        }
        public BranchViewModel CheckBranchCodeExist(BranchViewModel model)
        {
            if (dbContext.Branches.Where(x => x.BranchCode.ToLower() == model.BranchCode.ToLower() && x.RowKey != model.RowKey).Any())
            {
                model.IsSuccessful = false;
                model.Message = String.Format(EduSuiteUIResources.ExistsErrorMessage, EduSuiteUIResources.Branch + EduSuiteUIResources.Code);
            }
            else
            {
                model.IsSuccessful = true;
                model.Message = "";
            }
            return model;
        }
        //public BranchViewModel CheckBranchLocationExist(BranchViewModel model)
        //{
        //    if (dbContext.M_Branch.Where(x => x.BranchLocation.ToLower() == model.BranchLocation.ToLower() && x.RowKey != model.RowKey).Any())
        //    {
        //        model.IsSuccessful = false;
        //        model.Message = EduSuiteUIResources.ErrorBranchLocationExists;
        //    }
        //    else
        //    {
        //        model.IsSuccessful = true;
        //        model.Message = "";
        //    }
        //    return model;
        //}
        public BranchViewModel DeleteBranchLogo(short Id)
        {
            BranchViewModel model = new BranchViewModel();
            Branch Branch = new Branch();
            using (var transaction = dbContext.Database.BeginTransaction())
            {
                try
                {

                    Branch = dbContext.Branches.SingleOrDefault(row => row.RowKey == Id);
                    model.BranchLogo = Branch.BranchLogo;
                    model.BranchLogoPath = UrlConstants.BranchLogo + "/" + Branch.BranchLogo;
                    model.RowKey = Branch.RowKey;
                    model.BranchCode = Branch.BranchCode;
                    Branch.BranchLogo = null;
                    dbContext.SaveChanges();
                    transaction.Commit();
                    model.Message = EduSuiteUIResources.Success;
                    model.IsSuccessful = true;
                    ActivityLog.CreateActivityLog(MenuConstants.Branch, ActionConstants.Edit, DbConstants.LogType.Info, model.RowKey, model.Message);
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    model.Message = String.Format(EduSuiteUIResources.FailedToDeleteErrorMessage, EduSuiteUIResources.Branch);
                    model.IsSuccessful = false;
                    ActivityLog.CreateActivityLog(MenuConstants.Branch, ActionConstants.Edit, DbConstants.LogType.Error, model.RowKey, ex.GetBaseException().Message);
                }
            }
            return model;
        }

        public void CreateAttendanceConfiguration(BranchViewModel model)
        {
            AttendanceConfiguration attendanceConfigurationModel = new AttendanceConfiguration();
            AttendanceConfiguration AttendanceConfigurationexistingModel = dbContext.AttendanceConfigurations.FirstOrDefault();
            if (AttendanceConfigurationexistingModel != null)
            {
                Int32 maxKey = dbContext.AttendanceConfigurations.Select(p => p.RowKey).DefaultIfEmpty().Max();
                attendanceConfigurationModel.RowKey = Convert.ToInt32(maxKey + 1);
                attendanceConfigurationModel.BranchKey = model.RowKey;
                attendanceConfigurationModel.AttendanceConfigTypeKey = AttendanceConfigurationexistingModel.AttendanceConfigTypeKey;
                attendanceConfigurationModel.TotalWorkingHours = Convert.ToDecimal(AttendanceConfigurationexistingModel.TotalWorkingHours);
                attendanceConfigurationModel.OvertimeAdditionAmount = AttendanceConfigurationexistingModel.OvertimeAdditionAmount;
                attendanceConfigurationModel.UnitTypeKey = AttendanceConfigurationexistingModel.UnitTypeKey;
                attendanceConfigurationModel.AutoApproval = AttendanceConfigurationexistingModel.AutoApproval;
                attendanceConfigurationModel.MinimmDifferencePunch = AttendanceConfigurationexistingModel.MinimmDifferencePunch;
                attendanceConfigurationModel.BaseDaysPerMonth = AttendanceConfigurationexistingModel.BaseDaysPerMonth;
                dbContext.AttendanceConfigurations.Add(attendanceConfigurationModel);
            }
            else
            {
                Int32 maxKey = dbContext.AttendanceConfigurations.Select(p => p.RowKey).DefaultIfEmpty().Max();
                attendanceConfigurationModel.RowKey = Convert.ToInt32(maxKey + 1);
                attendanceConfigurationModel.BranchKey = model.RowKey;
                attendanceConfigurationModel.AttendanceConfigTypeKey = DbConstants.AttendanceConfigType.MarkPresent;
                attendanceConfigurationModel.TotalWorkingHours = Convert.ToDecimal(8);
                attendanceConfigurationModel.OvertimeAdditionAmount = null;
                attendanceConfigurationModel.UnitTypeKey = null;
                attendanceConfigurationModel.AutoApproval = false;
                attendanceConfigurationModel.MinimmDifferencePunch = null;
                attendanceConfigurationModel.BaseDaysPerMonth = Convert.ToDecimal(30);
                dbContext.AttendanceConfigurations.Add(attendanceConfigurationModel);
            }
        }
    }
}
