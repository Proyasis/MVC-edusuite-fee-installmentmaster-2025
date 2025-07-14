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
    public class ApplicationFeeInstallmentController : BaseController
    {
        private IApplicationFeeInstallmentService applicationFeeInstallmentservice;

        public ApplicationFeeInstallmentController(IApplicationFeeInstallmentService objApplicationFeeinstallmentService)
        {
            this.applicationFeeInstallmentservice = objApplicationFeeinstallmentService;
        }

        [HttpGet]
        public ActionResult AddEditApplicationFeeInstallment(long? id)
        {
            ApplicationFeeInstallmentViewModel objViewModel = new ApplicationFeeInstallmentViewModel();
            objViewModel.ApplicationKey = id ?? 0;
            var applicationIdenties = applicationFeeInstallmentservice.GetFeeInstallmentById(objViewModel);
            if (applicationIdenties == null)
            {
                applicationIdenties = new ApplicationFeeInstallmentViewModel();
            }
            return PartialView(applicationIdenties);
        }

        [HttpPost]
        public ActionResult AddEditApplicationFeeInstallment(ApplicationFeeInstallmentViewModel model)
        {

            if (ModelState.IsValid)
            {

                model = applicationFeeInstallmentservice.UpdateFeeInstallment(model);

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

        [HttpPost]
        public ActionResult DeleteApplicationFeeInstallment(Int64 id)
        {
            FeeInstallmentModel objViewModel = new FeeInstallmentModel();
            ApplicationFeeInstallmentViewModel objApplicationFeeInstallmentViewModel = new ApplicationFeeInstallmentViewModel();
            objViewModel.RowKey = id;
            try
            {
                objApplicationFeeInstallmentViewModel = applicationFeeInstallmentservice.DeleteFeeInstallment(objViewModel);

            }
            catch (Exception)
            {
                objApplicationFeeInstallmentViewModel.Message = EduSuiteUIResources.Failed;
            }
            return Json(objApplicationFeeInstallmentViewModel);
        }

        [HttpGet]
        public ActionResult GetFeeAmount(long? ApplicationKey, int? FeeYear)
        {
            ApplicationFeeInstallmentViewModel objViewModel = new ApplicationFeeInstallmentViewModel();
            objViewModel.ApplicationKey = ApplicationKey ?? 0;
            objViewModel.FeeYear = FeeYear ?? 0;
            var applicationIdenties = applicationFeeInstallmentservice.GetFeeInstallmentById(objViewModel);
            if (applicationIdenties == null)
            {
                applicationIdenties = new ApplicationFeeInstallmentViewModel();
            }
            return PartialView("AddEditApplicationFeeInstallment", applicationIdenties);
        }



        private void DeleteFile(string FilePath)
        {
            if (!System.IO.File.Exists(Server.MapPath(FilePath)))
            {
                System.IO.File.Delete(Server.MapPath(FilePath));
            }

        }
    }
}