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
using System.Xml.Linq;
using Newtonsoft.Json;
using System.Dynamic;
using iTextSharp.text;
using iTextSharp.text.pdf;
using System.IO;
using iTextSharp.tool.xml.pipeline.html;
using iTextSharp.tool.xml.html;
using iTextSharp.tool.xml;
using iTextSharp.tool.xml.pipeline.css;
using iTextSharp.tool.xml.pipeline.end;
using iTextSharp.tool.xml.parser;
using HandlebarsDotNet;
using System.Threading;
using CITS.EduSuite.Business.Models.Resources;

namespace CITS.EduSuite.UI.Controllers
{
    public class ScholarshipExamScheduleController : BaseController
    {
        private IScholarshipExamScheduleService scholarshipExamScheduleservice;
        private INotificationTemplateService notificationTemplateService;
        public ScholarshipExamScheduleController(IScholarshipExamScheduleService objscholarshipExamScheduleservice,INotificationTemplateService objnotificationTemplateService)
        {
            this.scholarshipExamScheduleservice = objscholarshipExamScheduleservice;
            this.notificationTemplateService = objnotificationTemplateService;
        }

        [ActionAuthenticationAttribute(MenuCode = MenuConstants.ScholarshipExamSchedule, ActionCode = ActionConstants.MenuAccess)]
        [HttpGet]
        public ActionResult ScholarshipExamScheduleList()
        {
            ScholarshipExamScheduleViewModel model = new ScholarshipExamScheduleViewModel();
            
            model = scholarshipExamScheduleservice.GetSearchDropDownLists(model);
            return View(model);
        }
        [HttpPost]
        public ActionResult GetScholarship(string SearchName, string SearchPhone, DateTime? SearchFromDate, DateTime? SearchToDate, int? SearchDistrictKey, short? SearchBranchKey, short? SearchScholarshipTypeKey, short? SearchSubBranchKey, bool ScheduleStatus, string sidx, string sord, int page, int rows)
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
            objViewModel.ScheduleStatus = ScheduleStatus;

            objViewModel.page = page;
            objViewModel.rows = rows;
            objViewModel.sidx = sidx;
            objViewModel.sord = sord;

            scholarshipList = scholarshipExamScheduleservice.GetScholarshipExamSchedules(objViewModel, out TotalRecords);

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


        [ActionAuthenticationAttribute(MenuCode = MenuConstants.ScholarshipExamSchedule, ActionCode = ActionConstants.AddEdit)]
        [HttpGet]
        public ActionResult AddEditScholarshipExamSchedule(long? id)
        {
            ScholarshipExamScheduleViewModel model = new ScholarshipExamScheduleViewModel();
            model.RowKey = id ?? 0;
            // model = scholarshipExamScheduleservice.GetScholarshipExamScheduleById(model);
            if (model == null)
            {
                model = new ScholarshipExamScheduleViewModel();
            }
            return PartialView(model);
        }

        [HttpPost]
        public ActionResult AddEditScholarshipExamSchedule(ScholarshipExamScheduleViewModel model)
        {


            if (ModelState.IsValid)
            {

                model = scholarshipExamScheduleservice.UpdateScholarshipExamSchedule(model);

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

        [HttpPost]
        [ActionAuthenticationAttribute(MenuCode = MenuConstants.ScholarshipExamSchedule, ActionCode = ActionConstants.Delete)]

        public ActionResult DeleteScholarshipExam(short id)
        {
            ScholarshipExamScheduleViewModel model = new ScholarshipExamScheduleViewModel();
            model.RowKey = id;
            try
            {
                model = scholarshipExamScheduleservice.DeleteScholarshipExamSchedule(model);
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
            ScholarshipExamScheduleViewModel model = new ScholarshipExamScheduleViewModel();
            model.DistrictKey = id;
            model = scholarshipExamScheduleservice.FillBranches(model);
            return Json(model, JsonRequestBehavior.AllowGet);
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
       


        //[HttpGet]
        //public ActionResult AddEditScholarshipExamScheduleBulk(long? Id)
        //{

        //    ScholarshipExamScheduleViewModel model = new ScholarshipExamScheduleViewModel();
        //    model.RowKey = Id ?? 0;
        //    
        //    model = scholarshipExamScheduleservice.GetSearchDropDownLists(model);
        //    return View(model);
        //}
        [HttpPost]
        public ActionResult AddEditScholarshipExamScheduleBulk(ScholarshipExamScheduleViewModel model)
        {
            if (ModelState.IsValid)
            {

                model = scholarshipExamScheduleservice.CreateScholarshipExamSchedule(model);


                if (model.Message != AppConstants.Common.SUCCESS)
                {
                    ModelState.AddModelError("error_msg", model.Message);
                }
                else
                {
                    SendNotificationInBackground(model);
                    return Json(model);

                }


                model.Message = "";
                return Json(model);
            }
            foreach (ModelState state in ViewData.ModelState.Values.Where(x => x.Errors.Count > 0))
            {
            }

            model.Message = EduSuiteUIResources.Failed;
            return Json(model);
        }



        //[HttpPost]
        //public ActionResult GetScholarshipExamScheduleDetails(ScholarshipExamScheduleViewModel model)
        //{
        //    scholarshipExamScheduleservice.FillScholarshipExamScheduleDetails(model);

        //    return PartialView(model);
        //}
        [HttpGet]
        public ActionResult Hallticket(long? id, string MobilNumber, string DOB)
        {
            if (id == null)
            {
                ScholarshipExamScheduleViewModel model = new ScholarshipExamScheduleViewModel();
                model.MobileNumber = MobilNumber;
                model.ScholarshipDate = DateTime.ParseExact(DOB, "dd/MM/yyyy", null);

                id = scholarshipExamScheduleservice.getScholarshipid(model);
            }
            ViewBag.RowKey = id ?? 0;
            return PartialView();
        }
        [HttpGet]
        public JsonResult GetHallticket(long id)
        {
            var xml = scholarshipExamScheduleservice.GetPrintHallTicket(id, 1);
            return Json(xml, JsonRequestBehavior.AllowGet);
        }
        [HttpPost]
        [ValidateInput(false)]
        public void DownloadHallticket(string Html, List<string> cssList, string fileName)
        {
            ////reset response
            Response.Clear();
            Response.ContentType = "application/pdf";

            ////define pdf filename
            Response.AddHeader("content-disposition", "attachment; filename=" + fileName);


            //Generate PDF
            using (var document = new Document(PageSize.A4, 40, 40, 40, 40))
            {
                //define output control HTML
                var memStream = new MemoryStream();
                TextReader xmlString = new StringReader(Html);

                PdfWriter writer = PdfWriter.GetInstance(document, memStream);

                //open doc
                document.Open();

                // register all fonts in current computer
                FontFactory.RegisterDirectories();

                // Set factories
                var htmlContext = new HtmlPipelineContext(null);
                htmlContext.SetTagFactory(Tags.GetHtmlTagProcessorFactory());

                // Set css
                ICSSResolver cssResolver = XMLWorkerHelper.GetInstance().GetDefaultCssResolver(false);
                if (cssList != null)
                {
                    foreach (string css in cssList)
                    {
                        cssResolver.AddCssFile(css, true);
                    }
                }

                IPipeline pipeline = new CssResolverPipeline(cssResolver,
                                                             new HtmlPipeline(htmlContext,
                                                                              new PdfWriterPipeline(document, writer)));
                var worker = new XMLWorker(pipeline, true);
                var xmlParse = new XMLParser(true, worker);
                xmlParse.Parse(xmlString);
                xmlParse.Flush();

                document.Close();
                document.Dispose();

                Response.BinaryWrite(memStream.ToArray());
            }

            Response.End();
            Response.Flush();
        }

        public void SendNotificationInBackground(ScholarshipExamScheduleViewModel model)
        {

            Thread bgThread = new Thread(new ParameterizedThreadStart(SendNotification));

            bgThread.IsBackground = true;
            bgThread.Start(model);

        }
        private void SendNotification(Object model)
        {
            ScholarshipExamScheduleViewModel objViewModel = (ScholarshipExamScheduleViewModel)model;
            string Url = string.Format("{0}://{1}{2}", Request.Url.Scheme, Request.Url.Authority, "");
            string FilePath = Server.MapPath("~/Templates/EmailTemplate/EmailTemplate.html");
            foreach (ScholarshipExamScheduleDetails objdetails in objViewModel.ScholarshipExamScheduleDetails)
            {
                objdetails.URL = Url;
                if (System.IO.File.Exists(FilePath) && objdetails.EmailAddress != "" && objdetails.EmailAddress != null)
                {
                    //Mustache.FormatCompiler mCompiler = new Mustache.FormatCompiler();
                    //Mustache.Generator generator = mCompiler.Compile(System.IO.File.ReadAllText(FilePath));
                    //var result = generator.Render(json);
                    var template = Handlebars.Compile(System.IO.File.ReadAllText(FilePath));
                    var result = template(objdetails);
                    EmailViewModel emailModel = new EmailViewModel();
                    emailModel.EmailBody = result;
                    emailModel.EmailTo = objdetails.EmailAddress;
                    // emailModel.EmailCC = objNotificationModel.AdminEmailAddress;
                    emailModel.EmailSubject = "JobTrack :: Hall Ticket - JMT-PSC'19";

                    EmailHelper.SendEmail(emailModel);

                }

            }

            //NotificationDataViewModel notificationModel = new NotificationDataViewModel();
            //NotificationHelper nofificationHelper = new NotificationHelper(notificationTemplateService);
            //notificationModel.EmailTemplateName = Server.MapPath("~/UploadedFiles/NotificationTemplate/");
            //notificationModel.RowKey = model.RowKey??0;
            //notificationModel.AutoSMS = model.AutoSMS;
            //notificationModel.AutoEmail = model.AutoEmail;
            //notificationModel.TemplateKey = model.TemplateKey;
            //nofificationHelper.SendNotificationInBackground(notificationModel);



        }

        [HttpGet]

        public ActionResult SearchHallTicket()
        {
            ScholarshipViewModel model = new ScholarshipViewModel();
            return PartialView(model);
        }
    }
}