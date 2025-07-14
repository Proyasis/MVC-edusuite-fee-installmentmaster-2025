using CITS.EduSuite.Business.Common;
using CITS.EduSuite.Business.Interfaces;
using CITS.EduSuite.Business.Models.Resources;
using CITS.EduSuite.Business.Models.ViewModels;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;



namespace CITS.EduSuite.UI.Controllers
{
    public class StudyMaterialUploadController : BaseController
    {
        private IStudentStudyMaterialService StudyMaterialService;

        public StudyMaterialUploadController(IStudentStudyMaterialService objStudyMaterialService)
        {
            this.StudyMaterialService = objStudyMaterialService;
        }

        [ActionAuthenticationAttribute(MenuCode = MenuConstants.StudentStudyMaterial, ActionCode = ActionConstants.View)]
        public ActionResult StudyMaterialList()
        {
            StudentStudyMaterialViewModel model = new StudentStudyMaterialViewModel();
            return View(model);
        }


        [ActionAuthenticationAttribute(MenuCode = MenuConstants.StudentStudyMaterial, ActionCode = ActionConstants.AddEdit)]
        [EncryptedActionParameter]
        [HttpGet]
        public ActionResult AddEditStudyMaterialUpload(long? id)
        {
            long ApplicationKey = id != null ? Convert.ToInt64(id) : 0;
            StudentStudyMaterialViewModel model = new StudentStudyMaterialViewModel();
            model.RowKey = id ?? 0;
            model = StudyMaterialService.GetStudyMaterialById(model);

            return View(model);
        }



        [HttpGet]
        public JsonResult GetStudyMaterials(string SearchText, string sidx, string sord, int page, int rows)
        {
            long TotalRecords = 0;
            List<StudentStudyMaterialViewModel> applicationList = new List<StudentStudyMaterialViewModel>();
            StudentStudyMaterialViewModel objViewModel = new StudentStudyMaterialViewModel();

            objViewModel.SearchText = SearchText;


            objViewModel.PageIndex = page;
            objViewModel.PageSize = rows;
            objViewModel.SortBy = sidx;
            objViewModel.SortOrder = sord;

            applicationList = StudyMaterialService.GetStudyMaterials(objViewModel, out TotalRecords);

            int pageIndex = Convert.ToInt32(page) - 1;
            int pageSize = rows;
            var totalPages = (int)Math.Ceiling((float)TotalRecords / (float)rows);

            var jsonData = new
            {
                total = totalPages,
                page,
                records = TotalRecords,
                rows = applicationList
            };
            return Json(jsonData, JsonRequestBehavior.AllowGet);
        }





        [HttpPost]
        public ActionResult AddEditStudyMaterialUpload(StudentStudyMaterialViewModel model)
        {

            if (ModelState.IsValid)
            {

                UpdateDocumentModel(model);


                if (model.RowKey == 0)
                {
                    model = StudyMaterialService.CreateStudyMaterial(model);
                }
                else
                {
                    model = StudyMaterialService.UpdateStudyMaterial(model);
                }

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
        public ActionResult GetStudyMaterialsByStudyMaterialId(StudentStudyMaterialViewModel model)
        {
            List<StudentStudyMaterialViewModel> List = StudyMaterialService.GetStudyMaterialByStudyMaterialKey(model);
            return PartialView(List);
        }



        //private void UpdateFileModel(StudentStudyMaterialViewModel model)
        //{
        //    int i = 0;
        //    foreach (var VideList in model.StudentStudyMaterialDetailsList)
        //    {
        //        if (Request.Files[i].ContentLength > 0)
        //        {
        //            VideList.StudyMaterialFileName = Path.GetExtension(VideList.StudyMaterialFileAttachment.FileName);
        //        }
        //        else
        //        {
        //            VideList.StudyMaterialFileName = null;
        //        }
        //        i = i + 1;
        //    }

        //}
        //private void UploadFile(StudentStudyMaterialViewModel model)
        //{
        //    string FilePath = Server.MapPath(UrlConstants.StudyMaterialUpload + model.RowKey + "/");
        //    if (!Directory.Exists(FilePath))
        //    {
        //        Directory.CreateDirectory(FilePath);
        //    }
        //    foreach (StudentStudyMaterialDetailsViewModel newModel in model.StudentStudyMaterialDetailsList)
        //    {
        //        if (newModel.StudyMaterialFileAttachment != null)
        //        {
        //            newModel.StudyMaterialFileAttachment.SaveAs(FilePath + newModel.StudyMaterialFileName);

        //        }
        //    }
        //}



        [ActionAuthenticationAttribute(MenuCode = MenuConstants.StudentStudyMaterial, ActionCode = ActionConstants.Delete)]
        [HttpPost]
        public ActionResult DeleteStudyMaterial(Int64 id)
        {
            StudentStudyMaterialViewModel objViewModel = new StudentStudyMaterialViewModel();

            objViewModel.RowKey = id;
            try
            {

                objViewModel = StudyMaterialService.DeleteStudyMaterial(objViewModel);
                if (objViewModel.IsSuccessful)
                {
                    DeleteFileFolder(UrlConstants.StudyMaterialUpload + id);
                }
            }
            catch (Exception ex)
            {
                objViewModel.Message = EduSuiteUIResources.Failed;
            }
            return Json(objViewModel);
        }



        [ActionAuthenticationAttribute(MenuCode = MenuConstants.StudentStudyMaterial, ActionCode = ActionConstants.Delete)]
        [HttpPost]
        public ActionResult DeleteStudyMaterialDetails(Int64 id)
        {
            StudentStudyMaterialViewModel objViewModel = new StudentStudyMaterialViewModel();

            try
            {
                objViewModel = StudyMaterialService.DeleteStudyMaterialDetails(id);
                if (objViewModel.IsSuccessful)
                {
                    DeleteFile(UrlConstants.StudyMaterialUpload + objViewModel.RowKey + "/" + objViewModel.StudentStudyMaterialDetailsList[0].StudyMaterialFilePath);
                }
            }
            catch (Exception ex)
            {
                objViewModel.Message = EduSuiteUIResources.Failed;
            }
            return Json(objViewModel);
        }

        private void UploadFile(StudentStudyMaterialViewModel objModel)
        {
            string FilePath = Server.MapPath(UrlConstants.StudyMaterialUpload + objModel.RowKey + "/");
            if (!Directory.Exists(FilePath))
            {
                Directory.CreateDirectory(FilePath);
            }
            foreach (StudentStudyMaterialDetailsViewModel model in objModel.StudentStudyMaterialDetailsList)
            {
                if (model.StudyMaterialFileAttachment != null)
                {

                    model.StudyMaterialFileAttachment.SaveAs(FilePath + model.StudyMaterialFilePath);
                    model.StudyMaterialFileAttachment = null;
                }
            }
        }

        private void UpdateDocumentModel(StudentStudyMaterialViewModel model)
        {
            for (int i = 0; i < model.StudentStudyMaterialDetailsList.Count; i++)
            {
                model.StudentStudyMaterialDetailsList[i].StudyMaterialFileAttachment = Request.Files["StudentStudyMaterialDetailsList[" + i + "][StudyMaterialFileAttachment]"];
                if (model.StudentStudyMaterialDetailsList[i].StudyMaterialFileAttachment != null)
                {
                    model.StudentStudyMaterialDetailsList[i].StudyMaterialFilePath = Path.GetExtension(model.StudentStudyMaterialDetailsList[i].StudyMaterialFileAttachment.FileName);
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
        private void DeleteFileFolder(string FilePath)
        {
            if (System.IO.Directory.Exists(Server.MapPath(FilePath)))
            {
                var path = Server.MapPath(FilePath);
                System.IO.Directory.Delete(path,true);
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