using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Mvc;
using CITS.EduSuite.Business.Common;
using CITS.EduSuite.Business.Interfaces;
using CITS.EduSuite.Business.Models.Resources;
using CITS.EduSuite.Business.Models.ViewModels;


namespace CITS.EduSuite.UI.Controllers
{
    public class EmployeeExperienceController : BaseController
    {
        private IEmployeeExperienceService employeeExperienceService;

        public EmployeeExperienceController(IEmployeeExperienceService objEmployeeExperienceService)
        {
            this.employeeExperienceService = objEmployeeExperienceService;
        }
        [HttpGet]
        public ActionResult AddEditEmployeeExperience(long? id)
        {
            var employeeExperience = employeeExperienceService.GetEmployeeExperienceById(id ?? 0);
            if (employeeExperience == null)
            {
                employeeExperience = new EmployeeExperienceViewModel();
            }
            return PartialView(employeeExperience);
        }
        [HttpPost]
        public ActionResult AddEditEmployeeExperience(EmployeeExperienceViewModel model)
        {

            if (ModelState.IsValid)
            {

                UpdateExperienceModel(model);
                model = employeeExperienceService.UpdateEmployeeExperience(model);

                if (model.Message != AppConstants.Common.SUCCESS)
                {
                    ModelState.AddModelError("error_msg", model.Message);
                }
                else
                {
                    UploadFile(model);
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
        public ActionResult DeleteEmployeeExperience(Int64 id)
        {
            ExperienceViewModel objViewModel = new ExperienceViewModel();
            EmployeeExperienceViewModel objEmployeeExperienceViewModel = new EmployeeExperienceViewModel();
            objViewModel.RowKey = id;
            try
            {
                objEmployeeExperienceViewModel = employeeExperienceService.DeleteEmployeeExperience(objViewModel);
                if (objEmployeeExperienceViewModel.IsSuccessful)
                {
                    DeleteFile(UrlConstants.EmployeeUrl + objEmployeeExperienceViewModel.EmployeeKey + "/Experience/" + objEmployeeExperienceViewModel.EmployeeExperiences[0].AttanchedFileNamePath);
                }
            }
            catch (Exception)
            {
                objEmployeeExperienceViewModel.Message = EduSuiteUIResources.Failed;
            }
            return Json(objEmployeeExperienceViewModel);
        }
        [HttpGet]
        public FileContentResult DownloadFile(string filename)
        {
            if (System.IO.File.Exists(Server.MapPath(filename)))
            {
                string FullPath = Server.MapPath(filename);
                return File(System.IO.File.ReadAllBytes(FullPath), System.Web.MimeMapping.GetMimeMapping(FullPath), EduSuiteUIResources.Experience + "_" + Path.GetFileName(FullPath));
            }
            else
            {
                return null;
            }
        }
        private void UploadFile(EmployeeExperienceViewModel employeeExperienceModel)
        {
            string FilePath = Server.MapPath(UrlConstants.EmployeeUrl + employeeExperienceModel.EmployeeKey + "/Experience/");
            if (!Directory.Exists(FilePath))
            {
                Directory.CreateDirectory(FilePath);
            }
            foreach (ExperienceViewModel model in employeeExperienceModel.EmployeeExperiences)
            {
                if (model.AttanchedFile != null)
                {
                    model.AttanchedFile.SaveAs(FilePath + model.AttanchedFileName);
                    model.AttanchedFile = null;
                }
            }
        }
        private void UpdateExperienceModel(EmployeeExperienceViewModel model)
        {
            for (int i = 0; i < model.EmployeeExperiences.Count; i++)
            {
                model.EmployeeExperiences[i].AttanchedFile = Request.Files["EmployeeExperiences[" + i + "][AttanchedFile]"];
                if (model.EmployeeExperiences[i].AttanchedFile != null)
                {
                    model.EmployeeExperiences[i].AttanchedFileName = Path.GetExtension(model.EmployeeExperiences[i].AttanchedFile.FileName);
                }
            }
        }
        private void DeleteFile(string FilePath)
        {
            if (System.IO.File.Exists(Server.MapPath(FilePath)))
            {
                System.IO.File.Delete(Server.MapPath(FilePath));
            }

        }
    }
}