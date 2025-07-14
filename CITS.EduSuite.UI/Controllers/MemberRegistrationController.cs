
using CITS.EduSuite.Business.Common;
using CITS.EduSuite.Business.Interfaces;
using CITS.EduSuite.Business.Models.Resources;
using CITS.EduSuite.Business.Models.ViewModels;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace CITS.EduSuite.UI.Controllers
{
    public class MemberRegistrationController : BaseController
    {
        // GET: MemberRegistration
        private IMemberRegistrationService MemberRegistrationService;
        public MemberRegistrationController(IMemberRegistrationService objMemberRegistration)
        {
            MemberRegistrationService = objMemberRegistration;
        }

        [HttpGet]
        public ActionResult MemberRegistrationList()
        {
            MemberRegistrationViewModel model = new MemberRegistrationViewModel();
            MemberRegistrationService.FillBranches(model);
            return View(model);
        }

        [HttpGet]
        public JsonResult GetMemberRegistration(string SearchText, short? branchkey)
        {
            int page = 1, rows = 10;

            List<MemberRegistrationViewModel> MemberRegistrationList = new List<MemberRegistrationViewModel>();
            MemberRegistrationList = MemberRegistrationService.GetMemberRegistration(SearchText, branchkey);

            int pageIndex = Convert.ToInt32(page) - 1;
            int pageSize = rows;
            int totalRecords = MemberRegistrationList.Count();
            var totalPages = (int)Math.Ceiling((float)totalRecords / (float)rows);

            var jsonData = new
            {
                total = totalPages,
                page,
                records = totalRecords,
                rows = MemberRegistrationList
            };
            return Json(jsonData, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public ActionResult AddEditMemberRegistration(Int32? Id)
        {
            MemberRegistrationViewModel model = new MemberRegistrationViewModel();
            model = MemberRegistrationService.GetMemberRegistrationById(Id);
            if (model == null)
            {
                model = new MemberRegistrationViewModel();
            }
            return View(model);
        }

        [HttpPost]
        public ActionResult AddEditMemberRegistration(MemberRegistrationViewModel model)
        {
            if (ModelState.IsValid)
            {
                if (model.RowKey != 0)
                {
                    model = MemberRegistrationService.UpdateMemberRregistration(model);
                }
                else
                {
                    model = MemberRegistrationService.CreateMemberRregistration(model);
                }

                if (model.Message != AppConstants.Common.SUCCESS)
                {
                    ModelState.AddModelError("errormsg", model.Message);
                }

                else
                {
                    if (model.MFile != null)
                    {
                        //UploadFile(model);
                        string Extension = Path.GetExtension(model.MFile.FileName);
                        string FileName = model.CardId + Extension;
                        UploadFiles(model.MFile, Server.MapPath(UrlConstants.LibraryMemberUrl + model.CardId + "/"), FileName);

                    }
                    return RedirectToAction("MemberRegistrationList");
                }
                model.Message = "";
                return View(model);
            }
            model.Message = EduSuiteUIResources.Failed;
            return View(model);
        }

        [HttpPost]
        public ActionResult DeleteMemberRegistration(Int32 Id)
        {
            MemberRegistrationViewModel objModel = new MemberRegistrationViewModel();
            objModel.RowKey = Id;
            try
            {
                objModel = MemberRegistrationService.DeleteMemberRegistration(objModel);
                if (objModel.IsSuccessful == true)
                {
                    string path = Server.MapPath("~/" + objModel.MemberPhotoUrl);
                    if (System.IO.File.Exists(path))
                    {
                        System.IO.File.Delete(path);
                    }
                }
            }
            catch (Exception ex)
            {
                objModel.Message = EduSuiteUIResources.Failed;
            }
            return Json(objModel);

        }

        [HttpGet]
        public JsonResult CheckPhoneExists(string PhoneNo, int RowKey)
        {
            MemberRegistrationViewModel model = new MemberRegistrationViewModel();
            model = MemberRegistrationService.CheckPhoneExists(PhoneNo, RowKey);
            return Json(model.IsSuccessful, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public JsonResult CheckEmailExists(string EmailAddress, int RowKey)
        {
            MemberRegistrationViewModel model = new MemberRegistrationViewModel();
            model = MemberRegistrationService.CheckEmailExists(EmailAddress, RowKey);
            return Json(model.IsSuccessful, JsonRequestBehavior.AllowGet);
        }

        private void UploadFile(MemberRegistrationViewModel model)
        {

            string Extension = Path.GetExtension(model.MFile.FileName);
            string FileName = model.CardId + Extension;
            if (model.MemberPhotoUrl != EduSuiteUIResources.DefaultPhotoUrl)
            {
                string OldPath = Server.MapPath("~/" + model.MemberPhotoUrl);
                if (System.IO.File.Exists(OldPath))
                {
                    System.IO.File.Delete(OldPath);
                }
            }
            var path = Server.MapPath("~/UploadFile/Library/MemberPhoto/" + FileName);
            model.MFile.SaveAs(path);
        }
        private void UploadFiles(HttpPostedFileBase PhotoFile, string FilePath, string FileName)
        {
            if (!Directory.Exists(FilePath))
            {
                Directory.CreateDirectory(FilePath);
            }
            if (PhotoFile != null)
            {
                PhotoFile.SaveAs(FilePath + FileName);
            }
        }


        [HttpGet]
        public JsonResult GetFeesById(byte memberTypeKey)
        {
            MemberRegistrationViewModel model = new MemberRegistrationViewModel();
            model.MemberTypeKey = memberTypeKey;
            model = MemberRegistrationService.GetFeesById(model);
            return Json(model, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public JsonResult CheckIdentificationExists(int IdentificationTypeKey, string IdentificationNumber, int RowKey)
        {
            MemberRegistrationViewModel model = new MemberRegistrationViewModel();
            model.IdentificationNumber = IdentificationNumber;
            model.IdentificationTypeKey = IdentificationTypeKey;
            model.RowKey = RowKey;
            model = MemberRegistrationService.CheckIdentificationExists(model);
            return Json(model.IsSuccessful, JsonRequestBehavior.AllowGet);
        }
    }
}