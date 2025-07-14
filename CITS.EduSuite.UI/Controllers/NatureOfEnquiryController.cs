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
    public class NatureOfEnquiryController : BaseController
    {
        // GET: ExamCenter
        private INatureOfEnquiryService NatureOfEnquiryService;
        public NatureOfEnquiryController(INatureOfEnquiryService objNatureOfEnquiryService)
        {
            this.NatureOfEnquiryService = objNatureOfEnquiryService;
        }
        [HttpGet]
        public ActionResult NatureOfEnquiryList()
        {
            return View();
        }
        [HttpGet]
        public ActionResult AddEditNatureOfEnquiry(short? id)
        {
            NatureOfEnquiryViewModel model = new NatureOfEnquiryViewModel();
            model = NatureOfEnquiryService.GetNatureOfEnquiryById(id);
            if (model == null)
            {
                model = new NatureOfEnquiryViewModel();
            }
            return PartialView(model);
        }


        [HttpPost]
        public ActionResult AddEditNatureOfEnquiry(NatureOfEnquiryViewModel model)
        {

            if (ModelState.IsValid)
            {
                if (model.RowKey == 0)
                {
                    model = NatureOfEnquiryService.CreateNatureOfEnquiry(model);
                }
                else
                {
                    model = NatureOfEnquiryService.UpdateNatureOfEnquiry(model);
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
        public JsonResult GetNatureOfEnquiry(string searchText)
        {
            int page = 1, rows = 15;
            List<NatureOfEnquiryViewModel> NatureOfEnquiryList = new List<NatureOfEnquiryViewModel>();
            NatureOfEnquiryList = NatureOfEnquiryService.GetNatureOfEnquiry(searchText);
            int pageIndex = Convert.ToInt32(page) - 1;
            int pageSize = rows;
            int toatalRecords = NatureOfEnquiryList.Count();
            var totalPage = (int)Math.Ceiling((float)toatalRecords / (float)rows);

            var jsonData = new
            {
                total = totalPage,
                page,
                records = toatalRecords,
                rows = NatureOfEnquiryList
            };
            return Json(jsonData, JsonRequestBehavior.AllowGet);
        }
        [HttpPost]
        public ActionResult DeleteNatureOfEnquiry(short id)
        {
            NatureOfEnquiryViewModel objViewModel = new NatureOfEnquiryViewModel();

            objViewModel.RowKey = id;
            try
            {
                objViewModel = NatureOfEnquiryService.DeleteNatureOfEnquiry(objViewModel);
            }
            catch (Exception)
            {
                objViewModel.Message = EduSuiteUIResources.Failed;
            }
            return Json(objViewModel);
        }

        [HttpGet]
        public JsonResult CheckNatureOfEnquiryNameExists(string NatureOfEnquiryName, short RowKey)
        {
            NatureOfEnquiryViewModel model = new NatureOfEnquiryViewModel();
            model.RowKey = RowKey;
            model.NatureOfEnquiryName = NatureOfEnquiryName;
            model = NatureOfEnquiryService.CheckNatureOfEnquiryNameExists(model);
            return Json(model.IsSuccessful, JsonRequestBehavior.AllowGet);
        }
    }
}
