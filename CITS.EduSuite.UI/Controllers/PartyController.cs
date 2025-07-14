using CITS.EduSuite.Business.Common;
using CITS.EduSuite.Business.Interfaces;
using CITS.EduSuite.Business.Models.Resources;
using CITS.EduSuite.Business.Models.Security;
using CITS.EduSuite.Business.Models.ViewModels;
using CITS.EduSuite.Business.Provider;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;

namespace CITS.EduSuite.UI.Controllers
{
    public class PartyController : BaseController
    {
        private IPartyService partyService;
        private INotificationTemplateService notificationTemplateService;
        public PartyController(IPartyService objPartyService, INotificationTemplateService objNotificationTemplateService)
        {
            this.partyService = objPartyService;
            this.notificationTemplateService = objNotificationTemplateService;
        }
        [ActionAuthenticationAttribute(MenuCode = MenuConstants.Party, ActionCode = ActionConstants.MenuAccess)]
        [HttpGet]
        public ActionResult PartyList()
        {
            PartyViewModel model = new PartyViewModel();
           
            model = partyService.FillPartyType(model);
            model = partyService.FillBranches(model);
            return View(model);
        }
        [ActionAuthenticationAttribute(MenuCode = MenuConstants.Party, ActionCode = ActionConstants.View)]
        [HttpGet]
        public ActionResult PartyWiseOrderDetails(byte? id)
        {
            PartyViewModel model = new PartyViewModel();
           
            partyService.FillBranches(model);
            model.PartyTypeKey = id;
            return View(model);
        }
        [ActionAuthenticationAttribute(MenuCode = MenuConstants.Party, ActionCode = ActionConstants.AddEdit)]
        [HttpGet]
        public ActionResult AddEditParty(long? id, byte? type, short? branch)
        {
            PartyViewModel model = new PartyViewModel();
            model = partyService.GetPartyById(id);
            if (model == null)
            {
                model = new PartyViewModel();
            }
            partyService.FillBranches(model);
            model.PartyTypeKey = type ?? model.PartyTypeKey;
            model.BranchKey = branch ?? model.BranchKey;
            return PartialView(model);

        }

        [HttpPost]
        public ActionResult AddEditParty(PartyViewModel model)
        {

            if (ModelState.IsValid)
            {
                if (model.RowKey == 0)
                {
                    model = partyService.CreateParty(model);
                }
                else
                {
                    model = partyService.UpdateParty(model);
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

        [HttpGet]
        public JsonResult GetParty(string searchText, byte? PartyTypeKey, short? branchKey, bool? isBalanceDue, string sidx, string sord, int page, int rows)
        {
            PartyViewModel model = new PartyViewModel();
            List<PartyViewModel> partyList = new List<PartyViewModel>();
            model.BranchKey = branchKey ?? 0;
            model.PartyTypeKey = PartyTypeKey;
            model.PageIndex = page;
            model.PageSize = rows;
            model.SortBy = sidx;
            model.SortOrder = sord;
            model.isBalanceDue = isBalanceDue ?? false;

            partyList = partyService.GetParty(searchText, model);

            int pageIndex = Convert.ToInt32(page) - 1;
            int pageSize = rows;
            int toatalRecords = model.TotalRecords;
            var totalPage = (int)Math.Ceiling((float)toatalRecords / (float)rows);
            var jsonData = new
            {
                total = totalPage,
                page,
                records = toatalRecords,
                rows = partyList,
                userData = new
                {
                    TotalAmount = model.TotalAmount,
                    TotalPaidAmount = model.TotalPaidAmount,
                    TotalBalanceAmount = model.TotalBalanceAmount
                }
            };
            if (partyList.Count > 0)
            {
                model = partyList[0];
            }

          
            return Json(jsonData, JsonRequestBehavior.AllowGet);
        }
        [ActionAuthenticationAttribute(MenuCode = MenuConstants.Party, ActionCode = ActionConstants.Delete)]
        [HttpPost]
        public ActionResult DeleteParty(long? id)
        {
            PartyViewModel objViewModel = new PartyViewModel();

            objViewModel.RowKey = id ?? 0;
            try
            {
                objViewModel = partyService.DeleteParty(objViewModel);

            }
            catch (Exception)
            {
                objViewModel.Message = EduSuiteUIResources.Failed;
            }
            return Json(objViewModel);
        }
        
        [ActionAuthenticationAttribute(MenuCode = MenuConstants.Party, ActionCode = ActionConstants.View)]
        [HttpGet]

        public ActionResult ViewPartyOrders(int? id)
        {
            PartyViewModel model = new PartyViewModel();
            var PartysalesOrder = partyService.GetPartyOrdersId(id);
            if (PartysalesOrder == null)
            {
                PartysalesOrder = new PartyViewModel();
            }
            return PartialView(PartysalesOrder);
        }

        [HttpGet]
        public JsonResult CheckGSTINNumberExists(string GSTINNumber, byte? PartyTypeKey, long? RowKey)
        {
            PartyViewModel appUser = new PartyViewModel();
            appUser = partyService.CheckGSTINNumberExists(GSTINNumber, PartyTypeKey, RowKey);
            return Json(appUser.IsSuccessful, JsonRequestBehavior.AllowGet);
        }

        #region GetBranchKey

      
        #endregion

        [HttpGet]
        public JsonResult FillPartyTypeById(byte? partyType)
        {
            PartyViewModel model = new PartyViewModel();
            model.PartyTypeKey = partyType ?? 0;
            model = partyService.FillPartyTypeById(model);
            return Json(model, JsonRequestBehavior.AllowGet);
        }
    }
}