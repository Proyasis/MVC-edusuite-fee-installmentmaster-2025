using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CITS.EduSuite.Business.Models.ViewModels;


namespace CITS.EduSuite.Business.Interfaces
{
   public interface IMediumService
    {
        MediumViewModel GetMediumById(int? id);
        MediumViewModel CreateMedium(MediumViewModel model);
        MediumViewModel UpdateMedium(MediumViewModel model);
        MediumViewModel DeleteMedium(MediumViewModel model);
        List<MediumViewModel> GetMedium(string searchText);
    }
}
