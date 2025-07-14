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
    public class AssetTypeController : BaseController
    {
        private IAssetTypeService AssetTypeService;
        public AssetTypeController(IAssetTypeService objAssetTypeService)
        {
            this.AssetTypeService = objAssetTypeService;
        }
        [ActionAuthenticationAttribute(MenuCode = MenuConstants.AssetType, ActionCode = ActionConstants.MenuAccess)]
        [HttpGet]
        public ActionResult AssetTypeList()
        {
            return View();
        }
        [ActionAuthenticationAttribute(MenuCode = MenuConstants.AssetType, ActionCode = ActionConstants.AddEdit)]
        [HttpGet]
        public ActionResult AddEditAssetType(int? id)
        {
            var AssetType = AssetTypeService.GetAssetTypeById(id);
            if (AssetType == null)
            {
                AssetType = new AssetTypeViewModel();
            }
            return PartialView(AssetType);
        }
        [HttpPost]
        public ActionResult AddEditAssetType(AssetTypeViewModel model)
        {
            //foreach (ModelState state in ViewData.ModelState.Values.Where(x => x.Errors.Count > 0))
            //{
            //}
            if (ModelState.IsValid)
            {
                if (model.RowKey == 0)
                {
                    model = AssetTypeService.CreateAssetType(model);
                }
                else
                {
                    model = AssetTypeService.UpdateAssetType(model);
                }
                if (model.Message != AppConstants.Common.SUCCESS)
                {
                    ModelState.AddModelError("error_msg", model.Message);
                }
                else
                {
                    model.Message = "";
                    return Json(model);
                }
                return PartialView(model);
            }
            model.Message =  EduSuiteUIResources.Failed;  
            return PartialView(model);
        }

        public JsonResult GetAssetType(string searchText)
        {
            int page = 1, rows = 15;
            List<AssetTypeViewModel> materialTypeList = new List<AssetTypeViewModel>();
            materialTypeList = AssetTypeService.GetAssetType(searchText);
            int pageIndex = Convert.ToInt32(page) - 1;
            int pageSize = rows;
            int toatalRecords = materialTypeList.Count();
            var totalPage = (int)Math.Ceiling((float)toatalRecords / (float)rows);

            if (materialTypeList.Count > 0)
            {
                AssetTypeViewModel model = materialTypeList[0];
            }


            var jsonData = new
            {
                total = totalPage,
                page,
                records = toatalRecords,
                rows = materialTypeList
            };
            return Json(jsonData, JsonRequestBehavior.AllowGet);
        }
        [ActionAuthenticationAttribute(MenuCode = MenuConstants.AssetType, ActionCode = ActionConstants.Delete)]
        [HttpPost]
        public ActionResult DeleteAssetType(int? id)
        {
            AssetTypeViewModel model = new AssetTypeViewModel();
            model.RowKey = (byte)(id);
            try
            {
                model = AssetTypeService.DeleteAssetType(model);
            }
            catch (Exception ex)
            {
                model.Message =  EduSuiteUIResources.Failed;  
            }
            return Json(model);
        }

        [HttpGet]
        public JsonResult CheckHSNCodeExists(string HSNCode, int? RowKey)
        {
            AssetTypeViewModel model = AssetTypeService.CheckHSNCodeExists(HSNCode, RowKey ?? 0);
            return Json(model.IsSuccessful, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public JsonResult GetHSNCodeDetails(long? id)
        {
            HSNCodeMasterViewModel model = new HSNCodeMasterViewModel();
            model.RowKey = id ?? 0;
            model = AssetTypeService.GetHSNCodeDetailsById(model);
            return Json(model, JsonRequestBehavior.AllowGet);
        }

       
    }
}