using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CITS.EduSuite.Business.Models.ViewModels;

namespace CITS.EduSuite.Business.Interfaces
{
    public interface IUniversityPaymentService
    {
        List<UniversityPaymentViewmodel> GetUniversityFeePaymentsByApplication(Int64 Id);
        UniversityPaymentViewmodel GetUniversityFeePaymentById(UniversityPaymentViewmodel model);
        UniversityPaymentViewmodel CreateUniversityFee(UniversityPaymentViewmodel model);
        UniversityPaymentViewmodel UpdateUniversityFee(UniversityPaymentViewmodel model);
        UniversityPaymentViewmodel DeleteUniversityFee(Int64 Id);
        UniversityPaymentViewmodel DeleteUniversityFeeOneByOne(Int64 Id);
        UniversityFeePrintViewModel ViewFeePrint(Int64 Id);
        byte FillCashFlowTypeKey(short id);
        List<ApplicationViewModel> GetApplications(ApplicationViewModel model, out long TotalRecords);
        UniversityPaymentDetailsmodel CheckFeeTypeMode(short id, int? Year, long ApplicationKey);

        // Bulk University Payment

        List<UniversityPaymentBulklist> GetUniversityPaymentStudentsList(UniversityPaymentBulkViewmodel model);
        UniversityPaymentBulkViewmodel CreateUniversityPaymentBulk(UniversityPaymentBulkViewmodel model);
        UniversityPaymentBulkViewmodel GetCourseTypeBySyllabus(UniversityPaymentBulkViewmodel model);
        UniversityPaymentBulkViewmodel GetCourseByCourseType(UniversityPaymentBulkViewmodel model);
        UniversityPaymentBulkViewmodel GetUniversityByCourse(UniversityPaymentBulkViewmodel model);
        UniversityPaymentBulkViewmodel GetBatches(UniversityPaymentBulkViewmodel model);
        UniversityPaymentBulkViewmodel GetYears(UniversityPaymentBulkViewmodel model);

        void FillDrodownLists(UniversityPaymentBulkViewmodel model);
        UniversityPaymentBulkViewmodel CheckFeeTypeModeBulk(short id);
        List<UniversityPaymentDetailsmodel> BindTotalFeeDetails(UniversityPaymentViewmodel model);

        decimal CheckShortBalance(short PaymentModeKey, long Rowkey, long BankAccountKey, short branchKey);
    }
}
