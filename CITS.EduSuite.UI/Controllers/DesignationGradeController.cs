using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using CITS.EduSuite.Business.Common;
using CITS.EduSuite.Business.Interfaces;
using CITS.EduSuite.Business.Models.ViewModels;
//using CITS.EduSuite.UI.Helper;
using CITS.EduSuite.Business.Provider;
using System.Web.Security;
using CITS.EduSuite.Business.Models.Security;
using CITS.EduSuite.Business.Models.Resources;

namespace CITS.EduSuite.UI.Controllers
{
    public class DesignationGradeController : BaseController
    {
        private IDesignationGradeService designationGradeService;
        public DesignationGradeController(IDesignationGradeService objDesignationGradeService)
        {
            this.designationGradeService = objDesignationGradeService;

        }

        [ActionAuthenticationAttribute(MenuCode = MenuConstants.GradeWiseSalaryStructure, ActionCode = ActionConstants.MenuAccess)]
        [HttpGet]
        public ActionResult DesignationGradeList()
        {
            DesignationGradeViewModel DesignationGrade = new DesignationGradeViewModel();

            return View(DesignationGrade);
        }

        [HttpGet]
        public JsonResult GetDesignationGrades(string searchText)
        {
            int page = 1, rows = 10;

            List<DesignationGradeViewModel> DesignationGradeList = new List<DesignationGradeViewModel>();
            DesignationGradeList = designationGradeService.GetDesignationGrades(searchText);

            int pageIndex = Convert.ToInt32(page) - 1;
            int pageSize = rows;
            int totalRecords = DesignationGradeList.Count();
            var totalPages = (int)Math.Ceiling((float)totalRecords / (float)rows);

            //if (DesignationGradeList.Count > 0)
            //{
            //    DesignationGradeViewModel model = DesignationGradeList[0];
            //    if (model.ExceptionMessage != null && model.ExceptionMessage != "")
            //    {
            //        ActivityLog.CreateActivityLog(GetUserKey(), MenuConstant.DesignationGrade, ActionConstant.MenuAccess, DbConstants.LogType.Error, 0, model.ExceptionMessage);
            //    }
            //}

            var jsonData = new
            {
                total = totalPages,
                page,
                records = totalRecords,
                rows = DesignationGradeList
            };
            return Json(jsonData, JsonRequestBehavior.AllowGet);
        }

        [ActionAuthenticationAttribute(MenuCode = MenuConstants.GradeWiseSalaryStructure, ActionCode = ActionConstants.AddEdit)]
        [HttpGet]
        //[EncryptedActionParameter]
        public ActionResult AddEditDesignationGrade(short? id)
        {
            DesignationGradeViewModel objViewModel = new DesignationGradeViewModel();
            objViewModel = designationGradeService.GetDesignationGradebyId(id ?? 0);
            //objViewModel.DesignationKey = id ?? 0;
            //if (objViewModel.ExceptionMessage != null && objViewModel.ExceptionMessage != "")
            //{
            //    ActivityLog.CreateActivityLog(GetUserKey(), MenuConstant.DesignationGrade, id != null && id != 0 ? ActionConstant.Edit : ActionConstant.Add, DbConstants.LogType.Error, id, objViewModel.ExceptionMessage);
            //}
            return View(objViewModel);
        }


        [HttpPost]
        public ActionResult AddEditDesignationGrade(DesignationGradeViewModel model)
        {

            List<DesignationGradeViewModel> modelList = new List<DesignationGradeViewModel>();
            if (ModelState.IsValid)
            {
                modelList.Add(model);
                model = designationGradeService.UpdateDesignationGrade(modelList);
               
                if (model.Message != AppConstants.Common.SUCCESS)
                {
                    ModelState.AddModelError("error_msg", model.Message);
                }
                else
                {
                    return RedirectToAction("DesignationGradeList");
                }

                model.Message = "";
                return View(model);
            }
            model.Message = EduSuiteUIResources.Failed;
            return View(model);
        }
        [HttpGet]
        public ActionResult SalaryDetailsList(short? id)
        {
            DesignationGradeViewModel objViewModel = new DesignationGradeViewModel();
            objViewModel.DesignationGradeDetails = designationGradeService.GetDesignationGradeDetailsById(id ?? 0);
            return PartialView(objViewModel);
        }

        [HttpPost]
        public ActionResult UpdateDesignationGradeName(int Id, string DesignationGradeName)
        {
            DesignationGradeViewModel designationGrade = new DesignationGradeViewModel();
            designationGrade.RowKey = Id;
            designationGrade.DesignationGradeName = DesignationGradeName;
            designationGrade = designationGradeService.UpdateDesignationGradeName(designationGrade);
            //if (designationGrade.ExceptionMessage != null && designationGrade.ExceptionMessage != "")
            //{
            //    ActivityLog.CreateActivityLog(GetUserKey(), MenuConstant.DesignationGrade, ActionConstant.Edit, DbConstants.LogType.Error, designationGrade.RowKey, designationGrade.ExceptionMessage);
            //}
            //else
            //{
            //    ActivityLog.CreateActivityLog(GetUserKey(), MenuConstant.DesignationGrade, ActionConstant.Edit, DbConstants.LogType.Info, designationGrade.RowKey, designationGrade.Message);
            //}
            if (designationGrade.Message != AppConstants.Common.SUCCESS)
            {
                ModelState.AddModelError("error_msg", designationGrade.Message);
            }
            else
            {
                return Json(designationGrade);
            }

            designationGrade.Message = "";
            return View(designationGrade);

        }


        [HttpGet]
        public JsonResult GetDesignationGradeDetailsById(short? id)
        {
            DesignationGradeViewModel objViewModel = new DesignationGradeViewModel();
            objViewModel.DesignationGradeDetails = designationGradeService.GetDesignationGradeDetailsById(id ?? 0);
            if (objViewModel == null)
            {
                objViewModel = new DesignationGradeViewModel();
            }
            //if (objViewModel.ExceptionMessage != null && objViewModel.ExceptionMessage != "")
            //{
            //    ActivityLog.CreateActivityLog(GetUserKey(), MenuConstant.DesignationGrade, id != null && id != 0 ? ActionConstant.Edit : ActionConstant.Add, DbConstants.LogType.Error, id, objViewModel.ExceptionMessage);
            //}
            var jsonData = objViewModel.DesignationGradeDetails.Select(row => new
            {
                row.DesignationGradeName,
                row.SalaryHeadCode,
                row.SalaryHeadName,
                row.Formula
            });
            return Json(jsonData, JsonRequestBehavior.AllowGet);
        }

        //[ActionAuthenticationAttribute(MenuCode = MenuConstant.DesignationGrade, ActionCode = ActionConstant.Delete)]
        [HttpPost]
        public ActionResult DeleteDesignationGrade(short id)
        {
            DesignationGradeViewModel objViewModel = new DesignationGradeViewModel();
            objViewModel.DesignationKey = id;
            try
            {
                objViewModel = designationGradeService.DeleteDesignationGrade(objViewModel);
                //if (objViewModel.ExceptionMessage != null && objViewModel.ExceptionMessage != "")
                //{
                //    ActivityLog.CreateActivityLog(GetUserKey(), MenuConstant.DesignationGrade, ActionConstant.Delete, DbConstants.LogType.Error, id, objViewModel.ExceptionMessage);
                //}
                //else
                //{
                //    ActivityLog.CreateActivityLog(GetUserKey(), MenuConstant.DesignationGrade, ActionConstant.Delete, DbConstants.LogType.Info, id, objViewModel.Message);
                //}
            }
            catch (Exception)
            {
                objViewModel.Message = EduSuiteUIResources.Failed;
            }
            return Json(objViewModel);

        }


        [HttpPost]
        public ActionResult DeleteDesignationGradeDetail(int id)
        {
            DesignationGradeViewModel objViewModel = new DesignationGradeViewModel();
            objViewModel.RowKey = id;
            try
            {
                objViewModel = designationGradeService.DeleteDesignationGradeDetail(objViewModel);
                //if (objViewModel.ExceptionMessage != null && objViewModel.ExceptionMessage != "")
                //{
                //    ActivityLog.CreateActivityLog(GetUserKey(), MenuConstant.DesignationGrade, ActionConstant.DeleteItem, DbConstants.LogType.Error, id, objViewModel.ExceptionMessage);
                //}
                //else
                //{
                //    ActivityLog.CreateActivityLog(GetUserKey(), MenuConstant.DesignationGrade, ActionConstant.DeleteItem, DbConstants.LogType.Info, id, objViewModel.Message);
                //}
            }
            catch (Exception)
            {
                objViewModel.Message = EduSuiteUIResources.Failed;
            }
            return Json(objViewModel);

        }





    }

}