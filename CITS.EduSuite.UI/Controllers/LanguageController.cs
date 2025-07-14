
using CITS.EduSuite.Business.Common;
using CITS.EduSuite.Business.Interfaces;
using CITS.EduSuite.Business.Models.Resources;
using CITS.EduSuite.Business.Models.ViewModels;
using CITS.EduSuite.Business.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace CITS.EduSuite.UI.Controllers
{
    public class LanguageController : BaseController
    {
        private ILanguageService languageService;


        public LanguageController(ILanguageService objLanguageService)
        {
            this.languageService = objLanguageService;

        }
        [HttpGet]
        public ActionResult LanguageList()
        {
            return View();
        }

        [HttpGet]
        public JsonResult GetLanguages(string searchText)
        {
            int page = 1, rows = 10;

            List<LanguageViewModel> languageList = new List<LanguageViewModel>();
            languageList = languageService.GetLanguages(searchText);

            int pageIndex = Convert.ToInt32(page) - 1;
            int pageSize = rows;
            int totalRecords = languageList.Count();
            var totalPages = (int)Math.Ceiling((float)totalRecords / (float)rows);

            var jsonData = new
            {
                total = totalPages,
                page,
                records = totalRecords,
                rows = languageList
            };
            return Json(jsonData, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public ActionResult AddEditLanguage(short? id)
        {
            var language = languageService.GetLanguagesById(id);
            if (language == null)
            {
                language = new LanguageViewModel();
            }
            return PartialView(language);
        }
        public ActionResult AddEditLanguage(LanguageViewModel language)
        {

            if (ModelState.IsValid)
            {
                if (language.RowKey == 0)
                {
                    language = languageService.CreateLanguage(language);
                }


                else
                {
                    language = languageService.UpdateLanguage(language);
                }

                if (language.Message != AppConstants.Common.SUCCESS)
                {
                    ModelState.AddModelError("error_msg", language.Message);
                }
                else
                {
                    return Json(language);
                }

                language.Message = "";
                return PartialView(language);
            }

            language.Message = EduSuiteUIResources.Failed;
            return PartialView(language);

        }

        [HttpPost]
        public ActionResult DeleteLanguage(Int16 id)
        {
            LanguageViewModel objViewModel = new LanguageViewModel();

            objViewModel.RowKey = id;
            try
            {
                objViewModel = languageService.DeleteLanguage(objViewModel);
            }
            catch (Exception)
            {
                objViewModel.Message = EduSuiteUIResources.Failed;
            }
            return Json(objViewModel);
        }
    }
}
