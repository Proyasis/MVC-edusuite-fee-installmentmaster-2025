using CITS.EduSuite.Business.Models.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CITS.EduSuite.Business.Interfaces
{
   public interface ISalaryOtherAmountTypeService
    {
        SalaryOtherAmountTypeViewModel GetSalaryOtherAmountTypeById(int? id);
        SalaryOtherAmountTypeViewModel CreateSalaryOtherAmountType(SalaryOtherAmountTypeViewModel model);
        SalaryOtherAmountTypeViewModel UpdateSalaryOtherAmountType(SalaryOtherAmountTypeViewModel model);
        SalaryOtherAmountTypeViewModel DeleteSalaryOtherAmountType(SalaryOtherAmountTypeViewModel model);
        List<SalaryOtherAmountTypeViewModel> GetSalaryOtherAmountType(string searchText);
    }
}
