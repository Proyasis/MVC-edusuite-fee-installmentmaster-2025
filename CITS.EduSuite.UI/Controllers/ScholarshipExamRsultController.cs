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
    public class ScholarshipExamResultController : BaseController
    {
        private IScholarshipExamScheduleService scholarshipExamScheduleservice;

        public ScholarshipExamResultController(IScholarshipExamScheduleService scholarshipExamScheduleservice)
        {
            this.scholarshipExamScheduleservice = scholarshipExamScheduleservice;
        }

        [ActionAuthenticationAttribute(MenuCode = MenuConstants.ScholarshipExamResult, ActionCode = ActionConstants.MenuAccess)]
        [HttpGet]
        public ActionResult ScholarshipExamResultList()
        {
            ScholarshipExamScheduleViewModel model = new ScholarshipExamScheduleViewModel();
            
            model = scholarshipExamScheduleservice.GetSearchDropDownLists(model);
            return View(model);
        }
        [HttpPost]
        public ActionResult GetScholarshipExamResult(string SearchName, string SearchPhone, DateTime? SearchFromDate, DateTime? SearchToDate, int? SearchDistrictKey, short? SearchBranchKey, short? SearchScholarshipTypeKey, short? SearchSubBranchKey, string sidx, string sord, int page, int rows)
        {


            long TotalRecords = 0;
            List<ScholarshipExamScheduleViewModel> scholarshipList = new List<ScholarshipExamScheduleViewModel>();
            ScholarshipExamScheduleViewModel objViewModel = new ScholarshipExamScheduleViewModel();

            //
            objViewModel.SearchName = SearchName;
            objViewModel.SearchPhone = SearchPhone;
            objViewModel.SearchFromDate = SearchFromDate;
            objViewModel.SearchToDate = SearchToDate;

            //objViewModel.SearchEmployeeKey = SearchEmployeeKey;
            objViewModel.SearchBranchKey = SearchBranchKey ?? 0;
            objViewModel.SearchDistrictKey = SearchDistrictKey ?? 0;
            objViewModel.SearchScholarshipTypeKey = SearchScholarshipTypeKey ?? 0;
            objViewModel.SubBranchKey = SearchSubBranchKey;


            objViewModel.page = page;
            objViewModel.rows = rows;
            objViewModel.sidx = sidx;
            objViewModel.sord = sord;

            scholarshipList = scholarshipExamScheduleservice.GetScholarshipExamResult(objViewModel, out TotalRecords);

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


       

        [ActionAuthenticationAttribute(MenuCode = MenuConstants.ScholarshipExamResult, ActionCode = ActionConstants.AddEdit)]
        [HttpGet]

        public ActionResult AddEditScholarshipExamResult(string Keys)
        {
            ScholarshipExamResultViewModel model = new ScholarshipExamResultViewModel();

            List<long> ScholarshipKeys = new List<long>();
            if (Keys != null)
            {
                Keys = String.IsNullOrEmpty((Keys ?? "0")) ? "0" : (Keys ?? "0");
                ScholarshipKeys = (Keys).Split(',').Select(row => Int64.Parse(row)).ToList();
                model = scholarshipExamScheduleservice.GetScholarshipExamResultDetails(model, ScholarshipKeys);
            }
            return PartialView(model);
        }

        [HttpPost]
        public ActionResult AddEditScholarshipExamResult(ScholarshipExamResultViewModel model)
        {
            if (ModelState.IsValid)
            {
                model = scholarshipExamScheduleservice.UpdateScholarshipExamResult(model);

                if (model.Message != AppConstants.Common.SUCCESS)
                {
                    ModelState.AddModelError("error_msg", model.Message);
                }
                else
                {
                    return Json(model);

                }


                model.Message = "";
                return View(model);
            }
            foreach (ModelState state in ViewData.ModelState.Values.Where(x => x.Errors.Count > 0))
            {
            }

            model.Message = EduSuiteUIResources.Failed;
            return PartialView(model);
        }

        [HttpGet]
        public JsonResult GetSerchBranchByDistrict(Int32 id)
        {
            ScholarshipExamScheduleViewModel model = new ScholarshipExamScheduleViewModel();
            model.SearchDistrictKey = id;
            model = scholarshipExamScheduleservice.FillSerachBranches(model);
            return Json(model, JsonRequestBehavior.AllowGet);
        }
        public JsonResult GetSubBranchByBranch(short id)
        {
            ScholarshipExamScheduleViewModel model = new ScholarshipExamScheduleViewModel();
            model.SearchBranchKey = id;
            model = scholarshipExamScheduleservice.FillSubBranches(model);
            return Json(model, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult AddBulkScholarshipExamResult(List<ScholarshipExamDetails> modelList)
        {
            ScholarshipExamResultViewModel objViewModel = new ScholarshipExamResultViewModel();
            objViewModel = scholarshipExamScheduleservice.UpdateBulkScholarshipExamResult(modelList);
            if (objViewModel.Message != AppConstants.Common.SUCCESS)
            {
                ModelState.AddModelError("error_msg", objViewModel.Message);
            }
            else
            {
                return Json(objViewModel);
            }
            return Json(objViewModel);
        }
    }
}