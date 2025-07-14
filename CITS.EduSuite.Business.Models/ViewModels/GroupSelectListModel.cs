using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CITS.EduSuite.Business.Models.ViewModels
{
    public class GroupSelectListModel : BaseModel
    {
        public GroupSelectListModel()
        {

        }

        public long RowKey { get; set; }
        public string Text { get; set; }
        public bool Selected { get; set; }
        public int IntValue { get; set; }
        public long GroupKey { get; set; }
        public string GroupName { get; set; }
    }
}
