using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using CITS.EduSuite.Business.Common;
using CITS.EduSuite.Business.Interfaces;
using CITS.EduSuite.Business.Models.Resources;
using CITS.EduSuite.Business.Models.Security;
using CITS.EduSuite.Business.Models.ViewModels;
using CITS.EduSuite.Business.Provider;



namespace CITS.EduSuite.UI.Controllers
{
    [MessagesActionFilter]
    public class EmployeeController : BaseController
    {

        private IEmployeeService employeeService;

        public EmployeeController(IEmployeeService objEmployeeService)
        {
            this.employeeService = objEmployeeService;
        }

        [ActionAuthenticationAttribute(MenuCode = MenuConstants.Employee, ActionCode = ActionConstants.MenuAccess)]
        [HttpGet]
        public ActionResult EmployeeList()
        {
            EmployeePersonalViewModel objViewModel = new EmployeePersonalViewModel();

            objViewModel = employeeService.GetSearchDropdownLists(objViewModel);
            return View(objViewModel);
        }

        [HttpGet]
        public JsonResult GetEmployees(string searchText, short? branchId, short? statusId)
        {
            int page = 1, rows = 10;

            List<EmployeePersonalViewModel> employeeList = new List<EmployeePersonalViewModel>();
            EmployeePersonalViewModel model = new EmployeePersonalViewModel();
            model.FullName = searchText;
            model.BranchKey = branchId ?? 0;
            model.EmployeeStatusKey = statusId ?? 0;
            employeeList = employeeService.GetEmployees(model);

            int pageIndex = Convert.ToInt32(page) - 1;
            int pageSize = rows;
            int totalRecords = employeeList.Count();
            var totalPages = (int)Math.Ceiling((float)totalRecords / (float)rows);

            var jsonData = new
            {
                total = totalPages,
                page,
                records = totalRecords,
                rows = employeeList
            };
            return Json(jsonData, JsonRequestBehavior.AllowGet);
        }

        [ActionAuthenticationAttribute(MenuCode = MenuConstants.Employee, ActionCode = ActionConstants.Delete)]
        [HttpPost]
        public ActionResult DeleteEmployee(Int64 id)
        {
            EmployeePersonalViewModel objViewModel = new EmployeePersonalViewModel();

            objViewModel.RowKey = id;
            try
            {
                objViewModel = employeeService.DeleteEmployee(objViewModel);
            }
            catch (Exception)
            {
                objViewModel.Message = EduSuiteUIResources.Failed;
            }
            return Json(objViewModel);




        }

        [ActionAuthenticationAttribute(MenuCode = MenuConstants.Employee, ActionCode = ActionConstants.AddEdit)]
        [EncryptedActionParameter]
        [HttpGet]
        public ActionResult AddEditEmployee(long? id)
        {
            EmployeePersonalViewModel objViewModel = new EmployeePersonalViewModel();
            objViewModel.RowKey = id ?? 0;
            return View(objViewModel);
        }

        [HttpGet]
        public ActionResult OrganizationChart()
        {
            return View();
        }

        [HttpGet]
        public JsonResult GetOrganizationChart()
        {
            List<EmployeePersonalViewModel> employeeList = new List<EmployeePersonalViewModel>();
            employeeList = employeeService.GetOrganizationChart();

            return Json(employeeList, JsonRequestBehavior.AllowGet);
        }

        [EncryptedActionParameter]
        [HttpGet]
        public ActionResult EmployeeFileHandOver(long id)
        {
            EmployeeFileHandOverViewModel objViewModel = new EmployeeFileHandOverViewModel();
            objViewModel.EmployeeFromKey = id;
            objViewModel = employeeService.GetFileHandoverDropdownLists(objViewModel);
            return View(objViewModel);
        }
                
        [HttpPost]
        public JsonResult AddEditEmployeeFileHandover(List<EmployeeFileHandOverViewModel> modelList)
        {
            EmployeeFileHandOverViewModel model = employeeService.UpdateEmployeesFileHandover(modelList);

            if (model.Message != AppConstants.Common.SUCCESS)
            {
                //ModelState.AddModelError("error_msg", model.Message);
                Toastr.AddToastMessage(AppConstants.Common.FAILED, model.Message, ToastType.Error);
            }
            else
            {
                Toastr.AddToastMessage(AppConstants.Common.SUCCESS, model.Message, ToastType.Success);
                return Json(model);
            }


            return Json(model);
        }

        [HttpPost]
        public JsonResult ResetHandover(List<long> keyList)
        {
            EmployeeFileHandOverViewModel objViewModel = new EmployeeFileHandOverViewModel();
            try
            {
                objViewModel = employeeService.DeleteHandover(keyList);
                if (objViewModel.IsSuccessful)
                {
                    Toastr.AddToastMessage(AppConstants.Common.SUCCESS, objViewModel.Message, ToastType.Success);
                }
                else
                {
                    Toastr.AddToastMessage(AppConstants.Common.FAILED, objViewModel.Message, ToastType.Error);
                }
            }
            catch (Exception)
            {
                Toastr.AddToastMessage(AppConstants.Common.FAILED, EduSuiteUIResources.Failed, ToastType.Error);
            }
            return Json(objViewModel);
        }

        [HttpGet]
        public ActionResult EmployeePhoto(long id)
        {
            EmployeePersonalViewModel objViewModel = new EmployeePersonalViewModel();
            objViewModel = employeeService.GetEmployeePhotoById(id);
            return PartialView(objViewModel);
        }

        [HttpPost]
        public ActionResult UploadPhoto(HttpPostedFileBase PhotoFile, long EmployeeKey)
        {
            EmployeePersonalViewModel objViewModel = new EmployeePersonalViewModel();
            objViewModel.RowKey = EmployeeKey;
            objViewModel = employeeService.UpdateEmployeePhoto(objViewModel);
            if (objViewModel.IsSuccessful)
            {
                UploadFile(PhotoFile, Server.MapPath(UrlConstants.EmployeeUrl + objViewModel.RowKey + "/"), objViewModel.EmployeePhoto);
            }
            return Json(objViewModel);
        }

        [HttpPost]
        public ActionResult DeleteEmployeePhoto(long id)
        {
            EmployeePersonalViewModel objViewModel = new EmployeePersonalViewModel();
            objViewModel = employeeService.DeleteEmployeePhoto(id);
            if (objViewModel.IsSuccessful)
            {
                DeleteFile(UrlConstants.EmployeeUrl + objViewModel.RowKey + "/" + objViewModel.EmployeePhoto);
            }
            return Json(objViewModel);
        }

        private void UploadFile(HttpPostedFileBase PhotoFile, string FilePath, string FileName)
        {
            if (!Directory.Exists(FilePath))
            {
                Directory.CreateDirectory(FilePath);
            }
            if (PhotoFile != null)
            {
                PhotoFile.SaveAs(FilePath + FileName);
            }
        }
        private void DeleteFile(string FilePath)
        {
            if (System.IO.File.Exists(Server.MapPath(FilePath)))
            {
                System.IO.File.Delete(Server.MapPath(FilePath));
            }

        }

        [ActionAuthenticationAttribute(MenuCode = MenuConstants.Employee, ActionCode = ActionConstants.View)]
        public ActionResult ViewEmployee(long? id)
        {
            EmployeeViewModel model = new EmployeeViewModel();
            model = employeeService.GetEmployeeDetailsById(id);
            return View(model);
        }

        [HttpGet]
        public FileContentResult DownloadFilePhoto(string filename, string Studentname, string AdmissionNo)
        {
            string Photoname = Studentname + EduSuiteUIResources.OpenBracket + AdmissionNo + EduSuiteUIResources.CloseBracket;
            string FullPath = Server.MapPath(filename);
            return File(System.IO.File.ReadAllBytes(FullPath), System.Web.MimeMapping.GetMimeMapping(FullPath), Photoname + "_" + Path.GetFileName(FullPath));
        }
    }
}