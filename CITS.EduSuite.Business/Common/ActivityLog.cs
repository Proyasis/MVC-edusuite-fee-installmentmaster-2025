using CITS.EduSuite.Business.Models.Security;
using CITS.EduSuite.Business.Models.ViewModels;
using System;
using System.Collections.Generic;
using System.Data.Entity.Core.EntityClient;
using System.Data.SqlClient;
using System.Linq;
using System.Threading;
using System.Web;

namespace CITS.EduSuite.Business.Services
{
    public static class ActivityLog
    {
        public static void CreateActivityLog(string menuCode, string actionCode, byte type, object RowKey, string message)
        {
            log4net.GlobalContext.Properties["appuserkey"] = Thread.CurrentPrincipal.Identity != null && Thread.CurrentPrincipal.Identity.IsAuthenticated ? (Thread.CurrentPrincipal as CITSEduSuitePrincipal).UserKey.ToString() : null;
            log4net.GlobalContext.Properties["menucode"] = menuCode;
            log4net.GlobalContext.Properties["actioncode"] = actionCode;
            if (RowKey != null)
            {
                log4net.GlobalContext.Properties["rowkey"] = RowKey.ToString();
            }
            else
            {
                log4net.GlobalContext.Properties["rowkey"] = RowKey;
            }
            ChangeConnection();
            if (type == DbConstants.LogType.Info)
            {

                LogHelper.Info(message);
            }
            else if (type == DbConstants.LogType.Error)
            {

                LogHelper.Error(message);
            }
            else if (type == DbConstants.LogType.Warn)
            {

                LogHelper.Warn(message);
            }
            else if (type == DbConstants.LogType.Debug)
            {

                LogHelper.Debug(message);
            }
            else if (type == DbConstants.LogType.Fatal)
            {
                LogHelper.Fatal(message);
            }
        }

        public static void ChangeConnection()
        {
            string ConnectionString = "EduSuiteDatabase";


            log4net.Repository.Hierarchy.Hierarchy hierarchy =
            log4net.LogManager.GetLoggerRepository()
            as log4net.Repository.Hierarchy.Hierarchy;
            var entityCnxStringBuilder = new EntityConnectionStringBuilder
                    (System.Configuration.ConfigurationManager
                        .ConnectionStrings[ConnectionString].ConnectionString);

            // init the sqlbuilder with the full EF connectionstring cargo
            var sqlCnxStringBuilder = new SqlConnectionStringBuilder
                (entityCnxStringBuilder.ProviderConnectionString);
            if (hierarchy != null)
            {
                log4net.Appender.AdoNetAppender appender
                    = (log4net.Appender.AdoNetAppender)hierarchy.GetAppenders()
                        .Where(x => x.GetType() ==
                            typeof(log4net.Appender.AdoNetAppender))
                        .FirstOrDefault();

                if (appender != null)
                {
                    appender.ConnectionString = sqlCnxStringBuilder.ConnectionString;
                    appender.ActivateOptions();
                }
            }

        }
    }
    public static class log4netManager
    {
        public static void Configure(){
         log4net.Config.XmlConfigurator.Configure();
        }
    }
}