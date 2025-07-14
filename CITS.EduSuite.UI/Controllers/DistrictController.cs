using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using CITS.EduSuite.Business.Models.ViewModels;
using CITS.EduSuite.Business.Common;
using CITS.EduSuite.Business.Services;
using CITS.EduSuite.Business.Interfaces;

namespace CITS.EduSuite.UI.Controllers
{
    public class DistrictController : BaseController 
    {
        private IDistrictService DistrictService;
        public DistrictController(IDistrictService objDistrictService)
        {
            this.DistrictService = objDistrictService;
        }

        [ActionAuthenticationAttribute(MenuCode = MenuConstants.District, ActionCode = ActionConstants.MenuAccess)]
        [HttpGet]
        public ActionResult DistrictList()
        {
            DistrictViewModel District = new DistrictViewModel();
            return View(District);
        }

        [HttpGet]
        public JsonResult GetDistrict(string searchText)
        {
            int page = 1, rows = 10;

            List<DistrictViewModel> DistrictList = new List<DistrictViewModel>();
            DistrictList = DistrictService.GetDistricts(searchText);

            int pageIndex = Convert.ToInt32(page) - 1;
            int pageSize = rows;
            int totalRecords = DistrictList.Count();
            var totalPages = (int)Math.Ceiling((float)totalRecords / (float)rows);

            var jsonData = new
            {
                total = totalPages,
                page,
                records = totalRecords,
                rows = DistrictList
            };
            return Json(jsonData, JsonRequestBehavior.AllowGet);
        }

        [ActionAuthenticationAttribute(MenuCode = MenuConstants.District, ActionCode = ActionConstants.AddEdit)]
        [HttpGet]
        public ActionResult AddEditDistrict(short? id)
        {
            var District = DistrictService.GetDistrictById(id);
            if (District == null)
            {
                District = new DistrictViewModel();
            }
            return PartialView(District);
        }

        [HttpPost]
        public ActionResult AddEditDistrict(DistrictViewModel District)
        {

            if (ModelState.IsValid)
            {
                if (District.RowKey == 0)
                {
                    District = DistrictService.CreateDistrict(District);
                }
                else
                {
                    District = DistrictService.UpdateDistrict(District);
                }

                if (District.Message != AppConstants.Common.SUCCESS)
                {
                    ModelState.AddModelError("error_msg", District.Message);
                }
                else
                {
                    return Json(District);
                }

                District.Message = "";
                return PartialView(District);
            }

            District.Message = EduSuiteResourceManager.GetApplicationString(AppConstants.Common.FAILED);
            return PartialView(District);

        }

        [ActionAuthenticationAttribute(MenuCode = MenuConstants.District, ActionCode = ActionConstants.Delete)]
        [HttpPost]
        public ActionResult DeleteDistrict(Int16 id)
        {
            DistrictViewModel objViewModel = new DistrictViewModel();

            objViewModel.RowKey = id;
            try
            {
                objViewModel = DistrictService.DeleteDistrict(objViewModel);
            }
            catch (Exception)
            {
                objViewModel.Message = EduSuiteResourceManager.GetApplicationString(AppConstants.Common.FAILED);
            }
            return Json(objViewModel);
        }


        [HttpGet]
        public JsonResult GetProvinceByCountry(Int16 id)
        {
            DistrictViewModel employeeContact = DistrictService.GetProvinceByCountry(id);
            return Json(employeeContact, JsonRequestBehavior.AllowGet);
        }
    }
}