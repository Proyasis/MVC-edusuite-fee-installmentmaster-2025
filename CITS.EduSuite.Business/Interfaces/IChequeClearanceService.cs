using CITS.EduSuite.Business.Models.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CITS.EduSuite.Business.Interfaces
{
    public interface IChequeClearanceService
    {
        ChequeClearanceViewModel GetChequeClearanceById(ChequeClearanceViewModel model);
        ChequeClearanceViewModel CreateChequeClearance(ChequeClearanceViewModel model);
        List<ChequeClearanceViewModel> GetChequeClearance(string searchText, short branchKey);
        decimal CheckShortBalance(short PaymentModeKey, long Rowkey, long BankAccountKey, short branchKey);
        ChequeClearanceViewModel FillBranch(ChequeClearanceViewModel model);
    }
}
