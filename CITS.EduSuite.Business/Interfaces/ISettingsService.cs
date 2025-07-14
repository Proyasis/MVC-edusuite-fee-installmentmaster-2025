using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CITS.EduSuite.Business.Interfaces
{
    public interface ISettingsService
    {
        void SyncApplicationSettings();
        void SyncEmployeeSettings();
        void SyncLibrarySettings();
    }
}
