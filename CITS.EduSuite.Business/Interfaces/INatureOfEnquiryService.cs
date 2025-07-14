using CITS.EduSuite.Business.Models.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CITS.EduSuite.Business.Interfaces
{
    public interface INatureOfEnquiryService
    {
        NatureOfEnquiryViewModel GetNatureOfEnquiryById(int? id);
        NatureOfEnquiryViewModel CreateNatureOfEnquiry(NatureOfEnquiryViewModel model);
        NatureOfEnquiryViewModel UpdateNatureOfEnquiry(NatureOfEnquiryViewModel model);
        NatureOfEnquiryViewModel DeleteNatureOfEnquiry(NatureOfEnquiryViewModel model);
        List<NatureOfEnquiryViewModel> GetNatureOfEnquiry(string searchText);
        NatureOfEnquiryViewModel CheckNatureOfEnquiryNameExists(NatureOfEnquiryViewModel model);
    }
}
