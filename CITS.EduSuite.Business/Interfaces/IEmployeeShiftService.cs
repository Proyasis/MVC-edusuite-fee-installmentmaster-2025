using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CITS.EduSuite.Business.Models.ViewModels;

namespace CITS.EduSuite.Business.Interfaces
{
    public interface IEmployeeShiftService
    {
        List<EmployeeShiftViewModel> GetEmployeeShifts(string SearchText);

        EmployeeShiftViewModel GetEmployeeShiftById(Int32 id);

        EmployeeShiftViewModel CreateEmployeeShift(EmployeeShiftViewModel model);


        EmployeeShiftViewModel UpdateEmployeeShift(EmployeeShiftViewModel model);


        EmployeeShiftViewModel DeleteEmployeeShift(EmployeeShiftViewModel model);
        EmployeeShiftViewModel DeleteEmployeeShiftDetails(long Id);
    }
}
