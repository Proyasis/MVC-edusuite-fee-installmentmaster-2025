using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using CITS.EduSuite.Business.Common;
using CITS.EduSuite.Business.Interfaces;
using CITS.EduSuite.Business.Models.ViewModels;
using CITS.EduSuite.Business.Provider;
using System.Web.Security;
using CITS.EduSuite.Business.Models.Security;
using CITS.EduSuite.Business.Models.Resources;
using System.IO;

namespace CITS.EduSuite.UI.Controllers
{
    public class UserManualController : BaseController
    {
        private IUserManualService userManualService;
        private ISelectListService selectListService;
        public UserManualController(IUserManualService objuserManualService, ISelectListService objselectListService)
        {
            this.userManualService = objuserManualService;
            this.selectListService = objselectListService;
        }

        [HttpGet]
        public ActionResult UserManualList()
        {
            UserManualViewModel UserManualView = new UserManualViewModel();
            UserManualView.MenuTypes = selectListService.FillMenuType();
            return View(UserManualView);
        }

        [HttpGet]
        public JsonResult GetUserManual(string searchText, short? MenuTypekey)
        {
            int page = 1, rows = 10;

            List<UserManualViewModel> UserManualViewList = new List<UserManualViewModel>();
            UserManualViewModel model = new UserManualViewModel();
            //model.MenuName = searchText;
            //model.MenuTypeKey = MenuTypekey;
            UserManualViewList = userManualService.GetUserManual(model);

            int pageIndex = Convert.ToInt32(page) - 1;
            int pageSize = rows;
            int totalRecords = UserManualViewList.Count();
            var totalPages = (int)Math.Ceiling((float)totalRecords / (float)rows);

            var jsonData = new
            {
                total = totalPages,
                page,
                records = totalRecords,
                rows = UserManualViewList
            };
            return Json(jsonData, JsonRequestBehavior.AllowGet);
        }

        [ActionAuthenticationAttribute(MenuCode = MenuConstants.Menu, ActionCode = ActionConstants.AddEdit)]
        [HttpGet]
        public ActionResult AddEditUserManual(int? id)
        {
            UserManualViewModel model = new UserManualViewModel();
            model = userManualService.GetUserManualById(id);
            if (model == null)
            {
                model = new UserManualViewModel();
            }
            model.UserManualTypes = selectListService.FillUserManualTypes();
            model.MenuTypes = selectListService.FillMenuType();
            model.DashBoardTypes = selectListService.FillDashBoardTypes();
            model.Menus = selectListService.FillMenu(model.MenuTypeKey);
            return PartialView(model);
        }

        [HttpPost]
        public ActionResult AddEditUserManual(UserManualViewModel model)
        {

            if (ModelState.IsValid)
            {
                UpdateDocumentModel(model);
                if (model.RowKey == 0)
                {
                    model = userManualService.CreateUserManual(model);
                }
                else
                {
                    model = userManualService.UpdateUserManual(model);
                }

                if (model.Message != AppConstants.Common.SUCCESS)
                {
                    ModelState.AddModelError("error_msg", model.Message);
                }
                else
                {
                    if (model.DocumentFile != null)
                    {
                        //UploadFile(model);
                        string Extension = Path.GetExtension(model.DocumentFile.FileName);
                        string FileName = model.RowKey + Extension;
                        UploadFiles(model.DocumentFile, Server.MapPath(UrlConstants.UserManual + model.RowKey + "/"), FileName);
                    }
                    UploadDetailsFiles(model);
                    return Json(model);
                }


                return Json(model);
            }
            foreach (ModelState state in ViewData.ModelState.Values.Where(x => x.Errors.Count > 0))
            {
            }

            model.Message = EduSuiteUIResources.Failed;
            return Json(model);
        }
        private void UploadFiles(HttpPostedFileBase PhotoFile, string FilePath, string FileName)
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

        private void UpdateDocumentModel(UserManualViewModel model)
        {
            for (int i = 0; i < model.UserManualDetails.Count; i++)
            {

                model.UserManualDetails[i].DocumentFileDetails = Request.Files["UserManualDetails[" + i + "].DocumentFileDetails"];
                if (model.UserManualDetails[i].DocumentFileDetails.ContentLength > 0)
                {
                    model.UserManualDetails[i].DocumentPath = Path.GetExtension(model.UserManualDetails[i].DocumentFileDetails.FileName);
                }
            }
        }
        private void UploadDetailsFiles(UserManualViewModel model)
        {
            string FilePath = Server.MapPath(UrlConstants.UserManual + model.RowKey + "/UserManualDetails/");
            if (!Directory.Exists(FilePath))
            {
                Directory.CreateDirectory(FilePath);
            }
            foreach (UserManualDetailsViewModel item in model.UserManualDetails)
            {
                if (item.DocumentFileDetails.ContentLength > 0)
                {

                    item.DocumentFileDetails.SaveAs(FilePath + item.DocumentPath);
                    item.DocumentFileDetails = null;
                }
            }
        }


        [ActionAuthenticationAttribute(MenuCode = MenuConstants.Menu, ActionCode = ActionConstants.Delete)]
        [HttpPost]
        public ActionResult DeleteUserManual(Int16 id)
        {
            UserManualViewModel objViewModel = new UserManualViewModel();

            objViewModel.RowKey = id;
            try
            {
                objViewModel = userManualService.DeleteUserManual(objViewModel);
                if (objViewModel.IsSuccessful)
                {
                    DeleteFolder(UrlConstants.UserManual + objViewModel.RowKey);
                }
            }
            catch (Exception)
            {
                objViewModel.Message = EduSuiteUIResources.Failed;
            }
            return Json(objViewModel);
        }


        [ActionAuthenticationAttribute(MenuCode = MenuConstants.Menu, ActionCode = ActionConstants.Delete)]
        [HttpPost]
        public ActionResult DeleteUserManualDetails(Int16 id)
        {
            UserManualDetailsViewModel objViewModel = new UserManualDetailsViewModel();

            objViewModel.RowKey = id;
            try
            {
                objViewModel = userManualService.DeleteUserManualDetails(objViewModel);
                if (objViewModel.IsSuccessful)
                {
                    DeleteFile(UrlConstants.UserManual + objViewModel.UserManualMasterKey + "/UserManualDetails/" + objViewModel.DocumentPath);
                }
            }
            catch (Exception)
            {
                objViewModel.Message = EduSuiteUIResources.Failed;
            }
            return Json(objViewModel);
        }

        [HttpPost]
        public ActionResult DeleteUserManualDocument(short id)
        {
            UserManualViewModel objViewModel = new UserManualViewModel();
            objViewModel = userManualService.DeleteUserManualDocument(id);
            if (objViewModel.IsSuccessful)
            {
                if (objViewModel.DocumentPath != null)
                {
                    DeleteFile(objViewModel.DocumentPath);
                }

            }
            return Json(objViewModel);
        }

        private void DeleteFile(string FilePath)
        {
            if (System.IO.File.Exists(Server.MapPath(FilePath)))
            {
                System.IO.File.Delete(Server.MapPath(FilePath));
            }

        }
        private void DeleteFolder(string FolderPath)
        {
            if (System.IO.Directory.Exists(Server.MapPath(FolderPath)))
            {
                System.IO.Directory.Delete(Server.MapPath(FolderPath), true);
            }
        }

        [ActionAuthenticationAttribute(MenuCode = MenuConstants.Menu, ActionCode = ActionConstants.AddEdit)]
        [HttpGet]
        public ActionResult UserManuaView()
        {
            UserManualAllViewModel model = new UserManualAllViewModel();
            model = userManualService.ViewUserManualAll();
            if (model == null)
            {
                model = new UserManualAllViewModel();
            }
           
            return View(model);
        }

    }
}