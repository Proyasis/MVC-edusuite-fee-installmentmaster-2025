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
    public class CounsellingTimeController : BaseController
    {
        private ICounsellingTimeService CounsellingTimeService;
        public CounsellingTimeController(ICounsellingTimeService objCounsellingTimeService)
        {
            this.CounsellingTimeService = objCounsellingTimeService;
        }
        [HttpGet]
        public ActionResult CounsellingTimeList()
        {
            return View();
        }
        [HttpGet]
        public ActionResult AddEditCounsellingTime(int? id)
        {
            CounsellingTimeViewModel model = new CounsellingTimeViewModel();
            model = CounsellingTimeService.GetCounsellingTimeById(id);
            if (model == null)
            {
                model = new CounsellingTimeViewModel();
            }
            return PartialView(model);

        }
        [HttpPost]
        public ActionResult AddEditCounsellingTime(CounsellingTimeViewModel model)
        {

            if (ModelState.IsValid)
            {
                if (model.RowKey == 0)
                {
                    model = CounsellingTimeService.CreateCounsellingTime(model);
                }
                else
                {
                    model = CounsellingTimeService.UpdateCounsellingTime(model);
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
        public JsonResult GetCounsellingTime(string searchText)
        {
            int page = 1, rows = 15;
            List<CounsellingTimeViewModel> CounsellingTimeList = new List<CounsellingTimeViewModel>();
            CounsellingTimeList = CounsellingTimeService.GetCounsellingTime(searchText);
            int pageIndex = Convert.ToInt32(page) - 1;
            int pageSize = rows;
            int toatalRecords = CounsellingTimeList.Count();
            var totalPage = (int)Math.Ceiling((float)toatalRecords / (float)rows);

            var jsonData = new
            {
                total = totalPage,
                page,
                records = toatalRecords,
                rows = CounsellingTimeList
            };
            return Json(jsonData, JsonRequestBehavior.AllowGet);
        }
        [HttpPost]
        public ActionResult DeleteCounsellingTime(short id)
        {
            CounsellingTimeViewModel objViewModel = new CounsellingTimeViewModel();

            objViewModel.RowKey = id;
            try
            {
                objViewModel = CounsellingTimeService.DeleteCounsellingTime(objViewModel);
            }
            catch (Exception)
            {
                objViewModel.Message = EduSuiteUIResources.Failed;
            }
            return Json(objViewModel);
        }
    }
}