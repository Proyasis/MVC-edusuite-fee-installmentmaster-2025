using CITS.EduSuite.Business.Models.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CITS.EduSuite.Business.Interfaces
{
    public interface INotificationTemplateService
    {
        List<NotificationTemplateViewModel> GetNotificationTemplates(string searchText);
        NotificationTemplateViewModel GetNotificationTemplateById(int id);
        NotificationTemplateViewModel UpdateEmailNotification(NotificationTemplateViewModel model);
        NotificationTemplateViewModel UpdateSMSNotification(NotificationTemplateViewModel model);
        NotificationDataViewModel GetNotificationData(NotificationDataViewModel model);
        NotificationDataViewModel GetPushNotificationData(NotificationDataViewModel model);
        NotificationDataViewModel CreateNotification(List<NotificationDataViewModel> modelList);
        void FillNotificationStatuses(NotificationTemplateViewModel model);

        IQueryable<NotificationDataViewModel> GetLatestNotification(NotificationDataViewModel model);

        NotificationDataViewModel UpdateViewNotification(NotificationDataViewModel model);
        NotificationDataViewModel UpdateReadNotification(NotificationDataViewModel model);
    }
}
