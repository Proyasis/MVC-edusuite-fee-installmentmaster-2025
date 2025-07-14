using CITS.EduSuite.Business.Models.Resources;
using CITS.Validations;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace CITS.EduSuite.Business.Models.ViewModels
{
    public class VideoViewModel : BaseModel
    {

        public VideoViewModel()
        {
            VideoList = new List<VideoDetailsViewModel>();
            VideoSubject = new List<SelectListModel>();
            VisibilityTypes = new List<SelectListModel>();
            SubjectList = new List<ViewModels.SubjectList>();
            Videos = new List<VideoViewModel>();
            VideoTypes = new List<SelectListModel>();
        }
        public long RowKey { get; set; }
        public long VideoDetailsKey { get; set; }
        public string SubjectName { get; set; }

        [Required(ErrorMessageResourceType = typeof(ModelResources), ErrorMessageResourceName = "RequiredErrorMessage")]
        public long SubjectKey { get; set; }
        public int VideoCount { get; set; }
        public string AddedBy { get; set; }
        [Required(ErrorMessageResourceType = typeof(ModelResources), ErrorMessageResourceName = "RequiredErrorMessage")]
        public string VideoTitle { get; set; }
        public string SearchText { get; set; }

        [Required(ErrorMessageResourceType = typeof(ModelResources), ErrorMessageResourceName = "RequiredErrorMessage")]
        public short VisibilityTypeKey { get; set; }
        public int PageIndex { get; set; }
        public int PageSize { get; set; }
        public int TotalRecords { get; set; }
        public string SortBy { get; set; }
        public string SortOrder { get; set; }
        public string PlanKeysList { get; set; }
        public DateTime? CreatedDate { get; set; }
        public int UserKey { get; set; }
        public short RoleKey { get; set; }
        public bool IsActive { get; set; }
        public bool IsAllowDownload { get; set; }
        public List<VideoDetailsViewModel> VideoList { get; set; }
        public List<SelectListModel> VideoSubject { get; set; }
        public List<SubjectList> SubjectList { get; set; }

        public List<VideoViewModel> Videos { get; set; }
        public List<SelectListModel> VisibilityTypes { get; set; }
        public List<SelectListModel> VideoTypes { get; set; }



    }




    public class SubjectList
    {
        public long SubjectKey { get; set; }
        public string SubjectName { get; set; }
        public bool IsActive { get; set; }
    }

    public class VideoDetailsViewModel
    {

        //[RequiredIf("VideoTypeKey", DbConstants.VideoType.UploadFile, ErrorMessageResourceType = typeof(ModelResources), ErrorMessageResourceName = "RequiredErrorMessage")]
        //[Display(Name = "UploadFile", ResourceType = typeof(EduSuiteUIResources))]
        public HttpPostedFileBase VideoFileAttachment { get; set; }
        public long RowKey { get; set; }
        public long VideoKey { get; set; }

        public string VideoFileName { get; set; }

        [Required(ErrorMessageResourceType = typeof(ModelResources), ErrorMessageResourceName = "RequiredErrorMessage")]
        [Display(Name = "VideoDescriptions", ResourceType = typeof(EduSuiteUIResources))]
        public string VideoDecription { get; set; }
        //[Required(ErrorMessageResourceType = typeof(ModelResources), ErrorMessageResourceName = "RequiredErrorMessage")]
        public short? VisibilityTypeKey { get; set; }
        public long VideoDetailsKey { get; set; }
        public string SubjectName { get; set; }
        public bool IsActive { get; set; }
        public string VideoTitle { get; set; }
        public string VisibilityName { get; set; }
        public long SubjectKey { get; set; }
        public DateTime? CreatedDate { get; set; }
        public bool IsAllowDownload { get; set; }

        
        [RequiredIf("VideoTypeKey", DbConstants.VideoType.YouTubeLink, ErrorMessageResourceType = typeof(ModelResources), ErrorMessageResourceName = "RequiredErrorMessage")]
        [Display(Name = "YoutubeLink", ResourceType = typeof(EduSuiteUIResources))]
        [AllowHtml]
        public string YouTubeLinks { get; set; }

        [Required(ErrorMessageResourceType = typeof(ModelResources), ErrorMessageResourceName = "RequiredErrorMessage")]
        [Display(Name = "VideoType", ResourceType = typeof(EduSuiteUIResources))]
        public byte? VideoTypeKey { get; set; }

        public int? VideoViewCount { get; set; }
        public int? VideoDownloadCount { get; set; }
    }


}
