using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CITS.EduSuite.Business.Models.ViewModels;

namespace CITS.EduSuite.Business.Interfaces
{
    public interface IEmployeeSubjectModuleService
    {
        EmployeeSubjectModuleViewModel FillEmployeeSubjectDetails(EmployeeSubjectModuleViewModel model);
        EmployeeSubjectModuleViewModel FillSubjectModules(EmployeeSubjectDetailsModel model);

        EmployeeSubjectModuleViewModel UpdateEmployeeSubjectModule(EmployeeSubjectModuleViewModel model);
    }
}
