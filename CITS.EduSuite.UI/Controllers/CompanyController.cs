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
    public class CompanyController : BaseController
    {
        public ICompanyService CompanyService;

        public CompanyController(ICompanyService objCompanyService)
        {
            this.CompanyService = objCompanyService;
        }
        [HttpGet]
        public ActionResult CompanyList()
        {
            return View();
        }

        [HttpGet]
        public JsonResult GetCompany(string SearchText)
        {
            int page = 1; int rows = 15;
            List<CompanyViewModel> CompanyList = new List<CompanyViewModel>();
            CompanyList = CompanyService.GetCompanies(SearchText);
            int pageindex = Convert.ToInt32(page) - 1;
            int pagesize = rows;
            int totalrecords = CompanyList.Count();
            var totalpage = (int)Math.Ceiling((float)totalrecords / (float)rows);

            var jsonData = new
            {
                total = totalpage,
                page,
                records = totalrecords,
                rows = CompanyList
            };
            return Json(jsonData, JsonRequestBehavior.AllowGet);
        }
        [HttpGet]
        public ActionResult AddEditCompany(short? id)
        {
            CompanyViewModel model = new CompanyViewModel();
            model = CompanyService.GetCompanyById(id);
            if (model == null)
            {
                model = new CompanyViewModel();
            }
            return PartialView(model);
        }
        [HttpPost]
        public ActionResult AddEditCompany(CompanyViewModel model)
        {
            if (ModelState.IsValid)
            {

                model = CompanyService.UpdateCompany(model);

                if (model.Message != AppConstants.Common.SUCCESS)
                {
                    ModelState.AddModelError("error_msg", model.Message);
                }
                else
                {
                    if (model.PhotoFile != null)
                    {
                        string Extension = Path.GetExtension(model.PhotoFile.FileName);
                        string FileName = model.CompanyName + Extension;
                        UploadFiles(model.PhotoFile, Server.MapPath(UrlConstants.CompanyLogo + "/"), FileName);
                    }
                    return Json(model);
                }

                return Json(model);
            }
            model.Message = EduSuiteUIResources.Failed;
            return Json(model);
        }

        private void UploadFiles(HttpPostedFileBase PhotoFile, string FilePath, string FileName)
        {
            if (!Directory.Exists(FilePath))
            {
                Directory.CreateDirectory(FilePath);
            }
            if (PhotoFile != null)
            {
                PhotoFile.SaveAs(FilePath + FileName);
            }
        }
        [HttpPost]
        public ActionResult DeleteCompanyLogo(short id)
        {
            CompanyViewModel objViewModel = new CompanyViewModel();
            objViewModel = CompanyService.DeleteCompanyLogo(id);
            if (objViewModel.IsSuccessful)
            {
                if (objViewModel.CompanyLogoPath != null)
                {
                    DeleteFile(objViewModel.CompanyLogoPath);
                }

            }
            return Json(objViewModel);
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