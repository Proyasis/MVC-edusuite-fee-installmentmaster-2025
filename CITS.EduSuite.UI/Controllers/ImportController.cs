using CITS.EduSuite.Business.Interfaces;
using CITS.EduSuite.Business.Models.ViewModels;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;

namespace CITS.EduSuite.UI.Controllers
{
    public class ImportController : BaseController
    {

        public IStudyMaterialService studyMaterialService;
        public IUniversityCertificateService universityCertificateService;
        public IExamResultService examResultService;
        public IInternalExamResultService internalExamResultService;
        public IEnquiryLeadService enquiryLeadService;
        public ISelectListService selectListService;
        private ISharedService sharedService;
        private IEmployeeAttendanceService employeeAttendanceService;
        public ImportController(IStudyMaterialService objStudyMaterialService,
            IUniversityCertificateService objUniversityCertificateService,
            IExamResultService objExamResultService,
            IInternalExamResultService objInternalExamResultService,
            IEnquiryLeadService objEnquiryLeadService,
            ISelectListService objSelectListService,
            ISharedService objSharedService, IEmployeeAttendanceService objEmployeeAttendanceService)
        {
            this.studyMaterialService = objStudyMaterialService;
            this.universityCertificateService = objUniversityCertificateService;
            this.internalExamResultService = objInternalExamResultService;
            this.examResultService = objExamResultService;
            this.enquiryLeadService = objEnquiryLeadService;
            this.selectListService = objSelectListService;
            this.sharedService = objSharedService;
            this.employeeAttendanceService = objEmployeeAttendanceService;
        }
    

        [HttpGet]
        public ActionResult SearchDropdownList()
        {
            ApplicationPersonalViewModel model = new ApplicationPersonalViewModel();
            FillDropdownLists(model);
            ViewBag.ShowAcademicTerm = sharedService.ShowAcademicTerm();
            ViewBag.ShowUniversity = sharedService.ShowUniversity();
            return PartialView(model);
        }

        [ActionAuthenticationAttribute(MenuCode = MenuConstants.StudentStudyMaterial, ActionCode = ActionConstants.BulkAdd)]
        [HttpGet]
        public ActionResult AvailableStudyMaterial()
        {
            return View();
        }
        [HttpPost]
        public ActionResult AvailableStudyMaterial(StudyMaterialViewModel model)
        {
            model.StudyMaterialList = model.StudyMaterialList.GroupBy(x => new { x.ApplicationKey, x.StudyMaterialKey }).Select(x => x.FirstOrDefault()).ToList();
            model = studyMaterialService.UpdateStudyMaterials(model);
            if (!model.IsSuccessful)
            {
                ModelState.AddModelError("error_msg", model.Message);
            }
            else
            {
                return Json(model);
            }
            return Json(model);
        }
        [HttpGet]
        public JsonResult FillAvailableStudyMaterialDropdowns(short? BranchKey, short? BatchKey, short? AcademicTermKey, short? CourseTypeKey, long? CourseKey, short? UniversityKey, int? StudentYearKey)
        {
            var Students = selectListService.FillStudentsById(BranchKey, BatchKey, AcademicTermKey, CourseKey, UniversityKey, false);
            var StudyMaterials = selectListService.FillStudyMaterialById(AcademicTermKey, CourseKey, UniversityKey, StudentYearKey);
            var result = new { Students, StudyMaterials };
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        [ActionAuthenticationAttribute(MenuCode = MenuConstants.UniversityCertificate, ActionCode = ActionConstants.BulkAdd)]
        [HttpGet]
        public ActionResult AvailableCertificate()
        {

            return View();
        }
        [HttpPost]
        public ActionResult AvailableCertificate(UniversityCertificateViewModel model)
        {
            model.UniversityCertificateDetails = model.UniversityCertificateDetails.GroupBy(x => new { x.ApplicationKey, x.CertificateTypeKey }).Select(x => x.FirstOrDefault()).ToList();
            model = universityCertificateService.UpdateUniversityCertificates(model);
            if (!model.IsSuccessful)
            {
                ModelState.AddModelError("error_msg", model.Message);
            }
            else
            {
                return Json(model);
            }
            return Json(model);
        }
        [HttpGet]
        public JsonResult FillAvailableCertificateDropdowns(short? BranchKey, short? BatchKey, short? AcademicTermKey, short? CourseTypeKey, long? CourseKey, short? UniversityKey)
        {
            var Students = selectListService.FillStudentsById(BranchKey, BatchKey, AcademicTermKey, CourseKey, UniversityKey, false);
            var CertificateTypes = selectListService.FillCertificates();
            var result = new { Students, CertificateTypes };
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public ActionResult ExamResult()
        {
            ExamResultDetail model = new ExamResultDetail();
            return View(model);
        }
        [HttpPost]
        public ActionResult ExamResult(ExamResultViewModel model)
        {
            model.ExamResultDetail = model.ExamResultDetail.GroupBy(x => new { x.ApplicationKey, x.SubjectKey }).Select(x => x.FirstOrDefault()).ToList();
            model = examResultService.UpdateExamResults(model);
            if (!model.IsSuccessful)
            {
                ModelState.AddModelError("error_msg", model.Message);
            }
            else
            {
                return Json(model);
            }
            return Json(model);
        }
        [HttpGet]
        public JsonResult FillExamResultDropdowns(short? BranchKey, short? BatchKey, short? AcademicTermKey, short? CourseTypeKey, long? CourseKey, short? UniversityKey, int? StudentYearKey)
        {
            var Students = selectListService.FillStudentsById(BranchKey, BatchKey, AcademicTermKey, CourseKey, UniversityKey, false);
            var Subjects = selectListService.FillSubjectsById(AcademicTermKey, CourseKey, UniversityKey, StudentYearKey);
            var ExamTerms = selectListService.FillExamTerms();
            var ExamCenters = selectListService.FillExamCenters();
            var result = new { Students, Subjects, ExamTerms, ExamCenters };
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public ActionResult InternalExamResult()
        {
            InternalExamViewModel model = new InternalExamViewModel();
            model.InternalExamTerm = selectListService.FillInternalExamTerms();
            return View(model);
        }
        [HttpPost]
        public ActionResult InternalExamResult(InternalExamResultViewModel model)
        {
            model.InternalExamResultDetails = model.InternalExamResultDetails.GroupBy(x => new { x.ApplicationKey, x.SubjectKey }).Select(x => x.FirstOrDefault()).ToList();
            model = internalExamResultService.UpdateInternalExamResults(model);
            if (!model.IsSuccessful)
            {
                ModelState.AddModelError("error_msg", model.Message);
            }
            else
            {
                return Json(model);
            }
            return Json(model);
        }
        [HttpGet]
        public JsonResult FillInternalExamResultDropdowns(short? BranchKey, short? BatchKey, short? AcademicTermKey, short? InternalExamTermKey, long? CourseKey, short? UniversityKey, int? StudentYearKey, long? ClassDetailsKey)
        {
            InternalExamViewModel model = new InternalExamViewModel();
            model.BranchKey = BranchKey;
            model.BatchKey = BatchKey ?? 0;
            model.AcademicTermKey = AcademicTermKey ?? 0;
            model.CourseKey = CourseKey ?? 0;
            model.UniversityMasterKey = UniversityKey ?? 0;
            model.ExamYear = StudentYearKey ?? 0;
            model.ClassDetailsKey = ClassDetailsKey ?? 0;
            model.InternalExamTermKey = InternalExamTermKey ?? 0;

            var CheckexistingExam = internalExamResultService.CheckexistingExam(model);
            if (CheckexistingExam)
            {
                var Students = internalExamResultService.FillStudentsById(model);
                var Subjects = internalExamResultService.FillSubjectsById(model);

                var result = new { Students, Subjects };
                return Json(result, JsonRequestBehavior.AllowGet);
            }
            else
            {
                var result = "";
                return Json(result, JsonRequestBehavior.AllowGet);
            }
        }

        [ActionAuthenticationAttribute(MenuCode = MenuConstants.EnquiryLead, ActionCode = ActionConstants.BulkAdd)]
        [HttpGet]
        public ActionResult EnquiryLead()
        {
            EnquiryLeadViewModel model = new EnquiryLeadViewModel();
            model.Branches = selectListService.FillBranches();
            return View(model);
        }
        [HttpPost]
        public ActionResult EnquiryLead(List<EnquiryLeadViewModel> modelList)
        {
            EnquiryLeadViewModel objViewModel = new EnquiryLeadViewModel();
            objViewModel = enquiryLeadService.UpdateEnquiryLeads(modelList);
            if (objViewModel.IsSuccessful)
            {
                ModelState.AddModelError("error_msg", objViewModel.Message);
            }
            else
            {
                return Json(objViewModel);
            }
            return Json(objViewModel);
        }
        [HttpGet]
        public JsonResult FillEnquiryLeadDropdowns(short? BranchKey, int PageIndex, int PageSize)
        {
            int TotalRecords;
            EnquiryLeadViewModel model = new EnquiryLeadViewModel();
            model.BranchKey = BranchKey;
            model.PageIndex = PageIndex;
            model.PageSize = PageSize;
            var Employees = selectListService.FillEmployeesById(BranchKey ?? 0);
            var TelephoneCodes = selectListService.FillTelephoneCodes();
            var data = enquiryLeadService.GetPendingLeadByBranch(model, out TotalRecords);
            var NatureOfEnquiryList = selectListService.FillNatureOfEnquiry();
            var result = new { Employees, TelephoneCodes, NatureOfEnquiryList, data, TotalRecords };
            return Json(result, JsonRequestBehavior.AllowGet);
        }
        private void FillDropdownLists(ApplicationPersonalViewModel model)
        {
            model.BranchKey = model.BranchKey == 0 ? DbConstants.User.BranchKey ?? 0 : model.BranchKey;
            model.Branches = selectListService.FillBranches();
            model.Batches = selectListService.FillBatches(model.BranchKey);
            model.AcademicTerms = selectListService.FillAcademicTerms();
        }

        [HttpGet]
        public JsonResult FillClassDetails(short? BranchKey, short? BatchKey, short? AcademicTermKey, long? CourseKey, short? UniversityKey, int? StudentYearKey, short? InternalExamTermKey)
        {
            InternalExamViewModel model = new InternalExamViewModel();
            model.BranchKey = BranchKey;
            model.BatchKey = BatchKey ?? 0;
            model.AcademicTermKey = AcademicTermKey ?? 0;
            model.CourseKey = CourseKey ?? 0;
            model.UniversityMasterKey = UniversityKey ?? 0;
            model.ExamYear = StudentYearKey ?? 0;
            model.InternalExamTermKey = InternalExamTermKey ?? 0;
            model.ClassDetails = internalExamResultService.FillClassDetailsBy(model);
            return Json(model.ClassDetails, JsonRequestBehavior.AllowGet);
        }

        [ActionAuthenticationAttribute(MenuCode = MenuConstants.AttendanceSheet, ActionCode = ActionConstants.BulkAdd)]
        [HttpGet]
        public ActionResult EmployeeAttendance()
        {
            EmployeeAttendanceViewModel objViewModel = new EmployeeAttendanceViewModel();
            objViewModel = employeeAttendanceService.GetBranches(objViewModel);
            return View(objViewModel);
        }
        [HttpPost]
        public ActionResult EmployeeAttendance(List<EmployeeAttendanceViewModel> modelList)
        {
            EmployeeAttendanceViewModel model = employeeAttendanceService.UpdateEmployeesAttendance(modelList, true);

            if (model.IsSuccessful)
            {
                ModelState.AddModelError("error_msg", model.Message);
            }
            else
            {
                return Json(model);
            }
            return Json(model);
        }
        [HttpGet]
        public JsonResult FillEmployeeAttendanceDropdowns(short? BranchKey)
        {
            EmployeeAttendanceViewModel model = new EmployeeAttendanceViewModel();
            model.BranchKey = BranchKey??0;
            var Employees = selectListService.FillEmployeesById(BranchKey ?? 0);
            var AttendanceStatus = selectListService.FillEmployeeAttendanceStatus(BranchKey ?? 0);
            var result = new { Employees, AttendanceStatus };
            return Json(result, JsonRequestBehavior.AllowGet);
        }

    }
}