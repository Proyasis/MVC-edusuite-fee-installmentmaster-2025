using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CITS.EduSuite.Business.Models.ViewModels
{
    public class ContactVerificationViewModel : BaseModel
    {
        public long RowKey { get; set; }
        public long EmployeeKey { get; set; }
        public string MobileNumber { get; set; }
        public string EmailAddress { get; set; }
        public string SMSVerificationCode { get; set; }
        public string EmailVerificationCode { get; set; }
        public bool? IsMobileVerified { get; set; }
        public bool? IsEmailVerified { get; set; }
    }
}
