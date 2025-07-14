using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CITS.EduSuite.Business.Models.Resources;
using CITS.Validations;

namespace CITS.EduSuite.Business.Models.ViewModels
{
    public class ApplicationFeeInstallmentViewModel : BaseModel
    {
        public ApplicationFeeInstallmentViewModel()
        {
            FeeYears = new List<SelectListModel>();
            ApplicationFeeInstallments = new List<FeeInstallmentModel>();
            InstallMentMonth = new List<SelectListModel>();
        }
        public List<SelectListModel> FeeYears { get; set; }
        public long ApplicationKey { get; set; }
        public string AdmissionNo { get; set; }
        public int? FeeYear { get; set; }
        public int? StartYear { get; set; }

        public List<FeeInstallmentModel> ApplicationFeeInstallments { get; set; }
        public decimal? FeeAmount { get; set; }
        public decimal? BalancePayment { get; set; }

        public decimal? TotalFeeAmount { get; set; }

        [EqualTo("TotalFeeAmount", PassOnNull = true, ErrorMessageResourceType = typeof(ModelResources), ErrorMessageResourceName = "InitialPaymentLessThanErrorMessage")]
        public decimal? TotalInstallmentFee { get; set; }

        public List<SelectListModel> InstallMentMonth { get; set; }
    }

    public class FeeInstallmentModel
    {
        public FeeInstallmentModel()
        {
            FeePaymentDay = 1;
            InstallmentMonth = 1;

        }
        public long RowKey { get; set; }



        public int InstallmentMonth { get; set; }

        [Required(ErrorMessageResourceType = typeof(ModelResources), ErrorMessageResourceName = "FeePaymentDayRequired")]
        public int FeePaymentDay { get; set; }
        public int InstallmentYear { get; set; }


        [Required(ErrorMessageResourceType = typeof(ModelResources), ErrorMessageResourceName = "InstallmentMonthRequired")]
        public string InstallmentYearMonth { get { return (InstallmentYear != 0 ? new DateTime(InstallmentYear, InstallmentMonth, 1).ToString("yyyy-MM") : null); } }


        [RegularExpression(@"^[0-9 +]+$", ErrorMessageResourceType = typeof(ModelResources), ErrorMessageResourceName = "DueDurationExpressionErrorMessage")]
        [Required(ErrorMessageResourceType = typeof(ModelResources), ErrorMessageResourceName = "DueDurationRequired")]
        public int? DueDuration { get; set; }

        [RegularExpression(@"^[0-9 +]+$", ErrorMessageResourceType = typeof(ModelResources), ErrorMessageResourceName = "InstallmentAmountExpressionErrorMessage")]
        [Required(ErrorMessageResourceType = typeof(ModelResources), ErrorMessageResourceName = "InstallmentAmountRequired")]
        public decimal? InstallmentAmount { get; set; }

        [RegularExpression(@"^[0-9 +]+$", ErrorMessageResourceType = typeof(ModelResources), ErrorMessageResourceName = "DueFineAmountExpressionErrorMessage")]
        public decimal? DueFineAmount { get; set; }

        [RegularExpression(@"^[0-9 +]+$", ErrorMessageResourceType = typeof(ModelResources), ErrorMessageResourceName = "SuperFineAmountExpressionErrorMessage")]
        public decimal? SuperFineAmount { get; set; }
        public bool AutoSMS { get; set; }
        public bool AutoEmail { get; set; }
        public int? AutoNotificationBeforeDue { get; set; }
        public int? AutoNotificationAfterDue { get; set; }

        // [Required(ErrorMessageResourceType = typeof(ModelResources), ErrorMessageResourceName = "InitialPaymentAmountRequired")]
        [RequiredIfTrue("IsInitialPayment", ErrorMessageResourceType = typeof(ModelResources), ErrorMessageResourceName = "InitialPaymentAmountRequired")]
        public decimal? InitialPayment { get; set; }
        public decimal? BalancePayment { get; set; }

        public bool IsInitialPayment { get; set; }



    }
}
