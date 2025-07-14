using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Microsoft.AspNet.SignalR.Hubs;
using CITS.EduSuite.Business.Models.Security;
using Microsoft.AspNet.SignalR;
using System.Threading.Tasks;
using CITS.EduSuite.Business.Models.ViewModels;

namespace CITS.EduSuite.UI
{
    public class NotificationHub : Hub
    {
        private readonly static ConnectionMapping<string> _connections =
           new ConnectionMapping<string>();

        public static void PushNotification(NotificationDataViewModel model)
        {
            IHubContext context = GlobalHost.ConnectionManager.GetHubContext<NotificationHub>();
            if (model.PushNotificationUserkeys != null)
            {
                foreach (long UserKey in model.PushNotificationUserkeys)
                {
                    context.Clients.Clients(_connections.GetConnections(UserKey.ToString()).ToList()).pushNotification(model.PushNotificationTitle, model.PushNotificationContent, model.PushNotificationType);
                }
            }
        }

        public static void PushUserLogout(List<long> UserKeys)
        {
            IHubContext context = GlobalHost.ConnectionManager.GetHubContext<NotificationHub>();
            foreach (long UserKey in UserKeys)
            {
                context.Clients.Clients(_connections.GetConnections(UserKey.ToString()).ToList()).pushUserLogout();
            }

        }

        //public void OnConnected()
        //{
        //    CITSEduSuitePrincipal userData = (CITSEduSuitePrincipal)Context.User;
        //    if (userData != null)
        //    {
        //        _connections.Add(userData.UserKey.ToString(), Context.ConnectionId);
        //    }

        //}

        public override Task OnConnected()
        {
            CITSEduSuitePrincipal userData = (CITSEduSuitePrincipal)Context.User;
            if (userData != null)
            {
                _connections.Add(userData.UserKey.ToString(), Context.ConnectionId);
            }
            return base.OnConnected();

        }

        public override Task OnReconnected()
        {
            CITSEduSuitePrincipal userData = (CITSEduSuitePrincipal)Context.User;

            if (userData != null && !_connections.GetConnections(userData.UserKey.ToString()).Contains(Context.ConnectionId))
            {
                _connections.Add(userData.UserKey.ToString(), Context.ConnectionId);
            }

            return base.OnReconnected();
        }
        public override Task OnDisconnected(bool stopCalled)
        {
            CITSEduSuitePrincipal userData = (CITSEduSuitePrincipal)Context.User;
            if (userData != null)
            {
                _connections.Remove(userData.UserKey.ToString(), Context.ConnectionId);
            }
            return base.OnDisconnected(stopCalled);
        }
    }
}