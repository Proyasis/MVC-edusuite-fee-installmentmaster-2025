using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CITS.EduSuite.Business.Models.ViewModels
{
    public class FieldValidationViewModel : BaseModel
    {
        public FieldValidationViewModel()
        {
            FieldItems = new List<FieldItemsModel>();
        }
        public short FieldType { get; set; }
        public string FieldTypeName { get; set; }

        public List<FieldItemsModel> FieldItems { get; set; }
    }

    public class FieldItemsModel
    {
        public long RowKey { get; set; }
        public string Name { get; set; }
        public string Value { get; set; }
        public bool IsActive { get; set; }

    }
}
