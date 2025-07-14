using CITS.EduSuite.Business.Models.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CITS.EduSuite.Business.Interfaces
{
   public interface IAttendanceTypeMasterService
    {
       AttendanceTypeMasterViewModel GetAttendaneTypeMasterById(short? id);
       AttendanceTypeMasterViewModel CreateAttendanceTypeMaster(AttendanceTypeMasterViewModel model);
       AttendanceTypeMasterViewModel UpdateAttendanceTypeMaster(AttendanceTypeMasterViewModel model);
       List<AttendanceTypeMasterViewModel> GetAttendanceTypeMaster(string searchText);
       AttendanceTypeMasterViewModel DeleteAttendanceTypeMaster(short? Id);
       AttendanceTypeMasterViewModel DeleteAttendanceTypeDetails(short? Id);
    }
}
