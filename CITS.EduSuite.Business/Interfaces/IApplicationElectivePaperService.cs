using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CITS.EduSuite.Business.Models.ViewModels;

namespace CITS.EduSuite.Business.Interfaces
{
   public partial interface IApplicationElectivePaperService
    {
       ApplicationElectivePaperViewModel GetApplicationElectivePaperById(Int64 ApplicationId);

       ApplicationElectivePaperViewModel UpdateApplicationElectivePaper(ApplicationElectivePaperViewModel model);

       ApplicationElectivePaperViewModel DeleteApplicationElectivePaper(ElectivePaperViewModel model);
    }
}
