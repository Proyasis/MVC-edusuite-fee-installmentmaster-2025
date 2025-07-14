
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
    public class BookIssueReturnController : BaseController
    {
        // GET: BookIssueReturn
        private IBookIssueReturnService BookIssueReturnService;
        private ISharedService sharedService;
        public BookIssueReturnController(IBookIssueReturnService objBookIssueReturnService, ISharedService objsharedService)
        {
            BookIssueReturnService = objBookIssueReturnService;
            sharedService = objsharedService;
        }

        public ActionResult BookIssueReturnList()
        {
            BookIssueReturnMasterViewModel model = new BookIssueReturnMasterViewModel();
            BookIssueReturnService.FillDropdownLists(model);
            return View(model);
        }

        public ActionResult BookIssueList(Int32? Id)
        {
            BookIssueReturnMasterViewModel model = new BookIssueReturnMasterViewModel();
            model.RowKey = Convert.ToInt32(Id);
            model = BookIssueReturnService.GetBookIssueReturnById(model);
            if (model == null)
            {
                model = new BookIssueReturnMasterViewModel();
            }
            return View(model);
        }

        public JsonResult GetBookIssueReturn(string searchText, short? BranchKey, byte? ApplicationTypeKey, string sidx, string sord, int page, int rows)
        {
            long TotalRecords = 0;

            List<BookIssueReturnMasterViewModel> BookIssueReturnList = new List<BookIssueReturnMasterViewModel>();

            BookIssueReturnMasterViewModel objViewModel = new BookIssueReturnMasterViewModel();

            //
            objViewModel.SearchText = searchText;
            objViewModel.BranchKey = BranchKey ?? 0;
            objViewModel.ApplicationTypeKey = ApplicationTypeKey ?? 0;
            objViewModel.PageIndex = page;
            objViewModel.PageSize = rows;
            objViewModel.SortBy = sidx;
            objViewModel.SortOrder = sord;
            BookIssueReturnList = BookIssueReturnService.GetBookIssueReturn(objViewModel, out TotalRecords);

            int pageIndex = Convert.ToInt32(page) - 1;
            int pageSize = rows;
            int totalRecords = BookIssueReturnList.Count();
            var totalPages = (int)Math.Ceiling((float)totalRecords / (float)rows);

            var jsonData = new
            {
                total = totalPages,
                page,
                records = totalRecords,
                rows = BookIssueReturnList
            };
            return Json(jsonData, JsonRequestBehavior.AllowGet);


        }

        [HttpGet]
        public ActionResult AddEditBookIssue(Int32? Id, string CardId)
        {
            BookIssueReturnMasterViewModel model = new BookIssueReturnMasterViewModel();
            model.CardId = CardId;
            if (model.CardId != null || model.CardId == "")
            {
                model.CardId = sharedService.GetMemberPlanKeyByCardId(CardId);
            }
            if (model.CardId == null || model.CardId == "")
            {
                ModelState.AddModelError("error_msg", EduSuiteUIResources.CardId_CannotFind);
            }


            model.RowKey = Convert.ToInt32(Id);

            model = BookIssueReturnService.GetBookIssueReturnById(model);

            return PartialView(model);
        }

        [HttpGet]
        public ActionResult AddEditBookReturn(Int32? Id)
        {
            BookIssueReturnMasterViewModel model = new BookIssueReturnMasterViewModel();
            model.RowKey = Convert.ToInt32(Id);
            model = BookIssueReturnService.GetBookIssueReturnById(model);
            if (model == null)
            {
                model = new BookIssueReturnMasterViewModel();
            }
            return PartialView(model);
        }

        [HttpPost]
        public JsonResult AddEditBookIssue(BookIssueReturnMasterViewModel model)
        {
            if (ModelState.IsValid)
            {
                if (Convert.ToInt32(model.RowKey) != 0)
                {
                    model = BookIssueReturnService.UpdateBookIssue(model);
                }
                else
                {
                    model = BookIssueReturnService.CreateBookIssue(model);
                }
                if (model.IsSuccessful == false)
                {
                    ModelState.AddModelError("error_msg", model.Message);
                }
                else
                {
                    return Json(model);
                }

                return Json(model, JsonRequestBehavior.AllowGet);
            }
            foreach (ModelState state in ViewData.ModelState.Values.Where(x => x.Errors.Count > 0))
            {
            }

            model.Message = EduSuiteUIResources.Failed;
            return Json(model, JsonRequestBehavior.AllowGet);

        }

        [HttpPost]
        public JsonResult AddEditBookReturn(BookIssueReturnMasterViewModel model)
        {
            if (ModelState.IsValid)
            {
                model = BookIssueReturnService.UpdateBookReturnDetails(model);
                if (model.IsSuccessful == false)
                {
                    ModelState.AddModelError("error_msg", model.Message);
                }
                else
                {
                    RedirectToAction("BookIssueReturnList");
                }
                model.Message = "";
                return Json(model, JsonRequestBehavior.AllowGet);
            }
            else
            {
                model.Message = EduSuiteUIResources.Failed;
                return Json(model, JsonRequestBehavior.AllowGet);
            }
        }

        [HttpGet]
        public JsonResult FillBook(Int32? BookCategoryKey)
        {
            BookIssueReturnDetailsViewModel model = new BookIssueReturnDetailsViewModel();
            model.BookCategoryKey = BookCategoryKey ?? 0;
            model = BookIssueReturnService.FillBook(model);
            if (model == null)
            {
                model = new BookIssueReturnDetailsViewModel();
            }
            return Json(model.Book, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public JsonResult FillBookCopy(Int32? BookKey, Int32? RowKey)
        {
            BookIssueReturnDetailsViewModel model = new BookIssueReturnDetailsViewModel();
            model.BookKey = BookKey ?? 0;
            model.RowKey = RowKey ?? 0;
            model = BookIssueReturnService.FillBookCopy(model);
            if (model == null)
            {
                model = new BookIssueReturnDetailsViewModel();
            }
            return Json(model.BookCopy, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public JsonResult CheckCardIdExists(string cardId, string issueDate)
        {
            BookIssueReturnMasterViewModel model = new BookIssueReturnMasterViewModel();
            model.CardId = cardId;
            model.IssueDate = Convert.ToDateTime(issueDate);
            model = BookIssueReturnService.CheckCardIdExists(model);
            return Json(model.IsSuccessful, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public JsonResult GetNumberofBooks(string cardId, string issueDate)
        {
            BookIssueReturnMasterViewModel model = new BookIssueReturnMasterViewModel();
            model.CardId = cardId;
            model.IssueDate = Convert.ToDateTime(issueDate);
            model = BookIssueReturnService.CheckCardIdExists(model);
            return Json(model, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public JsonResult GetBookIssueReturnByValues(string cardId, string issueDate)
        {
            BookIssueReturnMasterViewModel model = new BookIssueReturnMasterViewModel();
            model.CardId = cardId;
            model.IssueDate = Convert.ToDateTime(issueDate);
            model = BookIssueReturnService.GetBookIssueByValues(model);
            if (model == null)
            {
                model = new BookIssueReturnMasterViewModel();
            }
            return Json(model, JsonRequestBehavior.AllowGet);
        }

        public ActionResult DeleteBookIssueReturn(Int32 Id)
        {
            BookIssueReturnMasterViewModel objModel = new BookIssueReturnMasterViewModel();
            objModel.RowKey = Id;
            try
            {
                objModel = BookIssueReturnService.DeleteBookIssueReturn(objModel);
            }
            catch (Exception ex)
            {
                objModel.Message = EduSuiteUIResources.Failed;
            }
            return Json(objModel);

        }

        public ActionResult DeleteBookIssueReturnDetails(Int32 Id)
        {
            BookIssueReturnDetailsViewModel objDetailsViewModel = new BookIssueReturnDetailsViewModel();
            BookIssueReturnMasterViewModel objMasterViewModel = new BookIssueReturnMasterViewModel();
            objDetailsViewModel.RowKey = Id;
            try
            {
                objMasterViewModel = BookIssueReturnService.DeleteBookIssueReturnDetails(objDetailsViewModel);
            }
            catch (Exception)
            {
                objMasterViewModel.Message = EduSuiteUIResources.Failed;
            }
            return Json(objMasterViewModel);

        }

        [HttpGet]
        public JsonResult AutoFillFine(string returnDate, int rowKey)
        {
            BookIssueReturnDetailsViewModel model = new BookIssueReturnDetailsViewModel();
            model.ReturnDate = Convert.ToDateTime(returnDate);
            model.RowKey = rowKey;
            model = BookIssueReturnService.FillIfAnyFine(model);
            if (model == null)
            {
                model = new BookIssueReturnDetailsViewModel();
            }
            return Json(model.IfAnyFine, JsonRequestBehavior.AllowGet);
        }
        [HttpGet]
        public JsonResult CheckBookCopyExists(long BookCopyKey, long BookKey)
        {
            return Json(true, JsonRequestBehavior.AllowGet);
        }
    }
}