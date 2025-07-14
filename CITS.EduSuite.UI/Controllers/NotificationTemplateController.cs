using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using CITS.EduSuite.Business.Models.ViewModels;
using CITS.EduSuite.Business.Interfaces;
using CITS.EduSuite.Business.Provider;
using System.Web.Security;
using CITS.EduSuite.Business.Models.Security;

namespace CITS.EduSuite.UI.Controllers
{
    public class NotificationTemplateController : BaseController
    {
        // GET: NotificationTemplate

        private INotificationTemplateService notificationTemplateService;

        public NotificationTemplateController(INotificationTemplateService objNotificationTemplateService)
        {
            this.notificationTemplateService = objNotificationTemplateService;

        }

        [ActionAuthenticationAttribute(MenuCode = MenuConstants.NotificationTemplate, ActionCode = ActionConstants.MenuAccess)]
        public ActionResult NotificationTemplateList()
        {
            return View();

        }

        public JsonResult GetNotificationTemplates(string searchText)
        {
            int page = 1, rows = 15;
            List<NotificationTemplateViewModel> NotificationTemplateList = new List<NotificationTemplateViewModel>();
            NotificationTemplateList = notificationTemplateService.GetNotificationTemplates(searchText);
            int pageIndex = Convert.ToInt32(page) - 1;
            int pageSize = rows;
            int toatalRecords = NotificationTemplateList.Count();
            var totalPage = (int)Math.Ceiling((float)toatalRecords / (float)rows);

            var jsonData = new
            {
                total = totalPage,
                page,
                records = toatalRecords,
                rows = NotificationTemplateList
            };
            return Json(jsonData, JsonRequestBehavior.AllowGet);
        }

        [ActionAuthenticationAttribute(MenuCode = MenuConstants.NotificationTemplate, ActionCode = ActionConstants.AddEdit)]

        public ActionResult AddEditEmailNotification(int? id)
        {
            NotificationTemplateViewModel objViewModel = new NotificationTemplateViewModel();
            objViewModel = notificationTemplateService.GetNotificationTemplateById(id ?? 0);
            if (objViewModel == null)
            {
                objViewModel = new NotificationTemplateViewModel();
            }
            else
            {
                string filePath = Server.MapPath("~/Templates/NotificationTemplate/" + objViewModel.EmailTemplateFileName);
                if (System.IO.File.Exists(filePath))
                {
                    objViewModel.EmailTemplate = System.IO.File.ReadAllText(filePath);
                }
            }
            return View(objViewModel);
        }

        [ActionAuthenticationAttribute(MenuCode = MenuConstants.NotificationTemplate, ActionCode = ActionConstants.AddEdit)]

        public ActionResult AddEditSMSNotification(int? id)
        {
            NotificationTemplateViewModel objViewModel = new NotificationTemplateViewModel();
            objViewModel = notificationTemplateService.GetNotificationTemplateById(id ?? 0);
            if (objViewModel == null)
            {
                objViewModel = new NotificationTemplateViewModel();
            }
            return View(objViewModel);
        }

        [HttpPost]
        public ActionResult AddEditEmailNotification(NotificationTemplateViewModel model)
        {
            model = notificationTemplateService.UpdateEmailNotification(model);
            if (!model.IsSuccessful)
            {
                ModelState.AddModelError("error_msg", model.Message);
            }
            else
            {
                UpdateEmailTemplate(model);
                return RedirectToAction("NotificationTemplateList");
            }
            return View(model);
        }

        [HttpPost]
        public ActionResult AddEditSMSNotification(NotificationTemplateViewModel model)
        {
            model = notificationTemplateService.UpdateSMSNotification(model);
            if (!model.IsSuccessful)
            {
                ModelState.AddModelError("error_msg", model.Message);
            }
            else
            {
                return RedirectToAction("NotificationTemplateList");
            }
            return View(model);
        }

        [HttpPost]
        public JsonResult UpdateNotificationStatus()
        {
            return Json(true);
        }

        private void UpdateEmailTemplate(NotificationTemplateViewModel model)
        {
            string filePath = Server.MapPath("~/Templates/NotificationTemplate/" + model.EmailTemplateFileName);
            if (System.IO.File.Exists(filePath))
            {
                System.IO.File.WriteAllText(filePath, model.EmailTemplate);
            }
        }

        [HttpGet]
        public JsonResult GetLatestNotificationCount()
        {
            NotificationDataViewModel model = new NotificationDataViewModel();
            //model.UserKey = GetUserKey();
            notificationTemplateService.GetLatestNotification(model);
            return Json(model, JsonRequestBehavior.AllowGet);
        }
        [HttpGet]
        public JsonResult GetLatestNotification(int pageIndex, int pageSize)
        {
            NotificationDataViewModel model = new NotificationDataViewModel();
            model.PageIndex = pageIndex;
            model.PageSize = pageSize;
            //model.UserKey = GetUserKey();
            string Records = CommonHelper.RenderPartialToString("GetLatestNotification", notificationTemplateService.GetLatestNotification(model).ToList(), this.ControllerContext);
            var data = new
            {
                recordCount = model.TotalRecords,
                latestRecordCount = model.TotalLatestRecords,
                unreadRecordCount = model.TotalUnreadRecords,
                records = Records,
                pageIndex = model.PageIndex
            };
            return Json(data, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult UpdateReadNotification(long PushNotificationKey)
        {
            NotificationDataViewModel model = new NotificationDataViewModel();
            model.PushNotificationKey = PushNotificationKey;
          // model.UserKey = GetUserKey();
            model = notificationTemplateService.UpdateReadNotification(model);
            return Json(model);
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
    }
}