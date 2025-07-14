using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace CITS.EduSuite.Business.Models.ViewModels
{
    public class NotificationTemplateViewModel : BaseModel
    {
        public NotificationTemplateViewModel()
        {
            NotificationColumns = new List<NotificationColumnViewModel>();
            NotificationStatuses = new List<SelectListModel>();
        }
        public int RowKey { get; set; }

        public string NotificationTemplateName { get; set; }
        public bool? AutoSMS { get; set; }
        public int AutoSMSValue { get; set; }
        public string AutoSMSText { get; set; }
        public bool? AutoEmail { get; set; }
        public int AutoEmailValue { get; set; }
        public string AutoEmailText { get; set; }

        [UIHint("tinymce_jquery_simple"), AllowHtml]
        public string SMSTemplate { get; set; }

        [UIHint("tinymce_jquery_full"), AllowHtml]
        public string EmailTemplate { get; set; }
        public string EmailTemplateFileName { get; set; }
        public string EmailSubject { get; set; }
        public string NotificationColumnGroupKeys { get; set; }
        public List<NotificationColumnViewModel> NotificationColumns { get; set; }
        public List<SelectListModel> NotificationStatuses { get; set; }
        public bool? GuardianSMS { get; set; }
        public int GuardianSMSValue { get; set; }

        [UIHint("tinymce_jquery_simple"), AllowHtml]
        public string GuardianSMSTemplate { get; set; }
        public string SMSTemplateID { get; set; }
        public string SMSTemplateName { get; set; }
        public string SMSTemplateContent { get; set; }
    }

    public class NotificationColumnViewModel
    {
        public string NotificationColumnKey { get; set; }
        public string NotificationColumnName { get; set; }
        public int NotificationColumnGroupKey { get; set; }
        public string NotificationColumnGroupName { get; set; }
    }

}
