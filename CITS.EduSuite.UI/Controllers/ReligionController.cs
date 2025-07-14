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
    public class ReligionController : BaseController
    {
        // GET: ExamCenter
        private IReligionService ReligionService;
        public ReligionController(IReligionService objReligionService)
        {
            this.ReligionService = objReligionService;
        }
        [HttpGet]
        public ActionResult ReligionList()
        {
            return View();
        }
        [HttpGet]
        public ActionResult AddEditReligion(short? id)
        {
            ReligionViewModel model = new ReligionViewModel();
            model = ReligionService.GetReligionById(id);
            if (model == null)
            {
                model = new ReligionViewModel();
            }
            return PartialView(model);
        }


        [HttpPost]
        public ActionResult AddEditReligion(ReligionViewModel model)
        {

            if (ModelState.IsValid)
            {
                if (model.RowKey == 0)
                {
                    model = ReligionService.CreateReligion(model);
                }
                else
                {
                    model = ReligionService.UpdateReligion(model);
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
        public JsonResult GetReligion(string searchText)
        {
            int page = 1, rows = 15;
            List<ReligionViewModel> ReligionList = new List<ReligionViewModel>();
            ReligionList = ReligionService.GetReligion(searchText);
            int pageIndex = Convert.ToInt32(page) - 1;
            int pageSize = rows;
            int toatalRecords = ReligionList.Count();
            var totalPage = (int)Math.Ceiling((float)toatalRecords / (float)rows);

            var jsonData = new
            {
                total = totalPage,
                page,
                records = toatalRecords,
                rows = ReligionList
            };
            return Json(jsonData, JsonRequestBehavior.AllowGet);
        }
        [HttpPost]
        public ActionResult DeleteReligion(short id)
        {
            ReligionViewModel objViewModel = new ReligionViewModel();

            objViewModel.RowKey = id;
            try
            {
                objViewModel = ReligionService.DeleteReligion(objViewModel);
            }
            catch (Exception)
            {
                objViewModel.Message = EduSuiteUIResources.Failed;
            }
            return Json(objViewModel);
        }

        [HttpGet]
        public JsonResult CheckReligionNameExists(string ReligionName, short RowKey)
        {
            ReligionViewModel model = new ReligionViewModel();
            model.RowKey = RowKey;
            model.ReligionName = ReligionName;
            model = ReligionService.CheckReligionNameExists(model);
            return Json(model.IsSuccessful, JsonRequestBehavior.AllowGet);
        }
    }
}