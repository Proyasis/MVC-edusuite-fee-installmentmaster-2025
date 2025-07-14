using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CITS.EduSuite.Business.Models.ViewModels;

namespace CITS.EduSuite.Business.Interfaces
{
    public interface IAttendanceConfigurationService
    {
        List<AttendanceConfigurationViewModel> GetAttendanceConfigurations();

        AttendanceConfigurationViewModel GetAttendanceConfigurationById(AttendanceConfigurationViewModel model);

        AttendanceConfigurationViewModel CreateAttendanceConfiguration(AttendanceConfigurationViewModel model);


        AttendanceConfigurationViewModel UpdateAttendanceConfiguration(AttendanceConfigurationViewModel model);


        AttendanceConfigurationViewModel DeleteAttendanceConfiguration(AttendanceConfigurationViewModel model);
    }
}
