
using CITS.EduSuite.Business.Common;
using CITS.EduSuite.Business.Interfaces;
using CITS.EduSuite.Business.Models.Resources;
using CITS.EduSuite.Business.Models.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace CITS.EduSuite.UI.Controllers
{
    public class BookCategoryController : BaseController
    {
        public IBookCategoryService bookCategoryService;

        public BookCategoryController(IBookCategoryService objBookCategoryService)
        {
            this.bookCategoryService = objBookCategoryService;
        }

        [HttpGet]
        public ActionResult BookCategoryList()
        {
            return View();
        }

        [HttpGet]
        public JsonResult GetBookCategory(string searchText)
        {
            int page = 1, rows = 10;

            List<BookCategoryViewModel> bookCategoryList = new List<BookCategoryViewModel>();
            bookCategoryList = bookCategoryService.GetBookCategory(searchText);

            int pageIndex = Convert.ToInt32(page) - 1;
            int pageSize = rows;
            int totalRecords = bookCategoryList.Count();
            var totalPages = (int)Math.Ceiling((float)totalRecords / (float)rows);

            var jsonData = new
            {
                total = totalPages,
                page,
                records = totalRecords,
                rows = bookCategoryList
            };
            return Json(jsonData, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public ActionResult AddEditBookCategory(short? id)
        {
            var bookCategories = bookCategoryService.GetBookCategoryById(id);
            if (bookCategories == null)
            {
                bookCategories = new BookCategoryViewModel();
            }
            return PartialView(bookCategories);
        }

        [HttpPost]
        public ActionResult AddEditBookCategory(BookCategoryViewModel bookCategory)
        {
            if (ModelState.IsValid)
            {
                if (bookCategory.RowKey == 0)
                {
                    bookCategory = bookCategoryService.CreateBookCategory(bookCategory);
                }
                else
                {
                    bookCategory = bookCategoryService.UpdateBookCategory(bookCategory);
                }

                if (bookCategory.Message != AppConstants.Common.SUCCESS)
                {
                    ModelState.AddModelError("error_msg", bookCategory.Message);
                }
                else
                {
                    return Json(bookCategory);
                }

                bookCategory.Message = "";
                return PartialView(bookCategory);
            }

            bookCategory.Message = EduSuiteUIResources.Failed;
            return PartialView(bookCategory);
        }

        [HttpPost]
        public ActionResult DeleteBookCategory(short id)
        {
            BookCategoryViewModel objViewModel = new BookCategoryViewModel();

            objViewModel.RowKey = id;
            try
            {
                objViewModel = bookCategoryService.DeleteBookCategory(objViewModel);
            }
            catch (Exception)
            {
                objViewModel.Message = EduSuiteUIResources.Failed;
            }
            return Json(objViewModel);
        }
    }
}