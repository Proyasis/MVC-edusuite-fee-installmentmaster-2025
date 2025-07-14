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
    public class BookCopyController : BaseController
    {
        private IBookCopyService bookCopyService;

        public BookCopyController(IBookCopyService objBookCopyService)
        {
            bookCopyService = objBookCopyService;
        }
      
        public ActionResult BookCopyList(long id)
        {
            BookCopyViewModel objViewModel = new BookCopyViewModel();
            objViewModel.BookKey = id;
            return PartialView(objViewModel);
        }

      
        public JsonResult GetBookCopies(long BookId)
        {
            int page = 1, rows = 10;

            List<BookCopyViewModel> bookCopiesList = new List<BookCopyViewModel>();
            bookCopiesList = bookCopyService.GetBookCopies(BookId);

            int pageIndex = Convert.ToInt32(page) - 1;
            int pageSize = rows;
            int totalRecords = bookCopiesList.Count();
            var totalPages = (int)Math.Ceiling((float)totalRecords / (float)rows);

            var jsonData = new
            {
                total = totalPages,
                page,
                records = totalRecords,
                rows = bookCopiesList
            };
            return Json(jsonData, JsonRequestBehavior.AllowGet);
        }


        public ActionResult AddEditBookCopy(long? id,long BookId)
        {
            var bookCopies = bookCopyService.GetBookCopyById(id);
            bookCopies.BookKey = BookId;
            if (bookCopies == null)
            {
                bookCopies = new BookCopyViewModel();
            }
            return PartialView(bookCopies);
        }

        [HttpPost]
        public ActionResult AddEditBookCopy(BookCopyViewModel bookCopy)
        {

            if (ModelState.IsValid)
            {
                if (bookCopy.RowKey == 0)
                {
                    bookCopy = bookCopyService.CreateBookCopy(bookCopy);
                }
                else
                {
                    bookCopy = bookCopyService.UpdateBookCopy(bookCopy);
                }

                if (bookCopy.Message != AppConstants.Common.SUCCESS)
                {
                    ModelState.AddModelError("error_msg", bookCopy.Message);
                }
                else
                {
                    return Json(bookCopy);
                }

                bookCopy.Message = "";
                return PartialView(bookCopy);
            }

            bookCopy.Message = EduSuiteUIResources.Failed;
            return PartialView(bookCopy);

        }

        [HttpPost]
        public ActionResult DeleteBookCopy(Int64 id)
        {
            BookCopyViewModel objViewModel = new BookCopyViewModel();

            objViewModel.RowKey = id;
            try
            {
                objViewModel = bookCopyService.DeleteBookCopy(objViewModel);
            }
            catch (Exception)
            {
                objViewModel.Message = EduSuiteUIResources.Failed;
            }
            return Json(objViewModel);
        }

        [HttpPost]
        public JsonResult GetBooksById(List<long> BookIds)
        {
            if (BookIds == null)
            {
                BookIds = new List<long>();
            }
            List<BookCopyViewModel> model = bookCopyService.GetBooksById(BookIds);
            return Json(model, JsonRequestBehavior.AllowGet);
        }
    }
}