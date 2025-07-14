using CITS.EduSuite.Business.Models.ViewModels;
using CITS.EduSuite.Business.Interfaces;

using CITS.EduSuite.Business.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using CITS.EduSuite.Business.Models.Resources;
using System.IO;

namespace CITS.EduSuite.UI.Controllers
{
    public class BranchController : BaseController
    {
        public IBranchService BranchService;

        public BranchController(IBranchService objBranchService)
        {
            this.BranchService = objBranchService;
        }
        [HttpGet]
        public ActionResult BranchList()
        {
            return View();
        }
        [HttpGet]
        public ActionResult AddEditBranch(short? id)
        {
            BranchViewModel model = new BranchViewModel();
            model = BranchService.GetBranchById(id);
            if (model == null)
            {
                model = new BranchViewModel();
            }
            return PartialView(model);
        }
        [HttpPost]
        public ActionResult AddEditBranch(BranchViewModel model)
        {
            if (ModelState.IsValid)
            {
                if (model.RowKey == 0)
                {
                    model = BranchService.CreateBranch(model);
                }
                else
                {
                    model = BranchService.UpdateBranch(model);
                }
                if (model.Message != AppConstants.Common.SUCCESS)
                {
                    ModelState.AddModelError("error_msg", model.Message);
                }
                else
                {
                    if (model.PhotoFile != null)
                    {
                        //UploadFile(model);
                        string Extension = Path.GetExtension(model.PhotoFile.FileName);
                        string FileName = model.BranchCode + Extension;
                        UploadFiles(model.PhotoFile, Server.MapPath(UrlConstants.BranchLogo + "/"), FileName);
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

        [HttpGet]
        public JsonResult GetBranch(string SearchText)
        {
            int page = 1; int rows = 15;
            List<BranchViewModel> BranchList = new List<BranchViewModel>();
            BranchList = BranchService.GetBranches(SearchText);
            int pageindex = Convert.ToInt32(page) - 1;
            int pagesize = rows;
            int totalrecords = BranchList.Count();
            var totalpage = (int)Math.Ceiling((float)totalrecords / (float)rows);

            var jsonData = new
            {
                total = totalpage,
                page,
                records = totalrecords,
                rows = BranchList
            };
            return Json(jsonData, JsonRequestBehavior.AllowGet);
        }
        [HttpPost]
        public ActionResult DeleteBranch(short id)
        {
            BranchViewModel objViewmodel = new BranchViewModel();
            objViewmodel.RowKey = id;
            try
            {
                objViewmodel = BranchService.DeleteBranch(objViewmodel);
            }
            catch (Exception)
            {
                objViewmodel.Message = EduSuiteUIResources.Failed;
            }
            return Json(objViewmodel);
        }

        [HttpGet]
        public JsonResult CheckBranchCodeExists(string BranchCode, short RowKey)
        {
            BranchViewModel model = new BranchViewModel();
            model.BranchCode = BranchCode;
            model.RowKey = RowKey;
            model = BranchService.CheckBranchCodeExist(model);
            return Json(model.IsSuccessful, JsonRequestBehavior.AllowGet);
        }
        [HttpGet]
        public JsonResult GetProvinceAndCodeByCountry(Int16 id)
        {
            BranchViewModel model = BranchService.GetProvinceAndCodeByCountry(id);
            return Json(model.Provinces, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public JsonResult GetDistrictByProvince(Int32 id)
        {
            BranchViewModel model = BranchService.GetDistrictByProvince(id);
            return Json(model.Districts, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult DeleteBranchLogo(short id)
        {
            BranchViewModel objViewModel = new BranchViewModel();
            objViewModel = BranchService.DeleteBranchLogo(id);
            if (objViewModel.IsSuccessful)
            {
                if (objViewModel.BranchLogoPath != null)
                {
                    DeleteFile(objViewModel.BranchLogoPath);
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