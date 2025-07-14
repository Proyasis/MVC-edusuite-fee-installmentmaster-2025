
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
    public class MediumController : BaseController
    {
        private IMediumService Mediumservice;
        public MediumController(IMediumService objMediumService)
        {
            this.Mediumservice = objMediumService;
        }
        [HttpGet]
        public ActionResult MediumList()
        {
            return View();
        }
        [HttpGet]
        public ActionResult AddEditMedium(int? id)
        {
            MediumViewModel model = new MediumViewModel();
            model = Mediumservice.GetMediumById(id);
            if (model == null)
            {
                model = new MediumViewModel();
            }
            return PartialView(model);

        }
        [HttpPost]
        public ActionResult AddEditMedium(MediumViewModel model)
        {

            if (ModelState.IsValid)
            {
                if (model.RowKey == 0)
                {
                    model = Mediumservice.CreateMedium(model);
                }
                else
                {
                    model = Mediumservice.UpdateMedium(model);
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
        public JsonResult GetMedium(string searchText)
        {
            int page = 1, rows = 15;
            List<MediumViewModel> MediumList = new List<MediumViewModel>();
            MediumList = Mediumservice.GetMedium(searchText);
            int pageIndex = Convert.ToInt32(page) - 1;
            int pageSize = rows;
            int toatalRecords = MediumList.Count();
            var totalPage = (int)Math.Ceiling((float)toatalRecords / (float)rows);

            var jsonData = new
            {
                total = totalPage,
                page,
                records = toatalRecords,
                rows = MediumList
            };
            return Json(jsonData, JsonRequestBehavior.AllowGet);
        }
        [HttpPost]
        public ActionResult DeleteMedium(short id)
        {
            MediumViewModel objViewModel = new MediumViewModel();

            objViewModel.RowKey = id;
            try
            {
                objViewModel = Mediumservice.DeleteMedium(objViewModel);
            }
            catch (Exception)
            {
                objViewModel.Message = EduSuiteUIResources.Failed;
            }
            return Json(objViewModel);
        }
    }
}