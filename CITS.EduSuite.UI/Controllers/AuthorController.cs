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
    public class AuthorController : BaseController
    {
        private IAuthorService authorService;

        public AuthorController(IAuthorService objAuthorService)
        {
            authorService = objAuthorService;
        }

        [HttpGet]
        public ActionResult AuthorList()
        {
            return View();
        }

        
        public JsonResult GetAuthors(string searchText, string sidx, string sord, int page, int rows)
        {
            long TotalRecords = 0;

            List<AuthorViewModel> authorsList = new List<AuthorViewModel>();

            AuthorViewModel objViewModel = new AuthorViewModel();

            //
            objViewModel.SearchText = searchText;
            objViewModel.PageIndex = page;
            objViewModel.PageSize = rows;
            objViewModel.SortBy = sidx;
            objViewModel.SortOrder = sord;
            authorsList = authorService.GetAuthors(objViewModel, out TotalRecords);

            int pageIndex = Convert.ToInt32(page) - 1;
            int pageSize = rows;
            int totalRecords = authorsList.Count();
            var totalPages = (int)Math.Ceiling((float)totalRecords / (float)rows);

            var jsonData = new
            {
                total = totalPages,
                page,
                records = totalRecords,
                rows = authorsList
            };
            return Json(jsonData, JsonRequestBehavior.AllowGet);


        }

        [HttpGet]
        public ActionResult AddEditAuthor(int? id)
        {
            var authors = authorService.GetAuthorById(id);
            if (authors == null)
            {
                authors = new AuthorViewModel();
            }
            return PartialView(authors);
        }

        [HttpPost]
        public ActionResult AddEditAuthor(AuthorViewModel author)
        {

            if (ModelState.IsValid)
            {
                if (author.RowKey == 0)
                {
                    author = authorService.CreateAuthor(author);
                }
                else
                {
                    author = authorService.UpdateAuthor(author);
                }

                if (author.Message != AppConstants.Common.SUCCESS)
                {
                    ModelState.AddModelError("error_msg", author.Message);
                }
                else
                {
                    return Json(author);
                }

                author.Message = "";
                return PartialView(author);
            }

            author.Message = EduSuiteUIResources.Failed;
            return PartialView(author);

        }

        [HttpPost]
        public ActionResult DeleteAuthor(Int16 id)
        {
            AuthorViewModel objViewModel = new AuthorViewModel();

            objViewModel.RowKey = id;
            try
            {
                objViewModel = authorService.DeleteAuthor(objViewModel);
            }
            catch (Exception)
            {
                objViewModel.Message = EduSuiteUIResources.Failed;
            }
            return Json(objViewModel);
        }
	}
}