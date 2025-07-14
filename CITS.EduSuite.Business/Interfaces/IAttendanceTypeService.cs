using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CITS.EduSuite.Business.Models.ViewModels;


namespace CITS.EduSuite.Business.Interfaces
{
   public interface IAttendanceTypeService
    {
       AttendanceTypeViewModel GetAttendaneTypeById(short? id);
       AttendanceTypeViewModel CreateAttendanceType(AttendanceTypeViewModel model);
       AttendanceTypeViewModel UpdateAttendanceType(AttendanceTypeViewModel model);

       AttendanceTypeViewModel DeleteAttendanceTypeAll(short? id);
       List<AttendanceTypeViewModel> GetAttendanceType(string searchText);



    }
}
