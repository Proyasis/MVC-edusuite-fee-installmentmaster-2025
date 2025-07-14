using CITS.EduSuite.Business.Common;
using CITS.EduSuite.Business.Interfaces;
using CITS.EduSuite.Business.Models.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;

namespace CITS.EduSuite.UI.Controllers
{
    public class CountryController : BaseController
    {
        private ICountryService CountryService;
        public CountryController(ICountryService objCountryService)
        {
            this.CountryService = objCountryService;
        }

        [ActionAuthenticationAttribute(MenuCode = MenuConstants.Country, ActionCode = ActionConstants.MenuAccess)]
        [HttpGet]
        public ActionResult CountryList()
        {
            CountryViewModel Country = new CountryViewModel();
            return View(Country);
        }

        [HttpGet]
        public JsonResult GetCountry(string searchText)
        {
            int page = 1, rows = 10;

            List<CountryViewModel> CountryList = new List<CountryViewModel>();
            CountryList = CountryService.GetCountries(searchText);

            int pageIndex = Convert.ToInt32(page) - 1;
            int pageSize = rows;
            int totalRecords = CountryList.Count();
            var totalPages = (int)Math.Ceiling((float)totalRecords / (float)rows);

            var jsonData = new
            {
                total = totalPages,
                page,
                records = totalRecords,
                rows = CountryList
            };
            return Json(jsonData, JsonRequestBehavior.AllowGet);
        }

        [ActionAuthenticationAttribute(MenuCode = MenuConstants.Country, ActionCode = ActionConstants.AddEdit)]
        [HttpGet]
        public ActionResult AddEditCountry(short? id)
        {
            var Country = CountryService.GetCountryById(id);
            if (Country == null)
            {
                Country = new CountryViewModel();
            }
            return PartialView(Country);
        }

        [HttpPost]
        public ActionResult AddEditCountry(CountryViewModel Country)
        {

            if (ModelState.IsValid)
            {
                if (Country.RowKey == 0)
                {
                    Country = CountryService.CreateCountry(Country);
                }
                else
                {
                    Country = CountryService.UpdateCountry(Country);
                }

                if (Country.Message != AppConstants.Common.SUCCESS)
                {
                    ModelState.AddModelError("error_msg", Country.Message);
                }
                else
                {
                    return Json(Country);
                }

                Country.Message = "";
                return PartialView(Country);
            }

            Country.Message = EduSuiteResourceManager.GetApplicationString(AppConstants.Common.FAILED);
            return PartialView(Country);

        }

        [ActionAuthenticationAttribute(MenuCode = MenuConstants.Country, ActionCode = ActionConstants.Delete)]
        [HttpPost]
        public ActionResult DeleteCountry(Int16 id)
        {
            CountryViewModel objViewModel = new CountryViewModel();

            objViewModel.RowKey = id;
            try
            {
                objViewModel = CountryService.DeleteCountry(objViewModel);
            }
            catch (Exception)
            {
                objViewModel.Message = EduSuiteResourceManager.GetApplicationString(AppConstants.Common.FAILED);
            }
            return Json(objViewModel);
        }
    }
}

