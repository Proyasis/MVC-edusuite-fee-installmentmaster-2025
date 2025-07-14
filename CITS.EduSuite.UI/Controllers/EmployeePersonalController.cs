
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
    public class EmployeePersonalController : BaseController
    {
        private IEmployeePersonalService employeePersonalService;
        private ISharedService sharedService;
        public EmployeePersonalController(IEmployeePersonalService objEmployeePersonalService,ISharedService objSharedService)
        {
            this.employeePersonalService = objEmployeePersonalService;
            this.sharedService = objSharedService;
        }

        [HttpGet]
        public ActionResult AddEditEmployeePersonal(short? id)
        {
            var employeePersonal = employeePersonalService.GetEmployeePersonalById(id);
            if (employeePersonal == null)
            {
                employeePersonal = new EmployeePersonalViewModel();
            }
            
            ViewBag.ShowEmployeeAttendance = sharedService.CheckMenuActive(MenuConstants.EmployeeAttendance);
            ViewBag.ShowEmployeeSalary = sharedService.CheckMenuActive(MenuConstants.EmployeeSalary);
            return PartialView(employeePersonal);
        }

        [HttpPost]
        public ActionResult AddEditEmployeePersonal(EmployeePersonalViewModel model)
        {

            if (ModelState.IsValid)
            {
                if (model.RowKey == 0)
                {
                    model = employeePersonalService.CreateEmployeePersonal(model);
                }
                else
                {
                    model = employeePersonalService.UpdateEmployeePersonal(model);
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
                return Json(model);
            }
            foreach (ModelState state in ViewData.ModelState.Values.Where(x => x.Errors.Count > 0))
            {
            }

            model.Message = EduSuiteUIResources.Failed;
            return Json(model);

        }

        //[HttpGet]
        //public JsonResult GetDepartmentByBranchId(Int16 id)
        //{
        //    EmployeePersonalViewModel objViewModel = new EmployeePersonalViewModel();
        //    objViewModel = employeePersonalService.GetDepartmentByBranchId(id);
        //    return Json(objViewModel, JsonRequestBehavior.AllowGet);
        //}
        [HttpGet]
        public JsonResult GetGradeByDesignationId(Int16 id)
        {
            EmployeePersonalViewModel objViewModel = new EmployeePersonalViewModel();
            objViewModel = employeePersonalService.GetGradeByDesignationId(id);
            return Json(objViewModel, JsonRequestBehavior.AllowGet);



        }

        [HttpGet]
        public JsonResult GetHigherEmployeesById(Int16? BranchId, Int16? DesignationId)
        {
            EmployeePersonalViewModel objViewModel = new EmployeePersonalViewModel();
            objViewModel.BranchKey = BranchId ?? 0;
            objViewModel.DesignationKey = DesignationId ?? 0;
            objViewModel = employeePersonalService.GetHigherEmployeesById(objViewModel);
            return Json(objViewModel, JsonRequestBehavior.AllowGet);



        }

        [HttpGet]
        public JsonResult CheckEmployeeCodeExists(string EmployeeCode, long rowKey)
        {
            EmployeePersonalViewModel employeePersonal = new EmployeePersonalViewModel();
            employeePersonal = employeePersonalService.CheckEmployeeCodeExists(EmployeeCode, rowKey);
            return Json(employeePersonal.IsSuccessful, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public JsonResult CheckMobileNumberExists(string MobileNumber, long rowKey)
        {
            EmployeePersonalViewModel employeePersonal = new EmployeePersonalViewModel();
            employeePersonal = employeePersonalService.CheckMobileNumberExists(MobileNumber, rowKey);
            return Json(employeePersonal.IsSuccessful, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public JsonResult CheckEmailAddressExists(string EmailAddress, long RowKey)
        {
            EmployeePersonalViewModel employeePersonal = new EmployeePersonalViewModel();
            employeePersonal = employeePersonalService.CheckEmailAddressExists(EmailAddress, RowKey);
            return Json(employeePersonal.IsSuccessful, JsonRequestBehavior.AllowGet);
        }


        [HttpPost]
        public JsonResult EmailAddressConfirmation(EmailViewModel template, ContactVerificationViewModel model)
        {
            try
            {
                model = employeePersonalService.UpdateContactVerification(model);
                if (model.IsSuccessful)
                {
                    EmailHelper objEmailHelper = new EmailHelper();
                    objEmailHelper.SendMail(template);
                    model.IsSuccessful = true;
                }
                return Json(model.IsSuccessful);
            }
            catch (Exception ex)
            {
                return Json(false);
            }

        }

        [HttpGet]
        [EncryptedActionParameter]
        public ActionResult ConfirmEmailAddress(string Code, long Id)
        {
            ContactVerificationViewModel model = new ContactVerificationViewModel();
            model.EmployeeKey = Id;
            model.EmailVerificationCode = Code;
            model = employeePersonalService.ConfirmContactVerification(model);
            if (model.IsSuccessful)
            {
                EmailViewModel objEmailViewModel = new EmailViewModel();
                objEmailViewModel.EmailTo = model.EmailAddress;
                objEmailViewModel.EmailSubject = "Verification Successfull";
                objEmailViewModel.EmailBody = "Your email verification has been successfull ! ";
                EmailHelper objEmailHelper = new EmailHelper();
                objEmailHelper.SendMail(objEmailViewModel);
            }
            return Content("<h3>Your email verification has been successfull !.</h3>");
        }

        [HttpGet]
        public JsonResult ConfirmMobileNumber(string Code, long Id)
        {
            ContactVerificationViewModel model = new ContactVerificationViewModel();
            model.EmployeeKey = Id;
            model.SMSVerificationCode = Code;
            model = employeePersonalService.ConfirmContactVerification(model);
            return Json(model, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult MobileNumberConfirmation(ContactVerificationViewModel model)
        {
            try
            {

                model = employeePersonalService.UpdateContactVerification(model);
                return Json(model);
            }
            catch (Exception ex)
            {
                return Json(false);
            }

        }

        [HttpGet]
        public JsonResult GetGradeDetailsById(Int32 id)
        {
            EmployeePersonalViewModel objViewModel = new EmployeePersonalViewModel();
            objViewModel = employeePersonalService.GetGradeDetailsById(id);
            return Json(objViewModel, JsonRequestBehavior.AllowGet);



        }
    }
}