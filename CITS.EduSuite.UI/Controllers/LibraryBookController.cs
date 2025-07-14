using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using CITS.EduSuite.Business.Common;
using CITS.EduSuite.Business.Interfaces;
using CITS.EduSuite.Business.Models.Resources;
using CITS.EduSuite.Business.Models.ViewModels;


namespace CITS.EduSuite.UI.Controllers
{
    public class LibraryBookController : BaseController
    {
        private ILibraryBookService LibrarybookService;

        public LibraryBookController(ILibraryBookService objBookService)
        {
            LibrarybookService = objBookService;
        }
        [HttpGet]
        public ActionResult LibraryBookList()
        {
            LibraryBookViewModel model = new LibraryBookViewModel();
            LibrarybookService.FillBranch(model);
            return View(model);
        }
               
        public JsonResult GetLibraryBooks(string searchText, short? BranchKey, string sidx, string sord, int page, int rows)
        {
            long TotalRecords = 0;

            List<LibraryBookViewModel> booksList = new List<LibraryBookViewModel>();

            LibraryBookViewModel objViewModel = new LibraryBookViewModel();

            objViewModel.SearchText = searchText;
            objViewModel.BranchKey = BranchKey ?? 0;
            objViewModel.PageIndex = page;
            objViewModel.PageSize = rows;
            objViewModel.SortBy = sidx;
            objViewModel.SortOrder = sord;
            booksList = LibrarybookService.GetLibraryBooks(objViewModel, out TotalRecords);

            int pageIndex = Convert.ToInt32(page) - 1;
            int pageSize = rows;
           
            var totalPages = (int)Math.Ceiling((float)TotalRecords / (float)rows);

            var jsonData = new
            {
                total = totalPages,
                page,
                records = TotalRecords,
                rows = booksList
            };

            return Json(jsonData, JsonRequestBehavior.AllowGet);
        }


        [HttpGet]
        public ActionResult AddEditLibraryBook(long? id)
        {
            var books = LibrarybookService.GetLibraryBookById(id);
            if (books == null)
            {
                books = new LibraryBookViewModel();
            }
            return View(books);
        }

        [HttpPost]
        public ActionResult AddEditLibraryBook(LibraryBookViewModel book)
        {

            if (ModelState.IsValid)
            {
                if (book.RowKey == 0)
                {
                    book = LibrarybookService.CreateLibraryBook(book);
                }
                else
                {
                    book = LibrarybookService.UpdateLibraryBook(book);
                }

                if (book.Message != AppConstants.Common.SUCCESS)
                {
                    ModelState.AddModelError("error_msg", book.Message);
                }
                else
                {
                    if (book.PhotoFile != null)
                    {
                        //UploadFile(model);
                        string Extension = Path.GetExtension(book.PhotoFile.FileName);
                        string FileName = book.BookName + Extension;
                        UploadFiles(book.PhotoFile, Server.MapPath(UrlConstants.LibraryBooksUrl  + "/"), FileName);
                    }
                    return RedirectToAction("AddEditLibraryBook/" + book.RowKey);
                }

                book.Message = "";
                return View(book);
            }

            book.Message = EduSuiteUIResources.Failed;
            return View(book);

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

        [HttpPost]
        public ActionResult DeleteLibraryBook(long id)
        {
            LibraryBookViewModel objViewModel = new LibraryBookViewModel();

            objViewModel.RowKey = id;
            try
            {
                objViewModel = LibrarybookService.DeleteLibraryBook(objViewModel);
                if (objViewModel.IsSuccessful == true)
                {
                    string path = Server.MapPath("~/" + objViewModel.CoverPhotoPath);
                    if (System.IO.File.Exists(path))
                    {
                        System.IO.File.Delete(path);
                    }
                }
            }
            catch (Exception)
            {
                objViewModel.Message = EduSuiteUIResources.Failed;
            }
            return Json(objViewModel);
        }
        [HttpGet]
        public JsonResult FillSubRack(int? RackKey)
        {
            LibraryBookViewModel model = new LibraryBookViewModel();
            model.RackKey = RackKey ?? 0;
            model = LibrarybookService.FillSubRack(model);
            if (model == null)
            {
                model = new LibraryBookViewModel();
            }
            return Json(model.SubRacks, JsonRequestBehavior.AllowGet);
        }
        [HttpGet]
        public JsonResult FillRack(short? BranchKey)
        {
            LibraryBookViewModel model = new LibraryBookViewModel();
            model.BranchKey = BranchKey ?? 0;
            model = LibrarybookService.FillRacks(model);
            if (model == null)
            {
                model = new LibraryBookViewModel();
            }
            return Json(model.Racks, JsonRequestBehavior.AllowGet);
        }
    }
}