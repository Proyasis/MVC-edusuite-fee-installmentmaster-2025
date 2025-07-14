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
    public class EmployeePersonalService : IEmployeePersonalService
    {
        private EduSuiteDatabase dbContext;

        public EmployeePersonalService(EduSuiteDatabase objDB)
        {
            this.dbContext = objDB;
        }
        public EmployeePersonalViewModel GetEmployeePersonalById(Int64? id)
        {
            try
            {
                EmployeePersonalViewModel model = new EmployeePersonalViewModel();
                model = dbContext.Employees.Where(x => x.RowKey == id).Select(row => new EmployeePersonalViewModel
                {
                    RowKey = row.RowKey,
                    SalutationKey = row.SalutationKey,
                    EmployeeCode = row.EmployeeCode,
                    FirstName = row.FirstName,
                    MiddleName = row.MiddleName,
                    LastName = row.LastName,
                    DateOfBirth = row.DateOfBirth,
                    Gender = row.Gender,
                    TelephoneCodeKey = row.TelephoneCodeKey,
                    PhoneNumber = row.PhoneNumber,
                    MobileNumber = row.MobileNumber,
                    EmailAddress = row.EmailAddress,
                    NotificationByEmail = row.NotificationByEmail,
                    NotificationBySMS = row.NotificationBySMS,
                    JoiningDate = row.JoiningDate,
                    ReleiveDate = row.ReleiveDate,
                    EmployeePhoto = row.Photo,
                    EmergencyContactPerson = row.EmergencyContactPerson,
                    ContactPersonRelationship = row.ContactPersonRelationship,
                    ContactPersonNumber = row.ContactPersonNumber,
                    ReligionKey = row.ReligionKey,
                    BloodGroupKey = row.BloodGroupKey,
                    MaritalStatusKey = row.MaritalStatusKey,
                    NationalityKey = row.NationalityKey,
                    EmployeeStatusKey = row.EmployeeStatusKey,
                    EmployeeCategoryKey = row.EmployeeCategoryKey,
                    DesignationKey = row.DesignationKey,
                    GradeKey = row.GradeKey,
                    BranchKey = row.BranchKey,
                    DepartmentKey = row.DepartmentKey,
                    HigherEmployeeUserKey = row.HigherEmployeeUserKey,
                    IsTeacher = row.IsTeacher,
                    //CountryAccessKeys = row.CountryAccess,
                    SalaryTypeKey = row.SalaryTypeKey,
                    MonthlySalary = row.SalaryAmount,
                    EmployeeWorkTypeKey = row.WorkTypeKey ?? 0,
                    AttendanceConfigTypeKey = row.AttendanceConfigTypeKey,
                    AttendanceCategoryKey = row.AttendanceCategoryKey,
                    ShiftKey = row.ShiftKey ?? 0
                }).FirstOrDefault();
                if (model == null)
                {
                    model = new EmployeePersonalViewModel();
                }
                if (EmployeeSettingModel.ClassAllocationRequired == true && model.IsTeacher == false)
                {
                    model.IsTeacher = true;
                }
                //model.CountryAccess = model.CountryAccessKeys != null ? model.CountryAccessKeys.Split(',').Select(Int16.Parse).ToList() : model.CountryAccess;
                FillDropdownLists(model);
                return model;
            }
            catch (Exception ex)
            {
                ActivityLog.CreateActivityLog(MenuConstants.EmployeePersonal, ActionConstants.View, DbConstants.LogType.Error, id, ex.GetBaseException().Message);
                return new EmployeePersonalViewModel();

            }
        }
        public EmployeePersonalViewModel CreateEmployeePersonal(EmployeePersonalViewModel model)
        {
            Employee employeeModel = new Employee();

            using (var transaction = dbContext.Database.BeginTransaction())
            {
                try
                {
                    AccountHeadService accountHeadService = new AccountHeadService(dbContext);
                    AccountHeadViewModel accountHeadViewModel = new AccountHeadViewModel();
                    accountHeadViewModel.AccountHeadName = model.FirstName + EduSuiteUIResources.OpenBracketWithSpace + model.EmployeeCode + EduSuiteUIResources.ClosingBracketWithSpace;
                    accountHeadViewModel.AccountHeadTypeKey = DbConstants.AccountHeadType.SundryCreditors;
                    accountHeadViewModel.IsActive = true;
                    accountHeadViewModel.IsSystemAccount = true;
                    accountHeadViewModel.HideDaily = true;
                    accountHeadViewModel.HideFuture = true;
                    accountHeadViewModel = accountHeadService.createAccountChart(accountHeadViewModel);

                    Int64 maxKey = dbContext.Employees.Select(p => p.RowKey).DefaultIfEmpty().Max();

                    employeeModel.RowKey = Convert.ToInt64(maxKey + 1);
                    employeeModel.FirstName = model.FirstName;
                    employeeModel.LastName = model.LastName;
                    employeeModel.MiddleName = model.MiddleName;
                    employeeModel.EmployeeCode = model.EmployeeCode;
                    employeeModel.EmployeeCategoryKey = model.EmployeeCategoryKey;
                    employeeModel.BloodGroupKey = model.BloodGroupKey;
                    employeeModel.NationalityKey = model.NationalityKey;
                    employeeModel.ReligionKey = model.ReligionKey;
                    employeeModel.BranchKey = model.BranchKey;
                    employeeModel.SalutationKey = model.SalutationKey;
                    employeeModel.DateOfBirth = Convert.ToDateTime(model.DateOfBirth);
                    employeeModel.Gender = model.Gender;
                    employeeModel.TelephoneCodeKey = model.TelephoneCodeKey;
                    employeeModel.PhoneNumber = model.PhoneNumber;
                    employeeModel.MobileNumber = model.MobileNumber;
                    employeeModel.EmailAddress = model.EmailAddress;
                    employeeModel.NotificationByEmail = model.NotificationByEmail;
                    employeeModel.NotificationBySMS = model.NotificationBySMS;
                    employeeModel.MaritalStatusKey = model.MaritalStatusKey;
                    employeeModel.JoiningDate = model.JoiningDate;
                    employeeModel.ReleiveDate = model.ReleiveDate;
                    employeeModel.GradeKey = model.GradeKey;
                    employeeModel.DepartmentKey = model.DepartmentKey;
                    employeeModel.DesignationKey = model.DesignationKey;
                    employeeModel.EmployeeStatusKey = DbConstants.EmployeeStatus.Working;
                    employeeModel.BiomatricID = model.BiomatricID;
                    employeeModel.Photo = model.EmployeePhoto;
                    employeeModel.EmergencyContactPerson = model.EmergencyContactPerson;
                    employeeModel.ContactPersonRelationship = model.ContactPersonRelationship;
                    employeeModel.ContactPersonNumber = model.ContactPersonNumber;
                    employeeModel.HigherEmployeeUserKey = model.HigherEmployeeUserKey;
                    employeeModel.BranchAccess = model.BranchKey.ToString();
                    employeeModel.SalaryTypeKey = model.SalaryTypeKey;
                    employeeModel.WorkTypeKey = model.EmployeeWorkTypeKey;
                    employeeModel.AttendanceCategoryKey = model.AttendanceCategoryKey;
                    employeeModel.AttendanceConfigTypeKey = model.AttendanceConfigTypeKey;
                    employeeModel.ShiftKey = model.ShiftKey;
                    employeeModel.SalaryAmount = model.MonthlySalary;
                    employeeModel.IsActive = true;
                    employeeModel.IsTeacher = model.IsTeacher;
                    employeeModel.AccountHeadKey = accountHeadViewModel.RowKey;

                    dbContext.Employees.Add(employeeModel);

                    model.RowKey = employeeModel.RowKey;
                    CreateUserPermission(model);

                    dbContext.SaveChanges();
                    transaction.Commit();

                    model.Message = EduSuiteUIResources.Success;
                    model.IsSuccessful = true;
                    ActivityLog.CreateActivityLog(MenuConstants.EmployeePersonal, ActionConstants.Add, DbConstants.LogType.Info, model.RowKey, model.Message);
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    model.Message = String.Format(EduSuiteUIResources.FailedToSaveErrorMessage, EduSuiteUIResources.Employee);
                    model.IsSuccessful = false;
                    ActivityLog.CreateActivityLog(MenuConstants.EmployeePersonal, ActionConstants.Add, DbConstants.LogType.Error, model.RowKey, ex.GetBaseException().Message);
                }
            }
            return model;
        }
        public EmployeePersonalViewModel UpdateEmployeePersonal(EmployeePersonalViewModel model)
        {

            Employee employeeModel = new Employee();

            using (var transaction = dbContext.Database.BeginTransaction())
            {
                try
                {
                    employeeModel = dbContext.Employees.SingleOrDefault(row => row.RowKey == model.RowKey);

                    ContactVerification contactVerification = dbContext.ContactVerifications.Where(row => row.EmployeeKey == model.RowKey).FirstOrDefault();

                    if (employeeModel.MobileNumber != model.MobileNumber)
                        if (contactVerification != null)
                            contactVerification.IsMobileVerified = false;
                    if (employeeModel.EmailAddress != model.EmailAddress)
                        if (contactVerification != null)
                            contactVerification.IsEmailVerfied = false;

                    if (employeeModel.AccountHeadKey == null || employeeModel.AccountHeadKey == 0)
                    {
                        AccountHeadService accountHeadService = new AccountHeadService(dbContext);
                        AccountHeadViewModel accountHeadViewModel = new AccountHeadViewModel();
                        accountHeadViewModel.AccountHeadName = model.FirstName + EduSuiteUIResources.OpenBracketWithSpace + model.EmployeeCode + EduSuiteUIResources.ClosingBracketWithSpace;
                        accountHeadViewModel.AccountHeadTypeKey = DbConstants.AccountHeadType.SundryCreditors;
                        accountHeadViewModel.IsActive = true;
                        accountHeadViewModel.IsSystemAccount = true;
                        accountHeadViewModel.HideDaily = true;
                        accountHeadViewModel.HideFuture = true;
                        accountHeadViewModel = accountHeadService.createAccountChart(accountHeadViewModel);
                        employeeModel.AccountHeadKey = accountHeadViewModel.RowKey;
                    }
                    else
                    {
                        AccountHeadService accountHeadService = new AccountHeadService(dbContext);
                        AccountHeadViewModel accountHeadViewModel = new AccountHeadViewModel();
                        accountHeadViewModel.AccountHeadName = model.FirstName + EduSuiteUIResources.OpenBracketWithSpace + model.EmployeeCode + EduSuiteUIResources.ClosingBracketWithSpace;
                        accountHeadViewModel.AccountHeadTypeKey = DbConstants.AccountHeadType.SundryCreditors;
                        accountHeadViewModel.IsActive = true;
                        accountHeadViewModel.IsSystemAccount = true;
                        accountHeadViewModel.HideDaily = true;
                        accountHeadViewModel.HideFuture = true;
                        accountHeadViewModel.RowKey = dbContext.AccountHeads.Where(x => x.RowKey == employeeModel.AccountHeadKey).Select(x => x.RowKey).FirstOrDefault();
                        accountHeadViewModel = accountHeadService.updateAccountChart(accountHeadViewModel);
                    }

                    employeeModel.FirstName = model.FirstName;
                    employeeModel.LastName = model.LastName;
                    employeeModel.MiddleName = model.MiddleName;
                    employeeModel.EmployeeCode = model.EmployeeCode;
                    employeeModel.EmployeeCategoryKey = model.EmployeeCategoryKey;
                    employeeModel.BloodGroupKey = model.BloodGroupKey;
                    employeeModel.NationalityKey = model.NationalityKey;
                    employeeModel.ReligionKey = model.ReligionKey;
                    employeeModel.BranchKey = model.BranchKey;
                    employeeModel.SalutationKey = model.SalutationKey;
                    employeeModel.DateOfBirth = Convert.ToDateTime(model.DateOfBirth);
                    employeeModel.Gender = model.Gender;
                    employeeModel.TelephoneCodeKey = model.TelephoneCodeKey;
                    employeeModel.PhoneNumber = model.PhoneNumber;
                    employeeModel.MobileNumber = model.MobileNumber;
                    employeeModel.EmailAddress = model.EmailAddress;
                    employeeModel.NotificationByEmail = model.NotificationByEmail;
                    employeeModel.NotificationBySMS = model.NotificationBySMS;
                    employeeModel.MaritalStatusKey = model.MaritalStatusKey;
                    employeeModel.JoiningDate = model.JoiningDate;
                    employeeModel.ReleiveDate = model.ReleiveDate;
                    employeeModel.GradeKey = model.GradeKey;
                    employeeModel.DepartmentKey = model.DepartmentKey;
                    employeeModel.DesignationKey = model.DesignationKey;
                    employeeModel.BiomatricID = model.BiomatricID;
                    employeeModel.Photo = model.EmployeePhoto;
                    employeeModel.EmergencyContactPerson = model.EmergencyContactPerson;
                    employeeModel.ContactPersonRelationship = model.ContactPersonRelationship;
                    employeeModel.ContactPersonNumber = model.ContactPersonNumber;
                    employeeModel.HigherEmployeeUserKey = model.HigherEmployeeUserKey;
                    employeeModel.BranchAccess = model.BranchKey.ToString();
                    //employeeModel.CountryAccess = String.Join(",", model.CountryAccess);
                    //employeeModel.AccountHeadCode = accountHeadViewModel.AccountHeadCode;
                    employeeModel.IsTeacher = model.IsTeacher;
                    employeeModel.SalaryTypeKey = model.SalaryTypeKey;
                    employeeModel.WorkTypeKey = model.EmployeeWorkTypeKey;
                    employeeModel.AttendanceCategoryKey = model.AttendanceCategoryKey;
                    employeeModel.AttendanceConfigTypeKey = model.AttendanceConfigTypeKey;
                    employeeModel.ShiftKey = model.ShiftKey;
                    employeeModel.EmployeeStatusKey = model.EmployeeStatusKey;
                    employeeModel.SalaryAmount = model.MonthlySalary;

                    dbContext.SaveChanges();
                    transaction.Commit();
                    model.Message = EduSuiteUIResources.Success;
                    model.IsSuccessful = true;
                    ActivityLog.CreateActivityLog(MenuConstants.EmployeePersonal, ActionConstants.Edit, DbConstants.LogType.Info, model.RowKey, model.Message);
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    model.Message = String.Format(EduSuiteUIResources.FailedToSaveErrorMessage, EduSuiteUIResources.Employee);
                    model.IsSuccessful = false;
                    ActivityLog.CreateActivityLog(MenuConstants.EmployeeSalary, ActionConstants.Edit, DbConstants.LogType.Error, model.RowKey, ex.GetBaseException().Message);
                }
            }
            return model;
        }
        public EmployeePersonalViewModel GetGradeByDesignationId(Int16 DesignationKey)
        {
            EmployeePersonalViewModel model = new EmployeePersonalViewModel();
            model.DesignationKey = DesignationKey;
            FillGrade(model);
            return model;

        }
        public EmployeePersonalViewModel GetGradeDetailsById(Int32 DesignationKey)
        {
            EmployeePersonalViewModel model = new EmployeePersonalViewModel();
            model.MonthlySalary = dbContext.DesignationGrades.Where(row => row.RowKey == DesignationKey).Select(row => row.MonthlySalary).SingleOrDefault();
            return model;

        }
        public EmployeePersonalViewModel GetHigherEmployeesById(EmployeePersonalViewModel model)
        {
            //Designation designation = dbContext.Designations.SingleOrDefault(row => row.RowKey == model.DesignationKey);
            //if (designation != null)
            //{
            //model.HigherEmployees = (from au in dbContext.AppUsers
            //                         join e in dbContext.Employees.Where(row => row.RowKey != model.RowKey) on au.RowKey equals e.AppUserKey into ej
            //                         from e in ej.DefaultIfEmpty()
            //                         where au.IsActive && au.DesignationKey == designation.HigherDesignationKey && ((e.BranchKey == model.BranchKey) || e.BranchKey == null)
            //                         select new SelectListModel
            //                         {
            //                             RowKey = au.RowKey,
            //                             Text = au.FirstName + " " + (au.MiddleName ?? "") + " " + au.LastName
            //                         }).ToList();
            //if(model.HigherEmployees.Count==0)
            //{
            //    model.HigherEmployees = (from au in dbContext.AppUsers
            //                             join e in dbContext.Employees.Where(row => row.RowKey != model.RowKey) on au.RowKey equals e.AppUserKey into ej
            //                             from e in ej.DefaultIfEmpty()
            //                             where au.IsActive && au.DesignationKey == designation.HigherDesignationKey 
            //                             select new SelectListModel
            //                             {
            //                                 RowKey = au.RowKey,
            //                                 Text = au.FirstName + " " + (au.MiddleName ?? "") + " " + au.LastName
            //                             }).ToList();
            //}

            List<EmployeePersonalViewModel> HigherEmployees = (from au in dbContext.fnParentEmployeesByDesignation(model.DesignationKey)
                                                               join E in dbContext.Employees on au.RowKey equals E.AppUserKey into EJ
                                                               from E in EJ.DefaultIfEmpty()

                                                               select new EmployeePersonalViewModel
                                                               {
                                                                   RowKey = au.RowKey,
                                                                   FullName = au.EmployeeName,
                                                                   BranchName = E.BranchAccess,
                                                                   BranchKey = E.BranchKey != null ? E.BranchKey : (Int16)0
                                                               }).ToList();

            model.HigherEmployees = HigherEmployees.Where(row => (row.BranchName != null && row.BranchName.Split(',').Select(Int16.Parse).Contains(model.BranchKey)) || row.BranchKey == 0).Select(row => new SelectListModel
            {
                RowKey = row.RowKey,
                Text = row.FullName
            }).ToList();
            if (model.HigherEmployees.Count == 0)
            {
                model.HigherEmployees = (from au in dbContext.fnParentEmployeesByDesignation(model.DesignationKey)
                                         select new SelectListModel
                                         {
                                             RowKey = au.RowKey,
                                             Text = au.EmployeeName
                                         }).ToList();
            }

            //}


            return model;
        }
        public EmployeePersonalViewModel CheckEmployeeCodeExists(string EmployeeCode, Int64 RowKey)
        {
            EmployeePersonalViewModel model = new EmployeePersonalViewModel();
            if (dbContext.Employees.Where(row => row.EmployeeCode == EmployeeCode.Trim() && row.RowKey != RowKey).Any())
            {
                model.IsSuccessful = false;

            }
            else
            {
                model.IsSuccessful = true;
            }
            return model;
        }
        public EmployeePersonalViewModel CheckMobileNumberExists(string MobileNumber, Int64 RowKey)
        {
            EmployeePersonalViewModel model = new EmployeePersonalViewModel();
            if (dbContext.Employees.Where(row => row.MobileNumber == MobileNumber.Trim() && row.RowKey != RowKey).Any())
            {
                model.IsSuccessful = false;

            }
            else
            {
                model.IsSuccessful = true;
            }
            return model;
        }
        public EmployeePersonalViewModel CheckEmailAddressExists(string EmailAddress, Int64 RowKey)
        {
            EmployeePersonalViewModel model = new EmployeePersonalViewModel();
            if (dbContext.Employees.Where(row => row.EmailAddress == EmailAddress.Trim() && row.RowKey != RowKey).Any())
            {
                model.IsSuccessful = false;

            }
            else
            {
                model.IsSuccessful = true;
            }
            return model;
        }
        public ContactVerificationViewModel UpdateContactVerification(ContactVerificationViewModel model)
        {
            bool status = false;
            using (var transaction = dbContext.Database.BeginTransaction())
            {
                try
                {
                    ContactVerification contactVerificationModel = new ContactVerification();

                    if (dbContext.ContactVerifications.Where(row => row.EmployeeKey == model.EmployeeKey).Any())
                    {

                        contactVerificationModel = dbContext.ContactVerifications.SingleOrDefault(row => row.EmployeeKey == model.EmployeeKey);
                        if (model.MobileNumber != null && (contactVerificationModel.IsMobileVerified == false))
                        {
                            contactVerificationModel.MobileNumber = model.MobileNumber ?? contactVerificationModel.MobileNumber;
                            contactVerificationModel.IsMobileVerified = model.IsMobileVerified ?? contactVerificationModel.IsMobileVerified;
                            contactVerificationModel.SMSVerificationCode = model.SMSVerificationCode ?? contactVerificationModel.SMSVerificationCode;
                            status = true;
                        }
                        else
                        {
                            status = status ? status : false;
                        }

                        if (model.EmailAddress != null && (contactVerificationModel.IsEmailVerfied == false))
                        {
                            contactVerificationModel.EmailAddress = model.EmailAddress ?? contactVerificationModel.EmailAddress;
                            contactVerificationModel.IsEmailVerfied = model.IsEmailVerified ?? contactVerificationModel.IsEmailVerfied;
                            contactVerificationModel.EmailVerificationCode = model.EmailVerificationCode ?? contactVerificationModel.EmailVerificationCode;
                            status = true;
                        }
                        else
                        {
                            status = status ? status : false;
                        }


                    }
                    else
                    {
                        Int64 maxContactVerificationKey = dbContext.ContactVerifications.Select(p => p.RowKey).DefaultIfEmpty().Max();

                        contactVerificationModel.RowKey = Convert.ToInt64(maxContactVerificationKey + 1);
                        contactVerificationModel.EmployeeKey = model.EmployeeKey;
                        contactVerificationModel.MobileNumber = model.MobileNumber;
                        contactVerificationModel.SMSVerificationCode = model.SMSVerificationCode;
                        contactVerificationModel.EmailAddress = model.EmailAddress;
                        contactVerificationModel.EmailVerificationCode = model.EmailVerificationCode;
                        contactVerificationModel.IsMobileVerified = false;
                        contactVerificationModel.IsEmailVerfied = false;
                        dbContext.ContactVerifications.Add(contactVerificationModel);
                        status = true;

                    }



                    model.RowKey = contactVerificationModel.RowKey;
                    dbContext.SaveChanges();
                    transaction.Commit();

                    model.Message = EduSuiteUIResources.Success;
                    model.IsSuccessful = status;
                    ActivityLog.CreateActivityLog(MenuConstants.EmployeePersonal, ActionConstants.Edit, DbConstants.LogType.Info, model.EmployeeKey, model.Message);

                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    model.Message = EduSuiteUIResources.FailedToVerifyContact;
                    model.IsSuccessful = false;

                    ActivityLog.CreateActivityLog(MenuConstants.EmployeePersonal, ActionConstants.Edit, DbConstants.LogType.Error, model.EmployeeKey, ex.GetBaseException().Message);
                }
            }
            return model;
        }
        public ContactVerificationViewModel ConfirmContactVerification(ContactVerificationViewModel model)
        {
            bool status = false;
            using (var transaction = dbContext.Database.BeginTransaction())
            {
                try
                {
                    ContactVerification contactVerificationModel = new ContactVerification();
                    contactVerificationModel = dbContext.ContactVerifications.SingleOrDefault(row => row.EmployeeKey == model.EmployeeKey);
                    if (contactVerificationModel != null)
                    {
                        if (model.SMSVerificationCode != null && contactVerificationModel.IsMobileVerified == false && contactVerificationModel.SMSVerificationCode == model.SMSVerificationCode)
                        {
                            contactVerificationModel.IsMobileVerified = true;
                            status = true;
                        }
                        else
                        {
                            status = status ? status : false;
                        }
                        if (model.EmailVerificationCode != null && contactVerificationModel.IsEmailVerfied == false && contactVerificationModel.EmailVerificationCode == model.EmailVerificationCode)
                        {
                            contactVerificationModel.IsEmailVerfied = true;
                            status = true;
                            model.EmailAddress = contactVerificationModel.EmailAddress;
                        }
                        else
                        {
                            status = status ? status : false;
                        }

                    }

                    dbContext.SaveChanges();
                    transaction.Commit();

                    model.Message = EduSuiteUIResources.Success;
                    model.IsSuccessful = status;
                    ActivityLog.CreateActivityLog(MenuConstants.EmployeePersonal, ActionConstants.Edit, DbConstants.LogType.Info, model.EmployeeKey, model.Message);

                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    model.Message = EduSuiteUIResources.FailedToVerifyContact;
                    model.IsSuccessful = false;
                    ActivityLog.CreateActivityLog(MenuConstants.EmployeePersonal, ActionConstants.Edit, DbConstants.LogType.Error, model.EmployeeKey, model.Message);

                }
            }
            return model;
        }
        private void CreateUserPermission(EmployeePersonalViewModel model)
        {
            List<DesignationPermission> DesignationPermissionList = new List<DesignationPermission>();
            DesignationPermissionList = dbContext.DesignationPermissions.Where(row => row.DesignationKey == model.DesignationKey && row.IsActive).ToList();
            Int64 maxKey = dbContext.UserPermissions.Select(p => p.RowKey).DefaultIfEmpty().Max();

            DesignationPermissionList.ForEach(DesignationPermission => dbContext.UserPermissions.Add(new UserPermission
            {
                RowKey = ++maxKey,
                EmployeeKey = model.RowKey,
                MenuKey = DesignationPermission.MenuKey,
                ActionKey = DesignationPermission.ActionKey,
                IsActive = DesignationPermission.IsActive,
            }));
        }
        public void FillDropdownLists(EmployeePersonalViewModel model)
        {

            FillEmployeeStatus(model);
            FillSalutation(model);
            FillDesignation(model);
            FillGrade(model);
            FillEmployeeCategory(model);
            FillDepartment(model);
            FillBranches(model);
            FillMaritalStatus(model);
            FillNationality(model);
            FillTelephoneCodes(model);
            FillReligion(model);
            FillBloodGroup(model);
            FillCountries(model);
            GetHigherEmployeesById(model);
            FillSalaryTypes(model);
            FillEmployeeWorkType(model);
            FillAttendanceCategories(model);
            FillAttendanceConfigTypes(model);
            FillShifts(model);
        }
        private void FillSalutation(EmployeePersonalViewModel model)
        {
            model.Salutations = dbContext.VwSalutationSelectActiveOnlies.OrderBy(row => row.DisplayOrder).Select(row => new SelectListModel
            {
                RowKey = row.RowKey,
                Text = row.SalutationName
            }).ToList();
        }
        private void FillBloodGroup(EmployeePersonalViewModel model)
        {
            model.BloodGroups = dbContext.VwBloodGroupSelectActiveOnlies.OrderBy(row => row.DisplayOrder).Select(row => new SelectListModel
            {
                RowKey = row.RowKey,
                Text = row.BloodGroupName
            }).ToList();
        }
        private void FillNationality(EmployeePersonalViewModel model)
        {
            model.Nationalities = dbContext.VwCountrySelectActiveOnlies.OrderBy(row => row.DisplayOrder).Select(row => new SelectListModel
            {
                RowKey = row.RowKey,
                Text = row.NationalityName
            }).ToList();
        }
        private void FillReligion(EmployeePersonalViewModel model)
        {
            model.Religions = dbContext.VwReligionSelectActiveOnlies.OrderBy(row => row.DisplayOrder).Select(row => new SelectListModel
            {
                RowKey = row.RowKey,
                Text = row.ReligionName
            }).ToList();
        }
        private void FillMaritalStatus(EmployeePersonalViewModel model)
        {
            model.MaritalStatuses = dbContext.VwMaritalStatusSelectActiveOnlies.OrderBy(row => row.DisplayOrder).Select(row => new SelectListModel
            {
                RowKey = row.RowKey,
                Text = row.MaritalStatusName
            }).ToList();
        }
        private void FillBranches(EmployeePersonalViewModel model)
        {
            IQueryable<SelectListModel> BranchQuery = dbContext.vwBranchSelectActiveOnlies.OrderBy(row => row.DisplayOrder).Select(row => new SelectListModel
            {
                RowKey = row.RowKey,
                Text = row.BranchName
            });

            Employee Employee = dbContext.Employees.Where(x => x.AppUserKey == DbConstants.User.UserKey).SingleOrDefault();
            if (Employee != null)
            {
                if (Employee.BranchAccess != null)
                {
                    List<long> Branches = Employee.BranchAccess.Split(',').Select(Int64.Parse).ToList();
                    model.Branches = BranchQuery.Where(row => Branches.Contains(row.RowKey)).ToList();
                    //model.BranchKey = Employee.BranchKey;
                }
                else
                {
                    model.Branches = BranchQuery.Where(x => x.RowKey == Employee.BranchKey).ToList();
                    //model.BranchKey = Employee.BranchKey;
                }
            }
            else
            {
                model.Branches = BranchQuery.ToList();
            }

            if (model.Branches.Count == 1)
            {
                long? branchkey = model.Branches.Select(x => x.RowKey).FirstOrDefault();
                model.BranchKey = Convert.ToInt16(branchkey);
            }
        }
        private void FillDepartment(EmployeePersonalViewModel model)
        {
            model.Departments = dbContext.Departments.Where(x => x.IsActive).OrderBy(row => row.DisplayOrder).Select(row => new SelectListModel
            {
                RowKey = row.RowKey,
                Text = row.DepartmentName
            }).Distinct().ToList();
            if (model.AttendanceConfigTypeKey == null)
                model.AttendanceConfigTypeKey = dbContext.AttendanceConfigurations.Where(row => (row.BranchKey ?? model.BranchKey) == model.BranchKey).Select(row => row.AttendanceConfigTypeKey).SingleOrDefault();
        }
        private void FillEmployeeCategory(EmployeePersonalViewModel model)
        {
            model.EmployeeCategories = dbContext.VwEmployeeCategorySelectActiveOnlies.OrderBy(row => row.DisplayOrder).Select(row => new SelectListModel
            {
                RowKey = row.RowKey,
                Text = row.EmployeeCategoryName
            }).ToList();
        }
        private void FillDesignation(EmployeePersonalViewModel model)
        {
            model.Designations = dbContext.VwDesignationWithoutAdminSelectActiveOnlies.OrderBy(row => row.DisplayOrder).Select(row => new SelectListModel
            {
                RowKey = row.RowKey,
                Text = row.DesignationName
            }).ToList();
        }
        private void FillGrade(EmployeePersonalViewModel model)
        {
            model.Grades = dbContext.DesignationGrades.Where(row => row.DesignationKey == model.DesignationKey).OrderBy(row => row.DisplayOrder).Select(row => new SelectListModel
            {
                RowKey = row.RowKey,
                Text = row.DesignationGradeName
            }).ToList();
        }
        private void FillEmployeeStatus(EmployeePersonalViewModel model)
        {
            model.EmployeeStatuses = dbContext.VwEmployeeStatusSelectActiveOnlies.OrderBy(row => row.DisplayOrder).Select(row => new SelectListModel
            {
                RowKey = row.RowKey,
                Text = row.EmployeeStatusName
            }).ToList();
        }
        private void FillTelephoneCodes(EmployeePersonalViewModel model)
        {
            model.TelephoneCodes = dbContext.VwCountrySelectActiveOnlies.OrderBy(row => row.DisplayOrder).Select(row => new SelectListModel
            {
                RowKey = row.RowKey,
                Text = row.TelephoneCode
            }).ToList();
        }
        private void FillCountries(EmployeePersonalViewModel model)
        {

            model.Countries = dbContext.VwCountrySelectActiveOnlies.OrderBy(row => row.DisplayOrder).Select(row => new SelectListModel
            {
                RowKey = row.RowKey,
                Text = row.CountryName

            }).ToList();
        }
        private void FillSalaryTypes(EmployeePersonalViewModel model)
        {
            model.SalaryType = dbContext.SalaryTypes.Where(x => x.IsActive == true).Select(row => new SelectListModel
            {
                RowKey = row.RowKey,
                Text = row.SalaryTypeName
            }).ToList();
        }
        private void FillEmployeeWorkType(EmployeePersonalViewModel model)
        {
            model.WorkType = dbContext.EmployeeWorkTypes.Where(x => x.IsActive == true).Select(row => new SelectListModel
            {
                RowKey = row.RowKey,
                Text = row.WorkTypeName
            }).ToList();
        }
        private void FillAttendanceCategories(EmployeePersonalViewModel model)
        {
            model.AttendanceCategories = dbContext.AttendanceCategories.Where(x => x.IsActive == true).Select(row => new SelectListModel
            {
                RowKey = row.RowKey,
                Text = row.AttendanceCategoryName
            }).ToList();
        }
        private void FillAttendanceConfigTypes(EmployeePersonalViewModel model)
        {
            model.AttendanceConfigTypes = dbContext.AttendanceConfigTypes.Select(row => new SelectListModel
            {
                RowKey = row.RowKey,
                Text = row.AttendanceConfigTypeName
            }).OrderByDescending(x => x.RowKey).ToList();
        }
        private void FillShifts(EmployeePersonalViewModel model)
        {
            model.Shifts = dbContext.Shifts.Where(row => row.IsActive ?? true).Select(row => new SelectListModel
            {
                RowKey = row.RowKey,
                Text = row.ShiftName
            }).ToList();
        }
    }
}
