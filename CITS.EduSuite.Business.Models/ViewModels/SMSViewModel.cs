using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CITS.EduSuite.Business.Models.ViewModels
{
    public class SMSViewModel
    {
        public int RowKey { get; set; }
        public string SMSContent { get; set; }
        public string SMSReceiptants { get; set; }
        public string UserId { get; set; }
        public string Password { get; set; }
        public string URL { get; set; }
        public string SenderID { get; set; }
        public string APICode { get; set; }
        public string Root { get; set; }
        public bool IsApi { get; set; }
        public string SmsPEID { get; set; }
        public string SMSTemplateID { get; set; }
    }
}
