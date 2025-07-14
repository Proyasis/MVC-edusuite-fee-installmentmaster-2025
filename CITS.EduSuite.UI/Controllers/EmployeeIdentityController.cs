using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using CITS.EduSuite.Business.Common;
using CITS.EduSuite.Business.Interfaces;
using CITS.EduSuite.Business.Models.Common;
using CITS.EduSuite.Business.Models.Resources;
using CITS.EduSuite.Business.Models.ViewModels;


namespace CITS.EduSuite.UI.Controllers
{
    public class EmployeeIdentityController : BaseController
    {
        private IEmployeeIdentityService employeeIdentityService;

        public EmployeeIdentityController(IEmployeeIdentityService objEmployeeIdentityService)
        {
            this.employeeIdentityService = objEmployeeIdentityService;
        }

        [HttpGet]
        public ActionResult AddEditEmployeeIdentity(long? id)
        {
            long EmployeeKey = id != null ? Convert.ToInt64(id) : 0;
            var employeeIdenties = employeeIdentityService.GetEmployeeIdentitiesById(EmployeeKey);
            if (employeeIdenties == null)
            {
                employeeIdenties = new EmployeeIdentityViewModel();
            }
            return PartialView(employeeIdenties);
        }

        [HttpPost]
        public ActionResult AddEditEmployeeIdentity(EmployeeIdentityViewModel model)
        {

            if (ModelState.IsValid)
            {

                UpdateIdentityModel(model);
                model = employeeIdentityService.UpdateEmployeeIdentity(model);

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

            model.Message = EduSuiteUIResources.Failed;
            return Json(model);

        }

        [HttpPost]
        public ActionResult DeleteEmployeeIdentity(Int64 id)
        {
            IdentityViewModel objViewModel = new IdentityViewModel();
            EmployeeIdentityViewModel objEmployeeIdentityViewModel = new EmployeeIdentityViewModel();
            objViewModel.RowKey = id;
            try
            {
                objEmployeeIdentityViewModel = employeeIdentityService.DeleteEmployeeIdentity(objViewModel);
                if (objEmployeeIdentityViewModel.IsSuccessful)
                {
                    DeleteFile(UrlConstants.EmployeeUrl + objEmployeeIdentityViewModel.EmployeeKey + "/Identity/" + objEmployeeIdentityViewModel.EmployeeIdentities[0].AttanchedFileNamePath);
                }
            }
            catch (Exception)
            {
                objEmployeeIdentityViewModel.Message = EduSuiteUIResources.Failed;
            }
            return Json(objEmployeeIdentityViewModel);
        }

        [HttpGet]
        public JsonResult CheckIdentityTypeExists(short identityTypeKey, long employeeKey, long rowKey)
        {
            EmployeeIdentityViewModel employeeIdentity = new EmployeeIdentityViewModel();
            employeeIdentity = employeeIdentityService.CheckIdentityTypeExists(identityTypeKey, employeeKey, rowKey);
            return Json(employeeIdentity.IsSuccessful, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public JsonResult CheckAdharNumberExists(string IdentyUniqueID, long rowKey)
        {
            EmployeeIdentityViewModel employeeIdentity = new EmployeeIdentityViewModel();
            employeeIdentity = employeeIdentityService.CheckAdharNumberExists(IdentyUniqueID, rowKey);
            return Json(employeeIdentity.IsSuccessful, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public FileContentResult DownloadFile(string filename)
        {
            if (System.IO.File.Exists(Server.MapPath(filename)))
            {
                string FullPath = Server.MapPath(filename);
                return File(System.IO.File.ReadAllBytes(FullPath), System.Web.MimeMapping.GetMimeMapping(FullPath), EduSuiteUIResources.Identity + "_" + Path.GetFileName(FullPath));
            }
            else
            {
                return null;
            }
        }

        private void UploadFile(EmployeeIdentityViewModel employeeIdentityModel)
        {
            string FilePath = Server.MapPath(UrlConstants.EmployeeUrl + employeeIdentityModel.EmployeeKey + "/Identity/");
            if (!Directory.Exists(FilePath))
            {
                Directory.CreateDirectory(FilePath);
            }
            foreach (IdentityViewModel model in employeeIdentityModel.EmployeeIdentities)
            {
                if (model.AttanchedFile != null)
                {

                    model.AttanchedFile.SaveAs(FilePath + model.AttanchedFileName);
                    model.AttanchedFile = null;
                }
            }
        }

        private void UpdateIdentityModel(EmployeeIdentityViewModel model)
        {
            for (int i = 0; i < model.EmployeeIdentities.Count; i++)
            {
                model.EmployeeIdentities[i].AttanchedFile = Request.Files["EmployeeIdentities[" + i + "][AttanchedFile]"];
                if (model.EmployeeIdentities[i].AttanchedFile != null)
                {
                    model.EmployeeIdentities[i].AttanchedFileName = Path.GetExtension(model.EmployeeIdentities[i].AttanchedFile.FileName);
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