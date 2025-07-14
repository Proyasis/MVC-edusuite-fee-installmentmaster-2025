using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CITS.EduSuite.Business.Models.Resources;

namespace CITS.EduSuite.Business.Models.ViewModels
{
    public class HolidayViewModel : BaseModel
    {
        private DateTime? HolidayFromDate;
        private DateTime? HolidayToDate;
        public HolidayViewModel()
        {

            Branches = new List<SelectListModel>();
            HolidayTypes = new List<SelectListModel>();
            ClassDetails = new List<SelectListModel>();
        }
        public long RowKey { get; set; }

        //[Required(ErrorMessageResourceType = typeof(ModelResources), ErrorMessageResourceName = "BranchRequired")]
        public short? BranchKey { get; set; }
        public string BranchName { get; set; }

        public byte HolidayTypeKey { get; set; }
        public byte HolidayMonthKey { get; set; }

        public short HolidayYearKey { get; set; }

        public DateTime? HolidayMonth
        {
            get
            {
                try { return new DateTime(HolidayYearKey, HolidayMonthKey, 01); } catch { return null; };
            }
            set
            {

            }
        }


        [Required(ErrorMessageResourceType = typeof(ModelResources), ErrorMessageResourceName = "HolidayTitleRequired")]
        [RegularExpression(@"^[0-9 a-zA-Z ,()&-/\s]+$", ErrorMessageResourceType = typeof(ModelResources), ErrorMessageResourceName = "HolidayTitleRegularExpressionErrorMessage")]
        [StringLength(100, ErrorMessageResourceType = typeof(ModelResources), ErrorMessageResourceName = "HolidayTitleLengthErrorMessage")]

        public string HolidayTitle { get; set; }
        public string HolidayTitleLocal { get; set; }

        [Required(ErrorMessageResourceType = typeof(ModelResources), ErrorMessageResourceName = "HolidayFromDateRequired")]
        public DateTime? HolidayFrom
        {
            get { return HolidayFromDate; }

            set
            {
                HolidayFromDate = value != null && HolidayYearKey != 0 && HolidayTypeKey == DbConstants.HolidayType.Fixed ? new DateTime(HolidayYearKey, value.Value.Month, value.Value.Day) : value;
            }
        }

        //[Required( ErrorMessageResourceType = typeof(ModelResources), ErrorMessageResourceName = "HolidayToDateRequired")]
        public DateTime? HolidayTo
        {
            get { return HolidayToDate; }

            set
            {
                HolidayToDate = value != null && HolidayYearKey != 0 && HolidayTypeKey == DbConstants.HolidayType.Fixed ? new DateTime(HolidayYearKey, value.Value.Month, value.Value.Day) : value;
            }
        }

        [StringLength(100, ErrorMessageResourceType = typeof(ModelResources), ErrorMessageResourceName = "RemarksLengthErrorMessage")]
        public string Remarks { get; set; }

        public bool IsDayOff { get; set; }

        public List<long> ClassKeys { get; set; }

        
        
        public List<SelectListModel> ClassDetails { get; set; }

        public List<SelectListModel> Branches { get; set; }
        public List<SelectListModel> HolidayTypes { get; set; }
    }
}
