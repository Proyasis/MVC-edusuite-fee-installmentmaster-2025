using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CITS.EduSuite.Business.Models.ViewModels;

namespace CITS.EduSuite.Business.Interfaces
{
    public interface IEnquiryCallStatusService
    {
        List<EnquiryCallStatusViewModel> GetEnquiryCallStatus(string searchText);
        EnquiryCallStatusViewModel GetEnquiryCallStatusById(short? id);
        EnquiryCallStatusViewModel CreateEnquiryCallStatus(EnquiryCallStatusViewModel model);

        EnquiryCallStatusViewModel UpdateEnquiryCallStatus(EnquiryCallStatusViewModel model);

        EnquiryCallStatusViewModel DeleteEnquiryCallStatus(EnquiryCallStatusViewModel model);
    }
}
