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
    public class FeeTypeController : BaseController
    {
        public IFeeTypeService FeeTypeService;

        public FeeTypeController(IFeeTypeService ObjFeeTypeService)
        {
            this.FeeTypeService = ObjFeeTypeService;
        }

        [HttpGet]

        public ActionResult FeeTypeList()
        {
            return View();
        }

        [HttpGet]
        public ActionResult AddEditFeeType(int? id)
        {
            FeeTypeViewModel model = new FeeTypeViewModel();
            model = FeeTypeService.GetFeeTypeById(id);
            if (model == null)
            {
                model = new FeeTypeViewModel();
            }
            return PartialView(model);
        }
        [HttpPost]
        public ActionResult AddEditFeeType(FeeTypeViewModel model)
        {
            if (ModelState.IsValid)
            {
                if (model.RowKey == 0)
                {
                    model = FeeTypeService.CreateFeeType(model);
                }
                else
                {
                    model = FeeTypeService.UpdateFeeType(model);
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
        public JsonResult GetFeeType(string SearchText)
        {
            int page = 1, rows = 15;
            List<FeeTypeViewModel> FeeTypeList = new List<FeeTypeViewModel>();
            FeeTypeList = FeeTypeService.GetFeeType(SearchText);
            int pageindex = Convert.ToInt32(page) - 1;
            int pagesize = rows;
            int totalrecords = FeeTypeList.Count();
            var totalpage = (int)Math.Ceiling((float)totalrecords / (float)rows);
            var jsonData = new
            {
                total = totalpage,
                page,
                records = totalrecords,
                rows = FeeTypeList
            };
            return Json(jsonData, JsonRequestBehavior.AllowGet);
        }
        [HttpPost]
        public ActionResult DeleteFeeType(short id)
        {
            FeeTypeViewModel Model = new FeeTypeViewModel();
            Model.RowKey = id;
            try
            {
                Model = FeeTypeService.DeleteFeeType(Model);
            }
            catch (Exception)
            {
                Model.Message = EduSuiteUIResources.Failed;
            }
            return Json(Model);
        }

        [HttpGet]

        public ActionResult CheckFeeTypeExist(string FeeTypeName, short RowKey)
        {
            FeeTypeViewModel model = new FeeTypeViewModel();
            model.FeeTypeName = FeeTypeName;
            model.RowKey = RowKey;
            model = FeeTypeService.CheckFeeTypeExist(model);
            return Json(model.IsSuccessful, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public JsonResult FillAccountHeadType(byte key)
        {
            FeeTypeViewModel model = new FeeTypeViewModel();
            model.AccountGroupKey = key;
            model = FeeTypeService.FillAccountHeadType(model);
            return Json(model.AccountHeadType, JsonRequestBehavior.AllowGet);
        }

       
    }
}