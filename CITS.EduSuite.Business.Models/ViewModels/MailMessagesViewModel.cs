using CITS.EduSuite.Business.Models.Resources;
using CITS.EduSuite.Business.Models.Security;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using CITS.Validations;
using System.Web.Mvc;


namespace CITS.EduSuite.Business.Models.ViewModels
{
    public class MailMessagesViewModel : BaseModel
    {
        public MailMessagesViewModel()
        {
            SendList = new List<MailMessagesSenderListViewModel>();
            MessageFiles = new List<HttpPostedFileBase>();
            FilesNames = new List<MailMessageFileNameList>();
            ToEmployees = new List<SelectListModel>();
            MailToUserKeys = new List<int>();
            ActiveTabKey = DbConstants.MailMessage.InBox;
        }
        public long RowKey { get; set; }
        [Required(ErrorMessageResourceType = typeof(ModelResources), ErrorMessageResourceName = "MessageSubjectRequired")]
        public string MessageSubject { get; set; }

        [AllowHtml]
        [Required(ErrorMessageResourceType = typeof(ModelResources), ErrorMessageResourceName = "MessageRequired")]
        public string MessageContent { get; set; }
        public string MessageStatusName { get; set; }
        public int AddedBy { get; set; }
        public int UserKey { get; set; }
        public DateTime? CreatedOn { get; set; }
        public int PageIndex { get; set; }
        public int PageSize { get; set; }
        public long MailMessageKey { get; set; }
        public short RoleKey { get; set; }
        public int TabKey { get; set; }
        public int ActiveTabKey { get; set; }
        public string MailFromUserName { get; set; }
        public string MailToUserName { get; set; }
        public long InboxCount { get; set; }
        public long InboxNewCount { get; set; }
        public long OutBoxCount { get; set; }
        public long DraftCount { get; set; }
        public long TrashCount { get; set; }
        public long StaredCount { get; set; }
        public long TotalRecords { get; set; }
        public int MailFromUserKey { get; set; }
        public int MailToUserKey { get; set; }

        [RequiredIfNot("ActiveTabKey", DbConstants.MailMessage.Draft, ErrorMessageResourceType = typeof(ModelResources), ErrorMessageResourceName = "MailToUserRequired")]
        public List<int> MailToUserKeys { get; set; }

        public string UserId { get; set; }
        public string UserIdList { get; set; }
        public string DirectionName { get; set; }
        public int? FileKey { get; set; }
        public int ActiveTabMailCount { get; set; }
        public bool IsRead { get; set; }
        public string NewFileNames { get; set; }
        public string ForwardFileNames { get; set; }
        public List<HttpPostedFileBase> MessageFiles { get; set; }
        public List<MailMessagesSenderListViewModel> SendList { get; set; }
        public List<MailMessageFileNameList> FilesNames { get; set; }
        public List<SelectListModel> ToEmployees { get; set; }
        public string SearchText { get; set; }
    }


    public class MailMessagesSenderListViewModel
    {
        public short MailMessageKey { get; set; }
        public short MessageStatusKey { get; set; }
        public int MessageFromUserKey { get; set; }
        public int MessageToUserKey { get; set; }
        public string MessageStatusName { get; set; }
        public string MailMessageUserName { get; set; }
    }

    public class MailMessageFileNameList
    {
        public string FileName { get; set; }
    }

}
