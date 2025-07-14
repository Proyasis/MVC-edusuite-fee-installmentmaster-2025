using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CITS.EduSuite.Business.Models.ViewModels;


namespace CITS.EduSuite.Business.Interfaces
{
 public interface   IVideoService
    {
        List<VideoViewModel> GetVideos(VideoViewModel model, out long TotalRecords);
        VideoViewModel GetVideoById(VideoViewModel model);
        VideoViewModel CreateVideo(VideoViewModel model);
        VideoViewModel UpdateVideo(VideoViewModel model);
        void FillDropDownList(VideoViewModel model);
        VideoViewModel DeleteVideo(VideoViewModel model);
        VideoViewModel DeleteVideoDetails(VideoViewModel model);
        List<VideoViewModel> GetVideoByVideoKey(VideoViewModel model);
        List<VideoViewModel> GetVideoByVideoDetailsKey(VideoViewModel model);
        VideoViewModel GetSubjects(VideoViewModel model);
        VideoViewModel GetVideoList(VideoViewModel model);
        VideoViewModel GetVideoDetailsList(VideoViewModel model);

    }
}
