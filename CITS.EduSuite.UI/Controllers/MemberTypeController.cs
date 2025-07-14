using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using CITS.EduSuite.Business.Common;
using CITS.EduSuite.Business.Interfaces;
using CITS.EduSuite.Business.Models.Resources;
using CITS.EduSuite.Business.Models.ViewModels;


namespace CITS.EduSuite.UI.Controllers
{
    public class MemberTypeController : BaseController
    {
        private IMemberTypeService memberTypeService;

        public MemberTypeController(IMemberTypeService objMemberTypeService)
        {
            memberTypeService = objMemberTypeService;
        }
        [HttpGet]
        public ActionResult MemberTypeList()
        {
            return View();
        }

        [HttpGet]
        public JsonResult GetMemberTypes(string searchText)
        {
            int page = 1, rows = 10;

            List<MemberTypeViewModel> memberTypesList = new List<MemberTypeViewModel>();
            memberTypesList = memberTypeService.GetMemberTypes(searchText);

            int pageIndex = Convert.ToInt32(page) - 1;
            int pageSize = rows;
            int totalRecords = memberTypesList.Count();
            var totalPages = (int)Math.Ceiling((float)totalRecords / (float)rows);

            var jsonData = new
            {
                total = totalPages,
                page,
                records = totalRecords,
                rows = memberTypesList
            };
            return Json(jsonData, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public ActionResult AddEditMemberType(byte? id)
        {
            var memberTypes = memberTypeService.GetMemberTypeById(id);
            if (memberTypes == null)
            {
                memberTypes = new MemberTypeViewModel();
            }
            return PartialView(memberTypes);
        }

        [HttpPost]
        public ActionResult AddEditMemberType(MemberTypeViewModel memberType)
        {

            if (ModelState.IsValid)
            {
                if (memberType.RowKey == 0)
                {
                    memberType = memberTypeService.CreateMemberType(memberType);
                }
                else
                {
                    memberType = memberTypeService.UpdateMemberType(memberType);
                }

                if (memberType.Message != AppConstants.Common.SUCCESS)
                {
                    ModelState.AddModelError("error_msg", memberType.Message);
                }
                else
                {
                    return Json(memberType);
                }

                memberType.Message = "";
                return PartialView(memberType);
            }

            memberType.Message = EduSuiteUIResources.Failed;
            return PartialView(memberType);

        }

        [HttpPost]
        public ActionResult DeleteMemberType(byte id)
        {
            MemberTypeViewModel objViewModel = new MemberTypeViewModel();

            objViewModel.RowKey = id;
            try
            {
                objViewModel = memberTypeService.DeleteMemberType(objViewModel);
            }
            catch (Exception)
            {
                objViewModel.Message = EduSuiteUIResources.Failed;
            }
            return Json(objViewModel);
        }

        [HttpGet]
        public JsonResult CheckMemberTypeCodeExist(string MemberTypeCode, byte? RowKey)
        {
            MemberTypeViewModel model = new MemberTypeViewModel();
            model.MemberTypeCode = MemberTypeCode;
            model.RowKey = RowKey ?? 0;
            var Result = memberTypeService.CheckMemberTypeCodeExist(model);
            return Json(Result.IsSuccessful, JsonRequestBehavior.AllowGet);
        }
    }
}