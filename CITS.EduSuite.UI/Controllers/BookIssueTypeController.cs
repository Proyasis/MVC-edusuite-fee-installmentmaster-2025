using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using CITS.EduSuite.Business.Common;
using CITS.EduSuite.Business.Interfaces;
using CITS.EduSuite.Business.Models.Resources;
using CITS.EduSuite.Business.Models.ViewModels;


namespace CITS.EduSuite.UI.Controllers
{
    public class BookIssueTypeController : BaseController
    {
        private IBookIssueTypeService bookIssueTypeService;

        public BookIssueTypeController(IBookIssueTypeService objBookIssueTypeService)
        {
            bookIssueTypeService = objBookIssueTypeService;
        }
        [HttpGet]
        public ActionResult BookIssueTypeList()
        {
            return View();
        }

        [HttpGet]
        public JsonResult GetBookIssueTypes(string searchText)
        {
            int page = 1, rows = 10;

            List<BookIssueTypeViewModel> bookIssueTypesList = new List<BookIssueTypeViewModel>();
            bookIssueTypesList = bookIssueTypeService.GetBookIssueTypes(searchText);

            int pageIndex = Convert.ToInt32(page) - 1;
            int pageSize = rows;
            int totalRecords = bookIssueTypesList.Count();
            var totalPages = (int)Math.Ceiling((float)totalRecords / (float)rows);

            var jsonData = new
            {
                total = totalPages,
                page,
                records = totalRecords,
                rows = bookIssueTypesList
            };
            return Json(jsonData, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public ActionResult AddEditBookIssueType(byte? id)
        {
            var bookIssueTypes = bookIssueTypeService.GetBookIssueTypeById(id);
            if (bookIssueTypes == null)
            {
                bookIssueTypes = new BookIssueTypeViewModel();
            }
            return PartialView(bookIssueTypes);
        }

        [HttpPost]
        public ActionResult AddEditBookIssueType(BookIssueTypeViewModel bookIssueType)
        {

            if (ModelState.IsValid)
            {
                if (bookIssueType.RowKey == 0)
                {
                    bookIssueType = bookIssueTypeService.CreateBookIssueType(bookIssueType);
                }
                else
                {
                    bookIssueType = bookIssueTypeService.UpdateBookIssueType(bookIssueType);
                }

                if (bookIssueType.Message != AppConstants.Common.SUCCESS)
                {
                    ModelState.AddModelError("error_msg", bookIssueType.Message);
                }
                else
                {
                    return Json(bookIssueType);
                }

                bookIssueType.Message = "";
                return PartialView(bookIssueType);
            }

            bookIssueType.Message = EduSuiteUIResources.Failed;
            return PartialView(bookIssueType);

        }

        [HttpPost]
        public ActionResult DeleteBookIssueType(byte id)
        {
            BookIssueTypeViewModel objViewModel = new BookIssueTypeViewModel();

            objViewModel.RowKey = id;
            try
            {
                objViewModel = bookIssueTypeService.DeleteBookIssueType(objViewModel);
            }
            catch (Exception)
            {
                objViewModel.Message = EduSuiteUIResources.Failed;
            }
            return Json(objViewModel);
        }
    }
}