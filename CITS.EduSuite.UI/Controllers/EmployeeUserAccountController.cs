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
    public class EmployeeUserAccountController : BaseController
    {
        private IEmployeeUserAccountService EmployeeUserAccountService;

        public EmployeeUserAccountController(IEmployeeUserAccountService objEmployeeUserAccountService)
        {
            this.EmployeeUserAccountService = objEmployeeUserAccountService;
        }

        [ActionAuthenticationAttribute(MenuCode = MenuConstants.Employee, ActionCode = ActionConstants.UserAccount)]
        [HttpGet]
        [EncryptedActionParameter]
        public ActionResult AddEditEmployeeUserAccount(long? id)
        {
            long EmployeeKey = id != null ? Convert.ToInt64(id) : 0;
            var EmployeeUserAccount = EmployeeUserAccountService.GetEmployeeUserAccountById(EmployeeKey);
            if (EmployeeUserAccount == null)
            {
                EmployeeUserAccount = new EmployeeUserAccountViewModel();
            }
            return PartialView(EmployeeUserAccount);
        }

        [HttpPost]
        public ActionResult AddEditEmployeeUserAccount(EmployeeUserAccountViewModel model)
        {
            ModelState.Where(row=>row.Key=="Password").ToList().ForEach(row=>ModelState[row.Key].Errors.Clear());
            if (ModelState.IsValid)
            {
                if (model.RowKey == 0)
                {
                    model = EmployeeUserAccountService.CreateEmployeeUserAccount(model);
                }
                else
                {
                    model = EmployeeUserAccountService.UpdateEmployeeUserAccount(model);
                }

                if (model.Message != AppConstants.Common.SUCCESS)
                {
                    ModelState.AddModelError("error_msg", model.Message);
                }
                else
                {
                    return Json(model);
                }


                return Json(model);
            }

            model.Message = EduSuiteUIResources.Failed;
            return Json(model);

        }

        [HttpGet]
        public JsonResult CheckUserNameExists(string UserName, int RowKey)
        {
            EmployeeUserAccountViewModel EmployeeUserAccount = new EmployeeUserAccountViewModel();
            EmployeeUserAccount = EmployeeUserAccountService.CheckUserNameExists(UserName, RowKey);
            return Json(EmployeeUserAccount.IsSuccessful, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult DeleteEmployeeUserAccount(Int64 id)
        {
            EmployeeUserAccountViewModel objViewModel = new EmployeeUserAccountViewModel();

            objViewModel.EmployeeKey = id;
            try
            {
                objViewModel = EmployeeUserAccountService.DeleteEmployeeUserAccount(objViewModel);
            }
            catch (Exception)
            {
                objViewModel.Message = EduSuiteUIResources.Failed;
            }
            return Json(objViewModel);




        }
    }
}