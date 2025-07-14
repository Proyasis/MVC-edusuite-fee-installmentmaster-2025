using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CITS.EduSuite.Business.Models.ViewModels;

namespace CITS.EduSuite.Business.Interfaces
{
   public interface IDepartmentShiftService
    {
       List<DepartmentShiftViewModel> GetDepartmentShifts(string SearchText);

        DepartmentShiftViewModel GetDepartmentShiftById(Int32 id);

        DepartmentShiftViewModel CreateDepartmentShift(DepartmentShiftViewModel model);


        DepartmentShiftViewModel UpdateDepartmentShift(DepartmentShiftViewModel model);


        DepartmentShiftViewModel DeleteDepartmentShift(DepartmentShiftViewModel model);
    }
}
