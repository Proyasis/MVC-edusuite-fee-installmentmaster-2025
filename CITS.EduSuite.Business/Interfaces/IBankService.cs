using CITS.EduSuite.Business.Models.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CITS.EduSuite.Business.Interfaces
{
   public interface IBankService
    {
        BankViewModel GetBankById(int? id);
        BankViewModel CreateBank(BankViewModel model);
        BankViewModel UpdateBank(BankViewModel model);
        BankViewModel DeleteBank(BankViewModel model);
        List<BankViewModel> GetBank(string searchText);
    }
}
