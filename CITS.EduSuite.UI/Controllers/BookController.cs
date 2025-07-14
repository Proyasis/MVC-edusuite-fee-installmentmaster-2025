using CITS.EduSuite.Business.Models.ViewModels;
using CITS.EduSuite.Business.Interfaces;

using CITS.EduSuite.Business.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using CITS.EduSuite.Business.Models.Resources;

namespace CITS.EduSuite.UI.Controllers
{
    public class BookController : BaseController
    {
        public IBookService bookService;
        private ISharedService sharedService;
        public BookController(IBookService objBookService,
            ISharedService objSharedService)
        {
            this.bookService = objBookService;
        }
        [HttpGet]
        public ActionResult BookList()
        {
            return View();
        }

        [HttpGet]
        public ActionResult AddEditBook(int? CourseId, short? UniversityId, short? AcademicTermKey)
        {
            BookViewModel model = new BookViewModel();
            model.CourseKey = CourseId ?? 0;
            model.UniversityKey = UniversityId ?? 0;
            model.AcademicTermKey = AcademicTermKey ?? 0;
            model = bookService.GetBookById(model);
            if (model == null)
            {
                model = new BookViewModel();
            }
            if (AcademicTermKey != null)
            {
                model.AcademicTermKey = AcademicTermKey ?? 0;
            }
            ViewBag.ShowAcademicTerm = sharedService.ShowAcademicTerm();
            ViewBag.ShowUniversity = sharedService.ShowUniversity();
            return View(model);

        }

        [HttpPost]
        public ActionResult AddEditBook(BookViewModel model)
        {
            if (ModelState.IsValid)
            {

                model = bookService.UpdateBook(model);

                if (model.Message != AppConstants.Common.SUCCESS)
                {
                    ModelState.AddModelError("error_msg", model.Message);
                }
                else
                {
                    return Json(model);
                   
                }
                model.Message = "";
                return Json(model);
            }

            model.Message = EduSuiteUIResources.Failed;
            return View(model);
        }

        [HttpGet]
        public JsonResult GetBook(string SearchText)
        {
            int page = 1; int rows = 15;
            List<BookViewModel> BookList = new List<BookViewModel>();
            BookList = bookService.GetBook(SearchText);
            int pageindex = Convert.ToInt32(page) - 1;
            int pagesize = rows;
            int totalrecords = BookList.Count();
            var totalpage = (int)Math.Ceiling((float)totalrecords / (float)rows);

            var jsonData = new
            {
                total = totalpage,
                page,
                records = totalrecords,
                rows = BookList
            };
            return Json(jsonData, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult DeleteBookAll(int? Coursekey, short? UniversityKey, short? AcademicTermKey)
        {
            BookViewModel model = new BookViewModel();
            model.CourseKey = Coursekey ?? 0;
            model.UniversityKey = UniversityKey ?? 0;
            model.AcademicTermKey = AcademicTermKey ?? 0;
           
            try
            {
                model = bookService.DeleteBookAll(model);
            }
            catch (Exception)
            {
                model.Message = EduSuiteUIResources.Failed;
            }
            return Json(model);
        }

        public ActionResult DeleteBook(long Id)
        {
            BookViewModel model = new BookViewModel();

            try
            {
                model = bookService.DeleteBook(Id);
            }
            catch (Exception)
            {
                model.Message = EduSuiteUIResources.Failed;
            }
            return Json(model);
        }


        [HttpGet]
        public JsonResult CheckBookCodeExist(string BookCode, long? RowKey)
        {
            BookDetailsViewModel model = new BookDetailsViewModel();
            model.BookCode = BookCode;
            model.RowKey = RowKey ?? 0;
            var result = bookService.CheckBookCodeExist(model);
            return Json(result.IsSuccessful, JsonRequestBehavior.AllowGet);
        }

        public JsonResult FillCourse(short key)
        {
            BookViewModel model = new BookViewModel();
            model.CourseTypeKey = key;
            model = bookService.FillCourse(model);
            return Json(model.Courses, JsonRequestBehavior.AllowGet);
        }

        public JsonResult FillUniversity(long key, short AcademicTermKey)
        {
            BookViewModel model = new BookViewModel();
            model.AcademicTermKey = AcademicTermKey;
            model.CourseKey = key;
            model = bookService.FillUniversity(model);
            return Json(model.Universities, JsonRequestBehavior.AllowGet);
        }

    }
}