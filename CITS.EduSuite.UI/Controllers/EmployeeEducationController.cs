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
using Newtonsoft.Json;

namespace CITS.EduSuite.UI.Controllers
{
    public class EmployeeEducationController : BaseController
    {

        private IEmployeeEducationService employeeEducationService;

        public EmployeeEducationController(IEmployeeEducationService objEmployeeEducationService)
        {
            this.employeeEducationService = objEmployeeEducationService;
        }



        [HttpGet]
        public ActionResult AddEditEmployeeEducation(long? id)
        {
            long EmployeeKey = id != null ? Convert.ToInt64(id) : 0;
            var employeeEducation = employeeEducationService.GetEmployeeEducationById(EmployeeKey);
            if (employeeEducation == null)
            {
                employeeEducation = new EmployeeEducationViewModel();
            }
            return PartialView(employeeEducation);
        }

        [HttpPost]
        public ActionResult AddEditEmployeeEducation(EmployeeEducationViewModel model)
        {

            if (ModelState.IsValid)
            {
                UpdateEducationModel(model);

                model = employeeEducationService.UpdateEmployeeEducation(model);

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
        public ActionResult DeleteEmployeeEducation(Int64 id)
        {
            EducationViewModel objViewModel = new EducationViewModel();
            EmployeeEducationViewModel objEmployeeEducationViewModel = new EmployeeEducationViewModel();
            objViewModel.RowKey = id;
            try
            {
                objEmployeeEducationViewModel = employeeEducationService.DeleteEmployeeEducation(objViewModel);
                if (objEmployeeEducationViewModel.IsSuccessful)
                {
                    DeleteFile(UrlConstants.EmployeeUrl + objEmployeeEducationViewModel.EmployeeKey + "/Education/" + objEmployeeEducationViewModel.EmployeeEducations[0].AttanchedFileNamePath);
                }
            }
            catch (Exception)
            {
                objEmployeeEducationViewModel.Message = EduSuiteUIResources.Failed;
            }
            return Json(objEmployeeEducationViewModel);
        }


        [HttpGet]
        public JsonResult CheckEducationTypeExists(short EducationTypeKey, long EmployeeKey, long rowKey)
        {
            EmployeeEducationViewModel employeeEducation = new EmployeeEducationViewModel();
            employeeEducation = employeeEducationService.CheckEducationTypeExists(EducationTypeKey, EmployeeKey, rowKey);
            return Json(employeeEducation.IsSuccessful, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public FileContentResult DownloadFile(string filename)
        {
            if (System.IO.File.Exists(Server.MapPath(filename)))
            {
                string FullPath = Server.MapPath(filename);
                return File(System.IO.File.ReadAllBytes(FullPath), System.Web.MimeMapping.GetMimeMapping(FullPath), EduSuiteUIResources.Education + "_" + Path.GetFileName(FullPath));
            }
            else
            {
                return null;
            }
        }

        private void UploadFile(EmployeeEducationViewModel employeeEducationModel)
        {
            string FilePath = Server.MapPath(UrlConstants.EmployeeUrl + employeeEducationModel.EmployeeKey + "/Education/");
            if (!Directory.Exists(FilePath))
            {
                Directory.CreateDirectory(FilePath);
            }
            foreach (EducationViewModel model in employeeEducationModel.EmployeeEducations)
            {
                if (model.AttanchedFile != null)
                {
                    model.AttanchedFile.SaveAs(FilePath + model.AttanchedFileName);
                    model.AttanchedFile = null;
                }
            }
        }

        private void UpdateEducationModel(EmployeeEducationViewModel model)
        {
            for (int i = 0; i < model.EmployeeEducations.Count; i++)
            {
                model.EmployeeEducations[i].AttanchedFile = Request.Files["EmployeeEducations[" + i + "][AttanchedFile]"];
                if (model.EmployeeEducations[i].AttanchedFile != null)
                {
                    model.EmployeeEducations[i].AttanchedFileName = Path.GetExtension(model.EmployeeEducations[i].AttanchedFile.FileName);
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