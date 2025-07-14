using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CITS.EduSuite.Business.Models.Resources;
using CITS.Validations;
using CITS.EduSuite.Business.Models.Security;
using System.Threading;
using System.ComponentModel.DataAnnotations;

namespace CITS.EduSuite.Business.Models.ViewModels
{
    public class ScholarshipExamScheduleViewModel : BaseModel
    {
        public ScholarshipExamScheduleViewModel()
        {
            ScholarshipTypes = new List<SelectListModel>();
            ExamCenters = new List<SelectListModel>();
            ScholarshipExamScheduleDetails = new List<ScholarshipExamScheduleDetails>();
            ScholarshipKeyList = new List<long>();
            ExamDate = DateTime.UtcNow;
            ExamSubCenters = new List<SelectListModel>();
            //ScholarshipNameList = new List<SelectListModel>();
        }

        public long? RowKey { get; set; }
        public long? ScholarshipExamScheduleKey { get; set; }
        public short? ScholarshipTypeKey { get; set; }




        //public short? ScholarshipTypeKey { get; set; }
        public string ExamRegNo { get; set; }

        [Required(ErrorMessageResourceType = typeof(ModelResources), ErrorMessageResourceName = "ExamDateRequired")]
        public DateTime? ExamDate { get; set; }


        [Required(ErrorMessageResourceType = typeof(ModelResources), ErrorMessageResourceName = "CenterRequired")]
        public short ExamCenterKey { get; set; }
        public string ExamCentername { get; set; }

        public TimeSpan? ExamStartTime { get; set; }
        public TimeSpan? ExamEndTime { get; set; }

        public List<SelectListModel> ScholarshipTypes { get; set; }
        public List<SelectListModel> ExamCenters { get; set; }



        public string sidx { get; set; }
        public string sord { get; set; }
        public int page { get; set; }
        public int rows { get; set; }

        
        

        public string SearchName { get; set; }
        public string SearchPhone { get; set; }
        public DateTime? SearchFromDate { get; set; }
        public DateTime? SearchToDate { get; set; }
        [Required(ErrorMessageResourceType = typeof(ModelResources), ErrorMessageResourceName = "CenterRequired")]
        public short? SearchBranchKey { get; set; }
        public int? SearchDistrictKey { get; set; }
        [Required(ErrorMessageResourceType = typeof(ModelResources), ErrorMessageResourceName = "ScholarshipTypeKeyRequired")]
        public short? SearchScholarshipTypeKey { get; set; }

        //[Required(ErrorMessageResourceType = typeof(ModelResources), ErrorMessageResourceName = "DistrictRequired")]
        public int? DistrictKey { get; set; }
        public string DistrictName { get; set; }


        public List<SelectListModel> Branches { get; set; }
        public List<SelectListModel> Districts { get; set; }

        //[Required(ErrorMessageResourceType = typeof(ModelResources), ErrorMessageResourceName = "BranchRequired")]
        public short? BranchKey { get; set; }
        public string BranchName { get; set; }
        public string MobileNumber { get; set; }

        public string ScholarShipName { get; set; }
        public string EmailAddress { get; set; }
        public string LocationName { get; set; }
        public string ScholarshipTypeName { get; set; }
        public string ScholarShipEducationQualification { get; set; }
        public DateTime? ScholarshipDate { get; set; }

        public short? SubBranchKey { get; set; }

        public string SubBranchName { get; set; }
        public List<SelectListModel> ExamSubCenters { get; set; }

        public List<long> ScholarshipKeyList { get; set; }
        //public List<SelectListModel> ScholarshipNameList { get; set; }

        public bool ScheduleStatus { get; set; }

        public List<ScholarshipExamScheduleDetails> ScholarshipExamScheduleDetails { get; set; }
        public decimal? Mark { get; set; }
        public string ResultStatus { get; set; }

    }

    public class ScholarshipExamScheduleDetails
    {
        public long? ScholarshipKey { get; set; }

        public string MobileNumber { get; set; }

        public string ScholarShipName { get; set; }
        public string EmailAddress { get; set; }

        public string URL { get; set; }

    }
}
