using CITS.EduSuite.Business.Models.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CITS.EduSuite.Business.Interfaces
{
    public interface IStudentPortalService
    {

        string GetPersonalDetails(long ApplicationKey);
        string GetAttendanceDetails(long Applicationkey, long? ClassDetailsKey);
        string GetAllExamResults(long ApplicationKey);
        
        string GetUnitExamResults(long ApplicationKey, long SubjectKey);
        string GetNotification(long Applicationkey, long? ClassDetailsKey);
        string BindTotalFeeDetails(long Applicationkey, long? ClassDetailsKey);
        string BindInstallmentFeeDetails(long Applicationkey, long? ClassDetailsKey);
        string BindFeePaymentDetails(long Applicationkey, long? ClassDetailsKey);
    }
}
