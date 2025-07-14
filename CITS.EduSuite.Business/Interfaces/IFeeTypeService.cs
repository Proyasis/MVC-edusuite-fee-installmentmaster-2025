using CITS.EduSuite.Business.Models.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CITS.EduSuite.Business.Interfaces
{
    public interface IFeeTypeService
    {
        FeeTypeViewModel GetFeeTypeById(int? Id);

        FeeTypeViewModel CreateFeeType(FeeTypeViewModel model);
        FeeTypeViewModel UpdateFeeType(FeeTypeViewModel model);
        FeeTypeViewModel DeleteFeeType(FeeTypeViewModel model);
        List<FeeTypeViewModel> GetFeeType(string SearchText);
        FeeTypeViewModel CheckFeeTypeExist(FeeTypeViewModel model);
        FeeTypeViewModel FillAccountHeadType(FeeTypeViewModel model);

    }
}
