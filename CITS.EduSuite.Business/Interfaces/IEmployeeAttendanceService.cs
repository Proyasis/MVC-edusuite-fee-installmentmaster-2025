using CITS.EduSuite.Business.Models.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CITS.EduSuite.Business.Interfaces
{
    public interface IEmployeeAttendanceService
    {

        List<EmployeeAttendanceViewModel> GetEmployeeAttendance(EmployeeAttendanceViewModel model);
        List<EmployeeAttendanceViewModel> GetEmployeesForQuickAttendance(EmployeeAttendanceViewModel model);
        EmployeeAttendanceViewModel GetEmployeeAttendanceById(EmployeeAttendanceViewModel model);     

        //EmployeeAttendanceViewModel CreateEmployeeAttendance(EmployeeAttendanceViewModel model);

        //EmployeeAttendanceViewModel UpdateEmployeeAttendance(EmployeeAttendanceViewModel model);

        //EmployeeAttendanceViewModel UpdateEmployeesAttendance(List<EmployeeAttendanceViewModel> modelList);
        EmployeeAttendanceViewModel DeleteEmployeeAttendance(EmployeeAttendanceViewModel model);
        EmployeeAttendanceViewModel GetEmployeesByBranchId(EmployeeAttendanceViewModel model);
        EmployeeAttendanceViewModel GetBranches(EmployeeAttendanceViewModel model);
        List<GroupSelectListModel> FillAttendanceStatuses();
        List<EmployeeAttendanceViewModel> GetMultipleEmployeeAttendance(EmployeeAttendanceViewModel model, bool IsQuick);
        EmployeeAttendanceViewModel UpdateEmployeesAttendance(List<EmployeeAttendanceViewModel> modelList, bool IsMultiple);
        List<EmployeeAttendanceViewModel> GetEmployeesAttendanceLog(long RowKey);
        void FillMultipleDropdownList(List<EmployeeAttendanceViewModel> modelList);
        void FillDropdownList(EmployeeAttendanceViewModel model);
        byte GetAttendanceConfigTypeForQuickAttendance(EmployeeAttendanceViewModel model);
        EmployeeAttendanceViewModel UpdateAttendanceModelFromDevice(List<EmployeeAttendanceViewModel> modelList);
        EmployeeAttendanceViewModel DeleteBulkEmployeeAttendance(EmployeeAttendanceViewModel model, List<long> RowKeys);
    }
}
