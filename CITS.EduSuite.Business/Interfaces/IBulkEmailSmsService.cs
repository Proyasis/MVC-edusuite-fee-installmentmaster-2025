using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CITS.EduSuite.Business.Models.ViewModels;

namespace CITS.EduSuite.Business.Interfaces
{
    public interface IBulkEmailSmsService
    {
        BulkEmailSmsViewModel GetBulkEmailSms(BulkEmailSmsViewModel model);
        SendBulkEmailViewModel CreateBulkEmailTrack(SendBulkEmailViewModel model);
        SendBulkSMSViewModel CreateBulkSMSTrack(SendBulkSMSViewModel model);
        BulkEmailSmsViewModel GetSearchDropdownList(BulkEmailSmsViewModel model);
        void FillCourseTypes(BulkEmailSmsViewModel model);
        void FillCourse(BulkEmailSmsViewModel model);
        void FillUniversityMasters(BulkEmailSmsViewModel model);
        void FillYears(BulkEmailSmsViewModel model);
    }
}
