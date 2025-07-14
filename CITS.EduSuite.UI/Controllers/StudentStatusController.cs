
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
    public class StudentStatusController : BaseController
    {
      private IStudentStatusService StudentStatusservice;
      public StudentStatusController(IStudentStatusService objStudentStatusService)
        {
            this.StudentStatusservice = objStudentStatusService;
        }
        [HttpGet]
        public ActionResult StudentStatusList()
        {
            return View();
        }
        [HttpGet]
        public ActionResult AddEditStudentStatus(int? id)
        {
            StudentStatusViewModel model = new StudentStatusViewModel();
            model = StudentStatusservice.GetStudentStatusById(id);
            if (model == null)
            {
                model = new StudentStatusViewModel();
            }
            return PartialView(model);

        }
        [HttpPost]
        public ActionResult AddEditStudentStatus(StudentStatusViewModel model)
        {

            if (ModelState.IsValid)
            {
                if (model.RowKey == 0)
                {
                    model = StudentStatusservice.CreateStudentStatus(model);
                }
                else
                {
                    model = StudentStatusservice.UpdateStudentStatus(model);
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

            

            model.Message = EduSuiteUIResources.Failed;

            return PartialView(model);
        }
        public JsonResult GetStudentStatus(string searchText)
        {
            int page = 1, rows = 15;
            List<StudentStatusViewModel> StudentStatusList = new List<StudentStatusViewModel>();
            StudentStatusList = StudentStatusservice.GetStudentStatus(searchText);
            int pageIndex = Convert.ToInt32(page) - 1;
            int pageSize = rows;
            int toatalRecords = StudentStatusList.Count();
            var totalPage = (int)Math.Ceiling((float)toatalRecords / (float)rows);

            var jsonData = new
            {
                total = totalPage,
                page,
                records = toatalRecords,
                rows = StudentStatusList
            };
            return Json(jsonData, JsonRequestBehavior.AllowGet);
        }
        [HttpPost]
        public ActionResult DeleteStudentStatus(short id)
        {
            StudentStatusViewModel objViewModel = new StudentStatusViewModel();

            objViewModel.RowKey = id;
            try
            {
                objViewModel = StudentStatusservice.DeleteStudentStatus(objViewModel);
            }
            catch (Exception)
            {
                objViewModel.Message = EduSuiteUIResources.Failed;
            }
            return Json(objViewModel);
        }
    }
}