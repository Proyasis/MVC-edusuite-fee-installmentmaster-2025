using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CITS.EduSuite.Business.Models.ViewModels;

namespace CITS.EduSuite.Business.Interfaces
{
    public interface IAttendanceCategoryService
    {
        List<AttendanceCategoryViewModel> GetAttendanceCategory(string SearchText);

        AttendanceCategoryViewModel GeAttendanceCategoryById(int? Id);
        AttendanceCategoryViewModel CreateAttendanceCategory(AttendanceCategoryViewModel model);
        AttendanceCategoryViewModel UpdateAttendanceCategory(AttendanceCategoryViewModel model);
        AttendanceCategoryViewModel DeleteAttendanceCategory(AttendanceCategoryViewModel model);
        AttendanceCategoryViewModel DeleteAttendanceCategoryWeekOff(Int32 RowKey);
        AttendanceCategoryViewModel CheckAttendanceCategoryCodeExists(string AttendanceCategoryCode, short RowKey);
    }
}
