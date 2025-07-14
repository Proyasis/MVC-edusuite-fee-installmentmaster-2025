using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CITS.EduSuite.Business.Models.ViewModels;


namespace CITS.EduSuite.Business.Interfaces
{
    public interface IEmployeeEducationService
    {

        EmployeeEducationViewModel GetEmployeeEducationById(Int64 EmployeeId);

        EmployeeEducationViewModel UpdateEmployeeEducation(EmployeeEducationViewModel model);

        EmployeeEducationViewModel DeleteEmployeeEducation(EducationViewModel model);

        EmployeeEducationViewModel CheckEducationTypeExists(Int16 EducationTypeKey, Int64 EmployeeKey, Int64 RowKey);

    }
}
