
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
    public class MemberPlanDetailsController : BaseController
    {
        private IMemberPlanDetailsService MemberPlanDetailsService;

        public MemberPlanDetailsController(IMemberPlanDetailsService objMemberPlanDetailsService)
        {
            MemberPlanDetailsService = objMemberPlanDetailsService;
        }

        public ActionResult MemberPlanDetailsList()
        {
            MemberPlanDetailsViewModel model = new MemberPlanDetailsViewModel();
            MemberPlanDetailsService.FillBranches(model);
            return View(model);
        }
        public JsonResult GetMemberPlanDetails(string searchText, short? BranchKey, string sidx, string sord, int page, int rows)
        {
            long TotalRecords = 0;

            List<MemberPlanDetailsViewModel> MemberPlanDetailsList = new List<MemberPlanDetailsViewModel>();

            MemberPlanDetailsViewModel objViewModel = new MemberPlanDetailsViewModel();

            //
            objViewModel.SearchText = searchText;
            objViewModel.BranchKey = BranchKey ?? 0;
            objViewModel.PageIndex = page;
            objViewModel.PageSize = rows;
            objViewModel.SortBy = sidx;
            objViewModel.SortOrder = sord;
            MemberPlanDetailsList = MemberPlanDetailsService.GetMemberPlanDetails(objViewModel, out TotalRecords);

            int pageIndex = Convert.ToInt32(page) - 1;
            int pageSize = rows;
            int totalRecords = MemberPlanDetailsList.Count();
            var totalPages = (int)Math.Ceiling((float)totalRecords / (float)rows);

            var jsonData = new
            {
                total = totalPages,
                page,
                records = totalRecords,
                rows = MemberPlanDetailsList
            };
            return Json(jsonData, JsonRequestBehavior.AllowGet);


        }
        public ActionResult AddEditMemberPlanDetails(long? id)
        {
            var MemberPlanDetails = MemberPlanDetailsService.GetMemberPlanDetailsById(id);
            if (MemberPlanDetails == null)
            {
                MemberPlanDetails = new MemberPlanDetailsViewModel();
            }
            return PartialView(MemberPlanDetails);
        }

        [HttpPost]
        public ActionResult AddEditMemberPlanDetails(MemberPlanDetailsViewModel MemberPlanDetails)
        {

            if (ModelState.IsValid)
            {
                if (MemberPlanDetails.RowKey == 0)
                {
                    //MemberPlanDetails = MemberPlanDetailsService.CreateMemberPlanDetails(MemberPlanDetails);
                }
                else
                {
                    MemberPlanDetails = MemberPlanDetailsService.UpdateMemberPlanDetails(MemberPlanDetails);
                }

                if (MemberPlanDetails.Message != AppConstants.Common.SUCCESS)
                {
                    ModelState.AddModelError("error_msg", MemberPlanDetails.Message);
                }
                else
                {
                    return Json(MemberPlanDetails);
                }

                return Json(MemberPlanDetails);
            }


            return Json(MemberPlanDetails);

        }

        [HttpPost]
        public ActionResult DeleteMemberPlanDetails(Int64 id)
        {
            MemberPlanDetailsViewModel objViewModel = new MemberPlanDetailsViewModel();

            objViewModel.RowKey = id;
            try
            {
                objViewModel = MemberPlanDetailsService.DeleteMemberPlanDetails(objViewModel);
            }
            catch (Exception)
            {
                objViewModel.Message = EduSuiteUIResources.Failed;
            }
            return Json(objViewModel);
        }

        //[HttpPost]
        //public JsonResult FillCourse(List<short> academicTermKey, List<long> universityKey)
        //{
        //    MemberPlanDetailsViewModel model = new MemberPlanDetailsViewModel();
        //    model.AcademicTermKey = academicTermKey != null ? academicTermKey : new List<short>();
        //    model.UniversityKey = universityKey != null ? universityKey : new List<long>();
        //    model = MemberPlanDetailsService.FillCourses(model);
        //    return Json(model.Courses);
        //}

        [HttpPost]
        public JsonResult FillApplication(List<short> academicTermKey, List<long> universityKey, List<long> courseKey, List<short> batchKey)
        {
            MemberPlanDetailsViewModel model = new MemberPlanDetailsViewModel();
            //model.AcademicTermKey = academicTermKey != null ? academicTermKey : new List<short>();
            //model.UniversityKey = universityKey != null ? universityKey : new List<long>();
            //model.CourseKey = courseKey != null ? courseKey : new List<long>();
            //model.BatchKey = batchKey != null ? batchKey : new List<short>();
            //model = MemberPlanDetailsService.FillApplication(model);
            return Json(model.Applications);
        }

        public ActionResult MemberPlanList()
        {
            MemberPlanDetailsViewModel model = new MemberPlanDetailsViewModel();
            MemberPlanDetailsService.FillDropdownLists(model);
            return View(model);
        }

        [HttpPost]
        public ActionResult GetMemberDetails(MemberPlanDetailsViewModel model)
        {
            long TotalRecords = 0;
            model.SearchText = model.SearchText ?? "";
            List<MemberPlanDetailsViewModel> Memberlist = new List<MemberPlanDetailsViewModel>();
            Memberlist = MemberPlanDetailsService.GetMemberPlan(model, out TotalRecords);
            ViewBag.TotalRecords = TotalRecords;
            MemberPlanDetailsService.FillMemberType(model);
            MemberPlanDetailsService.FillBorrowerType(model);

            foreach (MemberPlanDetailsViewModel objmodel in Memberlist)
            {
                objmodel.MemberType = model.MemberType;
                objmodel.BorrowerType = model.BorrowerType;

                if (objmodel.ApplicationTypeKey == DbConstants.ApplicationType.Student)
                {
                    objmodel.CurrentYearText = CommonUtilities.GetYearDescriptionByCodeDetails(objmodel.CourseDuration ?? 0, objmodel.CurrentYear ?? 0, objmodel.AcademicTermKey ?? 0);
                }
            }


            return PartialView(Memberlist);
        }

        [HttpPost]
        public ActionResult AddEditMemberPlans(List<MemberPlanDetailsViewModel> model)
        {
            MemberPlanDetailsViewModel objViewmodel = new MemberPlanDetailsViewModel();

            objViewmodel = MemberPlanDetailsService.CreateMemberPlans(model);

            if (objViewmodel.Message != AppConstants.Common.SUCCESS)
            {
                ModelState.AddModelError("error_msg", objViewmodel.Message);
            }
            else
            {

                return Json(objViewmodel);
            }

            return Json(objViewmodel);

        }
    }
}