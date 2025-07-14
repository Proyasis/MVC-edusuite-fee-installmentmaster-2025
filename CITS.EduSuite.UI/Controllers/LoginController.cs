using CITS.EduSuite.Business.Interfaces;
using CITS.EduSuite.Business.Models.ViewModels;
using CITS.EduSuite.Business.Provider;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using System.Management;
using System.Net.NetworkInformation;
using CITS.EduSuite.Business.Models.Security;
using CITS.EduSuite.Business.Common;
using System.Collections.ObjectModel;
using System.Net;
using System.IO;
using CITS.EduSuite.Business.Models.Resources;

namespace CITS.EduSuite.UI.Controllers
{
    public class LoginController : BaseController
    {
        private ILoginService loginService;
        private ICookieAuthentationProvider cookieProvider;

        public LoginController(ILoginService objLoginService, ICookieAuthentationProvider objcookieProvider)
        {
            this.loginService = objLoginService;
            this.cookieProvider = objcookieProvider;
        }

        [OutputCache(NoStore = true, Duration = 0, VaryByParam = "None")]
        [HttpGet]
        public ActionResult Index(string returnUrl)
        {
            //NetworkInterface[] nics = NetworkInterface.GetAllNetworkInterfaces();
            //String MACAddress = string.Empty;
            //foreach (NetworkInterface ni in NetworkInterface.GetAllNetworkInterfaces())
            //{
            //    if (ni.OperationalStatus == OperationalStatus.Up)
            //    {
            //        MACAddress = ni.GetPhysicalAddress().ToString();
            //        break;
            //    }
            //}
            //Response.Write(MACAddress);
            // Response.Write(MACAddress);
            var model = new LoginModel();
            model.ReturnUrl = returnUrl;
            List<string> test = loginService.GetMACAddress();
            ViewBag.mac = test;
            return View(model);
        }

        [OutputCache(NoStore = true, Duration = 0, VaryByParam = "None")]
        //[ValidateAntiForgeryToken]
        public ActionResult Login(LoginModel model)
        {
            var loginModel = model;

            if (!String.IsNullOrEmpty(model.LoginUsername) && !String.IsNullOrEmpty(model.LoginPassword))
            {
                loginModel = loginService.AttemptLogin(model.LoginUsername, model.LoginPassword, model.RoleKey);

                if (loginModel.LoginSuccess)
                {
                    //Harshal TO DO - Uncomment the below code
                    HttpCookie cookie = cookieProvider.CreateCookie(loginModel.UserPrincipalData);

                    Response.Cookies.Add(cookie);

                    if (loginModel.NeedsPasswordChange)
                        return RedirectToAction("ChangePassword", new { userName = model.LoginUsername });
                    else if (loginModel.UserPrincipalData.IsTeacher)
                        return RedirectToAction("Index", "TeacherPortal");
                    else if (loginModel.RoleKey == DbConstants.Role.Students)
                        return RedirectToAction("StudentDashBoard", "StudentPortal");
                    else if (string.IsNullOrEmpty(model.ReturnUrl))
                        return RedirectToAction("DashBoard", "Home");
                    //return RedirectToAction("PeopleView", "People", new { appUserKey = loginModel.User });
                    else
                        return Redirect(model.ReturnUrl);
                }
            }

            //var model = new LoginModel();
            model.FailedLogin = true;
            model.Message = loginModel.Message;
            ModelState.AddModelError("error_msg", model.Message);
            return View("Index", model);
        }

        public ActionResult LogOut()
        {
            loginService.Logout();
            ControllerContext controllerContext = new ControllerContext();
            cookieProvider.DeleteCookie(HttpContext.Request, HttpContext.Response);
            FormsAuthentication.SignOut();
            if (DbConstants.User.RoleKey == DbConstants.Role.Students)
            {
                return RedirectToAction("Index", "StudentPortal");
            }
            else
            {
                return RedirectToAction("Index", "Login");
            }

        }

        [ActionAuthenticationAttribute(MenuCode = MenuConstants.ChangePassword, ActionCode = ActionConstants.MenuAccess)]

        [HttpGet]
        public ActionResult ChangePassword()
        {
            return PartialView();
        }

        [HttpPost]
        public ActionResult ChangePassword(LoginModel model)
        {
            CookieProvider cookieProvider = new CookieProvider();

            string[] modelAvoid = { "LoginPassword", "LoginUsername", "ForgotPasswordName" };

            ModelState.Where(row => modelAvoid.Contains(row.Key)).ToList().ForEach(row => ModelState[row.Key].Errors.Clear());
            if (ModelState.IsValid)
            {
                HttpCookie authCookie = Request.Cookies[FormsAuthentication.FormsCookieName];
                CITSEduSuitePrincipalData userData = cookieProvider.GetCookie(authCookie);
                if (authCookie != null)
                {
                    //int AppUserKey = userData.UserKey;

                    model = loginService.ChangePassword(model);
                }
                if (model.IsSuccessful)
                {
                    Toastr.AddToastMessage(AppConstants.Common.SUCCESS, model.Message, ToastType.Success);
                }
                else
                {
                    Toastr.AddToastMessage(AppConstants.Common.FAILED, model.Message, ToastType.Error);
                }
                //return Redirect(this.Request.UrlReferrer.AbsolutePath);
                return Json(model);
            }
            model.Message = EduSuiteUIResources.Failed;

            return Json(model);

            //return Redirect(this.Request.UrlReferrer.AbsolutePath);

        }

        [HttpGet]
        public JsonResult CheckPasswordExists(string OldPassword)
        {
            LoginModel model = new LoginModel();
            CookieProvider cookieProvider = new CookieProvider();

            HttpCookie authCookie = Request.Cookies[FormsAuthentication.FormsCookieName];
            CITSEduSuitePrincipalData userData = cookieProvider.GetCookie(authCookie);
            if (authCookie != null)
            {


                model = loginService.CheckPasswordExists(OldPassword);
            }
            return Json(model.IsSuccessful, JsonRequestBehavior.AllowGet);

        }

        //private int GetUserKey()
        //{
        //    CookieProvider cookieProvider = new CookieProvider();
        //    HttpCookie authCookie = Request.Cookies[FormsAuthentication.FormsCookieName];
        //    if (authCookie != null)
        //    {
        //        CITSEduSuitePrincipalData userData = cookieProvider.GetCookie(authCookie);
        //        return userData.UserKey;

        //    }
        //    return 0;
        //}


        [HttpPost]
        public ActionResult ForgotPassword(LoginModel model)
        {

            if (model.ForgotPasswordKey == 0)
            {
                // declare array string to generate random string with combination of small,capital letters and numbers

                model.OTP = getOTP();
                string OTP = model.OTP;
                model = loginService.ForgotPasswordSelect(model);

                if (model.IsSuccessful == true)
                {
                    if (model.OTPType == 1)
                    {
                        SMSViewModel smsModel = new SMSViewModel();
                        smsModel.SMSContent = "To authenticate, please use the following One Time Password (OTP) " + OTP;
                        smsModel.SMSReceiptants = model.MobileNumber;
                        string message = SMSHelper.SendSMS(smsModel);
                    }
                    else if (model.OTPType == 2)
                    {
                        if (model.EmailAddress != null && model.EmailAddress != "")
                        {
                            EmailViewModel EmailModel = new EmailViewModel();
                            EmailModel.EmailSubject = "Edusuite Password assistance";
                            EmailModel.EmailBody = "To authenticate, please use the following One Time Password (OTP): <br> <br>  <b>" + OTP + "</b> ";
                            EmailModel.EmailTo = model.EmailAddress;
                            EmailHelper.SendEmail(EmailModel);
                        }
                    }
                }

            }
            else
            {
                model = loginService.CheckOTP(model);
                if (model.IsOTPSuccess == true)
                {
                    model = loginService.AttempOTPLogin(model.LoginUsername);

                    if (model.LoginSuccess)
                    {
                        HttpCookie cookie = cookieProvider.CreateCookie(model.UserPrincipalData);
                        Response.Cookies.Add(cookie);


                        if (string.IsNullOrEmpty(model.ReturnUrl))
                            return RedirectToAction("NewPassword");

                        else
                            return Redirect(model.ReturnUrl);
                    }
                }

            }


            return View("ForgotPassword", model);
        }


        private string getOTP()
        {
            char[] charArr = "0123456789abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ".ToCharArray();
            string strrandom = string.Empty;
            Random objran = new Random();
            int noofcharacters = 6;
            for (int i = 0; i < noofcharacters; i++)
            {
                //It will not allow Repetation of Characters
                int pos = objran.Next(1, charArr.Length);
                if (!strrandom.Contains(charArr.GetValue(pos).ToString()))
                    strrandom += charArr.GetValue(pos);
                else
                    i--;
            }

            return strrandom.ToUpper();
        }

        [HttpPost]
        public ActionResult ChangePasswordByOTP(LoginModel model)
        {
            CookieProvider cookieProvider = new CookieProvider();

            string[] modelAvoid = { "LoginPassword", "LoginUsername", "OldPassword", "ForgotPasswordName" };

            ModelState.Where(row => modelAvoid.Contains(row.Key)).ToList().ForEach(row => ModelState[row.Key].Errors.Clear());

            var errors = ModelState.Select(x => x.Value.Errors).Where(y => y.Count > 0).ToList();

            if (ModelState.IsValid)
            {
                HttpCookie authCookie = Request.Cookies[FormsAuthentication.FormsCookieName];
                CITSEduSuitePrincipalData userData = cookieProvider.GetCookie(authCookie);
                if (authCookie != null)
                {
                    //int AppUserKey = userData.UserKey;

                    model = loginService.ChangePasswordByOTP(model);
                }
                if (model.IsSuccessful)
                {
                    Toastr.AddToastMessage(AppConstants.Common.SUCCESS, model.Message, ToastType.Success);
                }
                else
                {
                    Toastr.AddToastMessage(AppConstants.Common.FAILED, model.Message, ToastType.Error);
                }

                return View("NewPassword", model);
            }
            model.Message = EduSuiteUIResources.Failed;


            return View("ForgotPassword", model);

        }

        [HttpGet]
        public ActionResult ForgotPassword()
        {
            LoginModel dbLogin = new LoginModel();
            return View(dbLogin);
        }


        [HttpGet]
        public ActionResult NewPassword()
        {
            LoginModel dbLogin = new LoginModel();
            //GetUserKey(dbLogin);
            dbLogin = loginService.CheckOTPExist(dbLogin);
            return View(dbLogin);
        }



        //private void GetUserKey(LoginModel model)
        //{
        //    CookieProvider cookieProvider = new CookieProvider();
        //    HttpCookie authCookie = Request.Cookies[FormsAuthentication.FormsCookieName];
        //    if (authCookie != null)
        //    {
        //        CITSEduSuitePrincipalData userData = cookieProvider.GetCookie(authCookie);
        //        model.UserKey = userData.UserKey;
        //        model.RoleKey = userData.RoleKey;
        //    }
        //}




    }
}