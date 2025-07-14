using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using CITS.EduSuite.Business.Models.ViewModels;

namespace CITS.EduSuite.UI
{
    public static class URLEncodeHelper
    {
        public static MvcHtmlString EncodedActionLink(this HtmlHelper htmlHelper, string linkText, string actionName, string controllerName, object routeValues, object htmlAttributes)
        {
            string queryString = string.Empty;
            string htmlAttributesString = string.Empty;
            if (routeValues != null)
            {
                RouteValueDictionary d = new RouteValueDictionary(routeValues);
                for (int i = 0; i < d.Keys.Count; i++)
                {
                    if (i > 0)
                    {
                        queryString += "?";
                    }
                    queryString += d.Keys.ElementAt(i) + "=" + d.Values.ElementAt(i);
                }
            }

            if (htmlAttributes != null)
            {
                RouteValueDictionary d = new RouteValueDictionary(htmlAttributes);
                for (int i = 0; i < d.Keys.Count; i++)
                {
                    htmlAttributesString += " " + d.Keys.ElementAt(i) + "=" + d.Values.ElementAt(i);
                }
            }

            //What is Entity Framework??
            StringBuilder ancor = new StringBuilder();
            ancor.Append("<a ");
            if (htmlAttributesString != string.Empty)
            {
                ancor.Append(htmlAttributesString);
            }
            ancor.Append(" href='");
            if (controllerName != string.Empty)
            {
                ancor.Append("/" + controllerName);
            }

            if (actionName != "Index")
            {
                ancor.Append("/" + actionName);
            }
            if (queryString != string.Empty)
            {
                ancor.Append("?q=" + CryptographicHelper.Encryptor(queryString, DbConstants.EncryptionKey));
            }
            ancor.Append("'");
            ancor.Append(">");
            ancor.Append(linkText);
            ancor.Append("");
            return new MvcHtmlString(ancor.ToString());
        }

        public static string EncodedAction(string actionName, string controllerName, object routeValues)
        {
            List<string> queryStringList = new List<string>();
            string queryString = string.Empty;
            string htmlAttributesString = string.Empty;
            if (routeValues != null)
            {
                RouteValueDictionary d = new RouteValueDictionary(routeValues);
                for (int i = 0; i < d.Keys.Count; i++)
                {

                    queryStringList.Add(d.Keys.ElementAt(i) + "=" + d.Values.ElementAt(i));
                }
            }

            queryString = queryString + "?" + String.Join("&", queryStringList);

            //What is Entity Framework??
            StringBuilder ancor = new StringBuilder();
            if (controllerName != string.Empty)
            {
                ancor.Append("/" + controllerName);
            }

            if (actionName != "Index")
            {
                ancor.Append("/" + actionName);
            }
            if (queryString != string.Empty)
            {
                ancor.Append("?q=" + CryptographicHelper.Encryptor(queryString, DbConstants.EncryptionKey));
            }

            return ancor.ToString();
        }

    }

    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
    public class EncryptedActionParameterAttribute : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext filterContext)
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
                for (int i = 0; i < decryptedParameters.Count; i++)
                {
                    filterContext.ActionParameters[decryptedParameters.Keys.ElementAt(i)] = decryptedParameters.Values.ElementAt(i);
                }
                base.OnActionExecuting(filterContext);
            }
            catch(Exception e)
            {

            }

        }

        private Object TryConvertTo(Type type,string input)
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