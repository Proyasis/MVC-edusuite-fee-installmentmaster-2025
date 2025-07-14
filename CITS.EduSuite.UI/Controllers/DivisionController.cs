
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
    public class DivisionController : BaseController
    {
       private IDivisionService Divisionservice;
        public DivisionController(IDivisionService objDivisionService)
        {
            this.Divisionservice = objDivisionService;
        }
        [HttpGet]
        public ActionResult DivisionList()
        {
            return View();
        }
        [HttpGet]
        public ActionResult AddEditDivision(int? id)
        {
            DivisionViewModel model = new DivisionViewModel();
            model = Divisionservice.GetDivisionById(id);
            if (model == null)
            {
                model = new DivisionViewModel();
            }
            return PartialView(model);

        }
        [HttpPost]
        public ActionResult AddEditDivision(DivisionViewModel model)
        {

            if (ModelState.IsValid)
            {
                if (model.RowKey == 0)
                {
                    model = Divisionservice.CreateDivision(model);
                }
                else
                {
                    model = Divisionservice.UpdateDivision(model);
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
        public JsonResult GetDivision(string searchText)
        {
            int page = 1, rows = 15;
            List<DivisionViewModel> DivisionList = new List<DivisionViewModel>();
            DivisionList = Divisionservice.GetDivision(searchText);
            int pageIndex = Convert.ToInt32(page) - 1;
            int pageSize = rows;
            int toatalRecords = DivisionList.Count();
            var totalPage = (int)Math.Ceiling((float)toatalRecords / (float)rows);

            var jsonData = new
            {
                total = totalPage,
                page,
                records = toatalRecords,
                rows = DivisionList
            };
            return Json(jsonData, JsonRequestBehavior.AllowGet);
        }
        [HttpPost]
        public ActionResult DeleteDivision(short id)
        {
            DivisionViewModel objViewModel = new DivisionViewModel();

            objViewModel.RowKey = id;
            try
            {
                objViewModel = Divisionservice.DeleteDivision(objViewModel);
            }
            catch (Exception)
            {
                objViewModel.Message = EduSuiteUIResources.Failed;
            }
            return Json(objViewModel);
        }
    }
}