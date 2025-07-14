using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using CITS.EduSuite.Business.Common;
using CITS.EduSuite.Business.Interfaces;
using CITS.EduSuite.Business.Models.ViewModels;
using CITS.EduSuite.Business.Models.Resources;

namespace CITS.EduSuite.UI.Controllers
{
    public class UniversityPaymentCancelationController : BaseController
    {
        private IUniversityCancelationService universityCancelationService;
        private ISelectListService selectListService;
        private IReportService ReportService;

        public UniversityPaymentCancelationController(IUniversityCancelationService objuniversityCancelationService,
            ISelectListService objSelectListService, IReportService objReportService)
        {
            this.universityCancelationService = objuniversityCancelationService;
            this.selectListService = objSelectListService;
            this.ReportService = objReportService;
        }

        [ActionAuthenticationAttribute(MenuCode = MenuConstants.UniversityPaymentCancel, ActionCode = ActionConstants.MenuAccess)]
        [HttpGet]
        public ActionResult UniversityPaymentCancelationList()
        {
            UniversityCancelationViewModel model = new UniversityCancelationViewModel();
            model.Branches = selectListService.FillBranches();
            return View(model);
        }

        [HttpGet]
        public JsonResult GetUniversityCancelation(short? BranchKey, string SearchText, string sidx, string sord, int page, int rows)
        {
            long TotalRecords = 0;

            List<UniversityCancelationViewModel> universityPaymentCancelationList = new List<UniversityCancelationViewModel>();
            UniversityCancelationViewModel objViewModel = new UniversityCancelationViewModel();
            objViewModel.BranchKey = BranchKey ?? 0;
            objViewModel.SearchText = SearchText;
            objViewModel.PageIndex = page;
            objViewModel.PageSize = rows;
            objViewModel.SortBy = sidx;
            objViewModel.SortOrder = sord;

            universityPaymentCancelationList = universityCancelationService.GetUniversityCancelation(objViewModel, out TotalRecords);
            int pageIndex = Convert.ToInt32(page) - 1;
            int pageSize = rows;
            var totalPages = (int)Math.Ceiling((float)TotalRecords / (float)rows);

            var jsonData = new
            {
                total = totalPages,
                page,
                records = TotalRecords,
                rows = universityPaymentCancelationList
            };

            return Json(jsonData, JsonRequestBehavior.AllowGet);
        }

        [ActionAuthenticationAttribute(MenuCode = MenuConstants.UniversityPaymentCancel, ActionCode = ActionConstants.AddEdit)]

        [HttpGet]
        public ActionResult AddEditUniversityPaymentCancelation(long? id, long? ApplicationKey, long? UniversityPaymentDetailsKey)
        {
            UniversityCancelationViewModel model = new UniversityCancelationViewModel();
            model.RowKey = id ?? 0;
            model.ApplicationKey = ApplicationKey ?? 0;
            model.UniversityPaymentDetailsKey = UniversityPaymentDetailsKey ?? 0;
            model = universityCancelationService.GetUniversityCancelationById(model);
            return PartialView(model);
        }

        [HttpPost]
        public ActionResult AddEditUniversityPaymentCancelation(UniversityCancelationViewModel model)
        {
            if (ModelState.IsValid)
            {
                if (model.RowKey == 0)
                {
                    model = universityCancelationService.CreateUniversityCancelation(model);
                }
                else
                {
                    model = universityCancelationService.UpdateUniversityCancelation(model);
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
            return PartialView(model);
        }

        [ActionAuthenticationAttribute(MenuCode = MenuConstants.UniversityPaymentCancel, ActionCode = ActionConstants.Delete)]
        [HttpPost]
        public ActionResult DeleteUniversityPaymentCancelation(Int64 id)
        {
            UniversityCancelationViewModel objViewModel = new UniversityCancelationViewModel();

            objViewModel.RowKey = id;
            try
            {
                objViewModel = universityCancelationService.DeleteUniversityCancelation(objViewModel);
            }
            catch (Exception)
            {
                objViewModel.Message = EduSuiteUIResources.Failed;
            }
            return Json(objViewModel);
        }


        public ActionResult ViewUniversityPayment()
        {
            ReportViewModel model = new ReportViewModel();
            model.Branches = selectListService.FillBranches();
            return View(model);
        }

        public ActionResult BindUniversityPaymentList(ReportViewModel model)
        {

            List<ReportViewModel> objViewmodel = new List<ReportViewModel>();
            objViewmodel = ReportService.GetStudentsDayToDayUniversityPaymentSummary(model);
            return PartialView(objViewmodel);
        }
    }
}