using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CITS.EduSuite.Business.Interfaces;
using CITS.EduSuite.Business.Models.ViewModels;
using CITS.EduSuite.Data;
using System.Reflection;
namespace CITS.EduSuite.Business.Services
{
    public class SettingsService : ISettingsService
    {
        private EduSuiteDatabase dbContext;
        public SettingsService()
        {
            this.dbContext = new EduSuiteDatabase();
        }
        public void SyncApplicationSettings()
        {

            List<ApplicationConfig> settings = dbContext.ApplicationConfigs.ToList();
            foreach (ApplicationConfig setting in settings)
            {
                PropertyInfo propertyInfo = typeof(ApplicationSettingModel).GetProperty(setting.Name);
                if (propertyInfo != null)
                    propertyInfo.SetValue(typeof(ApplicationSettingModel), Convert.ChangeType(setting.Value, propertyInfo.PropertyType));
            }
        }
        public void SyncEmployeeSettings()
        {
            List<EmployeeConfig> settings = dbContext.EmployeeConfigs.ToList();
            foreach (EmployeeConfig setting in settings)
            {
                PropertyInfo propertyInfo = typeof(EmployeeSettingModel).GetProperty(setting.Name);
                if (propertyInfo != null)
                    propertyInfo.SetValue(typeof(EmployeeSettingModel), Convert.ChangeType(setting.Value, propertyInfo.PropertyType));
            }
        }

        public void SyncLibrarySettings()
        {
            List<LibraryConfig> settings = dbContext.LibraryConfigs.ToList();
            foreach (LibraryConfig setting in settings)
            {
                PropertyInfo propertyInfo = typeof(ApplicationSettingModel).GetProperty(setting.Name);
                if (propertyInfo != null)
                    propertyInfo.SetValue(typeof(ApplicationSettingModel), Convert.ChangeType(setting.Value, propertyInfo.PropertyType));
            }
        }
    }
}
