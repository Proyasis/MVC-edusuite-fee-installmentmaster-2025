using CITS.EduSuite.Business.Models.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CITS.EduSuite.Business.Interfaces
{
    public interface IEmployeeEnquiryTargetService
    {
        List<EmployeeEnquiryTargetViewModel> GetEmployeeList(EmployeeEnquiryTargetViewModel model, out long TotalRecords);
        EmployeeEnquiryTargetViewModel GetEmployeeEnquiryTargetId(EmployeeEnquiryTargetViewModel objmodel);
        EmployeeEnquiryTargetViewModel CreateEnquiryTarget(EmployeeEnquiryTargetViewModel model);
        EmployeeEnquiryTargetViewModel UpdateEnquiryTarget(EmployeeEnquiryTargetViewModel model);
        EmployeeEnquiryTargetDetailsViewModel DeleteEnquiryTargetDetails(EmployeeEnquiryTargetDetailsViewModel model);
    }
}
