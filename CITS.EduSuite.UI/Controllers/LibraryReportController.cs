using CITS.EduSuite.Business.Interfaces;
using CITS.EduSuite.Business.Models.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace CITS.EduSuite.UI.Controllers
{
    public class LibraryReportController : BaseController
    {
        private ILibraryReportService LibraryReportService;
        public LibraryReportController(ILibraryReportService objLibraryReportService)
        {
            this.LibraryReportService = objLibraryReportService;
        }


        #region Book Summary
        [ActionAuthenticationAttribute(MenuCode = MenuConstants.LibraryBookSummary, ActionCode = ActionConstants.MenuAccess)]
        public ActionResult LibraryBookSummaryList()
        {
            LibraryReportViewModel model = new LibraryReportViewModel();
            LibraryReportService.FillDropdownLists(model);
            return View(model);
        }

        [HttpPost]
        public JsonResult GetLibraryBookSummary(LibraryReportViewModel model)
        {
            //int page = 1; int rows = 15;
            List<LibraryReportViewModel> LibraryBookSummaryList = new List<LibraryReportViewModel>();
            LibraryBookSummaryList = LibraryReportService.GetBookSummary(model);
            int pageindex = Convert.ToInt32(model.page) - 1;
            int pagesize = model.rows ?? 0;
            long totalrecords = model.TotalRecords;
            var totalpage = (int)Math.Ceiling((float)totalrecords / (float)model.rows);

            var jsonData = new
            {
                total = totalpage,
                model.page,
                records = totalrecords,
                rows = LibraryBookSummaryList
            };
            return Json(jsonData, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public JsonResult GetBookCopies(Int32 id)
        {

            BookCopyViewModel objViewModel = new BookCopyViewModel();
            objViewModel.BookKey = Convert.ToInt64(id);

            var BookCopyDetails = LibraryReportService.BindBookCopies(objViewModel);
            var jsonData = new
            {
                rows = BookCopyDetails
            };
            return Json(jsonData, JsonRequestBehavior.AllowGet);

        }
        #endregion Book Summary


        #region MemberPlan Summary 

        [ActionAuthenticationAttribute(MenuCode = MenuConstants.LibraryMemberDetailsSummary, ActionCode = ActionConstants.MenuAccess)]
        public ActionResult MemberPlanSummaryList()
        {
            LibraryReportViewModel model = new LibraryReportViewModel();
            LibraryReportService.FillMemberplanDropDownList(model);
            return View(model);
        }

        [HttpPost]
        public JsonResult GetMemberPlanSummary(LibraryReportViewModel model)
        {
            if (model.ApplicationTypeKey != 0 && model.ApplicationTypeKey != null)
            {
                model.ApplicationTypeKeys.Add(Convert.ToInt64(model.ApplicationTypeKey));
            }
            List<LibraryReportViewModel> MemberplanSummaryList = new List<LibraryReportViewModel>();
            MemberplanSummaryList = LibraryReportService.GetMemberPlanSummary(model);
            int pageindex = Convert.ToInt32(model.page) - 1;
            int pagesize = model.rows ?? 0;
            long totalrecords = model.TotalRecords;
            var totalpage = (int)Math.Ceiling((float)totalrecords / (float)model.rows);

            var jsonData = new
            {
                total = totalpage,
                model.page,
                records = totalrecords,
                rows = MemberplanSummaryList
            };
            return Json(jsonData, JsonRequestBehavior.AllowGet);
        }

        public ActionResult MemberDetails(long? RowKey)
        {
            LibraryReportViewModel model = new LibraryReportViewModel();
            model.RowKey = RowKey ?? 0;
            model = LibraryReportService.GetLibraryBookDetails(model);
            return PartialView(model);
        }

        #endregion MemberPlan Summary 



        #region Book Issue Summary 

        [ActionAuthenticationAttribute(MenuCode = MenuConstants.LibraryBookIssueSummary, ActionCode = ActionConstants.MenuAccess)]
        public ActionResult LibraryBookIssueSummaryList()
        {
            LibraryReportViewModel model = new LibraryReportViewModel();
            LibraryReportService.FillMemberplanDropDownList(model);
            LibraryReportService.FillDropdownLists(model);
            LibraryReportService.FillBookDrownList(model);
            return View(model);
        }

        [HttpPost]
        public JsonResult GetBookIssueSummary(LibraryReportViewModel model)
        {
            if (model.ApplicationTypeKey != 0 && model.ApplicationTypeKey != null)
            {
                model.ApplicationTypeKeys.Add(Convert.ToInt64(model.ApplicationTypeKey));
            }
            List<LibraryReportViewModel> BookIssueSummaryList = new List<LibraryReportViewModel>();
            BookIssueSummaryList = LibraryReportService.GetBookIssueSummary(model);
            int pageindex = Convert.ToInt32(model.page) - 1;
            int pagesize = model.rows ?? 0;
            long totalrecords = model.TotalRecords;
            var totalpage = (int)Math.Ceiling((float)totalrecords / (float)model.rows);

            var jsonData = new
            {
                total = totalpage,
                model.page,
                records = totalrecords,
                rows = BookIssueSummaryList
            };
            return Json(jsonData, JsonRequestBehavior.AllowGet);
        }

        #endregion Book Issue Summary 



        public JsonResult FIllRack(LibraryReportViewModel model)
        {
            model = LibraryReportService.FillRacks(model);
            return Json(model.Racks, JsonRequestBehavior.AllowGet);
        }
        public JsonResult FIllSubRack(LibraryReportViewModel model)
        {
            model = LibraryReportService.FillSubRack(model);
            return Json(model.SubRacks, JsonRequestBehavior.AllowGet);
        }

        public JsonResult FIllLibraryBooks(LibraryReportViewModel model)
        {
            model = LibraryReportService.FillLibraryBooks(model);
            return Json(model.LibraryBooks, JsonRequestBehavior.AllowGet);
        }

        public JsonResult FIllLibraryBookCopies(LibraryReportViewModel model)
        {
            model = LibraryReportService.FillLibraryBooksCopies(model);
            return Json(model.LibraryBookCopies, JsonRequestBehavior.AllowGet);
        }
    }
}