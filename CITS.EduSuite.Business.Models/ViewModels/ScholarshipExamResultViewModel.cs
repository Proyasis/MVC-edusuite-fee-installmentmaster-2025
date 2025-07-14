using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using CITS.EduSuite.Business.Models.Resources;
using CITS.Validations;

namespace CITS.EduSuite.Business.Models.ViewModels
{
    public class ScholarshipExamResultViewModel : BaseModel
    {
        public ScholarshipExamResultViewModel()
        {

            ScholarshipExamDetails = new List<ScholarshipExamDetails>();
        }
        public List<ScholarshipExamDetails> ScholarshipExamDetails { get; set; }
    }

    public class ScholarshipExamDetails
    {
       
        public long RowKey { get; set; }
        public long? ScholarshipExamScheduleKey { get; set; }
        public long? ScholarshipKey { get; set; }
        [RequiredIf("AbsentStatus", false, ErrorMessageResourceType = typeof(ModelResources), ErrorMessageResourceName = "MarkRequired")]
        public decimal? Mark { get; set; }
        public string Remarks { get; set; }
        public string ResultStatus { get; set; }
        public string ExamRegNo { get; set; }
        public string MobileNumber { get; set; }
        public string ScholarShipName { get; set; }
        public string EmailAddress { get; set; }
        public bool AbsentStatus { get; set; }

    }
}
