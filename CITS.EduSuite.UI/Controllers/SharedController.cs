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
using CITS.EduSuite.Business.Models.Common;
using CITS.EduSuite.Business.Common;
using CITS.EduSuite.Business.Models.Resources;
using System.IO;
using System.Text.RegularExpressions;

namespace CITS.EduSuite.UI.Controllers
{
    public class SharedController : BaseController
    {
        private ISharedService sharedService;
        private IDocumentTrackService documentTrackService;

        private IEnquiryScheduleService enquirySchedulesService;
        public SharedController(ISharedService objSharedService, IEnquiryScheduleService ObjenquirySchedulesService, IDocumentTrackService objDocumentTrackService)
        {
            this.sharedService = objSharedService;
            this.enquirySchedulesService = ObjenquirySchedulesService;
            this.documentTrackService = objDocumentTrackService;
        }
        public ActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public JsonResult GetBranches()
        {
            var result = sharedService.GetBranches();
            foreach (SelectListModel model in result)
            {
                if (model.RowKey == GlobalVariables.Defaults.DefaultBranch)
                {
                    model.Selected = true;
                }
            }
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        public ActionResult MenuList()
        {
            MenuViewModel model = new MenuViewModel();

            List<MenuViewModel> modelList = new List<MenuViewModel>();
            modelList = sharedService.GetMenuByUserId(model);
            return PartialView(modelList);
        }
        [HttpGet]
        public ActionResult StudentDetail(long id, string AdmissionNo)
        {
            MenuViewModel model = new MenuViewModel();
            //GetUserKey(model);
            ApplicationPersonalViewModel application = new ApplicationPersonalViewModel();
            application = sharedService.FillApplicationDetails(AdmissionNo, id);
            if (application == null)
            {
                application = new ApplicationPersonalViewModel();
                if (model.Message != AppConstants.Common.SUCCESS)
                {
                    ModelState.AddModelError("error_msg", model.Message);
                }
            }
            return PartialView(application);
        }
        [HttpGet]
        public ActionResult EmployeeDetail(long id, string EmployeeCode)
        {
            MenuViewModel model = new MenuViewModel();
            //GetUserKey(model);
            EmployeePersonalViewModel employee = new EmployeePersonalViewModel();
            employee = sharedService.FillEmployeeDetails(EmployeeCode, id);
            if (employee == null)
            {
                employee = new EmployeePersonalViewModel();
                if (model.Message != AppConstants.Common.SUCCESS)
                {
                    ModelState.AddModelError("error_msg", model.Message);
                }
            }
            return PartialView(employee);
        }

        [HttpGet]
        public ActionResult LibraryDetail(long id, string CardId)
        {
            MenuViewModel model = new MenuViewModel();
            //GetUserKey(model);
            MemberPlanDetailsViewModel Library = new MemberPlanDetailsViewModel();
            Library = sharedService.FillLibraryDetails(CardId, id);
            if (Library == null)
            {
                Library = new MemberPlanDetailsViewModel();
                if (model.Message != AppConstants.Common.SUCCESS)
                {
                    ModelState.AddModelError("error_msg", model.Message);
                }
            }
            return PartialView(Library);
        }

        [HttpPost]
        public void SetDefaultBranch(short Id)
        {
            GlobalVariables.Defaults.DefaultBranch = Id;

        }

        [HttpPost]
        public string EncryptQueryString(string query)
        {
            return "?q=" + CryptographicHelper.Encryptor(query, DbConstants.EncryptionKey);

        }

        public ActionResult EnquiryFeedbacksList()
        {
            EnquiryScheduleViewModel model = new EnquiryScheduleViewModel();
            enquirySchedulesService.FillTelephoneCodes(model);
            return PartialView(model);
        }

        //[HttpGet]
        //public JsonResult GetBakup(string DBName, string Location)
        //{
        //    return Json(sharedService.GetBakup(DBName, Location), JsonRequestBehavior.AllowGet);
        //}

        public ActionResult GetBackup()
        {
            return PartialView();
        }

        [HttpPost]
        public ActionResult GetBakup(string DBName, string Location)
        {
            return Json(sharedService.GetBakup(DBName, Location), JsonRequestBehavior.AllowGet);
        }
        [HttpGet]
        public JsonResult GetBalance(short PaymentModeKey, long? BankAccountKey, short? branchKey)
        {
            decimal Balance = sharedService.CheckShortBalance(PaymentModeKey, BankAccountKey ?? 0, branchKey ?? 0);
            return Json(Balance, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult ViewFile(DocumentTrackViewModel model)
        {
            DocumentTrackViewModel objviewModel = new DocumentTrackViewModel();

            objviewModel = documentTrackService.CreateDocumentTrack(model);

            if (!model.IfDownload)
            {
                ViewBag.FullPath = model.FilePath.Substring(1);
                return PartialView(model);
            }
            else
            {
                return Json(model);
            }
        }

        public ActionResult DocumentTrackList(DocumentTrackViewModel model)
        {

            return PartialView(model);
        }

        [HttpGet]
        public JsonResult GetDocumentTrackList(string SearchText, short? DocumentType, long? RowDataKey, string sidx, string sord, int page, int rows)
        {
            long TotalRecords = 0;
            List<DocumentTrackViewModel> applicationList = new List<DocumentTrackViewModel>();
            DocumentTrackViewModel objViewModel = new DocumentTrackViewModel();

            objViewModel.SearchText = SearchText;
            objViewModel.DocumentType = DocumentType ?? 0;
            objViewModel.RowDataKey = RowDataKey ?? 0;


            objViewModel.PageIndex = page;
            objViewModel.PageSize = rows;
            objViewModel.SortBy = sidx;
            objViewModel.SortOrder = sord;

            applicationList = documentTrackService.GetDocumentTrackList(objViewModel, out TotalRecords);

            int pageIndex = Convert.ToInt32(page) - 1;
            int pageSize = rows;
            var totalPages = (int)Math.Ceiling((float)TotalRecords / (float)rows);

            var jsonData = new
            {
                total = totalPages,
                page,
                records = TotalRecords,
                rows = applicationList
            };
            return Json(jsonData, JsonRequestBehavior.AllowGet);
        }

        //[HttpPost]
        //public FileContentResult DownloadFile(DocumentTrackViewModel model)
        //{
        //    documentTrackService.CreateDocumentTrack(model);
        //    string FullPath = Server.MapPath(model.FilePath);
        //    return File(System.IO.File.ReadAllBytes(FullPath), System.Web.MimeMapping.GetMimeMapping(FullPath), EduSuiteUIResources.Document + "_" + Path.GetFileName(model.FilePath));
        //}
    }
}