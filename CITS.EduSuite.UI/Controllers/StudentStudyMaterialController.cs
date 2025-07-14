using CITS.EduSuite.Business.Interfaces;
using CITS.EduSuite.Business.Models.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace CITS.EduSuite.UI.Controllers
{
    public class StudentStudyMaterialController : BaseController
    {
        private IStudentStudyMaterialService StudyMaterialService;

        public StudentStudyMaterialController(IStudentStudyMaterialService objStudyMaterialService)
        {
            this.StudyMaterialService = objStudyMaterialService;
        }
        public ActionResult ViewStudyMaterial()
        {
            StudentStudyMaterialViewModel model = new StudentStudyMaterialViewModel();
            return View(model);
        }
        public ActionResult GetStudyMaterialCategory(StudentStudyMaterialViewModel model)
        {
            StudyMaterialService.GetStudyMaterialCategories(model);
            return PartialView(model);
        }
        public ActionResult GetStudyMaterials(StudentStudyMaterialViewModel model)
        {
            StudyMaterialService.GetStudyMaterialList(model);
            return PartialView(model);
        }
    }
}