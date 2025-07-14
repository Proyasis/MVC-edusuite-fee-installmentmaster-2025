using CITS.EduSuite.Business.Models.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CITS.EduSuite.Business.Interfaces
{
   public interface IFeeRefundService
    {
        List<FeeRefundViewModel> GetStudentFeeRefund(FeeRefundViewModel model, string fromDate, string toDate, out long TotalRecords);
        FeeRefundViewModel GetStudentFeeRefundById(FeeRefundViewModel model);
        FeeRefundViewModel FillPaymentModeSub(FeeRefundViewModel model);
        FeeRefundViewModel fillAdvances(FeeRefundViewModel model);
        decimal GetBalanceforRefund(short PaymentModeKey, long Rowkey, long BankAccountKey, short branchKey);
        FeeRefundViewModel DeleteFeeRefund(FeeRefundViewModel model);
        FeeRefundViewModel CreateFeeRefund(FeeRefundViewModel model);
        FeeRefundViewModel UpdateFeeRefund(FeeRefundViewModel model);
        FeeRefundViewModel ApproveFeeRefund(FeeRefundViewModel model);
        FeeRefundPrintViewModel ViewFeePrint(long Id);
    }
}
