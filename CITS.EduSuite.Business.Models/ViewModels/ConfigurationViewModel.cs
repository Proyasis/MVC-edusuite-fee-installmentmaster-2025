using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CITS.EduSuite.Business.Models.ViewModels
{
    public class ConfigurationViewModel
    {
        public string ReceiptNumber { get; set; }
        public long SerialNumber { get; set; }
        public bool IsDelete { get; set; }
        public byte ConfigType { get; set; }
        public byte? OldConfigType { get; set; }
        public short BranchKey { get; set; }
    }
}
