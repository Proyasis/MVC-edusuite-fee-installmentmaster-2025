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
    public class EmployeeService : IEmployeeService
    {

        private EduSuiteDatabase dbContext;

        public EmployeeService(EduSuiteDatabase objDB)
        {
            this.dbContext = objDB;
        }

        public List<EmployeePersonalViewModel> GetEmployees(EmployeePersonalViewModel model)
        {
            try
            {
                Employee Employee = dbContext.Employees.Where(x => x.AppUserKey == DbConstants.User.UserKey).SingleOrDefault();

                IQueryable<EmployeePersonalViewModel> employeesListQuery = (from e in dbContext.Employees
                                                                                //orderby e.RowKey
                                                                            where (e.FirstName.Contains(model.FullName)) || (e.MiddleName.Contains(model.FullName)) || (e.LastName.Contains(model.FullName))
                                                                            select new EmployeePersonalViewModel
                                                                            {
                                                                                RowKey = e.RowKey,
                                                                                SalutationName = e.Salutation.SalutationName,
                                                                                EmployeeCode = e.EmployeeCode,
                                                                                FirstName = e.FirstName,
                                                                                MiddleName = e.MiddleName,
                                                                                LastName = e.LastName,
                                                                                FullName = e.FirstName + " " + (e.MiddleName ?? "") + " " + e.LastName,
                                                                                DateOfBirth = e.DateOfBirth,
                                                                                Gender = e.Gender,
                                                                                JoiningDate = e.JoiningDate,
                                                                                ReleiveDate = e.ReleiveDate,
                                                                                EmployeePhoto = e.Photo,
                                                                                EmergencyContactPerson = e.EmergencyContactPerson,
                                                                                ContactPersonRelationship = e.ContactPersonRelationship,
                                                                                ContactPersonNumber = e.ContactPersonNumber,
                                                                                MobileNumber = e.MobileNumber,
                                                                                ReligionName = e.Religion.ReligionName,
                                                                                BloodGroupName = e.BloodGroup.BloodGroupName,
                                                                                MaritalStatusName = e.MaritalStatu.MaritalStatusName,
                                                                                NationalityName = e.Country.NationalityName,
                                                                                EmployeeStatusName = e.EmployeeStatu.EmployeeStatusName,
                                                                                EmployeeCategoryName = e.EmployeeCategory.EmployeeCategoryName,
                                                                                DesignationName = e.Designation.DesignationName,
                                                                                DepartmentName = e.Department.DepartmentName,
                                                                                BranchName = e.Branch.BranchName + (e.Branch.BranchCode != null ? EduSuiteUIResources.OpenBracket + (e.Branch.BranchCode ?? "") + EduSuiteUIResources.CloseBracket : ""),
                                                                                BranchKey = e.BranchKey,
                                                                                EmployeeStatusKey = e.EmployeeStatusKey,
                                                                                AppUserName = e.AppUser.AppUserName,
                                                                                PasswordHint = e.AppUser.PasswordHint
                                                                            });

                if (Employee != null)
                {
                    if (Employee.BranchAccess != null)
                    {
                        var Branches = Employee.BranchAccess.Split(',').Select(Int16.Parse).ToList();
                        employeesListQuery = employeesListQuery.Where(row => Branches.Contains(row.BranchKey));
                    }
                }


                if (model.BranchKey != 0)
                {
                    employeesListQuery = employeesListQuery.Where(row => row.BranchKey == model.BranchKey);
                }
                if (model.EmployeeStatusKey != 0)
                {
                    employeesListQuery = employeesListQuery.Where(row => row.EmployeeStatusKey == model.EmployeeStatusKey);
                }
                return employeesListQuery.GroupBy(x => x.RowKey).Select(y => y.FirstOrDefault()).ToList<EmployeePersonalViewModel>();
            }
            catch (Exception ex)
            {
                ActivityLog.CreateActivityLog(MenuConstants.Employee, ActionConstants.View, DbConstants.LogType.Error, model.BranchKey, ex.GetBaseException().Message);
                return new List<EmployeePersonalViewModel>();


            }
        }
        public EmployeePersonalViewModel GetSearchDropdownLists(EmployeePersonalViewModel model)
        {
            FillBranches(model);
            FillEmployeeStatuses(model);
            return model;
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
        private void FillEmployeeStatuses(EmployeePersonalViewModel model)
        {
            model.EmployeeStatuses = dbContext.VwEmployeeStatusSelectActiveOnlies.OrderBy(row => row.DisplayOrder).Select(row => new SelectListModel
            {
                RowKey = row.RowKey,
                Text = row.EmployeeStatusName
            }).ToList();
        }
        public EmployeePersonalViewModel DeleteEmployee(EmployeePersonalViewModel model)
        {
            //using (cPOSEntities dbContext = new cPOSEntities())
            //{
            using (var transaction = dbContext.Database.BeginTransaction())
            {
                try
                {
                    Employee employee = dbContext.Employees.SingleOrDefault(row => row.RowKey == model.RowKey);

                    //Delete Employee Contacts
                    List<EmployeeContact> employeeContactList = dbContext.EmployeeContacts.Where(row => row.EmployeeKey == model.RowKey).ToList();
                    List<long> AddressKeys = employeeContactList.Select(row => row.AddressKey).ToList();
                    List<Address> addressList = dbContext.Addresses.Where(row => AddressKeys.Contains(row.RowKey)).ToList();
                    List<ContactVerification> contactVerificationList = dbContext.ContactVerifications.Where(row => row.EmployeeKey == employee.RowKey).ToList();
                    //contactVerificationList.ForEach(contactVerification => dbContext.ContactVerifications.Remove(contactVerification));
                    //foreach (Address address in addressList)
                    //{
                    //    dbContext.Addresses.Remove(address);
                    //}

                    //employeeContactList.ForEach(employeeContact => dbContext.EmployeeContacts.Remove(employeeContact));

                    if (addressList.Count() > 0)
                        dbContext.Addresses.RemoveRange(addressList);
                    if (contactVerificationList.Count() > 0)
                        dbContext.ContactVerifications.RemoveRange(contactVerificationList);
                    if (contactVerificationList.Count() > 0)
                        dbContext.EmployeeContacts.RemoveRange(employeeContactList);


                    //Delete Employee Education
                    List<EmployeeEducation> employeeEducationList = dbContext.EmployeeEducations.Where(row => row.EmployeeKey == model.RowKey).ToList();
                    if (employeeEducationList.Count() > 0)
                        dbContext.EmployeeEducations.RemoveRange(employeeEducationList);


                    //Delete Employee Experience
                    List<EmployeeExperience> employeeExperienceList = dbContext.EmployeeExperiences.Where(row => row.EmployeeKey == model.RowKey).ToList();
                    ///employeeExperienceList.ForEach(employeeExperience => dbContext.EmployeeExperiences.Remove(employeeExperience));
                    if (employeeExperienceList.Count() > 0)
                        dbContext.EmployeeExperiences.RemoveRange(employeeExperienceList);

                    //Delete Employee Identity
                    List<EmployeeIdentity> employeeIdentityList = dbContext.EmployeeIdentities.Where(row => row.EmployeeKey == model.RowKey).ToList();
                    //employeeIdentityList.ForEach(employeeIdentity => dbContext.EmployeeIdentities.Remove(employeeIdentity));
                    if (employeeIdentityList.Count() > 0)
                        dbContext.EmployeeIdentities.RemoveRange(employeeIdentityList);

                    //Delete Employee Language Skills
                    List<EmployeeLanguageSkill> employeeLanguageSkillList = dbContext.EmployeeLanguageSkills.Where(row => row.EmployeeKey == model.RowKey).ToList();
                    //employeeLanguageSkillList.ForEach(employeeLanguageSkill => dbContext.EmployeeLanguageSkills.Remove(employeeLanguageSkill));
                    if (employeeLanguageSkillList.Count() > 0)
                        dbContext.EmployeeLanguageSkills.RemoveRange(employeeLanguageSkillList);

                    //Delete Employee Salary Settings
                    AdditionalSalaryComponent salarySettings = dbContext.AdditionalSalaryComponents.SingleOrDefault(row => row.EmployeeKey == model.RowKey);
                    if (salarySettings != null)
                        dbContext.AdditionalSalaryComponents.Remove(salarySettings);

                    //Delete Employee target
                    EmployeeEnquiryTarget employeeEnquiryTarget = dbContext.EmployeeEnquiryTargets.SingleOrDefault(row => row.EmployeeKey == model.RowKey);
                    if (employeeEnquiryTarget != null)
                    {
                        List<EmployeeEnquiryTargetDetail> EmployeeEnquiryTargetDetailList = dbContext.EmployeeEnquiryTargetDetails.Where(row => row.EnquiryTargetKey == employeeEnquiryTarget.RowKey).ToList();
                        if (EmployeeEnquiryTargetDetailList.Count() > 0)
                            dbContext.EmployeeEnquiryTargetDetails.RemoveRange(EmployeeEnquiryTargetDetailList);

                        dbContext.EmployeeEnquiryTargets.Remove(employeeEnquiryTarget);
                    }
                    List<UserPermission> userPermission = dbContext.UserPermissions.Where(x => x.EmployeeKey == model.RowKey).ToList();
                    if (userPermission.Count() > 0)
                    {
                        dbContext.UserPermissions.RemoveRange(userPermission);
                    }

                    //Delete Employee Account 
                    EmployeeAccount employeeAccount = dbContext.EmployeeAccounts.SingleOrDefault(row => row.EmployeeKey == model.RowKey);
                    if (employeeAccount != null)
                        dbContext.EmployeeAccounts.Remove(employeeAccount);
                    if (employee.AppUserKey != null)
                    {
                        AppUser appuser = dbContext.AppUsers.SingleOrDefault(x => x.RowKey == employee.AppUserKey);
                        dbContext.ActivityLogs.RemoveRange(dbContext.ActivityLogs.Where(row => row.AppUserKey == appuser.RowKey).ToList());
                        dbContext.AppUsers.Remove(appuser);

                    }
                    dbContext.Employees.Remove(employee);

                    dbContext.SaveChanges();
                    transaction.Commit();
                    model.Message = EduSuiteUIResources.Success;
                    model.IsSuccessful = true;

                    ActivityLog.CreateActivityLog(MenuConstants.Employee, ActionConstants.Delete, DbConstants.LogType.Info, model.RowKey, model.Message);
                }
                catch (DbUpdateException ex)
                {
                    transaction.Rollback();
                    if (ex.GetBaseException().Message.ToUpper().Contains(EduSuiteUIResources.ForeignKeyError.ToUpper()))
                    {
                        model.Message = String.Format(EduSuiteUIResources.CantDeleteErrorMessage, EduSuiteUIResources.Employee);
                        model.IsSuccessful = false;

                        ActivityLog.CreateActivityLog(MenuConstants.Employee, ActionConstants.Delete, DbConstants.LogType.Error, model.RowKey, ex.GetBaseException().Message);
                    }
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    model.Message = String.Format(EduSuiteUIResources.FailedToDeleteErrorMessage, EduSuiteUIResources.Employee);
                    model.IsSuccessful = false;

                    ActivityLog.CreateActivityLog(MenuConstants.Employee, ActionConstants.Delete, DbConstants.LogType.Error, model.RowKey, ex.GetBaseException().Message);
                }
            }
            //}
            return model;
        }
        public List<EmployeePersonalViewModel> GetOrganizationChart()
        {
            try
            {
                var employeeList = (from au in dbContext.AppUsers
                                    join e in dbContext.Employees on au.RowKey equals e.AppUserKey into ej
                                    from e in ej.DefaultIfEmpty()
                                    select new EmployeePersonalViewModel
                                    {
                                        AppUserKey = au.RowKey,
                                        FullName = au.FirstName + " " + (au.MiddleName ?? "") + " " + au.LastName,
                                        EmployeePhoto = au.Image,
                                        DesignationName = au.Designation.DesignationName,
                                        DepartmentName = e.Department.DepartmentName,
                                        BranchName = e.Branch.BranchName,
                                        HigherEmployeeUserKey = e.HigherEmployeeUserKey != null ? e.HigherEmployeeUserKey : 0
                                    }).ToList();
                return employeeList;

            }
            catch (Exception ex)
            {
                ActivityLog.CreateActivityLog(MenuConstants.Employee, ActionConstants.View, DbConstants.LogType.Error, null, ex.GetBaseException().Message);
                return new List<EmployeePersonalViewModel>();


            }
        }

        //public List<EmployeeFileHandOverViewModel> GetEmployeeFileHandOvers(EmployeeFileHandOverViewModel model)
        //{
        //    List<short> EnquiryLeadStatusKeys = new List<short>() { DbConstants.EnquiryStatus.FollowUp };
        //    List<short> EnquiryStatusKeys = new List<short>() { DbConstants.EnquiryStatus.FollowUp, DbConstants.EnquiryStatus.Intersted };
        //    IQueryable<EmployeeFileHandOverViewModel> EmployeeFileHandOverList = (from EL in dbContext.EnquiryLeads.Where(row => row.EmployeeKey == model.EmployeeFromKey && EnquiryLeadStatusKeys.Contains(row.EnquiryLeadStatusKey ?? DbConstants.EnquiryStatus.FollowUp))
        //                                                                          join FH in dbContext.EmployeeFileHandovers on new { FileKey = EL.RowKey, FileHandoverTypeKey = DbConstants.FileHandoverType.EnquiryLead } equals new { FH.FileKey, FH.FileHandoverTypeKey } into FHO
        //                                                                          from FH in FHO.DefaultIfEmpty()
        //                                                                          select new EmployeeFileHandOverViewModel
        //                                                                         {
        //                                                                             RowKey = FH.RowKey == null ? 0 : FH.RowKey,
        //                                                                             EmployeeToKey = FH.EmployeeToKey == null ? 0 : FH.EmployeeToKey,
        //                                                                             FileKey = EL.RowKey,
        //                                                                             FileHandoverTypeKey = FH.FileHandoverTypeKey == null ? DbConstants.FileHandoverType.EnquiryLead : FH.FileHandoverTypeKey,
        //                                                                             FileHandoverTypeName = FH.FileHandoverTypeKey == null ? dbContext.FileHandoverTypes.Where(row => row.RowKey == DbConstants.FileHandoverType.EnquiryLead).Select(row => row.FileHandoverTypeName).FirstOrDefault() : FH.FileHandoverType.FileHandoverTypeName,
        //                                                                             FileName = EL.Name,
        //                                                                             FileEmail = EL.EmailAddress,
        //                                                                             FileMobile = EL.PhoneNumber,
        //                                                                             FileStatusName = EL.EnquiryStatu.EnquiryStatusName,
        //                                                                             IsActive = FH.IsActive != null ? FH.IsActive : false,
        //                                                                             DateAdded = FH.DateAdded != null ? FH.DateAdded : EL.DateAdded
        //                                                                         }).Union(from E in dbContext.Enquiries.Where(row => row.EmployeeKey == model.EmployeeFromKey && EnquiryStatusKeys.Contains(row.EnquiryStatusKey ?? 0))
        //                                                                                  join FH in dbContext.EmployeeFileHandovers.Where(row => row.IsActive) on new { FileKey = E.RowKey, FileHandoverTypeKey = DbConstants.FileHandoverType.Enquiry } equals new { FH.FileKey, FH.FileHandoverTypeKey } into FHO
        //                                                                                  from FH in FHO.DefaultIfEmpty()
        //                                                                                  select new EmployeeFileHandOverViewModel
        //                                                                                 {
        //                                                                                     RowKey = FH.RowKey == null ? 0 : FH.RowKey,
        //                                                                                     EmployeeToKey = FH.EmployeeToKey == null ? 0 : FH.EmployeeToKey,
        //                                                                                     FileKey = E.RowKey,
        //                                                                                     FileHandoverTypeKey = FH.FileHandoverTypeKey == null ? DbConstants.FileHandoverType.Enquiry : FH.FileHandoverTypeKey,
        //                                                                                     FileHandoverTypeName = FH.FileHandoverTypeKey == null ? dbContext.FileHandoverTypes.Where(row => row.RowKey == DbConstants.FileHandoverType.Enquiry).Select(row => row.FileHandoverTypeName).FirstOrDefault() : FH.FileHandoverType.FileHandoverTypeName,
        //                                                                                     FileName = E.EnquiryName,
        //                                                                                     FileEmail = E.EnquiryEmail,
        //                                                                                     FileMobile = E.EnquiryPhone,
        //                                                                                     FileStatusName = E.EnquiryStatu.EnquiryStatusName,
        //                                                                                     IsActive = FH.IsActive != null ? FH.IsActive : false,
        //                                                                                     DateAdded = FH.DateAdded != null ? FH.DateAdded : E.DateAdded
        //                                                                                 });
        //    if (model.HandoverType == 1)
        //    {
        //        EmployeeFileHandOverList = EmployeeFileHandOverList.Where(row => row.RowKey == 0);
        //    }
        //    else
        //    {
        //        if (model.EmployeeToKey != 0)
        //        {
        //            EmployeeFileHandOverList = EmployeeFileHandOverList.Where(row => row.EmployeeToKey == model.EmployeeToKey);
        //        }
        //        EmployeeFileHandOverList = EmployeeFileHandOverList.Where(row => row.RowKey != 0);
        //    }
        //    if (model.FileHandoverTypeKey != 0)
        //    {
        //        EmployeeFileHandOverList = EmployeeFileHandOverList.Where(row => row.FileHandoverTypeKey == model.FileHandoverTypeKey);
        //    }

        //    return EmployeeFileHandOverList.OrderByDescending(row => row.DateAdded).ToList();
        //}

        public EmployeeFileHandOverViewModel GetFileHandoverDropdownLists(EmployeeFileHandOverViewModel model)
        {
            model.Employees = (from ET in dbContext.Employees.Where(row => row.RowKey != model.EmployeeFromKey)
                               join EF in dbContext.Employees.Where(row => row.RowKey == model.EmployeeFromKey) on ET.BranchKey equals EF.BranchKey
                               select new GroupSelectListModel
                               {
                                   RowKey = ET.RowKey,
                                   Text = ET.FirstName + " " + (ET.MiddleName ?? "") + " " + ET.LastName,
                                   GroupKey = ET.DepartmentKey,
                                   GroupName = ET.Department.DepartmentName
                               }).OrderBy(row => row.Text).ToList();
            model.FileHandoverTypes = dbContext.FileHandoverTypes.Select(row => new SelectListModel
            {
                RowKey = row.RowKey,
                Text = row.FileHandoverTypeName
            }).ToList();

            return model;
        }
        public EmployeeFileHandOverViewModel UpdateEmployeesFileHandover(List<EmployeeFileHandOverViewModel> modelList)
        {
            EmployeeFileHandOverViewModel employeeFileHandoverViewModel = new EmployeeFileHandOverViewModel();


            using (var transaction = dbContext.Database.BeginTransaction())
            {
                try
                {

                    long maxKey = dbContext.EmployeeFileHandovers.Select(p => p.RowKey).DefaultIfEmpty().Max();
                    foreach (EmployeeFileHandOverViewModel model in modelList)
                    {

                        EmployeeFileHandover EmployeeFileHandoverModel = new EmployeeFileHandover();
                        EmployeeFileHandoverModel.RowKey = Convert.ToInt64(maxKey + 1);
                        EmployeeFileHandoverModel.FileHandoverTypeKey = model.FileHandoverTypeKey ?? 0;
                        EmployeeFileHandoverModel.EmployeeFromKey = model.EmployeeFromKey;
                        EmployeeFileHandoverModel.EmployeeToKey = model.EmployeeToKey;
                        EmployeeFileHandoverModel.FileKey = model.FileKey;
                        EmployeeFileHandoverModel.IsActive = true;
                        dbContext.EmployeeFileHandovers.Add(EmployeeFileHandoverModel);
                        maxKey++;
                    }
                    dbContext.SaveChanges();
                    transaction.Commit();
                    employeeFileHandoverViewModel.Message = EduSuiteUIResources.Success;
                    employeeFileHandoverViewModel.IsSuccessful = true;

                    ActivityLog.CreateActivityLog(MenuConstants.Employee, ActionConstants.Edit, DbConstants.LogType.Info, employeeFileHandoverViewModel.RowKey, employeeFileHandoverViewModel.Message);
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    employeeFileHandoverViewModel.Message = String.Format(EduSuiteUIResources.FailedToSaveErrorMessage, EduSuiteUIResources.Employee + EduSuiteUIResources.BlankSpace + EduSuiteUIResources.FileHandOver);
                    employeeFileHandoverViewModel.IsSuccessful = false;
                    ActivityLog.CreateActivityLog(MenuConstants.Employee, ActionConstants.Edit, DbConstants.LogType.Error, employeeFileHandoverViewModel.RowKey, ex.GetBaseException().Message);
                }
            }
            return employeeFileHandoverViewModel;
        }
        public EmployeeFileHandOverViewModel DeleteHandover(List<long> Keys)
        {
            EmployeeFileHandOverViewModel model = new EmployeeFileHandOverViewModel();
            using (var transaction = dbContext.Database.BeginTransaction())
            {
                try
                {

                    List<EmployeeFileHandover> EmployeeFileHandoverList = dbContext.EmployeeFileHandovers.Where(row => Keys.Contains(row.RowKey)).ToList();

                    EmployeeFileHandoverList.ForEach(EmployeeFileHandover => dbContext.EmployeeFileHandovers.Remove(EmployeeFileHandover));


                    dbContext.SaveChanges();
                    transaction.Commit();
                    model.Message = EduSuiteUIResources.Success;
                    model.IsSuccessful = true;

                    ActivityLog.CreateActivityLog(MenuConstants.Employee, ActionConstants.Delete, DbConstants.LogType.Info, model.RowKey, model.Message);

                }
                catch (DbUpdateException ex)
                {
                    transaction.Rollback();
                    if (ex.GetBaseException().Message.ToUpper().Contains(EduSuiteUIResources.ForeignKeyError.ToUpper()))
                    {
                        model.Message = String.Format(EduSuiteUIResources.CantDeleteErrorMessage, EduSuiteUIResources.Employee + EduSuiteUIResources.BlankSpace + EduSuiteUIResources.FileHandOver);
                        model.IsSuccessful = false;
                        ActivityLog.CreateActivityLog(MenuConstants.Employee, ActionConstants.Delete, DbConstants.LogType.Error, model.RowKey, ex.GetBaseException().Message);
                    }
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    model.Message = String.Format(EduSuiteUIResources.FailedToDeleteErrorMessage, EduSuiteUIResources.Employee + EduSuiteUIResources.BlankSpace + EduSuiteUIResources.FileHandOver);
                    model.IsSuccessful = false;

                    ActivityLog.CreateActivityLog(MenuConstants.Employee, ActionConstants.Delete, DbConstants.LogType.Error, model.RowKey, ex.GetBaseException().Message);
                }
            }
            return model;

        }
        public EmployeePersonalViewModel GetEmployeePhotoById(long Id)
        {
            try
            {
                EmployeePersonalViewModel model = new EmployeePersonalViewModel();
                model = dbContext.Employees.Select(row => new EmployeePersonalViewModel
                {
                    RowKey = row.RowKey,
                    EmployeePhoto = UrlConstants.EmployeeUrl + row.RowKey + "/" + row.Photo,
                    EmployeePhotoPath = row.Photo

                }).Where(x => x.RowKey == Id).FirstOrDefault();
                if (model == null)
                {
                    model.RowKey = Id;
                }
                return model;
            }
            catch (Exception ex)
            {
                ActivityLog.CreateActivityLog(MenuConstants.Employee, ActionConstants.View, DbConstants.LogType.Debug, Id, ex.GetBaseException().Message);
                return new EmployeePersonalViewModel();

            }
        }
        public EmployeePersonalViewModel UpdateEmployeePhoto(EmployeePersonalViewModel model)
        {
            Employee employeeModel = new Employee();


            using (var transaction = dbContext.Database.BeginTransaction())
            {
                try
                {

                    employeeModel = dbContext.Employees.SingleOrDefault(row => row.RowKey == model.RowKey);
                    employeeModel.Photo = employeeModel.RowKey + model.EmployeePhoto + ".jpg";

                    dbContext.SaveChanges();
                    transaction.Commit();
                    model.EmployeePhoto = employeeModel.Photo;
                    model.EmployeeCode = employeeModel.EmployeeCode;
                    model.Message = EduSuiteUIResources.Success;
                    model.IsSuccessful = true;
                    ActivityLog.CreateActivityLog(MenuConstants.Employee, ActionConstants.AddEdit, DbConstants.LogType.Info, model.RowKey, model.Message);

                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    model.Message = String.Format(EduSuiteUIResources.FailedToSaveErrorMessage, EduSuiteUIResources.Employee + EduSuiteUIResources.BlankSpace + EduSuiteUIResources.UploadPhoto);
                    model.IsSuccessful = false;
                    ActivityLog.CreateActivityLog(MenuConstants.Employee, ActionConstants.AddEdit, DbConstants.LogType.Error, model.RowKey, ex.GetBaseException().Message);

                }
            }
            return model;
        }
        public EmployeePersonalViewModel DeleteEmployeePhoto(long Id)
        {
            EmployeePersonalViewModel model = new EmployeePersonalViewModel();
            Employee employeeModel = new Employee();


            using (var transaction = dbContext.Database.BeginTransaction())
            {
                try
                {

                    employeeModel = dbContext.Employees.SingleOrDefault(row => row.RowKey == Id);
                    model.EmployeePhoto = employeeModel.Photo;
                    model.RowKey = employeeModel.RowKey;
                    model.EmployeeCode = employeeModel.EmployeeCode;
                    employeeModel.Photo = null;
                    dbContext.SaveChanges();
                    transaction.Commit();
                    model.Message = EduSuiteUIResources.Success;
                    model.IsSuccessful = true;
                    ActivityLog.CreateActivityLog(MenuConstants.Employee, ActionConstants.Delete, DbConstants.LogType.Info, model.RowKey, model.Message);
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    model.Message = String.Format(EduSuiteUIResources.FailedToDeleteErrorMessage, EduSuiteUIResources.Employee + EduSuiteUIResources.BlankSpace + EduSuiteUIResources.UploadPhoto);
                    model.IsSuccessful = false;
                    ActivityLog.CreateActivityLog(MenuConstants.Employee, ActionConstants.Delete, DbConstants.LogType.Error, model.RowKey, ex.GetBaseException().Message);
                }
            }
            return model;
        }
        public EmployeeViewModel GetEmployeeDetailsById(long? Id)
        {
            EmployeeViewModel model = new EmployeeViewModel();
            Employee employee = dbContext.Employees.Where(x => x.RowKey == Id).FirstOrDefault();
            model.EmployeeKey = employee.RowKey;

            #region Personal Details
            model.EmployeePersonalDetails = (from e in dbContext.Employees.Where(x => x.RowKey == model.EmployeeKey)
                                             select new EmployeePersonalViewModel
                                             {
                                                 RowKey = e.RowKey,
                                                 EmployeeCode = e.EmployeeCode,
                                                 FullName = e.FirstName + " " + (e.MiddleName ?? "") + " " + e.LastName,
                                                 DateOfBirth = e.DateOfBirth,
                                                 Gender = e.Gender,
                                                 JoiningDate = e.JoiningDate,
                                                 ReleiveDate = e.ReleiveDate,
                                                 EmployeePhotoPath = UrlConstants.EmployeeUrl + e.RowKey + "/" + e.Photo,
                                                 EmployeePhoto = e.Photo,
                                                 MobileNumber = e.MobileNumber,
                                                 BloodGroupName = e.BloodGroup.BloodGroupName,
                                                 DesignationName = e.Designation.DesignationName,
                                                 BranchName = e.Branch.BranchName + (e.Branch.BranchCode != null ? EduSuiteUIResources.OpenBracket + (e.Branch.BranchCode ?? "") + EduSuiteUIResources.CloseBracket : ""),
                                                 BranchKey = e.BranchKey,
                                                 AppUserName = e.AppUser.AppUserName,
                                                 PasswordHint = e.AppUser.PasswordHint,
                                                 EmailAddress = e.EmailAddress,
                                                 //Address = e.Address
                                             }).FirstOrDefault();
            #endregion Personal Details


            #region Salary Details
            model.EmployeeSalaryDetails = (from es in dbContext.EmployeeSalaryMasters.Where(row => row.EmployeeKey == model.EmployeeKey)

                                           select new EmployeeSalaryMasterViewModel
                                           {
                                               SalaryMasterKey = es.RowKey,
                                               BranchKey = es.Employee.BranchKey,
                                               EmployeeKey = es.EmployeeKey,
                                               EmployeeName = es.Employee.FirstName + " " + (es.Employee.MiddleName ?? "") + " " + es.Employee.LastName,
                                               SalaryMonthKey = es.SalaryMonth,
                                               SalaryYearKey = es.SalaryYear,
                                               MonthlySalary = es.MonthlySalary,
                                               TotalSalary = es.TotalSalary,
                                               PaidAmount = es.EmployeeSalaryPayments.Select(row => row.PaidAmount).DefaultIfEmpty().Sum(),
                                               SalaryStatusName = es.ProcessStatu.ProcessStatusName,
                                               SalaryStatusKey = es.SalaryStatusKey,
                                               SalaryPaymentKey = es.EmployeeSalaryPayments.Select(SP => SP.RowKey).FirstOrDefault(),
                                               NoOfDaysWorked = es.NoOfDaysWorked,
                                               PaySlipFileName = es.PaySlipFileName,
                                               SalaryTypeKey = es.SalaryTypeKey,
                                               SalaryTypeName = es.SalaryType.SalaryTypeName,
                                               VoucherNumber = es.VoucherNumber,
                                               LOP = es.LOP,
                                               OvertimePerAHour = es.OverTimeAmount ?? 0,
                                               OverTimeHours = es.OverTimeHours,
                                               OverTimeMinutes = es.OverTimeMinutes,
                                               OverTimeTotalAmount = es.OverTimeTotalAmount,
                                               AdditionalDayAmount = es.AdditionalDayAmount ?? 0,
                                               AdditionalDayCount = es.AdditionalDayWorked,
                                               TotalWorkingDays = es.TotalWorkingDays,
                                               BaseWorkingDays = es.BaseWorkingDays ?? 0,
                                               AbsentDays = es.AbsentDays,
                                               WeekOffCount = es.WeekOffCount,
                                               OffdayCount = es.OffdayCount,
                                               HolidayCount = es.HolidayCount,

                                           }).ToList();
            #endregion Salary Details

            #region Salary Advance Details
            model.EmployeeSalaryAdvanceDetails = (from c in dbContext.EmployeeSalaryAdvancePayments.Where(row => row.EmployeeKey == model.EmployeeKey)
                                                  select new EmployeeSalaryAdvanceViewModel
                                                  {
                                                      PaymentKey = c.RowKey,
                                                      PaymentModeKey = c.PaymentModeKey,
                                                      PaymentModeSubKey = c.PaymentModeSubKey,
                                                      PaymentModeName = c.PaymentMode.PaymentModeName,
                                                      CardNumber = c.CardNumber,
                                                      BankAccountName = c.BankAccount.Bank.BankName,
                                                      ChequeOrDDNumber = c.ChequeOrDDNumber,
                                                      ChequeOrDDDate = c.ChequeOrDDDate,
                                                      Purpose = c.Purpose,
                                                      PaidBy = c.PaidBy,
                                                      AuthorizedBy = c.AuthorizedBy,
                                                      ReceivedBy = c.ReceivedBy,
                                                      OnBehalfOf = c.OnBehalfOf,
                                                      Remarks = c.Remarks,
                                                      EmployeeName = c.Employee.FirstName + " " + (c.Employee.MiddleName ?? "") + " " + (c.Employee.LastName ?? ""),
                                                      PaidAmount = c.PaidAmount,
                                                      IsCleared = c.IsCleared,
                                                      ClearedAmount = c.ClearedAmount ?? 0,
                                                      BalanceAmount = c.PaidAmount - (c.ClearedAmount ?? 0),
                                                      ChequeStatusKey = c.ChequeStatusKey,
                                                      EmployeeKey = c.EmployeeKey,
                                                      PaymentDate = c.PaymentDate,
                                                      ReceiptNumber = c.VoucherNumber,
                                                      BranchKey = c.PaidBranchKey != null ? c.PaidBranchKey : c.Employee.BranchKey,
                                                      Status = c.ChequeStatusKey == DbConstants.ProcessStatus.Pending ? EduSuiteUIResources.Pending : (c.ChequeStatusKey == DbConstants.ProcessStatus.Rejected ? EduSuiteUIResources.Rejected : EduSuiteUIResources.Approved)
                                                  }).ToList();

            #endregion Salary Advance Details

            #region Salary Advance Return
            model.EmployeeSalaryAdvanceReturnDetails = (from c in dbContext.EmployeeSalaryAdvanceReturnMasters.Where(row => row.EmployeeKey == model.EmployeeKey)
                                                        select new EmployeeSalaryAdvanceReturnViewModel
                                                        {
                                                            PaymentKey = c.RowKey,
                                                            PaymentModeKey = c.PaymentModeKey,
                                                            PaymentModeName = c.PaymentMode.PaymentModeName,
                                                            PaymentModeSubKey = c.PaymentModeSubKey,
                                                            PaymentModeSubName = c.PaymentModeSub.PaymentModeSubName,
                                                            CardNumber = c.CardNumber,
                                                            BankAccountName = c.BankAccount.Bank.BankName,
                                                            ChequeOrDDNumber = c.ChequeOrDDNumber,
                                                            ChequeOrDDDate = c.ChequeOrDDDate,
                                                            Purpose = c.Purpose,
                                                            PaidBy = c.PaidBy,
                                                            AuthorizedBy = c.AuthorizedBy,
                                                            ReceivedBy = c.ReceivedBy,
                                                            OnBehalfOf = c.OnBehalfOf,
                                                            Remarks = c.Remarks,
                                                            EmployeeName = c.Employee.FirstName + " " + (c.Employee.MiddleName ?? "") + " " + (c.Employee.LastName ?? ""),
                                                            PaidAmount = c.PaidAmount,
                                                            ChequeStatusKey = c.ChequeStatusKey,
                                                            BranchKey = c.PaidBranchKey != null ? c.PaidBranchKey : c.Employee.BranchKey,
                                                            EmployeeKey = c.EmployeeKey,
                                                            PaymentDate = c.PaymentDate,
                                                            ReceiptNumber = c.ReceiptNumber,
                                                            Status = c.ChequeStatusKey == DbConstants.ProcessStatus.Pending ? EduSuiteUIResources.Pending : (c.ChequeStatusKey == DbConstants.ProcessStatus.Rejected ? EduSuiteUIResources.Rejected : EduSuiteUIResources.Approved)
                                                        }).ToList();

            #endregion Salary Advance Details

            #region Salary Payments
            model.EmployeeSalaryPaymentDetails = dbContext.EmployeeSalaryPayments.Where(x => x.EmployeeSalaryMaster.EmployeeKey == model.EmployeeKey).Select(row => new PaymentWindowViewModel
            {
                PaymentKey = row.RowKey,
                MasterKey = row.EmployeeSalaryMasterKey,
                TotalSalary = row.EmployeeSalaryMaster.TotalSalary,
                PaymentModeKey = row.PaymentModeKey,
                PaymentModeName = row.PaymentMode.PaymentModeName,
                PaymentModeSubKey = row.PaymentModeSubKey,
                PaymentModeSubName = row.PaymentModeSub.PaymentModeSubName,
                PaidAmount = row.PaidAmount,
                BalanceAmount = row.BalanceAmount,
                PaymentDate = row.PaymentDate,
                BankAccountKey = row.BankAccountKey,
                BankAccountBalance = row.BankAccount.CurrentAccountBalance,
                CardNumber = row.CardNumber,
                ChequeOrDDNumber = row.ChequeOrDDNumber,
                ChequeOrDDDate = row.ChequeOrDDDate,
                Purpose = row.Purpose,
                ReceivedBy = row.ReceivedBy,
                OnBehalfOf = row.OnBehalfOf,
                PaidBy = row.PaidBy,
                AuthorizedBy = row.AuthorizedBy,
                AmountToPay = row.EmployeeSalaryMaster.TotalSalary,
                Remarks = row.Remarks,
                ReceiptNumber = row.VoucherNumber,
                BranchKey = row.PaidBranchKey != null ? row.PaidBranchKey : row.EmployeeSalaryMaster.Employee.BranchKey
            }).ToList();

            #endregion SalaryPayments

            #region Work Schedule Details
            model.WorkScheduleDetails = (from MT in dbContext.ModuleTopics
                                         join TSM in dbContext.TeacherSubjectModules on new { ModuleKey = MT.SubjectModuleKey ?? 0 } equals new { ModuleKey = TSM.ModuleKey ?? 0 }
                                         join TSA in dbContext.TeacherSubjectAllocations on MT.SubjectModule.SubjectKey equals TSA.SubjectKey
                                         join TCA in dbContext.TeacherClassAllocations on TSA.TeacherClassAllocationKey equals TCA.RowKey
                                         join TWS in dbContext.TeacherWorkScheduleMasters on new { ClassDetailsKey = TCA.ClassDetailsKey ?? 0, BatchKey = TCA.BatchKey ?? 0, TSA.SubjectKey, MT.SubjectModuleKey, TopicKey = MT.RowKey }
                                         equals new { TWS.ClassDetailsKey, TWS.BatchKey, TWS.SubjectKey, TWS.SubjectModuleKey, TopicKey = TWS.TopicKey ?? 0 } into TWSD
                                         from TWS in TWSD.DefaultIfEmpty()
                                         where (TCA.EmployeeKey == model.EmployeeKey && TSM.IsActive == true)
                                         select new WorkscheduleSubjectmodel
                                         {
                                             MasterRowKey = TWS.RowKey != null ? TWS.RowKey : 0,
                                             TopicKey = MT.RowKey,
                                             SubjectModuleKey = MT.SubjectModuleKey,
                                             ModuleName = MT.SubjectModule.ModuleName,
                                             TopicName = MT.TopicName,
                                             TotalWorkDuration = TWS.RowKey != null ? TWS.TeacherWorkScheduleDetails.Sum(x => x.Duration) : 0,
                                             ProgressStatus = TWS.RowKey != null ? TWS.TeacherWorkScheduleDetails.Sum(x => x.CurrentProgressStatus) : 0,
                                             SubjectKey = MT.SubjectModule.SubjectKey,
                                             SubjectName = MT.SubjectModule.Subject.SubjectName,
                                             ClassDetailsKey = TCA.ClassDetailsKey,
                                             ClassDetailsName = TCA.ClassDetail.ClassCode,
                                             BatchKey = TCA.BatchKey,
                                             BatchName = TCA.Batch.BatchName,
                                         }).ToList();

            #endregion Work Schedule Details

            #region Class Allocation
            model.EmployeeSubjectAllocationDetails = (from TSA in dbContext.TeacherSubjectAllocations
                                                      join TCA in dbContext.TeacherClassAllocations on TSA.TeacherClassAllocationKey equals TCA.RowKey
                                                      where (TSA.Employeekey == model.EmployeeKey && TCA.IsActive)
                                                      select new EmployeeSubjectDetailsModel
                                                      {
                                                          TeacherClassAllocationKey = TCA.RowKey != null ? TCA.RowKey : 0,
                                                          ClassDetailsKey = TCA.ClassDetailsKey,
                                                          ClassDetailsName = TCA.ClassDetail.ClassCode,
                                                          SubjectKey = TSA.SubjectKey,
                                                          SubjectName = TSA.Subject.SubjectName,
                                                          EmployeeKey = TCA.EmployeeKey,
                                                          ModuleCount = TCA.TeacherSubjectModules.Where(x => x.IsActive && x.SubjectKey == TSA.SubjectKey && x.ClassDetailsKey == TCA.ClassDetailsKey).Count(),
                                                          TotalModuleCount = dbContext.SubjectModules.Where(x => x.IsActive && x.SubjectKey == TSA.SubjectKey).Count(),
                                                          ModulesList = TCA.TeacherSubjectModules.Where(x => x.IsActive && x.SubjectKey == TSA.SubjectKey && x.ClassDetailsKey == TCA.ClassDetailsKey).Select(y => y.SubjectModule.ModuleName).ToList()
                                                      }).ToList();
            #endregion Class Allocation
            return model;
        }

    }
}
