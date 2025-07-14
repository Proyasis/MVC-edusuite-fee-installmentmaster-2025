using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
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
    public class SharedService : ISharedService
    {
        private EduSuiteDatabase dbContext;

        public SharedService(EduSuiteDatabase objDB)
        {
            this.dbContext = objDB;
        }
        public List<SelectListModel> GetBranches()
        {

            return (dbContext.vwBranchSelectActiveOnlies.OrderBy(row => row.DisplayOrder).Select(row => new SelectListModel
            {
                RowKey = row.RowKey,
                Text = row.BranchName
            }).ToList());

        }
        public List<MenuViewModel> GetMenuByUserId(MenuViewModel model)
        {
            string[] NonMenuCodes = { MenuConstants.ChangePassword };
            List<MenuViewModel> menuList = new List<MenuViewModel>();
            if (DbConstants.User.UserKey != DbConstants.AdminKey)
            {
                menuList = dbContext.MenuTypes.Where(row => row.IsActive).OrderBy(row => row.DisplayOrder).Select(row => new MenuViewModel
                {
                    MenuTypeName = row.MenuTypeName,
                    MenuTypeIconClassName = row.IconCLassName,
                    MenuDetails = dbContext.UserPermissions.Where(M => M.IsActive && M.Menu.IsActive == true && M.Menu.MenuTypeKey == row.RowKey && M.Employee.AppUserKey == DbConstants.User.UserKey && M.ActionKey == 1 && !NonMenuCodes.Contains(M.Menu.MenuCode))
                    .OrderBy(M => M.Menu.MenuCatagory.DisplayOrder).ThenBy(M => M.Menu.DisplayOrder)
                    .Select(M => new MenuDetailViewModel
                    {
                        MenuName = M.Menu.MenuName,
                        MenuCode = M.Menu.MenuCode,
                        ActionName = M.Menu.ActionName,
                        ControllerName = M.Menu.ControllerName,
                        OptionalParameter = M.Menu.OptionalParameter,
                        IconClassName = M.Menu.IconClassName,
                        MenucatagoryName = M.Menu.MenuCatagoryKey != null ? M.Menu.MenuCatagory.CatagoryName : "",
                        MenucatagoryKey = M.Menu.MenuCatagoryKey
                    }).ToList()

                }).ToList();
            }
            else
            {
                menuList = dbContext.MenuTypes.Where(row => row.IsActive).OrderBy(row => row.DisplayOrder).Select(row => new MenuViewModel
                {
                    MenuTypeName = row.MenuTypeName,
                    MenuTypeIconClassName = row.IconCLassName,
                    MenuDetails = dbContext.MenuActions.Where(M => M.IsActive && M.Menu.IsActive == true && M.Menu.MenuTypeKey == row.RowKey && M.ActionKey == 1 && !NonMenuCodes.Contains(M.Menu.MenuCode))
                    .OrderBy(M => M.Menu.MenuCatagory.DisplayOrder).ThenBy(M => M.Menu.DisplayOrder)
                    .Select(M => new MenuDetailViewModel
                    {
                        MenuName = M.Menu.MenuName,
                        MenuCode = M.Menu.MenuCode,
                        ActionName = M.Menu.ActionName,
                        ControllerName = M.Menu.ControllerName,
                        OptionalParameter = M.Menu.OptionalParameter,
                        IconClassName = M.Menu.IconClassName,
                        MenucatagoryName = M.Menu.MenuCatagoryKey != null ? M.Menu.MenuCatagory.CatagoryName : "",
                        MenucatagoryKey = M.Menu.MenuCatagoryKey

                    }).ToList()
                }).ToList();

            }
            return menuList;
        }
        //public BranchViewModel GetBranchDetailById(short? id)
        //{
        //    return dbContext.Branches.Where(x => x.RowKey == id).Select(row => new BranchViewModel
        //    {
        //        BranchCode = row.BranchCode,
        //        BranchName = row.BranchName,
        //        BranchNameLocal = row.BranchNameLocal,
        //        CityName = row.CityName,
        //        PhoneNumber1 = row.PhoneNumber1,
        //        PhoneNumber2 = row.PhoneNumber2,
        //        ContactPersonPhone = row.ContactPersonPhone,
        //        PostalCode = row.PostalCode,
        //        AddressLine1 = row.AddressLine1,
        //        AddressLine2 = row.AddressLine2,
        //        AddressLine3 = row.AddressLine3,
        //        EmailAddress = row.EmailAddress,
        //        FaxNumber = row.FaxNumber,
        //        TelephoneCode = row.District.Province.Country.TelephoneCode,
        //    }).FirstOrDefault();
        //}

        //public List<MenuViewModel> GetMenuByUserId(MenuViewModel model)
        //{
        //    string[] NonMenuCodes = { MenuConstants.ChangePassword };
        //    List<MenuViewModel> menuList = new List<MenuViewModel>();
        //    if (DbConstants.User.UserKey != DbConstants.AdminKey)
        //    {
        //        menuList = dbContext.MenuTypes.Where(row => row.IsActive).OrderBy(row => row.DisplayOrder).Select(row => new MenuViewModel
        //        {
        //            MenuTypeName = row.MenuTypeName,
        //            MenuTypeIconClassName = row.IconCLassName,
        //            MenuDetails = dbContext.UserPermissions.Where(M => M.IsActive && M.Menu.IsActive == true && M.Menu.MenuTypeKey == row.RowKey && M.Employee.AppUserKey == DbConstants.User.UserKey && M.ActionKey == 1 && !NonMenuCodes.Contains(M.Menu.MenuCode)).OrderBy(M => M.Menu.DisplayOrder).Select(M => new MenuDetailViewModel
        //            {
        //                MenuName = M.Menu.MenuName,
        //                MenuCode = M.Menu.MenuCode,
        //                ActionName = M.Menu.ActionName,
        //                ControllerName = M.Menu.ControllerName,
        //                OptionalParameter = M.Menu.OptionalParameter,
        //                IconClassName = M.Menu.IconClassName

        //            }).ToList()

        //        }).ToList();
        //    }
        //    else
        //    {
        //        menuList = dbContext.MenuTypes.Where(row => row.IsActive).OrderBy(row => row.DisplayOrder).Select(row => new MenuViewModel
        //        {
        //            MenuTypeName = row.MenuTypeName,
        //            MenuTypeIconClassName = row.IconCLassName,
        //            MenuDetails = dbContext.MenuActions.Where(M => M.IsActive && M.Menu.IsActive == true && M.Menu.MenuTypeKey == row.RowKey && M.ActionKey == 1 && !NonMenuCodes.Contains(M.Menu.MenuCode)).OrderBy(M => M.Menu.DisplayOrder).Select(M => new MenuDetailViewModel
        //            {
        //                MenuName = M.Menu.MenuName,
        //                MenuCode = M.Menu.MenuCode,
        //                ActionName = M.Menu.ActionName,
        //                ControllerName = M.Menu.ControllerName,
        //                OptionalParameter = M.Menu.OptionalParameter,
        //                IconClassName = M.Menu.IconClassName

        //            }).ToList()
        //        }).ToList();

        //    }
        //    return menuList;
        //}
        public BranchViewModel GetBranchDetailById(short? id)
        {
            return dbContext.Branches.Where(x => x.RowKey == id).Select(row => new BranchViewModel
            {
                BranchCode = row.BranchCode,
                BranchName = row.BranchName,
                BranchNameLocal = row.BranchNameLocal,
                CityName = row.CityName,
                PhoneNumber1 = row.PhoneNumber1,
                PhoneNumber2 = row.PhoneNumber2,
                ContactPersonPhone = row.ContactPersonPhone,
                PostalCode = row.PostalCode,
                AddressLine1 = row.AddressLine1,
                AddressLine2 = row.AddressLine2,
                AddressLine3 = row.AddressLine3,
                EmailAddress = row.EmailAddress,
                FaxNumber = row.FaxNumber,
                TelephoneCode = row.District.Province.Country.TelephoneCode,
                BranchLogo = row.IsFranchise == true ? row.BranchLogo : row.Company.CompanyLogo,
                BranchLogoPath = row.IsFranchise == true ? UrlConstants.BranchLogo + row.BranchLogo : UrlConstants.CompanyLogo + row.Company.CompanyLogo,
            }).FirstOrDefault();
        }
        public bool ShowUniversity()
        {
            return dbContext.VwUniversityMasterSelectActiveOnlies.Count() != 1;
        }
        public bool ShowAcademicTerm()
        {
            return dbContext.VwAcadamicTermSelectActiveOnlies.Count() != 1;
        }
        public bool CheckMenuActive(string MenuCode)
        {
            return dbContext.Menus.Where(row => row.MenuCode == MenuCode && row.IsActive).Any();
        }
        public string GetBakup(string DBName, string Location)
        {

            if (DBName == null || DBName == "")
            {
                string databaseName = dbContext.Database.Connection.Database;
                DBName = databaseName;
            }

            IEnumerable<string> getBackup = dbContext.Database.SqlQuery<string>("sp_Backup_Database @DBName,@Location",
                new SqlParameter("DBName", DBName),
                new SqlParameter("Location", Location));

            return string.Join("", getBackup);
        }
        public ApplicationPersonalViewModel FillApplicationDetails(string AdmissionNo, long? ApplicationKey)
        {
            ApplicationPersonalViewModel objViewModel = new ApplicationPersonalViewModel();
            if (!String.IsNullOrEmpty(AdmissionNo))
            {
                ApplicationKey = dbContext.Applications.Where(x => x.RollNoCode == AdmissionNo).Select(x => x.RowKey).FirstOrDefault();
            }

            objViewModel = dbContext.Applications.Where(x => x.RowKey == ApplicationKey).Select(row => new ApplicationPersonalViewModel
            {
                ApplicantName = row.StudentName,
                AdmissionNo = row.AdmissionNo,
                ApplicationKey = row.RowKey,
                ClassDetailsName = row.ClassDetail.ClassCode,
                ClassDetailsKey = row.ClassDetailsKey ?? 0,
                ApplicantPhoto = (row.OldStudentPhotoPath != null && row.OldStudentPhotoPath != "") ? row.OldStudentPhotoPath : UrlConstants.ApplicationUrl + row.RowKey + "/" + row.StudentPhotoPath,
                StudentPhotoPath = row.StudentPhotoPath,
                CourseName = row.Course.CourseName,
                UniversityName = row.UniversityMaster.UniversityMasterName,
                MobileNumber = row.StudentMobile,
                RollNoCode = row.RollNoCode,
                BatchKey = row.BatchKey,
                CourseDuration = row.Course.CourseDuration ?? 0,
                CurrentYear = row.CurrentYear,
                AcademicTermKey = row.AcademicTermKey,

                //CurrentYearText = CommonUtilities.GetYearDescriptionByCodeDetails(row.Course.CourseDuration ?? 0, row.CurrentYear, row.AcademicTermKey)

            }).FirstOrDefault();


            if (objViewModel == null)
            {
                objViewModel = new ApplicationPersonalViewModel();
                objViewModel.Message = EduSuiteUIResources.User_CannotFind;
                objViewModel.IsSuccessful = false;
            }
            else
            {
                objViewModel.CurrentYearText = CommonUtilities.GetYearDescriptionByCodeDetails(objViewModel.CourseDuration, objViewModel.CurrentYear ?? 0, objViewModel.AcademicTermKey ?? 0);
                objViewModel.Message = EduSuiteUIResources.Success;
                objViewModel.IsSuccessful = true;

            }

            return objViewModel;
        }
        public long GetApplicationKeyByCode(string RollNoCode)
        {
            return dbContext.Applications.Where(x => x.RollNoCode == RollNoCode || x.AdmissionNo == RollNoCode).Select(row => row.RowKey).FirstOrDefault();
        }
        public EmployeePersonalViewModel FillEmployeeDetails(string EmployeeCode, long? EmployeeKey)
        {
            EmployeePersonalViewModel objViewModel = new EmployeePersonalViewModel();
            //if (DbConstants.User.IsTeacher)
            //{
            //    EmployeeKey = DbConstants.User.EmployeeKey ?? 0;
            //}
            if (!String.IsNullOrEmpty(EmployeeCode))
            {
                EmployeeKey = dbContext.Employees.Where(x => x.EmployeeCode == EmployeeCode).Select(x => x.RowKey).FirstOrDefault();
            }

            objViewModel = dbContext.Employees.Where(x => x.RowKey == EmployeeKey).Select(row => new EmployeePersonalViewModel
            {
                EmployeeKey = row.RowKey,
                EmployeeCode = row.EmployeeCode,
                FirstName = row.FirstName,
                DesignationName = row.Designation.DesignationName,
                MobileNumber = row.MobileNumber,
                EmployeePhoto = UrlConstants.EmployeeUrl + row.RowKey + "/" + row.Photo,
                EmployeePhotoPath = row.Photo,
            }).FirstOrDefault();
            if (objViewModel == null)
            {
                objViewModel = new EmployeePersonalViewModel();
                objViewModel.Message = EduSuiteUIResources.User_CannotFind;
                objViewModel.IsSuccessful = false;
            }
            else
            {
                objViewModel.Message = EduSuiteUIResources.Success;
                objViewModel.IsSuccessful = true;

            }

            return objViewModel;
        }
        public decimal CheckShortBalance(short PaymentModeKey, long BankAccountKey, short branchKey)
        {
            decimal Balance = 0;
            if (PaymentModeKey == DbConstants.PaymentMode.Cash)
            {
                long AccountHeadKey = dbContext.AccountHeads.Where(x => x.RowKey == DbConstants.AccountHead.CashAccount).Select(x => x.RowKey).FirstOrDefault();
                decimal totalDebit = dbContext.AccountFlows.Where(x => x.AccountHeadKey == AccountHeadKey && x.CashFlowTypeKey == DbConstants.CashFlowType.In && x.BranchKey == branchKey).Select(x => x.Amount).DefaultIfEmpty().Sum();
                decimal totalCredit = dbContext.AccountFlows.Where(x => x.AccountHeadKey == AccountHeadKey && x.CashFlowTypeKey == DbConstants.CashFlowType.Out && x.BranchKey == branchKey).Select(x => x.Amount).DefaultIfEmpty().Sum();
                Balance = totalDebit - totalCredit;

            }
            else if (PaymentModeKey == DbConstants.PaymentMode.Bank || PaymentModeKey == DbConstants.PaymentMode.Cheque)
            {

                if (BankAccountKey != 0 && BankAccountKey != null)
                {
                    long AccountHeadKey = dbContext.BankAccounts.Where(x => x.RowKey == BankAccountKey).Select(x => x.AccountHeadKey).FirstOrDefault();
                    decimal totalDebit = dbContext.AccountFlows.Where(x => x.AccountHeadKey == AccountHeadKey && x.CashFlowTypeKey == DbConstants.CashFlowType.In && x.BranchKey == branchKey).Select(x => x.Amount).DefaultIfEmpty().Sum();
                    decimal totalCredit = dbContext.AccountFlows.Where(x => x.AccountHeadKey == AccountHeadKey && x.CashFlowTypeKey == DbConstants.CashFlowType.Out && x.BranchKey == branchKey).Select(x => x.Amount).DefaultIfEmpty().Sum();
                    Balance = totalDebit - totalCredit;

                }
            }
            return Balance;
        }
        public MemberPlanDetailsViewModel FillLibraryDetails(string CardId, long? MemberPlanKey)
        {
            MemberPlanDetailsViewModel objViewModel = new MemberPlanDetailsViewModel();
            if (!String.IsNullOrEmpty(CardId))
            {
                MemberPlanKey = dbContext.MemberPlanDetails.Where(x => x.CardId == CardId).Select(x => x.RowKey).FirstOrDefault();
            }

            objViewModel = (from a in dbContext.MemberPlanDetails
                            join AP in dbContext.Applications on new { a.ApplicationTypeKey, a.ApplicationKey } equals new { ApplicationTypeKey = DbConstants.ApplicationType.Student, ApplicationKey = AP.RowKey } into APJ
                            from AP in APJ.DefaultIfEmpty()
                            join EP in dbContext.Employees on new { a.ApplicationTypeKey, a.ApplicationKey } equals new { ApplicationTypeKey = DbConstants.ApplicationType.Staff, ApplicationKey = EP.RowKey } into EPJ
                            from EP in EPJ.DefaultIfEmpty()
                            join MR in dbContext.MemberRegistrations on new { a.ApplicationTypeKey, a.ApplicationKey } equals new { ApplicationTypeKey = DbConstants.ApplicationType.Other, ApplicationKey = MR.RowKey } into MRJ
                            from MR in MRJ.DefaultIfEmpty()
                            select new MemberPlanDetailsViewModel
                            {
                                RowKey = a.RowKey,
                                MemberTypeKey = a.MemberTypeKey,
                                BorrowerTypeKey = a.BorrowerTypeKey,
                                CardId = a.CardId,
                                CardSerialNo = a.CardSerialNo,
                                IsBlockMember = a.IsBlockMember,
                                ApplicationTypeKey = a.ApplicationTypeKey,
                                MemberFullName = a.ApplicationTypeKey == DbConstants.ApplicationType.Student ? AP.StudentName : a.ApplicationTypeKey == DbConstants.ApplicationType.Staff ? (EP.FirstName + " " + EP.LastName) : (MR.MemberFirstName + " " + MR.MemberLastName),
                                ApplicationKey = a.ApplicationKey,
                                ApplicationTypeName = a.ApplicationType.ApplicationTypeName,
                                MobilleNo = a.ApplicationTypeKey == DbConstants.ApplicationType.Student ? AP.StudentMobile : a.ApplicationTypeKey == DbConstants.ApplicationType.Staff ? (EP.MobileNumber) : (MR.PhoneNo),
                                MemberPhoto = a.ApplicationTypeKey == DbConstants.ApplicationType.Student && AP.StudentPhotoPath != null ? UrlConstants.ApplicationUrl + AP.AdmissionNo + "/" + AP.StudentPhotoPath : a.ApplicationTypeKey == DbConstants.ApplicationType.Staff && EP.Photo != null ? (UrlConstants.EmployeeUrl + EP.EmployeeCode + "/" + EP.Photo) : MR.MemberPhoto != null ? (UrlConstants.LibraryMemberUrl + a.CardId + "/" + MR.MemberPhoto) : "",
                            }).Where(x => x.RowKey == MemberPlanKey).FirstOrDefault();


            if (objViewModel == null)
            {
                objViewModel = new MemberPlanDetailsViewModel();
                objViewModel.Message = EduSuiteUIResources.CardId_CannotFind;
                objViewModel.IsSuccessful = false;
            }
            else
            {
                objViewModel.Message = EduSuiteUIResources.Success;
                objViewModel.IsSuccessful = true;

            }

            return objViewModel;
        }
        public string GetMemberPlanKeyByCardId(string CardId)
        {
            return dbContext.MemberPlanDetails.Where(x => x.CardId == CardId).Select(row => row.CardId).FirstOrDefault();
        }
    }
}
