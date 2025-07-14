using CITS.EduSuite.Business.Common;
using CITS.EduSuite.Business.Interfaces;
using CITS.EduSuite.Business.Models.Resources;
using CITS.EduSuite.Business.Models.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace CITS.EduSuite.UI.Controllers
{
    public class CertificateTypeController : BaseController
    {
        // GET: ExamCenter
        private ICertificateTypeService CertificateTypeService;
        public CertificateTypeController(ICertificateTypeService objCertificateTypeService)
        {
            this.CertificateTypeService = objCertificateTypeService;
        }
        [HttpGet]
        public ActionResult CertificateTypeList()
        {
            return View();
        }
        [HttpGet]
        public ActionResult AddEditCertificateType(short? id)
        {
            CertificateTypeViewModel model = new CertificateTypeViewModel();
            model = CertificateTypeService.GetCertificateTypeById(id);
            if (model == null)
            {
                model = new CertificateTypeViewModel();
            }
            return PartialView(model);
        }


        [HttpPost]
        public ActionResult AddEditCertificateType(CertificateTypeViewModel model)
        {

            if (ModelState.IsValid)
            {
                if (model.RowKey == 0)
                {
                    model = CertificateTypeService.CreateCertificateType(model);
                }
                else
                {
                    model = CertificateTypeService.UpdateCertificateType(model);
                }

                if (model.Message != AppConstants.Common.SUCCESS)
                {
                    ModelState.AddModelError("error_msg", model.Message);
                }
                else
                {
                    return Json(model);
                }

                model.Message = "";
                return PartialView(model);
            }



            model.Message = EduSuiteUIResources.Failed;

            return PartialView(model);
        }
        public JsonResult GetCertificateType(string searchText)
        {
            int page = 1, rows = 15;
            List<CertificateTypeViewModel> CertificateTypeList = new List<CertificateTypeViewModel>();
            CertificateTypeList = CertificateTypeService.GetCertificateType(searchText);
            int pageIndex = Convert.ToInt32(page) - 1;
            int pageSize = rows;
            int toatalRecords = CertificateTypeList.Count();
            var totalPage = (int)Math.Ceiling((float)toatalRecords / (float)rows);

            var jsonData = new
            {
                total = totalPage,
                page,
                records = toatalRecords,
                rows = CertificateTypeList
            };
            return Json(jsonData, JsonRequestBehavior.AllowGet);
        }
        [HttpPost]
        public ActionResult DeleteCertificateType(short id)
        {
            CertificateTypeViewModel objViewModel = new CertificateTypeViewModel();

            objViewModel.RowKey = id;
            try
            {
                objViewModel = CertificateTypeService.DeleteCertificateType(objViewModel);
            }
            catch (Exception)
            {
                objViewModel.Message = EduSuiteUIResources.Failed;
            }
            return Json(objViewModel);
        }

        [HttpGet]
        public JsonResult CheckCertificateTypeNameExists(string CertificateTypeName, short RowKey)
        {
            CertificateTypeViewModel model = new CertificateTypeViewModel();
            model.RowKey = RowKey;
            model.CertificateTypeName = CertificateTypeName;
            model = CertificateTypeService.CheckCertificateTypeNameExists(model);
            return Json(model.IsSuccessful, JsonRequestBehavior.AllowGet);
        }
    }
}