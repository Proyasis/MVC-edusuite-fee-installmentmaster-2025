using CITS.EduSuite.Business.Models.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CITS.EduSuite.Business.Interfaces
{
    public interface IAttendanceService
    {
        AttendanceViewModel GetAttendanceById(AttendanceViewModel model);
        AttendanceViewModel CreateAttendance(AttendanceViewModel model);
        AttendanceViewModel UpdateAttendance(AttendanceViewModel model);
        AttendanceViewModel DeleteAttendance(AttendanceViewModel model);
        List<AttendanceViewModel> GetAttendance(AttendanceViewModel model, out long TotalRecords);
        AttendanceViewModel FillBatch(AttendanceViewModel model);
        AttendanceViewModel FillClassDetails(AttendanceViewModel model);
        AttendanceViewModel GetSearchDropdownList(AttendanceViewModel model);
        AttendanceViewModel FillAttendanceDetailsViewModel(AttendanceViewModel model);
        AttendanceViewModel CheckAttendanceBlocked(long RowKey, long ApplicationKey, short AttendanceTypeKey, DateTime AttendanceDate);
        AttendanceViewModel DeleteBulkAttendance(AttendanceViewModel model, List<long> RowKeys);
    }
}
