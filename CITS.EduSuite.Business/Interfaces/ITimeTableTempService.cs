using CITS.EduSuite.Business.Models.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CITS.EduSuite.Business.Interfaces
{
    public interface ITimeTableTempService
    {
        List<TimeTableTempMasterViewModel> GetTimeTableTempMaster(string searchText);
        TimeTableTempMasterViewModel GetTimeTableMasterById(TimeTableTempMasterViewModel model);
        void FillTimeTableDetails(TimeTableTempMasterViewModel model);

        TimeTableTempMasterViewModel CreateTimeTableTemp(TimeTableTempMasterViewModel model);
        TimeTableTempMasterViewModel UpdateTimeTableTemp(TimeTableTempMasterViewModel model);
        TimeTableTempMasterViewModel DeleteTimeTableTempMaster(long? Id);
        void FillTimeTableEmployee(TimeTableTempMasterViewModel model);
        TimeTableTempMasterViewModel ViewTimeTableEmployee(TimeTableTempMasterViewModel model);
    }
}
