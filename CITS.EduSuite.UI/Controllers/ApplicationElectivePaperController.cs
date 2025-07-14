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
    public class ApplicationElectivePaperController : BaseController
    {
        private IApplicationElectivePaperService applicationElectivePaperservice;

        public ApplicationElectivePaperController(IApplicationElectivePaperService objApplicationElectivePaperService)
        {
            this.applicationElectivePaperservice = objApplicationElectivePaperService;
        }

        [HttpGet]
        public ActionResult AddEditApplicationElectivePaper(long? id)
        {
            long ApplicationKey = id != null ? Convert.ToInt64(id) : 0;
            var applicationIdenties = applicationElectivePaperservice.GetApplicationElectivePaperById(ApplicationKey);
            if (applicationIdenties == null)
            {
                applicationIdenties = new ApplicationElectivePaperViewModel();
            }
            return PartialView(applicationIdenties);
        }

        [HttpPost]
        public ActionResult AddEditApplicationElectivePaper(ApplicationElectivePaperViewModel model)
        {
            if (ModelState.IsValid)
            {
                model = applicationElectivePaperservice.UpdateApplicationElectivePaper(model);

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
        public ActionResult DeleteApplicationElectivePaper(Int64 id)
        {
            ElectivePaperViewModel objViewModel = new ElectivePaperViewModel();
            ApplicationElectivePaperViewModel objApplicationElectivePaperViewModel = new ApplicationElectivePaperViewModel();
            objViewModel.RowKey = id;
            try
            {
                objApplicationElectivePaperViewModel = applicationElectivePaperservice.DeleteApplicationElectivePaper(objViewModel);
               
            }
            catch (Exception)
            {
                objApplicationElectivePaperViewModel.Message = EduSuiteUIResources.Failed;
            }
            return Json(objApplicationElectivePaperViewModel);
        }
    }
}