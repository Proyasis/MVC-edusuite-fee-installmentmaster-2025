using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using CITS.EduSuite.Business.Common;
using CITS.EduSuite.Business.Interfaces;
using CITS.EduSuite.Business.Models.Security;
using CITS.EduSuite.Business.Models.ViewModels;
using CITS.EduSuite.Business.Provider;

namespace CITS.EduSuite.UI.Controllers
{
    public class NotificationDataController : BaseController
    {
        private INotificationDataService notificationDataService;
        public NotificationDataController(INotificationDataService objNotificationDataService)
        {
            this.notificationDataService = objNotificationDataService;
        }

        [ActionAuthenticationAttribute(MenuCode = MenuConstants.NotificationData, ActionCode = ActionConstants.MenuAccess)]
        [HttpGet]
        public ActionResult NotificationDataList()
        {
            NotificationDataViewModel objViewModel = new NotificationDataViewModel();

           

            return View(objViewModel);
        }



        [HttpGet]
        public JsonResult GetNotification(string SearchText, long? AppUserKey, bool NotificationType, string SearchFromDate, string SearchToDate, string sidx, string sord, int page, int rows)
        {
            long TotalRecords = 0;
            List<NotificationDataViewModel> notificationList = new List<NotificationDataViewModel>();
            NotificationDataViewModel objViewModel = new NotificationDataViewModel();


            objViewModel.SearchText = SearchText;
            objViewModel.AppUserKey = AppUserKey ?? 0;
            objViewModel.NotificationType = NotificationType;
            objViewModel.SearchFromDate = DateTime.ParseExact(SearchFromDate, "dd/MM/yyyy", null); 
            objViewModel.SearchToDate = DateTime.ParseExact(SearchToDate, "dd/MM/yyyy", null); 
            objViewModel.PageIndex = page;
            objViewModel.PageSize = rows;
            objViewModel.SortBy = sidx;
            objViewModel.SortOrder = sord;

            if (NotificationType == true)
            {
                notificationList = notificationDataService.GetNotification(objViewModel, out TotalRecords);
            }
            else
            {
                notificationList = notificationDataService.GetPushNotification(objViewModel, out TotalRecords);

            }

            int pageIndex = Convert.ToInt32(page) - 1;
            int pageSize = rows;
            var totalPages = (int)Math.Ceiling((float)TotalRecords / (float)rows);

            var jsonData = new
            {
                total = totalPages,
                page,
                records = TotalRecords,
                rows = notificationList
            };
            return Json(jsonData, JsonRequestBehavior.AllowGet);
        }

    }
}