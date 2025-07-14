using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CITS.EduSuite.Business.Models.ViewModels
{
    public class NotificationDataViewModel : BaseModel
    {
        public NotificationDataViewModel()
        {

        }
        public long RowKey { get; set; }
        public short BranchKey { get; set; }
        public int NotificationKey { get; set; }
        public long ApplicationKey { get; set; }     
        public string NotificationData { get; set; }
        public string MobileNumberSearchFeedback { get; set; }
        public string EmailTemplateName { get; set; }
        public string EmailSubject { get; set; }
        public string SMSTemplate { get; set; }
        public string EmailAddess { get; set; }
        public string MobileNumber { get; set; }
        public string AdminEmailAddress { get; set; }
        public long PushNotificationKey { get; set; }
        public int PushNotificationTemplateKey { get; set; }
        public string PushNotificationTitle { get; set; }
        public string PushNotificationContent { get; set; }
        public string PushNotificationRedirectUrl { get; set; }
        public List<long> PushNotificationUserkeys { get; set; }
        public bool PushNotificationRead { get; set; }
        public string PushNotificationType { get; set; }
        public DateTime? CreatedDate { get; set; }
        public byte NotificationTypeKey { get; set; }        
        public int PageIndex { get; set; }
        public int PageSize { get; set; }
        public int TotalRecords { get; set; }
        public int TotalLatestRecords { get; set; }
        public int TotalUnreadRecords { get; set; }
        public string GuardianSMSTemplate { get; set; }
        public string GuardianMobileNumber { get; set; }
        public long UserKey { get; set; }
        public string AddedByText { get; set; }
        public string SortBy { get; set; }
        public string SortOrder { get; set; }
        public string SearchText { get; set; }
        public long? AppUserKey { get; set; }
        public DateTime? SearchFromDate { get; set; }
        public DateTime? SearchToDate { get; set; }
        public bool? NotificationType { get; set; }
        public string SMSTemplateID { get; set; }
        public string SMSTemplateContent { get; set; }
    }
}
