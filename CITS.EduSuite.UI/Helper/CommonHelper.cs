using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using CITS.EduSuite.Business.Models.ViewModels;
using System.Web.Mvc;
using System.IO;
using HandlebarsDotNet;

namespace CITS.EduSuite.UI
{
    public static class CommonHelper
    {
        public static string GetMonthNameFromNumber(int MonthNumber)
        {
            return System.Globalization.CultureInfo.CurrentCulture.DateTimeFormat.GetMonthName(MonthNumber);
        }
        public static int CalculateAge(DateTime dateOfBirth)
        {
            int age = 0;
            age = DateTimeUTC.Now.Year - dateOfBirth.Year;
            if (DateTimeUTC.Now.DayOfYear < dateOfBirth.DayOfYear)
                age = age - 1;
            return age;
        }
        public static string RenderPartialToString(string viewName, object model, ControllerContext ControllerContext)
        {
            if (string.IsNullOrEmpty(viewName))
                viewName = ControllerContext.RouteData.GetRequiredString("action");
            ViewDataDictionary ViewData = new ViewDataDictionary();
            TempDataDictionary TempData = new TempDataDictionary();
            ViewData.Model = model;

            using (StringWriter sw = new StringWriter())
            {
                ViewEngineResult viewResult = ViewEngines.Engines.FindPartialView(ControllerContext, viewName);
                ViewContext viewContext = new ViewContext(ControllerContext, viewResult.View, ViewData, TempData, sw);
                viewResult.View.Render(viewContext, sw);

                return sw.GetStringBuilder().ToString();
            }

        }
        public static void RegisterHandleBarHelpers()
        {
            Handlebars.RegisterHelper("ifCond", (writer, options, context, arguments) =>
                    {
                        //var str = ((decimal)(arguments[0] ?? 0))==;
                        if (arguments[0] == arguments[1])
                        {
                            options.Template(writer, (object)context);
                        }
                        else
                        {
                            options.Inverse(writer, (object)context);
                        }


                        //if(arguments[1].ToString()=="==")
                        //    return (arguments[0].ToString() == arguments[3].ToString()) ? options.Template(writer, (object)context) : options.Inverse(writer, (object)context);
                        //  if(arguments[1].ToString()=="!=")
                        //    return (arguments[0].ToString() != arguments[3].ToString()) ? options.Template(writer, (object)context) : options.Inverse(writer, (object)context);
                        //     if(arguments[1].ToString()== "<")
                        //    return (arguments[0].ToString()  < arguments[3].ToString()) ? options.Template(writer, (object)context) : options.Inverse(writer, (object)context);
                        //      if(arguments[1].ToString()=="<=")
                        //         return   (arguments[0].ToString()  <= arguments[3].ToString() ) ? options.Template(writer, (object)context) : options.Inverse(writer, (object)context);
                        //     if(arguments[1].ToString()== ">")
                        //     return (arguments[0].ToString()  > arguments[3].ToString() ) ? options.Template(writer, (object)context) : options.Inverse(writer, (object)context);
                        //     if(arguments[1].ToString()==">=")
                        //     return (arguments[0].ToString()  >= arguments[3].ToString() ) ? options.Template(writer, (object)context) : options.Inverse(writer, (object)context);
                        //      if(arguments[1].ToString()=="&&")
                        //     return (arguments[0].ToString()  && arguments[3].ToString() ) ? options.Template(writer, (object)context) : options.Inverse(writer, (object)context);
                        //      if(arguments[1].ToString()=="||")
                        //    return (arguments[0].ToString()  || arguments[3].ToString() ) ? options.Template(writer, (object)context) : options.Inverse(writer, (object)context);


                    });
        }
        
    }

}