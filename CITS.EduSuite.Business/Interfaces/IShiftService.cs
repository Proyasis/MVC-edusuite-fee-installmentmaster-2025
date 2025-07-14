using CITS.EduSuite.Business.Models.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CITS.EduSuite.Business.Interfaces
{
    public interface IShiftService
    {

        List<ShiftViewModel> GetShift(string searchText);
        ShiftViewModel GetShiftById(Int32? id);
        ShiftViewModel CreateShift(ShiftViewModel model);
        ShiftViewModel UpdateShift(ShiftViewModel model);
        ShiftViewModel DeleteShift(ShiftViewModel model);
        ShiftViewModel CheckshiftCodeExists(string ShiftCode, int RowKey);
        ShiftViewModel DeleteShiftBreak(ShiftBreakModel objViewModel);
    }
}
