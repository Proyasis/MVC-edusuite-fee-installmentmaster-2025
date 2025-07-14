using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using CITS.EduSuite.Business.Interfaces;
using CITS.EduSuite.Business.Models.ViewModels;

using CITS.EduSuite.Business.Common;
using CITS.EduSuite.Business.Models.Resources;

namespace CITS.EduSuite.UI.Controllers
{
    public class BuildingController : BaseController
    {
        private IBuildingService buildingService;

        public BuildingController(IBuildingService objBuildingService)
        {
            this.buildingService = objBuildingService;
        }

        [HttpGet]
        public ActionResult BuildingDetailsList()
        {
            BuildingViewModel Model = new BuildingViewModel();
            return View(Model);
        }

        [HttpGet]
        public JsonResult GetBuildingDetails(string searchText)
        {
            int page = 1; int rows = 15;
            List<BuildingViewModel> buildingDetailsList = new List<BuildingViewModel>();
            BuildingViewModel objViewModel = new BuildingViewModel();
            objViewModel.BuildingMasterName = searchText;
            buildingDetailsList = buildingService.GetBuildingDetails(searchText);
            int pageindex = Convert.ToInt32(page) - 1;
            int pagesize = rows;
            int totalrecords = buildingDetailsList.Count();
            var totalpage = (int)Math.Ceiling((float)totalrecords / (float)rows);
            var jsonData = new
            {
                total = totalpage,
                page,
                records = totalrecords,
                rows = buildingDetailsList
            };
            return Json(jsonData, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public ActionResult AddEditBuildingDetails(int? id)
        {
            BuildingViewModel model = new BuildingViewModel();
            model.RowKey = id ?? 0;
            model = buildingService.GetBuildingDetailsById(id);
            if (model == null)
            {
                model = new BuildingViewModel();
            }
            return View(model);
        }

        [HttpPost]
        public ActionResult AddEditBuildingDetails(BuildingViewModel model)
        {
            if (ModelState.IsValid)
            {

                if (model.RowKey != 0)
                {
                    model = buildingService.UpdateBuildingMaster(model);
                }
                else
                {
                    model = buildingService.CreateBuildingMaster(model);
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
                return View(model);
            }
            foreach (ModelState state in ViewData.ModelState.Values.Where(x => x.Errors.Count > 0))
            {
            }

            model.Message = EduSuiteUIResources.Failed;
            return View(model);
        }

        [HttpPost]
        public ActionResult DeleteBuildingDetailsAll(int id)
        {
            BuildingViewModel model = new BuildingViewModel();
            try
            {
                model = buildingService.DeleteBuildingDetailsAll(id);
            }
            catch (Exception)
            {
                model.Message = EduSuiteUIResources.Failed;
            }
            return Json(model);

        }
        [HttpPost]
        public ActionResult DeleteBuildingDetails(long RowKey, int BuildingMasterKey)
        {
            BuildingViewModel model = new BuildingViewModel();
            try
            {
                model = buildingService.DeleteBuildingDetails(RowKey, BuildingMasterKey);
            }
            catch (Exception)
            {
                model.Message = EduSuiteUIResources.Failed;
            }
            return Json(model);

        }

        [HttpPost]
        public ActionResult GetRoomDetails(BuildingViewModel model)
        {
            List<BuildingDetailsModel> modelList = new List<BuildingDetailsModel>();
            for (int i = 0; i < model.RoomCount; i++)
            {
                if (i >= model.BuildingDetails.Count)
                {
                   // modelList.Add(new BuildingDetailsModel());
                    modelList.Add(new BuildingDetailsModel { BuildingDetailsName = (string)(model.BuildingMasterName + (i + 1)) });
                }
            }
            if (model.RoomCount < model.BuildingDetails.Count)
            {
                model.BuildingDetails.RemoveRange(((model.RoomCount ?? 0) - 1), (model.BuildingDetails.Count - (model.RoomCount ?? 0)));
               //model.BuildingDetails.RemoveRange((model.RoomCount ?? 0), (model.BuildingDetails.Count - (model.RoomCount ?? 0)));
            }
            model.BuildingDetails.AddRange(modelList);
            return PartialView(model.BuildingDetails);
        }

        [HttpGet]
        public ActionResult GetRoomDetails(int? Id)
        {
            BuildingViewModel model = new BuildingViewModel();

            model = buildingService.GetBuildingDetailsById(Id);
            if (model == null)
            {
                return Json(model);
            }
            return PartialView(model.BuildingDetails);
        }

        [HttpGet]
        public JsonResult CheckRoomName(string BuildingDetailsName)
        {
            return Json(true, JsonRequestBehavior.AllowGet);
        }
    }
}