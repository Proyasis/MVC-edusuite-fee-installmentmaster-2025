using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CITS.EduSuite.Business.Models.ViewModels;

namespace CITS.EduSuite.Business.Interfaces
{
    public interface IEmployeeExperienceService
    {
        EmployeeExperienceViewModel GetEmployeeExperienceById(Int64 EmployeeId);

        EmployeeExperienceViewModel UpdateEmployeeExperience(EmployeeExperienceViewModel model);

        EmployeeExperienceViewModel DeleteEmployeeExperience(ExperienceViewModel model);


    }
}
