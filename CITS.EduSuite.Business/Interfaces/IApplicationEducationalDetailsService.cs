using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CITS.EduSuite.Business.Models.ViewModels;

namespace CITS.EduSuite.Business.Interfaces
{
    public interface IApplicationEducationalDetailsService 
    {
        ApplicationEducationDetailsViewModel GetApplicationDocumentsById(Int64 EmployeeId);

        ApplicationEducationDetailsViewModel UpdateApplicationDocument(ApplicationEducationDetailsViewModel model);

        ApplicationEducationDetailsViewModel DeleteApplicationDocument(EducationDetailViewModel model);
    }
}
