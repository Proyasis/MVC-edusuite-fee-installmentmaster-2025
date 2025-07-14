using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using CITS.EduSuite.Business.Common;
using CITS.EduSuite.Business.Interfaces;
using CITS.EduSuite.Business.Models.Resources;
using CITS.EduSuite.Business.Models.ViewModels;


namespace CITS.EduSuite.UI.Controllers
{
    public class ApplicationEducationalDetailsController : BaseController
    {
        private IApplicationEducationalDetailsService applicationDocumentService;

        public ApplicationEducationalDetailsController(IApplicationEducationalDetailsService objApplicationDocumentService)
        {
            this.applicationDocumentService = objApplicationDocumentService;
        }

        [HttpGet]
        public ActionResult AddEditApplicationEducationalDetails(long? id)
        {
            long ApplicationKey = id != null ? Convert.ToInt64(id) : 0;
            var applicationIdenties = applicationDocumentService.GetApplicationDocumentsById(ApplicationKey);
            if (applicationIdenties == null)
            {
                applicationIdenties = new ApplicationEducationDetailsViewModel();
            }
            return PartialView(applicationIdenties);
        }

        [HttpPost]
        public ActionResult AddEditApplicationEducationalDetails(ApplicationEducationDetailsViewModel model)
        {
            if (ModelState.IsValid)
            {
                UpdateDocumentModel(model);
                model = applicationDocumentService.UpdateApplicationDocument(model);

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
        public ActionResult DeleteApplicationDocument(Int64 id)
        {
            EducationDetailViewModel objViewModel = new EducationDetailViewModel();
            ApplicationEducationDetailsViewModel objApplicationDocumentViewModel = new ApplicationEducationDetailsViewModel();
            objViewModel.RowKey = id;
            try
            {
                objApplicationDocumentViewModel = applicationDocumentService.DeleteApplicationDocument(objViewModel);
                if(objApplicationDocumentViewModel.IsSuccessful)
                {
                    DeleteFile(UrlConstants.ApplicationUrl + objApplicationDocumentViewModel.ApplicationKey + "/EducationQualification/" + objApplicationDocumentViewModel.ApplicationEducationalDetails[0].EducationQualificationCertificatePath);
                }
            }
            catch (Exception Ex)
            {
                objApplicationDocumentViewModel.Message = EduSuiteUIResources.Failed;
            }
            return Json(objApplicationDocumentViewModel);
        }
             

        private void UploadFile(ApplicationEducationDetailsViewModel applicationDocumentModel)
        {
            string FilePath = Server.MapPath(UrlConstants.ApplicationUrl + applicationDocumentModel.ApplicationKey + "/EducationQualification/");
            if (!Directory.Exists(FilePath))
            {
                Directory.CreateDirectory(FilePath);
            }
            foreach (EducationDetailViewModel model in applicationDocumentModel.ApplicationEducationalDetails)
            {
                if (model.DocumentFile != null)
                {

                    model.DocumentFile.SaveAs(FilePath + model.EducationQualificationCertificatePath);
                    model.DocumentFile = null;
                }
            }
        }

        private void UpdateDocumentModel(ApplicationEducationDetailsViewModel model)
        {
            for (int i = 0; i < model.ApplicationEducationalDetails.Count; i++)
            {
                model.ApplicationEducationalDetails[i].DocumentFile = Request.Files["ApplicationEducationalDetails[" + i + "][DocumentFile]"];
                if (model.ApplicationEducationalDetails[i].DocumentFile != null)
                {
                    model.ApplicationEducationalDetails[i].EducationQualificationCertificatePath = Path.GetExtension(model.ApplicationEducationalDetails[i].DocumentFile.FileName);
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

        [HttpGet]
        public FileContentResult DownloadFile(string filename)
        {
            string FullPath = Server.MapPath(filename);
            return File(System.IO.File.ReadAllBytes(FullPath), System.Web.MimeMapping.GetMimeMapping(FullPath), EduSuiteUIResources.Document + "_" + Path.GetFileName(FullPath));
        }
    }
}