using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CITS.EduSuite.Business.Models.ViewModels;

namespace CITS.EduSuite.Business.Interfaces
{
    public partial interface IApplicationDocumentService
    {
        ApplicationDocumentViewModel GetApplicationDocumentsById(Int64 EmployeeId);

        ApplicationDocumentViewModel UpdateApplicationDocument(ApplicationDocumentViewModel model);

        ApplicationDocumentViewModel DeleteApplicationDocument(DocumentViewModel model);
    }
}
