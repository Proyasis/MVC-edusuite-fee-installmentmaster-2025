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
    public class AssetPurchaseController : BaseController
    {
        private IAssetPurchaseService AssetPurchaseMasterService;
        public AssetPurchaseController(IAssetPurchaseService objAssetPurchaseMasterService)
        {
            this.AssetPurchaseMasterService = objAssetPurchaseMasterService;
        }
        [ActionAuthenticationAttribute(MenuCode = MenuConstants.AssetPurchase, ActionCode = ActionConstants.MenuAccess)]
        [HttpGet]
        public ActionResult AssetPurchaseList()
        {
            AssetPurchaseMasterViewModel model = new AssetPurchaseMasterViewModel();
          
            model = AssetPurchaseMasterService.FillBranches(model);
            return View(model);
        }
        [ActionAuthenticationAttribute(MenuCode = MenuConstants.AssetPurchase, ActionCode = ActionConstants.AddEdit)]
        [HttpGet]
        public ActionResult AddEditAssetPurchase(long? id)
        {
            AssetPurchaseMasterViewModel model = new AssetPurchaseMasterViewModel();
           
            model.RowKey = id ?? 0;
            var AssetPurchaseMaster = AssetPurchaseMasterService.GetAssetPurchaseMasterById(model);
            if (AssetPurchaseMaster == null)
            {
                AssetPurchaseMaster = new AssetPurchaseMasterViewModel();
            }
            return View(AssetPurchaseMaster);
        }
        [HttpPost]
        public ActionResult AddEditAssetPurchase(AssetPurchaseMasterViewModel model)
        {
            if (ModelState.IsValid)
            {
                if (model.RowKey == 0)
                {
                    model = AssetPurchaseMasterService.CreateAssetPurchaseMaster(model);
                }
                else
                {
                    model = AssetPurchaseMasterService.UpdateAssetPurchaseMaster(model);
                }

                if (model.Message != AppConstants.Common.SUCCESS)
                {
                    ModelState.AddModelError("error_msg", model.Message);
                }
                else
                {
                    return Json(model);
                }

                //model.Message = "";
                return Json(model);
            }
            //foreach (ModelState state in ViewData.ModelState.Values.Where(x => x.Errors.Count > 0))
            //{
            //}
            model.Message =  EduSuiteUIResources.Failed;  

            return View(model);
        }
        public JsonResult GetAssetPurchase(string searchText, short? branchkey, string sidx, string sord, int page, int rows)
        {
            List<AssetPurchaseMasterViewModel> AssetPurchaseMasterList = new List<AssetPurchaseMasterViewModel>();
            AssetPurchaseMasterViewModel model = new AssetPurchaseMasterViewModel();
            model.BranchKey = branchkey ?? 0;
            model.PageIndex = page;
            model.PageSize = rows;
            model.SortBy = sidx;
            model.SortOrder = sord;

            AssetPurchaseMasterList = AssetPurchaseMasterService.GetAssetPurchaseMaster(model, searchText);
            int pageIndex = Convert.ToInt32(page) - 1;
            int pageSize = rows;
            int toatalRecords = model.TotalRecords;
            var totalPage = (int)Math.Ceiling((float)toatalRecords / (float)rows);

            if (AssetPurchaseMasterList.Count > 0)
            {
                model = AssetPurchaseMasterList[0];
            }

            var jsonData = new
            {
                total = totalPage,
                page,
                records = toatalRecords,
                rows = AssetPurchaseMasterList
            };
            return Json(jsonData, JsonRequestBehavior.AllowGet);
        }
        [ActionAuthenticationAttribute(MenuCode = MenuConstants.AssetPurchase, ActionCode = ActionConstants.Delete)]
        [HttpPost]
        public ActionResult DeleteAssetPurchase(int id)
        {
            AssetPurchaseMasterViewModel objViewModel = new AssetPurchaseMasterViewModel();
            objViewModel.RowKey = id;
            try
            {
                objViewModel = AssetPurchaseMasterService.DeleteAssetPurchaseMaster(objViewModel);
            }
            catch (Exception)
            {
                objViewModel.Message =  EduSuiteUIResources.Failed;  
            }
            return Json(objViewModel);
        }
        [ActionAuthenticationAttribute(MenuCode = MenuConstants.AssetPurchase, ActionCode = ActionConstants.Delete)]
        [HttpPost]
        public ActionResult DeleteAssetPurchaseItem(long id)
        {
            AssetPurchaseDetailsViewModel objViewModel = new AssetPurchaseDetailsViewModel();
            AssetPurchaseMasterViewModel objAssetPurchaseMasterViewModel = new AssetPurchaseMasterViewModel();
            objViewModel.RowKey = id;
            try
            {
                objAssetPurchaseMasterViewModel = AssetPurchaseMasterService.DeleteAssetPurchaseItem(objViewModel);
            }
            catch (Exception)
            {
                objAssetPurchaseMasterViewModel.Message =  EduSuiteUIResources.Failed;  
            }
            return Json(objAssetPurchaseMasterViewModel);
        }

        [HttpGet]
        public JsonResult GetMobileNumberByPartyKey(long? id)
        {
            AssetPurchaseMasterViewModel AssetPurchaseMasterViewModel = AssetPurchaseMasterService.GetPartyDetailsByPartyKey(id);
            return Json(AssetPurchaseMasterViewModel, JsonRequestBehavior.AllowGet);
        }

        //Payment
        [ActionAuthenticationAttribute(MenuCode = MenuConstants.AssetPurchase, ActionCode = ActionConstants.Payment)]
        [HttpGet]
        public ActionResult AddEditAssetPurchasePayment(long? id)
        {
            var employeeSalaryPayment = AssetPurchaseMasterService.GetAssetPurchasePaymentById(id ?? 0);
            var Url = "~/Views/AssetPurchase/AssetPurchasePayment.cshtml";
            return PartialView(Url, employeeSalaryPayment);
        }
        [HttpPost]
        public ActionResult AddEditAssetPurchasePayment(PaymentWindowViewModel model)
        {

            if (ModelState.IsValid)
            {
                if (model.PaymentKey == 0)
                {
                    model = AssetPurchaseMasterService.CallAssetPurchasePayment(model);

                }
                else
                {
                    //model = salesOrderMasterService.UpdateSalesOrderPayment(model);
                }

                if (model.Message != AppConstants.Common.SUCCESS)
                {
                    ModelState.AddModelError("error_msg", model.Message);
                }
                else
                {
                    return Json(model);
                }

                //model.Message = "";
                return Json(model);
            }

            model.Message =  EduSuiteUIResources.Failed;  
            return PartialView(model);

        }

        [HttpGet]
        public JsonResult GetPartyByPartyType(byte id, short Branch)
        {
            AssetPurchaseMasterViewModel model = new AssetPurchaseMasterViewModel();
            model.PartyTypeKey = id;
            model.BranchKey = Branch;
            model = AssetPurchaseMasterService.GetPartyByPartyType(model);
            return Json(model, JsonRequestBehavior.AllowGet);
        }

        [ActionAuthenticationAttribute(MenuCode = MenuConstants.AssetPurchase, ActionCode = ActionConstants.View)]
        [HttpGet]
        public ActionResult ViewAssetPurchase(int? id)
        {
            var salesOrderMaster = AssetPurchaseMasterService.ViewAssetPurchaseMasterById(id);
            if (salesOrderMaster == null)
            {
                salesOrderMaster = new AssetPurchaseMasterViewModel();
            }

            //return View(salesOrderMaster);
            return PartialView(salesOrderMaster);
        }
        [HttpGet]
        public JsonResult GetBalance(short PaymentModeKey, long? AssetPurchasePaymentRowKey, long? BankAccountKey)
        {
            decimal Balance = AssetPurchaseMasterService.CheckShortBalance(PaymentModeKey, AssetPurchasePaymentRowKey ?? 0, BankAccountKey ?? 0);
            return Json(Balance, JsonRequestBehavior.AllowGet);
        }

       

        [HttpGet]
        public JsonResult GetRawMaterialDetailsById(int? id, short Branch)
        {
            AssetPurchaseDetailsViewModel model = AssetPurchaseMasterService.GetRawMaterialDetailsById(id, Branch);
            return Json(model, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public JsonResult FillRawMaterialsById(int? Id)
        {
            AssetPurchaseDetailsViewModel model = new AssetPurchaseDetailsViewModel();
            model.AssetTypeKey = Id??0;
            AssetPurchaseMasterService.FillRawMaterialsById(model);
            return Json(model, JsonRequestBehavior.AllowGet);
        }


        [HttpGet]
        public JsonResult CheckBillNumberExists(string BillNo, long? RowKey)
        {
            AssetPurchaseMasterViewModel appUser = new AssetPurchaseMasterViewModel();
            appUser = AssetPurchaseMasterService.CheckBillNumberExists(BillNo, RowKey);
            return Json(appUser.IsSuccessful, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public JsonResult FillRateTypes(int Id, byte? StockUOMKey, byte? WidthUOMKey, byte? LengthUOMKey)
        {
            AssetPurchaseDetailsViewModel model = new AssetPurchaseDetailsViewModel();
            model = AssetPurchaseMasterService.FillRateTypes(model);
            return Json(model, JsonRequestBehavior.AllowGet);
        }
    }
}