using CITS.EduSuite.Business.Common;
using CITS.EduSuite.Business.Interfaces;
using CITS.EduSuite.Business.Models.Resources;
using CITS.EduSuite.Business.Models.ViewModels;
using CITS.EduSuite.Business.Services;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;


namespace CITS.EduSuite.UI.Controllers
{
    public class TestPaperController : BaseController
    {
        ITestPaperService testPaperService;
        ISelectListService selectListService;

        public TestPaperController(ITestPaperService objTestPaperService, ISelectListService objSelectListService)
        {
            testPaperService = objTestPaperService;
            selectListService = objSelectListService;
        }
        // GET: TestPaper
        [ActionAuthenticationAttribute(MenuCode = MenuConstants.TestPaper, ActionCode = ActionConstants.MenuAccess)]
        [HttpGet]
        public ActionResult TestPaperList()
        {
            return View();
        }

        [HttpPost]
        public ActionResult GetFilters(TestPaperViewModel model)
        {
            model = testPaperService.FillDynamicDropDownlists(model);
            return Json(model);
        }

        [HttpGet]
        public ActionResult QuestionCrop(long? id)
        {
            TestPaperViewModel model = new TestPaperViewModel();    
            model.RowKey = id??0;
            return View(model);
        }
        private void releaseObject(object obj)
        {
            try
            {
                System.Runtime.InteropServices.Marshal.ReleaseComObject(obj);
                obj = null;
            }
            catch (Exception ex)
            {
                //TODO
            }
            finally
            {
                GC.Collect();
            }
        }

        [HttpGet]
        public JsonResult GetTestPapers(string searchText)
        {
            int page = 1, rows = 10;

            List<TestPaperViewModel> TestPaperList = new List<TestPaperViewModel>();
            TestPaperList = testPaperService.GetTestPapers(searchText);

            int pageIndex = Convert.ToInt32(page) - 1;
            int pageSize = rows;
            int totalRecords = TestPaperList.Count();
            var totalPages = (int)Math.Ceiling((float)totalRecords / (float)rows);

            var jsonData = new
            {
                total = totalPages,
                page,
                records = totalRecords,
                rows = TestPaperList
            };
            return Json(jsonData, JsonRequestBehavior.AllowGet);
        }

        [ActionAuthenticationAttribute(MenuCode = MenuConstants.TestPaper, ActionCode = ActionConstants.AddEdit)]
        [HttpGet]
        public ActionResult AddEditTestPaper(long? id,int? Qn)
        {
          
            TestPaperViewModel model = new TestPaperViewModel();
            model.RowKey = id ?? 0;
            model.ModuleKey = 1; ;
            model.TestSectionKey = null;
            model = testPaperService.GetQuestionPaperById(model);
            model.QuestionSections = selectListService.FillQuestionSectionById(id ?? 0, model.ModuleKey);

            model.QuestionTypes = selectListService.FillQuestionTypes();
            model.QuestionModules = selectListService.FillQuestionModules();
            //model.Plans = selectListService.FillPlans();
            model.Subjects = selectListService.FillSubjects();
            model.MarkGroups = selectListService.FillMarkGroups();

         

            //var pdfFile = Server.MapPath("~/testpaper.pdf");
            //var pdfToImg = new NReco.PdfRenderer.PdfToImageConverter();
            ////pdfToImg.ScaleTo = 200; // fit 200x200 box
            //pdfToImg.GenerateImage(pdfFile, 1,NReco.PdfRenderer.ImageFormat.Jpeg,Server.MapPath("~/Sample1.jpg"));

            foreach (var item in model.TestQuestions)
            {
                string FilePath = Server.MapPath(UrlConstants.TestQuestionPaperUrl + item.TestSectionKey + "/" + model.ModuleKey + "/"+item.QuestionPaperFileName);
                if (System.IO.File.Exists(FilePath))
                {
                    item.QuestionPaper = System.IO.File.ReadAllText(FilePath);
                }
            }


            if (model.TestInstructionFileName != null && model.TestInstructionFileName != "")
            {
                if (System.IO.File.Exists(Server.MapPath(model.TestInstructionFileName)))
                {
                    model.TestInstruction = System.IO.File.ReadAllText(Server.MapPath(model.TestInstructionFileName));
                }
            }

            //TestPaperQuestionsViewModel QuestionModel = new TestPaperQuestionsViewModel();
            //model.TestPaperQuestions = new List<TestPaperQuestionsViewModel>();
            //model.TestPaperQuestions.Add(QuestionModel);




            if (model.RowKey == 0 && Qn != null)
            {
                model.TestQuestions = new List<TestQuestionViewModel>();
            }
            else
            {
                model.LastQuestionIndex = (model.TestQuestions.Count());
            }

            model.QuestionCount = Qn??0;

            for (int i = 1; i <= Qn; i++)
            {
                TestQuestionViewModel tq = new TestQuestionViewModel();
                tq.QuestionTypeKey = DbConstants.QuestionType.Optional;
                tq.QuestionNumber = i;
                model.TestQuestions.Add(tq);
         

            }

            foreach (var item in model.TestQuestions)
            {
                if (item.QuestionOptions.Count == 0)
                {
                    TestQuestionOptionsViewModel qo = new TestQuestionOptionsViewModel();
                    item.QuestionOptions = new List<TestQuestionOptionsViewModel>();
                   
                    item.QuestionOptions.Add(qo);
                }
            }
            model = testPaperService.FillStaticDropDown(model);


            return View(model);
        }

        [HttpGet]
        public JsonResult QuestionPaperDetail(long? id, byte ModuleKey, long? SectionKey)
        {
            TestPaperViewModel model = new TestPaperViewModel();
            model.RowKey = id ?? 0;
            model.ModuleKey = ModuleKey;
            model.TestSectionKey = SectionKey;
            model = testPaperService.GetQuestionPaperById(model);
            model.QuestionSections = selectListService.FillQuestionSectionById(id ?? 0, ModuleKey);
            return Json(model, JsonRequestBehavior.AllowGet);
        }
        [HttpPost]
        public ActionResult AddEditTestPaper(TestPaperViewModel model)
        {

            foreach (var modelValue in ModelState.Values)
            {
                modelValue.Errors.Clear();
            }
            if (ModelState.IsValid)
            {
                if (model.SupportedFilePath != null)
                    model.SupportedFileName = Path.GetExtension(model.SupportedFilePath.FileName);
                if (model.RowKey == 0)
                {
                    model = testPaperService.CreateTestPaper(model);

                }
                else
                {
                    model = testPaperService.UpdateTestPaper(model);

                }

                if (model.Message != AppConstants.Common.SUCCESS)
                {
                    ModelState.AddModelError("error_msg", model.Message);
                }
                else
                {
                    UploadQuestions(model);
                    UploadFile(model);
                    return Json(model);
                }

                model.Message = "";
                return Json(model);
            }
            else
            {
                var errors = ModelState.Select(x => x.Value.Errors).Where(y => y.Count > 0).ToList();
            }
            model.Message = EduSuiteUIResources.Success;

            return Json(model);
        }



        private void UploadQuestions(TestPaperViewModel model)
        {
            string FilePath = "";

            foreach (var item in model.TestQuestions)
            {

                if (item.QuestionPaper != null && item.QuestionPaperFileName != null)
                {
                    FilePath = Server.MapPath(UrlConstants.TestQuestionPaperUrl + item.TestSectionKey + "/");
                    if (!Directory.Exists(FilePath))
                    {
                        Directory.CreateDirectory(FilePath);
                    }
                    FilePath = Server.MapPath(UrlConstants.TestQuestionPaperUrl + item.TestSectionKey + "/" + model.ModuleKey + "/");
                    if(!Directory.Exists(FilePath))
                    {
                        Directory.CreateDirectory(FilePath);
                    }
                    FilePath = FilePath + item.QuestionPaperFileName;
                    if (!System.IO.File.Exists(FilePath))
                    {
                        using (FileStream stream = System.IO.File.Create(FilePath))
                        {

                        }

                    }
                  
                    System.IO.File.WriteAllText(FilePath, item.QuestionPaper);

                    model.QuestionPaper = null;
                }
            }


        }



        [HttpGet]
        public ActionResult ViewQuestionGroup()
        {

            return PartialView();
        }
        [HttpGet]
        public ActionResult AddEditQuestionGroup(byte id)
        {
            TestQuestionViewModel model = new TestQuestionViewModel();
            model.QuestionTypeKey = id;
            return PartialView();
        }

        [HttpGet]
        public ActionResult AddEditTestSecton(long? id, byte ModuleKey)
        {
            TestPaperViewModel model = new TestPaperViewModel();
            return PartialView(model);
        }
        [HttpPost]
        public ActionResult AddEditTestSecton(TestPaperViewModel model)
        {
            return PartialView("TestPaperSecton", model);
        }

        [ActionAuthenticationAttribute(MenuCode = MenuConstants.TestPaper, ActionCode = ActionConstants.Delete)]
        [HttpPost]
        public ActionResult DeleteTestPaper(long id)
        {
            TestPaperViewModel model = new TestPaperViewModel();
            model.RowKey = id;
            try
            {
                model = testPaperService.DeleteTestPaper(model);
            }
            catch (Exception)
            {
                model.Message = EduSuiteUIResources.Success;
            }
            return Json(model);
        }
        [HttpPost]
        public ActionResult DeleteTestSection(long id)
        {
            TestPaperViewModel model = new TestPaperViewModel();
            model.RowKey = id;
            try
            {
                model = testPaperService.DeleteTestSection(model);
            }
            catch (Exception)
            {
                model.Message = EduSuiteUIResources.Success;
            }
            return Json(model);
        }


        [HttpPost]
        public ActionResult DeleteQuestion(long id)
        {
            TestPaperViewModel model = new TestPaperViewModel();
            model.RowKey = id;
            try
            {
                model = testPaperService.DeleteQuestion(model);
            }
            catch (Exception)
            {
                model.Message = EduSuiteUIResources.Success;
            }
            return Json(model);
        }


        [HttpPost]
        public ActionResult DeleteQuestionOption(long id)
        {
            TestPaperViewModel model = new TestPaperViewModel();
            model.RowKey = id;
           
            try
            {
                model = testPaperService.DeleteQuestionOption(model);
            }
            catch (Exception)
            {
                model.Message = EduSuiteUIResources.Success;
            }
            return Json(model);
        }

        [HttpGet]
        public JsonResult CheckTestNameExists(string TestPaperName, long RowKey)
        {
            return Json(testPaperService.CheckTestNameExists(TestPaperName, RowKey), JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public JsonResult GetFileTypeById(byte id)
        {
            return Json(testPaperService.GetFileTypeById(id), JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public ActionResult AddEditTestInstruction(long? id)
        {
            TestPaperViewModel model = new TestPaperViewModel();
            model = testPaperService.GetTestInstructionById(id ?? 0);
            return View(model);
        }
        [HttpPost]
        public ActionResult AddEditTestInstruction(TestPaperViewModel model)
        {


            model = testPaperService.UpdateTestInstruction(model);


            if (model.Message != AppConstants.Common.SUCCESS)
            {
                ModelState.AddModelError("error_msg", model.Message);
            }
            else
            {
                UploadFile(model);
                return RedirectToAction("TestPaperList");
            }

            model.Message = "";
            return View(model);

        }

        [ActionAuthenticationAttribute(MenuCode = MenuConstants.TestPaper, ActionCode = ActionConstants.Delete)]
        [HttpGet]
        public ActionResult AddEditAnswerKey(long? id)
        {
            TestPaperViewModel model = new TestPaperViewModel();
            model.RowKey = id ?? 0;
            model.QuestionModules = selectListService.FillAnswerKeyModules(id ?? 0);
            model.ModuleKey = 1;
            model = testPaperService.GetTestAnswerKeyById(model);
            return View(model);
        }
        [HttpGet]
        public JsonResult GetAnswerKeyById(long? id, byte ModuleKey)
        {
            TestPaperViewModel model = new TestPaperViewModel();
            model.RowKey = id ?? 0;
            model.ModuleKey = ModuleKey;
            model = testPaperService.GetTestAnswerKeyById(model);
            return Json(model, JsonRequestBehavior.AllowGet);
        }
        [HttpPost]
        public ActionResult AddEditAnswerKey(TestPaperViewModel model)
        {


            model = testPaperService.UpdateTestAnswerKey(model);

            UploadFile(model);
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



        private void UploadFile(TestPaperViewModel model)
        {
            string FilePath = "";

           




            if ((model.TestInstruction != null && model.TestInstructionFileName != null) || (model.RowKey>0 && model.TestInstructionFileName != null))
            {
                FilePath = Server.MapPath(UrlConstants.TestInstructionFileUrl);
                if (!Directory.Exists(FilePath))
                {
                    Directory.CreateDirectory(FilePath);
                }
                FilePath = FilePath + model.TestInstructionFileName;
                if (!System.IO.File.Exists(FilePath))
                {
                    using (FileStream stream = System.IO.File.Create(FilePath))
                    {

                    }

                }

                System.IO.File.WriteAllText(FilePath, model.TestInstruction??"");

                model.QuestionPaper = null;
            }

        }


        #region Exam Valuation

        [HttpGet]
        public ActionResult ExamValuationList()
        {
            ViewBag.TestModules = selectListService.FillQuestionModules();
            return View();
        }

        [HttpGet]
        public JsonResult GetExamValuation(string searchText, int page, int rows)
        {
            long TotalRecords = 0;
            var TestPaperList = testPaperService.GetExamValuations(searchText, page, rows, out TotalRecords);

            int pageIndex = Convert.ToInt32(page) - 1;
            int pageSize = rows;
            var totalPages = (int)Math.Ceiling((float)TotalRecords / (float)rows);

            var jsonData = new
            {
                total = totalPages,
                page,
                records = TotalRecords,
                rows = TestPaperList
            };
            return Json(jsonData, JsonRequestBehavior.AllowGet);
        }
        [HttpGet]
        public ActionResult AddEditTestValuation(long ApplicationKey, long TestPaperKey, byte ModuleKey)
        {
            ExamTestViewModel model = new ExamTestViewModel();
            model.TestPaperKey = TestPaperKey;
            model.ApplicationKey = ApplicationKey;
            model.ModuleKey = ModuleKey;
            model = testPaperService.GetExamValuationById(model);
            return PartialView(model);

        }
        [HttpPost]
        public ActionResult AddEditTestValuation(ExamTestViewModel model)
        {
            model = testPaperService.UpdateExamResult(model);

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
        #endregion
    }
}