using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CITS.EduSuite.Business.Models.ViewModels;
using System.Threading.Tasks;

namespace CITS.EduSuite.Business.Interfaces
{
    public interface IDivisionAllocationService
    {
        DivisionAllocationViewModel GetDivisionAllocationById(DivisionAllocationViewModel model);

        DivisionAllocationViewModel UpdateDivisionAllocation(DivisionAllocationViewModel model);

        DivisionAllocationViewModel DeleteDivisionAllocation(DivisionAllocationViewModel model);

        List<DivisionAllocationViewModel> GetDivisionAllocation(DivisionAllocationViewModel model);

        //DivisionAllocationViewModel FillCourseType(DivisionAllocationViewModel model);

        // DivisionAllocationViewModel FillCourse(DivisionAllocationViewModel model);

        // DivisionAllocationViewModel FillUniversity(DivisionAllocationViewModel model);

        DivisionAllocationViewModel FillBatch(DivisionAllocationViewModel model);

        // DivisionAllocationViewModel FillCourseYear(DivisionAllocationViewModel model);

        DivisionAllocationViewModel GenerateRollnumber(DivisionAllocationViewModel model);

        DivisionAllocationViewModel ResetDivisionAllocation(long Id);

        DivisionAllocationViewModel FillClassDetails(DivisionAllocationViewModel model);
        DivisionAllocationViewModel GetSearchDropdownList(DivisionAllocationViewModel model);
        DivisionAllocationViewModel FillSearchBatch(DivisionAllocationViewModel model);

    }
}
