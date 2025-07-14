using CITS.EduSuite.Business.Interfaces;
using CITS.EduSuite.Business.Models.Resources;
using CITS.EduSuite.Business.Models.ViewModels;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace CITS.EduSuite.UI.Controllers
{
    public class VideoUploadController : BaseController
    {
        private IVideoService videoService;

        public VideoUploadController(IVideoService objVideoService)
        {
            this.videoService = objVideoService;
        }

        [ActionAuthenticationAttribute(MenuCode = MenuConstants.Video, ActionCode = ActionConstants.View)]
        public ActionResult VideoList()
        {
            VideoViewModel model = new VideoViewModel();
            return View(model);
        }

   
        [ActionAuthenticationAttribute(MenuCode = MenuConstants.Video, ActionCode = ActionConstants.AddEdit)]
        [EncryptedActionParameter]
        [HttpGet]
        public ActionResult AddEditVideoUpload(long? id)
        {
            long ApplicationKey = id != null ? Convert.ToInt64(id) : 0;
            VideoViewModel model = new VideoViewModel();
            model.RowKey = id ?? 0;
            model = videoService.GetVideoById(model);
            
            return View(model);
        }

        [HttpGet]
        public JsonResult GetVideos(string SearchText, string sidx, string sord, int page, int rows)
        {
            long TotalRecords = 0;
            List<VideoViewModel> applicationList = new List<VideoViewModel>();
            VideoViewModel objViewModel = new VideoViewModel();

            objViewModel.SearchText = SearchText;


            objViewModel.PageIndex = page;
            objViewModel.PageSize = rows;
            objViewModel.SortBy = sidx;
            objViewModel.SortOrder = sord;

            applicationList = videoService.GetVideos(objViewModel, out TotalRecords);

            int pageIndex = Convert.ToInt32(page) - 1;
            int pageSize = rows;
            var totalPages = (int)Math.Ceiling((float)TotalRecords / (float)rows);

            var jsonData = new
            {
                total = totalPages,
                page,
                records = TotalRecords,
                rows = applicationList
            };
            return Json(jsonData, JsonRequestBehavior.AllowGet);
        }


        [HttpPost]
        public ActionResult AddEditVideoUpload(VideoViewModel model)
        {
            if (ModelState.IsValid)
            {

                UpdateFileModel(model);


                if (model.RowKey == 0)
                {
                    model = videoService.CreateVideo(model);
                }
                else
                {
                    model = videoService.UpdateVideo(model);
                }
                if (Request.Files.Count > 0)
                {
                    UploadFile(model);
                }
            }
            else
            {
                var errors = ModelState.Select(x => x.Value.Errors).Where(y => y.Count > 0).ToList();
            }
            //VideoViewModel newModel = new VideoViewModel();

            //newModel= videoService.GetVideoById(model);
            return Json(model);
        }

        [HttpPost]
        public ActionResult GetVideosByVideoId(VideoViewModel model)
        {
            List<VideoViewModel> List = videoService.GetVideoByVideoKey(model);
            return PartialView(List);
        }

        [HttpPost]
        public ActionResult GetVideosByVideoDetailsId(VideoViewModel model)
        {
            List<VideoViewModel> List = videoService.GetVideoByVideoDetailsKey(model);
            return PartialView(List);
        }

        private void UpdateFileModel(VideoViewModel model)
        {

            int i = 0;
            foreach (var VideList in model.VideoList)
            {
                if (Request.Files[i].ContentLength > 0)
                {

                    VideList.VideoFileName = Path.GetExtension(VideList.VideoFileAttachment.FileName);
                }
                else
                {
                    VideList.VideoFileName = null;
                }
                i = i + 1;
            }
        }

        private void UploadFile(VideoViewModel model)
        {
            string FilePath = Server.MapPath(UrlConstants.VideoUpload+model.RowKey+"/");
            if (!Directory.Exists(FilePath))
            {
                Directory.CreateDirectory(FilePath);
            }

            foreach (VideoDetailsViewModel newModel in model.VideoList)
            {
                if (newModel.VideoFileAttachment != null)
                {

                    newModel.VideoFileAttachment.SaveAs(FilePath+ newModel.VideoFileName);
                
                }
            }
        }


        [ActionAuthenticationAttribute(MenuCode = MenuConstants.Video, ActionCode = ActionConstants.Delete)]
        [HttpPost]
        public ActionResult DeleteVideoDetails(Int64 id)
        {
            VideoViewModel objViewModel = new VideoViewModel();

            objViewModel.VideoDetailsKey = id;
            try
            {
                objViewModel = videoService.DeleteVideoDetails(objViewModel);
                if (objViewModel.IsSuccessful)
                {
                    DeleteFile(UrlConstants.VideoUpload + objViewModel.RowKey + "/" + objViewModel.VideoList[0].VideoFileName);
                }
            }
            catch (Exception)
            {
                objViewModel.Message = EduSuiteUIResources.Failed;
            }
            return Json(objViewModel);

        }

        private void DeleteFile(string FilePath)
        {
            if (System.IO.File.Exists(Server.MapPath(FilePath)))
            {
                System.IO.File.Delete(Server.MapPath(FilePath));
            }

        }


        [ActionAuthenticationAttribute(MenuCode = MenuConstants.Video, ActionCode = ActionConstants.Delete)]
        [HttpPost]
        public ActionResult DeleteVideo(Int64 id)
        {
            VideoViewModel objViewModel = new VideoViewModel();

            objViewModel.RowKey = id;
            try
            {
                objViewModel = videoService.DeleteVideo(objViewModel);
                DeleteFileFolder(UrlConstants.VideoUpload + id);
            }
            catch (Exception)
            {
                objViewModel.Message = EduSuiteUIResources.Failed;
            }
            return Json(objViewModel);
        }

        private void DeleteFileFolder(string FilePath)
        {
            if (System.IO.Directory.Exists(Server.MapPath(FilePath)))
            {
                var path = Server.MapPath(FilePath);
                System.IO.Directory.Delete(path, true);
            }
        }



    }
}