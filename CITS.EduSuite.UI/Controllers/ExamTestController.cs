using CITS.EduSuite.Business.Common;
using CITS.EduSuite.Business.Interfaces;
using CITS.EduSuite.Business.Models.ViewModels;
using CITS.EduSuite.Business.Services;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using CITS.EduSuite.Business.Models.Resources;

namespace CITS.EduSuite.UI.Controllers
{
    public class ExamTestController : BaseController
    {

        IExamTestService examTestService;
        ISelectListService selectListService;

        public ExamTestController(IExamTestService objExamTestService, ISelectListService objSelectListService)
        {
            examTestService = objExamTestService;
            selectListService = objSelectListService;
        }

        //[PlanValidAuthentication()]
        [HttpGet]
        public ActionResult ExamTestList(ExamTestViewModel model)
        {

            //bool IsFeePaid = examTestService.IsFeePaid();
            //if (!IsFeePaid)
            //{
            //    return RedirectToAction("ApplicationFee", "ApplicationFeePayment");
            //}


            //model = examTestService.FillStaticDropDownlists(model);
           // model.SearchExamFilterTypeKey = DbConstants.ExamFilterTypes.FindExam;


            return View(model);
        }
        [HttpGet]
        public ActionResult ExamReviewList(ExamTestViewModel model)
        {
            //model = examTestService.FillStaticDropDownlists(model);
            //model.SearchExamFilterTypeKey = DbConstants.ExamFilterTypes.ExamReview;
            return View(model);
        }


        [HttpPost]
        public ActionResult ExamTestDetail(ExamTestViewModel model)
        {
            ExamTestViewModel examModel = examTestService.GetExamTests(model);
            return PartialView(examModel);
        }

        [HttpPost]
        public ActionResult GetFilters(ExamTestViewModel model)
        {
            //model = examTestService.FillDynamicDropDownlists(model);
            return Json(model);
        }


        //[ActionAuthenticationAttribute(MenuCode = MenuConstants.ExamTest, ActionCode = ActionConstants.AddEdit)]
        //[PlanAuthentication(MenuCode = MenuConstants.ExamTest, KeyName = "id")]
        [HttpGet]
        public ActionResult AddEditExamTest(long? id, long? TestPaperKey, long? ApplicationKey)
        {
            ExamTestViewModel model = new ExamTestViewModel();
            model.TestPaperKey = id ?? 0;
            model.ApplicationKey = ApplicationKey ?? 0;
            examTestService.InitializeData(model);
            model.TestModules = selectListService.FillExamModules(id ?? 0, ApplicationKey);

            model.TestPaperKey = id ?? 0;
            model.ModuleKey = 1;

            model = examTestService.GetExamTestById(model);

            foreach (var itemM in model.TestSections)
            {
                foreach (var item in itemM.QuestionDetails)
                {
                    string FilePath = Server.MapPath(UrlConstants.TestQuestionPaperUrl + itemM.RowKey + "/" + model.ModuleKey + "/" + itemM.QuestionPaperFileName);
                    if (System.IO.File.Exists(FilePath))
                    {
                        itemM.TestSectionFileName = System.IO.File.ReadAllText(FilePath);
                    }
                }
            }



            return View(model);
        }

        [HttpGet]
        public ActionResult UpdateTestPaperKey(long? id, int ExamStatusKey)
        {
            ExamTestViewModel model = new ExamTestViewModel();
            examTestService.InitializeData(model);
            model.TestPaperKey = id ?? 0;
            model.ExamStatusKey = ExamStatusKey;
            examTestService.UpdateExamStatus(model);
            return Json(model, JsonRequestBehavior.AllowGet);
        }


        [HttpGet]
        public ActionResult UpdateExamKey(long? id)
        {
            ExamTestViewModel model = new ExamTestViewModel();
            examTestService.InitializeData(model);
            model.TestPaperKey = id ?? 0;
            examTestService.UpdateExamKey(model);
            return Json(model, JsonRequestBehavior.AllowGet);
        }



        [HttpGet]
        public ActionResult ShowExamReview(long? id, long? TestPaperKey, long? ApplicationKey)
        {
            ExamTestViewModel model = new ExamTestViewModel();
            model.TestPaperKey = id ?? 0;
            model.ApplicationKey = ApplicationKey ?? 0;
            examTestService.InitializeData(model);
            model.TestModules = selectListService.FillExamModules(id ?? 0, ApplicationKey);

            model.TestPaperKey = id ?? 0;
            model.ModuleKey = 1;

            model = examTestService.GetExamTestById(model);
            foreach (var itemM in model.TestSections)
            {
                foreach (var item in itemM.QuestionDetails)
                {
                    string FilePath = Server.MapPath(UrlConstants.TestQuestionPaperUrl + itemM.RowKey + "/" + model.ModuleKey + "/" + itemM.QuestionPaperFileName);
                    if (System.IO.File.Exists(FilePath))
                    {
                        itemM.TestSectionFileName = System.IO.File.ReadAllText(FilePath);
                    }
                }
            }

            return View(model);
        }



        [HttpPost]
        public ActionResult AddEditExamTest(ExamTestViewModel model)
        {
            if (ModelState.IsValid)
            {
                model.ModuleKey = 1;
                if (model.ModuleKey == DbConstants.QuestionModule.Speaking)
                {
                    UpdateModel(model);
                }

                if (model.RowKey == 0)
                {

                    model = examTestService.CreateExamTest(model);
                }
                else
                {
                    model = examTestService.UpdateExamTest(model);

                }
                if (model.ModuleKey == DbConstants.QuestionModule.Speaking)
                {
                    UploadFile(model);
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
                return Json(model);
            }
            model.Message = EduSuiteUIResources.Success;
            var errors = ModelState.Select(x => x.Value.Errors).Where(y => y.Count > 0).ToList();
            return Json(model);
        }

        [HttpGet]
        public JsonResult QuestionPaperDetail(long? id, byte ModuleKey, long? ApplicationKey)
        {
            ExamTestViewModel model = new ExamTestViewModel();
            model.TestPaperKey = id ?? 0;
            model.ModuleKey = ModuleKey;
            model.ApplicationKey = ApplicationKey ?? 0;
            model = examTestService.GetExamTestById(model);
            if (model.SupportedFileName != null)
            {

            }
            return Json(model, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public JsonResult ExamReview(long? id)
        {
            var result = examTestService.GetExamReviewById(id ?? 0);
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public JsonResult GetExamTestsById(long? id)
        {
            var result = examTestService.GetExamTestsById(id ?? 0);
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        private void UploadFile(ExamTestViewModel model)
        {
            string FilePath = "";
            foreach (ExamTestAnswerViewModel item in model.ExamAnswers)
            {
                if (item.AnswerFile != null && item.AnswerText != null)
                {
                    FilePath = Server.MapPath(UrlConstants.ExamTestAnsweFileUrl + DbConstants.User.UserKey + "/" + model.TestPaperKey + "/");
                    if (!Directory.Exists(FilePath))
                    {
                        Directory.CreateDirectory(FilePath);
                    }
                    item.AnswerFile.SaveAs(FilePath + item.AnswerText);
                    item.AnswerFile = null;
                }
            }
        }

        private void UpdateModel(ExamTestViewModel model)
        {
            for (int i = 0; i < model.ExamAnswers.Count; i++)
            {
                model.ExamAnswers[i].AnswerFile = Request.Files["ExamAnswers[" + i + "][AnswerFile]"];

            }
        }


        public ActionResult ExamResultList()
        {
            return View();
        }


        [HttpGet]
        public JsonResult GetExamResultsList(string SearchText, string sidx, string sord, int page, int rows)
        {
            long TotalRecords = 0;
            List<ExamTestViewModel> ExamResultsList = new List<ExamTestViewModel>();
            ExamTestViewModel objViewModel = new ExamTestViewModel();



            objViewModel.PageIndex = page;
            objViewModel.PageSize = rows;
            objViewModel.SortBy = sidx;
            objViewModel.SortOrder = sord;
            objViewModel.SearchText = SearchText;
            ExamResultsList = examTestService.GetExamTestsAll(objViewModel, out TotalRecords);

            int pageIndex = Convert.ToInt32(page) - 1;
            int pageSize = rows;
            var totalPages = (int)Math.Ceiling((float)TotalRecords / (float)rows);

            var jsonData = new
            {
                total = totalPages,
                page,
                records = TotalRecords,
                rows = ExamResultsList
            };
            return Json(jsonData, JsonRequestBehavior.AllowGet);
        }



    }
}