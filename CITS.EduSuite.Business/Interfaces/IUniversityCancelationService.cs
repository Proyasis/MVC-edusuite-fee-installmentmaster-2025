using CITS.EduSuite.Business.Models.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CITS.EduSuite.Business.Interfaces
{
    public interface IUniversityCancelationService
    {
        List<UniversityCancelationViewModel> GetUniversityCancelation(UniversityCancelationViewModel model, out long TotalRecords);
        UniversityCancelationViewModel GetUniversityCancelationById(UniversityCancelationViewModel model);
        UniversityCancelationViewModel CreateUniversityCancelation(UniversityCancelationViewModel model);
        UniversityCancelationViewModel UpdateUniversityCancelation(UniversityCancelationViewModel model);
        UniversityCancelationViewModel DeleteUniversityCancelation(UniversityCancelationViewModel model);
        //List<UniversityPaymentViewmodel> GetUniversityFeePaymentList(string fromDate, string toDate, short? branchKey, string SearchText);
    }
}
