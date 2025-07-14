using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CITS.EduSuite.Business.Models.ViewModels
{
    public class ActivityLogReportViewModel:BaseSearchActivityLogViewModel
    {
        public long RowKey { get; set; }
        public DateTime ActivityDate { get; set; }
        public string Status { get; set; }
        public string HostName { get; set; }
        public string UserID { get; set; }
        public string MenuName { get; set; }
        public string MenuAction { get; set; }
        public string ActionDone { get; set; }

        
    }
}
