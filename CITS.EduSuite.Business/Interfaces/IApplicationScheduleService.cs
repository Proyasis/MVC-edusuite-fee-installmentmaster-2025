using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CITS.EduSuite.Business.Models.ViewModels;

namespace CITS.EduSuite.Business.Interfaces
{
    public interface IApplicationScheduleService
    {
        ApplicationScheduleViewModel GetApplicationSchedule(ApplicationScheduleViewModel model);
        ApplicationScheduleDetailsViewModel GetApplicationScheduleById(ApplicationScheduleDetailsViewModel model);
        ApplicationScheduleDetailsViewModel CreateApplicationSchedule(ApplicationScheduleDetailsViewModel model);
        ApplicationScheduleDetailsViewModel UpdateApplicationSchedule(ApplicationScheduleDetailsViewModel model);
        ApplicationScheduleDetailsViewModel DeleteApplicationSchedule(ApplicationScheduleDetailsViewModel model);
        ApplicationScheduleDetailsViewModel CheckDuration(ApplicationScheduleDetailsViewModel model);
        void FillSearchApplicationScheduleTypes(ApplicationScheduleViewModel model);
        void FillSearchApplicationCallStatus(ApplicationScheduleViewModel model);
        void FillSearchCallTypes(ApplicationScheduleViewModel model);
        void FillSearchProcessStatus(ApplicationScheduleViewModel model);
    }
}
