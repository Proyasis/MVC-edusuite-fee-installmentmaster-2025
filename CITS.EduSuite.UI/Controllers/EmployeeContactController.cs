using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Helpers;
using System.Web.Mvc;
using CITS.EduSuite.Business.Common;
using CITS.EduSuite.Business.Interfaces;
using CITS.EduSuite.Business.Models.Resources;
using CITS.EduSuite.Business.Models.ViewModels;



namespace CITS.EduSuite.UI.Controllers
{
    public class EmployeeContactController : BaseController
    {
        private IEmployeeContactService employeeContactService;

        public EmployeeContactController(IEmployeeContactService objEmployeeContactService)
        {
            this.employeeContactService = objEmployeeContactService;
        }

        [HttpGet]
        public ActionResult AddEditEmployeeContact(long? id)
        {
            var employeeContact = employeeContactService.GetEmployeeContactById(id ?? 0);
            if (employeeContact == null)
            {
                employeeContact = new EmployeeContactViewModel();
            }
            return PartialView(employeeContact);
        }

        [HttpPost]
        public ActionResult AddEditEmployeeContact(EmployeeContactViewModel model)
        {
            IEnumerable<ModelError> allErrors = ModelState.Values.SelectMany(v => v.Errors);
            if (ModelState.IsValid)
            {

                model = employeeContactService.UpdateEmployeeContact(model);

                if (model.Message != AppConstants.Common.SUCCESS)
                {
                    ModelState.AddModelError("error_msg", model.Message);
                }
                else
                {
                    return Json(model);
                }

                model.Message = "";
                return Json(model);
            }

            model.Message = EduSuiteUIResources.Failed;
            return Json(model);

        }



        [HttpPost]
        public ActionResult DeleteEmployeeContact(Int64 id)
        {
            ContactViewModel objViewModel = new ContactViewModel();
            EmployeeContactViewModel objEmployeeContactViewModel = new EmployeeContactViewModel();
            objViewModel.RowKey = id;
            try
            {
                objEmployeeContactViewModel = employeeContactService.DeleteEmployeeContact(objViewModel);
            }
            catch (Exception)
            {
                objEmployeeContactViewModel.Message = EduSuiteUIResources.Failed;
            }
            return Json(objEmployeeContactViewModel);
        }


        [HttpGet]
        public JsonResult CheckAddressTypeExists(short AddressTypeKey, long EmployeeKey, long rowKey)
        {
            EmployeeContactViewModel employeeContact = new EmployeeContactViewModel();
            employeeContact = employeeContactService.CheckAddressTypeExists(AddressTypeKey, EmployeeKey, rowKey);
            return Json(employeeContact.IsSuccessful, JsonRequestBehavior.AllowGet);
        }


        [HttpGet]
        public JsonResult GetProvinceByCountry(Int16 id)
        {
            ContactViewModel model = new ContactViewModel();
            model.CountryKey = id;
            model = employeeContactService.GetProvinceByCountry(model);
            return Json(model.Provinces, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public JsonResult GetDistrictByProvince(Int32 id)
        {
            ContactViewModel model = new ContactViewModel();
            model.ProvinceKey = id;
            model = employeeContactService.GetDistrictByProvince(model);
            return Json(model.Districts, JsonRequestBehavior.AllowGet);
        }




    }
}