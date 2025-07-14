using CITS.EduSuite.Business.Models.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CITS.EduSuite.Business.Interfaces
{
    public interface ICounsellingTimeService
    {
        CounsellingTimeViewModel GetCounsellingTimeById(int? id);
        CounsellingTimeViewModel CreateCounsellingTime(CounsellingTimeViewModel model);
        CounsellingTimeViewModel UpdateCounsellingTime(CounsellingTimeViewModel model);
        CounsellingTimeViewModel DeleteCounsellingTime(CounsellingTimeViewModel model);
        List<CounsellingTimeViewModel> GetCounsellingTime(string searchText);
    }
}
