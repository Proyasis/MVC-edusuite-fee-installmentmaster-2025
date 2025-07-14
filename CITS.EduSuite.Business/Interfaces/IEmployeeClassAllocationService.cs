using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CITS.EduSuite.Business.Models.ViewModels;

namespace CITS.EduSuite.Business.Interfaces
{
    public interface IEmployeeClassAllocationService
    {
        EmployeeClassAllocationViewModel GetEmployeeClassAllocationById(EmployeeClassAllocationViewModel model);
        EmployeeClassAllocationViewModel UpdateEmployeeClassAllocation(EmployeeClassAllocationViewModel model);
        EmployeeClassAllocationViewModel CreateEmployeeClassAllocation(EmployeeClassAllocationViewModel model);

        EmployeeClassAllocationViewModel DeleteEmployeeClassAllocation(Int64 Rowkey);


        //EmployeeClassAllocationViewModel GetSubjectDetailsByClassDetailsId(EmployeeClassAllocationViewModel model);

        EmployeeClassAllocationViewModel GetSubjectByClassDetailskey(EmployeeClassAllocationViewModel model);

        EmployeeClassAllocationViewModel CheckInCharge(long EmployeeKey, long ClassDetailsKey, short BatchKey);

        EmployeeClassAllocationViewModel FillBatchDetails(EmployeeClassAllocationViewModel model);
        List<EmployeeClassAllocationViewModel> GetEmployeeClassDetails(long EmployeeKey);
    }
}
