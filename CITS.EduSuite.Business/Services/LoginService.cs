using CITS.EduSuite.Business.Common;
using CITS.EduSuite.Business.Interfaces;
using CITS.EduSuite.Business.Models.Resources;
using CITS.EduSuite.Business.Models.Security;
using CITS.EduSuite.Business.Models.ViewModels;
using CITS.EduSuite.Data;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net.NetworkInformation;
using System.Text;
using System.Threading.Tasks;

namespace CITS.EduSuite.Business.Services
{
    public class LoginService : ILoginService
    {
        private EduSuiteDatabase dbContext;
        public virtual AppUserViewModel CurrentUser { get; set; }

        public LoginService(EduSuiteDatabase objDB)
        {
            //string s = CITS.EduSuite.Business.Common.SecurityManagement.GenerateKey();
            this.dbContext = objDB;
        }

        /// <summary>
        /// Attempts the login.
        /// </summary>
        /// <param name="userName">Name of the user.</param>
        /// <param name="password">The password.</param>
        /// <returns></returns>
        public LoginModel AttemptLogin(string userName, string password, int RoleKey)
        {
            var loginModel = new LoginModel();
            loginModel.RoleKey = RoleKey;
            loginModel = ValidateUser(userName, password, loginModel);
            if (loginModel.LoginSuccess)
            {
                ActivityLog.CreateActivityLog(MenuConstants.Login, ActionConstants.Login, DbConstants.LogType.Info, (loginModel.UserPrincipalData != null ? loginModel.UserPrincipalData.UserKey.ToString() : userName), EduSuiteUIResources.Success);

            }
            else
            {
                ActivityLog.CreateActivityLog(MenuConstants.Login, ActionConstants.Login, DbConstants.LogType.Error, (loginModel.UserPrincipalData != null ? loginModel.UserPrincipalData.UserKey.ToString() : userName), loginModel.Message);
            }
            return loginModel;
        }

        /// <summary>
        /// Logs the User out of the system
        /// </summary>
        public void Logout()
        {
            ActivityLog.CreateActivityLog(MenuConstants.Login, ActionConstants.Logout, DbConstants.LogType.Info, DbConstants.User.UserKey, EduSuiteUIResources.Success);

            //this.UserIdentityProvider.logout();
        }

        /// <summary>
        /// Validates the user.
        /// </summary>
        /// <param name="userName">Name of the user.</param>
        /// <param name="password">The password.</param>
        /// <param name="loginModel">The login model.</param>
        /// <returns></returns>
        private LoginModel ValidateUser(string userName, string password, LoginModel loginModel)
        {
            var binPassword = SecurityManagement.Encrypt(password);
            var isLockedOut = false;

            AppUserViewModel user = new AppUserViewModel();

            //// Check to see if user is locked out add code here later
            //var failedLogin = _dataUnit.FailedLoginRepository.Get().OrderByDescending(row => row.DateAdded).FirstOrDefault(row => row.AppUserName.Equals(userName, StringComparison.OrdinalIgnoreCase));
            //var lockedTimeMinutes = _configProvider.GetConfigurationInt(AppConstants.ConfigSetting.PASSWORD_LOCKED_TIME);

            // Tries to find a User that has the same username and password combo as what was given
            try
            {
                // Login Blocked based on date
                System.Configuration.AppSettingsReader settingsReader = new AppSettingsReader();
                //Get your key from config file to open the lock!
                string lockedDate = (string)settingsReader.GetValue("Datezonetime", typeof(String));

                // string dateLocked = SecurityManagement.Encrypt1("10/10/2018"); // dd-MM-yyyy
                DateTime DecryptLockedDate = Convert.ToDateTime(SecurityManagement.Decrypt1(lockedDate));

                DateTime currentDate = DateTimeUTC.Now.Date;

                if (currentDate < DecryptLockedDate)
                {

                    user = dbContext.AppUsers.Where(u => u.Password.Equals(binPassword) && (u.AppUserName.Equals(userName, StringComparison.OrdinalIgnoreCase) || u.EmailAddress.Equals(userName, StringComparison.OrdinalIgnoreCase)) && u.IsActive == true).Select(row => new AppUserViewModel
                    {
                        RowKey = row.RowKey,
                        AppUserName = row.AppUserName,
                        FirstName = row.FirstName + " " + (row.MiddleName ?? "") + " " + row.LastName,
                        EmailAddress = row.EmailAddress,
                        Phone1 = row.Phone1,
                        Phone2 = row.Phone2,
                        RoleKey = row.RoleKey,
                        IsActive = row.IsActive,
                        Image = row.Image,

                    }).SingleOrDefault();

                }
                else
                {
                    loginModel = LoadUser(user, loginModel, false);
                    //loginModel.Message = ApplicationResources.OutDate;
                    loginModel.Message = EduSuiteUIResources.LoginRestriction;
                    return loginModel;
                }
            }
            catch (Exception)
            {
                //this.UpdateLoginAttempts(userName, true);
                //this.OnProviderError(e);

                loginModel = LoadUser(user, loginModel, false);

                //if (IsUserLockedOut(userName))
                //{
                //    var lockedoutTime = failedLogin.AppUserLockDate.Value.AddMinutes(lockedTimeMinutes);
                //    loginModel.Message = String.Concat(SkillBaseResourceManager.GetApplicationString(AppConstants.ErrorResourceName.LOCKED_OUT), lockedoutTime);
                //    isLockedOut = true;
                //}
                return loginModel;
            }

            loginModel.User = user;

            //this.UpdateLoginAttempts(userName, user == null);
            if (user == null)
            {
                loginModel = LoadUser(user, loginModel, false);

                //// check if user is locked out
                //if (IsUserLockedOut(userName))
                //{
                //    var lockedoutTime = failedLogin.AppUserLockDate.Value.AddMinutes(lockedTimeMinutes);

                //    loginModel.Message = String.Concat(SkillBaseResourceManager.GetApplicationString(AppConstants.ErrorResourceName.LOCKED_OUT), lockedoutTime);
                //    isLockedOut = true;
                //}

                //if (!isLockedOut)
                //    loginModel.Message = SkillBaseResourceManager.GetApplicationString(AppConstants.ErrorResourceName.INVALID_LOGIN);
                loginModel.Message = EduSuiteUIResources.InvalidUserNamePassword;
                return loginModel;
            }
            else if (!user.IsActive)
            {
                loginModel = LoadUser(user, loginModel, false);
                loginModel.Message = EduSuiteUIResources.ErrorUserNotActive;
                return loginModel;
            }



            //DateTime lockTime = DateTime.UtcNow;

            //if (failedLogin != null && failedLogin.AppUserLockDate.HasValue && failedLogin.AppUserLockDate.Value.AddMinutes(lockedTimeMinutes) > lockTime)
            //{
            //    loginModel = LoadUser(user, loginModel, false);
            //    var lockedoutTime = failedLogin.AppUserLockDate.Value.AddMinutes(lockedTimeMinutes);
            //    loginModel.Message = String.Concat(SkillBaseResourceManager.GetApplicationString(AppConstants.ErrorResourceName.LOCKED_OUT), lockedoutTime);
            //    return loginModel;
            //}

            // Check If User has Role of Audit Log Viewer
            //loginModel =  IsAuditLogViewerRole(loginModel, user);

            //// check for password change
            //NeedsPasswordChange(loginModel, user);

            //// login was successfull delete the failed login entry 
            //if (failedLogin != null)
            //{
            //    var failedLoginList = this.DataUnit.FailedLoginRepository.Get().Where(row => row.AppUserName.Equals(userName, StringComparison.OrdinalIgnoreCase)).ToList();
            //    foreach (var failed in failedLoginList)
            //        this.DataUnit.FailedLoginRepository.Delete(failed);
            //}

            loginModel = LoadUser(user, loginModel, true);

            if (loginModel.User != null)
            {
                dbContext.SaveChanges();

                // Use the UserIdentityProvider from BasePresenter to load the user
                //this.UserIdentityProvider.LoadUser(loginModel.User.EmailAddress);

                // Update the LoginModel with the User information
                if (loginModel.User.RoleKey == DbConstants.Role.Staff)
                {
                    loginModel.UserPrincipalData = dbContext.Employees.Where(row => row.AppUserKey == loginModel.User.RowKey).Select(row => new CITSEduSuitePrincipalData
                    {
                        BranchKey = row.BranchKey,
                        CompanyKey = row.Branch.CompanyKey,
                        CompanyName = row.Branch.Company.CompanyName,
                        EmployeeKey = row.RowKey,
                        IsTeacher = row.IsTeacher,
                        Photo = row.Photo != null ? UrlConstants.EmployeeUrl + row.RowKey + "/" + row.Photo : "",

                    }).FirstOrDefault();
                }
                else if (loginModel.User.RoleKey == DbConstants.Role.Students)
                {
                    loginModel.UserPrincipalData = dbContext.Applications.Where(row => row.AppUserKey == loginModel.User.RowKey).Select(row => new CITSEduSuitePrincipalData
                    {
                        BranchKey = row.BranchKey,
                        CompanyKey = row.Branch.CompanyKey,
                        CompanyName = row.Branch.Company.CompanyName,
                        ApplicationKey = row.RowKey,
                        Photo = row.StudentPhotoPath != null ? UrlConstants.ApplicationUrl + row.RowKey + "/" + row.StudentPhotoPath : "",

                    }).FirstOrDefault();
                }
                if (loginModel.UserPrincipalData == null)
                {
                    loginModel.UserPrincipalData = new CITSEduSuitePrincipalData();
                }
                loginModel.UserPrincipalData.Name = loginModel.User.FirstName;
                loginModel.UserPrincipalData.UserKey = loginModel.User.RowKey;
                loginModel.UserPrincipalData.RoleKey = loginModel.User.RoleKey;
                loginModel.UserPrincipalData.Photo = loginModel.UserPrincipalData.Photo;

                //General Configuration Table
                GeneralConfiguration generalConfiguration = dbContext.GeneralConfigurations.FirstOrDefault();
                loginModel.UserPrincipalData.AllowAdmissionToAccoount = generalConfiguration.AllowAdmissionToAccoount;
                loginModel.UserPrincipalData.AllowCenterShare = generalConfiguration.AllowCenterShare;
                loginModel.UserPrincipalData.AllowSplitCostOfService = generalConfiguration.AllowSplitCostOfService;
                loginModel.UserPrincipalData.AllowUniversityAccountHead = generalConfiguration.AllowUniversityAccountHead;
                loginModel.UserPrincipalData.EducationTypeKey = generalConfiguration.EducationTypeKey;
                loginModel.LoginSuccess = true;
                loginModel.IsSuccessful = true;

                //Company
                Company company = dbContext.Companies.FirstOrDefault();
                loginModel.UserPrincipalData.CompanyLogo = company.CompanyLogo != null ? UrlConstants.CompanyLogo + company.CompanyLogo : "";
            }

            //loginModel.UserPrincipalData.MACAddress = GetMACAddress();

            //List<string> macadress = GetMACAddress();

            //if (System.Web.Configuration.WebConfigurationManager.AppSettings["UTS"] != null)
            //{
            //    string mac = System.Web.Configuration.WebConfigurationManager.AppSettings["UTS"].ToString();

            //    string decryptmac = SecurityManagement.Decrypt1(mac);

            //    if (!macadress.Contains(decryptmac))
            //    //if (decryptmac != loginModel.UserPrincipalData.MACAddress)
            //    {
            //        loginModel = LoadUser(user, loginModel, false);

            //        loginModel.Message = EduSuiteUIResources.LoginRestriction;
            //        return loginModel;
            //    }
            //}
            return loginModel;
        }

        /// <summary>
        /// Loads the user.
        /// </summary>
        /// <param name="user">The user.</param>
        /// <param name="model">The model.</param>
        /// <param name="loginSuccess">if set to <c>true</c> [login success].</param>
        /// <returns></returns>
        private LoginModel LoadUser(AppUserViewModel user, LoginModel model, bool loginSuccess)
        {
            model.User = user;
            if (user != null)
            {
                model.LoginUsername = user.EmailAddress;
            }

            model.LoginSuccess = loginSuccess;
            model.FailedLogin = !loginSuccess;

            return model;
        }

        private void LoadUser(string username)
        {
            CurrentUser = dbContext.AppUsers.Where(u => u.EmailAddress.Equals(username, StringComparison.OrdinalIgnoreCase)).Select(row => new AppUserViewModel
            {
                RowKey = row.RowKey,
                AppUserName = row.AppUserName,
                FirstName = row.FirstName + " " + (row.MiddleName ?? "") + " " + row.LastName,
                EmailAddress = row.EmailAddress,
                Phone1 = row.Phone1,
                Phone2 = row.Phone2,
                RoleKey = row.RoleKey,
                IsActive = row.IsActive,
                Image = row.Image,

            }).SingleOrDefault();
            //if (CurrentUser != null) // Harshal TO DO -- Do we need this if statement
            //    UserRoleModel = new RoleModel(CurrentUser.Role.RoleName);
        }

        public LoginModel ChangePassword(LoginModel model)
        {
            var binPassword = SecurityManagement.Encrypt(model.OldPassword);
            try
            {
                int userCount = dbContext.AppUsers.Where(u => u.Password.Equals(binPassword) && u.IsActive).Count();
                if (userCount > 0)
                {
                    AppUser appUserModel = new AppUser();

                    using (var transaction = dbContext.Database.BeginTransaction())
                    {
                        try
                        {
                            appUserModel = dbContext.AppUsers.SingleOrDefault(row => row.RowKey == DbConstants.User.UserKey);

                            appUserModel.Password = SecurityManagement.Encrypt(model.NewPassword);

                            dbContext.SaveChanges();
                            transaction.Commit();
                            model.Message = EduSuiteUIResources.Success;
                            model.IsSuccessful = true;
                            ActivityLog.CreateActivityLog(MenuConstants.ChangePassword, ActionConstants.Edit, DbConstants.LogType.Info, DbConstants.User.UserKey, model.Message);

                        }
                        catch (Exception ex)
                        {
                            transaction.Rollback();
                            model.Message = EduSuiteUIResources.FailedToChangePassword;
                            model.IsSuccessful = false;
                            ActivityLog.CreateActivityLog(MenuConstants.ChangePassword, ActionConstants.Edit, DbConstants.LogType.Error, DbConstants.User.UserKey, ex.GetBaseException().Message);
                        }
                    }
                    return model;
                }
            }
            catch (Exception ex)
            {

            }
            throw new NotImplementedException();
        }

        public LoginModel CheckPasswordExists(string Password)
        {
            LoginModel model = new LoginModel();
            var binPassword = SecurityManagement.Encrypt(Password);

            if (dbContext.AppUsers.Where(u => u.Password.Equals(binPassword) && u.RowKey == DbConstants.User.UserKey).Any())
            {
                model.Message = EduSuiteUIResources.Success;
                model.IsSuccessful = true;
            }
            else
            {
                model.Message = EduSuiteUIResources.NotExists;
                model.IsSuccessful = false;
            }
            return model;
        }
        public List<string> GetMACAddress()
        {
            NetworkInterface[] nics = NetworkInterface.GetAllNetworkInterfaces();
            String sMacAddress = string.Empty;
            List<string> macadress = new List<string>();
            foreach (NetworkInterface adapter in nics)
            {
                //if (sMacAddress == String.Empty)// only return MAC Address from first card  
                //{
                IPInterfaceProperties properties = adapter.GetIPProperties();
                sMacAddress = adapter.GetPhysicalAddress().ToString();
                macadress.Add(sMacAddress);
                //}
            }
            
            return macadress;
        }
        #region forgotpassword

        public LoginModel ChangePasswordByOTP(LoginModel model)
        {
            try
            {
                AppUser appUserModel = new AppUser();
                using (var transaction = dbContext.Database.BeginTransaction())
                {
                    try
                    {
                        dbContext.ForgotPasswords.Where(x => x.AppUserKey == DbConstants.User.UserKey).ToList().ForEach(x =>
                        {
                            x.IsActive = false;
                        });



                        appUserModel = dbContext.AppUsers.SingleOrDefault(row => row.RowKey == DbConstants.User.UserKey);
                        appUserModel.Password = SecurityManagement.Encrypt(model.NewPassword);
                        dbContext.SaveChanges();
                        transaction.Commit();

                        model = new LoginModel();
                        model.Message = EduSuiteUIResources.Success;
                        model.IsSuccessful = true;
                        ActivityLog.CreateActivityLog(MenuConstants.ChangePassword, ActionConstants.Edit, DbConstants.LogType.Info, DbConstants.User.UserKey, model.Message);

                    }
                    catch (Exception ex)
                    {
                        transaction.Rollback();
                        model.Message = EduSuiteUIResources.FailedToChangePassword;
                        model.IsSuccessful = false;
                        ActivityLog.CreateActivityLog(MenuConstants.ChangePassword, ActionConstants.Edit, DbConstants.LogType.Error, DbConstants.User.UserKey, ex.GetBaseException().Message);
                    }
                }
                return model;

            }
            catch (Exception ex)
            {

            }
            throw new NotImplementedException();
        }

        public LoginModel ForgotPasswordSelect(LoginModel model)
        {
            LoginModel dbmodel = (from row1 in dbContext.AppUsers

                                  where row1.AppUserName.ToLower() == model.ForgotPasswordName.ToLower() || row1.EmailAddress.ToLower() == model.ForgotPasswordName.ToLower() || row1.Phone1 == model.ForgotPasswordName.ToLower()
                                  select new LoginModel
                                  {
                                      LoginUsername = row1.AppUserName,
                                      MobileNumber = row1.Phone1,
                                      EmailAddress = row1.EmailAddress,
                                      UserKey = row1.RowKey

                                  }).FirstOrDefault();


            if (dbmodel == null)
            {
                model.Message = EduSuiteUIResources.User_CannotFind;
                model.IsSuccessful = false;
                return model;
            }
            else
            {
                model.EmailAddress = dbmodel.EmailAddress;
                model.MobileNumber = dbmodel.MobileNumber;

                using (var transaction = dbContext.Database.BeginTransaction())
                {

                    try
                    {


                        List<ForgotPassword> deleteItems = dbContext.ForgotPasswords.Where(x => x.AppUserKey == DbConstants.User.UserKey).ToList();
                        dbContext.ForgotPasswords.RemoveRange(deleteItems);
                        dbContext.SaveChanges();

                        ForgotPassword dbforgot = new ForgotPassword();
                        Int64 maxKey = dbContext.ForgotPasswords.Select(p => p.RowKey).DefaultIfEmpty().Max();
                        dbforgot.RowKey = Convert.ToInt64(maxKey + 1);
                        dbforgot.IsActive = true;
                        dbforgot.AppUserKey = DbConstants.User.UserKey;
                        dbforgot.OTP = model.OTP;
                        dbContext.ForgotPasswords.Add(dbforgot);
                        dbContext.SaveChanges();
                        transaction.Commit();
                        //model.Message = ApplicationResources.Success;
                        model.IsSuccessful = true;
                        model.OTP = "";
                        model.ForgotPasswordKey = dbforgot.RowKey;

                        return model;

                    }
                    catch (Exception ex)
                    {
                        transaction.Rollback();
                        model.Message = EduSuiteUIResources.OTPFailed;
                        model.IsSuccessful = false;
                    }

                }









            }
            return model;
        }
        public LoginModel CheckOTP(LoginModel model)
        {
            ForgotPassword dbForgotPassword = dbContext.ForgotPasswords.Where(x => x.RowKey == model.ForgotPasswordKey && x.OTP == model.OTP && x.IsActive == true).SingleOrDefault();
            if (dbForgotPassword != null)
            {
                model.IsOTPSuccess = true;
                model.LoginUsername = dbContext.ForgotPasswords.Where(x => x.RowKey == model.ForgotPasswordKey).Select(x => x.AppUser.AppUserName).SingleOrDefault();

                using (var transaction = dbContext.Database.BeginTransaction())
                {
                    try
                    {
                        dbForgotPassword.IsVerified = true;
                        dbContext.SaveChanges();
                        transaction.Commit();
                        return model;

                    }
                    catch (Exception ex)
                    {
                        transaction.Rollback();
                        model.Message = EduSuiteUIResources.OTPFailed;
                        model.IsSuccessful = false;
                    }

                }


            }
            else
            {
                model.IsOTPSuccess = false;
                model.Message = EduSuiteUIResources.OTPFailed;
            }
            model.IsSuccessful = true;
            return model;
        }


        public LoginModel AttempOTPLogin(string userName)
        {
            var loginModel = new LoginModel();

            loginModel = ValidateUserOTP(userName, loginModel);
            if (loginModel.LoginSuccess)
            {
                ActivityLog.CreateActivityLog(MenuConstants.Login, ActionConstants.Login, DbConstants.LogType.Info, (loginModel.UserPrincipalData != null ? loginModel.UserPrincipalData.UserKey.ToString() : userName), EduSuiteUIResources.Success);

            }
            else
            {
                ActivityLog.CreateActivityLog(MenuConstants.Login, ActionConstants.Login, DbConstants.LogType.Error, (loginModel.UserPrincipalData != null ? loginModel.UserPrincipalData.UserKey.ToString() : userName), loginModel.Message);
            }
            return loginModel;
        }


        public LoginModel CheckOTPExist(LoginModel model)
        {
            model.IsOTPSuccess = dbContext.ForgotPasswords.Where(x => x.AppUserKey == DbConstants.User.UserKey && x.IsActive == true && x.IsVerified == true).Any();
            return model;
        }

        private LoginModel ValidateUserOTP(string userName, LoginModel loginModel)
        {

            var isLockedOut = false;

            AppUserViewModel user = new AppUserViewModel();

            try
            {

                user = dbContext.AppUsers.Where(u => (u.AppUserName.Equals(userName, StringComparison.OrdinalIgnoreCase) || u.EmailAddress.Equals(userName, StringComparison.OrdinalIgnoreCase))).Select(row => new AppUserViewModel
                {
                    RowKey = row.RowKey,
                    AppUserName = row.AppUserName,
                    FirstName = row.FirstName + " " + (row.MiddleName ?? "") + " " + row.LastName,
                    EmailAddress = row.EmailAddress,
                    Phone1 = row.Phone1,
                    Phone2 = row.Phone2,
                    RoleKey = row.RoleKey,
                    IsActive = row.IsActive,
                    Image = row.Image,
                    //IsLocked = row.IsLocked ?? false
                }).SingleOrDefault();
            }
            catch (Exception e)
            {

                loginModel = LoadUser(user, loginModel, false);

                return loginModel;
            }

            loginModel.User = user;

            if (user == null)
            {
                loginModel = LoadUser(user, loginModel, false);

                loginModel.Message = EduSuiteUIResources.InvalidUserNamePassword;
                return loginModel;
            }
            else if (!user.IsActive)
            {
                loginModel = LoadUser(user, loginModel, false);
                loginModel.Message = EduSuiteUIResources.ErrorUserNotActive;
                return loginModel;
            }
            //else if (user.IsLocked)
            //{
            //    loginModel = LoadUser(user, loginModel, false);
            //    loginModel.Message = ApplicationResources.ErrorUserLocked;
            //    return loginModel;
            //}

            loginModel = LoadUser(user, loginModel, true);

            if (loginModel.User != null)
            {
                dbContext.SaveChanges();

                loginModel.UserPrincipalData = dbContext.Employees.Where(row => row.AppUserKey == loginModel.User.RowKey).Select(row => new CITSEduSuitePrincipalData
                {
                    BranchKey = row.BranchKey,
                    CompanyKey = row.Branch.CompanyKey,
                    CompanyName = row.Branch.Company.CompanyName

                }).FirstOrDefault();
                if (loginModel.UserPrincipalData == null)
                {
                    loginModel.UserPrincipalData = new CITSEduSuitePrincipalData();
                }
                loginModel.UserPrincipalData.Name = loginModel.User.FirstName;
                loginModel.UserPrincipalData.UserKey = loginModel.User.RowKey;
                loginModel.UserPrincipalData.RoleKey = loginModel.User.RoleKey;
                loginModel.LoginSuccess = true;
                loginModel.IsSuccessful = true;
            }

            return loginModel;
        }
        #endregion

    }

}
