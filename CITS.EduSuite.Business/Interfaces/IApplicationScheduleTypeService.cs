using CITS.EduSuite.Business.Models.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CITS.EduSuite.Business.Interfaces
{
   public interface IApplicationScheduleTypeService
    {
        List<ApplicationScheduleTypeViewModel> GetApplicationScheduleType(string serachText);
        ApplicationScheduleTypeViewModel GetApplicationScheduleTypeById(short? id);
        ApplicationScheduleTypeViewModel CreateApplicationScheduleType(ApplicationScheduleTypeViewModel model);
        ApplicationScheduleTypeViewModel UpdateApplicationScheduleType(ApplicationScheduleTypeViewModel model);
        ApplicationScheduleTypeViewModel DeleteApplicationScheduleType(ApplicationScheduleTypeViewModel model);
    }
}
