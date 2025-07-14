using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CITS.EduSuite.Business.Models.ViewModels;

namespace CITS.EduSuite.Business.Interfaces
{
    public interface IBorrowerTypeService
    {
        List<BorrowerTypeViewModel> GetBorrowerTypes(string searchText);
        BorrowerTypeViewModel GetBorrowerTypeById(byte? id);
        BorrowerTypeViewModel CreateBorrowerType(BorrowerTypeViewModel model);
        BorrowerTypeViewModel UpdateBorrowerType(BorrowerTypeViewModel model);
        BorrowerTypeViewModel DeleteBorrowerType(BorrowerTypeViewModel model);
        BorrowerTypeViewModel CheckBorrowerTypeCodeExist(BorrowerTypeViewModel model);
    }
}
