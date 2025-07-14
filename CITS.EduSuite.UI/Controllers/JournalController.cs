using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using CITS.EduSuite.Business.Common;
using CITS.EduSuite.Business.Interfaces;
using CITS.EduSuite.Business.Models.ViewModels;
using CITS.EduSuite.Business.Provider;
using System.Web.Security;
using CITS.EduSuite.Business.Models.Security;
using CITS.EduSuite.Business.Models.Resources;

namespace CITS.EduSuite.UI.Controllers
{
    public class JournalController : BaseController
    {
        public IJournalService journalService;

        public JournalController(IJournalService objJournalService)
        {
            this.journalService = objJournalService;
        }

        [ActionAuthenticationAttribute(MenuCode = MenuConstants.Journal, ActionCode = ActionConstants.MenuAccess)]
        [HttpGet]
        public ActionResult Journal()
        {
            JournalViewModel model = new JournalViewModel();
            
            journalService.FillBranches(model);
            return View(model);
        }

        public JsonResult GetJournal(short? BranchKey,string sidx, string sord, int page, int rows)
        {
            long TotalRecords = 0;
            List<JournalViewModel> JournalList = new List<JournalViewModel>();
            JournalViewModel objViewModel = new JournalViewModel();           
            objViewModel.BranchKey = BranchKey ?? 0;
            objViewModel.PageIndex = page;
            objViewModel.PageSize = rows;
            objViewModel.SortBy = sidx;
            objViewModel.SortOrder = sord;
            JournalList = journalService.GetJournal(objViewModel,out TotalRecords);
            var totalPages = (int)Math.Ceiling((float)TotalRecords / (float)rows);
            var jsonData = new
            {
                total = totalPages,
                page,
                records = TotalRecords,
                rows = JournalList
            };
            return Json(jsonData, JsonRequestBehavior.AllowGet);
        }

        [ActionAuthenticationAttribute(MenuCode = MenuConstants.Journal, ActionCode = ActionConstants.AddEdit)]
        [HttpGet]
        public ActionResult AddEditJournal(int? id)
        {

            JournalViewModel model = new JournalViewModel();
            
            model.RowKey =Convert.ToInt64(id);
            model = journalService.GetJournalById(model);
            if (model == null)
            {
                model = new JournalViewModel();
            }
            return PartialView(model);
        }

        [ActionAuthenticationAttribute(MenuCode = MenuConstants.Journal, ActionCode = ActionConstants.AddEdit)]
        [HttpPost]
        public ActionResult AddEditJournal(JournalViewModel model)
        {
            if (ModelState.IsValid)
            {
                
                if (model.RowKey == 0)
                {
                    model = journalService.CreateJournal(model);
                }
                else
                {
                    model = journalService.UpdateJournal(model);

                }
                if (model.Message != AppConstants.Common.SUCCESS)
                {
                    ModelState.AddModelError("error_msg", model.Message);
                }
                else
                {
                    return Json(model);
                }
                model.Message = "";
                return PartialView(model);
            }
            model.Message = EduSuiteUIResources.Failed;

            return PartialView(model);
        }

        //[HttpGet]
        //public JsonResult GetBankAccount(short? BranchKey)
        //{
        //    JournalViewModel model = new JournalViewModel();
        //    model.BranchKey = BranchKey ?? 0;
        //    model = journalService.FillBankAccounts(model);
        //    return Json(model.BankAccounts, JsonRequestBehavior.AllowGet);
        //}
      
        [ActionAuthenticationAttribute(MenuCode = MenuConstants.Journal, ActionCode = ActionConstants.Delete)]
        [HttpPost]
        public ActionResult DeleteJournal(long id)
        {
            JournalViewModel objViewModel = new JournalViewModel();
            objViewModel.RowKey = id;
            try
            {
                objViewModel = journalService.DeleteJournal(objViewModel);


            }
            catch (Exception)
            {
                objViewModel.Message = EduSuiteUIResources.Failed;
            }
            return Json(objViewModel);
        }

        [ActionAuthenticationAttribute(MenuCode = MenuConstants.Journal, ActionCode = ActionConstants.Delete)]
        [HttpPost]
        public ActionResult DeleteJournalItem(long id)
        {
            JournalViewModel objViewModel = new JournalViewModel();
            objViewModel.RowKey = id;
            try
            {
                objViewModel = journalService.DeleteJournalItem(objViewModel);


            }
            catch (Exception)
            {
                objViewModel.Message = EduSuiteUIResources.Failed;
            }
            return Json(objViewModel);
        }
        [HttpGet]
        public JsonResult CheckAccountHeadExists(long? AccountHeadKey)
        {
            return Json(true, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public JsonResult FillAccountHead(byte? Key)
        {
            JournalDetailsViewModel model = new JournalDetailsViewModel();
            model.AccountGroupKey = Key ?? 0;
            model = journalService.FillAcountHead(model);
            return Json(model.AccountHeads, JsonRequestBehavior.AllowGet);
        }
    }
}