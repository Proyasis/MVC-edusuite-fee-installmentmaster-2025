using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CITS.EduSuite.Business.Models.ViewModels;

namespace CITS.EduSuite.Business.Interfaces
{
    public interface IApplicationFeePaymentService
    {
        List<ApplicationFeePaymentViewModel> GetApplicationFeePaymentsByApplication(Int64 Id);

        ApplicationFeePaymentViewModel GetApplicationFeePaymentById(ApplicationFeePaymentViewModel model);
        ApplicationFeePaymentViewModel CreateApplicationFee(ApplicationFeePaymentViewModel model);
        ApplicationFeePaymentViewModel UpdateApplicationFee(ApplicationFeePaymentViewModel model);
        ApplicationFeePaymentViewModel DeleteApplicationFee(Int64 Id);
        ApplicationFeePaymentViewModel DeleteApplicationFeeOneByOne(Int64 Id);
        ApplicationFeePrintViewModel ViewFeePrint(Int64 Id);
        byte FillCashFlowTypeKey(short id);
        //bool CheckFeeTypeMode(short id);
        ApplicationFeePaymentDetailViewModel CheckFeeTypeMode(short id, int? Year, long ApplicationKey);
        List<ApplicationFeePaymentDetailViewModel> BindTotalFeeDetails(ApplicationFeePaymentViewModel model);
        List<ApplicationFeePaymentViewModel> BindInstallmentFeeDetails(ApplicationFeePaymentViewModel model);
        List<dynamic> GetApplications(ApplicationViewModel model, out long TotalRecords);
        ApplicationViewModel GetSearchDropdownList(ApplicationViewModel model);
        ApplicationFeePaymentViewModel FillPaymentModeSub(ApplicationFeePaymentViewModel model);
        ApplicationFeePaymentViewModel GenerateReceiptNo(ApplicationFeePaymentViewModel model);
        ApplicationFeePaymentViewModel FillFeeTypes(ApplicationFeePaymentViewModel model);
        List<ApplicationFeeFollowupDetailsViewModel> BindFeeScheduleDetails(ApplicationFeePaymentViewModel model);
    }
}
