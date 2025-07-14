using CITS.EduSuite.Business.Models.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CITS.EduSuite.Business.Interfaces
{
    public interface INotificationDataService
    {
        List<NotificationDataViewModel> GetPushNotification(NotificationDataViewModel model, out long TotalRecords);
        List<NotificationDataViewModel> GetNotification(NotificationDataViewModel model, out long TotalRecords);
    }
}
