
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
    public class SecondLanguageController : BaseController
    {
        private ISecondLanguageService SecondLanguageservice;
        public SecondLanguageController(ISecondLanguageService objSecondLanguageService)
        {
            this.SecondLanguageservice = objSecondLanguageService;
        }
        [HttpGet]
        public ActionResult SecondLanguageList()
        {
            return View();
        }
        [HttpGet]
        public ActionResult AddEditSecondLanguage(int? id)
        {
            SecondLanguageViewModel model = new SecondLanguageViewModel();
            model = SecondLanguageservice.GetSecondLanguageById(id);
            if (model == null)
            {
                model = new SecondLanguageViewModel();
            }
            return PartialView(model);

        }
        [HttpPost]
        public ActionResult AddEditSecondLanguage(SecondLanguageViewModel model)
        {

            if (ModelState.IsValid)
            {
                if (model.RowKey == 0)
                {
                    model = SecondLanguageservice.CreateSecondLanguage(model);
                }
                else
                {
                    model = SecondLanguageservice.UpdateSecondLanguage(model);
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
        public JsonResult GetSecondLanguage(string searchText)
        {
            int page = 1, rows = 15;
            List<SecondLanguageViewModel> SecondLanguageList = new List<SecondLanguageViewModel>();
            SecondLanguageList = SecondLanguageservice.GetSecondLanguage(searchText);
            int pageIndex = Convert.ToInt32(page) - 1;
            int pageSize = rows;
            int toatalRecords = SecondLanguageList.Count();
            var totalPage = (int)Math.Ceiling((float)toatalRecords / (float)rows);

            var jsonData = new
            {
                total = totalPage,
                page,
                records = toatalRecords,
                rows = SecondLanguageList
            };
            return Json(jsonData, JsonRequestBehavior.AllowGet);
        }
        [HttpPost]
        public ActionResult DeleteSecondLanguage(short id)
        {
            SecondLanguageViewModel objViewModel = new SecondLanguageViewModel();

            objViewModel.RowKey = id;
            try
            {
                objViewModel = SecondLanguageservice.DeleteSecondLanguage(objViewModel);
            }
            catch (Exception)
            {
                objViewModel.Message = EduSuiteUIResources.Failed;
            }
            return Json(objViewModel);
        }
    }
}