using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CITS.EduSuite.Business.Models.ViewModels
{
    public class LeadManagementViewModel:BaseModel
    {
        public long RowKey { get; set; }
        public DateTime created_time { get; set; }
        public string email { get; set; }
        public string full_name { get; set; }
        public string phone_number { get; set; }
        public string city { get; set; }
        public string street_address { get; set; }
        public string state { get; set; }
        public string country { get; set; }
        public string zip_code { get; set; }
        public string last_name { get; set; }
        public string first_name { get; set; }
        public string date_of_birth { get; set; }
        public string gender { get; set; }
        public string marital_status { get; set; }
        public string relationship_status { get; set; }
        public string military_status { get; set; }
        public string job_title { get; set; }
        public string work_phone_number { get; set; }
        public string work_email { get; set; }
        public string company_name { get; set; }
        public short BranchKey { get; set; }
    }

 

}
