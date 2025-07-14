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
    public class AgentController : BaseController
    {
        private IAgentService AgentService;
        public AgentController(IAgentService objAgentService)
        {
            this.AgentService = objAgentService;
        }

        [ActionAuthenticationAttribute(MenuCode = MenuConstants.Agent, ActionCode = ActionConstants.MenuAccess)]
        [HttpGet]
        public ActionResult AgentList()
        {
            return View();
        }

        [ActionAuthenticationAttribute(MenuCode = MenuConstants.Agent, ActionCode = ActionConstants.AddEdit)]
        [HttpGet]
        public ActionResult AddEditAgent(int? id)
        {
            AgentViewModel model = new AgentViewModel();
            model = AgentService.GetAgentById(id);
            if (model == null)
            {
                model = new AgentViewModel();
            }
            return PartialView(model);

        }
        [HttpPost]
        public ActionResult AddEditAgent(AgentViewModel model)
        {

            if (ModelState.IsValid)
            {
                if (model.RowKey == 0)
                {
                    model = AgentService.CreateAgent(model);
                }
                else
                {
                    model = AgentService.UpdateAgent(model);
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
        public JsonResult GetAgent(string searchText)
        {
            int page = 1, rows = 15;
            List<AgentViewModel> AgentList = new List<AgentViewModel>();
            AgentList = AgentService.GetAgent(searchText);
            int pageIndex = Convert.ToInt32(page) - 1;
            int pageSize = rows;
            int toatalRecords = AgentList.Count();
            var totalPage = (int)Math.Ceiling((float)toatalRecords / (float)rows);

            var jsonData = new
            {
                total = totalPage,
                page,
                records = toatalRecords,
                rows = AgentList
            };
            return Json(jsonData, JsonRequestBehavior.AllowGet);
        }

        [ActionAuthenticationAttribute(MenuCode = MenuConstants.Agent, ActionCode = ActionConstants.Delete)]
        [HttpPost]
        public ActionResult DeleteAgent(short id)
        {
            AgentViewModel objViewModel = new AgentViewModel();

            objViewModel.RowKey = id;
            try
            {
                objViewModel = AgentService.DeleteAgent(objViewModel);
            }
            catch (Exception)
            {
                objViewModel.Message = EduSuiteUIResources.Failed;
            }
            return Json(objViewModel);
        }
    }
}