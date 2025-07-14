using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CITS.EduSuite.Business.Models.ViewModels;

namespace CITS.EduSuite.Business.Interfaces
{
    public interface IDivisionService
    {
        DivisionViewModel GetDivisionById(int? id);
        DivisionViewModel CreateDivision(DivisionViewModel model);
        DivisionViewModel UpdateDivision(DivisionViewModel model);
        DivisionViewModel DeleteDivision(DivisionViewModel model);
        List<DivisionViewModel> GetDivision(string searchText);
    }
}
