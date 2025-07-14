using CITS.EduSuite.Business.Models.ViewModels;
using CITS.EduSuite.Business.Interfaces;
using CITS.EduSuite.Business.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;




namespace CITS.EduSuite.UI.Controllers
{
    public class BookIssueSummaryReportController : BaseController
    {
        public IBookIssueSummaryReportService bookIssueSummaryReportService;

        public BookIssueSummaryReportController(IBookIssueSummaryReportService ObjBookIssueSummaryReportService)
        {
            this.bookIssueSummaryReportService = ObjBookIssueSummaryReportService;
        }

        public ActionResult BookIssueSummaryReportList()
        {
            BookIssueSummaryReportViewModel model = new BookIssueSummaryReportViewModel();
            return View(model);
        }



        [HttpPost]
        public JsonResult GetBookIssueSummaryReport(BookIssueSummaryReportViewModel model)
        {
            //int page = 1; int rows = 15;
            List<BookIssueSummaryReportViewModel> StudentSummaryReportList = new List<BookIssueSummaryReportViewModel>();
            StudentSummaryReportList = bookIssueSummaryReportService.GetBookIssueSummaryReport(model);
            int pageindex = Convert.ToInt32(model.page) - 1;
            int pagesize = model.rows ?? 0;
            int totalrecords = model.TotalRecords ?? 0;
            var totalpage = (int)Math.Ceiling((float)totalrecords / (float)model.rows);
            var jsonData = new
            {
                total = totalpage,
                model.page,
                records = totalrecords,
                rows = StudentSummaryReportList
            };
            return Json(jsonData, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public JsonResult GetBookDetailsById(int id)
        {
            BookIssueSummaryReportViewModel model = new BookIssueSummaryReportViewModel();
            //model.RowKey = id;
            List<BookDetails> BookDetailsList = new List<BookDetails>();
            BookDetailsList = bookIssueSummaryReportService.GetBookDetailsById(model);
            int pageindex = 1;
            int pagesize = BookDetailsList.Count;
            int totalrecords = BookDetailsList.Count;
            // var totalpage = (int)Math.Ceiling((float)totalrecords / (float)totalrecords);
            var jsonData = new
            {
                total = 1,
                pageindex,
                records = totalrecords,
                rows = BookDetailsList
            };
            return Json(jsonData, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult GetExportStudentsSummaryReport(BookIssueSummaryReportViewModel model)
        {
            List<BookIssueSummaryReportViewModel> StudentSummaryReportList = new List<BookIssueSummaryReportViewModel>();
            StudentSummaryReportList = bookIssueSummaryReportService.GetBookIssueSummaryReport(model);
            return Json(StudentSummaryReportList, JsonRequestBehavior.AllowGet);
        }


    }
}