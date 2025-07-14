using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using CITS.EduSuite.Business.Interfaces;
using CITS.EduSuite.Business.Models.Security;
using CITS.EduSuite.Business.Models.ViewModels;
using CITS.EduSuite.Business.Provider;
using CITS.EduSuite.Business.Services;
using CITS.EduSuite.UI.Controllers;
using CITS.EduSuite.UI;
using Microsoft.Practices.Unity;
using CITS.EduSuite.Business.Models.Resources;

namespace CITS.EduSuite.UI
{
    public class ActionAuthenticationAttribute : ActionFilterAttribute, IAuthorizationFilter, IActionFilter
    {

        public string ActionCode { get; set; }
        public string MenuCode { get; set; }


        public void OnAuthorization(AuthorizationContext filterContext)
        {
            //var featureConst = (FeatureConst)filterContext.RouteData.Values["AllowFeature"];
            IEmployeeUserPermissionService employeeUserPermissionService = new EmployeeUserPermissionService();

            var filterAttribute = filterContext.ActionDescriptor.GetFilterAttributes(true)
                                    .Where(a => a.GetType() == typeof(ActionAuthenticationAttribute));
            if (filterAttribute != null)
            {
                foreach (ActionAuthenticationAttribute attr in filterAttribute)
                {
                    MenuCode = attr.MenuCode;
                    ActionCode = attr.ActionCode;
                }

                bool allowed = false;
                HttpCookie authCookie = filterContext.HttpContext.Request.Cookies[FormsAuthentication.FormsCookieName];

                if (authCookie != null)
                {
                    CookieProvider cookieAuthentationProvider = new CookieProvider();
                    CITSEduSuitePrincipalData userData = cookieAuthentationProvider.GetCookie(authCookie);

                    if (userData.UserKey != DbConstants.AdminKey)
                    {
                        UserPermissionViewModel model = new UserPermissionViewModel();
                        //model.UserKey = userData.UserKey;
                        if (ActionCode == ActionConstants.AddEdit)
                        {
                            var id = DecryptParameters(filterContext).Select(row => row.Value).FirstOrDefault();
                            if (id == null || (id ?? "0").ToString() == "0")
                            {
                                model.ActionCode = ActionConstants.Add;
                            }
                            else
                            {
                                model.ActionCode = ActionConstants.Edit;
                            }
                        }
                        else
                        {
                            model.ActionCode = ActionCode;
                        }
                        model.MenuCode = MenuCode;

                        allowed = employeeUserPermissionService.CheckUserPermission(model);
                        if (!allowed)
                        {
                            var baseController = ((BaseController)filterContext.Controller);
                            if (baseController != null)
                                baseController.Toastr = (baseController.TempData["Toastr"] as Toastr) ?? new Toastr();
                            if (filterContext.HttpContext.Request.IsAjaxRequest())
                            {
                                var response = filterContext.HttpContext.Response;

                                response.AddHeader("X-Message-Type", ToastType.Error.ToString());
                                response.AddHeader("X-Message-Title", EduSuiteUIResources.AccessDenied);
                                response.AddHeader("X-Message", EduSuiteUIResources.ActionPermisionMessage);
                                filterContext.Result = new EmptyResult();
                            }
                            else
                            {
                                baseController.AddToastMessage(EduSuiteUIResources.AccessDenied, EduSuiteUIResources.ActionPermisionMessage, ToastType.Error);

                                if (filterContext.HttpContext.Request.UrlReferrer != null)
                                {
                                    filterContext.Result = new RedirectResult(filterContext.HttpContext.Request.UrlReferrer.ToString(), false);
                                    baseController.TempData["Toastr"] = baseController.Toastr;

                                }
                                else
                                {
                                    baseController.ViewData["Toastr"] = baseController.Toastr;
                                    filterContext.Result = new EmptyResult();
                                }
                            }
                        }

                    }
                }
            }
        }

        private Dictionary<string, object> DecryptParameters(AuthorizationContext filterContext)
        {
            try
            {
                Dictionary<string, object> decryptedParameters = new Dictionary<string, object>();
                if (HttpContext.Current.Request.QueryString.Get("q") != null)
                {
                    string encryptedQueryString = HttpContext.Current.Request.QueryString.Get("q");
                    encryptedQueryString = encryptedQueryString.Replace(" ", "+");
                    string decrptedString = CryptographicHelper.DecryptFromBase64String(encryptedQueryString.ToString(), DbConstants.EncryptionKey);
                    string[] paramsArrs = decrptedString.Split('&');

                    var ActionInfo = filterContext.ActionDescriptor;
                    var pars = ActionInfo.GetParameters().Select(row => new { row.ParameterName, row.ParameterType });

                    for (int i = 0; i < paramsArrs.Length; i++)
                    {
                        string[] paramArr = paramsArrs[i].Split('=');
                        var param = ActionInfo.GetParameters().Where(row => row.ParameterName == paramArr[0]).FirstOrDefault();
                        var paramValue = String.IsNullOrEmpty(paramArr[1]) ? null : TryConvertTo(param.ParameterType, paramArr[1]);
                        decryptedParameters.Add(paramArr[0], paramValue);
                    }
                }
                else
                {
                    var param = filterContext.RouteData.Values["id"];
                    if (param == null || param.ToString() == "undefined" || param.ToString() == "null")
                    {
                        param = 0;
                    }
                    decryptedParameters.Add("id", param);
                }
                return decryptedParameters;
            }
            catch (Exception e)
            {
                return new Dictionary<string, object>();
            }
        }

        private Object TryConvertTo(Type type, string input)
        {
            Object result = null;
            var t = type;
            try
            {


                if (t.IsGenericType && t.GetGenericTypeDefinition().Equals(typeof(Nullable<>)))
                {
                    if (input == null)
                    {
                        return null;
                    }

                    t = Nullable.GetUnderlyingType(t);
                }
            }
            catch
            {
            }

            return Convert.ChangeType(input, t); ;
        }

    }
}