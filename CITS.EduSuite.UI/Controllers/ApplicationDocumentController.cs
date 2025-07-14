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
    public class ApplicationDocumentController : BaseController
    {
         private IApplicationDocumentService applicationDocumentService;

         public ApplicationDocumentController(IApplicationDocumentService objApplicationDocumentService)
        {
            this.applicationDocumentService = objApplicationDocumentService;
        }

        [HttpGet]
        public ActionResult AddEditApplicationDocument(long? id)
        {
            long ApplicationKey = id != null ? Convert.ToInt64(id) : 0;
            var applicationIdenties = applicationDocumentService.GetApplicationDocumentsById(ApplicationKey);
            if (applicationIdenties == null)
            {
                applicationIdenties = new ApplicationDocumentViewModel();
            }
            return PartialView(applicationIdenties);
        }

        [HttpPost]
        public ActionResult AddEditApplicationDocument(ApplicationDocumentViewModel model)
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

            model.Message = EduSuiteUIResources.Failed;
            return Json(model);

        }

        [HttpPost]
        public ActionResult DeleteApplicationDocument(Int64 id)
        {
            DocumentViewModel objViewModel = new DocumentViewModel();
            ApplicationDocumentViewModel objApplicationDocumentViewModel = new ApplicationDocumentViewModel();
            objViewModel.RowKey = id;
            try
            {
                objApplicationDocumentViewModel = applicationDocumentService.DeleteApplicationDocument(objViewModel);
                if(objApplicationDocumentViewModel.IsSuccessful)
                {
                    DeleteFile(UrlConstants.ApplicationUrl + objApplicationDocumentViewModel.ApplicationKey + "/" + objApplicationDocumentViewModel.ApplicationDocuments[0].StudentDocumentPath);
                }
            }
            catch (Exception)
            {
                objApplicationDocumentViewModel.Message = EduSuiteUIResources.Failed;
            }
            return Json(objApplicationDocumentViewModel);
        }

        //[HttpGet]
        //public JsonResult CheckDocumentTypeExists(short DocumentTypeKey, long ApplicationKey, long RowKey)
        //{
        //    ApplicationDocumentViewModel applicationDocument = new ApplicationDocumentViewModel();
        //    applicationDocument = applicationDocumentService.CheckDocumentTypeExists(DocumentTypeKey, ApplicationKey, RowKey);
        //    return Json(applicationDocument.IsSuccessful, JsonRequestBehavior.AllowGet);
        //}

        

        //[HttpGet]
        //public FileContentResult DownloadFile(string filename)
        //{
        //    string FullPath = Server.MapPath(filename);
        //    return File(System.IO.File.ReadAllBytes(FullPath), System.Web.MimeMapping.GetMimeMapping(FullPath), EduSuite.UI.Resources.EduSuiteUIResources.Document + "_" + Path.GetFileName(FullPath));
        //}

        private void UploadFile(ApplicationDocumentViewModel applicationDocumentModel)
        {
            string FilePath = Server.MapPath(UrlConstants.ApplicationUrl + applicationDocumentModel.ApplicationKey + "/Document/");
            if (!Directory.Exists(FilePath))
            {
                Directory.CreateDirectory(FilePath);
            }
            foreach (DocumentViewModel model in applicationDocumentModel.ApplicationDocuments)
            {
                if (model.DocumentFile != null)
                {

                    model.DocumentFile.SaveAs(FilePath + model.StudentDocumentPath);
                    model.DocumentFile = null;
                }
            }
        }

        private void UpdateDocumentModel(ApplicationDocumentViewModel model)
        {
            for (int i = 0; i < model.ApplicationDocuments.Count; i++)
            {
                model.ApplicationDocuments[i].DocumentFile = Request.Files["ApplicationDocuments[" + i + "][DocumentFile]"];
                if (model.ApplicationDocuments[i].DocumentFile != null)
                {
                    model.ApplicationDocuments[i].StudentDocumentPath = Path.GetExtension(model.ApplicationDocuments[i].DocumentFile.FileName);
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