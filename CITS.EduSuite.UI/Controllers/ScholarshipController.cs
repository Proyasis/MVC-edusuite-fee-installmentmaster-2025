using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using CITS.EduSuite.Business.Common;
using CITS.EduSuite.Business.Interfaces;
using CITS.EduSuite.Business.Models.ViewModels;
using CITS.EduSuite.Business.Provider;
using CITS.EduSuite.Business.Models.Security;
using System.Web.Security;
using CITS.EduSuite.Business.Models.Resources;

namespace CITS.EduSuite.UI.Controllers
{
    public class ScholarshipController : BaseController
    {

        private IScholarshipService scholarshipservice;

        public ScholarshipController(IScholarshipService objscholarshipservice)
        {
            this.scholarshipservice = objscholarshipservice;
        }

        [ActionAuthenticationAttribute(MenuCode = MenuConstants.Scholarship, ActionCode = ActionConstants.MenuAccess)]
        [HttpGet]
        public ActionResult ScholarshipList()
        {
            ScholarshipViewModel model = new ScholarshipViewModel();
            
            model = scholarshipservice.GetSearchDropDownLists(model);
            return View(model);
        }
        [HttpPost]
        public ActionResult GetScholarship(string SearchName, string SearchPhone, DateTime? SearchFromDate, DateTime? SearchToDate, int? SearchDistrictKey, short? SearchBranchKey, short? SearchScholarshipTypeKey, string sidx, string sord, int page, int rows)
        {


            long TotalRecords = 0;
            List<ScholarshipViewModel> scholarshipList = new List<ScholarshipViewModel>();
            ScholarshipViewModel objViewModel = new ScholarshipViewModel();

            //
            objViewModel.SearchName = SearchName;
            objViewModel.SearchPhone = SearchPhone;
            objViewModel.SearchFromDate = SearchFromDate;
            objViewModel.SearchToDate = SearchToDate;

            //objViewModel.SearchEmployeeKey = SearchEmployeeKey;
            objViewModel.SearchBranchKey = SearchBranchKey ?? 0;
            objViewModel.SearchDistrictKey = SearchDistrictKey ?? 0;
            objViewModel.SearchScholarshipTypeKey = SearchScholarshipTypeKey ?? 0;

            objViewModel.page = page;
            objViewModel.rows = rows;
            objViewModel.sidx = sidx;
            objViewModel.sord = sord;

            scholarshipList = scholarshipservice.GetScholarships(objViewModel, out TotalRecords);

            int pageIndex = Convert.ToInt32(page) - 1;
            int pageSize = rows;
            var totalPages = (int)Math.Ceiling((float)TotalRecords / (float)rows);

            var jsonData = new
            {
                total = totalPages,
                page,
                records = TotalRecords,
                rows = scholarshipList
            };
            return Json(jsonData, JsonRequestBehavior.AllowGet);

        }


        [ActionAuthenticationAttribute(MenuCode = MenuConstants.Scholarship, ActionCode = ActionConstants.AddEdit)]
        [HttpGet]
        public ActionResult AddEditScholarship(long? id)
        {
            ScholarshipViewModel model = new ScholarshipViewModel();
            
            model.RowKey = id ?? 0;
            model = scholarshipservice.GetScholarshipById(model);
            if (model == null)
            {
                model = new ScholarshipViewModel();
            }
            return PartialView(model);
        }

        [ActionAuthenticationAttribute(MenuCode = MenuConstants.Scholarship, ActionCode = ActionConstants.AddEdit)]
        [HttpPost]
        public ActionResult AddEditScholarship(ScholarshipViewModel model)
        {

            
            if (ModelState.IsValid)
            {
                if (model.RowKey == 0)
                {
                    model = scholarshipservice.CreateScholarship(model);
                }
                else
                {
                    model = scholarshipservice.UpdateScholarship(model);
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

            foreach (ModelState state in ViewData.ModelState.Values.Where(x => x.Errors.Count > 0))
            {
            }

            model.Message = EduSuiteUIResources.Failed;
            return Json(model);
        }

         [ActionAuthenticationAttribute(MenuCode = MenuConstants.Scholarship, ActionCode = ActionConstants.Delete)]
        [HttpPost]
        public ActionResult DeleteScholarship(short id)
        {
            ScholarshipViewModel model = new ScholarshipViewModel();
            model.RowKey = id;
            try
            {
                model = scholarshipservice.DeleteScholarship(model);
            }
            catch (Exception)
            {
                model.Message = EduSuiteUIResources.Failed;
            }
            return Json(model);
        }
        [HttpGet]
        public JsonResult GetBranchByDistrict(Int32 id)
        {
            ScholarshipViewModel model = new ScholarshipViewModel();
            model.DistrictKey = id;
            model = scholarshipservice.FillBranches(model);
            return Json(model, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public JsonResult GetSerchBranchByDistrict(Int32 id)
        {
            ScholarshipViewModel model = new ScholarshipViewModel();
            model.SearchDistrictKey = id;
            model = scholarshipservice.FillSerachBranches(model);
            return Json(model, JsonRequestBehavior.AllowGet);
        }
       
        [HttpPost]
        public ActionResult MoveToEnquiry(List<long> model)
        {
            ScholarshipViewModel objmodel = new ScholarshipViewModel();

            objmodel = scholarshipservice.MoveToEnquiry(model, objmodel);
            if (objmodel.Message != AppConstants.Common.SUCCESS)
            {
                ModelState.AddModelError("error_msg", objmodel.Message);
            }
            else
            {
                return Json(objmodel);

            }


            objmodel.Message = "";
            return Json(objmodel);
        }
    }
}