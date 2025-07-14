using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using CITS.EduSuite.Business.Common;
using CITS.EduSuite.Business.Interfaces;
using CITS.EduSuite.Business.Models.Resources;
using CITS.EduSuite.Business.Models.ViewModels;
using CITS.EduSuite.Business.Services;


namespace CITS.EduSuite.UI.Controllers
{
    public class BookStatusController : BaseController
    {
        private IBookStatusService bookStatusService;

        public BookStatusController(IBookStatusService objBookStatusService)
        {
            this.bookStatusService = objBookStatusService;

        }

        [HttpGet]

        public ActionResult BookStatusList()
        {
            return View();
        }

        [HttpGet]

        public JsonResult GetBookStatus(string searchText)
        {
            int page = 1, rows = 10;

            List<BookStatusViewModel> bookStatusList = new List<BookStatusViewModel>();
            bookStatusList = bookStatusService.GetBookStatus(searchText);

            int pageIndex = Convert.ToInt32(page) - 1;
            int pageSize = rows;
            int totalRecords = bookStatusList.Count();
            var totalPages = (int)Math.Ceiling((float)totalRecords / (float)rows);

            var jsonData = new
            {
                total = totalPages,
                page,
                records = totalRecords,
                rows = bookStatusList
            };
            return Json(jsonData, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]

        public ActionResult AddEditBookStatus(byte? id)
        {
            var bookstatus = bookStatusService.GetBookStatusById(id);
            if (bookstatus == null)
            {
                bookstatus = new BookStatusViewModel();
            }
            return PartialView(bookstatus);
        }

        [HttpPost]

        public ActionResult AddEditBookStatus(BookStatusViewModel BookStatus)
        {
            if (ModelState.IsValid)
            {
                if (BookStatus.RowKey == 0)
                {
                    BookStatus = bookStatusService.CreateBookStatus(BookStatus);
                }
                else
                {
                    BookStatus = bookStatusService.UpdateBooKStatus(BookStatus);
                }

                if (BookStatus.Message != AppConstants.Common.SUCCESS)
                {
                    ModelState.AddModelError("error_msg", BookStatus.Message);
                }
                else
                {
                    return Json(BookStatus);
                }

                BookStatus.Message = "";
                return PartialView(BookStatus);
            }

            BookStatus.Message = EduSuiteUIResources.Failed;
            return PartialView(BookStatus);
        }

        [HttpPost]

        public ActionResult DeleteBookStatus(byte id)
        {
            BookStatusViewModel objViewModel = new BookStatusViewModel();

            objViewModel.RowKey = id;
            try
            {
                objViewModel = bookStatusService.DeleteBookStatus(objViewModel);
            }
            catch (Exception)
            {
                objViewModel.Message = EduSuiteUIResources.Failed;
            }
            return Json(objViewModel);
        }
    }
}