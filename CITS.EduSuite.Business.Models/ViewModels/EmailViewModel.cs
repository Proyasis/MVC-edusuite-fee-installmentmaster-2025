using CITS.EduSuite.Business.Models.Resources;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CITS.EduSuite.Business.Models.ViewModels
{
    public class EmailViewModel
    {
        public EmailViewModel()
        {
            EmailAttachment =new List<string>();
            EmailTolist = new List<string>();
        }

        [Required(ErrorMessageResourceType = typeof(ModelResources), ErrorMessageResourceName = "EmailAddressRequired")]
        [StringLength(100, ErrorMessageResourceType = typeof(ModelResources), ErrorMessageResourceName = "EmailAddressLengthErrorMessage")]
        [RegularExpression(@"^\w+([-+.']\w+)*@\w+([-.]\w+)*\.\w+([-.]\w+)*$", ErrorMessageResourceType = typeof(ModelResources), ErrorMessageResourceName = "EmailAddressExpressionErrorMessage")]
        public string EmailTo { get; set; }

        public string EmailCC { get; set; }

        public string EmailBody { get; set; }

        public string EmailSubject { get; set; }

        public List<string> EmailAttachment { get; set; }

        public List<string> EmailTolist { get; set; }
    }
}
