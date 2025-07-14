using CITS.EduSuite.Business.Interfaces;
using CITS.EduSuite.Business.Models.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace CITS.EduSuite.UI.Controllers
{
    public class VideosController : BaseController
    {
        private IVideoService videoService;

        public VideosController(IVideoService objVideoService)
        {
            this.videoService = objVideoService;
        }



        //[ActionAuthenticationAttribute(MenuCode = MenuConstants.Application, ActionCode = ActionConstants.MenuAccess)]
        public ActionResult ViewVideos()
        {
            VideoViewModel model = new VideoViewModel();
            return View(model);
        }

        public ActionResult GetVideoCategory(VideoViewModel model)
        {
            videoService.GetSubjects(model);
            return PartialView(model);
        }
        public ActionResult GetVideos(VideoViewModel model)
        {
            videoService.GetVideoList(model);
            return PartialView(model);
        }

        public ActionResult GetVideosDetails(VideoViewModel model)
        {
            videoService.GetVideoDetailsList(model);
            return PartialView(model);
        }


    }
}