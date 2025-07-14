using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using CITS.EduSuite.Business.Common;
using CITS.EduSuite.Business.Interfaces;
using CITS.EduSuite.Business.Models.Common;
using CITS.EduSuite.Business.Models.ViewModels;
using CITS.EduSuite.Business.Provider;
using CITS.EduSuite.Business.Models.Security;
using System.Web.Security;

namespace CITS.EduSuite.UI.Controllers
{
    public class BulkEmailSMSController : BaseController
    {

        private IBulkEmailSmsService BulkEmailSMSService;
        private ISharedService sharedService;
        // GET: BulkEmailSMS


        public BulkEmailSMSController(IBulkEmailSmsService objBulkEmailSMSService, ISharedService objSharedService)
        {
            this.BulkEmailSMSService = objBulkEmailSMSService;
            this.sharedService = objSharedService;

        }

        public ActionResult BulkEmailSMSList( )
        {
            BulkEmailSmsViewModel model = new BulkEmailSmsViewModel();
            
            BulkEmailSMSService.GetSearchDropdownList(model);
            ViewBag.ShowAcademicTerm = sharedService.ShowAcademicTerm();
            ViewBag.ShowUniversity = sharedService.ShowUniversity();
            return View(model);
        }

       
        public ActionResult GetBulkEmailSMS(BulkEmailSmsViewModel model)
        {
            
            BulkEmailSMSService.GetBulkEmailSms(model);
            return PartialView(model);
        }

        public ActionResult GetBulkEmailPopUp(BulkEmailSmsViewModel model)
        {

            SendBulkEmailViewModel EmailModel = new SendBulkEmailViewModel();   
            EmailModel.BulkEmailList = new List<SendBulkEmailList>();

            if (model.RoleKeys != null)
            {
                int i = 0;
                foreach (var RoleKey in model.RoleKeys)
                {
                    SendBulkEmailList emailList = new SendBulkEmailList();
                    emailList.RoleKey = RoleKey;
                    emailList.RowKey = model.RowKeys[i];
                    emailList.EmailAddress = model.Emails[i];
                    EmailModel.BulkEmailList.Add(emailList);
                    i++;
                }
            }
            else
            {
                SendBulkEmailList emailList = new SendBulkEmailList();
                EmailModel.BulkEmailList.Add(emailList);
            }

            return PartialView(EmailModel);
        }

        public ActionResult GetBulkSMSPopUp(BulkEmailSmsViewModel model)
        {

            SendBulkSMSViewModel SMSModel = new SendBulkSMSViewModel();
            SMSModel.BulkSMSList = new List<SendBulkSMSList>();

            if (model.RoleKeys != null)
            {
                int i = 0;
                foreach (var RoleKey in model.RoleKeys)
                {
                    SendBulkSMSList SMSList = new SendBulkSMSList();
                    SMSList.RoleKey = RoleKey;
                    SMSList.RowKey = model.RowKeys[i];
                    SMSList.MobileNumber = model.SMS[i];
                    SMSModel.BulkSMSList.Add(SMSList);
                    i++;
                }
            }
            else
            {
                SendBulkSMSList SMSList = new SendBulkSMSList();
                SMSModel.BulkSMSList.Add(SMSList);
            }

            return PartialView(SMSModel);
        }
        
        [HttpPost]
        public ActionResult SendBulkEmail(SendBulkEmailViewModel model)
        {



            model = BulkEmailSMSService.CreateBulkEmailTrack(model);

            if(model.IsSuccessful)
            {
                EmailViewModel EmailModel = new EmailViewModel();
                foreach (var item in model.BulkEmailList)
                {
                    EmailModel.EmailTo = item.EmailAddress;
                    EmailModel.EmailBody = model.EmailContent;
                    EmailHelper.SendEmail(EmailModel);
                }

                UploadFile(model);
            }

            return Json(model);
        }


        [HttpPost]
        public ActionResult SendBulkSMS(SendBulkSMSViewModel model)
        {

            model = BulkEmailSMSService.CreateBulkSMSTrack(model);
            if (model.IsSuccessful)
            {                
                string MobileNumberList="";
                foreach (var item in model.BulkSMSList)
                {
                    if (MobileNumberList == "")
                    {
                        MobileNumberList = item.MobileNumber;
                    }
                    else
                    {
                        MobileNumberList = MobileNumberList + "," + item.MobileNumber;
                    }
                }

                SMSViewModel smsModel = new SMSViewModel();
                smsModel.SMSContent = model.SMSContent;
                smsModel.SMSReceiptants = MobileNumberList;
               string message= SMSHelper.SendSMS(smsModel);
              
            }

            return Json(model);
        }

        private void UploadFile(SendBulkEmailViewModel model)
        {
            string FilePath = Server.MapPath(UrlConstants.BulkEmailURL);
            if (!Directory.Exists(FilePath))
            {
                Directory.CreateDirectory(FilePath);
            }

            using (StreamWriter sw = System.IO.File.CreateText(FilePath + "/" + model.EmailFileName)) 
            {
                sw.WriteLine(model.EmailContent);

            }
                
           
        }



        [HttpPost]
        public JsonResult GetCourseTypeByAcademicTerm(BulkEmailSmsViewModel model)
        {
            BulkEmailSMSService.FillCourseTypes(model);
            return Json(model, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult GetCourseByCourseType(BulkEmailSmsViewModel model)
        {

            BulkEmailSMSService.FillCourse(model);
            return Json(model, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult GetUniversityByCourse(BulkEmailSmsViewModel model)
        {
            BulkEmailSMSService.FillUniversityMasters(model);
            return Json(model, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult GetYearsByAcademicTermKey(BulkEmailSmsViewModel model)
        {
            BulkEmailSMSService.FillYears(model);
            return Json(model, JsonRequestBehavior.AllowGet);
        }
    }
}