using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CITS.EduSuite.Business.Models.ViewModels
{
    public abstract class BaseModel
    {
        public bool IsSuccessful { get; set; }
        public string Message { get; set; }

        public bool? AutoEmail { get; set; }
        public bool? AutoSMS { get; set; }
        public bool? GuardianSMS { get; set; }
        public int TemplateKey { get; set; }
        public BaseModel()
        {

        }
    }
}
