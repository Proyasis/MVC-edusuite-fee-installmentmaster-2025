using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using CITS.EduSuite.Business.Common;
using CITS.EduSuite.Business.Interfaces;
using CITS.EduSuite.Business.Models.ViewModels;
using CITS.EduSuite.Business.Provider;
using System.Web.Security;
using CITS.EduSuite.Business.Models.Security;
using CITS.EduSuite.Business.Models.Resources;
using System.IO;

namespace CITS.EduSuite.UI.Controllers
{
    public class UniversityCertificateController : BaseController
    {
        private IUniversityCertificateService universityCertificateService;
        private ISelectListService selectListService;
        public UniversityCertificateController(IUniversityCertificateService objuniversityCertificate, ISelectListService objselectListService)
        {
            this.universityCertificateService = objuniversityCertificate;
            this.selectListService = objselectListService;
        }

        [ActionAuthenticationAttribute(MenuCode = MenuConstants.UniversityCertificate, ActionCode = ActionConstants.View)]
        [HttpGet]
        public ActionResult UniversityCertificateList()
        {
            ApplicationViewModel model = new ApplicationViewModel();
            model.Branches = selectListService.FillBranches();
            model.Batches = selectListService.FillSearchBatch(model.BranchKey);
            model.Courses = selectListService.FillSearchCourse(model.BranchKey);
            model.Universities = selectListService.FillSearchUniversity(model.BranchKey);
            return View(model);
        }

        [HttpGet]
        public JsonResult GetApplications(string ApplicantName, string MobileNumber, short? BranchKey, long? CourseKey, short? UniversityKey, short? BatchKey, string sidx, string sord, int page, int rows)
        {

            long TotalRecords = 0;
            List<ApplicationViewModel> applicationList = new List<ApplicationViewModel>();
            ApplicationViewModel objViewModel = new ApplicationViewModel();

            objViewModel.ApplicantName = ApplicantName??"";
            objViewModel.MobileNumber = MobileNumber ?? "";
            objViewModel.BranchKey = BranchKey ?? 0;
            objViewModel.BatchKey = BatchKey ?? 0;
            objViewModel.CourseKey = CourseKey ?? 0;
            objViewModel.UniversityKey = UniversityKey ?? 0;
            objViewModel.PageIndex = page;
            objViewModel.PageSize = rows;
            objViewModel.SortBy = sidx;
            objViewModel.SortOrder = sord;

            applicationList = universityCertificateService.GetApplications(objViewModel, out TotalRecords);

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

        [ActionAuthenticationAttribute(MenuCode = MenuConstants.UniversityCertificate, ActionCode = ActionConstants.AddEdit)]
        [HttpGet]
        public ActionResult AddEditUniversityCertificate(long? id)
        {
            long ApplicationKey = id != null ? Convert.ToInt64(id) : 0;
            var applicationIdenties = universityCertificateService.GetUniversityCertificateById(ApplicationKey);
            if (applicationIdenties == null)
            {
                applicationIdenties = new UniversityCertificateViewModel();
            }
            return PartialView(applicationIdenties);
        }

        [ActionAuthenticationAttribute(MenuCode = MenuConstants.UniversityCertificate, ActionCode = ActionConstants.AddEdit)]
        [HttpPost]
        public ActionResult AddEditUniversityCertificate(UniversityCertificateViewModel model)
        {
            if (ModelState.IsValid)
            {
                UpdateDocumentModel(model);
                model = universityCertificateService.UpdateUniversityCertificate(model);

                if (model.Message != AppConstants.Common.SUCCESS)
                {
                    ModelState.AddModelError("error_msg", model.Message);
                }
                else
                {
                    UploadFile(model);
                    return Json(model);
                    //return RedirectToAction("UniversityCertificateList");
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

        [ActionAuthenticationAttribute(MenuCode = MenuConstants.UniversityCertificate, ActionCode = ActionConstants.Delete)]
        public ActionResult ResetUniversityCertificate(long Id)
        {
            UniversityCertificateViewModel objviewModel = new UniversityCertificateViewModel();

            try
            {
                objviewModel = universityCertificateService.DeleteUniversityCertificate(Id);
                if (objviewModel.IsSuccessful)
                {
                    DeleteFile(UrlConstants.ApplicationUrl + objviewModel.ApplicationKey + "/AffiliatedCertificates/" + objviewModel.UniversityCertificateDetails[0].CertificatePath);
                }
            }
            catch (Exception)
            {
                objviewModel.Message = EduSuiteUIResources.Failed;
            }
            return Json(objviewModel);

        }

        [HttpGet]
        public JsonResult CheckCertificateTypeExists(short CertificateTypeKey, long RowKey)
        {
            return Json(true, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public JsonResult FillBatch(short? BranchKey)
        {
            UniversityCertificateViewModel model = new UniversityCertificateViewModel();
            model.BranchKey = BranchKey ?? 0;
            model.Batches = selectListService.FillBatches(model.BranchKey);
            return Json(model.Batches, JsonRequestBehavior.AllowGet);
        }

        private void UploadFile(UniversityCertificateViewModel model)
        {
            string FilePath = Server.MapPath(UrlConstants.ApplicationUrl + model.ApplicationKey + "/AffiliatedCertificates/");
            if (!Directory.Exists(FilePath))
            {
                Directory.CreateDirectory(FilePath);
            }
            foreach (UniversityCertificateDetails item in model.UniversityCertificateDetails)
            {
                if (item.DocumentFile != null)
                {
                    item.DocumentFile.SaveAs(FilePath + item.CertificatePath);
                    item.DocumentFile = null;
                }
            }
        }

        private void UpdateDocumentModel(UniversityCertificateViewModel model)
        {
            for (int i = 0; i < model.UniversityCertificateDetails.Count; i++)
            {
                model.UniversityCertificateDetails[i].DocumentFile = Request.Files["UniversityCertificateDetails[" + i + "][DocumentFile]"];
                if (model.UniversityCertificateDetails[i].DocumentFile != null)
                {
                    model.UniversityCertificateDetails[i].CertificatePath = Path.GetExtension(model.UniversityCertificateDetails[i].DocumentFile.FileName);
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