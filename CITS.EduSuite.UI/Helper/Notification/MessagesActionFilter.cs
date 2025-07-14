using System.Linq;
using System.Web.Mvc;
using CITS.EduSuite.UI.Controllers;


namespace CITS.EduSuite.UI
{
    public class MessagesActionFilter : ActionFilterAttribute
    {
        public bool IsRedirect { get; set; }
        // This method is called BEFORE the action method is executed
        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            // Check for incoming Toastr objects, in case we've been redirected here
            var controller = filterContext.Controller as BaseController;
            if (controller != null)
                controller.Toastr = (controller.TempData["Toastr"] as Toastr) ?? new Toastr();
            base.OnActionExecuting(filterContext);
        }

        // This method is called AFTER the action method is executed but BEFORE the
        // result is processed (in the view or in the redirection)
        public override void OnActionExecuted(ActionExecutedContext filterContext)
        {
            var controller = filterContext.Controller as BaseController;
            if (filterContext.Result.GetType() == typeof(ViewResult))
            {
                if (controller.Toastr != null && controller.Toastr.ToastMessages.Any())
                {
                    // We're going to a view so we store Toastr in the ViewData collection
                    controller.ViewData["Toastr"] = controller.Toastr;
                }
            }

            else if (filterContext.HttpContext.Request.IsAjaxRequest())
            {
                var response = filterContext.HttpContext.Response;
                foreach (var item in controller.Toastr.ToastMessages)
                {
                    response.AddHeader("X-Message-Type", item.ToastType.ToString());
                    response.AddHeader("X-Message-Title", item.Title);
                    response.AddHeader("X-Message", item.Message);
                }

            }
            else if (filterContext.Result.GetType() == typeof(RedirectToRouteResult) || IsRedirect)
            {
                if (controller.Toastr != null && controller.Toastr.ToastMessages.Any())
                {
                    // User is being redirected to another action method so we store Toastr in
                    // the TempData collection
                    controller.TempData["Toastr"] = controller.Toastr;
                }

            }
            base.OnActionExecuted(filterContext);
        }
    }
}