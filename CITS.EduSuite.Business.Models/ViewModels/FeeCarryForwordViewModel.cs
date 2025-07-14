using CITS.EduSuite.Business.Models.Resources;
using CITS.Validations;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CITS.EduSuite.Business.Models.ViewModels
{
    public class FeeCarryForwordViewModel : BaseModel
    {
        public FeeCarryForwordViewModel()
        {
            FeeCarryForwordDetailsModel = new List<FeeCarryForwordDetailViewModel>();
            CarryforwordDate = DateTimeUTC.Now;
        }
        public long RowKey { get; set; }
        public string Remarks { get; set; }
        public long ApplicationKey { get; set; }
        public string ApplicantName { get; set; }
        public string AdmissionNo { get; set; }
        public string StudentMobile { get; set; }
        public string StudentEmail { get; set; }

        [Required(ErrorMessageResourceType = typeof(ModelResources), ErrorMessageResourceName = "FeeDateRequired")]
        public DateTime? CarryforwordDate { get; set; }

        [Required(ErrorMessageResourceType = typeof(ModelResources), ErrorMessageResourceName = "RequiredErrorMessage")]
        [Display(Name = "AmountPaid", ResourceType = typeof(EduSuiteUIResources))]
        public decimal? CarryForwordAmount { get; set; }

        public long? FeePaymentMasterKey { get; set; }

        public List<FeeCarryForwordDetailViewModel> FeeCarryForwordDetailsModel { get; set; }

    }

    public class FeeCarryForwordDetailViewModel
    {
        public long RowKey { get; set; }
        public long FeeCarryForwordMasterKey { get; set; }
        public long? AdmissionFeeKey { get; set; }

        public short? FeeTypeKey { get; set; }
        public string FeeTypeName { get; set; }

        [RegularExpression(@"^\d{0,10}(\.\d{1,2})?$", ErrorMessageResourceType = typeof(ModelResources), ErrorMessageResourceName = "AmountExpressionErrorMessage")]
        public decimal? ReturnAmount { get; set; }

        public bool IsFeeTypeYear { get; set; }

        public int? AdmissionFeeYear { get; set; }
        public string AdmissionFeeYearText { get; set; }

        public decimal? AdmissionFeeAmount { get; set; }

        public decimal? TotalFee { get; set; }
        public decimal? TotalPaidAmount { get; set; }


        [RegularExpression(@"^\d{0,10}(\.\d{1,2})?$", ErrorMessageResourceType = typeof(ModelResources), ErrorMessageResourceName = "AmountExpressionErrorMessage")]
        public decimal? TotalAmount { get; set; }

        public decimal? BeforeTakenAdvance { get; set; }

        public bool IsDeduct { get; set; }
        public decimal? AdvanceBalance
        {
            get
            {
                return (BeforeTakenAdvance) - (ReturnAmount ?? 0);
            }
            set { }
        }
        public string RecieptNo { get; set; }
        public DateTime? FeeRecieptDate { get; set; }
    }
}