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
    public class EmployeeUserPermissionController : BaseController
    {
        private IEmployeeUserPermissionService employeeUserPermissionService;

        public EmployeeUserPermissionController(IEmployeeUserPermissionService objEmployeeUserPermissionService)
        {
            this.employeeUserPermissionService = objEmployeeUserPermissionService;
        }

        [ActionAuthenticationAttribute(MenuCode = MenuConstants.Employee, ActionCode = ActionConstants.UserPermission)]
        [HttpGet]
        [EncryptedActionParameter]
        public ActionResult AddEditEmployeeUserPermission(long? id)
        {
            EmployeeUserPermissionViewModel ObjViewModel = new EmployeeUserPermissionViewModel();
            ObjViewModel.EmployeeKey = id ?? 0;
            employeeUserPermissionService.FillMenuTypes(ObjViewModel);
            return View(ObjViewModel);
        }

        [HttpPost]
        public ActionResult AddEditEmployeeUserPermission(EmployeeUserPermissionViewModel model)
        {
            if (ModelState.IsValid)
            {

                model = employeeUserPermissionService.UpdateEmployeeUserPermission(model);
                if (model.Message != AppConstants.Common.SUCCESS)
                {
                    ModelState.AddModelError("error_msg", model.Message);
                }
                else
                {
                    return Json(model);
                }
                return Json(model);
            }

            model.Message = EduSuiteUIResources.Failed;
            return Json(model);

        }

        [HttpGet]
        public JsonResult GetEmployeeUserPermissionById(long id)
        {
            EmployeeUserPermissionViewModel EmployeeUserPermissionModel = new EmployeeUserPermissionViewModel();
            EmployeeUserPermissionModel = employeeUserPermissionService.GetUserPermissionsById(id);
            var tree = new
            {
                UserPermission = EmployeeUserPermissionModel.UserPermissions.Where(row => row.ActionKey == null).OrderBy(row => row.DisplayOrder).Select(row => new
                {
                    type = row.MenuTypeKey,
                    id = row.MenuKey,
                    text = row.MenuName,
                    state = new { selected = (row.IsActive ?? false) ? true : false },
                    children = EmployeeUserPermissionModel.UserPermissions.OrderBy(x => x.ActionKey).Where(x => x.MenuKey == row.MenuKey && x.ActionKey != null).Select(Y => new
                    {
                        id = "M" + Y.MenuKey + "A" + Y.ActionKey,
                        text = Y.ActionName,
                        state = new { selected = (Y.IsActive ?? false) ? true : false },
                        data = new { key = Y.RowKey, mid = Y.MenuKey, aid = Y.ActionKey }
                    })
                }).ToList(),

                DashBoardPermission = EmployeeUserPermissionModel.DashBoardPermission.Where(row => row.DashBoardContentKey == null).OrderBy(row => row.DisplayOrder).Select(row => new
                {
                    type = row.DashBoardTypeKey,
                    id = row.DashBoardTypeKey,
                    text = row.DashBoardTypeName,
                    state = new { selected = (row.IsActive ?? false) ? true : false },
                    children = EmployeeUserPermissionModel.DashBoardPermission.OrderBy(x => x.DashBoardContentKey).Where(x => x.DashBoardTypeKey == row.DashBoardTypeKey && x.DashBoardContentKey != null).Select(Y => new
                    {
                        id = "M" + Y.DashBoardTypeKey + "A" + Y.DashBoardContentKey,
                        text = Y.DashBoardContentName,
                        state = new { selected = (Y.IsActive ?? false) ? true : false },
                        data = new { key = Y.RowKey, mid = Y.DashBoardTypeKey, aid = Y.DashBoardContentKey }
                    })
                }).ToList(),
                //CountryAccess = new
                //{
                //    id = "Country",
                //    text = "Country",
                //    children = EmployeeUserPermissionModel.Countries.Select(Y => new
                //    {
                //        id = "C" + Y.RowKey,
                //        text = Y.Text,
                //        state = new { selected = Y.Selected ? true : false },
                //        data = new { key = Y.RowKey, mid = Y.RowKey }
                //    })
                //},
                BranchAccess = new
                {
                    id = "Branch",
                    text = "Branch",
                    children = EmployeeUserPermissionModel.Branches.Select(Y => new
                    {
                        id = "B" + Y.RowKey,
                        text = Y.Text,
                        state = new { selected = Y.Selected ? true : false },
                        data = new { key = Y.RowKey, mid = Y.RowKey }
                    })
                }
            };

            return Json(tree, JsonRequestBehavior.AllowGet);

        }

    }
}