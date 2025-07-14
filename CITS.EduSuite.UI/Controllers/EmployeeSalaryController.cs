using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Mvc;
using System.Web.UI;
using CITS.EduSuite.Business.Common;
using CITS.EduSuite.Business.Interfaces;
using CITS.EduSuite.Business.Models.ViewModels;

using Rotativa.MVC;
using Rotativa;
using CITS.EduSuite.Business.Models.Common;
using CITS.EduSuite.Business.Models.Resources;

namespace CITS.EduSuite.UI.Controllers
{
    public class EmployeeSalaryController : BaseController
    {
        private IEmployeeSalaryService employeeSalaryService;
        private ISelectListService selectListService;

        public EmployeeSalaryController(IEmployeeSalaryService objEmployeeSalaryService, ISelectListService objSelectListService)
        {
            this.employeeSalaryService = objEmployeeSalaryService;
            this.selectListService = objSelectListService;
        }


        [ActionAuthenticationAttribute(MenuCode = MenuConstants.EmployeeSalary, ActionCode = ActionConstants.MenuAccess)]
        [HttpGet]
        public ActionResult EmployeeSalaryList(byte? type)
        {
            EmployeeSalaryMasterViewModel model = new EmployeeSalaryMasterViewModel();
            model.Branches = selectListService.FillBranches();
            model.Salarytypelist = selectListService.FillSalaryType();

            model.SalaryTypeName = EduSuiteUIResources.Salary; // type == DbConstants.SalaryType.Monthly ? EduSuiteUIResources.Salary : EduSuiteUIResources.Wage;
            return View(model);
        }

        [HttpGet]
        public JsonResult GetEmployeeSalary(byte salaryMonthKey, short salaryYearKey, long? EmployeeKey, short? BranchKey, string sidx, string sord, int page, int rows)
        {
            long TotalRecords = 0;

            List<EmployeeSalaryMasterViewModel> employeeSalaryList = new List<EmployeeSalaryMasterViewModel>();
            EmployeeSalaryMasterViewModel employeeSalaryMasterViewModel = new EmployeeSalaryMasterViewModel();
            employeeSalaryMasterViewModel.SalaryMonthKey = salaryMonthKey;
            employeeSalaryMasterViewModel.SalaryYearKey = salaryYearKey;
            employeeSalaryMasterViewModel.EmployeeKey = EmployeeKey ?? 0;
            employeeSalaryMasterViewModel.BranchKey = BranchKey ?? 0;
            employeeSalaryMasterViewModel.PageIndex = page;
            employeeSalaryMasterViewModel.PageSize = rows;
            employeeSalaryMasterViewModel.SortBy = sidx;
            employeeSalaryMasterViewModel.SortOrder = sord;
            employeeSalaryList = employeeSalaryService.GetEmployeeSalaries(employeeSalaryMasterViewModel, out TotalRecords);

            var totalPages = (int)Math.Ceiling((float)TotalRecords / (float)rows);

            var jsonData = new
            {
                total = totalPages,
                page,
                records = TotalRecords,
                rows = employeeSalaryList
            };
            return Json(jsonData, JsonRequestBehavior.AllowGet);
        }

        [ActionAuthenticationAttribute(MenuCode = MenuConstants.EmployeeSalary, ActionCode = ActionConstants.AddEdit)]
        [HttpGet]
        public ActionResult AddEditEmployeeSalary(long? id, int? employeeKey, short? branchKey, byte? salaryMonthKey, short? salaryYearKey)
        {
            EmployeeSalaryMasterViewModel model = new EmployeeSalaryMasterViewModel();
            model.EmployeeKey = employeeKey ?? 0;
            model.BranchKey = branchKey ?? 0;
            model.SalaryMonthKey = salaryMonthKey ?? 0;
            model.SalaryYearKey = salaryYearKey ?? 0;

            model.SalaryMasterKey = id ?? 0;
            //employeeSalaryService.FillMasterDropdownLists(model);
            model.Branches = selectListService.FillBranches();
            employeeSalaryService.FillEmployeesMaster(model);
            return View(model);
        }

        public ActionResult EmployeeSalaryDetails(long? id, int? employeeKey, short? branchKey, byte? salaryMonthKey, short? salaryYearKey)
        {
            EmployeeSalaryMasterViewModel model = new EmployeeSalaryMasterViewModel();
            model.EmployeeKey = employeeKey ?? 0;
            model.BranchKey = branchKey ?? 0;
            model.SalaryMonthKey = salaryMonthKey ?? 0;
            model.SalaryYearKey = salaryYearKey ?? 0;
           

            model.SalaryMasterKey = id ?? 0;
            var employeeSalary = employeeSalaryService.GetEmployeesSalaryByMonth(model);
            if (employeeSalary.Count == 0)
            {
                employeeSalary.Add(model);
            }
            return PartialView(employeeSalary[0]);
        }

        [HttpPost]
        public ActionResult AddEditEmployeeSalary(EmployeeSalaryMasterViewModel model)
        {
            //ModelState.Where(row => row.Key.Contains("EmployeeSalaryPayment")).Select(row => row.Value).ToList().ForEach(value => value.Errors.Clear());
            if (ModelState.IsValid)
            {
                if (model.SalaryMasterKey == 0)
                {
                    model = employeeSalaryService.CreateEmployeeSalary(model);
                }
                else
                {
                    model = employeeSalaryService.UpdateEmployeeSalary(model);
                }

                if (model.Message != AppConstants.Common.SUCCESS)
                {
                    ModelState.AddModelError("error_msg", model.Message);
                }
                else
                {
                    //GeneratePdfPayslip(model.SalaryMasterKeyList);
                    return Json(model);
                }

                //model.Message = "";
                return Json(model);
            }
            foreach (ModelState state in ViewData.ModelState.Values.Where(x => x.Errors.Count > 0))
            {
            }

            model.Message = EduSuiteUIResources.Failed;
            return Json(model);

        }

        [HttpPost]
        public ActionResult AddEditEmployeesSalary(List<EmployeeSalaryMasterViewModel> model)
        {
            EmployeeSalaryMasterViewModel employeeSalaryModel = new EmployeeSalaryMasterViewModel();

            employeeSalaryModel = employeeSalaryService.UpdateEmployeesSalaryList(model);

            if (employeeSalaryModel.Message != AppConstants.Common.SUCCESS)
            {
                ModelState.AddModelError("error_msg", employeeSalaryModel.Message);
            }
            else
            {
                //GeneratePdfPayslip(employeeSalaryModel.SalaryMasterKeyList);
                return Json(employeeSalaryModel);
            }

            //model.Message = "";
            return Json(employeeSalaryModel);

        }

        [HttpGet]
        public ActionResult QuickSalaryProcess()
        {
            EmployeeSalaryMasterViewModel obViewModel = new EmployeeSalaryMasterViewModel();
            obViewModel.Branches = selectListService.FillBranches();
            obViewModel = employeeSalaryService.GetSalaryHeads(obViewModel);
            return View(obViewModel);

        }
        [HttpPost]
        public JsonResult GetQuickEmployeesSalary(EmployeeSalaryMasterViewModel model)
        {
            int page = 1, rows = 10;

            List<EmployeeSalaryMasterViewModel> employeeSalaryList = new List<EmployeeSalaryMasterViewModel>();
            employeeSalaryList = employeeSalaryService.GetEmployeesSalaryByMonth(model);
            int pageIndex = Convert.ToInt32(page) - 1;
            int pageSize = rows;
            int totalRecords = employeeSalaryList.Count();
            var totalPages = (int)Math.Ceiling((float)totalRecords / (float)rows);

            var jsonData = new
            {
                total = totalPages,
                page,
                records = totalRecords,
                rows = employeeSalaryList
            };
            return Json(jsonData);
        }

        [ActionAuthenticationAttribute(MenuCode = MenuConstants.EmployeeSalary, ActionCode = ActionConstants.Delete)]
        [HttpPost]
        public ActionResult DeleteEmployeeSalary(Int64 id)
        {
            EmployeeSalaryMasterViewModel model = new EmployeeSalaryMasterViewModel();

            model.SalaryMasterKey = id;
            try
            {
                model = employeeSalaryService.DeleteEmployeeSalary(model);
            }
            catch (Exception)
            {
                model.Message = EduSuiteUIResources.Failed;
            }
            return Json(model);
        }

        [HttpPost]
        public ActionResult DeleteEmployeeSalaryDetail(Int64 id)
        {
            EmployeeSalaryDetailViewModel model = new EmployeeSalaryDetailViewModel();
            EmployeeSalaryMasterViewModel objSalaryMasterViewModel = new EmployeeSalaryMasterViewModel();

            model.RowKey = id;
            try
            {
                objSalaryMasterViewModel = employeeSalaryService.DeleteEmployeeSalaryComponent(model);
            }
            catch (Exception)
            {
                objSalaryMasterViewModel.Message = EduSuiteUIResources.Failed;
            }
            return Json(objSalaryMasterViewModel);
        }

        //[HttpPost]
        //public JsonResult GetSalaryComponentsByMonth(EmployeeSalaryMasterViewModel model)
        //{
        //    model = employeeSalaryService.GetEmployeeSalaryByMonth(model);
        //    if (model == null)
        //    {
        //        model = new EmployeeSalaryMasterViewModel();
        //        model.SalaryComponents = employeeSalaryService.GetSalaryComponentsByMonth(model);
        //    }
        //    return Json(model);
        //}

        [HttpGet]
        public JsonResult GetEmployeesByBranchId(short? id)
        {
            EmployeeSalaryMasterViewModel model = new EmployeeSalaryMasterViewModel();
            model.BranchKey = id ?? 0;
            model = employeeSalaryService.FillEmployeesMaster(model);
            return Json(model.Employees, JsonRequestBehavior.AllowGet);

        }

        [HttpGet]
        public ActionResult AddEditEmployeeSalaryPayment(long? id)
        {
            var employeeSalaryPayment = employeeSalaryService.GetEmployeeSalaryPaymentById(id ?? 0);
            var Url = "~/Views/Shared/PaymentWindow.cshtml";
            return PartialView(Url, employeeSalaryPayment);
        }

        [HttpPost]
        public ActionResult AddEditEmployeeSalaryPayment(PaymentWindowViewModel model)
        {

            if (ModelState.IsValid)
            {
                if (model.PaymentKey == 0)
                {
                    model = employeeSalaryService.CreateSalaryPayment(model);
                }
                else
                {
                    model = employeeSalaryService.UpdateSalaryPayment(model);
                }

                if (model.Message != AppConstants.Common.SUCCESS)
                {
                    ModelState.AddModelError("error_msg", model.Message);
                }
                else
                {
                    return Json(model);
                }

                //model.Message = "";
                return Json(model);
            }

            model.Message = EduSuiteUIResources.Failed;
            return PartialView(model);

        }

        //[ActionAuthenticationAttribute(MenuCode = MenuConstants.EmployeeSalary, ActionCode = ActionConstants.Payment)]
        [HttpGet]
        public ActionResult SalaryPaySlip(int? Id)
        {
            //List<SalaryPaymentSlipViewModel> employeeSalary = new List<SalaryPaymentSlipViewModel>();
            //if (Ids != null)
            //{

            //    List<long> Id = Ids.Split(',').Select(Int64.Parse).ToList();
            //   employeeSalary = employeeSalaryService.GetSalaryPaymentSlipByIds(Id);

            //}
            List<SalaryPaymentSlipViewModel> employeeSalary = new List<SalaryPaymentSlipViewModel>();
            long SalaryMasterKey = Id ?? 0;
            employeeSalary = employeeSalaryService.GetSalaryPaymentSlipByIds(SalaryMasterKey);
            return PartialView(employeeSalary);

        }

        //[HttpPost]
        //public void UploadPdfPayslip(HttpPostedFileBase file)
        //{
        //    EmployeeSalaryMasterViewModel model = new EmployeeSalaryMasterViewModel() { SalaryMasterKey = Convert.ToInt64(Path.GetFileNameWithoutExtension(file.FileName)), PaySlipFile = file };

        //    try
        //    {


        //        model = employeeSalaryService.UpdateEmployeeSalaryPaySlipFileName(model);
        //        string FilePath = Server.MapPath("~/UploadFiles/Employee/" + model.EmployeeKey + "/Payslip/");
        //        if (!Directory.Exists(FilePath))
        //        {
        //            Directory.CreateDirectory(FilePath);
        //        }
        //        string MonthName = CommonHelper.GetMonthNameFromNumber(model.SalaryMonthKey) + model.SalaryYearKey;
        //        string FileName = MonthName + ".pdf";
        //        string FileNameEncrypt = MonthName + "Encrypt.pdf";
        //        MemoryStream target = new MemoryStream();
        //        model.PaySlipFile.InputStream.CopyTo(target);
        //        byte[] inputData = target.ToArray();

        //        byte[] outputData = PdfHelper.GenerateProtectedPdfFromFile(inputData, model.PaySlipPassword);
        //        model.PaySlipFile.SaveAs(FilePath + FileName);
        //        System.IO.File.WriteAllBytes(FilePath + FileNameEncrypt, outputData);
        //        SendPayslipToEmail(model.EmployeeEmailAddress, model.PaySlipPassword, MonthName, (FilePath + FileNameEncrypt));
        //    }
        //    catch (Exception ex)
        //    {

        //    }
        //}

        [HttpGet]
        public FileContentResult DownloadPdfPayslip(long id, string fileName)
        {
            string FullPath = Server.MapPath(UrlConstants.EmployeeUrl + id + "/Payslip/" + fileName);
            return File(System.IO.File.ReadAllBytes(FullPath), System.Web.MimeMapping.GetMimeMapping(FullPath), EduSuiteUIResources.PaySlip + "_" + Path.GetFileName(FullPath));
        }

        private void SendPayslipToEmail(string EmailAddress, string Password, string Month, string FilePath)
        {
            if (EmailAddress != "")
            {
                EmailViewModel objEmailViewModel = new EmailViewModel();
                objEmailViewModel.EmailTo = EmailAddress;
                objEmailViewModel.EmailSubject = "Payslip for " + Month;
                objEmailViewModel.EmailBody = "Password : <b>" + Password + "</b>";
                objEmailViewModel.EmailAttachment.Add(FilePath);
                EmailHelper.SendMailWithAttachment(objEmailViewModel);
            }
        }

        [HttpGet]
        public JsonResult FillEmployees(short? id)
        {
            EmployeeSalaryAdvanceViewModel model = new EmployeeSalaryAdvanceViewModel();
            model.BranchKey = id ?? 0;
            model = employeeSalaryService.FillEmployees(model);
            return Json(model.Employees, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetBalance(short PaymentModeKey, long? RowKey, long? BankAccountKey, short? branchKey)
        {
            decimal Balance = employeeSalaryService.GetBalanceforAdvance(PaymentModeKey, RowKey ?? 0, BankAccountKey ?? 0, branchKey ?? 0);
            return Json(Balance, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public ActionResult FillBankAccount(short? id)
        {
            EmployeeSalaryAdvanceViewModel model = new EmployeeSalaryAdvanceViewModel();
            model.BranchKey = id ?? 0;
            model = employeeSalaryService.FillBankAccounts(model);
            return Json(model.BankAccounts, JsonRequestBehavior.AllowGet);
        }

        [ActionAuthenticationAttribute(MenuCode = MenuConstants.EmployeeSalaryAdvance, ActionCode = ActionConstants.MenuAccess)]
        [HttpGet]
        public ActionResult EmployeeSalaryAdvanceList()
        {
            EmployeeSalaryAdvanceViewModel model = new EmployeeSalaryAdvanceViewModel();
            model.Branches = selectListService.FillBranches();
            return View(model);
        }
        [HttpGet]
        public JsonResult GetEmployeeSalaryAdvance(short? BranchKey, long? EmployeeKey, string FromDate, string ToDate, string sidx, string sord, int page, int rows)
        {

            List<EmployeeSalaryAdvanceViewModel> employeeSalaryList = new List<EmployeeSalaryAdvanceViewModel>();
            EmployeeSalaryAdvanceViewModel model = new EmployeeSalaryAdvanceViewModel();
            model.EmployeeKey = EmployeeKey ?? 0;
            model.BranchKey = BranchKey ?? 0;
            model.PageIndex = page;
            model.PageSize = rows;
            model.SortBy = sidx;
            model.SortOrder = sord;
            employeeSalaryList = employeeSalaryService.GetEmployeeSalaryAdvances(model, FromDate, ToDate);
            int pageIndex = Convert.ToInt32(page) - 1;
            int pageSize = rows;
            int totalRecords = model.TotalRecords;
            var totalPages = (int)Math.Ceiling((float)totalRecords / (float)rows);

            if (employeeSalaryList.Count > 0)
            {
                model = employeeSalaryList[0];

            }

            var jsonData = new
            {
                total = totalPages,
                page,
                records = totalRecords,
                rows = employeeSalaryList
            };
            return Json(jsonData, JsonRequestBehavior.AllowGet);
        }

        [ActionAuthenticationAttribute(MenuCode = MenuConstants.EmployeeSalaryAdvance, ActionCode = ActionConstants.AddEdit)]
        [HttpGet]
        public ActionResult AddEditEmployeeSalaryAdvance(long? id)
        {
            EmployeeSalaryAdvanceViewModel model = new EmployeeSalaryAdvanceViewModel();
            model.PaymentKey = id ?? 0;
            var employeeSalaryPayment = employeeSalaryService.GetEmployeeSalaryAdvancePaymentById(model);

            return PartialView(employeeSalaryPayment);
        }

        [HttpPost]
        public ActionResult AddEditEmployeeSalaryAdvance(EmployeeSalaryAdvanceViewModel model)
        {

            if (ModelState.IsValid)
            {
                if (model.PaymentKey == 0)
                {
                    model = employeeSalaryService.CreateSalaryAdvancePayment(model);

                }
                else
                {
                    model = employeeSalaryService.UpdateSalaryAdvancePayment(model);

                }

                if (model.Message != AppConstants.Common.SUCCESS)
                {
                    ModelState.AddModelError("error_msg", model.Message);
                }
                else
                {
                    return Json(model);
                }

                //model.Message = "";
                return Json(model);
            }

            model.Message = EduSuiteUIResources.Failed;
            return PartialView(model);

        }

        [ActionAuthenticationAttribute(MenuCode = MenuConstants.EmployeeSalaryAdvance, ActionCode = ActionConstants.Delete)]
        [HttpPost]
        public ActionResult DeleteEmployeeSalaryAdvance(Int64 id)
        {
            EmployeeSalaryAdvanceViewModel model = new EmployeeSalaryAdvanceViewModel();

            model.PaymentKey = id;
            try
            {
                model = employeeSalaryService.DeleteSalaryAdvancePayment(model);

            }
            catch (Exception)
            {
                model.Message = EduSuiteUIResources.Failed;
            }
            return Json(model);
        }

        #region AdvanceReturn

        [HttpGet]
        public JsonResult GetEmployeeAdvanceReturn(short? BranchKey, long? EmployeeKey, string FromDate, string ToDate, string sidx, string sord, int page, int rows)
        {

            List<EmployeeSalaryAdvanceReturnViewModel> employeeSalaryList = new List<EmployeeSalaryAdvanceReturnViewModel>();
            EmployeeSalaryAdvanceReturnViewModel model = new EmployeeSalaryAdvanceReturnViewModel();
            model.EmployeeKey = EmployeeKey ?? 0;
            model.BranchKey = BranchKey ?? 0;
            model.PageIndex = page;
            model.PageSize = rows;
            model.SortBy = sidx;
            model.SortOrder = sord;
            employeeSalaryList = employeeSalaryService.GetEmployeeSalaryAdvanceReturn(model, FromDate, ToDate);
            int pageIndex = Convert.ToInt32(page) - 1;
            int pageSize = rows;
            int totalRecords = model.TotalRecords;
            var totalPages = (int)Math.Ceiling((float)totalRecords / (float)rows);

            if (employeeSalaryList.Count > 0)
            {
                model = employeeSalaryList[0];

            }

            var jsonData = new
            {
                total = totalPages,
                page,
                records = totalRecords,
                rows = employeeSalaryList
            };
            return Json(jsonData, JsonRequestBehavior.AllowGet);
        }

        [ActionAuthenticationAttribute(MenuCode = MenuConstants.EmployeeSalaryAdvance, ActionCode = ActionConstants.AddEdit)]
        [HttpGet]
        public ActionResult AddEditEmployeeAdvanceReturn(long? id)
        {
            EmployeeSalaryAdvanceReturnViewModel model = new EmployeeSalaryAdvanceReturnViewModel();
            model.PaymentKey = id ?? 0;
            var employeeSalaryPayment = employeeSalaryService.GetEmployeeSalaryAdvanceReturnById(model);

            return PartialView(employeeSalaryPayment);
        }

        [HttpGet]
        public ActionResult ReturnAdvanceList(long? id, long? employeeKey)
        {
            EmployeeSalaryAdvanceReturnViewModel model = new EmployeeSalaryAdvanceReturnViewModel();
            model.PaymentKey = id ?? 0;
            model.EmployeeKey = employeeKey ?? 0;
            model = employeeSalaryService.fillAdvances(model);
            return PartialView(model);
        }

        [HttpPost]
        public ActionResult AddEditEmployeeAdvanceReturn(EmployeeSalaryAdvanceReturnViewModel model)
        {

            if (ModelState.IsValid)
            {
                if (model.PaymentKey == 0)
                {
                    model = employeeSalaryService.CreateSalaryAdvanceReturn(model);
                }
                else
                {
                    model = employeeSalaryService.UpdateSalaryAdvanceReturn(model);
                }

                if (model.Message != AppConstants.Common.SUCCESS)
                {
                    ModelState.AddModelError("error_msg", model.Message);
                }
                else
                {
                    return Json(model);
                }

                //model.Message = "";
                return Json(model);
            }

            model.Message = EduSuiteUIResources.Failed;
            return PartialView(model);

        }

        [ActionAuthenticationAttribute(MenuCode = MenuConstants.EmployeeSalaryAdvance, ActionCode = ActionConstants.Delete)]
        [HttpPost]
        public ActionResult DeleteEmployeeAdvanceReturn(Int64 id)
        {
            EmployeeSalaryAdvanceReturnViewModel model = new EmployeeSalaryAdvanceReturnViewModel();

            model.PaymentKey = id;
            try
            {
                model = employeeSalaryService.DeleteSalaryAdvanceReturn(model);

            }
            catch (Exception)
            {
                model.Message = EduSuiteUIResources.Failed;
            }
            return Json(model);
        }

        #endregion

        [HttpGet]
        public JsonResult FillPaymentModeSub(short? PaymentModeKey)
        {
            ApplicationFeePaymentViewModel model = new ApplicationFeePaymentViewModel();
            model.PaymentModeKey = PaymentModeKey ?? 0;
            model.PaymentModeSub = selectListService.FillPaymentModeSub(model.PaymentModeKey);
            return Json(model.PaymentModeSub, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public JsonResult GetEmployeePaymentDetails(string Id)
        {
            long SalaryMasterKey = Convert.ToInt64(Id);
            var TotalPaymentDetails = employeeSalaryService.GetEmployeePaymentDetails(SalaryMasterKey);
            var jsonData = new
            {
                rows = TotalPaymentDetails
            };
            return Json(jsonData, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public ActionResult AddEditEmployeeSalaryPaymentEdit(long? id)
        {
            var employeeSalaryPayment = employeeSalaryService.GetEmployeePaymentById(id ?? 0);
            var Url = "~/Views/Shared/PaymentWindow.cshtml";
            return PartialView(Url, employeeSalaryPayment);
        }

        [HttpPost]
        public ActionResult DeleteSalaryPayment(long? id)
        {
            PaymentWindowViewModel model = new PaymentWindowViewModel();

            try
            {
                model = employeeSalaryService.DeleteSalaryPayment(id);
            }
            catch (Exception)
            {
                model.Message = EduSuiteUIResources.Failed;
            }
            return Json(model);
        }
    }
}
