using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using CITS.EduSuite.Business.Interfaces;
using CITS.EduSuite.Business.Models.ViewModels;

using CITS.EduSuite.Business.Common;
using CITS.EduSuite.Business.Models.Resources;

namespace CITS.EduSuite.UI.Controllers
{
    public class ApplicationFamilyDetailsController : BaseController
    {
        private IApplicationFamilyDetailsService applicationFamilyDetailsService;

        public ApplicationFamilyDetailsController(IApplicationFamilyDetailsService objApplicationFamilyDetailsService)
        {
            this.applicationFamilyDetailsService = objApplicationFamilyDetailsService;
        }

        [HttpGet]
        public ActionResult AddEditApplicationFamilyDetails(long? id)
        {
            long ApplicationKey = id != null ? Convert.ToInt64(id) : 0;
            var applicationIdenties = applicationFamilyDetailsService.GetApplicationFamilyDetailsById(ApplicationKey);
            if (applicationIdenties == null)
            {
                applicationIdenties = new ApplicationFamilyDetailsViewModel();
            }
            return PartialView(applicationIdenties);
        }

        [HttpPost]
        public ActionResult AddEditApplicationFamilyDetails(ApplicationFamilyDetailsViewModel model)
        {
            if (ModelState.IsValid)
            {
                model = applicationFamilyDetailsService.UpdateApplicationFamilyDetails(model);

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

        [HttpPost]
        public ActionResult DeleteApplicationFamiliyDetails(long Id)
        {
            ApplicationFamilyDetailsViewModel objviewModel = new ApplicationFamilyDetailsViewModel();

            try
            {
                objviewModel = applicationFamilyDetailsService.DeleteApplicationFamilyDetails(Id);
            }
            catch (Exception)
            {
                objviewModel.Message = EduSuiteUIResources.Failed;
            }
            return Json(objviewModel);

        }

    }
}