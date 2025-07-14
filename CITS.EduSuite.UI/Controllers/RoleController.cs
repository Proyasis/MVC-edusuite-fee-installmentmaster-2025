
using CITS.EduSuite.Business.Common;
using CITS.EduSuite.Business.Interfaces;
using CITS.EduSuite.Business.Models.Resources;
using CITS.EduSuite.Business.Models.ViewModels;
using CITS.EduSuite.Business.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace CITS.EduSuite.UI.Controllers
{
    public class RoleController : BaseController
    {
        private IRoleService roleService;

        public RoleController(IRoleService objRoleService)
        {
            this.roleService = objRoleService;
        }

        [HttpGet]
        public ActionResult RoleList()
        {
            RolesViewModel objViewModel = new RolesViewModel();
            return View();
        }

        [HttpGet]
        public JsonResult GetRoles(string searchText)
        {
            int page = 1, rows = 10;

            List<RolesViewModel> rolesList = new List<RolesViewModel>();
            rolesList = roleService.GetRoles(searchText);

            int pageIndex = Convert.ToInt32(page) - 1;
            int pageSize = rows;
            int totalRecords = rolesList.Count();
            var totalPages = (int)Math.Ceiling((float)totalRecords / (float)rows);

            var jsonData = new
            {
                total = totalPages,
                page,
                records = totalRecords,
                rows = rolesList
            };
            return Json(jsonData, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public ActionResult AddEditRole(short? id)
        {
            var roles = roleService.GetRoleById(id);
            if (roles == null)
            {
                roles = new RolesViewModel();
            }
            return View(roles);
        }

        [HttpPost]
        public ActionResult AddEditRole(RolesViewModel role)
        {

            if (ModelState.IsValid)
            {
                if (role.RowKey == 0)
                {
                    role = roleService.CreateRole(role);
                }
                else
                {
                    role = roleService.UpdateRole(role);
                }

                if (role.Message != AppConstants.Common.SUCCESS)
                {
                    ModelState.AddModelError("error_msg", role.Message);
                }
                else
                {
                    return RedirectToAction("RoleList");
                }

                role.Message = "";
                return View(role);
            }

            role.Message = EduSuiteUIResources.Failed;
            return View(role);

        }

        [HttpPost]
        public ActionResult DeleteRole(Int16 id)
        {
            RolesViewModel objViewModel = new RolesViewModel();

            objViewModel.RowKey = id;
            try
            {
                objViewModel = roleService.DeleteRole(objViewModel);
            }
            catch (Exception)
            {
                objViewModel.Message = EduSuiteUIResources.Failed;
            }
            return Json(objViewModel);
        }
    }
}