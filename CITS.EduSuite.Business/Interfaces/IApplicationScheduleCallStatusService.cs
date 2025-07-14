using CITS.EduSuite.Business.Models.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CITS.EduSuite.Business.Interfaces
{
    public interface IApplicationScheduleCallStatusService
    {
        List<ApplicationScheduleCallStatusViewModel> GetApplicationScheduleCallStatus(string serachText);
        ApplicationScheduleCallStatusViewModel GetApplicationScheduleCallStatusById(short? id);
        ApplicationScheduleCallStatusViewModel CreateApplicationScheduleCallStatus(ApplicationScheduleCallStatusViewModel model);
        ApplicationScheduleCallStatusViewModel UpdateApplicationScheduleCallStatus(ApplicationScheduleCallStatusViewModel model);
        ApplicationScheduleCallStatusViewModel DeleteApplicationScheduleCallStatus(ApplicationScheduleCallStatusViewModel model);
    }
}
