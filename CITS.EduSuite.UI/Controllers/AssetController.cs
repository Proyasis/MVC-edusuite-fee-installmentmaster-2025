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
    public class AssetController : BaseController
    {
        private IAssetService AssetService;
        public AssetController(IAssetService objAssetService)
        {
            this.AssetService = objAssetService;
        }
        [ActionAuthenticationAttribute(MenuCode = MenuConstants.Asset, ActionCode = ActionConstants.MenuAccess)]
        [HttpGet]
        public ActionResult AssetList(AssetViewModel model)
        {
           
            return View(model);
        }
        [ActionAuthenticationAttribute(MenuCode = MenuConstants.Asset, ActionCode = ActionConstants.AddEdit)]
        [HttpGet]
        public ActionResult AddEditAsset(int? id)
        {
            AssetViewModel model = new AssetViewModel();
         
            model = AssetService.GetAssetById(id);
            if (model == null)
            {
                model = new AssetViewModel();
            }
            model.AssetTypeKey = id ?? model.AssetTypeKey;
            return View(model);

        }
        [ActionAuthenticationAttribute(MenuCode = MenuConstants.Asset, ActionCode = ActionConstants.View)]
        [HttpGet]
        public ActionResult ViewAsset(int id)
        {
            AssetViewModel model = new AssetViewModel();
            model = AssetService.GetAssetById(id);
            if (model == null)
            {
                model = new AssetViewModel();
            }
            model.AssetTypeKey = id;
            return PartialView(model);

        }
        [HttpPost]
        public ActionResult AddEditAsset(AssetViewModel model)
        {

            if (ModelState.IsValid)
            {
                if (model.MasterKey == 0)
                {
                    model = AssetService.CreateAsset(model);
                }
                else
                {
                    model = AssetService.UpdateAsset(model);
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
                return View(model);
            }

            model.Message =  EduSuiteUIResources.Failed;  
            return View(model);
        }
        public JsonResult GetAsset(string searchText)
        {
            int page = 1, rows = 15;
            List<AssetViewModel> AssetList = new List<AssetViewModel>();
            AssetList = AssetService.GetAsset(searchText);
            int pageIndex = Convert.ToInt32(page) - 1;
            int pageSize = rows;
            int toatalRecords = AssetList.Count();
            var totalPage = (int)Math.Ceiling((float)toatalRecords / (float)rows);

            if (AssetList.Count > 0)
            {
                AssetViewModel model = AssetList[0];
            }

            var jsonData = new
            {
                total = totalPage,
                page,
                records = toatalRecords,
                rows = AssetList
            };
            return Json(jsonData, JsonRequestBehavior.AllowGet);
        }
        [ActionAuthenticationAttribute(MenuCode = MenuConstants.Asset, ActionCode = ActionConstants.Delete)]
        [HttpPost]
        public ActionResult DeleteAsset(short id)
        {
            AssetViewModel objViewModel = new AssetViewModel();

            objViewModel.AssetTypeKey = id;
            try
            {
                objViewModel = AssetService.DeleteAsset(objViewModel);
            }
            catch (Exception)
            {
                objViewModel.Message =  EduSuiteUIResources.Failed;  
            }
            return Json(objViewModel);
        }
        [ActionAuthenticationAttribute(MenuCode = MenuConstants.Asset, ActionCode = ActionConstants.Delete)]
        [HttpPost]
        public ActionResult DeleteAssetDetails(short id)
        {
            AssetViewModel objViewModel = new AssetViewModel();
            try
            {
                objViewModel = AssetService.DeleteAssetDetails(id);
            }
            catch (Exception)
            {
                objViewModel.Message =  EduSuiteUIResources.Failed;  
            }
            return Json(objViewModel);
        }
        [HttpGet]
        public JsonResult CheckAssetDetailCode(string AssetDetailCode, long? RowKey)
        {
            AssetDetailsViewModel model = new AssetDetailsViewModel();
            model.AssetDetailCode = AssetDetailCode;
            model.RowKey = RowKey;
            AssetViewModel Assetmodel = AssetService.CheckAssetDetailCode(model);
            return Json(Assetmodel.IsSuccessful, JsonRequestBehavior.AllowGet);
        }
        [HttpGet]
        public JsonResult GetAccountHead()
        {
            AssetViewModel model = new AssetViewModel();
            model = AssetService.FillAccountHead(model);
            return Json(model, JsonRequestBehavior.AllowGet);
        }

      

    }
}