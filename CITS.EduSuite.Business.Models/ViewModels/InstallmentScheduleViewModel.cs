using System.Collections.Generic;

namespace CITS.EduSuite.Business.Models.ViewModels
{
    public class InstallmentScheduleViewModel
    {
        public InstallmentScheduleViewModel()
        {
            ScheduleList = new List<InstallmentScheduleViewModel>();
        }

        public List<InstallmentScheduleViewModel> ScheduleList { get; set; }

        public long? ScheduleId { get; set; }
        public string InstallmentMonth { get; set; } = "";
        public int? PaymentDay { get; set; }
        public int? DueDuration { get; set; }
        public decimal? Amount { get; set; }
        public decimal? DueFineAmount { get; set; }
        public decimal? SuperFineAmount { get; set; }
        public bool AutoSMS { get; set; }
        public bool AutoEmail { get; set; }
        public int? BeforeDue { get; set; }
        public int? AfterDue { get; set; }
        public string YearLabel { get; set; }
        public long InstallmentId { get; set; }
    }
}
