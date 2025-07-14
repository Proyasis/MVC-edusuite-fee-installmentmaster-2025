
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
    public class PublisherController : BaseController
    {
        private IPublisherService publisherService;

        public PublisherController(IPublisherService objPublisherService)
        {
            this.publisherService = objPublisherService;
        }

        [HttpGet]
        public ActionResult PublisherList()
        {
            return View();
        }

      

        public JsonResult GetPublisher(string searchText, string sidx, string sord, int page, int rows)
        {
            long TotalRecords = 0;

            List<PublisherViewModel> publisherList = new List<PublisherViewModel>();

            PublisherViewModel objViewModel = new PublisherViewModel();

            //
            objViewModel.SearchText = searchText;
            objViewModel.PageIndex = page;
            objViewModel.PageSize = rows;
            objViewModel.SortBy = sidx;
            objViewModel.SortOrder = sord;
            publisherList = publisherService.GetPublisher(objViewModel, out TotalRecords);

            int pageIndex = Convert.ToInt32(page) - 1;
            int pageSize = rows;
            int totalRecords = publisherList.Count();
            var totalPages = (int)Math.Ceiling((float)totalRecords / (float)rows);

            var jsonData = new
            {
                total = totalPages,
                page,
                records = totalRecords,
                rows = publisherList
            };
            return Json(jsonData, JsonRequestBehavior.AllowGet);


        }


        [HttpGet]
        public ActionResult AddEditPublisher(Int32? id)
        {
            var publishers = publisherService.GetPublisherById(id);
            if (publishers == null)
            {
                publishers = new PublisherViewModel();
            }
            return PartialView(publishers);
        }
        
        [HttpPost]
        public ActionResult AddEditPublisher(PublisherViewModel publisher)
        {
            if (ModelState.IsValid)
            {
                if (publisher.RowKey == 0)
                {
                    publisher = publisherService.CreatePublisher(publisher);
                }
                else
                {
                    publisher = publisherService.UpdatePublisher(publisher);
                }

                if (publisher.Message != AppConstants.Common.SUCCESS)
                {
                    ModelState.AddModelError("error_msg", publisher.Message);
                }
                else
                {
                    return Json(publisher);
                }

                publisher.Message = "";
                return PartialView(publisher);
            }

            publisher.Message = EduSuiteUIResources.Failed;
            return PartialView(publisher);
        }

        [HttpPost]
        public ActionResult DeletePublisher(Int32 id)
        {
            PublisherViewModel objViewModel = new PublisherViewModel();

            objViewModel.RowKey = id;
            try
            {
                objViewModel = publisherService.DeletePublisher(objViewModel);
            }
            catch (Exception)
            {
                objViewModel.Message = EduSuiteUIResources.Failed;
            }
            return Json(objViewModel);
        }
    }
}